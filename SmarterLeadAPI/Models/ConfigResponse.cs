using Newtonsoft.Json;

public class ConfigResponse
{
    [JsonProperty("publishableKey")]
    public string PublishableKey { get; set; }

    [JsonProperty("proPrice")]
    public string ProPrice { get; set; }

    [JsonProperty("goldPrice")]
    public string GoldPrice { get; set; }

    [JsonProperty("silverPrice")]
    public string SilverPrice { get; set; }

    [JsonProperty("basicPrice")]
    public string BasicPrice { get; set; }
}
