using BlindMatch.Core.Common;
using BlindMatch.Core.Entities;
using BlindMatch.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlindMatch.Infrastructure.Data;

public static class DemoDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var db          = services.GetRequiredService<ApplicationDbContext>();

        // Idempotency — bail out if demo data already present
        if (await userManager.FindByEmailAsync("alex.smith@student.nsbm.ac.lk") is not null)
            return;

        const string pwd = "Password123!";

        // ── Module Leaders ───────────────────────────────────────────────────────
        var ml1 = await CreateAsync(userManager, new ApplicationUser
        {
            UserName = "tommy.dwight@nsbm.ac.lk", Email = "tommy.dwight@nsbm.ac.lk",
            FirstName = "Tommy", LastName = "Dwight", IsActive = true, EmailConfirmed = true
        }, pwd, Roles.ModuleLeader);

        var ml2 = await CreateAsync(userManager, new ApplicationUser
        {
            UserName = "sarah.mitchell@nsbm.ac.lk", Email = "sarah.mitchell@nsbm.ac.lk",
            FirstName = "Sarah", LastName = "Mitchell", IsActive = true, EmailConfirmed = true
        }, pwd, Roles.ModuleLeader);

        var ml3 = await CreateAsync(userManager, new ApplicationUser
        {
            UserName = "robert.chen@nsbm.ac.lk", Email = "robert.chen@nsbm.ac.lk",
            FirstName = "Robert", LastName = "Chen", IsActive = true, EmailConfirmed = true
        }, pwd, Roles.ModuleLeader);

        // ── Supervisors ──────────────────────────────────────────────────────────
        var sup1 = new Supervisor
        {
            UserName = "john.doe@nsbm.ac.lk", Email = "john.doe@nsbm.ac.lk",
            FirstName = "John", LastName = "Doe", IsActive = true, EmailConfirmed = true,
            Department = "Software Engineering", MaxProjects = 4, CurrentProjects = 1
        };
        await CreateAsync(userManager, sup1, pwd, Roles.Supervisor);

        var sup2 = new Supervisor
        {
            UserName = "emma.williams@nsbm.ac.lk", Email = "emma.williams@nsbm.ac.lk",
            FirstName = "Emma", LastName = "Williams", IsActive = true, EmailConfirmed = true,
            Department = "Computer Science", MaxProjects = 3, CurrentProjects = 0
        };
        await CreateAsync(userManager, sup2, pwd, Roles.Supervisor);

        var sup3 = new Supervisor
        {
            UserName = "david.kumar@nsbm.ac.lk", Email = "david.kumar@nsbm.ac.lk",
            FirstName = "David", LastName = "Kumar", IsActive = true, EmailConfirmed = true,
            Department = "Cybersecurity", MaxProjects = 3, CurrentProjects = 0
        };
        await CreateAsync(userManager, sup3, pwd, Roles.Supervisor);

        // ── Students ─────────────────────────────────────────────────────────────
        var stu1 = new Student
        {
            UserName = "alex.smith@student.nsbm.ac.lk", Email = "alex.smith@student.nsbm.ac.lk",
            FirstName = "Alex", LastName = "Smith", IsActive = true, EmailConfirmed = true,
            Programme = "BSc Computer Science", YearOfStudy = 3
        };
        await CreateAsync(userManager, stu1, pwd, Roles.Student);

        var stu2 = new Student
        {
            UserName = "jamie.lee@student.nsbm.ac.lk", Email = "jamie.lee@student.nsbm.ac.lk",
            FirstName = "Jamie", LastName = "Lee", IsActive = true, EmailConfirmed = true,
            Programme = "BSc Information Technology", YearOfStudy = 3
        };
        await CreateAsync(userManager, stu2, pwd, Roles.Student);

        var stu3 = new Student
        {
            UserName = "priya.patel@student.nsbm.ac.lk", Email = "priya.patel@student.nsbm.ac.lk",
            FirstName = "Priya", LastName = "Patel", IsActive = true, EmailConfirmed = true,
            Programme = "BSc Computer Science", YearOfStudy = 4
        };
        await CreateAsync(userManager, stu3, pwd, Roles.Student);

        // ── Supervisor research areas ────────────────────────────────────────────
        var areas = await db.ResearchAreas.ToDictionaryAsync(a => a.Id);

        var dbSup1 = await db.Supervisors.Include(s => s.PreferredResearchAreas).FirstAsync(s => s.Email == sup1.Email);
        dbSup1.PreferredResearchAreas.Add(areas[2]); // Web Development
        dbSup1.PreferredResearchAreas.Add(areas[5]); // Machine Learning

        var dbSup2 = await db.Supervisors.Include(s => s.PreferredResearchAreas).FirstAsync(s => s.Email == sup2.Email);
        dbSup2.PreferredResearchAreas.Add(areas[1]); // Artificial Intelligence
        dbSup2.PreferredResearchAreas.Add(areas[5]); // Machine Learning
        dbSup2.PreferredResearchAreas.Add(areas[6]); // Data Science

        var dbSup3 = await db.Supervisors.Include(s => s.PreferredResearchAreas).FirstAsync(s => s.Email == sup3.Email);
        dbSup3.PreferredResearchAreas.Add(areas[3]); // Cybersecurity
        dbSup3.PreferredResearchAreas.Add(areas[7]); // Networking

        await db.SaveChangesAsync();

        // ── Proposals ────────────────────────────────────────────────────────────
        var p1SubmittedAt = new DateTime(2026, 4,  8, 10, 30, 0, DateTimeKind.Utc);
        var p2SubmittedAt = new DateTime(2026, 4,  9,  9, 15, 0, DateTimeKind.Utc);
        var p3SubmittedAt = new DateTime(2026, 4, 10, 11, 30, 0, DateTimeKind.Utc);

        var p1 = new Proposal
        {
            Title          = "Real-Time Predictive Analytics Dashboard Using Machine Learning",
            Abstract       = "This project proposes a real-time predictive analytics dashboard that leverages machine learning algorithms to analyse student academic performance data. The system ingests data from university platforms, applies supervised and unsupervised learning models, and presents actionable insights through an interactive visualisation layer. The goal is to enable proactive academic interventions by identifying at-risk students before performance deteriorates significantly.",
            TechnicalStack = "Python, TensorFlow, scikit-learn, React, ASP.NET Core Web API, SQL Server",
            Keywords       = "machine learning, predictive analytics, student performance, data visualisation, real-time",
            ResearchAreaId = 5,
            StudentId      = stu1.Id,
            Status         = ProposalStatus.Matched,
            SubmittedAt    = p1SubmittedAt,
            UpdatedAt      = p1SubmittedAt
        };

        var p2 = new Proposal
        {
            Title          = "Blockchain-Based Decentralised Identity Verification System",
            Abstract       = "This research investigates the design and implementation of a decentralised identity verification system using blockchain technology. The system replaces centralised identity stores with a distributed ledger, enabling individuals to own and control their digital identities. Smart contracts govern access policies, and zero-knowledge proofs allow attribute-selective disclosure without revealing underlying personal data.",
            TechnicalStack = "Solidity, Ethereum, Hardhat, Node.js, React, IPFS",
            Keywords       = "blockchain, decentralised identity, smart contracts, zero-knowledge proofs, digital identity",
            ResearchAreaId = 3,
            StudentId      = stu2.Id,
            Status         = ProposalStatus.Submitted,
            SubmittedAt    = p2SubmittedAt,
            UpdatedAt      = p2SubmittedAt
        };

        var p3 = new Proposal
        {
            Title          = "Conversational AI Assistant for University Academic Support",
            Abstract       = "This project designs and builds an intelligent conversational assistant for university students seeking academic support. The assistant uses large language models with retrieval-augmented generation to answer questions about course content, assignment requirements, and university policies. A feedback loop enables continuous improvement of response quality over time.",
            TechnicalStack = "Python, LangChain, OpenAI API, FastAPI, PostgreSQL, React, Docker",
            Keywords       = "conversational AI, retrieval-augmented generation, NLP, academic support, LLM",
            ResearchAreaId = 1,
            StudentId      = stu3.Id,
            Status         = ProposalStatus.Matched,
            SubmittedAt    = p3SubmittedAt,
            UpdatedAt      = p3SubmittedAt
        };

        db.Proposals.AddRange(p1, p2, p3);
        await db.SaveChangesAsync();

        // ── Supervisor interests ─────────────────────────────────────────────────
        var i1 = new SupervisorInterest   // John → Alex (confirmed)
        {
            SupervisorId = dbSup1.Id, ProposalId = p1.Id,
            Status = InterestStatus.Confirmed,
            CreatedAt   = new DateTime(2026, 4, 9, 14,  0, 0, DateTimeKind.Utc),
            ConfirmedAt = new DateTime(2026, 4, 9, 14, 20, 0, DateTimeKind.Utc)
        };
        var i2 = new SupervisorInterest   // David → Jamie (pending)
        {
            SupervisorId = dbSup3.Id, ProposalId = p2.Id,
            Status = InterestStatus.Pending,
            CreatedAt = new DateTime(2026, 4, 11, 10, 20, 0, DateTimeKind.Utc)
        };
        var i3 = new SupervisorInterest   // Emma → Jamie (pending)
        {
            SupervisorId = dbSup2.Id, ProposalId = p2.Id,
            Status = InterestStatus.Pending,
            CreatedAt = new DateTime(2026, 4, 12,  9, 45, 0, DateTimeKind.Utc)
        };
        var i4 = new SupervisorInterest   // Emma → Priya (confirmed)
        {
            SupervisorId = dbSup2.Id, ProposalId = p3.Id,
            Status = InterestStatus.Confirmed,
            CreatedAt   = new DateTime(2026, 4, 14, 11,  0, 0, DateTimeKind.Utc),
            ConfirmedAt = new DateTime(2026, 4, 14, 11,  5, 0, DateTimeKind.Utc)
        };

        db.SupervisorInterests.AddRange(i1, i2, i3, i4);
        await db.SaveChangesAsync();

        // ── Matches ──────────────────────────────────────────────────────────────
        var m1 = new Match   // Alex + John → Approved
        {
            StudentId    = stu1.Id,
            SupervisorId = dbSup1.Id,
            ProposalId   = p1.Id,
            Status       = MatchStatus.Approved,
            CreatedAt    = new DateTime(2026, 4,  9, 14, 21, 0, DateTimeKind.Utc),
            ApprovedAt   = new DateTime(2026, 4, 10, 14,  0, 0, DateTimeKind.Utc)
        };
        var m2 = new Match   // Priya + Emma → Pending
        {
            StudentId    = stu3.Id,
            SupervisorId = dbSup2.Id,
            ProposalId   = p3.Id,
            Status       = MatchStatus.Pending,
            CreatedAt    = new DateTime(2026, 4, 14, 11,  6, 0, DateTimeKind.Utc)
        };

        db.Matches.AddRange(m1, m2);
        await db.SaveChangesAsync();

        // ── Identity reveals ─────────────────────────────────────────────────────
        db.IdentityReveals.AddRange(
            new IdentityReveal
            {
                MatchId = m1.Id, TriggeredBySupervisorId = dbSup1.Id,
                RevealedAt = new DateTime(2026, 4,  9, 14, 21, 0, DateTimeKind.Utc)
            },
            new IdentityReveal
            {
                MatchId = m2.Id, TriggeredBySupervisorId = dbSup2.Id,
                RevealedAt = new DateTime(2026, 4, 14, 11,  6, 0, DateTimeKind.Utc)
            }
        );
        await db.SaveChangesAsync();

        // ── Audit events (chronological) ─────────────────────────────────────────
        db.AuditEvents.AddRange(
            new AuditEvent
            {
                Action = "ProposalSubmitted", UserId = stu1.Id, UserFullName = stu1.FullName,
                Timestamp = p1SubmittedAt,
                Details = $"Submitted proposal '{p1.Title}'"
            },
            new AuditEvent
            {
                Action = "ProposalSubmitted", UserId = stu2.Id, UserFullName = stu2.FullName,
                Timestamp = p2SubmittedAt,
                Details = $"Submitted proposal '{p2.Title}'"
            },
            new AuditEvent
            {
                Action = "InterestExpressed", UserId = dbSup1.Id, UserFullName = dbSup1.FullName,
                Timestamp = i1.CreatedAt,
                Details = $"Expressed interest in proposal #{p1.Id} ('{p1.Title}')"
            },
            new AuditEvent
            {
                Action = "InterestConfirmed", UserId = dbSup1.Id, UserFullName = dbSup1.FullName,
                Timestamp = i1.ConfirmedAt!.Value,
                Details = $"Confirmed interest in proposal #{p1.Id} — pending match created"
            },
            new AuditEvent
            {
                Action = "MatchCreated", UserId = dbSup1.Id, UserFullName = dbSup1.FullName,
                Timestamp = m1.CreatedAt,
                Details = $"Match #{m1.Id} created between {stu1.FullName} and {dbSup1.FullName}"
            },
            new AuditEvent
            {
                Action = "ProposalSubmitted", UserId = stu3.Id, UserFullName = stu3.FullName,
                Timestamp = p3SubmittedAt,
                Details = $"Submitted proposal '{p3.Title}'"
            },
            new AuditEvent
            {
                Action = "MatchApproved", UserId = ml1.Id, UserFullName = ml1.FullName,
                Timestamp = m1.ApprovedAt!.Value,
                Details = $"Approved match #{m1.Id} — {stu1.FullName} assigned to {dbSup1.FullName}"
            },
            new AuditEvent
            {
                Action = "InterestExpressed", UserId = dbSup3.Id, UserFullName = dbSup3.FullName,
                Timestamp = i2.CreatedAt,
                Details = $"Expressed interest in proposal #{p2.Id} ('{p2.Title}')"
            },
            new AuditEvent
            {
                Action = "InterestExpressed", UserId = dbSup2.Id, UserFullName = dbSup2.FullName,
                Timestamp = i3.CreatedAt,
                Details = $"Expressed interest in proposal #{p2.Id} ('{p2.Title}')"
            },
            new AuditEvent
            {
                Action = "InterestExpressed", UserId = dbSup2.Id, UserFullName = dbSup2.FullName,
                Timestamp = i4.CreatedAt,
                Details = $"Expressed interest in proposal #{p3.Id} ('{p3.Title}')"
            },
            new AuditEvent
            {
                Action = "InterestConfirmed", UserId = dbSup2.Id, UserFullName = dbSup2.FullName,
                Timestamp = i4.ConfirmedAt!.Value,
                Details = $"Confirmed interest in proposal #{p3.Id} — pending match created"
            },
            new AuditEvent
            {
                Action = "MatchCreated", UserId = dbSup2.Id, UserFullName = dbSup2.FullName,
                Timestamp = m2.CreatedAt,
                Details = $"Match #{m2.Id} created between {stu3.FullName} and {dbSup2.FullName}"
            }
        );
        await db.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> CreateAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string password,
        string role)
    {
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        await userManager.AddToRoleAsync(user, role);
        return user;
    }
}
