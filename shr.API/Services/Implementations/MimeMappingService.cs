using Microsoft.AspNetCore.StaticFiles;

namespace shr.API.Services
{
    public class MimeMappingService : IMimeMappingService
    {
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public MimeMappingService(FileExtensionContentTypeProvider contentTypeProvider)
        {
            _contentTypeProvider = contentTypeProvider;
        }

        public string Map(string fileName)
        {
            string? contentType;

            if (!_contentTypeProvider.TryGetContentType(fileName, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}
