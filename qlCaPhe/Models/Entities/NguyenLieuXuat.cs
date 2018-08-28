using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities
{
    public class NguyenLieuXuat
    {
        public int maNguyenLieu { get; set; }
        public int soLuongXuat { get; set; }
        public long tongTienXuat { get; set; }

        public NguyenLieuXuat(int maNguyenLieu, int soLuongXuat, long tongTienXuat)
        {
            this.maNguyenLieu = maNguyenLieu;
            this.soLuongXuat = soLuongXuat;
            this.tongTienXuat = tongTienXuat;
        }
    }
}