using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.ViewModels
{
    public class DepartmentCreateViewModel : DepartmentBaseViewModel
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public override DateTime StartDate { get; set; }

        public int InstructorID { get; set; } = 0;
    }
}
