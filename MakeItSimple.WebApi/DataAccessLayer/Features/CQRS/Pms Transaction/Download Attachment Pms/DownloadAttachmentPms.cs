using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Download_Attachment_Pms
{
    public partial class DownloadAttachmentPms
    {

        public class Handler : IRequestHandler<DownloadAttachmentPmsCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(DownloadAttachmentPmsCommand command, CancellationToken cancellationToken)
            {

                var pmsAttachmentExist = await unitOfWork.Pms
                    .PmsAttachmentExist(command.Id);

                if (pmsAttachmentExist is null)
                    return Result.Failure(PmsError.PmsAttachmentNotExist());

                var filePath = pmsAttachmentExist.Attachment;
                var documentName = pmsAttachmentExist.FileName;

                if (!File.Exists(filePath))
                {
                    return Result.Failure(TicketRequestError.FileNotFound());
                }

                var fileResult = await unitOfWork.Pms.FileResult(filePath,documentName);

                return Result.Success(fileResult);
            }
        }
    }
}
