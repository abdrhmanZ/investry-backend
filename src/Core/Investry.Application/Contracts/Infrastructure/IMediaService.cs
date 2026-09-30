using Investry.Application.Models.Media;
using Investry.Domain.Enums;

namespace Investry.Application.Contracts.Infrastructure
{
    public interface IMediaService
    {
        Task<(string Url, string PublicId, MediaType Type)> AddMediaAsync(FileDto file);
        Task<bool> DeleteMediaAsync(string publicId, MediaType type);
    }
}
