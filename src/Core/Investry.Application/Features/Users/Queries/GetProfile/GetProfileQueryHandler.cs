using AutoMapper;
using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Persistence;
using MediatR;

namespace Investry.Application.Features.Users.Queries.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        public GetProfileQueryHandler(IUnitOfWork unitOfWork, IIdentityService identityService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _identityService = identityService;
            _mapper = mapper;
        }
        public async Task<Result<ProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.GetUserByIdAsync(request.UserId);
            if (user is null)
                return Result<ProfileDto>.Failure(new List<Error> { new Error("User.NotFound", "User not found.", ErrorType.NotFound) });

            var profileDto = _mapper.Map<ProfileDto>(user);
            
            return Result<ProfileDto>.Success(profileDto);
        }
    }
}
