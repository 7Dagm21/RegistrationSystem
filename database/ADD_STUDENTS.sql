-- Add Students to Base Database
-- These students can sign up for accounts in the system
-- Only students in this table are allowed to register

USE [AASTURegistrationDB]
GO

-- Example: Add a new student
INSERT INTO [dbo].[Students] 
    ([StudentID], [FullName], [UniversityEmail], [EnrollmentYear], [Department], [Role])
VALUES
    ('STU004', 'Alice Johnson', 'alice.johnson@aastustudent.edu.et', 2022, 'Computer Science', 'Student'),
    ('STU005', 'Bob Williams', 'bob.williams@aastustudent.edu.et', 2022, 'Electrical Engineering', 'Student'),
    ('STU006', 'Carol Brown', 'carol.brown@aastustudent.edu.et', 2021, 'Mechanical Engineering', 'Student');

-- Verify the insertion
SELECT * FROM [dbo].[Students] WHERE StudentID IN ('STU004', 'STU005', 'STU006');
GO

-- Template for adding more students:
/*
INSERT INTO [dbo].[Students] 
    ([StudentID], [FullName], [UniversityEmail], [EnrollmentYear], [Department], [Role])
VALUES
    ('STUXXX', 'Full Name', 'email@aastustudent.edu.et', YYYY, 'Department Name', 'Student');
*/

-- Required Fields:
-- StudentID: Unique identifier (e.g., 'STU001', 'STU002')
-- FullName: Student's full name
-- UniversityEmail: Must end with @aastustudent.edu.et
-- EnrollmentYear: Year student enrolled (e.g., 2020, 2021, 2022)
-- Department: Department name (e.g., 'Computer Science', 'Electrical Engineering')
-- Role: Always 'Student'
