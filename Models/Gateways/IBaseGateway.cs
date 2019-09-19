using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankomatServer.Models.Gateways
{
    /// <summary>
    /// общие функции для гетвеев
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface IBaseGateway<T>
    {
        /// <summary>
        /// создание объекта в БД
        /// </summary>
        /// <param name="item">объект</param>
        void Create(T item);
        /// <summary>
        /// получение всех объектов из БД
        /// </summary>
        /// <returns></returns>
        List<T> GetAll();
        /// <summary>
        /// обновление объекта в БД
        /// </summary>
        /// <param name="item">объект</param>
        void Update(T item);
        /// <summary>
        /// удаление объекта из БД
        /// </summary>
        /// <param name="item">объект</param>
        void Delete(T item);
        /// <summary>
        /// получить объект по идентификатору из БД
        /// </summary>
        /// <param name="id">идентификат</param>
        /// <returns></returns>
        T GetItemById(int id);
    }
}
