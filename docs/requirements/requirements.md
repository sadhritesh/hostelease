
#  Hostel Management System (HMS) — Requirement Document

* **Version:** 1.0
* **Date:** 13-Mar-2026
* **Owner:** Ritesh (Developer)
* **Status:** MVP Scope Finalized ✅

---

# 1️⃣ Project Overview

The **Hostel Management System (HMS)** is a web application that helps hostel owners manage:

* Students
* Hostels
* Rooms
* Room allocation
* Monthly rent
* Payments
* Security deposits

Instead of maintaining **manual registers or Excel sheets**, all information will be managed through the system.

### Main Goals

* Digitize hostel operations
* Avoid room overbooking
* Automatically generate monthly fees
* Track payments and security deposits
* Support multiple hostels with role-based access

---

# 2️⃣ Stakeholders

Stakeholders are people involved with the system.

| Stakeholder | Role                            |
| ----------- | ------------------------------- |
| Owner       | Runs the hostel business        |
| Manager     | Handles daily hostel operations |
| Student     | Lives in hostel                 |
| Developer   | Builds the system               |
| Tester      | Ensures system works correctly  |

---

# 3️⃣ Actors (Users of System)

The system has **three types of users**.

## Admin (Owner)

Admin manages the overall system.

Responsibilities:

* Create hostels
* Assign managers
* Manage room types

Example:

```
Admin creates:
Hostel A
Hostel B
```

---

## Manager

Manager handles day-to-day hostel operations.

Responsibilities:

* Manage rooms
* Add students
* Allocate rooms
* Collect rent
* Record payments
* Handle security deposits
* Process student checkout

---

## Student

Student can only **view information**.

Student can see:

* Their profile
* Room details
* Monthly fees
* Payment status
* Security deposit status

Students cannot modify any data.

---

# 4️⃣ System Scope (MVP)

MVP means **Minimum Viable Product**.

This is the **first working version** of the system.

## Included in Phase-1

The system will include:

* Login and role-based access
* Hostel management
* Room types and room management
* Student management
* Room allocation
* Monthly fee generation
* Payment recording
* Security deposit handling
* Student checkout
* Daily background job for fees
* Basic dashboards

---

## Excluded (Phase-2)

These features will be added later.

* Online payment gateway
* Late fee penalties
* Discounts or refunds
* Visitor management
* Complaint system
* Reports and analytics
* Pro-rata billing (partial month fee)

---

# 5️⃣ Billing Policy (Important Rule)

The hostel follows **calendar month billing**.

Rules:

* Students must pay the **full month's rent at check-in**, even if they join mid-month.
* Fees are generated **monthly**.
* Past fees are not modified even if room changes.

### Example

Student joins:

```
20 April
Monthly Rent: ₹5000
```

Student pays:

```
April Fee = ₹5000
```

Next:

```
1 May → May Fee generated
```

---

# 6️⃣ Check-In Flow

When a student joins the hostel.

### Manager performs:

```
Create Student
↓
Collect Security Deposit
↓
Allocate Room
↓
Generate Current Month Fee
↓
Collect Monthly Rent
↓
Student Check-In Completed
```

---

# 7️⃣ Checkout Flow

Checkout happens when a student leaves the hostel.

### Manager performs:

```
Select Student
↓
Close Room Allocation
↓
Settle Security Deposit
↓
Mark Student Inactive
↓
Room becomes available
```

### Example

Student leaves on:

```
15 July
```

System updates:

RoomAllocation

```
FromDate: 20 April
ToDate: 15 July
IsActive: false
```

Deposit Settlement

```
DepositAmount: 10000
DeductedAmount: 1000
ReturnedAmount: 9000
Status: Settled
```

Student becomes inactive.

Future fees will **not be generated**.

---

# 8️⃣ Core System Features

---

# Authentication & Authorization

Users must login to access the system.

Roles:

```
Admin
Manager
Student
```

Permissions depend on role.

Example:

Manager can only access **assigned hostels**.

---

# Hostel Management (Admin)

Admin can:

```
Create hostel
Update hostel
Deactivate hostel
Assign managers
```

Example:

| Hostel   | Manager |
| -------- | ------- |
| Hostel A | Amit    |
| Hostel B | Rakesh  |

---

# Room Types

Room types define **pricing categories**.

Example:

| Room Type | Monthly Fee |
| --------- | ----------- |
| 2 Sharing | ₹5000       |
| 3 Sharing | ₹3500       |
| AC Room   | ₹7000       |

---

# Room Management

Managers manage rooms inside hostels.

Room fields:

```
RoomNumber
Capacity
RoomType
CurrentOccupancy
```

Example:

| Room | Capacity |
| ---- | -------- |
| 101  | 2        |
| 102  | 3        |

---

# Student Management

Managers can:

```
Add student
Update student
Deactivate student
```

Example:

```
Student: Rahul
Hostel: A
Course: Engineering
```

---

# Room Allocation

Room allocation assigns a student to a room.

Example:

```
Student: Rahul
Room: 101
FromDate: 20 April
```

If student changes room:

```
Old allocation closed
New allocation created
```

Allocation history remains stored.

---

# Fees (Monthly Rent)

The system generates **monthly fees automatically**.

Example:

```
Student: Rahul
Month: 2026-04
Amount: ₹5000
Status: Pending
```

Fee status:

```
Pending
PartiallyPaid
Paid
```

---

# Payments

Managers record payments against fees.

Example:

| Fee  | Payment |
| ---- | ------- |
| 5000 | 2000    |

Status becomes:

```
PartiallyPaid
```

Payment information stored:

```
Amount
PaymentDate
PaymentMode
ReferenceNumber
```

---

# Security Deposit

Students must pay a **one-time security deposit at check-in**.

Example:

```
DepositAmount: ₹10000
Status: Paid
```

During checkout:

```
DeductedAmount: 2000
ReturnedAmount: 8000
```

Deposit status becomes:

```
Settled
```

---

# 9️⃣ Business Rules

Important rules enforced by the system.

### Room Allocation

* Room cannot exceed capacity
* Student can have only one active room allocation
* Student and room must belong to same hostel
* Room change closes old allocation

---

### Fee Rules

* One fee per student per month
* Fee amount is snapshot of room price
* Past fees are never changed

---

### Payment Rules

* Payment amount must be greater than zero
* Payment cannot exceed remaining fee amount
* Payment history must remain unchanged

---

### Deposit Rules

* Only one active deposit per student
* Deposit settlement allowed only once
* Deduction cannot exceed deposit amount

---

# 🔟 Background Job (Automatic Fee Generation)

System runs a **daily background job**.

Purpose:

Generate monthly fees.

Flow:

```
Run job daily
↓
Check active students
↓
Check if fee exists for current month
↓
If not → create fee
```

Example:

```
1 May → Create May Fee
1 June → Create June Fee
```

Duplicate fees prevented using:

```
Unique(StudentId, Month)
```

---

# 1️⃣1️⃣ Data Model (Conceptual)

Main system entities:

```
User
Hostel
HostelManager
RoomType
Room
Student
RoomAllocation
Fee
Payment
SecurityDeposit
SecurityDepositSettlement
```

Relationship example:

```
Hostel → Rooms
Room → RoomAllocations
Student → Fees
Fee → Payments
```

---

# 1️⃣2️⃣ Role Permissions

| Feature          | Admin | Manager | Student |
| ---------------- | ----- | ------- | ------- |
| Login            | ✔     | ✔       | ✔       |
| Manage Hostels   | ✔     | ✖       | ✖       |
| Assign Managers  | ✔     | ✖       | ✖       |
| Manage Rooms     | ✖     | ✔       | ✖       |
| Manage Students  | ✖     | ✔       | ✖       |
| Allocate Rooms   | ✖     | ✔       | ✖       |
| Record Payments  | ✖     | ✔       | ✖       |
| Security Deposit | ✖     | ✔       | ✖       |
| Checkout Student | ✖     | ✔       | ✖       |
| View Own Data    | ✖     | ✔       | ✔       |

---

# 1️⃣3️⃣ Validations

System must check:

### Uniqueness

```
User.Email
Room (HostelId + RoomNumber)
Fee (StudentId + Month)
```

### Business validations

```
Room capacity must not exceed limit
Payment must not exceed due amount
Deposit settlement allowed only once
```

---

# 1️⃣4️⃣ Non-Functional Requirements

These define system quality.

### Security

* Secure authentication
* Role-based authorization
* Anti-forgery protection

### Performance

* Pagination for large lists
* Database indexes

### Reliability

* Transactions for allocation and payments
* Error handling

### Maintainability

Architecture layers:

```
Controller
↓
Service
↓
Repository
↓
Database
```

---

# 1️⃣5️⃣ Database Constraints

Important database rules.

Unique constraints:

```
Room (HostelId, RoomNumber)
Fee (StudentId, Month)
```

Checks:

```
Room.CurrentOccupancy <= Capacity
Payment.AmountPaid > 0
DeductedAmount <= DepositAmount
```

---

# 1️⃣6️⃣ Acceptance Criteria

Features are correct when:

Example:

Room Allocation:

```
Fails if room capacity exceeded
```

Payment:

```
Fails if payment exceeds remaining fee
```

Deposit Settlement:

```
Allowed only once
```

---

# 1️⃣7️⃣ Risks

Possible system risks.

Examples:

```
Duplicate fee generation
Concurrent room allocation
Incorrect payment entries
Timezone issues in background jobs
```

Mitigation:

```
Unique constraints
Transactions
Idempotent background job logic
```



