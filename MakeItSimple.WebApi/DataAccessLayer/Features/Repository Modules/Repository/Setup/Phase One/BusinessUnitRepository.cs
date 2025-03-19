using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_One
{
    public class BusinessUnitRepository : IBusinessUnitRepository
    {
        private readonly MisDbContext context;

        public BusinessUnitRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<BusinessUnit> BusinessUnitExist(int? id)
        {
            return await context.BusinessUnits.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
