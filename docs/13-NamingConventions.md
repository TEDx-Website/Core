# TEDxAlkawmia — Naming Conventions

> **Version:** 1.1
> **Date:** 2026-08-16
> **Applies to:** `TEDx-Backend` (C#) and `TEDx-Frontend` (TypeScript/React) — every new file, every PR.
> **Reads from:** [07 — API Contract](./07-ApiContract.md) · [09 — System Design](./09-SystemDesign.md) · [10 — Data Model](./10-DataModel.md)
> **Status:** **Normative.** A reviewer may reject a PR on this document alone. Where it conflicts with personal habit, this document wins.
>
> **Why this exists.** A name is the only documentation a teammate reads before they read the code. When `ChangeEventStatusDTO` and `CancelEventResponse` are the same *kind* of thing under two different names, nobody can tell from a search result what a type is for — they have to open it. Every rule below exists to make the answer to "what is this?" readable from the identifier alone.

---

## 0. The single decision that causes the most confusion

Three suffixes, three jobs. Pick with these questions, in this order:

| Question | Answer | Suffix | Lives in |
|---|---|---|---|
| Is this the **body a client sends in**? | yes | **`Request`** | `TEDx.Api/Requests/<Area>/` |
| Is this the **complete `data` payload one endpoint sends back**? | yes | **`Response`** | next to its command/query, in the feature folder |
| Is it a **piece inside** a Response, or **shared** by more than one? | yes | **`Dto`** | `<Area>/Dtos/`, or `Common/Dtos/` if cross-area |

These three rows are about **payload** types. A type whose name happens to end in `Request` without being an inbound body is not covered and does not move: MediatR's own `IRequest`, the marker `ITrackScopedRequest`, and the query-string binder `PagedRequest` all keep their names and locations.

### 0.1 `Response` — the whole payload of one operation

`Response` means: *this object, serialized, is exactly what lands in the envelope's `data` field for one endpoint.*

```csharp
// POST /api/v1/admin/events/{id}/status  →  data: { status, rowVersion }
public sealed record ChangeEventStatusResponse(EventStatus Status, string RowVersion);
```

Name it after the **operation**, not the entity: `ChangeEventStatusResponse`, `CancelEventResponse`, `RegisterResponse`.

**One exception, deliberately allowed:** `GET` and `PUT` of the *same resource* return the same representation and therefore share one `Response` (`GET /me` and `PUT /me` both return `MyProfileResponse`). That is one payload reachable two ways, not two payloads. Two *different* operations never share a `Response` — if you are tempted, what you actually have is a `Dto`.

### 0.2 `Dto` — a part, or a shared shape

`Dto` means: *this is a building block. It never appears alone at the top of a response.*

```csharp
public sealed record MoneyDto(decimal Amount, string Currency);          // shared by many payloads
public sealed record EventOrderDto(Guid Id, /* … */ MoneyDto Total);     // one row of a list payload
public sealed record TrackAssignmentDto(Guid? MemberOfTrackId, Guid? BoardOfTrackId);  // nested in MyProfileResponse
```

Name it after the **thing**, not the operation. A list endpoint's element is a `Dto` (`AdminEventListItemDto`), because the payload is the *list*, not the item.

### 0.3 `Request` — only when the wire body is not the command

Most endpoints bind the `Command`/`Query` straight from the body; no `Request` type is needed. Add one only when the API layer has real work to do first:

- the id comes from the **route** and the rest from the body (`ChangeEventStatusRequest`), so the body cannot carry a second, conflicting id;
- a field needs decoding before the Application layer sees it (`UpdateEventRequest.RowVersion` is base64 on the wire, `byte[]` in the command).

A `Request` never leaves `TEDx.Api`. The Application layer knows only commands and queries.

### 0.4 Never `DTO`

Write **`Dto`**, not `DTO`. Framework Design Guidelines: acronyms of three letters or more are Pascal-cased. So: `Dto`, `Api`, `Url`, `Jwt`, `Smtp`, `Html`, `Db`. Two-letter acronyms stay upper (`IO`). `Id` is a shortened word, not an acronym — `Id`.

---

## 1. MediatR triad

For a feature called `X`:

| Type | Name | Shape |
|---|---|---|
| Request | `XCommand` / `XQuery` | `public sealed record` |
| Handler | `XCommandHandler` / `XQueryHandler` | `public sealed class` |
| Validator | `XCommandValidator` / `XQueryValidator` | `public sealed class` |

**The `Command`/`Query` infix is never dropped.** `GetEventOrdersQueryHandler`, not `GetEventOrdersHandler`. The suffix is what tells a reader whether the thing writes or reads, and a bare `…Handler` in a search result tells them nothing.

**Naming the operation.** Imperative verb + subject: `CreateEvent`, `ChangeEventStatus`, `GetEventOrders`. The subject is **plural only when the result is a collection** (`GetEventOrdersQuery` returns many orders; `CreateEventCommand` creates one event).

**Every command and query gets a validator** — even a one-line `NotEmpty()` on an id. A missing validator file must read as an omission, not as a deliberate "nothing to check". If there is genuinely nothing to validate, write the validator with a comment saying so.

**Handlers are `public`.** An `internal` handler still works (MediatR finds it by assembly scan) but it breaks the symmetry that makes the folder skimmable.

---

## 2. Files and folders

### 2.1 One public type per file, and the file is named after it

The file name must equal the type name. `MoneyDto` lives in `MoneyDto.cs`. If you rename a type, rename the file in the same commit.

The rule governs **top-level** types. A nested type is a *member* of its container, reached as `Container.Nested`, so §2.1 does not automatically apply to it — see the third exception below for when it does.

**The closed list of allowed exceptions** — nothing else:

1. `TOptions` + `TOptionsValidator` in one file (they are a unit; the validator has no independent meaning).
2. A partial type split by aspect, named `Type.Aspect.cs` — e.g. `CommonErrors.Sorting.cs`. This is the standard C# convention and is encouraged for long error catalogs.
3. A nested type with **no consumer outside its containing file**. It cannot fail to be discovered by someone who does not know it exists, because only its own file names it. The moment a second file has to write `Container.Nested`, promote it to its own file and drop the qualifier — that qualification is the reader hunting for a type in a file not named after it, which is the exact failure §2.1 exists to prevent.

Everything else gets its own file, including: a discriminator enum a single exception uses, a nested `Dto` inside a `Response` (always — a payload piece is by definition read by a consumer), a result record next to the interface that returns it.

### 2.2 Folder names

| Rule | Correct | Wrong |
|---|---|---|
| Container folders are **plural** | `Commands/`, `Queries/`, `Dtos/`, `Requests/`, `Entities/`, `Enums/`, `Services/`, `Configurations/` | `Command/`, `Service/`, `DTOs/` |
| A **feature folder** is the operation name, singular-as-the-type, **no suffix** | `Commands/CreateEvent/` holding `CreateEventCommand` | `Commands/CreateEvents/` |
| The feature folder name **matches its command exactly** | `Commands/UpdateMyProfile/` for `UpdateMyProfileCommand` | `Commands/UpdateProfile/` |
| **No hyphens** — a folder is a namespace segment, and `-` becomes `_` | `Outbox/` | `Cross-Cutting/` → `Cross_Cutting` |
| Domain areas subdivide into `Entities/` + `Enums/` | `Communication/Entities/ContactMessage.cs` | entities loose in `Communication/` |

**Proofread folder names.** A folder name is compiled into the namespace and then into every `using` across the solution, so a typo is expensive to fix and cheap to prevent.

### 2.3 Where a payload type lives

- **`Response`** → in the feature folder, beside the command/query that returns it. It is owned by that one operation, so it belongs where that operation lives. Do not park it in a shared `Dtos/` bucket.
- **`Dto`** → `<Area>/Dtos/` if used inside one area; `Common/Dtos/` if crossed by two or more areas. There is exactly **one** definition per concept — if you need `MoneyDto` in a second area, reference the existing one, never re-declare it.
- **`Request`** → `TEDx.Api/Requests/<Area>/`.

---

## 3. Names outside the request pipeline

| Kind | Rule | Example |
|---|---|---|
| Entity | **Singular** noun, no suffix | `OutboxMessage`, `Event`, `RefundEntry` |
| `DbSet` property | **Plural** of the entity | `DbSet<User> Users` |
| EF configuration | `<Entity>Configuration`, using the entity's **real** name | `UserConfiguration` for `User` |
| Interface / implementation | Same stem, `I` prefix only | `IApplicationDbContext` ↔ `ApplicationDbContext` |
| Options | `<Thing>Options`, bound to config section `<Thing>` | `JwtOptions` ← `"Jwt"` |
| Options validator | `<Thing>OptionsValidator` | `JwtOptionsValidator` |
| Static error catalog | `<Area>Errors` | `TicketingErrors`, `CommonErrors` |
| Exception | `<Condition>Exception` | `EventNotPublishableException` |
| Pipeline behavior | `<Concern>Behavior` | `ValidationBehavior` |
| Middleware | `<Concern>Middleware` | `CorrelationIdMiddleware` |
| Extension class | `<Thing>ServiceExtensions` for DI, `<Thing>Extensions` otherwise | `InfrastructureServiceExtensions` |

### 3.1 No underscores in type names

C# identifiers are Pascal-cased. `Errors_Ticketing` is not a C# name — it is a namespace pretending to be a type. Use `TicketingErrors`, and let the namespace do the grouping.

### 3.2 Record parameters are PascalCase

Positional record parameters become **public properties**, so they follow property casing:

```csharp
public sealed record CancelEventResponse(Guid EventId, EventStatus Status, int VoidedTickets);  // correct
public sealed record CancelEventResponse(Guid eventId, EventStatus status, int voidedTickets);  // wrong — these are properties
```

The JSON is camelCase either way, which is exactly why this one goes unnoticed. Get it right anyway.

### 3.3 No `And` in a type name

`OutboxAndHoldExpirySweeper` is announcing that it does two things. Either split it, or name it for the single job both halves serve.

### 3.4 A property name is a wire contract

Property names serialize into the API payload, so **[07 — API Contract](./07-ApiContract.md) is the authority**, not your preference. If the contract says `globalRole`, the property is `GlobalRole` — naming it `Role` silently ships `"role"` and breaks the frontend. Before naming a payload property, grep the contract for the field.

A plural property name must hold a collection. `Assignments` holding a single object is a lie the compiler will not catch.

---

## 4. Declaration shape (consistency, not just spelling)

Same kind of type, same modifiers — so that a differing declaration means something.

| Kind | Declaration |
|---|---|
| Command / Query | `public sealed record` |
| Response / Dto | `public sealed record` — **immutable**, positional parameters |
| Handler / Validator | `public sealed class` |
| Entity | `public class` with `private set` properties and behavior methods |

**Payloads are records, not mutable classes.** A `class` with `{ get; set; }` properties can be half-filled by any caller and silently mutated after construction; a positional record cannot compile unless every field is supplied. This is the immutability rule from the coding standards applied to the wire.

Mark everything `sealed` unless you have a designed reason to allow inheritance.

---

## 5. Frontend (TypeScript / React)

The frontend is still a skeleton — these rules are here so it starts consistent rather than being audited later.

| Kind | Convention | Example |
|---|---|---|
| File | `kebab-case.ts` / `kebab-case.tsx` | `event-status-badge.tsx` |
| Component | `PascalCase`, matching its file's subject | `EventStatusBadge` |
| Hook | `use` + camelCase | `useEventOrders` |
| Type / interface | `PascalCase`, **no `I` prefix** | `ChangeEventStatusResponse` |
| Value / function | `camelCase` | `changeEventStatus` |
| Constant | `SCREAMING_SNAKE_CASE` only for true module-level constants | `MAX_UPLOAD_BYTES` |

**Mirror the backend payload names exactly.** A response type in `src/types/` carries the *same identifier* as the C# type it deserializes: `ChangeEventStatusResponse`, `EventOrderDto`, `MoneyDto`. One grep should cross the whole stack. Do not invent a parallel vocabulary (`AuthTokens` for what the backend calls `AuthTokensResponse`) — the moment the names diverge, nobody can tell whether two shapes are meant to match.

---

## 6. PR checklist

Paste into the PR description and tick before requesting review:

- [ ] Every new payload type is a `Request`, a `Response`, or a `Dto`, chosen by §0 — and none says `DTO`
- [ ] Each `Response` maps to exactly one endpoint (or one `GET`/`PUT` resource pair)
- [ ] Command/query/handler/validator names are the full triad, with the `Command`/`Query` infix intact
- [ ] Every new command and query has a validator
- [ ] One public type per file, and each file name equals its type name
- [ ] Folder names are plural containers / suffix-free feature folders, spelled correctly, no hyphens
- [ ] Payload property names were checked against [07 — API Contract](./07-ApiContract.md)
- [ ] Record parameters are PascalCase; payloads are `sealed record`, not mutable `class`
- [ ] No underscores in type names; no `And` in a type name
- [ ] Frontend types reuse the backend identifiers verbatim

---

## 7. Changing this document

Amend by PR, with the reason in the version block below. A rule that is being routinely ignored is a rule to fix or delete, not to keep as decoration.

### Changelog

**v1.1 (2026-08-16)** — Two clarifications, both forced by executing the audit against the rules as written.

- **§2.1** was silent on nested types, and a literal reading forced every nested type into its own file. Nested types are *members*, so the rule now says so explicitly and adds exception 3: a nested type with no consumer outside its own file may stay nested, because nothing outside that file can fail to find it. Two cases drove this — `ErrorResultMapper.MappedError` (zero external references, left nested) and `EmailTemplates.EmailBody` (one external consumer forced to write the qualifier, promoted to `EmailBody.cs`). A nested payload piece is still always promoted.
- **§0** now states that the three-suffix table governs payload types only, so that `ITrackScopedRequest` and `PagedRequest` do not read as violations of the `Request` row.

**v1.0 (2026-08-16)** — First issue. Written in response to `ChangeEventStatusDTO`: a type that is a response payload, named as if it were a shared transfer object, in a folder that told the reader nothing. §0 is the rule that case needed. The audit of the existing codebase against this document is in [naming-audit-2026-08-16.md](./naming-audit-2026-08-16.md).
