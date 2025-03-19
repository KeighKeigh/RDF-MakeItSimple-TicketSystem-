using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating
{
    public partial class DownloadImageTicketing
    {

        public class Handler : IRequestHandler<DownloadImageTicketingCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly ContentType _contentType;
            private readonly IUnitOfWork unitOfWork;

            public Handler(MisDbContext context, ContentType contentType, IUnitOfWork unitOfWork)
            {
                _context = context;
                _contentType = contentType;
                this.unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(DownloadImageTicketingCommand request, CancellationToken cancellationToken)
            {

                var ticketAttachment = await _context.TicketAttachments
                    .FirstOrDefaultAsync(x => x.Id == request.TicketAttachmentId, cancellationToken);

                if (ticketAttachment == null)
                {
                    return Result.Failure(TicketRequestError.AttachmentNotFound());
                }

                var filePath = ticketAttachment.Attachment;
                var documentName = ticketAttachment.FileName;

                if (!File.Exists(filePath))
                {
                    return Result.Failure(TicketRequestError.FileNotFound());
                }

                var fileName = Path.GetFileName(filePath);
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
                var contentType = _contentType.GetContentType(fileName);

                var fileResult = new FileStreamResult(new FileStream(filePath, FileMode.Open, FileAccess.Read), contentType)
                {
                    FileDownloadName = documentName
                };

                return Result.Success(fileResult);
            }
        }
    }
}
