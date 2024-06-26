
# Project Introduction

## Contents
- [Project Description](#project-description)
- [Illustrative Images](#illustrative-images)
- [Major Features (Epics)](#major-features-epics)
- [Technologies Used](#technologies-used)
- [Team Member Assignment Table](#team-member-assignment-table)
  - [Table 1: User Stories of Each Sprint](#table-1-user-stories-of-each-sprint)
  - [Table 2: Member Assignment for Sprint 1](#table-2-member-assignment-for-sprint-1)

## Project Description
The software is designed to manage jewelry sales for a company operating a single store with multiple counters. It handles order creation, invoicing, and warranty slip printing. Products can be inputted via barcode scanning or direct code entry. The software supports pricing calculations based on gold price, labor costs, and stone prices, along with promotional management and customer-specific discounts.

## Major Features (Epics)
- **Manage Account**: Includes functionalities like login, logout, password reset, create account, edit account, delete account, search account, grant permissions.
- **Manage Profile**: View and update profile.
- **Manage Staff**: Add, edit, delete staff members and assign task.
- **Booking**: View, create, update, and cancel, evaluate service.
- **Manage Service**: View, add, edit and delete service.
- **Manage Combo**: Create, update, delete, and view combo.
- **Manage Pet**: Add, update, and delete pet.
- **Payment**: Save payment, view payment history, view online transaction history.

## Technologies Used
- **Backend**:
  - Swagger
  - Spring Security
  - Sending Email
  - Sending OTP via mail
  - Spring Data JPA
  - MySQL Database
  - MultipartFile
  - Barbecue (barcode 1D Generator)
  - Metal Price API to update gold price
- **Frontend**:
  - React Js
  - Tailwind
  - Quill React JS

## Team Member Assignment Table

### Table 1: User Stories of Each Sprint
| Sprint   | User Story ID | Description                |
|----------|----------------|----------------------------|
| Sprint 1 | UC-01    | Login                      |
|          | UC-02    | Logout                     |
|          | UC-03    | Reset Password             |
|          | UC-04    | Change Password            |
|          | UC-05    | Register Account           |
|          | UC-06    | Add Pet                    |
|          | UC-07    | Edit Pet                   |
|          | UC-08    | Delete Pet                 |
| Sprint 2 | UC-09    | View Service               |
|          | UC-10    | Add Service                |
|          | UC-11    | Edit Service               |
|          | UC-12    | Delete Service             |
|          | UC-13    | Update Profile             |
|          | UC-14    | View Profile               |
|          | UC-15    | Add Staff                  |
|          | UC-16    | Edit Staff                 |
|          | UC-17    | Delete Staff               |
| Sprint 3 | UC-18    | Search Account             |
|          | UC-19    | Add Account                |
|          | UC-20    | Edit Account               |
|          | UC-21    | Delete Account             |
|          | UC-22    | Add Combo                  |
|          | UC-23    | Edit Combo                 |
|          | UC-24    | Delete Combo               |
|          | UC-25    | View Combo                 |
|          | UC-26    | Grant Permissions          |
| Sprint 4 | UC-27    | Create Appointment         |
|          | UC-28    | Cancel Appointment         |
|          | UC-29    | Update Appointment         |
|          | UC-30    | View Booking history       |
|          | UC-31    | Evaluate The Service       |
|          | UC-32    | Staff Dashboard            |
|          | UC-33    | Save Payment               |
|          | UC-34    | View Payment History       |
|          | UC-35    | View Transaction History   |
|          | UC-36    | Assign Task                |

### Table 2: Member Assignment for Sprint 1
| Member Name | User Story ID | Description            |
|-------------|---------------|------------------------|
| Ha Huy Hoang     | UC-01   | Login                  |
| Ha Huy Hoang     | UC-04   | Change Password        |
| Tran Phan Phuc Nguyen    | UC-06    | Add Pet                    |
| Tran Phan Phuc Nguyen    | UC-07    | Edit Pet                   |
| Nguyen Ba Minh Duc       | UC-05    | Register Account           |
| Nguyen Ba Minh Duc       | UC-03    | Reset Password             |
| Pham Tien Dat   | UC-02    | Logout                     |
| Pham Tien Dat   | UC-08    | Delete Pet                 |

### Table 3: Member Assignment for Sprint 2
| Member Name | User Story ID | Description            |
|-------------|---------------|------------------------|
| Ha Huy Hoang     | UC-73    | Update Profile             |
| Ha Huy Hoang     | UC-74    | View Profile               |
| Ha Huy Hoang     | UC-77    | Delete Staff               |
| Tran Phan Phuc Nguyen    | UC-78    | View Service               |
| Tran Phan Phuc Nguyen    | UC-79    | Add Service                |
| Nguyen Ba Minh Duc       | UC-80    | Edit Service               |
| Nguyen Ba Minh Duc       | UC-81    | Delete Service             |
| Pham Tien Dat    | UC-75    | Add Staff                  |
| Pham Tien Dat    | UC-76    | Edit Staff                 |
