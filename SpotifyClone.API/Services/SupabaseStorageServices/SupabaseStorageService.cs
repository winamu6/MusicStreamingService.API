using SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces;
using Supabase;
using Supabase.Storage;
using System.Net.Http.Headers;

namespace SpotifyClone.API.Services.SupabaseStorageServices
{
    public class SupabaseStorageService : ISupabaseStorageService
    {
        private readonly Supabase.Client _client;

        public SupabaseStorageService(IConfiguration config)
        {
            var url = config["Supabase:Url"];
            var key = config["Supabase:Key"];

            _client = new Supabase.Client(url, key, new SupabaseOptions
            {
                AutoConnectRealtime = false
            });

            _client.InitializeAsync().Wait();
        }

        public async Task<string> UploadFileAsync(string bucket, string fileName, Stream fileStream)
        {
            var storage = _client.Storage;
            var bucketRef = storage.From(bucket);

            try
            {
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                var result = await bucketRef.Upload(fileBytes, fileName);

                return fileName;
            }
            catch (Supabase.Storage.Exceptions.SupabaseStorageException ex)
            {
                throw new InvalidOperationException($"Bucket '{bucket}' not found in Supabase. Error: {ex.Message}");
            }
        }

        public async Task DeleteFileAsync(string bucket, string filePath)
        {
            var storage = _client.Storage;
            var bucketRef = storage.From(bucket);

            try
            {
                var fileName = Path.GetFileName(filePath);

                await bucketRef.Remove(fileName);
            }
            catch (Supabase.Storage.Exceptions.SupabaseStorageException ex)
            {
                throw new InvalidOperationException($"Error removing file '{filePath}' from bucket '{bucket}'. Error: {ex.Message}");
            }
        }

        public async Task<string> GetPublicUrlAsync(string bucket, string filePath)
        {
            return _client.Storage.From(bucket).GetPublicUrl(filePath);
        }
    }

}
