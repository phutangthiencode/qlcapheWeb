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
    
    public partial class nhatKy
    {
        public string tenDangNhap { get; set; }
        public Nullable<System.DateTime> thoiDiem { get; set; }
        public string IP { get; set; }
        public string trinhDuyet { get; set; }
        public string OS { get; set; }
        public string chucNang { get; set; }
    
        public virtual taiKhoan taiKhoan { get; set; }
    }
}