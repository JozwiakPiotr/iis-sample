using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
var users = new ConcurrentDictionary<Guid, User>();

app.MapGet("/api/users", () => users.Values.OrderBy(user => user.Name));

app.MapPost("/api/users", (UserCreateDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name))
    {
        return Results.BadRequest("Name is required.");
    }

    var user = new User(Guid.NewGuid(), dto.Name.Trim());
    users[user.Id] = user;
    return Results.Created($"/api/users/{user.Id}", user);
});

app.MapDelete("/api/users/{id:guid}", (Guid id) =>
{
    var ok = users.TryRemove(id, out _);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.Run();

public record User(Guid Id, string Name);

public record UserCreateDto(string Name);
