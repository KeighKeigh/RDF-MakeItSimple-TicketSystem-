using MakeItSimple.WebApi.Models.Setup.ChannelSetup;

namespace MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup
{
    public class ServiceProviderChannel
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; }
        public int? ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
        public int? ServiceProviderId { get; set; }
        public virtual ServiceProviders ServiceProvider { get; set; }


    }
}
