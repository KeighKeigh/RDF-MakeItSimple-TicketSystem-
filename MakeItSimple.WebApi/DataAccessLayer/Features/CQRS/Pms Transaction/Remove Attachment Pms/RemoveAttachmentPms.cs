using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Remove_Attachment_Pms
{
    public partial class RemoveAttachmentPms
    {

        public class Handler : IRequestHandler<RemoveAttachmentPmsCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(RemoveAttachmentPmsCommand command, CancellationToken cancellationToken)
            {
                var pmsAttachmentExist = await unitOfWork.Pms
                    .PmsAttachmentExist(command.Id);

                if (pmsAttachmentExist is null)
                    return Result.Failure(PmsError.PmsAttachmentNotExist());

                await unitOfWork.Pms.DeletePmsAttachment(command.Id);

                return Result.Success();
            }
        }
    }
}
