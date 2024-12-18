using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarterLead.API.DataServices;
using SmarterLead.API.Helper;
using SmarterLead.API.Models.RequestModel;
using SmarterLead.API.Models.ResponseModel;
using System.Text;
using System.Text.Json;

namespace SmarterLead.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
       
        private IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly JsonSerializerOptions _serializerOptions;
        public CPAService _service;
        public LeadsController(IConfiguration config, ApplicationDbContext context)
        {
            
            _config = config;
            _context = context;
            _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _service = new CPAService(_config);
        }
        [HttpGet("GetDashboardHeaderDetails")]
        [Authorize]
        public async Task<IActionResult> GetDashboardHeaderDetails(int clientLoginId)
        {
            var userDetails = await _context.GetDashBoardHeaders(clientLoginId);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();
        }
        [HttpGet("GetDashboardLeadStats")]
        [Authorize]
        public async Task<IActionResult> GetDashboardLeadStats(int clientLoginId)
        {
            var userDetails = await _context.GetDashBoardLeadStats(clientLoginId);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();
        }
        [HttpGet("GetDashboardPie")]
        [Authorize]
        public async Task<IActionResult> GetDashboardPie(int clientLoginId)
        {
            var userDetails = await _context.GetDashBoardPie(clientLoginId);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();
        }
        [HttpPost("GetSearchLeads")]
        [Authorize]
        public async Task<IActionResult> GetSearchLeads([FromBody] SearchLeadRequest r)
        {

            var leadsCount = await _context.GetSearchLeadStats(r);

            if (leadsCount != null)
            {
                return Ok(leadsCount);
            }
            return Unauthorized();
        }

        [HttpPost("DownloadLeads")]
        [Authorize]
        public async Task<IActionResult> DownloadLeads([FromBody] DownloadLeadsRequest r)
        {

            var leadsCount = await _context.DownloadLeads(r);

            if (leadsCount != null)
            {
                return Ok(leadsCount);
            }
            return Unauthorized();
        }
        [HttpGet("GetData")]
        [Authorize]
        public async Task<IActionResult> GetData()
        {
            var result = await _context.GetData();
            if (result != null)
            {
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpGet("GetDwldLeadSummary")]
        [Authorize]
        public async Task<IActionResult> GetDwldLeadSummary(int clientLoginId, int summaryId = 0)
        {
            var result = await _context.GetDwldLeadSummary(clientLoginId, summaryId);
            if (result != null)
            {
                return Ok(result);
            }
            return Unauthorized();
        }
        [HttpPost("GetDwldLeadDetails")]
        [Authorize]
        public async Task<IActionResult> GetClientDwldLeadDetail([FromBody] EncryptIdRequest eir)
        {
            var d = await _service.Decrypt(eir.Id);
            var result = await _context.GetDwldLeadDetails(int.Parse(d));
            if (result != null)
            {
                return Ok(result);
            }
            return Unauthorized();
        }

        [HttpGet("GetPurchaseHistory")]
        [Authorize]
        public async Task<IActionResult> GetPurchaseHistory(int ClientID)
        {
            //var userDetails = await _context.clientplan.Where(c => c.PlanID == clientLoginId).ToListAsync();
            var userDetails = await _context.GetPurchaseHistory(ClientID);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();


        }
        [HttpPost("GetInvoice")]
        [Authorize]
        public async Task<IActionResult> GetInvoice([FromBody] EncryptIdRequest eir)
        {
            var d = await _service.Decrypt(eir.Id);
            var userDetails = await _context.GetInvoice(int.Parse(d));
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();
        }

        [HttpGet("GetCurrentPlan")]
        [Authorize]
        public async Task<IActionResult> GetCurrentPlan(int clientLoginId)
        {
            //var userDetails = await _context.clientplan.FindAsync(id);
            var userDetails = await _context.GetCurrentPlan(clientLoginId);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();


        }

        [HttpGet("GetPlans")]
        //[Authorize]
        public async Task<IActionResult> GetPlans()
        {
            //var userDetails = await _context.clientplan.FindAsync(id);
            var userDetails = await _context.GetPlans();
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();
        }

        [HttpPost("anymer")]
        //[Authorize]
        public async Task<IActionResult> Anymer([FromBody] object r)
        {

            var leadsCount = r;
            
            var body = System.Text.Json.JsonSerializer.Serialize(r, _serializerOptions);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var result = client.PostAsync("https://prod-15.centralindia.logic.azure.com:443/workflows/0f59d427e3f44f83a3701b4f9b9b7f94/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=Ty6Z_AaEcWXPVQ4sFYtEeVli5ycGUuiVP30TFQ0zoEc", content);

            if (leadsCount != null)
            {
                return Ok(leadsCount);
            }
            return Unauthorized();
        }


    }
}
