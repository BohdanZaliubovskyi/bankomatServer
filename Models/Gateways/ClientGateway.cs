using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.Gateways
{
    /// <summary>
    /// слой доступа к таблице клиентов
    /// </summary>
    class ClientGateway : BaseSingleton<ClientGateway>, IBaseGateway<Clients>
    {
        public void Create(Clients item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.Clients.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(Clients item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                //db.Entry(item).Collection(c => c.Cards).Load();
                //db.Entry(item).Collection(c => c.ConfirmKeys).Load();
                //db.Entry(item).Collection(c => c.Phones).Load();

                db.Clients.Attach(item);
                db.Clients.Remove(item);
                db.SaveChanges();
            }
        }

        public List<Clients> GetAll()
        {
            List<Clients> clients = null;

            using (var db = new mainEntities())
            {
                clients = db.Clients.ToList();
            }

            return clients;
        }

        public Clients GetItemById(long id)
        {
            Clients cl = null;

            using (var db = new mainEntities())
            {
                var transactions = from c in db.Clients
                                   where c.Id == id
                                   select c;
                if (transactions != null)
                    cl = transactions.SingleOrDefault();
            }

            return cl;
        }

        public void Update(Clients item)
        {
            using (var db = new mainEntities())
            {
                db.Clients.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}