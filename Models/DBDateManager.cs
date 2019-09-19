using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankomatServer.Models
{
    /// <summary>
    /// класс для преобразования дат
    /// </summary>
    public static class DBDateManager
    {
        static DateTime _dateTime1970;
        static DBDateManager()
        {
            _dateTime1970 = new DateTime(1970, 1, 1);
        }
        /// <summary>
        /// получить дату в секундах для корректного взаимодействия с БД
        /// </summary>
        /// <param name="dt">нужная дата</param>
        /// <returns>количество секунд без 1970 года</returns>
        public static long GetDateInSeconds(DateTime dt)
        {
            return (Int64)(dt.Subtract(_dateTime1970)).TotalSeconds;
        }
    }
}