using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{
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