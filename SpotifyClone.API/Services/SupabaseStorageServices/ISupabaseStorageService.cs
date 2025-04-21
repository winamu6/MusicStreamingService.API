namespace SpotifyClone.API.Services.SupabaseStorageServices
{
    public interface ISupabaseStorageService { 
        Task<string> UploadFileAsync(string bucket, string fileName, Stream fileStream); 
        Task DeleteFileAsync(string bucket, string filePath); 
    }
}
