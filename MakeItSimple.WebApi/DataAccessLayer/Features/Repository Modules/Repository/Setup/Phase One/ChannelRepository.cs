using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_One
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly MisDbContext context;

        public ChannelRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<Channel> ChannelExist(int? id)
        {
            return await context.Channels.FindAsync(id);
        }
    }
}
