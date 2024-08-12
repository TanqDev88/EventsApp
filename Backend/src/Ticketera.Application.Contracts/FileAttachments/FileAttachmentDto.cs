using Ticketera.Extensions;
using Volo.Abp.Application.Dtos;

namespace Ticketera.FileAttachments
{
    public class FileAttachmentDto: EntityDto<long>
    {
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; }
        public string FileName { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;

        public long? EventId { get; set; }


        public string Extension => Path.GetExtension();

        public string MimeType => Path.GetMimeType();

        public bool IsFileOfDomain => !Path.StartsWith("http");

        public bool IsImage => FileName.IsImage();

        public bool IsVideo => FileName.IsVideo();

        public string PathToLower => Path.ToLower();


        public string SizeTranslated
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = Size;
                int order = 0;
                while (len >= 1024 && order + 1 < sizes.Length)
                {
                    order++;
                    len = len / 1024;
                }
                return string.Format("{0:0.##} {1}", len, sizes[order]);
            }
        }

    }
}
