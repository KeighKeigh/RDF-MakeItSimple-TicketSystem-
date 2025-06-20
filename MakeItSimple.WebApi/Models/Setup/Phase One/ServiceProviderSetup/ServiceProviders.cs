

using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Ticketing;

namespace MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup
{
    public class ServiceProviders : BaseEntity
    {
        public int Id { get; set; }
        public string ServiceProviderName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }
        public DateTime? ModifiedAt { get; set; }


        public ICollection<RequestConcern> RequestConcerns { get; set; }
        public ICollection<ServiceProviderChannel> ServiceProviderChannels { get; set; }

    }
}
