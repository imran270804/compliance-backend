using AutoMapper;
using ComplianceCalendar.Models;
using ComplianceCalendar.Models.DTO;
using System.Reflection.Metadata;

namespace ComplianceCalendar.MappingConfig;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Filings, UpdateFilingStatusDTO>().ReverseMap();
        CreateMap<AddDocumentDTO, Documents>().ReverseMap();
        CreateMap<GetDocLinkDTO, Documents>().ReverseMap();
        CreateMap<Department, GetDepartmentDTO>().ReverseMap();
        CreateMap<Notification, NotificationDTO>().ReverseMap();
        CreateMap<Filings, ReviewDTO>().ReverseMap();
        CreateMap<Filings, ReassignFilingsDTO>().ReverseMap();
        CreateMap<AddAdminDTO, Employee>()
           .ForMember(dest => dest.EmpName, opt => opt.MapFrom(src => src.EmpName))
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        CreateMap<Employee, AddAdminDTO>()
            .ForMember(dest => dest.EmpName, opt => opt.MapFrom(src => src.EmpName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}
