# Naming Audit — TEDx Backend & Frontend

> **Date:** 2026-08-16
> **Audited against:** [13 — Naming Conventions v1.0](./13-NamingConventions.md)
> **Scope:** every non-generated source file in `TEDx-Backend` (224 `.cs` files, 242 type declarations) and `TEDx-Frontend` (13 files). EF migrations excluded from naming rules (generated), but their coupling to renames is noted in §17.
> **Passes:** two — an initial type-by-type sweep, then a verification pass that re-ran the mechanical checks and walked the folder tree. The second pass corrected one count in the first and added seven findings; both are recorded in [§19.1](#191-one-error-the-verification-pass-found-in-the-first-pass).
> **Status: PROPOSAL — nothing has been renamed.** This is the "what exists / what it should be / why" pass. No code was touched to produce it.

---

## Summary

| Group | Items | Severity |
|---|---|---|
| [1. `Response` vs `Dto` semantics](#1-response-vs-dto-semantics) — the reported problem | 9 | **High** — a reader cannot tell what a type is for |
| [2. `DTO` → `Dto` casing](#2-dto--dto-casing) | 7 (overlaps §1) | Medium |
| [3. Duplicate `Money` type](#3-duplicate-money-type) | 2 → 1 | **High** — two definitions of one concept, both live |
| [4. File name ≠ type name](#4-file-name--type-name) | 4 | **High** — breaks search; already cost us a lookup |
| [5. Triad suffix drift](#5-triad-suffix-drift) | 3 | Medium |
| [6. Folder names](#6-folder-names) | 14 | Medium |
| [7. Entity, enum & persistence names](#7-entity-enum--persistence-names) | 7 | Medium |
| [8. Error catalog names](#8-error-catalog-names) | 5 | Medium |
| [9. One type per file](#9-one-type-per-file) | 17 files (7 allowed, 10 to split) | Low |
| [10. Record parameter casing](#10-record-parameter-casing) | 1 | Medium |
| [11. Declaration shape](#11-declaration-shape) | 17 | Low–Medium |
| [12. Missing validator](#12-missing-validator) | 1 | Medium |
| [13. `ApiErrorResponse` is misnamed](#13-apierrorresponse-is-misnamed) | 1 | Medium — caused a live Swagger defect |
| [14. Pagination prefix trio](#14-pagination-prefix-trio) | 4 | Low |
| [15. Property names vs the API contract](#15-property-names-vs-the-api-contract) | 2 | **CRITICAL — live contract violation** |
| [16. Frontend](#16-frontend) | 2 | Low (skeleton) |

**One finding is a real bug, not a style issue: [§15.1](#151-critical-globalrole-ships-as-role).** `GET /me` and `PUT /me` return `"role"` where [07 — API Contract](./07-ApiContract.md) line 217 specifies `"globalRole"`. The naming audit is how it surfaced.

---

## 1. `Response` vs `Dto` semantics

The rule: [§0](./13-NamingConventions.md#0-the-single-decision-that-causes-the-most-confusion) — `Response` = the whole `data` payload of one endpoint; `Dto` = a piece inside one, or a shape shared by several.

| # | Current | Should be | Why |
|---|---|---|---|
| 1.1 | `ChangeEventStatusDTO` | **`ChangeEventStatusResponse`** | It is the entire `data` of `POST /admin/events/{id}/status` and appears nowhere else. Named `DTO`, it reads like a shared transfer shape, so finding out what it was required opening the file — the exact failure that triggered this audit. |
| 1.2 | `CreateEventDTO` | **`CreateEventResponse`** | Entire `data` of the `201` from `POST /admin/events`. Belongs to one operation. |
| 1.3 | `UpdateEventDTO` | **`UpdateEventResponse`** | Entire `data` of `PUT /admin/events/{id}`. |
| 1.4 | `MyProfileDTO` | **`MyProfileResponse`** | Entire `data` of `GET /me` and `PUT /me` — the sanctioned `GET`/`PUT` same-resource pair in [§0.1](./13-NamingConventions.md#01-response--the-whole-payload-of-one-operation), so one `Response` is correct rather than two. |
| 1.5 | `AdminEventListItemDTO` | **`AdminEventListItemDto`** | Stays a `Dto`: the payload of `GET /admin/events` is the *list*, this is one element of it. Casing fix only — the classification was already right. |
| 1.6 | `TrackAssignmentDTO` | **`TrackAssignmentDto`** | Nested inside `MyProfileResponse` as the `assignments` object; never a top-level payload. Classification right, casing wrong. |
| 1.7 | `AuthUserResponse` | **`AuthUserDto`** | It is the `user` object *inside* `AuthTokensResponse`, not a payload any endpoint returns on its own. Currently the only `Response` in the codebase that is not one — which makes the suffix meaningless if left. |
| 1.8 | `CancelEventResponse` in `Ticketing/DTOs/` | **`CancelEventResponse`** moved to `Ticketing/Commands/CancelEvent/` | Name is correct. Placement is not: a `Response` is owned by one operation, so it belongs beside that operation ([§2.3](./13-NamingConventions.md#23-where-a-payload-type-lives)). Sitting in `DTOs/` is what makes `DTOs/` look like the place responses go — and that is how `ChangeEventStatusDTO` got its name. |
| 1.9 | `AuthTokensResponse` in `Identity/Common/` | **`AuthTokensResponse`** moved to `Identity/Commands/Login/` | Same reason. Three different homes currently exist for response types (`Identity/Common/`, `Identity/Commands/Register/`, `Ticketing/DTOs/`); one of them has to win, and the one that ties a payload to its endpoint is the useful one. Shared by `Login` and `RefreshToken` — keep it in `Login/` and reference it, since `RefreshToken` re-issues the same representation. |

**Already correct, no change:** `EventOrderDto`, `MoneyDto` (the Ticketing spelling), `RegisterResponse`, `ProfilePictureResponse`, `ChangeEventStatusRequest`, `UpdateEventRequest`. These are the precedent the rest should match.

---

## 2. `DTO` → `Dto` casing

Rule: [§0.4](./13-NamingConventions.md#04-never-dto). Acronyms of 3+ letters are Pascal-cased.

Affected: `ChangeEventStatusDTO`, `CreateEventDTO`, `UpdateEventDTO`, `MyProfileDTO`, `AdminEventListItemDTO`, `TrackAssignmentDTO`, `MoneyDTO` — all resolved by the renames in §1 and §3. Folder `DTOs/` → `Dtos/` is in §6.

**Why it matters beyond taste:** the codebase currently contains both `MoneyDTO` and `MoneyDto` as *distinct live types* (§3). Case-only variation stopped being cosmetic the moment it became load-bearing.

---

## 3. Duplicate `Money` type

| # | Current | Should be | Why |
|---|---|---|---|
| 3.1 | `Common/DTOs/MoneyDTO.cs` → `MoneyDTO`, used by `AdminEventListItemDTO` + `GetAdminEventsQueryHandler` (3 refs) | **deleted** | Two records with identical shape `(decimal Amount, string Currency)` for one concept. Which one a new payload picks is currently a coin flip, and a handler that has both in scope compiles with either. |
| 3.2 | `Ticketing/DTOs/MoneyDTO.cs` → `MoneyDto`, used in 10 places | **`Common/Dtos/MoneyDto.cs` → `MoneyDto`** — the single definition | Keep the correctly-cased type and the more-used one; move it to `Common/Dtos/` because money is not a ticketing-only concept. Note the file is *also* a §4 mismatch: the file says `MoneyDTO.cs`, the type says `MoneyDto`. |

---

## 4. File name ≠ type name

Rule: [§2.1](./13-NamingConventions.md#21-one-public-type-per-file-and-the-file-is-named-after-it). Found by comparing every file's basename against its declared types — **4 real mismatches** (`Program.cs` excluded: top-level statements, no type).

| # | Current file | Declares | Should be | Why |
|---|---|---|---|---|
| 4.1 | `Ticketing/DTOs/MoneyDTO.cs` | `MoneyDto` | `Common/Dtos/MoneyDto.cs` | See §3.2. |
| 4.2 | `Queries/GetEventOrders/GetEventOrderQuery.cs` | `GetEventOrdersQuery` | `GetEventOrdersQuery.cs` | Singular file, plural type. **This already cost time**: opening `GetEventOrdersQuery.cs` during the last task returned nothing, and the directory had to be listed to find the file. |
| 4.3 | `Queries/GetEventOrders/GetEventOrdersHandler.cs` | `GetEventOrdersQueryHandler` | `GetEventOrdersQueryHandler.cs` | Also §5.1. |
| 4.4 | `Queries/GetEventOrders/GetEventOrderValidator.cs` | `GetEventOrdersQueryValidator` | `GetEventOrdersQueryValidator.cs` | Also §5.2. Singular *and* missing the infix. |

All three of `GetEventOrders/`'s files disagree with their own contents — this one folder is the worst offender in the solution.

---

## 5. Triad suffix drift

Rule: [§1](./13-NamingConventions.md#1-mediatr-triad). The `Command`/`Query` infix is never dropped.

| # | Current | Should be | Why |
|---|---|---|---|
| 5.1 | file `GetEventOrdersHandler.cs` | `GetEventOrdersQueryHandler.cs` | The type inside is already `GetEventOrdersQueryHandler`; only the file lags. Every other handler in the solution carries the infix, so this one file makes the pattern look optional. |
| 5.2 | file `GetEventOrderValidator.cs` | `GetEventOrdersQueryValidator.cs` | Same; type is already `GetEventOrdersQueryValidator`. |
| 5.3 | file `GetEventOrderQuery.cs` | `GetEventOrdersQuery.cs` | Plurality: the query returns a collection, so plural is correct ([§1](./13-NamingConventions.md#1-mediatr-triad)); the type already agrees. |

Note all three are file-level only — the **types** are already right. This is the cheapest group to fix and has zero compile risk.

---

## 6. Folder names

Rule: [§2.2](./13-NamingConventions.md#22-folder-names).

| # | Current | Should be | Why |
|---|---|---|---|
| 6.1 | `TEDx.Application/Ticketing/Command/` | `Ticketing/Commands/` | `Identity/Commands/` is already plural. One of the two areas is wrong; plural is the rule and the majority. |
| 6.2 | `Ticketing/Command/CreateEvents/` | `Ticketing/Commands/CreateEvent/` | Plural feature folder holding a singular `CreateEventCommand` that creates exactly one event. |
| 6.3 | `Identity/Commands/UpdateProfile/` | `Identity/Commands/UpdateMyProfile/` | Holds `UpdateMyProfileCommand`. The folder drops the "My", so the folder and the type disagree about what is being updated — someone else's profile is a different, admin-only operation. |
| 6.4 | `Identity/Service/` | `Identity/Services/` | Singular container folder; every other container is plural. |
| 6.5 | `TEDx.Api/Common/Respones/` | `TEDx.Api/Common/Responses/` | **Typo.** It is compiled into the namespace `TEDx.Api.Common.Respones` and repeated in 12 files' `using` lines. Cheap now, embarrassing later. |
| 6.6 | `TEDx.Domain/Cross-Cutting/` | `TEDx.Domain/Outbox/` | The hyphen is illegal in an identifier, so the namespace silently became `TEDx.Domain.Cross_Cutting` — an underscore in a namespace, from a folder nobody re-read. It holds exactly one entity (the outbox), so name it for that. |
| 6.7 | `*/DTOs/` (3 folders) | `*/Dtos/` | Consistency with [§0.4](./13-NamingConventions.md#04-never-dto). Also: after §1, `Ticketing/Dtos/` holds only `EventOrderDto` and `AdminEventListItemDto` — actual DTOs, which is the point. |
| 6.8 | `TEDx.Domain/Communication/` — 3 entities and 2 enums loose in the folder | `Communication/Entities/` + `Communication/Enums/` | `Ticketing`, `Training`, and `Identity` all subdivide this way; `Communication` alone does not, so the same kind of file is two clicks away in one area and one click in another. Confirmed contents: `ContactMessage`, `Notification`, `NotificationRecipient` (entities) + `ContactStatus`, `NotificationAudienceType` (enums). |
| 6.9 | `TEDx.Domain/Common/DomainInterfaces/` | `Common/Abstractions/` | **Namespace stutter:** the full name is `TEDx.Domain.Common.DomainInterfaces` — "Domain" twice. It is already inside `TEDx.Domain`, so the prefix carries no information. `TEDx.Application` names the same concept `Common/Interfaces/`, so the two layers disagree for no reason. |
| 6.10 | `TEDx.Application/Identity/Service/` and `TEDx.Application/Ticketing/Availability/` | both → `<Area>/Services/` | Two areas put an interface+implementation service pair in two differently-named folders, and **neither is `Services/`**. `Availability/` also mixes in a value type (`EventSeatAvailability`), so the folder name describes a topic while its siblings describe a kind. |
| 6.11 | `TEDx.Infrastructure/Configuration/` (9 options classes) vs `TEDx.Infrastructure/Persistence/Configurations/` (19 EF configs) | `Infrastructure/Options/` | Two folders **one letter apart** in the same project meaning entirely different things — options binding vs EF entity mapping. "Which Configuration folder?" is a question nobody should have to ask. Renaming the options one to `Options/` also matches the `<Thing>Options` type rule. |
| 6.12 | `AdminSeederOptions.cs` in `Persistence/Seeding/` | `Infrastructure/Options/AdminSeederOptions.cs` | The one options class that does not live with the other nine. Grepping the options folder to see everything that is configurable misses it. |
| 6.13 | `TEDx.Application/Identity/Common/` — sole occupant is `AuthTokensResponse.cs` | **delete** after §1.9 moves the file | A folder holding one file whose correct home is elsewhere. Leaving it empty invites the next response type to land there and re-create the §1 problem. |
| 6.14 | `TEDx.Application/Training/` — **empty folder, committed** | either delete, or leave with a `.gitkeep` and a one-line README saying it is a reserved area | An empty folder in the tree reads as "this area exists and is done"; a reader looks for the training features and finds nothing, with no way to tell absence from omission. |

**Verified correct, no change:** `TEDx.Api/Requests/Events/`, `Infrastructure/Persistence/Configurations/`, `Common/Behaviors/`, `Common/Interfaces/Authorization/`, `Domain/Common/Exceptions/`, `Domain/Common/Entities/`, `Common/Validation/`, `Api/Controllers|Extensions|Filters|Mapping|Middleware|RateLimiting/`.

---

## 7. Entity, enum & persistence names

Rule: [§3](./13-NamingConventions.md#3-names-outside-the-request-pipeline).

| # | Current | Should be | Why |
|---|---|---|---|
| 7.1 | `OutOfBokMessages` (entity) | **`OutboxMessage`** | Three defects in one name: **"Bok" is a typo** for "Box"; the class is **plural** for a single row; and "OutOf" inverts the meaning — an *outbox* message is not an *out-of-book* message. The rest of the system already knows the real name: the table is `OutboxMessages`, the config is `OutboxMessageConfiguration`, the `DbSet` is `OutboxMessages`. Only the CLR type is wrong. |
| 7.2 | `ApplicationUserConfiguration` | **`UserConfiguration`** | It configures `User`. The name refers to an `ApplicationUser` type that does not exist in this codebase (it is the ASP.NET template's name, not ours), so a reader searches for the wrong entity. |
| 7.3 | `DbSet<User> ApplicationUsers` (in `IAppDbContext` **and** `ApplicationDbContext`) | **`Users`** | Same reason; the `DbSet` is named after a type that isn't there. All other sets are the plural of their real entity. |
| 7.4 | `IAppDbContext` | **`IApplicationDbContext`** | The implementation is `ApplicationDbContext`. Interface and implementation must share a stem ([§3](./13-NamingConventions.md#3-names-outside-the-request-pipeline)) — `App…`/`Application…` forces a reader to know they are the same thing. 26 refs, mechanical. |
| 7.5 | `OutboxAndHoldExpirySweeper` | e.g. **`OutboxDispatcher`** + **`HoldExpirySweeper`**, or one name for the shared job | The `And` announces two responsibilities ([§3.3](./13-NamingConventions.md#33-no-and-in-a-type-name)). Splitting is a design change, not a rename — **flagged for a decision, not proposed as a mechanical fix.** |
| 7.6 | `enum ReasonRevoked` | **`RevocationReason`** | Participle-first word order: it reads as a predicate ("reason revoked") rather than naming a thing ("the reason for revocation"). Every other enum in the solution is a noun phrase (`OrderStatus`, `DiscountType`, `PromoRedemptionStatus`). **Two-part change, and only the first part is free:** the *enum* rename is CLR-only (stored as `int`) and safe; the *property* `RefreshToken.ReasonRevoked` is the DB column name, so renaming it needs either a migration or `HasColumnName("ReasonRevoked")`. Recommend renaming the enum now and leaving the property, or doing both with an explicit column mapping. Separately, the property is `{ get; set; }` on a domain entity — see §11. |
| 7.7 | `IAuditable`, `IConcurrent`, `ISoftDelete` — three marker interfaces in one folder, three naming shapes | `IAuditable`, **`IHasRowVersion`**, **`ISoftDeletable`** | `IAuditable` is idiomatic (`-able` = capability). `ISoftDelete` names an *action*, and `IConcurrent` is a bare adjective describing the type rather than what it carries — its whole content is `byte[] RowVersion { get; set; }`, so it announces "has a concurrency token", not "is concurrent". Three shapes for three sibling interfaces means a reader cannot guess the fourth one's name. Cheap: 3 types, few refs. |

---

## 8. Error catalog names

Rule: [§3.1](./13-NamingConventions.md#31-no-underscores-in-type-names).

| # | Current | Should be | Why |
|---|---|---|---|
| 8.1 | `Errors_Common` (25 refs, 13 files) | **`CommonErrors`** | Underscores are not C# type-name casing. The `Errors_X` form is a namespace expressed as an identifier — the namespace already groups these. |
| 8.2 | `Errors_Ticketing` (6 refs) | **`TicketingErrors`** | Same. |
| 8.3 | `Errors_Identity` (24 refs, 14 files) | **`IdentityErrors`** | Same. |
| 8.4 | `Errors_Media` (4 refs) | **`MediaErrors`** | Same. |
| 8.5 | `Errors_Training` (2 refs) | **`TrainingErrors`** | Same. |

**No change:** `Errors_Common.Sorting.cs` → becomes `CommonErrors.Sorting.cs`. The `Type.Aspect.cs` partial-file pattern is *correct* and explicitly allowed ([§2.1](./13-NamingConventions.md#21-one-public-type-per-file-and-the-file-is-named-after-it)); only the stem changes.

**Trade-off, stated honestly:** this is the largest mechanical rename in the audit (61 refs across 33 files) and buys the least functional benefit — it is pure convention compliance. Reasonable to defer or reject. It is listed because leaving it means the convention doc has a standing exception on day one.

---

## 9. One type per file

Rule: [§2.1](./13-NamingConventions.md#21-one-public-type-per-file-and-the-file-is-named-after-it) — allowed exceptions are `TOptions`+validator, and `Type.Aspect.cs` partials.

**17 files declare more than one type.** 7 are the sanctioned options+validator pair; 10 are not.

**Allowed — no change (7 files):** `CloudinaryOptions.cs`, `FrontendOptions.cs`, `IdentityPolicyOptions.cs`, `JwtOptions.cs`, `PaymobOptions.cs`, `SmtpOptions.cs`, `SweeperOptions.cs` — each an options class plus its validator, exactly the exception in §2.1.

| # | Current | Should be | Why |
|---|---|---|---|
| 9.1 | `Identity/Common/AuthTokensResponse.cs` — 2 types (`AuthTokensResponse`, `AuthUserResponse`) | split; `AuthUserDto.cs` separate (see §1.7) | A nested payload type is a thing a reader searches for by name. |
| 9.2 | `Common/Interfaces/IRefreshTokenService.cs` — 3 types (interface, `RefreshTokenIssued`, `RefreshTokenRotated`) | split the two result records into their own files | Two domain-meaningful records hidden inside an interface file; neither is findable by file name. |
| 9.3 | `Common/Interfaces/IJwtTokenService.cs` — 2 types (interface, `AccessTokenResult`) | split | Same. |
| 9.4 | `Common/Interfaces/IUserAccountService.cs` — 2 types (interface, `enum PasswordCheckResult`) | split the enum out | An enum with wire-adjacent meaning buried in a service file. |
| 9.5 | `Common/Pagination/SortSyntax.cs` — 2 types (`enum SortDirection`, `SortSyntax`) | split `SortDirection.cs` | Same. |
| 9.6 | `Api/Mapping/ErrorResultMapper.cs` — 2 types (`ErrorResultMapper`, `readonly record struct MappedError`) | split `MappedError.cs` | Same. `MappedError` is the type that defines the real failure body shape (§13) — the single most useful type to be able to find by name, and it is invisible in the file tree. |
| 9.7 | `Domain/Common/Exceptions/EventNotPublishableException.cs` — 2 types (`enum EventPublishBlock`, exception) | split `EventPublishBlock.cs` | Added during the status-change fix; the discriminator is now read by the Application layer's error mapping, so it is a type people look up. Held to the same rule as the rest. |
| 9.8 | `Infrastructure/Email/EmailTemplates.cs` — 2 types (`EmailTemplates`, `record EmailBody`) | split `EmailBody.cs` | Same. |
| 9.9 | `Infrastructure/Identity/EmailConfirmationTokenProvider.cs` — 2 types (`EmailConfirmationTokenProviderOptions`, `EmailConfirmationTokenProvider`) | **split** — or amend §2.1 to cover it | Reads like the options exception, but strictly is not: §2.1 permits `TOptions` + `TOptions**Validator**`, and this is `TOptions` + the *consumer*. Also, the options type is declared **first**, so the file's leading type is not the type the file is named after. Flagged rather than waved through — the exception list is closed on purpose, and if this pairing should be allowed the rules doc is what changes. |
| 9.10 | `Infrastructure/Configuration/RateLimitingOptions.cs` — 3 types (`RateLimitingOptions`, `RateLimitGroupOptions`, `RateLimitingOptionsValidator`) | split `RateLimitGroupOptions.cs`; keep options+validator together | The options+validator pair is allowed; a *second, independent* options class in the same file is not. |

---

## 10. Record parameter casing

| # | Current | Should be | Why |
|---|---|---|---|
| 10.1 | `CancelEventResponse(Guid eventId, EventStatus status, int voidedTickets, int checkedInTicketsRetained, int releasedHolds, int refundEntriesRecorded)` | PascalCase every parameter | Positional record parameters **become public properties**. These are six public properties in camelCase — a C# convention violation the compiler accepts silently. **No wire impact** (the serializer camelCases anyway), which is exactly why it survived. It is the only record in the solution written this way. |

---

## 11. Declaration shape

Rule: [§4](./13-NamingConventions.md#4-declaration-shape-consistency-not-just-spelling). Adjacent to naming rather than strictly naming — **separable from the rest of this audit if you'd rather scope it out.**

### 11.1 Mutable `class` payloads that should be `sealed record`

| Current | Why |
|---|---|
| `MyProfileDTO` (`public class`, 9 settable properties, 5 non-nullable `string` with no initializer) | Any caller can half-construct it; the uninitialized non-nullable strings are also contributing to the solution's nullable warnings. A positional record cannot compile half-filled. |
| `TrackAssignmentDTO` (`public class`) | Same. |
| `CreateEventDTO` (`public class`, 14 nullable settable properties) | Every field optional means the type documents nothing about what a created event actually has. Also carries a commented-out JSON sample at the bottom of the file — [07 — API Contract](./07-ApiContract.md) is the place for that. |

### 11.2 Commands/queries declared as `class` instead of `sealed record`

`UpdateMyProfileCommand`, `GetMyProfileQuery`, `DeleteEventCommand` — the remaining three of ~15; every other command and query is already `sealed record`. `DeleteEventCommand` is additionally constructed with an object initializer (`new DeleteEventCommand { EventId = id }`) while its siblings use positional construction.

### 11.3 Accessibility and `sealed` drift

- `DeleteEventCommandHandler` is `internal`; every other handler is `public`.
- Not `sealed` while their siblings are: `CreateEventCommandHandler`, `CreateEventCommandValidator`, `UpdateMyProfileCommand`/`Handler`/`Validator`, `GetMyProfileQuery`/`QueryHandler`, `AuthorizationBehavior`, `SystemClock`, `AuditInterceptor`, `OrderConfiguration`, `IdentityPolicyOptionsValidator`.
- `RefreshToken.ReasonRevoked` is `{ get; set; }` — a **public setter on a domain entity**, against the `private set` rule in [§4](./13-NamingConventions.md#4-declaration-shape-consistency-not-just-spelling). `RefreshTokenService` mutates it directly from four call sites, so tightening it means moving those into a `Revoke(reason)` method on the aggregate. Same shape as the `Status`-assignment problem already fixed in `Event` — **a design change, not a rename;** flagged, not proposed.

**Why it matters:** when 90% of a kind shares a shape, the 10% that differs reads as intentional. A reviewer stops to ask why `DeleteEventCommandHandler` is `internal` — and the answer is "no reason", which is a wasted question every time.

---

## 12. Missing validator

| # | Current | Should be | Why |
|---|---|---|---|
| 12.1 | `Ticketing/Command/DeleteEvent/` has `Command` + `Handler`, **no validator** | add `DeleteEventCommandValidator` | Every other command folder has all three files, so the gap reads as "nothing to validate here" rather than "not written yet". `DeleteEventCommand.EventId` is unvalidated: `Guid.Empty` reaches the handler and 404s instead of 422. That is the same class of bug already fixed in `ChangeEventStatusCommand`. |

---

## 13. `ApiErrorResponse` is misnamed

| # | Current | Should be | Why |
|---|---|---|---|
| 13.1 | `ApiErrorResponse` — the `{code, message, fieldErrors, traceId}` object **nested inside** `ApiResponse<T>.Error` | **`ApiError`** | The name says "the error response", but it is not a response — the response is `ApiResponse<T>` with `Error` populated. **This has already caused a live defect:** all 81 `[ProducesResponseType(typeof(ApiErrorResponse), 4xx)]` attributes advertise the bare error object as the failure body, while `ErrorResultMapper` actually returns `ApiResponse<object>`. Swagger and the generated client therefore describe a 4xx shape the server never sends. Renaming to `ApiError` makes the mistake unwriteable. |

**Fixing the attributes is a separate change** (81 references) and is a *behavioural* documentation fix, not a rename — noted here because the rename is what makes it visible. The frontend's `src/types/api.ts` already models it correctly as a nested `ApiErrorResponse` inside `ApiResponse<T>`, so the FE is right and the BE's attributes are wrong.

---

## 14. Pagination prefix trio

| # | Current | Should be | Why |
|---|---|---|---|
| 14.1 | `PageRequest`, `PagedResult<T>`, `PaginationDefaults`, `PaginationMeta` | pick one prefix — suggested: **`PagedRequest`, `PagedResult`, `PagedDefaults`, `PagedMeta`** | Three prefixes (`Page`, `Paged`, `Pagination`) for one concept means autocomplete on "Pag" gives no signal about which family a type belongs to. Lowest-value group in the audit; listed for completeness. Note `PaginationMeta` is also the wire shape of the envelope's `meta`, so check the contract before renaming that one. |

### 14.2 Remaining minor inconsistencies

Recorded for completeness; each is one type, and none is worth its own group.

| Current | Should be | Why |
|---|---|---|
| `DevDiagnosticsEndpoints` in `Api/Extensions/` | `DevDiagnosticsEndpointExtensions`, or move to `Api/Endpoints/` | It is an extension class (`MapDevDiagnostics(this WebApplication)`) sitting among four siblings that all end in `Extensions`. It is the only file in the folder that does not follow the folder's own pattern. |
| `PaymentIntention` and `PaymobTransactionResult` in `Ticketing/Payments/` | give both the same treatment — either both suffix-free or both `…Result` | One carries a `Result` suffix and the other does not, for two halves of the same vendor call. Vendor-boundary types, so low stakes, but they are the pair a reader compares side by side. |
| `Ticketing/Payments/` holds an interface, a request shape, and a result shape flat | `Payments/Services/` + `Payments/Dtos/`, or leave as-is | Only three files, so the flat layout is defensible today. Noted because it is the same pattern as §6.10 and will not scale. |

---

## 15. Property names vs the API contract

Rule: [§3.4](./13-NamingConventions.md#34-a-property-name-is-a-wire-contract) — the contract is the authority.

### 15.1 CRITICAL: `globalRole` ships as `role`

| Current | Should be | Why |
|---|---|---|
| `MyProfileDTO.Role` (type `GlobalRole`) | **`GlobalRole`** | [07 — API Contract](./07-ApiContract.md) line 217 specifies `GET /me` → `"globalRole": "Attendee"`, and line 223 makes that field authoritative over the JWT claim. The property named `Role` serializes as **`"role"`**, so `GET /me` and `PUT /me` are shipping a field name the contract does not define and the frontend will not find. Every other payload in the system uses `globalRole` (`RegisterResponse.GlobalRole`, `AuthUserResponse.GlobalRole`). **This is a live contract violation, not a style preference** — the FE is a skeleton, which is the only reason nobody has hit it. |

### 15.2 Plural property holding a single object

| Current | Should be | Why |
|---|---|---|
| `MyProfileDTO.Assignments` — type `TrackAssignmentDTO` (one object) | keep the wire name **`assignments`** (the contract specifies it), but the plural is only defensible because the *object* holds two assignment slots | The contract is explicit that `assignments` is "two nullable scalars, not an array". So the wire name stays; the note is that `Assignments` reads as a collection in C# and is not one. If it is ever renamed, the contract must change first. **No action — recorded so nobody "fixes" it into `assignments: []` and breaks the contract.** |

---

## 16. Frontend

13 files, no feature code yet — `src/lib/api.ts` is entirely commented out. Nothing to rename; two items to align before the FE build starts.

| # | Current | Should be | Why |
|---|---|---|---|
| 16.1 | `AuthTokens` in `src/types/api.ts` | **`AuthTokensResponse`** | The backend type is `AuthTokensResponse`. Divergent names for one wire shape mean a grep across the stack finds one side only ([§5](./13-NamingConventions.md#5-frontend-typescript--react)). |
| 16.2 | `ApiResponse<T>` in `src/types/api.ts` has `success`, `data`, `error` — **no `meta`** | add `meta?: PaginationMeta` | The backend envelope carries `meta` on paged endpoints (`ApiResponse<T>.Meta`, emitted by `OkPagedEnvelope`). Not a naming defect — a missing field — but it is in the file this audit covers and would silently drop pagination. |

---

## 17. Execution notes (for when this is approved)

**Ordering.** Groups §4 and §5 first — file-only renames, zero compile risk. Then §1–§3 and §7–§8 (mechanical symbol renames, done with IDE rename so references follow). §9's splits and §11's shape changes last, since they touch declarations.

**Blast radius,** measured across `TEDx-Backend` + `TEDx-Frontend`:

| Symbol | Files | Refs |
|---|---|---|
| `ApiErrorResponse` | 10 | 81 |
| `IAppDbContext` | 17 | 26 |
| `Errors_Common` | 13 | 25 |
| `Errors_Identity` | 14 | 24 |
| `MyProfileDTO` | 8 | 18 |
| `ChangeEventStatusDTO` | 4 | 13 |
| `UpdateEventDTO` | 4 | 11 |
| `MoneyDto` / `MoneyDTO` | 10 / 3 | 10 / 3 |
| `AdminEventListItemDTO` | 4 | 10 |
| `EventOrderDto` (no rename) | 4 | 9 |
| `OutOfBokMessages` | 7 | 9 |
| `CreateEventDTO` | 4 | 7 |
| `Errors_Ticketing` | 4 | 6 |
| `ApplicationUsers` | 6 | 6 |
| `AuthUserResponse` | 3 | 4 |
| `Errors_Media` | 4 | 4 |
| namespace `…Respones` | 12 | 12 |
| `TrackAssignmentDTO` | 3 | 3 |
| `Errors_Training` | 2 | 2 |

Added by the verification pass:

| Symbol | Files | Refs |
|---|---|---|
| namespace `…Common.DomainInterfaces` (§6.9) | 16 | 16 |
| `ReasonRevoked` (§7.6) | 7 | 23 |
| namespace `Infrastructure.Configuration` (§6.11) | 20 | 20 |
| `IConcurrent` (§7.7) | 11 | 11 |
| `ISoftDelete` (§7.7) | 8 | 8 |
| `SortDirection` (§9.5, file split only) | 3 | 11 |
| `RateLimitGroupOptions` (§9.10, file split only) | 2 | 8 |
| `PasswordCheckResult` (§9.4, file split only) | 3 | 8 |
| `EmailBody` / `EventPublishBlock` / `MappedError` / `AccessTokenResult` (file splits only) | 1–3 each | 4–6 each |

The §9 splits move a declaration between files without changing any identifier, so their reference counts are informational — nothing outside the moved file needs editing.

**Two renames touch the database layer.**

1. `OutOfBokMessages` → `OutboxMessage` (§7.1) appears as a fully-qualified CLR name in `20260812124227_Init.Designer.cs:145`, `20260815104702_AddRefundEntryAmount.Designer.cs:145`, and `ApplicationDbContextModelSnapshot.cs:142`. Because the table name is pinned by `ToTable("OutboxMessages")`, the rename produces **no schema change** — but the model snapshot must be regenerated (an empty migration) or the next `migrations add` will emit a spurious drop/create. Same applies to the `Cross-Cutting` → `Outbox` namespace move.
2. `ReasonRevoked` (§7.6): renaming the **enum** is free (stored as `int`, CLR-only). Renaming the **property** would rename the `RefreshToken.ReasonRevoked` column, which is a real migration — so either keep the property name, or rename it with `HasColumnName("ReasonRevoked")` to keep the schema still.

Every other rename in this audit is snapshot-free.

**Build command:** the backend has both `TEDx.sln` and `TEDx.slnx`, so a bare `dotnet build` fails with MSB1011 — use `dotnet build TEDx.sln`.

---

## 18. Non-naming findings surfaced during the scan

Recorded because a complete audit should not silently drop them. **Out of scope for this task; no action proposed here.**

1. **`[ProducesResponseType]` advertises the wrong 4xx body** — 81 attributes; see §13.
2. **`DeleteEventCommandHandler` counts orders without filtering by status**, so a cancelled or expired order blocks a soft-delete. Same bug class as the one fixed in `Event.Revert` during the status-change task.
3. **`GET /api/v1/admin/events/{id}`** (admin event detail) is specified at [07 — API Contract](./07-ApiContract.md) line 380 but unimplemented — currently an unclaimed 404.
4. **Stale `EVENT_HAS_ORDERS` references** for the unpublish case in `03-UserFlows.md:386`, `09-SystemDesign.md:264`, `11-StateMachines.md:75,86` — superseded by `HAS_ORDERS_CANNOT_UNPUBLISH` (OPEN-S2-2).
5. **`MyProfileDTO`'s uninitialized non-nullable strings** contribute to the solution's 17 build warnings; §11.1's record conversion would clear them.
6. **6 Dependabot vulnerabilities** on `dev` (4 high, 2 moderate).

---

## 19. Method — how "nothing missed" was established

So the completeness claim is checkable rather than asserted:

1. **Full enumeration**, not sampling: `find` over all four backend projects (229 `.cs`, 224 excluding migrations) and the frontend `src/` (13 files). Every path was read, not spot-checked.
2. **Every type declaration extracted** — 244 total, 242 non-migration — by regex over `public|internal` + `class|record|interface|enum|struct`, then sorted with its file path. Each was classified against [13 — Naming Conventions](./13-NamingConventions.md).
3. **File-vs-type mismatch found mechanically**, not by eye: a loop grepping each file for a type matching its own basename. Result: 4 mismatches + `Program.cs` (expected, top-level statements). §4 is provably complete.
4. **Multi-type files found mechanically**: per-file declaration count. Result: **17 files** with >1 type, 7 of them the allowed options+validator pattern. §9 is provably complete.
5. **Duplicate concepts traced by usage**, not assumed: `grep` per symbol confirmed both `MoneyDTO` and `MoneyDto` are live and which call sites use which.
6. **Wire names checked against the authoritative contract** — `docs/07-ApiContract.md` grepped for `globalRole`/`role` and the `/me` and `/status` payloads read directly. That is what caught §15.1.
7. **Rename cost measured, not guessed**: per-symbol file and reference counts (§17), plus a migration-snapshot grep to find the one rename with an EF prerequisite.
8. **Folder tree enumerated separately** from the file scan (`find -type d`), then every folder's contents listed. That is what caught the empty `Application/Training/`, the single-file `Identity/Common/`, and the `Configuration/` vs `Configurations/` collision — none of which a file-level scan surfaces.

### 19.1 One error the verification pass found in the first pass

Stated because a report that claims completeness should show where it was wrong.

The first scan's regex allowed `sealed|abstract|static|partial` before the type keyword but **not `readonly`**, so `public readonly record struct MappedError` was invisible to it. That undercounted §9 as 16 files and would have dropped `ErrorResultMapper.cs` from the split list had it not also been found by reading. Re-running with `readonly` and `record struct` in the pattern gives the 17 in §9. The corrected regex also confirmed the 244/242 declaration total was unaffected.

The verification pass added seven findings the first pass missed — §6.9–6.14, §7.6, §7.7, §14.2 — all of them folder-level or enum-level, i.e. the categories a type-by-type sweep is structurally blind to.

**Known limits of this audit, stated plainly:**
- **Method, parameter, and local-variable names were not audited** — only types, files, folders, namespaces, and payload properties. A method-level pass is a separate, much larger job; say if you want it.
- **EF migrations were excluded** as generated code.
- **Frontend rules are forward-looking.** With 13 skeleton files there is almost nothing to audit; §16 is the whole finding.
- **§11 (declaration shape), §14 (pagination prefixes) and §14.2 are judgement calls**, not rule violations in the strict sense. They are separable — reject them without affecting anything else.
- **§7.5 (`OutboxAndHoldExpirySweeper`) is a design question, not a rename.** It is listed because the name reveals it, not because a new name fixes it.

### 19.2 A second error, found while executing §9

The same declaration-counting regex had a second defect, and it produced two wrong findings — **9.6 and 9.8 were not violations at all.**

The per-file count in §19 point 4 counted *every* declaration in a file, including nested ones. So `ErrorResultMapper.cs` and `EmailTemplates.cs` were reported as multi-type files when `MappedError` and `EmailBody` are **nested members** of the single top-level type in each — confirmed with `cat -A` on both declarations before touching them. §2.1 governs top-level types, so neither file ever violated it.

Resolution, on evidence rather than mechanically:

- **`MappedError` — left nested.** Zero references outside `ErrorResultMapper.cs`. Promoting it would have been pure churn.
- **`EmailBody` — promoted** to `TEDx.Infrastructure/Email/EmailBody.cs`. `SmtpEmailSender.cs:50` had to write `EmailTemplates.EmailBody`, and that qualifier is the discoverability failure §2.1 exists to prevent.

The asymmetry is now a rule, not a judgement call: [13 — Naming Conventions](./13-NamingConventions.md) §2.1 exception 3, added in v1.1. **The corrected count for §9 is 14 files, not 17** — 7 sanctioned options+validator pairs, 7 genuine splits.

---

## Execution record

Approved 2026-08-16 with "go ahead and fix" — no tier named, so **tiers A, B and C were executed in full** on branch `refactor/unify-naming-conventions`, in the §17 order, building after each stage. Final state: **0 errors, 0 warnings**, down from a 17-warning baseline. **Tier D was not applied** — those are design decisions, listed below for your call.

Three deviations from what this report proposed, all deliberate:

| # | Report said | What was done | Why |
|---|---|---|---|
| 1 | §9: split 17 multi-type files | 14 files, and one nested type left nested | Findings 9.6 and 9.8 were mis-derived — see [§19.2](#192-a-second-error-found-while-executing-9). |
| 2 | §14.2: `PaymentIntention` + `TransactionResult` → "either both suffix-free or both `…Result`" | **`PaymobPaymentIntention`** + **`PaymobTransactionResult`** | The axis that was actually inconsistent is the vendor prefix, not the suffix. Prefixing both lets each type keep the vendor's own noun, and makes it obvious at the call site that these are Paymob's shapes, not ours. |
| 3 | §14.2: split `Ticketing/Payments/` into `Services/` + `Dtos/` | left as one folder | Four files. The split would cost two folders to separate two interfaces from two records that only those interfaces use. |
| 4 | §16: two frontend items | four | Two follow from this report's own rules and were missed when §16 was written. The frontend's `ApiErrorResponse` had to become `ApiError` to keep mirroring the backend after §13 renamed it. And `AuthTokens` carried 2 of the backend record's 5 fields — renaming it to `AuthTokensResponse` without completing the shape would have produced a name that promises a mirror and delivers a subset, which is worse than the divergent name §16.1 set out to fix. `AuthUserDto` was added because it is the `user` block's type. |

**Tier D — still open, needs your decision, nothing changed:**

1. **`OutboxAndHoldExpirySweeper`** (§7.5) — the `And` in the name is honest: it really is two jobs in one background service. Renaming it hides that; splitting it is a design change.
2. **`RefreshToken.ReasonRevoked`'s public setter** (§11) — `RefreshTokenService` assigns it from five places (lines 69, 78, 122, 151, 175). Replacing it with a `Revoke(reason)` aggregate method is the correct fix and is a behaviour change, not a rename. Note the enum it holds *was* renamed (`ReasonRevoked` → `RevocationReason`); only the property and its setter are untouched.
3. **The six items in §18** — including the `DeleteEventCommandHandler` order-status bug and the 6 Dependabot advisories.

---

## The tier split as it was proposed

| Tier | Groups | Cost | Value |
|---|---|---|---|
| **A — fix now** | §15.1 (the `globalRole` bug), §3 (duplicate `MoneyDto`), §4 + §5 (file renames), §1 + §2 (the reported problem), §6.5 (`Respones` typo), §7.1 (`OutOfBokMessages` typo) | low | high — one live bug, one duplicate type, two typos, and the confusion that started this |
| **B — worth doing** | §6 rest, §7.2–7.4, §7.6–7.7, §12, §13, §16 | medium | consistency plus the Swagger shape fix |
| **C — take or leave** | §8 (`Errors_*`, 61 refs for pure convention), §9 splits, §11 shape changes, §14 | medium–high | convention only |
| **D — decisions, not renames** | §7.5 (`OutboxAndHoldExpirySweeper`), §11's `ReasonRevoked` setter, §18 | — | needs your call on design, not a rename |
