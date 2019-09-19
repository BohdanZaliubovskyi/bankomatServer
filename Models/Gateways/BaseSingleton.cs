using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.Gateways
{
    // Database.Connection.ConnectionString = "Data Source=" + System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/BankomatDB.db") + ";Version=3;";
    /// <summary>
    /// паттерн синглтон для гетвеев
    /// </summary>
    /// <typeparam name="T">обязательно должен иметь конструктор без параметров</typeparam>
    abstract class BaseSingleton<T> where T : new()
    {
        static T _instance;

        public static T Instance
        {
            get {
                if (_instance == null)
                    _instance = new T();
                return _instance;
            }
        }
    }
}