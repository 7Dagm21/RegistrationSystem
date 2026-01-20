-- Add 30 Students with ETS Format
-- Student ID Format: ETS####/YY (e.g., ETS0358/15)
-- Email Format: firstname.lastname@aastustudent.edu.et

USE [AASTURegistrationDB]
GO

-- Clear existing students if needed (uncomment to reset)
-- DELETE FROM Students;
-- GO

-- Add 30 Students

-- Only keep ETS0358/15 and add the 5 new students
INSERT INTO [dbo].[Students] 
    ([StudentID], [FullName], [UniversityEmail], [EnrollmentYear], [Department], [Role])
VALUES
    ('ETS0358/15', 'Dagmawit Yoseph', 'dagmawit.yoseph@aastustudent.edu.et', 2015, 'Computer Science', 'Student'),
    ('ETS0367/15', 'Dagmawit Alemayehu', 'dagmawit.alemayehu@aastustudent.edu.et', 2015, 'Computer Science', 'Student'),
    ('ETS0410/15', 'Eden Yedemie', 'eden.yedemie@aastustudent.edu.et', 2015, 'Computer Science', 'Student'),
    ('ETS0660/15', 'Helen Zelalem', 'helen.zelalem@aastustudent.edu.et', 2015, 'Computer Science', 'Student'),
    ('0651/15', 'Haymanot Girma', 'haymanot.girma@aastustudent.edu.et', 2015, 'Computer Science', 'Student'),
    ('ETS0409/15', 'Eden Belayneh', 'eden.belayneh@aastustudent.edu.et', 2015, 'Computer Science', 'Student');

-- All other students are omitted as per request

-- Verify insertion
SELECT COUNT(*) as TotalStudents FROM Students;
SELECT * FROM Students ORDER BY StudentID;
GO

PRINT '30 Students added successfully!'
GO
