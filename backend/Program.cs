using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Host as a Windows Service and use HttpSys (Windows-only)
builder.Host.UseWindowsService();
builder.WebHost.UseHttpSys(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.AddControllers();
builder.Services.AddSingleton<backend.Services.UserService>();

var app = builder.Build();

app.MapControllers();

app.Run();
