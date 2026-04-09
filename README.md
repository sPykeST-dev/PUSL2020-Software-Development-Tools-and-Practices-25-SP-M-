# BlindMatch — Project Approval System
### PUSL2020 Software Development Tools and Practices | 2026–2027

A secure, web-based **Blind-Match Project Approval System (PAS)** that facilitates fair matching of student research projects with academic supervisors.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET 9) |
| Web Framework | ASP.NET Core MVC |
| ORM | Entity Framework Core + Migrations |
| Database | SQL Server (LocalDB for development) |
| Auth | ASP.NET Core Identity + RBAC |
| Testing | xUnit, Moq, EF Core InMemory |

---

## Architecture

```
src/
  BlindMatch.Core/           # Domain entities, enums, interfaces — no external deps
  BlindMatch.Infrastructure/ # EF Core, repositories, services, Identity
  BlindMatch.Web/            # ASP.NET Core MVC controllers, Razor views, ViewModels
tests/
  BlindMatch.Tests/          # Unit, integration, and functional tests
```

---

## User Roles

| Role | Responsibilities |
|---|---|
| **Student** | Submit proposals, track status, view supervisor after reveal |
| **Supervisor** | Browse anonymous proposals, express & confirm interest |
| **Module Leader** | Oversight dashboard, approve/reject matches, manage research areas |
| **System Administrator** | User management, RBAC configuration, audit log |

---

## Blind-Match Flow

1. Student submits a proposal (title, abstract, tech stack, research area)
2. Supervisor browses **anonymous** proposals — no student name/ID visible
3. Supervisor expresses interest in a proposal
4. Supervisor **confirms** interest → system triggers **Identity Reveal**
5. Both parties can now see each other's contact details
6. Module Leader reviews and approves the final match

---

## Setup

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server LocalDB

### Run locally
```bash
# Restore dependencies
dotnet restore

# Apply EF Core migrations
dotnet ef database update --project src/BlindMatch.Infrastructure --startup-project src/BlindMatch.Web

# Run the web app
dotnet run --project src/BlindMatch.Web
```

### Run tests
```bash
dotnet test
```

---

## Group Members

| Name | Role |
|---|---|
| TBA | TBA |
