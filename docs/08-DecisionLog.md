# TEDxAlkawmia Platform — Design Decision Log

> **Version:** 1.3
> **Date:** 2026-07-24
> **Status:** Accepted — basis for [05 — User Stories](./05-UserStories.md), [06 — Acceptance Criteria](./06-AcceptanceCriteria.md), [07 — API Contract](./07-ApiContract.md), [09 — System Design](./09-SystemDesign.md), [10 — Data Model](./10-DataModel.md), [11 — State Machines](./11-StateMachines.md), and [12 — Sequence Diagrams](./12-SequenceDiagrams.md)
> **References:** [01 — PRD](./01-PRD.md) · [02 — SRS](./02-SRS.md) (v1.5) · [03 — User Flows](./03-UserFlows.md) · [04 — Personas](./04-Personas.md)

---

## Purpose

This document records the design questions raised during the grilling sessions and the decision reached on each. It has two parts:

- **Q1–Q28 — Requirements grilling (2026-07-20).** The PRD, SRS, and User Flows left these points ambiguous or under-specified; each was resolved before writing the User Stories, Acceptance Criteria, and API Contract. Decisions are cited across those documents as **(D:Qn)**.
- **Q29–Q55 — Architecture grilling (2026-07-22).** Application, infrastructure, and data-model decisions locked *before* writing the `architecture/` doc set (System Design 09, Data Model 10). These are the authoritative basis for that doc set and for the code scaffold that follows. Cited as **(D:Qn)** the same way.

**Governing philosophy for Q29–Q55:** every architectural decision is kept **proportional to a problem this project actually has** — a modular monolith on a single SQL Server, built by a two-developer team, not a future microservice/enterprise-scale system. Enterprise patterns are adopted only where they solve a concrete problem here; they are declined where their only payoff is a hypothetical we have decided not to build toward.

Each entry records:
- **Question** — the ambiguity or gap.
- **Decision** — what we chose.
- **Rationale / notes** — why, plus the requirement IDs it touches.

Where the decision resolves a conflict in an existing document, that is called out explicitly.

**Legend:** FR/NFR IDs refer to [02 — SRS](./02-SRS.md); feature IDs (ORD-xx, EVT-xx…) to [01 — PRD](./01-PRD.md).

---

## Section A — Ordering & Seat Holds

### Q1 — Order composition
**Question:** The SRS glossary defines an Order as "a purchase of one or more ticket packages," but `FR-ORD-02` implies a single package. Which is authoritative — a multi-package cart, or one package per order?
**Decision:** An order is a **single package type × quantity** (e.g. 2 × "Group-5"). To buy different package types, an Attendee places **separate orders**. There is no multi-package cart.
**Rationale:** `FR-ORD-02` (one package per order) wins; the glossary wording was the error. Keeps the reserve → pay → issue flow atomic per package and simplifies price snapshotting. **SRS glossary corrected to match in v1.1.**

> **Addendum (2026-07-21, Model B) — individual tickets are the base unit; packages are optional.** The prior wording implied a **package is the only purchasable unit**. Corrected: **every event always sells individual tickets at a face price (`event.ticketPrice`); a package is an *optional* discount bundle, never a prerequisite.** An event with **zero packages is fully publishable and sellable** via the individual-ticket flow. Concretely:
> - An order references a **nullable package**: `packageId = null` ⇒ an **individual-ticket order** priced at `event.ticketPrice × quantity`; `packageId` set ⇒ a **package (bundle) order** priced at the package unit price × quantity.
> - **Q1 is preserved:** an order is still **one unit-type × quantity** — an individual-ticket order *or* a single package order, never a mix of singles and a bundle. Different unit-types require separate orders.
> - The individual-ticket quantity cap is an event-level **`MaxIndividualQtyPerOrder`** (nullable = no cap), mirroring the package-level `MaxQuantityPerOrder` from Q2.
> - `event.ticketPrice = 0` ⇒ a **free individual ticket**, which takes the existing gateway-bypass / confirm-free path (Q18/`FR-PAY-06`).
> This addendum is the **authoritative basis** for the Model-B alignment applied to the PRD, SRS (v1.3), User Flows, User Stories, Acceptance Criteria, and API Contract. Touches `FR-EVT-01`, `FR-PKG-01`, `FR-ORD-01`, `FR-ORD-02`, `FR-ORD-04`; supersedes the "publish requires ≥ 1 package" rule wherever it lingered (notably User Flows §6.1).

### Q2 — Quantity cap per order
**Question:** Is there an upper bound on how many of one package a single order may contain?
**Decision:** Each package carries an Admin-configurable **`MaxQuantityPerOrder`** (nullable; `null` = no cap). Validated at **both quote and reserve**.
**Rationale:** Lets Admins limit bulk grabs per package without a global rule; nullable keeps it optional. Touches `FR-PKG-01`, `FR-ORD-02`.

### Q3 — Definition of "held" seats
**Question:** What counts against capacity when computing remaining seats — and does availability depend on the expiry sweeper running?
**Decision:** Held seats = **Paid** orders + **PendingPayment** orders whose **`HoldExpiresAt > now`** (clock-aware, not status-only). A lapsed hold stops counting the instant it expires, independent of the background sweeper. The concurrency-safe (`SERIALIZABLE`) reserve check uses the same clock-aware predicate.
**Rationale:** Availability must be correct even if the sweeper is delayed; the sweeper becomes cleanup, not a correctness dependency. Touches `FR-EVT-07`, `FR-ORD-03`, `FR-ORD-05`, `NFR-REL-01`, `NFR-PERF-05`.

### Q4 — Re-pricing at reserve
**Question:** Is the price shown at quote trusted at reserve, and can the client submit a price?
**Decision:** The server **always re-prices and re-validates** at reserve (live package price + promo state). The **quote is advisory**; the client never sends a price. If the recomputed price differs, the API returns a **`PRICE_CHANGED`** conflict with the new quote for explicit re-confirmation — **never a silent charge**. The order **snapshots** package name, unit price, base, discount, and final at reserve.
**Rationale:** Prevents stale-price and client-tampering charges; the snapshot makes historical orders immutable against later catalog/promo edits. Touches `FR-ORD-04`, `NFR-REL-04`, `NFR-SEC-08`.

### Q5 — Concurrent orders per user
**Question:** May one user hold multiple pending orders for the same event simultaneously?
**Decision:** **At most one active (PendingPayment, unexpired) order per user per event.** Re-reserving returns/points to the existing pending order. **Paid** orders do **not** block a new purchase, and pending orders on **different** events are independent.
**Rationale:** Stops a single user hoarding capacity across abandoned holds while still allowing repeat purchases and multi-event buying. Touches `FR-ORD-02`, `FR-ORD-05`.

### Q6 — Voiding a paid order
**Question:** Who can void a paid order, how are refunds handled, and what happens to already-used tickets?
**Decision:** Voiding a Paid order is **Admin-only**; refunds are **offline/manual** (`FR-PAY-07`). A void releases only seats **not yet checked in**; **checked-in tickets are non-voidable** (the seat stays consumed). Attendees may self-cancel **unpaid** orders only.
**Rationale:** No automated gateway refund in scope; a consumed admission can't be un-consumed. Touches `FR-ORD-06`, `FR-PAY-07`.

---

## Section B — Tickets & Check-in

### Q7 — Ticket states
**Question:** What is the ticket state set, and is "no-show" or "expired" stored?
**Decision:** Ticket states are **Issued / CheckedIn / Voided** only. **No-show is derived** (`Issued ∧ event.date < now`), not stored. There is no `Expired` ticket state.
**Rationale:** Derivable facts aren't persisted; keeps the state machine minimal. Touches `FR-TKT-01`, `FR-TKT-05`.

### Q8 — QR token design
**Question:** What does the QR encode, and how is it validated without storing a forgeable secret?
**Decision:** The QR encodes a **public reference** (indexed, non-secret, e.g. `TKT-7F3A9C`) **+ a 256-bit random secret**. On scan, the server looks up by reference, then compares the presented secret against a stored **SHA-256 hash**. The **raw secret is never persisted**.
**Rationale:** Reference enables fast lookup and human support; the hashed secret prevents forgery even if the DB leaks. Touches `FR-TKT-04`, `NFR-SEC-05`.

### Q9 — Check-in authority & scan outcomes
**Question:** Who may check tickets in, and what are the distinct scan results?
**Decision:** **Admin-only** (per the PRD role matrix). The scan endpoint is **event-scoped**. Five distinct outcomes: **success**, **already-checked-in** (returns who + when), **wrong-event**, **voided** (`TICKET_VOIDED` — a known ticket whose paid order was voided/refunded, distinct from an unknown token), **unknown/invalid**. **All rejections are logged** (`FR-TKT-06`). A delegated-scanner role is deferred.
**Rationale:** Matches the PRD authorization matrix; distinct outcomes give door staff actionable feedback and an audit trail; the voided outcome lets staff distinguish a refund from a forgery. Touches `FR-TKT-05`, `FR-TKT-06`.

---

## Section C — Roles, Users & Training

### Q10 — User deactivation ripple
**Question:** What exactly happens when an account is deactivated?
**Decision:** Deactivation **blocks login/refresh only**. **Issued tickets stay valid** (admission is by QR, not login). Any active **PendingPayment order is cancelled and its seats released**. Track assignments are **ended** (`EndedAt` set — this frees the FR-ROLE-04 dual-role slots; not restored on reactivation), while all history (attendance/evaluations) is retained. Deactivating a **Board** flags the track as **needing a new supervisor** for the Admin — never silently orphaned.

> **Audit refinement (2026-07-20, Issue 6):** the original wording "retained but frozen" was ambiguous about whether the assignment stayed active. Resolved to **ended**: deactivation vacates the Member/Board slots immediately and reactivation does not auto-restore them (re-assign explicitly). Aligns US-MNG-03/04, US-ROLE-06, and the API contract.
**Rationale:** A deactivated buyer's paid admission shouldn't evaporate; training history must survive; supervision gaps must be visible. Touches `FR-AUTH-05`, `FR-ROLE-05`, `NFR-SEC-09`.

### Q11 — Enrollment lifecycle
**Question:** Are attendance/evaluations tied to the user↔track pair or to a specific enrollment, and what happens on re-enrollment?
**Decision:** Attendance and evaluations are **keyed on the enrollment**, not the raw user↔track pair. Removing a member **ends the enrollment** (`EndedAt`, records retained); re-enrolling creates a **new enrollment with a fresh attendance %**. The `FR-ATT-03` percentage is always scoped to the **current active enrollment**.
**Rationale:** Matches the SRS `(session, enrollment)` uniqueness; prevents a prior stint from diluting a new one. Touches `FR-ATT-03`, `FR-ROLE-05`.

### Q12 — Attendance percentage denominator
**Question:** What is the denominator for attendance % — all planned sessions, all past sessions, or something else?
**Decision:** Denominator = sessions that have **occurred AND have a recorded attendance entry** for that enrollment. **Future sessions are excluded**; a **past session with no record is excluded** (an Absent must be **explicitly recorded**, never inferred by omission). **Late counts as attended.**
**Rationale:** The percentage reflects recorded reality, not assumptions; avoids silently penalizing members for un-recorded sessions. Touches `FR-ATT-03`.

### Q13 — Session edit / delete
**Question:** Can a session with attendance/evaluation records be edited or deleted, and is a Board limited to their own track?
**Decision:** A session **with any attendance/evaluation records** can be **edited** (topic/time/location) but **not hard-deleted** — soft-delete/cancel only. A **records-free** session can be removed outright. **Board session writes are restricted to the track they supervise**; cross-track writes are rejected (403).
**Rationale:** Protects training history from destructive edits; enforces the per-track authority boundary. Touches `FR-TRK-02`, `NFR-REL-03`.

### Q14 — Track soft-delete
**Question:** What happens to enrollments and the Board assignment when a track is soft-deleted?
**Decision:** Soft-deleting a track **auto-ends its active Member enrollments and Board assignment** (`EndedAt`, all history retained), **freeing those users for reassignment**. Behind an **Admin confirmation stating the impact** ("ends N enrollments + 1 Board assignment"). This **unblocks** the `FR-ROLE-04` dual-role caps for those users.
**Rationale:** A retired track shouldn't orphan people or block their future roles; the confirmation makes the blast radius explicit. Touches `FR-TRK-01`, `FR-ROLE-04`, `FR-ROLE-05`.

### Q15 — Board enrolling a member
**Question:** Does enrollment create an account, and how are the dual-role constraints enforced at enroll time?
**Decision:** Enrollment adds an **existing Attendee account** (found by email/search) — **no account creation** by enrollment. At enroll time (`FR-ROLE-04`): **reject** if the target is already an active Member of **any** track; **reject** if it would make them **Member and Board of the same track**; **allow** if they are Board of a **different** track (the sanctioned dual-role case). A **Board may only enroll into the track they supervise**. Every rejection carries a clear machine-readable reason.
**Rationale:** Keeps enrollment and registration separate; enforces the one-Member-track / dual-role rules at the point of change. Touches `FR-ROLE-03`, `FR-ROLE-04`.

### Q16 — Evaluation timing
**Question:** When may a Board evaluate a member — must the session have happened, and must attendance be recorded first?
**Decision:** Evaluation requires the **session date to be in the past** and the member to have an **active enrollment at evaluation time**. **Attendance is not a prerequisite** — evaluation and attendance are independent. Existing evaluations for a departed member are retained.
**Rationale:** You can only assess a session that occurred, but a Board may evaluate someone they observed regardless of a marked attendance row. Touches `FR-EVL-01`, `FR-EVL-02`.

### Q17 — Evaluation score range & edits
**Question:** What is the valid score range, and is edit history versioned?
**Decision:** Score is an **integer 0–100 inclusive** (reject `<0`, `>100`, non-integer). Editing **overwrites in place** with audit columns (who/when); **no version-history table**.
**Rationale:** Simple bounded score; audit columns satisfy accountability without the cost of full history. Touches `FR-EVL-01`, `FR-EVL-02`, `NFR-SEC-09`.

---

## Section D — Promotions & Payment Math

### Q18 — Promo math & money handling
**Question:** How is the discount rounded, can the final go negative, and where do piastres enter?
**Decision:** Discount is rounded **half-up to 2 decimals (EGP)**; `final = max(base − discount, 0)` — an over-large discount yields a **free (0.00) order**, which takes the `FR-PAY-06` gateway-bypass path. Money is held internally as **`decimal(18,2)` EGP** and converted to **integer piastres (×100) only at the Paymob boundary**; that piastre amount is what `FR-PAY-04` validates.
**Rationale:** Deterministic rounding, no negative charges, and a single well-defined currency boundary. Touches `FR-PAY-04`, `FR-PAY-06`, `NFR-CMP-03`.

### Q19 — Promo redemption timing
**Question:** At what point is a limited promo's redemption "used up," so caps are never exceeded but abandoned holds don't burn slots?
**Decision:** Promo is **validated at quote** (advisory); its slot is **atomically claimed at payment initiation** (paid orders) or **at confirmation** (free / 100%-off orders); **confirmed on Paid**; **released on payment failure or hold expiry**. Unpaid holds never burn a limited promo, and the cap is never exceeded.
**Rationale:** Balances not over-issuing against not locking codes behind abandoned carts. The `FR-PROMO-04` redemption record is written on confirmation. Touches `FR-PROMO-03`, `FR-PROMO-04`.

---

## Section E — Public & Contact

### Q20 — Contact form
**Question:** Who can submit, how is it abuse-protected, and what happens to a submission?
**Decision:** **Unauthenticated write only.** Protected by **IP rate-limiting + length caps** (subject ≤ 200, message ≤ 2000) + **email-format validation**; **no CAPTCHA** in scope. Stored with status **New / Read / Archived**, **Admin-only visibility**, **no in-app reply**, and no Board/Member notification.
**Rationale:** Low-friction public channel with basic abuse controls; keeps the feature lean. Touches `FR-PUB-02`.

---

## Section F — Notifications

### Q21 — Notification fan-out & audiences
**Question:** Are recipients resolved once at send time or dynamically, and what audiences exist?
**Decision:** Recipients are **resolved and fanned out to per-recipient rows at send time** (a snapshot — later enrollees do **not** retroactively receive past notifications), with **per-row read state** (`FR-NTF-03`). **Audiences:** Admin = platform-wide (all active) | by global role (Attendees / Admins) | by track; **Board = own-track active members only**.
**Rationale:** A snapshot gives stable delivery semantics and per-user read tracking; audience scoping mirrors the role boundaries. Touches `FR-NTF-01`, `FR-NTF-03`.

---

## Section G — Events

### Q22 — Editing an event with sold tickets
**Question:** What can change on an event that already has orders, and what does cancellation do?
**Decision:** Capacity is **raisable anytime**, **lowerable only to ≥ (held + paid) seats** (else rejected — never invalidates sold seats). **Cancel** (→ Cancelled) **voids Issued tickets, releases holds, and records offline refunds** (`FR-PAY-07`); the event is **hidden but retained**. **Soft-delete only when zero orders** exist (otherwise Cancel). Date/location remain **editable and audited**; automatic holder notification on change is **deferred**.
**Rationale:** Protects sold inventory and financial history while still allowing legitimate edits and a clean cancellation path. Touches `FR-EVT-02`, `FR-EVT-03`, `FR-PAY-07`, `NFR-REL-06`.

### Q23 — Event state machine
**Question:** What are the legal event status transitions, and how do "upcoming/past" and "archived" relate?
**Decision:** **Draft ⇄ Published** only while **zero orders** exist; **Published → Archived** or **→ Cancelled**; **Archived → Published**; **Cancelled is terminal.** Public **upcoming/past** is **date-derived among Published events**; **Archived is a manual hide** (kept, off listings); Draft/Cancelled are never public.
**Rationale:** Prevents un-publishing an event that already sold seats; separates the date-based public split from the manual archive action. Touches `FR-EVT-04`.

---

## Section H — API & Cross-Cutting

### Q24 — Token lifetimes & transport
**Question:** What are the access/refresh/reset token lifetimes, rotation policy, and where does the refresh token live?
**Decision (all config-overridable defaults):** **Access JWT 15 min** (claims: account id, email, global role). **Refresh 7 days, single-use, rotated, stored hashed**, with **family-revoke on reuse**. **Reset token 1 hour, single-use.** The refresh token travels in the **JSON body** (uniform for web + future mobile), **not** an httpOnly cookie.
**Rationale:** Short access window limits blast radius; rotation + family-revoke detects theft; body transport keeps web and mobile symmetric. Touches `FR-AUTH-04`, `FR-AUTH-06`, `FR-AUTH-08`, `FR-AUTH-10`, `FR-AUTH-11`, `NFR-SEC-02`.

### Q25 — API response envelope
**Question:** What is the uniform response shape, and how are errors expressed for i18n?
**Decision:** A **Result pattern over HTTP** — every response is `{ success, data, error }`. Error = `{ code, message, fieldErrors? }`, where **`code` is a stable machine string** (e.g. `EMAIL_TAKEN`, `SEATS_UNAVAILABLE`, `PROMO_EXPIRED`, `PRICE_CHANGED`, `TICKET_ALREADY_CHECKED_IN`, `WRONG_EVENT`) the **client maps to i18n**, `message` is the English fallback, and `fieldErrors` appears **only** for validation failures. Correct HTTP status is still used; internals are logged with a `traceId` (may be echoed as `error.traceId`).
**Rationale:** Consistent parsing for the client, localizable errors, and no internal leakage. Touches `NFR-USE-02`, `NFR-MNT-03`.

### Q26 — Pagination, sorting, filtering
**Question:** What pagination style and query conventions do list endpoints use?
**Decision:** **Offset pagination** — `?page=1&pageSize=20&sort=field:dir&<named filters>`. `pageSize` default **20**, cap **100**. List responses add a sibling **`meta { page, pageSize, totalItems, totalPages }`**. Sortable fields are **whitelisted per endpoint** (unknown → rejected); filters are **explicit named params** per endpoint.
**Rationale:** Predictable, cache-friendly, and safe against arbitrary sort/filter injection. Applies across all list endpoints.

### Q27 — Wire formats
**Question:** How are versions, dates, money, IDs, and enums represented on the wire?
**Decision:** **`/api/v1`** (version in URL path). Dates **ISO 8601 UTC with `Z`**. Money = **JSON number, 2 dp, EGP**, paired with a **`currency: "EGP"`** field (always EGP now, i18n-ready); **piastres never exposed**. IDs are **GUID strings**, plus a **separate short human ticket reference**. Enums are **PascalCase strings**, never raw integers.
**Rationale:** Unambiguous, forward-compatible formats; the currency pairing and enum strings keep the API self-describing. Touches `NFR-REL-05`, `NFR-CMP-03`.

### Q28 — Remaining small items
**Question:** Three loose ends — idempotency, rate-limit shape, and the scope boundary of the three documents.
**Decision (all accepted):**
- **28a — Idempotency:** payment initiation accepts an optional **`Idempotency-Key`** header; a repeat with the same key returns the **same checkout session** (no duplicate Paymob intention). Reserve is guarded by the one-pending-order rule (Q5); the webhook is idempotent (`FR-PAY-03`).
- **28b — Rate limiting:** exceeding a limit returns **HTTP 429** with the standard envelope, **`error.code = "RATE_LIMITED"`**, and a **`Retry-After`** header. Limits are config-driven and documented per endpoint group as "SHOULD" targets.
- **28c — Scope boundary:** the three documents cover **current-scope `FR-*`/`NFR-*` only**, with an explicit **"Out of scope"** note listing: mobile app, SignalR real-time, automated gateway refunds, extra payment channels, financial reconciliation, analytics beyond `RPT-*`, and email/SMS beyond the password-reset email. **Reports `RPT-01/02/03`** get full stories + read endpoints; **CSV/PDF export `RPT-04`** is a **`?format=csv|pdf`** query param on those report endpoints, not a separate feature.
**Rationale:** Closes the double-submit and abuse-response gaps and draws a clear, documented line around what these deliverables include. Touches `FR-PAY-03`, `NFR-SEC-10`.

---

# Part 2 — Architecture Grilling (Q29–Q55, 2026-07-22)

> Locked before writing the `architecture/` doc set. Stack context (from prior scope): ASP.NET Core Web API (.NET 8, C#), React + Vite + TypeScript SPA, SQL Server + EF Core; external services Paymob (payments + HMAC webhook), Cloudinary (images), SMTP (password-reset email only). Three bounded contexts: **Identity**, **Eventing/Ticketing**, **Training**.

## Section I — Solution Shape & Application Layer

### Q29 — Overall architecture style
**Decision:** **Modular monolith + Clean Architecture**, four projects: **Domain / Application / Infrastructure / Api**. The three bounded contexts (Identity, Eventing/Ticketing, Training) are **folders inside each layer** (Q29a), not separate assemblies.
**Rationale:** A single deployable with clean internal boundaries fits a two-dev team and a single database; context-as-folder keeps separation without assembly overhead. No microservice split is planned.

### Q29b — Persistence context count
**Decision:** **A single `DbContext`** now. If the contexts ever need physical separation, splitting into three `DbContext`s is a future migration.
**Rationale:** One DB, one team — a single context is the proportional choice; premature splitting buys nothing today.

### Q30 — CQRS & the MediatR pipeline
**Decision:** **MediatR** with commands/queries; **`ValidationBehavior`** and **`LoggingBehavior`** pipeline behaviors; **explicit transactions in handlers**; **CQRS-lite** (queries read the same EF model — no separate read store).
**Rationale:** Behaviors centralize cross-cutting concerns; explicit transactions keep the money path readable; a separate read model would be over-engineering at this scale.

### Q31 — Data-access abstraction
**Decision:** **`IApplicationDbContext`** (the `DbContext` behind an interface); **no generic repositories**; Domain stays **pure POCO**; **targeted aggregate methods** for invariant-heavy writes.
**Rationale:** The interface keeps Application testable and Infrastructure-free without a repository layer that would just wrap EF. Generic repos add indirection with no payoff here.

### Q32 — Domain model richness
**Decision:** **Rich domain model** only for the invariant-bearing aggregates — **Order, Ticket, Event, User + track assignments, and Training write-records where a real invariant exists**; **CRUD-simple** everywhere else. Concurrency invariants enforced at **both** the domain and the database.
**Rationale:** Rich modeling is spent where invariants are real (money, seats, roles); elsewhere it is ceremony. Explicitly *not* a full DDD showcase.

### Q33 — Seat-reservation concurrency
**Decision:** **`SERIALIZABLE` reserve transaction** + **mandatory Polly retry** on deadlock (1205) / serialization failures. Held-seats are **computed, never stored** (consistent with D:Q3, `FR-EVT-07`).
**Rationale:** Serializable + retry is the correct, simplest guarantee against oversell on a single SQL Server; computed seats keep availability correct regardless of the sweeper.

### Q34 — Background work (hold-expiry sweeper)
**Decision:** In-process **`BackgroundService` timer** guarded by **`sp_getapplock`** (single-instance sweep). The sweeper is **cleanup-only** (correctness never depends on it — D:Q3). Hangfire/external scheduler deferred.
**Rationale:** No external scheduler needed for one cleanup job; the app-lock makes it safe even if more than one instance runs.

### Q35 — Authorization mechanism
**Decision:** **MediatR `AuthorizationBehavior`** + **marker interfaces** (`ITrackScopedRequest`, `IRequireAdmin`, …) + an **`ICurrentUser`** abstraction. The **global role** is gated at the controller (`[Authorize]`); the **per-track Member/Board scope is resolved per request** in the pipeline behavior against current DB state (never baked into the JWT). The dual-role legality invariant is enforced separately at the domain + DB (see Q51).
**Rationale:** Per-request track resolution is the only correct approach when assignments can change; keeps authZ out of the token and centralized in one behavior.

## Section J — Infrastructure & Cross-Cutting

### Q36 — Identity / account store
**Decision:** **ASP.NET Core Identity** for the **user/password store, hashing, lockout, and reset-token provider only**; **custom JWT + refresh tokens** for sessions with Identity's **cookie stack disabled**. Global role is a **plain `GlobalRole` column** (2 values), *not* Identity roles. **Member/Board remain first-class relational track assignments**, never Identity roles.
**Rationale:** Reuses Identity's proven crypto without its cookie/role machinery; a 2-value column is lighter than the roles tables; track roles are relational and can't be Identity roles.

### Q37 — Failure signalling & the HTTP envelope
**Decision:** Handlers return a typed **`Result<T>`** carrying either data or a structured `Error { code, message, type }` (`type ∈ Validation | NotFound | Conflict | Business | Unauthorized`). A **single `Result → ActionResult` mapper** translates `type` → HTTP status (`Business/Validation → 422`, `Conflict → 409`, `NotFound → 404`, `Unauthorized → 401/403`). A central **`ExceptionHandlingMiddleware`** handles **only unexpected faults** → 500 + correlationId + Serilog error. A static **`Errors` catalog** holds every `code`+`type`.
**Rationale:** Expected business outcomes (seats gone, price changed, hold expired) are *values*, not exceptions; one mapping table makes the error taxonomy (D:Q25) enforceable and greppable. Implements the Result pattern already chosen in D:Q25.

### Q38 — DTO ↔ entity mapping
**Decision:** **Manual mapping** in the Application layer (explicit `ToDto()` / `ToResponse()`); **no mapper library**.
**Rationale:** Explicit-over-implicit; structurally prevents leaking QR secrets, password hashes, or cross-context fields — nothing maps unless a line is written. Matches the pure-POCO domain (Q31).

### Q39 — Validation placement (three tiers, no overlap)
**Decision:** **Shape/format** → **FluentValidation** in `ValidationBehavior` → `422 VALIDATION_ERROR` + per-field details. **State-dependent business rules** → in handlers as typed `Result` flat codes (inside the tx where needed). **Domain invariants** → enforced in the aggregate as a last-line **safety net** (an invariant throw = handler bug = 500, never a user-facing 422). **No DataAnnotations.** The individual-vs-package XOR and qty-cap rules are FluentValidation cross-field rules.
**Rationale:** One rule per tier prevents duplicated or missing checks; FluentValidation handles the conditional cross-field cases DataAnnotations can't.

### Q40 — Configuration & secrets
**Decision:** Layered **`IOptions<T>`**: `appsettings.json` for **non-secret defaults**; **.NET User Secrets locally**; **environment variables in production** for all secrets. Every secret area is a typed options class **validated at startup (`ValidateOnStart`) — fail-fast** if a required secret is missing. Committed `appsettings.json` holds **only placeholders**. A cloud secret manager (Key Vault / Secrets Manager) is a **deferred, zero-consumer-change `IConfiguration` provider**. Required env-var names documented in `appsettings.example` / README.
**Rationale:** Satisfies "never hardcode secrets" (NFR-SEC-08); fail-fast surfaces a missing key at boot, not on the first webhook.

### Q41 — Logging & observability
**Decision:** **Serilog**, **structured JSON** logs, **request-scoped correlationId** enrichment (shared with the Q37 error envelope), **console sink now** (config-swappable later), and a **destructuring/scrubbing policy** that makes secret / QR-secret / PAN leakage structurally hard. Handlers keep injecting `ILogger<T>` (no call-site lock-in). Team rules: **log codes + ids, never secret-bearing payloads; correlationId end-to-end.**
**Rationale:** Structured logs make the webhook idempotency and reserve/hold paths queryable; the scrubbing policy *enforces* the log-hygiene AC rather than relying on reviewers.

### Q42 — EF migrations & seeding
**Decision:** **Code-first migrations** applied as an **explicit, auditable deploy step** (migration **bundle** preferred); **no auto-migrate on production boot** (dev may migrate by hand / guarded). An **idempotent seeder** inserts fixed reference data and **bootstraps the first Admin create-if-none** — email from config, **password from a one-time env secret**, Identity-hashed, **never committed**.
**Rationale:** Migrations stay reviewable git artifacts; explicit application avoids multi-instance migration races; the Admin bootstrap obeys NFR-SEC-08.

### Q43 — Testing strategy (agreed; authoring **deferred**)
**Decision:** **Target strategy** = risk-weighted pyramid: heavy **unit tests** on Domain aggregates + handlers; **integration tests on real SQL Server via Testcontainers** for the concurrency/money/unique-index paths (SERIALIZABLE reserve/hold, Paymob HMAC webhook idempotency, dual-role filtered unique indexes, QR-hash index, promo cap/user-limit races, hold-expiry sweeper); thin **`WebApplicationFactory` E2E smoke**; 80% coverage gate focused on Domain + critical handlers. Test-project layout: `Domain.UnitTests`, `Application.UnitTests`, `Integration.Tests`, `Api.SmokeTests`.
**⚠️ Timing:** **No test suite is authored until the stakeholder explicitly green-lights it.** Early development delivers core features and stabilizes the architecture first; the pyramid above is the plan for *when* tests are written, not a signal to write them now. The architecture docs document this as "target design — deferred."
**Rationale:** Test budget belongs on oversell/double-charge/double-check-in, tested against a *real* engine (InMemory can't model SERIALIZABLE, filtered indexes, or rowversion). Deferring authoring avoids tests written against a still-moving design.

### Q44 — API versioning
**Decision:** **No versioning machinery** (no library, no negotiation, no per-version Swagger). Keep the **`/api/v1` literal path prefix** as a stable static string (consistent with D:Q27). Real versioning is introduced only if a second client version / breaking change with backward-compat need arises — non-breaking, since v1 is already the path.
**Rationale:** A single co-evolving FE/BE with no external consumers gains nothing from versioning infrastructure now; keeping the `/api/v1` prefix means adding it later needs no doc-wide rewrite.

### Q45 — Side-effects & domain events
**Decision:** **No domain-event bus** now. Handlers **orchestrate side-effects explicitly**; the money mutation (HMAC verify → mark Paid → issue tickets) stays in **one transaction**. Crash-safe email/notification delivery uses a **transactional outbox** (Q53) written in the business tx and **drained by the existing sweeper** (Q34). **Firm rule: external side-effects fire *after* DB commit via the outbox, never inside the money transaction.** MediatR `INotification` is the deferred seam if multi-reaction fan-out appears.
**Rationale:** Cross-context runtime fan-out is thin today; explicit orchestration keeps the critical path readable, and the outbox gives at-least-once delivery with infrastructure already chosen.

## Section K — Data Model

### Q46 — Identity: the account table
**Decision:** **`ApplicationUser : IdentityUser<Guid>`**, **GUID PK** (this GUID *is* the account id other contexts reference). Keep Identity's `Email/NormalizedEmail/PasswordHash/SecurityStamp/ConcurrencyStamp/LockoutEnd/AccessFailedCount/EmailConfirmed`; add `GlobalRole` (enum, default Attendee), `FullName`, audit columns, soft-delete. Configure **`AddIdentityCore` with no roles store and no claims/external-login tables**.
**Rationale:** Identity's crypto without its schema baggage; GUID PK consistent with the project-wide convention.

### Q47 — Identity: refresh + reset tokens
**Decision:** A dedicated **`RefreshToken`** table — **hashed** token (`TokenHash`, raw never stored), rotation chain (`ReplacedByTokenHash`), revoke metadata (`RevokedAtUtc`, `ReasonRevoked`), `ExpiresAtUtc`, `AccountId` (FK). Unique index on `TokenHash`; index on `AccountId` (revoke-all). Reuse of an already-revoked token ⇒ revoke the whole chain (`TOKEN_REUSED`). **Password reset uses Identity's built-in `SecurityStamp`-backed provider** (single-use, expiring) — **no separate reset-token table**; `RESET_TOKEN_INVALID` = a failed `ResetPasswordAsync`. Implements D:Q24.
**Rationale:** Hashed secrets satisfy log-hygiene; the rotation chain backs reuse-detection; the built-in reset provider avoids a second secret at rest.

### Q48 — Eventing: Event & Package
**Decision:** **`Event`** carries `TicketPrice` (**`decimal(18,2)`** EGP, ≥ 0; 0 ⇒ free path), nullable `MaxIndividualQtyPerOrder` (Model B, D:Q1 addendum), `Capacity`, i18n text pairs, `Status`, `ImageUrl`, audit, soft-delete, `RowVersion`. Remaining seats **computed, never stored**. **`Package`** is an **optional child** of Event with a **real intra-context FK** (`EventId`), its own `Price`, seat count, nullable `MaxQuantityPerOrder` (D:Q2), audit, soft-delete, `RowVersion`. **Zero packages is valid and publishable.**
**Rationale:** Price + cap on the event make the individual-ticket path first-class; `decimal(18,2)` is the project money type; intra-context FK is fine (see the FK revision below).

### Q49 — Ticketing: Order & Ticket
**Decision:** A **flat `Order`** (no `OrderItem` table — D:Q1 forbids mixed baskets): `OrderReference` (unique), `AccountId` (FK), `EventId` (FK), **nullable `PackageId` FK** (null ⇒ individual, Model B), `UnitType` enum, `Quantity`, **price-snapshot columns** (`UnitPriceSnapshot/SubtotalSnapshot/DiscountSnapshot/TotalSnapshot` + `PromoCodeId`/`PromoCodeSnapshot`) for anti-tamper → `PRICE_CHANGED` (D:Q4), `Status` (PendingPayment/Paid/Cancelled/Expired), `HoldExpiresAtUtc`, payment linkage, audit, `RowVersion`. **Orders are append-only** — cancel = status, never deleted (no soft-delete flag). **`Ticket`** is fanned **one row per seat** on payment confirmation: `TicketReference` (unique), **`QrSecretHash`** (SHA-256, raw never stored — D:Q8), optional `HolderName`, `Status` (Issued/CheckedIn/Voided), `CheckedInAtUtc/By`, `RowVersion`, `EventId` (denormalized for the scan). **Invariant indexes:** unique on `Ticket.QrSecretHash`, `Ticket.TicketReference`, `Order.OrderReference`; index on `Ticket(EventId,Status)`, `Order(EventId,Status)`, `Order(AccountId,Status)`. Held-seats = `SUM(Quantity)` over `Paid OR (PendingPayment AND HoldExpiresAtUtc > now)`, computed in the SERIALIZABLE reserve tx.
**Rationale:** No pointless single-row line table; snapshots make price-tampering impossible; hashed QR; append-only financial history; every concurrency/scan query indexed.

### Q50 — Ticketing: PromoCode & redemption ledger
**Decision:** **`PromoCode`** columns map **1:1 to the flat 422 codes**: `IsActive`→`PROMO_INACTIVE`, `ValidFromUtc`→`PROMO_NOT_YET_VALID`, `ValidUntilUtc`→`PROMO_EXPIRED`, `MaxTotalRedemptions`→`PROMO_CAP_REACHED`, `MaxPerUser`→`PROMO_USER_LIMIT`, nullable `EventId`→`PROMO_WRONG_EVENT`; unique on normalized `Code`; `DiscountType`/`DiscountValue`, audit, soft-delete, `RowVersion`. An **append-only `PromoRedemption` ledger** (`PromoCodeId` FK, `AccountId`, `OrderId` FK, `RedeemedAtUtc`) enforces both caps by **counting inside the SERIALIZABLE reserve tx**; indexes on `(PromoCodeId)` and `(PromoCodeId, AccountId)`. **The slot is atomically claimed at payment-initiation (paid orders) or at confirmation (free / 100%-off), confirmed on Paid, and released on payment failure or hold-expiry — never burned by an unpaid hold** (D:Q19).
**Rationale:** Every flat promo code has an exact column/query behind it; ledger-counting in the serializable tx keeps the global cap and per-user limit race-safe (a stored counter can't).

### Q51 — Training/Identity: TrackAssignment & the dual-role invariants
**Decision:** **`Track`** (i18n, `IsActive`, audit, soft-delete, `RowVersion`). **`TrackAssignment`** (`AccountId` FK, `TrackId` FK, `TrackRole` enum Member/Board, `AssignedAtUtc`, `AssignedBy`, audit, `RowVersion`). The **≤1-Member / ≤1-Board-per-user** rule is enforced by **two filtered unique indexes**: `UNIQUE(AccountId) WHERE TrackRole='Member'` and `UNIQUE(AccountId) WHERE TrackRole='Board'` (race-proof at the DB). A plain unique `(AccountId, TrackId, TrackRole)` blocks exact dupes; index on `(TrackId, TrackRole)` for roster queries. The **"different track" rule** (no Member@X + Board@X; Member@X + Board@Y is the *allowed* dual role) is a **domain invariant** checked in the same transaction (a filtered index can't express it without a trigger/indexed view, and the concurrency-dangerous part *is* covered by the DB).
**Rationale:** The filtered indexes kill the double-assignment race where only the DB can; the different-track check is a cheap in-tx read; keeps track data off the Identity user (Q29 separation).

### FK revision — cross-context referential integrity (supersedes the earlier no-cross-context-FK reading)
**Decision:** **Keep real foreign keys across contexts at the DB level** (single SQL Server → referential integrity stays on): `Order.AccountId`, `TrackAssignment.AccountId`, `PromoRedemption.AccountId`, `Attendance.AccountId`, `Evaluation.AccountId`, and `Ticket` (via Order) are **real FKs to `ApplicationUser.Id`** with **`DeleteBehavior.Restrict`** (never cascade — accounts are soft-deleted and financial records append-only). **Decoupling becomes a *code* rule, not a schema rule:** **no cross-context EF navigation properties** (no `order.Account.Email`); domain + handlers reference other contexts by **`AccountId` GUID** only. Contexts stay separate folders/aggregates.
**Rationale:** The no-cross-context-FK rule's only real payoff is physical DB extraction — a future explicitly ruled out — so it was disproportionate. Real FKs give free, always-on integrity on one database; the code convention preserves the boundary. If a split ever happens, dropping FKs is a migration and the seam is still clean because code never traversed it. **This revises the earlier reading of NFR-MNT-02 / Q29b** ("related only by account id, no cross-context FK") to: *no cross-context navigation in code; real FKs in the schema.*

### Q52 — Training: Session, Attendance, Evaluation
**Decision:** Three **plain record tables** (CRUD-simple per Q32; no aggregates). **Enrollment is collapsed into `TrackAssignment`** — the `TrackRole=Member` row *is* the enrollment, `AssignedAtUtc` = join date / attendance denominator start (single source of truth for "who's in this track"). **`Session`** (track-scoped, i18n title, Scheduled/Held/Cancelled, audit, soft-delete, `RowVersion`). **`Attendance`** (one row per member per session, **unique `(SessionId, AccountId)`**, status Present/Late/Absent, `RecordedBy`, `RowVersion`) — **attendance % computed never stored**, `(Present + Late) / totalSessions`, **Late counts as attended** (D:Q12). **`Evaluation`** (per-member, optional per-session, **`Score` = integer 0–100** per D:Q17, i18n comment, `EvaluatedBy`, audit, `RowVersion`) — visibility (member sees own, Board sees their track) enforced in the Q35 handler, not the schema.
**Rationale:** Record-keeping with one simple invariant each doesn't clear the rich-aggregate bar; collapsing Enrollment removes a redundant source of truth; computed attendance mirrors computed seats.

### Q53 — Transactional outbox
**Decision:** A single **`OutboxMessage`** table (`Id`, `Type`, `PayloadJson` — **ids + non-secret fields only**, `CreatedAtUtc`, `ProcessedAtUtc` nullable, `Attempts`, `LastError`, `NextAttemptAtUtc`), **written inside the business transaction** (atomic with the state change) and **drained by the sweeper** with attempt/backoff → **at-least-once** delivery. Filtered index on `WHERE ProcessedAtUtc IS NULL`. Handlers tolerate a rare duplicate send; the money path stays idempotent via the HMAC webhook.
**Rationale:** Delivers Q45's crash-safety with one table and the existing sweeper; retry/backoff self-heals transient SMTP/callback failures without a message broker.

### Q54 — Cross-cutting column conventions
**Decision:** Base markers + EF interceptor + global filter, **applied by category** (not blanket). **`IAuditable`** (`CreatedAtUtc/By`, `UpdatedAtUtc/By`) auto-set by a **`SaveChanges` interceptor** from `ICurrentUser` + `IClock`, on admin/money/training-write tables; append-only ledgers keep just `CreatedAtUtc`. **`ISoftDeletable`** (`IsDeleted` + **global query filter**) on **catalog tables only** (Event, Package, Track, Session, ApplicationUser) — **Order/Ticket/PromoRedemption are append-only (cancel-by-status), no soft-delete**; Attendance/Evaluation corrected by update. **`RowVersion`** only where concurrent writers exist (Event, Order, Ticket, PromoCode, TrackAssignment). **Team note:** admin "archived" views call `IgnoreQueryFilters()`.
**Rationale:** Interceptor + global filter make the conventions automatic and uniform; by-category application keeps a column off any table that has no use for it.

### Q55 — State machines (Order / Ticket / Event)
**Decision:** **Explicit transition methods on the aggregates are the only way status changes** (enum never set directly from a handler). **Order:** `MarkAsPaid()` (PendingPayment + within hold → Paid, fans out tickets; **re-call on an already-Paid order = idempotent no-op success** = the HMAC guarantee at domain level), `Cancel()`, `Expire()` (sweeper). **Ticket:** `CheckIn()` (Issued only; second call → `TICKET_ALREADY_CHECKED_IN`), `Void()` (check-in of a voided ticket → `TICKET_VOIDED`). **Event:** `Publish()` (allowed with **zero packages** — Model B; Published→Draft blocked once orders exist — D:Q23), `Cancel()`, `Archive()`. Illegal transitions rejected in-domain and mapped by the handler to the right flat code.
**Rationale:** The aggregate is the single place a transition can happen, so the enum can never reach an illegal value by an illegal path; idempotent-webhook and double-check-in guarantees live in the domain. The proportional use of rich domain (Q32) — confined to the three entities with real lifecycles.

### Q56 — Cancelling an Archived event directly (extends Q23)
**Question:** Can a `Draft` or `Archived` event be `Cancelled` directly, or must it be `Published` first?
**Decision:** **`Archived → Cancelled` is allowed** (same cancel ripple as `Published → Cancelled`); **`Draft → Cancelled` is not**. An Archived event is a *Published* event manually hidden — per Q23 its orders/tickets are **unaffected**, so it may hold sold tickets and paid orders; forcing `Archived → Published → Cancelled` would re-expose a hidden event publicly just to cancel it. A `Draft` event can never have orders and is disposed of by **soft-delete** (Q22: "soft-delete only when zero orders exist, otherwise Cancel") — cancelling it would add nothing (no tickets/refunds/holds) and would pollute the `Cancelled` state, which reports read as "an event that sold seats and was called off." **Cancelled remains terminal; Archived is not terminal** (it re-lists to Published).
**Rationale:** Closes a Q23 gap — an archived, sold-out event had no direct disposal path. Keeps `Cancelled` meaningful (only for events that could have sold seats) and preserves soft-delete as the zero-order Draft path. Touches `FR-EVT-04`, `FR-PAY-07`; extends **Q22, Q23**.

---

## Summary table

| # | Topic | Decision (one line) |
|---|-------|---------------------|
| Q1 | Order composition | One unit-type × quantity per order (individual ticket **or** one package); separate orders for different types — Model-B addendum 2026-07-21 |
| Q2 | Quantity cap | Per-package `MaxQuantityPerOrder`, nullable, checked at quote + reserve |
| Q3 | Held seats | Paid + PendingPayment with `HoldExpiresAt > now` (clock-aware, sweeper is cleanup) |
| Q4 | Re-price at reserve | Server always re-prices; quote advisory; `PRICE_CHANGED` on mismatch; snapshot on order |
| Q5 | Concurrent orders | One active pending order per user per event; paid orders don't block |
| Q6 | Void paid order | Admin-only; refund offline; checked-in seats non-voidable |
| Q7 | Ticket states | Issued / CheckedIn / Voided; no-show derived, not stored |
| Q8 | QR token | Public reference + 256-bit secret; validate against SHA-256 hash |
| Q9 | Check-in | Admin-only, event-scoped; 5 scan outcomes (incl. TICKET_VOIDED); all rejects logged |
| Q10 | Deactivation | Blocks login only; tickets stay valid; pending order cancelled; Board gap flagged |
| Q11 | Enrollment | Records keyed on enrollment; re-enroll = fresh %; scoped to active enrollment |
| Q12 | Attendance % | Denominator = occurred + recorded sessions; Late = attended; Absent explicit |
| Q13 | Session edit/delete | Records-bearing session editable not hard-deletable; Board own-track only |
| Q14 | Track soft-delete | Auto-ends enrollments + Board; retains history; Admin-confirmed impact |
| Q15 | Board enroll | Existing accounts only; dual-role rules enforced at enroll |
| Q16 | Evaluation timing | Past session + active enrollment; attendance not required |
| Q17 | Score & edits | Integer 0–100; overwrite in place with audit; no version history |
| Q18 | Promo math | Half-up 2dp; `max(base−discount,0)`; piastres only at Paymob boundary |
| Q19 | Redemption timing | Claim slot at pay-init/confirm; confirm on Paid; release on fail/expiry |
| Q20 | Contact form | Unauthenticated; IP rate-limit + length caps; Admin-only; no CAPTCHA/reply |
| Q21 | Notifications | Per-recipient rows at send time (snapshot); per-row read state; scoped audiences |
| Q22 | Event w/ sold tickets | Capacity floor = held+paid; Cancel voids+refunds offline; soft-delete only if zero orders |
| Q23 | Event state machine | Draft⇄Published (zero orders); →Archived/Cancelled; Archived→Cancelled (Q56); Cancelled terminal; date-derived public split |
| Q24 | Token lifetimes | Access 15m / refresh 7d rotating-hashed (family-revoke) / reset 1h; refresh in body |
| Q25 | API envelope | `{success,data,error}`; stable machine `code` for i18n; `fieldErrors` for validation |
| Q26 | Pagination | Offset `page`/`pageSize` (def 20, cap 100) + `meta`; whitelisted sort/filter |
| Q27 | Wire formats | `/api/v1`; UTC ISO dates; decimal EGP + `currency`; GUID ids; PascalCase enums |
| Q28 | Small items | Idempotency-Key on pay-init; 429 + `RATE_LIMITED` + `Retry-After`; explicit out-of-scope; reports export via `?format` |
| Q29 | Architecture style | Modular monolith + Clean Architecture (Domain/Application/Infrastructure/Api); contexts as folders |
| Q29b | DbContext count | Single `DbContext` now; 3-way split deferred |
| Q30 | CQRS/MediatR | MediatR + Validation/Logging behaviors; explicit tx in handlers; CQRS-lite (same read model) |
| Q31 | Data access | `IApplicationDbContext`; no generic repos; pure-POCO domain; targeted aggregate methods |
| Q32 | Domain richness | Rich domain only for Order/Ticket/Event/assignments; CRUD-simple elsewhere |
| Q33 | Seat concurrency | SERIALIZABLE reserve tx + Polly retry on 1205/serialization; held-seats computed |
| Q34 | Sweeper | In-process `BackgroundService` + `sp_getapplock`; cleanup-only; Hangfire deferred |
| Q35 | Authorization | MediatR `AuthorizationBehavior` + marker interfaces + `ICurrentUser`; track-scope resolved per request |
| Q36 | Identity store | ASP.NET Core Identity (store/hash/reset) + custom JWT/refresh; `GlobalRole` column; Member/Board relational |
| Q37 | Failure envelope | Typed `Result<T>` + one `Result→HTTP` mapper + exception middleware for 500s only; `Errors` catalog |
| Q38 | Mapping | Manual mapping in Application layer; no mapper library |
| Q39 | Validation | FluentValidation (shape) / handler `Result` (business) / domain invariant (safety net); no DataAnnotations |
| Q40 | Config & secrets | Layered `IOptions<T>` + User Secrets (dev) + env vars (prod); fail-fast; Key Vault deferred |
| Q41 | Logging | Serilog structured JSON + correlationId + secret-scrubbing policy; console sink now |
| Q42 | Migrations & seed | Code-first, applied explicitly (bundle), no prod auto-migrate; idempotent seeder + first-Admin from env |
| Q43 | Testing | Risk-weighted pyramid (Testcontainers for concurrency/money); **authoring deferred until stakeholder go** |
| Q44 | API versioning | No versioning machinery; keep `/api/v1` literal prefix |
| Q45 | Side-effects | Explicit handler orchestration + transactional outbox drained by sweeper; no domain-event bus |
| Q46 | Account table | `ApplicationUser : IdentityUser<Guid>`; core columns only; no roles/claims/logins tables |
| Q47 | Token tables | Hashed `RefreshToken` w/ rotation chain + reuse-revoke; reset via Identity built-in provider |
| Q48 | Event/Package | Price+cap on Event; optional Package child (intra-context FK); `decimal(18,2)`; computed seats |
| Q49 | Order/Ticket | Flat append-only `Order` (no OrderItem) + one-per-seat hashed `Ticket`; snapshot + concurrency indexes |
| Q50 | Promo | `PromoCode` columns↔flat codes + append-only `PromoRedemption` ledger; caps in SERIALIZABLE tx |
| Q51 | Track assignments | `TrackAssignment` + two filtered unique indexes (≤1 Member/≤1 Board); different-track rule in-domain |
| FK | Cross-context FKs | **Revised:** real DB FKs + `Restrict` delete; decoupling is a code rule (no cross-context nav props) |
| Q52 | Training records | Session/Attendance/Evaluation records; Enrollment collapsed into TrackAssignment; attendance % computed; score 0–100 |
| Q53 | Outbox | Single `OutboxMessage` table, tx-written, sweeper-drained, retry/backoff, at-least-once |
| Q54 | Column conventions | Base markers + audit interceptor + global soft-delete filter, applied by category |
| Q55 | State machines | Explicit transition methods on Order/Ticket/Event; illegal transitions rejected in-domain |
| Q56 | Cancel Archived event | `Archived → Cancelled` allowed (cancel ripple); `Draft → Cancelled` not (Draft disposed via soft-delete); extends Q22/Q23 |

---

*Requirements session (Q1–Q28): 2026-07-20. Architecture session (Q29–Q55): 2026-07-22. Q56 (event-cancel gap, extends Q22/Q23): 2026-07-23. All questions resolved and accepted by the stakeholder. Q1–Q28 are the authoritative basis for documents 05–07; Q29–Q56 are the authoritative basis for the 09/10/11/12 doc set and the code scaffold. Where any conflicts with the PRD/SRS, this log and the SRS corrections prevail.*
