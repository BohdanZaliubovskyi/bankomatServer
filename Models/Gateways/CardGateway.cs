using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.Gateways
{
    /// <summary>
    /// слой доступа к таблице карточекы
    /// </summary>
    class CardGateway : BaseSingleton<CardGateway>, IBaseGateway<Cards>
    {
        public static bool TestFunc()
        {
            return true;
        }
        public void Create(Cards item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.Cards.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(Cards item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.Entry(item).Collection(c => c.Transactions).Load();

                db.Cards.Attach(item);
                db.Cards.Remove(item);
                db.SaveChanges();
            }
        }

        public List<Cards> GetAll()
        {
            List<Cards> cards = null;

            using (var db = new mainEntities())
            {
                cards = db.Cards.ToList();
            }

            return cards;
        }

        public Cards GetItemById(int id)
        {
            Cards cr = null;

            using (var db = new mainEntities())
            {
                var transactions = from c in db.Cards
                                   where c.Id == id
                                   select c;
                if (transactions != null)
                    cr = transactions.SingleOrDefault();
            }

            return cr;
        }

        public void Update(Cards item)
        {
            using (var db = new mainEntities())
            {
                db.Cards.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// получить все карты клиента
        /// </summary>
        /// <param name="clientId">идентификатор клиента</param>
        /// <returns>список карт</returns>
        public List<Cards> GetCardsByClientId(long clientId)
        {
            List<Cards> cardsList = null;

            using (var db = new mainEntities())
            {
                var transactions = from c in db.Cards
                                   where c.ClientId == clientId
                                   select c;
                if (transactions != null)
                    cardsList = transactions.ToList();
            }

            return cardsList;
        }

        /// <summary>
        /// получить карту по номеру
        /// </summary>
        /// <param name="cardNumber">номер искомой карты</param>
        /// <returns></returns>
        public Cards GetCardByNumber(string cardNumber)
        {
            Cards cr = null;

            using (var db = new mainEntities())
            {
                var transactions = from c in db.Cards
                                   where c.CardNumber == cardNumber
                                   select c;
                if (transactions != null)
                    cr = transactions.SingleOrDefault();
            }

            return cr;
        }
    }
}