namespace SmarterLead.API.AgencyLeadsManager.Entities
{
    public class ZohoLeadEntity
    {
        public int DB_Lead_ID { get; set; }
        public int USDOT { get; set; }
        public string Last_Name { get; set; }

        public string STATUS { get; set; }
        public string RECORD_ID { get; set; }
        public string ERRORS { get; set; }

    }
    public class ZohoLeadStatusData
    {
        public string ZohoLeadId { get; set; }
        public string CallStatus { get; set; }
        public string LeadStatus { get; set; }
    }
}
