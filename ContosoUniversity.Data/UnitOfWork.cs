using ContosoUniversity.Data.Entities;
using ContosoUniversity.Data.Interfaces;

namespace ContosoUniversity.Data
{
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
            get
            {
                if (_departmentRepo == null)
                {
                    _departmentRepo = new Repository<Department>(_context);
                }

                return _departmentRepo;
            }
        }

        public IPersonRepository<Instructor> InstructorRepository
        {
            get
            {
                if (_instructorRepo == null)
                {
                    _instructorRepo = new PersonRepository<Instructor>(_context);
                }

                return _instructorRepo;
            }
        }

        public IPersonRepository<Student> StudentRepository
        {
            get
            {
                if (_studentRepo == null)
                {
                    _studentRepo = new PersonRepository<Student>(_context);
                }
                return _studentRepo;
            }
        }

        public IRepository<Course> CourseRepository {
            get
            {
                if (_courseRepo == null)
                {
                    _courseRepo = new Repository<Course>(_context);
                }
                return _courseRepo;
            }
        }

        public IRepository<CourseAssignment> CourseAssignmentRepository
        {
            get
            {
                if (_courseAssignmentRepo == null)
                {
                    _courseAssignmentRepo = new Repository<CourseAssignment>(_context);
                }
                return _courseAssignmentRepo;
            }
        }

        public IRepository<OfficeAssignment> OfficeAssignmentRepository
        {
            get
            {
                if (_officeAssignmentRepo == null)
                {
                    _officeAssignmentRepo = new Repository<OfficeAssignment>(_context);
                }
                return _officeAssignmentRepo;
            }
        }

        public IRepository<Enrollment> EnrollmentRepository
        {
            get
            {
                if (_enrollmentRepo == null)
                {
                    _enrollmentRepo = new Repository<Enrollment>(_context);
                }
                return _enrollmentRepo;
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }
    }
}