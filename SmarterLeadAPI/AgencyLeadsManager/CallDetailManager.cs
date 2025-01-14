using SmarterLead.API.AgencyLeadsManager.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmarterLead.API.AgencyLeadsManager
{
    public class PromptText
    {
        public string SystemPrompt { get; set; }
        public string UserPrompt { get; set; }
    }
    public class CallDetailManager
    {
        public static CallDetail ParseCallDetail(string jsonString)
        {
            CallDetail callDetail = new CallDetail();

            return callDetail;
        }

        public static async Task<string> GetPromptResult(string userPrompt, string systemPrompt, string apiUrl, string key, string model)
        {


            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", key);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };

                    AIApiPayload apiPayload = new AIApiPayload()
                    {
                        Model = model,
                        Messages = new List<AIApiMessage>()
                        {
                            new AIApiMessage()
                            {
                                Role = "system",
                                Content = systemPrompt
                            },

                            new AIApiMessage()
                            {
                                Role = "user",
                                Content = userPrompt
                            }
                        }
                    };

                    string jsonString = JsonSerializer.Serialize(apiPayload, options);
                    var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

                    var resp = await client.PostAsync(apiUrl, content);

                    if (resp.IsSuccessStatusCode)
                    {
                        string respContent = await resp.Content.ReadAsStringAsync();
                        int firstIndex = respContent.IndexOf('{');
                        int lastIndex = respContent.LastIndexOf('}');
                        if (firstIndex != -1 && lastIndex != -1)
                        {
                            respContent = respContent.Substring(firstIndex, lastIndex - firstIndex + 1);
                            return respContent;
                        }
                    }

                }
                catch (Exception ex)
                {

                }

                return string.Empty;
            }
        }

        public static CallDataPayload GetCallDetail(JsonElement rootElement, string callStatus)
        {
            CallDataPayload callDataPayload = new CallDataPayload();
            callDataPayload.Data = new List<CallData>();

            CallData callDetails = new CallData();

            try
            {
                string name = rootElement.GetProperty("call_information").GetProperty("name").GetString() ?? string.Empty;
                string zohoLeadId = ExtractLeadIdFromText(name);
                callDetails.LeadContact = new LeadContact() { Id = zohoLeadId };

                callDetails.Id = rootElement.GetProperty("id").GetString() ?? string.Empty;
                callDetails.CallStatus = callStatus ?? string.Empty;
                callDetails.CallDirection = rootElement.GetProperty("direction").GetString() ?? string.Empty;
                callDetails.FromNumber = rootElement.GetProperty("from_number").GetString() ?? string.Empty;
                callDetails.ToNumber = rootElement.GetProperty("to_number").GetString() ?? string.Empty;
                callDetails.CallDuration = rootElement.GetProperty("duration").GetInt32().ToString() ?? string.Empty;
                callDetails.CalleeName = rootElement.GetProperty("call_information").GetProperty("name").GetString() ?? string.Empty;
                callDetails.CallNote = rootElement.GetProperty("call_information").GetProperty("note").GetString() ?? string.Empty;
                callDetails.CallCategory = rootElement.GetProperty("call_information").GetProperty("category_of_call").GetString() ?? string.Empty;
                callDetails.WentToVoiceMail = rootElement.GetProperty("call_information").GetProperty("voicemail").GetString() ?? string.Empty;
                callDetails.CallReason = rootElement.GetProperty("call_information").GetProperty("reason_of_call").GetString() ?? string.Empty;

                callDetails.CallRecording = rootElement.GetProperty("recording_url").GetString() ?? string.Empty;
                callDetails.CallBackTime = rootElement.GetProperty("callback_start_time").GetString() ?? string.Empty;
                callDetails.CallSummary = rootElement.GetProperty("call_information").GetProperty("summary").GetString() ?? string.Empty;

                callDetails.CallBackRequested = rootElement.GetProperty("call_information").GetProperty("call_back_requested").GetString() ?? string.Empty;





            }
            catch (Exception ex)
            {
                try
                {
                    callDetails.CallBackRequested = rootElement.GetProperty("call_information").GetProperty("call_back").GetProperty("requested").GetString() ?? string.Empty;
                }
                catch (Exception ex2)
                {

                }

            }
            finally
            {
                callDataPayload.Data.Add(callDetails);
            }



            return callDataPayload;
        }

        private static string ExtractLeadIdFromText(string text)
        {
            string leadIdregexPattern = @"(?<=\s)\d{10,}(?=\s|$)";
            string leadId = System.Text.RegularExpressions.Regex.Match(text.Trim(), leadIdregexPattern).Value;
            return leadId;
        }

    }

    public class CallDataPayload
    {
        [JsonPropertyName("data")]
        public List<CallData> Data { get; set; }
    }

    public class CallData
    {
        [JsonPropertyName("Name")]
        public string Id { get; set; }

        [JsonPropertyName("Call_Status")]
        public string CallStatus { get; set; }

        [JsonPropertyName("Call_Direction")]
        public string CallDirection { get; set; }

        [JsonPropertyName("From_Number")]
        public string FromNumber { get; set; }

        [JsonPropertyName("To_Number")]
        public string ToNumber { get; set; }

        [JsonPropertyName("Call_Duration")]
        public string CallDuration { get; set; }

        [JsonPropertyName("Callee_Name")]
        public string CalleeName { get; set; }

        [JsonPropertyName("Call_Note")]
        public string CallNote { get; set; }

        [JsonPropertyName("Call_Category")]
        public string CallCategory { get; set; }

        [JsonPropertyName("Went_to_Voicemail")]
        public string WentToVoiceMail { get; set; }

        [JsonPropertyName("Call_Reason")]
        public string CallReason { get; set; }

        [JsonPropertyName("Call_Back_Requested")]
        public string CallBackRequested { get; set; }

        [JsonPropertyName("Call_Recording")]
        public string CallRecording { get; set; }

        [JsonPropertyName("Call_Back_Time")]
        public string CallBackTime { get; set; }

        [JsonPropertyName("Call_Summary")]
        public string CallSummary { get; set; }

        [JsonPropertyName("Lead_Contact")]
        public LeadContact LeadContact { get; set; }
    }

    public class LeadContact
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    class AIApiMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    class AIApiPayload
    {
        public string Model { get; set; }
        public List<AIApiMessage> Messages { get; set; }
    }

}
