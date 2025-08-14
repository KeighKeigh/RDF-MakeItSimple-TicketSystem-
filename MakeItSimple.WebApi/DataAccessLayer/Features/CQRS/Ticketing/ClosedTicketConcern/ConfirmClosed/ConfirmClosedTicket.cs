using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ConfirmClosed
{
    public partial class ConfirmClosedTicket
    {

        public class Handler : IRequestHandler<ConfirmClosedTicketCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ConfirmClosedTicketCommand command, CancellationToken cancellationToken)
            {

                foreach (var confirm in command.ConfirmTicketRequests)
                {
                    var requestConcernExist = await unitOfWork.RequestTicket
                        .RequestConcernExist(confirm.RequestConcernId);

                    //var requestorConfirmation = await unitOfWork.ClosingTicket.RequestorConfirmation(command.RequestConcernId, command.Transacted_By);
                    if (requestConcernExist is null)
                        return Result.Failure(TicketRequestError.RequestConcernIdNotExist());

                    if (requestConcernExist.TicketConcerns.First().IsClosedApprove is not true)
                        return Result.Failure(TicketRequestError.TicketAlreadyReject());

                    if (requestConcernExist.Is_Confirm is true)
                        return Result.Failure(TicketRequestError.ConfirmAlready());

                    var ticketConcernExist = await unitOfWork.RequestTicket
                        .TicketConcernByRequest(confirm.RequestConcernId);

                    await unitOfWork.ClosingTicket.ConfirmClosingTicket(confirm.RequestConcernId);

                    await unitOfWork.ClosingTicket.ConfirmTicketHistory(ticketConcernExist.Id);

                    var addNewTicketTransactionNotification = new TicketTransactionNotification
                    {

                        Message = $"Ticket number {ticketConcernExist.Id} has been closed",
                        AddedBy = command.Transacted_By.Value,
                        Created_At = DateTime.Now,
                        ReceiveBy = ticketConcernExist.UserId.Value,
                        Modules = PathConString.IssueHandlerConcerns,
                        Modules_Parameter = PathConString.Closed,
                        PathId = ticketConcernExist.Id

                    };

                    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);
                }
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

        }
    }
}
