using BankomatServer.Models;
using BankomatServer.Models.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

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
    public class BankomatService : System.Web.Services.WebService
    {
        /// <summary>
        /// проверка в базе номера карты
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        [WebMethod(Description = "Получить объект карты по номеру карты, параметры: string cardNumber")]
        public Cards CheckCardNumber(string cardNumber)
        {
            return CardGateway.Instance.GetCardByNumber(cardNumber);
        }

    }
}
