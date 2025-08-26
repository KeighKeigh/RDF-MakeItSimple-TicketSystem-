using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.ApprovalDateTicket
{
    public partial class ApprovalDateTicket
    {
        public class Handler : IRequestHandler<ApprovalDateTicketCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ApprovalDateTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;


                var userDetails = await unitOfWork.User
                    .UserExist(command.Transacted_By);

                var approverPermissionList = await unitOfWork.UserRole
                         .UserRoleByPermission(TicketingConString.Approver);

                foreach (var date in command.ApproveDateRequests)
                {

                    var dateApproveTicketExist = await unitOfWork.ApproverDate
                          .ApproverDateExist(date.ApprovalDateTicketId);

                    if (dateApproveTicketExist is null)
                        return Result.Failure(TicketDateError.DateApproverTicketIdNotExist());

                    if (dateApproveTicketExist.IsActive is false)
                        return Result.Failure(TicketDateError.TicketAlreadyCancel());

                    var selectNonDateApproveRequestId = await unitOfWork.ApproverDate
                        .ApproverByMinLevel(dateApproveTicketExist.Id);

                    if (selectNonDateApproveRequestId is not null)
                    {

                        if (dateApproveTicketExist.TicketApprover != command.Users
                            || !approverPermissionList.Any(x => x.Contains(command.Role)))
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());

                        var userApprovalList = await unitOfWork.ApproverDate
                            .ApproverByDateApprovalTicketList(selectNonDateApproveRequestId.ApproverDateId);

                        //var validateUserApprover = await unitOfWork.ApproverDate
                        //    .ApproverPlusOne(selectNonDateApproveRequestId.ApproverDateId, selectNonDateApproveRequestId.ApproverLevel.Value);

                        await unitOfWork.ApproverDate.ApprovedApproval(selectNonDateApproveRequestId.Id);

                        var ticketHistoryApproval = await unitOfWork.RequestTicket
                             .TicketHistoryMinByForApproval(dateApproveTicketExist.TicketConcernId);

                        var updateHistoryApproval = new TicketHistory
                        {
                            Id = ticketHistoryApproval.Id,
                            TransactedBy = command.Transacted_By,
                            Request = TicketingConString.Approve,
                            Status = $"{TicketingConString.DateApproval} {userDetails.Fullname}"
                        };

                        await unitOfWork.RequestTicket.UpdateTicketHistory(updateHistoryApproval, cancellationToken);

                        //if (validateUserApprover is not null)
                        //{

                        //    await unitOfWork.ApproverDate.NextApproverUser(dateApproveTicketExist.Id, validateUserApprover.UserId);

                        //    var addNewTicketTransactionNotification = new TicketTransactionNotification
                        //    {

                        //        Message = $"Ticket number {dateApproveTicketExist.TicketConcernId} is pending for Target Date approval",
                        //        AddedBy = command.Transacted_By.Value,
                        //        Created_At = DateTime.Now,
                        //        ReceiveBy = validateUserApprover.UserId.Value,
                        //        Modules = PathConString.Approval,
                        //        Modules_Parameter = PathConString.ForDateApproval,
                        //        PathId = dateApproveTicketExist.TicketConcernId

                        //    };

                        //    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                        //    var addTicketApproveNotification = new TicketTransactionNotification
                        //    {

                        //        Message = $"Ticket number {dateApproveTicketExist.TicketConcernId} was approved by {userDetails.Fullname}",
                        //        AddedBy = command.Transacted_By.Value,
                        //        Created_At = DateTime.Now,
                        //        ReceiveBy = dateApproveTicketExist.TicketConcern.UserId.Value,
                        //        Modules = PathConString.IssueHandlerConcerns,
                        //        Modules_Parameter = PathConString.ForDateApproval,
                        //        PathId = dateApproveTicketExist.TicketConcernId

                        //    };

                        //    await unitOfWork.RequestTicket.CreateTicketNotification(addTicketApproveNotification, cancellationToken);

                        //}
                        //else
                        //{

                            var approvedClosingTicket = new ApproverDate
                            {
                                Id = dateApproveTicketExist.Id,
                                ApprovedDateBy = command.ApprovedDateBy,

                            };

                            await unitOfWork.ApproverDate.ApprovedDateTicket(approvedClosingTicket, cancellationToken);

                            var approvedTicketByApprovingDate = new TicketConcern
                            {
                                Id = dateApproveTicketExist.TicketConcernId,
                                ApprovedDateBy = command.ApprovedDateBy,
                                ConcernStatus = TicketingConString.OnGoing,
                                IsApprove = true,
                                IsAssigned = true,

                            };

                            await unitOfWork.ApproverDate.ApprovedTicketConcernByApprovingDate(approvedTicketByApprovingDate, cancellationToken);

                            var approvedRequestByApprovingDate = new RequestConcern
                            {
                                Id = dateApproveTicketExist.TicketConcern.RequestConcernId.Value,
                                ConcernStatus = TicketingConString.OnGoing,
                                

                            };

                            await unitOfWork.ApproverDate.ApprovedRequestConcernByApprovingDate(approvedRequestByApprovingDate, cancellationToken);

                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {dateApproveTicketExist.TicketConcernId}, Target Date is approved",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = dateApproveTicketExist.TicketConcern.RequestConcern.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = dateApproveTicketExist.TicketConcernId

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                            var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {dateApproveTicketExist.TicketConcernId}, Target Date is approved",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = dateApproveTicketExist.TicketConcern.UserId.Value,
                                Modules = PathConString.IssueHandlerConcerns,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = dateApproveTicketExist.TicketConcernId

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTransactionConfirmationNotification, cancellationToken);

                            var addNewTicketTransactionOngoing = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {dateApproveTicketExist.TicketConcernId} is now ongoing",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = dateApproveTicketExist.TicketConcern.RequestConcern.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = dateApproveTicketExist.TicketConcernId,


                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                        //}
                    }
                    else
                    {
                        return Result.Failure(TicketDateError.ApproverUnAuthorized());

                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

        }

    }
}
