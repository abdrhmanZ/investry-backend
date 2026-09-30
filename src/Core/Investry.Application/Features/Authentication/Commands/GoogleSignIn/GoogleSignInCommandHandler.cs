//using Investry.Application.Common;
//using Investry.Application.Contracts.Identity;
//using Investry.Application.Models.Identity;
//using Investry.Domain.Enums;
//using MediatR;

//namespace Investry.Application.Features.Authentication.Commands.GoogleSignIn
//{
//    public class GoogleSignInCommandHandler : IRequestHandler<GoogleSignInCommand, Result<AuthResponse>>
//    {
//        private readonly IAuthService _authService;
//        public GoogleSignInCommandHandler(IAuthService authService)
//        {
//            _authService = authService;
//        }
//        public async Task<Result<AuthResponse>> Handle(GoogleSignInCommand request, CancellationToken cancellationToken)
//        {
//            return await _authService.SignInWithGoogleAsync(request.IdToken, request.Role);
//        }
//    }
//}

///*
// هنا في حالة لو عايز ايند بوينت واحده للاتنين رول
// */
