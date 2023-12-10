using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using shr.API.Models.Share;
using System.Security.Cryptography;
using System.Text.Json;

namespace shr.API.Services
{
    public class ShareService : IShareService
    {
        public const int ID_SHORT_BYTE_SIZE = 2;
        public const int ID_LONG_BYTE_SIZE = 6;
        public const int SECRET_BYTE_SIZE = 12;

        private readonly AmazonS3Client _client;
        private readonly string _bucketName;

        public ShareService(string bucketName, AmazonS3Config? config)
        {
            _client = new AmazonS3Client(config);
            _bucketName = bucketName;
        }

        public async Task<Share?> FindAsync(string id)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = $"{id}/data.json"
                };

                var response = await _client.GetObjectAsync(request);

                var data = JsonSerializer.Deserialize<Share>(response.ResponseStream);

                return data;
            }
            catch (AmazonS3Exception e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw e;
            }
        }

        public async Task<Share> SaveAsync(Share share)
        {
            await using var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync(stream, share);

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{share.Id}/data.json",
                InputStream = stream,
                ContentType = "application/json"
            };

            await _client.PutObjectAsync(request);

            return share;
        }

        public async Task DeleteAsync(string id)
        {
            var request = new DeleteObjectsRequest
            {
                BucketName = _bucketName,
                Objects = { new KeyVersion { Key = $"{id}/data.json" }, new KeyVersion { Key = $"{id}/content" } }
            };

            await _client.DeleteObjectsAsync(request);
        }

        public async Task UploadAsync(string id, Stream stream)
        {
            var request = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = $"{id}/content",
                BucketName = _bucketName
            };

            var fileTransferUtility = new TransferUtility(_client);
            await fileTransferUtility.UploadAsync(request);
        }

        public async Task<Stream> DownloadAsync(string id)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{id}/content"
            };

            var response = await _client.GetObjectAsync(request);

            return response.ResponseStream;
        }

        public async Task MoveAsync(string srcId, string destId)
        {
            var request = new CopyObjectRequest
            {
                SourceBucket = _bucketName,
                DestinationBucket = _bucketName,
                SourceKey = $"{srcId}/content",
                DestinationKey = $"{destId}/content"
            };

            await _client.CopyObjectAsync(request);
        }

        public async Task<string> GenIdAsync(bool isSecure)
        {
            return await GenIdAsync(isSecure ? ID_LONG_BYTE_SIZE : ID_SHORT_BYTE_SIZE);
        }

        public async Task<string> GenIdAsync(bool isSecure, string old)
        {
            var length = isSecure ? ID_LONG_BYTE_SIZE : ID_SHORT_BYTE_SIZE;

            return length << 1 != old.Length ? await GenIdAsync(length) : old;
        }

        private async Task<string> GenIdAsync(int length)
        {
            while (true)
            {
                var id = Convert.ToHexString(RandomNumberGenerator.GetBytes(length));

                if (await FindAsync(id) is null)
                {
                    return id;
                }
            }
        }

        public string GenSecret()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(SECRET_BYTE_SIZE));
        }
    }
}
