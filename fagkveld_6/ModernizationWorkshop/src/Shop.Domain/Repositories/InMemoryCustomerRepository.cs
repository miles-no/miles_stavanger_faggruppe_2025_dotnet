using System.Collections.Concurrent;

namespace Shop.Domain;

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly ConcurrentDictionary<Guid, Customer> _store = new();
    public Customer? Get(Guid id) => _store.TryGetValue(id, out var c) ? c : null;
    public IEnumerable<Customer> GetAll() => _store.Values;
    public void Add(Customer c) => _store[c.Id] = c;
}
