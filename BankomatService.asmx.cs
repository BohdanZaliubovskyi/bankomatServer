using BankomatServer.Models;
using BankomatServer.Models.DataClasses;
using BankomatServer.Models.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;

namespace BankomatServer
{
    /// <summary>
    /// Summary description for BankomatService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BankomatService : WebService
    {
        /// <summary>
        /// проверка в базе номера карты
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        [WebMethod(Description = "Получить объект карты по номеру карты, параметры: string cardNumber")]
        [XmlInclude(typeof(Cards))]
        public BaseCard CheckCardNumber(string cardNumber)
        {
            return CardGateway.Instance.GetCardByNumber(cardNumber).GetBaseObj();
        }

        /// <summary>
        /// получение всех карт одного пользователя
        /// </summary>
        /// <param name="clientId">идентификатор клиента, для которого выбираем банковские карты</param>
        /// <returns></returns>
        [WebMethod(Description = "Получить массив банковских карт для клиента: long clientId")]
        [XmlInclude(typeof(Cards))]
        public List<BaseCard> GetCardsByClientId(long clientId)
        {
            List<Cards> cl = CardGateway.Instance.GetCardsByClientId(clientId);
            List<BaseCard> bcl = null;

            if (cl != null && cl.Count > 0)
            {
                bcl = new List<BaseCard>();
                foreach (Cards c in cl)
                    bcl.Add(c.GetBaseObj());
            }
            else
                return null;

            return bcl;
        }

        [WebMethod(Description = "Получить на сервере номер телефона для клиента, по которому надо перезвонить")]
        public bool SetPhoneNumber(string phoneNumber, long clientId)
        {
            bool ph = PhoneGateway.Instance.GetItemByPhoneNumberAndClientId(phoneNumber, clientId);
            if (!ph)
                PhoneGateway.Instance.Create(new Phones() { ClientId = clientId, Number = phoneNumber });

            // генерация части телефона, с которого будет звонок клиенту
            long rez = new Random().Next(1000, 9999);

            try
            {
                // удалить старые ключи, если таковые остались по каким-либо причинам
                ConfirmKeysGateway.Instance.DelItemsByClientId(clientId);
                // занести в таблицу временных ключей и запустить таймер для удаления
                ConfirmKeys ck = new ConfirmKeys() { ClientId = clientId, PhoneKey = rez };
                ConfirmKeysGateway.Instance.Create(ck);
                PhoneKeyManager pkm = new PhoneKeyManager(ck);

                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod(Description = "Проверить, получил ли клиент звонок на телефон, и правильно ли он ввел последние цифры")]
        public bool CheckPhoneKey(long clientId, long key)
        {
            ConfirmKeys ck = ConfirmKeysGateway.Instance.GetItemByClientIdAndConfirmKey(clientId, key);
            if (ck != null)
            {
                ConfirmKeysGateway.Instance.Delete(ck);
                return true;
            }

            return false;
            //return ck == null ? false : true;
        }

        [WebMethod(Description = "Получение номера телефона, с которого звонит банк для подтверждения пользователя у банкомата")]
        public string GetBankPhoneNumber(long clientId)
        {
            ConfirmKeys ck = ConfirmKeysGateway.Instance.GetItemByClientId(clientId);
            if (ck == null)
                return "";

            return $"{Constants.BankNumber}{ck.PhoneKey}";
        }

        [WebMethod]
        public Phones TestPhones(string phone)
        {
            return new Phones();
        }
        [WebMethod]
        public CardsStatuses GetGardStatus()
        {
            return CardsStatuses.BlackCard;
        }

        /// <summary>
        /// зачисление денег на карту с банкомата
        /// </summary>
        /// <param name="cardNumber">номер карты</param>
        /// <param name="sum">деньги</param>
        /// <returns>успешен ли результат</returns>
        [WebMethod(Description = "зачисление денег на карту с банкомата, параметры: string cardNumber, double sum")]
        public bool PutMoneyOnTheCard(string cardNumber, double sum)
        {
            try
            {
                Cards c = CardGateway.Instance.GetCardByNumber(cardNumber);
                c.Balance += sum;
                CardGateway.Instance.Update(c);
                // запомним совершенную транзакцию
                TransactionGateway.Instance.Create(new Transactions() { CardId = c.Id, Date = DateTime.Now, Form = (long)TransactionForm.Adding, Sum = sum });
            }
            catch
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// получение клиента по идентификатору
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [WebMethod(Description = "получение клиента по идентификатору")]
        public Clients GetClientById(long clientId)
        {
            return ClientGateway.Instance.GetItemById(clientId);
        }

        /// <summary>
        /// проверка совпадения пина на карте
        /// </summary>
        /// <param name="cardNumber">номер карты</param>
        /// <param name="pin">пин</param>
        /// <returns></returns>
        [WebMethod(Description = "проверка совпадения пина на карте")]
        public BaseCard CheckCardPin(string cardNumber, long pin)
        {
            Cards c = CardGateway.Instance.GetCardByNumberAndPin(cardNumber, pin);
            if (c == null)
                return null;

            return c.GetBaseObj();
        }

        [WebMethod(Description = "Получить деньги с карты, параметры: string cardNumber, double gettingSum")]
        public string GetMoneyFromCard(string cardNumber, long pin, double gettingSum)
        {
            Cards card = null;
            string message = CardGateway.Instance.GetMoneyCardCheck(cardNumber, pin, gettingSum, ref card);
            if (message != "")
                return message;

            // списать деньги, отправить в банкомат сообщение
            card.Balance -= gettingSum;
            try
            {
                CardGateway.Instance.Update(card);
                // запомним совершенную транзакцию
                TransactionGateway.Instance.Create(new Transactions() { CardId = card.Id, Date = DateTime.Now, Form = (long)TransactionForm.Subtraction, Sum = gettingSum });
                return $"С вашего счета было успешно снято {gettingSum} грн.";
            }
            catch
            {
                return "Произошла ошибка при списании средств со счета. Операция не была произведена.";
            }
        }

        [WebMethod(Description = "Передача денег на другую карту, параметры: string cardNumber, double gettingSum, string toCardNumber")]
        public string SendMoneyToOtherCard(string cardNumber, long pin, double gettingSum, string toCardNumber)
        {
            Cards card = null;
            string message = CardGateway.Instance.GetMoneyCardCheck(cardNumber, pin, gettingSum, ref card);
            if (message != "")
                return message;

            // списать деньги
            card.Balance -= gettingSum;

            Cards toCard;
            message = CardGateway.Instance.BaseCardCheck(toCardNumber, out toCard);
            if (message != "")
                return message;

            toCard.Balance += gettingSum;
            try
            {
                CardGateway.Instance.SendMoney(card, toCard, gettingSum);
                return "Передача денег прошла успешно";
            }
            catch
            {
                return "Произошла ошибка при передаче средств с картой отправителя. Операция не была произведена.";
            }
        }

        [WebMethod(Description = "Изменить пин-код карты, параметры: string cardNumber, double gettingSum")]
        public string ChangeCardPin(string cardNumber, long oldPin, long newPin)
        {
            Cards card;
            string message = CardGateway.Instance.BaseCardCheck(cardNumber, out card);
            if (message != "")
                return message;

            if (card.Pin != oldPin)
                return "Неверный пин.";

            card.Pin = newPin;
            try
            {
                CardGateway.Instance.Update(card);
                return "Пин-код был успешно поменян.";
            }
            catch
            {
                return "Не удалось изменить пин-код.";
            }
        }

        [WebMethod(Description = "Заблокировать карту, параметры: string cardNumber, long pin")]
        public string BlockCard(string cardNumber, long pin)
        {
            Cards card;
            string message = CardGateway.Instance.BaseCardCheck(cardNumber, out card);
            if (message != "")
                return message;

            if (card.Pin != pin)
                return "Неверный пин.";

            card.IsBlocked = true;
            try
            {
                CardGateway.Instance.Update(card);
                return "Карта была заблокирована.";
            }
            catch
            {
                return "Не удалось заблокировать карту.";
            }
        }

        [WebMethod(Description = "Получить лимиты по картам")]
        [XmlInclude(typeof(CardsLimits))]
        public BaseCardsLimits GetCardsLimits()
        {
            return new CardsLimits().GetBaseObj();
        }

        [WebMethod(Description = "Получить новую карту для клиента, параметры: long clientId, CardsStatuses cardsStatuses")]
        public string GetNewCard(long clientId, CardsStatuses cardsStatuses)
        {
            long cs;
            try
            {
                cs = Convert.ToInt64(cardsStatuses);
            }
            catch
            {
                return "Ошибка статуса карты";
            }
            if (clientId > 0)
            {
                int pin = 1111; // стандартный пинкод для новой карты
                try
                {
                    CardGateway.Instance.Create(new Cards() { Balance = 0, CardNumber = CardGateway.Instance.GenerateCardNumber(), CardStatus = cs, ClientId = clientId, DateOfEndUsing = DateTime.Now.AddYears(5), IsBlocked = false, Pin = pin });
                    return $"Поздравляем! Новая карта успешно добавлена в ваш кошелек. Пинкод {pin}.";
                }
                catch
                {
                    return "Не удалось добавить новую карту";
                }
            }
            else
                return "Ошибка идентификатора клиента.";

        }

    }
}
