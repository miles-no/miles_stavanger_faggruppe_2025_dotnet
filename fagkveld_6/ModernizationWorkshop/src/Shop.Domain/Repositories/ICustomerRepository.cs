namespace Shop.Domain;

public interface ICustomerRepository
{
    Customer? Get(Guid id);
    IEnumerable<Customer> GetAll();
    void Add(Customer c);
}
