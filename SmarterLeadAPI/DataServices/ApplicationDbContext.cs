﻿using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Newtonsoft.Json;
using SmarterLead.API.Models.RequestModel;
using SmarterLead.API.Models.ResponseModel;
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
                        parameters.Add("_userId", user.Username, DbType.String);
                        parameters.Add("_pwd", user.Password,DbType.String);
                        userLogin = await connection.QueryFirstOrDefaultAsync<UserLoginResponse>(
                            "pGetValidUser",
                            parameters,
                            commandType: CommandType.StoredProcedure);

                        if (userLogin == null)
                        {
                            _logger.LogWarning("User with UserId: {UserId} not found", user.Username);
                        }
                        else
                        {
                            _logger.LogInformation("User with UserId: {UserId} found", user.Username);
                        }

                        return userLogin;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while calling stored procedure GetUserById with UserId: {UserId}", user.Username);
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
                        parameters.Add("_UserID", user.UserID, DbType.String);
                        parameters.Add("_firstname", user.firstname, DbType.String);
                        parameters.Add("_lastname", user.lastname, DbType.String);
                        parameters.Add("_email", user.email, DbType.String);
                        parameters.Add("_phone", user.phone, DbType.String);
                        parameters.Add("_birthday", user.birthday, DbType.String);
                        parameters.Add("_imagepath", user.imagepath, DbType.String);
                        var response = await connection.ExecuteAsync(
                            "pUpdateUserProfile",
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
                        parameters.Add("_username", user.username, DbType.String);
                        parameters.Add("_email", user.email, DbType.String);
                        parameters.Add("_password", user.password, DbType.String);
                        parameters.Add("_firstname", user.firstname, DbType.String);
                        parameters.Add("_lastname", user.lastname, DbType.String);
                        parameters.Add("_phone", user.phone, DbType.String);
                        parameters.Add("_imagepath", user.imagepath, DbType.String);
                        parameters.Add("_birthday", user.birthday, DbType.String);
                        parameters.Add("_address", user.address, DbType.String);
                        parameters.Add("_city", user.city, DbType.String);
                        parameters.Add("_statecode", user.statecode, DbType.String);
                        parameters.Add("_zipcode", user.zipcode, DbType.String);
                        

                        var response = await connection.ExecuteAsync(
                            "pSignup",
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
                            "pGetDashboardLeadStats",
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
                        parameters.Add("_stateCode", r.State, DbType.String);
                        parameters.Add("_entityType", r.EntityType, DbType.String);
                        parameters.Add("_cargoCarriedName", r.CargoCarried, DbType.String);
                        parameters.Add("_operatingStatus", r.Classifications, DbType.String);
                        parameters.Add("_carcarried", r.CargoCarried, DbType.String);
                        parameters.Add("_fromPU", r.PowerUnitSt, DbType.String);
                        parameters.Add("_toPU", r.PowerUnitEnd, DbType.String);
                        parameters.Add("_fromTD", r.DriverSt, DbType.String);
                        parameters.Add("_toTD", r.DriverEnd, DbType.String);
                        parameters.Add("_fromVI", r.VehicleInsSt, DbType.String);
                        parameters.Add("_toVI", r.VehicleInsEnd, DbType.String);
                        parameters.Add("_fromDI", r.DriveInsSt, DbType.String);
                        parameters.Add("_toDI", r.DriveInsEnd, DbType.String);
                        parameters.Add("_fromHI", r.HazmatSt, DbType.String);
                        parameters.Add("_toHI", r.HazmatEnd, DbType.String);
                        parameters.Add("_fromOV", r.OOsSt, DbType.String);
                        parameters.Add("_toOV", r.OOsEnd, DbType.String);

                        var response = await connection.QueryAsync(
                            "pGetSearchedLeads111",
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
        public async Task<List<dynamic>> GetData()
        {
            string resp = "";
            List<dynamic> data = new List<dynamic>();
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        
                        var response = await connection.QueryAsync(
                            "pGetOperatingStatus",
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //data.Add(resp);
                        response = await connection.QueryAsync(
                            "pGetEntityType",
                            commandType: CommandType.StoredProcedure);
                        string resp1 = JsonConvert.SerializeObject(response);
                        //data.Add(resp1);
                        response = await connection.QueryAsync(
                            "pGetStateCode",
                            commandType: CommandType.StoredProcedure);
                        string resp2 = JsonConvert.SerializeObject(response);
                        //data.Add(resp2);
                        response = await connection.QueryAsync(
                            "pGetCargoCarried",
                            commandType: CommandType.StoredProcedure);
                        string resp3 = JsonConvert.SerializeObject(response);
                        //data.Add(resp3);
                        data= [resp, resp1, resp2, resp3];
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
        public async Task<dynamic> GetAllUsers()
        {
            //string resp = "";
            dynamic resp = new {};
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {

                        var response = await connection.QueryAsync(
                            "pGetAllUsers",
                            commandType: CommandType.StoredProcedure);
                        resp = JsonConvert.SerializeObject(response);
                        //data.Add(resp);
                        
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

                        var response = await connection.ExecuteAsync(
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
    }
}
