namespace MakeItSimple.WebApi.Models.OneCharging
{
    public class OneUnit
    {
        public int Id { get; set; }
        public int? sync_id { get; set; }
        public int? unit_id { get; set; }
        public string unit_code { get; set; }
        public string unit_name { get; set; }
        public DateTime? deleted_at { get; set; }
        public DateTime? DateAdded { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
