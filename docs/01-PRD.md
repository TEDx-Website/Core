# TEDxAlkawmia Platform — Product Requirements Document (PRD)

> **Version:** 1.3
> **Date:** 2026-07-24
> **Author:** Engineering Team
> **Status:** Draft — Pending Stakeholder Approval

> **Authority note (v1.3).** This PRD is **authoritative for product scope** — *what* the platform does and *why*. The requirement-level detail lives in the **SRS (02)**; resolved design questions live in the **Decision Log (08)**; the downstream specs (**User Flows 03, Personas 04, User Stories 05, Acceptance Criteria 06, API Contract 07, System Design 09, Data Model 10, State Machines 11, Sequence Diagrams 12**) are all written and consistent with this PRD. Where any of these conflict with the PRD on *scope*, the PRD wins; on *requirement detail* the SRS wins; on a *resolved design question* the Decision Log wins; on *schema* the Data Model wins.
>
> **Doc map:** 00 Brief · 01 PRD *(this)* · 02 SRS · 03 User Flows · 04 Personas · 05 User Stories · 06 Acceptance Criteria · 07 API Contract · 08 Decision Log · 09 System Design · 10 Data Model · 11 State Machines · 12 Sequence Diagrams.
>
> **Changelog**
> - **v1.3 (2026-07-24):** Cross-doc consistency pass. Cleared the stale *(pending)* marker on **System Design (09)** — it is now written and authoritative. Added **State Machines (11)** and **Sequence Diagrams (12)** to the authority note and doc map. Scope, roles, and feature catalog re-verified against Decision Log Q1–Q56 (no scope change).
> - **v1.2 (2026-07-21):** Model-B ticketing — an event sells **individual tickets at a face price**; **packages are optional** bundles, not the only unit (glossary, §4.1, §5, EVT-01/05, ORD-01/03/05). Fixed the doc map in this note (08 is the Decision Log, not a Domain Model; the Data Model is 10).
> - **v1.1:** Roles reworked to **Attendee / Admin** (global) + **Member / Board** (per-track); a person may be **Member of one track and Board of another**. Ticketing became **paid events via Paymob** with **one QR per seat** and the reserve → hold → pay → QR flow. "Guest" renamed **Attendee**; **Visitor** = an unauthenticated browser. English UI, **i18n-ready** (Arabic/RTL later).

---

## 1. Executive Summary

TEDxAlkawmia Platform is a full-stack web application designed to digitize and streamline all operations for the TEDxAlkawmia community. The platform serves two primary functions:

1. **Public Event Management** — Enabling guests to discover events, register, receive QR-coded tickets, and attend TEDx events.
2. **Internal Training & Track Management** — Enabling the organizing team to manage training tracks, schedule sessions, record attendance, and evaluate members.

Features below are organized by **capability area**, not by release. Phasing and the MVP cut are decided separately in a later planning document; nothing here implies a delivery order.

---

## 2. Problem Statement

TEDxAlkawmia currently manages its operations through manual processes (spreadsheets, social media, manual attendance tracking). This leads to:

- **Inefficient event management** — No centralized system for event creation, registration, or ticketing.
- **Poor attendance tracking** — Manual check-ins are error-prone and slow.
- **No evaluation history** — Member progress in training tracks is not systematically tracked.
- **Communication gaps** — No unified channel for announcements, reminders, or feedback.
- **Administrative overhead** — Organizers spend excessive time on tasks that could be automated.

---

## 3. Product Vision

> *A single, beautiful, and scalable platform where the TEDxAlkawmia community can manage events, training tracks, and member engagement — replacing all manual processes with a modern digital experience.*

---

## 4. Target Users & Personas

### 4.1 Attendee (Public User)
- **Who:** Anyone interested in TEDx events (renamed from "Guest"; an unauthenticated browser is a **Visitor**).
- **Goal:** Discover events, buy tickets (individual or an optional package), pay online, get a QR ticket per seat, attend.
- **Pain Point:** Currently registers via social media or forms — no unified experience, no digital ticket, no online payment.
- **Key Needs:** Transparent pricing (incl. discounts), optional package selection, online payment, digital QR ticket per seat.

### 4.2 Member (Training Participant)
- **Who:** A person **enrolled in exactly one training track** within TEDxAlkawmia (a per-track role, not a global rank).
- **Goal:** Attend training sessions, track their progress, view evaluations.
- **Pain Point:** No visibility into their attendance record, evaluation history, or upcoming sessions.
- **Key Needs:** Personal dashboard, session schedule, evaluation feedback, attendance percentage.

### 4.3 Board Member (Track Supervisor)
- **Who:** A person **assigned by an Admin to supervise exactly one track**. May *also* be a Member of a different track at the same time.
- **Goal:** Supervise members, record attendance, provide evaluations, send notifications — for their one track only.
- **Pain Point:** Manages everything manually (attendance sheets, evaluation forms).
- **Key Needs:** Track dashboard, attendance recording, evaluation tools, member list.

### 4.4 Admin (Platform Administrator)
- **Who:** The TEDxAlkawmia organizing committee leadership.
- **Goal:** Full platform control — manage users, assign track roles, create events/packages/promo codes, oversee tracks, scan tickets at the door, and view reports.
- **Pain Point:** No centralized view of all operations.
- **Key Needs:** Admin dashboard, user & role management, event/package management, check-in, reports.

---

## 5. Roles & Permissions

Two dimensions: a **global role** (Attendee/Admin) and **per-track assignments** (Member/Board). "Board@T" means "Board of that specific track."

| Capability | Visitor | Attendee | Member@T | Board@T | Admin |
| ----------------------------- | :---: | :----: | :---: | :---: | :---: |
| Browse published events | ✅ | ✅ | ✅ | ✅ | ✅ |
| Register / Login / Edit Profile | — | ✅ | ✅ | ✅ | ✅ |
| Buy tickets (individual or package), pay online, view own QR tickets | ❌ | ✅ | ✅ | ✅ | ✅ |
| View own training dashboard (their track) | ❌ | ❌ | ✅ (T) | ✅ (T) | ✅ |
| Manage sessions / attendance / evaluations | ❌ | ❌ | ❌ | ✅ (**T only**) | ✅ |
| Enroll / remove **Members** in a track | ❌ | ❌ | ❌ | ✅ (**T only**) | ✅ |
| Send track notifications | ❌ | ❌ | ❌ | ✅ (T) | ✅ |
| Assign the **Board** role / global roles | ❌ | ❌ | ❌ | ❌ | ✅ |
| Create/manage events, packages, promo codes | ❌ | ❌ | ❌ | ❌ | ✅ |
| Confirm/refund & view payments | ❌ | ❌ | ❌ | ❌ | ✅ |
| Scan tickets at door (check-in) | ❌ | ❌ | ❌ | ❌ | ✅ |
| Manage users (list, deactivate) | ❌ | ❌ | ❌ | ❌ | ✅ |

> **Rules:** All new registrations default to **Attendee**. A **Board** may enroll/remove **Members** in **their own track** only; **only an Admin** assigns the **Board** role (and global roles). A person may hold **at most one Member track and one Board track**, and they must be **different** tracks (a Member of Track X can be Board of Track Y). A Board's powers apply **only to their assigned track**.

---

## 6. Feature Specifications

### Area A — Public Ticketing & Platform Core

#### 6.1 Authentication & Authorization
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| AUTH-01 | User Registration | Email + password registration. Auto-assigns Attendee role. | P0 |
| AUTH-02 | Login | Email + password login. Returns JWT access + refresh tokens. | P0 |
| AUTH-03 | Logout | Invalidate refresh token. | P0 |
| AUTH-04 | Forgot Password | Send password reset link via email. | P0 |
| AUTH-05 | Reset Password | Reset password using token from email. | P0 |
| AUTH-06 | JWT Authentication | Stateless API authentication via Bearer tokens. | P0 |
| AUTH-07 | Role-Based Authorization | API endpoints protected by global role (Attendee, Admin) plus per-track policies (Member/Board). | P0 |

#### 6.2 User Management
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| USER-01 | View Profile | Users can view their profile information. | P0 |
| USER-02 | Edit Profile | Users can update name, phone, bio. | P0 |
| USER-03 | Upload Profile Picture | Upload to Cloudinary, store URL in DB. | P1 |
| USER-04 | Change Password | Authenticated password change. | P0 |
| USER-05 | Admin: List Users | Paginated list of all users with filters (role, status, search). | P0 |
| USER-06 | Admin: Manage Track Assignments | Assign/remove a user's Member or Board role on a specific track (max 1 Member track + 1 Board track, must differ — R-ROLE-3). | P0 |
| USER-07 | Admin: Deactivate User | Soft-delete / deactivate a user account. | P1 |

#### 6.3 Event Management
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| EVT-01 | Create Event | Admin creates event (title, description, date, location, capacity, **individual-ticket price ≥ 0**, image). | P0 |
| EVT-02 | Update Event | Admin edits event details. | P0 |
| EVT-03 | Delete Event | Admin soft-deletes an event. | P1 |
| EVT-04 | List Events (Public) | Paginated public event listing with filters (upcoming, past). | P0 |
| EVT-05 | Event Details (Public) | Detailed event page with description, date, location, capacity remaining, **ticket price, and any optional packages**. | P0 |
| EVT-06 | Publish/Archive Event | Admin can toggle event visibility. | P1 |

#### 6.4 Ticketing, Orders & QR Codes
> Supersedes the old "one registration per user" model.

| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| ORD-01 | Browse Tickets & Packages | View an event's individual-ticket price and any **optional** packages (Duo/Group-5) with prices. | P0 |
| ORD-02 | Price Quote | Preview base price, promo discount, and final price before paying. | P0 |
| ORD-03 | Reserve Order | Authenticated user reserves individual tickets **or** an optional package; seats held for a short checkout window (exact duration set in the system design). | P0 |
| ORD-04 | Online Payment (Paymob) | Pay via Paymob (cards + wallets, EGP); QR issued on verified webhook. | P0 |
| ORD-05 | Free / Promo Orders | Promo code, 0-price individual ticket, or 0-price package skips the gateway; QR issued immediately. | P0 |
| ORD-06 | Per-Seat QR Tickets | One unique QR ticket per seat; optional guest name per ticket. | P0 |
| ORD-07 | Cancel Order | User cancels before the event; paid-order refunds handled offline. | P1 |
| ORD-08 | Order History | User views past/upcoming orders and their tickets. | P1 |
| ORD-09 | Hold Expiry | Unpaid holds auto-release when the checkout window elapses. | P0 |
| CHK-01 | Scan & Check-in | Admin scans a ticket QR at the door; single-use. | P0 |

#### 6.5 Public Pages (Frontend)
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| PUB-01 | Home Page | Hero section, upcoming events, about section, CTA. | P0 |
| PUB-02 | About Page | TEDxAlkawmia mission, vision, history. | P1 |
| PUB-03 | Team Page | Display organizing team members. | P1 |
| PUB-04 | Events Page | List of upcoming and past events. | P0 |
| PUB-05 | Event Detail Page | Full event information + registration button. | P0 |
| PUB-06 | Contact Page | Contact form or contact information. | P1 |
| PUB-07 | Login Page | Authentication form. | P0 |
| PUB-08 | Register Page | Registration form. | P0 |
| PUB-09 | Forgot Password Page | Password reset request form. | P0 |

#### 6.6 Admin Dashboard (Basic)
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| ADM-01 | Dashboard Overview | Summary cards (total users, events, registrations). | P0 |
| ADM-02 | User Management Page | CRUD users, role assignment, search/filter. | P0 |
| ADM-03 | Event Management Page | CRUD events, view registrations per event. | P0 |

---

### Area B — Internal Training & Evaluation

#### 6.7 Track Management
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| TRK-01 | Create Track | Admin creates a training track (name, description, schedule). | P0 |
| TRK-02 | Assign Board to Track | Admin assigns Board members to supervise a track. | P0 |
| TRK-03 | Add Members to Track | Admin adds members to any track; a Board adds members to **their own track only**. Enrolling a member = creating their single Member assignment (R-ROLE-3). | P0 |
| TRK-04 | Remove Members from Track | Admin removes from any track; a Board removes from **their own track only**. | P1 |
| TRK-05 | View Track Details | Track info, members list, sessions, progress. | P0 |

#### 6.8 Session Management
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| SES-01 | Create Session | Board/Admin creates a session within a track (topic, date, time, location). | P0 |
| SES-02 | Update Session | Edit session details. | P0 |
| SES-03 | Delete Session | Remove a session. | P1 |
| SES-04 | View Sessions | Members view upcoming and past sessions in their track. | P0 |

#### 6.9 Attendance Management
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| ATT-01 | Record Attendance (Manual) | Board marks members as Present, Late, or Absent. Training attendance is **manual only** (no QR); QR tokens exist solely for event tickets. | P0 |
| ATT-03 | View Attendance (Member) | Members view their own attendance percentage. | P0 |
| ATT-04 | View Attendance (Board) | Board views attendance for their track. | P0 |
| ATT-05 | Attendance Report (Admin) | Admin views attendance across all tracks. | P1 |

#### 6.10 Evaluation Management
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| EVL-01 | Create Evaluation | Board evaluates a member after a session (score, notes). | P0 |
| EVL-02 | Edit Evaluation | Board edits an existing evaluation. | P0 |
| EVL-03 | View Evaluations (Member) | Member views their evaluation history. | P0 |
| EVL-04 | View Evaluations (Board) | Board views evaluations for all members in their track. | P0 |
| EVL-05 | Add Feedback | Board adds textual feedback to an evaluation. | P0 |

#### 6.11 Member Dashboard
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| MDB-01 | Dashboard Overview | Summary of attendance %, latest evaluations, upcoming sessions. | P0 |
| MDB-02 | Sessions View | List of upcoming and past sessions. | P0 |
| MDB-03 | Evaluations View | Evaluation history with scores and feedback. | P0 |
| MDB-04 | Attendance View | Detailed attendance log with percentage. | P0 |

#### 6.12 Board Dashboard
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| BDB-01 | Dashboard Overview | Summary of assigned tracks, member count, attendance averages. | P0 |
| BDB-02 | Track Members View | List of members with attendance and evaluation summaries. | P0 |
| BDB-03 | Session Management | Create, edit, view sessions. | P0 |
| BDB-04 | Attendance Recording | Mark attendance for a session. | P0 |
| BDB-05 | Evaluation Entry | Evaluate members after sessions. | P0 |

#### 6.13 In-App Notifications
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| NTF-01 | Send Notification (Admin) | Admin sends platform-wide or role-based notifications. | P0 |
| NTF-02 | Send Notification (Board) | Board sends notifications to their track members. | P0 |
| NTF-03 | View Notifications | Users view their notification inbox. | P0 |
| NTF-04 | Mark as Read | Users mark notifications as read. | P1 |

#### 6.14 Caching Layer
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| CACHE-01 | Redis Integration | Add Redis for distributed caching. | P1 |
| CACHE-02 | Cache Event Data | Cache public event listings. | P1 |
| CACHE-03 | Rate Limiting | API rate limiting via Redis. | P2 |

---

### Area C — Advanced Payments, Analytics & Enhancements

> **Note:** Core **online payment (Paymob)** is part of the ticketing model (Area A), because a ticket's QR is issued only after a confirmed payment. This area covers only the **advanced** money features below.

#### 6.15 Advanced Payment Features
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| PAY-01 | Automated Gateway Refunds | Refund via Paymob API (replaces manual/offline refunds). | P1 |
| PAY-02 | Additional Payment Channels | Installments, additional wallets, saved cards. | P2 |
| PAY-03 | Financial Reconciliation | Match gateway settlements against orders. | P1 |
| PAY-04 | Payment History & Receipts | Downloadable receipts/invoices per order. | P1 |

> Base online payment (Paymob checkout, HMAC webhook, QR-on-paid, promo codes, free orders) is specified in the ticketing feature set above — **not here**.

#### 6.16 Reports & Analytics
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| RPT-01 | Event Reports | Registration counts, attendance rates per event. | P0 |
| RPT-02 | Track Reports | Member progress, attendance, evaluation averages. | P0 |
| RPT-03 | Financial Reports | Revenue per event, payment summaries. | P1 |
| RPT-04 | Export to CSV/PDF | Export reports in downloadable formats. | P1 |

#### 6.17 Advanced Notifications
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| ANTF-01 | Real-Time Notifications | SignalR-based push notifications. | P1 |
| ANTF-02 | Email Notifications | SMTP/SendGrid integration for critical notifications. | P1 |
| ANTF-03 | Session Reminders | Automated reminders before sessions. | P2 |
| ANTF-04 | Event Reminders | Automated reminders before events. | P2 |

#### 6.18 Mobile App (Future)
| ID | Feature | Description | Priority |
|----|---------|-------------|----------|
| MOB-01 | Mobile App | React Native or Flutter app consuming the same REST API. | P2 |
| MOB-02 | Push Notifications | Mobile push notifications via Firebase. | P2 |

---

## 7. User Flows

> **Note:** the summaries below are the scope-level view of each actor's journey. The detailed step-by-step flows live in **[[03-UserFlows|03 — User Flows]]**, which is authoritative for user-facing behavior.

### 7.1 Attendee Flow (booking)
```
Register → Login → Browse Events → Select Event → Choose Tickets (individual or optional package) (+ optional promo)
→ Reserve (seats held, 15-min checkout window) → Pay on Paymob
→ (webhook confirms) → Receive 1 QR per seat → Attend (each QR scanned at entry)
```

### 7.2 Member Flow
```
Login → Dashboard → View Upcoming Sessions → Attend Session
→ Board Records Attendance → Board Submits Evaluation
→ Member Views Evaluation & Attendance % → Register for Events
```

### 7.3 Board Flow
```
Login → Dashboard → Select Assigned Track → View Members
→ Create/Manage Sessions → Record Attendance → Evaluate Members
→ Send Notifications to Track
```

### 7.4 Admin Flow
```
Login → Dashboard → Manage Users (assign roles)
→ Manage Events (CRUD) → Manage Packages & Promo Codes → View Orders/Tickets
→ Manage Tracks (assign Board) → View Reports → Configure System
```

---

## 8. Success Metrics

| Metric | Target | Measurement |
|--------|--------|-------------|
| Event registration completion rate | > 80% | Registrations / Event page views |
| QR code scan success rate | > 95% | Successful scans / Total attendees |
| Member attendance tracking adoption | 100% of tracks | Tracks using digital attendance |
| Admin time saved on manual tasks | > 50% reduction | Pre/post comparison survey |
| Platform uptime | > 99.5% | Monitoring tools |
| Page load time (public pages) | < 2 seconds | Lighthouse / monitoring |

---

## 9. Constraints & Assumptions

### Constraints
- **Budget:** Minimize paid third-party services early (Cloudinary free tier; Paymob is transaction-based).
- **Team:** 2 backend, 2 frontend, 1 UI/UX designer.
- **Scale:** Under 100 concurrent users initially.
- **Language:** English UI, but **i18n-ready** — no hardcoded user-facing strings, locale-aware dates/currency, so Arabic/RTL can be added later without a rewrite.
- **Payments:** online via **Paymob** (needs a merchant account); this is core, not deferred.

### Assumptions
- Users have access to modern web browsers (Chrome, Firefox, Safari, Edge).
- Users have smartphones capable of displaying QR codes.
- The TEDxAlkawmia team will provide content (event details, team bios, about text).
- The Admin user will be seeded in the database during initial deployment.
- Email service (for password reset) will use a basic SMTP provider.

---

## 10. Out of Scope (entire product)

These are not planned for any current area of work. (What ships *when* — the MVP cut and release slices — is decided in the separate planning document, deliberately written last.)

- Mobile application (the API is built client-agnostic so one can be added later)
- Social login (Google, Facebook)
- Automated gateway refunds (refunds are manual/offline for now)
- SMS notifications
- Public-facing analytics

---

## 11. Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| Scope creep across capability areas | High | Clear area boundaries, PRD sign-off before starting each releasable slice |
| QR code scanning failures | Medium | Server-generated codes with unique tokens, fallback manual entry |
| Security vulnerabilities | High | JWT best practices, input validation, HTTPS, CORS policy |
| Team unfamiliar with Clean Architecture | Medium | Documentation, code reviews, pair programming |
| Cloudinary free tier limits | Low | Monitor usage, upgrade plan if needed |

---

## 12. Release Plan

> **Deferred to a dedicated planning document.** Phasing, the MVP cut, milestones, and timeline are decided in a later planning pass — deliberately after the business and system models are settled. This PRD defines *what* the platform does (organized by capability area), not *when* each slice ships. The one fixed sequencing fact: **Identity + Public Ticketing + Check-in form one coherent releasable slice** (a ticket can't be checked in if payment never issued it); the internal Training area builds on top of that foundation.

---

## Appendix A: Glossary

| Term | Definition |
|------|------------|
| **Visitor** | An unauthenticated browser — can view public pages only. |
| **Attendee** | A registered user (renamed from "Guest"); can buy tickets (individual or packages) and attend events. |
| **Track** | A training program/course within TEDxAlkawmia (e.g., Public Speaking, Content Creation). |
| **Session** | A single meeting/class within a Track. |
| **Member** | A user enrolled in **exactly one** training track (per-track role). |
| **Board** | A user supervising **exactly one** training track (per-track role); may also be a Member of a *different* track at the same time. |
| **Evaluation** | A score and feedback given by a Board member to a Member after a session. |
| **Event** | A public TEDx event that sells tickets — **individual tickets at a face price**, plus optional package bundles. |
| **Ticket Package** | An **optional** purchasable bundle of seats for an event (e.g., Duo, Group-5), each with its own price. Packages are discount offers on top of individual tickets, **not** the only way to buy; an event may have none. |
| **Order** | A purchase for an event — **either individual tickets or one package** (one unit-type × quantity) — by an Attendee; holds seats, tracks payment, and produces tickets once paid. |
| **Ticket** | A single admittance credential for one seat, carrying its own unique QR code and optional guest name. |
| **Promo Code** | An Admin-defined discount applied to an order's price; may reduce the total to zero (free). |
| **QR Code** | A machine-readable code generated **per ticket (one per seat)** for door check-in; issued only after the order is paid. |
| **Check-in** | The act of scanning a ticket's QR at the venue to admit the holder; each ticket is single-use. |
