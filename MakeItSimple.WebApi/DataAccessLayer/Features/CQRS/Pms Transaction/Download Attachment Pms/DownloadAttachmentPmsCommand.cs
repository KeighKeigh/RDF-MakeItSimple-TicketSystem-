using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Download_Attachment_Pms
{
    public partial class DownloadAttachmentPms
    {
        public sealed class DownloadAttachmentPmsCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}
