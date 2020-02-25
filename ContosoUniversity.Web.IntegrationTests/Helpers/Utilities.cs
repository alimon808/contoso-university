using System;
using ContosoUniversity.Data;
using ContosoUniversity.Data.DbContexts;
using ContosoUniversity.Data.Entities;

namespace ContosoUniversity.Web.IntegrationTests
{
    public static class Utilities
    {
        public static void InitializeDbForTest(ApplicationContext db)
        {
            // id = 1
            var student = new Student { FirstMidName = "Anna", LastName = "Garland", EnrollmentDate = DateTime.Now };
            db.Students.Add(student);
            db.SaveChanges();

            // id = 1
            var office = new OfficeAssignment();
            office.Location = "A";
            db.OfficeAssignments.Add(office);
            db.SaveChanges();

            // id = 2
            var instructor = new Instructor { };
            instructor.LastName = "Smith";
            instructor.FirstMidName = "John";
            instructor.HireDate = DateTime.Now;
            instructor.OfficeAssignment = office;
            db.Instructors.Add(instructor);
            db.SaveChanges();

            // id = 1
            var department = new Department();
            department.Administrator = instructor;
            department.Name = "Engineering";
            department.Budget = 100;
            db.Departments.Add(department);
            db.SaveChanges();

            // id = 1
            var course = new Course();
            course.Department = department;
            course.Title = "Intro to Engineering";
            course.Credits = 3;
            db.Courses.Add(course);
            db.SaveChanges();

            var courseAssignment = new CourseAssignment();
            courseAssignment.CourseID = 1;
            courseAssignment.InstructorID = 2;
            db.CourseAssignments.Add(courseAssignment);
            db.SaveChanges();

        }
    }

}