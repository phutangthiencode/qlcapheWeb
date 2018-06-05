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
    
    public partial class sanPham
    {
        public sanPham()
        {
            this.congThucPhaChes = new HashSet<congThucPhaChe>();
            this.ctHoaDonOrders = new HashSet<ctHoaDonOrder>();
            this.ctHoaDonTams = new HashSet<ctHoaDonTam>();
            this.lichSuGias = new HashSet<lichSuGia>();
        }
    
        public int maSanPham { get; set; }
        public string tenSanPham { get; set; }
        public int maLoai { get; set; }
        public string moTa { get; set; }
        public long donGia { get; set; }
        public byte[] hinhAnh { get; set; }
        public Nullable<int> thoiGianPhaChe { get; set; }
        public int trangThai { get; set; }
        public string ghiChu { get; set; }
    
        public virtual ICollection<congThucPhaChe> congThucPhaChes { get; set; }
        public virtual ICollection<ctHoaDonOrder> ctHoaDonOrders { get; set; }
        public virtual ICollection<ctHoaDonTam> ctHoaDonTams { get; set; }
        public virtual ICollection<lichSuGia> lichSuGias { get; set; }
        public virtual loaiSanPham loaiSanPham { get; set; }
    }

    /// <summary>
    /// Class d�nh cho web services
    /// </summary>
    public class svSanPham
    {
        public int maSanPham { get; set; }
        public string tenSanPham { get; set; }
        public int maLoai { get; set; }
        public string moTa { get; set; }
        public long donGia { get; set; }
        public byte[] hinhAnh { get; set; }
        public int thoiGianPhaChe { get; set; }
        public int trangThai { get; set; }
        public string ghiChu { get; set; }
    }
}