using Investry.API.Common;

namespace Investry.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;


        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request failed: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse<object>
                {
                    Success = false,
                    Errors = new List<ApiError>
                    {
                        new ApiError
                        {
                            Code = "Server.Error",
                            Message = _environment.IsDevelopment() ? ex.Message : "An unexpected error occurred.",
                            Details = _environment.IsDevelopment() ? ex.StackTrace : null
                        }
                    }
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
