using Dapper;
using Microsoft.EntityFrameworkCore;
//using MySql.Data.MySqlClient;
using MySqlConnector;
using Newtonsoft.Json;
using SmarterLead.API.Models.RequestModel;
using SmarterLead.API.Models.ResponseModel;
using Sprache;
using Stripe;
using System.Data;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.WebRequestMethods;
namespace SmarterLead.API.DataServices
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> _logger;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger)
            : base(options)
        {
            _logger = logger;
        }
        public async Task<UserLoginResponse> ValidateUser(UserLoginRequest user)
        {
            var userLogin = new UserLoginResponse();
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_email", user.Email, DbType.String);
                        parameters.Add("_pwd", user.Password,DbType.String);
                        userLogin = await connection.QueryFirstOrDefaultAsync<UserLoginResponse>(
                            "pGetValidUser",
                            parameters,
                            commandType: CommandType.StoredProcedure);

                        if (userLogin == null)
                        {
                            _logger.LogWarning("User with UserId: {UserId} not found", user.Email);
                        }
                        else
                        {
                            _logger.LogInformation("User with UserId: {UserId} found", user.Email);
                        }

                        return userLogin;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.Email);
                    }
                }
            }
            catch (Exception ex) 
            {
                
            }
            return userLogin;
        }
        //public async Task<string> IsOtpRiq(UserLoginRequest user)
        //{
        //    string resp = "";
        //    try
        //    {
        //        using (var connection = new MySqlConnection(Database.GetConnectionString()))
        //        {
        //            try
        //            {
        //                var parameters = new DynamicParameters();
        //                parameters.Add("_email", user.Email, DbType.String);
        //                parameters.Add("_password", user.Password, DbType.String);
        //                //parameters.Add("_otp", user.otp, DbType.String);
        //                var response = await connection.QueryAsync(
        //                    "pIsOtpRiq",
        //                    parameters,
        //                    commandType: CommandType.StoredProcedure);
        //                resp = JsonConvert.SerializeObject(response);
        //                //if (userLogin == null)
        //                //{
        //                //    _logger.LogWarning("User with UserId: {UserId} not found", user.Email);
        //                //}
        //                //else
        //                //{
        //                //    _logger.LogInformation("User with UserId: {UserId} found", user.Email);
        //                //}
        //            }
        //            catch (Exception ex)
        //            {
        //                //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return resp;
        //}
        public async Task<string> LoginOtp(UserLoginRequest user)
        {
            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_email", user.Email, DbType.String);
                        parameters.Add("_password", user.Password, DbType.String);
                        parameters.Add("_otp", user.otp, DbType.String);
                        var response = await connection.QueryAsync(
                            "pVerifyLoginforOtp",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //if (userLogin == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.Email);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.Email);
                        //}

                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.Email);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> ResendLoginOtp(VerifyOtpRequest user)
        {
            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_email", user.email, DbType.String);
                        //parameters.Add("_password", user.Password, DbType.String);
                        parameters.Add("_otp", user.otp, DbType.String);
                        var response = await connection.QueryAsync(
                            "pResendLoginOtp",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //if (userLogin == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.Email);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.Email);
                        //}


                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.email);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<UserLoginResponse> ValidateLoginOtp(string otp, string email)
        {
            var userLogin = new UserLoginResponse();
            DataTable dt = new DataTable();
            //string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("_email", email, DbType.String);
                        parameters.Add("_otp", otp, DbType.String);


                        userLogin = await connection.QueryFirstOrDefaultAsync<UserLoginResponse>(
                            "pValidateLoginOtp",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        //resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return userLogin;

        }
        public async Task<string> ChangePassword(ChangePasswordRequest user)
        {

            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginId", user.ClientLoginID, DbType.Int32);
                        parameters.Add("_oldpwd", user.oldpwd, DbType.String);
                        parameters.Add("_newpwd", user.newpwd, DbType.String);

                        var response = await connection.ExecuteAsync(
                            "pChangePassword",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
            
        }
        public async Task<string> UpdateProfile(UserProfile user)
        {

            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientID", user.ClientID, DbType.Int32);
                        //parameters.Add("_UserID", user.UserID, DbType.String);
                        parameters.Add("_firstname", user.firstname, DbType.String);
                        parameters.Add("_lastname", user.lastname, DbType.String);
                        parameters.Add("_email", user.email, DbType.String);
                        parameters.Add("_phone", user.phone, DbType.String);
                        //parameters.Add("_birthday", user.birthday, DbType.String);
                        parameters.Add("_statecode", user.StateName, DbType.String);
                        parameters.Add("_company", user.CompanyName, DbType.String);
                        parameters.Add("_zipcode", user.Zip, DbType.String);
                        parameters.Add("_address", user.Address, DbType.String);
                        parameters.Add("_city", user.City, DbType.String);
                        var response = await connection.ExecuteAsync(
                            "pUpdateUserProfile",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        if (resp != "0")
                        {
                            var result = new List<string>();

                            for (int i = 0; i < 4; i++)
                            {
                                result.Add("0");  // Add a string "0" to the list
                            }
                            
                            foreach (string item in user.roleid)
                            {
                                result[Int32.Parse(item)] = "1";
                            }
                            var param = result[1] + "," + result[2] + "," + result[3];
                            var parameters1 = new DynamicParameters();
                            parameters1.Add("_email", user.email, DbType.String);
                            parameters1.Add("param", param, DbType.String);
                            var response1 = await connection.ExecuteAsync(
                                "pupdaterole",
                                parameters1,
                                commandType: CommandType.StoredProcedure);
                            

                        }
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;

        }
        public async Task<string> UploadImage(int clientLoginId)
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginId", clientLoginId, DbType.Int32);
                        var response = await connection.ExecuteAsync(
                            "pUploadImage",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> SignUp(SignUpRequest user)
        {

            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                       
                        parameters.Add("_email", user.email, DbType.String);
                        parameters.Add("_password", user.password, DbType.String);
                        parameters.Add("_firstname", user.firstName, DbType.String);
                        parameters.Add("_lastname", user.lastName, DbType.String);
                        parameters.Add("_phone", user.phone, DbType.String);
                        parameters.Add("_company", user.company, DbType.String);
                        parameters.Add("_address", user.address, DbType.String);
                        parameters.Add("_city", user.city, DbType.String);
                        parameters.Add("_statecode", user.statecode, DbType.String);
                        parameters.Add("_zipcode", user.zipcode, DbType.String);
                        parameters.Add("_otp", user.otp, DbType.String);

                        var response = await connection.ExecuteAsync(
                            "pSignup",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        if (resp != "0")
                        {
                            foreach ( string item in  user.roleid )
                            {
                                
                                var parameters1 = new DynamicParameters();
                                parameters1.Add("_email", user.email, DbType.String);
                                parameters1.Add("_roleid", Int32.Parse(item), DbType.Int32);
                                var response1 = await connection.ExecuteAsync(
                                "pinsertrole",
                                parameters1,
                                commandType: CommandType.StoredProcedure);
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;

        }
        public async Task<string> VerifyEmail(ForgotPasswordRequest fpr, string otp)
        {

            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        
                        parameters.Add("_email", fpr.Email, DbType.String);
                        parameters.Add("_otp", otp, DbType.String);


                        var response = await connection.QueryAsync(
                            "pVerifyEmail",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;

        }
        public async Task<string> VerifyOtp(string otp, string email)
        {

            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("_email", email, DbType.String);
                        parameters.Add("_otp", otp, DbType.String);


                        var response = await connection.QueryAsync(
                            "pVerifyOtp",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;

        }
        public async Task<UserLoginResponse> VerifySignUp(VerifyOtpRequest vor)
        {
            var userLogin = new UserLoginResponse();
            //DataTable dt = new DataTable();
            //string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("_email", vor.email, DbType.String);
                        parameters.Add("_otp", vor.otp, DbType.String);


                        userLogin = await connection.QueryFirstOrDefaultAsync<UserLoginResponse>(
                            "pVerifySignUp",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        //return userLogin;
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}");
                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}");
            }
            return userLogin;

        }
        public async Task<string> CreatePassword(CreatePasswordRequest cpr)
        {

            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("_email", cpr.email, DbType.String);
                        parameters.Add("_pwd", cpr.password, DbType.String);


                        var response = await connection.ExecuteAsync(
                            "pCreatePassword",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;

        }
        public async Task<User> GetUserById(int userId) 
        {
            var user = new User();
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_userId", userId, DbType.String);
                        user = await connection.QueryFirstOrDefaultAsync<User>(
                            "pGetUserById",
                            parameters,
                            commandType: CommandType.StoredProcedure);

                        if (user == null)
                        {
                            _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        }
                        else
                        {
                            _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return user;
        }
        public async Task<string> GetDashBoardHeaders(int clientLoginId)
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginID", clientLoginId, DbType.String);
                        var response = await connection.QueryFirstOrDefaultAsync(
                            "pGetDashboardDetails",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> GetDashBoardLeadStats(int clientLoginId)
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginID", clientLoginId, DbType.String);
                        var response = await connection.QueryAsync(
                            //"pGetDashboardLeadStats",
                            "potest",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> GetDashBoardPie(int clientLoginId)
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginID", clientLoginId, DbType.Int32);
                        var response = await connection.QueryAsync(
                            "pGetPie",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> GetSearchLeadStats(SearchLeadRequest r)
        {
            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientID", r.ClientLoginID, DbType.Int32);
                        parameters.Add("_userLimit", r.UserLimit, DbType.Int32);
                        parameters.Add("_stateCode", r.statetext, DbType.String);
                        parameters.Add("_entityType", r.entitytypetext, DbType.String);
                        parameters.Add("_role", r.role, DbType.Int32);
                        parameters.Add("_cargoCarriedName", r.cargocarriedtext, DbType.String);
                        parameters.Add("_operation", r.classificationtext, DbType.String);
                        //parameters.Add("_carcarried", r.CargoCarried, DbType.String);
                        parameters.Add("_fromPU", r.PowerUnitSt, DbType.Int32);
                        parameters.Add("_toPU", r.PowerUnitEnd, DbType.Int32);
                        parameters.Add("_fromTD", r.DriverSt, DbType.Int32);
                        parameters.Add("_toTD", r.DriverEnd, DbType.Int32);
                        parameters.Add("_fromVI", r.VehicleInsSt, DbType.Int32);
                        parameters.Add("_toVI", r.VehicleInsEnd, DbType.Int32);
                        parameters.Add("_fromDI", r.DriveInsSt, DbType.Int32);
                        parameters.Add("_toDI", r.DriveInsEnd, DbType.Int32);
                        parameters.Add("_fromHI", r.HazmatSt, DbType.Int32);
                        parameters.Add("_toHI", r.HazmatEnd, DbType.Int32);
                        parameters.Add("_fromOV", r.OOsSt, DbType.Int32);
                        parameters.Add("_toOV", r.OOsEnd, DbType.Int32);

                        var response = await connection.QueryAsync(
                            "pGetSearchedLeads",
                            //"pNewTest1",
                            //"pppp3",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<List<List<dynamic>>> GetData()
        {
            string resp = "";
            List<List<dynamic>> data = new List<List<dynamic>>();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {

                        using (var multi = await connection.QueryMultipleAsync("pGetData", commandType: CommandType.StoredProcedure))
                        {
                            var resultSet1 = multi.Read<dynamic>().ToList();
                            //data.Add(resultSet1);

                            var resultSet2 = multi.Read<dynamic>().ToList();
                            //data.Add(resultSet2);

                            var resultSet3 = multi.Read<dynamic>().ToList();
                            //data.Add(resultSet3);

                            var resultSet4 = multi.Read<dynamic>().ToList();


                            var resultSet5 = multi.Read<dynamic>().ToList();

                            data = [resultSet1, resultSet2, resultSet3, resultSet4, resultSet5];


                        }
                        //var response = await connection.QueryAsync(
                        //    "pGetOperatingStatus",
                        //    commandType: CommandType.StoredProcedure);
                        //resp = JsonConvert.SerializeObject(response);
                        //data.Add(resp);
                        //response = await connection.QueryAsync(
                        //    "pGetEntityType",
                        //    commandType: CommandType.StoredProcedure);
                        //string resp1 = JsonConvert.SerializeObject(response);
                        ////data.Add(resp1);
                        //response = await connection.QueryAsync(
                        //    "pGetStateCode",
                        //    commandType: CommandType.StoredProcedure);
                        //string resp2 = JsonConvert.SerializeObject(response);
                        ////data.Add(resp2);
                        //response = await connection.QueryAsync(
                        //    "pGetCargoCarried",
                        //    commandType: CommandType.StoredProcedure);
                        //string resp3 = JsonConvert.SerializeObject(response);
                        ////data.Add(resp3);
                        //data = [resp, resp1, resp2, resp3];
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return data;
        }
        //public async Task<dynamic> GetAllUsers()
        //{
        //    //string resp = "";
        //    dynamic resp = new {};
        //    try
        //    {
        //        using (var connection = new MySqlConnection(Database.GetConnectionString()))
        //        {
        //            try
        //            {

        //                var response = await connection.QueryAsync(
        //                    "pGetAllUsers",
        //                    commandType: CommandType.StoredProcedure);
        //                resp = JsonConvert.SerializeObject(response);
        //                //data.Add(resp);
                        
        //            }
        //            catch (Exception ex)
        //            {
        //                //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return resp;
        //}
        public async Task<string> DownloadLeads(DownloadLeadsRequest r)
        {
            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientID", r.ClientLoginID, DbType.Int32);
                        parameters.Add("_dwdCount", r.Count, DbType.Int32);
                        parameters.Add("_searchID", r.SearchId, DbType.Int32);
                        var response = await connection.QueryAsync(
                            "pDownloadLeads",
                            //"pNewTest2",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> GetDwldLeadSummary(int clientLoginId, int summaryId)
        {
            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginId", clientLoginId, DbType.String);
                        parameters.Add("_summaryId", summaryId, DbType.String);
                        var response = await connection.QueryAsync(
                            "pGetDwldLeadSummary",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> GetDwldLeadDetails(int _clientDwdLeadSummaryID)
        {
            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientDwdLeadSummaryID", _clientDwdLeadSummaryID, DbType.String);
                        var response = await connection.QueryAsync(
                            "pGetClientdwdLeadDetail",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }

        public async Task<string> GetPurchaseHistory(int ClientID)
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_ClientID", ClientID, DbType.Int32);
                        var response = await connection.QueryAsync(
                            "pGetPaymentHistory",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        //return response;
                        //resp = response;
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }

        public async Task<string> GetCurrentPlan(int ClientID)
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_ClientID", ClientID);
                        var response = await connection.QueryAsync(
                            "pGetCurrentPlan",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        //return response;
                        //resp = response;
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }

        public async Task<string> GetInvoice(int ClientPlanID)
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_ClientPlanID", ClientPlanID);
                        var response = await connection.QueryAsync(
                            "pGetInvoice",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        //return response;
                        //resp = response;
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> GetPlans()
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        //var parameters = new DynamicParameters();
                        //parameters.Add("_ClientPlanID", ClientPlanID);
                        var response = await connection.QueryAsync(
                            "pGetPlans",
                            
                            commandType: CommandType.StoredProcedure);
                        //return response;
                        //resp = response;
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }

        //public async Task<string> DownloadLeads(int clientID, int searchID, int dwdCount)
        //{
        //    string resp = "";
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        using (var connection = new MySqlConnection(Database.GetConnectionString()))
        //        {
        //            try
        //            {
        //                var parameters = new DynamicParameters();
        //                parameters.Add("_clientID", clientID, DbType.Int32);
        //                parameters.Add("_searchID", searchID, DbType.Int32);
        //                parameters.Add("_dwdCount", dwdCount, DbType.Int32);
        //                var response = await connection.QueryAsync(
        //                    "pDownloadLeads",
        //                    parameters,
        //                    commandType: CommandType.StoredProcedure);
        //                resp = JsonConvert.SerializeObject(response);
        //                //if (resp == null)
        //                //{
        //                //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
        //                //}
        //                //else
        //                //{
        //                //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
        //                //}
        //            }
        //            catch (Exception ex)
        //            {
        //                //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return resp;
        //}
        public async Task<string> PaymentDataUpdate(PaymentDataRequest r)
        {
            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginId", r.ClientLoginId, DbType.Int32);
                        parameters.Add("_clientID", r.ClientId, DbType.Int32);
                        parameters.Add("_paymentDate", r.PaymentDate, DbType.String);
                        parameters.Add("_paymentStatus", r.PaymentStatus, DbType.String);
                        parameters.Add("_invoiceNumber", r.InvoiceNumber, DbType.String);
                        parameters.Add("_invoiceAmount", r.InvoiceAmount, DbType.String);
                        parameters.Add("_couponCode", r.CouponCode, DbType.String);
                        parameters.Add("_planID", r.PlanID, DbType.Int32);
                        var response = await connection.QueryAsync(
                            "pSavePayment",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        public async Task<string> GetUserProfile(int clientLoginId)
        {
            string resp = "";
            DataTable dt = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginID", clientLoginId, DbType.Int32);
                        var response = await connection.QueryFirstOrDefaultAsync(
                            "pGetUserProfile",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //if (resp == null)
                        //{
                        //    _logger.LogWarning("User with UserId: {UserId} not found", user.UserName);
                        //}
                        //else
                        //{
                        //    _logger.LogInformation("User with UserId: {UserId} found", user.UserName);
                        //}
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;
        }
        //public async Task<string> GetStates()
        //{
        //    string resp = "";
        //    try
        //    {
        //        using (var connection = new MySqlConnection(Database.GetConnectionString()))
        //        {
        //            try
        //            {
        //                var parameters = new DynamicParameters();
                        
        //                var response = await connection.QueryAsync(
        //                    "pGetStateCode",
        //                    parameters,
        //                    commandType: CommandType.StoredProcedure);
        //                resp = JsonConvert.SerializeObject(response);
        //            }
        //            catch (Exception ex)
        //            {
        //                //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return resp;
        //}
        public async Task<List<List<dynamic>>> GetStates()
        {
            string resp = "";
            List<List<dynamic>> data = new List<List<dynamic>>();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {

                        using (var multi = await connection.QueryMultipleAsync("pGetStateCode", commandType: CommandType.StoredProcedure))
                        {
                            var resultSet1 = multi.Read<dynamic>().ToList();
                            //data.Add(resultSet1);

                            var resultSet2 = multi.Read<dynamic>().ToList();
                            //data.Add(resultSet2);

                            
                            data = [resultSet1, resultSet2];


                        }
                        
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return data;
        }

        public async Task<string> CheckCoupon(string cc)
        {

            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();

                        parameters.Add("_couponCode", cc, DbType.String);
                        var response = await connection.QueryAsync(
                            "pCheckCoupon",
                            parameters,
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.UserName);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return resp;

        }
    }
}
