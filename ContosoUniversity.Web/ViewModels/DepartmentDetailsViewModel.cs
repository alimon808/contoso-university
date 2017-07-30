using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.ViewModels
{
    public class DepartmentDetailsViewModel : DepartmentBaseViewModel
    {
        public int ID { get; set; }

        public override string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public override DateTime StartDate { get; set; }
        
        public int InstructorID { get; set; }
    }
}
