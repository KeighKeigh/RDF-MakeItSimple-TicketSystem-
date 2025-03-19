using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Get_Pms_Approver.GetPmsApprover;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two
{
    public interface IPmsApproverRepository
    {
        Task Create(PmsApprover pmsApprover);
        Task Update(PmsApprover pmsApprover);   
        Task UpdateStatus(int id);
        Task Remove(int  id);

        IQueryable<PmsApprover> Search(string search);
        IQueryable<PmsApprover> Archived(bool? is_Archived);
        IQueryable<GetPmsApproverResult> Orders(IQueryable<GetPmsApproverResult> query,string order_By);

        Task<PmsApprover> PmsApproverExist(int? id);
        Task<IReadOnlyList<PmsApprover>> PmsApproverByPForm(int id);
        Task<User> UserIdNotExist(Guid? id);

        Task<IReadOnlyList<PmsApprover>> PmsApproverList();

    }
}
