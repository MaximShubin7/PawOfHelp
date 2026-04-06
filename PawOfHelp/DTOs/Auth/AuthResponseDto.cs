namespace PawOfHelp.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
