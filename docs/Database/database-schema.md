## Reading Time : 15 min ;-) 
# Logical ER Diagram – Database Schema Specification
## Indigenous Language Phrasebook & Translation Portal

**Version:** 1.0  
**Date:** July 2026  
**Prepared By:** Siyabonga
**Project:** Geeked-On Incubation Program – Project 5

---

## 📋 Table of Contents

- [1. Introduction](#1-introduction)
- [2. Entity Descriptions](#2-entity-descriptions)
  - [2.1 Category](#21-category)
  - [2.2 Phrase](#22-phrase)
  - [2.3 Translation](#23-translation)
  - [2.4 Favourite](#24-favourite)
  - [2.5 Submission](#25-submission)
  - [2.6 LanguageStatistic](#26-languagestatistic)
  - [2.7 ASP.NET Identity Tables](#27-aspnet-identity-tables)
- [3. Relationship Diagram](#3-relationship-diagram)
- [4. Foreign Key Summary](#4-foreign-key-summary)
- [5. Complete SQL Script](#5-complete-sql-script)
- [6. Entity Relationship Diagram (Visual)](#6-entity-relationship-diagram-visual)

---

## 1. Introduction

### 1.1 Purpose

This document provides the **Logical ER Diagram** specification for the Indigenous Language Phrasebook & Translation Portal. It details every table, column, data type, constraint, and relationship in the database schema. This is the implementation-ready design that will be used to generate Entity Framework Core models and migrations.

### 1.2 Scope

The logical model includes:
- 7 custom application tables
- 6 ASP.NET Identity tables (standard)
- All primary keys, foreign keys, and indexes
- Data types and constraints
- Relationship definitions with cardinality
- Complete SQL DDL scripts

### 1.3 Notation Conventions

| Notation | Meaning |
|----------|---------|
| **PK** | Primary Key |
| **FK** | Foreign Key |
| **UK** | Unique Key |
| **IX** | Index |
| **NN** | Not Null |
| **IDENTITY** | Auto-incrementing column |
| **DEFAULT** | Default value |
| **[Table]** | Table name in brackets |

---

## 2. Entity Descriptions

### 2.1 Category

**Purpose:** Organises phrases into campus-specific topics for easy filtering and navigation.

**Table Name:** `Categories`

| Column | Data Type | Nullable | Constraints | Description |
|--------|-----------|----------|-------------|-------------|
| **CategoryId** | int | No | **PK**, IDENTITY(1,1) | Unique identifier for the category |
| **Name** | nvarchar(100) | No | **UK**, NN | Category name (e.g., "Registration") |
| **Description** | nvarchar(500) | Yes | - | Optional description of the category |
| **IsActive** | bit | No | NN, DEFAULT(1) | Soft delete flag (1=active, 0=inactive) |
| **CreatedDate** | datetime2 | No | NN, DEFAULT(GETUTCDATE()) | When the category was created |
| **ModifiedDate** | datetime2 | Yes | - | Last update timestamp |

**Business Rules:**
- `Name` must be unique
- Only active categories (`IsActive = 1`) can be assigned to phrases
- Deactivating a category soft-deletes it; no data is lost

**Predefined Data:**
```sql
INSERT INTO Categories (Name, Description) VALUES
('Registration', 'Phrases related to course registration and enrolment'),
('Accommodation', 'Phrases about university housing and residence life'),
('Health Services', 'Phrases for campus health and wellness services'),
('Library', 'Phrases about library resources and services'),
('Academic Support', 'Phrases for tutoring, academic advising, and support services');
```

**Indexes:**
```sql
CREATE INDEX IX_Category_Name ON Categories(Name);
CREATE INDEX IX_Category_IsActive ON Categories(IsActive);
```

---

### 2.2 Phrase

**Purpose:** Stores the core content – campus phrases in English with category association.

**Table Name:** `Phrases`

| Column | Data Type | Nullable | Constraints | Description |
|--------|-----------|----------|-------------|-------------|
| **PhraseId** | int | No | **PK**, IDENTITY(1,1) | Unique identifier for the phrase |
| **EnglishText** | nvarchar(1000) | No | NN | The phrase in English |
| **CategoryId** | int | No | **FK** → Categories.CategoryId, NN | Category the phrase belongs to |
| **IsActive** | bit | No | NN, DEFAULT(1) | Soft delete flag |
| **CreatedDate** | datetime2 | No | NN, DEFAULT(GETUTCDATE()) | When the phrase was created |
| **ModifiedDate** | datetime2 | Yes | - | Last update timestamp |

**Business Rules:**
- Each phrase belongs to exactly one category
- Only active phrases (`IsActive = 1`) appear in search results
- A phrase can have multiple translations (one per language)
- EnglishText should be checked for duplicates (application-level validation)

**Foreign Key:**
```sql
ALTER TABLE Phrases ADD CONSTRAINT FK_Phrases_Categories 
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
    ON DELETE RESTRICT;
```

**Indexes:**
```sql
CREATE INDEX IX_Phrase_EnglishText ON Phrases(EnglishText);
CREATE INDEX IX_Phrase_CategoryId ON Phrases(CategoryId);
CREATE INDEX IX_Phrase_IsActive ON Phrases(IsActive);
```

---

### 2.3 Translation

**Purpose:** Stores translations of phrases in South African languages with approval workflow.

**Table Name:** `Translations`

| Column | Data Type | Nullable | Constraints | Description |
|--------|-----------|----------|-------------|-------------|
| **TranslationId** | int | No | **PK**, IDENTITY(1,1) | Unique identifier for the translation |
| **PhraseId** | int | No | **FK** → Phrases.PhraseId, NN | The phrase being translated |
| **Language** | nvarchar(50) | No | NN | Target language (e.g., "isiZulu") |
| **Text** | nvarchar(2000) | No | NN | The translated text |
| **Status** | nvarchar(20) | No | NN, CHECK | "Pending", "Approved", "Rejected" |
| **RejectionReason** | nvarchar(500) | Yes | - | Reason if rejected |
| **SubmittedBy** | nvarchar(450) | No | **FK** → AspNetUsers.Id, NN | The student who submitted this translation |
| **ReviewedBy** | nvarchar(450) | Yes | **FK** → AspNetUsers.Id | Administrator who reviewed this translation |
| **SubmittedDate** | datetime2 | No | NN, DEFAULT(GETUTCDATE()) | When the translation was submitted |
| **ReviewedDate** | datetime2 | Yes | - | When the translation was reviewed |

**Business Rules:**
- Only one "Approved" translation per phrase per language
- Pending translations are hidden from search results
- Only Administrators can change status from Pending to Approved/Rejected
- Rejection reason is required when rejecting
- A translation is submitted by a Student, reviewed by an Administrator

**Status Values:**
```
Pending   - Awaiting administrator review
Approved  - Published and visible in search results
Rejected  - Not published; rejection reason provided
```

**Foreign Keys:**
```sql
ALTER TABLE Translations ADD CONSTRAINT FK_Translations_Phrases 
    FOREIGN KEY (PhraseId) REFERENCES Phrases(PhraseId)
    ON DELETE CASCADE;

ALTER TABLE Translations ADD CONSTRAINT FK_Translations_SubmittedBy 
    FOREIGN KEY (SubmittedBy) REFERENCES AspNetUsers(Id)
    ON DELETE CASCADE;

ALTER TABLE Translations ADD CONSTRAINT FK_Translations_ReviewedBy 
    FOREIGN KEY (ReviewedBy) REFERENCES AspNetUsers(Id)
    ON DELETE SET NULL;
```

**Check Constraint:**
```sql
ALTER TABLE Translations ADD CONSTRAINT CK_Translation_Status 
    CHECK (Status IN ('Pending', 'Approved', 'Rejected'));
```

**Indexes:**
```sql
CREATE INDEX IX_Translation_PhraseId ON Translations(PhraseId);
CREATE INDEX IX_Translation_Language ON Translations(Language);
CREATE INDEX IX_Translation_Status ON Translations(Status);
CREATE INDEX IX_Translation_SubmittedBy ON Translations(SubmittedBy);

-- Filtered unique index: only one approved translation per phrase per language
CREATE UNIQUE INDEX UX_Translation_UniqueApproved 
    ON Translations(PhraseId, Language) 
    WHERE Status = 'Approved';
```

---

### 2.4 Favourite

**Purpose:** Junction table linking Students to their favourite Phrases.

**Table Name:** `Favourites`

| Column | Data Type | Nullable | Constraints | Description |
|--------|-----------|----------|-------------|-------------|
| **FavouriteId** | int | No | **PK**, IDENTITY(1,1) | Unique identifier for the favourite record |
| **UserId** | nvarchar(450) | No | **FK** → AspNetUsers.Id, NN | The student who favourited the phrase |
| **PhraseId** | int | No | **FK** → Phrases.PhraseId, NN | The phrase being favourited |
| **AddedDate** | datetime2 | No | NN, DEFAULT(GETUTCDATE()) | When the phrase was favourited |

**Business Rules:**
- A user can favourite a phrase only once (unique constraint on UserId + PhraseId)
- Only active phrases can be favourited
- Favourites persist across sessions
- Removing a favourite deletes the record

**Foreign Keys:**
```sql
ALTER TABLE Favourites ADD CONSTRAINT FK_Favourites_Users 
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
    ON DELETE CASCADE;

ALTER TABLE Favourites ADD CONSTRAINT FK_Favourites_Phrases 
    FOREIGN KEY (PhraseId) REFERENCES Phrases(PhraseId)
    ON DELETE CASCADE;
```

**Unique Constraint:**
```sql
ALTER TABLE Favourites ADD CONSTRAINT UC_Favourite_Unique 
    UNIQUE (UserId, PhraseId);
```

**Indexes:**
```sql
CREATE INDEX IX_Favourite_UserId ON Favourites(UserId);
CREATE INDEX IX_Favourite_PhraseId ON Favourites(PhraseId);
```

---

### 2.5 Submission

**Purpose:** Tracks community-contributed translation suggestions awaiting administrator review.

**Table Name:** `Submissions`

| Column | Data Type | Nullable | Constraints | Description |
|--------|-----------|----------|-------------|-------------|
| **SubmissionId** | int | No | **PK**, IDENTITY(1,1) | Unique identifier for the submission |
| **UserId** | nvarchar(450) | No | **FK** → AspNetUsers.Id, NN | The student who submitted the suggestion |
| **PhraseId** | int | No | **FK** → Phrases.PhraseId, NN | The phrase being translated |
| **Language** | nvarchar(50) | No | NN | Target language for the suggestion |
| **SuggestedText** | nvarchar(2000) | No | NN | The suggested translation text |
| **Status** | nvarchar(20) | No | NN, CHECK | "Pending", "Approved", "Rejected", "CorrectionRequested" |
| **RejectionReason** | nvarchar(500) | Yes | - | Reason if rejected |
| **CorrectionInstructions** | nvarchar(500) | Yes | - | Instructions if correction requested |
| **SubmittedDate** | datetime2 | No | NN, DEFAULT(GETUTCDATE()) | When the suggestion was submitted |
| **ReviewedDate** | datetime2 | Yes | - | When the suggestion was reviewed |

**Business Rules:**
- A student can submit multiple suggestions for the same phrase and language
- Pending submissions are not visible in search results
- When approved, a Translation record is created
- When rejected, the student sees the reason on their profile
- When correction requested, the student can resubmit
- Administrators can approve, reject, or request correction

**Status Values:**
```
Pending             - Awaiting administrator review
Approved            - Approved; a Translation record will be created
Rejected            - Rejected; reason provided
CorrectionRequested - Needs correction; instructions provided
```

**Foreign Keys:**
```sql
ALTER TABLE Submissions ADD CONSTRAINT FK_Submissions_Users 
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
    ON DELETE CASCADE;

ALTER TABLE Submissions ADD CONSTRAINT FK_Submissions_Phrases 
    FOREIGN KEY (PhraseId) REFERENCES Phrases(PhraseId)
    ON DELETE CASCADE;
```

**Check Constraint:**
```sql
ALTER TABLE Submissions ADD CONSTRAINT CK_Submission_Status 
    CHECK (Status IN ('Pending', 'Approved', 'Rejected', 'CorrectionRequested'));
```

**Indexes:**
```sql
CREATE INDEX IX_Submission_UserId ON Submissions(UserId);
CREATE INDEX IX_Submission_PhraseId ON Submissions(PhraseId);
CREATE INDEX IX_Submission_Status ON Submissions(Status);
CREATE INDEX IX_Submission_Language ON Submissions(Language);
```

---

### 2.6 LanguageStatistic

**Purpose:** Tracks usage statistics for analytics and reporting.

**Table Name:** `LanguageStatistics`

| Column | Data Type | Nullable | Constraints | Description |
|--------|-----------|----------|-------------|-------------|
| **StatisticId** | int | No | **PK**, IDENTITY(1,1) | Unique identifier for the statistic record |
| **UserId** | nvarchar(450) | Yes | **FK** → AspNetUsers.Id | User who performed the action (null if anonymous) |
| **PhraseId** | int | Yes | **FK** → Phrases.PhraseId | Phrase viewed (null for search-only events) |
| **Language** | nvarchar(50) | Yes | - | Language selected in search/filter (null if none) |
| **CategoryId** | int | Yes | **FK** → Categories.CategoryId | Category selected in search/filter (null if none) |
| **EventType** | nvarchar(20) | No | NN, CHECK | "View" or "Search" |
| **EventDate** | datetime2 | No | NN, DEFAULT(GETUTCDATE()) | When the event occurred |

**Business Rules:**
- Every phrase detail view is logged (EventType = "View")
- Every search is logged (EventType = "Search") with language and category filters if applied
- Anonymous users are logged with UserId = null
- Statistics are append-only – never deleted
- Administrators can view aggregated reports

**Foreign Keys:**
```sql
ALTER TABLE LanguageStatistics ADD CONSTRAINT FK_LanguageStatistics_Users 
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
    ON DELETE SET NULL;

ALTER TABLE LanguageStatistics ADD CONSTRAINT FK_LanguageStatistics_Phrases 
    FOREIGN KEY (PhraseId) REFERENCES Phrases(PhraseId)
    ON DELETE SET NULL;

ALTER TABLE LanguageStatistics ADD CONSTRAINT FK_LanguageStatistics_Categories 
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
    ON DELETE SET NULL;
```

**Check Constraint:**
```sql
ALTER TABLE LanguageStatistics ADD CONSTRAINT CK_LanguageStatistics_EventType 
    CHECK (EventType IN ('View', 'Search'));
```

**Indexes:**
```sql
CREATE INDEX IX_LanguageStatistics_User ON LanguageStatistics(UserId);
CREATE INDEX IX_LanguageStatistics_EventDate ON LanguageStatistics(EventDate);
CREATE INDEX IX_LanguageStatistics_Language ON LanguageStatistics(Language);
CREATE INDEX IX_LanguageStatistics_EventType ON LanguageStatistics(EventType);
CREATE INDEX IX_LanguageStatistics_Category ON LanguageStatistics(CategoryId);

-- Composite index for analytics queries
CREATE INDEX IX_LanguageStatistics_Analytics 
    ON LanguageStatistics(EventDate, EventType, Language, CategoryId);
```

---

### 2.7 ASP.NET Identity Tables

**Purpose:** Standard ASP.NET Identity tables for authentication and authorization.

| Table Name | Description |
|------------|-------------|
| **AspNetUsers** | User account information (Id, UserName, Email, PasswordHash, etc.) |
| **AspNetRoles** | Role definitions (Student, Administrator) |
| **AspNetUserRoles** | User-to-role assignments (junction table) |
| **AspNetUserClaims** | User-specific claims for fine-grained permissions |
| **AspNetUserLogins** | External login providers (e.g., Google, Facebook) |
| **AspNetUserTokens** | User tokens (e.g., for password reset, email confirmation) |

**Note:** These tables are generated by ASP.NET Identity and should not be modified manually. Custom properties can be added by extending the `ApplicationUser` class.

**Custom Fields (Extended ApplicationUser):** *(Optional – add if needed)*
```csharp
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
```

---

## 3. Relationship Diagram

### 3.1 Entity Relationship Overview

```
┌─────────────────────────┐
│      AspNetUsers        │
│   (Identity User)       │
│  ──────────────────     │
│  Id (PK)                │
│  UserName               │
│  Email                  │
│  PasswordHash           │
│  ...                    │
└────────┬────────────────┘
         │
         │ 1
         │
         │ M
         ▼
┌─────────────────────────┐          ┌─────────────────────────┐
│      Favourites         │          │      Submissions        │
│  ──────────────────     │          │  ──────────────────     │
│  FavouriteId (PK)       │          │  SubmissionId (PK)      │
│  UserId (FK)            │◄─────────│  UserId (FK)            │
│  PhraseId (FK)          │          │  PhraseId (FK)          │
│  AddedDate              │          │  Language               │
└────────┬────────────────┘          │  SuggestedText          │
         │                           │  Status                 │
         │                           │  RejectionReason        │
         │ M                         │  CorrectionInstructions │
         │                           │  SubmittedDate          │
         │ 1                         │  ReviewedDate           │
         ▼                           └────────┬────────────────┘
┌─────────────────────────┐                   │
│        Phrases          │                   │ 1
│  ──────────────────     │                   │
│  PhraseId (PK)          │                   │
│  EnglishText            │                   │ M
│  CategoryId (FK)        │◄──────────────────┘
│  IsActive               │
│  CreatedDate            │
│  ModifiedDate           │
└────────┬────────────────┘          ┌─────────────────────────┐
         │                           │      Translations       │
         │ 1                         │  ──────────────────     │
         │                           │  TranslationId (PK)     │
         │ M                         │  PhraseId (FK)          │
         ▼                           │  Language               │
┌─────────────────────────┐          │  Text                   │
│      Categories         │          │  Status                 │
│  ──────────────────     │          │  RejectionReason        │
│  CategoryId (PK)        │          │  SubmittedBy (FK)       │──┐
│  Name                   │          │  ReviewedBy (FK)        │──┼──┐
│  Description            │          │  SubmittedDate          │  │  │
│  IsActive               │          │  ReviewedDate           │  │  │
│  CreatedDate            │          └─────────────────────────┘  │  │
│  ModifiedDate           │                                       │  │
└─────────────────────────┘                                       │  │
                                                                  │  │
         ┌─────────────────────────┐                              │  │
         │   LanguageStatistics    │                              │  │
         │  ──────────────────     │                              │  │
         │  StatisticId (PK)       │                              │  │
         │  UserId (FK)            │──────────────────────────────┘  │
         │  PhraseId (FK)          │─────────────────────────────────┘
         │  CategoryId (FK)        │────────────────────────────────┐
         │  Language               │                                │
         │  EventType              │                                │
         │  EventDate              │                                │
         └─────────────────────────┘                                │
                    │                                               │
                    │ (references Categories)                      │
                    └───────────────────────────────────────────────┘
```

### 3.2 Cardinality Summary

| Relationship | From Entity | To Entity | Cardinality |
|--------------|-------------|-----------|-------------|
| Category → Phrase | Category | Phrase | 1 : M |
| Phrase → Translation | Phrase | Translation | 1 : M |
| User → Favourite | AspNetUser | Favourite | 1 : M |
| Phrase → Favourite | Phrase | Favourite | 1 : M |
| User → Submission | AspNetUser | Submission | 1 : M |
| Phrase → Submission | Phrase | Submission | 1 : M |
| User → Translation (SubmittedBy) | AspNetUser | Translation | 1 : M |
| User → Translation (ReviewedBy) | AspNetUser | Translation | 1 : M |
| User → LanguageStatistic | AspNetUser | LanguageStatistic | 1 : M |
| Phrase → LanguageStatistic | Phrase | LanguageStatistic | 1 : M |
| Category → LanguageStatistic | Category | LanguageStatistic | 1 : M |

---

## 4. Foreign Key Summary

### 4.1 Foreign Key Reference Table

| Foreign Key Name | Child Table | Parent Table | ON DELETE |
|------------------|-------------|--------------|-----------|
| FK_Phrases_Categories | Phrases | Categories | RESTRICT |
| FK_Translations_Phrases | Translations | Phrases | CASCADE |
| FK_Translations_SubmittedBy | Translations | AspNetUsers | CASCADE |
| FK_Translations_ReviewedBy | Translations | AspNetUsers | SET NULL |
| FK_Favourites_Users | Favourites | AspNetUsers | CASCADE |
| FK_Favourites_Phrases | Favourites | Phrases | CASCADE |
| FK_Submissions_Users | Submissions | AspNetUsers | CASCADE |
| FK_Submissions_Phrases | Submissions | Phrases | CASCADE |
| FK_LanguageStatistics_Users | LanguageStatistics | AspNetUsers | SET NULL |
| FK_LanguageStatistics_Phrases | LanguageStatistics | Phrases | SET NULL |
| FK_LanguageStatistics_Categories | LanguageStatistics | Categories | SET NULL |

### 4.2 ON DELETE Behavior Explanation

| Behavior | Use Case | Rationale |
|----------|----------|-----------|
| **CASCADE** | Translations → Phrases | If a phrase is deleted, its translations should be deleted too |
| **CASCADE** | Favourites → Users/Phrases | User favourites should be deleted when the user or phrase is deleted |
| **CASCADE** | Submissions → Users/Phrases | User submissions should be deleted when the user or phrase is deleted |
| **SET NULL** | Translations → ReviewedBy | When an admin is deleted, keep the translation but remove the reviewer reference |
| **SET NULL** | LanguageStatistics → Users/Phrases/Categories | Keep statistics even if the referenced entity is deleted |
| **RESTRICT** | Phrases → Categories | Prevent deleting a category that still has phrases assigned |

---

## 5. Complete SQL Script

### 5.1 Create Tables Script

```sql
-- =============================================
-- Database: Indigenous Language Phrasebook
-- Version: 1.0
-- Description: Complete database schema
-- =============================================

-- Enable full-text search (optional)
-- CREATE FULLTEXT CATALOG PhraseCatalog AS DEFAULT;

-- =============================================
-- CUSTOM TABLES
-- =============================================

-- 1. Categories Table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NULL
);
CREATE INDEX IX_Category_Name ON Categories(Name);
CREATE INDEX IX_Category_IsActive ON Categories(IsActive);

-- 2. Phrases Table
CREATE TABLE Phrases (
    PhraseId INT IDENTITY(1,1) PRIMARY KEY,
    EnglishText NVARCHAR(1000) NOT NULL,
    CategoryId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NULL,
    CONSTRAINT FK_Phrases_Categories FOREIGN KEY (CategoryId) 
        REFERENCES Categories(CategoryId) ON DELETE RESTRICT
);
CREATE INDEX IX_Phrase_EnglishText ON Phrases(EnglishText);
CREATE INDEX IX_Phrase_CategoryId ON Phrases(CategoryId);
CREATE INDEX IX_Phrase_IsActive ON Phrases(IsActive);

-- Optional: Full-text index on EnglishText
-- CREATE FULLTEXT INDEX ON Phrases(EnglishText) 
--     KEY INDEX PK_Phrases 
--     WITH STOPLIST = SYSTEM;

-- 3. Translations Table
CREATE TABLE Translations (
    TranslationId INT IDENTITY(1,1) PRIMARY KEY,
    PhraseId INT NOT NULL,
    Language NVARCHAR(50) NOT NULL,
    Text NVARCHAR(2000) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    RejectionReason NVARCHAR(500) NULL,
    SubmittedBy NVARCHAR(450) NOT NULL,
    ReviewedBy NVARCHAR(450) NULL,
    SubmittedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ReviewedDate DATETIME2 NULL,
    CONSTRAINT FK_Translations_Phrases FOREIGN KEY (PhraseId) 
        REFERENCES Phrases(PhraseId) ON DELETE CASCADE,
    CONSTRAINT FK_Translations_SubmittedBy FOREIGN KEY (SubmittedBy) 
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Translations_ReviewedBy FOREIGN KEY (ReviewedBy) 
        REFERENCES AspNetUsers(Id) ON DELETE SET NULL,
    CONSTRAINT CK_Translation_Status CHECK (Status IN ('Pending', 'Approved', 'Rejected'))
);
CREATE INDEX IX_Translation_PhraseId ON Translations(PhraseId);
CREATE INDEX IX_Translation_Language ON Translations(Language);
CREATE INDEX IX_Translation_Status ON Translations(Status);
CREATE INDEX IX_Translation_SubmittedBy ON Translations(SubmittedBy);
CREATE UNIQUE INDEX UX_Translation_UniqueApproved 
    ON Translations(PhraseId, Language) WHERE Status = 'Approved';

-- Optional: Full-text index on Translation.Text
-- CREATE FULLTEXT INDEX ON Translations(Text) 
--     KEY INDEX PK_Translations 
--     WITH STOPLIST = SYSTEM;

-- 4. Favourites Table
CREATE TABLE Favourites (
    FavouriteId INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    PhraseId INT NOT NULL,
    AddedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Favourites_Users FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Favourites_Phrases FOREIGN KEY (PhraseId) 
        REFERENCES Phrases(PhraseId) ON DELETE CASCADE,
    CONSTRAINT UC_Favourite_Unique UNIQUE (UserId, PhraseId)
);
CREATE INDEX IX_Favourite_UserId ON Favourites(UserId);
CREATE INDEX IX_Favourite_PhraseId ON Favourites(PhraseId);

-- 5. Submissions Table
CREATE TABLE Submissions (
    SubmissionId INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    PhraseId INT NOT NULL,
    Language NVARCHAR(50) NOT NULL,
    SuggestedText NVARCHAR(2000) NOT NULL,
    Status NVARCHAR(20) NOT NULL,
    RejectionReason NVARCHAR(500) NULL,
    CorrectionInstructions NVARCHAR(500) NULL,
    SubmittedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ReviewedDate DATETIME2 NULL,
    CONSTRAINT FK_Submissions_Users FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Submissions_Phrases FOREIGN KEY (PhraseId) 
        REFERENCES Phrases(PhraseId) ON DELETE CASCADE,
    CONSTRAINT CK_Submission_Status CHECK (Status IN ('Pending', 'Approved', 'Rejected', 'CorrectionRequested'))
);
CREATE INDEX IX_Submission_UserId ON Submissions(UserId);
CREATE INDEX IX_Submission_PhraseId ON Submissions(PhraseId);
CREATE INDEX IX_Submission_Status ON Submissions(Status);
CREATE INDEX IX_Submission_Language ON Submissions(Language);

-- 6. LanguageStatistics Table
CREATE TABLE LanguageStatistics (
    StatisticId INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NULL,
    PhraseId INT NULL,
    Language NVARCHAR(50) NULL,
    CategoryId INT NULL,
    EventType NVARCHAR(20) NOT NULL,
    EventDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_LanguageStatistics_Users FOREIGN KEY (UserId) 
        REFERENCES AspNetUsers(Id) ON DELETE SET NULL,
    CONSTRAINT FK_LanguageStatistics_Phrases FOREIGN KEY (PhraseId) 
        REFERENCES Phrases(PhraseId) ON DELETE SET NULL,
    CONSTRAINT FK_LanguageStatistics_Categories FOREIGN KEY (CategoryId) 
        REFERENCES Categories(CategoryId) ON DELETE SET NULL,
    CONSTRAINT CK_LanguageStatistics_EventType CHECK (EventType IN ('View', 'Search'))
);
CREATE INDEX IX_LanguageStatistics_User ON LanguageStatistics(UserId);
CREATE INDEX IX_LanguageStatistics_EventDate ON LanguageStatistics(EventDate);
CREATE INDEX IX_LanguageStatistics_Language ON LanguageStatistics(Language);
CREATE INDEX IX_LanguageStatistics_EventType ON LanguageStatistics(EventType);
CREATE INDEX IX_LanguageStatistics_Category ON LanguageStatistics(CategoryId);
CREATE INDEX IX_LanguageStatistics_Analytics 
    ON LanguageStatistics(EventDate, EventType, Language, CategoryId);

-- =============================================
-- SEED DATA
-- =============================================

-- Seed Categories
INSERT INTO Categories (Name, Description) VALUES
('Registration', 'Phrases related to course registration and enrolment'),
('Accommodation', 'Phrases about university housing and residence life'),
('Health Services', 'Phrases for campus health and wellness services'),
('Library', 'Phrases about library resources and services'),
('Academic Support', 'Phrases for tutoring, academic advising, and support services');

-- Seed Sample Phrases
INSERT INTO Phrases (EnglishText, CategoryId) VALUES
('Where is the registration office?', 1),
('How do I register for courses?', 1),
('Where can I find accommodation?', 2),
('How do I apply for a residence?', 2),
('Where is the campus health centre?', 3),
('How do I make an appointment at the clinic?', 3),
('Where is the library?', 4),
('How do I borrow a book?', 4),
('Where is the academic advising office?', 5),
('How do I get a tutor?', 5);

-- Seed Sample Translations (Approved)
INSERT INTO Translations (PhraseId, Language, Text, Status, SubmittedBy, SubmittedDate) VALUES
(1, 'isiZulu', 'Iphi ihhovisi lokubhalisa?', 'Approved', '1', GETUTCDATE()),
(1, 'isiXhosa', 'Liphi iofisi yokubhalisa?', 'Approved', '1', GETUTCDATE()),
(2, 'isiZulu', 'Ngibhalisa kanjani izifundo?', 'Approved', '1', GETUTCDATE()),
(3, 'isiZulu', 'Ngiyithola kuphi indawo yokuhlala?', 'Approved', '1', GETUTCDATE());
```

### 5.2 ASP.NET Identity Tables

The ASP.NET Identity tables are generated automatically when you run the Entity Framework Core migrations. No manual creation is needed.

---

## 6. Entity Relationship Diagram (Visual)

### 6.1 Diagram Description

**Note:** The visual diagram should be created using a tool like draw.io or DBDiagram. Below is the text-based representation showing tables, columns, and relationships.

```
+---------------------------+        +---------------------------+
|        Categories         |        |         Phrases           |
+---------------------------+        +---------------------------+
| CategoryId (PK)           |<-------| PhraseId (PK)             |
| Name (UK)                 |  1  :M | EnglishText               |
| Description               |        | CategoryId (FK)           |
| IsActive                  |        | IsActive                  |
| CreatedDate               |        | CreatedDate               |
| ModifiedDate              |        | ModifiedDate              |
+---------------------------+        +---------------------------+
         |      ^                             |      ^
         |      |                             |      |
         |      +-----------------------------+      |
         |      | 1 : M          M : 1             |
         |      +----------------------------------+
         |      |                                    |
         |      |                                    |
         v      v                                    v
+---------------------------+        +---------------------------+
|     LanguageStatistics    |        |      Translations         |
+---------------------------+        +---------------------------+
| StatisticId (PK)          |        | TranslationId (PK)        |
| UserId (FK)               |        | PhraseId (FK)             |
| PhraseId (FK)             |        | Language                  |
| CategoryId (FK)           |        | Text                      |
| Language                  |        | Status                    |
| EventType                 |        | RejectionReason           |
| EventDate                 |        | SubmittedBy (FK)          |
+---------------------------+        | ReviewedBy (FK)           |
                                      | SubmittedDate             |
                                      | ReviewedDate              |
                                      +---------------------------+
         |                                      |      ^
         |                                      |      |
         |                                      |      |
         v                                      v      |
+---------------------------+        +---------------------------+
|       AspNetUsers         |        |       Favourites          |
+---------------------------+        +---------------------------+
| Id (PK)                   |<-------| FavouriteId (PK)          |
| UserName                  |  1  :M | UserId (FK)               |
| Email                     |        | PhraseId (FK)             |
| PasswordHash              |        | AddedDate                 |
| ...                       |        +---------------------------+
+---------------------------+        |      ^
         |      ^                     |      |
         |      |                     |      |
         |      +---------------------+      |
         |      | 1 : M          M : 1      |
         |      +----------------------------+
         |
         v
+---------------------------+
|       Submissions         |
+---------------------------+
| SubmissionId (PK)         |
| UserId (FK)               |
| PhraseId (FK)             |
| Language                  |
| SuggestedText             |
| Status                    |
| RejectionReason           |
| CorrectionInstructions    |
| SubmittedDate             |
| ReviewedDate              |
+---------------------------+
```

---

## 7. Summary

### 7.1 Database Statistics

| Metric | Count |
|--------|-------|
| **Custom Tables** | 6 |
| **Identity Tables** | 6 (standard) |
| **Total Tables** | 12 |
| **Primary Keys** | 6 |
| **Foreign Keys** | 10 |
| **Unique Constraints** | 3 |
| **Check Constraints** | 2 |
| **Total Indexes** | 18+ |

### 7.2 Key Business Rules Enforced

| Rule | Enforced By |
|------|-------------|
| Unique category names | Unique constraint on Categories.Name |
| No duplicate favourites | Unique constraint on Favourites (UserId, PhraseId) |
| One approved translation per phrase per language | Filtered unique index on Translations |
| Status must be valid | Check constraints on Translations and Submissions |
| Soft delete protection | IsActive flag on Categories and Phrases |
| Cascade delete for dependent records | Foreign key constraints |

---

**Document Status:** ✅ Complete

**Last Updated:** July 2026

**Next Steps:** Create visual ER diagram using draw.io or DBDiagram

---

## 🔗 Related Issues

- **Blocks:** Issue 3 – Design ER Diagram – Logical Model
- **Depends on:** Issue 1 – Database Design Planning Document
- **Related to:** Issue 5 – Write Initial Database Schema Migration Script

---