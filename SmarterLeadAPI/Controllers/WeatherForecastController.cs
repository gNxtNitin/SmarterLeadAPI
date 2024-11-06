using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmarterLead.API.DataServices;
using SmarterLead.API.Helper;
using SmarterLead.API.Models.RequestModel;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Stripe;
using System.Text.Json;

namespace SmarterLeadAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public WeatherForecastController( ApplicationDbContext context)
        {
            
            _context = context;
            
            
        }

        [HttpGet("zohodata")]
        //[Authorize]
        public async Task<IActionResult> Zohodata()
        {

            var leadsCount = await _context.ZohoData("er", "this");

            if (leadsCount != null)
            {
                return Ok(leadsCount);
            }
            return BadRequest("Invalid request.");
        }


    }
}
