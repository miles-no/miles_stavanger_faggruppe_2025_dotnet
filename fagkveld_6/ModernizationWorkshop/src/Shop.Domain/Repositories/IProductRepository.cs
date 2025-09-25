namespace Shop.Domain;

public interface IProductRepository
{
    Product? Get(Guid id);
    IEnumerable<Product> GetAll();
    void Add(Product p);
}
