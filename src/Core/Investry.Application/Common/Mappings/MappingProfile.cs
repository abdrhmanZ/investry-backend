using AutoMapper;
using Investry.Application.Contracts.Identity;
using Investry.Application.Features.Categories.Queries.GetAllCategories;
using Investry.Application.Features.Users.Queries.GetProfile;
using Investry.Domain.Entities;

namespace Investry.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<UserDto, ProfileDto>()
                .ForMember(dest => dest.KycStatus, opt => opt.MapFrom(src => src.KycStatus.ToString()));
        }
    }
}
