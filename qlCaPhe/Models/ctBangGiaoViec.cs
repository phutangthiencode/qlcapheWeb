//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace qlCaPhe.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ctBangGiaoViec
    {
        public int maBang { get; set; }
        public int maCa { get; set; }
        public string ghiChu { get; set; }
    
        public virtual BangGiaoViec BangGiaoViec { get; set; }
        public virtual caLamViec caLamViec { get; set; }
    }
}
