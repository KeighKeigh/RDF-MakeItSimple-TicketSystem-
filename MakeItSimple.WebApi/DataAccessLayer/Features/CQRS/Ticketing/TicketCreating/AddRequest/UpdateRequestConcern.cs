using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Caching;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using System;
using System.Threading;
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
                if (command.AssignTo == null)
                {
                    
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

                    foreach (var category in command.AddRequestTicketCategory)
                    {
                        var ticketCategoryExist = await unitOfWork.Category
                          .CategoryExist(category.CategoryId);

                        if (ticketCategoryExist is null)
                            return Result.Failure(TicketRequestError.CategoryNotExist());
                    }

                    foreach (var subCategory in command.AddRequestTicketSubCategory)
                    {
                        var ticketSubCategoryExist = await unitOfWork.SubCategory
                            .SubCategoryExist(subCategory.SubCategoryId);

                        if (ticketSubCategoryExist is null)
                            return Result.Failure(TicketRequestError.SubCategoryNotExist());
                    }


                    
                }

                else if(command.AssignTo != null)
                {
                    var dateToday = DateTime.Today;

                    var requestConcernId = new int();
                    var ticketCategoryList = new List<int>();
                    var ticketSubCategoryList = new List<int>();

                    var userDetails = await unitOfWork.User
                        .UserExist(command.Modified_By);

                    var handlerDetails = await unitOfWork.User
                        .UserExist(command.UserId);

                    var receiverPermissionList = await unitOfWork.UserRole
                        .UserRoleByPermission(TicketingConString.Requestor);

                    var issueHandlerPermissionList = await unitOfWork.UserRole
                        .UserRoleByPermission(TicketingConString.Requestor);

                    var requestorPermissionList = await unitOfWork.UserRole
                        .UserRoleByPermission(TicketingConString.Requestor);

                    switch (await unitOfWork.User.UserExist(command.UserId))
                    {
                        case null:
                            return Result.Failure<int?>(TicketRequestError.UserNotExist());
                    }

                    switch (await unitOfWork.Channel.ChannelExist(command.ChannelId))
                    {
                        case null:
                            return Result.Failure<int?>(TicketRequestError.ChannelNotExist());
                    }

                    foreach (var category in command.AddRequestTicketCategory)
                    {
                        var ticketCategoryExist = await unitOfWork.Category
                          .CategoryExist(category.CategoryId);

                        if (ticketCategoryExist is null)
                            return Result.Failure<int?>(TicketRequestError.CategoryNotExist());
                    }

                    foreach (var subCategory in command.AddRequestTicketSubCategory)
                    {
                        var ticketSubCategoryExist = await unitOfWork.SubCategory
                            .SubCategoryExist(subCategory.SubCategoryId);

                        if (ticketSubCategoryExist is null)
                            return Result.Failure<int?>(TicketRequestError.SubCategoryNotExist());

                    }

                    if (dateToday > command.TargetDate)
                        return Result.Failure<int?>(TicketRequestError.DateTimeInvalid());

                    if (command.AssignTo == null)
                    {
                        var ticketConcernId = new int();
                        
                        
                        
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
                                return Result.Failure(TicketRequestError.RequestConcernIdNotExist());
                            }


                            if (command.AddRequestTicketCategory != null)
                            {
                                foreach (var category in command.AddRequestTicketCategory)
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

                                foreach (var subCategory in command.AddRequestTicketSubCategory)
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
                        
                    }


                    if (command.AssignTo != null)
                    {

                        
                        
                            var ticketConcernExist = await unitOfWork.RequestTicket
                                   .TicketConcernExist(command.TicketConcernId);

                            if (ticketConcernExist is not null)
                            {
                                if (ticketConcernExist.IsActive is false)
                                    return Result.Failure<int?>(TicketRequestError.TicketAlreadyCancel());

                                var assignTicket = new TicketConcern
                                {
                                    Id = ticketConcernExist.Id,
                                    UserId = command.UserId,
                                    TargetDate = command.TargetDate,
                                    ModifiedBy = command.Modified_By,
                                    ConcernStatus = TicketingConString.CurrentlyFixing,


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
                                        Severity = command.Severity,
                                        ServiceProviderId = command.ServiceProviderId,
                                        DateNeeded = command.DateNeeded,
                                        TargetDate = command.TargetDate,
                                        ConcernStatus = TicketingConString.CurrentlyFixing,

                                    };

                                    await unitOfWork.RequestTicket.UpdateRequestConcern(updateRequestConcern, cancellationToken);
                                }

                                requestConcernId = ticketConcernExist.Id;

                            }


                            foreach (var category in command.AddRequestTicketCategory)
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

                            foreach (var subCategory in command.AddRequestTicketSubCategory)
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

                            if (command.RequestAttachmentsFile.Count(x => x.Attachment != null) > 0)
                            {
                                foreach (var attachments in command.RequestAttachmentsFile.Where(a => a.Attachment.Length > 0))
                                {

                                    if (attachments.Attachment.Length > 10 * 1024 * 1024)
                                    {
                                        return Result.Failure<int?>(TicketRequestError.InvalidAttachmentSize());
                                    }

                                    var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                                    var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                                    if (extension == null || !allowedFileTypes.Contains(extension))
                                    {
                                        return Result.Failure<int?>(TicketRequestError.InvalidAttachmentType());
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
                        
                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            
        }

    }
}
