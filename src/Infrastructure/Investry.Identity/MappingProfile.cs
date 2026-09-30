using AutoMapper;
using Investry.Application.Contracts.Identity;
using Investry.Identity.Models;

namespace Investry.Identity
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
