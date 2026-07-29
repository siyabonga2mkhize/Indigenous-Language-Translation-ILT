-- List all tables in your database
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Seed Categories with IsActive
INSERT INTO Categories (Name, IsActive) VALUES 
('Greetings', 1),
('Everyday Phrases', 1),
('Academic', 1),
('Business', 1),
('Travel', 1),
('Emergency', 1),
('Food & Dining', 1),
('Family & Relationships', 1);
GO

-- Verify Categories
SELECT * FROM Categories;


-- Create Faculties table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Faculties')
BEGIN
    CREATE TABLE Faculties (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL
    );
    PRINT 'Faculties table created';
END
GO

-- Seed Faculties
INSERT INTO Faculties (Name) VALUES 
('Faculty of Engineering'),
('Faculty of Sciences'),
('Faculty of Humanities'),
('Faculty of Commerce'),
('Faculty of Education');
GO

SELECT * FROM Faculties;


-- Create Campuses table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Campuses')
BEGIN
    CREATE TABLE Campuses (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NOT NULL
    );
    PRINT 'Campuses table created';
END
GO

-- Seed Campuses
INSERT INTO Campuses (Name) VALUES 
('Main Campus'),
('City Campus'),
('South Campus'),
('North Campus'),
('Online Campus');
GO

SELECT * FROM Campuses;

-- First, check what columns exist in AspNetUsers
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetUsers';

-- Then insert users with only existing columns
-- If you have different column names, adjust accordingly
INSERT INTO AspNetUsers (
    Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
    EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
    PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount
) VALUES 
(
    NEWID(), 'admin@innodevs.com', 'ADMIN@INNODEVS.COM', 
    'admin@innodevs.com', 'ADMIN@INNODEVS.COM', 1,
    'AQAAAAIAAYagAAAAEIPQ0z0LxJfH5J2Pt1Lw2V3N4Q2M7Z8K1J6Y9U0I0N1O2P3Q4R5S6T7U8V9W0X1Y2Z3=', 
    NEWID(), NEWID(),
    0, 0, 1, 0
),
(
    NEWID(), 'student@test.com', 'STUDENT@TEST.COM', 
    'student@test.com', 'STUDENT@TEST.COM', 1,
    'AQAAAAIAAYagAAAAEKpB4N1M2X7Y8J0K3L4M5N6O7P8Q9R0S1T2U3V4W5X6Y7Z8A9B0C1D2E3F4G5H6I=', 
    NEWID(), NEWID(),
    0, 0, 1, 0
),
(
    NEWID(), 'teacher@test.com', 'TEACHER@TEST.COM', 
    'teacher@test.com', 'TEACHER@TEST.COM', 1,
    'AQAAAAIAAYagAAAAELM6N7O8P9Q0R1S2T3U4V5W6X7Y8Z9A0B1C2D3E4F5G6H7I8J9K0L1M2N3O4P5Q=', 
    NEWID(), NEWID(),
    0, 0, 1, 0
);
GO

-- Verify Users
SELECT Id, UserName, Email FROM AspNetUsers;

s

-- Seed Roles
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES 
(NEWID(), 'Admin', 'ADMIN', NEWID()),
(NEWID(), 'Student', 'STUDENT', NEWID()),
(NEWID(), 'Teacher', 'TEACHER', NEWID());
GO

SELECT * FROM AspNetRoles;

-- Assign roles to users
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
CROSS JOIN AspNetRoles r
WHERE (u.Email = 'admin@innodevs.com' AND r.Name = 'Admin')
   OR (u.Email = 'student@test.com' AND r.Name = 'Student')
   OR (u.Email = 'teacher@test.com' AND r.Name = 'Teacher');
GO

-- Verify user roles
SELECT u.Email, r.Name as Role
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id;

-- Check if Submissions table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Submissions')
BEGIN
    DECLARE @StudentUserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email = 'student@test.com');
    DECLARE @PhraseId INT = (SELECT TOP 1 Id FROM Phrases);

    INSERT INTO Submissions (UserId, SubmittedText, SubmittedAt, IsApproved, ReviewedBy, PhraseId) VALUES 
    (@StudentUserId, 'Ngiyaxolisa (I''m sorry)', DATEADD(day, -2, GETUTCDATE()), 0, NULL, @PhraseId),
    (@StudentUserId, 'Ngicela usizo (Please help me)', DATEADD(day, -1, GETUTCDATE()), 0, NULL, @PhraseId);
    
    PRINT 'Submissions seeded';
END
ELSE
BEGIN
    PRINT 'Submissions table does not exist - skipping';
END
GO

-- First, check what columns exist in Phrases
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Phrases';

-- If Language and Transcription don't exist, use the correct column names
-- Example: If the columns are 'PhraseText' and 'Translation' instead of 'EnglishText' and 'Language'
INSERT INTO Phrases (EnglishText, Language, Transcription, IsActive, CategoryId) VALUES 
('Hello', 'Zulu', 'Sawubona', 1, 1),
('How are you?', 'Zulu', 'Unjani?', 1, 1),
('Good morning', 'Zulu', 'Sawubona ekuseni', 1, 1),
('Thank you', 'Zulu', 'Ngiyabonga', 1, 2),
('Yes', 'Zulu', 'Yebo', 1, 2),
('No', 'Zulu', 'Cha', 1, 2);
GO

-- If the column names are different, use this alternative:
-- INSERT INTO Phrases (EnglishText, CategoryId) VALUES 
-- ('Hello', 1),
-- ('How are you?', 1),
-- ('Good morning', 1),
-- ('Thank you', 2),
-- ('Yes', 2),
-- ('No', 2);

-- First, check what columns exist in Translations
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Translations';

-- If Text and IsApproved don't exist, use the correct column names
INSERT INTO Translations (Text, Language, IsApproved, PhraseId) VALUES 
('Sawubona', 'Zulu', 1, 1),
('Molo', 'Xhosa', 1, 1),
('Hallo', 'Afrikaans', 1, 1),
('Unjani?', 'Zulu', 1, 2),
('Uphi?', 'Xhosa', 1, 2),
('Hoe gaan dit?', 'Afrikaans', 1, 2);
GO


-- Check AspNetUsers columns
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetUsers'
ORDER BY ORDINAL_POSITION;

-- Check Phrases columns
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Phrases'
ORDER BY ORDINAL_POSITION;

-- Check Translations columns
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Translations'
ORDER BY ORDINAL_POSITION;

-- Check Categories columns
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Categories'
ORDER BY ORDINAL_POSITION;


-- Example with different column names
INSERT INTO Phrases (EnglishText, NativeText, Pronunciation, IsActive, CategoryId) VALUES 
('Hello', 'Sawubona', 'sa-woo-bo-na', 1, 1),
('How are you?', 'Unjani?', 'oon-ja-nee', 1, 1);


INSERT INTO Faculties (Name) VALUES 
('Faculty of Engineering'),
('Faculty of Sciences'),
('Faculty of Humanities'),
('Faculty of Commerce'),
('Faculty of Education');
GO