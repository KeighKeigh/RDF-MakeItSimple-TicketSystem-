using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Remove_Attachment_Pms
{
    public partial class RemoveAttachmentPms
    {
        public sealed class RemoveAttachmentPmsCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}
