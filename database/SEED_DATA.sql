-- AASTU Registration System - Seed Data
-- This script populates initial data (students, staff, courses)
-- Note: EF Core SeedData.cs does this automatically, but you can use this for manual seeding

USE [AASTURegistrationDB]
GO

-- =============================================
-- Seed Students
-- =============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Students])
BEGIN
    INSERT INTO [dbo].[Students] ([StudentID], [FullName], [UniversityEmail], [EnrollmentYear], [Department], [Role])
    VALUES
        ('STU001', 'John Doe', 'john.doe@aastustudent.edu.et', 2020, 'Computer Science', 'Student'),
        ('STU002', 'Jane Smith', 'jane.smith@aastustudent.edu.et', 2021, 'Electrical Engineering', 'Student'),
        ('STU003', 'Mike Johnson', 'mike.johnson@aastustudent.edu.et', 2019, 'Mechanical Engineering', 'Student');
    
    PRINT 'Students seeded successfully'
END
ELSE
BEGIN
    PRINT 'Students table already contains data'
END
GO

-- =============================================
-- Seed Staff
-- =============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Staff])
BEGIN
    INSERT INTO [dbo].[Staff] ([StaffID], [FullName], [Email], [Department], [Role])
    VALUES
        ('ADV001', 'Dr. Sarah Williams', 'sarah.williams@aastu.edu.et', 'Computer Science', 'Advisor'),
        ('ADV002', 'Dr. Robert Brown', 'robert.brown@aastu.edu.et', 'Electrical Engineering', 'Advisor'),
        ('DH001', 'Prof. Emily Davis', 'emily.davis@aastu.edu.et', 'Computer Science', 'DepartmentHead'),
        ('DH002', 'Prof. James Wilson', 'james.wilson@aastu.edu.et', 'Electrical Engineering', 'DepartmentHead'),
        ('REG001', 'Ms. Lisa Anderson', 'lisa.anderson@aastu.edu.et', 'Registrar Office', 'Registrar'),
        ('CSO001', 'Mr. David Martinez', 'david.martinez@aastu.edu.et', 'Finance Office', 'CostSharingOfficer'),
        ('ADM001', 'System Administrator', 'admin@aastu.edu.et', 'IT Department', 'SystemAdmin');
    
    PRINT 'Staff seeded successfully'
END
ELSE
BEGIN
    PRINT 'Staff table already contains data'
END
GO

-- =============================================
-- Seed Courses
-- =============================================
IF NOT EXISTS (SELECT * FROM [dbo].[Courses])
BEGIN
    INSERT INTO [dbo].[Courses] ([CourseCode], [CourseName], [CreditHours], [AcademicYear], [Department], [Semester])
    VALUES
        -- Computer Science Courses
        ('CS101', 'Introduction to Programming', 3, 1, 'Computer Science', 'Fall'),
        ('CS102', 'Data Structures', 4, 2, 'Computer Science', 'Spring'),
        ('CS201', 'Database Systems', 3, 2, 'Computer Science', 'Fall'),
        ('CS301', 'Software Engineering', 4, 3, 'Computer Science', 'Spring'),
        
        -- Electrical Engineering Courses
        ('EE101', 'Circuit Analysis', 4, 1, 'Electrical Engineering', 'Fall'),
        ('EE201', 'Digital Electronics', 3, 2, 'Electrical Engineering', 'Spring');
    
    PRINT 'Courses seeded successfully'
END
ELSE
BEGIN
    PRINT 'Courses table already contains data'
END
GO

PRINT 'Seed data insertion completed!'
GO
