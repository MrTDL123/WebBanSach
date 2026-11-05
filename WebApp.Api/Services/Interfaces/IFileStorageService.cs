namespace WebApp.Api.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveBase64ImageAsync(string base64Data, string subFolder, string prefix = "images");
        void DeletePhysicalFile(string? relativeUrl);
        void DeletePhysicalFolder(string? relativeFolderPath);
    }
}
