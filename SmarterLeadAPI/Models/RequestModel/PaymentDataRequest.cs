namespace SmarterLead.API.Models.RequestModel
{
    public class PaymentDataRequest
    {
        public int ClientLoginId { get; set; }
        public int ClientId { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceAmount { get; set; }
        public string? CouponCode { get; set; }
        public string? PlanID { get; set; }

    }
}
