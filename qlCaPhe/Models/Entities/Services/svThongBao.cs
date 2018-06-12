using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{

    /// <summary>
    /// Class dành cho services
    /// </summary>
    public class svThongBao
    {
        public int maThongBao { get; set; }
        public string ndThongBao { get; set; }
        public string taiKhoan { get; set; }
        public DateTime ngayTao { get; set; }
        public bool daXem { get; set; }
        public string ghiChu { get; set; }
    }
}