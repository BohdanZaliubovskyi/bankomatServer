using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.DataClasses
{
    /// <summary>
    /// класс для передачи лимитов по карте
    /// </summary>
    [Serializable]
    public class CardsLimits : BaseCardsLimits
    {        
        public CardsLimits()
        {
            BlackCardLimit = 5000;
            BlueCardLimit = 10000;
            GoldCardLimit = -1;
        }

        public override BaseCardsLimits GetBaseObj()
        {
            BaseCardsLimits bcl = new CardsLimits();
            bcl.BlackCardLimit = BlackCardLimit;
            bcl.BlueCardLimit = BlueCardLimit;
            bcl.GoldCardLimit = GoldCardLimit;

            return bcl;
        }
    }
}