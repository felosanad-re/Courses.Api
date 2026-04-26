using Courses.Core.Models;

namespace Courses.Core.ModelsDTO
{
    public class ApplicationServiceResult<T>
    {
        public bool Succeed { get; set; }
        public string? Message { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public T? Data { get; set; }

        // Static Method For Succeeded
        public static ApplicationServiceResult<T> Success(T data, string message)
        {
            return new ApplicationServiceResult<T>
            {
                Succeed = true,
                Message = message,
                Data = data
            };
        }

        // Static Method For Failed

        public static ApplicationServiceResult<T> Fail(string message)
        {
            return new ApplicationServiceResult<T>
            {
                Succeed = false,
                Message = message,
            };
        }
    }
}
