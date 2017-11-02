using ContosoUniversity.Data.Entities;
using ContosoUniversity.Common.Interfaces;
using ContosoUniversity.Data;
using ContosoUniversity.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Common
{
    // use c# 7 expression-bodied members
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private TContext _context;
        private IRepository<Department> _departmentRepo;
        private IPersonRepository<Instructor> _instructorRepo;
        private IPersonRepository<Student> _studentRepo;
        private IRepository<Course> _courseRepo;
        private IRepository<CourseAssignment> _courseAssignmentRepo;
        private IRepository<OfficeAssignment> _officeAssignmentRepo;
        private IRepository<Enrollment> _enrollmentRepo;

        public UnitOfWork()
        {
        }

        public UnitOfWork(TContext context)
        {
            context.Database.EnsureCreated();
            _context = context;
        }

        public virtual IRepository<Department> DepartmentRepository
        {
            get => _departmentRepo = _departmentRepo ?? new Repository<Department, TContext>(_context);
        }

        public virtual IPersonRepository<Instructor> InstructorRepository
        {
            get => _instructorRepo = _instructorRepo ?? new PersonRepository<Instructor, TContext>(_context);
        }

        public virtual IPersonRepository<Student> StudentRepository
        {
            get => _studentRepo = _studentRepo ?? new PersonRepository<Student, TContext>(_context);
        }

        public virtual IRepository<Course> CourseRepository
        {
            get => _courseRepo = _courseRepo ?? new Repository<Course, TContext>(_context);
        }

        public virtual IRepository<CourseAssignment> CourseAssignmentRepository
        {
            get => _courseAssignmentRepo = _courseAssignmentRepo ?? new Repository<CourseAssignment, TContext>(_context);
        }

        public virtual IRepository<OfficeAssignment> OfficeAssignmentRepository
        {
            get => _officeAssignmentRepo = _officeAssignmentRepo ?? new Repository<OfficeAssignment, TContext>(_context);
        }

        public virtual IRepository<Enrollment> EnrollmentRepository
        {
            get => _enrollmentRepo = _enrollmentRepo ?? new Repository<Enrollment, TContext>(_context);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}