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
    
    public partial class nhomTaiKhoan
    {
        public nhomTaiKhoan()
        {
            this.taiKhoans = new HashSet<taiKhoan>();
        }
    
        public int maNhomTK { get; set; }
        public string tenNhom { get; set; }
        public string dienGiai { get; set; }
        public string quyenHan { get; set; }
        public string trangMacDinh { get; set; }
        public string ghiChu { get; set; }
    
        public virtual ICollection<taiKhoan> taiKhoans { get; set; }
    }
}
