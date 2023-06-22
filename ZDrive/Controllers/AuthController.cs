using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Models;
using ZDrive.Services;

namespace ZDrive.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ZDriveDbContext _context;
    private readonly ISessionStorage _session;

    public AuthController(ZDriveDbContext context, ISessionStorage session)
    {
        _context = context;
        _session = session;
    }

    [Route("check")]
    [HttpGet]
    public async Task<IResult> Test()
    {
        var sid = User.FindFirstValue(ClaimTypes.Sid);
        if (sid == null) return Results.Unauthorized();

        var _user = await _context.Users.FindAsync(int.Parse(sid));
        if (_user == null) return Results.NotFound();

        var userData = UserData.User(_user);
        return Results.Ok(userData);
    }

    [Route("login")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IResult> Login(Login login)
    {
        var _user = await FindUserByStdNumAsync(login.StudentNumber);
        if (_user == null) return Results.NotFound();
        if (GeneratePasswordHash(login.Password, _user.Salt) != _user.PasswordHash) return Results.NotFound();
        if (!_user.IsVerified) return Results.Forbid();

        if (_session.AddSession(UserData.User(_user), out var ssid))
        {
            Response.Cookies.Append("sessionId", ssid.ToString(), new CookieOptions
            {
                SameSite = SameSiteMode.None, // 프로덕션 환경에서는 Lax로 설정해야함
                Secure = true,
                HttpOnly = true
            });
        }

        var userData = UserData.User(_user);

        return Results.Ok(userData);
    }

    [Route("logout")]
    [HttpGet]
    [AllowAnonymous]
    public IResult Logout()
    {
        var ssid = Request.Cookies["sessionId"];
        if (ssid == null) return Results.NotFound();
        if (!Guid.TryParse(ssid, out var parse)) return Results.BadRequest();

        try
        {
            _session.RemoveUser(parse);
            Response.Cookies.Delete("sessionId");
            return Results.Ok();
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine(e);
            return Results.NotFound();
        }
    }

    [Route("register")]
    [HttpPost]
    [AllowAnonymous]
    public async Task<IResult> Register(Registration reg)
    {
        var checkUser = await FindUserByStdNumAsync(reg.StudentNumber);
        if (checkUser != null) return Results.Conflict();

        var checkStudentNum =
            await _context.StudentNums.FirstOrDefaultAsync(s => s.StudentNumber == reg.StudentNumber);
        if (checkStudentNum == null)
        {
            var newStNum = new StudentNum
            {
                StudentNumber = reg.StudentNumber,
                Name = reg.Name
            };
            await _context.StudentNums.AddAsync(newStNum);
        }

        var salt = GenerateToken(32);
        var newUser = new User
        {
            Name = reg.Name,
            StudentNumber = reg.StudentNumber,
            PasswordHash = GeneratePasswordHash(reg.Password, salt),
            Salt = salt
        };

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        var userData = UserData.User(newUser);

        return Results.Created($"/auth/register/{newUser.Id}", userData);
    }

    [Route("remove")]
    [HttpDelete]
    public async Task<IResult> Remove(Login login)
    {
        var _user = await FindUserByStdNumAsync(login.StudentNumber);
        if (_user == null) return Results.NotFound();
        if (_user.PasswordHash != GeneratePasswordHash(login.Password, _user.Salt))
            return Results.NotFound();

        _session.RemoveUser(UserData.User(_user));
        _context.Users.Remove(_user);
        await _context.SaveChangesAsync();

        return Results.Ok();
    }

    private Task<User?> FindUserByStdNumAsync(string studentNum)
        => _context.Users.FirstOrDefaultAsync(u => u.StudentNumber == studentNum);

    private string GenerateToken(int size)
    {
        Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, size)
        .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string GeneratePasswordHash(string text, string salt)
    {
        var ret = text + salt;

        for (int i = 0; i < 5; i++)
        {
            ret = GetSHA256(ret);
        }

        return ret;
    }

    private string GetSHA256(string text)
    {
        using (SHA256 sha = SHA256.Create())
        {
            var origin = Encoding.UTF8.GetBytes(text);
            var hash = sha.ComputeHash(origin);
            return Convert.ToBase64String(hash);
        }
    }
}
