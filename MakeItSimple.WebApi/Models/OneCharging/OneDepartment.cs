namespace MakeItSimple.WebApi.Models.OneCharging
{
    public class OneDepartment
    {
        public int Id { get; set; }
        public int? sync_id { get; set; }
        public string department_code { get; set; }
        public string department_name { get; set; }
        public string department_id { get; set; }
        public DateTime? deleted_at { get; set; }
        public DateTime? DateAdded { get; set; }

        public bool? IsActive { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
