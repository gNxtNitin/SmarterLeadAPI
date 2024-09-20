namespace SmarterLead.API.Models.RequestModel
{
    public class ProductRequest
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductPrice { get; set; }
        public string SuccessUrl { get; set; }
        public string FailedUrl { get; set; }
        public bool IsCoupon { get; set; }
        public string? CouponCode { get; set; }
        public string? DiscountType { get; set; }
        public string? DiscountValue { get; set; }

    }
}
