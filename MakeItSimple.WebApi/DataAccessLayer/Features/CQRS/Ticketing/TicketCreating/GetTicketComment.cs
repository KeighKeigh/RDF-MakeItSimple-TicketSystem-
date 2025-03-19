﻿using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.GetTicketComment.GetTicketCommentResult;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating
{
    public class GetTicketComment
    {
        public class GetTicketCommentResult
        {
            public int? TicketConcernId { get; set; }
            public List<GetComment> GetComments { get; set; }

            public class GetComment
            {
                public int? TicketCommentId { get; set; }
                public string Comment { get; set; }
                public string Added_By { get; set; }
                public DateTime Created_At { get; set; }
                public string Modified_By { get; set; }
                public DateTime? Updated_At { get; set; }

            }
        }

        public class GetTicketCommentQuery : IRequest<Result>
        {
            public int? TicketConcernId { get; set; }
        }


        public class Handler : IRequestHandler<GetTicketCommentQuery, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(GetTicketCommentQuery request, CancellationToken cancellationToken)
            {
                var result = await _context.TicketComments
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Where(x => x.IsActive == true && x.TicketConcernId == request.TicketConcernId)
                    .GroupBy(x => x.TicketConcernId)
                    .Select(x => new GetTicketCommentResult
                    {
                        TicketConcernId = x.Key,
                        GetComments = x.Select(x => new GetComment
                        {
                            TicketCommentId = x.Id,
                            Comment = !string.IsNullOrEmpty(x.Comment) ? x.Comment : x.Attachment,
                            Added_By = x.AddedByUser.Fullname,
                            Created_At = x.CreatedAt,
                            Modified_By = x.ModifiedByUser.Fullname,
                            Updated_At = x.UpdatedAt

                        }).OrderByDescending(x => x.Created_At)
                       .ToList()

                    }).ToListAsync();


                return Result.Success(result);
            }
        }
    }
}
