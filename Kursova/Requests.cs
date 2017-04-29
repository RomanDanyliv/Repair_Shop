//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kursova
{
    using System;
    using System.Collections.Generic;
    
    public partial class Requests
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Requests()
        {
            this.Need_Parts = new HashSet<Need_Parts>();
        }
    
        public int ID_Request { get; set; }
        public string Serial { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> Date_In { get; set; }
        public Nullable<System.DateTime> Date_Out { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> ID_Address { get; set; }
        public int ID_Customer { get; set; }
        public int ID_Employee { get; set; }
        public int ID_Branch { get; set; }
    
        public virtual Addresses Addresses { get; set; }
        public virtual Branches Branches { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employees Employees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Need_Parts> Need_Parts { get; set; }
    }
}