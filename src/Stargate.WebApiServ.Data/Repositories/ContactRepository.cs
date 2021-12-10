using System.Collections.Concurrent;

namespace Stargate.WebApiServ.Data.Repositories;

public class ContactRepository : IContactRepository
{
    private static readonly ConcurrentDictionary<string, Contact> _contacts = new();

    public ContactRepository()
    {
        Add(new Contact { FirstName = "Nancy", LastName = "Davolio" });
    }

    public void Add(Contact contact)
    {
        contact.ID = Guid.NewGuid().ToString();
        _contacts[contact.ID] = contact;
    }

    public Contact? Get(string id)
    {
        _contacts.TryGetValue(id, out var contact);
        return contact;
    }

    public IEnumerable<Contact> GetAll()
    {
        return _contacts.Values;
    }

    public Contact? Remove(string id)
    {
        _contacts.TryRemove(id, out var contact);
        return contact;
    }

    public void Update(Contact contact)
    {
        _contacts[contact.ID] = contact;
    }
}
