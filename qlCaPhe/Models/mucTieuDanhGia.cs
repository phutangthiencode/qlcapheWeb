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
    
    public partial class mucTieuDanhGia
    {
        public mucTieuDanhGia()
        {
            this.ctDanhGias = new HashSet<ctDanhGia>();
        }
    
        public int maMucTieu { get; set; }
        public string tenMucTieu { get; set; }
        public string dienGiai { get; set; }
        public Nullable<bool> trangThai { get; set; }
        public string ghiChu { get; set; }
    
        public virtual ICollection<ctDanhGia> ctDanhGias { get; set; }
    }
}
