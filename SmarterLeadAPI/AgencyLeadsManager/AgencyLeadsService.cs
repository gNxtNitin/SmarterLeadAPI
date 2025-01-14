using SmarterLead.API.AgencyLeadsManager.Entities;
using SmarterLead.API.DataServices;
using Stripe;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace SmarterLead.API.AgencyLeadsManager
{
    public class AgencyLeadsService : IAgencyLeadsService
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;


        public AgencyLeadsService(ApplicationDbContext dbcontext, IWebHostEnvironment webHostEnv, IConfiguration config, HttpClient httpClient)
        {
            _dbcontext = dbcontext;
            _webHostEnv = webHostEnv;
            _config = config;
            _httpClient = httpClient;
        }

        //public void CheckDNDStatus(string phoneNo)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<string> CreateDailyLeadCSVForAgency(int agencyId)
        //{
        //    string csvPath = string.Empty;
        //    try
        //    {
        //        DateTime currentDay = DateTime.Now;
        //        var dailyLeadsForAgencyId = await _dbcontext.GetDailyLeadForAgency(agencyId, currentDay);


        //        if (dailyLeadsForAgencyId.Count() > 0)
        //        {
        //            csvPath = Path.Join(_webHostEnv.ContentRootPath, "TempFiles", "TempCSV", string.Concat(agencyId, "_Leads_", currentDay.ToString("yyyy_MM_dd"), ".csv"));


        //            using (var writer = new StreamWriter(csvPath))
        //            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        //            {
        //                csv.WriteRecords(dailyLeadsForAgencyId);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return csvPath;
        //}



        //public async Task<Dictionary<int, bool>> UploadLeadsToZohoCRM()
        //{

        //    Dictionary<int, bool> uploadRes = new Dictionary<int, bool>();

        //    try
        //    {
        //        var activeAgencyIds = await _dbcontext.GetActiveAgencyIds();

        //        foreach (int agencyId in activeAgencyIds)
        //        {

        //            string csvPath = await CreateDailyLeadCSVForAgency(agencyId);
        //            string accessToken = string.Empty;

        //            if (!string.IsNullOrEmpty(csvPath))
        //            {
        //                string tempZipPath = csvPath.Replace("TempCSV", "TempZip").Replace(".csv", ".zip");
        //                FileManager.ZipCSV(Path.Join(_webHostEnv.ContentRootPath, "TempFiles", "TempCSV"), tempZipPath);
        //                AgencyZohoSecret agencySecret = await _dbcontext.GetAgencySecret(agencyId, true);

        //                if (agencySecret != null)
        //                {
        //                    accessToken = GetOAuthToken(agencySecret.ZohoAccountURL, agencySecret.ZohoCID, agencySecret.ZohoKey, agencySecret.ZohoRtk);
        //                }
        //                else
        //                {
        //                    uploadRes.Add(agencyId, false);
        //                    continue;
        //                }

        //                if (!string.IsNullOrEmpty(accessToken))
        //                {

        //                    string fileId = await PostZipToZoho(agencySecret, tempZipPath, accessToken);
        //                    if (!string.IsNullOrEmpty(fileId))
        //                    {
        //                        var bulkApiCallResp = await CallBulkCreateAPI(agencySecret.ZohoAPIDomain, agencySecret.APIVersion, agencySecret.LeadsLayoutId, fileId, accessToken);
        //                        if (bulkApiCallResp == true)
        //                        {
        //                            uploadRes.Add(agencyId, true);
        //                        }
        //                        else
        //                        {
        //                            uploadRes.Add(agencyId, false);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        uploadRes.Add(agencyId, false);
        //                        continue;
        //                    }
        //                }
        //                else
        //                {
        //                    uploadRes.Add(agencyId, false);
        //                    continue;
        //                }

        //            }
        //            else
        //            {
        //                uploadRes.Add(agencyId, false);
        //                continue;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return uploadRes;
        //}

        private async Task<string> GetOAuthToken(string accountsUrl, string cid, string ckey, string rtoken)
        {
            string token = string.Empty;
            _httpClient.DefaultRequestHeaders.Clear();
            try
            {
                var reqBody = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", cid),
                    new KeyValuePair<string, string>("client_secret", ckey),
                    new KeyValuePair<string, string>("refresh_token", rtoken)
               });

                var resp = await _httpClient.PostAsync(accountsUrl, reqBody);

                if (resp.IsSuccessStatusCode)
                {
                    var respBody = resp.Content.ReadAsStringAsync().Result;
                    token = JsonSerializer.Deserialize<JsonElement>(respBody).GetProperty("access_token").GetString();
                }


            }
            catch (Exception ex)
            {
                //log exceptions
            }

            return token;
        }

        //private async Task<string> PostZipToZoho(AgencyZohoSecret secret, string zipPath, string token)
        //{
        //    string fileId = string.Empty;

        //    try
        //    {

        //        if (FileManager.DoesFileExist(zipPath))
        //        {
        //            var reqBody = new MultipartFormDataContent();
        //            var fileContent = new ByteArrayContent(File.ReadAllBytes(zipPath));
        //            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        //            reqBody.Add(fileContent, "file", Path.GetFileName(zipPath));

        //            string apiUrl = _config.GetValue<string>("ZohoAPIUrls:UploadZipFile").Replace("{api_domain}", secret.ZohoAPIDomain).Replace("{api_version}", secret.APIVersion);
        //            using (HttpClient client = new HttpClient())
        //            {
        //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);
        //                client.DefaultRequestHeaders.Add("feature", "bulk-write");
        //                client.DefaultRequestHeaders.Add("X-CRM-ORG", secret.ZohoOrgId);

        //                var resp = await client.PostAsync(apiUrl, reqBody);

        //                if (resp.IsSuccessStatusCode)
        //                {
        //                    var respBody = await resp.Content.ReadAsStringAsync();
        //                    var jsonResp = JsonSerializer.Deserialize<JsonElement>(respBody);
        //                    if (jsonResp.GetProperty("status").GetString().ToLower() == "success")
        //                    {
        //                        fileId = jsonResp.GetProperty("details").GetProperty("file_id").GetString();
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //log exceptions
        //    }
        //    finally
        //    {
        //        FileManager.DeleteFile(zipPath);
        //        FileManager.DeleteFile(zipPath.Replace("TempZip", "TempCSV").Replace(".zip", ".csv"));
        //    }

        //    return fileId;
        //}

        //private async Task<bool> CallBulkCreateAPI(string apiDomain, string apiVersion, string layoutId, string fileId, string token)
        //{
        //    try
        //    {
        //        HttpClient client = new HttpClient();
        //        string apiUrl = _config["ZohoAPIUrls:BulkWriteUrl"].Replace("{api_domain}", apiDomain).Replace("{api_version}", apiVersion);

        //        //string callbackUrl = _config["ZohoBulkWriteCallbackUrl"] + "/api/AgencyLeads/bulkJobStatus";
        //        string callbackUrl = "https://webhook.site/e2cf6ab4-e476-45f8-b143-21c0725e1dab";

        //        string reqBodyRaw = $@"{{
        //            ""operation"": ""insert"",
        //            ""ignore_empty"": true,
        //            ""callback"": {{
        //                ""url"": ""{callbackUrl}"",
        //                ""method"": ""post""
        //            }},
        //            ""resource"": [
        //                {{
        //                    ""type"": ""data"",
        //                    ""file_id"": ""{fileId}"",

        //                    ""module"": {{
        //                        ""api_name"": ""Leads""
        //                    }},
        //                    ""field_mappings"": [
        //                        {{
        //                            ""api_name"": ""Layout"",
        //                            ""default_value"": {{
        //                                ""value"": ""{layoutId}""
        //                            }}
        //                        }},

        //                        {{ ""api_name"": ""DB_Lead_ID"", ""index"": 0 }},
        //                        {{ ""api_name"": ""USDOT"", ""index"": 1 }},
        //                        {{ ""api_name"": ""Last_Name"", ""index"": 2 }},
        //                        {{ ""api_name"": ""State"", ""index"": 3 }},
        //                        {{ ""api_name"": ""Years_in_Business"", ""index"": 4 }},
        //                        {{ ""api_name"": ""Total_Vehicles"", ""index"": 5 }},
        //                        {{ ""api_name"": ""MVR_Violations"", ""index"": 6 }},
        //                        {{ ""api_name"": ""Radius_of_Operations"", ""index"": 7 }},
        //                        {{ ""api_name"": ""Current_Insurance_Company"", ""index"": 8 }},
        //                        {{ ""api_name"": ""Phone"", ""index"": 9 }},
        //                        {{ ""api_name"": ""Email"", ""index"": 10 }},
        //                        {{ ""api_name"": ""Company"", ""index"": 11 }},
        //                        {{ ""api_name"": ""Entity_Type"", ""index"": 12 }},
        //                        {{ ""api_name"": ""Cargo_Carried_Name"", ""index"": 13 }},
        //                        {{ ""api_name"": ""Operation_classification"", ""index"": 14 }},
        //                        {{ ""api_name"": ""Coverage_Required"", ""index"": 15 }},
        //                        {{ ""api_name"": ""Insurance_Lead_Type"", ""index"": 16 }},
        //                        {{ ""api_name"": ""Type_of_Trucking"", ""index"": 17 }},
        //                        {{ ""api_name"": ""Total_Drivers"", ""index"": 18 }},
        //                        {{ ""api_name"": ""Power_Units"", ""index"": 19 }}
        //                    ]
        //                }}
        //            ]
        //        }}";


        //        var reqBody = new StringContent(reqBodyRaw, encoding: System.Text.Encoding.UTF8, "application/json");

        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);
        //        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

        //        var resp = await client.PostAsync(apiUrl, reqBody);
        //        var test = resp.Content.ReadAsStringAsync().Result;

        //        if (resp.IsSuccessStatusCode)
        //        {
        //            var content = resp.Content.ReadAsStringAsync().Result;
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //log exceptions
        //    }

        //    return false;
        //}

        //public async Task<bool> CaptureCompletionResponse(object resp)
        //{
        //    bool status = false;
        //    try
        //    {
        //        var parsedJson = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(resp));
        //        string jobStatus = parsedJson.GetProperty("resource")[0].GetProperty("status").GetString();

        //        if (jobStatus != null && jobStatus.ToLower() == "completed")
        //        {

        //            string jobId = parsedJson.GetProperty("id").GetString();
        //            string uId = parsedJson.GetProperty("created_by").GetProperty("id").GetString();

        //            if (!string.IsNullOrEmpty(jobId) && !string.IsNullOrEmpty(uId))
        //            {
        //                AgencyZohoSecret secret = await _dbcontext.GetAgencySecret(uId, false);
        //                string token = await GetOAuthToken(secret.ZohoAccountURL, secret.ZohoCID, secret.ZohoKey, secret.ZohoRtk);

        //                var records = await GetRecordStatusFromZohoResponse(jobId, uId, token, secret.ZohoAPIDomain, secret.APIVersion);
        //                if (records.Count() > 0)
        //                {
        //                    int updateCount = 0;
        //                    foreach (var record in records)
        //                    {
        //                        if (record.STATUS.ToLower() == "added")
        //                        {
        //                            int updatedRow = await _dbcontext.AddZohoLeadId(secret.ClientId, record.DB_Lead_ID, record.RECORD_ID);
        //                            updateCount = updatedRow > 0 ? updateCount + 1 : updateCount;
        //                        }
        //                    }

        //                    if (updateCount == records.Count())
        //                    {
        //                        status = true;
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //log exceptions
        //    }

        //    return status;
        //}

        //private async Task<List<ZohoLeadEntity>> GetRecordStatusFromZohoResponse(string jobId, string uId, string token, string apiDomain, string apiVersion)
        //{

        //    List<ZohoLeadEntity> records = new List<ZohoLeadEntity>();

        //    try
        //    {

        //        HttpClient httpClient = new HttpClient();
        //        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);
        //        string jobDetailUrl = _config["ZohoAPIUrls:JobDetailUrl"].Replace("{api_domain}", apiDomain).Replace("{api_version}", apiVersion).Replace("{jobId}", jobId);

        //        var resp = await httpClient.GetAsync(jobDetailUrl);
        //        if (resp.IsSuccessStatusCode)
        //        {
        //            string content = await resp.Content.ReadAsStringAsync();
        //            var json = JsonSerializer.Deserialize<JsonElement>(content);

        //            string downloadUrl = json.GetProperty("result").GetProperty("download_url").GetString();

        //            if (!string.IsNullOrEmpty(downloadUrl))
        //            {

        //                var fileResp = await httpClient.GetAsync(downloadUrl);
        //                if (fileResp.IsSuccessStatusCode)
        //                {
        //                    byte[] filecontent = await fileResp.Content.ReadAsByteArrayAsync();
        //                    records = FileManager.ReadCSVFromZip(filecontent);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // log exceptions
        //    }

        //    return records;
        //}

        public async Task<bool> UpdateLeadStatus(ZohoLeadStatusData data)
        {
            bool isRowChanged = await _dbcontext.UpdateAgencyLeadStatus(data.ZohoLeadId, data.CallStatus, data.LeadStatus);


            return isRowChanged;
        }

        public async Task<Dictionary<int, string>> UploadLeadsToZohoCRM()
        {
            Dictionary<int, string> uploadStatus = new Dictionary<int, string>();

            var activeAgencyIds = await _dbcontext.GetActiveAgencyIds();
            if (activeAgencyIds.Count() > 0)
            {
                foreach (var agencyId in activeAgencyIds)
                {
                    uploadStatus.Add(agencyId, "Starting Upload process");
                    bool isUploadSuccess = await InsertRecordsForAgency(agencyId);
                    if (isUploadSuccess)
                    {
                        uploadStatus[agencyId] = "Leads Uploaded Successfully";
                    }
                }
            }

            return uploadStatus;
        }


        private async Task<bool> InsertRecordsForAgency(int agencyId)
        {
            bool isSuccess = false;
            _httpClient.DefaultRequestHeaders.Clear();

            try
            {
                DateTime currentDay = DateTime.Now;  // remove adddays(-1) before publish
                var dailyLeadsForAgencyId = await _dbcontext.GetDailyLeadForAgency(agencyId, currentDay);
                var agencySecret = await _dbcontext.GetAgencySecret(agencyId, true);

                RecordData recordData = new RecordData();

                recordData.Data = dailyLeadsForAgencyId.ToList();

                if (dailyLeadsForAgencyId.Count() > 0)
                {
                    for (int i = 0; i < recordData.Data.Count(); i++)
                    {
                        recordData.Data[i].Layout = new Layout() { Id = agencySecret.LeadsLayoutId };
                    }
                }
                else
                {
                    return false;
                }

                string jsonContent = System.Text.Json.JsonSerializer.Serialize(recordData);
                if (agencySecret != null)
                {
                    string token = await GetOAuthToken(agencySecret.ZohoAccountURL, agencySecret.ZohoCID, agencySecret.ZohoKey, agencySecret.ZohoRtk);


                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);
                    string apiUrl = _config["ZohoAPIUrls:InsertRecordUrl"].Replace("{api_domain}", agencySecret.ZohoAPIDomain).Replace("{api_version}", agencySecret.APIVersion);

                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var resp = await _httpClient.PostAsync(apiUrl, content);

                    var test = await resp.Content.ReadAsStringAsync();

                    if (resp.IsSuccessStatusCode)
                    {
                        isSuccess = true;

                        var respContent = await resp.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(respContent))
                        {
                            ApiResponse zohoResponse = JsonSerializer.Deserialize<ApiResponse>(respContent);
                            for (int i = 0; i < zohoResponse.Data.Count(); i++)
                            {
                                try
                                {
                                    if (zohoResponse.Data[i].Code.ToLower().Trim().Equals("success"))
                                    {
                                        await _dbcontext.AddZohoLeadId(agencyId, recordData.Data[i].DB_Lead_ID, zohoResponse.Data[i].Details.Id);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return isSuccess;
        }

        public async Task<string> SaveEmailResponse(string id, string response, string forAgency)
        {
            try
            {
                string decryptedResponse;

                bool isValidResponse = EmailResponseManager.ValidateResponse(response, _config["EncryptionData:ZOHOEmailClientResponseEncKEY"], _config["EncryptionData:ZOHOEmailClientResponseIV"], out decryptedResponse);

                if (isValidResponse && !string.IsNullOrEmpty(decryptedResponse))
                {
                    string decryptedLeadId = EmailResponseManager.DecodeEncText(id, _config["EncryptionData:LeadIdDecryptSalt"]);
                    string decryptedAgency = EmailResponseManager.DecodeEncText(forAgency, _config["EncryptionData:LeadIdDecryptSalt"]);

                    var agencySecret = await _dbcontext.GetAgencySecret(decryptedAgency, false);

                    if (agencySecret != null)
                    {

                        decryptedResponse = decryptedResponse.Replace('_', ' ').Replace("Email Response ", "Email Response-");
                        string updateParams = $@"{{
                         ""data"": [
                              {{
                                ""id"": ""{decryptedLeadId}"",
                                ""Lead_Status"": ""{decryptedResponse}""
                              }}
                            ]
                        }}";

                        var updateContent = new StringContent(updateParams, Encoding.UTF8, "application/json");
                        string updateUrl = _config["ZohoAPIUrls:InsertRecordUrl"].Replace("{api_domain}", agencySecret.ZohoAPIDomain).Replace("{api_version}", agencySecret.APIVersion);

                        string token = await GetOAuthToken(agencySecret.ZohoAccountURL, agencySecret.ZohoCID, agencySecret.ZohoKey, agencySecret.ZohoRtk);

                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);

                        var resp = await _httpClient.PutAsync(updateUrl, updateContent);

                        var respContent = await resp.Content.ReadAsStringAsync();
                        if (resp.IsSuccessStatusCode)
                        {
                            return UserFeedbackContent.GetFeedbackContent(decryptedResponse);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return UserFeedbackContent.GetFeedbackContent("Error");
            }

            return UserFeedbackContent.GetFeedbackContent("Error");
        }


        public async Task<bool> ParseAndSaveCallInfo(int agencyId, string callInfo)
        {
            bool isSaved = false;

            try
            {
                //interpret call summary
                JsonDocument jsonDocument = JsonDocument.Parse(callInfo);
                JsonElement root = jsonDocument.RootElement;

                string callSummary = root.GetProperty("call_information").GetProperty("summary").GetString().Trim();
                string apiUrl = _config["AIApiUrl"];
                var models = _config.GetSection("LLMModelIds").Get<List<string>>();

                PromptText callStatusPrompt = await _dbcontext.GetPromptText(1);
                callStatusPrompt.UserPrompt = callStatusPrompt.UserPrompt.Replace("{{call_summary}}", callSummary);

                string callStatus = string.Empty;

                foreach (var model in models)
                {
                    string callStatusResp = await CallDetailManager.GetPromptResult(callStatusPrompt.UserPrompt, callStatusPrompt.SystemPrompt, apiUrl, _config["AIApiKey"], models[0]);
                    try
                    {
                        if (!string.IsNullOrEmpty(callStatusResp))
                        {
                            JsonDocument respJsonDoc = JsonDocument.Parse(callStatusResp);
                            JsonElement respJsonRoot = respJsonDoc.RootElement;
                            callStatus = respJsonRoot.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString().Replace("{", "").Replace("}", "").Replace("\n", "").Trim();
                            break;
                        }
                        else
                        {
                            throw new Exception("invalid json from AI api respone");
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }



                CallDataPayload callDetail = CallDetailManager.GetCallDetail(root, callStatus);
                CallDetail dbCallInfo = new CallDetail()
                {
                    CallStatus = callDetail.Data[0].CallStatus,
                    CallSummary = callDetail.Data[0].CallSummary
                };

                //save to zoho
                AgencyZohoSecret secret = await _dbcontext.GetAgencySecret(agencyId, true);
                string token = await GetOAuthToken(secret.ZohoAccountURL, secret.ZohoCID, secret.ZohoKey, secret.ZohoRtk);
                string zohoCallDataAPIUrl = _config["ZohoAPIUrls:InsertRelatedRecord"].Replace("{api_domain}", secret.ZohoAPIDomain).Replace("{api_version}", secret.APIVersion).Replace("{custom_module_name}", "Gail_Calls");
                PostToZohoCRM(JsonSerializer.Serialize(callDetail), zohoCallDataAPIUrl, token);

                //update lead status on zoho - call completed

                string leadApiUrl = _config["ZohoAPIUrls:InsertRecordUrl"];
                leadApiUrl = leadApiUrl.Replace("{api_domain}", secret.ZohoAPIDomain).Replace("{api_version}", secret.APIVersion);

                await UpdateLeadStatusOnZoho(callDetail.Data[0].LeadContact.Id, "Call Completed", token, leadApiUrl);


                //parse and save quick quote information:
                JsonElement dataCollectTextKey;
                bool hasQuickQuoteData = root.GetProperty("call_information").TryGetProperty("data_collected_text", out dataCollectTextKey);

                if (hasQuickQuoteData)
                {
                    PromptText quickQuotePrompt = await _dbcontext.GetPromptText(2);
                    quickQuotePrompt.UserPrompt = quickQuotePrompt.UserPrompt.Replace("{{input_json}}", dataCollectTextKey.GetString());

                    QuickQuoteRoot quickQuoteDetail = null;
                    string quickQuoteText = string.Empty;

                    foreach (string model in models)
                    {
                        string quickQuoteInfo = await CallDetailManager.GetPromptResult(quickQuotePrompt.UserPrompt, quickQuotePrompt.SystemPrompt, apiUrl, _config["AIApiKey"], model);
                        try
                        {
                            if (!string.IsNullOrEmpty(quickQuoteInfo))
                            {
                                JsonDocument respJsonDoc = JsonDocument.Parse(quickQuoteInfo);
                                JsonElement respJsonRoot = respJsonDoc.RootElement;

                                quickQuoteText = respJsonRoot.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                                int firstIndex = quickQuoteText.IndexOf("{");
                                int lastIndex = quickQuoteText.LastIndexOf("}");

                                if (firstIndex != -1 && lastIndex != -1)
                                {
                                    quickQuoteText = quickQuoteText.Substring(firstIndex, lastIndex - firstIndex + 1);
                                    quickQuoteDetail = JsonSerializer.Deserialize<QuickQuoteRoot>(quickQuoteText);
                                    break;
                                }
                            }
                            else
                            {
                                throw new Exception("invalid json from AI api respone");
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }

                    if (quickQuoteDetail != null && quickQuoteDetail.QuickQuoteDetails != null)
                    {

                        //save call data and quickquotedetails to db

                        await _dbcontext.AddCallData(callDetail.Data[0].LeadContact.Id, dbCallInfo, quickQuoteDetail);


                        QuickQuoteDetailRoot quickQuoteDetailRoot = new QuickQuoteDetailRoot(quickQuoteDetail);

                        if (quickQuoteDetailRoot.data != null && quickQuoteDetailRoot.data.Count > 0)
                        {
                            quickQuoteDetailRoot.data[0].QQLead = new QQLead() { id = callDetail.Data[0].LeadContact.Id };
                            string quickQuoteAPIUrl = _config["ZohoAPIUrls:InsertRelatedRecord"].Replace("{api_domain}", secret.ZohoAPIDomain).Replace("{api_version}", secret.APIVersion).Replace("{custom_module_name}", "QuickQuotes");
                            PostToZohoCRM(JsonSerializer.Serialize(quickQuoteDetailRoot), quickQuoteAPIUrl, token);
                        }

                        isSaved = true;
                    }
                    else
                    {
                        //save only call details to db
                        await _dbcontext.AddCallData(callDetail.Data[0].LeadContact.Id, dbCallInfo, null);
                        isSaved = true;
                    }
                }
                else
                {
                    await _dbcontext.AddCallData(callDetail.Data[0].LeadContact.Id, dbCallInfo, null);
                    isSaved = true;
                }

            }
            catch (Exception ex)
            {

            }

            return isSaved;
        }

        private async void PostToZohoCRM(string json, string url, string token)
        {

            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);


                var resp = _httpClient.PostAsync(url, content).Result;

                string respContent = await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {

            }
        }


        public async Task<bool> CallCreateCampaignFlow(int agencyId, GailContact gailContact)
        {
            bool isFlowTriggered = false;

            AgencyZohoSecret secret = await _dbcontext.GetAgencySecret(agencyId, true);

            try
            {
                CampaignFlowData campaignFlowData = new CampaignFlowData();
                campaignFlowData.Contact = gailContact;
                campaignFlowData.GailUserId = secret.GAILUserId;
                campaignFlowData.GailPwd = secret.GAILPwd;
                campaignFlowData.ForAgency = agencyId;

                var content = new StringContent(JsonSerializer.Serialize(campaignFlowData), Encoding.UTF8, "application/json");

                string flowUrl = _config["RPAFlowURL"];
                var resp = await _httpClient.PostAsync(flowUrl, content);
                string respContent = await resp.Content.ReadAsStringAsync();
                if (resp.IsSuccessStatusCode)
                {
                    isFlowTriggered = true;
                }
            }
            catch (Exception ex)
            {

            }

            return isFlowTriggered;

        }

        public async Task<bool> CreateCampaignFlowCompleted(int agencyId, string leadId, bool isCampaignCreated)
        {
            AgencyZohoSecret secret = await _dbcontext.GetAgencySecret(agencyId, true);

            bool isUpdated = false;

            if (isCampaignCreated == true)
            {
                _httpClient.DefaultRequestHeaders.Clear();
                string token = await GetOAuthToken(secret.ZohoAccountURL, secret.ZohoCID, secret.ZohoKey, secret.ZohoRtk);
                string apiUrl = _config["ZohoAPIUrls:InsertRecordUrl"];
                apiUrl = apiUrl.Replace("{api_domain}", secret.ZohoAPIDomain).Replace("{api_version}", secret.APIVersion);

                isUpdated = await UpdateLeadStatusOnZoho(leadId, "Call Campaign Created", token, apiUrl);
            }

            return isUpdated;
        }

        private async Task<bool> UpdateLeadStatusOnZoho(string leadId, string status, string token, string url)
        {
            try
            {
                string jsonBodyforZoho = $@"{{
                    ""data"" : [
                         {{
                             ""id"":""{leadId}"",
                             ""Lead_Status"":""{status}""
                         }}
                    ]
                }}";


                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", token);

                var apiContent = new StringContent(jsonBodyforZoho, Encoding.UTF8, "application/json");
                var apiResp = _httpClient.PutAsync(url, apiContent).Result;

                if (apiResp.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }

        }

    }
}
