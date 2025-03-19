using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.Models.Setup.FormSetup
{
    public class PmsAttachment : BaseIdEntity
    {

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }

        public string Attachment { get; set; }
        public string FileName { get; set; }
        public decimal? FileSize { get; set; }

        public int? PmsId { get; set; }
        public virtual Pms Pms { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}
