-- =============================================
-- Database: Indigenous Language Phrasebook & Translation Portal
-- Version: 1.0
-- Author: Siyabonga
-- Date: July 2026
-- Description: Complete database schema with foreign keys, indexes, and seed data
-- =============================================

-- =============================================
-- SECTION 1: ASP.NET IDENTITY TABLES
-- =============================================

-- Note: These are the standard ASP.NET Identity tables.
-- If you're using EF Core migrations, these will be generated automatically.
-- If you're running this script manually, ensure these tables exist.

-- AspNetUsers
CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR(450)     NOT NULL,
    [UserName]             NVARCHAR(256)     NULL,
    [NormalizedUserName]   NVARCHAR(256)     NULL,
    [Email]                NVARCHAR(256)     NULL,
    [NormalizedEmail]      NVARCHAR(256)     NULL,
    [EmailConfirmed]       BIT               NOT NULL,
    [PasswordHash]         NVARCHAR(MAX)     NULL,
    [SecurityStamp]        NVARCHAR(MAX)     NULL,
    [ConcurrencyStamp]     NVARCHAR(MAX)     NULL,
    [PhoneNumber]          NVARCHAR(MAX)     NULL,
    [PhoneNumberConfirmed] BIT               NOT NULL,
    [TwoFactorEnabled]     BIT               NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET    NULL,
    [LockoutEnabled]       BIT               NOT NULL,
    [AccessFailedCount]    INT               NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_AspNetUsers_UserName] ON [dbo].[AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
CREATE INDEX [IX_AspNetUsers_Email] ON [dbo].[AspNetUsers] ([NormalizedEmail]);
GO

-- AspNetRoles
CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               NVARCHAR(450)     NOT NULL,
    [Name]             NVARCHAR(256)     NULL,
    [NormalizedName]   NVARCHAR(256)     NULL,
    [ConcurrencyStamp] NVARCHAR(MAX)     NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_AspNetRoles_NormalizedName] ON [dbo].[AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

-- AspNetUserRoles
CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR(450) NOT NULL,
    [RoleId] NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

-- AspNetUserClaims
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT               NOT NULL IDENTITY(1,1),
    [UserId]     NVARCHAR(450)     NOT NULL,
    [ClaimType]  NVARCHAR(MAX)     NULL,
    [ClaimValue] NVARCHAR(MAX)     NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

-- AspNetUserLogins
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider]       NVARCHAR(450) NOT NULL,
    [ProviderKey]         NVARCHAR(450) NOT NULL,
    [ProviderDisplayName] NVARCHAR(MAX) NULL,
    [UserId]              NVARCHAR(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

-- AspNetUserTokens
CREATE TABLE [dbo].[AspNetUserTokens] (
    [UserId]        NVARCHAR(450) NOT NULL,
    [LoginProvider] NVARCHAR(450) NOT NULL,
    [Name]          NVARCHAR(450) NOT NULL,
    [Value]         NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

-- =============================================
-- SECTION 2: CUSTOM APPLICATION TABLES
-- =============================================

-- =============================================
-- Table: Categories
-- =============================================
CREATE TABLE [dbo].[Categories] (
    [CategoryId]   INT               NOT NULL IDENTITY(1,1),
    [Name]         NVARCHAR(100)     NOT NULL,
    [Description]  NVARCHAR(500)     NULL,
    [IsActive]     BIT               NOT NULL CONSTRAINT [DF_Categories_IsActive] DEFAULT (1),
    [CreatedDate]  DATETIME2         NOT NULL CONSTRAINT [DF_Categories_CreatedDate] DEFAULT (GETUTCDATE()),
    [ModifiedDate] DATETIME2         NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId])
);
GO

-- Unique constraint on Name
ALTER TABLE [dbo].[Categories] ADD CONSTRAINT [UC_Categories_Name] UNIQUE ([Name]);
GO

-- Check constraint for IsActive (optional)
ALTER TABLE [dbo].[Categories] ADD CONSTRAINT [CK_Categories_IsActive] CHECK ([IsActive] IN (0, 1));
GO

-- Indexes
CREATE INDEX [IX_Categories_Name] ON [dbo].[Categories] ([Name]);
CREATE INDEX [IX_Categories_IsActive] ON [dbo].[Categories] ([IsActive]);
GO

-- =============================================
-- Table: Phrases
-- =============================================
CREATE TABLE [dbo].[Phrases] (
    [PhraseId]     INT               NOT NULL IDENTITY(1,1),
    [EnglishText]  NVARCHAR(1000)    NOT NULL,
    [CategoryId]   INT               NOT NULL,
    [IsActive]     BIT               NOT NULL CONSTRAINT [DF_Phrases_IsActive] DEFAULT (1),
    [CreatedDate]  DATETIME2         NOT NULL CONSTRAINT [DF_Phrases_CreatedDate] DEFAULT (GETUTCDATE()),
    [ModifiedDate] DATETIME2         NULL,
    CONSTRAINT [PK_Phrases] PRIMARY KEY ([PhraseId])
);
GO

-- Foreign key to Categories
ALTER TABLE [dbo].[Phrases] ADD CONSTRAINT [FK_Phrases_Categories] 
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([CategoryId])
    ON DELETE RESTRICT;  -- Prevent deleting category if phrases exist
GO

-- Check constraint for IsActive
ALTER TABLE [dbo].[Phrases] ADD CONSTRAINT [CK_Phrases_IsActive] CHECK ([IsActive] IN (0, 1));
GO

-- Indexes
CREATE INDEX [IX_Phrases_EnglishText] ON [dbo].[Phrases] ([EnglishText]);
CREATE INDEX [IX_Phrases_CategoryId] ON [dbo].[Phrases] ([CategoryId]);
CREATE INDEX [IX_Phrases_IsActive] ON [dbo].[Phrases] ([IsActive]);
GO

-- =============================================
-- Table: Translations
-- =============================================
CREATE TABLE [dbo].[Translations] (
    [TranslationId]   INT               NOT NULL IDENTITY(1,1),
    [PhraseId]        INT               NOT NULL,
    [Language]        NVARCHAR(50)      NOT NULL,
    [Text]            NVARCHAR(2000)    NOT NULL,
    [Status]          NVARCHAR(20)      NOT NULL,
    [RejectionReason] NVARCHAR(500)     NULL,
    [SubmittedBy]     NVARCHAR(450)     NOT NULL,
    [ReviewedBy]      NVARCHAR(450)     NULL,
    [SubmittedDate]   DATETIME2         NOT NULL CONSTRAINT [DF_Translations_SubmittedDate] DEFAULT (GETUTCDATE()),
    [ReviewedDate]    DATETIME2         NULL,
    CONSTRAINT [PK_Translations] PRIMARY KEY ([TranslationId])
);
GO

-- Foreign keys
ALTER TABLE [dbo].[Translations] ADD CONSTRAINT [FK_Translations_Phrases] 
    FOREIGN KEY ([PhraseId]) REFERENCES [dbo].[Phrases] ([PhraseId])
    ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[Translations] ADD CONSTRAINT [FK_Translations_SubmittedBy] 
    FOREIGN KEY ([SubmittedBy]) REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[Translations] ADD CONSTRAINT [FK_Translations_ReviewedBy] 
    FOREIGN KEY ([ReviewedBy]) REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE SET NULL;
GO

-- Check constraint for Status
ALTER TABLE [dbo].[Translations] ADD CONSTRAINT [CK_Translations_Status] 
    CHECK ([Status] IN ('Pending', 'Approved', 'Rejected'));
GO

-- Filtered unique index: only one approved translation per phrase per language
CREATE UNIQUE INDEX [UX_Translations_UniqueApproved] 
    ON [dbo].[Translations] ([PhraseId], [Language]) 
    WHERE [Status] = 'Approved';
GO

-- Indexes
CREATE INDEX [IX_Translations_PhraseId] ON [dbo].[Translations] ([PhraseId]);
CREATE INDEX [IX_Translations_Language] ON [dbo].[Translations] ([Language]);
CREATE INDEX [IX_Translations_Status] ON [dbo].[Translations] ([Status]);
CREATE INDEX [IX_Translations_SubmittedBy] ON [dbo].[Translations] ([SubmittedBy]);
CREATE INDEX [IX_Translations_ReviewedBy] ON [dbo].[Translations] ([ReviewedBy]);
CREATE INDEX [IX_Translations_SubmittedDate] ON [dbo].[Translations] ([SubmittedDate]);
GO

-- =============================================
-- Table: Favourites
-- =============================================
CREATE TABLE [dbo].[Favourites] (
    [FavouriteId] INT               NOT NULL IDENTITY(1,1),
    [UserId]      NVARCHAR(450)     NOT NULL,
    [PhraseId]    INT               NOT NULL,
    [AddedDate]   DATETIME2         NOT NULL CONSTRAINT [DF_Favourites_AddedDate] DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_Favourites] PRIMARY KEY ([FavouriteId])
);
GO

-- Foreign keys
ALTER TABLE [dbo].[Favourites] ADD CONSTRAINT [FK_Favourites_Users] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[Favourites] ADD CONSTRAINT [FK_Favourites_Phrases] 
    FOREIGN KEY ([PhraseId]) REFERENCES [dbo].[Phrases] ([PhraseId])
    ON DELETE CASCADE;
GO

-- Unique constraint to prevent duplicate favourites
ALTER TABLE [dbo].[Favourites] ADD CONSTRAINT [UC_Favourites_Unique] 
    UNIQUE ([UserId], [PhraseId]);
GO

-- Indexes
CREATE INDEX [IX_Favourites_UserId] ON [dbo].[Favourites] ([UserId]);
CREATE INDEX [IX_Favourites_PhraseId] ON [dbo].[Favourites] ([PhraseId]);
CREATE INDEX [IX_Favourites_AddedDate] ON [dbo].[Favourites] ([AddedDate]);
GO

-- =============================================
-- Table: Submissions
-- =============================================
CREATE TABLE [dbo].[Submissions] (
    [SubmissionId]          INT               NOT NULL IDENTITY(1,1),
    [UserId]                NVARCHAR(450)     NOT NULL,
    [PhraseId]              INT               NOT NULL,
    [Language]              NVARCHAR(50)      NOT NULL,
    [SuggestedText]         NVARCHAR(2000)    NOT NULL,
    [Status]                NVARCHAR(20)      NOT NULL,
    [RejectionReason]       NVARCHAR(500)     NULL,
    [CorrectionInstructions] NVARCHAR(500)    NULL,
    [SubmittedDate]         DATETIME2         NOT NULL CONSTRAINT [DF_Submissions_SubmittedDate] DEFAULT (GETUTCDATE()),
    [ReviewedDate]          DATETIME2         NULL,
    CONSTRAINT [PK_Submissions] PRIMARY KEY ([SubmissionId])
);
GO

-- Foreign keys
ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [FK_Submissions_Users] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE CASCADE;
GO

ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [FK_Submissions_Phrases] 
    FOREIGN KEY ([PhraseId]) REFERENCES [dbo].[Phrases] ([PhraseId])
    ON DELETE CASCADE;
GO

-- Check constraint for Status
ALTER TABLE [dbo].[Submissions] ADD CONSTRAINT [CK_Submissions_Status] 
    CHECK ([Status] IN ('Pending', 'Approved', 'Rejected', 'CorrectionRequested'));
GO

-- Indexes
CREATE INDEX [IX_Submissions_UserId] ON [dbo].[Submissions] ([UserId]);
CREATE INDEX [IX_Submissions_PhraseId] ON [dbo].[Submissions] ([PhraseId]);
CREATE INDEX [IX_Submissions_Status] ON [dbo].[Submissions] ([Status]);
CREATE INDEX [IX_Submissions_Language] ON [dbo].[Submissions] ([Language]);
CREATE INDEX [IX_Submissions_SubmittedDate] ON [dbo].[Submissions] ([SubmittedDate]);
GO

-- =============================================
-- Table: LanguageStatistics
-- =============================================
CREATE TABLE [dbo].[LanguageStatistics] (
    [StatisticId] INT               NOT NULL IDENTITY(1,1),
    [UserId]      NVARCHAR(450)     NULL,
    [PhraseId]    INT               NULL,
    [Language]    NVARCHAR(50)      NULL,
    [CategoryId]  INT               NULL,
    [EventType]   NVARCHAR(20)      NOT NULL,
    [EventDate]   DATETIME2         NOT NULL CONSTRAINT [DF_LanguageStatistics_EventDate] DEFAULT (GETUTCDATE()),
    CONSTRAINT [PK_LanguageStatistics] PRIMARY KEY ([StatisticId])
);
GO

-- Foreign keys
ALTER TABLE [dbo].[LanguageStatistics] ADD CONSTRAINT [FK_LanguageStatistics_Users] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE SET NULL;
GO

ALTER TABLE [dbo].[LanguageStatistics] ADD CONSTRAINT [FK_LanguageStatistics_Phrases] 
    FOREIGN KEY ([PhraseId]) REFERENCES [dbo].[Phrases] ([PhraseId])
    ON DELETE SET NULL;
GO

ALTER TABLE [dbo].[LanguageStatistics] ADD CONSTRAINT [FK_LanguageStatistics_Categories] 
    FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([CategoryId])
    ON DELETE SET NULL;
GO

-- Check constraint for EventType
ALTER TABLE [dbo].[LanguageStatistics] ADD CONSTRAINT [CK_LanguageStatistics_EventType] 
    CHECK ([EventType] IN ('View', 'Search'));
GO

-- Indexes
CREATE INDEX [IX_LanguageStatistics_UserId] ON [dbo].[LanguageStatistics] ([UserId]);
CREATE INDEX [IX_LanguageStatistics_PhraseId] ON [dbo].[LanguageStatistics] ([PhraseId]);
CREATE INDEX [IX_LanguageStatistics_CategoryId] ON [dbo].[LanguageStatistics] ([CategoryId]);
CREATE INDEX [IX_LanguageStatistics_Language] ON [dbo].[LanguageStatistics] ([Language]);
CREATE INDEX [IX_LanguageStatistics_EventType] ON [dbo].[LanguageStatistics] ([EventType]);
CREATE INDEX [IX_LanguageStatistics_EventDate] ON [dbo].[LanguageStatistics] ([EventDate]);

-- Composite index for analytics queries
CREATE INDEX [IX_LanguageStatistics_Analytics] 
    ON [dbo].[LanguageStatistics] ([EventDate], [EventType], [Language], [CategoryId]);
GO

-- =============================================
-- SECTION 3: SEED DATA
-- =============================================

-- =============================================
-- 3.1 Seed Roles
-- =============================================
-- Note: These roles should be inserted via ASP.NET Identity, but here's the SQL for reference
-- INSERT INTO [dbo].[AspNetRoles] (Id, Name, NormalizedName, ConcurrencyStamp) VALUES
-- (NEWID(), 'Student', 'STUDENT', NEWID()),
-- (NEWID(), 'Administrator', 'ADMINISTRATOR', NEWID());

-- =============================================
-- 3.2 Seed Categories
-- =============================================
SET IDENTITY_INSERT [dbo].[Categories] ON;
GO

INSERT INTO [dbo].[Categories] ([CategoryId], [Name], [Description], [IsActive], [CreatedDate]) VALUES
(1, 'Registration', 'Phrases related to course registration and enrolment', 1, GETUTCDATE()),
(2, 'Accommodation', 'Phrases about university housing and residence life', 1, GETUTCDATE()),
(3, 'Health Services', 'Phrases for campus health and wellness services', 1, GETUTCDATE()),
(4, 'Library', 'Phrases about library resources and services', 1, GETUTCDATE()),
(5, 'Academic Support', 'Phrases for tutoring, academic advising, and support services', 1, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[Categories] OFF;
GO

-- Verify Categories
-- SELECT * FROM [dbo].[Categories];

-- =============================================
-- 3.3 Seed Phrases
-- =============================================
SET IDENTITY_INSERT [dbo].[Phrases] ON;
GO

INSERT INTO [dbo].[Phrases] ([PhraseId], [EnglishText], [CategoryId], [IsActive], [CreatedDate]) VALUES
(1, 'Where is the registration office?', 1, 1, GETUTCDATE()),
(2, 'How do I register for courses?', 1, 1, GETUTCDATE()),
(3, 'Where can I find accommodation?', 2, 1, GETUTCDATE()),
(4, 'How do I apply for a residence?', 2, 1, GETUTCDATE()),
(5, 'Where is the campus health centre?', 3, 1, GETUTCDATE()),
(6, 'How do I make an appointment at the clinic?', 3, 1, GETUTCDATE()),
(7, 'Where is the library?', 4, 1, GETUTCDATE()),
(8, 'How do I borrow a book?', 4, 1, GETUTCDATE()),
(9, 'Where is the academic advising office?', 5, 1, GETUTCDATE()),
(10, 'How do I get a tutor?', 5, 1, GETUTCDATE());

SET IDENTITY_INSERT [dbo].[Phrases] OFF;
GO

-- Verify Phrases
-- SELECT p.PhraseId, p.EnglishText, c.Name AS Category
-- FROM [dbo].[Phrases] p
-- JOIN [dbo].[Categories] c ON p.CategoryId = c.CategoryId;

-- =============================================
-- 3.4 Seed Admin User
-- =============================================
-- Note: The admin user should be created via ASP.NET Identity UserManager.
-- The following SQL is for reference only. In production, use UserManager.CreateAsync().

-- Insert admin user (password: Admin@123! - hashed)
-- INSERT INTO [dbo].[AspNetUsers] (
--     Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
--     PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
--     TwoFactorEnabled, LockoutEnabled, AccessFailedCount
-- ) VALUES (
--     '00000000-0000-0000-0000-000000000001',
--     'admin@phrasebook.local',
--     'ADMIN@PHRASEBOOK.LOCAL',
--     'admin@phrasebook.local',
--     'ADMIN@PHRASEBOOK.LOCAL',
--     1,
--     'AQAAAAIAAYagAAAAE...', -- Placeholder - use actual hash
--     'YOUR_SECURITY_STAMP',
--     'YOUR_CONCURRENCY_STAMP',
--     0, 0, 1, 0
-- );

-- -- Assign admin role
-- INSERT INTO [dbo].[AspNetUserRoles] (UserId, RoleId) VALUES (
--     '00000000-0000-0000-0000-000000000001',
--     (SELECT Id FROM [dbo].[AspNetRoles] WHERE NormalizedName = 'ADMINISTRATOR')
-- );

-- =============================================
-- 3.5 Seed Translations (Approved)
-- =============================================
SET IDENTITY_INSERT [dbo].[Translations] ON;
GO

INSERT INTO [dbo].[Translations] (
    [TranslationId], [PhraseId], [Language], [Text], [Status],
    [SubmittedBy], [ReviewedBy], [SubmittedDate], [ReviewedDate]
) VALUES
-- isiZulu translations
(1, 1, 'isiZulu', 'Iphi ihhovisi lokubhalisa?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(2, 2, 'isiZulu', 'Ngibhalisa kanjani izifundo?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(3, 3, 'isiZulu', 'Ngiyithola kuphi indawo yokuhlala?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(4, 4, 'isiZulu', 'Ngifaka kanjani isicelo sendawo yokuhlala?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(5, 5, 'isiZulu', 'Iphi indawo yezempilo?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(6, 6, 'isiZulu', 'Ngenza kanjani isivumelwano emtholampilo?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(7, 7, 'isiZulu', 'Iphi umtapo wolwazi?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(8, 8, 'isiZulu', 'Ngiboleka kanjani incwadi?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(9, 9, 'isiZulu', 'Iphi ihhovisi lokweluleka ngezifundo?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(10, 10, 'isiZulu', 'Ngithola kanjani umfundisi?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
-- isiXhosa translations
(11, 1, 'isiXhosa', 'Liphi iofisi yokubhalisa?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(12, 2, 'isiXhosa', 'Ndibhalisa njani izifundo?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(13, 3, 'isiXhosa', 'Ndiyifumana phi indawo yokuhlala?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(14, 4, 'isiXhosa', 'Ndiyifaka njani isicelo sendawo yokuhlala?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE()),
(15, 5, 'isiXhosa', 'Liphi iziko lezempilo?', 'Approved', '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001', GETUTCDATE(), GETUTCDATE());

SET IDENTITY_INSERT [dbo].[Translations] OFF;
GO

-- Verify Translations
-- SELECT t.TranslationId, p.EnglishText, t.Language, t.Text, t.Status
-- FROM [dbo].[Translations] t
-- JOIN [dbo].[Phrases] p ON t.PhraseId = p.PhraseId;

-- =============================================
-- 3.6 Seed Sample Favourites (Optional)
-- =============================================
-- SET IDENTITY_INSERT [dbo].[Favourites] ON;
-- GO

-- INSERT INTO [dbo].[Favourites] ([FavouriteId], [UserId], [PhraseId], [AddedDate]) VALUES
-- (1, '00000000-0000-0000-0000-000000000001', 1, GETUTCDATE()),
-- (2, '00000000-0000-0000-0000-000000000001', 3, GETUTCDATE()),
-- (3, '00000000-0000-0000-0000-000000000001', 5, GETUTCDATE());

-- SET IDENTITY_INSERT [dbo].[Favourites] OFF;
-- GO

-- =============================================
-- SECTION 4: VERIFICATION QUERIES
-- =============================================

-- Verify all tables exist
-- SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME;

-- Verify foreign keys
-- SELECT 
--     FK.name AS ForeignKeyName,
--     TP.name AS ParentTable,
--     TR.name AS ReferencedTable
-- FROM sys.foreign_keys FK
-- INNER JOIN sys.tables TP ON FK.parent_object_id = TP.object_id
-- INNER JOIN sys.tables TR ON FK.referenced_object_id = TR.object_id
-- ORDER BY ParentTable, ReferencedTable;

-- Verify indexes
-- SELECT 
--     t.name AS TableName,
--     i.name AS IndexName,
--     i.type_desc AS IndexType,
--     STRING_AGG(c.name, ', ') AS Columns
-- FROM sys.indexes i
-- INNER JOIN sys.tables t ON i.object_id = t.object_id
-- INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
-- INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
-- WHERE i.type > 0
-- GROUP BY t.name, i.name, i.type_desc
-- ORDER BY TableName, IndexName;

-- =============================================
-- SECTION 5: CLEANUP SCRIPTS (Optional)
-- =============================================

-- Drop all custom tables (in reverse order due to foreign keys)
-- DROP TABLE IF EXISTS [dbo].[LanguageStatistics];
-- DROP TABLE IF EXISTS [dbo].[Favourites];
-- DROP TABLE IF EXISTS [dbo].[Submissions];
-- DROP TABLE IF EXISTS [dbo].[Translations];
-- DROP TABLE IF EXISTS [dbo].[Phrases];
-- DROP TABLE IF EXISTS [dbo].[Categories];

-- Drop Identity tables (if needed - careful!)
-- DROP TABLE IF EXISTS [dbo].[AspNetUserTokens];
-- DROP TABLE IF EXISTS [dbo].[AspNetUserLogins];
-- DROP TABLE IF EXISTS [dbo].[AspNetUserClaims];
-- DROP TABLE IF EXISTS [dbo].[AspNetUserRoles];
-- DROP TABLE IF EXISTS [dbo].[AspNetRoles];
-- DROP TABLE IF EXISTS [dbo].[AspNetUsers];

-- =============================================
-- END OF SCRIPT
-- =============================================
GO