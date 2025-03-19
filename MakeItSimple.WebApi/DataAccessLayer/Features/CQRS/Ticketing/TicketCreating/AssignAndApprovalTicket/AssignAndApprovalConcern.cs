using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;

using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AssignAndApprovalTicket.AssignAndApprovalConcern;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddAndApprovalTicket
{
    public partial class AssignAndApprovalConcern
    {
        public class Handler : IRequestHandler<AssignAndApprovalConcernCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;
           

            public Handler(IUnitOfWork unitOfWork, MisDbContext misDbContext)
            {
                this.unitOfWork = unitOfWork;
               

            }

            public async Task<Result> Handle(AssignAndApprovalConcernCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                var userDetails = await unitOfWork.User
                    .UserExist(command.UserId);

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



                // AddRequest

                var requestConcernId = new int();
                var ticketCategoryList = new List<int>();
                var ticketSubCategoryList = new List<int>();



                var handlerDetails = await unitOfWork.User
                    .UserExist(command.UserId);



                var issueHandlerPermissionList = await unitOfWork.UserRole
                    .UserRoleByPermission(TicketingConString.Requestor);

                var requestorPermissionList = await unitOfWork.UserRole
                    .UserRoleByPermission(TicketingConString.Requestor);

                switch (await unitOfWork.User.UserExist(command.UserId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.UserNotExist());
                }

                switch (await unitOfWork.Channel.ChannelExist(command.ChannelId))
                {
                    case null:
                        return Result.Failure(TicketRequestError.ChannelNotExist());
                }

                foreach (var category in command.RequestorTicketCategories)
                {
                    var ticketCategoryExist = await unitOfWork.Category
                      .CategoryExist(category.CategoryId);

                    if (ticketCategoryExist is null)
                        return Result.Failure(TicketRequestError.CategoryNotExist());
                }

                foreach (var subCategory in command.RequestorTicketSubCategories)
                {
                    var ticketSubCategoryExist = await unitOfWork.SubCategory
                        .SubCategoryExist(subCategory.SubCategoryId);

                    if (ticketSubCategoryExist is null)
                        return Result.Failure(TicketRequestError.SubCategoryNotExist());

                }

                if (dateToday > command.Target_Date)
                    return Result.Failure(TicketRequestError.DateTimeInvalid());



                if (ticketConcernExist is not null)
                {
                    if (ticketConcernExist.IsActive is false)
                        return Result.Failure(TicketRequestError.TicketAlreadyCancel());

                    var assignTicket = new TicketConcern
                    {
                        Id = ticketConcernExist.Id,
                        UserId = command.UserId,
                        TargetDate = command.Target_Date,
                        ModifiedBy = command.Modified_By,
                    };

                    await unitOfWork.RequestTicket.UpdateTicketConcern(assignTicket, cancellationToken);

                    if (ticketConcernExist.RequestConcernId is not null)
                    {

                        var updateRequestConcern = new RequestConcern
                        {
                            Id = ticketConcernExist.RequestConcernId.Value,
                            ChannelId = command.ChannelId,
                            Concern = command.Concern,
                            ModifiedBy = command.Modified_By,

                        };

                        await unitOfWork.RequestTicket.UpdateRequestConcern(updateRequestConcern, cancellationToken);
                    }

                    var addNewTicketTransactionNotification = new TicketTransactionNotification
                    {

                        Message = $"Ticket number {ticketConcernExist.Id} has been assigned",
                        AddedBy = command.Added_By.Value,
                        Created_At = DateTime.Now,
                        ReceiveBy = command.UserId.Value,
                        Modules = PathConString.IssueHandlerConcerns,
                        Modules_Parameter = PathConString.OpenTicket,
                        PathId = ticketConcernExist.Id,

                    };

                    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                    var addNewTicketTransactionOngoing = new TicketTransactionNotification
                    {

                        Message = $"Ticket number {ticketConcernExist.RequestConcernId} is now ongoing",
                        AddedBy = command.Added_By.Value,
                        Created_At = DateTime.Now,
                        ReceiveBy = ticketConcernExist.RequestConcern.UserId.Value,
                        Modules = PathConString.ConcernTickets,
                        Modules_Parameter = PathConString.Ongoing,
                        PathId = ticketConcernExist.RequestConcernId.Value,

                    };

                    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                    requestConcernId = ticketConcernExist.Id;

                }
                else
                {

                    var requestorDetails = await unitOfWork.User
                         .UserExist(command.Requestor_By);

                    var addRequestConcern = new RequestConcern
                    {
                        UserId = command.Requestor_By,
                        Concern = command.Concern,
                        AddedBy = command.Added_By,
                        ConcernStatus = TicketingConString.CurrentlyFixing,
                        CompanyId = requestorDetails.CompanyId,
                        BusinessUnitId = requestorDetails.BusinessUnitId,
                        DepartmentId = requestorDetails.DepartmentId,
                        UnitId = requestorDetails.UnitId,
                        SubUnitId = requestorDetails.SubUnitId,
                        LocationId = requestorDetails.LocationId,
                        ChannelId = command.ChannelId,
                        DateNeeded = command.DateNeeded,
                        ContactNumber = command.Contact_Number,
                        RequestType = command.Request_Type,
                        BackJobId = command.BackJobId,
                        Notes = command.Notes,
                        IsDone = false,

                    };

                    await unitOfWork.RequestTicket.CreateRequestConcern(addRequestConcern, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    requestConcernId = addRequestConcern.Id;

                    var addTicketConcern = new TicketConcern
                    {
                        RequestConcernId = requestConcernId,
                        TargetDate = command.Target_Date,
                        UserId = command.UserId,
                        RequestorBy = command.Requestor_By,
                        IsApprove = true,
                        AddedBy = command.Added_By,
                        ConcernStatus = TicketingConString.CurrentlyFixing,
                        IsAssigned = true,
                        ApprovedBy = command.Added_By,
                        ApprovedAt = DateTime.Now,


                    };

                    await unitOfWork.RequestTicket.CreateTicketConcern(addTicketConcern, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    ticketConcernExist = addTicketConcern;

                    var addRequestTicketHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        TransactedBy = command.Added_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Request,
                        Status = $"{TicketingConString.ConcernCreated} {userDetails.Fullname}"
                    };

                    await unitOfWork.RequestTicket.CreateTicketHistory(addRequestTicketHistory, cancellationToken);

                    var assignedTicketHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcernExist.Id,
                        TransactedBy = command.Added_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.ConcernAssign,
                        Status = $"{TicketingConString.RequestAssign} {handlerDetails.Fullname}"
                    };

                    await unitOfWork.RequestTicket.CreateTicketHistory(assignedTicketHistory, cancellationToken);


                    var addNewTicketTransactionNotification = new TicketTransactionNotification
                    {

                        Message = $"Ticket number {ticketConcernExist.Id} has been assigned",
                        AddedBy = command.Added_By.Value,
                        Created_At = DateTime.Now,
                        ReceiveBy = command.UserId.Value,
                        Modules = PathConString.IssueHandlerConcerns,
                        Modules_Parameter = PathConString.OpenTicket,
                        PathId = ticketConcernExist.Id,

                    };

                    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                    var addNewTicketTransactionOngoing = new TicketTransactionNotification
                    {

                        Message = $"Ticket number {ticketConcernExist.RequestConcernId} is now ongoing",
                        AddedBy = command.Added_By.Value,
                        Created_At = DateTime.Now,
                        ReceiveBy = command.Requestor_By.Value,
                        Modules = PathConString.ConcernTickets,
                        Modules_Parameter = PathConString.Ongoing,
                        PathId = ticketConcernExist.RequestConcernId.Value,

                    };

                    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);
                }

                foreach (var category in command.RequestorTicketCategories)
                {
                    var ticketCategoryExist = await unitOfWork.RequestTicket
                        .TicketCategoryExist(category.TicketCategoryId);

                    if (ticketCategoryExist is not null)
                    {
                        ticketCategoryList.Add(category.TicketCategoryId.Value);
                    }
                    else
                    {
                        var addTicketCategory = new TicketCategory
                        {
                            RequestConcernId = requestConcernId,
                            CategoryId = category.CategoryId.Value,

                        };

                        await unitOfWork.RequestTicket.CreateTicketCategory(addTicketCategory, cancellationToken);
                    }

                }

                foreach (var subCategory in command.RequestorTicketSubCategories)
                {
                    var ticketSubCategoryExist = await unitOfWork.RequestTicket
                        .TicketSubCategoryExist(subCategory.TicketSubCategoryId);

                    if (ticketSubCategoryExist is not null)
                    {
                        ticketSubCategoryList.Add(subCategory.TicketSubCategoryId.Value);
                    }
                    else
                    {
                        var addTicketSubCategory = new TicketSubCategory
                        {
                            RequestConcernId = requestConcernId,
                            SubCategoryId = subCategory.SubCategoryId.Value,

                        };

                        await unitOfWork.RequestTicket.CreateTicketSubCategory(addTicketSubCategory, cancellationToken);
                    }

                }

                if (ticketCategoryList.Any())
                    await unitOfWork.RequestTicket.RemoveTicketCategory(requestConcernId, ticketCategoryList, cancellationToken);

                if (ticketSubCategoryList.Any())
                    await unitOfWork.RequestTicket.RemoveTicketSubCategory(requestConcernId, ticketSubCategoryList, cancellationToken);

                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.ConcernAttachments.Count(x => x.Attachment != null) > 0)
                {
                    foreach (var attachments in command.ConcernAttachments.Where(a => a.Attachment.Length > 0))
                    {

                        if (attachments.Attachment.Length > 10 * 1024 * 1024)
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                        }

                        var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                        var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                        if (extension == null || !allowedFileTypes.Contains(extension))
                        {
                            return Result.Failure(TicketRequestError.InvalidAttachmentType());
                        }

                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(TicketingConString.AttachmentPath, fileName);

                        var ticketAttachment = await unitOfWork.RequestTicket
                            .TicketAttachmentExist(attachments.TicketAttachmentId);

                        if (ticketAttachment != null)
                        {
                            var updateTicketAttachment = new TicketAttachment
                            {
                                Attachment = filePath,
                                FileName = attachments.Attachment.FileName,
                                FileSize = attachments.Attachment.Length,
                                UpdatedAt = DateTime.Now,

                            };

                            await unitOfWork.RequestTicket.UpdateTicketAttachment(updateTicketAttachment, cancellationToken);
                        }
                        else
                        {
                            var addAttachment = new TicketAttachment
                            {
                                TicketConcernId = ticketConcernExist.Id,
                                Attachment = filePath,
                                FileName = attachments.Attachment.FileName,
                                FileSize = attachments.Attachment.Length,
                                AddedBy = command.Added_By,
                            };

                            await unitOfWork.RequestTicket.CreateTicketAttachment(addAttachment, cancellationToken);

                        }

                        await using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await attachments.Attachment.CopyToAsync(stream);
                        }
                    }

                }
                 

                //Approver

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
