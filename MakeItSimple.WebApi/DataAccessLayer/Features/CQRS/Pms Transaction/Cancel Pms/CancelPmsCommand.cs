using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Cancel_Pms
{
    public partial class CancelPms
    {
        public sealed class CancelPmsCommand : IRequest<Result>
        {
            public int Id { get; set; }
        }
    }
}
