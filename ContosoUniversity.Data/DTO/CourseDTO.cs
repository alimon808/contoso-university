namespace ContosoUniversity.Data.DTO
{
    public class CourseDTO
    {
        public int ID { get; set; }
        public int CourseNumber { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public int DepartmentID { get; set; }
    }
}
