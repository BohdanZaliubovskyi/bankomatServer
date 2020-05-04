using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using BankomatServer.Models.Gateways;

namespace BankomatServer.Models
{
    /// <summary>
    /// класс для управления временными ключами для подтверждения телефонов
    /// </summary>
    public class PhoneKeyManager
    {
        public PhoneKeyManager(ConfirmKeys item)
        {
            Timer timer = new Timer(DeletePhoneKey, item, 1000*60*3, Timeout.Infinite);
        }
        void DeletePhoneKey(object data)
        {
            ConfirmKeys item = data as ConfirmKeys;
            // возможно объект был уже удален, проверим
            item = ConfirmKeysGateway.Instance.GetItemById(item.Id);
            if (item != null)
                ConfirmKeysGateway.Instance.Delete(item);
        }
    }
}