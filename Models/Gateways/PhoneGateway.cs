using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.Gateways
{
    /// <summary>
    /// слой доступа к таблице телефонов
    /// </summary>
    class PhoneGateway : BaseSingleton<PhoneGateway>, IBaseGateway<Phones>
    {
        public void Create(Phones item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.Phones.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(Phones item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.Phones.Attach(item);
                db.Phones.Remove(item);
                db.SaveChanges();
            }
        }

        public List<Phones> GetAll()
        {
            List<Phones> phones = null;

            using (var db = new mainEntities())
            {
                phones = db.Phones.ToList();
            }

            return phones;
        }

        public Phones GetItemById(long id)
        {
            Phones ph = null;

            using (var db = new mainEntities())
            {
                var transactions = from p in db.Phones
                                   where p.Id == id
                                   select p;
                //if (transactions != null)
                    ph = transactions.SingleOrDefault();
            }

            return ph;
        }

        public void Update(Phones item)
        {
            using (var db = new mainEntities())
            {
                db.Phones.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// получить объект телефона по номеру телефона и клиенту
        /// </summary>
        /// <param name="phoneNumber">номер телефона</param>
        /// <param name="clientId">ид клиента</param>
        /// <returns>true=такая комбинация существует false=такой комбинации не существует</returns>
        public bool GetItemByPhoneNumberAndClientId(string phoneNumber, long clientId)
        {
            Phones ph = null;

            using (var db = new mainEntities())
            {
                var transactions = from p in db.Phones
                                   where p.Number == phoneNumber && p.ClientId == clientId
                                   select p;
                //if (transactions != null)
                    ph = transactions.SingleOrDefault();
            }
            bool rez = ph==null? false :  true;
            return rez;
            
        }
    }
}