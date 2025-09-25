using System.Collections.Concurrent;

namespace Shop.Domain;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _store = new();
    public Product? Get(Guid id) => _store.TryGetValue(id, out var p) ? p : null;
    public IEnumerable<Product> GetAll() => _store.Values;
    public void Add(Product p) => _store[p.Id] = p;
}
