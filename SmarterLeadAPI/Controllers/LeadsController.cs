using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarterLead.API.DataServices;
using SmarterLead.API.Models.RequestModel;
using SmarterLead.API.Models.ResponseModel;

namespace SmarterLead.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeadsController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public LeadsController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
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
        [HttpPost("GetSearchLeads")]
        [Authorize]
        public async Task<IActionResult> GetSearchLeads(SearchLeadRequest r)
        {
            var leadsCount = await _context.GetSearchLeadStats(r);
            if (leadsCount != null)
            {
                return Ok(leadsCount);
            }
            return Unauthorized();
        }
        [HttpGet("GetDwldLeadSummary")]
        [Authorize]
        public async Task<IActionResult> GetDwldLeadSummary(int clientLoginId,int summaryId = 0)
        {
            var result = await _context.GetDwldLeadSummary(clientLoginId,summaryId);
            if (result != null)
            {
                return Ok(result);
            }
            return Unauthorized();
        }
        [HttpGet("GetDwldLeadDetails")]
        [Authorize]
        public async Task<IActionResult> GetClientDwldLeadDetail(int ClientDwdLeadSummaryID)
        {
            var result = await _context.GetDwldLeadDetails(ClientDwdLeadSummaryID);
            if (result != null)
            {
                return Ok(result);
            }
            return Unauthorized();
        }
    }
}
