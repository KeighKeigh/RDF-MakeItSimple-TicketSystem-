using DocumentFormat.OpenXml.InkML;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.RejectClosing
{
    public partial class RejectClosingTicket
    {

        public class Handler : IRequestHandler<RejectClosingTicketCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(RejectClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var userDetails = await unitOfWork.User
                    .UserExist(command.Transacted_By);

                var closedTicketExist = await unitOfWork.ClosingTicket
                    .ClosingTicketExist(command.ClosingTicketId);

                if (closedTicketExist is null)          
                    return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());

                if (closedTicketExist.IsActive is false)
                    return Result.Failure(ClosingTicketError.TicketAlreadyCancel());

                var rejectClosingTicket = new ClosingTicket
                {
                    Id = closedTicketExist.Id,
                    RejectClosedBy = command.RejectClosed_By,
                    RejectRemarks = command.Reject_Remarks,
                };

                await unitOfWork.ClosingTicket.RejectClosingTicket(rejectClosingTicket);

                await unitOfWork.ClosingTicket.RemoveClosingApprover(command.ClosingTicketId);

                await unitOfWork.RequestTicket.RemoveTicketHistory(closedTicketExist.TicketConcernId);

                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = closedTicketExist.TicketConcernId,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Reject,
                    Status = $"{TicketingConString.CloseReject} {userDetails.Fullname}",
                    Remarks = command.Reject_Remarks,

                };

                await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Closing request for ticket number {closedTicketExist.TicketConcernId} was rejected.",
                    AddedBy = command.RejectClosed_By.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = closedTicketExist.TicketConcern.UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = closedTicketExist.TicketConcernId,

                };

                await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification,cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

        }
    }
}
