-- View Base Database Data
-- Check what students and staff are registered in the system

USE [AASTURegistrationDB]
GO

-- View all students
PRINT '========================================'
PRINT 'STUDENTS IN BASE DATABASE'
PRINT '========================================'
SELECT 
    StudentID,
    FullName,
    UniversityEmail,
    EnrollmentYear,
    Department,
    Role
FROM Students
ORDER BY StudentID;
GO

-- View all staff
PRINT ''
PRINT '========================================'
PRINT 'STAFF IN BASE DATABASE'
PRINT '========================================'
SELECT 
    StaffID,
    FullName,
    Email,
    Department,
    Role
FROM Staff
ORDER BY StaffID;
GO

-- Count students by department
PRINT ''
PRINT '========================================'
PRINT 'STUDENT COUNT BY DEPARTMENT'
PRINT '========================================'
SELECT 
    Department,
    COUNT(*) as StudentCount
FROM Students
GROUP BY Department
ORDER BY Department;
GO

-- Count staff by role
PRINT ''
PRINT '========================================'
PRINT 'STAFF COUNT BY ROLE'
PRINT '========================================'
SELECT 
    Role,
    COUNT(*) as StaffCount
FROM Staff
GROUP BY Role
ORDER BY Role;
GO

-- Check for duplicate emails
PRINT ''
PRINT '========================================'
PRINT 'CHECKING FOR DUPLICATE EMAILS'
PRINT '========================================'
SELECT 
    UniversityEmail,
    COUNT(*) as Count
FROM Students
GROUP BY UniversityEmail
HAVING COUNT(*) > 1;

SELECT 
    Email,
    COUNT(*) as Count
FROM Staff
GROUP BY Email
HAVING COUNT(*) > 1;
GO

PRINT ''
PRINT '========================================'
PRINT 'VERIFICATION COMPLETE'
PRINT '========================================'
GO
