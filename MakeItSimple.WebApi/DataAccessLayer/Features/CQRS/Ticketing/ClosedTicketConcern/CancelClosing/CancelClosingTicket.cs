using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.CancelClosing
{
    public partial class CancelClosingTicket
    {

        public class Handler : IRequestHandler<CancelClosingTicketCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CancelClosingTicketCommand command, CancellationToken cancellationToken)
            {

                var closingTicketExist = await unitOfWork.ClosingTicket
                    .ClosingTicketExist(command.ClosingTicketId);

                if (closingTicketExist is null)
                    return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());

                var approver = await unitOfWork.ClosingTicket
                    .ApproverThatNotNullByClosingTicket(command.ClosingTicketId);
                    
                if(approver is not null) 
                    return Result.Failure(TicketRequestError.TicketAlreadyApproved());

                var ticketConcernExist = await unitOfWork.RequestTicket
                    .TicketConcernExist(closingTicketExist.TicketConcernId);

                await unitOfWork.ClosingTicket.RemoveClosingApprover(command.ClosingTicketId);
                await unitOfWork.RequestTicket.RemoveTicketHistory(ticketConcernExist.Id);
                await unitOfWork.ClosingTicket.CancelClosingTicket(command.ClosingTicketId);

                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = closingTicketExist.TicketConcernId,
                    TransactedBy = command.Transacted_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Cancel,
                    Status = TicketingConString.CloseCancel,
                };

                await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket closing request #{closingTicketExist.TicketConcernId} has been canceled",
                    AddedBy = closingTicketExist.AddedBy.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = closingTicketExist.TicketApprover.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = closingTicketExist.TicketConcernId,
                };

                await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification,cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

        }
    }
}
