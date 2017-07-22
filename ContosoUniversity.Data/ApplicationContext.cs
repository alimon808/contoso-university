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
        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
        public DbSet<CourseAssignment> CourseAssignments { get; set; }
        public DbSet<Person> People { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().ToTable("Course", "Contoso");
            modelBuilder.Entity<Course>().HasKey(k => k.ID);
            modelBuilder.Entity<Course>().Property(p => p.ID)
                .ValueGeneratedNever();

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
