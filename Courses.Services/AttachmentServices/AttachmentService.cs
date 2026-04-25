using Courses.Core.Services.Contract.AttachmentServices;
using Microsoft.AspNetCore.Http;

namespace Courses.Services.AttachmentServices
{
    public class AttachmentService : IAttachmentService
    {
        public async Task<string> UploadAsync(IFormFile file, string folderName, IEnumerable<string> allowedExtensions, int maxSize)
        {
            // Get Extension
            var extention = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extention)) throw new Exception("Invalid file extension");
            if (file.Length > maxSize) throw new Exception("The File should be 2Mb");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath); // Create New FolderPath
            var fileName = $"{Guid.NewGuid()}{extention}"; // Create New File Name Unique
            var filePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return fileName;
        }

        public async Task<List<string>> UploadsAsync(List<IFormFile> files, string folderName, IEnumerable<string> allowedExtensions, int maxSize)
        {
            if (files == null || !files.Any())
                throw new Exception("No files uploaded");

            var fileNames = new List<string>();
            foreach (var file in files)
            {
                var fileName = await UploadAsync(file, folderName, allowedExtensions, maxSize);
                fileNames.Add(fileName);
            }
            return fileNames;
        }

        public Task DeleteImageAsync(string fileName, string folderName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot",
                "Files",
                folderName, // represent Saved folder[products- brands...]
                fileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
            return Task.CompletedTask;
        }
    }
}
