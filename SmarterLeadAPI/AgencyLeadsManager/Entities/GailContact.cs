namespace SmarterLead.API.AgencyLeadsManager.Entities
{
    public class CampaignFlowData
    {
        public GailContact Contact { get; set; }
        public string GailUserId { get; set; }
        public string GailPwd { get; set; }
        public int ForAgency { get; set; }

    }
    public class GailContact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string LeadSource { get; set; }
    }
}
