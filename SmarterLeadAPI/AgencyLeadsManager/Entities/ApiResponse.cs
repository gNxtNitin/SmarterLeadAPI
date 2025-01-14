using System.Text.Json.Serialization;

namespace SmarterLead.API.AgencyLeadsManager.Entities
{
    public class ApiResponse
    {
        [JsonPropertyName("data")]
        public List<Item> Data { get; set; }
    }
    public class Item
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("details")]
        public Details Details { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class Details
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("Modified_Time")]
        public DateTime ModifiedTime { get; set; }

        [JsonPropertyName("Modified_By")]
        public ZohoUser ModifiedBy { get; set; }

        [JsonPropertyName("Created_Time")]
        public DateTime CreatedTime { get; set; }

        [JsonPropertyName("Created_By")]
        public ZohoUser CreatedBy { get; set; }
    }

    public class ZohoUser
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
