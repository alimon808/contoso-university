using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Api.DTO
{
    public class CreateDepartmentDTO
    {
        [Required]
        public int InstructorID { get; set; }
        [Required]
        public string Name { get; set; }

        public decimal Budget { get; set; } = 0;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }
}
