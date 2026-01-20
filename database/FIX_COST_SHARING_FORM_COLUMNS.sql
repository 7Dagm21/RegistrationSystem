-- Fix CostSharingForms table - Add ALL missing columns
-- Run this script in SQL Server Management Studio or via sqlcmd

USE AASTURegistrationDB;
GO

-- Add missing columns that should exist from the original model
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'TuitionFee15Percent')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [TuitionFee15Percent] DECIMAL(18,2) NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'FoodExpense')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [FoodExpense] DECIMAL(18,2) NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'BoardingExpense')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [BoardingExpense] DECIMAL(18,2) NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'TotalCost')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [TotalCost] DECIMAL(18,2) NOT NULL DEFAULT 0;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'ServiceSelection')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [ServiceSelection] NVARCHAR(500) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE();
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'PdfPath')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [PdfPath] NVARCHAR(500) NULL;
END
GO

-- Add new columns for paper-form fields
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'PhotoDataUrl')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [PhotoDataUrl] NVARCHAR(MAX) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'FullName')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [FullName] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'IdentityNo')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [IdentityNo] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'Sex')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [Sex] NVARCHAR(10) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'Nationality')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [Nationality] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'DateOfBirth')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [DateOfBirth] DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'PlaceOfBirth')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [PlaceOfBirth] NVARCHAR(500) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'MothersFullName')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [MothersFullName] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'MothersAddress')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [MothersAddress] NVARCHAR(500) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'SchoolName')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [SchoolName] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'DateCompleted')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [DateCompleted] DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'FacultyOrCollege')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [FacultyOrCollege] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'Department')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [Department] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'EntranceYearEC')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [EntranceYearEC] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'AcademicYearText')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [AcademicYearText] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'SemesterText')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [SemesterText] NVARCHAR(50) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'AdvancePaymentDate')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [AdvancePaymentDate] DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'Discount')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [Discount] NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'ReceiptNo')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [ReceiptNo] NVARCHAR(100) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'BeneficiarySignatureName')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [BeneficiarySignatureName] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'BeneficiarySignedAt')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [BeneficiarySignedAt] DATETIME2 NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'InstituteRepresentativeName')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [InstituteRepresentativeName] NVARCHAR(200) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[CostSharingForms]') AND name = 'InstituteSignedAt')
BEGIN
    ALTER TABLE [dbo].[CostSharingForms]
    ADD [InstituteSignedAt] DATETIME2 NULL;
END
GO

PRINT 'CostSharingForms table fixed successfully! All columns added.';
GO
