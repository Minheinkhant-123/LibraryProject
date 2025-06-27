# 📚 Library Management System

A full-stack web-based Library Management System built using **ASP.NET Core MVC**, **Entity Framework Core**, **SQL Server**, and **JWT Token-based Authentication**. This application allows library staff to manage books, members, loans, and run various reports securely.

---

## 🚀 Features

- 🔐 **JWT Authentication** (no cookies)
- 📚 Book Management (CRUD, authors, genres, soft delete)
- 👤 Member Management (CRUD, status tracking, soft delete)
- 🔁 Book Loans with:
  - Due date tracking
  - Loan limit enforcement (5 max)
  - Late fee calculation (€0.50/day)
  - Renewal support (if no reservation)
- 📊 Reports:
  - Books currently on loan
  - Overdue books
  - Popular books (most borrowed)
  - Active member statistics
- 🧪 Unit Testing with `xUnit` and InMemory EF DB
- 🧱 Clean architecture (Core, Infrastructure, Web layers)

---

## 🧰 Requirements

- .NET 8 SDK
- SQL Server or LocalDB
- Visual Studio 2022+ or VS Code

---

## 🛠️ Project Structure

```bash
LibraryProject.sln
│
├── LibraryProject.Web           # ASP.NET Core MVC app
├── LibraryProject.Core          # Models and interfaces (domain layer)
├── LibraryProject.Infrastructure# EF Core DbContext and repositories
├── LibraryProject.Tests         # Unit tests using xUnit

🧪 Setup Instructions
1. Clone the Project

git clone https://github.com/your-username/LibraryProject.git
cd LibraryProject

2. Update DB Connection

Open appsettings.json in LibraryProject.Web and update the connection string:

"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LibraryDB;Trusted_Connection=True;"
}
🔐 Default Login Credentials

For testing and initial use, you can log in with the following default user accounts:
Role	Username	Password
Admin	admin	adminpass
Librarian	librarian	libpass