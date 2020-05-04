using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankomatServer.Models.DataClasses
{
    /// <summary>
    /// класс для включения функционала отдачи базового объекта для передачи через сериализатор
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseObjClass<T>
    {
        public abstract T GetBaseObj();
    }
}