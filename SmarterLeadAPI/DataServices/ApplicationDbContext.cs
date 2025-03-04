﻿using Dapper;
using Microsoft.EntityFrameworkCore;
//using MySql.Data.MySqlClient;
using MySqlConnector;
using Newtonsoft.Json;
using SmarterLead.API.AgencyLeadsManager.Entities;
using SmarterLead.API.AgencyLeadsManager;
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
                        if (userLogin == null)
                        {
                            _logger.LogWarning("User with UserId: {UserId} not found", vor.email);
                        }
                        else
                        {
                            _logger.LogInformation("User with UserId: {UserId} found", vor.email);
                        }
                        return userLogin;
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
                            "pGetColumn",
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
                        parameters.Add("_role", decimal.Parse(r.role), DbType.Decimal);
                        parameters.Add("_cargoCarriedName", r.cargocarriedtext, DbType.String);
                        parameters.Add("_operation", r.classificationtext, DbType.String);
                        parameters.Add("_insurancecarrier", r.insurancecarriertext, DbType.String);
                        parameters.Add("_radiusOfOperation", r.radiusofoperationtext, DbType.String);
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
                        parameters.Add("_fromCR", r.CoverageSt, DbType.Int32);
                        parameters.Add("_toCR", r.CoverageEnd, DbType.Int32);
                        parameters.Add("_fromED", r.ExpirySt, DbType.Int32);
                        parameters.Add("_toED", r.ExpiryEnd, DbType.Int32);
                        parameters.Add("_fromMVR", r.MVRSt, DbType.Int32);
                        parameters.Add("_toMVR", r.MVREnd, DbType.Int32);

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
        public async Task<string> SaveSearchFilters(SearchLeadRequest r)
        {
            string resp = "";

            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientLoginID", r.ClientLoginID, DbType.Int32);
                        parameters.Add("_stateCode", r.statetext, DbType.String);
                        parameters.Add("_entityType", r.entitytypetext, DbType.String);
                        parameters.Add("_role", decimal.Parse(r.role), DbType.Decimal);
                        parameters.Add("_cargoCarriedName", r.cargocarriedtext, DbType.String);
                        parameters.Add("_operation", r.classificationtext, DbType.String);
                        parameters.Add("_insurancecarrier", r.insurancecarriertext, DbType.String);
                        parameters.Add("_radiusOfOperation", r.radiusofoperationtext, DbType.String);
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
                        parameters.Add("_fromCR", r.CoverageSt, DbType.Int32);
                        parameters.Add("_toCR", r.CoverageEnd, DbType.Int32);
                        parameters.Add("_fromED", r.ExpirySt, DbType.Int32);
                        parameters.Add("_toED", r.ExpiryEnd, DbType.Int32);
                        parameters.Add("_fromMVR", r.MVRSt, DbType.Int32);
                        parameters.Add("_toMVR", r.MVREnd, DbType.Int32);

                        var response = await connection.ExecuteAsync(
                            "pSaveFilterPreference2",
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
                            var resultSet6 = multi.Read<dynamic>().ToList();
                            var resultSet7 = multi.Read<dynamic>().ToList();


                            data = [resultSet1, resultSet2, resultSet3, resultSet4, resultSet5, resultSet6, resultSet7];


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
        public async Task<string> GetSavedFilters(int clientLoginId, string role)
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
                        parameters.Add("_roleE", role, DbType.String);
                        
                        var response = await connection.QueryFirstOrDefaultAsync(
                            "pGetSavedFilters2",
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
        public async Task<string> GetSearchedData(int summaryId)
        {
            string resp = "";
            try
            {
                using (var connection = new MySqlConnection(Database.GetConnectionString()))
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientDwdLeadSummaryID", summaryId, DbType.String);

                        var response = await connection.QueryFirstOrDefaultAsync(
                            "pGetSearchedData",
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
                            //"pGetClientdwdLeadDetail111",
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
                        var response = await connection.QueryFirstOrDefaultAsync<InvoiceRequest>(
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

        //public async Task<string> ZohoData(string id, string message)
        //{

        //    string resp = "";
        //    try
        //    {
        //        using (var connection = new MySqlConnection(Database.GetConnectionString()))
        //        {
        //            try
        //            {
        //                var parameters = new DynamicParameters();

        //                parameters.Add("_id", id, DbType.String);
        //                parameters.Add("_message", message, DbType.String);


        //                var response = await connection.ExecuteAsync(
        //                    //"pCreatePassword",
        //                    "polo",
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

        //Agency related Store procedures
        public async Task<IEnumerable<int>> GetActiveAgencyIds()
        {

            using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
            {
                IEnumerable<int> activeAgencyIds = Enumerable.Empty<int>();
                try
                {

                    activeAgencyIds = await connection.QueryAsync<int>("pGetActiveAgencyIds", commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    //log exception
                }

                return activeAgencyIds;
            }
        }

        public async Task<IEnumerable<AgencyDailyLeadEntity>> GetDailyLeadForAgency(int agencyId, DateTime allocatedDate)
        {
            IEnumerable<AgencyDailyLeadEntity> agencyDailyLeads = Enumerable.Empty<AgencyDailyLeadEntity>();

            using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("_clientid", agencyId, DbType.Int32);
                    parameters.Add("_date", allocatedDate, DbType.Date);

                    agencyDailyLeads = await connection.QueryAsync<AgencyDailyLeadEntity>("pGetDailyAgencyLeads", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    //log exception
                }

                return agencyDailyLeads;
            }
        }


        //public async void UpdateDNDStatus(int agencyId, bool DNDStatus)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("_clientid", agencyId, DbType.Int32);
        //        parameters.Add("_dndStatus", DNDStatus, DbType.Binary);

        //        await connection.ExecuteAsync("pGetActiveAgencyIds", parameters, commandType: CommandType.StoredProcedure);


        //    }
        //}

        public async void UpdateAgencyLeadStatus(string ZohoLeadId, string status)
        {
            using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("_zohoLeadId", ZohoLeadId, DbType.String);
                    parameters.Add("_leadStatus", status, DbType.String);
                    await connection.ExecuteAsync("pUpdateAgencyLeadStatus", commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    //log exception
                }

            }
        }

        public async Task<AgencyZohoSecret> GetAgencySecret(dynamic spParam, bool fromAgencyId)
        {
            AgencyZohoSecret secret = null;

            using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
            {
                try
                {
                    if (fromAgencyId)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientId", spParam, DbType.String);
                        parameters.Add("_zohoUId", DBNull.Value, DbType.String);
                        secret = await connection.QueryFirstOrDefaultAsync<AgencyZohoSecret>("pGetAgencySecrets", parameters, commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("_clientId", DBNull.Value, DbType.String);
                        parameters.Add("_zohoUId", spParam, DbType.String);
                        secret = await connection.QueryFirstOrDefaultAsync<AgencyZohoSecret>("pGetAgencySecrets", parameters, commandType: CommandType.StoredProcedure);

                    }
                }

                catch (Exception ex)
                {
                    //log exceptions
                }

            }

            return secret;
        }

        public async Task<int> AddZohoLeadId(int agencyId, int leadId, string zohoLeadId)
        {
            int rowAffected = 0;
            using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("_clientId", agencyId, DbType.Int32);
                    parameters.Add("_leadId", leadId, DbType.Int32);
                    parameters.Add("_zohoLeadId", zohoLeadId, DbType.String);

                    rowAffected = await connection.ExecuteAsync("pUpdateZohoLeadId", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    //log exception
                }

            }
            return rowAffected;
        }

        public async Task<bool> UpdateAgencyLeadStatus(string zohoLeadId, string callStatus, string leadStatus)
        {
            bool isRowAffected = false;

            using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("_zohoLeadId", zohoLeadId, DbType.String);
                    parameters.Add("_callStatus", callStatus, DbType.String);
                    parameters.Add("_leadStatus", leadStatus, DbType.String);

                    isRowAffected = await connection.ExecuteAsync("pSyncAgencyLeadStatus", parameters, commandType: CommandType.StoredProcedure) > 0;
                }
                catch (Exception ex)
                {
                    //log exception
                }

            }

            return isRowAffected;
        }

        public async Task<PromptText> GetPromptText(int promptId)
        {

            using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
            {
                PromptText promptText = null;

                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("promptId", promptId, DbType.Int32);
                    promptText = await connection.QueryFirstAsync<PromptText>("pGetPrompt", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {
                    //log exception
                }

                return promptText;
            }
        }

        public async Task<int> AddCallData(string zohoLeadId, CallDetail calldetail, QuickQuoteRoot quickQuoteDetail = null)
        {
            int rowAffected = 0;
            using (MySqlConnection connection = new MySqlConnection(Database.GetConnectionString()))
            {
                try
                {
                    string callDataInsertQuery = "INSERT INTO calldetail(CallSummary, CallStatus) VALUES (@CallSummary, @CallStatus); SELECT LAST_INSERT_ID();";

                    int callDetailId = await connection.ExecuteScalarAsync<int>(callDataInsertQuery, calldetail);



                    if (callDetailId > 0 && quickQuoteDetail != null)
                    {
                        quickQuoteDetail.QuickQuoteDetails.CallId = callDetailId;

                        string getIdQuery = "SELECT AgencyAssociateLeadID FROM agencyassociatelead WHERE ZohoLeadID = @ZohoLeadId";
                        int agencyassociateLeadId = await connection.ExecuteScalarAsync<int>(getIdQuery, new { ZohoLeadId = zohoLeadId });

                        if (agencyassociateLeadId > 0)
                        {
                            quickQuoteDetail.QuickQuoteDetails.AgencyAssociateLeadId = agencyassociateLeadId;

                            string quickQuoteInsertQuery = "INSERT INTO quickquotedetail (CallId, AgencyAssociateLeadId, OwnerName, EIN, BusinessName, BusinessYears, BusinessType, Violations, CargoTypes) VALUES (@CallId, @AgencyAssociateLeadId, @OwnerName, @EIN, @BusinessName, @BusinessYears, @BusinessType, @Violations, @CargoTypes); SELECT LAST_INSERT_ID();";

                            int quickQuoteRecordId = await connection.ExecuteScalarAsync<int>(quickQuoteInsertQuery, quickQuoteDetail.QuickQuoteDetails);

                            if (quickQuoteRecordId > 0 && quickQuoteDetail.Vehicles != null && quickQuoteDetail.Vehicles.Count > 0)
                            {
                                string quickQuoteVehicleInsertQuery = "INSERT INTO quickquotevehicledetail(QuickQuoteDetailId, VIN, Make_Model, Value, SafetyFeature, GaragingAddress, GVW) VALUES (@QuickQuoteDetailId, @VIN, @MakeModel, @Value, @SafetyFeatures, @GaragingAddress, @GVW)";

                                foreach (var vehicle in quickQuoteDetail.Vehicles)
                                {
                                    vehicle.QuickQuoteDetailId = quickQuoteRecordId;
                                    await connection.ExecuteAsync(quickQuoteVehicleInsertQuery, vehicle);
                                }
                            }

                            if (quickQuoteRecordId > 0 && quickQuoteDetail.Drivers != null && quickQuoteDetail.Drivers.Count > 0)
                            {
                                string quickQuoteDriverInsertQuery = "INSERT INTO quickquotedriverdetaiL(QuickQuoteDetailId, DriverName, LicenseNumber, CDLDuration, LicenseState) VALUES (@QuickQuoteDetailId, @DriverName, @LicenseNumber, @CDLDuration, @LicenseState)";

                                foreach (var driver in quickQuoteDetail.Drivers)
                                {
                                    driver.QuickQuoteDetailId = quickQuoteRecordId;
                                    await connection.ExecuteAsync(quickQuoteDriverInsertQuery, driver);
                                }
                            }
                        }
                    }

                    await UpdateAgencyLeadStatus(zohoLeadId, calldetail.CallStatus, string.Empty);
                }
                catch (Exception ex)
                {
                    //log exception
                }

            }
            return rowAffected;
        }

    }
}
