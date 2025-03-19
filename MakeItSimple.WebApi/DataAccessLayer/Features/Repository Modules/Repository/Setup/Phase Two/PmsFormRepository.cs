using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository_Interface.IPms_Form;
using MakeItSimple.WebApi.Models.Setup.Phase_Two.Pms_Form_Setup;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Form_Setup.Update_Pms_Form.UpdatePmsForm;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form.CreatePmsForm;

namespace MakeItSimple.WebApi.DataAccessLayer.Repository_Modules.Repository.Pms_Form
{
    public class PmsFormRepository : IPmsFormRepository
    {
        private readonly MisDbContext context;

        public PmsFormRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task CreatePmsForm(CreatePmsFormCommand pmsForm)
        {
            var add = new PmsForm
            {
               Form_Name = pmsForm.Form_Name,
               AddedBy = pmsForm.Added_By,
            };

            await context.PmsForms.AddAsync(add);
        }

        public async Task<bool> FormNameAlreadyExist(string form,string currentForm)
        {
            if (string.IsNullOrEmpty(currentForm))
                return await context.PmsForms
                    .AnyAsync(x => x.Form_Name == form);

            return await context.PmsForms
                .Where(x => x.Form_Name == form
                && !form.Equals(currentForm))
                .AnyAsync();
        }

        public IQueryable<PmsForm> SearchPmsForm(string search)
        {
            return  context.PmsForms.Where(x => x.Form_Name.ToLower().Contains(search));
        }
        public  IQueryable<PmsForm> ArchivedPmsForm(bool? is_Archived)
        {
            return context.PmsForms.Where(q => q.IsActive == is_Archived);
        }
        public IQueryable<PmsForm> OrdersPmsForm(string order_By)
        {
            var query = context.PmsForms.AsQueryable();

            switch (order_By)
            {
                case PmsConsString.asc:
                    query = query.OrderBy(x => x.Id);
                    break;

                case PmsConsString.desc:
                    query = query.OrderByDescending(x => x.Id);
                    break;

                default:
                    query = query.OrderBy(x => x.Form_Name);
                    break;
            }

            return query;
        }

        public async Task<PmsForm> PmsFormIdNotExist(int? id)
        {
            return await context.PmsForms.FindAsync(id);
        }

        public async Task UpdatePmsForm(UpdatePmsFormCommand pmsForm)
        {
            await context.PmsForms
                 .Where(x => x.Id == pmsForm.Id)
                 .ExecuteUpdateAsync(update => update
                 .SetProperty(u => u.Form_Name, u => pmsForm.Form_Name)
                 .SetProperty(u => u.ModifiedBy, u => pmsForm.Modified_By)
                 .SetProperty(u => u.UpdatedAt, u => DateTime.Now));   
        }

        public async Task UpdateStatus(int id)
        {

            await context.PmsForms
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(update => update
                .SetProperty(u => u.IsActive, u => !u.IsActive));

        }
    }
}
