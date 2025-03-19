using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Get_Pms.GetPms;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Pms_Transaction
{
    public class PmsRepository : IPmsRepository
    {
        private readonly MisDbContext context;
        private readonly ContentType contentType;

        public PmsRepository(MisDbContext context, ContentType contentType)
        {
            this.context = context;
            this.contentType = contentType;
        }

        public IQueryable<Pms> ApprovedFilter()
        {
            return context.Pms.Where(x => x.IsApproved == true);
        }

        public async Task ApprovedPms(int id)
        {
            var approved = await context.Pms
                .FirstOrDefaultAsync(x => x.Id == id);

            approved.IsApproved = true;
        }

        public async Task ApprovedPmsApproval(int id)
        {
            var approved = await context.PmsApprovals
                .FirstOrDefaultAsync(x => x.Id == id);

            approved.IsApproved = true;
        }

        public async Task Create(Pms pms)
        {
           await context.Pms.AddAsync(pms);
        }

        public async Task CreateApproval(PmsApproval pmsApproval)
        {
           await context.PmsApprovals.AddAsync(pmsApproval);
        }

        public async Task CreateAttachment(PmsAttachment pmsAttachment)
        {
           await context.PmsAttachments.AddAsync(pmsAttachment);
        }
        public async Task CreateDetail(PmsDetail pmsDetail)
        {
           await context.PmsDetails.AddAsync(pmsDetail);
        }


        public async Task CreateHistory(PmsHistory pmsHistory)
        {
            await context.PmsHistories.AddAsync(pmsHistory);
        }

        public async Task CreateTechnician(PmsTechnician pmsTechnician)
        {
          await context.PmsTechnicians.AddAsync(pmsTechnician);
        }

        public async Task DeletePms(int id)
        {
            await context.Pms.Where(x => x.Id == id)
                .ExecuteUpdateAsync(delete => delete
                .SetProperty(d => d.IsDeleted, d => true));
       
        }

        public async Task DeletePmsApproval(int id)
        {
            await context.PmsApprovals
                .Where(x => x.PmsId == id)
                .ExecuteUpdateAsync(x => x.SetProperty(d => d.IsDeleted, d => true));
        }

        public async Task DeletePmsAttachment(int id)
        {
            await context.PmsAttachments
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(delete => delete
                .SetProperty(d => d.IsDeleted, d => true));
        }

        public async Task DeletePmsDetail(int id)
        {
            await context.PmsDetails
                .Where(x => x.PmsId == id)
                .ExecuteUpdateAsync(x => x.SetProperty(d => d.IsDeleted, d => true ));
        }

        public async Task DeletePmsHistory(int id)
        {
            await context.PmsHistories.Where(x => x.PmsId == id 
                 && x.Request.ToLower().Contains(PmsConsString.ForApproval.ToLower()))
                .ExecuteUpdateAsync(x => x.SetProperty(d => d.IsDeleted,
                d => true));
        }

        public async Task<FileStreamResult> FileResult(string filePath, string documentName)
        {
            FileStreamResult fileResult = null;

            var fileName = Path.GetFileName(filePath);
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            var type = contentType.GetContentType(fileName);

            if(!string.IsNullOrEmpty(documentName))
            {
                fileResult = new FileStreamResult(new FileStream(filePath, FileMode.Open, FileAccess.Read), type)
                {
                    FileDownloadName = documentName
                };
            }
            else
            {
                fileResult = new FileStreamResult(new FileStream(filePath, FileMode.Open, FileAccess.Read), type);
            }

            return fileResult;

        }

        public IQueryable<Pms> ForApprovalFilter()
        {
            return context.Pms.Where(x => x.IsApproved == false);
        }

        public async Task<IReadOnlyList<PmsApproval>> MinimumLevelOfApproverList()
        {

            return await context.PmsApprovals
                .Where(x => x.IsApproved == null) 
                .GroupBy(x => x.PmsId) 
                .Select(group => group.OrderBy(x => x.ApproverLevel).First())
                .ToListAsync();
        }

        public IQueryable<GetPmsResult> Orders(IQueryable<GetPmsResult> query, string order_By)
        {

            switch (order_By)
            {
                case PmsConsString.asc:
                    query = query.OrderBy(x => x.Id);
                    break;

                case PmsConsString.desc:
                    query = query.OrderByDescending(x => x.Id);
                    break;

                default:
                    query = query.OrderBy(x => x.PmsFormId);
                    break;
            }

            return query;
        }

        public async Task<IReadOnlyList<PmsApproval>>PmsApprovalByPms(int id)
        {
            return await context.PmsApprovals
                .Where(x => x.PmsId == id && x.IsApproved == null)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<PmsApproval>> PmsApprovalIsApprovedTrueByPms(int id)
        {
            return await context.PmsApprovals
                .Where(x => x.PmsId == id && x.IsApproved == true)
                .ToListAsync();
        }

        public async Task<PmsAttachment> PmsAttachmentExist(int? id)
        {
            return await context.PmsAttachments.FindAsync(id);
        }

        public async Task<PmsDetail> PmsDetailIdNotExist(int? id)
        {
            return await context.PmsDetails.FindAsync(id);
        }

        public async Task<PmsHistory> PmsHistoryExist(int id)
        {
            return await context.PmsHistories.FindAsync(id);
        }

        public async Task<Pms> PmsIdNotExist(int? id)
        {
            return await context.Pms.FindAsync(id);
        }

        public async Task<PmsTechnician> PmsTechnicianExist(int? id)
        {
            return await context.PmsTechnicians.FindAsync(id);
        }

        public async Task<IReadOnlyList<PmsTechnician>> PmsTechnicianListByPms(int id)
        {
            return await context.PmsTechnicians
                .Where(x => x.Id == id)
                .ToListAsync();
        }

        public IQueryable<Pms> RejectedFilter()
        {
            return context.Pms.Where(x => x.IsRejected == true);
        }

        public async Task RejectPms(int id)
        {
            await context.Pms
           .Where(x => x.Id == id)
           .ExecuteUpdateAsync(x => x.SetProperty(d => d.IsRejected, d => true));
        }

        public async Task RemoveAttachment(int id)
        {
            await context.PmsAttachments
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task RemovePmsTechnician(int id)
        {
            var remove = await context.PmsTechnicians
                .FirstOrDefaultAsync(x => x.Id == id);

            context.PmsTechnicians.Remove(remove); 
        }

        public IQueryable<Pms> Search(string search)
        {
            return context.Pms.Where(x => x.Id.ToString().Contains(search));
        }

        public async Task Update(PmsDetail pmsDetail, Guid modified_By)
        {
            var update = await context.PmsDetails
                .FirstOrDefaultAsync(x => x.Id == pmsDetail.Id);

            bool is_Change = false;

            if(update.Answer != pmsDetail.Answer)
            {
                update.Answer = pmsDetail.Answer;
                is_Change = true;
            }

            if(is_Change)
            {
                update.Pms.ModifiedBy = modified_By;
                update.Pms.UpdatedAt = DateTime.Now;
            }

        }

        public async Task UpdateApprovalHistory(PmsHistory pmsHistory)
        {

            await context.PmsHistories
                .Where(x => x.Approver_Level == pmsHistory.Approver_Level && x.PmsId == pmsHistory.PmsId)
                .ExecuteUpdateAsync(x => x
                .SetProperty(u => u.Status, u => pmsHistory.Status)
                .SetProperty(u => u.Request, u => pmsHistory.Request)
                .SetProperty(u => u.TransactedBy, u => pmsHistory.TransactedBy)
                .SetProperty(u => u.TransactionDate, u => DateTime.Now));

        }

        public IQueryable<Pms> UserTypeRequestor( Guid? UserId)
        {
            return context.Pms.Where(x => x.AddedBy == UserId);
        }

    }
}
