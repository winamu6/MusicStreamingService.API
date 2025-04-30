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
            var key = config["Supabase:ServiceRoleKey"];

            _client = new Supabase.Client(url, key, new SupabaseOptions
            {
                AutoConnectRealtime = false
            });

            try
            {
                _client.InitializeAsync().Wait();
            }
            catch (Exception ex)
            {
                throw new Exception("Supabase init failed: " + ex.Message);
            }
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

                // Определите MIME-тип на основе расширения файла
                var contentType = GetMimeType(fileName);

                var result = await bucketRef.Upload(
                    fileBytes,
                    fileName,
                    new Supabase.Storage.FileOptions
                    {
                        ContentType = contentType
                    }
                );

                return fileName;
            }
            catch (Supabase.Storage.Exceptions.SupabaseStorageException ex)
            {
                throw new InvalidOperationException($"Error uploading to '{bucket}': {ex.Message}");
            }
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
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
