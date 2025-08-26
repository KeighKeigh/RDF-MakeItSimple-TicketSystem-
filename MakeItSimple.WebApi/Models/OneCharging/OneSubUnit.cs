namespace MakeItSimple.WebApi.Models.OneCharging
{
    public class OneSubUnit
    {
        public int Id { get; set; }
        public int? sync_id { get; set; }
        public int? sub_unit_id { get; set; }
        public string sub_unit_code { get; set; }
        public string sub_unit_name { get; set; }
        public DateTime? deleted_at { get; set; }
        public DateTime? DateAdded { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
