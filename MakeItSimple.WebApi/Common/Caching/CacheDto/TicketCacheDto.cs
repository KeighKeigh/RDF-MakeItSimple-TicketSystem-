namespace MakeItSimple.WebApi.Common.Caching.CacheDto
{
    public class TicketCacheDto
    {
        public int TicketNumber { get; set; }
        public string TicketDescription { get; set; }
        public DateTime? TargetDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public Guid? UserId { get; set; }


    }
}
