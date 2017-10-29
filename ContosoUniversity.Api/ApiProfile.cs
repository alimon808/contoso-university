using AutoMapper;
using ContosoUniversity.Api.DTO;
using ContosoUniversity.Common.DTO;
using ContosoUniversity.Data.Entities;

namespace ContosoUniversity.Api
{
    public class ApiProfile : Profile
    {
        public ApiProfile()
        {
            CreateMap<DepartmentDTO, Department>().ReverseMap();
            CreateMap<CreateDepartmentDTO, Department>().ReverseMap();
        }
    }
}
