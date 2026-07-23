# TEDxAlkawmia — Personas

> **Version:** 1.0
> **Date:** 2026-07-17
> **Status:** Authoritative for user-facing personas
> **Reads from:** [01 — PRD](./01-PRD.md) · [02 — SRS](./02-SRS.md) · [03 — User Flows](./03-UserFlows.md) · [08 — Decision Log](./08-DecisionLog.md) · [10 — Data Model](./10-DataModel.md)

---

## How to read this document

Each persona is a **representative** user, not a job title. The platform has two independent role dimensions — a **global role** (Attendee / Admin) and **per-track assignments** (Member / Board) — so one real person can match more than one persona at once (the classic case: *Member of one track, Board of another*). Personas here map to those roles but describe **motivations, context, and frustrations**, which requirements and flows alone don't capture.

Every persona lists:
- **Snapshot** — who they are in one line.
- **Context & devices** — where and how they use the platform.
- **Goals** — what success looks like for them.
- **Frustrations** — the pain the platform must remove.
- **Key journeys** — the User-Flow sections they live in.
- **What they must never be able to do** — the guardrails their role implies.

Personas are ranked by how central they are to the product, not by privilege.

---

## Persona map

| Persona | Global role | Track assignment | Primary job on the platform |
|---------|-------------|------------------|-----------------------------|
| Nour — the Attendee | Attendee | — | Discover events, buy tickets, attend |
| Kareem — the Group Buyer | Attendee | — | Buy a multi-seat package for friends |
| Salma — the Member | Attendee | Member @ Track X | Train, track attendance & evaluations |
| Yousef — the Board (dual-role) | Attendee | Board @ Track Y **+** Member @ Track X | Supervise one track while training in another |
| Mariam — the Admin | Admin | — | Run events, payments, roles, check-in |
| Omar — the Visitor | *(none)* | — | Browse publicly, decide whether to join |

---

## 1. Nour — the Attendee ⭐

> *The person the paid-events core is built for.*

**Snapshot.** 24, university student and TEDx enthusiast. Heard about an upcoming event on Instagram and wants a ticket without the usual DM-and-screenshot dance.

**Context & devices.** Almost entirely on her phone, often on mobile data. Impatient with slow or confusing checkout. Pays with a debit card or a mobile wallet.

**Goals.**
- Find the next event and see clearly whether seats are left.
- Understand the price up front, including any discount, before committing.
- Buy a **single individual ticket** (or an optional package) and pay online in a few taps, and get a digital ticket she can show at the door.
- Not worry about losing a paper ticket or a screenshot.

**Frustrations (today).**
- Registration lives in social-media DMs; no confirmation, no record.
- No online payment — she has to transfer money and send proof.
- No real ticket — just a name on a list that slows down entry.

**Key journeys.** Discover & view events (§2) → the booking flow: quote → reserve → pay → tickets (§3) → manage my tickets (§4) → check-in at the door (§5, as the ticket holder).

**Stories.** [[05-UserStories#US-AUTH-01]], [[05-UserStories#US-AUTH-02]], [[05-UserStories#US-AUTH-03]], [[05-UserStories#US-AUTH-04]], [[05-UserStories#US-AUTH-05]], [[05-UserStories#US-AUTH-06]], [[05-UserStories#US-AUTH-07]], [[05-UserStories#US-USER-01]], [[05-UserStories#US-USER-02]], [[05-UserStories#US-USER-04]], [[05-UserStories#US-EVT-02]], [[05-UserStories#US-EVT-03]], [[05-UserStories#US-ORD-01]], [[05-UserStories#US-ORD-02]], [[05-UserStories#US-ORD-03]], [[05-UserStories#US-ORD-05]], [[05-UserStories#US-PAY-01]], [[05-UserStories#US-PAY-02]], [[05-UserStories#US-PAY-03]], [[05-UserStories#US-PAY-04]], [[05-UserStories#US-TKT-01]], [[05-UserStories#US-TKT-03]]

**What she must never be able to do.** See Draft/Archived/Cancelled events; hold seats without a real order; mark her own ticket as checked-in; see anyone else's orders or tickets.

**Design implications.** Mobile-first booking, a live remaining-seats signal, a transparent quote (base − discount = final), and a QR ticket that renders cleanly on a phone screen at the venue.

---

## 2. Kareem — the Group Buyer

> *A variant of the Attendee whose needs shape the package model.*

**Snapshot.** 29, wants to bring four friends to an event and pay for all of them at once.

**Context & devices.** Desktop when planning, phone at the door. Coordinates the group over chat and fronts the payment.

**Goals.**
- Buy a **multi-seat package** (e.g. Group-5) in a single order and payment.
- Get **one QR per seat** so each friend can enter independently.
- Optionally put each friend's name on their ticket — but not be forced to.

**Frustrations (today).** Buying five tickets means five separate manual registrations and five transfers to reconcile.

**Key journeys.** Booking flow with a multi-seat package (§3.1–3.3), naming individual tickets (§3.3, §4), distributing per-seat QRs.

**Stories.** [[05-UserStories#US-ORD-04]], [[05-UserStories#US-TKT-02]]

**What he must never be able to do.** Exceed remaining capacity with a large package (concurrency-safe check applies to the whole seat count); reassign a ticket after it's been checked in.

**Design implications.** Packages are **optional** bundles of seats layered on top of the individual ticket (the base unit); a paid order fans out to one ticket per seat; guest names are optional and a nameless ticket is still valid.

---

## 3. Salma — the Member

> *The heart of the training side.*

**Snapshot.** 21, accepted into one training track. Attends weekly sessions and wants to know how she's doing.

**Context & devices.** Phone for quick checks (next session, latest score), occasionally desktop to read feedback in detail.

**Goals.**
- See her track's **upcoming and past sessions** in one place.
- Track her **attendance percentage** (and understand that arriving late still counts as attended).
- Read her **evaluation history** — scores and written feedback — privately.
- Still behave as a normal Attendee when she wants to buy an event ticket.

**Frustrations (today).** No visibility into her own attendance or evaluations; everything lives in a supervisor's spreadsheet she never sees.

**Key journeys.** Member training dashboard (§8); and independently, the full Attendee booking flow (§3) since ticketing and training are separate concerns.

**Stories.** [[05-UserStories#US-MEM-01]], [[05-UserStories#US-MEM-02]], [[05-UserStories#US-MEM-03]], [[05-UserStories#US-MEM-04]], [[05-UserStories#US-USER-03]], [[05-UserStories#US-NTF-03]]

**What she must never be able to do.** See another member's evaluations; view or act on any track other than her own; record attendance or write evaluations (those are Board actions).

**Design implications.** A track-scoped dashboard; attendance math where **Late counts as attended**; evaluation visibility strictly limited to the member themselves.

---

## 4. Yousef — the Board (dual-role)

> *The persona that proves the platform's signature rule.*

**Snapshot.** 26, supervises **Track Y** as a Board member, and is simultaneously a **Member training in Track X**. He is the reason the role model has two independent dimensions.

**Context & devices.** Desktop for supervising (recording attendance, writing evaluations, managing sessions), phone for his own training view.

**Goals.**
- Manage **his one supervised track**: create/edit sessions, record attendance, write evaluations, notify members.
- Enroll or remove **Members in that track** without needing an Admin.
- Switch cleanly into his **own** Member view for Track X, with no bleed between the two roles.

**Frustrations (today).** Manual attendance sheets and evaluation forms; no way to message just his track; being treated as a single "rank" when his two roles are genuinely different.

**Key journeys.** Board attendance & evaluation (§9), Member enrollment within his track (§7 context, FR-ROLE-03), and — wearing his other hat — the Member dashboard (§8) for Track X.

**Stories.** [[05-UserStories#US-BRD-01]], [[05-UserStories#US-BRD-02]], [[05-UserStories#US-BRD-03]], [[05-UserStories#US-BRD-04]], [[05-UserStories#US-BRD-05]], [[05-UserStories#US-ROLE-04]], [[05-UserStories#US-NTF-02]]

**What he must never be able to do.** Act on **any track except the one he supervises** (403 even though he's a Member elsewhere); be Board of the same track he trains in; assign the Board role to anyone (Admin-only); touch payments or event management; hard-delete a session that has attendance or evaluation records (D:Q13).

**Design implications.** Authorization resolved **per request** from track assignments (never baked into the token); a hard boundary between his Board@Y powers and his Member@X data; the dual-role constraints (≤1 Member track, ≤1 Board track, must differ) enforced at assignment time and in the database.

---

## 5. Mariam — the Admin

> *The operator who runs everything the other personas rely on.*

**Snapshot.** 32, on the organizing-committee leadership. Owns the platform end to end and answers for the numbers.

**Context & devices.** Desktop for management and reporting; phone as a **scanner at the venue** for check-in.

**Goals.**
- Create events (with an **individual-ticket price**), optionally define **ticket packages**, and issue **promo codes**; publish when ready.
- See every order, attendee, and payment for an event; handle refunds (manual/offline).
- Assign **global roles** and the **Board** role; manage users and deactivate accounts.
- **Check tickets in** at the door quickly and reject invalid or already-used QRs.
- Oversee all tracks and read cross-track attendance/evaluation summaries.

**Frustrations (today).** No single source of truth; reconciling payments by hand; manual door lists; no audit trail for who did what.

**Key journeys.** Admin events/packages/promo (§6), orders & attendees per event (§6.2), users & roles incl. the dual-role assignment (§7), check-in at the door (§5), paid-order cancellation & offline refund (§4.2).

**Stories.** [[05-UserStories#US-ADM-EVT-01]], [[05-UserStories#US-ADM-EVT-02]], [[05-UserStories#US-ADM-EVT-03]], [[05-UserStories#US-ADM-EVT-04]], [[05-UserStories#US-ADM-EVT-05]], [[05-UserStories#US-ADM-DASH-01]], [[05-UserStories#US-ADM-TRK-01]], [[05-UserStories#US-ADM-PKG-01]], [[05-UserStories#US-ADM-PKG-02]], [[05-UserStories#US-ADM-PRM-01]], [[05-UserStories#US-ADM-PRM-02]], [[05-UserStories#US-MNG-01]], [[05-UserStories#US-MNG-02]], [[05-UserStories#US-MNG-03]], [[05-UserStories#US-ROLE-01]], [[05-UserStories#US-ROLE-02]], [[05-UserStories#US-ROLE-03]], [[05-UserStories#US-ROLE-04]], [[05-UserStories#US-ROLE-05]], [[05-UserStories#US-ROLE-06]], [[05-UserStories#US-ROLE-07]], [[05-UserStories#US-CHK-01]], [[05-UserStories#US-CHK-02]], [[05-UserStories#US-CHK-03]], [[05-UserStories#US-CHK-04]], [[05-UserStories#US-PAY-05]], [[05-UserStories#US-NTF-01]]

**What she must never be able to do (by design, not privilege).** Bypass payment verification (tickets issue only on a signature-verified Paymob webhook); hard-delete financial records (orders/payments are never deleted); silently drop a rejected scan (must be logged).

**Design implications.** A privileged global role with broad reach but bounded by integrity rules — idempotent check-in, verified payments, append-only financial history, and audit columns on admin- and money-touching tables.

---

## 6. Omar — the Visitor

> *Not logged in — the top of the funnel.*

**Snapshot.** 35, saw a TEDx talk online and landed on the site to see what's happening locally. Hasn't decided whether to sign up.

**Context & devices.** Any browser, desktop or mobile, first-time visit.

**Goals.**
- Browse **public pages** (Home, About, Team, Events, Event Detail, Contact) freely.
- See what an event is and roughly whether it's still available.
- Ask a question via the **contact form** without creating an account.
- Decide, on his own time, whether to register.

**Frustrations (today).** Public info is scattered across social posts; no clean event page; no obvious way to ask a question.

**Key journeys.** Discover & view events as an unauthenticated user (§2, "Login to book" gate), contact form (§12), then registration (§1.1) if he converts.

**Stories.** [[05-UserStories#US-EVT-01]], [[05-UserStories#US-EVT-02]], [[05-UserStories#US-EVT-03]], [[05-UserStories#US-EVT-04]], [[05-UserStories#US-PUB-01]], [[05-UserStories#US-PUB-02]], [[05-UserStories#US-PUB-03]], [[05-UserStories#US-AUTH-01]]

**What he must never be able to do.** Book or hold seats; see anything beyond Published public content; reach any authenticated endpoint.

**Design implications.** A read-only public surface with a clear conversion prompt ("Login to book"); a contact form open to anyone and stored without a user account.

---

## Persona → role → flow traceability

| Persona | Roles exercised | Central flows |
|---------|-----------------|---------------|
| Nour (Attendee) | Attendee | §2, §3, §4, §5 (holder) |
| Kareem (Group Buyer) | Attendee | §3 (multi-seat), §4 |
| Salma (Member) | Attendee + Member@X | §8, §3 |
| Yousef (Board, dual-role) | Attendee + Board@Y + Member@X | §9, §7, §8 |
| Mariam (Admin) | Admin | §5, §6, §7, §4.2 |
| Omar (Visitor) | none | §2, §12, §1.1 |

> **Note on overlap.** Every Member, Board, and Admin is also an Attendee — buying a ticket is a baseline capability of any authenticated account. Personas isolate the *distinctive* motivation of each role; they are not mutually exclusive people.
