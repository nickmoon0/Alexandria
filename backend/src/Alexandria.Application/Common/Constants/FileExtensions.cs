// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace Alexandria.Application.Common.Constants;

public static class FileExtensions
{
    public static class Images
    {
        public const string AVIF = ".avif";
        public const string GIF = ".gif";
        public const string HEIC = ".heic";
        public const string JPEG = ".jpeg";
        public const string JPG = ".jpg";
        public const string PNG = ".png";
        public const string SVG = ".svg";
        public const string WEBP = ".webp";

        public static readonly IReadOnlySet<string> Extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            AVIF,
            GIF,
            HEIC,
            JPEG,
            JPG,
            PNG,
            SVG,
            WEBP,
        };
    }

    public static class Videos
    {
        public const string AVI = ".avi";
        public const string MOV = ".mov";
        public const string MP4 = ".mp4";
        public const string WEBM = ".webm";

        public static readonly IReadOnlySet<string> Extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            AVI,
            MOV,
            MP4,
            WEBM,
        };
    }

    public static class Documents
    {
        public const string DOC = ".doc";
        public const string DOCX = ".docx";
        public const string PDF = ".pdf";
        public const string XLS = ".xls";
        public const string XLSX = ".xlsx";
        public const string PPT = ".ppt";
        public const string PPTX = ".pptx";
        public const string TXT = ".txt";

        public static readonly IReadOnlySet<string> Extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            DOC,
            DOCX,
            PDF,
            XLS,
            XLSX,
            PPT,
            PPTX,
            TXT,
        };
    }
}

public enum FileType
{
    Image,
    Video,
    Document
}