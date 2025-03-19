using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Approved_Pms
{
    public partial class ApprovedPms
    {

        public class Handler : IRequestHandler<ApprovedPmsCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ApprovedPmsCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await unitOfWork.User.UserExist(command.UserId);

                var pmsExist = await unitOfWork.Pms.PmsIdNotExist(command.PmsId);

                if (pmsExist is null)
                    return Result.Failure(PmsError.PmsNotExist());

                var pmsApprovalList = await unitOfWork.Pms.MinimumLevelOfApproverList();

                var pmsApproverById = pmsApprovalList
                    .FirstOrDefault(x => x.PmsId == command.PmsId && x.UserId.Equals(command.UserId));

                if (pmsApproverById is null)
                    return Result.Failure(PmsError.PmsNotAuthorized());

                var pmsApprovalListByPms = await unitOfWork.Pms.PmsApprovalByPms(command.PmsId);

                var plusOne = pmsApprovalListByPms
                    .FirstOrDefault(x => x.ApproverLevel == pmsApproverById.ApproverLevel + 1);

                var updatePmsHistory = new PmsHistory
                {
                    PmsId = pmsApproverById.PmsId,
                    Approver_Level = pmsApproverById.ApproverLevel,
                    TransactedBy = pmsApproverById.UserId,
                    TransactionDate = DateTime.Now,
                    Request = PmsConsString.Approved,
                    Status = $"{PmsConsString.PmsApproved} {userDetails.Fullname}"
                };
                
                await unitOfWork.Pms.UpdateApprovalHistory(updatePmsHistory);
                await unitOfWork.Pms.ApprovedPmsApproval(pmsApproverById.Id);

                if (plusOne is not null)
                {
                   
                }
                else
                {
                    await unitOfWork.Pms.ApprovedPms(command.PmsId);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }
        }
    }
}
