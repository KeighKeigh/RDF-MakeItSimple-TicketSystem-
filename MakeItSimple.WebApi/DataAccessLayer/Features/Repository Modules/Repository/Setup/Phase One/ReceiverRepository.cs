using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One;
using MakeItSimple.WebApi.Models.Setup.ReceiverSetup;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_One
{
    public class ReceiverRepository : IReceiverRepository
    {
        private readonly MisDbContext context;

        public ReceiverRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<Receiver> ReceiverExistByBusinessUnitId(int? businessUnitId)
        {
            return await context.Receivers
                 .FirstOrDefaultAsync(x => x.BusinessUnitId == businessUnitId);
        }
    }
}
