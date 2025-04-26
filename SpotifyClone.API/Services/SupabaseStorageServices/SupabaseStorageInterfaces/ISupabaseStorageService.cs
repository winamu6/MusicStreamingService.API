namespace SpotifyClone.API.Services.SupabaseStorageServices.SupabaseStorageInterfaces
{
    public interface ISupabaseStorageService { 
        Task<string> UploadFileAsync(string bucket, string fileName, Stream fileStream); 
        Task DeleteFileAsync(string bucket, string filePath);
        Task<string> GetPublicUrlAsync(string bucket, string filePath);

    }
}
