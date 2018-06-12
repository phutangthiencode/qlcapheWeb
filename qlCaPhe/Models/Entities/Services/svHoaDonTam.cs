using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{
    public class svHoaDonTam
    {
        public int maBan { get; set; }
        public int trangThaiHoadon { get; set; }
        public int trangthaiphucVu { get; set; }
        public DateTime ngayLap { get; set; }
        public DateTime thoiDiemDen { get; set; }
        public string nguoiPhucVu { get; set; }
        public long tongTien { get; set; }
        public string ghiChu { get; set; }
        public string tenBan { get; set; }
        public byte[] hinhAnhBan { get; set; }
        public int soLuongSanPham { get; set; }
        public string dienGiaiChiTiet { get; set; } //-----Thuộc tính chứa một số sản phẩm có trong hóa đơn để hiện lên listview
    }
}