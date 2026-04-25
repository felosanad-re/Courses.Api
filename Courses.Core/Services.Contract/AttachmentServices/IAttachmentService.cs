using Microsoft.AspNetCore.Http;

namespace Courses.Core.Services.Contract.AttachmentServices
{
    public interface IAttachmentService
    {
        // Take Any Type [Image || PDF]
        Task<string> UploadAsync(IFormFile file, string folderName, IEnumerable<string> allowedExtensions, int maxSize);

        // Add Multi Images
        Task<List<string>> UploadsAsync(List<IFormFile> files, string folderName, IEnumerable<string> allowedExtensions, int maxSize);

        Task DeleteImageAsync(string fileName, string folderName);
    }
}
