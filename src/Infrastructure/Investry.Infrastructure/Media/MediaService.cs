using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Models.Media;
using Investry.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Investry.Infrastructure.Media
{
    public class MediaService : IMediaService
    {
        private readonly Cloudinary _cloudinary;

        public MediaService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        public async Task<(string Url, string PublicId, MediaType Type)> AddMediaAsync(FileDto file)
        {
            if (file?.Content == null || file.Content.Length == 0)
                throw new ArgumentException("File content is empty.");

            var fileType = GetMediaType(file.ContentType);
            var stream = file.Content;
            UploadResult uploadResult;

            switch (fileType)
            {
                case MediaType.Image:
                    uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams { File = new FileDescription(file.FileName, stream) });
                    break;
                case MediaType.Video:
                    uploadResult = await _cloudinary.UploadAsync(new VideoUploadParams { File = new FileDescription(file.FileName, stream) });
                    break;
                default:
                    uploadResult = await _cloudinary.UploadAsync(new RawUploadParams { File = new FileDescription(file.FileName, stream) });
                    break;
            }

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");

            return (uploadResult.SecureUrl.AbsoluteUri, uploadResult.PublicId, fileType);
        }

        public async Task<bool> DeleteMediaAsync(string publicId, MediaType type)
        {
            var resourceType = type switch
            {
                MediaType.Video => ResourceType.Video,
                MediaType.Document => ResourceType.Raw,
                _ => ResourceType.Image
            };
            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId) { ResourceType = resourceType });
            return result.Result == "ok";
        }
        private MediaType GetMediaType(string contentType)
        {
            if (contentType.StartsWith("image/")) return MediaType.Image;
            if (contentType.StartsWith("video/")) return MediaType.Video;
            return MediaType.Document;
        }
    }
}
