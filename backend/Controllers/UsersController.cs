using Microsoft.AspNetCore.Mvc;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly Services.UserService _users;
    public UsersController(Services.UserService users) => _users = users;

    [HttpGet]
    public IEnumerable<User> Get() => _users.GetAll();

    [HttpPost]
    public ActionResult<User> Create([FromBody] UserCreateDto dto)
    {
        var user = _users.Add(dto.Name);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var ok = _users.Remove(id);
        return ok ? NoContent() : NotFound();
    }
}

public record UserCreateDto(string Name);
