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

