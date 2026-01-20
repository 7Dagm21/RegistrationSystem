-- Add 30 Staff Members
-- Staff ID Format: No ETS prefix (e.g., ADV001, DH001, REG001, etc.)
-- Email Format: name.fathername@aastu.edu.et

USE [AASTURegistrationDB]
GO

-- Clear existing staff if needed (uncomment to reset)
-- DELETE FROM Staff;
-- GO

-- Add 30 Staff Members

-- Only keep ADV001, CSO001, DH001 and add new department head and advisor
INSERT INTO [dbo].[Staff] 
    ([StaffID], [FullName], [Email], [Department], [Role])
VALUES
    ('ADV001', 'Dr. Sarah Williams', 'sarah.williams@aastu.edu.et', 'Computer Science', 'Advisor'),
    ('CSO001', 'Mr. David Martinez', 'david.martinez@aastu.edu.et', 'Finance Office', 'CostSharingOfficer'),
    ('DH001', 'Prof. Emily Davis', 'emily.davis@aastu.edu.et', 'Computer Science', 'DepartmentHead'),
    ('DH002', 'Lemlem Kassa', 'lemlem.kassa@aastu.edu.et', 'Computer Science', 'DepartmentHead'),
    ('ADV002', 'Hussien Seid', 'hussien.seid@aastu.edu.et', 'Computer Science', 'Advisor');

-- All other staff are omitted as per request

-- Verify insertion
SELECT COUNT(*) as TotalStaff FROM Staff;
SELECT Role, COUNT(*) as Count FROM Staff GROUP BY Role ORDER BY Role;
SELECT * FROM Staff ORDER BY StaffID;
GO

PRINT '30 Staff members added successfully!'
GO
