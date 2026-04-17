# BlindMatch — Project Approval System
### PUSL2020 Software Development Tools and Practices

A secure, web-based **Blind-Match Project Approval System (PAS)** that facilitates fair matching of student research projects with academic supervisors.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET 9) |
| Web Framework | ASP.NET Core MVC |
| ORM | Entity Framework Core + Migrations |
| Database | SQL Server (Docker) |
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
4. Supervisor **confirms** interest → a pending match is created
5. Module Leader reviews and **approves** the match
6. Identity reveal occurs — both parties can now see each other's contact details

---

## Setup

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run with Docker

The application ships with two Docker profiles backed by separate database volumes.

**Clean profile** — admin account only, blank database:
```bash
docker compose up --build -d
```

**Demo profile** — pre-populated with accounts, proposals, matches, and audit history:
```bash
docker compose -f docker-compose.yml -f docker-compose.demo.yml up --build -d
```

The app runs at **http://localhost:8080**.

Migrations and seeding run automatically on startup. To switch profiles, bring down the current one first:
```bash
docker compose down
# or, if running demo:
docker compose -f docker-compose.yml -f docker-compose.demo.yml down
```

### Demo accounts

| Role | Name | Email | Password |
|---|---|---|---|
| System Administrator | System Administrator | admin@blindmatch.ac.lk | Admin@BlindMatch1 |
| Module Leader | Tommy Dwight | tommy.dwight@nsbm.ac.lk | Password123! |
| Module Leader | Sarah Mitchell | sarah.mitchell@nsbm.ac.lk | Password123! |
| Module Leader | Robert Chen | robert.chen@nsbm.ac.lk | Password123! |
| Supervisor | John Doe | john.doe@nsbm.ac.lk | Password123! |
| Supervisor | Emma Williams | emma.williams@nsbm.ac.lk | Password123! |
| Supervisor | David Kumar | david.kumar@nsbm.ac.lk | Password123! |
| Student | Alex Smith | alex.smith@student.nsbm.ac.lk | Password123! |
| Student | Jamie Lee | jamie.lee@student.nsbm.ac.lk | Password123! |
| Student | Priya Patel | priya.patel@student.nsbm.ac.lk | Password123! |

### Demo data state

| Student | Proposal | Status |
|---|---|---|
| Alex Smith | Real-Time Predictive Analytics Dashboard Using ML | Matched — approved by Tommy Dwight |
| Priya Patel | Conversational AI Assistant for University Academic Support | Matched — pending Module Leader approval |
| Jamie Lee | Blockchain-Based Decentralised Identity Verification System | Submitted — 2 supervisors with pending interest |

### Run locally (without Docker)
```bash
dotnet restore
dotnet ef database update --project src/BlindMatch.Infrastructure --startup-project src/BlindMatch.Web
dotnet run --project src/BlindMatch.Web
```

### Run tests
```bash
dotnet test
```

