using AutoMapper;
using ContosoUniversity.Data.Entities;
using ContosoUniversity.ViewModels;
using System.Text;

namespace ContosoUniversity.Web
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<Department, DepartmentDetailsViewModel>()
                .ForMember(dest => dest.Administrator, opts => opts.MapFrom(src => src.Administrator.FullName));
            CreateMap<DepartmentCreateViewModel, Department>()
                .ForMember(dest => dest.Administrator, opts => opts.Ignore());
            CreateMap<Department, DepartmentEditViewModel>()
                .ForMember(dest => dest.Administrator, opts => opts.MapFrom(src => src.Administrator.FullName));
            CreateMap<DepartmentEditViewModel, Department>()
                .ForMember(dest => dest.RowVersion, opts => opts.MapFrom(src => Encoding.ASCII.GetBytes(src.RowVersion)))
                .ForMember(dest => dest.Administrator, opts => opts.Ignore());
        }
    }
}
