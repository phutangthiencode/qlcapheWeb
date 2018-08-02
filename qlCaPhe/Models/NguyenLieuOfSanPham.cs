using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models
{
    public class NguyenLieuOfSanPham 
    {
        public int maSanPham { get; set; }  
        public string tenSanPham { get; set; }
        public int maLoai { get; set; }
        public string moTaSanPham { get; set; }
        public long donGia { get; set; }
        public byte[] hinhAnh { get; set; }
        public Nullable<int> thoiGianPhaChe { get; set; }
        public int trangThaiSanPham { get; set; }
        public string ghiChuSanPham { get; set; }
        public string tenLoai { get; set; }

        public int maCongThuc { get; set; }
        public string tenCongThuc { get; set; }
        public string dienGiaiCongThuc { get; set; }
    }
}