using System;

namespace backend.Models;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
