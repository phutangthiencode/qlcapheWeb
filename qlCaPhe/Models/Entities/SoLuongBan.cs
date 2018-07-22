using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities
{
    /// <summary>
    /// Class lưu só lượng bàn theo trạng thái: Trống, Chưa order, Đã order, chờ thanh toán, đã thanh toán, tổng cộng bàn
    /// </summary>
    public class SoLuongBan
    {
        public int trong { get; set; }
        public int choOrder { get; set; }
        public int daOrder { get; set; }
        public int choThanhToan { get; set; }
        public int daThanhToan { get; set; }
        public int tongCongBan { get; set; }


    }
}