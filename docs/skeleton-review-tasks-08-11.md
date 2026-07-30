# Skeleton Review — Core Persistence Tasks T-S0-08 → T-S0-11

**Reviewer:** Claude · **Date:** 2026-07-30 · **Branch:** `dev`
**Scope:** The four interdependent skeleton tasks that form the persistence core:

| Task | Title | Traces |
|------|-------|--------|
| T-S0-08 | Cross-cutting persistence contracts (`IAuditable`, `ISoftDeletable`, `IConcurrent`, `IClock`, interceptor, query filter) | DM §0/§0.1 › D:Q54 |
| T-S0-09 | Create ALL domain entities (Identity, Ticketing, Training+Comms) + enums + state-machine methods | DM §1–5 › D:Q1, Q46–Q55 |
| T-S0-10 | `IApplicationDbContext` + `ApplicationDbContext : IdentityDbContext<…>` + DI wiring | SD §4.1 › DM §0 › D:Q29b, Q31 |
| T-S0-11 | All `IEntityTypeConfiguration<T>` + initial migration + idempotent Admin seeder | DM §1–6,§11 › D:Q42,Q46,Q51,Q54 › NFR-SEC-08 |

> **Standing instruction honored:** this is a *review only*. No code was changed. Fixes are listed as recommendations for you to approve.

---

## Severity legend

| Level | Meaning |
|-------|---------|
| 🔴 CRITICAL | Wrong at the data/spec-contract level. Will corrupt data, break the model build the moment a migration is added, or violate a frozen invariant. Must fix before any feature work. |
| 🟠 HIGH | Breaks a task's Definition of Done or a D:Qxx decision; feature code built on top will be wrong. |
| 🟡 MEDIUM | Works but diverges from spec/conventions; will cause friction or rework. |
| ⚪ LOW | Cosmetic, naming, hygiene. |

## Headline

The solution **compiles (0 errors, 118 CS8618 warnings)** but that is misleading. The four tasks are the foundation everything else sits on, and **none of the four meets its Definition of Done.** The build is green only because there are **no migrations** and **the DbContext is never instantiated**, so every model-build-time defect (duplicate FKs, config-class-used-as-entity, `ToTable` collision, get-only DbSets) is *latent* — it will detonate the first time someone runs `Add-Migration`.

The single most damaging class of error: **the frozen enum integer values (DM §10) are almost entirely wrong.** Those ints are a permanent data contract. If any data is written before they are corrected, it is mislabeled forever.

**Recommendation: treat T-S0-08…11 as not-done and rework them as a unit before building anything on the DbContext.**

---

## Scoreboard

| Task | Verdict | 🔴 | 🟠 | 🟡 |
|------|---------|----|----|----|
| T-S0-08 Persistence contracts | ⚠️ Partial | 1 | 2 | 2 |
| T-S0-09 Domain entities + enums | ❌ Not met | 3 | 3 | 2 |
| T-S0-10 DbContext + DI | ❌ Not met | 2 | 2 | 1 |
| T-S0-11 EF configs + migration + seeder | ❌ Not met | 4 | 2 | 2 |

---

# 🔴 CRITICAL findings

## C-1 — Frozen enum integer values are wrong across the board (T-S0-09)

DM §10 freezes the *int* wire values ("Never renumber — append only"). The DB stores these via `.HasConversion<int>()`. Almost every enum in `TEDx.Domain` disagrees with the spec. Because the int is the permanent contract, any row written under the current values is **permanently mislabeled**.

| Enum file | Current (wrong) | Spec §10 (required, in order = int value) |
|-----------|-----------------|-------------------------------------------|
| `Ticketing/Enums/OrderStatus.cs` | `Pending, Processing, Shipped, Delivered, Cancelled` | `PendingPayment=0, Paid=1, Cancelled=2, Expired=3` |
| `EventStatus.cs` | `Active, Cancelled, Ended, Voided` | `Draft=0, Published=1, Archived=2, Cancelled=3` |
| `TicketsStatus.cs` | `Active, CheckedIn, Cancelled, Refunded, Voided` | `Issued=0, CheckedIn=1, Voided=2` |
| `OrderUnitType.cs` | `Ticketing, Training` | `Individual=0, Package=1` |
| `DiscountType.cs` | **empty body** | `Percentage=0, FixedAmount=1` |
| `PromoRedemptionStatus.cs` | `Active, Expired, Used, NotValid` | `Claimed=0, Confirmed=1, Released=2` |
| `PayementStatus.cs` | `pending`(lowercase), `Succeeded, Failed, Cancelled, Refunded` | `Initiated=0, Succeeded=1, Failed=2` |
| `Training/Enums/SessionStatus.cs` | `public class SessionStatus {}` — **not an enum, empty** | `Scheduled=0, Held=1, Cancelled=2` |
| `TrackRole.cs` | `Board=0, Member=1` — **reversed** | `Member=0, Board=1` |
| `AttendeceStatus.cs` | `Absent, Present, Late, Excused` (typo name) | `AttendanceStatus`: `Present=0, Late=1, Absent=2` |
| `ReasonRevoke.cs` | `Logout, Replaced, Expired, SecurityIssue, AdminRevoked` | `ReasonRevoked`: `Rotated=0, Reuse=1, Logout=2, Expired=3` |
| `ContactStatus.cs` | `New, InProgress, Resolved, Closed` | `New=0, Read=1, Archived=2` |
| `NotificationAudienceType.cs` | `All, Role, Track, SpecificUsers` | `PlatformWide=0, GlobalRole=1, Track=2` |

Two of these are especially dangerous:
- **`TrackRole` is reversed** (`Board=0, Member=1`). The dual-role filtered unique indexes (Member@X + Board@Y) key off this value; reversed, every role-scoped query and every filtered index is inverted.
- **`SessionStatus` is a `class`, not an `enum`** — which is why its EF configuration was commented out. It cannot be stored at all as written.

**Fix:** rewrite every enum to the exact members *and order* of DM §10 (explicit `= 0,1,2…` to make the freeze visible). Rename `AttendeceStatus`→`AttendanceStatus`, `ReasonRevoke`→`ReasonRevoked`, `PayementStatus`→`PaymentStatus`, `SessionStatus` class→enum. This must land before *any* row is ever written.

## C-2 — No state-machine transition methods on aggregates (T-S0-09, D:Q55)

D:Q55 requires status to change **only** through explicit named aggregate methods that throw on illegal transitions (`Order.MarkAsPaid/Cancel/Expire`, `Ticket.CheckIn/Void`, `Event.Publish/Cancel/Archive`). `Order.cs`, `Event.cs`, `Tickets.cs` expose every field as `public { get; set; }` with **zero** transition methods. `Event.cs` (verified) is a bare property bag. This is a core invariant of the domain and it is entirely absent.

**Fix:** make status setters `private`; add the named transition methods; throw a domain exception on illegal source→target transitions.

## C-3 — `ApplicationDbContext` extends plain `DbContext`, not `IdentityDbContext` (T-S0-10, D:Q46)

`sealed class AppDbContext : DbContext, IAppDbContext`. T-S0-10 / D:Q46 require `ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>` (with `AddIdentityCore`, no roles table). As written, ASP.NET Identity's user store has no backing schema and Identity cannot function. Compounding this, the `DbSet`s are declared **get-only with no `= null!` and no setter**, so EF never populates them → `NullReferenceException` on first use.

**Fix:** derive from `IdentityDbContext<ApplicationUser,…>`; declare `public DbSet<T> X => Set<T>();` (or `{ get; set; } = null!;`); call `base.OnModelCreating(builder)` first in `OnModelCreating`.

## C-4 — `ApplicationUserConfiguration` maps users to the `Events` table (T-S0-11)

`ApplicationUserConfiguration.Configure` opens with `builder.ToTable("Events")`. `ApplicationUser` is thereby mapped onto the `Events` table, colliding with the real `Event` entity. This alone breaks the model build. Also in this file: `builder.Property(x => x.Role).HasDefaultValue("Attendee")` sets a **string** default on an enum property with **no `.HasConversion<int>()`** — so the global role isn't stored as its frozen int either.

**Fix:** `ToTable("Users")` (or the agreed Identity table name); `Role` → `.HasConversion<int>().HasDefaultValue(GlobalRole.Attendee)`.

## C-5 — Duplicate FK on `Tickets.EventId` (T-S0-11)

`TicketsConfiguration` lines 43–47 map **two** relationships onto the same column:
```csharp
builder.HasOne<Event>().WithMany().HasForeignKey(x => x.EventId)…;
builder.HasOne<Order>().WithMany().HasForeignKey(x => x.EventId)…;  // should be x.OrderId
```
The Order relationship points at `EventId` instead of `OrderId`. Two FKs on one column → model-build failure the moment a migration is generated.

**Fix:** second relationship → `.HasForeignKey(x => x.OrderId)`.

## C-6 — Configuration classes used as entity type arguments (T-S0-11)

`AttendanceConfiguration` (verified) does:
```csharp
builder.HasOne<TrackAssignmentConfiguration>().WithMany()
       .HasForeignKey(x => x.SessionId)…;   // wrong type AND wrong key
```
`TrackAssignmentConfiguration` is an EF *configuration class*, not an entity — EF will try to model it as an entity. The FK is also on `SessionId` rather than the enrollment key (`TrackAssignmentId`). The same pattern is expected in `EvaluationConfiguration`. Model-build failure.

**Fix:** reference the **entity** (`HasOne<TrackAssignment>()`) and the correct FK column.

---

# 🟠 HIGH findings

## H-1 — Interceptor never wired; no `ICurrentUser`/`IClock` implementation (T-S0-08, T-S0-10)

`AuditInterceptor` exists but:
- It is **not added** to the context options (no `.AddInterceptors(...)` — the context isn't even constructed with options).
- `CurrentUser.cs` is **entirely commented out**, so `ICurrentUser` has **no live implementation** — yet the interceptor depends on it. Nothing can inject it.

Net effect: even once the context is fixed, audit stamping does nothing.

**Fix:** implement `CurrentUser : ICurrentUser` (reads `NameIdentifier` from `IHttpContextAccessor`); implement `SystemClock : IClock`; register both; wire the interceptor in `AddDbContext`.

## H-2 — Interceptor does not stamp soft-delete (T-S0-08, D:Q54)

`AuditInterceptor` stamps `IAuditable` on `Added`/`Modified` only. DM §0.1 also requires soft-delete handling: an entity marked `IsDeleted = true` should be converted `Deleted → Modified` with `DeletedAtUtc` set, so the row is retained. As written, a delete on an `ISoftDelete` entity issues a hard SQL `DELETE`.

**Fix:** in the interceptor, detect `ISoftDelete` entries in `Deleted` state, flip to `Modified`, set `IsDeleted`/`DeletedAtUtc`.

## H-3 — Infrastructure DI registers almost nothing (T-S0-10, T-S0-11)

`InfrastructureServiceExtensions.AddInfrastructureServices` (verified) registers only `AddTedxOptions` + `IPaymobClient`. Missing: `AddDbContext<ApplicationDbContext>` (+ `IApplicationDbContext` → same instance), `AddIdentityCore`, the interceptor, `ICurrentUser`, `IClock`, and the seeder. The persistence layer is effectively not registered.

**Fix:** add all of the above to the extension.

## H-4 — Cross-context navigation properties present (T-S0-09, D:Q51)

D:Q51 (revised): real DB FKs across contexts with `DeleteBehavior.Restrict` but **no navigation properties across contexts** — reference `AccountId`/`TrackId` by value only. `Event.cs` (verified) carries `List<Order>`, `List<Tickets>`, `List<PromoCodes>`, `List<Packages>` collection navs. These are *within* the Ticketing context so are arguably allowed, but the same pattern must be audited entity-by-entity to ensure no Ticketing↔Identity or Training↔Identity nav props exist (e.g. `Order.Account`, `TrackAssignment.Account`).

**Fix:** remove any cross-context nav; keep the FK value column only. Confirm intra-context navs are intentional.

## H-5 — No initial migration; no idempotent Admin seeder (T-S0-11, D:Q42, NFR-SEC-08)

T-S0-11 requires an initial EF migration plus an idempotent Admin seeder (email from config, password from an **env secret** — never hardcoded). Neither exists. (This is also *why* the model-build defects C-3…C-6 stay hidden.)

**Fix:** only after C-1…C-6 + H-1…H-4 are resolved, generate the migration and add a `dotnet run`-safe idempotent seeder reading `ADMIN_EMAIL` from config and the password from an environment secret.

---

# 🟡 MEDIUM findings

## M-1 — Fragmented per-entity audit interfaces (T-S0-08)

Alongside the correct `IAuditable`/`ISoftDelete`/`IConcurrent`, the domain has `IContactAudit`, `IPayementAudit`, `IRefreshTokenAudit`. `RefundEntry` wrongly implements `IRefreshTokenAudit`. DM §0.1 defines **one** matrix of three cross-cutting contracts; per-entity variants defeat the single interceptor.

**Fix:** delete the fragmented interfaces; have each entity implement the three canonical ones per the §0.1 matrix.

## M-2 — Nullable/typo mismatches on `Order` (T-S0-09)

`Order.cs`: `PackageId`/`PromoCodeId` are non-nullable `Guid` (spec: nullable — an individual-ticket order has no package, an order without a promo has no code); field typos `UnitNAmeSnapshot`, `PAidAtUtc`.

**Fix:** make both nullable; fix casing.

## M-3 — 118 CS8618 non-nullable warnings (T-S0-09)

Every entity has uninitialized non-nullable reference properties. Not fatal, but noise that hides real issues and signals missing `required`/`= null!`/constructor discipline.

**Fix:** apply `required` or `= null!` / sensible defaults consistently.

## M-4 — `HasDefaultValue` on enum stored via conversion (T-S0-11)

Defaults like `TicketsStatus.Active` are set but (a) the value name is wrong per C-1 and (b) some are set without the matching `.HasConversion<int>()`. Re-verify each once enums are corrected.

---

# ⚪ LOW findings

- **L-1** Entity/type name typos: `Attendence`, `NotificationRecepient`, `Sessions`, `PromoCodes`, `Packages`, `PayementStatus`, `ReasonRevoke`, `AttendeceStatus`. Rename to spec spelling before code is built on top.
- **L-2** `Event.eventStatus` uses camelCase for a public property; convention is `Status` (PascalCase).
- **L-3** Inline `// nn 200`, `// Check` comments in entities should move into the config layer or XML docs; DM §0.1 matrix is not documented in code as T-S0-08 asked.
- **L-4** Commented-out relationship blocks in `ApplicationUserConfiguration` (PromoRedemption/TrackAssignment) should be implemented or removed, not left dangling.

---

# Per-task Definition of Done

### T-S0-08 — Persistence contracts ⚠️ Partial
- ✅ `IAuditable`, `ISoftDelete`, `IConcurrent` shapes correct; `AuditInterceptor` structure reasonable.
- ❌ Interceptor not wired (H-1); no soft-delete stamping (H-2); no `IClock`/`ICurrentUser` impl (H-1); fragmented extra interfaces (M-1); §0.1 matrix not documented in code (L-3).

### T-S0-09 — Domain entities + enums ❌ Not met
- ✅ Entities exist and compile; no EF dependency in Domain.
- ❌ Frozen enum values wrong (C-1); no state-machine methods (C-2); cross-context nav audit needed (H-4); nullability/typos (M-2, L-1).

### T-S0-10 — DbContext + DI ❌ Not met
- ✅ `IApplicationDbContext` exists; single-context intent (D:Q29b) honored in shape.
- ❌ Extends plain `DbContext` not `IdentityDbContext` (C-3); get-only DbSets (C-3); not registered in DI (H-3); interceptor not wired (H-1).

### T-S0-11 — EF configs + migration + seeder ❌ Not met
- ✅ Most configs exist with sensible property mappings, indexes, `IsRowVersion`, `Restrict` deletes.
- ❌ `ToTable("Events")` on user (C-4); duplicate FK (C-5); config-class-as-entity (C-6); enum default/conversion gaps (M-4); **no migration, no seeder** (H-5).

---

# Recommended fix order

Do these as one unit, top-down — later steps depend on earlier ones, and **nothing should touch the database until step 6.**

1. **Enums (C-1)** — rewrite all to DM §10 exact members + explicit int values; rename mistyped enums; convert `SessionStatus` class→enum. *Foundation for everything.*
2. **Entities (C-2, H-4, M-1, M-2)** — add state-machine methods + private setters; make `Order.PackageId`/`PromoCodeId` nullable; collapse to the three canonical audit interfaces; audit cross-context navs.
3. **DbContext (C-3)** — derive from `IdentityDbContext<ApplicationUser,…>`; fix DbSet declarations; `base.OnModelCreating` first.
4. **Interceptor + clock/user (H-1, H-2)** — implement `CurrentUser`, `SystemClock`; add soft-delete stamping.
5. **Configs (C-4, C-5, C-6, M-4)** — fix `ToTable`, duplicate/typed FKs, enum conversions & defaults; implement the commented-out relationships.
6. **DI + migration + seeder (H-3, H-5)** — register everything; generate the initial migration (this validates 1–5 at model-build time); add the idempotent Admin seeder (config email + env-secret password).
7. **Hygiene (M-3, L-1…L-4)** — resolve CS8618, rename typo'd types, move inline comments, document the §0.1 matrix in code.

> After step 6, `Add-Migration` succeeding is the real proof the core is sound — the current green build is not.

