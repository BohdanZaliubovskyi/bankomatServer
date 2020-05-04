using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.DataClasses
{
    /// <summary>
    /// вид/статус карты
    /// </summary>
    //public static enum CardsStatuses
    //{
    //    BlackCard = 1,
    //    BlueCard = 2,
    //    GoldCard = 3,
    //}
    public static class CardStatus
    {
        static readonly List<CardsStatuses> CardsStatuses;
        static CardStatus()
        {
            CardsStatuses = new List<CardsStatuses>();
            //CardsStatuses.Add(DataClasses.CardsStatuses.BlackCard);
            //CardsStatuses.Add(DataClasses.CardsStatuses.BlueCard);
            //CardsStatuses.Add(DataClasses.CardsStatuses.GoldCard);
        }
    }
}