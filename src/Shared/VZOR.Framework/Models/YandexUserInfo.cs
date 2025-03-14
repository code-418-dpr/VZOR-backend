using Newtonsoft.Json;

namespace VZOR.Framework.Models;

public class YandexUserInfo
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("login")]
    public string Login { get; set; }

    [JsonProperty("emails")]
    public List<string> Emails { get; set; }

    [JsonProperty("default_email")]
    public string DefaultEmail { get; set; }
}