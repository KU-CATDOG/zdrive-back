using Microsoft.EntityFrameworkCore;
using ZDrive.Data;
using ZDrive.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ZDriveDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddSingleton<ISessionStorage, SessionStorage>();
builder.Services.AddTransient<IAuthorizationManager, AuthorizationManager>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
