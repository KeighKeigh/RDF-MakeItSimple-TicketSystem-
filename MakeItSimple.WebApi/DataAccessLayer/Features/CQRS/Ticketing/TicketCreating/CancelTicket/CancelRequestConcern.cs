using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.CancelTicket
{
    public partial class CancelRequestConcern
    {
        public class Handler : IRequestHandler<CancelRequestConcernCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CancelRequestConcernCommand command, CancellationToken cancellationToken)
            {

                var requestTransactionExist = await unitOfWork.RequestTicket
                    .RequestConcernExist(command.RequestConcernId);

                if (requestTransactionExist is null)
                    return Result.Failure(TicketRequestError.RequestConcernIdNotExist());

                if (requestTransactionExist.TicketConcerns.First().IsApprove == true)
                    return Result.Failure(TicketRequestError.TicketAlreadyAssign());


                await unitOfWork.RequestTicket.CancelledRequestConcern(command.RequestConcernId);

                var ticketConcernExist = await unitOfWork.RequestTicket
                         .TicketConcernByRequest(requestTransactionExist.Id);

                await unitOfWork.RequestTicket.CancelledTicketConcern(ticketConcernExist.Id);

                await unitOfWork.RequestTicket.CancelledTicketAttachment(ticketConcernExist.Id);

                var userReceiver = await unitOfWork.Receiver
                        .ReceiverExistByBusinessUnitId(requestTransactionExist.User.BusinessUnitId);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Ticket number {ticketConcernExist.Id} has been canceled",
                    AddedBy = requestTransactionExist.UserId.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = userReceiver.UserId.Value,
                    Modules = PathConString.ConcernTickets,
                    Modules_Parameter = PathConString.ForApproval,
                    PathId = ticketConcernExist.Id,

                };

                await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification,cancellationToken);


                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

        }
    }
}
