using System.IO;

namespace proj.Services
{
    public class ImageUploadService
    {
        public static bool CheckExtensionValidity(IFormFile file)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            return allowedExtensions.Contains(Path.GetExtension(file.FileName).ToString());
        }
        public static string UploadFile(IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                return "/images/" + fileName;
            }

            return null; // or throw an exception or handle the case where no file is provided
        }

    }
}
