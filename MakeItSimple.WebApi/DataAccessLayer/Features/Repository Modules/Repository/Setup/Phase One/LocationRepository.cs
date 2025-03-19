using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_One
{
    public class LocationRepository : ILocationRepository
    {
        private MisDbContext context;

        public LocationRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> LocationCodeExist(string locationCode)
        {
            return await context.Locations.AnyAsync(x => x.LocationCode == locationCode);
        }
    }
}
