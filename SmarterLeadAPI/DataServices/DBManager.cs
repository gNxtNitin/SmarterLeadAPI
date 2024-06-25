using System.Data;
using System.Text.Json;
using System.Text;
using MySql.Data.MySqlClient;
using Serilog;
using System.Security.Cryptography;
using ILogger = Serilog.ILogger;
using Newtonsoft.Json;

namespace SmarterLeadAPI.DataServices
{
    public class DBManager
    {
        private string connStr="";
        private readonly ILogger _logger;
        public static IConfiguration _config;
        public DBManager(IConfiguration config,ILogger logger) 
        {
            _config = config;
            _logger = logger;
        }  
        public DataTable GetDataTable(string query, string connStr)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();
                    MySqlDataAdapter adap = new MySqlDataAdapter(command);
                    adap.Fill(dt);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error while running the query {query}\n Message: {ex.Message}\n Inner Exception: {ex.InnerException}\n Stack Trace: {ex.StackTrace}\n");
            }
            return dt;
        }
        public DataTable GetDataTable(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();
                    MySqlDataAdapter adap = new MySqlDataAdapter(command);
                    adap.Fill(dt);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error while running the query {query}\n Message: {ex.Message}\n Inner Exception: {ex.InnerException}\n Stack Trace: {ex.StackTrace}\n");
            }
            return dt;
        }
        public DataSet GetDataSet(string query, string connStr)
        {
            DataSet ds = new DataSet();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();
                    MySqlDataAdapter adap = new MySqlDataAdapter(command);
                    adap.Fill(ds);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error while running the query {query}\n Message: {ex.Message}\n Inner Exception: {ex.InnerException}\n Stack Trace: {ex.StackTrace}\n");
            }
            return ds;
        }
        public DataSet GetDataSet(string query)
        {
            DataSet ds = new DataSet();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();
                    MySqlDataAdapter adap = new MySqlDataAdapter(command);
                    adap.Fill(ds);
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error while running the query {query}\n Message: {ex.Message}\n Inner Exception: {ex.InnerException}\n Stack Trace: {ex.StackTrace}\n");
            }
            return ds;
        }
        public int ExecuteNonQuery(string query)
        {
            int result = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error while running the query {query}\n Message: {ex.Message}\n Inner Exception: {ex.InnerException}\n Stack Trace: {ex.StackTrace}\n");
            }
            return result;
        }
        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value).Replace("\\", "");
        }
        public string ConvertToJSON(DataSet dataSet)
        {
            int counter = 0;
            var JSONString = new StringBuilder();
            string json = string.Empty;
            JSONString.Append("{");
            foreach (DataTable table in dataSet.Tables)
            {
                counter++;
                string tableName = table.TableName;
                if (tableName == null || String.IsNullOrEmpty(tableName))
                {
                    tableName = "Table" + counter.ToString();
                }
                JSONString.Append("\"" + tableName + "\":");
                JSONString.Append("[");
                if (table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        JSONString.Append("{");
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            string rowData = table.Rows[i][j].ToString();
                            StringWriter wr = new StringWriter();
                            var jsonWriter = new JsonTextWriter(wr);
                            jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                            new Newtonsoft.Json.JsonSerializer().Serialize(jsonWriter, rowData);
                            rowData = wr.ToString();
                            if (j < table.Columns.Count - 1)
                            {
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData + ",");
                            }
                            else if (j == table.Columns.Count - 1)
                            {
                                JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData);
                            }
                        }
                        if (i == table.Rows.Count - 1)
                        {
                            JSONString.Append("}");
                        }
                        else
                        {
                            JSONString.Append("},");
                        }
                    }
                }
                else
                {
                    //JSONString.Append("{}");
                }
                if (counter == dataSet.Tables.Count)
                {
                    JSONString.Append("]");
                }
                else
                {
                    JSONString.Append("],");
                }
            }
            JSONString.Append("}");
            return JSONString.ToString();// JToken.Parse(JSONString.ToString()).ToString();
        }
        public string ConvertToJSON(DataTable table)
        {
            int counter = 0;
            var JSONString = new StringBuilder();
            string json = string.Empty;
            JSONString.Append("{");
            counter++;
            string tableName = table.TableName;
            if (tableName == null || String.IsNullOrEmpty(tableName))
            {
                tableName = "Table" + counter.ToString();
            }
            JSONString.Append("\"" + tableName + "\":");
            JSONString.Append("[");
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        string rowData = table.Rows[i][j].ToString();
                        StringWriter wr = new StringWriter();
                        var jsonWriter = new JsonTextWriter(wr);
                        jsonWriter.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
                        new Newtonsoft.Json.JsonSerializer().Serialize(jsonWriter, rowData);
                        rowData = wr.ToString();
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData + ",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + rowData);
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
            }
            JSONString.Append("]}");
            return JSONString.ToString();// JToken.Parse(JSONString.ToString()).ToString();
        }
    }
}
