//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BankomatServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class ConfirmKeys
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public long PhoneKey { get; set; }
    
        public virtual Clients Clients { get; set; }
    }
}