using AutoMapper;
using BusinessTier.DTOS;
using DataTier.Entities;

namespace AddressBook.Mapper
{
    public class DomainPrpfile : Profile
    {
        public DomainPrpfile()
        {
            CreateMap<Department, DepartmentDto>();
            CreateMap<DepartmentDto, Department>();

            CreateMap<Department, DepartmentsNamesDto>();
            CreateMap<DepartmentsNamesDto, Department>();

            CreateMap<Job, JobDto>();//.ForMember(src => src.DepartmentId, opt => opt.MapFrom(p => p.Department.Name));
            CreateMap<JobDto, Job>();

            CreateMap<Job, JobsNamesDto>();
            CreateMap<JobsNamesDto, Job>();

            CreateMap<Entry, EntryDto>();
            CreateMap<EntryDto, Entry>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            
  

            

        }
    }
}
