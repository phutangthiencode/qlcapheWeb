using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{
    public class svCtHoaDonTam
    {
        public int maCtTam { get; set; }
        public int maBan { get; set; }
        public int maSP { get; set; }
        public long donGia { get; set; }
        public int soLuong { get; set; }
        public int trangThaiPhaChe { get; set; }

        public string tenSP { get; set; }
        public byte[] hinhAnh { get; set; }
    }
}