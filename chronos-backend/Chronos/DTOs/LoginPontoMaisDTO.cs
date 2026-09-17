using System.Text.Json.Serialization;

namespace Chronos.DTOs
{
    public class LoginPontoMaisDTO
    {
        [JsonPropertyName("success")]
        public string? Success { get; set; }

        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("client_id")]
        public string? ClientId { get; set; }

        [JsonPropertyName("data")]
        public LoginDataDTO? Data { get; set; } 
    }

    public class LoginDataDTO
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("sign_in_count")]
        public int SignInCount { get; set; }

        [JsonPropertyName("last_sign_in_ip")]
        public string? LastSignInIp { get; set; }

        [JsonPropertyName("last_sign_in_at")]
        public long LastSignInAt { get; set; }
    }
}