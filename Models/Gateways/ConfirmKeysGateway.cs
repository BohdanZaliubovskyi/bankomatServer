using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.Gateways
{
    /// <summary>
    /// слой доступа к таблице кодов подтверждения
    /// </summary>
    class ConfirmKeysGateway : BaseSingleton<ConfirmKeysGateway>, IBaseGateway<ConfirmKeys>
    {
        public void Create(ConfirmKeys item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.ConfirmKeys.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(ConfirmKeys item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.ConfirmKeys.Attach(item);
                db.ConfirmKeys.Remove(item);
                db.SaveChanges();
            }
        }

        public List<ConfirmKeys> GetAll()
        {
            List<ConfirmKeys> confirmKeys = null;

            using (var db = new mainEntities())
            {
                confirmKeys = db.ConfirmKeys.ToList();
            }

            return confirmKeys;
        }

        public ConfirmKeys GetItemById(long id)
        {
            ConfirmKeys cc = null;

            using (var db = new mainEntities())
            {
                var transactions = from c in db.ConfirmKeys
                                   where c.Id == id
                                   select c;
                if (transactions != null)
                    cc = transactions.SingleOrDefault();
            }

            return cc;
        }

        public void Update(ConfirmKeys item)
        {
            using (var db = new mainEntities())
            {
                db.ConfirmKeys.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// получить ключ доступа для клиента
        /// </summary>
        /// <param name="clientId">идентификатор клиента</param>
        /// <returns></returns>
        public ConfirmKeys GetItemByClientId(long clientId)
        {
            ConfirmKeys cc = null;

            using (var db = new mainEntities())
            {
                var transactions = from c in db.ConfirmKeys
                                   where c.ClientId == clientId
                                   select c;
                if (transactions != null)
                    cc = transactions.SingleOrDefault();
            }

            return cc;
        }

        /// <summary>
        /// удаление старых ключей для идентификации, если таковые имеются
        /// </summary>
        /// <param name="clientId"></param>
        public void DelItemsByClientId(long clientId)
        {
            List<ConfirmKeys> ckl;
            using (var db = new mainEntities())
            {
                var transactions = from c in db.ConfirmKeys
                                   where c.ClientId == clientId
                                   select c;

                ckl = transactions.ToList();

                if (ckl != null && ckl.Count > 0)
                {
                    foreach (ConfirmKeys ck in ckl)
                    {
                        db.ConfirmKeys.Attach(ck);
                        db.ConfirmKeys.Remove(ck);
                    }
                    db.SaveChanges();
                }               
            }
        }
        /// <summary>
        /// получить ключ доступа для клиента
        /// </summary>
        /// <param name="clientId">идентификатор клиента</param>
        /// <returns></returns>
        public ConfirmKeys GetItemByClientIdAndConfirmKey(long clientId, long key)
        {
            ConfirmKeys cc = null;

            using (var db = new mainEntities())
            {
                var transactions = from c in db.ConfirmKeys
                                   where c.ClientId == clientId && c.PhoneKey == key
                                   select c;
                if (transactions != null)
                    cc = transactions.SingleOrDefault();
            }

            return cc;
        }

    }
}