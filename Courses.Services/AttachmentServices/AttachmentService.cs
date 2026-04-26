using Courses.Core.Services.Contract.AttachmentServices;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Courses.Services.AttachmentServices
{
    public class AttachmentService : IAttachmentService
    {
        private static readonly Regex SafeFolderNamePattern = new("^[a-zA-Z0-9_-]+$", RegexOptions.Compiled);

        public async Task<string> UploadAsync(IFormFile file, string folderName, IEnumerable<string> allowedExtensions, int maxSize)
        {
            var normalizedFolder = NormalizeFolderName(folderName);
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var normalizedExtensions = allowedExtensions.Select(item => item.ToLowerInvariant()).ToHashSet();

            if (!normalizedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file extension.");
            }

            if (file.Length > maxSize)
            {
                throw new InvalidOperationException("The file exceeds the allowed size.");
            }

            var rootPath = GetRootPath();
            Directory.CreateDirectory(rootPath);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = BuildSafePath(rootPath, normalizedFolder, fileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return fileName;
        }

        public async Task<List<string>> UploadsAsync(List<IFormFile> files, string folderName, IEnumerable<string> allowedExtensions, int maxSize)
        {
            if (files == null || files.Count == 0)
            {
                throw new InvalidOperationException("No files uploaded.");
            }

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
            var normalizedFolder = NormalizeFolderName(folderName);
            var normalizedFileName = Path.GetFileName(fileName);
            if (string.IsNullOrWhiteSpace(normalizedFileName) || normalizedFileName != fileName)
            {
                throw new InvalidOperationException("Invalid file name.");
            }

            var filePath = BuildSafePath(GetRootPath(), normalizedFolder, normalizedFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        private static string GetRootPath()
            => Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files");

        private static string NormalizeFolderName(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName) || !SafeFolderNamePattern.IsMatch(folderName))
            {
                throw new InvalidOperationException("Invalid folder name.");
            }

            return folderName;
        }

        private static string BuildSafePath(string rootPath, string folderName, string fileName)
        {
            var folderPath = Path.Combine(rootPath, folderName);
            Directory.CreateDirectory(folderPath);

            var fullRootPath = Path.GetFullPath(rootPath);
            var fullPath = Path.GetFullPath(Path.Combine(folderPath, fileName));

            if (!fullPath.StartsWith(fullRootPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid file path.");
            }

            return fullPath;
        }
    }
}