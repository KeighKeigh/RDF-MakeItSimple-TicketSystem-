using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Upsert_Pms_Approver
{
    public partial class UpsertPmsApprover
    {

        public class Handler : IRequestHandler<UpsertPmsApproverCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(UpsertPmsApproverCommand command, CancellationToken cancellationToken)
            {
                var pmsFormIdList = new List<int>();

                var pmsFormIdNotExist = await unitOfWork.PmsForm
                    .PmsFormIdNotExist(command.PmsFormId);

                if (pmsFormIdNotExist is null)
                    return Result.Failure(PmsFormError.PmsFormIdNotExist());

                foreach (var approver in command.PmsApprovers)
                {

                    if (command.PmsApprovers.Count(x => x.UserId == approver.UserId) > 1)
                        return Result.Failure(PmsApproverError.UserIdDuplicate());

                    if (command.PmsApprovers.Count(x => x.ApproverLevel == approver.ApproverLevel) > 1)
                        return Result.Failure(PmsApproverError.ApproverLevelDuplicate());

                    var userNotExist = await unitOfWork.PmsApprover
                        .UserIdNotExist(approver.UserId);

                    if (userNotExist is null)
                        return Result.Failure(PmsApproverError.UserIdNotExist());

                    var pmsApproverExist = await unitOfWork.PmsApprover
                        .PmsApproverExist(approver.PmsApproverId);

                    if(pmsApproverExist is not null)
                    {
                       pmsFormIdList.Add(pmsApproverExist.Id);

                        var updatePmsApprover = new PmsApprover
                        {

                            Id = approver.PmsApproverId.Value,
                            UserId = approver.UserId,
                            ApproverLevel = approver.ApproverLevel.Value,
                            ModifiedBy = command.Modified_By,

                        };

                       await unitOfWork.PmsApprover.Update(updatePmsApprover);

                    }
                    else
                    {
                        var createPmsApprover = new PmsApprover
                        {
                            PmsFormId = command.PmsFormId,
                            UserId = approver.UserId,
                            ApproverLevel = approver.ApproverLevel,
                            AddedBy = command.Added_By,
                        };

                       await unitOfWork.PmsApprover.Create(createPmsApprover);
                    }
                }

                var pmsApproverByPForm = await unitOfWork.PmsApprover
                    .PmsApproverByPForm(command.PmsFormId);

                var removeApproverList = pmsApproverByPForm
                    .Where(x => !pmsFormIdList.Contains(x.PmsFormId.Value));


                if (removeApproverList.Any())
                {
                    foreach (var remove in removeApproverList)
                    {
                       await unitOfWork.PmsApprover.Remove(remove.Id);
                    }

                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }
        }

    }
}
