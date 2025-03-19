using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Cancel_Pms
{
    public partial class CancelPms
    {

        public class Handler : IRequestHandler<CancelPmsCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CancelPmsCommand command, CancellationToken cancellationToken)
            {
                var pmsExist = await unitOfWork.Pms
                    .PmsIdNotExist(command.Id);

                if (pmsExist is null)
                    return Result.Failure(PmsError.PmsNotExist());

                var pmsAnyAlreadyApproved = await unitOfWork.Pms.PmsApprovalIsApprovedTrueByPms(command.Id);

                if(pmsAnyAlreadyApproved.Any())
                    return Result.Failure(PmsError.PmsAlreadyInApproval());

              
                await unitOfWork.Pms.DeletePms(command.Id);
                await unitOfWork.Pms.DeletePmsApproval(command.Id);
                await unitOfWork.Pms.DeletePmsDetail(command.Id);
                await unitOfWork.Pms.DeletePmsHistory(command.Id);

                return Result.Success();
            }
        }
    }
}
