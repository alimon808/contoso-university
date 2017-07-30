using System;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.ViewModels
{
    public abstract class DepartmentBaseViewModel
    {
        [StringLength(50, MinimumLength = 3)]
        public virtual string Name { get; set; }
        public virtual decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual DateTime StartDate { get; set; }

        public virtual string Administrator { get; set; }
    }
}
