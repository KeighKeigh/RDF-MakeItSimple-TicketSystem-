using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using NuGet.Protocol.Plugins;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.RejectDateTicket
{
    public partial class RejectDateTicket
    {
        public class Handler : IRequestHandler<RejectDateTicketCommand, Result>
        {
            public readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(RejectDateTicketCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await unitOfWork.User.UserExist(command.Transacted_By);

                foreach (var date in command.RejectDateRequests)
                {
                    var approvalDateExist = await unitOfWork.ApproverDate.ApproverDateExist(date.ApproverDateId);

                    if (approvalDateExist is null)
                        return Result.Failure(TicketDateError.DateApproverTicketIdNotExist());

                    if (approvalDateExist.IsActive is false)
                        return Result.Failure(TicketDateError.TicketAlreadyCancel());

                    var rejectTargetDate = new ApproverDate
                    {
                        Id = approvalDateExist.Id,
                        RejectDateBy = command.RejectDate_By,
                        RejectRemarks = command.Reject_Remarks,
                    };

                    await unitOfWork.ApproverDate.RejectTargetDateTicket(rejectTargetDate);

                    await unitOfWork.ApproverDate.RemoveTargetDateApprover(date.ApproverDateId);
                    await unitOfWork.RequestTicket.RemoveTicketHistory(approvalDateExist.TicketConcernId);


                    var removeChannel = new RequestConcern
                    {
                        Id = approvalDateExist.TicketConcern.RequestConcernId.Value,
                        TargetDate = null,
                        ConcernStatus = TicketingConString.DateRejected,
                        AssignTo = null,


                    };

                    await unitOfWork.RequestTicket.UpdateRequestConcerns(removeChannel, cancellationToken);

                    var removeOpenTicket = new TicketConcern
                    {
                        Id = approvalDateExist.TicketConcernId,
                        AssignTo = null,
                        IsAssigned = false,
                        ConcernStatus = TicketingConString.DateRejected,
                        TargetDate = null,

                    };

                    await unitOfWork.RequestTicket.UpdateTicketConcernss(removeOpenTicket, cancellationToken);

                    
                    var addTicketHistory = new TicketHistory
                    {
                        TicketConcernId = approvalDateExist.TicketConcernId,
                        TransactedBy = command.Transacted_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Reject,
                        Status = $"{TicketingConString.CloseReject} {userDetails.Fullname}",
                        Remarks = command.Reject_Remarks,

                    };

                    await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                    var addNewTicketTransactionNotification = new TicketTransactionNotification
                    {

                        Message = $"Closing request for ticket number {approvalDateExist.TicketConcernId} was rejected.",
                        AddedBy = command.RejectDate_By.Value,
                        Created_At = DateTime.Now,
                        ReceiveBy = approvalDateExist.TicketConcern.UserId.Value,
                        Modules = PathConString.IssueHandlerConcerns,
                        Modules_Parameter = PathConString.OpenTicket,
                        PathId = approvalDateExist.TicketConcernId,

                    };

                    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
        }

    }
}
