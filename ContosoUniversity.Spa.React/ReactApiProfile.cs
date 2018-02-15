using AutoMapper;
using ContosoUniversity.Spa.React.DTO;
using ContosoUniversity.Common.DTO;
using ContosoUniversity.Data.Entities;

namespace ContosoUniversity.Spa.React
{
    public class ReactApiProfile : Profile
    {
        public ReactApiProfile()
        {
            CreateMap<DepartmentDTO, Department>().ReverseMap();
        }
    }
}
