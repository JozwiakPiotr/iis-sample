using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
var ocelotFileName = $"ocelot.{builder.Environment.EnvironmentName}.json";
builder.Configuration.AddJsonFile(ocelotFileName, optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Ocelot must be awaited
await app.UseOcelot();

app.Run();
