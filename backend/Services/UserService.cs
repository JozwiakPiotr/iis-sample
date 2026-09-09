using System.Collections.Concurrent;
using backend.Models;

namespace backend.Services;

public class UserService
{
    private readonly ConcurrentDictionary<Guid, User> _store = new();

    public IEnumerable<User> GetAll() => _store.Values.OrderBy(u => u.Name);

    public User Add(string name)
    {
        var u = new User { Id = Guid.NewGuid(), Name = name };
        _store[u.Id] = u;
        return u;
    }

    public bool Remove(Guid id) => _store.TryRemove(id, out _);
}
