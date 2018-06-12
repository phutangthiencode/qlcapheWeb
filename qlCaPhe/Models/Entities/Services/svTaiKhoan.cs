using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{
    /// <summary>
    /// Class chứa các thuộc tính tài khoản dành cho Webservices
    /// </summary>
    public class svTaiKhoan
    {
        public string tenDangNhap { get; set; }
        public string matKhau { get; set; }
        public int maTV { get; set; }
        public int maNhomTK { get; set; }
        public bool trangThai { get; set; }
        public string ghiChu { get; set; }
        public string quyenHan { get; set; }
    }
}