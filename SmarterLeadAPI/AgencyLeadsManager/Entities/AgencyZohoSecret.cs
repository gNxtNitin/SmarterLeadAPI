namespace SmarterLead.API.AgencyLeadsManager.Entities
{
    public class AgencyZohoSecret
    {
        public string ZohoCID { get; }
        public string ZohoKey { get; }
        public string ZohoRtk { get; }
        public string ZohoUId { get; set; }
        public int ClientId { get; set; }
        public string ZohoOrgId { get; }
        public string ZohoAccountURL { get; }
        public string ZohoAPIDomain { get; }
        public string APIVersion { get; }
        public string LeadsLayoutId { get; }
        public string GAILUserId { get; }
        public string GAILPwd { get; }

    }
}
