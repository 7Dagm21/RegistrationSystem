namespace AASTU.RegistrationSystem.API.Services
{
    public static class GradeCalculationService
    {
        /// <summary>
        /// Converts a number grade (0-100) to letter grade based on AASTU grading scale
        /// </summary>
        public static string GetLetterGrade(decimal numberGrade)
        {
            if (numberGrade >= 90 && numberGrade <= 100) return "A+";
            if (numberGrade >= 85 && numberGrade <= 89) return "A";
            if (numberGrade >= 80 && numberGrade <= 84) return "A-";
            if (numberGrade >= 75 && numberGrade <= 79) return "B+";
            if (numberGrade >= 70 && numberGrade <= 74) return "B";
            if (numberGrade >= 65 && numberGrade <= 69) return "B-";
            if (numberGrade >= 60 && numberGrade <= 64) return "C+";
            if (numberGrade >= 50 && numberGrade <= 59) return "C";
            if (numberGrade >= 45 && numberGrade <= 49) return "C-";
            if (numberGrade >= 40 && numberGrade <= 44) return "D";
            if (numberGrade >= 35 && numberGrade <= 39) return "Fx";
            if (numberGrade >= 0 && numberGrade <= 34) return "F";
            return "";
        }

        /// <summary>
        /// Gets the grade point value for a letter grade
        /// </summary>
        public static decimal GetGradePointValue(string letterGrade)
        {
            return letterGrade.ToUpper() switch
            {
                "A+" => 4.0m,
                "A" => 4.0m,
                "A-" => 3.75m,
                "B+" => 3.5m,
                "B" => 3.0m,
                "B-" => 2.75m,
                "C+" => 2.5m,
                "C" => 2.0m,
                "C-" => 1.75m,
                "D" => 1.0m,
                "FX" => 0.0m,
                "F" => 0.0m,
                _ => 0.0m
            };
        }

        /// <summary>
        /// Calculates GradePoint = CreditHours × GradePointValue
        /// </summary>
        public static decimal CalculateGradePoint(decimal creditHours, decimal gradePointValue)
        {
            return creditHours * gradePointValue;
        }

        /// <summary>
        /// Calculates GPA: Sum(CreditHours × GradePointValue) / Sum(CreditHours)
        /// </summary>
        public static decimal CalculateGPA(List<(decimal CreditHours, decimal GradePointValue)> courses)
        {
            if (courses == null || courses.Count == 0)
                return 0;

            decimal totalCreditHours = courses.Sum(c => c.CreditHours);
            if (totalCreditHours == 0)
                return 0;

            decimal totalGradePoints = courses.Sum(c => c.CreditHours * c.GradePointValue);
            return totalGradePoints / totalCreditHours;
        }
    }
}
