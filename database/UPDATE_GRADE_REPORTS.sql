-- Update GradeReports table to add new fields for grade report feature
-- Run this script in SQL Server Management Studio or via sqlcmd

USE AASTURegistrationDB;
GO

-- Add new columns if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'StudentName')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [StudentName] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'Major')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [Major] NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'AdmissionClassification')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [AdmissionClassification] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'Program')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [Program] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'Year')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [Year] INT NOT NULL DEFAULT 1;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'AcademicYear')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [AcademicYear] NVARCHAR(50) NOT NULL DEFAULT '';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'PreviousCredit')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [PreviousCredit] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'PreviousGP')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [PreviousGP] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'PreviousANG')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [PreviousANG] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'SemesterCredit')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [SemesterCredit] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'SemesterGP')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [SemesterGP] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'SemesterANG')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [SemesterANG] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'CumulativeCredit')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [CumulativeCredit] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'CumulativeGP')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [CumulativeGP] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'CumulativeANG')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [CumulativeANG] DECIMAL(18,2) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'Remark')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [Remark] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'RegistrarRecorderName')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [RegistrarRecorderName] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'RegistrarSignedDate')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [RegistrarSignedDate] DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'GeneratedBy')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [GeneratedBy] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'GeneratedAt')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [GeneratedAt] DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'Status')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [Status] NVARCHAR(50) NOT NULL DEFAULT 'Created';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'ApprovedBy')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [ApprovedBy] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'ApprovedAt')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [ApprovedAt] DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GradeReports]') AND name = 'RejectionReason')
BEGIN
    ALTER TABLE [dbo].[GradeReports]
    ADD [RejectionReason] NVARCHAR(500) NULL;
END
GO

PRINT 'GradeReports table updated successfully!';
GO
