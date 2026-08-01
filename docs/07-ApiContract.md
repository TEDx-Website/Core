# TEDxAlkawmia — API Contract

> **Version:** 1.4
> **Date:** 2026-08-01
> **Reads from:** [01 — PRD](./01-PRD.md) · [02 — SRS](./02-SRS.md) · [03 — User Flows](./03-UserFlows.md) · [05 — User Stories](./05-UserStories.md) · [06 — Acceptance Criteria](./06-AcceptanceCriteria.md)
> **Decisions:** grilling sessions 2026-07-20 to 2026-07-31 — **Q1–Q57** (requirements Q1–Q28 + architecture Q29–Q55 + Q56 + Q57), cited as **(D:Qn)**.
>
> **v1.4 (2026-08-01) — EP-AUTH frontend sign-off pass.** No behaviour changes; §1 and §2 are now complete enough for the SPA to be built against without reading the backend. Every §1/§2 endpoint had its request/response shape reconciled against the implementation docs, which closed six real gaps: `POST /auth/register` publishes `firstName`/`lastName` in the 201 body and `confirmPassword` in the request; `WEAK_PASSWORD` (422) is attached to register, reset-password, and change-password instead of sitting unreferenced in §0.9; `POST /auth/logout` documents that **every** outcome is `204` (optional/unknown/foreign token) and that the access token survives; `POST /auth/reset-password` and `POST /me/change-password` document the server-side `confirmPassword` check and the full refresh-token revocation that logs the caller out; `POST /auth/confirm-email` returns a real body (`email`, `emailConfirmed`), not `data:null`; `POST /me/profile-picture` promotes `INVALID_FILE_TYPE`/`FILE_TOO_LARGE` to **top-level codes** (D-2) and registers them in §0.9; `GET /me` documents why `assignments` is two nullable scalars and where the FE gets `enrollmentId`; `PUT /me` documents **replace-not-patch** semantics and field lengths.
>
> **v1.3 (2026-07-31):** **Email confirmation (D:Q57)** — new `POST /auth/confirm-email` and `POST /auth/resend-confirmation`; registration returns `emailConfirmationRequired`; login gains a `403 EMAIL_NOT_CONFIRMED` branch (checked after the password, so it is not an enumeration oracle); new codes `EMAIL_NOT_CONFIRMED` (403) and `CONFIRM_TOKEN_INVALID` (400); §0.7 auth rate-limit group extended with the two new endpoints. Also: `POST /auth/reset-password` gains the **required `email` field** it was missing — Identity resolves the user before validating the token, so the token alone was not a sufficient request body (contract gap, not a behaviour change).
>
> **v1.2 (2026-07-24):** §6 `POST /events/{id}/status` documents **Archived→Cancelled (D:Q56)**; `POST /events/{id}/cancel` widens the precondition from "Published" to "**Published or Archived**" (identical ripple). Provenance refreshed to Q1–Q56.
>
> **v1.1 (2026-07-21):** Model-B ticketing (DecisionLog Q1 addendum) — events carry `ticketPrice` + optional `maxIndividualQtyPerOrder`; quote/reserve `packageId` is **nullable** (omit for an individual-ticket order); publishing has **no package precondition**. Error-model cleanup (D-2): promo failures are `422` + flat codes (no `PROMO_INVALID` umbrella; `PROMO_NOT_STARTED` → `PROMO_NOT_YET_VALID`), session preconditions are `422`, over-max quantity is `422 QUANTITY_EXCEEDS_MAX`; change-password wrong-current is `400 CURRENT_PASSWORD_INCORRECT` (D-3) so `INVALID_CREDENTIALS` stays uniquely `401`.

---

## 0. Conventions (apply to every endpoint)

### 0.1 Base path & versioning (D:Q27)
- All endpoints are under **`/api/v1`**. The version is in the URL path.
- Transport is **HTTPS only** (NFR-SEC-06). CORS restricts origins to the known frontend(s).

### 0.2 Response envelope — Result pattern (D:Q25)
Every response — success or failure — uses the same envelope:

```jsonc
// Success
{ "success": true, "data": <payload-or-null>, "error": null }

// Success with a list (adds a sibling meta block, D:Q26)
{ "success": true, "data": [ ... ], "error": null,
  "meta": { "page": 1, "pageSize": 20, "totalItems": 137, "totalPages": 7 } }

// Failure
{ "success": false, "data": null,
  "error": {
    "code": "MACHINE_CODE",
    "message": "Human-readable English fallback.",
    "fieldErrors": { "email": ["Email already registered."] }, // only for validation failures
    "traceId": "0HMV..."                                        // optional correlation id
  } }
```

- `code` is a **stable machine-readable string** the client maps to i18n messages (NFR-USE-02). `message` is the English fallback. `fieldErrors` appears **only** for input-validation failures.
- Internal detail (stack traces, SQL) is **never** in the payload; it is logged server-side (NFR-MNT-03) with the `traceId`.

### 0.3 HTTP status usage
| Status | Meaning in this API |
|--------|---------------------|
| 200 | OK (read, or command with a body) |
| 201 | Created (new resource) |
| 204 | No content (command with no body) |
| 400 | Malformed request |
| 401 | Missing/invalid/expired access token |
| 403 | Authenticated but not authorized (wrong role / cross-track) |
| 404 | Resource not found (or hidden from this caller) |
| 409 | Conflict — concurrency token mismatch, price changed, state conflict |
| 422 | Validation failed (`fieldErrors` populated) |
| 429 | Rate limited (`Retry-After` header, `code=RATE_LIMITED`) (D:Q28) |
| 500 | Unhandled server error (generic message only) |

### 0.4 Authentication
- Access token sent as **`Authorization: Bearer <jwt>`** (D:Q24). Claims: account id, email, **global role only** — per-track authority resolved per request (FR-AUTH-06).
- Refresh token travels in the **JSON body** of refresh/login responses (not a cookie) (D:Q24).

### 0.5 Wire formats (D:Q27)
- **Dates/times:** ISO 8601 UTC with `Z` (e.g. `2026-08-01T18:30:00Z`). Stored UTC; localized client-side.
- **Money:** JSON number with 2 decimals (`150.00`), always paired with a `currency` field (`"EGP"`). Piastres are never exposed — conversion happens only at the Paymob boundary.
- **IDs:** GUID strings. Public ticket reference is a separate short human string (e.g. `TKT-7F3A9C`).
- **Enums:** PascalCase strings (`PendingPayment`, `CheckedIn`), never raw integers.

### 0.6 Pagination, sorting, filtering (D:Q26)
- Query params: `?page=1&pageSize=20&sort=field:dir&<named filters>`.
- `pageSize` default **20**, max **100** (over-max is clamped or rejected per endpoint note).
- `sort` = `field:asc|desc`; sortable fields are **whitelisted per endpoint** (unknown field → 422).
- Filters are explicit named params documented per endpoint.

### 0.7 Idempotency & rate limiting (D:Q28)
- **`Idempotency-Key`** header is accepted on **payment initiation**; a repeat with the same key returns the same checkout session rather than creating a new one.
- Rate-limited endpoint groups (NFR-SEC-10, D:Q28): **auth** (`/auth/login`, `/auth/register`, `/auth/forgot-password`, `/auth/reset-password`, `/auth/refresh`, `/auth/confirm-email`, `/auth/resend-confirmation`), **ordering** (`/orders/quote`, `/orders/reserve`, `/orders/{id}/pay`), and **contact** (`/contact`, by IP). Limits are config-driven per group ("SHOULD" targets, D:Q28b). Exceeding a limit → **429** with `error.code = "RATE_LIMITED"` and a `Retry-After` header (seconds).
- Within the **auth** group, the mail-sending endpoints (`/auth/forgot-password`, `/auth/resend-confirmation`) take a **tighter** limit than the rest: each request costs an outbound email, so an unthrottled loop turns the API into a mail-bomb relay against a third party's inbox and burns the provider quota (D:Q57).
- The mail-sending limit is enforced on **two independent dimensions** — per **target email** and per **client IP** — and a request must satisfy both. Client-visible consequence: a `429` on `/auth/resend-confirmation` is **not** cleared by retrying from a different network, because the per-email counter is unaffected by the caller's IP. Clients must surface `Retry-After` rather than prompting the user to retry immediately.

### 0.8 Optimistic concurrency (D:Q22, NFR-REL-06)
- Admin-managed records (Event, Package, PromoCode, Order, TrackAssignment) carry a `rowVersion` (base64 string). Mutations must echo it back; a stale token → **409** `CONCURRENCY_CONFLICT`.
- **Order `rowVersion` is required only for the Admin void operation** (§12) — Attendee order operations (reserve, cancel, pay, confirm-free) mutate the order via guarded transition methods (`MarkAsPaid`/`Cancel`/`Expire`, D:Q55) and do **not** submit `rowVersion` (DataModel §2.3).

### 0.9 Common error codes (non-exhaustive, extended per section)
`VALIDATION_ERROR`, `UNAUTHENTICATED`, `FORBIDDEN`, `TRACK_FORBIDDEN`, `NOT_FOUND`, `CONCURRENCY_CONFLICT`, `RATE_LIMITED`, `EMAIL_TAKEN`, `INVALID_CREDENTIALS`, `CURRENT_PASSWORD_INCORRECT`, `ACCOUNT_DEACTIVATED`, `EMAIL_NOT_CONFIRMED`, `WEAK_PASSWORD`, `INVALID_FILE_TYPE`, `FILE_TOO_LARGE`, `TOKEN_INVALID`, `TOKEN_REUSED`, `RESET_TOKEN_INVALID`, `CONFIRM_TOKEN_INVALID`, `TICKET_ALREADY_CHECKED_IN`, `WRONG_EVENT`, `TICKET_VOIDED`, `TICKET_INVALID`, `PRICE_CHANGED`, `SEATS_UNAVAILABLE`, `QUANTITY_EXCEEDS_MAX`, `ACTIVE_ORDER_EXISTS`, `HOLD_EXPIRED`, `ORDER_NOT_CANCELLABLE`, `ORDER_NOT_PAYABLE`, `ORDER_IS_FREE`, `ORDER_NOT_FREE`, `PROMO_CODE_TAKEN`, `PROMO_INACTIVE`, `PROMO_NOT_YET_VALID`, `PROMO_EXPIRED`, `PROMO_CAP_REACHED`, `PROMO_USER_LIMIT`, `PROMO_WRONG_EVENT`, `NO_RECIPIENTS_RESOLVED`.

> **Error-model convention (D-2, audit).** A **well-formed request that violates a business rule** returns **`422`** with a **flat, distinct `error.code`** (e.g. `QUANTITY_EXCEEDS_MAX`, the `PROMO_*` reasons, `SESSION_NOT_OCCURRED`, `MEMBER_NOT_ENROLLED`). **`409`** is reserved for genuine **state/concurrency conflicts** (`CONCURRENCY_CONFLICT`, `PRICE_CHANGED`, `SEATS_UNAVAILABLE`, `ACTIVE_ORDER_EXISTS`, `HOLD_EXPIRED`, event/order state transitions). There is no `PROMO_INVALID` umbrella code — each promo failure has its own reason.

> **Status/code pairing (audit-Issue-10).** A given `error.code` maps to exactly one HTTP status. Notably: token failures on the **refresh** credential use `401 TOKEN_INVALID` / `401 TOKEN_REUSED`; an invalid/expired **password-reset** token (a submitted field, not a session credential) uses `400 RESET_TOKEN_INVALID` — a distinct code so clients never see the same code under two statuses. By the same rule an invalid **email-confirmation** token uses `400 CONFIRM_TOKEN_INVALID`, and the unconfirmed-login gate uses `403 EMAIL_NOT_CONFIRMED` (**403**, not 401 — the credentials were correct; it is the account state that forbids the session, exactly like `ACCOUNT_DEACTIVATED`).
>
> **State-transition codes (audit-Issue-30).** An illegal lifecycle transition (Event, Order, Session status) is a **state conflict → `409`** with a single shared code family: **`ILLEGAL_STATUS_TRANSITION`** for the generic case, plus the specific state codes (`EVENT_HAS_ORDERS`, `SESSION_HAS_RECORDS`, `CAPACITY_BELOW_SOLD`, …). This aligns with the D-2 convention (409 = state/concurrency); **`INVALID_STATUS_TRANSITION` (422) is retired** — sessions use `409 ILLEGAL_STATUS_TRANSITION` like events/orders. A `422` transition code is never used.

### 0.10 Internationalized text fields (DataModel §0, audit-Issue-33)
- Entities with bilingual copy (Event `title`/`description`, Package `name`, Track `name`/`description`, Session `title`) store **explicit `En`/`Ar` column pairs** (DataModel). The API is **explicit, not content-negotiated**: request and response bodies carry the **suffixed field pairs** — `titleEn` + `titleAr`, `descriptionEn` + `descriptionAr`, `nameEn` + `nameAr`. There is **no `Accept-Language` server-side selection**; the SPA picks the language to render.
- On **write**, both members of a required pair are mandatory (missing → `422 VALIDATION_ERROR` on the missing field). Fields the DataModel marks nullable (e.g. an optional Armenian description) may be omitted.
- **`date` is never a single field on events** — see §0.11.

### 0.11 Event date fields (DataModel §2.1, audit-Issue-17/18)
- An Event exposes **two** UTC instants, mirroring the DataModel columns `StartsAtUtc`/`EndsAtUtc`: **`startsAtUtc`** and **`endsAtUtc`** (both ISO 8601 UTC with `Z`). The earlier single `date` field is **replaced** everywhere by this pair so the SPA can render duration and the "upcoming/past" split (date-derived on `startsAtUtc`, D:Q23). "No-show" derivation and attendance-recording preconditions use `endsAtUtc` (D:Q7, D:Q12).

---

## 1. Authentication (SRS §3.1 · US-AUTH-01..08)

### POST `/api/v1/auth/register`
Create an Attendee account. **Public.**
```jsonc
// Request
{ "firstName": "Nour", "lastName": "Adel", "email": "nour@example.com",
  "password": "Str0ngPass", "confirmPassword": "Str0ngPass" }
// 201 → data
{ "id": "3f...", "email": "nour@example.com",
  "firstName": "Nour", "lastName": "Adel",
  "globalRole": "Attendee", "emailConfirmationRequired": true }
```
- **Errors:** 422 `VALIDATION_ERROR` (format/mismatch → `fieldErrors`); 422 `WEAK_PASSWORD` (policy not met); 409 `EMAIL_TAKEN`.
- Password policy ≥ 8 chars, ≥1 upper, ≥1 lower, ≥1 digit (server-enforced, FR-AUTH-03).
- **No tokens are returned** — there is no auto-login. The account is created with the email **unconfirmed** and a confirmation link is emailed (FR-AUTH-12, D:Q57); `emailConfirmationRequired` is always `true` and exists so the client routes to a "check your inbox" screen rather than the logged-in shell. A mail-provider failure does **not** fail this call (the user can resend), so a `201` is not proof the email was delivered.

### POST `/api/v1/auth/login`
**Public.** Returns token pair.
```jsonc
// Request
{ "email": "nour@example.com", "password": "Str0ngPass" }
// 200 → data
{ "accessToken": "<jwt>", "accessTokenExpiresIn": 900,
  "refreshToken": "<opaque>", "refreshTokenExpiresIn": 604800,
  "user": { "id": "3f...", "email": "nour@example.com", "globalRole": "Attendee",
            "firstName": "Nour", "lastName": "Adel" } }
```
- **Errors:** 401 `INVALID_CREDENTIALS` (generic, for unknown email **or** wrong password — no enumeration); 403 `ACCOUNT_DEACTIVATED`; 403 `EMAIL_NOT_CONFIRMED` (FR-AUTH-13, D:Q57).
- **Gate order is normative:** password first, then `IsActive`, then `EmailConfirmed`. The two `403`s are only ever returned to a caller who already proved the password, so neither leaks account existence; a wrong password against an unconfirmed or deactivated account still returns the generic `401`. On `EMAIL_NOT_CONFIRMED` the client should offer **resend confirmation**; on `ACCOUNT_DEACTIVATED` it should not.

### POST `/api/v1/auth/refresh`
**Public** (refresh token is the credential). Single-use rotation (D:Q24).
```jsonc
// Request
{ "refreshToken": "<opaque>" }
// 200 → data: same shape as login (new pair)
```
- **Errors:** 401 `TOKEN_INVALID` (expired/unknown); 401 `TOKEN_REUSED` (consumed/revoked → **whole family revoked**, force re-login).

### POST `/api/v1/auth/logout`
**Authenticated.** Revokes the presented refresh token.
```jsonc
// Request
{ "refreshToken": "<opaque>" }
// 204
```
- **Errors:** 401 `UNAUTHENTICATED` (missing/expired **access** token — this endpoint is authenticated, unlike `/auth/refresh`). Note this is *not* `TOKEN_INVALID`, which per §0.9 belongs to the refresh credential alone.
- `refreshToken` is **optional**. Omitting it still returns `204`; it just means only the client-side session is dropped and that refresh token stays alive until it expires. Send it — the FE has it, and this is the only way the server-side session actually dies.
- An unknown, already-revoked, or **another account's** refresh token also returns `204`. The server verifies the token belongs to the caller and silently does nothing otherwise; it never reports which of those happened, so this endpoint cannot be used to probe whether a token is valid.
- The **access token stays valid** for the remainder of its ≤15 minutes — nothing revokes a JWT (D:Q24). The FE must discard it in memory on logout rather than rely on the server rejecting it.

### POST `/api/v1/auth/forgot-password`
**Public.** Always neutral response (no enumeration, FR-AUTH-10).
```jsonc
// Request
{ "email": "nour@example.com" }
// 200 → data: null   (identical whether or not the account exists)
```

### POST `/api/v1/auth/reset-password`
**Public.** Consumes a single-use, time-limited reset token (default 1h, D:Q24).
```jsonc
// Request
{ "email": "nour@example.com", "token": "<reset-token>",
  "newPassword": "N3wStr0ng", "confirmPassword": "N3wStr0ng" }
// 200 → data: null
```
- **Errors:** 422 `VALIDATION_ERROR` (missing field, malformed email, `newPassword` ≠ `confirmPassword`); 422 `WEAK_PASSWORD` (fails Identity's complexity rules); 400 `RESET_TOKEN_INVALID` (used/expired/unknown email/token-email mismatch). On success, existing refresh tokens for the account **MUST** be revoked (D:Q24, NFR-SEC-02).
- `confirmPassword` is checked **server-side**, not only in the browser. It is a `422 VALIDATION_ERROR` with a `fieldErrors` entry on `confirmPassword` — distinct from `WEAK_PASSWORD`, which is about the password's strength rather than the two fields disagreeing. The FE can surface them on different inputs.
- **`email` is required** (added v1.3). Identity validates a reset token *against a resolved user*, so the token alone cannot identify the account. The client takes both values from the reset link's query string and posts them in the **body** — never re-sending them as query parameters, which would land the token in server logs and `Referer` headers.
- An unknown email returns `400 RESET_TOKEN_INVALID`, identical to a bad token — this endpoint must not become the enumeration oracle that `/auth/forgot-password` carefully avoids.

### POST `/api/v1/auth/confirm-email`
**Public.** Consumes a 24-hour confirmation token (FR-AUTH-14, D:Q57).
```jsonc
// Request
{ "userId": "3f...", "token": "<confirm-token>" }
// 200 → data
{ "email": "nour@example.com", "emailConfirmed": true }
```
- **Errors:** 400 `CONFIRM_TOKEN_INVALID` (expired/forged/tampered/unknown user); 422 `VALIDATION_ERROR` (missing field).
- Confirming an **already-confirmed** account returns **200** with the same body, not an error — mail clients and link-prefetchers routinely fire the link twice, and a second click must not read as failure.
- Success does **not** log the user in; the client redirects to sign-in. Returning `email` lets it pre-fill that field.
- The emailed link points at the **frontend** (`{FrontendBaseUrl}/confirm-email?userId=…&token=…`), which reads the query string and POSTs this endpoint — the same indirection as password reset. Both values are URL-encoded in the link (`Uri.EscapeDataString`); the token is base64-ish and **will** contain `+` and `/`, which silently corrupt if unencoded.

### POST `/api/v1/auth/resend-confirmation`
**Public.** Always neutral response (no enumeration, FR-AUTH-15).
```jsonc
// Request
{ "email": "nour@example.com" }
// 200 → data: null   (identical whether the account is unknown, already confirmed, or pending)
```
- **Errors:** 422 `VALIDATION_ERROR` (malformed email); 429 `RATE_LIMITED` (+ `Retry-After`).
- An email is sent **only** when an account exists and is genuinely unconfirmed. The response body, status, and timing are otherwise indistinguishable across all three states.
- Each resend issues an **additional valid token**; it does not revoke earlier ones. Identity derives confirmation tokens from the account's `SecurityStamp` and neither generating nor consuming one rotates that stamp, so every unexpired link for the account keeps working until its own 24 hours run out. Clicking an older link is therefore *not* an error — do not design the email copy or the FE around "only the newest link works." (Contrast password reset, where `ResetPasswordAsync` **does** rotate the stamp, making reset tokens genuinely single-use.)

---

## 2. User & Profile (SRS §3.2 · US-USER-01..04)

### GET `/api/v1/me`
**Authenticated.** Own profile incl. role and track assignments.
```jsonc
// 200 → data
{ "id": "3f...", "firstName": "Nour", "lastName": "Adel", "email": "nour@example.com",
  "phone": "+20...", "bio": "…", "profilePictureUrl": "https://res.cloudinary.com/…",
  "globalRole": "Attendee",
  "assignments": { "memberOfTrackId": null, "boardOfTrackId": null } }
```
- **`assignments` is two nullable scalars, not an array** — and the FE can rely on that. The DataModel enforces it physically: `UQ_Assignment_OneActiveMember` and `UQ_Assignment_OneActiveBoard` cap a user at **≤ 1 active Member track and ≤ 1 active Board track**. Both may be set at once (`Member@X` + `Board@Y` is the sanctioned dual role); both being the *same* track is rejected server-side (`MEMBER_BOARD_SAME_TRACK`).
- Only **active** assignments appear here. Ended ones (`EndedAtUtc != null`) are retained in the database for attendance history but are **not** reported — a user whose enrollment ended reads as `null`, exactly like one who never enrolled.
- **No track names and no `enrollmentId` here, by design.** This response is the session/identity payload; the FE gets the Member-side details (`enrollmentId`, `trackNameEn`/`trackNameAr`, progress) from **`GET /me/enrollment`** (§14) and the Board side from **`GET /me/board-dashboard`**. Duplicating them would give the SPA two sources of truth for the same track that drift the moment an admin reassigns someone.
- **`globalRole` here is authoritative over the JWT claim.** The token carries the role at issue time and lives up to 15 minutes; a role changed by an admin shows up here on the next fetch but *not* in the current access token. Render from `/me`, and never gate a **security** decision client-side on either value — the server re-checks on every request (§0.4).

### PUT `/api/v1/me`
**Authenticated.** Edit own mutable fields. **Email is immutable** (FR-USER-02).
```jsonc
// Request
{ "firstName": "Nour", "lastName": "Adel", "phone": "+20...", "bio": "…" }
// 200 → data: updated profile   (same shape as GET /me, including assignments)
```
- **Errors:** 422 `VALIDATION_ERROR` (missing/over-length field, with `fieldErrors` per input).
- **This is a `PUT`, so it replaces — it does not patch.** `firstName` and `lastName` are **required**; `phone` and `bio` are nullable and **omitting them clears the stored value**. The FE must send the full object it read from `GET /me`, not just the inputs the user touched, or editing a name will silently wipe the bio.
- Lengths (DataModel §1.1): `firstName`/`lastName` ≤ **100**, `phone` ≤ **32**, `bio` ≤ **1000**. Mirror these as `maxlength` client-side so the user hits the limit while typing instead of on submit.
- `email`, `globalRole`, `isActive`, and `assignments` are **not** accepted here. Sending them is not an error — they are ignored. Email changes are unsupported product-wide (there is no verified-change flow); role and activation are admin-only (§3, §4).

### POST `/api/v1/me/profile-picture`
**Authenticated.** `multipart/form-data`, field `file`. Validated for image type + size (FR-USER-03).
```jsonc
// 200 → data
{ "profilePictureUrl": "https://res.cloudinary.com/…" }
```
- **Errors:** 422 `INVALID_FILE_TYPE`; 422 `FILE_TOO_LARGE`. These are **top-level `error.code` values**, not `fieldErrors` nested under `VALIDATION_ERROR` — per D-2 a well-formed request that breaks a business rule gets its own flat code, and the FE needs to switch on them to show "PNG/JPEG only" versus "under 2 MB".
- Allowed types and the size ceiling are configuration, not contract; the FE must not hardcode a limit for validation — it should let the server decide and render `error.message`.

### POST `/api/v1/me/change-password`
**Authenticated.** Requires current + new password.
```jsonc
// Request
{ "currentPassword": "Str0ngPass", "newPassword": "N3wStr0ng", "confirmPassword": "N3wStr0ng" }
// 200 → data: null
```
- **Errors:** 422 `VALIDATION_ERROR` (missing field, `newPassword` ≠ `confirmPassword`); 422 `WEAK_PASSWORD` (fails complexity rules); 400 `CURRENT_PASSWORD_INCORRECT` (current wrong).
- On success, **all** refresh tokens for the account are revoked (same rule as reset-password). The caller's own session dies too: its next `/auth/refresh` returns `401 TOKEN_INVALID`, so the FE must either re-authenticate immediately or treat a successful change-password as a logout. This is deliberate — a password change is how a user evicts someone else from their account.

---

## 3. Admin — Users (SRS §3.2 · US-MNG-01..04)

### GET `/api/v1/admin/users`
**Admin.** Paginated list.
- **Query:** `page`, `pageSize`, `sort` (whitelist: `createdAt`, `lastName`, `email`), `search` (name/email), `globalRole` (`Attendee|Admin`), `status` (`active|inactive`).
```jsonc
// 200 → data: [ { "id", "firstName", "lastName", "email", "globalRole",
//                 "isActive", "memberOfTrackId", "boardOfTrackId" } ], meta: {...}
```

### GET `/api/v1/admin/users/{id}`
**Admin.** Full detail incl. assignments and status (US-MNG-02).
```jsonc
// 200 → data
{ "id", "firstName", "lastName", "email", "phone", "bio", "profilePictureUrl",
  "globalRole": "Attendee", "isActive": true, "createdAtUtc": "…",
  "assignments": { "memberOfTrackId": null, "boardOfTrackId": null } }
```

### POST `/api/v1/admin/users/{id}/deactivate`
**Admin.** Soft-deactivate with cross-context ripple (D:Q10).
```jsonc
// 200 → data (effect summary)
{ "userId": "…", "isActive": false,
  "cancelledPendingOrders": 1, "ticketsRemainValid": 3,
  "endedAssignments": { "member": "trackX", "board": "trackY" },
  "trackNeedingSupervisor": "trackY" }
```
- Deactivated user cannot log in/refresh; refresh tokens revoked; active PendingPayment order cancelled + seats released; Issued tickets remain valid; **track assignments are ended (`EndedAt` set), freeing the dual-role slots (FR-ROLE-04)**; supervised track flagged (D:Q6/Q10, Issue 6).

### POST `/api/v1/admin/users/{id}/reactivate`
**Admin.** Restores login ability only. **Ended assignments are NOT restored** — the user returns as a plain Attendee and must be re-enrolled / re-assigned explicitly (Issue 6).

### GET `/api/v1/admin/users/{id}/deactivation-impact`
**Admin.** Dry-run for the confirmation surface (US-MNG-04): returns the same effect-summary counts **without** applying them.
```jsonc
// 200 → data (same shape as deactivate response, no side effects)
{ "userId": "…", "isActive": true,
  "cancelledPendingOrders": 1, "ticketsRemainValid": 3,
  "endedAssignments": { "member": "trackX", "board": "trackY" },
  "trackNeedingSupervisor": "trackY" }
```

---

## 4. Roles & Track Assignments (SRS §3.3 · US-ROLE-01..07)

> Dual-role rule (FR-ROLE-04, D:Q15): ≤1 active Member enrollment + ≤1 active Board assignment, and the two tracks **must differ**. Enforced at assignment time and by filtered unique indexes.

### PUT `/api/v1/admin/users/{id}/global-role`
**Admin only** (FR-ROLE-01).
```jsonc
{ "globalRole": "Admin" }   // Attendee ↔ Admin
// 200 → data: updated user
```

### POST `/api/v1/admin/tracks/{trackId}/board`
**Admin only** (FR-ROLE-02). Assign the Board of a track.
```jsonc
{ "userId": "…" }
// 200 → data: assignment
```
- **Errors:** 409 `MEMBER_BOARD_SAME_TRACK` (would make them Member **and** Board of this same track — same code as the enrollment path §14); 409 `DUAL_ROLE_CONFLICT` (target already holds a Board assignment on another track); 409 `TRACK_ALREADY_HAS_BOARD`.

### DELETE `/api/v1/admin/tracks/{trackId}/board`
**Admin only.** Ends the Board assignment (retains history, FR-ROLE-05).

> **Member enrollment/removal** (`POST`/`DELETE /api/v1/tracks/{trackId}/members`) is specified once, canonically, in **§14 (Enrollment & Member Management)** — it is not duplicated here. This section covers **global-role and Board-role** assignment only.

### GET `/api/v1/admin/tracks/{trackId}/assignments`
**Admin, or the Board of that track.** Current Board + active member enrollments.


---

## 5. Events — Public (SRS §3.4 · US-EVT-01..05)

> Only **Published** events are visible to Visitors/Attendees. "upcoming/past" is **date-derived among Published events**; Archived/Draft/Cancelled never appear publicly (D:Q23).

### GET `/api/v1/events`
**Public.** Paginated list of Published events.
- **Query:** `page`, `pageSize`, `sort` (whitelist: `startsAtUtc`, `titleEn`), `when` (`upcoming|past`, default `upcoming`).
```jsonc
// 200 → data: [ { "id", "titleEn", "titleAr", "summary",
//   "startsAtUtc": "2026-08-01T18:30:00Z", "endsAtUtc": "2026-08-01T21:00:00Z",
//   "location", "imageUrl", "capacity", "remainingSeats", "status": "Published",
//   "ticketPrice": { "amount": 200.00, "currency": "EGP" },
//   "priceFrom": { "amount": 100.00, "currency": "EGP" } } ], meta: {...}
```
- `remainingSeats` is **computed live** = `capacity − (paid + unexpired-held seats)` (D:Q3, FR-EVT-07); never cached for booking decisions (NFR-PERF-05).
- `ticketPrice` is the event's **individual-ticket face price** (Model B); `priceFrom` = `min(ticketPrice, active package unit prices)` — the cheapest way to attend.
- `upcoming|past` is derived from `startsAtUtc` relative to now (D:Q23).

### GET `/api/v1/events/{id}`
**Public** for Published; **404** otherwise (hidden from non-admins).
```jsonc
// 200 → data
{ "id", "titleEn", "titleAr", "descriptionEn", "descriptionAr",
  "startsAtUtc": "2026-08-01T18:30:00Z", "endsAtUtc": "2026-08-01T21:00:00Z",
  "location", "imageUrl", "capacity", "remainingSeats", "status": "Published",
  "ticketPrice": { "amount": 200.00, "currency": "EGP" },
  "maxIndividualQtyPerOrder": 6,
  "packages": [ { "id", "nameEn", "nameAr", "seatsPerPackage", "maxQuantityPerOrder",
                  "price": { "amount": 250.00, "currency": "EGP" }, "isActive": true } ] }
```
- The event **always** exposes an individual-ticket `ticketPrice`; **`packages` MAY be empty** — individual tickets are sold regardless (Model B). Only **active** packages of a Published event are returned to the public (FR-PKG-04).

---

## 6. Events — Admin (SRS §3.4 · US-ADM-EVT-01..07)

### GET `/api/v1/admin/events`
**Admin.** Paginated list of events across **all statuses** (Draft, Published, Archived, Cancelled) — the Admin management view (US-ADM-EVT-03/04/05, PRD ADM-03).
- **Query:** `page`, `pageSize`, `sort` (whitelist: `startsAtUtc`, `titleEn`, `createdAt`), `status` (`Draft|Published|Archived|Cancelled`), `search` (title).
```jsonc
// 200 → data: [ { "id", "titleEn", "titleAr", "startsAtUtc", "endsAtUtc",
//   "location", "capacity", "status", "ticketPrice": { "amount", "currency" },
//   "remainingSeats", "rowVersion" } ], meta: {...}
```

### GET `/api/v1/admin/events/{id}`
**Admin.** Full event detail for any status (Draft, Published, Archived, Cancelled). Returns 404 only if the event does not exist (US-ADM-EVT-02, PRD ADM-03).
```jsonc
// 200 → data
{ "id", "titleEn", "titleAr", "descriptionEn", "descriptionAr",
  "startsAtUtc", "endsAtUtc", "location", "imageUrl",
  "capacity", "remainingSeats", "status",
  "ticketPrice": { "amount": 200.00, "currency": "EGP" },
  "maxIndividualQtyPerOrder": 6, "rowVersion": "AAAA…",
  "packages": [ { "id", "nameEn", "nameAr", "seatsPerPackage", "maxQuantityPerOrder",
                  "price": { "amount": 250.00, "currency": "EGP" },
                  "isActive": true, "isDeleted": false } ] }
```

### POST `/api/v1/admin/events`
**Admin.** Create (starts as **Draft**).
```jsonc
{ "titleEn": "TEDx 2026", "titleAr": "تيدكس 2026",
  "descriptionEn": "…", "descriptionAr": "…",
  "startsAtUtc": "2026-08-01T18:30:00Z", "endsAtUtc": "2026-08-01T21:00:00Z",
  "location": "Cairo",
  "capacity": 200,
  "ticketPrice": { "amount": 200.00, "currency": "EGP" },   // individual-ticket face price (Model B)
  "maxIndividualQtyPerOrder": 6,                             // nullable = no cap (mirrors package cap, D:Q2)
  "imageUrl": null }
// 201 → data: event (status "Draft", rowVersion)
```
- **Errors:** 422 `VALIDATION_ERROR` (capacity ≤ 0 → `INVALID_CAPACITY`; `ticketPrice.amount` < 0 → `INVALID_TICKET_PRICE`; missing required i18n field → `fieldErrors`).

### PUT `/api/v1/admin/events/{id}`
**Admin.** Edit. Requires `rowVersion` (D:Q22, NFR-REL-06).
```jsonc
{ "titleEn", "titleAr", "descriptionEn", "descriptionAr",
  "startsAtUtc", "endsAtUtc", "location", "capacity",
  "ticketPrice", "maxIndividualQtyPerOrder", "imageUrl", "rowVersion": "AAAA…" }
// 200 → data: updated event
```
- **Capacity floor (D:Q22):** may be raised anytime; lowering below current `paid + held` seats → **409** `CAPACITY_BELOW_SOLD`.
- **Ticket-price edits** do **not** alter historical orders — those are snapshotted (D:Q4, NFR-REL-04). `ticketPrice.amount` < 0 → **422** `INVALID_TICKET_PRICE`.
- **Errors:** 409 `CONCURRENCY_CONFLICT` (stale rowVersion).

### POST `/api/v1/admin/events/{id}/status`
**Admin.** State transition (D:Q23).
```jsonc
{ "status": "Published" }   // legal targets validated by the state machine
```
- **State machine (D:Q23, D:Q56):** Draft⇄Published *(only while zero orders)*; Published→Archived; **Published→Cancelled**; **Archived→Cancelled (D:Q56)**; Archived→Published. **Draft→Cancelled is blocked** (dispose a zero-order Draft via soft-delete, D:Q22). **Cancelled is terminal.**
- **No package precondition (Model B):** an event with **zero packages is publishable** — individual tickets are sold at `ticketPrice`. (There is no `NO_PACKAGES` block.)
- **Errors:** 409 `ILLEGAL_STATUS_TRANSITION`; 409 `HAS_ORDERS_CANNOT_UNPUBLISH` (Published→Draft with existing orders).

### POST `/api/v1/admin/events/{id}/cancel`
**Admin.** Cancel a **Published or Archived** event with side effects (D:Q22, D:Q56).
```jsonc
// 200 → data (effect summary)
{ "eventId", "status": "Cancelled",
  "voidedTickets": 42, "checkedInTicketsRetained": 5,
  "releasedHolds": 3, "refundEntriesRecorded": 40 }
```
- Voids all **Issued** tickets (non-checked-in only — checked-in tickets are non-voidable, D:Q6), releases all PendingPayment holds, records **offline** refund entries for Paid orders (FR-PAY-07). Hidden from listings, retained.
- `checkedInTicketsRetained` = count of tickets that were CheckedIn and therefore not voided; their seats remain consumed.

### DELETE `/api/v1/admin/events/{id}`
**Admin.** Soft-delete — available for **any status** (Draft, Published, Archived) **only when the event has zero orders** (D:Q22). A Published or Archived event with orders must be cancelled first.
- **Errors:** 409 `EVENT_HAS_ORDERS` (use cancel instead).

### GET `/api/v1/admin/events/{id}/orders`
**Admin.** All orders + attendees for the event (FR-EVT-08). Paginated; filter `status`.

---

## 7. Ticket Packages — Admin (SRS §3.5 · US-ADM-PKG-01..03)

### POST `/api/v1/admin/events/{eventId}/packages`
**Admin.**
```jsonc
{ "nameEn": "Group-5", "nameAr": "مجموعة-5",
  "seatsPerPackage": 5, "maxQuantityPerOrder": 4,
  "price": { "amount": 1000.00, "currency": "EGP" } }
// 201 → data: package
```
- `seatsPerPackage` ≥ 1; `price.amount` ≥ 0 (0 = free package, FR-PKG-02); `maxQuantityPerOrder` nullable (null = no cap, D:Q2).
- **Errors:** 422 `VALIDATION_ERROR` (`INVALID_SEATS`, `INVALID_PRICE`).

### PUT `/api/v1/admin/events/{eventId}/packages/{id}`
**Admin.** Edit nameEn/nameAr/seats/price/cap/active. Requires `rowVersion`.
- Price edits do **not** alter historical orders — those are snapshotted (D:Q4, NFR-REL-04).

### POST `/api/v1/admin/events/{eventId}/packages/{id}/activate` · POST `…/deactivate`
**Admin.** Activate / deactivate a package. Deactivating hides it from new orders (historical orders keep their price snapshot); activating re-lists it. Reversible either direction.

### DELETE `/api/v1/admin/events/{eventId}/packages/{id}`
**Admin.** Soft-delete. **Errors:** 409 `PACKAGE_REFERENCED_BY_ORDERS` (cannot hard-delete; FR-PKG-03).

### GET `/api/v1/admin/events/{eventId}/packages`
**Admin.** List all packages for an event (US-ADM-PKG-03), including inactive/soft-deleted (filter `includeInactive`). Each row shows remaining seats (computed from held + issued, not stored) and redemption counts (DataModel §2.2).

---

## 8. Promo Codes — Admin (SRS §3.6 · US-ADM-PRM-01..02)

### POST `/api/v1/admin/promo-codes`
**Admin.**
```jsonc
{ "code": "TEDX20", "discountType": "Percentage",   // "Percentage" | "FixedAmount"
  "discountValue": 20,                                // percent, or EGP amount
  "globalRedemptionCap": 100, "perUserLimit": 1,
  "validFrom": "2026-07-20T00:00:00Z", "validUntil": "2026-08-01T00:00:00Z",
  "eventId": null }                                   // null = all events
// 201 → data: promo code
```
- `code` unique among **live** (non-deleted) codes (FR-PROMO-05) → 409 `PROMO_CODE_TAKEN`.
- For `Percentage`, `discountValue` ∈ [1,100] — the lower bound of `1` rejects a no-op 0% promo; the upper bound of `100` permits a 100%-off code, which yields a `finalPrice` of 0 and takes the confirm-free path (D:Q18). For `FixedAmount`, ≥ 0 EGP.
- If both bounds are present, `validFrom` **MUST** be earlier than `validUntil` (DataModel §2.7) → `422 VALIDATION_ERROR`; each bound is independently nullable.

### PUT `/api/v1/admin/promo-codes/{id}`
**Admin.** Edit caps/window/scope/active. Requires `rowVersion`.

### DELETE `/api/v1/admin/promo-codes/{id}`
**Admin.** Soft-delete (redemption history retained, FR-PROMO-04).

### GET `/api/v1/admin/promo-codes`
**Admin.** Paginated list; filter `active`, `eventId`. Includes `redemptionCount` and remaining-cap.

### GET `/api/v1/admin/events/{eventId}/promo-codes`
**Admin.** Read-only report of promo codes scoped to a specific event (US-ADM-EVT-07, D:Q50). Equivalent to `GET /admin/promo-codes?eventId={eventId}` but event-scoped for the event management UI. Each row includes `code`, `discountType`, `discountValue`, `redemptionCount`, `globalRedemptionCap`, `isActive`, `validFrom`, `validUntil`.
```jsonc
// 200 → data: [ { "id", "code", "discountType", "discountValue",
//   "redemptionCount", "globalRedemptionCap", "perUserLimit",
//   "isActive", "validFrom", "validUntil" } ], meta: {...}
```

> **Validation & redemption accounting** (applied during ordering, D:Q19) is specified in the Ordering section (§9): a promo is validated at quote (advisory), its slot **atomically claimed at payment initiation** (or at confirmation for free/100%-off), **confirmed on Paid**, and **released on payment failure / hold expiry**.


---

## 9. Quote & Ordering (SRS §3.7 · US-ORD-01..05)

### POST `/api/v1/orders/quote`
**Attendee.** Price preview — **creates nothing, holds no seats** (FR-ORD-01).
```jsonc
// request — omit packageId (or send null) for an individual-ticket order (Model B)
{ "eventId": "…", "packageId": null, "quantity": 2, "promoCode": "TEDX20" }   // packageId & promoCode optional
// 200 → data (individual-ticket example)
{ "eventId", "packageId": null, "unitType": "Individual", "quantity": 2,
  "totalSeats": 2,
  "basePrice":  { "amount": 400.00, "currency": "EGP" },
  "discount":   { "amount":  80.00, "currency": "EGP" },
  "finalPrice": { "amount": 320.00, "currency": "EGP" },
  "promo": { "code": "TEDX20", "applied": true } }
// 200 → data (package example)
{ "eventId", "packageId", "unitType": "Package", "packageName": "Group-5",
  "seatsPerPackage": 5, "quantity": 2, "totalSeats": 10,
  "basePrice": {…}, "discount": {…}, "finalPrice": {…}, "promo": {…} }
```
- Unit price = `event.ticketPrice` when `packageId` is null (individual), else the package price; `totalSeats` = `quantity` (individual) or `seatsPerPackage × quantity` (package). Discount = half-up 2dp; `finalPrice = max(base − discount, 0)` (D:Q18).
- Quote is **advisory** — reserve re-prices server-side (D:Q4).
- **Errors:** 422 `QUANTITY_EXCEEDS_MAX` (> `package.maxQuantityPerOrder`, or > `event.maxIndividualQtyPerOrder` for individual; D:Q2); 422 `VALIDATION_ERROR` (quantity < 1); 422 promo failure with a flat code `PROMO_INACTIVE | PROMO_NOT_YET_VALID | PROMO_EXPIRED | PROMO_CAP_REACHED | PROMO_USER_LIMIT | PROMO_WRONG_EVENT` (FR-PROMO-03).

### POST `/api/v1/orders/reserve`
**Attendee.** Reserve → hold seats for the **15-min** checkout window (FR-ORD-02/05).
```jsonc
// request — omit packageId (or send null) for an individual-ticket order (Model B)
{ "eventId": "…", "packageId": null, "quantity": 2, "promoCode": "TEDX20" }
// 201 → data
{ "orderId", "orderReference": "ORD-A1B2C3D4",
  "status": "PendingPayment", "unitType": "Individual",
  "totalSeats": 2, "holdExpiresAt": "2026-07-20T18:45:00Z",
  "basePrice": {…}, "discount": {…}, "finalPrice": {…},
  "priceSnapshotAt": "2026-07-20T18:30:00Z" }
```
- An order is **one unit-type × quantity** — individual tickets **or** a single package, never mixed (D:Q1). The order's package reference is **nullable** (null ⇒ individual-ticket order).
- **Concurrency-safe** capacity check at `SERIALIZABLE` (D:Q3, FR-ORD-03, NFR-REL-01) using the clock-aware held-seat predicate.
- **Snapshots** unit price, base, discount, final — plus package name (package order) or event title (individual order) (D:Q4, FR-ORD-04).
- **One active pending order per user per event** (D:Q5): a second reserve returns **409** `ACTIVE_ORDER_EXISTS` with `{ existingOrderId }` (client resumes or cancels it). Paid orders don't block.
- **Errors:** 422 `EVENT_NOT_PUBLISHED` (event is not in Published status); 409 `SEATS_UNAVAILABLE` (`{ remainingSeats }`); 409 `PRICE_CHANGED` (`{ newQuote }`, D:Q4) — client must re-confirm; 422 promo failure (flat `PROMO_*` code as above); 422 `QUANTITY_EXCEEDS_MAX`.

### GET `/api/v1/orders`
**Attendee.** Own order history, all statuses (FR-ORD-07). Paginated; filter `status`, `eventId`; sort whitelist: `createdAt:asc|desc`.
```jsonc
// 200 → data: [ { "orderId", "orderReference": "ORD-A1B2C3D4",
//   "eventId", "eventTitleEn", "status": "Paid",
//   "unitType": "Individual",   // Individual | Package
//   "totalSeats": 2, "finalPrice": { "amount": 320.00, "currency": "EGP" },
//   "holdExpiresAt": null,      // non-null only for PendingPayment
//   "createdAt": "…" } ], meta: {...}
```

### GET `/api/v1/orders/{id}`
**Attendee (owner) / Admin.** Order detail incl. snapshot prices, status, `holdExpiresAt`, and tickets (if Paid). **403** if not owner/admin.
```jsonc
// 200 → data
{ "orderId", "orderReference": "ORD-A1B2C3D4",
  "eventId", "eventTitleEn", "eventTitleAr",
  "status": "Paid", "unitType": "Individual",
  "quantity": 2, "totalSeats": 2,
  "basePrice": { "amount": 400.00, "currency": "EGP" },
  "discount":  { "amount":  80.00, "currency": "EGP" },
  "finalPrice": { "amount": 320.00, "currency": "EGP" },
  "priceSnapshotAt": "…", "holdExpiresAt": null,
  "promo": { "code": "TEDX20", "applied": true },
  "tickets": [ { "ticketId", "publicReference": "TKT-7F3A9C",
                 "guestName": null, "status": "Issued" } ] }
```

### POST `/api/v1/orders/{id}/cancel`
**Attendee (owner).** Cancel an **unpaid** order → releases held seats immediately (FR-ORD-06).
- **Errors:** 409 `ORDER_NOT_CANCELLABLE` (already Paid — Paid voiding is Admin-only, D:Q6; or already Cancelled/Expired).

---

## 10. Payment (SRS §3.8 · Paymob · US-PAY-01..02)

### POST `/api/v1/orders/{id}/pay`
**Attendee (owner).** Initiate payment for a PendingPayment order with `finalPrice > 0`.
- **Header:** `Idempotency-Key: <uuid>` (optional) — a repeat with the same key returns the **same** checkout session, no new Paymob intention (D:Q28a).
```jsonc
// 200 → data
{ "orderId", "checkoutUrl": "https://accept.paymob.com/…",
  "paymentSessionId": "…", "amount": { "amount": 1600.00, "currency": "EGP" },
  "expiresAt": "2026-07-20T18:45:00Z" }
```
- **Promo redemption slot atomically claimed here** (D:Q19) → 409 `PROMO_CAP_REACHED` if it filled since reserve.
- Amount sent to Paymob is `finalPrice × 100` piastres (converted only at the boundary, NFR-CMP-03).
- **Errors:** 409 `HOLD_EXPIRED` (window elapsed → order Expired, seats released, D:Q3); 409 `ORDER_NOT_PAYABLE` (not PendingPayment); 409 `ORDER_IS_FREE` (finalPrice 0 → use confirm-free path).

### POST `/api/v1/orders/{id}/confirm-free`
**Attendee (owner).** Confirm a `finalPrice == 0` order (free package or 100%-off promo) — **bypasses the gateway** (D:Q18, FR-PAY-06).
- Redemption slot claimed **at confirmation** (D:Q19). On success → order **Paid**, tickets issued immediately.
- **Errors:** 409 `ORDER_NOT_FREE` (finalPrice > 0); 409 `HOLD_EXPIRED`.

### POST `/api/v1/webhooks/paymob`
**Public (HMAC-verified).** Paymob server-to-server callback. **Not** called by the SPA.
- **MUST** verify HMAC signature before acting (FR-PAY-02, NFR-SEC-04); unsigned/mismatched → **400** `INVALID_SIGNATURE`, no state change.
- **MUST** validate reported amount == order's snapshotted `finalPrice` piastres (FR-PAY-04) → mismatch logged, order **not** marked Paid.
- **Idempotent** (D:Q28, FR-PAY-03): replay for an already-Paid order returns 200 with no duplicate tickets/seats.
- On verified success: order → **Paid**, **one ticket per held seat issued** (FR-TKT-01), promo redemption **confirmed** (D:Q19), payment attempt recorded (status, txn id, amount, raw verified payload, FR-PAY-05).
- On verified failure / expiry: promo slot **released** (D:Q19); order stays PendingPayment until it pays or the hold expires.

### GET `/api/v1/admin/payments`
**Admin.** Payment attempts across orders (FR-PAY-05; US-PAY-05); filter `orderId`, `status`, `eventId`. For reconciliation/support.

---

## 11. Tickets & Check-in (SRS §3.9 · US-TKT-01..02, US-CHK-01..04)

### GET `/api/v1/orders/{id}/tickets`
**Attendee (owner) / Admin.** Tickets for a **Paid** order (FR-ORD-07). Unpaid orders → empty list (FR-TKT-02).
```jsonc
// 200 → data: [ { "ticketId", "publicReference": "TKT-7F3A9C",
//   "guestName": null, "status": "Issued",         // Issued | CheckedIn | Voided (D:Q7)
//   "qrImageUrl": "/api/v1/tickets/TKT-7F3A9C/qr" } ] // link to the rendered QR image; no raw secret in JSON (D:Q8, Issue 3)
```
- The raw QR payload (reference + secret) is **never returned as a JSON field**. The DB stores only the SHA-256 hash of the secret (D:Q8, FR-TKT-04, NFR-SEC-05). Clients render the ticket by fetching the QR **image** below.

### GET `/api/v1/tickets/{ticketId}/qr`
**Attendee (owner) / Admin.** Returns the ticket's QR as a **rendered image** (`image/png`), generated server-side (D:Q8, Issue 3). The raw payload (public reference + 256-bit secret) is encoded **only inside the image bytes** — it never appears as a readable JSON field anywhere in the API.
- **Response:** `200 image/png` (binary). **Not** wrapped in the JSON envelope (it is a binary asset).
- Owner-only: **403** `FORBIDDEN` if the caller is neither the order owner nor an Admin; **404** `NOT_FOUND` if the ticket does not exist or its order is unpaid (no tickets yet, FR-TKT-02).
- **Cache-Control: no-store** — the image embeds the admission secret and MUST NOT be cached by shared caches.

### PUT `/api/v1/tickets/{id}/guest-name`
**Attendee (owner).** Set/clear optional guest name (FR-TKT-03). A nameless ticket stays valid.
- **Errors:** 409 `TICKET_CHECKED_IN` (cannot rename after check-in, Persona: Kareem).

### GET `/api/v1/admin/events/{eventId}/tickets`
**Admin.** List/search tickets for an event (US-ADM-TKT-01). Query params: `status` (`Issued|CheckedIn|Voided`), `search` (guest name or public reference). Paginated with `meta` (D:Q26).
- No-show is **derived** (`Issued AND event.endsAt < now`), never a stored status (D:Q7). Each row exposes `checkedInBy` / `checkedInAt` for auditing. Backed by `IX_Ticket_Event_Status`.

### POST `/api/v1/admin/events/{eventId}/check-in`
**Admin only** (D:Q9, FR-TKT-05). Scan a QR at the door — **event-scoped**.
```jsonc
// request
{ "qrPayload": "TKT-7F3A9C.<secret>" }
// 200 → data (success)
{ "result": "CheckedIn", "ticketId", "publicReference": "TKT-7F3A9C",
  "guestName": "Sara", "checkedInAt": "…", "checkedInBy": "<adminId>" }
```
- **Five outcomes** (D:Q9, D:Q2 audit-Issue-2), all reject attempts **logged** (FR-TKT-06):
  - success → 200 `result: "CheckedIn"`.
  - already checked in → **409** `TICKET_ALREADY_CHECKED_IN` with `{ checkedInAt, checkedInBy }`.
  - ticket belongs to another event → **409** `WRONG_EVENT` with `{ ticketEventId }`.
  - ticket is voided (known ticket, refunded/cancelled order) → **409** `TICKET_VOIDED`.
  - unknown reference / bad secret → **404** `TICKET_INVALID` (does not reveal which).
- Look-up by indexed `publicReference`, then constant-time compare of secret vs. stored hash (D:Q8).
- Check-in records who + when (FR-TKT-06, NFR-SEC-09).

---

## 12. Admin: Paid-order Void & Refund (SRS §3.8 · US-ORD (admin) · D:Q6)

### GET `/api/v1/admin/orders`
**Admin.** List all orders across events (US-ADM-PAY-01). Query params: `eventId`, `status` (`PendingPayment|Paid|Cancelled|Expired`), `fromDate`, `toDate`, `search` (attendee name/email). Paginated with `meta` (D:Q26).
- A `Cancelled` order with a `refundEntryId` was previously Paid (voided); one without was never paid — status alone is insufficient to distinguish (DataModel §2.3 Issue 7).

### POST `/api/v1/admin/orders/{id}/void`
**Admin.** Void a **Paid** order; refund handled **offline** (FR-PAY-07).
```jsonc
// request
{ "reason": "Customer request", "rowVersion": "…" }
// 200 → data
{ "orderId", "status": "Cancelled",
  "voidedTickets": 3, "seatsReleased": 3, "checkedInTicketsRetained": 2,
  "refundEntryId": "…" }
```
- Sets voidable tickets → **Voided**, releases **only not-yet-checked-in** seats; **checked-in tickets are non-voidable** and their seats stay consumed (D:Q6).
- Records a manual refund entry (FR-PAY-07, NFR-REL-03 — no hard delete).
- **Errors:** 409 `ORDER_NOT_VOIDABLE` (not Paid).

> **Order-status identity (Issue 7).** A voided-paid order and a user-cancelled unpaid order **both** end in status `Cancelled`; the order status enum stays `PendingPayment → Paid / Cancelled / Expired` (FR-ORD-08). The two are distinguished **not by status** but by the presence of an associated **refund entry**: a `Cancelled` order that was previously `Paid` has exactly one refund entry; a `Cancelled` order that was never paid has none. Financial reports (§19) and reconciliation MUST use the refund entry (and prior payment record), never the status alone, to separate "paid then refunded" from "never paid." The Data Model (10) MUST expose this join cleanly (e.g. `Order.RefundEntryId` nullable, or a queryable `RefundEntries` table keyed by order).


---

## 13. Tracks & Sessions (SRS §3.10 · US-TRK / US-SES)

### POST `/api/v1/admin/tracks`
**Admin.** Create a track.
```jsonc
{ "nameEn": "Public Speaking", "nameAr": "الخطابة العامة",
  "descriptionEn": "…", "descriptionAr": "…",
  "schedule": "Every Saturday 10:00–12:00" }   // nvarchar(500), nullable (DataModel §3.1)
// 201 → data: { "trackId", "nameEn", "nameAr", "descriptionEn", "descriptionAr",
//               "schedule", "isActive": true, "rowVersion" }
```
- `nameEn` unique among live (non-deleted) tracks → 409 `TRACK_NAME_TAKEN`.

### PUT `/api/v1/admin/tracks/{id}`
**Admin.** Edit nameEn/nameAr/descriptionEn/descriptionAr/schedule/isActive. Requires `rowVersion`.
- **Errors:** 409 `TRACK_NAME_TAKEN`; 409 `CONCURRENCY_CONFLICT`.

### DELETE `/api/v1/admin/tracks/{id}`
**Admin.** Soft-delete a track (D:Q14).
- **Soft-delete ripple (D:Q14):** auto-ends the track's active Member enrollments **and** Board assignment (sets `EndedAt`, retains all history), freeing those users for reassignment. Requires confirmation echo:
```jsonc
// DELETE request
{ "confirmImpact": true, "rowVersion": "…" }
// 200 → data: { "trackId", "status": "Deleted", "enrollmentsEnded": 12, "boardAssignmentsEnded": 1 }
```
- **Errors:** 409 `TRACK_NAME_TAKEN`; 428 `CONFIRMATION_REQUIRED` (impact not confirmed); 409 `CONCURRENCY_CONFLICT`.

### GET `/api/v1/tracks/{id}`
**Board@T / Admin.** Track detail: members, sessions, progress summaries (FR-TRK-04). Board limited to their own track → **403** `TRACK_FORBIDDEN` otherwise. A **Member** sees a scoped view via their dashboard (§16), not this endpoint.

### GET `/api/v1/admin/tracks`
**Admin.** List tracks (paginated); filter `includeDeleted`, `isActive`; sort whitelist: `nameEn`, `createdAt`; search by `nameEn`/`nameAr`. Each row includes member count and whether a Board is assigned (US-ADM-TRK-04, D:Q26).

### POST `/api/v1/tracks/{trackId}/sessions`
**Board@T (own track) / Admin.** Create a session.
```jsonc
{ "titleEn": "Storytelling Basics", "titleAr": "أساسيات السرد",
  "description": "…",   // single column (DataModel §3.3)
  "startsAtUtc": "2026-08-10T10:00:00Z", "endsAtUtc": "2026-08-10T12:00:00Z",
  "location": "Room A" }
// 201 → data: { "sessionId", "titleEn", "titleAr", "description",
//               "startsAtUtc", "endsAtUtc", "location", "status": "Scheduled" }
```
- Board writes restricted to their supervised track → **403** `TRACK_FORBIDDEN` (D:Q13).

### PUT `/api/v1/tracks/{trackId}/sessions/{id}`
**Board@T (own track) / Admin.** Edit titleEn/titleAr/description/startsAtUtc/endsAtUtc/location.

### DELETE `/api/v1/tracks/{trackId}/sessions/{id}`
**Board@T (own track) / Admin.** Delete a session.
- **Delete (D:Q13):** a session **with any attendance/evaluation records** cannot be hard-deleted → **409** `SESSION_HAS_RECORDS` (soft-delete/cancel only); a records-free session deletes outright.

### PATCH `/api/v1/sessions/{id}/status`
**Board@T (own track) / Admin.** Transition session status (US-BRD-SES-04, DataModel §3.3).
```jsonc
// request: { "status": "Held" }   // "Held" | "Cancelled"
```
- `Scheduled → Held` (only after `EndsAtUtc`); `Scheduled | Held → Cancelled`. No other transitions.
- A `Cancelled` session with attendance/evaluation records → **409** `SESSION_HAS_RECORDS` (soft-delete only).
- **Errors:** 403 `TRACK_FORBIDDEN`; 409 `ILLEGAL_STATUS_TRANSITION` (invalid transition or precondition not met — e.g. `Scheduled → Held` before `EndsAtUtc`).

### GET `/api/v1/tracks/{trackId}/sessions`
**Member@T / Board@T / Admin.** Upcoming & past sessions of the track (FR-TRK-03). Member limited to their own track. Paginated; filter `status` (`Scheduled|Held|Cancelled`), `when` (`upcoming|past`).
```jsonc
// 200 → data: [ { "sessionId", "titleEn", "titleAr", "description",
//   "startsAtUtc", "endsAtUtc", "location", "status": "Scheduled" } ], meta: {...}
```

---

## 14. Enrollment & Member Management (SRS §3.3 · US-TRK-03/04 · D:Q15)

### GET `/api/v1/tracks/{trackId}/enrollable-users`
**Board@T (own track) / Admin.** Search existing Attendee-role accounts eligible for enrollment into this track (US-ROLE-08, D:Q15). Returns only active accounts with global role `Attendee` that are **not** already an active Member of any track.
- **Query:** `search` (name or email, min 2 chars), `page`, `pageSize`.
```jsonc
// 200 → data: [ { "id", "firstName", "lastName", "email",
//                 "boardOfTrackId": null } ], meta: {...}
```
- `boardOfTrackId` is included so the caller can see if the candidate is already Board of another track (the sanctioned dual-role case, D:Q15). Accounts that are already Member of any track are **excluded** from results.
- **Errors:** 403 `TRACK_FORBIDDEN` (Board, not own track).

### POST `/api/v1/tracks/{trackId}/members`
**Board@T (own track) / Admin.** Enroll an **existing Attendee account** by email/id (D:Q15 — no account creation here).
```jsonc
// request: { "userEmail": "someone@example.com" }
// 201 → data: { "enrollmentId", "trackId", "userId", "status": "Active", "startedAt": "…" }
```
- Enforces the dual-role rule at enroll time (D:Q15, FR-ROLE-04):
  - 409 `ALREADY_MEMBER_ELSEWHERE` — target already an active Member of any track.
  - 409 `MEMBER_BOARD_SAME_TRACK` — would make them Member **and** Board of this same track.
  - (Allowed if they are Board of a **different** track — the sanctioned dual-role case.)
- 404 `USER_NOT_FOUND`; 403 `TRACK_FORBIDDEN` (Board, not own track).

### DELETE `/api/v1/tracks/{trackId}/members/{enrollmentId}`
**Board@T (own track) / Admin.** End an enrollment (FR-TRK-04). Sets `EndedAt`, **retains records** (D:Q11, FR-ROLE-05). Re-enrolling later creates a fresh enrollment with a clean attendance %.

---

## 15. Attendance & Evaluations (SRS §3.11–3.12 · US-ATT / US-EVL)

### PUT `/api/v1/sessions/{sessionId}/attendance`
**Board@T (own track) / Admin.** Record/update a member's attendance (FR-ATT-01/02).
```jsonc
// request: { "enrollmentId": "…", "status": "Late" }   // Present | Late | Absent
```
- **Upsert:** at most one record per (session, enrollment); re-recording updates in place (FR-ATT-02). **Late counts as attended** (D:Q12, FR-ATT-03).
- **Errors:** 403 `TRACK_FORBIDDEN`; 422 `SESSION_NOT_OCCURRED` (future session); 404 `ENROLLMENT_NOT_IN_TRACK`.

### GET `/api/v1/tracks/{trackId}/attendance`
**Board@T / Admin.** All members' attendance for the track (FR-ATT-04). Paginated with `meta` (D:Q26).
```jsonc
// 200 → data: [ { "enrollmentId", "userId", "firstName", "lastName",
//   "attendancePercentage": 0.75,
//   "records": [ { "sessionId", "sessionTitleEn", "status": "Present", "recordedAt": "…" } ] } ]
```

### GET `/api/v1/admin/attendance`
**Admin.** Cross-track attendance (FR-ATT-04, RPT). Paginated; filter `trackId`, `sessionId`, `status`.
```jsonc
// 200 → data: [ { "enrollmentId", "userId", "firstName", "lastName", "trackId", "trackNameEn",
//   "sessionId", "sessionTitleEn", "status": "Late", "recordedAt": "…" } ], meta: {...}
```

### GET `/api/v1/tracks/{trackId}/members`
**Board@T (own track) / Admin.** Paginated roster of the track's **active** members (`EndedAtUtc IS NULL`) with each member's current attendance % and latest evaluation score (US-BRD-07, BDB-02). Board limited to own track → **403** `TRACK_FORBIDDEN`. Paginated with `meta` (D:Q26).

### PUT `/api/v1/sessions/{sessionId}/evaluations`
**Board@T (own track) / Admin.** Create/edit a member's evaluation (FR-EVL-01/02).
```jsonc
// request: { "enrollmentId": "…", "score": 87, "feedback": "Strong delivery." }
```
- Score **integer 0–100 inclusive** (D:Q17) → 422 `INVALID_SCORE` otherwise. Feedback optional.
- **Upsert:** one per (session, enrollment), overwrite in place with audit columns (D:Q17); no version history.
- **Preconditions (D:Q16):** session date in the **past** → 422 `SESSION_NOT_OCCURRED`; member has an **active enrollment** → 422 `MEMBER_NOT_ENROLLED`. Attendance is **not** required first.
- 403 `TRACK_FORBIDDEN`.

### GET `/api/v1/tracks/{trackId}/evaluations`
**Board@T / Admin.** All members' evaluations for the track (FR-EVL-04). Paginated with `meta` (D:Q26).
```jsonc
// 200 → data: [ { "enrollmentId", "userId", "firstName", "lastName",
//   "evaluations": [ { "sessionId", "sessionTitleEn", "score": 87,
//                      "feedback": "Strong delivery.", "recordedAt": "…" } ] } ]
```

---

## 16. Member & Board Dashboards (SRS §3.11–3.12 · US-MEM-01..04 · US-BRD-06/07)

### GET `/api/v1/me/board-dashboard`
**Board@T.** Supervisory summary for the Board's own supervised track (US-BRD-06, US-BRD-07). 403 `FORBIDDEN` if the caller holds no active Board assignment.
```jsonc
// 200 → data
{ "trackId", "trackNameEn", "trackNameAr",
  "activeMemberCount": 18,
  "sessionCount": { "scheduled": 3, "held": 7, "cancelled": 1 },
  "attendanceAverage": 0.82,          // (Present+Late) ÷ recorded-occurred sessions, D:Q12
  "evaluationAverage": 74.5,          // average score across all evaluations in the track
  "openContactSubmissions": 0 }       // always 0 — included for future use
```

### GET `/api/v1/me/enrollment`
**Member.** The caller's active enrollment summary: track, attendance % (= (Present+Late) ÷ **recorded-occurred** sessions, D:Q12), latest evaluations, upcoming sessions (FR-MDB / MDB-01).
```jsonc
// 200 → data
{ "enrollmentId", "trackId", "trackNameEn", "trackNameAr",
  "startedAt": "2026-07-01T00:00:00Z",
  "attendancePercentage": 0.75,
  "latestEvaluations": [ { "sessionId", "sessionTitleEn", "score": 82, "feedback": "…", "recordedAt": "…" } ],
  "upcomingSessions": [ { "sessionId", "titleEn", "titleAr", "startsAtUtc", "endsAtUtc", "location" } ] }
```
- Returns **404** `NOT_FOUND` if the caller has no active enrollment.

### GET `/api/v1/me/attendance`
**Member.** Own attendance log + percentage (FR-ATT-03). Scoped to current active enrollment (D:Q11).
```jsonc
// 200 → data
{ "enrollmentId", "attendancePercentage": 0.75,
  "records": [ { "sessionId", "sessionTitleEn", "sessionTitleAr",
                 "sessionStartsAtUtc", "status": "Present",   // Present | Late | Absent
                 "recordedBy": "<boardUserId>", "recordedAt": "…" } ] }
```

### GET `/api/v1/me/evaluations`
**Member.** Own evaluation history only (FR-EVL-03). Never other members' → enforced server-side (D:Q11).
```jsonc
// 200 → data: [ { "evaluationId", "sessionId", "sessionTitleEn", "sessionTitleAr",
//   "sessionStartsAtUtc", "score": 87, "feedback": "Strong delivery.", "recordedAt": "…" } ]
```

---

## 17. Notifications (SRS §3.13 · US-NTF-01..03)

### POST `/api/v1/admin/notifications`
**Admin.** Send a notification; recipients **resolved and fanned out to per-recipient rows at send time** (D:Q21).
```jsonc
// request
{ "title": "…", "body": "…",
  "audience": { "type": "PlatformWide" } }              // PlatformWide | GlobalRole | Track
// audience variants:
//   { "type": "GlobalRole", "role": "Attendee" }        // Attendee | Admin
//   { "type": "Track", "trackId": "…" }
// 201 → data: { "notificationId", "recipientsCreated": 128 }
```
- **Errors:** 422 `NO_RECIPIENTS_RESOLVED` — the resolved audience is empty (e.g. a Track with no active members, or a GlobalRole with no accounts of that role). No notification row is created.

### POST `/api/v1/tracks/{trackId}/notifications`
**Board@T (own track).** Send to the track's **current active members** only (FR-NTF-02, D:Q21). 403 `TRACK_FORBIDDEN` otherwise.
```jsonc
// request: { "title": "…", "body": "…" }
// 201 → data: { "notificationId", "recipientsCreated": 14 }
```
- **Errors:** 403 `TRACK_FORBIDDEN`; 422 `NO_RECIPIENTS_RESOLVED` — track has no active members at send time.

### GET `/api/v1/me/notifications`
**Authenticated.** Own inbox (paginated), filter `unreadOnly`. Each row has its own read state (FR-NTF-03).
```jsonc
// 200 → data: [ { "id", "title", "body", "isRead", "createdAtUtc" } ], meta: {...}
```

### POST `/api/v1/me/notifications/{id}/read`
**Authenticated.** Mark one notification as read (FR-NTF-04). **204 No Content.** 404 `NOTIFICATION_NOT_FOUND` (own only).

### POST `/api/v1/me/notifications/read-all`
**Authenticated.** Mark all unread notifications as read (FR-NTF-04). **204 No Content.**

---

## 18. Public Pages & Contact (SRS §3.14 · US-PUB-01..03)

### POST `/api/v1/contact`
**Public (unauthenticated) — the only unauthenticated write (D:Q20).**
```jsonc
// request: { "name": "…", "email": "…", "subject": "…", "message": "…" }
```
- Validation: email format; `subject ≤ 200`, `message ≤ 2000` chars (D:Q20). **Rate-limited by IP** → 429 `RATE_LIMITED` + `Retry-After` (NFR-SEC-10). No CAPTCHA in current scope.
- Stored with status **New** → Admin reviews (D:Q20).

### GET `/api/v1/admin/contact-submissions`
**Admin.** List (paginated), filter `status` (New|Read|Archived). Sort whitelist: `createdAt:asc|desc`.
```jsonc
// 200 → data: [ { "id", "name", "email", "subject",
//   "messageExcerpt": "First 120 chars…", "status": "New",
//   "createdAtUtc": "…" } ], meta: {...}
```

### PUT `/api/v1/admin/contact-submissions/{id}`
**Admin.** Update status (Read/Archived). No in-app reply (D:Q20).
```jsonc
// request: { "status": "Read" }   // Read | Archived
// 200 → data: { "id", "status": "Read" }
```

> Home/About/Team/Partners content is **static** in current scope (FR-PUB-03) — no admin-editing endpoints, no dedicated tables.

---

## 18b. Admin Dashboard Overview (PRD ADM-01 · US-ADM-DASH-01 · Issue 8)

### GET `/api/v1/admin/dashboard`
**Admin.** Summary cards for the admin landing page — a single round-trip aggregating counts already derivable from the per-domain endpoints (PRD ADM-01).
```jsonc
// 200 → data
{ "totalUsers": 512, "activeUsers": 498,
  "totalEvents": 12, "publishedEvents": 3,
  "ticketsSold": 1840, "checkedInToday": 0,
  "openTracks": 4,
  "newContactSubmissions": 7 }
```
- Read-only aggregate; each figure reflects committed data (not cached for money decisions, NFR-PERF-05). Fields are additive — clients tolerate new keys.

---

## 19. Reports (SRS §RPT · US-RPT · D:Q28c)

### GET `/api/v1/admin/reports/events/{eventId}`
**Admin.** Registration counts, seats sold/held/remaining, attendance (check-in) rate (RPT-01).
```jsonc
// 200 → data
{ "eventId", "titleEn", "titleAr", "startsAtUtc", "endsAtUtc",
  "capacity": 200, "seatsSold": 142, "seatsHeld": 8, "seatsRemaining": 50,
  "checkInRate": 0.71,   // checkedIn ÷ seatsSold
  "checkedIn": 101, "noShow": 41, "voided": 3 }
```

### GET `/api/v1/admin/reports/tracks/{trackId}`
**Admin.** Member progress, attendance averages, evaluation averages (RPT-02).
```jsonc
// 200 → data
{ "trackId", "nameEn", "nameAr",
  "activeMemberCount": 18, "sessionsHeld": 7,
  "attendanceAverage": 0.82,
  "evaluationAverage": 74.5,
  "members": [ { "userId", "firstName", "lastName",
                 "attendancePercentage": 0.86, "evaluationAverage": 78.0 } ] }
```

### GET `/api/v1/admin/reports/financial`
**Admin.** Revenue per event, payment summaries (RPT-03); filter `eventId`, `fromDate`, `toDate`.
```jsonc
// 200 → data
{ "totalRevenue": { "amount": 284000.00, "currency": "EGP" },
  "totalRefunded": { "amount": 3200.00, "currency": "EGP" },
  "netRevenue": { "amount": 280800.00, "currency": "EGP" },
  "byEvent": [ { "eventId", "titleEn",
                 "revenue": { "amount": 142000.00, "currency": "EGP" },
                 "refunded": { "amount": 1600.00, "currency": "EGP" },
                 "paidOrders": 71, "voidedOrders": 4,
                 "byUnitType": [ { "unitType": "Individual", "orders": 50,
                                   "revenue": { "amount": 100000.00, "currency": "EGP" } },
                                 { "unitType": "Package", "orders": 21,
                                   "revenue": { "amount": 42000.00, "currency": "EGP" } } ] } ] }
```
- Revenue is based on `PaidAtUtc`-stamped orders only; refunds are based on refund entries (DataModel §2.6). `Cancelled` orders without a refund entry (never paid) are excluded.

- **Export (D:Q28c, RPT-04):** all three accept `?format=csv|pdf`. Default JSON envelope; `csv`/`pdf` return the file with the appropriate `Content-Type` and `Content-Disposition` (not the JSON envelope).

---

## 20. Out of scope for this contract (D:Q28c)

Mobile app, SignalR real-time push, automated gateway refunds (PAY-01), additional payment channels (PAY-02), financial reconciliation (PAY-03), analytics beyond RPT-01..03, and email/SMS beyond the password-reset and email-confirmation messages. These are acknowledged future work, intentionally excluded from these endpoints.
