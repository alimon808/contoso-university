using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.ViewModels
{
    public class DepartmentEditViewModel : DepartmentBaseViewModel
    {
        [Required]
        public int ID { get; set; }
        
        public string RowVersion { get; set; }

        [Display(Name = "Administrator")]
        public int InstructorID { get; set; }
    }
}
