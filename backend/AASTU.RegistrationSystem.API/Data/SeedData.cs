using Microsoft.EntityFrameworkCore;
using AASTU.RegistrationSystem.API.Models;

namespace AASTU.RegistrationSystem.API.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Check if tables exist and have data
            try
            {
                // Check if already seeded
                if (context.Students.Any() || context.Staff.Any())
                {
                    return; // Database already seeded
                }
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 208) // Invalid object name
            {
                // Tables don't exist yet - this should not happen if EnsureCreated ran first
                // But if it does, return and let tables be created first
                return;
            }
            catch
            {
                // Other errors - try to continue seeding
            }

            // Seed Students (30 students with ETS format)
            var students = new List<Student>
            {
                // Computer Science Students
                new Student { StudentID = "ETS0358/15", FullName = "Dagmawit Yoseph", UniversityEmail = "dagmawit.yoseph@aastustudent.edu.et", EnrollmentYear = 2015, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS0421/16", FullName = "Meron Tesfaye", UniversityEmail = "meron.tesfaye@aastustudent.edu.et", EnrollmentYear = 2016, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS0512/17", FullName = "Yonas Alemayehu", UniversityEmail = "yonas.alemayehu@aastustudent.edu.et", EnrollmentYear = 2017, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS0623/18", FullName = "Selamawit Bekele", UniversityEmail = "selamawit.bekele@aastustudent.edu.et", EnrollmentYear = 2018, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS0734/19", FullName = "Tewodros Getachew", UniversityEmail = "tewodros.getachew@aastustudent.edu.et", EnrollmentYear = 2019, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS0845/20", FullName = "Marta Tadesse", UniversityEmail = "marta.tadesse@aastustudent.edu.et", EnrollmentYear = 2020, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS0956/21", FullName = "Daniel Haile", UniversityEmail = "daniel.haile@aastustudent.edu.et", EnrollmentYear = 2021, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS1067/22", FullName = "Hanna Mekonnen", UniversityEmail = "hanna.mekonnen@aastustudent.edu.et", EnrollmentYear = 2022, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS1178/23", FullName = "Kebede Assefa", UniversityEmail = "kebede.assefa@aastustudent.edu.et", EnrollmentYear = 2023, Department = "Computer Science", Role = "Student" },
                new Student { StudentID = "ETS1289/24", FullName = "Rahel Solomon", UniversityEmail = "rahel.solomon@aastustudent.edu.et", EnrollmentYear = 2024, Department = "Computer Science", Role = "Student" },
                
                // Electrical Engineering Students
                new Student { StudentID = "ETS2145/16", FullName = "Abebe Demissie", UniversityEmail = "abebe.demissie@aastustudent.edu.et", EnrollmentYear = 2016, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS2256/17", FullName = "Tigist Fisseha", UniversityEmail = "tigist.fisseha@aastustudent.edu.et", EnrollmentYear = 2017, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS2367/18", FullName = "Mulugeta Worku", UniversityEmail = "mulugeta.worku@aastustudent.edu.et", EnrollmentYear = 2018, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS2478/19", FullName = "Eyerusalem Gebre", UniversityEmail = "eyerusalem.gebre@aastustudent.edu.et", EnrollmentYear = 2019, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS2589/20", FullName = "Bereket Hailu", UniversityEmail = "bereket.hailu@aastustudent.edu.et", EnrollmentYear = 2020, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS2690/21", FullName = "Mihret Tsegaye", UniversityEmail = "mihret.tsegaye@aastustudent.edu.et", EnrollmentYear = 2021, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS2701/22", FullName = "Solomon Tekle", UniversityEmail = "solomon.tekle@aastustudent.edu.et", EnrollmentYear = 2022, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS2812/23", FullName = "Bethelhem Negash", UniversityEmail = "bethelhem.negash@aastustudent.edu.et", EnrollmentYear = 2023, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS2923/24", FullName = "Yared Lemma", UniversityEmail = "yared.lemma@aastustudent.edu.et", EnrollmentYear = 2024, Department = "Electrical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3034/25", FullName = "Meskel Alemayehu", UniversityEmail = "meskel.alemayehu@aastustudent.edu.et", EnrollmentYear = 2025, Department = "Electrical Engineering", Role = "Student" },
                
                // Mechanical Engineering Students
                new Student { StudentID = "ETS3124/17", FullName = "Genet Asnake", UniversityEmail = "genet.asnake@aastustudent.edu.et", EnrollmentYear = 2017, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3235/18", FullName = "Temesgen Addis", UniversityEmail = "temesgen.addis@aastustudent.edu.et", EnrollmentYear = 2018, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3346/19", FullName = "Meron Girma", UniversityEmail = "meron.girma@aastustudent.edu.et", EnrollmentYear = 2019, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3457/20", FullName = "Dawit Yohannes", UniversityEmail = "dawit.yohannes@aastustudent.edu.et", EnrollmentYear = 2020, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3568/21", FullName = "Sara Mengistu", UniversityEmail = "sara.mengistu@aastustudent.edu.et", EnrollmentYear = 2021, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3679/22", FullName = "Nahom Berhanu", UniversityEmail = "nahom.berhanu@aastustudent.edu.et", EnrollmentYear = 2022, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3780/23", FullName = "Liya Tadesse", UniversityEmail = "liya.tadesse@aastustudent.edu.et", EnrollmentYear = 2023, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3891/24", FullName = "Henok Gebremariam", UniversityEmail = "henok.gebremariam@aastustudent.edu.et", EnrollmentYear = 2024, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS3902/25", FullName = "Marta Haileselassie", UniversityEmail = "marta.haileselassie@aastustudent.edu.et", EnrollmentYear = 2025, Department = "Mechanical Engineering", Role = "Student" },
                new Student { StudentID = "ETS4013/26", FullName = "Yohannes Teklu", UniversityEmail = "yohannes.teklu@aastustudent.edu.et", EnrollmentYear = 2026, Department = "Mechanical Engineering", Role = "Student" },
            };

            context.Students.AddRange(students);

            // Seed Staff (30 staff members without ETS prefix)
            var staff = new List<Staff>
            {
                // Advisors
                new Staff { StaffID = "ADV001", FullName = "Dr. Sarah Williams", Email = "sarah.williams@aastu.edu.et", Department = "Computer Science", Role = "Advisor" },
                new Staff { StaffID = "ADV002", FullName = "Dr. Robert Brown", Email = "robert.brown@aastu.edu.et", Department = "Electrical Engineering", Role = "Advisor" },
                new Staff { StaffID = "ADV003", FullName = "Dr. Michael Chen", Email = "michael.chen@aastu.edu.et", Department = "Computer Science", Role = "Advisor" },
                new Staff { StaffID = "ADV004", FullName = "Dr. Amanuel Tekle", Email = "amanuel.tekle@aastu.edu.et", Department = "Mechanical Engineering", Role = "Advisor" },
                new Staff { StaffID = "ADV005", FullName = "Dr. Meseret Bekele", Email = "meseret.bekele@aastu.edu.et", Department = "Electrical Engineering", Role = "Advisor" },
                new Staff { StaffID = "ADV006", FullName = "Dr. Tewodros Haile", Email = "tewodros.haile@aastu.edu.et", Department = "Computer Science", Role = "Advisor" },
                new Staff { StaffID = "ADV007", FullName = "Dr. Selamawit Yohannes", Email = "selamawit.yohannes@aastu.edu.et", Department = "Mechanical Engineering", Role = "Advisor" },
                new Staff { StaffID = "ADV008", FullName = "Dr. Daniel Mekonnen", Email = "daniel.mekonnen@aastu.edu.et", Department = "Electrical Engineering", Role = "Advisor" },
                
                // Department Heads
                new Staff { StaffID = "DH001", FullName = "Prof. Emily Davis", Email = "emily.davis@aastu.edu.et", Department = "Computer Science", Role = "DepartmentHead" },
                new Staff { StaffID = "DH002", FullName = "Prof. James Wilson", Email = "james.wilson@aastu.edu.et", Department = "Electrical Engineering", Role = "DepartmentHead" },
                new Staff { StaffID = "DH003", FullName = "Prof. Maria Garcia", Email = "maria.garcia@aastu.edu.et", Department = "Mechanical Engineering", Role = "DepartmentHead" },
                new Staff { StaffID = "DH004", FullName = "Prof. Alemayehu Gebre", Email = "alemayehu.gebre@aastu.edu.et", Department = "Computer Science", Role = "DepartmentHead" },
                new Staff { StaffID = "DH005", FullName = "Prof. Tigist Assefa", Email = "tigist.assefa@aastu.edu.et", Department = "Electrical Engineering", Role = "DepartmentHead" },
                new Staff { StaffID = "DH006", FullName = "Prof. Yonas Tadesse", Email = "yonas.tadesse@aastu.edu.et", Department = "Mechanical Engineering", Role = "DepartmentHead" },
                
                // Registrars
                new Staff { StaffID = "REG001", FullName = "Ms. Lisa Anderson", Email = "lisa.anderson@aastu.edu.et", Department = "Registrar Office", Role = "Registrar" },
                new Staff { StaffID = "REG002", FullName = "Mr. Thomas Lee", Email = "thomas.lee@aastu.edu.et", Department = "Registrar Office", Role = "Registrar" },
                new Staff { StaffID = "REG003", FullName = "Ms. Meron Worku", Email = "meron.worku@aastu.edu.et", Department = "Registrar Office", Role = "Registrar" },
                new Staff { StaffID = "REG004", FullName = "Mr. Bereket Hailu", Email = "bereket.hailu@aastu.edu.et", Department = "Registrar Office", Role = "Registrar" },
                new Staff { StaffID = "REG005", FullName = "Ms. Rahel Solomon", Email = "rahel.solomon@aastu.edu.et", Department = "Registrar Office", Role = "Registrar" },
                
                // Cost Sharing Officers
                new Staff { StaffID = "CSO001", FullName = "Mr. David Martinez", Email = "david.martinez@aastu.edu.et", Department = "Finance Office", Role = "CostSharingOfficer" },
                new Staff { StaffID = "CSO002", FullName = "Ms. Genet Fisseha", Email = "genet.fisseha@aastu.edu.et", Department = "Finance Office", Role = "CostSharingOfficer" },
                new Staff { StaffID = "CSO003", FullName = "Mr. Solomon Tekle", Email = "solomon.tekle@aastu.edu.et", Department = "Finance Office", Role = "CostSharingOfficer" },
                new Staff { StaffID = "CSO004", FullName = "Ms. Mihret Tsegaye", Email = "mihret.tsegaye@aastu.edu.et", Department = "Finance Office", Role = "CostSharingOfficer" },
                new Staff { StaffID = "CSO005", FullName = "Mr. Yared Lemma", Email = "yared.lemma@aastu.edu.et", Department = "Finance Office", Role = "CostSharingOfficer" },
                new Staff { StaffID = "CSO006", FullName = "Ms. Bethelhem Negash", Email = "bethelhem.negash@aastu.edu.et", Department = "Finance Office", Role = "CostSharingOfficer" },
                
                // System Admins
                new Staff { StaffID = "ADM001", FullName = "System Administrator", Email = "admin@aastu.edu.et", Department = "IT Department", Role = "SystemAdmin" },
                new Staff { StaffID = "ADM002", FullName = "Mr. Meskel Alemayehu", Email = "meskel.alemayehu@aastu.edu.et", Department = "IT Department", Role = "SystemAdmin" },
                new Staff { StaffID = "ADM003", FullName = "Ms. Eyerusalem Gebre", Email = "eyerusalem.gebre@aastu.edu.et", Department = "IT Department", Role = "SystemAdmin" },
                new Staff { StaffID = "ADM004", FullName = "Mr. Mulugeta Worku", Email = "mulugeta.worku@aastu.edu.et", Department = "IT Department", Role = "SystemAdmin" },
                new Staff { StaffID = "ADM005", FullName = "Ms. Tigist Fisseha", Email = "tigist.fisseha@aastu.edu.et", Department = "IT Department", Role = "SystemAdmin" },
            };

            context.Staff.AddRange(staff);

            // Seed Courses
            var courses = new List<Course>
            {
                // Computer Science Courses
                new Course { CourseCode = "CS101", CourseName = "Introduction to Programming", CreditHours = 3, AcademicYear = 1, Department = "Computer Science", Semester = "Fall" },
                new Course { CourseCode = "CS102", CourseName = "Data Structures", CreditHours = 4, AcademicYear = 2, Department = "Computer Science", Semester = "Spring" },
                new Course { CourseCode = "CS201", CourseName = "Database Systems", CreditHours = 3, AcademicYear = 2, Department = "Computer Science", Semester = "Fall" },
                new Course { CourseCode = "CS301", CourseName = "Software Engineering", CreditHours = 4, AcademicYear = 3, Department = "Computer Science", Semester = "Spring" },
                
                // Electrical Engineering Courses
                new Course { CourseCode = "EE101", CourseName = "Circuit Analysis", CreditHours = 4, AcademicYear = 1, Department = "Electrical Engineering", Semester = "Fall" },
                new Course { CourseCode = "EE201", CourseName = "Digital Electronics", CreditHours = 3, AcademicYear = 2, Department = "Electrical Engineering", Semester = "Spring" },
            };

            context.Courses.AddRange(courses);

            context.SaveChanges();
        }
    }
}
