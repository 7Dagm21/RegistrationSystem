-- AASTU Registration System - Database Verification Script
-- Run this script to verify database, tables, and data are set up correctly

USE [AASTURegistrationDB]
GO

PRINT '========================================'
PRINT 'Database Verification Report'
PRINT '========================================'
PRINT ''

-- Check if database exists
IF DB_NAME() = 'AASTURegistrationDB'
BEGIN
    PRINT '✓ Database AASTURegistrationDB exists'
END
ELSE
BEGIN
    PRINT '✗ Database AASTURegistrationDB NOT found!'
    RETURN
END
GO

-- List all tables
PRINT ''
PRINT 'Tables in database:'
PRINT '----------------------------------------'
SELECT 
    TABLE_NAME as [Table Name],
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as [Columns]
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME
GO

-- Check table row counts
PRINT ''
PRINT 'Row counts:'
PRINT '----------------------------------------'

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Students')
BEGIN
    DECLARE @StudentCount INT = (SELECT COUNT(*) FROM [dbo].[Students])
    PRINT 'Students: ' + CAST(@StudentCount AS VARCHAR(10)) + ' rows'
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Staff')
BEGIN
    DECLARE @StaffCount INT = (SELECT COUNT(*) FROM [dbo].[Staff])
    PRINT 'Staff: ' + CAST(@StaffCount AS VARCHAR(10)) + ' rows'
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Courses')
BEGIN
    DECLARE @CourseCount INT = (SELECT COUNT(*) FROM [dbo].[Courses])
    PRINT 'Courses: ' + CAST(@CourseCount AS VARCHAR(10)) + ' rows'
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    DECLARE @UserCount INT = (SELECT COUNT(*) FROM [dbo].[Users])
    PRINT 'Users: ' + CAST(@UserCount AS VARCHAR(10)) + ' rows'
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RegistrationSlips')
BEGIN
    DECLARE @SlipCount INT = (SELECT COUNT(*) FROM [dbo].[RegistrationSlips])
    PRINT 'RegistrationSlips: ' + CAST(@SlipCount AS VARCHAR(10)) + ' rows'
END

-- Check indexes
PRINT ''
PRINT 'Indexes:'
PRINT '----------------------------------------'
SELECT 
    t.name AS [Table Name],
    i.name AS [Index Name],
    i.type_desc AS [Index Type]
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name IS NOT NULL
ORDER BY t.name, i.name
GO

-- Verify seed data
PRINT ''
PRINT 'Seed Data Verification:'
PRINT '----------------------------------------'

IF EXISTS (SELECT * FROM [dbo].[Students] WHERE StudentID = 'STU001')
    PRINT '✓ Student STU001 exists'
ELSE
    PRINT '✗ Student STU001 NOT found'

IF EXISTS (SELECT * FROM [dbo].[Staff] WHERE StaffID = 'DH001')
    PRINT '✓ Department Head DH001 exists'
ELSE
    PRINT '✗ Department Head DH001 NOT found'

IF EXISTS (SELECT * FROM [dbo].[Courses] WHERE CourseCode = 'CS101')
    PRINT '✓ Course CS101 exists'
ELSE
    PRINT '✗ Course CS101 NOT found'

PRINT ''
PRINT '========================================'
PRINT 'Verification Complete!'
PRINT '========================================'
GO
