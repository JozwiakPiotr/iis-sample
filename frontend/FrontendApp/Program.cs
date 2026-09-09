using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseStaticFiles();
app.MapGet("/", () => Results.Redirect("/index.html"));

// Ocelot must be awaited
await app.UseOcelot();

app.Run();
