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
    
    public partial class ctPhieuNhapKho
    {
        public int maPhieu { get; set; }
        public int maNguyenLieu { get; set; }
        public int maNhaCC { get; set; }
        public Nullable<double> soLuongNhap { get; set; }
        public long donGiaNhap { get; set; }
        public string ghiChu { get; set; }
    
        public virtual nguyenLieu nguyenLieu { get; set; }
        public virtual nhaCungCap nhaCungCap { get; set; }
        public virtual phieuNhapKho phieuNhapKho { get; set; }
    }
}