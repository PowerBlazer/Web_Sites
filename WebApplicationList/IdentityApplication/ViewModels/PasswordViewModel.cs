using Newtonsoft.Json;

namespace WebApplicationList.IdentityApplication.ViewModels
{
    public class PasswordViewModel
    {
        [JsonProperty("OldPassword")]
        public string? OldPassword { get; set; }
        [JsonProperty("NewPassword")]
        public string? NewPassword { get; set; }
    }
}
