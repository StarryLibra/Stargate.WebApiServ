using System.Collections.Generic;
using Stargate.WebApiServ.Data.Models;

namespace Stargate.WebApiServ.Data.Repositories
{
    public interface IContactRepository
    {
        void Add(Contact contact);
        IEnumerable<Contact> GetAll();
        Contact Get(string key);
        Contact Remove(string key);
        void Update(Contact contact);
    }
}