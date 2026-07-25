# TEDxAlkawmia Platform — Software Requirements Specification (SRS)

> **Version:** 1.5
> **Date:** 2026-07-23
> **Status:** Draft — Pending Stakeholder Approval
> **References:** [01 — PRD](./01-PRD.md) · [03 — User Flows](./03-UserFlows.md) · [04 — Personas](./04-Personas.md) · [05 — User Stories](./05-UserStories.md) · [06 — Acceptance Criteria](./06-AcceptanceCriteria.md) · [07 — API Contract](./07-ApiContract.md) · [08 — Decision Log](./08-DecisionLog.md) · [09 — System Design](./09-SystemDesign.md) · [10 — Data Model](./10-DataModel.md) · [11 — State Machines](./11-StateMachines.md) · [12 — Sequence Diagrams](./12-SequenceDiagrams.md)
>
> **v1.1 (2026-07-20):** Glossary **Order** definition corrected to a **single package × quantity** (one package type per order), aligning with `FR-ORD-02`/`FR-ORD-04` and grilling decision Q1. The prior "one or more ticket packages" wording is superseded.
>
> **v1.2 (2026-07-20):** `FR-ATT-03` attendance-percentage denominator corrected to **sessions that have occurred and have a recorded entry** (future and un-recorded sessions excluded; Absent must be recorded explicitly), scoped to the current active enrollment — aligning with decisions Q11/Q12 and the User Stories, Acceptance Criteria, and User Flows. The prior "÷ total sessions" wording is superseded.
>
> **v1.3 (2026-07-21):** **Model-B ticketing** (Decision Log Q1 addendum). Individual tickets are the base purchasable unit at an **event-level face price (`ticketPrice`)**; **packages are optional discount bundles**, never a prerequisite. An order references a **nullable package** (individual-ticket order when null). An event with **zero packages is publishable and sellable**. Affects the glossary and `FR-EVT-01`, `FR-PKG-01`, `FR-ORD-01`, `FR-ORD-02`, `FR-ORD-04`; adds `FR-ORD-09` (hold-expiry sweeper). Supersedes any "one package per order is the only unit" reading of v1.1 (Q1 still holds: one unit-type × quantity per order).
>
> **v1.5 (2026-07-23):** **Aligned with the architecture grilling pass (Decision Log Q29–Q56) and the now-written 09/10/11/12 doc set.** Changes: (1) **`NFR-MNT-02` rewritten** — the "no foreign keys across contexts" rule is superseded by the **FK revision** (real cross-context FKs with `RESTRICT`; decoupling is a *code* rule: no cross-context navigation properties). (2) All "(pending)" markers for System Design (09) and Data Model (10) removed — both now exist and are authoritative. (3) "28 resolved design questions" → **Q1–Q56** throughout. (4) Added `FR-EVT-09` (event cancel ripple from Published **or** Archived — D:Q22/Q56) and `NFR-REL-07` (transactional outbox, at-least-once side-effects — D:Q45/Q53); expanded `FR-EVT-04` with the full legal transition set (D:Q23/Q56) and `FR-AUTH-06` with the per-request track-scope basis (D:Q35). (5) §7 traceability and §8 open-items updated to reflect that 09/10 are written, and 11/12 added as the lifecycle/flow authorities. No requirement was invented — every added/changed line cites its `D:Q`.

---

## 1. Introduction

### 1.1 Purpose

This document specifies the **detailed software requirements** for the TEDxAlkawmia Platform. It sits between the [PRD](./01-PRD.md) (which defines *what* the product does and *why*) and the design/data documents (which define *how* it is built). The SRS turns the PRD's capability areas into precise, testable **functional** and **non-functional** requirements that engineering can implement and QA can verify against.

It is a **bridge document**: it does not restate the full feature catalog from the PRD or the full schema from the [Data Model](./10-DataModel.md). Where those are authoritative, this document references them and adds the requirement-level detail (behaviour, inputs, validation, error handling, quality attributes) that neither of them carries.

### 1.2 Scope

The platform is a web application with two faces:

1. **Public ticketing** — visitors discover paid events, reserve **individual tickets or optional package bundles**, pay online via Paymob, receive one QR ticket per seat, and get checked in at the door.
2. **Internal training** — the organizing team runs training tracks: enrolling members, scheduling sessions, recording attendance, and evaluating members.

The system comprises:

- a **REST API backend** (ASP.NET Core Web API, C#),
- a **single-page application frontend** (React + Vite + TypeScript),
- a **SQL Server** relational database,
- integration with **Paymob** (payment), **Cloudinary** (image storage), and an **SMTP** provider (transactional email).

What ships in which release is **out of scope for this document** — phasing and the MVP cut are decided in a separate planning document. This SRS specifies the requirements for the whole product.

### 1.3 Definitions & acronyms

| Term | Definition |
|------|------------|
| **Visitor** | An unauthenticated browser of the public site. |
| **Attendee** | A registered user; the default global role. Can buy tickets and attend events. |
| **Member** | A per-track role: a user enrolled in exactly one training track. |
| **Board** | A per-track role: a user who supervises exactly one training track. |
| **Admin** | Global role with full platform control. |
| **Order** | A purchase for a single event of **either individual tickets (at the event face price) or one package** (one unit-type × quantity); holds seats, then produces tickets once paid. |
| **Ticket** | One admission credential per seat, carrying a unique QR token. |
| **Package** | An **optional** named seat-bundle an event offers on top of individual tickets (e.g. Duo = 2 seats, Group-5 = 5), each with its own price. An event may have zero packages. |
| **Hold** | The temporary reservation of seats for an unpaid order, released when the checkout window expires. |
| **Check-in** | Validating a ticket's QR at the venue; each ticket is single-use. |
| **API** | Application Programming Interface. |
| **CQRS** | Command Query Responsibility Segregation. |
| **DTO** | Data Transfer Object. |
| **JWT** | JSON Web Token. |
| **HMAC** | Hash-based Message Authentication Code (used to verify Paymob webhooks). |
| **QR** | Quick Response code. |
| **RBAC** | Role-Based Access Control. |
| **SPA** | Single Page Application. |

### 1.4 References

- [00 — Original brief](./00-TEDxAlkawmia.md) — the initial project description (superseded where it conflicts).
- [01 — PRD](./01-PRD.md) — product requirements, roles, and feature catalog. **Authoritative for scope.**
- [03 — User Flows](./03-UserFlows.md) · [04 — Personas](./04-Personas.md) — behavioural flows and the user classes this SRS serves.
- [05 — User Stories](./05-UserStories.md) · [06 — Acceptance Criteria](./06-AcceptanceCriteria.md) · [07 — API Contract](./07-ApiContract.md) — the story/test/endpoint layer that refines these `FR-*`.
- [08 — Decision Log](./08-DecisionLog.md) — the resolved design questions **Q1–Q56** (cited as **D:Qn**): requirements grilling (Q1–Q28, incl. the Model-B addendum) plus the architecture grilling (Q29–Q56). **Authoritative for resolved design questions.**
- [09 — System Design](./09-SystemDesign.md) — layering, CQRS, module boundaries, and the authorization model (this SRS defers architecture detail to it).
- [10 — Data Model](./10-DataModel.md) — ERD, tables, constraints, EF Core mapping. **Authoritative for the database.**
- [11 — State Machines](./11-StateMachines.md) — the entity lifecycle (state) diagrams the `FR-*` behaviors move entities through.
- [12 — Sequence Diagrams](./12-SequenceDiagrams.md) — the cross-subsystem runtime flows (reserve, pay, check-in, refresh, cancel ripples) these requirements are realized by.

### 1.5 Requirement notation

- Functional requirements are identified as **`FR-<area>-<n>`** and traced back to the PRD feature IDs (e.g. `AUTH-01`) they refine.
- The key words **MUST**, **MUST NOT**, **SHOULD**, and **MAY** are used in the RFC 2119 sense.
- Each requirement is written to be **testable** — a single, observable behaviour.

---

## 2. Overall description

### 2.1 Product perspective

The platform is a new, self-contained system replacing manual processes (spreadsheets, social media forms, WhatsApp). It is a **client–server** application: a stateless REST API serves a browser SPA today and is built client-agnostic so a mobile client could consume the same API later.

```
        Visitor / Attendee / Member / Board / Admin
                        │  (HTTPS)
                        ▼
             ┌────────────────────┐
             │   React SPA (web)   │
             └─────────┬──────────┘
                       │ REST + JWT
                       ▼
             ┌────────────────────┐         ┌──────────────┐
             │ ASP.NET Core Web API│──────▶ │  Paymob      │  (payment + webhook)
             │  (stateless)        │        └──────────────┘
             │                     │        ┌──────────────┐
             │                     │──────▶ │  Cloudinary  │  (image storage)
             │                     │        └──────────────┘
             │                     │        ┌──────────────┐
             │                     │──────▶ │  SMTP        │  (transactional email)
             └─────────┬──────────┘         └──────────────┘
                       │ EF Core
                       ▼
              ┌──────────────────┐
              │   SQL Server DB   │
              └──────────────────┘
```

The internal software architecture (layering, CQRS, module boundaries) is specified in **[09 — System Design](./09-SystemDesign.md)**; the persistent structure is specified in the **[Data Model](./10-DataModel.md)**.

### 2.2 User classes

| User class | Description | Technical implication |
|------------|-------------|----------------------|
| **Visitor** | Unauthenticated. Browses public pages and events only. | No token; read-only public endpoints. |
| **Attendee** | Registered user (default). Buys tickets, attends events, manages own profile. | Baseline authenticated role. |
| **Member** | Attendee enrolled in one track. | Attendee capabilities + own training dashboard. |
| **Board** | Supervises one track; may also be a Member of a different track. | Track-scoped write access, resolved per request. |
| **Admin** | Organizing-committee leadership; full control. | Global privileged role; multiple admins allowed. |

Role rules are authoritative in [PRD §5](./01-PRD.md). The **global role** (Attendee/Admin) and the **per-track assignments** (Member/Board) are two independent dimensions; the authorization model that combines them per request is detailed in [09 — System Design §6](./09-SystemDesign.md) (D:Q35).

### 2.3 Operating environment

- **Clients:** current evergreen browsers (Chrome, Edge, Firefox, Safari) on desktop and mobile.
- **Server:** .NET 8 runtime; deployable to Windows or Linux containers.
- **Database:** SQL Server (2019+ or Azure SQL).
- **External services:** Paymob (EGP payments), Cloudinary (images), an SMTP provider (email).

### 2.4 Design & implementation constraints

- **Money** is EGP only, stored `decimal(18,2)`; conversions to Paymob piastres happen only at the gateway boundary (see [Data Model §1](./10-DataModel.md)).
- **English UI**, built **i18n-ready**: no hardcoded user-facing strings, locale-aware dates/currency, so Arabic/RTL can be added without a rewrite.
- **Statelessness:** the API MUST NOT hold session state in memory; the only server-side auth state (refresh tokens) lives in the database, enabling horizontal scaling.
- **Initial scale:** designed for a single community, hundreds of attendees per event, tens of members, under 100 concurrent users initially — while remaining horizontally scalable.

### 2.5 Assumptions & dependencies

- A **Paymob merchant account** is available with API key, integration ID, and HMAC secret.
- The first **Admin account is seeded** at deployment; it is not self-registered.
- Users have a smartphone capable of displaying a QR code at the venue.
- The TEDxAlkawmia team supplies content (event copy, team bios, about text).

---

## 3. Functional requirements

Requirements are grouped by capability area and keyed to the PRD feature IDs. Each `FR-*` is a testable behaviour; the corresponding acceptance scenarios will live in the Acceptance Criteria document. **MUST/SHOULD/MAY** carry RFC-2119 weight.

### 3.1 Authentication & Authorization (PRD §6.1)

| Ref | Requirement |
|-----|-------------|
| **FR-AUTH-01** | A Visitor MUST be able to register with first name, last name, email, and password. On success the system creates an account with the **Attendee** global role and no track assignments. |
| **FR-AUTH-02** | The system MUST reject registration if the email already belongs to an account, returning a field-level validation error (without revealing whether the address is otherwise in use beyond this uniqueness check). |
| **FR-AUTH-03** | Passwords MUST meet a minimum policy (≥ 8 chars, at least one upper, one lower, one digit) enforced server-side; violations return validation errors. |
| **FR-AUTH-04** | A registered user MUST be able to log in with email + password, receiving a short-lived **JWT access token** and a **refresh token**. |
| **FR-AUTH-05** | Login MUST fail with a generic "invalid credentials" message when the email is unknown or the password is wrong (no user enumeration), and MUST be rejected for deactivated accounts. |
| **FR-AUTH-06** | The access token MUST carry the account id, email, and **global role** claims. Per-track (Member/Board) authority MUST NOT be baked into the token; it is resolved per request against live `TrackAssignment` rows (D:Q35; see [09 — System Design §6](./09-SystemDesign.md)). |
| **FR-AUTH-07** | A user MUST be able to log out, which revokes the presented refresh token so it can no longer be exchanged. |
| **FR-AUTH-08** | A user MUST be able to exchange a valid, unexpired, unrevoked refresh token for a new access token. Refresh tokens are **single-use and rotated**: exchange revokes the old token and issues a new one, linked via `ReplacedByTokenHash`. Reuse of a revoked token MUST **revoke the entire rotation family** (walk the `ReplacedByTokenHash` chain) and return `TOKEN_REUSED` — forcing re-login (D:Q24, Q47). |
| **FR-AUTH-09** | Refresh tokens MUST be stored **hashed**; the raw token exists only with the client. |
| **FR-AUTH-10** | A user MUST be able to request a password reset by email; the response MUST be identical whether or not the email exists (no enumeration). |
| **FR-AUTH-11** | A valid, unexpired, single-use reset token MUST allow the user to set a new password; used or expired tokens MUST be rejected. |

### 3.2 User & Profile Management (PRD §6.2)

| Ref | Requirement |
|-----|-------------|
| **FR-USER-01** | An authenticated user MUST be able to view their own profile (name, email, phone, bio, profile picture, global role, track assignments). |
| **FR-USER-02** | A user MUST be able to edit their first name, last name, phone, and bio. Email is immutable after registration. |
| **FR-USER-03** | A user SHOULD be able to upload a profile picture; the file is stored in Cloudinary and only its URL is persisted. Uploads MUST be validated for type (image) and size. |
| **FR-USER-04** | A user MUST be able to change their password by supplying the current password and a new one that meets the password policy. |
| **FR-USER-05** | An Admin MUST be able to list all users with pagination, search (name/email), and filters (global role, active status). |
| **FR-USER-06** | An Admin MUST be able to activate/deactivate a user account. A deactivated user cannot log in or refresh tokens, and existing refresh tokens are revoked. Deactivation also **cancels any active PendingPayment orders** (releasing held seats) and **ends all active track assignments** (`EndedAt` set, history retained, slots freed for reassignment). Deactivating a Board flags the track as needing a new supervisor. Reactivation does **not** auto-restore assignments (D:Q10). |
| **FR-USER-07** | Deactivation MUST be a soft action (the account and its historical records are retained), never a hard delete. |

### 3.3 Track Assignments & Roles (PRD §5, §6.2)

| Ref | Requirement |
|-----|-------------|
| **FR-ROLE-01** | Only an Admin MUST be able to change a user's **global role** (Attendee ↔ Admin). |
| **FR-ROLE-02** | Only an Admin MUST be able to assign or remove the **Board** role on a track. |
| **FR-ROLE-03** | An Admin, **or the Board of that track**, MUST be able to enroll or remove **Members** in that track. A Board can only do so for the single track they supervise. Enrollment adds an **existing Attendee account** (found by email/search) — no account creation at enroll time. At enroll time the system MUST reject if the target is already an active Member of any track, or if it would make them Member and Board of the same track (D:Q15). |
| **FR-ROLE-04** | The system MUST enforce that a user holds **at most one active Member enrollment** and **at most one active Board assignment**, and that the two are **different tracks**. Violations MUST be rejected at assignment time and prevented at the database level. |
| **FR-ROLE-05** | Ending a track assignment MUST retain the historical attendance and evaluation records tied to it. |

### 3.4 Event Management (PRD §6.3)

| Ref | Requirement |
|-----|-------------|
| **FR-EVT-01** | An Admin MUST be able to create an event with title, description, date/time (UTC), location, capacity, an **individual-ticket price (`ticketPrice`, ≥ 0 EGP)**, and optional image. Capacity MUST be greater than zero. The event MAY carry an optional **`MaxIndividualQtyPerOrder`** (nullable = no cap) limiting individual tickets per order. |
| **FR-EVT-02** | An Admin MUST be able to edit an event's details. Capacity MUST be raisable at any time but lowerable only to ≥ (held + paid) seats — a lower value that would invalidate sold seats MUST be rejected (D:Q22). Concurrent edits MUST be guarded by an optimistic-concurrency token. |
| **FR-EVT-03** | An Admin SHOULD be able to soft-delete an event; soft-deleted events are hidden from all listings but retained with their orders/tickets intact. An event with existing orders MUST NOT be hard-deleted (D:Q22). |
| **FR-EVT-04** | An event MUST have a status of Draft, Published, Archived, or Cancelled. Only **Published** events are visible to Visitors and open for booking. Legal transitions: `Draft ⇄ Published` (revert only if zero orders); `Published → Archived`; `Published → Cancelled`; `Archived → Published` (re-list); `Archived → Cancelled` (D:Q56). `Cancelled` is terminal. `Draft → Cancelled` is not legal — a Draft event is disposed of by soft-delete (D:Q22, Q23, Q56; see [[11-StateMachines#3. Event (D:Q22, Q23, Q55, Q56)|11 §3]]). |
| **FR-EVT-05** | Any user (including a Visitor) MUST be able to browse a paginated list of Published events, filterable by upcoming/past. |
| **FR-EVT-06** | Any user MUST be able to view an event's detail, including its **individual-ticket price**, any optional packages, and **remaining seats**. |
| **FR-EVT-07** | Remaining seats MUST be computed as `Capacity − seats held by active orders`, never stored as a mutable counter. |
| **FR-EVT-08** | An Admin MUST be able to view all orders and attendees for a given event. |
| **FR-EVT-09** | Cancelling an event (from Published **or** Archived) MUST trigger the cancel ripple: void all Issued tickets, cancel all PendingPayment orders (releasing held seats), and record a `RefundEntry` per Paid order (offline/manual refund). The event is hidden but retained. `Draft → Cancelled` is blocked (D:Q22, Q56; see [[12-SequenceDiagrams#9.2 Event cancel ripple (D:Q22, Q56)|12 §9.2]]). |

### 3.5 Ticket Packages (PRD §6.4)

| Ref | Requirement |
|-----|-------------|
| **FR-PKG-01** | An Admin MAY define zero or more **optional** ticket packages per event, each with a name, seats-per-package (≥ 1), and price (≥ 0, EGP). Packages are discount bundles layered on top of individual tickets; an event with **zero packages is still fully publishable and sellable** via the individual-ticket flow (`FR-ORD-02`). |
| **FR-PKG-02** | A package price of 0 MUST be permitted (a free package). |
| **FR-PKG-03** | An Admin MUST be able to activate/deactivate and soft-delete a package. A package referenced by existing orders MUST NOT be hard-deleted. |
| **FR-PKG-04** | Any user MUST be able to view the active packages and prices for a Published event. |

### 3.6 Promo Codes (PRD §6.4)

| Ref | Requirement |
|-----|-------------|
| **FR-PROMO-01** | An Admin MUST be able to create a promo code with a discount that is either a **percentage** or a **fixed amount** (EGP). |
| **FR-PROMO-02** | A promo code MAY carry a global redemption cap, a per-user redemption limit, a validity window (from/until), and an optional event scope (null = valid on all events). |
| **FR-PROMO-03** | The system MUST reject a promo code that is inactive, outside its validity window, over its global cap, over the user's per-user limit, or scoped to a different event. Caps are counted over `Claimed + Confirmed` redemptions inside the SERIALIZABLE reserve transaction so concurrent buyers can never both slip under the limit (D:Q19, Q50). |
| **FR-PROMO-04** | The system MUST record each redemption as a lifecycle-status ledger row (`Claimed → Confirmed | Released`): the slot is **claimed** at payment initiation (paid orders) or at confirmation (free/100%-off orders); **confirmed** on Paid; **released** on payment failure or hold expiry. Unpaid holds MUST NOT permanently burn a limited promo slot (D:Q19). |
| **FR-PROMO-05** | Promo-code codes MUST be unique among live (non-deleted) codes. |

### 3.7 Ordering & Seat Holds (PRD §6.4)

| Ref | Requirement |
|-----|-------------|
| **FR-ORD-01** | An authenticated user MUST be able to request a **price quote** for an order — **either an individual-ticket order (no package) or a package order** — with a quantity and optional promo, without creating an order or holding seats. Base price = `event.ticketPrice × quantity` for individual tickets, or `package.price × quantity` for a package. The quote MUST show base price, discount, and final price. |
| **FR-ORD-02** | An authenticated user MUST be able to **reserve** an order for a quantity ≥ 1 of **either an individual ticket (default; no package selected) or a single package**. Reserving an individual-ticket order holds `quantity` seats; reserving a package order holds `seats-per-package × quantity` seats. An order is **one unit-type × quantity** — individual tickets and a package MUST NOT be mixed in one order (Q1). A user MUST hold **at most one active (PendingPayment, unexpired) order per event**; a second reserve attempt MUST return the existing pending order rather than creating a duplicate hold (D:Q5). |
| **FR-ORD-03** | An order MUST be rejected if the event's remaining seats are fewer than the requested seat count. This capacity check MUST be **concurrency-safe**: two simultaneous reservations MUST NOT oversell the last seats. |
| **FR-ORD-04** | On reservation, the order MUST **snapshot** the unit price, base price, discount amount, and final price — plus the package name for a package order, or the event title for an individual-ticket order — so later package, event-price, or promo edits never alter historical orders. The package reference is nullable (null on an individual-ticket order). |
| **FR-ORD-05** | A reserved (unpaid) order MUST hold its seats for a **15-minute checkout window**. If payment is not confirmed within that window, the order MUST transition to Expired and its seats MUST be released automatically. |
| **FR-ORD-06** | A user MUST be able to cancel their own unpaid order, releasing its held seats immediately. |
| **FR-ORD-07** | A user MUST be able to view their order history (all statuses) and the tickets belonging to any paid order. |
| **FR-ORD-08** | Orders MUST never be deleted; their lifecycle is expressed only through status. Full lifecycle: `PendingPayment → Paid / Cancelled / Expired`; additionally `Paid → Cancelled` when an Admin voids a paid order (a `RefundEntry` is recorded; D:Q6). A paid-then-voided order and a user-cancelled unpaid order both land in `Cancelled` and are distinguished by the presence of a `RefundEntry`, never by status alone (see [[11-StateMachines#1. Order (D:Q3, Q6, Q55)|11 §1]]). |
| **FR-ORD-09** | A background sweeper MUST transition lapsed `PendingPayment` orders (whose 15-minute window has elapsed without a confirmed payment) to **Expired** and release any claimed promo redemption slot. Seat availability MUST NOT depend on the sweeper having run — a hold with `HoldExpiresAt < now` stops counting against capacity immediately (`FR-EVT-07`, D:Q3); the sweeper is cleanup only. |

### 3.8 Payment (PRD §6.4, Paymob)

| Ref | Requirement |
|-----|-------------|
| **FR-PAY-01** | For an order with a final price > 0, the system MUST initiate an online payment via **Paymob** (cards + wallets, EGP) and return a checkout URL/session to the client. An optional **`Idempotency-Key`** header MUST make a repeated initiation resolve to the **same** checkout session — no duplicate Paymob intention (D:Q28a). |
| **FR-PAY-02** | An order MUST be marked **Paid only** upon a **signature-verified (HMAC) Paymob webhook** confirming success. The system MUST NOT trust a client-reported payment result. |
| **FR-PAY-03** | The webhook handler MUST be **idempotent**: a repeated or replayed callback for an already-paid order MUST NOT issue duplicate tickets. |
| **FR-PAY-04** | The system MUST validate that the amount reported by Paymob matches the order's final price before confirming. |
| **FR-PAY-05** | Each payment attempt MUST be recorded (status, Paymob transaction id, amount, raw verified payload) to support reconciliation and support requests. |
| **FR-PAY-06** | An order with a final price of **0** (free package or 100%-off promo) MUST bypass the gateway and be confirmed immediately. Discount is rounded **half-up to 2 decimal places (EGP)**; `final = max(base − discount, 0)` — an over-large discount yields a free (0.00) order, never a negative charge (D:Q18). |
| **FR-PAY-07** | Refunds are **manual/offline** for the current scope: an Admin cancelling a Paid order MUST void its tickets and record a refund entry; no automated gateway refund is required yet. |

### 3.9 Tickets & Check-in (PRD §6.4)

| Ref | Requirement |
|-----|-------------|
| **FR-TKT-01** | When an order becomes Paid, the system MUST issue exactly **one ticket per held seat**, each with a unique QR token and a short human-readable public reference. |
| **FR-TKT-02** | A reserved (unpaid) order MUST have **zero tickets**. |
| **FR-TKT-03** | Each ticket MAY carry an optional guest name; a nameless ticket MUST still be a fully valid credential. Guests are not required to have accounts. |
| **FR-TKT-04** | The QR token MUST encode a **public reference** (indexed, non-secret) **and a 256-bit random secret**. The DB stores only the reference and a **SHA-256 hash** of the secret; the raw secret is never persisted and exists only inside the QR image (D:Q8). |
| **FR-TKT-05** | An Admin MUST be able to check in a ticket by scanning its QR at the venue. The scan endpoint is **event-scoped** (D:Q9). Five distinct outcomes MUST be returned: **success** (`Issued → CheckedIn`, scanner + timestamp recorded); **already-checked-in** (returns original scanner + time); **wrong-event** (valid ticket, wrong door); **voided** (`TICKET_VOIDED` — a known ticket whose paid order was voided/refunded, distinct from a forgery); **unknown/invalid** (no matching reference or secret fails the hash comparison). A ticket MUST be checkable in **at most once** (D:Q7, Q9). |
| **FR-TKT-06** | Check-in MUST record who scanned and when. Rejected/duplicate scan attempts MUST be logged (not silently ignored). |

### 3.10 Track & Session Management (PRD §6.7–6.8)

| Ref | Requirement |
|-----|-------------|
| **FR-TRK-01** | An Admin MUST be able to create, edit, and soft-delete tracks (name, description, schedule). Track names MUST be unique among live tracks. Soft-deleting a track MUST **auto-end all active Member enrollments and the Board assignment** (`EndedAt` set, all history retained, dual-role slots freed), behind an **Admin confirmation stating the impact** (D:Q14). |
| **FR-TRK-02** | A Board or Admin MUST be able to create and edit **sessions** within a track (topic, date, time, location). A Board may only manage sessions of the track they supervise. A session with existing attendance or evaluation records MUST NOT be hard-deleted — soft-delete/cancel only; a records-free session may be removed outright (D:Q13). |
| **FR-TRK-03** | A Member MUST be able to view the upcoming and past sessions of their own track. |
| **FR-TRK-04** | A Board and Admin MUST be able to view a track's full details: members, sessions, and progress summaries. |

### 3.11 Attendance (PRD §6.9)

| Ref | Requirement |
|-----|-------------|
| **FR-ATT-01** | A Board MUST be able to record each member's attendance for a session as **Present**, **Late**, or **Absent**. Attendance is recorded **manually** (no QR scan for training in current scope). |
| **FR-ATT-02** | There MUST be at most **one attendance record per member per session**; re-recording updates the existing record. |
| **FR-ATT-03** | A Member MUST be able to view their own attendance percentage, where percentage = (Present + Late) ÷ **the sessions that have occurred and have a recorded attendance entry for that enrollment** (future sessions and past sessions with no recorded entry are excluded; an Absent MUST be recorded explicitly to count — D:Q12). The percentage is scoped to the member's **current active enrollment** (D:Q11). **Late counts as attended.** |
| **FR-ATT-04** | A Board MUST be able to view attendance for all members of their track; an Admin MUST be able to view attendance across all tracks. |

### 3.12 Evaluations (PRD §6.10)

| Ref | Requirement |
|-----|-------------|
| **FR-EVL-01** | A Board MUST be able to evaluate a member after a session with a **score 0–100** (integer; values outside this range MUST be rejected) and optional text feedback. Evaluation requires the **session date to be in the past** and the member to have an **active enrollment at evaluation time**; attendance is not a prerequisite (D:Q16, Q17). |
| **FR-EVL-02** | There MUST be at most **one evaluation per member per session**, editable in place (no duplicates). |
| **FR-EVL-03** | A Member MUST be able to view their own evaluation history (scores + feedback); they MUST NOT see other members' evaluations. |
| **FR-EVL-04** | A Board MUST be able to view evaluations for all members of their track. |

### 3.13 Notifications (PRD §6.13)

| Ref | Requirement |
|-----|-------------|
| **FR-NTF-01** | An Admin MUST be able to send platform-wide or role-scoped in-app notifications. Recipients MUST be **resolved and fanned out to per-recipient rows at send time** (a snapshot — later enrollees do NOT retroactively receive past notifications). Audiences: platform-wide (all active), by global role (Attendees / Admins), or by track (D:Q21). |
| **FR-NTF-02** | A Board MUST be able to send in-app notifications to the members of their own track only. |
| **FR-NTF-03** | Each recipient MUST have their own read state; a user MUST be able to view their inbox and mark notifications as read. |
| **FR-NTF-04** | Notifications are **in-app only** in current scope (no email/SMS/push beyond the password-reset email). |

### 3.14 Contact & Public Pages (PRD §6.5)

| Ref | Requirement |
|-----|-------------|
| **FR-PUB-01** | Visitors MUST be able to view public pages: Home, About, Team, Events, Event Detail, Contact, and the auth pages (Login, Register, Forgot Password). |
| **FR-PUB-02** | A visitor MUST be able to submit a **contact form** (name, email, subject ≤ 200 chars, message ≤ 2000 chars); submissions are stored with status New/Read/Archived for **Admin-only** review. There is **no in-app reply** and no Board/Member notification. The endpoint MUST be protected by **IP rate-limiting** and field-length caps; email format MUST be validated. No CAPTCHA in current scope (D:Q20). |
| **FR-PUB-03** | Team and Partners/Sponsors content is **static** in current scope (no admin editing, no dedicated tables). |
| **FR-PUB-04** | Public pages MUST be responsive and mobile-friendly. |

---

## 4. Non-functional requirements

Non-functional requirements are grouped by quality attribute. Each is testable; where a number is given, it is a target to verify, not a hard SLA.

### 4.1 Performance & scalability

| Ref | Requirement |
|-----|-------------|
| **NFR-PERF-01** | Under the initial target load (**< 100 concurrent users**, hundreds of attendees per event), API endpoints SHOULD respond within **500 ms at p95**, excluding third-party (Paymob) round-trips. |
| **NFR-PERF-02** | Public pages SHOULD achieve a first-contentful paint within **2 s** on a typical mobile connection. |
| **NFR-PERF-03** | The API MUST be **stateless** (no server-side session affinity); the only persisted server state is the refresh token in the database, so the API can scale horizontally behind a load balancer. |
| **NFR-PERF-04** | The seat-availability calculation and order placement MUST remain correct and performant under concurrent booking of the same event (see NFR-REL-01). |
| **NFR-PERF-05** | Public event and package listings MAY be cached (in-memory now, Redis later) with a bounded staleness; caching MUST NOT be used for seat-availability decisions, which MUST read committed data. |

### 4.2 Reliability & data integrity

| Ref | Requirement |
|-----|-------------|
| **NFR-REL-01** | Concurrent orders for the last remaining seats of an event MUST NOT oversell. The reserve operation MUST be serialized (transaction at `SERIALIZABLE` isolation, or equivalent) so total held+paid seats never exceed capacity. |
| **NFR-REL-02** | The Paymob webhook handler MUST be **idempotent**: a repeated or replayed callback for an already-settled transaction MUST NOT issue duplicate tickets or double-count seats. |
| **NFR-REL-03** | Financial records (Orders, Tickets, Payments) MUST never be hard-deleted; state changes only. |
| **NFR-REL-04** | Order prices MUST be **snapshotted** at reserve time; later edits to a package price or promo code MUST NOT alter historical orders. |
| **NFR-REL-05** | All timestamps MUST be stored in **UTC**; localization to the user's timezone happens at the presentation layer. |
| **NFR-REL-06** | Concurrent edits to the same admin-managed record (Event, Order, Package, Promo Code) MUST be detected via optimistic concurrency (`rowversion`) and surfaced rather than silently overwritten. |
| **NFR-REL-07** | External side-effects (confirmation/notification email) MUST be dispatched via a **transactional outbox**: the outbox row is written **inside** the business transaction that changes state, and delivered **after** commit by the background sweeper with retry/backoff — **at-least-once**, consumers idempotent. No external side-effect fires inside the money transaction (D:Q45, Q53; see [[12-SequenceDiagrams#6. Outbox drain + hold expiry (D:Q3, Q34, Q45, Q53)|12 §6]]). |

### 4.3 Security

| Ref | Requirement |
|-----|-------------|
| **NFR-SEC-01** | Passwords MUST be stored using a strong one-way hash (ASP.NET Identity default). Plaintext passwords MUST never be logged or persisted. |
| **NFR-SEC-02** | Authentication MUST use short-lived JWT access tokens and rotating, single-use refresh tokens stored **hashed**; reuse of a consumed refresh token MUST revoke the token family. |
| **NFR-SEC-03** | Authorization MUST enforce both the global role and per-track policies server-side on every protected endpoint; the client MUST NOT be trusted for access decisions. |
| **NFR-SEC-04** | The Paymob webhook MUST verify the **HMAC signature** before acting; unsigned or mismatched callbacks MUST be rejected. The payment amount MUST be validated against the order's snapshotted final price. |
| **NFR-SEC-05** | QR ticket tokens MUST be cryptographically random and stored **hashed**; the raw token exists only in the QR image, never in the database. |
| **NFR-SEC-06** | All traffic MUST be over HTTPS. CORS MUST restrict origins to the known frontend(s). |
| **NFR-SEC-07** | All input MUST be validated at the API boundary (schema/`FluentValidation`); database access MUST be parameterized (EF Core) to prevent injection. |
| **NFR-SEC-08** | Secrets (connection strings, JWT keys, Paymob keys, Cloudinary keys) MUST come from configuration/environment, never source control. |
| **NFR-SEC-09** | Sensitive actions (payment settlement, role/assignment changes, check-in) MUST be attributable via audit columns (who + when). |
| **NFR-SEC-10** | Rate limiting SHOULD protect authentication and ordering endpoints against abuse. |

### 4.4 Usability & accessibility

| Ref | Requirement |
|-----|-------------|
| **NFR-USE-01** | The UI MUST be responsive across mobile, tablet, and desktop breakpoints. |
| **NFR-USE-02** | The UI MUST be **i18n-ready**: no hardcoded user-facing strings, locale-aware date/number/currency formatting, and layout that can flip to RTL without a rewrite. English is the only shipped locale in current scope. |
| **NFR-USE-03** | User-facing errors MUST be clear and actionable; internal error detail MUST NOT leak to the client. |
| **NFR-USE-04** | Interactive elements SHOULD meet WCAG 2.1 AA guidance where feasible (contrast, keyboard navigation, labels). |

### 4.5 Maintainability & observability

| Ref | Requirement |
|-----|-------------|
| **NFR-MNT-01** | The backend MUST follow a layered architecture that keeps domain logic independent of framework and infrastructure concerns. |
| **NFR-MNT-02** | The three bounded contexts (Identity, Eventing/Ticketing, Training) MUST use **real database foreign keys with `RESTRICT` delete** across context boundaries (D:Q FK revision). Decoupling is a **code rule**: no cross-context navigation properties or direct aggregate references in application code — contexts relate only through the account id at the code level. The prior "no foreign keys across contexts" wording is superseded by the FK revision. |
| **NFR-MNT-03** | The system MUST emit structured logs (Serilog or equivalent) for requests, errors, payment events, and check-in attempts (including rejected/duplicate scans). |
| **NFR-MNT-04** | Configuration MUST be environment-specific (development/production) without code changes. |

### 4.6 Compatibility & portability

| Ref | Requirement |
|-----|-------------|
| **NFR-CMP-01** | The frontend MUST support current versions of Chrome, Firefox, Safari, and Edge. |
| **NFR-CMP-02** | The API MUST be client-agnostic (REST + JSON) so a future mobile client can consume it without server changes. |
| **NFR-CMP-03** | Money MUST be represented as `decimal` with an explicit currency (EGP) internally, converting to Paymob's minor units only at the gateway boundary. |

---

## 5. Data requirements

The complete entity model, columns, constraints, and indexes are specified in **[10 — Data Model](./10-DataModel.md)**, which is authoritative for the database layer. The SRS defers to it rather than duplicating schema. Key data-level requirements that functional behavior depends on:

- **Referential integrity & delete behavior** — financial and training history use `RESTRICT`; pure dependents (refresh tokens, notification recipients) use `CASCADE`.
- **Soft delete** — catalog/identity entities carry `IsDeleted`/`DeletedAt` with query filters; financial records are never deleted.
- **Uniqueness invariants** — enforced physically: hashed QR token, promo code, Paymob transaction id, one attendance/evaluation per (session, enrollment), and the two filtered unique indexes enforcing the Member/Board caps.

---

## 6. External interface requirements

| Ref | Requirement |
|-----|-------------|
| **EIR-01 — Paymob** | The system integrates with Paymob for card and wallet payments in EGP: create a payment intention, redirect/iframe for the buyer, and receive a signed webhook confirming the result. Concrete request/response contracts are in the [API Contract (07)](./07-ApiContract.md) §10. |
| **EIR-02 — Cloudinary** | Profile pictures and event images are uploaded to Cloudinary; only URLs are stored. |
| **EIR-03 — Email (SMTP)** | Transactional email (password reset) is sent via an SMTP provider. |
| **EIR-04 — API surface** | The platform exposes a versioned REST API (`/api/v1`) consumed by the SPA; the concrete endpoint list, payloads, and error envelopes live in the [API Contract (07)](./07-ApiContract.md). |

---

## 7. Traceability

Every functional requirement above traces upward to a PRD capability (section references in each heading) and downward to the Data Model entities it operates on. The downstream traceability layer is complete:

- **User Stories (05)** — one story per user-facing behaviour, each citing the `FR-*` it refines.
- **Acceptance Criteria (06)** — Gherkin scenarios per story, each citing the `FR-*` and decision references.
- **API Contract (07)** — per-endpoint request/response shapes, error codes, and `Implements: US-*/AC-*` links.
- **System Design (09)** — the implementation architecture these requirements are realized in; entity lifecycle diagrams are in [[11-StateMachines]], runtime flow diagrams in [[12-SequenceDiagrams]].
- **Data Model (10)** — the authoritative schema; this SRS defers all schema detail to it.

---

## 8. Open items

These are acknowledged and deferred, not gaps:

- **Automated gateway refunds** — manual/offline in current scope (see PRD §10).
- **Real-time (SignalR), email/SMS notifications beyond password reset, and analytics** — later enhancements (PRD Area C).
- **Testing** — strategy agreed (risk-weighted pyramid, Testcontainers for concurrency/money paths); authoring deferred until stakeholder go-ahead (D:Q43).
