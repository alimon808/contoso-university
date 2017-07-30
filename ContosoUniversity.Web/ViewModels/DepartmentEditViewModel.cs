namespace ContosoUniversity.ViewModels
{
    public class DepartmentEditViewModel : DepartmentBaseViewModel
    {
        public int ID { get; set; }
        
        public string RowVersion { get; set; }
        public int InstructorID { get; set; }
    }
}
