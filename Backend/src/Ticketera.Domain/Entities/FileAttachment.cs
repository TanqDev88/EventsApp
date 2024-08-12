using Ticketera.Enum;
using Ticketera.Events;

namespace Ticketera.Entities
{
    public class FileAttachment : Base
    {
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileNameGuid { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
        public FileAttachmentType FileType { get; set; }

        public long? EventId { get; set; }
        public virtual Event Event { get; set; }

    }
}
