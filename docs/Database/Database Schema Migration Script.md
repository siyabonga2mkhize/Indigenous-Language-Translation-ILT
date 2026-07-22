
---
# Database Schema Migration Script
## Indigenous Language Phrasebook & Translation Portal

**Version:** 1.0  
**Date:** July 2026  
**Prepared By:** Team  
**Project:** Geeked-On Incubation Program – Project 5

---

## 📋 Table of Contents

- [1. Introduction](#1-introduction)
- [2. Entity Models (C# Classes)](#2-entity-models-c-classes)
  - [2.1 ApplicationUser](#21-applicationuser)
  - [2.2 Category](#22-category)
  - [2.3 Phrase](#23-phrase)
  - [2.4 Translation](#24-translation)
  - [2.5 Favourite](#25-favourite)
  - [2.6 Submission](#26-submission)
  - [2.7 LanguageStatistic](#27-languagestatistic)
- [3. ApplicationDbContext Configuration](#3-applicationdbcontext-configuration)
- [4. Seed Data](#4-seed-data)
- [5. Running the Migration](#5-running-the-migration)

---

## 1. Introduction

### 1.1 Purpose

This document contains the complete Entity Framework Core migration script for the Indigenous Language Phrasebook & Translation Portal. It includes:

- All entity models (C# classes)
- DbContext configuration with Fluent API
- Seed data for initial development
- Instructions for running migrations

### 1.2 Technology Stack

| Component | Version |
|-----------|---------|
| .NET | 6.0 / 8.0 |
| Entity Framework Core | 6.0 / 8.0 |
| SQL Server | 2019 / 2022 / Azure SQL |
| ASP.NET Identity | 6.0 / 8.0 |

### 1.3 Prerequisites

```bash
# Ensure you have the required NuGet packages
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

---

## 2. Entity Models (C# Classes)

### 2.1 ApplicationUser

**File:** `Models/ApplicationUser.cs`

```csharp
using Microsoft.AspNetCore.Identity;

namespace IndigenousLanguagePhrasebook.Models
{
    /// <summary>
    /// Application user extending ASP.NET Identity User
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        // Custom properties can be added here if needed
        // For example:
        // public string FirstName { get; set; }
        // public string LastName { get; set; }
        // public DateTime? DateOfBirth { get; set; }

        // Navigation properties
        public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public virtual ICollection<Translation> SubmittedTranslations { get; set; } = new List<Translation>();
        public virtual ICollection<Translation> ReviewedTranslations { get; set; } = new List<Translation>();
        public virtual ICollection<LanguageStatistic> Statistics { get; set; } = new List<LanguageStatistic>();
    }
}
```

---

### 2.2 Category

**File:** `Models/Category.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigenousLanguagePhrasebook.Models
{
    /// <summary>
    /// Represents a category for organising phrases
    /// </summary>
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Phrase> Phrases { get; set; } = new List<Phrase>();
        public virtual ICollection<LanguageStatistic> Statistics { get; set; } = new List<LanguageStatistic>();
    }
}
```

---

### 2.3 Phrase

**File:** `Models/Phrase.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigenousLanguagePhrasebook.Models
{
    /// <summary>
    /// Represents a campus phrase in English
    /// </summary>
    public class Phrase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PhraseId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string EnglishText { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }

        public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
        public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public virtual ICollection<LanguageStatistic> Statistics { get; set; } = new List<LanguageStatistic>();
    }
}
```

---

### 2.4 Translation

**File:** `Models/Translation.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigenousLanguagePhrasebook.Models
{
    /// <summary>
    /// Represents a translation of a phrase in a South African language
    /// </summary>
    public class Translation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TranslationId { get; set; }

        [Required]
        public int PhraseId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Text { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        [Required]
        public string SubmittedBy { get; set; } = string.Empty;

        public string? ReviewedBy { get; set; }

        [Required]
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedDate { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PhraseId))]
        public virtual Phrase? Phrase { get; set; }

        [ForeignKey(nameof(SubmittedBy))]
        public virtual ApplicationUser? Submitter { get; set; }

        [ForeignKey(nameof(ReviewedBy))]
        public virtual ApplicationUser? Reviewer { get; set; }
    }

    /// <summary>
    /// Translation status enum
    /// </summary>
    public enum TranslationStatus
    {
        Pending,
        Approved,
        Rejected
    }
}
```

---

### 2.5 Favourite

**File:** `Models/Favourite.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigenousLanguagePhrasebook.Models
{
    /// <summary>
    /// Junction table linking users to their favourite phrases
    /// </summary>
    public class Favourite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FavouriteId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int PhraseId { get; set; }

        [Required]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey(nameof(PhraseId))]
        public virtual Phrase? Phrase { get; set; }
    }
}
```

---

### 2.6 Submission

**File:** `Models/Submission.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigenousLanguagePhrasebook.Models
{
    /// <summary>
    /// Represents a community-contributed translation suggestion
    /// </summary>
    public class Submission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubmissionId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int PhraseId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string SuggestedText { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [MaxLength(500)]
        public string? RejectionReason { get; set; }

        [MaxLength(500)]
        public string? CorrectionInstructions { get; set; }

        [Required]
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedDate { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey(nameof(PhraseId))]
        public virtual Phrase? Phrase { get; set; }
    }

    /// <summary>
    /// Submission status enum
    /// </summary>
    public enum SubmissionStatus
    {
        Pending,
        Approved,
        Rejected,
        CorrectionRequested
    }
}
```

---

### 2.7 LanguageStatistic

**File:** `Models/LanguageStatistic.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigenousLanguagePhrasebook.Models
{
    /// <summary>
    /// Tracks usage statistics for analytics
    /// </summary>
    public class LanguageStatistic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatisticId { get; set; }

        public string? UserId { get; set; }

        public int? PhraseId { get; set; }

        [MaxLength(50)]
        public string? Language { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        [MaxLength(20)]
        public string EventType { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey(nameof(PhraseId))]
        public virtual Phrase? Phrase { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }
    }

    /// <summary>
    /// Event type enum
    /// </summary>
    public enum EventType
    {
        View,
        Search
    }
}
```

---

## 3. ApplicationDbContext Configuration

**File:** `Data/ApplicationDbContext.cs`

```csharp
using IndigenousLanguagePhrasebook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IndigenousLanguagePhrasebook.Data
{
    /// <summary>
    /// Application database context
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for custom entities
        public DbSet<Category> Categories { get; set; }
        public DbSet<Phrase> Phrases { get; set; }
        public DbSet<Translation> Translations { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<LanguageStatistic> LanguageStatistics { get; set; }

        /// <summary>
        /// Configure entity relationships and constraints using Fluent API
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Category Configuration ===
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.HasIndex(c => c.Name).IsUnique();
                entity.HasIndex(c => c.IsActive);
                entity.Property(c => c.IsActive)
                    .HasDefaultValue(true);
                entity.Property(c => c.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // === Phrase Configuration ===
            modelBuilder.Entity<Phrase>(entity =>
            {
                entity.HasKey(p => p.PhraseId);
                entity.Property(p => p.EnglishText)
                    .IsRequired()
                    .HasMaxLength(1000);
                entity.Property(p => p.IsActive)
                    .HasDefaultValue(true);
                entity.Property(p => p.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Phrases)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                entity.HasIndex(p => p.EnglishText);
                entity.HasIndex(p => p.CategoryId);
                entity.HasIndex(p => p.IsActive);
            });

            // === Translation Configuration ===
            modelBuilder.Entity<Translation>(entity =>
            {
                entity.HasKey(t => t.TranslationId);
                entity.Property(t => t.Language)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(t => t.Text)
                    .IsRequired()
                    .HasMaxLength(2000);
                entity.Property(t => t.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion<string>();
                entity.Property(t => t.RejectionReason)
                    .HasMaxLength(500);
                entity.Property(t => t.SubmittedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(t => t.Phrase)
                    .WithMany(p => p.Translations)
                    .HasForeignKey(t => t.PhraseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Submitter)
                    .WithMany(u => u.SubmittedTranslations)
                    .HasForeignKey(t => t.SubmittedBy)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.Reviewer)
                    .WithMany(u => u.ReviewedTranslations)
                    .HasForeignKey(t => t.ReviewedBy)
                    .OnDelete(DeleteBehavior.SetNull);

                // Indexes
                entity.HasIndex(t => t.PhraseId);
                entity.HasIndex(t => t.Language);
                entity.HasIndex(t => t.Status);
                entity.HasIndex(t => t.SubmittedBy);

                // Check constraint for status
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Translation_Status",
                    "Status IN ('Pending', 'Approved', 'Rejected')"));

                // Filtered unique index: only one approved translation per phrase per language
                entity.HasIndex(t => new { t.PhraseId, t.Language })
                    .IsUnique()
                    .HasFilter("Status = 'Approved'")
                    .HasDatabaseName("UX_Translation_UniqueApproved");
            });

            // === Favourite Configuration ===
            modelBuilder.Entity<Favourite>(entity =>
            {
                entity.HasKey(f => f.FavouriteId);
                entity.Property(f => f.AddedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(f => f.User)
                    .WithMany(u => u.Favourites)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Phrase)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(f => f.PhraseId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint to prevent duplicate favourites
                entity.HasIndex(f => new { f.UserId, f.PhraseId })
                    .IsUnique()
                    .HasDatabaseName("UC_Favourite_Unique");

                // Indexes
                entity.HasIndex(f => f.UserId);
                entity.HasIndex(f => f.PhraseId);
            });

            // === Submission Configuration ===
            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(s => s.SubmissionId);
                entity.Property(s => s.Language)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(s => s.SuggestedText)
                    .IsRequired()
                    .HasMaxLength(2000);
                entity.Property(s => s.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion<string>();
                entity.Property(s => s.RejectionReason)
                    .HasMaxLength(500);
                entity.Property(s => s.CorrectionInstructions)
                    .HasMaxLength(500);
                entity.Property(s => s.SubmittedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(s => s.User)
                    .WithMany(u => u.Submissions)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Phrase)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(s => s.PhraseId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Check constraint for status
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Submission_Status",
                    "Status IN ('Pending', 'Approved', 'Rejected', 'CorrectionRequested')"));

                // Indexes
                entity.HasIndex(s => s.UserId);
                entity.HasIndex(s => s.PhraseId);
                entity.HasIndex(s => s.Status);
                entity.HasIndex(s => s.Language);
            });

            // === LanguageStatistic Configuration ===
            modelBuilder.Entity<LanguageStatistic>(entity =>
            {
                entity.HasKey(ls => ls.StatisticId);
                entity.Property(ls => ls.Language)
                    .HasMaxLength(50);
                entity.Property(ls => ls.EventType)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasConversion<string>();
                entity.Property(ls => ls.EventDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Relationships
                entity.HasOne(ls => ls.User)
                    .WithMany(u => u.Statistics)
                    .HasForeignKey(ls => ls.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(ls => ls.Phrase)
                    .WithMany(p => p.Statistics)
                    .HasForeignKey(ls => ls.PhraseId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(ls => ls.Category)
                    .WithMany(c => c.Statistics)
                    .HasForeignKey(ls => ls.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Check constraint for event type
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_LanguageStatistics_EventType",
                    "EventType IN ('View', 'Search')"));

                // Indexes for analytics
                entity.HasIndex(ls => ls.UserId);
                entity.HasIndex(ls => ls.EventDate);
                entity.HasIndex(ls => ls.Language);
                entity.HasIndex(ls => ls.EventType);
                entity.HasIndex(ls => ls.CategoryId);

                // Composite index for common analytics queries
                entity.HasIndex(ls => new { ls.EventDate, ls.EventType, ls.Language, ls.CategoryId })
                    .HasDatabaseName("IX_LanguageStatistics_Analytics");
            });
        }
    }
}
```

---

## 4. Seed Data

**File:** `Data/SeedData.cs`

```csharp
using IndigenousLanguagePhrasebook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IndigenousLanguagePhrasebook.Data
{
    /// <summary>
    /// Database seed data for development and testing
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Seed the database with initial data
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Apply migrations if needed
            await context.Database.MigrateAsync();

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles
            await CreateRolesAsync(roleManager);

            // Create admin user
            await CreateAdminUserAsync(userManager);

            // Seed categories
            await SeedCategoriesAsync(context);

            // Seed phrases
            await SeedPhrasesAsync(context);

            // Seed sample translations
            await SeedTranslationsAsync(context, userManager);
        }

        /// <summary>
        /// Create default roles
        /// </summary>
        private static async Task CreateRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Student", "Administrator" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        /// <summary>
        /// Create default admin user
        /// </summary>
        private static async Task CreateAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@phrasebook.local";
            const string adminPassword = "Admin@123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    await userManager.AddToRoleAsync(adminUser, "Student");
                }
            }
        }

        /// <summary>
        /// Seed categories
        /// </summary>
        private static async Task SeedCategoriesAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync())
                return;

            var categories = new[]
            {
                new Category { Name = "Registration", Description = "Phrases related to course registration and enrolment" },
                new Category { Name = "Accommodation", Description = "Phrases about university housing and residence life" },
                new Category { Name = "Health Services", Description = "Phrases for campus health and wellness services" },
                new Category { Name = "Library", Description = "Phrases about library resources and services" },
                new Category { Name = "Academic Support", Description = "Phrases for tutoring, academic advising, and support services" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed sample phrases
        /// </summary>
        private static async Task SeedPhrasesAsync(ApplicationDbContext context)
        {
            if (await context.Phrases.AnyAsync())
                return;

            // Get category IDs
            var registrationId = await context.Categories
                .Where(c => c.Name == "Registration")
                .Select(c => c.CategoryId)
                .FirstOrDefaultAsync();

            var accommodationId = await context.Categories
                .Where(c => c.Name == "Accommodation")
                .Select(c => c.CategoryId)
                .FirstOrDefaultAsync();

            var healthId = await context.Categories
                .Where(c => c.Name == "Health Services")
                .Select(c => c.CategoryId)
                .FirstOrDefaultAsync();

            var libraryId = await context.Categories
                .Where(c => c.Name == "Library")
                .Select(c => c.CategoryId)
                .FirstOrDefaultAsync();

            var academicId = await context.Categories
                .Where(c => c.Name == "Academic Support")
                .Select(c => c.CategoryId)
                .FirstOrDefaultAsync();

            var phrases = new[]
            {
                new Phrase { EnglishText = "Where is the registration office?", CategoryId = registrationId },
                new Phrase { EnglishText = "How do I register for courses?", CategoryId = registrationId },
                new Phrase { EnglishText = "Where can I find accommodation?", CategoryId = accommodationId },
                new Phrase { EnglishText = "How do I apply for a residence?", CategoryId = accommodationId },
                new Phrase { EnglishText = "Where is the campus health centre?", CategoryId = healthId },
                new Phrase { EnglishText = "How do I make an appointment at the clinic?", CategoryId = healthId },
                new Phrase { EnglishText = "Where is the library?", CategoryId = libraryId },
                new Phrase { EnglishText = "How do I borrow a book?", CategoryId = libraryId },
                new Phrase { EnglishText = "Where is the academic advising office?", CategoryId = academicId },
                new Phrase { EnglishText = "How do I get a tutor?", CategoryId = academicId }
            };

            await context.Phrases.AddRangeAsync(phrases);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Seed sample translations
        /// </summary>
        private static async Task SeedTranslationsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (await context.Translations.AnyAsync())
                return;

            // Get admin user ID
            var adminUser = await userManager.FindByEmailAsync("admin@phrasebook.local");
            if (adminUser == null) return;

            // Get phrases
            var phrases = await context.Phrases.ToListAsync();

            var translations = new[]
            {
                new Translation
                {
                    PhraseId = phrases[0].PhraseId,
                    Language = "isiZulu",
                    Text = "Iphi ihhovisi lokubhalisa?",
                    Status = TranslationStatus.Approved.ToString(),
                    SubmittedBy = adminUser.Id,
                    SubmittedDate = DateTime.UtcNow,
                    ReviewedBy = adminUser.Id,
                    ReviewedDate = DateTime.UtcNow
                },
                new Translation
                {
                    PhraseId = phrases[1].PhraseId,
                    Language = "isiZulu",
                    Text = "Ngibhalisa kanjani izifundo?",
                    Status = TranslationStatus.Approved.ToString(),
                    SubmittedBy = adminUser.Id,
                    SubmittedDate = DateTime.UtcNow,
                    ReviewedBy = adminUser.Id,
                    ReviewedDate = DateTime.UtcNow
                },
                new Translation
                {
                    PhraseId = phrases[2].PhraseId,
                    Language = "isiZulu",
                    Text = "Ngiyithola kuphi indawo yokuhlala?",
                    Status = TranslationStatus.Approved.ToString(),
                    SubmittedBy = adminUser.Id,
                    SubmittedDate = DateTime.UtcNow,
                    ReviewedBy = adminUser.Id,
                    ReviewedDate = DateTime.UtcNow
                },
                new Translation
                {
                    PhraseId = phrases[3].PhraseId,
                    Language = "isiZulu",
                    Text = "Ngifaka kanjani isicelo sendawo yokuhlala?",
                    Status = TranslationStatus.Approved.ToString(),
                    SubmittedBy = adminUser.Id,
                    SubmittedDate = DateTime.UtcNow,
                    ReviewedBy = adminUser.Id,
                    ReviewedDate = DateTime.UtcNow
                },
                new Translation
                {
                    PhraseId = phrases[4].PhraseId,
                    Language = "isiZulu",
                    Text = "Iphi indawo yezempilo?",
                    Status = TranslationStatus.Approved.ToString(),
                    SubmittedBy = adminUser.Id,
                    SubmittedDate = DateTime.UtcNow,
                    ReviewedBy = adminUser.Id,
                    ReviewedDate = DateTime.UtcNow
                },
                new Translation
                {
                    PhraseId = phrases[0].PhraseId,
                    Language = "isiXhosa",
                    Text = "Liphi iofisi yokubhalisa?",
                    Status = TranslationStatus.Approved.ToString(),
                    SubmittedBy = adminUser.Id,
                    SubmittedDate = DateTime.UtcNow,
                    ReviewedBy = adminUser.Id,
                    ReviewedDate = DateTime.UtcNow
                }
            };

            await context.Translations.AddRangeAsync(translations);
            await context.SaveChangesAsync();
        }
    }
}
```

---

## 5. Running the Migration

### 5.1 Program.cs Configuration

**File:** `Program.cs` or `Startup.cs`

```csharp
using IndigenousLanguagePhrasebook.Data;
using IndigenousLanguagePhrasebook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ... rest of your configuration

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// ... rest of your app configuration

app.Run();
```

---

### 5.2 appsettings.json Configuration

**File:** `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PhrasebookDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

### 5.3 Migration Commands

```bash
# Step 1: Add the initial migration
dotnet ef migrations add InitialCreate

# Step 2: Apply the migration to the database
dotnet ef database update

# Step 3: (Optional) Remove last migration if needed
dotnet ef migrations remove

# Step 4: (Optional) Revert to a specific migration
dotnet ef database update PreviousMigrationName
```

---

## 6. Summary

### 6.1 Files Created

| File | Path | Description |
|------|------|-------------|
| ApplicationUser.cs | Models/ | Custom Identity user |
| Category.cs | Models/ | Category entity |
| Phrase.cs | Models/ | Phrase entity |
| Translation.cs | Models/ | Translation entity |
| Favourite.cs | Models/ | Favourite junction table |
| Submission.cs | Models/ | Submission entity |
| LanguageStatistic.cs | Models/ | Statistics entity |
| ApplicationDbContext.cs | Data/ | Database context with Fluent API |
| SeedData.cs | Data/ | Seed data for development |
| Program.cs | Root | Application configuration with seeding |

### 6.2 Migration Status

| Step | Command | Status |
|------|---------|--------|
| 1 | Add-Migration InitialCreate | ✅ Ready |
| 2 | Update-Database | ✅ Ready |
| 3 | Seed Data | ✅ Ready |

---

**Document Status:** ✅ Complete

**Last Updated:** July 2026

**Next Steps:** Run the migration and verify the database schema

---

## 🔗 Related Issues

- **Depends on:** Issue 3 – Design ER Diagram – Logical Model
- **Blocks:** Phase 3 – Environment Setup & Project Scaffolding
- **Related to:** Phase 4 – Core Feature Development

---

**Ready to run the migration!** 🚀
```

---

## 📁 What to Commit

1. **Create Models Folder:** Add all the entity model files
2. **Create Data Folder:** Add ApplicationDbContext.cs and SeedData.cs
3. **Update Program.cs:** Add database seeding
4. **Update appsettings.json:** Add connection string
5. **Run migration commands:** Generate the actual migration

## 🔧 Quick Setup Commands

```bash
# Add required packages
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

# Create the initial migration
dotnet ef migrations add InitialCreate

# Update the database
dotnet ef database update
```