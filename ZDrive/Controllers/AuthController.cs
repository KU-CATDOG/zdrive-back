using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using ZDrive.Data;
using ZDrive.Models;
using ZDrive.Services;

namespace ZDrive.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ZDriveDbContext _context;
    private readonly IAuthorizationManager _auth;
    private readonly ISessionStorage _session;

    public AuthController(ZDriveDbContext context, IAuthorizationManager auth, ISessionStorage session)
    {
        _context = context;
        _auth = auth;
        _session = session;
    }

    [Route("test")]
    [HttpGet]
    public string Test()
    {
        return GeneratePasswordHash("password", "realtest");
    }

    [Route("login")]
    [HttpPost]
    public IResult Login(Login login)
    {
        var _user = _context.Users.FirstOrDefault(u => u.StudentNumber == login.StudentNumber);
        if (_user == null) return Results.NotFound();
        if (!_user.IsVerified) return Results.Forbid();
        if (GeneratePasswordHash(login.Password, _user.Salt) != _user.PasswordHash) return Results.NotFound();

        _session.AddSession(_user.Id, out var ssid);

        Response.Cookies.Append("sessionId", ssid.ToString(), new CookieOptions
        {
            SameSite = SameSiteMode.None,
            Secure = true,
            HttpOnly = true
        });

        return Results.Ok();
    }

    [Route("logout")]
    [HttpGet]
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
    public async Task<IResult> Register(Registration reg)
    {
        var check = await (from user in _context.Users
                           where user.StudentNumber == reg.StudentNumber
                           select user).FirstOrDefaultAsync();

        if (check != null) return Results.Conflict();

        var checkStudentNum = await (from user in _context.Users
                           where user.StudentNumber == reg.StudentNumber
                           select user).FirstOrDefaultAsync();

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

        return Results.Created($"/auth/register/{newUser.Id}", newUser);
    }

    [Route("remove")]
    [HttpDelete]
    public async Task<IResult> Remove(Login login)
    {
        var _user = await (from u in _context.Users
                           where u.StudentNumber == login.StudentNumber
                           select u).FirstOrDefaultAsync();

        if (_user == null) return Results.NotFound();
        if (_user.PasswordHash != GeneratePasswordHash(login.Password, _user.Salt)) return Results.NotFound();

        _session.RemoveUser(_user.Id);
        _context.Users.Remove(_user);
        await _context.SaveChangesAsync();

        return Results.Ok();
    }

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
