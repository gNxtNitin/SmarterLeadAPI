﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarterLead.API.DataServices;
using SmarterLead.API.Helper;
using SmarterLead.API.Models.RequestModel;
using SmarterLead.API.Models.ResponseModel;
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
        

    }
}
