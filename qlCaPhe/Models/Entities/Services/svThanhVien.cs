using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{
    /// <summary>
    /// Class chứa các thuộc tính thành viên dành cho wevservices
    /// </summary>
    public class svThanhVien
    {
        public int maTV { get; set; }
        public string hoTV { get; set; }
        public string tenTV { get; set; }
        public Nullable<bool> gioiTinh { get; set; }
        public Nullable<System.DateTime> ngaySinh { get; set; }
        public string noiSinh { get; set; }
        public string diaChi { get; set; }
        public string soDT { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string soCMND { get; set; }
        public Nullable<System.DateTime> ngayCap { get; set; }
        public string noiCap { get; set; }
        public byte[] hinhDD { get; set; }
        public Nullable<System.DateTime> ngayThamGia { get; set; }
        public string ghiChu { get; set; }
    }
}