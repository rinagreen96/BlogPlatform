using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IClientManager : IDisposable
    {
        void Create(ClientProfile item);
        void Update(ClientProfile item_old);
        void Delete(string id);
        ClientProfile FindById(string id);
        ClientProfile Get(string login);
    }
}
