using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{
    /// <summary>
    /// Class phục vụ cho việc lấy thông tin của webservice
    /// </summary>
    public class svCtTonKho
    {
        public int maSoKy { get; set; }
        public int maNguyenLieu { get; set; }
        public long donGia { get; set; }
        public int soLuongDauKy { get; set; }
        public int soLuongCuoiKyLyThuyet { get; set; }
        public int soLuongThucTe { get; set; }
        public double tyLeHaoHut { get; set; }
        public string nguyenNhanHaoHut { get; set; }
        public string tenNguyenLieu { get; set; }
        public byte[] hinhNguyenLieu { get; set; }
        public string donViPhaChe { get; set; }
        public string donViHienThi { get; set; }
    }
}