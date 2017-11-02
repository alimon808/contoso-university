using ContosoUniversity.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data.DbContexts
{
    public class DbContextConfig
    {
        public void ApplicationContextConfig(ModelBuilder modelBuilder, string schema = "")
        {
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

        public void SecureApplicationContextConfig(ModelBuilder modelBuilder, string schema = "")
        {
            modelBuilder.Entity<ApplicationUser>().ToTable("Users", schema);
            modelBuilder.Entity<IdentityRole>().ToTable("Role", schema);
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim", schema);
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRole", schema);
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin", schema);
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim", schema);
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserToken", schema);
        }
    }
}
