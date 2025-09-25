using System.Collections.Concurrent;

namespace Shop.Domain;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _store = new();
    public Order? Get(Guid id) => _store.TryGetValue(id, out var o) ? o : null;
    public IEnumerable<Order> GetAll() => _store.Values.OrderByDescending(o => o.CreatedUtc);
    public void Add(Order o) => _store[o.Id] = o;
    public void Update(Order o) => _store[o.Id] = o;
}
