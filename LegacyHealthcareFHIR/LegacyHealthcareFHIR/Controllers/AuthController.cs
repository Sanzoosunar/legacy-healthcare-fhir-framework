using System.Security.Claims;
using System.Text;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly AuthService _authService;

    public AuthController(JwtTokenService tokenService, AuthService authService)
    {
        _jwtTokenService = tokenService;
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        LoginResponseDto responseDto;
        try
        {
            var user = await _authService.Login(request.Username, request.Password);
            responseDto = new LoginResponseDto
            {
                Success = true,
                Token = _jwtTokenService.GenerateToken(user),
            };

        }
        catch (Exception ex)
        {
            responseDto = new LoginResponseDto
            {
                Success = false,
                Token = string.Empty,
                ErrorMessage = ex.Message
            };
        }
        return Ok(responseDto);
    }
}

public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public bool Success { get; set; } = false;
    public string Token { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}