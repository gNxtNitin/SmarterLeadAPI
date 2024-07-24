namespace SmarterLead.API.Models.RequestModel
{
    public class DownloadLeadsRequest
    {
        public int ClientLoginID { get; set; }
        public int Count { get; set; }
        public int SearchId { get; set; }
    }
}
