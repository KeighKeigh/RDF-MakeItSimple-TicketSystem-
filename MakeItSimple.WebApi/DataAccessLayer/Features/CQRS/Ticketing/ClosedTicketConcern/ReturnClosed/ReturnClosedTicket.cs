using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ReturnClosed
{
    public partial class ReturnClosedTicket
    {

        public class Handler : IRequestHandler<ReturnClosedTicketCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IUnitOfWork unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(ReturnClosedTicketCommand command, CancellationToken cancellationToken)
            {

                var userDetails = await unitOfWork.User
                   .UserExist(command.Added_By);

                var requestConcernExist = await unitOfWork.RequestTicket
                    .RequestConcernExist(command.RequestConcernId);

                if (requestConcernExist is null)
                    return Result.Failure(TicketRequestError.RequestConcernIdNotExist());

                if (requestConcernExist.Is_Confirm is true)
                    return Result.Failure(TicketRequestError.TicketAlreadyApproved());

                await unitOfWork.ClosingTicket.ReturnClosingTicket(command.RequestConcernId, TicketingConString.OnGoing);

                await unitOfWork.RequestTicket.RemoveTicketHistory(requestConcernExist.TicketConcerns.First().Id);

                var addTicketHistory = new TicketHistory
                {
                    TicketConcernId = requestConcernExist.TicketConcerns.First().Id,
                    TransactedBy = command.Added_By,
                    TransactionDate = DateTime.Now,
                    Request = TicketingConString.Disapprove,
                    Status = TicketingConString.CloseReturn,
                    Remarks = command.Remarks,
                };

                await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                var addNewTicketTransactionNotification = new TicketTransactionNotification
                {

                    Message = $"Confirmation request for ticket number {requestConcernExist.TicketConcerns.First().Id} was rejected.",
                    AddedBy = command.Added_By.Value,
                    Created_At = DateTime.Now,
                    ReceiveBy = requestConcernExist.TicketConcerns.First().UserId.Value,
                    Modules = PathConString.IssueHandlerConcerns,
                    Modules_Parameter = PathConString.OpenTicket,
                    PathId = requestConcernExist.TicketConcerns.First().Id

                };

                await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification,cancellationToken);


                if (!Directory.Exists(TicketingConString.AttachmentPath))
                {
                    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                }

                if (command.ReturnTicketAttachments.Count(x => x.Attachment != null) > 0)
                {

                    foreach (var attachments in command.ReturnTicketAttachments.Where(a => a.Attachment.Length > 0))
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
                            ticketAttachment.Attachment = filePath;
                            ticketAttachment.FileName = attachments.Attachment.FileName;
                            ticketAttachment.FileSize = attachments.Attachment.Length;
                            ticketAttachment.UpdatedAt = DateTime.Now;

                        }
                        else
                        {
                            var addAttachment = new TicketAttachment
                            {
                                TicketConcernId = requestConcernExist.TicketConcerns.First().Id,
                                Attachment = filePath,
                                FileName = attachments.Attachment.FileName,
                                FileSize = attachments.Attachment.Length,
                                AddedBy = command.Added_By,
                            };

                            await unitOfWork.RequestTicket.CreateTicketAttachment(addAttachment,cancellationToken);

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




            private async Task<Result?> TicketAttachments(RequestConcern requestConcern, ReturnClosedTicketCommand command, CancellationToken cancellationToken)
            {


                return null;
            }



        }
    }
}
