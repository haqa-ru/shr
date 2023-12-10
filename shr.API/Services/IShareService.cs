using shr.API.Models.Share;

namespace shr.API.Services
{
    public interface IShareService
    {
        Task<Share?> FindAsync(string id);

        Task<Share> SaveAsync(Share share);
        Task DeleteAsync(string id);

        Task UploadAsync(string id, Stream stream);
        Task<Stream> DownloadAsync(string id);
        Task MoveAsync(string srcId, string destId);

        Task<string> GenIdAsync(bool isSecure);
        Task<string> GenIdAsync(bool isSecure, string old);

        string GenSecret();
    }
}
