using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ApprovalClosing
{
    public partial class ApprovalClosingTicket
    {
        public class Handler : IRequestHandler<ApproveClosingTicketCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ApproveClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;


                var userDetails = await unitOfWork.User
                    .UserExist(command.Transacted_By);

                var approverPermissionList = await unitOfWork.UserRole
                         .UserRoleByPermission(TicketingConString.Approver);

                foreach (var close in command.ApproveClosingRequests)
                {

                    var closingTicketExist = await unitOfWork.ClosingTicket
                          .ClosingTicketExist(close.ClosingTicketId);

                    if (closingTicketExist is null)
                        return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());

                    if(closingTicketExist.IsActive is false)
                        return Result.Failure(ClosingTicketError.TicketAlreadyCancel());

                    var selectClosedRequestId = await unitOfWork.ClosingTicket
                        .ApproverByMinLevel(closingTicketExist.Id);

                    if (selectClosedRequestId is not null)
                    {

                        if (closingTicketExist.TicketApprover != command.Users
                            || !approverPermissionList.Any(x => x.Contains(command.Role)))                    
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());

                        var userApprovalList = await unitOfWork.ClosingTicket
                            .ApproverByClosingTicketList(selectClosedRequestId.ClosingTicketId);

                        //var validateUserApprover = await unitOfWork.ClosingTicket
                        //    .ApproverPlusOne(selectClosedRequestId.ClosingTicketId);

                        await unitOfWork.ClosingTicket.ApprovedApproval(selectClosedRequestId.Id);

                        var ticketHistoryApproval = await unitOfWork.RequestTicket
                             .TicketHistoryMinByForApproval(closingTicketExist.TicketConcernId);

                        var updateHistoryApproval = new TicketHistory
                        {
                            Id = ticketHistoryApproval.Id,
                            TransactedBy = command.Transacted_By,
                            Request = TicketingConString.Approve,
                            Status = $"{TicketingConString.CloseApprove} {userDetails.Fullname}"
                        };

                        await unitOfWork.RequestTicket.UpdateTicketHistory(updateHistoryApproval, cancellationToken);

                        //var resolutionHIstory = new TicketHistory
                        //{
                        //    Id = ticketHistoryApproval.Id,
                        //    TransactedBy = command.Transacted_By,
                        //    Request = "Resolution",
                        //    Status = $"Resolution : {closingTicketExist.Resolution}"
                        //};

                        //await unitOfWork.RequestTicket.UpdateTicketHistory(updateHistoryApproval, cancellationToken);

                        var resolutionHIstory = new TicketHistory
                        {
                            TicketConcernId = closingTicketExist.TicketConcernId,
                            TransactedBy = closingTicketExist.TicketConcern.AssignTo,
                            TransactionDate = DateTime.Now,
                            Request = "Resolution",
                            Status = $"Resolution : {closingTicketExist.Resolution}"
                        };

                        await unitOfWork.RequestTicket.CreateTicketHistory(resolutionHIstory, cancellationToken);


                     

                            var approvedClosingTicket = new ClosingTicket
                            {
                                Id = closingTicketExist.Id,
                                ClosedBy = command.Closed_By,
                                IsClosing = true
                            };

                            await unitOfWork.ClosingTicket.ApprovedClosingTicket(approvedClosingTicket,cancellationToken);



                            var approvedTicketByClosingTicket = new TicketConcern
                            {
                                Id = closingTicketExist.TicketConcernId,
                                ClosedApproveBy = command.Closed_By,
                                IsDone = true,
                                ConcernStatus = TicketingConString.NotConfirm,
                                IsClosedApprove = true,
                            };

                            await unitOfWork.ClosingTicket.ApprovedTicketConcernByClosing(approvedTicketByClosingTicket,cancellationToken);

                            var approvedRequestByClosingTicket = new RequestConcern
                            {
                                Id = closingTicketExist.TicketConcern.RequestConcernId.Value,
                                Resolution = closingTicketExist.Resolution,
                                ConcernStatus = TicketingConString.NotConfirm,
                                CategoryConcernName = closingTicketExist.CategoryConcernName,

                            };

                            await unitOfWork.ClosingTicket.ApprovedRequestConcernByClosing(approvedRequestByClosingTicket,cancellationToken);

                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {closingTicketExist.TicketConcernId} is pending for closing Confirmation",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = closingTicketExist.TicketConcern.RequestConcern.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.ForConfirmation,
                                PathId = closingTicketExist.TicketConcernId

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification,cancellationToken);

                            var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {closingTicketExist.TicketConcernId} is waiting for Confirmation",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = closingTicketExist.TicketConcern.UserId.Value,
                                Modules = PathConString.IssueHandlerConcerns,
                                Modules_Parameter = PathConString.ForConfirmation,
                                PathId = closingTicketExist.TicketConcernId

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTransactionConfirmationNotification, cancellationToken);


                        
                    }
                    else
                    {
                        return Result.Failure(ClosingTicketError.ApproverUnAuthorized());

                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

        }
    }
}
