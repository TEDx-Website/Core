# ERD — Entity Relationships

> **Version:** 1.0 · **Date:** 2026-07-22 · Part of [Architecture Overview](./Architecture.md)
> **Decisions:** D:Q46–Q55 + the FK revision · **Companion:** [Database.md](./Database.md) (columns + indexes)

---

## Purpose

The relationship view of the schema: which tables reference which, the delete behavior on each edge, and the one rule that shapes the whole diagram — **cross-context edges are real FKs but carry no navigation property in code** (D:Q51 revision).

## Reading the diagram

- **Solid intra-context edges** — normal EF relationships *with* navigation properties (e.g. `Event ↔ Package`, `Track ↔ Session`).
- **Dashed cross-context edges** — real DB FKs to `ApplicationUser.Id` with `DeleteBehavior.Restrict`, **but no C# navigation property**; the owning entity holds only an `AccountId` GUID (D:Q51 revision, [Architecture §4](./Architecture.md)).

```mermaid
erDiagram
    ApplicationUser ||..o{ RefreshToken : "AccountId (Restrict)"
    ApplicationUser ||..o{ Order : "AccountId (Restrict)"
    ApplicationUser ||..o{ PromoRedemption : "AccountId (Restrict)"
    ApplicationUser ||..o{ TrackAssignment : "AccountId (Restrict)"
    ApplicationUser ||..o{ Attendance : "AccountId (Restrict)"
    ApplicationUser ||..o{ Evaluation : "AccountId (Restrict)"

    Event ||--o{ Package : "EventId (Cascade, intra)"
    Event ||--o{ Order : "EventId (Restrict)"
    Event ||--o{ Ticket : "EventId (Restrict, denormalized)"
    Package |o--o{ Order : "PackageId? (Restrict)"
    Order ||--o{ Ticket : "OrderId (Cascade, intra)"
    PromoCode |o--o{ Order : "PromoCodeId? (Restrict)"
    PromoCode ||--o{ PromoRedemption : "PromoCodeId (Restrict)"
    Order ||--o{ PromoRedemption : "OrderId (Restrict)"
    Event |o--o{ PromoCode : "EventId? scoped (Restrict)"

    Track ||--o{ TrackAssignment : "TrackId (Restrict, intra)"
    Track ||--o{ Session : "TrackId (Cascade, intra)"
    Track ||--o{ Evaluation : "TrackId (Restrict)"
    Session ||--o{ Attendance : "SessionId (Cascade, intra)"
    Session |o--o{ Evaluation : "SessionId? (Restrict)"

    ApplicationUser {
        Guid Id PK
        string GlobalRole
        bool IsActive
    }
    RefreshToken {
        Guid Id PK
        Guid AccountId FK
        string TokenHash
    }
    Event {
        Guid Id PK
        decimal TicketPrice
        int Capacity
        string Status
    }
    Package {
        Guid Id PK
        Guid EventId FK
        decimal Price
        int SeatsPerPackage
    }
    Order {
        Guid Id PK
        Guid AccountId FK
        Guid EventId FK
        Guid PackageId FK "nullable"
        string Status
        datetime HoldExpiresAtUtc
    }
    Ticket {
        Guid Id PK
        Guid OrderId FK
        Guid EventId FK
        string QrSecretHash
        string Status
    }
    PromoCode {
        Guid Id PK
        Guid EventId FK "nullable"
        string Code
    }
    PromoRedemption {
        Guid Id PK
        Guid PromoCodeId FK
        Guid AccountId FK
        Guid OrderId FK
    }
    Track {
        Guid Id PK
        string Slug
    }
    TrackAssignment {
        Guid Id PK
        Guid AccountId FK
        Guid TrackId FK
        string TrackRole
    }
    Session {
        Guid Id PK
        Guid TrackId FK
        string Status
    }
    Attendance {
        Guid Id PK
        Guid SessionId FK
        Guid AccountId FK
        string Status
    }
    Evaluation {
        Guid Id PK
        Guid AccountId FK
        Guid TrackId FK
        Guid SessionId FK "nullable"
        int Score
    }
```

*(`OutboxMessage` has no FKs — it's a standalone cross-cutting table; omitted from the diagram.)*

## Delete-behavior rules

| Edge kind | Behavior | Why |
|-----------|----------|-----|
| **Cross-context → ApplicationUser** | `Restrict` | Never cascade-delete a user's orders/records; accounts are soft-deleted anyway (D:Q54). Restrict makes an accidental hard-delete fail loudly. |
| **Intra-aggregate** (`Event→Package`, `Order→Ticket`, `Track→Session`, `Session→Attendance`) | `Cascade` OK | Child has no meaning without its parent, same context, same aggregate. |
| **Intra-context reference** (`Event→Order`, `Track→Assignment`, `PromoCode→…`) | `Restrict` | These are catalog rows referenced by transactional history; deleting them out from under a paid order must fail. Catalog rows use soft-delete instead. |

## The cross-context rule, restated

Every dashed edge above is enforced by a real `FOREIGN KEY` constraint in SQL Server — referential integrity is **on**. What's absent is the C# side: `Order` has an `AccountId` property, **not** an `ApplicationUser Account { get; set; }` navigation. A handler that needs buyer details queries the Identity context by `AccountId`; it cannot lazy-walk from `Order` into `ApplicationUser`. This keeps the contexts decoupled in code (D:Q51 revision) without giving up the database's integrity guarantees — the proportional resolution recorded in [Architecture §4](./Architecture.md).

---

*ERD v1.0 — 2026-07-22. Columns + indexes in [Database.md](./Database.md).*
