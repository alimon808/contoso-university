using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;

namespace ContosoUniversity.Data
{
    // use c# 7 expression-bodied members
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationContext _context;
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

        public UnitOfWork(ApplicationContext context)
        {
            context.Database.EnsureCreated();
            _context = context;
        }

        public IRepository<Department> DepartmentRepository
        {
            get => _departmentRepo = _departmentRepo ?? new Repository<Department>(_context);
        }

        public IPersonRepository<Instructor> InstructorRepository
        {
            get => _instructorRepo = _instructorRepo ?? new PersonRepository<Instructor>(_context);
        }

        public IPersonRepository<Student> StudentRepository
        {
            get => _studentRepo = _studentRepo ?? new PersonRepository<Student>(_context);
        }

        public IRepository<Course> CourseRepository
        {
            get => _courseRepo = _courseRepo ?? new Repository<Course>(_context);
        }

        public IRepository<CourseAssignment> CourseAssignmentRepository
        {
            get => _courseAssignmentRepo = _courseAssignmentRepo ?? new Repository<CourseAssignment>(_context);
        }

        public IRepository<OfficeAssignment> OfficeAssignmentRepository
        {
            get => _officeAssignmentRepo = _officeAssignmentRepo ?? new Repository<OfficeAssignment>(_context);
        }

        public IRepository<Enrollment> EnrollmentRepository
        {
            get => _enrollmentRepo = _enrollmentRepo ?? new Repository<Enrollment>(_context);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}