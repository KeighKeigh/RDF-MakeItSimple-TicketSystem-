using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApprovalTicket.RequestApprovalReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AssignTicket.AddRequestConcernReceiver;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApprovalTicket
{
    public partial class RequestApprovalReceiver
    {

        public class Handler : IRequestHandler<RequestApprovalReceiverCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;
            private readonly MisDbContext misDbContext;

            public Handler(IUnitOfWork unitOfWork, MisDbContext misDbContext)
            {
                this.unitOfWork = unitOfWork;
                this.misDbContext = misDbContext;

            }

            public async Task<Result> Handle(RequestApprovalReceiverCommand command, CancellationToken cancellationToken)
            {
                

                var receiverPermissionList = await unitOfWork.UserRole
                    .UserRoleByPermission(TicketingConString.Receiver);


                var ticketConcernExist = await unitOfWork.RequestTicket
                    .TicketConcernExist(command.TicketConcernId);
                var PermissionUser = await unitOfWork.UserRole.UserRoleCheckPermission(TicketingConString.Receiver);



                if (ticketConcernExist is null)
                    return Result.Failure(TicketRequestError.TicketConcernIdNotExist());


                var businessUnitList = await unitOfWork.BusinessUnit
                .BusinessUnitExist(ticketConcernExist.RequestorByUser.BusinessUnitId);

                
                    var receiverList = await unitOfWork.Receiver
                .ReceiverExistByBusinessUnitId(businessUnitList.Id);

                    if (receiverList is null)
                        return Result.Failure(TicketRequestError.NoReceiver());

               

                if (receiverList.UserId == command.UserId && receiverPermissionList.Contains(command.Role))
                {
                    var approveOpenTicket = new TicketConcern
                    {
                        Id = ticketConcernExist.Id,
                        IsApprove = true,
                        ApprovedBy = command.Approved_By,
                        ApprovedAt = DateTime.Now,
                        ConcernStatus = TicketingConString.CurrentlyFixing,
                        IsAssigned = true,

                    };

                    await unitOfWork.RequestTicket.ApproveOpenTicket(approveOpenTicket, cancellationToken);

                    if (ticketConcernExist.RequestConcernId is not null)
                    {
                        var updateRequestConcern = new RequestConcern
                        {
                            Id = ticketConcernExist.RequestConcernId.Value,
                            ConcernStatus = TicketingConString.CurrentlyFixing

                        };

                        await unitOfWork.RequestTicket.UpdateRequestConcern(updateRequestConcern, cancellationToken);
                    }

                    var addTicketHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        TransactedBy = command.UserId,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.ConcernAssign,
                        Status = $"{TicketingConString.RequestAssign} {ticketConcernExist.User.Fullname}"
                    };

                    await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                }
                else
                {
                    return Result.Failure(TicketRequestError.NotAutorize());
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

        }
    }
}
