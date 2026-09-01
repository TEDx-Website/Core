# TEDxAlkawmia — Acceptance Criteria

> **Version:** 1.4
> **Date:** 2026-08-05
> **Reads from:** [01 — PRD](./01-PRD.md) · [02 — SRS](./02-SRS.md) · [03 — User Flows](./03-UserFlows.md) · [05 — User Stories](./05-UserStories.md)
> **Decisions:** grilling sessions 2026-07-20 to 2026-07-24 — **Q1–Q56** (requirements Q1–Q28 + architecture Q29–Q55 + Q56), cited as **(D:Qn)**.
>
> **v1.4 (2026-08-05) — OPEN-S2 rulings.** **OPEN-S2-2:** AC-EVT-05's unpublish-with-orders scenario now asserts **`HAS_ORDERS_CANNOT_UNPUBLISH`** instead of `EVENT_HAS_ORDERS`, which it shared with AC-EVT-07's soft-delete block in violation of one-code-↔-one-status (audit-Issue-10); AC-EVT-07 is unchanged and keeps `EVENT_HAS_ORDERS`. **OPEN-S2-1:** a new AC-EVT-05 scenario pins that `Cancelled` is **rejected 422** at `POST /{id}/status` and reachable only via `/cancel`. No behaviour is added — both changes make the document agree with [07 §6](./07-ApiContract.md), which was already correct.
>
> **v1.3 (2026-08-03):** AC-AUTH-04 logout aligns with the API contract v1.4 change log: response is **204 No Content** (not 200), and the scenario is now testable against the implemented contract. Refresh remains 401 TOKEN_REUSED / 401 TOKEN_INVALID per AC-AUTH-03.
>
> **v1.2 (2026-07-24):** AC-EVT-05 transition table gains **Archived→Cancelled (D:Q56)** and an explicit "Draft→Cancelled rejected (use soft-delete)" row; a new scenario covers Archived-with-sold-tickets cancellation; AC-EVT-06 widens to "Published or Archived" (identical ripple). Provenance refreshed to Q1–Q56.
>
> **v1.1 (2026-07-21):** Model-B ticketing (individual tickets at event face price; packages optional — DecisionLog Q1 addendum): new scenarios for individual-ticket quote/reserve, negative ticket-price rejection, and zero-package publish/sell. Error-taxonomy alignment with the API contract: change-password wrong-current → `400 CURRENT_PASSWORD_INCORRECT`; refresh-token codes → `TOKEN_REUSED`/`TOKEN_INVALID`; bad upload → `422 VALIDATION_ERROR (INVALID_FILE_TYPE|FILE_TOO_LARGE)`; check-in outcomes carry explicit HTTP statuses. Reinstated cross-cutting ACs for log hygiene and i18n/RTL readiness.

---

## How to read this document

Each block gives the **Gherkin-style** acceptance criteria for one or more user stories. Most AC IDs share the story's suffix (`US-AUTH-01` → `AC-AUTH-01`), but some blocks consolidate several stories or use a shortened token (e.g. `AC-PROMO-*` covers `US-ADM-PRM-*`, `AC-EVT-03..08` cover `US-ADM-EVT-01..06`). **The authoritative story→AC mapping is the `Covers:` line at the top of each block** — build the traceability matrix from it, not from the ID alone. Scenarios are written to be **testable** — one observable behaviour each — and cover the happy path plus the alternate/error paths named in the SRS and User Flows.

- **Given/When/Then** is the canonical form; `And`/`But` extend a step.
- **`Covers:`** names the exact User Story ID(s) each block satisfies.
- Error scenarios name the machine `error.code` from the API envelope (D:Q25) where relevant.
- HTTP status codes follow the contract in the API document.

---

## AC-AUTH-01 — Register an account
> **Covers:** US-AUTH-01

```gherkin
Scenario: Successful registration creates an Attendee
  Given no account exists for "nour@example.com"
  When a Visitor submits first name "Nour", last name "Adel", email "nour@example.com",
       password "Passw0rd!", and matching confirm-password
  Then an account is created with the global role "Attendee"
  And the account has no track assignments
  And the account's email is unconfirmed (AC-AUTH-08)
  And the response is 201 with the created account summary (no password echoed)

Scenario: Registration rejected for a duplicate email
  Given an account already exists for "nour@example.com"
  When a Visitor submits registration with email "nour@example.com"
  Then the response is 409 with error.code "EMAIL_TAKEN"
  And no new account is created

Scenario Outline: Registration rejected for a weak password
  When a Visitor submits password "<password>"
  Then the response is 422 with error.code "VALIDATION_ERROR"
  And fieldErrors.password names the unmet rule
  Examples:
    | password  |
    | short1A   |   # < 8 chars
    | password1 |   # no uppercase
    | PASSWORD1 |   # no lowercase
    | Password  |   # no digit

Scenario: Registration rejected when passwords do not match
  When a Visitor submits password "Passw0rd!" and confirm-password "Passw0rd?"
  Then the response is 422 with fieldErrors.confirmPassword "Passwords do not match"
```

## AC-AUTH-02 — Log in
> **Covers:** US-AUTH-02

```gherkin
Scenario: Successful login issues a token pair
  Given an active account exists for "nour@example.com" with password "Passw0rd!"
  And her email is confirmed
  When she logs in with the correct email and password
  Then the response is 200 with an access token and a refresh token
  And the access token contains claims: account id, email, global role
  And the access token contains no per-track (Member/Board) claim
  And the access token expires in 15 minutes (default) (D:Q24)

Scenario: Login fails with a generic message for a wrong password
  Given an active account exists for "nour@example.com"
  When she logs in with the wrong password
  Then the response is 401 with error.code "INVALID_CREDENTIALS"
  And the message does not reveal whether the email exists

Scenario: Login fails identically for an unknown email
  Given no account exists for "ghost@example.com"
  When someone logs in with "ghost@example.com"
  Then the response is 401 with error.code "INVALID_CREDENTIALS"
  And the message is identical to the wrong-password case

Scenario: Login rejected for a deactivated account
  Given an account for "nour@example.com" is deactivated
  When she logs in with correct credentials
  Then the response is 403 with error.code "ACCOUNT_DEACTIVATED"
  And a distinct message advises contacting an organizer

Scenario: Login rejected for an unconfirmed email
  Given an active account for "nour@example.com" whose email is unconfirmed
  When she logs in with correct credentials
  Then the response is 403 with error.code "EMAIL_NOT_CONFIRMED"
  And the full gate order is: credentials, then deactivation, then confirmation
       (so a deactivated-and-unconfirmed account reports ACCOUNT_DEACTIVATED)

Scenario: Repeated failed logins trigger account lockout (ASP.NET Core Identity)
  Given an active account
  When the account exceeds the configured failed-attempt threshold
  Then further login attempts are rejected until the lockout window expires
  And the lockout is recorded in AccessFailedCount / LockoutEnd (DataModel §1.1)
```

## AC-AUTH-03 — Refresh the session
> **Covers:** US-AUTH-03

```gherkin
Scenario: Valid refresh token is exchanged and rotated
  Given a valid, unexpired, unrevoked refresh token
  When the client exchanges it
  Then the response is 200 with a new access token and a new refresh token
  And the old refresh token is revoked (single-use)

Scenario: Reusing a consumed refresh token revokes the family
  Given a refresh token that has already been exchanged once
  When the client presents the old (consumed) token again
  Then the response is 401 with error.code "TOKEN_REUSED"
  And every refresh token in that family is revoked
  And the user must log in again

Scenario: Expired refresh token is rejected
  Given a refresh token past its 7-day lifetime
  When the client presents it
  Then the response is 401 with error.code "TOKEN_INVALID"

Scenario: Refresh tokens are stored hashed
  Given any issued refresh token
  Then the database stores only its hash, never the raw token
```

## AC-AUTH-04 — Log out
> **Covers:** US-AUTH-04

```gherkin
Scenario: Logout revokes the presented refresh token
  Given a logged-in user with a valid refresh token
  When she logs out
  Then the response is 204 No Content
  And that refresh token can no longer be exchanged
  And a subsequent exchange returns 401 error.code "TOKEN_INVALID"
```

## AC-AUTH-05 — Request a password reset
> **Covers:** US-AUTH-05

```gherkin
Scenario: Reset requested for an existing account sends an email
  Given an account exists for "nour@example.com"
  When she requests a password reset for that email
  Then the response is 200 with a neutral message
  And a single-use reset token (default 1-hour lifetime) is emailed (D:Q24)

Scenario: Reset requested for an unknown email responds identically
  Given no account exists for "ghost@example.com"
  When someone requests a reset for "ghost@example.com"
  Then the response is 200 with a message identical to the existing-account case
  And no email is sent
  And no information reveals whether the account exists
```

## AC-AUTH-06 — Set a new password via reset link
> **Covers:** US-AUTH-06

```gherkin
Scenario: Valid reset token sets a new password
  Given a valid, unexpired, unused reset token
  When the user submits a new policy-compliant password twice
  Then the password is updated
  And the response is 200
  And existing refresh tokens for the account are revoked

Scenario: Used or expired reset token is rejected
  Given a reset token that is expired or already used
  When the user submits a new password
  Then the response is 400 with error.code "RESET_TOKEN_INVALID"
  And the message invites requesting a new link

Scenario: New password must meet the policy
  Given a valid reset token
  When the user submits a weak new password
  Then the response is 422 with fieldErrors.password naming the unmet rule
```

## AC-AUTH-07 — Server-side authorization (cross-cutting)
> **Covers:** US-AUTH-07

```gherkin
Scenario: Protected endpoint rejects an unauthenticated caller
  Given no access token is presented
  When a protected endpoint is called
  Then the response is 401 with error.code "UNAUTHENTICATED"

Scenario: Global-role gate blocks an under-privileged caller
  Given an Attendee (no Admin role)
  When they call an Admin-only endpoint
  Then the response is 403 with error.code "FORBIDDEN"

Scenario: Board cannot act on a track they do not supervise
  Given Yousef is Board of Track Y and Member of Track X
  When he attempts a Board action on Track X
  Then the response is 403 with error.code "FORBIDDEN"
  And the decision is made server-side from his per-request track assignments

Scenario: Per-track authority is not taken from the token
  Given an access token carrying only the global role
  When any per-track action is authorized
  Then the server resolves the caller's assignments from current data, not the token
```

## AC-AUTH-08 — Confirm email address
> **Covers:** US-AUTH-08

```gherkin
Scenario: Registration creates an unconfirmed account and emails a link
  Given a Visitor registers with "nour@example.com"
  Then the account is created with the email unconfirmed
  And a confirmation token valid for 24 hours is emailed (D:Q57)
  And the response is 201 with emailConfirmationRequired true
  And the response does not include tokens (no auto-login)

Scenario: Registration succeeds even if the mail provider fails
  Given the mail provider is unavailable
  When a Visitor registers
  Then the account is still created
  And the response is still 201
  And the failure is logged server-side
  And the user can obtain a link later via resend

Scenario: Valid confirmation token confirms the address
  Given a valid, unexpired, unused confirmation token for "nour@example.com"
  When it is submitted
  Then the email is marked confirmed
  And the response is 200
  And she can now log in

Scenario: Confirming twice is idempotent, not an error
  Given a confirmation token that was already used successfully
  When it is submitted again while still unexpired
  Then the response is 200 with emailConfirmed true
  And no error is raised
  # Identity does not rotate the SecurityStamp on confirm, so the token stays
  # valid for its 24 hours; mail prefetchers routinely fire the link twice

Scenario: An older link still works after a resend
  Given a pending account that requested a second confirmation email
  When the user clicks the link from the FIRST email, still unexpired
  Then the response is 200 and the email is confirmed
  # Resend adds a valid token; it does not revoke earlier ones

Scenario: Expired or forged confirmation token is rejected
  Given a confirmation token that is expired, tampered with, or not issued by us
  When it is submitted
  Then the response is 400 with error.code "CONFIRM_TOKEN_INVALID"
  And the message invites requesting a new link

Scenario: Login is refused while the email is unconfirmed
  Given an account whose email is not yet confirmed
  When she logs in with the correct password
  Then the response is 403 with error.code "EMAIL_NOT_CONFIRMED"
  And no tokens are issued
  And the client can offer to resend the confirmation email

Scenario: An unconfirmed address is not disclosed by a wrong password
  Given an account whose email is not yet confirmed
  When someone submits the wrong password for it
  Then the response is 401 with error.code "INVALID_CREDENTIALS"
  And the response is identical to that for an unknown email
  And nothing reveals that the account exists or is unconfirmed

Scenario: Resend responds identically for every address state
  Given three addresses: one pending confirmation, one already confirmed,
        and one with no account at all
  When a resend is requested for each
  Then all three responses are 200 with an identical neutral body
  And an email is sent only for the pending one

Scenario: Resend is rate limited
  Given repeated resend requests from one client beyond the configured limit
  Then the response is 429 with error.code "RATE_LIMITED"
  And a Retry-After header in seconds

Scenario: Pre-existing accounts are not locked out
  Given accounts created before this feature shipped, including the seeded Admin
  When the confirmation migration runs
  Then their emails are marked confirmed
  And they can log in without confirming (FR-AUTH-16)
```

---

## AC-USER-01 — View own profile
> **Covers:** US-USER-01

```gherkin
Scenario: Authenticated user views their profile
  Given a logged-in user
  When they request their profile
  Then the response includes name, email, phone, bio, profile picture URL,
       global role, and current track assignments (Member@T and/or Board@T)
```

## AC-USER-02 — Edit own profile
> **Covers:** US-USER-02

```gherkin
Scenario: User updates editable fields
  Given a logged-in user
  When they update first name, last name, phone, and bio
  Then the changes are saved and returned

Scenario: Email cannot be changed
  Given a logged-in user
  When they attempt to change their email
  Then the field is rejected (immutable after registration)

Scenario: Users cannot self-assign roles
  Given a logged-in Attendee
  When they attempt to set their global role or a track assignment via profile edit
  Then the attempt is rejected (those are Admin/Board actions)
```

## AC-USER-03 — Upload profile picture
> **Covers:** US-USER-03

```gherkin
Scenario: Valid image is stored in Cloudinary
  Given a logged-in user
  When they upload a JPEG/PNG within the size limit
  Then the file is stored in Cloudinary
  And only the returned URL is persisted on the account

Scenario: Non-image or oversized upload is rejected
  Given a logged-in user
  When they upload a non-image file or one over the size limit
  Then the response is 422 with error.code "VALIDATION_ERROR" (INVALID_FILE_TYPE or FILE_TOO_LARGE)
  And no URL is persisted
```

## AC-USER-04 — Change password (authenticated)
> **Covers:** US-USER-04

```gherkin
Scenario: Correct current password allows a change
  Given a logged-in user
  When they supply the correct current password and a policy-compliant new one
  Then the password is updated and the response is 200
  And all existing refresh tokens for the account are revoked (D:Q24, NFR-SEC-02)

Scenario: Wrong current password blocks the change
  Given a logged-in user
  When they supply an incorrect current password
  Then the response is 400 with error.code "CURRENT_PASSWORD_INCORRECT"
  And the password is unchanged
```

---

## AC-MNG-01 — Admin lists users
> **Covers:** US-MNG-01

```gherkin
Scenario: Paginated, filtered user list
  Given an Admin
  When they list users with page=1, pageSize=20, filter role=Attendee, status=active,
       and search "nour"
  Then the response data is the matching page
  And meta contains page, pageSize, totalItems, totalPages (D:Q26)

Scenario: Non-admin cannot list users
  Given an Attendee
  When they call the user-list endpoint
  Then the response is 403 error.code "FORBIDDEN"
```

## AC-MNG-02 — Admin views a user
> **Covers:** US-MNG-02

```gherkin
Scenario: Admin sees full user detail
  Given an Admin
  When they open a user by id
  Then the response shows global role, active Member/Board assignments, and status
```

## AC-MNG-03 — Admin deactivates / reactivates a user
> **Covers:** US-MNG-03

```gherkin
Scenario: Deactivation blocks access but retains history (D:Q10)
  Given an active user with historical orders, tickets, and a Member enrollment
  When an Admin deactivates the account
  Then the account can no longer log in or refresh
  And existing refresh tokens are revoked (D:Q24)
  And the account and all historical records are retained (soft action)

Scenario: Deactivation keeps issued tickets valid (D:Q10)
  Given a user holding Issued tickets for a future event
  When the account is deactivated
  Then those tickets remain valid and admittable at the door

Scenario: Deactivation cancels an active pending order and releases seats (D:Q10)
  Given the user has a PendingPayment order holding seats
  When the account is deactivated
  Then that order is cancelled
  And its held seats are released to availability
  And any claimed promo slot on that order is released (D:Q19)

Scenario: Deactivating a Board ends the assignment and flags the track (D:Q10, D:Q6-audit-Issue6)
  Given the user is Board of Track Y
  When the account is deactivated
  Then the Board assignment is ended (EndedAt set), freeing the Board slot
  And Track Y is flagged as needing a new supervisor for the Admin
  And the assignment history (attendance/evaluation records) is retained, not deleted

Scenario: Deactivation ends track assignments and frees the dual-role slots (D:Q10, Issue 6)
  Given the user has an active Member enrollment and/or Board assignment
  When the account is deactivated
  Then each assignment is ended (EndedAt set), not left active/dormant
  And the freed slots may immediately be taken by reassigning another user

Scenario: Reactivation restores login only
  Given a deactivated account
  When an Admin reactivates it
  Then the user can log in again
  And previously ended assignments are NOT restored (must be reassigned explicitly)
```

## AC-MNG-04 — Deactivation impact preview
> **Covers:** US-MNG-04

```gherkin
Scenario: Admin sees impact before confirming (D:Q10)
  Given an Admin about to deactivate a user
  When they open the confirmation
  Then it states the count of active orders to be cancelled,
       that issued tickets remain valid,
       the assignments to be ended (freeing their dual-role slots),
       and whether a supervised track will be left without a Board
```

---

## AC-ROLE-01 — Admin changes a global role
> **Covers:** US-ROLE-01

```gherkin
Scenario: Admin promotes/demotes global role
  Given an Admin and a target user
  When the Admin sets the target's global role to Admin (or back to Attendee)
  Then the change is saved and audited (who + when) (NFR-SEC-09)

Scenario: Non-admin cannot change global roles
  Given a non-admin caller
  When they attempt to change any global role
  Then the response is 403 error.code "FORBIDDEN"
```

## AC-ROLE-02 — Admin assigns / removes the Board role
> **Covers:** US-ROLE-02

```gherkin
Scenario: Admin assigns Board of a track
  Given an Admin, a target user, and Track Y
  When the Admin assigns the user as Board of Track Y
  Then the user holds an active Board assignment for Track Y
  And the action is audited

Scenario: Board role is Admin-only
  Given a Board (not Admin)
  When they attempt to assign the Board role to anyone
  Then the response is 403 error.code "FORBIDDEN"
```

## AC-ROLE-03 — Board / Admin enrolls a Member (D:Q15)
> **Covers:** US-ROLE-03

```gherkin
Scenario: Board enrolls an existing Attendee in their own track
  Given Yousef is Board of Track Y
  And an existing Attendee account "salma@example.com" with no Member enrollment
  When Yousef enrolls her (found by email) in Track Y
  Then she holds an active Member enrollment for Track Y

Scenario: Enrolling a non-existent account is rejected (D:Q15)
  Given no account exists for "unknown@example.com"
  When a Board attempts to enroll that email
  Then the response is 404 error.code "USER_NOT_FOUND"
  And no account is created by the enrollment

Scenario: Target already a Member elsewhere is rejected (D:Q15, FR-ROLE-04)
  Given the target already has an active Member enrollment in Track Z
  When a Board attempts to enroll them in Track Y
  Then the response is 409 error.code "ALREADY_MEMBER_ELSEWHERE"

Scenario: Member-and-Board of the same track is rejected (D:Q15, FR-ROLE-04)
  Given the target is Board of Track Y
  When enrolling them as Member of Track Y
  Then the response is 409 error.code "MEMBER_BOARD_SAME_TRACK"

Scenario: Board of a different track may be enrolled as Member (sanctioned dual-role)
  Given the target is Board of Track Y
  When enrolling them as Member of Track X
  Then the enrollment succeeds

Scenario: Board cannot enroll into a track they do not supervise
  Given Yousef is Board of Track Y only
  When he attempts to enroll a Member into Track X
  Then the response is 403 error.code "FORBIDDEN"
```

## AC-ROLE-04 — Remove a Member from a track
> **Covers:** US-ROLE-04

```gherkin
Scenario: Board removes a Member from their own track (D:Q11)
  Given Yousef is Board of Track Y and Salma is an active Member of Track Y
  When Yousef removes her
  Then her enrollment is ended (EndedAt set), not deleted
  And her attendance and evaluation records for that enrollment are retained

Scenario: Removal frees the user for a new enrollment (D:Q11)
  Given Salma's Track Y enrollment has been ended
  When an Admin or Board enrolls her in Track X
  Then a new enrollment is created with a fresh attendance percentage
  And her ended Track Y records remain queryable but excluded from the new percentage
```

## AC-ROLE-05 — Dual-role invariant enforced (FR-ROLE-04)
> **Covers:** US-ROLE-05, US-ROLE-07

```gherkin
Scenario: At most one active Member and one active Board assignment
  Given a user
  Then the system permits at most one active Member enrollment
  And at most one active Board assignment
  And the two must be different tracks
  And violations are rejected at assignment time and blocked by a database constraint

Scenario: History retained when an assignment ends (FR-ROLE-05)
  Given an assignment with attendance/evaluation history
  When the assignment is ended
  Then all historical records tied to it are retained
```

## AC-ROLE-06 / AC-ROLE-07 — Track soft-delete side effects (D:Q14)
> **Covers:** US-ROLE-06, US-ADM-TRK-02

```gherkin
Scenario: Soft-deleting a track ends its active assignments (D:Q14)
  Given Track Y has active Member enrollments and one Board assignment
  When an Admin soft-deletes Track Y after confirming the stated impact
  Then all active enrollments and the Board assignment are ended (EndedAt set)
  And all history (sessions, attendance, evaluations) is retained
  And the freed users may be assigned to other tracks

Scenario: Impact is shown before soft-delete (D:Q14)
  Given an Admin about to soft-delete a track
  When they open the confirmation
  Then it states the number of enrollments and Board assignments that will end
```


---

## AC-EVT-01 — Public browses published events
> **Covers:** US-EVT-01

```gherkin
Scenario: Anyone lists published upcoming events
  Given events exist in Draft, Published, Archived, and Cancelled states
  When a Visitor lists events filtered to upcoming
  Then only Published events with a date in the future are returned (D:Q23)
  And Draft, Archived, and Cancelled events never appear
  And the response is paginated with a meta block (D:Q26)

Scenario: Past filter is date-derived among Published events (D:Q23)
  When a Visitor lists events filtered to past
  Then only Published events with a date before now are returned
  And Archived events (manually hidden) are excluded
```

## AC-EVT-02 — Public views event detail
> **Covers:** US-EVT-02

```gherkin
Scenario: Event detail shows the individual-ticket price, packages, and remaining seats
  Given a Published event with defined packages
  When any user views its detail
  Then the response includes title, description, startsAtUtc + endsAtUtc, location,
       the always-present individual-ticket price (ticketPrice, Model B),
       active packages with prices, and remaining seats
  And remaining seats = Capacity − seats held by active orders (D:Q3, FR-EVT-07)

Scenario: Non-published event detail is not publicly visible
  Given an event in Draft, Archived, or Cancelled state
  When a Visitor requests its detail
  Then the response is 404 (not disclosed)
```

## AC-EVT-PUB-01 — Visitor "Login to book" gate
> **Covers:** US-EVT-03

```gherkin
Scenario: Visitor sees the event and a login-to-book prompt
  Given a Visitor (unauthenticated) viewing a Published event's detail
  Then the booking action is presented as a "Login to book" prompt
  And no reserve/hold action is available to the Visitor

Scenario: Unauthenticated reserve attempt is rejected
  Given a Visitor (no access token)
  When they call the reserve endpoint directly
  Then the response is 401 error.code "UNAUTHENTICATED"
  And no seats are held
```

## AC-EVT-AVAIL-01 — Live remaining-seat availability
> **Covers:** US-EVT-04

```gherkin
Scenario: Availability reflects a lapsed hold immediately (D:Q3)
  Given an event whose only held seats belong to a PendingPayment order
  And that order's HoldExpiresAt is now in the past
  When an Attendee views remaining seats
  Then the lapsed hold's seats are counted as available
  And this does not depend on the background sweeper having run

Scenario: Availability is read from committed data, never cache (NFR-PERF-05)
  When remaining seats are computed for a booking decision
  Then the value is read from committed data
  And is not served from a cached listing
```

## AC-EVT-03 — Admin creates an event
> **Covers:** US-ADM-EVT-01

```gherkin
Scenario: Admin creates a valid event
  Given an Admin
  When they create an event with title, description, date/time (UTC), location,
       capacity 200, individual-ticket price 200.00 EGP, and an optional image
  Then the event is created in Draft state
  And capacity must be greater than zero
  And the individual-ticket price must be >= 0

Scenario: Zero or negative capacity is rejected
  When an Admin creates an event with capacity 0
  Then the response is 422 with fieldErrors.capacity

Scenario: Negative ticket price is rejected (Model B)
  When an Admin creates an event with ticketPrice -1.00
  Then the response is 422 with error.code "VALIDATION_ERROR" (fieldErrors.ticketPrice)

Scenario: Optional individual quantity cap is stored (Model B, mirrors D:Q2)
  Given an Admin creating an event
  When they set maxIndividualQtyPerOrder to 6 (or leave it null)
  Then the value is stored (null means no per-order cap on individual tickets)
```

## AC-EVT-04 — Admin edits an event (D:Q22)
> **Covers:** US-ADM-EVT-02

```gherkin
Scenario: Capacity may be raised anytime
  Given a Published event with 150 held+paid seats and capacity 200
  When an Admin raises capacity to 300
  Then the change is accepted

Scenario: Capacity cannot drop below held+paid seats (D:Q22)
  Given a Published event with 150 held+paid seats
  When an Admin sets capacity to 120
  Then the response is 409 error.code "CAPACITY_BELOW_SOLD"
  And the capacity is unchanged

Scenario: Concurrent edits are guarded by optimistic concurrency
  Given two Admins load the same event
  When both submit edits and the second uses a stale rowversion
  Then the second submission returns 409 error.code "CONCURRENCY_CONFLICT" (NFR-REL-06)

Scenario: Editing date/location is allowed and audited (D:Q22)
  Given a Published event with sold tickets
  When an Admin changes its date or location
  Then the change is accepted and audited (who + when)
```

## AC-EVT-05 — Event status transitions (D:Q23)
> **Covers:** US-ADM-EVT-03

```gherkin
Scenario: Publish a draft
  Given an event in Draft
  When an Admin publishes it
  Then its status becomes Published and it appears in public listings

Scenario: Publishing has no package precondition (Model B)
  Given a Draft event with zero packages and a valid individual-ticket price
  When an Admin publishes it
  Then it is published successfully (individual tickets are sellable without any package)
  And no "add a package first" error is returned

Scenario Outline: Legal transitions
  Given an event in "<from>"
  When an Admin transitions it to "<to>"
  Then the transition "<result>"
  Examples:
    | from      | to        | result     |
    | Draft     | Published | is allowed |
    | Published | Archived  | is allowed |
    | Published | Cancelled | is allowed |
    | Archived  | Published | is allowed |
    | Archived  | Cancelled | is allowed (D:Q56) |
    | Draft     | Cancelled | is rejected (use soft-delete, D:Q22/Q56) |
    | Cancelled | Published | is rejected (terminal) |
    | Cancelled | Draft     | is rejected (terminal) |

Scenario: Archived event is cancelled directly (D:Q56)
  Given an Archived event with Paid orders (Issued tickets) and PendingPayment holds
  When an Admin cancels it
  Then it becomes Cancelled with the same void/refund/release ripple as Published → Cancelled
  And it is NOT re-published first (a hidden event is never re-exposed just to cancel it)

Scenario: Unpublish to Draft only while zero orders (D:Q23)
  Given a Published event with no orders
  When an Admin moves it back to Draft
  Then it is allowed

Scenario: Unpublish blocked once orders exist (D:Q23)
  Given a Published event with at least one order
  When an Admin attempts to move it to Draft
  Then the response is 409 error.code "HAS_ORDERS_CANNOT_UNPUBLISH"
  And the Admin is directed to Cancel instead

Scenario: Cancelled is not reachable through the status endpoint (OPEN-S2-1)
  Given a Published event
  When an Admin posts status "Cancelled" to the status endpoint
  Then the response is 422 error.code "VALIDATION_ERROR"
  And the Admin is directed to the dedicated cancel endpoint
```

> **Error-code split (OPEN-S2-2).** `HAS_ORDERS_CANNOT_UNPUBLISH` belongs to the **unpublish** block above; `EVENT_HAS_ORDERS` belongs to the **soft-delete** block in AC-EVT-07. Both codes exist in `Errors_Ticketing`, and the one-code-↔-one-status rule (audit-Issue-10) requires each to name exactly one situation. This scenario previously asserted `EVENT_HAS_ORDERS`, contradicting [07 §6](./07-ApiContract.md).

## AC-EVT-06 — Cancel an event (D:Q22)
> **Covers:** US-ADM-EVT-04

```gherkin
Scenario: Cancelling an event with sold tickets voids and refunds offline (D:Q22, D:Q56)
  Given a Published or Archived event with Paid orders (Issued tickets) and PendingPayment holds
  When an Admin cancels the event
  Then all Issued tickets for the event become Voided
  And all PendingPayment holds are released
  And a refund entry is recorded for each Paid order (money handled offline, FR-PAY-07)
  And the event is hidden from public listings but retained
  And the ripple is identical whether the event was Published or Archived (D:Q56)

Scenario: Checked-in tickets are not un-checked by cancellation
  Given an event with some already CheckedIn tickets
  When the event is cancelled
  Then CheckedIn tickets remain CheckedIn (historical fact preserved)
```

## AC-EVT-07 — Soft-delete an event (D:Q22)
> **Covers:** US-ADM-EVT-05

```gherkin
Scenario: Soft-delete allowed only with zero orders (D:Q22)
  Given an event with no orders
  When an Admin soft-deletes it
  Then it is removed from all listings and retained

Scenario: Soft-delete blocked when orders exist (D:Q22)
  Given an event with at least one order
  When an Admin attempts to soft-delete it
  Then the response is 409 error.code "EVENT_HAS_ORDERS"
  And the Admin is directed to Cancel instead
```

## AC-EVT-08 — Admin views orders/attendees for an event
> **Covers:** US-ADM-EVT-06

```gherkin
Scenario: Admin lists all orders and attendees for an event
  Given an Admin and an event with orders
  When they view the event's orders
  Then all orders (every status) and their tickets/attendees are listed, paginated
```

---

## AC-PKG-01 — Admin defines packages
> **Covers:** US-ADM-PKG-01

```gherkin
Scenario: Admin creates a package
  Given an Admin and an event
  When they create package "Group-5" with seatsPerPackage 5 and price 750.00 EGP
  Then the package is created and available on the event

Scenario Outline: Package validation
  When an Admin creates a package with seatsPerPackage "<seats>" and price "<price>"
  Then the result is "<result>"
  Examples:
    | seats | price  | result                         |
    | 1     | 100.00 | created                        |
    | 5     | 0.00   | created (free package allowed) |
    | 0     | 100.00 | 422 fieldErrors.seatsPerPackage |
    | 2     | -1.00  | 422 fieldErrors.price          |

Scenario: Max quantity per order is configurable and optional (D:Q2)
  Given an Admin creating a package
  When they set MaxQuantityPerOrder to 4 (or leave it null)
  Then the value is stored (null means no per-order cap)

Scenario: Packages are optional — a zero-package event still sells individual tickets (Model B)
  Given a Published event with an individual-ticket price and zero packages
  When an Attendee views and books it
  Then they can buy individual tickets at the event face price
  And the absence of packages does not block viewing, publishing, or booking
```

## AC-PKG-02 — Admin activates / deactivates / soft-deletes a package
> **Covers:** US-ADM-PKG-02

```gherkin
Scenario: Deactivated package is hidden from buyers
  Given an active package on a Published event
  When an Admin deactivates it
  Then it no longer appears in the public package list
  And existing orders that referenced it are unaffected (snapshot, D:Q4)

Scenario: A referenced package cannot be hard-deleted
  Given a package referenced by at least one order
  When an Admin attempts to remove it
  Then only soft-delete is permitted; hard delete is rejected
```

## AC-PKG-03 — Public views active packages
> **Covers:** US-PKG-03

```gherkin
Scenario: Only active packages of a published event are listed
  Given a Published event with active and deactivated packages
  When any user views its packages
  Then only active, non-deleted packages and their prices are returned
```

---

## AC-PROMO-01 — Admin creates a promo code
> **Covers:** US-ADM-PRM-01

```gherkin
Scenario: Percentage promo
  Given an Admin
  When they create promo "TEDX15" as 15% off
  Then the promo is created

Scenario: Fixed-amount promo
  Given an Admin
  When they create promo "SAVE50" as 50.00 EGP off
  Then the promo is created

Scenario: Promo code uniqueness among live codes (FR-PROMO-05)
  Given an active promo "TEDX15" exists
  When an Admin creates another live promo "TEDX15"
  Then the response is 409 error.code "PROMO_CODE_TAKEN"
```

## AC-PROMO-02 — Promo constraints
> **Covers:** US-ADM-PRM-02

```gherkin
Scenario: Promo carries optional caps, window, and event scope
  Given an Admin creating a promo
  When they set a global redemption cap, a per-user limit, a validity window,
       and an optional event scope (null = all events)
  Then all constraints are stored
```

## AC-PROMO-03 — Promo validation at quote/reserve (FR-PROMO-03, D:Q19)
> **Covers:** US-ADM-PRM-02, US-ORD-01

```gherkin
Scenario Outline: Promo rejection reasons (advisory at quote, same codes at reserve)
  Given a promo in state "<state>"
  When a user applies it at quote or reserve
  Then the response is 422 with error.code "<code>"
  Examples:
    | state                          | code             |
    | inactive                       | PROMO_INACTIVE   |
    | before its validity window     | PROMO_NOT_YET_VALID |
    | after its validity window      | PROMO_EXPIRED    |
    | over its global cap            | PROMO_CAP_REACHED |
    | over the user's per-user limit | PROMO_USER_LIMIT |
    | scoped to a different event    | PROMO_WRONG_EVENT |

Scenario: Valid promo is accepted in a quote
  Given a valid, in-window promo scoped to the event (or global)
  When a user requests a quote with it
  Then the quote shows base, discount, and final price
  And no redemption slot is consumed (quote is advisory, D:Q19)

Scenario: Cap check at quote is advisory; atomic enforcement is at payment initiation (D:Q19)
  Given a promo with 1 slot remaining
  When two users each request a quote with it
  Then both quotes succeed (no slot consumed)
  When both users reserve (no slot consumed at reserve either)
  When user A initiates payment first
  Then A's slot is atomically claimed
  And user B initiating payment is rejected with PROMO_CAP_REACHED
```

## AC-PROMO-04 — Redemption accounting (D:Q19)
> **Covers:** US-ADM-PRM-03

```gherkin
Scenario: Reserving does not consume a redemption slot (D:Q19)
  Given a promo with a global cap of 1 remaining
  When two users reserve orders using it without paying
  Then neither reservation consumes the cap
  And the promo still shows 1 slot available

Scenario: Slot is claimed atomically at payment initiation (D:Q19)
  Given a promo with 1 slot remaining
  When user A initiates payment for a promo order
  Then the slot is claimed for A
  And user B initiating payment with the same promo is rejected with PROMO_CAP_REACHED

Scenario: Slot released on payment failure or hold expiry (D:Q19)
  Given user A has claimed the last promo slot at payment initiation
  When A's payment fails or the hold expires
  Then the slot is released and becomes available again

Scenario: Redemption confirmed and recorded on Paid (FR-PROMO-04, D:Q19)
  Given a promo order that becomes Paid
  Then a redemption record (code, user, order, timestamp) is written

Scenario: Free/100%-off promo claims the slot at confirmation (D:Q19)
  Given a 100%-off promo makes an order's final price 0
  When the order is confirmed (no gateway step)
  Then the redemption slot is claimed and recorded at confirmation
```


---

## AC-ORD-01 — Price quote (no hold) (FR-ORD-01)
> **Covers:** US-ORD-01

```gherkin
Scenario: Quote for an individual ticket uses the event face price (Model B)
  Given a Published event with ticketPrice 200.00 EGP
  When an authenticated user requests a quote for 2 individual tickets (no package) with no promo
  Then the response shows base 400.00, discount 0.00, final 400.00
  And no order is created and no seats are held

Scenario: Quote for a package uses the package price
  Given a Published event with package "Group-5" (5 seats, 750.00 EGP)
  When an authenticated user requests a quote for quantity 1 with promo "TEDX15" (15%)
  Then the response shows base 750.00, discount 112.50, final 637.50 (D:Q18)
  And no order is created and no seats are held

Scenario: Discount is rounded half-up to 2 decimals (D:Q18)
  Given a base price of 99.99 EGP and a 15% promo
  When a quote is requested
  Then discount = 15.00 (14.9985 rounded half-up) and final = 84.99

Scenario: Over-large fixed discount floors the final at zero (D:Q18)
  Given a base price of 30.00 EGP and a 50.00 EGP fixed promo
  When a quote is requested
  Then discount = 30.00 and final = 0.00 (never negative)

Scenario: Quote respects per-package max quantity (D:Q2)
  Given package "Single" with MaxQuantityPerOrder = 10
  When a user requests a quote for quantity 11
  Then the response is 422 error.code "QUANTITY_EXCEEDS_MAX"

Scenario: Quote respects the event individual max quantity (Model B, mirrors D:Q2)
  Given an event with maxIndividualQtyPerOrder = 6
  When a user requests a quote for 7 individual tickets
  Then the response is 422 error.code "QUANTITY_EXCEEDS_MAX"
```

## AC-ORD-02 — Reserve an order (FR-ORD-02, D:Q1)
> **Covers:** US-ORD-02, US-ORD-04

```gherkin
Scenario: Reserve rejected when event is not Published
  Given an event in Draft, Archived, or Cancelled state
  When an authenticated user attempts to reserve an order for it
  Then the response is 422 error.code "EVENT_NOT_PUBLISHED"
  And no order is created and no seats are held

Scenario: Reserve an individual-ticket order holds seats and snapshots price (Model B, FR-ORD-04)
  Given a Published event with 100 remaining seats and ticketPrice 200.00 EGP
  When an authenticated user reserves 3 individual tickets (no package)
  Then an order is created in PendingPayment with a null package reference
  And 3 seats are held
  And the order snapshots the unit price (200.00), base, discount, and final price, plus the event title
  And a 15-minute hold window is set (D:Q3, FR-ORD-05)

Scenario: Reserve a package order holds seats and snapshots price (D:Q1, FR-ORD-04)
  Given a Published event with 100 remaining seats and package "Group-5" (5 seats)
  When an authenticated user reserves quantity 1
  Then an order is created in PendingPayment
  And 5 seats are held
  And the order snapshots package name, unit price, base, discount, and final price
  And a 15-minute hold window is set (D:Q3, FR-ORD-05)

Scenario: One unit-type per order (D:Q1)
  When a user reserves an order
  Then the order references exactly one unit-type × quantity — individual tickets OR a single package
  And mixing individual tickets and a package (or two package types) requires separate orders

Scenario: Reserve re-prices server-side; quote is advisory (D:Q4)
  Given a user was quoted 637.50 but the price changed since
  When they reserve
  Then the server recomputes the price from the live event/package + promo state
  And if it differs from the quote, responds 409 error.code "PRICE_CHANGED" with the new quote
  And no order is created until the user re-confirms
  And the client never sends a price to be trusted
```

## AC-ORD-03 — Capacity & concurrency safety (FR-ORD-03, NFR-REL-01, D:Q3)
> **Covers:** US-ORD-02

```gherkin
Scenario: Reservation rejected when insufficient seats
  Given an event with 3 remaining seats
  When a user reserves package "Group-5" (5 seats)
  Then the response is 409 error.code "SEATS_UNAVAILABLE"

Scenario: Concurrent reservations never oversell (NFR-REL-01)
  Given an event with exactly 5 remaining seats
  When two users simultaneously reserve 5 seats each
  Then exactly one reservation succeeds and the other gets SEATS_UNAVAILABLE
  And total held+paid seats never exceed capacity

Scenario: Expired-but-unswept hold does not block new reservations (D:Q3)
  Given a PendingPayment order whose HoldExpiresAt is in the past
  And the sweeper has not yet flipped it to Expired
  When another user reserves those seats
  Then the seats are available (availability uses HoldExpiresAt > now, not status)
```

## AC-ORD-04 — One active pending order per user per event (D:Q5)
> **Covers:** US-ORD-02

```gherkin
Scenario: Re-reserving is rejected when an unexpired pending order exists (D:Q5)
  Given a user has a PendingPayment (unexpired) order for event E
  When they attempt to reserve another order for event E
  Then the response is 409 error.code "ACTIVE_ORDER_EXISTS" with the existingOrderId
  And no second hold is created (client resumes or cancels the existing order)

Scenario: A paid order does not block a new purchase (D:Q5)
  Given a user has a Paid order for event E
  When they reserve another order for event E
  Then a new PendingPayment order is created

Scenario: Pending orders on different events are independent (D:Q5)
  Given a user has a PendingPayment order for event E1
  When they reserve for event E2
  Then a separate PendingPayment order is created
```

## AC-ORD-05 — Hold expiry (FR-ORD-05, FR-ORD-09)
> **Covers:** US-ORD-06

```gherkin
Scenario: Unpaid hold auto-releases after the window
  Given a PendingPayment order older than 15 minutes with no confirmed payment
  When the hold sweeper runs
  Then the order transitions to Expired
  And its held seats are released
  And any claimed promo slot is released (D:Q19)
```

## AC-ORD-06 — Cancel an unpaid order (FR-ORD-06)
> **Covers:** US-ORD-03

```gherkin
Scenario: User cancels their own unpaid order
  Given a user's PendingPayment order
  When they cancel it
  Then the order becomes Cancelled and its seats are released immediately
  And any claimed promo slot is released (D:Q19)

Scenario: User cannot cancel a paid order (D:Q6)
  Given a user's Paid order
  When they attempt to cancel it themselves
  Then the response is 409 error.code "ORDER_NOT_CANCELLABLE"
  And the state is unchanged (paid-order voiding is Admin-only; refunds handled offline)
```

## AC-ORD-07 — Order history (FR-ORD-07)
> **Covers:** US-ORD-05

```gherkin
Scenario: User views their orders and tickets
  Given a user with orders in various states
  When they list their order history
  Then all their orders (every status) are returned, paginated
  And tickets are visible for Paid orders only (D:Q7: unpaid orders have zero tickets)

Scenario: A user cannot see another user's orders
  When a user requests an order they do not own
  Then the response is 403 (or 404) and no data leaks
```

## AC-ORD-08 — Orders are never deleted (FR-ORD-08, NFR-REL-03)
> **Covers:** US-ORD-05, US-ORD-06

```gherkin
Scenario: Order lifecycle is status-only
  Then an order transitions PendingPayment → Paid | Cancelled | Expired
  And no order is ever hard-deleted
```

---

## AC-PAY-01 — Initiate online payment (FR-PAY-01)
> **Covers:** US-PAY-01

```gherkin
Scenario: Paid order initiates a Paymob session
  Given a PendingPayment order with final price 637.50 EGP
  When the user initiates payment
  Then the system creates a Paymob intention and returns a checkout URL/session
  And the amount sent to Paymob equals 63750 piastres (×100 at the boundary only, D:Q18/Q27)

Scenario: Idempotency-Key returns the same checkout session (D:Q28a)
  Given a user initiates payment with Idempotency-Key "K1"
  When they retry initiation with the same key "K1"
  Then the same checkout session is returned (no duplicate Paymob intention)

Scenario: Promo slot claimed at initiation (D:Q19)
  Given the order carries a capped promo
  When payment is initiated
  Then a redemption slot is atomically claimed (or PROMO_CAP_REACHED if none remain)

Scenario: Hold expired at payment initiation (D:Q3)
  Given a PendingPayment order whose HoldExpiresAt is in the past
  When the user attempts to initiate payment
  Then the response is 409 error.code "HOLD_EXPIRED"
  And the order is transitioned to Expired and its seats are released

Scenario: Free order cannot use the paid path (D:Q18)
  Given a PendingPayment order with finalPrice 0.00
  When the user calls the pay endpoint
  Then the response is 409 error.code "ORDER_IS_FREE"
  And the user is directed to the confirm-free path
```

## AC-PAY-02 — Webhook confirms payment (FR-PAY-02/03/04, NFR-SEC-04, NFR-REL-02)
> **Covers:** US-PAY-02, US-PAY-04

```gherkin
Scenario: Signature-verified success marks the order Paid and issues tickets
  Given a PendingPayment order awaiting payment
  When Paymob sends a webhook with a valid HMAC signature confirming success
  And the reported amount matches the order's snapshotted final price (D:Q18)
  Then the order becomes Paid
  And exactly one ticket per held seat is issued (D:Q7, FR-TKT-01)
  And the promo redemption (if any) is confirmed and recorded (D:Q19)

Scenario: Unsigned or mismatched-signature webhook is rejected (NFR-SEC-04)
  When a webhook arrives with a missing or invalid HMAC signature
  Then it is rejected and the order is not marked Paid

Scenario: Amount mismatch is rejected and recorded as Failed (FR-PAY-04)
  Given an order with final price 637.50
  When a signed webhook reports a different amount
  Then the order is not confirmed (stays PendingPayment)
  And a Payment record is written with Status = Failed and the mismatched amount (US-PAY-02)
  And the discrepancy is logged

Scenario: Signature-verified failure releases the claimed promo slot (D:Q19)
  Given a PendingPayment order that claimed a promo slot at initiation
  When Paymob sends a validly-signed webhook reporting payment failure
  Then the order stays PendingPayment (until it pays again or the hold expires)
  And the claimed promo redemption slot is released
  And a Payment record is written with Status = Failed

Scenario: Webhook is idempotent (FR-PAY-03, NFR-REL-02)
  Given an order already marked Paid with tickets issued
  When a repeated/replayed webhook arrives for the same transaction
  Then no duplicate tickets are issued and seats are not double-counted

Scenario: The client-reported result is never trusted (FR-PAY-02)
  When a client claims payment success without a verified webhook
  Then the order remains PendingPayment
```

## AC-PAY-03 — Free / zero-price orders bypass the gateway (FR-PAY-06, D:Q18)
> **Covers:** US-PAY-03

```gherkin
Scenario: Zero final price confirms immediately
  Given an order whose final price is 0 (free package or 100%-off promo)
  When the user confirms
  Then the order becomes Paid without any gateway call
  And tickets are issued immediately
  And the promo slot (if any) is claimed and recorded at confirmation (D:Q19)
```

## AC-PAY-04 — Payment attempts recorded (FR-PAY-05)
> **Covers:** US-PAY-02 (FR-PAY-05 facet)

```gherkin
Scenario: Each attempt is recorded for reconciliation
  When a payment is attempted
  Then a payment record captures status, Paymob transaction id, amount, and the raw verified payload
```

## AC-PAY-05 — Admin voids a paid order (offline refund) (FR-PAY-07, D:Q6)
> **Covers:** US-CHK-04

```gherkin
Scenario: Voiding a paid order releases only not-checked-in seats (D:Q6)
  Given a Paid order with 5 Issued tickets, 2 of them already CheckedIn
  When an Admin voids the order
  Then the 3 not-checked-in tickets become Voided and their seats are released
  And the 2 CheckedIn tickets are NOT voided and their seats stay consumed (D:Q6)
  And a refund entry is recorded (money handled offline)
```

---

## AC-TKT-01 — Ticket issuance (FR-TKT-01/02, D:Q7)
> **Covers:** US-TKT-01

```gherkin
Scenario: One ticket per seat on Paid
  Given a Paid order holding 5 seats
  Then exactly 5 tickets are issued, each Issued state
  And each has a unique QR token and a short human-readable public reference (D:Q8)

Scenario: Unpaid order has zero tickets (FR-TKT-02)
  Given a PendingPayment order
  Then it has no tickets
```

## AC-TKT-02 — Optional guest names (FR-TKT-03)
> **Covers:** US-TKT-02

```gherkin
Scenario: A nameless ticket is still valid
  Given an issued ticket with no guest name
  Then it is a fully valid admission credential

Scenario: Guest name can be set per ticket
  Given a paid order's tickets
  When the buyer sets a guest name on a ticket
  Then that name is stored; guests need no account

Scenario: Guest name cannot be changed after check-in
  Given a ticket that is already CheckedIn
  When the buyer attempts to set or change the guest name
  Then the response is 409 error.code "TICKET_CHECKED_IN"
  And the name is unchanged
```

## AC-TKT-03 — QR token security (FR-TKT-04, NFR-SEC-05, D:Q8)
> **Covers:** US-TKT-03

```gherkin
Scenario: Only the hash is persisted (D:Q8)
  When a ticket is issued
  Then the QR encodes the public reference + a 256-bit random secret
  And the server stores only a deterministic SHA-256 hash of the secret
  And the raw secret is never persisted

Scenario: QR image is served as a binary asset, owner-only, never cached
  Given a Paid order's ticket
  When the owner requests the QR image
  Then the response is 200 image/png (binary, not the JSON envelope)
  And the response carries Cache-Control: no-store
  And the raw QR payload (reference + secret) is never returned as a JSON field anywhere in the API
  When a non-owner (not Admin) requests the same QR image
  Then the response is 403 error.code "FORBIDDEN"
```

## AC-CHK-01 — Check-in outcomes (FR-TKT-05/06, D:Q8, D:Q9)
> **Covers:** US-CHK-01, US-CHK-02, US-CHK-03

```gherkin
Scenario: Successful first scan
  Given an Admin checking in for event E
  And an Issued ticket for event E
  When the Admin scans it (reference + secret)
  Then the server looks up by reference, verifies the secret against the stored hash
  And the ticket becomes CheckedIn, recording who scanned and when

Scenario: Second scan is rejected as already-checked-in
  Given a ticket already CheckedIn
  When it is scanned again
  Then the response is 409 error.code "TICKET_ALREADY_CHECKED_IN" with the original who/when
  And the rejected attempt is logged (FR-TKT-06)

Scenario: Wrong-event ticket rejected distinctly (D:Q9)
  Given an Admin checking in for event E
  And a valid Issued ticket for a different event F
  When the Admin scans it
  Then the response is 409 error.code "WRONG_EVENT"
  And the attempt is logged

Scenario: Unknown or tampered token rejected (D:Q8)
  When a scan presents an unknown reference or a secret that fails hash comparison
  Then the response is 404 error.code "TICKET_INVALID"
  And the attempt is logged

Scenario: Voided ticket cannot be checked in
  Given a Voided ticket
  When it is scanned
  Then the response is 409 error.code "TICKET_VOIDED" and the attempt is logged

Scenario: Only an Admin can check in (D:Q9)
  Given a non-Admin caller
  When they call the check-in endpoint
  Then the response is 403
```


---

## AC-TRK-01 — Track CRUD (FR-TRK-01)
> **Covers:** US-ADM-TRK-01, US-BRD-TRK-01

```gherkin
Scenario: Admin creates a track with a unique name
  Given an Admin
  When they create a track "Public Speaking" with description and schedule
  Then the track is created
  And creating another live track named "Public Speaking" is rejected (unique among live tracks)

Scenario: Soft-deleting a track auto-ends its assignments (D:Q14)
  Given a track with 8 active Member enrollments and 1 Board assignment
  When an Admin soft-deletes the track after confirming the stated impact ("ends 8 enrollments and 1 Board assignment")
  Then all active enrollments and the Board assignment are ended (EndedAt set)
  And all attendance/evaluation history is retained
  And those users become free to be assigned elsewhere (D:Q11, FR-ROLE-04)
```

## AC-SES-01 — Session management, scoped to the supervised track (FR-TRK-02, D:Q13)
> **Covers:** US-BRD-SES-01, US-BRD-SES-02, US-BRD-SES-03, US-BRD-SES-04, US-ADM-SES-01

```gherkin
Scenario: Board creates a session in their own track
  Given a Board@Y
  When they create a session in Track Y
  Then the session is created

Scenario: Admin can manage sessions for any track (US-ADM-SES-01, D:Q13)
  Given an Admin
  When they create, edit, or delete a session in any track
  Then the action succeeds regardless of which track it is

Scenario: Board cannot manage another track's sessions (D:Q13)
  Given a Board@Y who is also a Member of Track X
  When they attempt to create/edit a session in Track X
  Then the response is 403 error.code "TRACK_FORBIDDEN"

Scenario: Session status transitions (US-BRD-SES-04, DataModel §3.3)
  Given a Scheduled session whose EndsAtUtc is in the past
  When a Board or Admin transitions it to Held
  Then the session status becomes Held
  Given a Scheduled or Held session
  When a Board or Admin transitions it to Cancelled
  Then the session status becomes Cancelled
  Given a Scheduled session whose EndsAtUtc is in the future
  When a Board attempts to transition it to Held
  Then the response is 409 error.code "ILLEGAL_STATUS_TRANSITION"

Scenario: Session with records can be edited but not hard-deleted (D:Q13)
  Given a session that has attendance or evaluation records
  When a Board edits its topic/time
  Then the edit succeeds
  When the Board attempts to hard-delete it
  Then the delete is rejected with 409 error.code "SESSION_HAS_RECORDS"

Scenario: Records-free session can be removed
  Given a session with zero attendance and zero evaluations
  When a Board deletes it
  Then it is removed
```

## AC-TRK-02 — Member views their own track's sessions (FR-TRK-03)
> **Covers:** US-MEM-SES-01

```gherkin
Scenario: Member sees upcoming and past sessions of their track
  Given a Member@X
  When they view sessions
  Then only Track X's sessions are returned, split into upcoming and past

Scenario: Member cannot view another track's sessions
  When a Member@X requests Track Y's sessions
  Then the response is 403
```

## AC-ATT-01 — Recording attendance (FR-ATT-01/02, D:Q11)
> **Covers:** US-BRD-01

```gherkin
Scenario: Board records attendance as Present, Late, or Absent
  Given a Board@Y and a session in Track Y that has occurred
  When they mark a member Present
  Then an attendance record is stored for that (session, enrollment)

Scenario: At most one attendance record per member per session (FR-ATT-02)
  Given an existing attendance record for a member and session
  When the Board records again
  Then the existing record is updated (no duplicate)

Scenario: Attendance cannot be recorded for a future session (SESSION_NOT_OCCURRED)
  Given a session whose EndsAtUtc is in the future
  When a Board attempts to record attendance for a member
  Then the response is 422 error.code "SESSION_NOT_OCCURRED"
  And no attendance record is created

Scenario: Attendance is keyed on enrollment, not raw user (D:Q11)
  Given a member who left Track X and later re-enrolled (new enrollment)
  Then attendance for the old enrollment is retained separately
  And the new enrollment starts with a fresh attendance record set
```

## AC-ATT-02 — Attendance percentage (FR-ATT-03, D:Q12)
> **Covers:** US-MEM-01

```gherkin
Scenario: Late counts as attended (FR-ATT-03)
  Given a member with 3 Present, 1 Late, 1 Absent across 5 recorded past sessions
  Then attendance % = (3 + 1) / 5 = 80%

Scenario: Only occurred, recorded sessions count in the denominator (D:Q12)
  Given a track with 10 planned sessions, of which 2 have occurred and are recorded for the member (both attended)
  Then attendance % = 100% (2/2); future sessions are excluded

Scenario: A past session with no record for the member is excluded (D:Q12)
  Given a past session for which the Board recorded no entry for this member
  Then that session is NOT counted as absent and is excluded from the denominator
  And Absent must be recorded explicitly to count against the member
```

## AC-ATT-03 — Attendance visibility (FR-ATT-04)
> **Covers:** US-BRD-02, US-ADM-ATT-01

```gherkin
Scenario: Board sees their track's attendance; Admin sees all tracks
  Given a Board@Y
  Then they can view attendance for all Track Y members only
  Given an Admin
  Then they can view attendance across all tracks
```

## AC-EVL-01 — Creating and editing evaluations (FR-EVL-01/02, D:Q16, D:Q17)
> **Covers:** US-BRD-03, US-BRD-04

```gherkin
Scenario: Evaluation requires a past session and an active enrollment (D:Q16)
  Given a Board@Y and a member with an active enrollment in Track Y
  And a session that has already occurred
  When the Board submits a score of 85
  Then the evaluation is stored

Scenario: Cannot evaluate a future session (D:Q16)
  Given a session whose date is in the future
  When a Board attempts to evaluate a member for it
  Then the response is 422 (session has not occurred)

Scenario: Evaluation for a member with an ended enrollment is rejected (D:Q16)
  Given a member whose enrollment in Track Y has been ended (EndedAt set)
  When a Board attempts to submit an evaluation for that member
  Then the response is 422 error.code "MEMBER_NOT_ENROLLED"
  And no evaluation record is created

Scenario: Score bounds are enforced (D:Q17)
  When a Board submits a score of 101, -1, or 87.5
  Then the response is 422 (score must be an integer 0–100 inclusive)

Scenario: One evaluation per member per session, edited in place (FR-EVL-02, D:Q17)
  Given an existing evaluation for a (member, session)
  When the Board edits it
  Then the same record is overwritten with audit columns (who/when), no duplicate and no version history
```

## AC-EVL-02 — Evaluation visibility (FR-EVL-03/04)
> **Covers:** US-MEM-02, US-BRD-05

```gherkin
Scenario: A member sees only their own evaluations
  Given a Member@X
  When they view evaluations
  Then only their own scores and feedback are returned

Scenario: A member cannot see another member's evaluations
  When a member requests another member's evaluation
  Then the response is 403

Scenario: A Board sees evaluations for all members of their track
  Given a Board@Y
  Then they can view evaluations for all Track Y members
```

---

## AC-DASH-01 — Member & Board dashboards, role isolation (D via FR-ROLE-04)
> **Covers:** US-MEM-03, US-MEM-04, US-BRD-06

```gherkin
Scenario: Member dashboard aggregates the current enrollment (US-MEM-03)
  Given a Member@X with a recorded attendance history and evaluations
  When they open their dashboard
  Then it shows their attendance % (current active enrollment, D:Q11),
       their latest evaluations, and their upcoming sessions for Track X

Scenario: Training and ticketing views stay separate (US-MEM-04)
  Given a Member@X who is also an Attendee
  When they use the booking flow
  Then it behaves identically to any Attendee's booking
  And their training dashboard data never leaks into ticketing views (and vice versa)

Scenario: Board dashboard summarizes only the supervised track (US-BRD-06)
  Given Yousef is Board of Track Y and Member of Track X
  When he opens his Board dashboard
  Then it summarizes Track Y (member count, attendance averages) only

Scenario: Board role isolation — acting on another track is forbidden (US-BRD-06, security)
  Given Yousef is Board of Track Y and Member of Track X
  When he attempts any Board action (session, attendance, evaluation, enroll, notify) on Track X
  Then the response is 403 error.code "TRACK_FORBIDDEN"
  And the decision is made server-side from his per-request assignments, not the token

Scenario: Switching to his own Member view exposes no Board powers on Track X
  Given Yousef viewing his Member@X dashboard
  Then he sees only his own attendance and evaluations for Track X
  And no supervisory controls are available for Track X
```

---

## AC-NTF-01 — Sending notifications (FR-NTF-01/02, D:Q21)
> **Covers:** US-NTF-01, US-NTF-02

```gherkin
Scenario: Admin sends to an audience; recipients are snapshotted at send time (D:Q21)
  Given 40 users are Members at send time
  When an Admin sends a notification to audience "all Members"
  Then 40 per-recipient rows are created, each with its own read state
  And a member enrolled tomorrow does NOT retroactively receive it

Scenario: Board sends only to their own track's active members (FR-NTF-02, D:Q21)
  Given a Board@Y
  When they send a track notification
  Then it fans out to Track Y's current active members only
  When a Board attempts to notify another track
  Then the response is 403

Scenario: Zero-recipient send is rejected (US-NTF-01, DataModel §4.1)
  Given an audience that resolves to zero recipients (e.g. a Track with no active members)
  When an Admin or Board sends a notification to that audience
  Then the response is 422 error.code "NO_RECIPIENTS_RESOLVED"
  And no Notification row is created

Scenario: Admin audience taxonomy (D:Q21)
  Then an Admin may target platform-wide (all active users), by global role (all Attendees / all Admins), or by track
```

## AC-NTF-02 — Reading notifications (FR-NTF-03/04)
> **Covers:** US-NTF-03

```gherkin
Scenario: Each recipient has independent read state
  Given a user with notifications in their inbox
  When they mark one as read
  Then only their own read state changes; other recipients are unaffected

Scenario: Mark all as read clears the inbox in one call
  Given a user with multiple unread notifications
  When they call the mark-all-as-read endpoint
  Then all their unread notifications are marked read
  And the response is 204 No Content

Scenario: A user sees only their own inbox
  When a user lists notifications
  Then only rows addressed to them are returned, paginated
  And filtering by unreadOnly returns only unread rows
```

---

## AC-PUB-01 — Public pages & contact form (FR-PUB-01/02/03, D:Q20)
> **Covers:** US-PUB-01, US-PUB-02, US-PUB-03, US-ADM-CON-01

```gherkin
Scenario: Visitor views public pages
  Given an unauthenticated Visitor
  Then they can view Home, About, Team, Events, Event Detail, Contact, and auth pages

Scenario: Contact form accepts a submission with limits (D:Q20)
  Given a Visitor
  When they submit name, email, subject (≤ 200 chars), and message (≤ 2000 chars) with a valid email format
  Then the submission is stored with status New for Admin review

Scenario: Contact form is rate-limited (D:Q20, NFR-SEC-10)
  Given repeated submissions from the same IP beyond the limit
  Then further submissions get 429 error.code "RATE_LIMITED" with a Retry-After header

Scenario: Admin manages submissions (D:Q20)
  Given an Admin
  Then they can list, read (→ Read), and archive (→ Archived) submissions
  And there is no in-app reply; submissions are Admin-only
```

---

## AC-RPT-01 — Reports & export (RPT-01/02/03/04, D:Q28c)
> **Covers:** US-ADM-RPT-01, US-ADM-RPT-02, US-ADM-RPT-03

```gherkin
Scenario: Event report
  Given an Admin
  When they request the report for an event
  Then it returns registration counts and attendance rate for that event

Scenario: Track report
  When an Admin requests a track report
  Then it returns member progress, attendance, and evaluation averages

Scenario: Financial report separates revenue from refunds (US-ADM-RPT-03, Issue 7)
  When an Admin requests a financial report
  Then revenue counts only orders that reached Paid (by PaidAtUtc)
  And a Paid order that was later voided (has a matching RefundEntry) is categorized as Refunded, not Revenue
  And a Cancelled order that was never Paid (no RefundEntry) is excluded from both revenue and refunds
  And revenue is broken down by event and by unitType (Individual vs Package)

Scenario: Financial report honors a date range (US-ADM-RPT-03)
  Given Paid orders spanning several months
  When an Admin requests the financial report with fromDate/toDate
  Then only orders with PaidAtUtc inside the range are counted

Scenario: Report over empty data returns zeroed totals, not an error
  Given an event/track/date-range with no qualifying orders or records
  When an Admin requests the corresponding report
  Then the response is 200 with zeroed counts and totals (empty arrays, 0.00 EGP), not a 404

Scenario: CSV/PDF export as a format parameter (D:Q28c)
  Given any report endpoint
  When the Admin adds ?format=csv or ?format=pdf
  Then the report is returned in that format with the appropriate Content-Type and Content-Disposition
  And the JSON envelope is not used for the file response
```

---

## AC-ROLE-08 — Search for enrollable users (D:Q15)
> **Covers:** US-ROLE-08

```gherkin
Scenario: Board searches for an Attendee to enroll
  Given a Board@Y
  When they search by name or email (min 2 chars)
  Then only active accounts with global role Attendee that have no active Member enrollment are returned
  And each result includes boardOfTrackId so the caller can see if the candidate is already Board elsewhere
  And results are paginated (D:Q26)

Scenario: Already-enrolled users are excluded from results
  Given a user who is already an active Member of any track
  When a Board searches for enrollable users
  Then that user does not appear in the results

Scenario: Board cannot search enrollable users for another track
  Given Yousef is Board of Track Y only
  When he calls the enrollable-users endpoint for Track X
  Then the response is 403 error.code "TRACK_FORBIDDEN"
```

## AC-ADM-EVT-07 — View event-scoped promo codes (D:Q50)
> **Covers:** US-ADM-EVT-07

```gherkin
Scenario: Admin views promo codes scoped to an event
  Given an Admin and an event with associated promo codes
  When they request the event-scoped promo code report
  Then the response lists each code with discountType, discountValue, redemptionCount,
       globalRedemptionCap, perUserLimit, isActive, validFrom, validUntil
  And results are paginated

Scenario: Event with no promo codes returns an empty list
  Given an event with no associated promo codes
  When an Admin requests its promo code report
  Then the response is 200 with an empty data array
```

## AC-ADM-PKG-03 — List packages for an event
> **Covers:** US-ADM-PKG-03

```gherkin
Scenario: Admin lists all packages including inactive and soft-deleted
  Given an event with active, inactive, and soft-deleted packages
  When an Admin lists packages with includeInactive=true
  Then all packages are returned with computed remaining seats and redemption counts

Scenario: Default listing excludes soft-deleted packages
  When an Admin lists packages without includeInactive
  Then soft-deleted packages are excluded; inactive (but not deleted) packages are included
```

## AC-ADM-PRM-04 — Promo code CRUD lifecycle (D:Q50)
> **Covers:** US-ADM-PRM-04

```gherkin
Scenario: Admin edits a promo code
  Given an Admin and an existing promo code
  When they update caps, validity window, scope, or active status with the correct rowVersion
  Then the changes are saved

Scenario: Concurrent promo edit is guarded by optimistic concurrency
  Given two Admins editing the same promo code
  When the second save uses a stale rowVersion
  Then the response is 409 error.code "CONCURRENCY_CONFLICT"

Scenario: ValidFrom must be earlier than ValidUntil when both are set
  When an Admin sets validFrom after validUntil
  Then the response is 422 error.code "VALIDATION_ERROR" with fieldErrors on the date fields

Scenario: Soft-delete retains redemption history
  Given a promo code with recorded redemptions
  When an Admin soft-deletes it
  Then the code is removed from active listings
  And all redemption history is retained (FR-PROMO-04)

Scenario: Soft-deleted code's code string is freed for reuse (FR-PROMO-05)
  Given a soft-deleted promo with code "TEDX20"
  When an Admin creates a new live promo with code "TEDX20"
  Then the new promo is created (uniqueness is among live codes only)
```

## AC-SYS-01 — Background sweeper (D:Q3, D:Q19, D:Q34, D:Q45, D:Q53)
> **Covers:** US-SYS-01

```gherkin
Scenario: Sweeper expires overdue holds and releases seats (D:Q3)
  Given a PendingPayment order whose HoldExpiresAt is in the past
  When the sweeper runs
  Then the order transitions to Expired
  And its held seats are released to availability
  And any claimed promo slot is released (D:Q19)

Scenario: Sweeper drains the outbox with retry/backoff (D:Q34, D:Q45)
  Given outbox messages pending delivery (e.g. order-confirmation email)
  When the sweeper processes the outbox
  Then each message is delivered at least once
  And failed deliveries are retried with backoff up to the configured max attempts
  And permanently failed messages are marked dead-letter, not silently dropped

Scenario: Only one sweeper instance runs at a time (D:Q53)
  Given multiple application instances running concurrently
  When the sweeper tick fires
  Then only one instance acquires the distributed lock (sp_getapplock or equivalent)
  And the others skip that tick without error
```

## AC-BRD-07 — Board member roster with attendance and evaluation summary
> **Covers:** US-BRD-07

```gherkin
Scenario: Board views paginated roster of active members
  Given a Board@Y
  When they request the member roster for Track Y
  Then only active members (EndedAtUtc IS NULL) are returned, paginated
  And each row includes the member's current attendance % and latest evaluation score
  And the Board cannot view the roster for another track (403 TRACK_FORBIDDEN)
```

## AC-MEM-05 — Member detailed attendance log
> **Covers:** US-MEM-05

```gherkin
Scenario: Member views session-by-session attendance breakdown
  Given a Member@X with an active enrollment
  When they view their attendance log
  Then each session in their current enrollment is listed with status (Present/Late/Absent)
  And the Board's recordedBy stamp is visible per record
  And records from previous (ended) enrollments are excluded from this view

Scenario: Attendance % is computed from the current active enrollment only (D:Q11, D:Q12)
  Given a member who was previously enrolled in Track X (ended) and is now enrolled in Track Y
  When they view their attendance log
  Then only Track Y records appear and the % reflects Track Y sessions only
```

## AC-ADM-TRK-03 — Edit track details
> **Covers:** US-ADM-TRK-03

```gherkin
Scenario: Admin edits track fields
  Given an Admin and an existing track
  When they update nameEn, nameAr, descriptionEn, descriptionAr, schedule, or isActive with the correct rowVersion
  Then the changes are saved

Scenario: NameEn must be unique among live tracks
  Given a live track named "Public Speaking"
  When an Admin renames another track to "Public Speaking"
  Then the response is 409 error.code "TRACK_NAME_TAKEN"

Scenario: Concurrent track edit is guarded by optimistic concurrency
  Given two Admins editing the same track
  When the second save uses a stale rowVersion
  Then the response is 409 error.code "CONCURRENCY_CONFLICT"
```

## AC-ADM-TRK-04 — List tracks with filters
> **Covers:** US-ADM-TRK-04

```gherkin
Scenario: Admin lists tracks with filters and search
  Given an Admin
  When they list tracks with isActive=true, search="speaking", page=1, pageSize=20
  Then only matching live tracks are returned, paginated
  And each row includes member count and whether a Board is currently assigned

Scenario: includeDeleted shows soft-deleted tracks
  When an Admin lists tracks with includeDeleted=true
  Then soft-deleted tracks are included in the results
```

## AC-ADM-TRK-05 — View full enrollment history for a track
> **Covers:** US-ADM-TRK-05

```gherkin
Scenario: Admin views active and ended enrollments for a track
  Given a track with both active and ended Member enrollments
  When an Admin views the track's enrollment history
  Then both active (EndedAtUtc IS NULL) and ended (EndedAtUtc IS NOT NULL) rows are returned
  And each row shows startedAt, endedAt (nullable), and the member's identity

Scenario: Ended enrollments retain their attendance and evaluation records
  Given an ended enrollment with recorded attendance and evaluations
  When an Admin views the enrollment history
  Then the ended enrollment row is present with its historical records accessible
```

---



```gherkin
Scenario: Standard response envelope (D:Q25)
  Then every response is { success, data, error }
  And errors carry { code, message, fieldErrors? } with correct HTTP status
  And internal detail never leaks to the client (logged with a traceId)

Scenario: Pagination on all list endpoints (D:Q26)
  Then lists accept ?page&pageSize&sort=field:dir plus named filters
  And responses include meta { page, pageSize, totalItems, totalPages }
  And pageSize is capped at 100 (default 20); unknown sort fields are rejected

Scenario: Wire formats (D:Q27)
  Then dates are ISO 8601 UTC with Z; money is a 2-dp number paired with currency "EGP"
  And IDs are GUID strings; enums are PascalCase strings

Scenario: Optimistic concurrency on admin-managed records (NFR-REL-06)
  Given two Admins editing the same Event/Package/Promo/Order
  When the second save uses a stale rowversion
  Then the response is 409 error.code "CONCURRENCY_CONFLICT" (not a silent overwrite)

Scenario: Log hygiene — no secrets or card data in logs (NFR-SEC-01/05, NFR-MNT-03)
  Given structured logs for requests, payment events, and check-in attempts (including rejected/duplicate scans)
  When the logs are inspected
  Then no plaintext passwords, refresh tokens, QR secrets, or full card/PAN data appear in them
  And rejected/duplicate scans are logged, never silently dropped

Scenario: i18n / RTL readiness (NFR-USE-02)
  Given the shipped English UI
  When the codebase is inspected
  Then user-facing strings are externalized (no hardcoded literals)
  And dates, numbers, and currency use locale-aware formatting
  And the layout can flip to RTL without a rewrite
```
