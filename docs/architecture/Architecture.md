# TEDxAlkawmia Platform — Architecture Overview

> **Version:** 1.0
> **Date:** 2026-07-22
> **Status:** Authoritative for system design & data model — the basis for the code scaffold
> **Decisions:** every choice here traces to [08 — Decision Log](../08-DecisionLog.md) **Q29–Q55** (cited inline as **D:Qn**)
> **Reads from:** [01 — PRD](../01-PRD.md) · [02 — SRS](../02-SRS.md) (v1.4) · [03 — User Flows](../03-UserFlows.md) · [04 — Personas](../04-Personas.md) · [05 — User Stories](../05-UserStories.md) · [06 — Acceptance Criteria](../06-AcceptanceCriteria.md) · [07 — API Contract](../07-ApiContract.md)

---

## 0. How to read this document set

This `architecture/` folder is the **system-design + data-model layer** (the "09/10" the PRD/SRS point to as pending). It sits *below* the requirement docs and *above* the code: requirements say **what**, this set says **how the system is shaped**, and the code implements it.

| Doc | Answers |
|-----|---------|
| **Architecture.md** (this file) | The big picture: style, layers, contexts, the load-bearing rules, and the guiding philosophy. |
| [C4/Context.md](./C4/Context.md) | System-in-the-world: who and what talks to the platform. |
| [C4/Container.md](./C4/Container.md) | The deployable pieces (SPA, API, DB, external services) and how they connect. |
| [C4/Component.md](./C4/Component.md) | Inside the API: layers, MediatR pipeline, the three contexts. |
| [C4/Deployment.md](./C4/Deployment.md) | Runtime topology, single-instance constraints, config/secrets. |
| [Database.md](./Database.md) | Every table, column, index, and the invariants the schema enforces. |
| [ERD.md](./ERD.md) | The entity-relationship diagram + relationship rules (incl. the cross-context FK rule). |
| [ClassDiagrams.md](./ClassDiagrams.md) | Domain aggregates and their methods (the rich-domain slice). |
| [StateMachines.md](./StateMachines.md) | Order / Ticket / Event lifecycles and legal transitions. |
| [SequenceDiagrams.md](./SequenceDiagrams.md) | The critical flows: reserve, pay-webhook, check-in, refresh, sweeper. |
| [APIArchitecture.md](./APIArchitecture.md) | Request lifecycle, Result envelope, error taxonomy, cross-cutting middleware. |

**A note on scope.** This set describes the *target* architecture. It is a **design document, not a build order** — in particular the **testing strategy is documented but its authoring is deferred** until the stakeholder green-lights it (D:Q43). Nothing here should be read as "scaffold this now"; it is the shared blueprint the team reads before we cut the first project.

---

## 1. Guiding philosophy — proportional design

Every decision in D:Q29–Q55 was taken against one rule:

> **Keep each architectural decision proportional to a problem this project actually has.**

The concrete shape of "this project":
- A **modular monolith** on a **single SQL Server** database.
- **No microservice split planned**; no external API consumers; one SPA and one API evolving together.
- A **two-developer** backend team.
- Real, non-negotiable invariants around **money, seats, and roles** (oversell, double-charge, double-check-in, dual-role legality).

So we adopt the patterns that earn their place against *those* problems — Clean Architecture, CQRS-lite, MediatR, rich domain for the invariant-bearing aggregates, the Result pattern, structured logging, a serializable reserve path — and we **decline** the ones whose only payoff is a future we've decided not to build (no-cross-context-FK for physical extraction, API versioning machinery, a domain-event bus, per-context DbContexts, generic repositories). This is deliberately **not** a full DDD/enterprise showcase; it is a clean, maintainable, production-quality system a small team can own.

---

## 2. Architecture style (D:Q29)

**Modular monolith + Clean Architecture**, one deployable API, four projects with a strict dependency direction (inward only):

```
┌─────────────────────────────────────────────┐
│                     Api                       │  Controllers, middleware, DI, Program.cs
│   (depends on Application + Infrastructure)    │
├─────────────────────────────────────────────┤
│                Infrastructure                 │  EF Core, Identity, Paymob/Cloudinary/SMTP
│        (depends on Application + Domain)       │  clients, Serilog sinks, outbox drain
├─────────────────────────────────────────────┤
│                 Application                   │  MediatR handlers, DTOs, validators,
│           (depends on Domain only)             │  IApplicationDbContext, ICurrentUser, IClock
├─────────────────────────────────────────────┤
│                    Domain                     │  Entities/aggregates, enums, domain rules,
│              (depends on nothing)              │  invariants — pure POCO, no EF/framework
└─────────────────────────────────────────────┘
```

- **Domain** — pure POCO (D:Q31). Entities, value objects, enums, and the transition methods that hold invariants (D:Q32, D:Q55). No EF, no attributes, no framework types.
- **Application** — use-cases as MediatR commands/queries + handlers (D:Q30); FluentValidation validators (D:Q39); manual DTO mapping (D:Q38); the `IApplicationDbContext`, `ICurrentUser`, `IClock` abstractions (D:Q31, Q35); the `Result<T>` type and `Errors` catalog (D:Q37).
- **Infrastructure** — EF Core `DbContext` (implements `IApplicationDbContext`), ASP.NET Core Identity, the Paymob/Cloudinary/SMTP clients, Serilog configuration, the background sweeper + outbox drain, migrations.
- **Api** — thin controllers under `/api/v1`, the middleware stack, DI wiring, `Program.cs`.

### 2.1 Bounded contexts as folders (D:Q29a)

Three contexts, expressed as **folders inside each layer** (not separate assemblies):

| Context | Owns | Key entities |
|---------|------|--------------|
| **Identity** | Accounts, auth, global role, tokens | `ApplicationUser`, `RefreshToken` |
| **Eventing/Ticketing** | Events, packages, orders, tickets, promos, payments | `Event`, `Package`, `Order`, `Ticket`, `PromoCode`, `PromoRedemption` |
| **Training** | Tracks, assignments, sessions, attendance, evaluations | `Track`, `TrackAssignment`, `Session`, `Attendance`, `Evaluation` |

Boundaries are kept by **convention, enforced in code review** — see §4.

---

## 3. The load-bearing rules

These are the rules the whole design leans on. If one is violated, an invariant breaks — so they're stated once, here, and referenced everywhere.

1. **One order = one unit-type × quantity** (D:Q1). Individual ticket *or* one package, never mixed. This is why `Order` has **no line-item table** (D:Q49).
2. **Model B** (D:Q1 addendum): every event sells **individual tickets at `Event.TicketPrice`**; packages are **optional** bundles; **`Order.PackageId` is nullable** (null ⇒ individual); a **zero-package event is publishable and sellable**.
3. **Held seats are computed, never stored** (D:Q3, Q33): `SUM(Quantity)` over `Paid OR (PendingPayment AND HoldExpiresAtUtc > now)`. Availability is correct even if the sweeper is late.
4. **The reserve path is `SERIALIZABLE` + Polly retry** (D:Q33). This is the single guarantee against oversell.
5. **Money is snapshotted at reserve** (D:Q4, Q49): the order stores unit/subtotal/discount/total; the server re-prices and returns `PRICE_CHANGED` (409) on mismatch. The client never sends a price.
6. **Tickets issue only on a signature-verified Paymob webhook** (D:Q49, SequenceDiagrams §2), inside one transaction; the webhook is **idempotent** (re-`MarkAsPaid()` is a no-op success, D:Q55).
7. **Financial records are append-only** (D:Q49): orders/tickets/redemptions are never deleted; cancel is a **status**.
8. **QR secret is stored hashed** (SHA-256), raw never persisted (D:Q8, Q49).
9. **Dual-role legality** (D:Q51): ≤1 Member track and ≤1 Board track per user (two filtered unique indexes), and they must be **different tracks** (domain invariant).
10. **Secrets never in code or logs** (D:Q40, Q41): config/env only, fail-fast at startup, scrubbing policy on logs.

---

## 4. Cross-context coupling — the code rule (FK revision, D:Q51 addendum)

On a single database we **keep real foreign keys across contexts** for referential integrity, but we **forbid cross-context coupling in code**:

- ✅ **Real FK** `Order.AccountId → ApplicationUser.Id`, with **`DeleteBehavior.Restrict`** (never cascade — accounts are soft-deleted, financial rows are append-only). Same for `TrackAssignment.AccountId`, `PromoRedemption.AccountId`, `Attendance.AccountId`, `Evaluation.AccountId`.
- ❌ **No cross-context EF navigation properties.** A handler in Ticketing never writes `order.Account.Email`; it holds an `AccountId` GUID and, if it needs the email, asks the Identity context through its own query/service.
- ✅ Intra-context navigation is fine (`Event.Packages`, `Order.Tickets`, `Session` ↔ `Attendance`).

**Why both:** referential integrity is a free, always-on correctness guarantee on one DB; the no-navigation rule keeps the contexts genuinely separable. If the system is ever split, dropping the FKs is a migration — and because code never traversed the boundary, the seam stays clean. This **supersedes** the earlier "related only by account id, no cross-context FK" reading of NFR-MNT-02.

**Review checklist item:** *"Does this change add an EF navigation property, `Include`, or join across context boundaries?"* → if yes, reject; reference by `AccountId` instead.

---

## 5. Request lifecycle at a glance (detail in [APIArchitecture.md](./APIArchitecture.md))

```
HTTP → [Api] Controller (/api/v1, [Authorize] global role)
     → MediatR Send(command/query)
        → LoggingBehavior      (correlationId, structured log)
        → ValidationBehavior   (FluentValidation → 422 VALIDATION_ERROR)
        → AuthorizationBehavior (ICurrentUser + marker ifaces → per-track scope, 403)
        → Handler              (explicit tx where needed; returns Result<T>)
            → IApplicationDbContext / Domain aggregate methods
     ← Result<T>
     → Result→ActionResult mapper (Error.type → 422/409/404/401/403; success → 200/201)
     ← JSON { success, data, error }
```

Unexpected exceptions are caught by `ExceptionHandlingMiddleware` → **500 + correlationId** (D:Q37). Expected business outcomes are **values**, not exceptions.

---

## 6. Technology summary

| Concern | Choice | Decision |
|---------|--------|----------|
| API | ASP.NET Core Web API (.NET 8, C#) | scope |
| SPA | React + Vite + TypeScript | scope |
| DB | SQL Server + EF Core (single `DbContext`) | D:Q29b |
| Mediation | MediatR + pipeline behaviors | D:Q30 |
| Data access | `IApplicationDbContext`, no generic repos | D:Q31 |
| Validation | FluentValidation | D:Q39 |
| Result/errors | typed `Result<T>` + `Errors` catalog + mapper | D:Q37 |
| Mapping | manual | D:Q38 |
| Identity | ASP.NET Core Identity (store/hash/reset) + custom JWT/refresh | D:Q36, Q46, Q47 |
| Concurrency | SERIALIZABLE reserve + Polly retry; `RowVersion` | D:Q33, Q54 |
| Background | `BackgroundService` + `sp_getapplock` | D:Q34 |
| Side-effects | explicit orchestration + transactional outbox | D:Q45, Q53 |
| Logging | Serilog structured JSON + correlationId + scrubbing | D:Q41 |
| Config/secrets | `IOptions<T>` + User Secrets/env + fail-fast | D:Q40 |
| Migrations | code-first, explicit bundle, seeder + first-Admin | D:Q42 |
| Payments | Paymob (piastres at the boundary, HMAC webhook) | D:Q18, Q24 scope |
| Images | Cloudinary | scope |
| Email | SMTP (password-reset only) | scope |
| API versioning | none now; `/api/v1` literal prefix | D:Q44 |
| Testing | risk-weighted pyramid — **deferred authoring** | D:Q43 |

---

## 7. What is deliberately deferred

Named here so no reviewer mistakes an omission for a gap:

- **Test suite authoring** (strategy agreed, D:Q43) — until explicit go.
- **Per-context DbContexts** (D:Q29b) — single context now.
- **API versioning machinery** (D:Q44) — prefix kept, library not.
- **Domain-event bus** (D:Q45) — explicit orchestration + outbox instead.
- **Cloud secret manager** (D:Q40) — env vars now, drop-in provider later.
- **Hangfire / external scheduler** (D:Q34) — in-process sweeper now.
- Everything in the PRD/SRS **out-of-scope** list (D:Q28c): mobile app, SignalR real-time, automated gateway refunds, extra payment channels, analytics beyond `RPT-*`.

---

*Version 1.0 — 2026-07-22. Authoritative system-design overview; all choices trace to Decision Log Q29–Q55. Where this conflicts with an earlier doc, the Decision Log prevails.*
