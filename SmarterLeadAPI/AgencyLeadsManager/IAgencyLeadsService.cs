using SmarterLead.API.AgencyLeadsManager.Entities;

namespace SmarterLead.API.AgencyLeadsManager
{
    public interface IAgencyLeadsService
    {
        //public Task<string> CreateDailyLeadCSVForAgency(int agencyId);

        //public void CheckDNDStatus(string phoneNo);

        public Task<Dictionary<int, string>> UploadLeadsToZohoCRM();

        //public Task<bool> CaptureCompletionResponse(object resp);

        public Task<bool> UpdateLeadStatus(ZohoLeadStatusData data);

        public Task<string> SaveEmailResponse(string id, string response, string forAgency);

        public Task<bool> ParseAndSaveCallInfo(int agencyId, string callInfo);

        public Task<bool> CallCreateCampaignFlow(int agencyId, GailContact gailContact);

        public Task<bool> CreateCampaignFlowCompleted(int agencyId, string leadId, bool isCampaignCreated);
    }
}
