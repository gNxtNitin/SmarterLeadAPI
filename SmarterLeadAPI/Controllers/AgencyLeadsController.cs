using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmarterLead.API.AgencyLeadsManager.Entities;
using SmarterLead.API.AgencyLeadsManager;
using Microsoft.AspNetCore.Authorization;

namespace SmarterLead.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgencyLeadsController : ControllerBase
    {
        IAgencyLeadsService _agencyLeadsService;
        public AgencyLeadsController(IAgencyLeadsService agencyLeadsService)
        {
            //_logger = logger;
            _agencyLeadsService = agencyLeadsService;

        }


        [HttpGet("UpldLeadOnZoho")]
        //[Authorize]
        public async Task<IActionResult> UploadLeadOnZoho()
        {
            try
            {
                var res = await _agencyLeadsService.UploadLeadsToZohoCRM();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }



        [HttpPost("SyncStatus")]

        public async Task<IActionResult> SyncLeadNCallStatus([FromBody] ZohoLeadStatusData zohoLeadStatus)
        {
            var res = await _agencyLeadsService.UpdateLeadStatus(zohoLeadStatus);

            return Ok(res);
        }

        [HttpGet("/api/submitresponse")]

        public async Task<ContentResult> SaveUserResponseFromEmail([FromQuery] string id, string response, string source)
        {

            var res = await _agencyLeadsService.SaveEmailResponse(id, response, source);

            return Content(res, "text/html");
        }

        [HttpPost("SaveCallDetail/{agencyId:int}")]

        public async Task<IActionResult> SaveGailCallInfo([FromRoute] int agencyId, [FromBody] object callInfo)
        {
            bool res = await _agencyLeadsService.ParseAndSaveCallInfo(agencyId, callInfo.ToString());

            return Ok(res);
        }


        [HttpPost("triggerFlow/{agencyId:int}")]

        public async Task<IActionResult> CreateCampaignFlowTrigger([FromRoute] int agencyId, [FromBody] GailContact contact)
        {
            bool res = await _agencyLeadsService.CallCreateCampaignFlow(agencyId, contact);

            return Ok(res);
        }

        [HttpPost("flowCompletedCallback")]
        [Authorize]
        public async Task<IActionResult> CreateCampaignFlowCompleted([FromQuery] int agencyId, string leadId, bool campaignStatus)
        {
            bool res = await _agencyLeadsService.CreateCampaignFlowCompleted(agencyId, leadId, campaignStatus);

            return Ok(res);
        }
    }
}
