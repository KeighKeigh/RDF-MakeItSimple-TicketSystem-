using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.View_Image_Pms
{
    public partial class ViewImagePms
    {

        public class Handler : IRequestHandler<ViewImagePmsCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ViewImagePmsCommand command, CancellationToken cancellationToken)
            {

                var pmsAttachmentExist = await unitOfWork.Pms
                    .PmsAttachmentExist(command.Id.Value);

                if (pmsAttachmentExist is null)
                    return Result.Failure(PmsError.PmsAttachmentNotExist());

                var filePath = pmsAttachmentExist.Attachment;

                if (!File.Exists(filePath))
                    return Result.Failure(TicketRequestError.FileNotFound());

                var fileResult = await unitOfWork.Pms.FileResult(filePath,null);

                return Result.Success(fileResult);
            }
        }
    }
}
