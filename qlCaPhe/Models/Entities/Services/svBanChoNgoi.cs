using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{
    public class svBanChoNgoi
    {
        public int maBan { get; set; }
        public string tenBan { get; set; }
        public int maKhuVuc { get; set; }
        public int sucChua { get; set; }
        public byte[] hinhAnh { get; set; }
        public string gioiThieu { get; set; }
        public int trangThai { get; set; }
        public string ghiChu { get; set; }
    }
}