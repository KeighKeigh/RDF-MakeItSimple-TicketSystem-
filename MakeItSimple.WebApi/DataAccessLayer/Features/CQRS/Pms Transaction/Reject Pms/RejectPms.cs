using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Reject_Pms
{
    public class RejectPms
    {
        public class RejectPmsCommand : IRequest<Result>
        {
            public Guid Rejected_By { get; set; }
            public int id {  get; set; }
        }

        public class Handler : IRequestHandler<RejectPmsCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(RejectPmsCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await unitOfWork.User.UserExist(command.Rejected_By);

                var pmsExist = await unitOfWork.Pms.PmsIdNotExist(command.id);

                if (pmsExist is null)
                    return Result.Failure(PmsError.PmsNotExist());

                await unitOfWork.Pms.RejectPms(command.id);

                await unitOfWork.Pms.DeletePmsDetail(command.id);
                await unitOfWork.Pms.DeletePmsApproval(command.id);
                await unitOfWork.Pms.DeletePmsHistory(command.id);

                var createRejectedPmsHistory = new PmsHistory
                {
                    PmsId = command.id,
                    TransactedBy = command.Rejected_By,
                    TransactionDate = DateTime.Now,
                    Request = PmsConsString.Rejected,
                    Status = $"{PmsConsString.PmsRejected} {userDetails.Fullname}"

                };

                await unitOfWork.Pms.CreateHistory(createRejectedPmsHistory);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }
    }
}
