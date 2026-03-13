# 📘 Hostel Management System (HMS) — Requirement Document (Final)

**Version:** 1.0 
**Date:** 12-Mar-2026  
**Owner:** Ritesh (Developer)  
**Status:** MVP scope finalized ✅

***

## 1) Purpose & Goals

Digitize hostel operations: **students**, **rooms**, **room allocation**, **fees & payments**, and **security deposit**.  
Primary goals:

*   Enforce **capacity** and **allocation** rules
*   Automate **monthly fee generation**
*   Maintain **payment** & **deposit** history with auditability
*   Support **multi-hostel** management with role-based access
*   Keep the design **scalable & maintainable**

***

## 2) Actors (Users)

1.  **Admin (Owner)**
    *   Manages hostels, assigns managers
2.  **Manager**
    *   Operates **one or more hostels** (via mapping)
    *   Handles rooms, students, allocations, fees, payments, deposits
3.  **Student**
    *   Views own profile, room, fees, and deposit status

***

## 3) Scope (MVP)

**Included (Phase‑1):**

*   Authentication & Role-based Authorization
*   Hostels, Manager–Hostel assignment (many-to-many)
*   RoomTypes (pricing), Rooms (capacity)
*   Students
*   Room Allocation (with history)
*   Fees (monthly, system-generated)
*   Payments (partial/full)
*   **Security Deposit + Settlement**
*   Daily background job for fee generation
*   Basic dashboards/lists

**Excluded (Phase‑2):**

*   Online payment gateway
*   Late fee/fines, discounts, refunds
*   Complaints, Attendance, Visitors
*   Reports/Analytics, PDF receipts
*   Pro‑rata billing on mid‑month room change

***

## 4) High-Level Features

### 4.1 Authentication & Authorization

*   Secure login/logout
*   Roles: **Admin**, **Manager**, **Student**
*   Managers can access **only assigned hostels**
*   Students access **self-only** data

### 4.2 Hostel Management (Admin)

*   Create/update/deactivate hostels
*   Assign/unassign managers (mapping table with IsActive, AssignedDate)
*   View hostels & their managers

### 4.3 Room Type Management (Admin/Manager)

*   Define room types (e.g., “2 Sharing”, “3 Sharing”, “AC”, “Non-AC”)
*   Set **BaseMonthlyFee** at RoomType level
*   Activate/deactivate room types

### 4.4 Room Management (Manager)

*   Create/update/deactivate rooms per hostel
*   Fields: RoomNumber (unique per hostel), Capacity, RoomTypeId
*   System maintains **CurrentOccupancy** (allocation-driven)

### 4.5 Student Management (Manager)

*   Create/update/deactivate students (linked to a User)
*   Assign students to a hostel at creation
*   View details, allocations, fees, deposit

### 4.6 Room Allocation (Manager)

*   Allocate room → enforce capacity & uniqueness
*   Vacate/change room → close current allocation, open new
*   Keep **history** via `FromDate/ToDate/IsActive`
*   **Transactional** update: allocation + occupancy change

### 4.7 Fees (System + Manager)

*   **Daily background job** generates **monthly Fee** per active student
*   Fee snapshot: StudentId, Month (YYYY‑MM), Amount (from current RoomType), Status, DueDate
*   Manual “Generate Fees” trigger per hostel (optional)
*   Status auto-updates based on payments

### 4.8 Payments (Manager)

*   Record payments against a Fee (amount, date, mode, reference)
*   Support **partial** and **full** payments
*   Update Fee status: `Pending` / `PartiallyPaid` / `Paid`

### 4.9 **Security Deposit** (NEW)

*   Collect **one-time** deposit at **check-in**
*   Store `DepositAmount`, `PaidDate`, `Status=Paid`
*   At **checkout**, perform **Settlement**:
    *   Record `DeductedAmount`, `Reason`, `ReturnedAmount`, `SettlementDate`
    *   Update `SecurityDeposit.Status = Settled`
*   Prevent multiple settlements; enforce audit trail

### 4.10 Dashboards/Lists

*   Manager: Pending fees, occupancy, active allocations, unsettled deposits
*   Student: My profile, my room, my fee status, my deposit status

***

## 5) Business Rules (Critical)

### 5.1 Allocation Rules

*   Room **cannot exceed capacity**
*   Student has **only one active allocation**
*   Student & Room must be within the **same hostel**
*   Room change = **close old allocation** + **create new**
*   Use **transactions** for allocation and occupancy updates

### 5.2 Fee Rules

*   **Fee = Due** (monthly); independent of Payment
*   Fee **amount** = snapshot of **RoomType.BaseMonthlyFee** at generation time
*   Fee month must be **unique per student**
*   Room change after fee generation **does not** alter past fee
*   Status logic:
    *   `Pending` (no payments)
    *   `PartiallyPaid` (sum < amount)
    *   `Paid` (sum == amount)
*   Paid fees cannot be deleted; use soft-delete/lock policy

### 5.3 Payment Rules

*   `AmountPaid > 0`
*   Payments **cannot exceed** remaining due
*   Maintain immutable **payment history**
*   Record: mode (Cash/UPI/Bank), reference no., timestamp, performed by

### 5.4 Manager–Hostel Rules

*   A **manager can manage multiple hostels**
*   A **hostel can have multiple managers**
*   Assignment tracked via mapping with IsActive, AssignedDate

### 5.5 **Security Deposit Rules**

*   **Mandatory at check-in** (before or along with first allocation)
*   Only **one active** deposit per student
*   **Settlement** allowed **once** at checkout
*   `DeductedAmount <= DepositAmount`
*   `ReturnedAmount = DepositAmount - DeductedAmount`
*   Checkout requires:
    *   Active allocation closed (room vacated)
    *   Deposit settled
    *   (Optional future) No pending dues

***

## 6) Background Job (Daily Fee Generation)

**Schedule:** Once daily (e.g., 02:00 AM server time)

**Flow (idempotent):**

1.  For each **active student**:
    *   Determine latest **generated fee month**
    *   If a **new billing month** is due:
        *   Find **active room allocation**
        *   Fetch **RoomType.BaseMonthlyFee**
        *   Create **Fee**: (StudentId, Month, Amount, Status=Pending, DueDate)
2.  If (StudentId, Month) already exists → **skip**
3.  Log stats (created/skipped/errors); retry on next run

**Tech (MVP):** `BackgroundService`  
**Future:** Hangfire/Quartz.NET with dashboard, retries, cron

**Billing policy (simple & clear):**

*   Generate **calendar-month** fee on the **1st** for students with an **active allocation** (or active status) at that time.

***

## 7) Data Model Overview (Conceptual)

*   **User**: Auth (Role, Email, PasswordHash, IsActive)
*   **Hostel**: (OwnerId → User)
*   **HostelManager**: (HostelId, ManagerUserId, AssignedDate, IsActive)
*   **RoomType**: (Name, BaseMonthlyFee, IsAC?, metadata)
*   **Room**: (HostelId, RoomNumber \[unique per hostel], Capacity, RoomTypeId, CurrentOccupancy)
*   **Student**: (UserId, HostelId, AdmissionDate, Course, IsActive)
*   **RoomAllocation**: (StudentId, RoomId, FromDate, ToDate?, IsActive)
*   **Fee**: (StudentId, Month YYYY‑MM, Amount, Status, DueDate)
*   **Payment**: (FeeId, AmountPaid, PaymentDate, Mode, ReferenceNo)
*   **SecurityDeposit**: (StudentId, DepositAmount, PaidDate, Status: Paid/Settled)
*   **SecurityDepositSettlement**: (SecurityDepositId, DeductedAmount, Reason, ReturnedAmount, SettlementDate)

***

## 8) Role Matrix (MVP)

| Feature                           | Admin  | Manager              | Student  |
| --------------------------------- | ------ | -------------------- | -------- |
| Login                             | ✔      | ✔                    | ✔        |
| Manage Hostels                    | ✔      | ✖                    | ✖        |
| Assign Managers                   | ✔      | ✖                    | ✖        |
| Manage RoomTypes                  | ✔      | ✔ (optional/limited) | ✖        |
| Manage Rooms                      | ✖      | ✔                    | ✖        |
| Manage Students                   | ✖      | ✔                    | ✖        |
| Room Allocation                   | ✖      | ✔                    | ✖        |
| Generate Fees (manual)            | ✔      | ✔                    | ✖        |
| Background Job                    | system | system               | system   |
| Record Payments                   | ✖      | ✔                    | ✖        |
| Security Deposit (collect/settle) | ✖      | ✔                    | ✖        |
| View Own Data                     | ✖      | ✔ (assigned)         | ✔ (self) |

***

## 9) Validations (UI + Server)

*   Required fields, formats, lengths
*   Uniqueness:
    *   `User.Email` (global)
    *   `Room (HostelId, RoomNumber)`
    *   `Fee (StudentId, Month)`
*   Business validations:
    *   Allocation capacity & single active allocation per student
    *   Payment not exceeding remaining due
    *   Deposit: only one active; settlement only once; deduction within deposit
    *   Manager operations restricted to assigned hostels

***

## 10) Non‑Functional Requirements (NFRs)

*   **Security:** Identity/Authorization, anti-forgery tokens, minimal PII in logs
*   **Performance:** Pagination; indexes on FKs & lookups
*   **Auditability:** Immutable histories for Allocation, Payment, Deposit Settlement
*   **Reliability:** Transactions for allocation, payment, and settlement flows
*   **Maintainability:** Layered architecture (Controller → Service → Repository), DTOs/ViewModels
*   **Observability:** Structured logs (Serilog), correlation IDs, error tracing
*   **Time/Locale:** Consistent server timezone for jobs; store UTC timestamps where needed

***

## 11) Constraints & Indexes (DB)

*   **Unique:**
    *   `Room (HostelId, RoomNumber)`
    *   `Fee (StudentId, Month)`
*   **Indexes:**
    *   `RoomAllocation (StudentId, IsActive)`
    *   `RoomAllocation (RoomId, IsActive)`
    *   `Payment (FeeId, PaymentDate)`
    *   `HostelManager (ManagerUserId, IsActive)`
*   **Checks:**
    *   `Room.CurrentOccupancy <= Room.Capacity`
    *   `Payment.AmountPaid > 0`
    *   `SecurityDepositSettlement.DeductedAmount <= SecurityDeposit.DepositAmount`
*   **FK delete behavior:** `RESTRICT` for historical tables (Allocations, Fees, Payments, Deposits)

***

## 12) Acceptance Criteria (Samples)

*   **Allocate Room:** Succeeds only if capacity available and student has no active allocation; increments room occupancy in the same transaction.
*   **Vacate Room:** Sets `ToDate`, `IsActive=false`; decrements occupancy atomically.
*   **Generate Monthly Fees (Job):** On the 1st, creates pending fees with correct amount based on RoomType; skips duplicates.
*   **Record Payment:** Adds a payment and updates Fee status; prevents overpayment.
*   **Security Deposit (Check-in):** Creates a deposit record with amount & date; status `Paid`.
*   **Deposit Settlement (Checkout):** Creates settlement with deduction and returned amount; marks deposit `Settled`; prevents multiple settlements.
*   **Authorization:** Managers see/manage only assigned hostels; students see only their data.

***

## 13) User Stories (MVP)

*   As a **Manager**, I can allocate rooms to students ensuring capacity is not exceeded.
*   As a **System**, I generate monthly fees for all active students automatically.
*   As a **Manager**, I can record partial or full fee payments and see pending balances.
*   As a **Manager**, I collect a security deposit at check-in and settle it at checkout with deductions and refunds.
*   As a **Student**, I can view my room details, fee status, and deposit status.

***

## 14) Risks & Mitigations

*   **Duplicate fee generation by job** → Idempotent logic + unique (StudentId, Month).
*   **Concurrent allocations/payments** → Transaction scopes + DB constraints.
*   **Price changes** → Fee stores snapshot; RoomType affects future fees only.
*   **Timezone confusion** → Fixed server timezone policy; store UTC where appropriate.


