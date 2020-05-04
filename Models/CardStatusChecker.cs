using BankomatServer.Models.DataClasses;
using BankomatServer.Models.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BankomatServer.Models
{
    /// <summary>
    /// помощник проверки операций с картой
    /// </summary>
    public static class CardStatusChecker
    {
        /// <summary>
        /// проверка, возможна ли следующая транзакция с картой
        /// </summary>
        /// <param name="transactionsSum">итоговая сумма транзакций(суммарная совершонная + желаемая для совершения)</param>
        /// <param name="cardStatus">вид карты</param>
        /// <param name="transactionForm">вид операции со счетом карты</param>
        /// <returns>false=желаемая транзакция невозможна, true=желаемая транзакция возможна</returns>
        public static bool CheckCardLimit(double transactionsSum, CardsStatuses cardStatus, TransactionForm transactionForm)
        {
            bool rez = false;
            CardsLimits cl = new CardsLimits();

            switch(cardStatus)
            {
                case CardsStatuses.GoldCard:                    
                    rez = IsInLimit(cl.GoldCardLimit, transactionsSum);
                    break;
                case CardsStatuses.BlackCard:
                    rez = IsInLimit(cl.BlackCardLimit, transactionsSum);
                    break;
                case CardsStatuses.BlueCard:
                    rez = IsInLimit(cl.BlueCardLimit, transactionsSum);
                    break;
            }

            return rez;
        }

        /// <summary>
        /// проверка входит ли сумма транзакций в лимит
        /// </summary>
        /// <param name="limit">заданный лимит</param>
        /// <param name="transactionSum">сумма транзакций</param>
        /// <returns>true=следующая транзакция одобрена, false=выход за лимит, невозможно совершить следующую транзакцию</returns>
        static bool IsInLimit(int limit, double transactionSum)
        {
            if (limit < 0)
                return true;
            else
            {
                if (transactionSum < limit)
                    return true;
            }

            return false;
        }

    }
}