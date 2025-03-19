using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Get_Pms.GetPms;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Pms_Transaction
{
    public interface IPmsRepository
    {
        Task Create(Pms pms);
        Task CreateDetail(PmsDetail pmsDetail);
        Task CreateApproval(PmsApproval pmsApproval);
        Task CreateHistory(PmsHistory pmsHistory);
        Task CreateAttachment(PmsAttachment pmsAttachment);
        Task CreateTechnician(PmsTechnician pmsTechnician);

        Task<PmsAttachment> PmsAttachmentExist(int? id);

        Task Update(PmsDetail pmsDetail, Guid modified_By);
        Task UpdateApprovalHistory(PmsHistory pmsHistory);

        Task RejectPms(int id);
        Task DeletePmsDetail(int id);
        Task DeletePmsHistory(int id);
        Task DeletePmsApproval(int id);
        Task DeletePms(int id);
        Task DeletePmsAttachment(int id);
        Task RemoveAttachment(int id);

        Task<Pms> PmsIdNotExist(int? id);
        Task<PmsDetail> PmsDetailIdNotExist(int? id);
        Task<PmsTechnician> PmsTechnicianExist(int? id);
        Task<PmsHistory> PmsHistoryExist(int id);

        Task<IReadOnlyList<PmsTechnician>> PmsTechnicianListByPms(int id);
        Task<IReadOnlyList<PmsApproval>> MinimumLevelOfApproverList();
        Task<IReadOnlyList<PmsApproval>> PmsApprovalByPms(int id);
        Task<IReadOnlyList<PmsApproval>> PmsApprovalIsApprovedTrueByPms(int id);

        Task<FileStreamResult> FileResult(string filePath,string documentName);
        

        IQueryable<Pms> UserTypeRequestor(Guid? UserId);
        IQueryable<Pms> Search(string search);  
        IQueryable<Pms> RejectedFilter();
        IQueryable<Pms> ForApprovalFilter();
        IQueryable<Pms> ApprovedFilter();
        IQueryable<GetPmsResult> Orders (IQueryable<GetPmsResult> query , string order_By);
        
        Task RemovePmsTechnician(int id);
        Task ApprovedPmsApproval (int id);
        Task ApprovedPms(int id);   
    }
}
