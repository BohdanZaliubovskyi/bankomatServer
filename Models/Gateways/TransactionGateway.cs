using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.Gateways
{
    /// <summary>
    /// вид совершонной транзакции
    /// </summary>
    public enum TransactionForm
    {
        /// <summary>
        /// добавление денег на счет
        /// </summary>
        Adding = 1,
        /// <summary>
        /// снятие денег со счета, наличка
        /// </summary>
        SubtractionCash,
        /// <summary>
        /// снятие денег со счета, безнал
        /// </summary>
        SubtractionCashless,
    }
    class TransactionGateway : BaseSingleton<TransactionGateway>, IBaseGateway<Transactions>
    {
        public void Create(Transactions item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.Transactions.Add(item);
                db.SaveChanges();
            }
        }

        public void Delete(Transactions item)
        {
            if (item == null)
                return;

            using (var db = new mainEntities())
            {
                db.Transactions.Attach(item);
                db.Transactions.Remove(item);
                db.SaveChanges();
            }
        }

        public List<Transactions> GetAll()
        {
            List<Transactions> transactions = null;

            using (var db = new mainEntities())
            {
                transactions = db.Transactions.ToList();
            }

            return transactions;
        }

        public Transactions GetItemById(int id)
        {
            Transactions tr = null;

            using (var db = new mainEntities())
            {
                var transactions = from t in db.Transactions
                                   where t.Id == id
                                   select t;
                if (transactions != null)
                    tr = transactions.SingleOrDefault();
            }

            return tr;
        }

        public void Update(Transactions item)
        {
            using (var db = new mainEntities())
            {
                db.Transactions.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }       

        /// <summary>
        /// получить список транзакций по критериям
        /// </summary>
        /// <param name="transactionForm">вид транзакции</param>
        /// <param name="transactionDay">день транзакции</param>
        /// <param name="cardID">идентификатор </param>
        /// <returns></returns>
        public List<Transactions> GetTransactionsByFormAndDayAndCardId(TransactionForm transactionForm, DateTime transactionDay, int cardID)
        {
            DateTime sDate = new DateTime(transactionDay.Year, transactionDay.Month, transactionDay.Day, 0, 0, 0);
            DateTime eDate = new DateTime(transactionDay.Year, transactionDay.Month, transactionDay.Day, 23, 59, 59);
            //long lsDate = DBDateManager.GetDateInSeconds(sDate);
            //long leDate = DBDateManager.GetDateInSeconds(eDate);

            int form = (int)transactionForm;

            List<Transactions> transactionsList = null;

            using (var db = new mainEntities())
            {
                var transactions = from t in db.Transactions
                               where t.CardId == cardID && t.Form == form && t.Date <= eDate && t.Date >= sDate
                               select t;
                if (transactions != null)
                    transactionsList = transactions.ToList();
            }

            return transactionsList;
        }
    }
}