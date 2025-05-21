using CloudinaryDotNet;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Hubs;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest.AddRequestConcern.AddRequestConcernCommand;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest
{
    public partial class AddRequestConcern
    {

        public class Handler : IRequestHandler<AddRequestConcernCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;
            private readonly ICacheService cacheService;
            private readonly IHubCaller hubCaller;
            private readonly MisDbContext context;

            public Handler(IUnitOfWork unitOfWork, ICacheService cacheService, IHubCaller hubCaller, MisDbContext context)
            {
                this.unitOfWork = unitOfWork;
                this.cacheService = cacheService;
                this.hubCaller = hubCaller;
                this.context = context;

            }

            public async Task<Result> Handle(AddRequestConcernCommand command, CancellationToken cancellationToken)
            {

                var ticketConcernId = new int(); //????kkkk
                var requestConcernId = new int();
                var ticketCategoryList = new List<int>();
                var ticketSubCategoryList = new List<int>();

                var userDetails = await unitOfWork.User
                    .UserExist(command.Added_By);

                var userIdExist = await unitOfWork.User
                    .UserExist(command.UserId);

                if (userIdExist is null)
                    return Result.Failure(UserError.UserNotExist());

                var channelExist = await unitOfWork.Channel
                  .ChannelExist(command.ChannelId);

                if (channelExist is null)
                    return Result.Failure(TicketRequestError.ChannelNotExist());

                foreach (var category in command.AddRequestTicketCategories)
                {
                    var ticketCategoryExist = await unitOfWork.Category
                      .CategoryExist(category.CategoryId);

                    if (ticketCategoryExist is null)
                        return Result.Failure(TicketRequestError.CategoryNotExist());
                }

                foreach (var subCategory in command.AddRequestTicketSubCategories)
                {
                    var ticketSubCategoryExist = await unitOfWork.SubCategory
                        .SubCategoryExist(subCategory.SubCategoryId);

                    if (ticketSubCategoryExist is null)
                        return Result.Failure(TicketRequestError.SubCategoryNotExist());
                }

                var requestConcernIdExist = await unitOfWork.RequestTicket
                    .RequestConcernExist(command.RequestConcernId);


                if (requestConcernIdExist is not null)
                {

                    var ticketConcernExist = await unitOfWork.RequestTicket
                        .TicketConcernExistByRequestConcernId(command.RequestConcernId);

                    if (ticketConcernExist.IsApprove is true)
                        return Result.Failure(TicketRequestError.TicketAlreadyAssign());

                    var updateRequestConcern = new RequestConcern
                    {
                        Id = requestConcernIdExist.Id,
                        Concern = command.Concern,
                        ChannelId = command.ChannelId,
                        ContactNumber = command.Contact_Number,
                        RequestType = command.Request_Type,
                        DateNeeded = command.DateNeeded,
                        BackJobId = command.BackJobId,
                        ModifiedBy = command.Modified_By,
                        Severity = command.Severity,
                        
                    };
                    
                    await unitOfWork.RequestTicket.UpdateRequestConcern(updateRequestConcern, cancellationToken);

                    ticketConcernId = ticketConcernExist.Id; //kk
                    requestConcernId = requestConcernIdExist.Id;
                }
                else
                {
                    var addRequestConcern = new RequestConcern
                    {
                        UserId = command.UserId,
                        Concern = command.Concern,
                        AddedBy = command.Added_By,
                        ConcernStatus = TicketingConString.ForApprovalTicket,
                        CompanyId = userIdExist.CompanyId,
                        BusinessUnitId = userIdExist.BusinessUnitId,
                        DepartmentId = userIdExist.DepartmentId,
                        UnitId = userIdExist.UnitId,
                        SubUnitId = userIdExist.SubUnitId,
                        LocationId = userIdExist.LocationId,
                        DateNeeded = command.DateNeeded,
                        ChannelId = command.ChannelId,
                        Notes = command.Notes,
                        IsDone = false,
                        ContactNumber = command.Contact_Number,
                        RequestType = command.Request_Type,
                        BackJobId = command.BackJobId,
                        Severity = command.Severity,
                        
                        //TicketCategories = 

                    };

                    await unitOfWork.RequestTicket.CreateRequestConcern(addRequestConcern,cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    requestConcernId = addRequestConcern.Id;

                    //kk

                    var addTicketConcern = new TicketConcern
                    {
                        RequestConcernId = requestConcernId,
                        RequestorBy = command.UserId,
                        IsApprove = false,
                        AddedBy = command.Added_By,
                        ConcernStatus = TicketingConString.PendingTicket,
                        IsAssigned = false,
                        
                        

                    };

                    await unitOfWork.RequestTicket.CreateTicketConcern(addTicketConcern, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    var cache = await cacheService.GetOpenTickets();

                    var chechcache = cache.Where(x => x.Id == ticketConcernId);

                    if (!chechcache.Any())
                    {
                        cache.Add(addTicketConcern);
                    }



                    await hubCaller.SendToChannelAsync(command.ChannelId.Value, "NewTicketSubmitted", addTicketConcern);
                    await hubCaller.SendNotificationAsync(command.UserId.Value, "NewPendingTicket", addTicketConcern);

                    await hubCaller.SendNotificationAsync(command.UserId.Value, "NewPendingTicket", new
                    {
                        TicketId = ticketConcernId,
                        RequestConcernId = requestConcernId,
                        Message = "A new Ticket has been submitted.",
                        ChannelId = command.ChannelId
                    });

                    ticketConcernId = addTicketConcern.Id;

                    var addTicketHistory = new TicketHistory
                    {
                        TicketConcernId = ticketConcernId,
                        TransactedBy = command.Added_By,
                        TransactionDate = DateTime.Now,
                        Request = TicketingConString.Request,
                        Status = $"{TicketingConString.RequestCreated} {userIdExist.Fullname}"
                    };

                    await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                    //kk

                    var userReceiverByBusinessUnit = await unitOfWork.Receiver
                         .ReceiverExistByBusinessUnitId(userIdExist.BusinessUnitId);

                    var addNewTicketTransactionNotification = new TicketTransactionNotification
                    {

                        Message = $"New request concern number {requestConcernId} has received",
                        AddedBy = command.Added_By.Value,
                        Created_At = DateTime.Now,
                        ReceiveBy = userReceiverByBusinessUnit.UserId.Value,
                        Modules = PathConString.ReceiverConcerns,
                        PathId = requestConcernId

                    };

                    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                }


                foreach (var category in command.AddRequestTicketCategories)
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

                foreach (var subCategory in command.AddRequestTicketSubCategories)
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

                if (command.RequestAttachmentsFiles.Count(x => x.Attachment != null) > 0)
                {
                    foreach (var attachments in command.RequestAttachmentsFiles.Where(a => a.Attachment.Length > 0))
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

                            await unitOfWork.RequestTicket.UpdateTicketAttachment(updateTicketAttachment,cancellationToken);
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
                //await cacheService.UpdateOpenTicketCacheAsync();

                return Result.Success();

            }

        }
    }
}
