namespace MakeItSimple.WebApi.Models.OneCharging
{
    public class OneCompany
    {
        public int Id { get; set; }
        public int? sync_id { get; set; }
        public string company_code { get; set; }
        public string company_name { get; set; }
        public int? company_id { get; set; }
        public DateTime? deleted_at { get; set; }
        public DateTime? DateAdded { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
