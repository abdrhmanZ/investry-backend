using Investry.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Investry.API.Common.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToApiResponse<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(new ApiResponse<T>
                {
                    Success = true,
                    Data = result.Value,
                    Errors = null
                })
                { StatusCode = 200 };
            }

            if (result.Errors == null || !result.Errors.Any())
            {
                return new ObjectResult(new ApiResponse<T>
                {
                    Success = false,
                    Data = default,
                    Errors = new List<ApiError>
                    {
                        new ApiError { Code = "UnknownError", Message = "An unknown error occurred." }
                    }
                })
                { StatusCode = 500 };
            }

            var statusCode = result.Errors.Max(e => e.Type switch
            {
                ErrorType.Validation => 400,
                ErrorType.NotFound => 404,
                ErrorType.Conflict => 409,
                ErrorType.Unauthorized => 401,
                ErrorType.Forbidden => 403,
                _ => 500
            });

            return new ObjectResult(new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Errors = result.Errors.Select(e => new ApiError
                {
                    Code = e.Code,
                    Message = e.Message
                }).ToList()
            })
            { StatusCode = statusCode };
        }
    }
}
