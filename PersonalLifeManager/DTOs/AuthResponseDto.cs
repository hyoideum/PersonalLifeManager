namespace PersonalLifeManager.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    
}

public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}