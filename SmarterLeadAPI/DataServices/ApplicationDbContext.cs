using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Newtonsoft.Json;
using SmarterLead.API.Models.RequestModel;
using SmarterLead.API.Models.ResponseModel;
using System.Data;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
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
                        parameters.Add("_state", r.State, DbType.String);
                        parameters.Add("_entitytype", r.EntityType, DbType.String);
                        parameters.Add("_cargo", r.Cargo, DbType.String);
                        parameters.Add("_class", r.Classifications, DbType.String);
                        parameters.Add("_carcarried", r.CargoCarried, DbType.String);
                        parameters.Add("_powerunitst", r.PowerUnitSt, DbType.Int32);
                        parameters.Add("_powerunitend", r.PowerUnitEnd, DbType.Int32);
                        parameters.Add("_driverst", r.DriverSt, DbType.Int32);
                        parameters.Add("_driverend", r.DriverEnd, DbType.Int32);
                        parameters.Add("_driveinsst", r.DriveInsSt, DbType.Int32);
                        parameters.Add("_driveinsend", r.DriveInsEnd, DbType.Int32);
                        parameters.Add("_hazmatst", r.HazmatSt, DbType.Int32);
                        parameters.Add("_hazmatend", r.HazmatEnd, DbType.Int32);
                        parameters.Add("_oosst", r.OOsSt, DbType.Int32);
                        parameters.Add("_oostend", r.OOsEnd, DbType.Int32);

                        var response = await connection.QueryAsync(
                            "pGetSearchedLeads",
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
                            "GetDwldLeadSummary",
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
                        parameters.Add("ClientID", ClientID);
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

        public async Task<IEnumerable<dynamic>> GetInvoice(int ClientPlanID)
        {
            IEnumerable<dynamic> resp = [];
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
                        resp = response;
                        //resp = JsonConvert.SerializeObject(response);
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
        public async Task<IEnumerable<dynamic>> GetPlans()
        {
            IEnumerable<dynamic> resp = [];
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
                        resp = response;
                        //resp = JsonConvert.SerializeObject(response);
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
