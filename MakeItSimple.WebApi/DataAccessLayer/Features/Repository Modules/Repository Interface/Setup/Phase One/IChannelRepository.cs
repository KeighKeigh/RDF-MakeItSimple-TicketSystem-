using MakeItSimple.WebApi.Models.Setup.ChannelSetup;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_One
{
    public interface IChannelRepository
    {
        Task<Channel> ChannelExist(int? id);
    }
}
