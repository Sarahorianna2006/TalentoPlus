namespace Application.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
}