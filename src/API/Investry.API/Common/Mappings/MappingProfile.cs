using AutoMapper;
using Investry.API.DTOs.Requests;
using Investry.Application.Features.Projects.Commands.CreateProject;
using Investry.Application.Features.Projects.Commands.UpdateProject;

namespace Investry.API.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateProjectRequest, CreateProjectCommand>()
                .ForMember(dest => dest.CoverImage, opt => opt.Ignore())
                .ForMember(dest => dest.MediaFiles, opt => opt.Ignore());

            CreateMap<UpdateProjectApiRequest, UpdateProjectCommand>()
                .ForMember(dest => dest.NewMediaFiles, opt => opt.Ignore());
        }
    }
}
