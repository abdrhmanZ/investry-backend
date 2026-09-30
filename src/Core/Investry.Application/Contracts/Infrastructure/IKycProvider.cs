using Investry.Application.DTOs;

namespace Investry.Application.Contracts.Infrastructure
{
    public interface IKycProvider
    {
        Task<StartKycSessionResponse> CreateSessionAsync(string vendorData);
    }
}
