# TEDxAlkawmia — User Stories

> **Version:** 1.1
> **Date:** 2026-07-24
> **Reads from:** [01 — PRD](./01-PRD.md) · [02 — SRS](./02-SRS.md) · [03 — User Flows](./03-UserFlows.md) · [04 — Personas](./04-Personas.md)
> **Decisions:** grilling sessions 2026-07-20 to 2026-07-24 — **Q1–Q56** (requirements Q1–Q28 + architecture Q29–Q55 + Q56 Archived→Cancelled). Where a story depends on a resolved decision, it is cited as **(D:Qn)**.
>
> **v1.1 (2026-07-24):** Added Q56 Archived→Cancelled to the event lifecycle (US-ADM-EVT-03/04). Refreshed provenance from (Q1–Q28) to (Q1–Q56). No story IDs renamed — anchors `[[05-UserStories#US-*]]` are stable.

---

## How to read this document

Each story follows the canonical form:

> **US-XXX-nn** — *As a `<role>`, I want `<capability>`, so that `<benefit>`.*
> **Traces:** PRD feature ID(s) · SRS `FR-*` · User Flow §.
> **Notes:** scope, edge cases, and cross-references to grilling decisions.

- **Role vocabulary:** Visitor, Attendee, Member@T (of track T), Board@T (supervisor of track T), Admin — per [PRD §5](./01-PRD.md). Every Member/Board/Admin is also an Attendee.
- Acceptance criteria for each story live in the companion **Acceptance Criteria** document, keyed by the same ID.
- Story IDs are stable; the Personas document links to them by `[[05-UserStories#US-...]]`.

> **Story-ID remap note (2026-07-21).** This document's ID scheme was reorganized during the grilling rewrite. A few prefixes now mean something different from the pre-grilling drafts — external links must point at the **current** meaning:
> - **US-MNG-\*** = **Admin user management** (list/view/deactivate users). *(The old Attendee "my orders/tickets" stories are now **US-ORD-05** and the ticket stories **US-TKT-01…03**.)*
> - **US-ROLE-\*** = track-assignment & dual-role stories (US-ROLE-07 = the system dual-role invariant).
> - **US-CHK-04** = Admin void a paid order (offline refund). *(Old "scan auditing" is now folded into US-CHK-03.)*
> - **US-EVT-\*** = public discovery; admin event management is **US-ADM-EVT-\***.
> Anyone linking these anchors (Personas, PRD) should use the meanings above.

---

## 1. Authentication & Authorization (PRD §6.1 · SRS §3.1 · Flow §1)

### US-AUTH-01
*As a **Visitor**, I want to register with my first name, last name, email, and password, so that I get an account and can book tickets.*
- **Traces:** AUTH-01 · FR-AUTH-01, FR-AUTH-02, FR-AUTH-03 · Flow §1.1
- **Notes:** On success the account is created with the **Attendee** global role and **no track assignments**. Email must be unique among accounts; password policy ≥ 8 chars with at least one upper, one lower, one digit (server-enforced). Confirm-password match is a client+server check.

### US-AUTH-02
*As an **Attendee**, I want to log in with my email and password, so that I receive tokens and can use authenticated features.*
- **Traces:** AUTH-02, AUTH-06 · FR-AUTH-04, FR-AUTH-05, FR-AUTH-06 · Flow §1.2
- **Notes:** Returns a short-lived **access token (JWT)** carrying account id, email, and **global role only** — per-track authority is resolved per request, never baked into the token. Login fails with a generic "invalid email or password" for both unknown email and wrong password (no enumeration), and is rejected for deactivated accounts with a distinct message. Access token default lifetime **15 min** (D:Q24).

### US-AUTH-03
*As an **Attendee**, I want my session to refresh silently, so that I stay logged in without re-entering my password.*
- **Traces:** AUTH-06 · FR-AUTH-08, FR-AUTH-09 · Flow §1.2
- **Notes:** Refresh tokens are **single-use, rotated** on each exchange, stored **hashed** (raw token only on the client), default lifetime **7 days** (D:Q24). Reuse of a consumed/revoked refresh token is rejected and **revokes the whole token family** (D:Q24, NFR-SEC-02). An expired refresh token (past `ExpiresAtUtc`) is rejected with `TOKEN_INVALID` (`ReasonRevoked = Expired`), distinct from `TOKEN_REUSED` (rotation violation) — the client must distinguish these for appropriate UX (D:Q47 `ReasonRevoked` enum: `Rotated | Reuse | Logout | Expired`).

### US-AUTH-04
*As an **Attendee**, I want to log out, so that my refresh token can no longer be used.*
- **Traces:** AUTH-03 · FR-AUTH-07 · Flow §1.2
- **Notes:** Logout revokes the presented refresh token server-side. The (stateless) access token is left to expire naturally.

### US-AUTH-05
*As an **Attendee**, I want to request a password-reset email, so that I can regain access if I forget my password.*
- **Traces:** AUTH-04 · FR-AUTH-10 · Flow §1.3
- **Notes:** The response is **identical whether or not the email exists** (no enumeration). If the account exists, a single-use, time-limited reset token is emailed (default **1 hour**, D:Q24).

### US-AUTH-06
*As an **Attendee**, I want to set a new password using the reset link, so that I can log in again.*
- **Traces:** AUTH-05 · FR-AUTH-11 · Flow §1.3
- **Notes:** The reset token must be valid, unexpired, and unused; used/expired/already-consumed tokens are rejected with `RESET_TOKEN_INVALID` (D:Q47, DataModel §1.2). New password must meet the policy. On success, **all existing refresh tokens for the account are revoked** (mandatory — NFR-SEC-02, D:Q24; implemented via `IX_RefreshToken_AccountId`).

### US-AUTH-07
*As the **system**, I want every protected endpoint to enforce the global role and per-track policies server-side, so that the client is never trusted for access decisions.*
- **Traces:** AUTH-07 · FR-AUTH-06, NFR-SEC-03 · Flow (cross-cutting)
- **Notes:** Authorization combines the **global role** (from the token) with **per-track assignments** (resolved per request). A Board's powers apply only to the single track they supervise; cross-track access returns **403** even when the caller is a Member of another track (Persona: Yousef).

---

## 2. User & Profile Management (PRD §6.2 · SRS §3.2 · Flow §10)

### US-USER-01
*As an **Attendee**, I want to view my profile, so that I can see my details, role, and track assignments.*
- **Traces:** USER-01 · FR-USER-01 · Flow §10
- **Notes:** Profile shows name, email, phone, bio, profile picture, global role, and current track assignments (Member@T and/or Board@T).

### US-USER-02
*As an **Attendee**, I want to edit my first name, last name, phone, and bio, so that my profile stays current.*
- **Traces:** USER-02 · FR-USER-02 · Flow §10
- **Notes:** **Email is immutable** after registration. Global role and track assignments are not self-editable.

### US-USER-03
*As an **Attendee**, I want to upload a profile picture, so that my account is personalized.*
- **Traces:** USER-03 · FR-USER-03 · Flow §10
- **Notes:** File validated for **type (image)** and **size**; stored in **Cloudinary**, only the URL persisted. Priority P1.

### US-USER-04
*As an **Attendee**, I want to change my password while logged in, so that I can keep my account secure.*
- **Traces:** USER-04 · FR-USER-04 · Flow §10
- **Notes:** Requires the **current password** plus a new one meeting the policy.

---

## 3. Admin: User Management & Roles (PRD §6.2, §5 · SRS §3.2–3.3 · Flow §7)

### US-MNG-01
*As an **Admin**, I want to list all users with pagination, search, and filters, so that I can find and manage accounts.*
- **Traces:** USER-05, ADM-02 · FR-USER-05 · Flow §7
- **Notes:** Search by name/email; filter by **global role** and **active status**. Offset pagination with `meta` block (D:Q26).

### US-MNG-02
*As an **Admin**, I want to view a single user's full detail, so that I can see their roles, assignments, and status before acting.*
- **Traces:** USER-05 · FR-USER-01, FR-USER-05 · Flow §7
- **Notes:** Shows global role, active Member/Board assignments, and account status. **Read-only prerequisite** for the role-action stories (US-ROLE-01–04) — role mutations are performed from this surface. See PRD ADM-02 for the full user management feature scope.

### US-MNG-03
*As an **Admin**, I want to deactivate (and reactivate) a user account, so that I can revoke access without losing history.*
- **Traces:** USER-07 · FR-USER-06, FR-USER-07 · Flow §7
- **Notes:** Deactivation is a **soft action** — the account and all historical records are retained (never hard-deleted). A deactivated user **cannot log in or refresh**, and existing refresh tokens are revoked. **Cross-context ripple (D:Q10):** the user's **Issued tickets remain valid** (a paid seat admits by QR, not by login); any **active PendingPayment order is cancelled and its seats released**; **track assignments are ended** (`EndedAt` set — records retained per FR-ROLE-05, D:Q11/Q14), which **frees the dual-role slots** (FR-ROLE-04) so those tracks can be re-staffed. Ended assignments are **not restored on reactivation** — the Admin re-assigns explicitly. If the user is a **Board**, their track is **flagged as needing a new supervisor** for the Admin — never silently orphaned.

### US-MNG-04
*As an **Admin**, I want to see the operational impact before deactivating a user, so that I don't accidentally orphan a track or void something important.*
- **Traces:** USER-07 · FR-USER-06 · Flow §7
- **Notes:** Confirmation surface states counts: active orders to be cancelled, tickets that remain valid, assignments to be ended (freeing their dual-role slots), and whether a supervised track will be left without a Board (D:Q10).

---

## 4. Track Assignments & the Dual-Role Rule (PRD §5, §6.2 · SRS §3.3 · Flow §7)

> **Signature rule (PRD §5, FR-ROLE-04):** a person may hold **at most one active Member enrollment** and **at most one active Board assignment**, and the two **must be different tracks**. Enforced at assignment time **and** at the database level (filtered unique indexes).

### US-ROLE-01
*As an **Admin**, I want to change a user's global role (Attendee ↔ Admin), so that I can grant or revoke platform administration.*
- **Traces:** — · FR-ROLE-01 · Flow §7
- **Notes:** **Only an Admin** may change global roles. Multiple Admins are allowed.

### US-ROLE-02
*As an **Admin**, I want to assign or remove the Board role on a specific track, so that I can appoint track supervisors.*
- **Traces:** TRK-02 · FR-ROLE-02, FR-ROLE-04 · Flow §7
- **Notes:** **Only an Admin** assigns/removes Board. Rejected if it would give the user a Board track equal to their Member track, or a second active Board assignment (D:Q15 enforcement mirror).

### US-ROLE-03
*As an **Admin** or **Board@T**, I want to enroll a Member into track T, so that they can participate in training.*
- **Traces:** TRK-03 · FR-ROLE-03, FR-ROLE-04 · Flow §7, §9
- **Notes:** Target must be an **existing Attendee account** (found by email/search) — no account creation by enrollment (D:Q15). Enrollment is **rejected** if the target is already an active Member of **any** track, or if it would make them **Member and Board of the same track**; it is **allowed** if they are Board of a **different** track (the sanctioned dual-role case). A **Board may only enroll into the single track they supervise**. Every rejection carries a clear machine-readable reason.

### US-ROLE-04
*As an **Admin** or **Board@T**, I want to remove a Member from track T, so that I can manage the roster.*
- **Traces:** TRK-04 · FR-ROLE-03, FR-ROLE-05 · Flow §7, §9
- **Notes:** Removal **ends the enrollment** (sets `EndedAt`) but **retains the enrollment row and its attendance/evaluation records** (D:Q11, FR-ROLE-05). A Board may remove only within their supervised track. A re-enrolled user starts a **new** enrollment with a fresh attendance percentage.

### US-ROLE-05
*As an **Admin**, I want role and assignment changes to be attributable, so that there is an audit trail of who changed what and when.*
- **Traces:** — · FR-ROLE-05, NFR-SEC-09 · Flow §7
- **Notes:** Assignment/role mutations record actor + timestamp (audit columns).

### US-ROLE-06
*As a **Member**, I want my past training records to survive changes to my enrollment, so that my history is never lost.*
- **Traces:** — · FR-ROLE-05 · Flow §8
- **Notes:** Ending an assignment (by removal, track soft-delete, or user deactivation) retains all linked attendance and evaluation records (D:Q11, D:Q14).

### US-ROLE-07
*As the **system**, I want the dual-role constraints enforced physically, so that invalid role states cannot exist even under concurrency.*
- **Traces:** — · FR-ROLE-04 · Flow §7
- **Notes:** Two filtered unique indexes enforce ≤1 active Member enrollment and ≤1 active Board assignment per user; the "must differ" rule is checked at assignment time. Violations are rejected, not silently corrected.

### US-ROLE-08
*As an **Admin** or **Board@T**, I want to search for existing Attendees by name or email to find a user to enroll, so that I can identify the right account without creating a duplicate.*
- **Traces:** TRK-03 · FR-ROLE-03 · DataModel §1.1 · D:Q15, D:Q26
- **Notes:** Returns only **active** (`IsActive = true`) Attendee-global-role accounts. Excludes users already active Members or who would violate dual-role rules — server-side enforcement at enroll time (D:Q15, D:Q51). Paginated with `meta` (D:Q26). A Board may only search in the context of enrolling into their supervised track.

---


---

## 5. Public Event Discovery (PRD §6.3 · SRS §3.4 · Flow §2)

### US-EVT-01
*As a **Visitor**, I want to browse a list of published events, so that I can discover what's happening.*
- **Traces:** EVT-04, PUB-04 · FR-EVT-04, FR-EVT-05 · Flow §2
- **Notes:** Only **Published** events are visible to Visitors. List is paginated with a `meta` block and filterable by **upcoming/past** — where "past" is **date-derived** (`event.date < now`) among Published events, **not** the Archived status (D:Q23). Draft/Archived/Cancelled events never appear publicly.

### US-EVT-02
*As a **Visitor**, I want to view an event's detail page, so that I can decide whether to book.*
- **Traces:** EVT-05, PUB-05 · FR-EVT-06 · Flow §2
- **Notes:** Detail includes description, date/time (UTC, localized at presentation), location, the **individual-ticket price**, any **optional packages with prices**, and **remaining seats**. Remaining seats are **computed** as `Capacity − held seats`, never a stored counter (FR-EVT-07), where held = Paid + PendingPayment with a live unexpired hold (D:Q3).

### US-EVT-03
*As a **Visitor**, I want a clear "Login to book" prompt on an event, so that I know how to proceed to purchase.*
- **Traces:** EVT-05 · FR-EVT-06 · Flow §2
- **Notes:** Booking actions are gated behind authentication; a Visitor sees the event and the CTA but cannot reserve or hold seats.

### US-EVT-04
*As an **Attendee**, I want to see live remaining-seat availability, so that I know whether I can still book.*
- **Traces:** EVT-05 · FR-EVT-06, FR-EVT-07 · Flow §2, §3
- **Notes:** Availability tracks the wall clock: a lapsed hold stops counting the instant `HoldExpiresAt < now`, independent of the background sweeper (D:Q3). Availability MUST read committed data and MUST NOT be served from cache (NFR-PERF-05).

---

## 6. Admin: Event Management (PRD §6.3 · SRS §3.4 · Flow §6)

### US-ADM-EVT-01
*As an **Admin**, I want to create an event, so that it can be published and sell tickets.*
- **Traces:** EVT-01, ADM-03 · FR-EVT-01 · Flow §6
- **Notes:** Fields: title, description, date/time (**stored UTC**, NFR-REL-05), location, **capacity (> 0)**, **individual-ticket price (`ticketPrice` ≥ 0 EGP)**, optional **`maxIndividualQtyPerOrder`** (nullable = no cap, mirrors package cap D:Q2), optional image (Cloudinary URL). Created in **Draft** status. Packages are optional (US-ADM-PKG-01) — an event sells individual tickets without any.

### US-ADM-EVT-02
*As an **Admin**, I want to edit an event's details, so that I can correct or update it.*
- **Traces:** EVT-02 · FR-EVT-02, NFR-REL-06 · Flow §6
- **Notes:** Concurrent edits guarded by an **optimistic-concurrency token** (`rowversion`); a stale edit is surfaced, not silently overwritten. **Capacity** may be **raised anytime** but **lowered only to ≥ current held+paid seats** (else rejected — never invalidates sold seats) (D:Q22). Core fields (date/location) remain editable and are audited; automatic holder notification on change is **deferred** (D:Q22).

### US-ADM-EVT-03
*As an **Admin**, I want to publish, archive, or re-list an event, so that I control its public visibility.*
- **Traces:** EVT-06 · FR-EVT-04 · Flow §6
- **Notes:** **State machine (D:Q23, D:Q56):** Draft ⇄ Published **only while zero orders exist**; Published → Archived; Archived → Published. **Draft → Cancelled is blocked** — a zero-order Draft is disposed of by soft-delete (D:Q22, D:Q56), never cancelled. Archived is a **manual hide** (kept, off all listings) distinct from a date-past event. Cancellation (from Published or Archived) is handled entirely by **US-ADM-EVT-04** — do not implement cancel transitions here.

### US-ADM-EVT-04
*As an **Admin**, I want to cancel an event that is called off, so that ticket holders are handled correctly.*
- **Traces:** EVT-06 · FR-EVT-04, FR-PAY-07 · Flow §6
- **Notes:** Cancel is reachable **from Published or Archived** (D:Q56) — the ripple is identical either way. **Draft → Cancelled is blocked** — a zero-order Draft is disposed of by soft-delete (D:Q22, D:Q56), never cancelled. Cancelling (status → **Cancelled**, terminal) **voids all Issued tickets**, **releases all PendingPayment holds**, and **creates a `RefundEntry` row for each Paid order** recording `Reason` (admin-supplied), `VoidedTicketCount`, `SeatsReleased`, and `CheckedInTicketsRetained` (D:Q6, DataModel §2.6); money is handled offline. A ticket already **CheckedIn is non-voidable** and its seat stays consumed. Cancelled events are hidden from listings but **retained**. No un-cancel.

### US-ADM-EVT-05
*As an **Admin**, I want to soft-delete a mistaken/empty event, so that it disappears without harming history.*
- **Traces:** EVT-03 · FR-EVT-03 · Flow §6
- **Notes:** **Soft-delete is allowed only when the event has zero orders** (D:Q22); an event with any orders can only be **Cancelled** (preserving financial history). Soft-deleted events are hidden from all listings but retained.

### US-ADM-EVT-06
*As an **Admin**, I want to view all orders and attendees for an event, so that I can manage and reconcile it.*
- **Traces:** EVT-06, ADM-03 · FR-EVT-08 · Flow §6.2
- **Notes:** Paginated; shows orders across all statuses and the tickets/attendees they produced.

### US-ADM-EVT-07
*As an **Admin**, I want to view the promo codes scoped to a specific event (including redemption counts against caps), so that I can manage event-level discounts and monitor uptake.*
- **Traces:** ORD-05 · FR-PROMO-05 · DataModel §2.7 · D:Q50
- **Notes:** Read-only report surface on the event management page. Uses `IX_PromoRedemption_PromoCode` to count redemptions with `Status IN (Claimed, Confirmed)`. Shows both active and inactive event-scoped codes.

---

## 7. Admin: Ticket Packages (PRD §6.4 · SRS §3.5 · Flow §6)

### US-ADM-PKG-01
*As an **Admin**, I want to define ticket packages for an event, so that Attendees can buy seat bundles.*
- **Traces:** ORD-01 · FR-PKG-01, FR-PKG-02 · Flow §6
- **Notes:** Packages are **optional** discount bundles on top of the event's individual ticket — an event with **zero packages** still sells individual tickets and is fully publishable. Each package has a **name**, **seats-per-package (≥ 1)**, **price (≥ 0 EGP)** — price **0 is permitted** (free package). Also carries **`MaxQuantityPerOrder`** (nullable; null = no cap) enforced at quote and reserve (D:Q2).

### US-ADM-PKG-02
*As an **Admin**, I want to activate/deactivate and soft-delete packages, so that I can manage what's on sale without breaking history.*
- **Traces:** ORD-01 · FR-PKG-03 · Flow §6
- **Notes:** A package **referenced by existing orders MUST NOT be hard-deleted** (soft-delete only). Deactivating hides it from new purchases but leaves historical orders intact (prices are snapshotted, D:Q4).

### US-ADM-PKG-03
*As an **Admin**, I want to list all packages for an event and view each package's details, so that I can manage the event's package offerings.*
- **Traces:** ORD-01 · FR-PKG-01, FR-PKG-04 · DataModel §2.2 · D:Q2
- **Notes:** Read path. Remaining seats per package are computed (not stored) from held + issued seats. Active and inactive packages shown with redemption counts. Backed by `MaxQuantityPerOrder` and `RowVersion` (DataModel §2.2).

### US-PKG-03
*As an **Attendee** or **Visitor**, I want to see an event's individual-ticket price and any active packages, so that I can choose how to buy.*
- **Traces:** ORD-01 · FR-PKG-04 · Flow §2, §3
- **Notes:** The event's **individual-ticket price** is always shown; **active** packages of a **Published** event are shown as optional bundles alongside it.

---

## 8. Admin: Promo Codes (PRD §6.4 · SRS §3.6 · Flow §6)

### US-ADM-PRM-01
*As an **Admin**, I want to create a promo code with a discount, so that I can offer price reductions.*
- **Traces:** ORD-05 · FR-PROMO-01, FR-PROMO-05 · Flow §6
- **Notes:** Discount is **percentage** or **fixed amount (EGP)**. Code must be **unique among live (non-deleted) codes**. Discount math: computed on base, **rounded half-up to 2dp**, and **final = max(base − discount, 0)** — an over-large discount yields a free (0) order (D:Q18).

### US-ADM-PRM-02
*As an **Admin**, I want to constrain a promo code's usage, so that I can control its reach and validity.*
- **Traces:** ORD-05 · FR-PROMO-02, FR-PROMO-03 · Flow §6
- **Notes:** Optional **global redemption cap**, **per-user limit**, **validity window**, and **event scope** (null = all events). `ValidFrom` and `ValidUntil` are **independently nullable** — `null ValidFrom` = no lower bound (never raises `PROMO_NOT_YET_VALID`); `null ValidUntil` = no expiry (never raises `PROMO_EXPIRED`); both null = always valid (D:Q50, DataModel §2.7). If both are provided, `ValidFromUtc` must be strictly before `ValidUntilUtc` — enforced at the application layer (DataModel §2.7), not a schema constraint. The system rejects a code that is inactive, outside its window, over its global cap, over the user's per-user limit, or scoped to a different event; runtime failures return distinct codes: `PROMO_NOT_YET_VALID`, `PROMO_EXPIRED`, `PROMO_CAP_REACHED`, `PROMO_USER_LIMIT`, `PROMO_WRONG_EVENT` (D:Q50).

### US-ADM-PRM-03
*As an **Admin**, I want each redemption recorded, so that limits are enforced and there's an audit trail.*
- **Traces:** ORD-05 · FR-PROMO-04 · Flow §6
- **Notes:** A redemption records code, user, order, timestamp. **Timing (D:Q19):** a redemption **slot is atomically claimed at payment initiation** (paid orders) or **at confirmation** (free/100%-off orders), **confirmed on Paid**, and **released on payment failure or hold expiry** — so unpaid holds never burn a limited promo and the cap is never exceeded.

### US-ADM-PRM-04
*As an **Admin**, I want to list, view, edit, and soft-delete existing promo codes, so that I can manage promotions throughout their lifecycle.*
- **Traces:** ORD-05 · FR-PROMO-05 · DataModel §2.7 · D:Q50, D:Q28c
- **Notes:** Editable fields: discount value, validity window, caps, active status, event scope. Soft-delete is blocked if the code has active/confirmed redemptions. Editing when both `ValidFromUtc` and `ValidUntilUtc` are present requires `ValidFrom < ValidUntil` (DataModel §2.7). Concurrent edits guarded by `RowVersion` → `CONCURRENCY_CONFLICT`.

---


---

## 9. Ordering & Seat Holds (PRD §6.4 · SRS §3.7 · Flow §3)

> **Order shape (D:Q1 + Model-B addendum):** an order is **one unit-type × quantity** — **either individual tickets (at the event face price) or a single package** — not a multi-package cart and never a mix of the two. To buy a different unit-type, an Attendee places a separate order. (The SRS glossary was aligned to this in v1.1/v1.3.)

### US-ORD-01
*As an **Attendee**, I want a price quote for individual tickets or a package, a quantity, and optional promo code, so that I see the full cost before committing — without holding seats.*
- **Traces:** ORD-01, ORD-02 · FR-ORD-01 · Flow §3.1
- **Notes:** The quote shows **base price, discount, and final price**. It **holds no seats** and creates no order. `base = unitPrice × quantity`, where `unitPrice = event.ticketPrice` for an individual-ticket order (no package selected) or `package.price` for a package order; discount rounded **half-up to 2dp EGP**; `final = max(base − discount, 0)` (D:Q18). The quote is **advisory only** — pricing is re-validated at reserve (D:Q4). Quantity must be within the package's `MaxQuantityPerOrder` cap, or the event's `MaxIndividualQtyPerOrder` cap for individual tickets (D:Q2).

### US-ORD-02
*As an **Attendee**, I want to reserve individual tickets or a package, so that the seats are held while I pay.*
- **Traces:** ORD-03 · FR-ORD-02, FR-ORD-03, FR-ORD-04, FR-ORD-05 · Flow §3.2
- **Notes:** Reserving holds `quantity` seats for an individual-ticket order, or `seatsPerPackage × quantity` for a package order, for a **15-minute checkout window** (D:Q3, FR-ORD-05). The order carries a **nullable package reference** — `null` for an individual-ticket order (D:Q1 addendum). The server **re-prices and re-validates** at reserve (live event/package price + promo validity + per-user limit) and **snapshots** the unit price, base, discount, and final — plus the package name (package order) or event title (individual order) as `UnitNameSnapshot` — onto the order (D:Q4, FR-ORD-04); later catalog/price/promo edits never alter this order. If the price differs from the quote, the API responds with `PRICE_CHANGED` and the new quote for explicit re-confirmation (D:Q4) — it never silently charges a different amount. Capacity check is **concurrency-safe** (SERIALIZABLE) and uses the clock-aware held-seats predicate (D:Q3); it must not oversell (Persona: Kareem, whole-package seat count). **At most one active (PendingPayment, unexpired) order per user per event** (D:Q5) — re-reserving returns `ACTIVE_ORDER_EXISTS` and points the user to their existing pending order. Quantity-cap violation returns `QUANTITY_EXCEEDS_MAX` (D:Q2). A **Paid** order does not block a new reserve for the same event — only one *pending* unexpired order is blocked (D:Q5).

### US-ORD-03
*As an **Attendee**, I want to cancel my own unpaid order, so that I can release the seats if I change my mind.*
- **Traces:** ORD-07 · FR-ORD-06 · Flow §4.2
- **Notes:** Self-cancel applies to **PendingPayment orders only**. Cancelling releases the held seats immediately and, if a promo redemption slot was claimed, releases it (D:Q19). Paid orders cannot be self-cancelled — see US-CHK-void / Admin void (D:Q6).

### US-ORD-04
*As a **Group Buyer (Attendee)**, I want to buy a multi-seat package in one order and payment, so that my whole group is covered at once.*
- **Traces:** ORD-06 · FR-ORD-02, FR-TKT-01 · Flow §3 · Persona: Kareem
- **Notes:** A single order for a multi-seat package (e.g. Group-5) fans out to **one ticket per seat** once Paid (D:Q1, FR-TKT-01). The concurrency-safe capacity check applies to the **whole seat count**, not per ticket.

### US-ORD-05
*As an **Attendee**, I want to view my order history and the tickets of any paid order, so that I can track and retrieve my purchases.*
- **Traces:** ORD-08 · FR-ORD-07 · Flow §4.1
- **Notes:** History lists orders of **all statuses**; tickets are visible for **Paid** orders only (a reserved/unpaid order has zero tickets, FR-TKT-02). Offset pagination with `meta` (D:Q26). Users see **only their own** orders/tickets.

### US-ORD-06
*As the **system**, I want unpaid holds to auto-release when the checkout window elapses, so that abandoned reservations don't block real buyers.*
- **Traces:** ORD-09 · FR-ORD-05, FR-ORD-08 · Flow §3.4
- **Notes:** A background sweeper transitions lapsed `PendingPayment` orders to **Expired** and releases any claimed promo slot (D:Q19). **Seat availability does not depend on the sweeper** — a hold with `HoldExpiresAt < now` stops counting against capacity immediately (D:Q3); the sweeper is cleanup only.

### US-SYS-01
*As the **system**, I want a background sweeper that expires lapsed holds, releases associated promo slots, and drains the outbox with retry/backoff, so that seat availability is always correct and side-effects are delivered reliably.*
- **Traces:** ORD-09 · FR-ORD-05, FR-ORD-08 · D:Q3, Q19, Q34, Q45, Q53
- **Notes:** Sweeper runs as a `BackgroundService` guarded by `sp_getapplock` (single-instance). Hold expiry: orders with `HoldExpiresAtUtc < now` AND `Status = PendingPayment` → `Order.Expire()` + `PromoRedemption → Released`. Outbox: unprocessed rows with `NextAttemptAtUtc <= now` delivered at-least-once; backoff incremented on failure. Correctness of seat availability does NOT depend on sweeper timing — the clock-aware predicate handles it immediately (D:Q3).

---

## 10. Payment via Paymob (PRD §6.4 · SRS §3.8 · Flow §3)

### US-PAY-01
*As an **Attendee**, I want to pay for my reserved order online, so that my seats are confirmed and my tickets issued.*
- **Traces:** ORD-04 · FR-PAY-01 · Flow §3.3
- **Notes:** For an order with **final price > 0**, the system initiates a Paymob payment (cards + wallets, EGP) and returns a checkout URL/session. Amounts convert to **integer piastres only at the Paymob boundary** (D:Q18, NFR-CMP-03). Payment initiation accepts an optional **`Idempotency-Key`** header; a repeat with the same key returns the same checkout session rather than creating a new one (D:Q28a). A promo **redemption slot is atomically claimed at payment initiation** (D:Q19).

### US-PAY-02
*As the **system**, I want to confirm payment only from a signature-verified Paymob webhook, so that tickets are never issued on an unverified or client-reported result.*
- **Traces:** ORD-04 · FR-PAY-02, FR-PAY-03, FR-PAY-04, FR-PAY-05 · Flow §3.3
- **Notes:** The webhook handler **verifies the HMAC signature** and **validates the reported amount against the order's snapshotted final price** before marking the order **Paid** (NFR-SEC-04). **Amount mismatch** (`TotalSnapshot ≠ reported amount`): webhook rejected, **no tickets issued**, `Payment.Status = Failed` with the mismatched amount recorded for audit, and a structured error is logged at ERROR severity with `correlationId` for manual review (D:Q18, SD §7.6). Signature failure is rejected before any DB write. It is **idempotent** — a replayed callback for an already-paid order issues **no duplicate tickets** and double-counts no seats (FR-PAY-03, NFR-REL-02). Each payment attempt is recorded (status, Paymob transaction id, amount, raw verified payload) for reconciliation (FR-PAY-05). On success the promo redemption is **confirmed** (D:Q19); on failure/expiry the claimed slot is **released**.

### US-PAY-03
*As an **Attendee** buying a free or 100%-off order, I want my tickets issued immediately, so that I skip the payment gateway.*
- **Traces:** ORD-05 · FR-PAY-06 · Flow §3.3
- **Notes:** An order with **final price 0** (free package or 100%-off promo) **bypasses Paymob** and is confirmed immediately, issuing tickets at once (D:Q18). The promo redemption slot is claimed **at confirmation** (D:Q19).

### US-PAY-04
*As an **Attendee**, I want to see my payment result reflected in my order, so that I know whether it succeeded.*
- **Traces:** ORD-08 · FR-PAY-02, FR-ORD-07 · Flow §3.3
- **Notes:** The order status transitions to **Paid** only after the verified webhook; the client polls or re-fetches the order and **never** self-reports success. A failed/abandoned payment leaves the order `PendingPayment` until it either succeeds or expires.

### US-PAY-05
*As an **Admin**, I want to view payment attempts across orders, so that I can reconcile revenue and support buyers.*
- **Traces:** ORD-04 · FR-PAY-05 · Flow §6.2
- **Notes:** Lists each recorded payment attempt (status, Paymob transaction id, amount, timestamp) for reconciliation and support. A paid-then-refunded order is distinguished from a never-paid cancellation by the presence of a `RefundEntry`, not by order status (D:Q6, DataModel §2.3 Issue 7). Read-only; Admin-only. Backs `GET /admin/payments`.

### US-ADM-PAY-01
*As an **Admin**, I want to list and filter all orders across events (by event, status, date range, attendee name/email), so that I can manage the platform's financial state and support buyers.*
- **Traces:** ADM-03 · FR-ORD-07 · PRD §7.4 · DataModel §2.3 · D:Q26
- **Notes:** Paginated with `meta` (D:Q26). Distinguishes voided-paid (has `RefundEntry`) from user-cancelled-unpaid (no `RefundEntry`) even though both carry `Cancelled` status (DataModel §2.3 Issue 7). Backs `GET /admin/orders`.

---

## 11. Tickets & Per-Seat QR (PRD §6.4 · SRS §3.9 · Flow §3, §4)

### US-TKT-01
*As an **Attendee**, I want one QR ticket per seat once my order is paid, so that each attendee can enter independently.*
- **Traces:** ORD-06 · FR-TKT-01, FR-TKT-02, FR-TKT-04 · Flow §3.3
- **Notes:** On **Paid**, the system issues exactly **one ticket per held seat**, each with a **unique QR token** and a short human-readable **public reference** (e.g. `TKT-7F3A9C`), and an optional **guest name** (`guestName`, DataModel §2.4). The QR encodes the **public reference + a 256-bit random secret**; the server stores only the reference (indexed) and a **deterministic SHA-256 hash** of the secret — the raw secret exists only inside the QR image (D:Q8, FR-TKT-04, NFR-SEC-05). A reserved/unpaid order has **zero tickets** (FR-TKT-02).

### US-TKT-02
*As a **Group Buyer (Attendee)**, I want to put an optional guest name on each ticket, so that my friends can each carry their own, without being forced to name them.*
- **Traces:** ORD-06 · FR-TKT-03 · Flow §3.3, §4 · Persona: Kareem
- **Notes:** Each ticket **may** carry a guest name (`guestName`); a **nameless ticket is still fully valid**. Guests need no account. A ticket's `GuestName` is editable only while `Status = Issued`; `CheckedIn` and `Voided` tickets are non-editable (D:Q7). The `RowVersion` guards concurrent edits (Persona: Kareem — "reassign after check-in" forbidden).

### US-TKT-03
*As an **Attendee**, I want to view and present my tickets on my phone, so that I can be scanned at the door.*
- **Traces:** ORD-06 · FR-TKT-01 · Flow §4.1
- **Notes:** Tickets render from a **server-generated QR image** returned only to the account's own paid orders over HTTPS (D:Q8). The raw QR payload (public reference + 256-bit secret) is **never returned as a JSON field** — the backend renders the QR to an image and the frontend simply displays it, so the secret never appears in an API response body.

---

## 12. Check-in at the Door (PRD §6.4 · SRS §3.9 · Flow §5)

### US-ADM-TKT-01
*As an **Admin**, I want to list, search (by guest name or ticket reference), and filter tickets for an event by status (Issued/CheckedIn/Voided), so that I can manage and verify admissions at the door.*
- **Traces:** CHK-01 · FR-TKT-01 · DataModel §2.4 · D:Q7, Q9
- **Notes:** Read-only. Backed by `IX_Ticket_Event_Status (EventId, Status)`. No-show is derived (`Issued AND event.EndsAtUtc < now`), never a stored status (D:Q7). `CheckedInBy` and `CheckedInAtUtc` visible for auditing. Paginated with `meta` (D:Q26).

### US-CHK-01
*As an **Admin**, I want to scan a ticket's QR at the venue, so that I admit the holder exactly once.*
- **Traces:** CHK-01 · FR-TKT-05, FR-TKT-06 · Flow §5 · Persona: Mariam
- **Notes:** Check-in is **Admin-only** (per PRD matrix; a delegated scanner role is deferred, D:Q9). The scan endpoint is **scoped to a specific event**. The server resolves the scan by **public reference**, then verifies the **secret against the stored hash** (D:Q8). A ticket is **checkable in at most once** (FR-TKT-05).

### US-CHK-02
*As an **Admin**, I want a rejected scan to tell me exactly why, so that I can act correctly at the door.*
- **Traces:** CHK-01 · FR-TKT-05, FR-TKT-06 · Flow §5
- **Notes:** The scan yields one of **five distinct outcomes** (D:Q9; every rejection logged per FR-TKT-06): **success** · **already-checked-in** (returns who scanned and when) · **wrong-event** (valid ticket, but for a different event) · **voided** (a known ticket whose paid order was voided/refunded — `TICKET_VOIDED`, distinct from a garbage token so door staff can tell a refunded ticket from a fake one) · **unknown/invalid** (no matching reference or bad secret — `TICKET_INVALID`). Every rejected outcome is logged (FR-TKT-06).

### US-CHK-03
*As an **Admin**, I want every scan — success or rejection — recorded, so that there's an audit trail and no silent drops.*
- **Traces:** CHK-01 · FR-TKT-06, NFR-SEC-09, NFR-MNT-03 · Flow §5 · Persona: Mariam
- **Notes:** Successful check-in records **who scanned and when**. **Rejected/duplicate scans are logged too** (never silently ignored), with the reason code.

### US-CHK-04
*As an **Admin**, I want to void a paid order and its tickets when I process an offline refund, so that those seats can be resold and the tickets can't be used.*
- **Traces:** ORD-07, PAY-01 · FR-PAY-07 · Flow §4.2 · Persona: Mariam
- **Notes:** Voiding a Paid order is **Admin-only**; refund money is handled **offline** and recorded as a manual refund entry (D:Q6, FR-PAY-07). Voiding sets tickets to **Voided** and **releases only not-yet-checked-in seats** back to availability; a ticket already **CheckedIn is non-voidable** and its seat stays consumed (D:Q6). Financial records are never hard-deleted (NFR-REL-03).

---

## Traceability — Ordering, Payment, Tickets, Check-in

| Story | PRD | SRS | Flow | Decisions |
|-------|-----|-----|------|-----------|
| US-ORD-01 | ORD-01/02 | FR-ORD-01 | §3.1 | Q1,Q2,Q4,Q18 |
| US-ORD-02 | ORD-03 | FR-ORD-02/03/04/05 | §3.2 | Q3,Q4,Q5 |
| US-ORD-03 | ORD-07 | FR-ORD-06 | §4.2 | Q19 |
| US-ORD-04 | ORD-06 | FR-ORD-02, FR-TKT-01 | §3 | Q1 |
| US-ORD-05 | ORD-08 | FR-ORD-07 | §4.1 | Q26 |
| US-ORD-06 | ORD-09 | FR-ORD-05/08 | §3.4 | Q3,Q19 |
| US-SYS-01 | ORD-09 | FR-ORD-05/08 | §3.4 | Q3,Q19,Q34,Q45,Q53 |
| US-PAY-01 | ORD-04 | FR-PAY-01 | §3.3 | Q18,Q19,Q28a |
| US-PAY-02 | ORD-04 | FR-PAY-02/03/04/05 | §3.3 | Q19 |
| US-PAY-03 | ORD-05 | FR-PAY-06 | §3.3 | Q18,Q19 |
| US-PAY-04 | ORD-08 | FR-PAY-02 | §3.3 | — |
| US-PAY-05 | ORD-04 | FR-PAY-05 | §6.2 | Q6,Q7 |
| US-ADM-PAY-01 | ADM-03 | FR-ORD-07 | §6.2 | Q6,Q26 |
| US-TKT-01 | ORD-06 | FR-TKT-01/02/04 | §3.3 | Q8 |
| US-ADM-TKT-01 | CHK-01 | FR-TKT-01 | §5 | Q7,Q9,Q26 |
| US-TKT-02 | ORD-06 | FR-TKT-03 | §3.3,§4 | Q6 |
| US-TKT-03 | ORD-06 | FR-TKT-01 | §4.1 | Q8 |
| US-CHK-01 | CHK-01 | FR-TKT-05/06 | §5 | Q8,Q9 |
| US-CHK-02 | CHK-01 | FR-TKT-05/06 | §5 | Q9 |
| US-CHK-03 | CHK-01 | FR-TKT-06 | §5 | — |
| US-CHK-04 | ORD-07 | FR-PAY-07 | §4.2 | Q6 |


---

## 13. Track Management (PRD §6.7 · SRS §3.10 · Flow §7, §9)

### US-ADM-TRK-01
*As an **Admin**, I want to create a training track, so that I can add it to the training program.*
- **Traces:** TRK-01 · FR-TRK-01 · Flow §7
- **Notes:** Fields: `NameEn`, `NameAr`, `DescriptionEn`, `DescriptionAr`, `Schedule`. **Track names must be unique among live (non-deleted) tracks** (`TRACK_NAME_TAKEN`). Created with `IsActive = true` by default. See US-ADM-TRK-03 for editing, US-ADM-TRK-02 for soft-delete.

### US-ADM-TRK-03
*As an **Admin**, I want to edit a track's name, description, schedule, and active status, so that I can keep track information accurate.*
- **Traces:** TRK-01 · FR-TRK-01 · DataModel §3.1 · D:Q14
- **Notes:** `NameEn` must be unique among live tracks (`TRACK_NAME_TAKEN`). Both `NameEn` and `NameAr` are required. Concurrent edits guarded by `RowVersion` → `CONCURRENCY_CONFLICT`.

### US-ADM-TRK-04
*As an **Admin**, I want to list all training tracks with filters (active/inactive, search by name), so that I can navigate and manage the training program.*
- **Traces:** TRK-01 · FR-TRK-01 · DataModel §3.1 · D:Q26
- **Notes:** Paginated with `meta` (D:Q26). Shows each track's member count, Board assignment status, and active/inactive state. Soft-deleted tracks are hidden from the normal list but visible via archive view for Admin (D:Q54).

### US-ADM-TRK-02
*As an **Admin**, I want to soft-delete a track and be told its impact, so that I retire a track without orphaning people or losing history.*
- **Traces:** TRK-01 · FR-TRK-01, FR-ROLE-05 · Flow §7
- **Notes:** **(D:Q14)** Soft-deleting a track **auto-ends its active Member enrollments and Board assignment** (sets `EndedAt`), **retaining all history** (attendance, evaluations, sessions). Those users become **free to be assigned elsewhere** (unblocks the dual-role caps, FR-ROLE-04). The action is behind an **Admin confirmation stating the impact** ("this ends N enrollments and 1 Board assignment"). The track and its records remain queryable for reporting.

### US-ADM-TRK-05
*As an **Admin**, I want to view all enrollments for a track (active and ended), so that I can review the full membership history and investigate any disputes.*
- **Traces:** TRK-05 · FR-TRK-04 · DataModel §3.2 · D:Q11, Q14
- **Notes:** Shows `EndedAtUtc` where set. Backed by `IX_Assignment_Track_Role (TrackId, TrackRole, EndedAtUtc)`. Different from the Board's roster view (which shows active only). Admin can access soft-deleted track's enrollment history.

### US-BRD-TRK-01
*As a **Board@T** or **Admin**, I want to view a track's full detail — members, sessions, and progress summaries — so that I can supervise it.*
- **Traces:** TRK-05 · FR-TRK-04 · Flow §9
- **Notes:** A Board sees this **only for their supervised track**; an Admin sees any track. Members see the member-facing view (US-MEM-*), not the supervisory detail.

---

## 14. Session Management (PRD §6.8 · SRS §3.10 · Flow §9)

### US-BRD-SES-01
*As a **Board@T** or **Admin**, I want to create a session in the track, so that members know when and what the next meeting is.*
- **Traces:** SES-01 · FR-TRK-02 · Flow §9
- **Notes:** Session fields: topic, date, time, location. **A Board may only create sessions for the track they supervise** (D:Q13); cross-track writes return **403**.

### US-BRD-SES-02
*As a **Board@T** or **Admin**, I want to edit a session, so that I can fix its topic, time, or location.*
- **Traces:** SES-02 · FR-TRK-02 · Flow §9
- **Notes:** **(D:Q13)** A session **with any attendance or evaluation records may be edited** (metadata) but **MUST NOT be hard-deleted** — see US-BRD-SES-03. Board scope restricted to own track.

### US-BRD-SES-03
*As a **Board@T** or **Admin**, I want to delete a session created in error, so that stray sessions don't clutter the track.*
- **Traces:** SES-03 · FR-TRK-02, NFR-REL-03 · Flow §9
- **Notes:** **(D:Q13)** A session may be **hard-deleted only if it has zero attendance and zero evaluation records**. A session that has any records can only be **soft-deleted/cancelled**, preserving training history (FR-ROLE-05). Board scope restricted to own track. Priority P1.

### US-BRD-SES-04
*As a **Board@T** or **Admin**, I want to mark a session as Held (after it occurs) or Cancelled (if it is called off), so that the session status accurately reflects reality for attendance and reporting.*
- **Traces:** SES-02 · DataModel §3.3 · D:Q12, Q13
- **Notes:** `Scheduled → Held` (after `EndsAtUtc`). `Scheduled | Held → Cancelled` (session called off). A Cancelled session with attendance/evaluation records is soft-deleted only (`SESSION_HAS_RECORDS`). Board scope restricted to own track (D:Q13); Admin may transition any track's session.

### US-ADM-SES-01
*As an **Admin**, I want to manage sessions for any track (create, edit, delete, status transitions), so that I can administer the training program platform-wide.*
- **Traces:** SES-01, SES-02, SES-03 · FR-TRK-02 · Flow §9
- **Notes:** An **Admin** may manage sessions for **any** track; a **Board** may only manage sessions for their supervised track (D:Q13). The API enforces this via `ITrackScopedRequest` for Board requests; Admin bypasses the track-scope check (SD §9.5). All session write rules (US-BRD-SES-01/02/03/04) apply identically to the Admin — the only difference is scope.

### US-MEM-SES-01
*As a **Member@T**, I want to view my track's upcoming and past sessions, so that I know where to be and what I missed.*
- **Traces:** SES-04 · FR-TRK-03 · Flow §8
- **Notes:** A Member sees sessions **only for their own track**.

---

## 15. Attendance (PRD §6.9 · SRS §3.11 · Flow §9)

### US-BRD-01
*As a **Board@T**, I want to record each member's attendance for a session as Present, Late, or Absent, so that I keep an accurate participation record.*
- **Traces:** ATT-01 · FR-ATT-01, FR-ATT-02 · Flow §9
- **Notes:** Attendance is **manual only** (no QR for training in current scope). There is **at most one attendance record per member per session**; re-recording **updates** the existing record (keyed on the enrollment, D:Q11). Board scope restricted to own track.

### US-MEM-01
*As a **Member@T**, I want to view my attendance percentage, so that I know how I'm doing.*
- **Traces:** ATT-03, MDB-04 · FR-ATT-03 · Flow §8
- **Notes:** Percentage = (Present + Late) ÷ **counted sessions**; **Late counts as attended**. **(D:Q12)** The denominator is **only sessions that have occurred AND have a recorded attendance entry for this enrollment** — future sessions are excluded, and a past session with no record for the member is **excluded** (not silently counted absent; an Absent must be **explicitly recorded**). **(D:Q11)** The percentage is scoped to the member's **current active enrollment**; prior enrollments don't dilute it.

### US-MEM-05
*As a **Member@T**, I want to view my detailed attendance log (session-by-session breakdown with Present/Late/Absent status and dates), so that I know exactly which sessions I attended or missed.*
- **Traces:** MDB-04 · FR-ATT-03 · DataModel §3.4 · D:Q11, Q12
- **Notes:** Scoped to **current active enrollment** (D:Q11). Shows all sessions with a recorded entry; sessions with no recorded entry are excluded (never inferred as Absent — D:Q12). Ordered by session date, newest first. Board's `RecordedBy` stamp visible for transparency.

### US-BRD-02
*As a **Board@T**, I want to view attendance for all members of my track, so that I can spot who's falling behind.*
- **Traces:** ATT-04 · FR-ATT-04 · Flow §9
- **Notes:** Board scope restricted to own track.

### US-BRD-07
*As a **Board@T**, I want to view a paginated roster of my track's active members with each member's current attendance percentage and latest evaluation score, so that I can quickly identify who needs attention.*
- **Traces:** BDB-02 · FR-ATT-04, FR-EVL-04 · DataModel §3.2, §3.4, §3.5 · D:Q11, Q26
- **Notes:** Only **active** enrollments shown (`EndedAtUtc IS NULL`). Board sees **own track only** (D:Q13). Attendance % computed per D:Q12. Board scope enforced via `AuthorizationBehavior` (D:Q35). Paginated with `meta` (D:Q26).

### US-ADM-ATT-01
*As an **Admin**, I want to view attendance across all tracks, so that I can oversee the whole program.*
- **Traces:** ATT-05 · FR-ATT-04 · Flow §7
- **Notes:** Priority P1.

---

## 16. Evaluations (PRD §6.10 · SRS §3.12 · Flow §9)

> **Coverage note (2026-07-25):** 4 stories cover all **5** EVL features — `US-BRD-03` traces **both EVL-01 (score) and EVL-05 (feedback)** in a single story (they're one user action). No feature is unmapped; this is not a gap.


### US-BRD-03
*As a **Board@T**, I want to evaluate a member after a session with a score and optional feedback, so that I can track their development.*
- **Traces:** EVL-01, EVL-05 · FR-EVL-01, FR-EVL-02 · Flow §9
- **Notes:** **(D:Q17)** Score is an **integer 0–100 inclusive** (reject <0, >100, non-integer); feedback text is optional. **(D:Q16)** Evaluation is permitted only when the **session date is in the past** and the member has an **active enrollment** at evaluation time; **attendance is not a prerequisite** (evaluation and attendance are independent). There is **at most one evaluation per member per session**, editable in place. Board scope restricted to own track.

### US-BRD-04
*As a **Board@T**, I want to edit an evaluation I entered, so that I can correct a score or refine feedback.*
- **Traces:** EVL-02 · FR-EVL-02 · Flow §9
- **Notes:** **(D:Q17)** Editing **overwrites in place** with **audit columns** (who/when last modified, NFR-SEC-09); there is **no separate version-history table**.

### US-MEM-02
*As a **Member@T**, I want to view my own evaluation history (scores + feedback), so that I can learn from it privately.*
- **Traces:** EVL-03, MDB-03 · FR-EVL-03 · Flow §8
- **Notes:** A Member **MUST NOT see any other member's evaluations**. Evaluations from a **prior ended enrollment are retained** and viewable as history (D:Q11).

### US-BRD-05
*As a **Board@T**, I want to view evaluations for all members of my track, so that I can compare progress.*
- **Traces:** EVL-04 · FR-EVL-04 · Flow §9
- **Notes:** Board scope restricted to own track.

---

## 17. Member & Board Dashboards (PRD §6.11–6.12 · SRS §3.10–3.12 · Flow §8–9)

### US-MEM-03
*As a **Member@T**, I want a dashboard summarizing my attendance %, latest evaluations, and upcoming sessions, so that I have one place to check my standing.*
- **Traces:** MDB-01, MDB-02 · FR-ATT-03, FR-EVL-03, FR-TRK-03 · Flow §8
- **Notes:** Aggregates US-MEM-01, US-MEM-02, US-MEM-SES-01 for the member's current enrollment.

### US-MEM-04
*As a **Member@T** who is also an **Attendee**, I want my training view kept separate from my ticket-buying, so that the two concerns don't bleed together.*
- **Traces:** MDB-01 · (cross-cutting) · Flow §8, §3
- **Notes:** Ticketing and training are independent; a Member uses the full Attendee booking flow (§9–12) unchanged (Persona: Salma).

### US-BRD-06
*As a **Board@T** (possibly also a Member@X), I want a supervisory dashboard for my track and a clean switch into my own Member view, so that my two roles never bleed together.*
- **Traces:** BDB-01, BDB-02 · FR-TRK-04 · Flow §9, §8
- **Notes:** Board dashboard summarizes assigned track, member count, and attendance averages. **(Persona: Yousef, D via FR-ROLE-04)** The Board@Y powers and the Member@X data are hard-separated; acting on any track other than Y returns **403** even though the caller is a Member of X.

---

## 18. Notifications (PRD §6.13 · SRS §3.13 · Flow §11)

### US-NTF-01
*As an **Admin**, I want to send platform-wide or role-scoped in-app notifications, so that I can reach the right audience.*
- **Traces:** NTF-01 · FR-NTF-01 · Flow §11
- **Notes:** **(D:Q21)** Audience options: **platform-wide** (all active users), **by global role** (all Attendees / all Admins), or **by track**. Recipients are **resolved and fanned out to per-recipient rows at send time (snapshot)** — later enrollees do not retroactively receive past notifications. If the chosen audience resolves to **zero recipients** (e.g. a track with no active members), the send is **rejected with `NO_RECIPIENTS_RESOLVED` (422)** and **no `Notification` row is created** — the rejection is atomic (DataModel §4.1, D:Q-ERD2). On success, the response includes `recipientsCreated` count (DataModel §4.1). Acceptance tests must verify zero `Notification` rows exist after a zero-audience send.

### US-NTF-02
*As a **Board@T**, I want to send an in-app notification to my track's members, so that I can reach just my group.*
- **Traces:** NTF-02 · FR-NTF-02 · Flow §11
- **Notes:** **(D:Q21)** Targets the **track's current active members only**, fanned out at send time. Board scope restricted to own track. A track with **no active members** is rejected with **`NO_RECIPIENTS_RESOLVED` (422)** — no `Notification` row is created (D:Q-ERD2).

### US-NTF-03
*As any authenticated user, I want an inbox of my notifications with unread state, and to mark them read, so that I can keep track of what I've seen.*
- **Traces:** NTF-03, NTF-04 · FR-NTF-03 · Flow §11
- **Notes:** **(D:Q21)** Each recipient has **their own read state** (per-recipient row). Notifications are **in-app only** in current scope (no email/SMS/push beyond the password-reset email — D:Q28c / PRD ANTF-01/02).

---

## 19. Public Pages & Contact (PRD §6.5 · SRS §3.14 · Flow §2, §12)

### US-PUB-01
*As a **Visitor**, I want to view the public pages (Home, About, Team, Events, Event Detail, Contact) and the auth pages, so that I can learn about TEDxAlkawmia and decide whether to join.*
- **Traces:** PUB-01…PUB-09 · FR-PUB-01, FR-PUB-04 · Flow §2
- **Notes:** Pages are **responsive and mobile-friendly**. Team and Partners/Sponsors content is **static** in current scope (no admin editing, no dedicated tables, FR-PUB-03). The Home Page **dynamically renders the nearest N upcoming Published events** (same dataset as the events list, D:Q23) — it is not purely static. Static content (About, Team, Partners) is provided by the TEDxAlkawmia team and rendered without an admin CMS.

### US-PUB-02
*As a **Visitor**, I want to submit a contact form without an account, so that I can ask a question.*
- **Traces:** PUB-06 · FR-PUB-02 · Flow §12
- **Notes:** **(D:Q20)** Fields: name, email, subject, message. This is the **only unauthenticated write**; it is **rate-limited by IP**, with **input length caps** (subject ≤ 200, message ≤ 2000) and email-format validation. **No CAPTCHA** in current scope. Submissions are stored with status **New/Read/Archived**.

### US-PUB-03
*As a **Visitor**, I want a clear "Login to book" prompt on events, so that I know how to proceed to a purchase.*
- **Traces:** PUB-05 · FR-EVT-05 · Flow §2
- **Notes:** The public surface is read-only; booking requires authentication (Persona: Omar).

### US-ADM-CON-01
*As an **Admin**, I want to review, read, and archive contact submissions, so that I can respond to inquiries.*
- **Traces:** PUB-06 · FR-PUB-02 · Flow §12
- **Notes:** **(D:Q20)** Admin-only visibility; submissions listed with New/Read/Archived status. Opening a contact message automatically transitions its status from `New → Read` (DataModel §4.3); `UpdatedAtUtc` and `UpdatedBy` are stamped by the audit interceptor on status change. The Admin may manually set `Read → Archived`. **No in-app reply** in current scope (Admin replies via their own email client); no notification to Board/Member.

---

## 20. Reports & Analytics (PRD §6.16 · SRS §7 · Flow §6)

> **(D:Q28c)** Reports `RPT-01/02/03` get stories + read endpoints. **CSV/PDF export (`RPT-04`) is a `?format=csv|pdf` parameter** on those report endpoints, not a separate feature.

### US-ADM-DASH-01
*As an **Admin**, I want a dashboard overview with summary cards, so that I can see the platform's state at a glance.*
- **Traces:** ADM-01 · (SRS §3, cross-cutting) · Flow §6
- **Notes:** Read-only aggregate for the admin landing page — counts derivable from the per-domain endpoints (total/active users, total/published events, tickets sold, checked-in today, open tracks, new contact submissions). Dashboard includes a count/list of tracks **flagged as needing a new Board** (where the active Board assignment was ended by deactivation or explicit removal — D:Q10); clicking a flagged track navigates to the track detail for re-assignment. Each figure reflects committed data (not cached for money decisions, NFR-PERF-05). Backs `GET /admin/dashboard`. Priority P0.

### US-ADM-RPT-01
*As an **Admin**, I want event reports (registration counts, attendance/check-in rates per event), so that I can measure event performance.*
- **Traces:** RPT-01 · (SRS §7) · Flow §6
- **Notes:** Check-in rate uses the Ticket states (Issued/CheckedIn); no-show is **derived** (Issued ∧ event date past, D:Q7). Exportable via `?format=csv|pdf`.

### US-ADM-RPT-02
*As an **Admin**, I want track reports (member progress, attendance, evaluation averages), so that I can oversee training outcomes.*
- **Traces:** RPT-02 · (SRS §7) · Flow §6
- **Notes:** Aggregates per current enrollment (D:Q11). Exportable via `?format=csv|pdf`.

### US-ADM-RPT-03
*As an **Admin**, I want financial reports (revenue per event, payment summaries), so that I can reconcile money.*
- **Traces:** RPT-03 · (SRS §7), FR-PAY-05 · Flow §6
- **Notes:** Revenue counts **Paid** orders using `PaidAtUtc` (the write-once revenue-recognition timestamp, never the mutable `UpdatedAtUtc`) for date-ranged queries (D:Q55, DataModel §2.3). Revenue summary includes breakdown by `UnitType` (Individual vs. Package, DataModel §2.3). Paid orders with a matching `RefundEntry` are categorized as 'Refunded', not 'Revenue'; date range filter uses `PaidAtUtc` for revenue and `CancelledAtUtc` for refunds. Voided/refunded orders are identified by joining `RefundEntry` — both voided-paid and user-cancelled-unpaid orders land in `Cancelled` status, so status alone is insufficient (D:Q6, DataModel §2.3 Issue 7). Prices are snapshotted at reserve (D:Q4, FR-ORD-04). Priority P1. Exportable via `?format=csv|pdf` (D:Q28c).

---

## Out of scope for this document (D:Q28c)

The following PRD items are **not** covered by these user stories, acceptance criteria, or the API contract in current scope:

- Mobile application (MOB-01, MOB-02)
- Real-time notifications via SignalR (ANTF-01)
- Email/SMS/push notifications beyond the password-reset email (ANTF-02, ANTF-03, ANTF-04)
- Automated gateway refunds (PAY-01) — refunds are manual/offline
- Additional payment channels: installments, saved cards, extra wallets (PAY-02)
- Financial reconciliation against gateway settlements (PAY-03)
- Downloadable receipts/invoices per order (PAY-04) — distinct from report export
- Public-facing analytics
- Redis caching and Redis-based rate limiting (CACHE-01…03) — in-memory only in current scope

---

## Story index — Training, Notifications, Public, Reports

| ID | Title | Primary role |
|----|-------|--------------|
| US-ADM-TRK-01 | Create training track | Admin |
| US-ADM-TRK-02 | Soft-delete track with impact | Admin |
| US-ADM-TRK-03 | Edit track details | Admin |
| US-ADM-TRK-04 | List all tracks with filters | Admin |
| US-ADM-TRK-05 | View all enrollments for a track | Admin |
| US-BRD-TRK-01 | View track detail | Board/Admin |
| US-BRD-SES-01 | Create session | Board/Admin |
| US-BRD-SES-02 | Edit session | Board/Admin |
| US-BRD-SES-03 | Delete session (records-free only) | Board/Admin |
| US-BRD-SES-04 | Transition session status (Held/Cancelled) | Board/Admin |
| US-ADM-SES-01 | Manage sessions across any track | Admin |
| US-MEM-SES-01 | View my track's sessions | Member |
| US-BRD-01 | Record attendance | Board |
| US-MEM-01 | View my attendance % | Member |
| US-MEM-05 | View detailed attendance log | Member |
| US-BRD-02 | View track attendance | Board |
| US-BRD-07 | Member roster with attendance % and eval score | Board |
| US-ADM-ATT-01 | Cross-track attendance | Admin |
| US-BRD-03 | Evaluate member | Board |
| US-BRD-04 | Edit evaluation | Board |
| US-MEM-02 | View my evaluations | Member |
| US-BRD-05 | View track evaluations | Board |
| US-MEM-03 | Member dashboard | Member |
| US-MEM-04 | Training/ticketing separation | Member |
| US-BRD-06 | Board dashboard + role switch | Board |
| US-NTF-01 | Admin send notification | Admin |
| US-NTF-02 | Board send notification | Board |
| US-NTF-03 | Notification inbox + read state | All |
| US-PUB-01 | View public pages | Visitor |
| US-PUB-02 | Submit contact form | Visitor |
| US-PUB-03 | Login-to-book prompt | Visitor |
| US-ADM-CON-01 | Review contact submissions | Admin |
| US-ADM-DASH-01 | Admin dashboard overview | Admin |
| US-ADM-RPT-01 | Event reports | Admin |
| US-ADM-RPT-02 | Track reports | Admin |
| US-ADM-RPT-03 | Financial reports | Admin |
