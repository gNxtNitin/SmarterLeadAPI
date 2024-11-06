using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmarterLead.API.DataServices;
using SmarterLead.API.Helper;
using SmarterLead.API.Models.RequestModel;
using Stripe;
using Stripe.Checkout;
using System.Web;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SmarterLead.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly ILogger<PaymentsController> _logger;
    private IConfiguration _config;
    private readonly IOptions<StripeOptions> options;
    private readonly IStripeClient client;
    private readonly ApplicationDbContext _context;
    public CPAService _service;

    public PaymentsController(IOptions<StripeOptions> options, ILogger<PaymentsController> logger, IConfiguration config, ApplicationDbContext context)
    {
        _logger = logger;
        _config = config;
        this.options = options;
        _context = context;
        this.client = new StripeClient(this.options.Value.SecretKey);
        _service = new CPAService(_config);
    }

    [HttpGet("config")]
    public ConfigResponse Setup()
    {
        return new ConfigResponse
        {
            ProPrice = this.options.Value.ProPrice,
            GoldPrice = this.options.Value.GoldPrice,
            SilverPrice = this.options.Value.SilverPrice,
            BasicPrice = this.options.Value.BasicPrice,
            PublishableKey = this.options.Value.PublishableKey,
        };
    }

    [HttpPost("Create-Prouct-Checkout-Session")]
    [Authorize]
    public async Task<ActionResult> CreateCheckoutSessionByProduct([FromBody] ProductRequest pr)
    {
        var jsonData = new {
                CouponCode = (pr.CouponCode == null ? "X" : pr.CouponCode),
                ProductID = pr.ProductId
        };
        
        
        var tkn1 = JsonConvert.SerializeObject(jsonData);
        //var tkn1 = ( pr.CouponCode == null ? "X": pr.CouponCode ) + "&" + pr.ProductId;
        // tkn1 = await _service.Encrypt(tkn1);
        


        
        tkn1=await _service.Encrypt(tkn1);


        
        tkn1 = HttpUtility.UrlEncode(tkn1);
        //tkn1 = Uri.UnescapeDataString(tkn1);
        var options = new SessionCreateOptions
        {

           PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = pr.ProductName,
                        Description = pr.ProductDescription,
                    },
                    UnitAmount = int.Parse(pr.ProductPrice) * 100, // Amount in cents
                },
                Quantity = 1,
            },
        },
            
            Mode = "payment",
            //SuccessUrl = "https://localhost:7184/Payments/success?session_id={CHECKOUT_SESSION_ID}",
            //CancelUrl = "https://localhost:7184/Payments/Fail"
            SuccessUrl = pr.SuccessUrl + "?id={CHECKOUT_SESSION_ID}" + "&cc=" + tkn1,
            CancelUrl = pr.FailedUrl,
            CustomerEmail = pr.Email,
            

        };
        //var p = CHECKOUT_SESSION_ID;

        //var percentageOff = 20 / 100.0; // Assume the DiscountValue is 20 for 20% off
        //var discountAmount = 200000 * percentageOff;
        //var service = new CouponService();
        //var options = new CouponCreateOptions
        //{
        //    Duration = "repeating",
        //    DurationInMonths = 3,
        //    PercentOff = 25.5M,
        //};
        //service.Update("jMT0WJUD", options);
        //options.Discounts = new List<SessionDiscountOptions>
        //    {
        //        new SessionDiscountOptions
        //        {
        //            // Manually create a discount for the percentage off
        //            Coupon = "manual_coupon_" + "ABCDFG" // This can be a unique identifier for tracking
        //        }
        //    };

        //options.LineItems[0].PriceData.UnitAmount = 200000 - (int)discountAmount;

        if (pr.IsCoupon != false) // Check if the coupon is valid
        {
            // If it's a percentage discount
            if (pr.DiscountType == "P")
            {
                var percentageOff = float.Parse(pr.DiscountValue) / 100.0; // Assume the DiscountValue is 20 for 20% off
                var discountAmount = float.Parse(pr.ProductPrice) * percentageOff;
                //options.Discounts = new List<SessionDiscountOptions>
                // {
                //new SessionDiscountOptions
                //{
                //    // Manually create a discount for the percentage off
                //    Coupon = "manual_coupon_" + pr.CouponCode, // This can be a unique identifier for tracking
                //}
                // };

                options.LineItems[0].PriceData.UnitAmount = (int)(int.Parse(pr.ProductPrice) - discountAmount) * 100;
            }
            // If it's a fixed amount discount
            else
            {
                var discountAmount = int.Parse(pr.DiscountValue) * 100; // Convert to cents (e.g., $10 off becomes 1000)
                                                                        //options.Discounts = new List<SessionDiscountOptions>
                                                                        //{
                                                                        //    new SessionDiscountOptions
                                                                        //    {
                                                                        //        // Manually create a discount for the fixed amount off
                                                                        //        Coupon = "manual_coupon_" + pr.CouponCode, // Unique identifier for tracking
                                                                        //    }
                                                                        //};

                options.LineItems[0].PriceData.UnitAmount = (int.Parse(pr.ProductPrice) - discountAmount) * 100;
            }
        }

       
        try
        {
            var service = new SessionService(this.client);
            Session session = await service.CreateAsync(options);

            return Ok(session);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "An exception has occured in creating instance of the stripe Object.");
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("create-checkout-session")]
    [Authorize]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] PriceTag sk)
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{this.options.Value.Domain}/Success/{{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{this.options.Value.Domain}/Fail",
            Mode = "subscription",
            LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = sk.pk.ToString(),
                        Quantity = 1,
                    },
                },
            // AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
        };
        var service = new SessionService(this.client);
        try
        {
            var session = await service.CreateAsync(options);
            return Json(session);
        }
        catch (StripeException e)
        {
            Console.WriteLine(e.StripeError.Message);
            return BadRequest(new ErrorResponse
            {
                ErrorMessage = new ErrorMessage
                {
                    Message = e.StripeError.Message,
                }
            });
        }
    }

    [HttpGet("checkout-session")]
    public async Task<IActionResult> CheckoutSession(string sessionId)
    {
        var service = new SessionService(this.client);
        var session = await service.GetAsync(sessionId);
        return Ok(session);
    }

    [HttpPost("customer-portal")]
    public async Task<IActionResult> CustomerPortal(string sessionId)
    {
        // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
        // Typically this is stored alongside the authenticated user in your database.
        var checkoutService = new SessionService(this.client);
        var checkoutSession = await checkoutService.GetAsync(sessionId);

        // This is the URL to which your customer will return after
        // they are done managing billing in the Customer Portal.
        var returnUrl = this.options.Value.Domain;

        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = checkoutSession.CustomerId,
            ReturnUrl = returnUrl,
        };
        var service = new Stripe.BillingPortal.SessionService(this.client);
        var session = await service.CreateAsync(options);

        return Redirect(session.Url);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                this.options.Value.WebhookSecret
            );
            Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something failed {e}");
            return BadRequest();
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            Console.WriteLine($"Session ID: {session.Id}");
            // Take some action based on session.
        }

        return Ok();
    }
    [HttpPost("SaveData")]
    [Authorize]
    public async Task<IActionResult> SaveData([FromBody] PaymentDataRequest r)
    {

        var result = await _context.PaymentDataUpdate(r);

        if (result.Count() > 2)
        {
            return Ok(result);
        }
        return Unauthorized();
    }
    [HttpGet("CheckCoupon")]
    [Authorize]
    public async Task<IActionResult> CheckCoupon(string cc)
    {

        var result = await _context.CheckCoupon(cc);

        if (result.Count() > 2)
        {
            return Ok(result);
        }
        return Unauthorized();
    }
}

