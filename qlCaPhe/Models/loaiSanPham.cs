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
    
    public partial class loaiSanPham
    {
        public loaiSanPham()
        {
            this.sanPhams = new HashSet<sanPham>();
        }
    
        public int maLoai { get; set; }
        public string tenLoai { get; set; }
        public string dienGiai { get; set; }
        public string ghiChu { get; set; }
    
        public virtual ICollection<sanPham> sanPhams { get; set; }
    }


    public class svLoaiSanPham
    {
        public int maLoai { get; set; }
        public string tenLoai { get; set; }
        public string dienGiai { get; set; }
        public string ghiChu { get; set; }
    }
}