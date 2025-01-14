using System.Text.Json.Serialization;

namespace SmarterLead.API.AgencyLeadsManager.Entities
{
    public class RecordData
    {
        [JsonPropertyName("data")]
        public List<AgencyDailyLeadEntity> Data { get; set; }
    }
    public class AgencyDailyLeadEntity
    {
        public Layout Layout { get; set; }
        public int DB_Lead_ID { get; set; }
        public int USDOT { get; set; }
        public string Last_Name { get; set; }
        public string State { get; set; }
        public string Years_in_Business { get; set; }
        public int Total_Vehicles { get; set; }
        public int MVR_Violations { get; set; }
        public string Radius_of_Operations { get; set; }
        public string Current_Insurance_Company { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Entity_Type { get; set; }
        public string Cargo_Carried_Name { get; set; }
        public string Operation_classification { get; set; }
        public int Coverage_Required { get; set; }
        public string Insurance_Lead_Type { get; set; }
        public string Type_of_Trucking { get; set; }
        public int Total_Drivers { get; set; }
        public int Power_Units { get; set; }

    }
    public class Layout
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
