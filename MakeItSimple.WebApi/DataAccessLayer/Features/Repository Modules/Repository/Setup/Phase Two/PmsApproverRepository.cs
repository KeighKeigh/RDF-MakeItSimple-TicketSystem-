using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Setup.FormSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_Two;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Get_Pms_Approver.GetPmsApprover;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_Two
{
    public class PmsApproverRepository : IPmsApproverRepository
    {
        private readonly MisDbContext context;

        public PmsApproverRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task Create(PmsApprover pmsApprover)
        {
           await context.PmsApprovers.AddAsync(pmsApprover);
        }

        public async Task Update(PmsApprover pmsApprover)
        {
            var update = await context.PmsApprovers
                .FirstOrDefaultAsync(x => x.Id == pmsApprover.Id);
        
            bool isChange = false;

            if (update.ApproverLevel != pmsApprover.ApproverLevel)
            {
                update.ApproverLevel = pmsApprover.ApproverLevel;
                isChange = true;
            }

            if(update.UserId != pmsApprover.UserId)
            {
                update.UserId = pmsApprover.UserId;
                isChange = true;
            }

            if (isChange)
            {
                update.ModifiedBy = pmsApprover.ModifiedBy;
                update.UpdatedAt = DateTime.Now;
            }

        }
        public async Task<PmsApprover> PmsApproverExist(int? id)
        {
           return await context.PmsApprovers.FindAsync(id);
        }

        public async Task<IReadOnlyList<PmsApprover>> PmsApproverByPForm(int id)
        {
            return await context.PmsApprovers
                .Where(x => x.PmsFormId == id)
                .ToListAsync();
        }

        public async Task Remove(int id)
        {

            await context.PmsApprovers
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<User> UserIdNotExist(Guid? id)
        {
            return await context.Users.FindAsync(id);
        }

        public IQueryable<PmsApprover> Search(string search)
        {
            return context.PmsApprovers.Where(x => x.PmsForms.Form_Name.ToLower().Contains(search));
        }

        public IQueryable<PmsApprover> Archived(bool? is_Archived)
        {
            return context.PmsApprovers.Where(q => q.IsActive == is_Archived);
        }

        public IQueryable<GetPmsApproverResult> Orders(IQueryable<GetPmsApproverResult> query,string order_By)
        {

            switch (order_By)
            {
                case PmsConsString.asc:
                    query = query.OrderBy(x => x.PmsFormId);
                    break;

                case PmsConsString.desc:
                    query = query.OrderByDescending(x => x.PmsFormId);
                    break;

                default:
                    query = query.OrderBy(x => x.Form_Name);
                    break;
            }

            return query;
        }

        public async Task<IReadOnlyList<PmsApprover>> PmsApproverList()
        {
            return await context.PmsApprovers
                .Where(x => x.IsActive)
                .ToListAsync();
        }


        public async Task UpdateStatus(int id)
        {
            await context.PmsApprovers
           .Where(x => x.PmsFormId == id)
           .ExecuteUpdateAsync(update => update
           .SetProperty(u => u.IsActive, u => !u.IsActive));
        }
    }
}
