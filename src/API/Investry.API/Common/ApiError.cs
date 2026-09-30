namespace Investry.API.Common
{
    public class ApiError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
    }
}
