namespace SmarterLead.API.Models.RequestModel
{
    public class InvoiceRequest
    {
        public int PaymentID { get; set; }
        public int ClientID { get; set; }
        public int PlanID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public string PhoneNumber { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Zip { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string PlanName { get; set; }
        public string Description { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsCoupon { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
    }
}
