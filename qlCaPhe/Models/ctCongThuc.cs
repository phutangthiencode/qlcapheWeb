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
    
    public partial class ctCongThuc
    {
        public int maChiTiet { get; set; }
        public int maCongThuc { get; set; }
        public int buocSo { get; set; }
        public Nullable<int> maNguyenLieu { get; set; }
        public Nullable<double> soLuongNguyenLieu { get; set; }
        public string donViSuDung { get; set; }
        public string hanhDong { get; set; }
        public string ghiChu { get; set; }
    
        public virtual congThucPhaChe congThucPhaChe { get; set; }
        public virtual nguyenLieu nguyenLieu { get; set; }
    }
}