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
//                        parameters.Add("_clientLoginID", clientLoginId, DbType.String);
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
    }
}
