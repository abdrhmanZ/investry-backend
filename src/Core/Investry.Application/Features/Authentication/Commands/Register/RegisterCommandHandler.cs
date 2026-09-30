//using Investry.Application.Common;
//using Investry.Application.Contracts.Identity;
//using MediatR;

//namespace Investry.Application.Features.Authentication.Commands.Register
//{
//    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
//    {
//        private readonly IAuthService _authService;
//        public RegisterCommandHandler(IAuthService authService)
//        {
//            _authService = authService;
//        }
//        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
//        {
//            return await _authService.RegisterAsync(request.RegistrationRequest, request.Role);
//        }
//    }
//}
///*
// هنا في حالة لو عايز ايند بوينت واحده للاتنين رول

// */
