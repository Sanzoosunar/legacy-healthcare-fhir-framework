using LegacyHealthcareFHIR.Core.Models;
using LegacyHealthcareFHIR.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly AuthService _authService;
    private readonly CurrentUser _currentUser;

    public AuthController(
        AuthService authService,
        JwtTokenService jwtTokenService,
        CurrentUser currentUser)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _currentUser = currentUser;
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

    [Authorize]
    [HttpGet("me")]
    public ActionResult<CurrentUserDto> GetCurrentUser()
    {
        return Ok(new CurrentUserDto
        {
            UserId = _currentUser.UserId,
            HospitalId = _currentUser.HospitalId,
            Username = _currentUser.Username,
            HospitalName = _currentUser.HospitalName
        });
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

public class CurrentUserDto
{
    public int UserId { get; set; }
    public int HospitalId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string HospitalName { get; set; } = string.Empty;
}