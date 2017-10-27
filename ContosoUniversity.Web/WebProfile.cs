using AutoMapper;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.ViewModels;

namespace ContosoUniversity.Web
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<Department, DepartmentDetailsViewModel>()
                .ForMember(dest => dest.Administrator, opts => opts.MapFrom(src => src.Administrator.FullName));
            CreateMap<DepartmentCreateViewModel, Department>();
            CreateMap<Department, DepartmentEditViewModel>()
                .ForMember(dest => dest.Administrator, opts => opts.MapFrom(src => src.Administrator.FullName));
        }
    }
}
