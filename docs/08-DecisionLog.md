# TEDxAlkawmia Platform — Design Decision Log

> **Version:** 1.0
> **Date:** 2026-07-20
> **Status:** Accepted — basis for [05 — User Stories](./05-UserStories.md), [06 — Acceptance Criteria](./06-AcceptanceCriteria.md), [07 — API Contract](./07-ApiContract.md)
> **References:** [01 — PRD](./01-PRD.md) · [02 — SRS](./02-SRS.md) (v1.1) · [03 — User Flows](./03-UserFlows.md) · [04 — Personas](./04-Personas.md)

---

## Purpose

This document records the **28 design questions** raised during the requirements grilling session (2026-07-20) and the decision reached on each. The PRD, SRS, and User Flows left these points ambiguous or under-specified; each was resolved before writing the User Stories, Acceptance Criteria, and API Contract. Decisions are cited across those documents as **(D:Qn)**.

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
**Decision:** **Admin-only** (per the PRD role matrix). The scan endpoint is **event-scoped**. Four distinct outcomes: **success**, **already-checked-in** (returns who + when), **wrong-event**, **unknown/invalid**. **All rejections are logged** (`FR-TKT-06`). A delegated-scanner role is deferred.
**Rationale:** Matches the PRD authorization matrix; distinct outcomes give door staff actionable feedback and an audit trail. Touches `FR-TKT-05`, `FR-TKT-06`.

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
| Q9 | Check-in | Admin-only, event-scoped; 4 scan outcomes; all rejects logged |
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
| Q23 | Event state machine | Draft⇄Published (zero orders); →Archived/Cancelled; Cancelled terminal; date-derived public split |
| Q24 | Token lifetimes | Access 15m / refresh 7d rotating-hashed (family-revoke) / reset 1h; refresh in body |
| Q25 | API envelope | `{success,data,error}`; stable machine `code` for i18n; `fieldErrors` for validation |
| Q26 | Pagination | Offset `page`/`pageSize` (def 20, cap 100) + `meta`; whitelisted sort/filter |
| Q27 | Wire formats | `/api/v1`; UTC ISO dates; decimal EGP + `currency`; GUID ids; PascalCase enums |
| Q28 | Small items | Idempotency-Key on pay-init; 429 + `RATE_LIMITED` + `Retry-After`; explicit out-of-scope; reports export via `?format` |

---

*Session date: 2026-07-20. All 28 questions resolved and accepted by the stakeholder. These decisions are the authoritative basis for documents 05–07; where any conflicts with the PRD/SRS, this log and the v1.1 SRS correction (Q1) prevail.*
