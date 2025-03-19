using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Update_Pms_Approver_Status
{
    public partial class UpdatePmsApproverStatus
    {

        public class Handler : IRequestHandler<UpdatePmsApproverStatusCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpdatePmsApproverStatusCommand command, CancellationToken cancellationToken)
            {
                var pmsApproverExist = await unitOfWork.PmsForm
                    .PmsFormIdNotExist(command.Id);

                if(pmsApproverExist is null)
                    return Result.Failure(PmsFormError.PmsFormIdNotExist());

                 
                await unitOfWork.PmsApprover.UpdateStatus(command.Id);

                //foreach(var approver in pmsApproverListByForm)
                //{
                //    await unitOfWork.PmsApprover.UpdateStatus(approver.Id);
                //}

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
        }
    }
}
