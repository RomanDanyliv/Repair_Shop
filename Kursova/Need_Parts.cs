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
    
    public partial class Need_Parts
    {
        public int ID_Request { get; set; }
        public int ID_Part { get; set; }
        public Nullable<int> Used_Parts { get; set; }
    
        public virtual Spare_parts Spare_parts { get; set; }
        public virtual Requests Requests { get; set; }
    }
}
