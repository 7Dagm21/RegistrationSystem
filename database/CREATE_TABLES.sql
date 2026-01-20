-- AASTU Registration System - Database Schema
-- This script creates all tables manually (optional - EF Core does this automatically)
-- Use this if you prefer manual table creation or need to understand the schema

USE [AASTURegistrationDB]
GO

-- =============================================
-- Table: Students (Base Data)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Students]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Students] (
        [StudentID] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [FullName] NVARCHAR(100) NOT NULL,
        [UniversityEmail] NVARCHAR(100) NOT NULL,
        [EnrollmentYear] INT NOT NULL,
        [Department] NVARCHAR(100) NOT NULL,
        [Role] NVARCHAR(50) NOT NULL DEFAULT 'Student'
    );
    
    CREATE INDEX IX_Students_Email ON [dbo].[Students]([UniversityEmail]);
END
GO

-- =============================================
-- Table: Staff (Base Data)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Staff]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Staff] (
        [StaffID] NVARCHAR(50) NOT NULL PRIMARY KEY,
        [FullName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [Department] NVARCHAR(100) NOT NULL,
        [Role] NVARCHAR(50) NOT NULL
    );
    
    CREATE INDEX IX_Staff_Email ON [dbo].[Staff]([Email]);
END
GO

-- =============================================
-- Table: Users (System Accounts)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [FullName] NVARCHAR(100) NOT NULL,
        [UserId] NVARCHAR(50) NOT NULL,
        [Email] NVARCHAR(100) NOT NULL,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [Role] NVARCHAR(50) NOT NULL,
        [Department] NVARCHAR(100) NULL,
        [IsEmailVerified] BIT NOT NULL DEFAULT 0,
        [EmailVerificationToken] NVARCHAR(MAX) NULL,
        [EmailVerificationTokenExpiry] DATETIME2 NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastLoginAt] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    );
    
    CREATE UNIQUE INDEX IX_Users_Email ON [dbo].[Users]([Email]);
    CREATE UNIQUE INDEX IX_Users_UserId ON [dbo].[Users]([UserId]);
END
GO

-- =============================================
-- Table: Courses
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Courses]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Courses] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CourseCode] NVARCHAR(20) NOT NULL,
        [CourseName] NVARCHAR(200) NOT NULL,
        [CreditHours] INT NOT NULL,
        [AcademicYear] INT NOT NULL,
        [Department] NVARCHAR(100) NOT NULL,
        [Semester] NVARCHAR(20) NULL
    );
    
    CREATE INDEX IX_Courses_Department_Year ON [dbo].[Courses]([Department], [AcademicYear]);
END
GO

-- =============================================
-- Table: RegistrationSlips
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegistrationSlips]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RegistrationSlips] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [StudentID] NVARCHAR(50) NOT NULL,
        [StudentName] NVARCHAR(100) NOT NULL,
        [Department] NVARCHAR(100) NOT NULL,
        [Semester] NVARCHAR(20) NOT NULL,
        [AcademicYear] INT NOT NULL,
        [CoursesJson] NVARCHAR(MAX) NOT NULL DEFAULT '[]',
        [TotalCreditHours] INT NOT NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Created',
        [IsAdvisorApproved] BIT NOT NULL DEFAULT 0,
        [AdvisorID] NVARCHAR(50) NULL,
        [AdvisorComment] NVARCHAR(500) NULL,
        [AdvisorApprovedAt] DATETIME2 NULL,
        [IsCostSharingVerified] BIT NOT NULL DEFAULT 0,
        [CostSharingOfficerID] NVARCHAR(50) NULL,
        [CostSharingVerifiedAt] DATETIME2 NULL,
        [IsRegistrarFinalized] BIT NOT NULL DEFAULT 0,
        [RegistrarID] NVARCHAR(50) NULL,
        [RegistrarFinalizedAt] DATETIME2 NULL,
        [QrCodeData] NVARCHAR(MAX) NULL,
        [SerialNumber] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsLocked] BIT NOT NULL DEFAULT 0
    );
    
    CREATE INDEX IX_RegistrationSlips_StudentID ON [dbo].[RegistrationSlips]([StudentID]);
    CREATE UNIQUE INDEX IX_RegistrationSlips_SerialNumber ON [dbo].[RegistrationSlips]([SerialNumber]) WHERE [SerialNumber] IS NOT NULL;
END
GO

-- =============================================
-- Table: CostSharingForms
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[CostSharingForms] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [RegistrationSlipId] INT NOT NULL,
        [StudentID] NVARCHAR(50) NOT NULL,
        [PhotoPath] NVARCHAR(500) NULL,
        [PaymentInfo] NVARCHAR(500) NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        [VerifiedBy] NVARCHAR(50) NULL,
        [VerifiedAt] DATETIME2 NULL,
        [SubmittedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        FOREIGN KEY ([RegistrationSlipId]) REFERENCES [dbo].[RegistrationSlips]([Id])
    );
    
    CREATE INDEX IX_CostSharingForms_RegistrationSlipId ON [dbo].[CostSharingForms]([RegistrationSlipId]);
    CREATE INDEX IX_CostSharingForms_StudentID ON [dbo].[CostSharingForms]([StudentID]);
END
GO

-- =============================================
-- Table: GradeReports
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[GradeReports] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [StudentID] NVARCHAR(50) NOT NULL,
        [Semester] NVARCHAR(20) NOT NULL,
        [AcademicYear] INT NOT NULL,
        [GradesJson] NVARCHAR(MAX) NOT NULL DEFAULT '[]',
        [GPA] DECIMAL(5,2) NULL,
        [CGPA] DECIMAL(5,2) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_GradeReports_StudentID ON [dbo].[GradeReports]([StudentID]);
END
GO

-- =============================================
-- Table: AuditLogs
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditLogs] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserID] NVARCHAR(50) NOT NULL,
        [UserRole] NVARCHAR(100) NOT NULL,
        [Action] NVARCHAR(100) NOT NULL,
        [Details] NVARCHAR(500) NULL,
        [IPAddress] NVARCHAR(50) NULL,
        [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_AuditLogs_UserID ON [dbo].[AuditLogs]([UserID]);
    CREATE INDEX IX_AuditLogs_Timestamp ON [dbo].[AuditLogs]([Timestamp]);
END
GO

PRINT 'All tables created successfully!'
GO
