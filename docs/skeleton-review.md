# Skeleton Review — Tasks T-S0-01 & T-S0-02

**Reviewer:** Claude (automated)  
**Date:** 2026-07-27  
**Branch:** `dev` (commit `66fd6b0`)  
**Scope:** Modular Monolith structure + Result/Error pattern  
**Verdict:** Solid foundation, several spec deviations need fixing before feature work begins.

---

## Severity Legend

| Tag | Meaning |
|-----|---------|
| **CRITICAL** | Blocks DoD or breaks a spec rule; must fix before merge |
| **HIGH** | Spec deviation or design gap that will cause rework later |
| **MEDIUM** | Inconsistency or missing polish; fix in this sprint |
| **LOW** | Style / quality; fix opportunistically |

---

## Task 1 — Modular Monolith Structure (T-S0-01)

### 1.1 What's Correct

- Four projects with correct dependency rule: Domain(nothing) < Application(Domain) < Infrastructure(Application+Domain) < Api(Application+Infrastructure). No circular references.
- `net10.0`, `Nullable enable`, `ImplicitUsings enable` across all projects.
- Solution compiles (`dotnet build` succeeds).
- Three bounded-context folder stubs exist in Domain, Application, and Infrastructure.

### 1.2 Findings

#### CRITICAL-1: `.gitignore` missing standard .NET entries

**File:** `E:\.NET Projects\TedX\.gitignore`

The repo `.gitignore` only contains three documentation exclusions. It has **no** entries for `bin/`, `obj/`, `.vs/`, `*.user`, `*.suo`, `*.DotSettings.user`, or any other standard .NET artifacts. This means all build output and IDE state is likely being tracked in git — the `git status` already shows dozens of `obj/` file changes.

**Fix:** Replace with the standard [.NET gitignore from GitHub](https://github.com/github/gitignore/blob/main/VisualStudio.gitignore), then add the three custom doc exclusions back. After updating, run `git rm -r --cached .` and re-add to strip already-tracked `bin/obj/.vs` from the index.

**Spec ref:** Task 1 DoD explicitly requires `.gitignore`.

---

#### CRITICAL-2: `Directory.Build.props` does not exist

**Spec ref:** Task 1 explicitly says "Set up Directory.Build.props for shared settings (nullable, implicit usings, target framework)."

Currently all four `.csproj` files independently duplicate these three properties. This is exactly what `Directory.Build.props` is meant to eliminate.

**Fix:** Create `TEDx_Backend/Directory.Build.props`:

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

Then remove the `<PropertyGroup>` with these three properties from each `.csproj`.

---

#### CRITICAL-3: `.editorconfig` does not exist

**Spec ref:** Task 1 DoD explicitly requires `.editorconfig`.

**Fix:** Add a `.editorconfig` at `TEDx_Backend/` with at minimum C# defaults (file-scoped namespaces, severity levels for common analyzers, indent style).

---

#### HIGH-1: Project naming diverges from spec

**Spec says:** `TEDxAlkawmia.Domain`, `TEDxAlkawmia.Application`, `TEDxAlkawmia.Infrastructure`, `TEDxAlkawmia.Api`

**Code has:** `TEDx.Domain`, `TEDx.Application`, `TEDx.Infrastructure`, `TEDx` (the Api project is just `TEDx`, not `TEDx.Api`)

This is a two-part issue:
1. `TEDxAlkawmia` prefix vs `TEDx` prefix — needs a decision on which is canonical.
2. The Api project is named `TEDx` not `TEDx.Api` — all four layers should follow the same `{Root}.{Layer}` convention. A bare `TEDx` project name is ambiguous.

---

#### HIGH-2: Bounded context folder names don't match the architecture docs

**Spec (09-SystemDesign.md §2.3, §3.2) declares five contexts:**
| Context | Folder name in docs |
|---------|---------------------|
| Identity | `Identity/` |
| Eventing / Ticketing | `Ticketing/` |
| Training | `Training/` |
| Communications | `Communications/` |
| Cross-cutting | `Common/` |

**Code uses:**
- `Eventing/` instead of `Ticketing/` — the docs use "Eventing/Ticketing" interchangeably in prose but the folder layout in §3.2 says `Ticketing/`.
- `Communications/` context is **missing** from all layers.
- `Common/` exists only as `Common/Errors/` in Domain and Application — not as a full context folder with sub-structure (for `IAuditable`, `ISoftDeletable`, base markers per §3.2).

**Fix:** Rename `Eventing/` to `Ticketing/`. Add `Communications/` stubs. Flesh out `Common/` with its sub-folders.

---

#### HIGH-3: Infrastructure internal structure doesn't match architecture docs

**Spec (09-SystemDesign.md §3.2) says Infrastructure should contain:**
```
Infrastructure/
  Persistence/     (AppDbContext, configs, interceptors, migrations)
  Identity/        (Identity setup, JWT + refresh services)
  Payments/        (Paymob client + HMAC verifier)
  Media/           (Cloudinary client)
  Email/           (SMTP client)
  BackgroundJobs/  (sweeper BackgroundService)
  Logging/         (Serilog config)
```

**Code has:** Three flat context folders (`Identity/`, `Eventing/`, `Training/`) — a fundamentally different organization. Infrastructure in Clean Architecture is organized by **concern** (persistence, identity, external services), not by business context.

**Fix:** Restructure Infrastructure stubs to match the spec's concern-based layout.

---

#### MEDIUM-1: Solution file format and location

- Uses `.slnx` (newer XML format) instead of traditional `.sln`. Not all tooling (CI, older VS versions, Rider versions) may support `.slnx` yet.
- The `.slnx` lives inside `TEDx_Backend/TEDx/` (the Api project folder) instead of at `TEDx_Backend/` root where convention expects it.

**Fix:** Consider either generating a traditional `.sln` alongside it, or at minimum moving the `.slnx` to `TEDx_Backend/`.

---

#### MEDIUM-2: Domain folder naming inconsistency

In `TEDx.Domain.csproj`:
- `Identity/ValueObjects/` (plural)
- `Eventing/ValueObjects/` (plural)  
- `Training/ValueObject/` (**singular** — typo)

**Fix:** Rename to `Training/ValueObjects/` (plural) for consistency.

---

#### MEDIUM-3: Application sub-folder naming

Application uses `Command/` (singular) but CQRS convention and the spec use plural forms: `Commands/`, `Queries/`, `Validators/`, `DTOs/`. The current `Queries/` folder IS plural, creating an inconsistency even within the same context.

**Fix:** Rename `Command/` to `Commands/` in all three contexts.

---

#### LOW-1: Program.cs uses old-style entry point

```csharp
public class Program
{
    public static void Main(string[] args) { ... }
}
```

Modern .NET (6+) prefers top-level statements / minimal hosting. The file also contains a template comment (`// Learn more about configuring OpenAPI...`).

**Fix:** Convert to minimal hosting (`var builder = WebApplication.CreateBuilder(args);` at top level). Remove template comments.

---

#### LOW-2: Api project missing `Mapping/` folder

Spec (§3.2) shows `Api/Mapping/` for the `Result -> ActionResult` mapper. The `.csproj` only declares `Controllers/` and `Middleware/`.

**Fix:** Add `Mapping/` folder stub.

---

## Task 2 — Result Pattern & Error Catalog (T-S0-02)

### 2.1 What's Correct

- `Result<T>` in Domain layer with `Success(T)` and `Failure(Error)` factories. Immutable (sealed class, private constructor).
- `Error` is a `readonly record struct` with `Code`, `Message`, `Type` — immutable value type.
- `ErrorType` enum has all five specified types: `Validation`, `NotFound`, `Conflict`, `Business`, `Unauthorized`.
- `Errors` catalog is in `Application/Common/Errors/` — correct layer placement.
- **All 40 error codes present** — 36 from API contract §0.9 plus 4 from audit-Issue-30 (`ILLEGAL_STATUS_TRANSITION`, `EVENT_HAS_ORDERS`, `SESSION_HAS_RECORDS`, `CAPACITY_BELOW_SOLD`). Complete coverage.
- No HTTP or Serilog dependency in Domain or Application. DoD met.

### 2.2 Findings

#### CRITICAL-4: `RESET_TOKEN_INVALID` maps to wrong ErrorType

**Spec (07-ApiContract.md §0.9, audit-Issue-10):** "An invalid/expired password-reset token (a submitted field, not a session credential) uses **400** `RESET_TOKEN_INVALID` — a distinct code so clients never see the same code under two statuses."

**Code (Errors.cs:52-53):**
```csharp
public static readonly Error ResetTokenInvalid =
    new("RESET_TOKEN_INVALID", "Reset token is invalid.", ErrorType.Unauthorized);
```

`ErrorType.Unauthorized` maps to 401/403. The spec explicitly says this should be **400** because it's a submitted field, not a session credential. This violates audit-Issue-10 (each code maps to exactly one HTTP status) — `TOKEN_INVALID` already occupies 401 under `Unauthorized`.

**Impact:** This is a spec-breaking error type assignment. The current `ErrorType` enum has no value that maps to 400. Two options:
1. Add `ErrorType.BadRequest` (maps to 400) and use it for `RESET_TOKEN_INVALID`.
2. Use `ErrorType.Validation` (maps to 422) as a pragmatic fallback — but this also violates the spec's explicit 400.

**Recommendation:** Add `BadRequest` to the enum. It's the cleanest solution and preserves audit-Issue-10 compliance.

---

#### CRITICAL-5: `RATE_LIMITED` maps to wrong ErrorType

**Code (Errors.cs:28-29):**
```csharp
public static readonly Error RateLimited =
    new("RATE_LIMITED", "Too many requests.", ErrorType.Business);
```

`ErrorType.Business` maps to 422. Rate limiting universally returns **429 Too Many Requests**. Like `RESET_TOKEN_INVALID`, this code has no matching `ErrorType` in the current enum.

**Recommendation:** Add `ErrorType.RateLimited` (maps to 429), or handle this as a special case in the mapper. Either way, `Business` is wrong — a rate limit is not a business rule violation.

---

#### HIGH-4: No non-generic `Result` for void operations

Many command handlers return no data on success (e.g., `DeleteUser`, `ChangePassword`, `TransitionStatus`). With only `Result<T>`, these handlers would need to return `Result<bool>` or `Result<Unit>` — both are unidiomatic.

**Fix:** Add a non-generic `Result` class:

```csharp
public sealed class Result
{
    public bool IsSuccess { get; }
    public Error? Error { get; }

    private Result(bool isSuccess, Error? error) { ... }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
}
```

---

#### HIGH-5: `ErrorType.Unauthorized` conflates 401 and 403

The spec (D:Q37) maps `Unauthorized` to "401 / 403", but these are semantically different:
- **401** = not authenticated (no valid credentials presented)
- **403** = authenticated but not authorized (credentials valid, insufficient permissions)

The current catalog assigns `ErrorType.Unauthorized` to both `UNAUTHENTICATED` (should be 401) and `FORBIDDEN` / `TRACK_FORBIDDEN` (should be 403). The mapper cannot distinguish these from the `ErrorType` alone — it would need to inspect `Error.Code`, which undermines the purpose of having an `ErrorType`.

**Recommendation:** Split into `ErrorType.Unauthenticated` (401) and `ErrorType.Forbidden` (403). This makes the type-to-status mapping deterministic and eliminates mapper special-casing.

---

#### HIGH-6: `EMAIL_TAKEN` and `PROMO_CODE_TAKEN` ErrorType needs verification

Both use `ErrorType.Conflict` (409). The API contract §0.9 D-2 convention explicitly lists the 409 codes: `CONCURRENCY_CONFLICT`, `PRICE_CHANGED`, `SEATS_UNAVAILABLE`, `ACTIVE_ORDER_EXISTS`, `HOLD_EXPIRED`, plus state-transition codes. Neither `EMAIL_TAKEN` nor `PROMO_CODE_TAKEN` appears in that list.

- `EMAIL_TAKEN` — uniqueness violation when registering. Could be 409 (resource conflict) or 422 (business rule: email in use). The spec is ambiguous.
- `PROMO_CODE_TAKEN` — uniqueness violation when creating a promo. Same ambiguity.

**Action needed:** Make an explicit decision and document it. If 409, add them to the D-2 409 list. If 422, change to `ErrorType.Business`.

---

#### MEDIUM-4: Commented-out error codes in `ErrorType.cs`

Lines 14-49 contain all 30+ error codes commented out — these were the developer's first attempt before understanding that error codes belong in the `Errors` catalog, not the enum. This is dead code that confuses readers.

**Fix:** Remove all commented-out lines from `ErrorType.cs`.

---

#### MEDIUM-5: Stray comment and unnecessary using directives

All four files (`Result.cs`, `Error.cs`, `ErrorType.cs`, `Errors.cs`) have unnecessary imports:
```csharp
using System;
using System.Collections.Generic;
using System.Text;
```

These are not used anywhere in the files. `Result.cs` line 9 also has a stray `//` comment after `IsSuccess`.

**Fix:** Remove unused `using` statements. Remove stray comment. With `ImplicitUsings enable`, `System` is already available.

---

#### MEDIUM-6: Old-style namespace declarations

All files use block-scoped namespaces:
```csharp
namespace TEDx.Domain.Common.Errors
{
    public sealed class Result<T> { ... }
}
```

Modern C# 10+ convention (and what `.editorconfig` should enforce) is file-scoped:
```csharp
namespace TEDx.Domain.Common.Errors;

public sealed class Result<T> { ... }
```

**Fix:** Convert all files to file-scoped namespaces.

---

#### MEDIUM-7: Error message quality issues in audit-Issue-30 entries

| Field | Code string | Message | Issue |
|-------|-------------|---------|-------|
| `IllegalStatusTransition` | `ILLEGAL_STATUS_TRANSITION` | `"the status is illegal."` | Lowercase start, vague |
| `EventHasOrders` | `EVENT_HAS_ORDERS` | `"Event has order."` | Singular "order" |
| `SessionHasRecords` | `SESSION_HAS_RECORDS` | `"Session has records."` | Vague — what records? |
| `CapacityBelowSolid` | `CAPACITY_BELOW_SOLD` | `"Capacity is Solid."` | Typo: "Solid" should be "Sold"; message doesn't describe the problem |

Also: the field name `CapacityBelowSolid` has a typo — should be `CapacityBelowSold` to match the code string `CAPACITY_BELOW_SOLD`.

**Suggested messages:**
- `"This status transition is not allowed."`
- `"Event cannot be modified because it has existing orders."`
- `"Session cannot be deleted because it has attendance or evaluation records."`
- `"New capacity cannot be lower than the number of tickets already sold."`

---

#### MEDIUM-8: No grouping in the Errors catalog

All 40 errors are flat fields in a single class. As the catalog grows during feature sprints, this becomes hard to navigate. The docs pattern (§3.2) organizes code by bounded context.

**Recommendation:** Group with nested static classes:

```csharp
public static class Errors
{
    public static class Auth { ... }
    public static class Ticketing { ... }
    public static class Training { ... }
    public static class Promos { ... }
    public static class General { ... }
}
```

Usage: `Errors.Ticketing.SeatsUnavailable` — self-documenting and context-aware.

---

#### MEDIUM-9: Inconsistent indentation in Errors.cs

Lines 118-127 have inconsistent leading whitespace (some entries use 8 spaces, the last four use 9-10). Minor but visible in code review.

**Fix:** Normalize to consistent indentation (8 spaces or 2 tabs).

---

#### LOW-3: No `IsFailure` convenience property on `Result<T>`

Common in Result pattern implementations. `if (result.IsFailure)` reads better than `if (!result.IsSuccess)`.

```csharp
public bool IsFailure => !IsSuccess;
```

---

#### LOW-4: Nullable `Error?` on a value type

`Error` is a `readonly record struct` (value type). Making it nullable (`Error?`) causes boxing on the heap, defeating the value-type performance benefit. Options:
1. Make `Error` a `record class` instead (reference type, nullable is natural).
2. Keep as struct but use `default` as sentinel for success (check `Error.Code is null`).

Not blocking, but worth addressing if performance matters.

---

## Summary Scoreboard

| Severity | Task 1 | Task 2 | Total |
|----------|--------|--------|-------|
| CRITICAL | 3 | 2 | **5** |
| HIGH | 3 | 3 | **6** |
| MEDIUM | 3 | 6 | **9** |
| LOW | 2 | 2 | **4** |

### Task 1 DoD Assessment

| Criterion | Status |
|-----------|--------|
| Solution compiles with correct dependencies | PASS |
| No circular references | PASS |
| `dotnet build` succeeds | PASS |
| Context folders exist in each layer | PARTIAL — names wrong, 2 contexts missing |
| `Directory.Build.props` | FAIL — does not exist |
| `.editorconfig` | FAIL — does not exist |
| `.gitignore` | FAIL — missing .NET entries |

**Verdict:** DoD NOT fully met. Three explicitly required deliverables are missing.

### Task 2 DoD Assessment

| Criterion | Status |
|-----------|--------|
| `Result<T>` compiles | PASS |
| `Error` record with all five types | PASS |
| `Errors` catalog has all §0.9 codes | PASS (40/40 codes) |
| No HTTP/logging dependency in Domain/Application | PASS |
| Each code maps to exactly one HTTP status (audit-Issue-10) | FAIL — `RESET_TOKEN_INVALID` violates this |

**Verdict:** Structurally sound, but two ErrorType assignments violate the spec's status-mapping rules.

---

## Recommended Fix Order

1. **`.gitignore`** (CRITICAL-1) — fix immediately, `bin/obj` in git causes merge noise for every dev.
2. **`Directory.Build.props`** (CRITICAL-2) — quick win, 15 minutes.
3. **`.editorconfig`** (CRITICAL-3) — quick win, use dotnet template or VS defaults.
4. **`RESET_TOKEN_INVALID` type** (CRITICAL-4) — decide on adding `ErrorType.BadRequest`.
5. **`RATE_LIMITED` type** (CRITICAL-5) — decide on adding `ErrorType.RateLimited` or handling separately.
6. **Context folder names** (HIGH-2) — rename `Eventing/` to `Ticketing/`, add `Communications/`.
7. **Infrastructure structure** (HIGH-3) — reorganize by concern.
8. **Non-generic `Result`** (HIGH-4) — add before any handler implementation.
9. **Split `Unauthorized` type** (HIGH-5) — add `Forbidden` before building the mapper (Task 4).
10. Everything else (MEDIUM/LOW) — batch as cleanup pass.
