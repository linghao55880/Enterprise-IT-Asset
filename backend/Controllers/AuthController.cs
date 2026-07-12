using Microsoft.AspNetCore.Mvc;
using ITAssetManagement.Models;
using ITAssetManagement.Services;

namespace ITAssetManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly MockDataService _data;

    public AuthController(MockDataService data)
    {
        _data = data;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _data.Users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null || request.Password != "123456")
        {
            return Unauthorized(new { message = "用户名或密码错误" });
        }

        user.LastLogin = DateTime.Now;
        _data.AddAuditLog(user.Id, user.DisplayName, "登录", "系统", null, "成功", GetClientIp(), "用户登录系统");

        var userCopy = new User
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Department = user.Department,
            IsActive = user.IsActive,
            LastLogin = user.LastLogin,
            CreatedAt = user.CreatedAt
        };

        return Ok(new LoginResponse { Token = $"mock-token-{user.Id}", User = userCopy });
    }

    [HttpGet("me")]
    public IActionResult Me([FromQuery] string username)
    {
        var user = _data.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return NotFound(new { message = "用户不存在" });
        }

        return Ok(new User
        {
            Id = user.Id,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Department = user.Department,
            IsActive = user.IsActive,
            LastLogin = user.LastLogin,
            CreatedAt = user.CreatedAt
        });
    }

    private string? GetClientIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
