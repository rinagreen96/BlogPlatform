using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using System.Data.Entity;

namespace DAL.Repositories
{
    public class ClientManager : IClientManager
    {
        public DataBaseContext Database { get; set; }
        public ClientManager(DataBaseContext db)
        {
            Database = db;
        }

        public void Create(ClientProfile item)
        {
            Database.ClientProfiles.Add(item);
            Database.SaveChanges();
        }

        public void Update(ClientProfile cp1)
        {
            Database.Entry(cp1).State = EntityState.Modified;
        }

        public ClientProfile Get(string login)
        {
            return Database.ClientProfiles.FirstOrDefault(x=>x.ApplicationUser.Email==login);
        }

        public ClientProfile FindById(string id)
        {
            return Database.ClientProfiles.FirstOrDefault(x=>x.ApplicationUser.Id==id);
        }

        public void Delete(string id)
        {
            ClientProfile clientProfile = Database.ClientProfiles.FirstOrDefault(x=>x.ApplicationUser.Id==id);
            if (clientProfile != null)
                Database.ClientProfiles.Remove(clientProfile);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
