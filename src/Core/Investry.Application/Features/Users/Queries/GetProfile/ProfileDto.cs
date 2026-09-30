namespace Investry.Application.Features.Users.Queries.GetProfile
{
    public class ProfileDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string KycStatus { get; set; }
    }
}
