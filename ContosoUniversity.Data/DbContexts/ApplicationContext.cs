using ContosoUniversity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Services;

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
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string schema = "Contoso";
            if (OperatingSystem.IsMacOs())
            {
                schema = null;
            }
            modelBuilder.Entity<Course>().ToTable("Course", schema);
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment", schema);
            modelBuilder.Entity<Student>().ToTable("Student", schema);
            modelBuilder.Entity<Department>().ToTable("Department", schema);
            modelBuilder.Entity<Instructor>().ToTable("Instructor", schema);
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment", schema);
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment", schema);
            modelBuilder.Entity<Person>().ToTable("Person", schema);
            modelBuilder.Entity<CourseAssignment>().HasKey(c => new { c.CourseID, c.InstructorID });
        }
    }
}
