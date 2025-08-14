using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.OpenTicketConcern.ViewOpenTicket.GetOpenTicket.GetOpenTicketResult.GetForClosingTicket;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest.AddRequestConcern;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddRequest
{
    public partial class UpdateRequestConcern
    {
        public class Handler : IRequestHandler<UpdateRequestConcernCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;
            private readonly ICacheService cacheService;
            //private readonly IHubCaller hubCaller;
            private readonly MisDbContext context;

            public Handler(IUnitOfWork unitOfWork, ICacheService cacheService, MisDbContext context)
            {
                this.unitOfWork = unitOfWork;
                this.cacheService = cacheService;
                //this.hubCaller = hubCaller;
                this.context = context;

            }


            public async Task<Result> Handle(UpdateRequestConcernCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Now;
                var ticketConcernId = new int(); //????kkkk
                var requestConcernId = new int();
                var ticketCategoryList = new List<int?>();
                var ticketSubCategoryList = new List<int?>();

                var userDetails = await unitOfWork.User
                    .UserExist(command.Added_By);

                var approvedDate = DateTime.Now.AddDays(2);

                var handlerDetails = await unitOfWork.User
                        .UserExist(command.AssignTo);

                //if (userIdExist is null)
                //    return Result.Failure(UserError.UserNotExist());


                foreach (var category in command.AddRequestTicketCategory)
                {
                    var ticketCategoryExist = await unitOfWork.Category
                      .CategoryExist(category.CategoryId);


                }

                foreach (var subCategory in command.AddRequestTicketSubCategory)
                {
                    var ticketSubCategoryExist = await unitOfWork.SubCategory
                        .SubCategoryExist(subCategory.SubCategoryId);

                }

                var requestConcernIdExist = await unitOfWork.RequestTicket
                    .RequestConcernExist(command.RequestConcernId);

                var ticketConcernIdExist = await unitOfWork.RequestTicket.TicketConcernExist(command.TicketConcernId);

                

                if (requestConcernIdExist is not null)
                {

                    

                    var ticketConcernExist = await unitOfWork.RequestTicket
                        .TicketConcernExistByRequestConcernId(command.RequestConcernId);

                    if (ticketConcernExist.IsApprove is true)
                        return Result.Failure(TicketRequestError.TicketAlreadyAssign());

                    var requestorDetails = await unitOfWork.User
                             .UserExist(command.UserId);
                    var updateRequestConcern = new RequestConcern
                    {
                        Id = requestConcernIdExist.Id,
                        Concern = command.Concern,
                        ChannelId = command.ChannelId == 0 ? null : command.ChannelId,
                        ContactNumber = command.Contact_Number,
                        RequestType = command.Request_Type,
                        DateNeeded = command.DateNeeded,
                        BackJobId = command.BackJobId,
                        ModifiedBy = command.Modified_By,
                        Severity = command.Severity,
                        TargetDate = command.TargetDate.ToString() == "" ? null : command.TargetDate,
                        AssignTo = command.AssignTo.ToString() == "" ? null : command.AssignTo,
                        ConcernStatus = TicketingConString.ForApprovalTicket

                    };

                    await unitOfWork.RequestTicket.UpdateRequestConcern(updateRequestConcern, cancellationToken);
                    if (command.AssignTo != null)
                    {

                        var userIdExist = await unitOfWork.User
                             .UserExist(command.AssignTo);
                        var updateRequest = new RequestConcern
                        {
                            Id = requestConcernIdExist.Id,
                            UnitId = userIdExist.UnitId,
                            DepartmentId = userIdExist.DepartmentId,
                            SubUnitId = userIdExist.SubUnitId,
                            ConcernStatus = command.TargetDate.Value.Date <= approvedDate.Date ? TicketingConString.OnGoing : TicketingConString.ForApprovalTicket,
                        };
                        await unitOfWork.RequestTicket.UpdateRequestConcern(updateRequest, cancellationToken);

                        var handlerIds = await context.Approvers.Where(x => x.SubUnitId == updateRequest.SubUnitId).FirstOrDefaultAsync();
                        var updateTicketConcern = new TicketConcern
                        {
                            Id = ticketConcernIdExist.Id,
                            TargetDate = command.TargetDate,
                            UserId = command.AssignTo,
                            IsApprove = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                            IsAssigned = true,
                            ApprovedBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerIds.UserId : null,
                            ApprovedAt = command.TargetDate.Value.Date <= approvedDate.Date ? dateToday : null,
                            ConcernStatus = command.TargetDate.Value.Date <= approvedDate.Date ? TicketingConString.OnGoing : TicketingConString.ForApprovalTicket,
                            AssignTo = command.AssignTo,
                            IsDateApproved = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                            DateApprovedAt = command.TargetDate.Value.Date <= approvedDate.Date ? dateToday : null,
                            ApprovedDateBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerIds.UserId : null,
                        };

                        await unitOfWork.RequestTicket.UpdateTicketConcerns(updateTicketConcern, cancellationToken);
                    }
                    ticketConcernId = ticketConcernExist.Id; //kk
                    requestConcernId = requestConcernIdExist.Id;
                }

                if (command.AssignTo != null)
                {


                    var ticketConcernExists = await unitOfWork.RequestTicket
                               .TicketConcernExist(command.TicketConcernId);

                    var approverList = await unitOfWork.RequestTicket
                    .ApproverBySubUnitList(ticketConcernExists.User.SubUnitId);

                    if (!approverList.Any())
                        return Result.Failure(ClosingTicketError.NoApproverHasSetup());

                    if (!approverList.Any())
                        return Result.Failure(ClosingTicketError.NoApproverHasSetup());

                    var approverUser = approverList
                .First(x => x.ApproverLevel == approverList.Min(x => x.ApproverLevel));

                    var handlerId = await context.Approvers.Where(x => x.SubUnitId == requestConcernIdExist.SubUnitId).FirstOrDefaultAsync();
                    var handlerName = await context.Users.Where(x => x.Id == handlerId.UserId).Select(x => x.Fullname).FirstOrDefaultAsync();
                    var addNewDateApproveConcern = new ApproverDate
                    {
                        TicketConcernId = ticketConcernExists.Id,
                        IsApproved = command.TargetDate.Value.Date <= approvedDate.Date ? true : false,
                        TicketApprover = approverUser.UserId,
                        AddedBy = command.Added_By,
                        Notes = command.Notes,
                        ApprovedDateBy = command.TargetDate.Value.Date <= approvedDate.Date ? handlerId.UserId : null,
                    };

                    await unitOfWork.RequestTicket.ApproveDateTicket(addNewDateApproveConcern, cancellationToken);

                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    
                        foreach (var approver in approverList)
                        {
                            var addNewApprover = new ApproverTicketing
                            {
                                TicketConcernId = ticketConcernExists.Id,
                                ApproverDateId = addNewDateApproveConcern.Id,
                                UserId = approver.UserId,
                                ApproverLevel = approver.ApproverLevel,
                                AddedBy = command.Added_By,
                                CreatedAt = DateTime.Now,
                                Status = TicketingConString.ApprovalDate,
                                IsApprove = command.TargetDate.Value.Date <= approvedDate.Date ? true : null,
                            };

                            await unitOfWork.RequestTicket.CreateApproval(addNewApprover, cancellationToken);

                        }
                    

                    var assignedTicketHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcernExists.Id,
                        TransactedBy = command.Added_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.ConcernAssign,
                        Status = $"{TicketingConString.RequestAssign} {handlerDetails.Fullname}"
                    };

                    await unitOfWork.RequestTicket.CreateTicketHistory(assignedTicketHistory, cancellationToken);



                    //kk
                    if (command.TargetDate.Value.Date > approvedDate.Date)
                    {

                        var addNewTicketTransactionNotification = new TicketTransactionNotification
                        {

                            Message = $"Ticket number {ticketConcernExists.Id} has been assigned",
                            AddedBy = command.Added_By.Value,
                            Created_At = DateTime.Now,
                            ReceiveBy = command.UserId.Value,
                            Modules = PathConString.IssueHandlerConcerns,
                            Modules_Parameter = PathConString.ForApproval,
                            PathId = ticketConcernExists.Id,

                        };

                        await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);



                        var approveTicketDateHistory = new TicketHistory
                        {
                            TicketConcernId = ticketConcernExists.Id,
                            TransactedBy = handlerId.UserId,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.ForApprovalTicket,
                            Status = $"{TicketingConString.ForApprovalDate}"
                        };

                        await unitOfWork.RequestTicket.CreateTicketHistory(approveTicketDateHistory, cancellationToken);

                        var addNewTicketTransactionOngoing = new TicketTransactionNotification
                        {

                            Message = $"Ticket number {ticketConcernExists.RequestConcernId} Target Date is being approved",
                            AddedBy = command.Added_By.Value,
                            Created_At = DateTime.Now,
                            ReceiveBy = command.UserId.Value,
                            Modules = PathConString.ConcernTickets,
                            Modules_Parameter = PathConString.ForApproval,
                            PathId = ticketConcernExists.RequestConcernId.Value,


                        };

                        await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                    }
                    else
                    {

                        var addNewTicketTransactionNotifications = new TicketTransactionNotification
                        {

                            Message = $"Ticket number {ticketConcernExists.Id}, Target Date is approved",
                            AddedBy = handlerId.UserId,
                            Created_At = DateTime.Now,
                            ReceiveBy = requestConcernIdExist.UserId.Value,
                            Modules = PathConString.ConcernTickets,
                            Modules_Parameter = PathConString.Ongoing,
                            PathId = ticketConcernExists.Id

                        };

                        await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotifications, cancellationToken);

                        var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                        {

                            Message = $"Ticket number {ticketConcernExists.Id}, Target Date is approved",
                            AddedBy = handlerId.UserId,
                            Created_At = DateTime.Now,
                            ReceiveBy = ticketConcernExists.UserId.Value,
                            Modules = PathConString.IssueHandlerConcerns,
                            Modules_Parameter = PathConString.Ongoing,
                            PathId = ticketConcernExists.Id

                        };

                        await unitOfWork.RequestTicket.CreateTicketNotification(addNewTransactionConfirmationNotification, cancellationToken);

                        var addNewTicketTransactionOngoing = new TicketTransactionNotification
                        {

                            Message = $"Ticket number {ticketConcernExists.Id} is now ongoing",
                            AddedBy = handlerId.UserId,
                            Created_At = DateTime.Now,
                            ReceiveBy = requestConcernIdExist.UserId.Value,
                            Modules = PathConString.ConcernTickets,
                            Modules_Parameter = PathConString.Ongoing,
                            PathId = ticketConcernExists.Id,


                        };

                        await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                    }
                }

                //foreach (var category in command.AddRequestTicketCategory)
                //{
                //    var ticketCategoryExist = await unitOfWork.RequestTicket
                //        .TicketCategoryExist(category.CategoryId);

                //    var listofCategory = await context.TicketCategories.Where(x => x.RequestConcernId == ticketCategoryExist.RequestConcernId).Select(x => x.CategoryId).ToListAsync();
                //    if (listofCategory != null)
                //    {
                //        foreach (var list in listofCategory)
                //        {
                //            ticketCategoryList.Add(list);
                //        }
                //    }

                //    if (ticketCategoryExist is not null)
                //    {


                //         var addTicketCategory = new TicketCategory
                //         {
                //                Id = ticketCategoryExist.Id,
                //                RequestConcernId = requestConcernId,
                //                CategoryId = category.CategoryId.ToString() == "" ? null : category.CategoryId,
                //         };

                //         await unitOfWork.RequestTicket.CreateTicketCategory(addTicketCategory, cancellationToken);

                //    }


                //}

                //foreach (var subCategory in command.AddRequestTicketSubCategory)
                //{
                //    var ticketSubCategoryExist = await unitOfWork.RequestTicket
                //        .TicketSubCategoryExist(subCategory.SubCategoryId);

                //var listofSubCategory = await context.TicketSubCategories.Where(x => x.RequestConcernId == ticketSubCategoryExist.RequestConcernId).Select(x => x.SubCategoryId).ToListAsync();

                //if (listofSubCategory != null)
                //{
                //    foreach (var list in listofSubCategory)
                //    {
                //        ticketSubCategoryList.Add(list);
                //    }
                //}

                //    if (ticketSubCategoryExist is not null)
                //    {

                //        var addTicketSubCategory = new TicketSubCategory
                //        {
                //            RequestConcernId = requestConcernId,
                //            SubCategoryId = subCategory.SubCategoryId.ToString() == "" ? null : subCategory.SubCategoryId,

                //        };

                //        await unitOfWork.RequestTicket.CreateTicketSubCategory(addTicketSubCategory, cancellationToken);
                //    }


                //}

                // Extract IDs from command for validation


                

                    // Process categories - add new ones
                    foreach (var category in command.AddRequestTicketCategory)
                    {
                        var ticketCategoryExist = await unitOfWork.RequestTicket
                            .TicketCategoryExist(category.CategoryId, command.RequestConcernId);
                    if (category.CategoryId != null)
                    {
                        if (ticketCategoryExist is null)
                        {
                            ticketCategoryList.Add(category.CategoryId.Value);
                            // Add new category
                            var addTicketCategory = new TicketCategory
                            {
                                RequestConcernId = requestConcernId,
                                CategoryId = category.CategoryId,
                                
                            };
                            await unitOfWork.RequestTicket.CreateTicketCategory(addTicketCategory, cancellationToken);
                        }
                        else
                        {
                            ticketCategoryList.Add(category.CategoryId.Value);
                        }
                    }
                    
                    }

                    // Process subcategories - add new ones
                    foreach (var subCategory in command.AddRequestTicketSubCategory)
                    {
                        var ticketSubCategoryExist = await unitOfWork.RequestTicket
                            .TicketSubCategoryExist(subCategory.SubCategoryId, command.RequestConcernId);
                    if (subCategory.SubCategoryId != null)
                    {
                        if (ticketSubCategoryExist is null)
                        {
                            ticketSubCategoryList.Add(subCategory.SubCategoryId.Value);
                            // Add new subcategory
                            var addTicketSubCategory = new TicketSubCategory
                            {
                                RequestConcernId = requestConcernId,
                                SubCategoryId = subCategory.SubCategoryId,
                            };
                            await unitOfWork.RequestTicket.CreateTicketSubCategory(addTicketSubCategory, cancellationToken);
                        }
                        else
                        {
                            ticketSubCategoryList.Add(subCategory.SubCategoryId.Value);
                        }
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

                if (command.RequestAttachmentsFile.Count(x => x.Attachment != null) > 0)
                {
                    foreach (var attachments in command.RequestAttachmentsFile.Where(a => a.Attachment.Length > 0))
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
                                TicketConcernId = ticketConcernId, //kk
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
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();

            }
            
        }

    }
}
