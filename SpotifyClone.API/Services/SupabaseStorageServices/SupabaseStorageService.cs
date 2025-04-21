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

            // Инициализация Supabase клиента с использованием переданных конфигураций
            _client = new Supabase.Client(url, key, new SupabaseOptions
            {
                AutoConnectRealtime = false
            });

            // Асинхронная инициализация клиента
            _client.InitializeAsync().Wait();
        }

        // Метод загрузки файла
        public async Task<string> UploadFileAsync(string bucket, string fileName, Stream fileStream)
        {
            var storage = _client.Storage;
            var bucketRef = storage.From(bucket);

            try
            {
                // Попытка загрузить файл в бакет. Если бакет не существует, будет выброшено исключение
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray(); // Сохраняем данные потока в массив байтов
                }

                // Загружаем файл в бакет, передавая массив байтов
                var result = await bucketRef.Upload(fileBytes, fileName);

                // Возвращаем путь к файлу в бакете
                return $"{bucket}/{fileName}";
            }
            catch (Supabase.Storage.Exceptions.SupabaseStorageException ex)
            {
                // Обработка ошибки, если бакет не найден
                throw new InvalidOperationException($"Bucket '{bucket}' not found in Supabase. Error: {ex.Message}");
            }
        }

        // Метод удаления файла
        public async Task DeleteFileAsync(string bucket, string filePath)
        {
            var storage = _client.Storage;
            var bucketRef = storage.From(bucket);

            try
            {
                // Получаем имя файла из пути
                var fileName = Path.GetFileName(filePath);

                // Удаляем файл из бакета
                await bucketRef.Remove(fileName);
            }
            catch (Supabase.Storage.Exceptions.SupabaseStorageException ex)
            {
                // Обработка ошибки, если бакет не найден
                throw new InvalidOperationException($"Error removing file '{filePath}' from bucket '{bucket}'. Error: {ex.Message}");
            }
        }
    }

}
