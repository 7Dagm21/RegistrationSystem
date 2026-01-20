using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AASTU.RegistrationSystem.API.Models
{
    [Table("Courses")]
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public int CreditHours { get; set; }

        [Required]
        public int AcademicYear { get; set; } // 1, 2, 3, 4, 5

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Semester { get; set; } // Fall, Spring, Summer
    }
}
