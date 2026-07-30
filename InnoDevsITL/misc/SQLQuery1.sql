-- ========================================
-- COMPLETE SEED SCRIPT - CORRECTED FOR YOUR TABLE STRUCTURE
-- ========================================

-- 1. Insert Faculties
INSERT INTO Faculties (Name) VALUES 
('Faculty of Engineering'),
('Faculty of Sciences'),
('Faculty of Humanities'),
('Faculty of Commerce'),
('Faculty of Education');
GO

-- 2. Insert Campuses
INSERT INTO Campuses (Name) VALUES 
('Main Campus'),
('City Campus'),
('South Campus'),
('North Campus'),
('Online Campus');
GO

-- Insert Categories
INSERT INTO Categories (Name) VALUES 
('Greetings'),
('Everyday Phrases'),
('Academic'),
('Business'),
('Travel'),
('Emergency'),
('Food & Dining'),
('Family & Relationships');
GO

-- Verify Categories
SELECT * FROM Categories;


-- 4. Insert Roles
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES 
(NEWID(), 'Admin', 'ADMIN', NEWID()),
(NEWID(), 'Student', 'STUDENT', NEWID()),
(NEWID(), 'Teacher', 'TEACHER', NEWID());
GO

-- 5. Insert Users (with ALL required fields)
DECLARE @AdminId NVARCHAR(450) = NEWID();
DECLARE @StudentId NVARCHAR(450) = NEWID();
DECLARE @TeacherId NVARCHAR(450) = NEWID();

INSERT INTO AspNetUsers (
    Id, 
    FirstName, 
    LastName, 
    DateOfBirth, 
    PhysicalAddress, 
    FacultyId, 
    CampusId,
    UserName, 
    NormalizedUserName, 
    Email, 
    NormalizedEmail, 
    EmailConfirmed, 
    PasswordHash, 
    SecurityStamp, 
    ConcurrencyStamp,
    PhoneNumberConfirmed, 
    TwoFactorEnabled, 
    LockoutEnabled, 
    AccessFailedCount
) VALUES 
(
    @AdminId, 
    'System', 
    'Administrator', 
    '1980-01-01', 
    '123 Admin Street', 
    1, 
    1,
    'admin@innodevs.com', 
    'ADMIN@INNODEVS.COM', 
    'admin@innodevs.com', 
    'ADMIN@INNODEVS.COM', 
    1, 
    'AQAAAAIAAYagAAAAEIPQ0z0LxJfH5J2Pt1Lw2V3N4Q2M7Z8K1J6Y9U0I0N1O2P3Q4R5S6T7U8V9W0X1Y2Z3=', 
    NEWID(), 
    NEWID(),
    0, 
    0, 
    1, 
    0
),
(
    @StudentId, 
    'Test', 
    'Student', 
    '2000-05-15', 
    '456 Student Avenue', 
    1, 
    1,
    'student@test.com', 
    'STUDENT@TEST.COM', 
    'student@test.com', 
    'STUDENT@TEST.COM', 
    1, 
    'AQAAAAIAAYagAAAAEKpB4N1M2X7Y8J0K3L4M5N6O7P8Q9R0S1T2U3V4W5X6Y7Z8A9B0C1D2E3F4G5H6I=', 
    NEWID(), 
    NEWID(),
    0, 
    0, 
    1, 
    0
),
(
    @TeacherId, 
    'Test', 
    'Teacher', 
    '1985-08-20', 
    '789 Teacher Lane', 
    1, 
    1,
    'teacher@test.com', 
    'TEACHER@TEST.COM', 
    'teacher@test.com', 
    'TEACHER@TEST.COM', 
    1, 
    'AQAAAAIAAYagAAAAELM6N7O8P9Q0R1S2T3U4V5W6X7Y8Z9A0B1C2D3E4F5G6H7I8J9K0L1M2N3O4P5Q=', 
    NEWID(), 
    NEWID(),
    0, 
    0, 
    1, 
    0
);
GO

-- 6. Assign Roles to Users
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
CROSS JOIN AspNetRoles r
WHERE (u.Email = 'admin@innodevs.com' AND r.Name = 'Admin')
   OR (u.Email = 'student@test.com' AND r.Name = 'Student')
   OR (u.Email = 'teacher@test.com' AND r.Name = 'Teacher');
GO




-- Insert Phrases
INSERT INTO Phrases (EnglishText, Language, Transcription, IsActive, CategoryId) VALUES 
('Hello', 'Zulu', 'Sawubona', 1, 1),
('How are you?', 'Zulu', 'Unjani?', 1, 1),
('Good morning', 'Zulu', 'Sawubona ekuseni', 1, 1),
('Thank you', 'Zulu', 'Ngiyabonga', 1, 2),
('Yes', 'Zulu', 'Yebo', 1, 2),
('No', 'Zulu', 'Cha', 1, 2),
('Please', 'Zulu', 'Ngiyacela', 1, 2),
('Sorry', 'Zulu', 'Ngiyaxolisa', 1, 2),
('Goodbye', 'Zulu', 'Hamba kahle', 1, 1),
('Welcome', 'Zulu', 'Siyakwamukela', 1, 1),
('What is your name?', 'Zulu', 'Ungubani igama lakho?', 1, 1),
('My name is', 'Zulu', 'Igama lami ngu', 1, 1),
('I love you', 'Zulu', 'Ngiyakuthanda', 1, 2),
('Help me', 'Zulu', 'Ngisiza', 1, 2),
('Where is the bathroom?', 'Zulu', 'Likuphi indlu yangasese?', 1, 5);
GO

-- Verify Phrases
SELECT * FROM Phrases;
-- Insert Translations with correct Phrase IDs
INSERT INTO Translations (Text, Language, IsApproved, PhraseId) VALUES 
-- Hello (PhraseId = 5)
('Sawubona', 'Zulu', 1, 5),
('Molo', 'Xhosa', 1, 5),
('Hallo', 'Afrikaans', 1, 5),

-- How are you? (PhraseId = 6)
('Unjani?', 'Zulu', 1, 6),
('Uphi?', 'Xhosa', 1, 6),
('Hoe gaan dit?', 'Afrikaans', 1, 6),

-- Good morning (PhraseId = 7)
('Sawubona ekuseni', 'Zulu', 1, 7),
('Molo kusasa', 'Xhosa', 1, 7),
('Goeie more', 'Afrikaans', 1, 7),

-- Thank you (PhraseId = 8)
('Ngiyabonga', 'Zulu', 1, 8),
('Enkosi', 'Xhosa', 1, 8),
('Dankie', 'Afrikaans', 1, 8),

-- Yes (PhraseId = 9)
('Yebo', 'Zulu', 1, 9),
('Ewe', 'Xhosa', 1, 9),
('Ja', 'Afrikaans', 1, 9),

-- No (PhraseId = 10)
('Cha', 'Zulu', 1, 10),
('Hayi', 'Xhosa', 1, 10),
('Nee', 'Afrikaans', 1, 10),

-- Please (PhraseId = 11)
('Ngiyacela', 'Zulu', 1, 11),
('Ndiyacela', 'Xhosa', 1, 11),
('Asseblief', 'Afrikaans', 1, 11),

-- Sorry (PhraseId = 12)
('Ngiyaxolisa', 'Zulu', 1, 12),
('Ndiyaxolisa', 'Xhosa', 1, 12),
('Jammer', 'Afrikaans', 1, 12),

-- Goodbye (PhraseId = 13)
('Hamba kahle', 'Zulu', 1, 13),
('Hamba kakuhle', 'Xhosa', 1, 13),
('Totsiens', 'Afrikaans', 1, 13),

-- Welcome (PhraseId = 14)
('Siyakwamukela', 'Zulu', 1, 14),
('Wamkelekile', 'Xhosa', 1, 14),
('Welkom', 'Afrikaans', 1, 14),

-- What is your name? (PhraseId = 15)
('Ungubani igama lakho?', 'Zulu', 1, 15),
('Ngubani igama lakho?', 'Xhosa', 1, 15),
('Wat is jou naam?', 'Afrikaans', 1, 15),

-- My name is (PhraseId = 16)
('Igama lami ngu', 'Zulu', 1, 16),
('Igama lam ndingu', 'Xhosa', 1, 16),
('My naam is', 'Afrikaans', 1, 16),

-- I love you (PhraseId = 17)
('Ngiyakuthanda', 'Zulu', 1, 17),
('Ndiyakuthanda', 'Xhosa', 1, 17),
('Ek is lief vir jou', 'Afrikaans', 1, 17),

-- Help me (PhraseId = 18)
('Ngisiza', 'Zulu', 1, 18),
('Ndince', 'Xhosa', 1, 18),
('Help my', 'Afrikaans', 1, 18),

-- Where is the bathroom? (PhraseId = 19)
('Likuphi indlu yangasese?', 'Zulu', 1, 19),
('Iphi indlu yangasese?', 'Xhosa', 1, 19),
('Waar is die badkamer?', 'Afrikaans', 1, 19);
GO

-- Verify Translations
SELECT * FROM Translations;
-- 9. Verify Everything
SELECT 'Faculties' as TableName, COUNT(*) as Count FROM Faculties
UNION ALL
SELECT 'Campuses', COUNT(*) FROM Campuses
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'AspNetRoles', COUNT(*) FROM AspNetRoles
UNION ALL
SELECT 'AspNetUsers', COUNT(*) FROM AspNetUsers
UNION ALL
SELECT 'Phrases', COUNT(*) FROM Phrases
UNION ALL
SELECT 'Translations', COUNT(*) FROM Translations;

-- Show users
SELECT Id, UserName, Email, FirstName, LastName FROM AspNetUsers;

-- Show user roles
SELECT u.Email, r.Name as Role
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id;

-- Insert Phrases
INSERT INTO Phrases (EnglishText, CategoryId) VALUES 
('Hello', 1),
('How are you?', 1),
('Good morning', 1),
('Thank you', 2),
('Yes', 2),
('No', 2),
('Please', 2),
('Sorry', 2),
('Goodbye', 1),
('Welcome', 1)
('What is your name?', 1),
('My name is', 1),
('I love you', 2),
('Help me', 2),
('Where is the bathroom?', 5);
GO

-- Verify Phrases
SELECT * FROM Phrases;

-- Check Categories columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Categories'
ORDER BY ORDINAL_POSITION;

-- Check Phrases columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Phrases'
ORDER BY ORDINAL_POSITION;

-- Check Translations columns
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Translations'
ORDER BY ORDINAL_POSITION;

-- Check all phrases with their IDs
SELECT Id, EnglishText, Language, CategoryId 
FROM Phrases 
ORDER BY Id;



-- Add more phrases
INSERT INTO Phrases (EnglishText, Language, Transcription, IsActive, CategoryId) VALUES 
('How much does it cost?', 'Zulu', 'Kubiza malini?', 1, 5),
('I would like water', 'Zulu', 'Ngicela amanzi', 1, 7),
('What time is it?', 'Zulu', 'Isikhathi sini?', 1, 5),
('Delicious', 'Zulu', 'Kumnandi', 1, 7),
('Beautiful', 'Zulu', 'Kuhle', 1, 8);
GO

-- Check the new IDs
SELECT Id, EnglishText, Language, CategoryId 
FROM Phrases 
WHERE Id > 19
ORDER BY Id;
GO

-- Add translations for new phrases (replace IDs with actual ones from above query)
INSERT INTO Translations (Text, Language, IsApproved, PhraseId) VALUES 
-- How much does it cost? (replace 20 with actual ID)
('Kubiza malini?', 'Zulu', 1, 20),
('Kubiza malini?', 'Xhosa', 1, 20),
('Hoeveel kos dit?', 'Afrikaans', 1, 20),

-- I would like water (replace 21 with actual ID)
('Ngicela amanzi', 'Zulu', 1, 21),
('Ndicela amanzi', 'Xhosa', 1, 21),
('Ek wil water hê', 'Afrikaans', 1, 21),

-- What time is it? (replace 22 with actual ID)
('Isikhathi sini?', 'Zulu', 1, 22),
('Liliphi ixesha?', 'Xhosa', 1, 22),
('Hoe laat is dit?', 'Afrikaans', 1, 22),

-- Delicious (replace 23 with actual ID)
('Kumnandi', 'Zulu', 1, 23),
('Kumnandi', 'Xhosa', 1, 23),
('Heerlik', 'Afrikaans', 1, 23),

-- Beautiful (replace 24 with actual ID)
('Kuhle', 'Zulu', 1, 24),
('Kuhle', 'Xhosa', 1, 24),
('Pragtig', 'Afrikaans', 1, 24);
GO






------------
DELETE FROM [AspNetUserLogins];
DELETE FROM [AspNetUserClaims];
DELETE FROM [AspNetUserTokens];
DELETE FROM [AspNetUserRoles];
DELETE FROM [AspNetUsers];
DELETE FROM [AspNetRoles];


-- Drop the old database
DROP DATABASE [InnoDevsITL];
GO



SELECT * FROM AspNetRoles;
SELECT ur.*, u.Email FROM AspNetUserRoles ur
JOIN AspNetUsers u ON ur.UserId = u.Id;