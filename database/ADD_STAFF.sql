-- Add Staff to Base Database
-- These staff members can sign up for accounts in the system
-- Only staff in this table are allowed to register

USE [AASTURegistrationDB]
GO

-- Example: Add a new staff member
INSERT INTO [dbo].[Staff] 
    ([StaffID], [FullName], [Email], [Department], [Role])
VALUES
    ('ADV003', 'Dr. Michael Chen', 'michael.chen@aastu.edu.et', 'Computer Science', 'Advisor'),
    ('DH003', 'Prof. Maria Garcia', 'maria.garcia@aastu.edu.et', 'Mechanical Engineering', 'DepartmentHead'),
    ('REG002', 'Mr. Thomas Lee', 'thomas.lee@aastu.edu.et', 'Registrar Office', 'Registrar');

-- Verify the insertion
SELECT * FROM [dbo].[Staff] WHERE StaffID IN ('ADV003', 'DH003', 'REG002');
GO

-- Template for adding more staff:
/*
INSERT INTO [dbo].[Staff] 
    ([StaffID], [FullName], [Email], [Department], [Role])
VALUES
    ('STAFFXXX', 'Full Name', 'email@aastu.edu.et', 'Department Name', 'Role');
*/

-- Required Fields:
-- StaffID: Unique identifier (e.g., 'ADV001', 'DH001', 'REG001')
-- FullName: Staff member's full name
-- Email: Must be @aastu.edu.et or @staff.aastu.edu.et
-- Department: Department name
-- Role: One of: 'Advisor', 'DepartmentHead', 'Registrar', 'CostSharingOfficer', 'SystemAdmin'

-- Valid Roles:
-- 'Advisor' - Academic advisor
-- 'DepartmentHead' - Department head
-- 'Registrar' - Registrar office staff
-- 'CostSharingOfficer' - Cost sharing/finance officer
-- 'SystemAdmin' - System administrator
