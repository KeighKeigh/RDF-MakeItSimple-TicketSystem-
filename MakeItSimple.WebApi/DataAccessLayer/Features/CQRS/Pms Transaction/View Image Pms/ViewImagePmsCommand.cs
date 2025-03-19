using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.View_Image_Pms
{
    public partial class ViewImagePms
    {
        public sealed class ViewImagePmsCommand : IRequest<Result>
        {
            public int? Id { get; set; } 
        }
    }
}
