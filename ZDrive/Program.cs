using System.Net;
using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var configFileLocation = builder.Configuration.GetValue<string>("ConfigLocation") ?? throw new InvalidOperationException("Config file not found.");
builder.Services.AddDbContext<ZDriveDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddSingleton<ISessionStorage, SessionStorage>();
builder.Services.AddScoped<ConfigProvider>(p => new ConfigProvider(configFileLocation));

builder.Services.AddAuthentication()
    .AddScheme<SessionTokenAuthenticationSchemeOptions, SessionTokenAuthenticationSchemeHandler>(
        "SessionTokens",
        opts => { }
    );

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.PermanentRedirect;
        options.HttpsPort = 443;
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("CORS: AllowAnyOrigin");
    app.UseCors(options =>
    {
        options.WithOrigins("https://mer.kucatdog.net", "http://localhost:3001", "http://localhost:3332");
        options.AllowAnyHeader();
        options.AllowAnyMethod();
        options.AllowCredentials();
    });

    app.UseStatusCodePages();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers().RequireAuthorization();
app.Run();
