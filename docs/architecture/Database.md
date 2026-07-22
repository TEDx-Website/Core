# Data Model — Database Design

> **Version:** 1.0 · **Date:** 2026-07-22 · Part of [Architecture Overview](./Architecture.md)
> **Decisions:** D:Q46–Q55 + the FK revision · **Reads from:** [02 — SRS](../02-SRS.md), [08 — Decision Log](../08-DecisionLog.md)
> **Companion:** [ERD.md](./ERD.md) (diagram + relationship rules)

---

## 0. Conventions (apply to every table)

| Convention | Rule | Decision |
|-----------|------|----------|
| **Primary keys** | `Guid` (`uniqueidentifier`), sequential where it matters for index locality | project-wide |
| **Money** | `decimal(18,2)`, EGP; piastres only at the Paymob boundary | D:Q18, Q48 |
| **Timestamps** | `datetime2`, UTC, suffix `...Utc` | D:Q27 |
| **Enums** | stored as `int` (or a short string where readability wins); exposed on the wire as PascalCase strings | D:Q27 |
| **i18n text** | `...En` / `...Ar` column pairs | scope |
| **Audit** (`IAuditable`) | `CreatedAtUtc`, `CreatedBy`, `UpdatedAtUtc`, `UpdatedBy` — set by a `SaveChanges` interceptor from `ICurrentUser`+`IClock` | D:Q54 |
| **Soft-delete** (`ISoftDeletable`) | `IsDeleted` + a **global query filter**; **catalog tables only** | D:Q54 |
| **Concurrency** | `RowVersion` (`rowversion`/`timestamp`) — only where concurrent writers exist | D:Q54 |
| **Cross-context FK** | real FK to `ApplicationUser.Id`, **`DeleteBehavior.Restrict`**; **no navigation property in code** | D:Q51 revision |

**Which tables get which cross-cutting columns (D:Q54):**

| Table | Audit | Soft-delete | RowVersion |
|-------|:----:|:----------:|:---------:|
| ApplicationUser | ✅ | ✅ | (Identity `ConcurrencyStamp`) |
| RefreshToken | CreatedAtUtc only | — | — |
| Event | ✅ | ✅ | ✅ |
| Package | ✅ | ✅ | ✅ |
| Order | ✅ | — (append-only) | ✅ |
| Ticket | ✅ | — (append-only) | ✅ |
| PromoCode | ✅ | ✅ | ✅ |
| PromoRedemption | CreatedAtUtc only | — (append-only) | — |
| Track | ✅ | ✅ | ✅ |
| TrackAssignment | ✅ | — | ✅ |
| Session | ✅ | ✅ | ✅ |
| Attendance | ✅ | — (update to correct) | ✅ |
| Evaluation | ✅ | — (update to correct) | ✅ |
| OutboxMessage | CreatedAtUtc only | — | — |

---

## 1. Identity context

### 1.1 `ApplicationUser` (D:Q46) — `: IdentityUser<Guid>`

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | **This GUID is the account id every other context references by value.** |
| `Email`, `NormalizedEmail` | nvarchar | Identity; unique |
| `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp` | nvarchar | Identity crypto |
| `LockoutEnd`, `AccessFailedCount`, `EmailConfirmed` | — | Identity lockout/confirm |
| `GlobalRole` | int enum | **Attendee \| Admin**, default Attendee (D:Q36 — a column, not Identity roles) |
| `FullName` | nvarchar | |
| `IsActive` | bit | Deactivation blocks login/refresh (D:Q10) |
| audit + `IsDeleted` | | soft-deletable catalog entity |

- **Identity configured `AddIdentityCore` with no roles store and no claims/external-login tables** (D:Q46).
- Unique index on `NormalizedEmail`.

### 1.2 `RefreshToken` (D:Q47)

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `AccountId` | Guid FK → ApplicationUser (Restrict) | |
| `TokenHash` | nvarchar | **SHA-256 of the raw token — raw never stored** |
| `ExpiresAtUtc` | datetime2 | 7-day default (D:Q24) |
| `CreatedAtUtc` | datetime2 | |
| `CreatedByIp` | nvarchar? | optional |
| `RevokedAtUtc` | datetime2? | null = active |
| `ReplacedByTokenHash` | nvarchar? | the rotation chain link |
| `ReasonRevoked` | int enum? | Rotated \| Reuse \| Logout \| Expired |

- **Unique index on `TokenHash`**; index on `AccountId` (revoke-all).
- **Reuse detection:** presenting a token whose row is already revoked ⇒ walk the chain and revoke the whole family ⇒ `TOKEN_REUSED` (D:Q24, Q47).
- **Password reset** uses Identity's built-in `SecurityStamp`-backed provider — **no table here**; `RESET_TOKEN_INVALID` = failed `ResetPasswordAsync`.

---

## 2. Eventing / Ticketing context

### 2.1 `Event` (D:Q48)

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `TitleEn`, `TitleAr`, `DescriptionEn`, `DescriptionAr` | nvarchar | i18n |
| `TrackId` | Guid? | owning track (value ref) |
| `Venue` / location fields | nvarchar | |
| `StartsAtUtc`, `EndsAtUtc` | datetime2 | |
| `Capacity` | int | total seats |
| `TicketPrice` | **decimal(18,2)** | **≥ 0**; **0 ⇒ free individual ticket** → confirm-free path (Model B, D:Q1/Q48). Negative → `422 INVALID_TICKET_PRICE` |
| `MaxIndividualQtyPerOrder` | int? | **nullable = no cap** (mirrors package cap, D:Q1) |
| `Status` | int enum | Draft \| Published \| Archived \| Cancelled (D:Q23) |
| `ImageUrl` | nvarchar? | Cloudinary |
| audit + `IsDeleted` + `RowVersion` | | |

- **Remaining seats is computed, never stored** (D:Q3, `FR-EVT-07`).
- **Zero packages is valid + publishable** (Model B).

### 2.2 `Package` (D:Q48) — optional child of Event

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `EventId` | Guid FK → Event (**intra-context**, Cascade OK within aggregate) | |
| `NameEn`, `NameAr` | nvarchar | |
| `Price` | decimal(18,2) | bundle unit price |
| `SeatsPerPackage` | int | seats this bundle grants |
| `MaxQuantityPerOrder` | int? | nullable (D:Q2) |
| `IsActive` | bit | |
| audit + `IsDeleted` + `RowVersion` | | |

### 2.3 `Order` (D:Q49) — flat, append-only, **no OrderItem table**

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `OrderReference` | nvarchar | public code, **unique** |
| `AccountId` | Guid FK → ApplicationUser (**Restrict**) | buyer |
| `EventId` | Guid FK → Event | |
| `PackageId` | Guid? FK → Package | **nullable ⇒ individual-ticket order** (Model B) |
| `UnitType` | int enum | Individual \| Package (explicit alongside PackageId) |
| `Quantity` | int | seats in this order |
| `UnitPriceSnapshot` | decimal(18,2) | event `TicketPrice` or package `Price` **at reserve** |
| `SubtotalSnapshot` | decimal(18,2) | |
| `DiscountSnapshot` | decimal(18,2) | from promo, if any |
| `TotalSnapshot` | decimal(18,2) | `max(subtotal − discount, 0)` (D:Q18) |
| `PromoCodeId` | Guid? FK → PromoCode | |
| `PromoCodeSnapshot` | nvarchar? | the code string, for audit |
| `Status` | int enum | PendingPayment \| Paid \| Cancelled \| Expired |
| `HoldExpiresAtUtc` | datetime2? | 15-min hold; NULL once Paid (D:Q3) |
| `PaymobOrderId` / `PaymentReference` | nvarchar? | null until payment initiated |
| audit + `RowVersion` | | **no `IsDeleted`** — cancel = status (append-only) |

**Indexes:** unique `OrderReference`; `(EventId, Status)` (held-seats), `(AccountId, EventId, Status)` (one-active-pending-**per-event** rule D:Q5, "active order exists").
**Anti-tamper:** server re-prices at reserve vs the snapshot → `PRICE_CHANGED` (409) on mismatch (D:Q4).

### 2.4 `Ticket` (D:Q49) — one row per seat, issued on payment confirmation

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `OrderId` | Guid FK → Order | |
| `EventId` | Guid FK → Event | **denormalized** for the event-scoped scan |
| `TicketReference` | nvarchar | public, e.g. `TKT-7F3A9C`, **unique** |
| `QrSecretHash` | nvarchar | **SHA-256 of the 256-bit secret — raw never stored** (D:Q8) |
| `HolderName` | nvarchar? | optional guest name (Kareem); nameless ticket still valid |
| `Status` | int enum | Issued \| CheckedIn \| Voided (D:Q7) |
| `CheckedInAtUtc` | datetime2? | |
| `CheckedInBy` | Guid? | the Admin scanner |
| audit + `RowVersion` | | append-only; `RowVersion` guards idempotent check-in |

**Indexes:** **unique `QrSecretHash`**, **unique `TicketReference`**, `(EventId, Status)` (scan path).

### 2.5 `PromoCode` (D:Q50) — columns map 1:1 to the flat 422 codes

| Column | Type | Maps to error |
|--------|------|---------------|
| `Id` | Guid PK | |
| `Code` | nvarchar | normalized/upper, **unique** |
| `EventId` | Guid? FK → Event | null = any event; set = scoped → `PROMO_WRONG_EVENT` |
| `DiscountType` | int enum | Percentage \| FixedAmount |
| `DiscountValue` | decimal(18,2) | |
| `IsActive` | bit | → `PROMO_INACTIVE` |
| `ValidFromUtc` | datetime2 | → `PROMO_NOT_YET_VALID` |
| `ValidUntilUtc` | datetime2 | → `PROMO_EXPIRED` |
| `MaxTotalRedemptions` | int? | → `PROMO_CAP_REACHED` |
| `MaxPerUser` | int? | → `PROMO_USER_LIMIT` |
| audit + `IsDeleted` + `RowVersion` | | |

### 2.6 `PromoRedemption` (D:Q50) — append-only ledger

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `PromoCodeId` | Guid FK → PromoCode | |
| `AccountId` | Guid FK → ApplicationUser (**Restrict**) | |
| `OrderId` | Guid FK → Order | |
| `RedeemedAtUtc` | datetime2 | |

- **Indexes:** `(PromoCodeId)` (global cap count), `(PromoCodeId, AccountId)` (per-user count).
- Both caps counted **inside the SERIALIZABLE reserve tx** (D:Q33/Q50) so concurrent redemptions can't both slip under. **Recorded at reserve; released by the sweeper on hold-expiry** (D:Q19).

---

## 3. Training context

### 3.1 `Track` (D:Q51)

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `NameEn`, `NameAr`, `Slug`, `Description` | nvarchar | |
| `IsActive` | bit | |
| audit + `IsDeleted` + `RowVersion` | | |

### 3.2 `TrackAssignment` (D:Q51) — Member/Board + **is the enrollment** (D:Q52)

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `AccountId` | Guid FK → ApplicationUser (**Restrict**) | |
| `TrackId` | Guid FK → Track | intra-context |
| `TrackRole` | int enum | **Member \| Board** |
| `AssignedAtUtc` | datetime2 | **for a Member, this is the enrollment/join date** = attendance-% denominator start (D:Q52) |
| `AssignedBy` | Guid | |
| audit + `RowVersion` | | |

**The dual-role invariants as constraints (D:Q51):**
- **Filtered unique index** `UQ_Assignment_OneMember`: `UNIQUE(AccountId) WHERE TrackRole = Member` → ≤1 Member track per user (race-proof).
- **Filtered unique index** `UQ_Assignment_OneBoard`: `UNIQUE(AccountId) WHERE TrackRole = Board` → ≤1 Board track per user.
- **Plain unique** `(AccountId, TrackId, TrackRole)` → no exact-duplicate rows.
- **Index** `(TrackId, TrackRole)` → roster queries ("list the Board / Members of track X").
- **Different-track rule** (no Member@X + Board@X; Member@X + Board@Y allowed) → **domain invariant**, checked in the same tx (a filtered index can't express it without a trigger/indexed view; the concurrency-dangerous part is already covered by the two filtered indexes).

### 3.3 `Session` (D:Q52)

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `TrackId` | Guid FK → Track | intra-context |
| `TitleEn`, `TitleAr`, `Description` | nvarchar | |
| `StartsAtUtc`, `EndsAtUtc` | datetime2 | |
| `Location` | nvarchar | |
| `Status` | int enum | Scheduled \| Held \| Cancelled |
| audit + `IsDeleted` + `RowVersion` | | records-bearing session editable, not hard-deletable (D:Q13) |

### 3.4 `Attendance` (D:Q52)

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `SessionId` | Guid FK → Session | |
| `AccountId` | Guid FK → ApplicationUser (**Restrict**) | the member |
| `Status` | int enum | **Present \| Late \| Absent** |
| `RecordedAtUtc` | datetime2 | |
| `RecordedBy` | Guid | the Board member |
| audit + `RowVersion` | | |

- **Unique `(SessionId, AccountId)`** — one record per member per session (re-record = update, not duplicate).
- **Attendance % computed, never stored:** `(Present + Late) / totalRecordedSessions` — **Late counts as attended** (D:Q12); an Absent must be explicitly recorded (D:Q12).

### 3.5 `Evaluation` (D:Q52)

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `AccountId` | Guid FK → ApplicationUser (**Restrict**) | the evaluated member |
| `TrackId` | Guid FK → Track | |
| `SessionId` | Guid? FK → Session | nullable — per-session or general |
| `Score` | int | **0–100 inclusive** (D:Q17); reject `<0`/`>100`/non-integer |
| `CommentEn`, `CommentAr` | nvarchar? | |
| `EvaluatedBy` | Guid | Board |
| audit + `RowVersion` | | |

- **Visibility enforced in the handler** (member sees only their own; Board sees their track) — not a schema concern (D:Q52).
- Evaluation requires a **past session + active enrollment** (D:Q16).

---

## 4. Cross-cutting: `OutboxMessage` (D:Q53)

| Column | Type | Notes |
|--------|------|-------|
| `Id` | Guid PK | |
| `Type` | nvarchar | e.g. `OrderConfirmationEmail`, `TicketIssuedNotification` |
| `PayloadJson` | nvarchar(max) | **ids + non-secret fields only** (log-hygiene, D:Q41) |
| `CreatedAtUtc` | datetime2 | |
| `ProcessedAtUtc` | datetime2? | null = pending |
| `Attempts` | int | |
| `LastError` | nvarchar? | |
| `NextAttemptAtUtc` | datetime2? | backoff |

- **Filtered index** on `WHERE ProcessedAtUtc IS NULL` (+ `NextAttemptAtUtc`) — cheap "due, unprocessed" sweep.
- Written **inside the business transaction**; drained **after commit** by the sweeper; **at-least-once** delivery (consumers idempotent). See [SequenceDiagrams §2](./SequenceDiagrams.md).

---

## 5. Index summary (the invariant-critical ones)

| Index | Table | Enforces |
|-------|-------|----------|
| `UQ Ticket.QrSecretHash` | Ticket | No two tickets share a QR; scan lookup |
| `UQ Ticket.TicketReference` | Ticket | Unambiguous public code |
| `UQ Order.OrderReference` | Order | Unambiguous public code |
| `IX Order (EventId, Status)` | Order | Held-seats computation |
| `IX Order (AccountId, EventId, Status)` | Order | One-active-pending-order **per event** rule (D:Q5) |
| `IX Ticket (EventId, Status)` | Ticket | Event-scoped check-in scan |
| `UQ Assignment (AccountId) WHERE Member` | TrackAssignment | ≤1 Member track per user |
| `UQ Assignment (AccountId) WHERE Board` | TrackAssignment | ≤1 Board track per user |
| `UQ Assignment (AccountId, TrackId, TrackRole)` | TrackAssignment | No duplicate assignment |
| `UQ Attendance (SessionId, AccountId)` | Attendance | One attendance record per member per session |
| `UQ PromoCode.Code` | PromoCode | Unique code |
| `IX PromoRedemption (PromoCodeId, AccountId)` | PromoRedemption | Per-user promo limit count |
| `UQ RefreshToken.TokenHash` | RefreshToken | Token lookup / no dupes |
| `Filtered IX Outbox WHERE ProcessedAtUtc IS NULL` | OutboxMessage | Cheap pending-sweep |

---

*Data model v1.0 — 2026-07-22. Diagram + relationship rules in [ERD.md](./ERD.md); aggregate behavior in [ClassDiagrams.md](./ClassDiagrams.md); lifecycles in [StateMachines.md](./StateMachines.md).*
