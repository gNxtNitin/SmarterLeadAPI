using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmarterLead.API.DataServices;
using SmarterLead.API.Models.RequestModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmarterLead.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public AdminController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }
        [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest model)
        {
            var userDetails = await _context.ValidateUser(model);
            if (userDetails !=null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("jwt:key"));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.Username)
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                userDetails.Token = tokenString;
                return Ok(userDetails);
            }
            return Unauthorized();
        }
        [Authorize]
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById( int userId)
        {
            var userDetails = await _context.GetUserById(userId);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();
        }
    }
}
