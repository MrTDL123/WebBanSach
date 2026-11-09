using Microsoft.AspNetCore.Components.Forms;

namespace WebApp.Admin.Utilities
{
    public static class ImageHelper
    {
        public static async Task<string?> ConvertToBase64Async(IBrowserFile? file, int maxWidth = 800, int maxHeight = 800)
        {
            if (file == null) return null;

            var resizedFile = await file.RequestImageFileAsync(file.ContentType, maxWidth, maxHeight);
            using var ms = new MemoryStream();
            using (var stream = resizedFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024))
            {
                await stream.CopyToAsync(ms);
            }
            
            var bytes = ms.ToArray();

            return $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
        }
    }
}
