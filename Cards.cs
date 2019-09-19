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
    using System.Xml.Serialization;

    public partial class Cards
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cards()
        {
            this.Transactions = new HashSet<Transactions>();
        }
    
        public long Id { get; set; }
        public string CardNumber { get; set; }
        public long Pin { get; set; }
        public long CardStatus { get; set; }
        public bool IsBlocked { get; set; }
        public System.DateTime DateOfEndUsing { get; set; }
        public long ClientId { get; set; }
    
        public virtual Clients Clients { get; set; }
        [XmlIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transactions> Transactions { get; set; }
    }
}
