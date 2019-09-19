using BankomatServer.Models.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BankomatServer.Models
{
    /// <summary>
    /// виды карт
    /// </summary>
    public enum CardStatus
    {
        /// <summary>
        /// обычная
        /// </summary>
        Base = 1,
        /// <summary>
        /// золотая
        /// </summary>
        Gold,
        /// <summary>
        /// вип
        /// </summary>
        Vip,
    }
    /// <summary>
    /// помощник проверки операций с картой
    /// </summary>
    public static class CardStatusChecker
    {
        /// <summary>
        /// лимит базовая карта, наличка
        /// </summary>
        static readonly int _baseCashLimit = 50000;
        /// <summary>
        /// лимит базовая карта, безнал
        /// </summary>
        static readonly int _baseCashLessLimit = 100000;
        /// <summary>
        /// лимит золотая карта, наличка
        /// </summary>
        static readonly int _goldCashLimit = 200000;
        /// <summary>
        /// лимит золотая карта, безнал
        /// </summary>
        static readonly int _goldCashLessLimit = 400000;

        /// <summary>
        /// проверка, возможна ли следующая транзакция с картой
        /// </summary>
        /// <param name="transactionsSum">итоговая сумма транзакций(суммарная совершонная + желаемая для совершения)</param>
        /// <param name="cardStatus">вид карты</param>
        /// <param name="transactionForm">вид операции со счетом карты</param>
        /// <returns>false=желаемая транзакция невозможна, true=желаемая транзакция возможна</returns>
        public static bool CheckCardLimit(long transactionsSum, CardStatus cardStatus, TransactionForm transactionForm)
        {
            bool rez = false;

            switch(cardStatus)
            {
                case CardStatus.Vip:
                    rez = true;
                    break;
                case CardStatus.Base:
                    switch (transactionForm)
                    {
                        case TransactionForm.SubtractionCash:
                            if (transactionsSum <= _baseCashLimit)
                                rez = true;
                            break;
                        case TransactionForm.SubtractionCashless:
                            if (transactionsSum <= _baseCashLessLimit)
                                rez = true;
                            break;
                    }
                    break;
                case CardStatus.Gold:
                    switch (transactionForm)
                    {
                        case TransactionForm.SubtractionCash:
                            if (transactionsSum <= _goldCashLimit)
                                rez = true;
                            break;
                        case TransactionForm.SubtractionCashless:
                            if (transactionsSum <= _goldCashLessLimit)
                                rez = true;
                            break;
                    }
                    break;
            }

            return rez;
        }

    }
}