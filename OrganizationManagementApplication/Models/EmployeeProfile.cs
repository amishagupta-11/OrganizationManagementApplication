using AutoMapper;

namespace OrganizationManagementApplication.Models
{
    /// <summary>
    /// Represents the AutoMapper profile for mapping Employee to EmployeeDetails.
    /// </summary>
    public class EmployeeProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the "EmployeeProfile" class.
        /// </summary>
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDetails>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        }
    }
}
