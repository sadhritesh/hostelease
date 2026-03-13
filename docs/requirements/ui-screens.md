
# UI Screens Documentation

## Overview

The Hostel Management System provides different interfaces based on user roles. Each role has access only to the features necessary for their responsibilities.

### System Roles

* Admin
* Manager
* Student

---

# 1. Common Screens

These screens are accessible to all users.

---

## 1.1 Login Screen

### Purpose

Authenticate users before allowing access to the system.

### Fields

* Email
* Password

### Actions

* Login

### Validations

* Email must exist in system
* Password must match stored password

### Outcome

System verifies credentials and redirects user to their dashboard based on role.

---

## 1.2 Dashboard Screen

### Purpose

Display key summary data and quick access to important modules.

### Dashboard Widgets (Example)

* Total Students
* Total Rooms
* Available Rooms
* Pending Fees
* Unsettled Deposits

The content displayed will vary depending on the user role.

---

# 2. Admin Screens

Admin manages hostels and manager accounts.

---

# 2.1 Admin Dashboard

### Displays

* Total Hostels
* Total Managers
* Total Students
* Total Rooms

### Quick Actions

* Add Hostel
* Add Manager

---

# 2.2 Hostel Management Screen

### Purpose

Allows admin to manage hostels in the system.

### Table Columns

* Hostel Name
* Address
* City
* Contact Number
* Status

### Actions

* Add Hostel
* Edit Hostel
* Activate / Deactivate Hostel

---

# 2.3 Create / Edit Hostel Screen

### Fields

* Hostel Name
* Address
* City
* State
* Contact Number
* Status

### Actions

* Save
* Cancel

---

# 2.4 Manager Management Screen

### Purpose

Allows admin to create and manage manager accounts.

### Table Columns

* Manager Name
* Email
* Phone
* Assigned Hostel
* Status

### Actions

* Add Manager
* Edit Manager
* Activate / Deactivate Manager

---

# 2.5 Create / Edit Manager Screen

### Fields

* Name
* Email
* Phone
* Password
* Assigned Hostel
* Status

### Actions

* Save
* Cancel

---

# 3. Manager Screens

Managers handle daily hostel operations.

---

# 3.1 Manager Dashboard

### Displays

* Total Students
* Total Rooms
* Available Rooms
* Pending Fees
* Unsettled Deposits

---

# 3.2 Room Type Management Screen

### Purpose

Define room categories and rent prices.

### Table Columns

* Room Type Name
* Monthly Rent
* Status

### Actions

* Add Room Type
* Edit Room Type
* Activate / Deactivate Room Type

---

# 3.3 Create / Edit Room Type Screen

### Fields

* Room Type Name
* Monthly Rent
* Status

### Actions

* Save
* Cancel

---

# 3.4 Room Management Screen

### Purpose

Manage rooms inside the hostel.

### Table Columns

* Room Number
* Room Type
* Capacity
* Current Occupancy
* Status

### Actions

* Add Room
* Edit Room
* Deactivate Room

---

# 3.5 Create / Edit Room Screen

### Fields

* Hostel
* Room Number
* Room Type
* Capacity
* Status

### Actions

* Save
* Cancel

---

# 3.6 Student Management Screen

### Purpose

Manage student records.

### Table Columns

* Student Name
* Phone
* Hostel
* Room
* Admission Date
* Status

### Actions

* Add Student
* Edit Student
* View Student
* Checkout Student

---

# 3.7 Create / Edit Student Screen

### Fields

* Name
* Phone
* Email
* Course / College
* Hostel
* Admission Date
* Status

### Actions

* Save
* Cancel

---

# 3.8 Room Allocation Screen

### Purpose

Assign a room to a student.

### Fields

* Select Student
* Select Room
* Allocation Date

### Validations

* Room must have available capacity
* Student must not have active room allocation

### Actions

* Allocate Room

---

# 3.9 Fee Management Screen

### Purpose

View monthly billing records.

### Table Columns

* Student Name
* Billing Month
* Fee Amount
* Paid Amount
* Balance Amount
* Status

### Filters

* Month
* Payment Status

---

# 3.10 Payment Recording Screen

### Purpose

Record student fee payments.

### Fields

* Student
* Fee Month
* Payment Amount
* Payment Mode (Cash / UPI / Bank Transfer)
* Payment Date
* Reference Number (optional)

### Actions

* Record Payment

---

# 3.11 Security Deposit Screen

### Purpose

View and manage student security deposits.

### Table Columns

* Student Name
* Deposit Amount
* Paid Date
* Status

### Actions

* View Deposit
* Settle Deposit

---

# 3.12 Deposit Settlement Screen

### Purpose

Process deposit settlement when student leaves.

### Fields

* Student Name
* Deposit Amount
* Deducted Amount
* Deduction Reason

### System Calculation

Returned Amount = Deposit Amount − Deducted Amount

### Actions

* Complete Settlement

---

# 3.13 Student Checkout Screen

### Purpose

Complete the student exit process.

### Fields

* Student Name
* Checkout Date
* Deduction Amount
* Deduction Reason

### System Actions

When checkout is completed:

1. Room allocation is closed
2. Room occupancy is updated
3. Deposit settlement is recorded
4. Student status becomes inactive

### Actions

* Complete Checkout

---

# 4. Student Screens

Students can view their own information and payment status.

---

# 4.1 Student Dashboard

### Displays

* Assigned Room
* Current Month Fee Status
* Security Deposit Status
* Recent Payments

---

# 4.2 My Room Screen

### Displays

* Room Number
* Room Type
* Hostel Name
* Allocation Date

---

# 4.3 My Fees Screen

### Displays

| Month | Amount | Paid | Status  |
| ----- | ------ | ---- | ------- |
| April | 5000   | 5000 | Paid    |
| May   | 5000   | 0    | Pending |

---

# 4.4 My Deposit Screen

### Displays

* Deposit Amount
* Deposit Paid Date
* Deposit Status
* Settlement Details (if checkout completed)

---

# Navigation Structure

### Admin Menu

* Dashboard
* Hostel Management
* Manager Management

---

### Manager Menu

* Dashboard
* Room Types
* Rooms
* Students
* Room Allocation
* Fees
* Payments
* Security Deposits
* Checkout

---

### Student Menu

* Dashboard
* My Room
* My Fees
* My Deposit

