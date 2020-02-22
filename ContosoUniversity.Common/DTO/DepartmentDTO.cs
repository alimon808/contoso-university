using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Common.DTO
{
    public class DepartmentDTO
    {
        [Required]
        public int ID { get; set; }
        public int InstructorID { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
    }
}
