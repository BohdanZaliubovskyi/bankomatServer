using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace BankomatServer.Models.DataClasses
{
    [Serializable, XmlInclude(typeof(CardsLimits))]
    public abstract class BaseCardsLimits : BaseObjClass<BaseCardsLimits>
    {
        /// <summary>
        /// лимит черная карта
        /// </summary>
        public int BlackCardLimit { get; set; }
        /// <summary>
        /// лимит синяя карта
        /// </summary>
        public int BlueCardLimit { get; set; }
        /// <summary>
        /// лимит золотая карта
        /// </summary>
        public int GoldCardLimit { get; set; }
    }
}