using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.Gateways
{
    /// <summary>
    /// слой доступа к таблице карточек
    /// </summary>
    class CardGateway : BaseSingleton<CardGateway>, IBaseGateway<Cards>
    {
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
                //db.Entry(item).Collection(c => c.Transactions).Load();

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

        public Cards GetItemById(long id)
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

        /// <summary>
        /// получить карту по номеру и пин коду
        /// </summary>
        /// <param name="cardNumber">номер искомой карты</param>
        /// <param name="pin">пин код</param>
        /// <returns></returns>
        public Cards GetCardByNumberAndPin(string cardNumber, long pin)
        {
            Cards cr = null;

            using (var db = new mainEntities())
            {
                var transactions = from c in db.Cards
                                   where c.CardNumber == cardNumber &&
                                   c.Pin == pin
                                   select c;
                if (transactions != null)
                    cr = transactions.SingleOrDefault();
            }

            return cr;
        }

        /// <summary>
        /// базовая проверка карты: на блокировку, на валидность номера
        /// </summary>
        /// <param name="cardNumber">номер карты</param>
        /// <param name="card">карта из БД</param>
        /// <returns>сообщение состояния</returns>
        public string BaseCardCheck(string cardNumber, out Cards card)
        {
            card = null;

            if (cardNumber == "")
                return "Не указан номер карты.";

            card = CardGateway.Instance.GetCardByNumber(cardNumber);
            if (card == null)
                return "Не найдено карты по такому номеру";

            // проверить не заблокирована ли карта
            if (card.IsBlocked == true)
                return "Ваша карта заблокирована. Любые операции с ней запрещены.";            

            return "";
        }

        /// <summary>
        /// проверка карты для списания денег
        /// </summary>
        /// <param name="cardNumber">номер карты</param>
        /// <param name="pin">пин</param>
        /// <param name="gettingSum">сумма для списания</param>
        /// <param name="card">объект для передачи данных дальше</param>
        /// <returns>сообщение состояния</returns>
        public string GetMoneyCardCheck(string cardNumber, long pin, double gettingSum, ref Cards card)
        {
            string message = BaseCardCheck(cardNumber, out card);
            if (message != "")
                return message;

            if (card.Pin != pin)
                return "Неверный пин.";

            // проверить лимит по карте через транзакции
            List<Transactions> tL = TransactionGateway.Instance.GetTransactionsByFormAndDayAndCardId(TransactionForm.Subtraction, DateTime.Now, card.Id);
            double sum2 = tL.Sum(t => t.Sum);

            // проверить хватит ли денег на счету карты
            if (card.Balance < gettingSum)
                return "Не хватает средств на карте.";

            bool rez = CardStatusChecker.CheckCardLimit(sum2 + gettingSum, (CardsStatuses)card.CardStatus, TransactionForm.Subtraction);
            if (!rez) // превышение лимита
                return "Списать средства невозможно. Превышение суточного лимита снятия средств.";            

            return "";
        }

        /// <summary>
        /// передача денег с карты на карту
        /// </summary>
        /// <param name="fromCard">отправитель</param>
        /// <param name="toCard">получатель</param>
        /// <param name="sum">сумма передачи</param>
        public void SendMoney(Cards fromCard, Cards toCard, double sum)
        {
            using (var db = new mainEntities())
            {
                using (db.Database.BeginTransaction())
                {
                    db.Cards.Attach(fromCard);
                    db.Entry(fromCard).State = EntityState.Modified;

                    db.Cards.Attach(toCard);
                    db.Entry(toCard).State = EntityState.Modified;

                    db.Transactions.Add(new Transactions() { CardId=fromCard.Id, Date = DateTime.Now, Form = (long)TransactionForm.Subtraction, Sum = sum });
                    db.Transactions.Add(new Transactions() { CardId=toCard.Id, Date = DateTime.Now, Form = (long)TransactionForm.Adding, Sum = sum });
                }
                db.SaveChanges();
            }
        }

        /// <summary>
        /// генерация случайного номера карты
        /// </summary>
        /// <returns></returns>
        public string GenerateCardNumber()
        {
            bool flag = false;
            string number;
            do
            {
                number = "";
                for (int i = 0; i < 16; ++i)
                    number = $"{number}{new Random().Next(0, 9)}";

                Cards c = GetCardByNumber(number);
                if (c != null)
                    flag = true;
            } while (flag);

            return number;
        }
    }
}