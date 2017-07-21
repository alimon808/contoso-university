using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using ContosoUniversity.Data;

namespace ContosoUniversity.Data.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Course", b =>
                {
                    b.Property<int>("CourseID");

                    b.Property<int>("Credits");

                    b.Property<int>("DepartmentID");

                    b.Property<string>("Title")
                        .HasMaxLength(50);

                    b.HasKey("CourseID");

                    b.HasIndex("DepartmentID");

                    b.ToTable("Course","Contoso");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.CourseAssignment", b =>
                {
                    b.Property<int>("CourseID");

                    b.Property<int>("InstructorID");

                    b.HasKey("CourseID", "InstructorID");

                    b.HasIndex("InstructorID");

                    b.ToTable("CourseAssignment","Contoso");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Department", b =>
                {
                    b.Property<int>("DepartmentID")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Budget")
                        .HasColumnType("money");

                    b.Property<int?>("InstructorID");

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("StartDate");

                    b.HasKey("DepartmentID");

                    b.HasIndex("InstructorID");

                    b.ToTable("Department","Contoso");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Enrollment", b =>
                {
                    b.Property<long>("EnrollmentID")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("CourseID");

                    b.Property<int?>("CourseID1");

                    b.Property<int?>("Grade");

                    b.Property<long>("StudentID");

                    b.Property<int?>("StudentID1");

                    b.HasKey("EnrollmentID");

                    b.HasIndex("CourseID1");

                    b.HasIndex("StudentID1");

                    b.ToTable("Enrollment","Contoso");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.OfficeAssignment", b =>
                {
                    b.Property<int>("InstructorID");

                    b.Property<string>("Location")
                        .HasMaxLength(50);

                    b.HasKey("InstructorID");

                    b.ToTable("OfficeAssignment","Contoso");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Person", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("FirstMidName")
                        .IsRequired()
                        .HasColumnName("FirstName")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.ToTable("Person","Contoso");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Person");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Instructor", b =>
                {
                    b.HasBaseType("ContosoUniversity.Data.Entities.Person");

                    b.Property<DateTime>("HireDate");

                    b.ToTable("Instructor","Contoso");

                    b.HasDiscriminator().HasValue("Instructor");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Student", b =>
                {
                    b.HasBaseType("ContosoUniversity.Data.Entities.Person");

                    b.Property<DateTime>("EnrollmentDate");

                    b.ToTable("Student","Contoso");

                    b.HasDiscriminator().HasValue("Student");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Course", b =>
                {
                    b.HasOne("ContosoUniversity.Data.Entities.Department", "Department")
                        .WithMany("Courses")
                        .HasForeignKey("DepartmentID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.CourseAssignment", b =>
                {
                    b.HasOne("ContosoUniversity.Data.Entities.Course", "Course")
                        .WithMany("CourseAssignments")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContosoUniversity.Data.Entities.Instructor", "Instructor")
                        .WithMany("CourseAssignments")
                        .HasForeignKey("InstructorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Department", b =>
                {
                    b.HasOne("ContosoUniversity.Data.Entities.Instructor", "Administrator")
                        .WithMany()
                        .HasForeignKey("InstructorID");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.Enrollment", b =>
                {
                    b.HasOne("ContosoUniversity.Data.Entities.Course", "Course")
                        .WithMany("Enrollments")
                        .HasForeignKey("CourseID1");

                    b.HasOne("ContosoUniversity.Data.Entities.Student", "Student")
                        .WithMany("Enrollments")
                        .HasForeignKey("StudentID1");
                });

            modelBuilder.Entity("ContosoUniversity.Data.Entities.OfficeAssignment", b =>
                {
                    b.HasOne("ContosoUniversity.Data.Entities.Instructor", "Instructor")
                        .WithOne("OfficeAssignment")
                        .HasForeignKey("ContosoUniversity.Data.Entities.OfficeAssignment", "InstructorID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
