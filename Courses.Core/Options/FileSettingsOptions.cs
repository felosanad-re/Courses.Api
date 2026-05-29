namespace Courses.Core.Options
{
    public class FileSettingsOptions
    {
        public const string SectionName = "FileSettings";

        public string FolderName { get; set; } = "Images";
        public int MaxSize { get; set; }
        public string[] AllowedExtensions { get; set; } = [];
        public string[] AllowedContentTypes { get; set; } = [];
    }
}
