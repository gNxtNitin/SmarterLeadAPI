using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmarterLead.API.DataServices;
using SmarterLead.API.Models.ResponseModel;
using SmarterLead.API.Models.RequestModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;


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
        private string GenerateRandomOTP()

        {
            string[] saAllowedCharacters = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };

            string sOTP = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();

            for (int i = 0; i < 6; i++)

            {

                int p = rand.Next(0, saAllowedCharacters.Length);

                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];

                sOTP += sTempChars;

            }

            return sOTP;

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
                        new Claim(ClaimTypes.Name, model.Email)
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
        [HttpPost("LoginOtp")]
        public async Task<IActionResult> LoginOtp([FromBody] UserLoginRequest model)
        {
            //model.otp = GenerateRandomOTP();
            model.otp = "000000";
            var resp = await _context.LoginOtp(model);
            if(resp.Count() > 2 )
            {
                return Ok(model.otp);
            }
            return StatusCode(404, "An error occurred while finding User. User Details Not Found!");
            
            
        }
        [HttpPost("VerifyLoginOtp")]
        public async Task<IActionResult> VerifyLoginOtp(VerifyOtpRequest vor)
        {

            var userDetails = await _context.ValidateLoginOtp(vor.otp, vor.email);
            if (userDetails != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("jwt:key"));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, vor.email)
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                userDetails.Token = tokenString;
                //return Ok(userDetails);
                return Ok(userDetails);
            }
            return Unauthorized();


        }
        [HttpPost("ResendLoginOtp")]
        public async Task<IActionResult> ResendLoginOtp([FromBody] VerifyOtpRequest model)
        {
            model.otp = GenerateRandomOTP();
            var resp = await _context.ResendLoginOtp(model);
            if (resp.Count() > 2)
            {
                return Ok(model.otp);
            }
            return StatusCode(404, "An error occurred while finding User. User Details Not Found!");
            

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

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null || request.ClientLoginID== null || string.IsNullOrEmpty(request.newpwd) || string.IsNullOrEmpty(request.oldpwd))
            {
                return BadRequest("Invalid request.");
            }

            // Call the stored procedure to update the password
            var result = await _context.ChangePassword(request);
            //return Ok(result);

            if (result == "1")
            {
                return Ok("Password updated successfully.");
            }
            
            return StatusCode(500, "An error occurred while updating the password.");
            
        }
        //[HttpGet("GetAllUsers")]
       
        //public async Task<IActionResult> GetAllUsers()
        //{
            
        //    var userDetails = await _context.GetAllUsers();
        //    if (userDetails != null)
        //    {
        //        return Ok(userDetails);
        //    }
        //    return Unauthorized();


        //}
        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfile request)
        {
            if (request == null || request.ClientID == null || string.IsNullOrEmpty(request.firstname))
            {
                return BadRequest("Invalid request.");
            }

            // Call the stored procedure to update the password
            var result = await _context.UpdateProfile(request);
            //return Ok(result);

            if (result == "1")
            {
                return Ok("Profile updated successfully.");
            }
            
            
             return StatusCode(500, "An error occurred while updating the profile.");
            
        }

        [HttpGet("GetUserProfile")]
        //[Authorize]
        public async Task<IActionResult> GetUserProfile(int clientLoginId)
        {
            //var userDetails = await _context.clientplan.FindAsync(id);
            var userDetails = await _context.GetUserProfile(clientLoginId);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();


        }
        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest model)
        {
            var userDetails = await _context.SignUp(model);
            if (userDetails != null)
            {
               
                return Ok(userDetails);
            }
            return Unauthorized();
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest fpr)
        {
            string otp = GenerateRandomOTP();
            var userDetails = await _context.VerifyEmail(fpr, otp);
            //var pp= userDetails.Count();
            if (userDetails.Count() > 2)
            {
                return Ok(otp);
                
            }
            return StatusCode(404, "An error occurred while finding email. Email Not Found!");
        }
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpRequest vor)
        {
            
            var userDetails = await _context.VerifyOtp(vor.otp, vor.email);
            if (userDetails.Count() > 2)
            {
                return Ok(userDetails);
            }
            return StatusCode(404, "An error occurred while finding email. Email Not Found!");


        }
        [HttpPost("VerifySignUp")]
        public async Task<IActionResult> VerifySignUp(VerifyOtpRequest vor)
        {

            var userDetails = await _context.VerifySignUp(vor.otp, vor.email);
            if (userDetails.Count() > 2)
            {
                return Ok(userDetails);
            }
            return StatusCode(404, "An error occurred while Verifying signup details. Email Not Found!");


        }
        [HttpPost("CreatePassword")]
        public async Task<IActionResult> CreatePassword(CreatePasswordRequest cpr)
        {
            
            var userDetails = await _context.CreatePassword(cpr);
            //var pp= userDetails.Count();
            if (userDetails != "0")
            {
                return Ok(userDetails);

            }
            return StatusCode(404, "An error occurred while creating new Password.");
        }
        [HttpGet("UploadImage")]
        public async Task<IActionResult> UploadImage(int clientLoginId)
        {
            var userDetails = await _context.UploadImage(clientLoginId);
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();
        }
        [HttpGet("GetStates")]
        //[Authorize]
        public async Task<IActionResult> GetStates()
        {
            //var userDetails = await _context.clientplan.FindAsync(id);
            var userDetails = await _context.GetStates();
            if (userDetails != null)
            {
                return Ok(userDetails);
            }
            return Unauthorized();


        }

    }
}
