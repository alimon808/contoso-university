using ContosoUniversity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course", "Contoso");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment", "Contoso");
            modelBuilder.Entity<Student>().ToTable("Student", "Contoso");
            modelBuilder.Entity<Department>().ToTable("Department", "Contoso");
            modelBuilder.Entity<Instructor>().ToTable("Instructor", "Contoso");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment", "Contoso");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment", "Contoso");
            modelBuilder.Entity<Person>().ToTable("Person", "Contoso");
            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseID, c.InstructorID });
        }
    }
}
