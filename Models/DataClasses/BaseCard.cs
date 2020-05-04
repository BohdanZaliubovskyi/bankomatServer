using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace BankomatServer.Models.DataClasses
{
    /// <summary>
    /// базовый класс карты, вынужденный костыль для передачи данных через веб сервис
    /// </summary>
    [Serializable, XmlInclude(typeof(Cards))]
    public abstract class BaseCard : BaseObjClass<BaseCard>
    {
        public long Id { get; set; }
        public string CardNumber { get; set; }
        public long Pin { get; set; }
        public long CardStatus { get; set; }
        public bool IsBlocked { get; set; }
        public System.DateTime DateOfEndUsing { get; set; }
        public long ClientId { get; set; }
        public double Balance { get; set; }              
    }
}