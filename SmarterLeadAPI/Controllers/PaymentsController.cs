using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmarterLead.API.DataServices;
using SmarterLead.API.Models.RequestModel;
using Stripe;
using Stripe.Checkout;

namespace SmarterLead.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IOptions<StripeOptions> options;
    private readonly IStripeClient client;
    private readonly ApplicationDbContext _context;

    public PaymentsController(IOptions<StripeOptions> options, ApplicationDbContext context)
    {
        this.options = options;
        _context = context;
        this.client = new StripeClient(this.options.Value.SecretKey);
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

    [HttpGet("Testing")]
    public async Task<ActionResult> CreateCheckoutSession1()
    {
        var options1 = new CouponCreateOptions
        {
            Duration = "repeating",
            DurationInMonths = 3,
            PercentOff = 25.5M,
        };
        var service1 = new CouponService();
        service1.Create(options1);
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
                        Name = "Sample Product",
                    },
                    UnitAmount = 200000, // Amount in cents
                },
                Quantity = 1,
            },
        },
            Mode = "payment",
            SuccessUrl = "https://localhost:7184/Payments/success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = "https://localhost:7184/Payments/Fail",
        };

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

        //if (coupon != null && coupon.IsValid) // Check if the coupon is valid
        //{
        //    // If it's a percentage discount
        //    if (coupon.DiscountType == "percentage")
        //    {
        //        var percentageOff = coupon.DiscountValue / 100.0; // Assume the DiscountValue is 20 for 20% off
        //        var discountAmount = productPrice * percentageOff;
        //        options.Discounts = new List<SessionDiscountOptions>
        //    {
        //        new SessionDiscountOptions
        //        {
        //            // Manually create a discount for the percentage off
        //            Coupon = "manual_coupon_" + couponCode // This can be a unique identifier for tracking
        //        }
        //    };

        //        options.LineItems[0].PriceData.UnitAmount = productPrice - (int)discountAmount;
        //    }
        //    // If it's a fixed amount discount
        //    else if (coupon.DiscountType == "fixed")
        //    {
        //        var discountAmount = coupon.DiscountValue * 100; // Convert to cents (e.g., $10 off becomes 1000)
        //        options.Discounts = new List<SessionDiscountOptions>
        //    {
        //        new SessionDiscountOptions
        //        {
        //            // Manually create a discount for the fixed amount off
        //            Coupon = "manual_coupon_" + couponCode // Unique identifier for tracking
        //        }
        //    };

        //        options.LineItems[0].PriceData.UnitAmount = productPrice - discountAmount;
        //    }
        //}

        // Apply coupon if available
        //if (!string.IsNullOrEmpty(couponCode))
        //{
        //    var couponService = new CouponService();
        //    try
        //    {
        //        var coupon = couponService.Get(couponCode);

        //        if (coupon.Valid)
        //        {
        //            options.Discounts = new List<SessionDiscountOptions>
        //        {
        //            new SessionDiscountOptions
        //            {
        //                Coupon = coupon.Id
        //            }
        //        };
        //        }
        //    }
        //    catch (StripeException e)
        //    {
        //        // Handle invalid coupon code (e.g., coupon not found or expired)
        //        ViewBag.Error = "Invalid coupon code.";
        //        return View("Error");
        //    }
        //}

        var service = new SessionService(this.client);
        Session session = await service.CreateAsync(options);

        return Ok(session);
    }


    [HttpPost("create-checkout-session")]
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
    //[Authorize]
    public async Task<IActionResult> SaveData([FromBody] PaymentDataRequest r)
    {

        var leadsCount = await _context.PaymentDataUpdate(r);

        if (leadsCount != "")
        {
            return Ok(leadsCount);
        }
        return Unauthorized();
    }
}

