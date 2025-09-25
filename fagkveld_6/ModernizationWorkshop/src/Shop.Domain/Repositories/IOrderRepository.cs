namespace Shop.Domain;

public interface IOrderRepository
{
    Order? Get(Guid id);
    IEnumerable<Order> GetAll();
    void Add(Order o);
    void Update(Order o);
}
