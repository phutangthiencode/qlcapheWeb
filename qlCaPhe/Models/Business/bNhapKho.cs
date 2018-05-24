using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Business
{
    /// <summary>
    /// Class chứa các phương thức liên quan đến bảng PhieuNhapKho và ctPhieuNhapKho
    /// </summary>
    public class bNhapKho
    {
        /// <summary>
        /// Hàm tính tiền bình quân nhập nguyên liệu
        /// <para/> theo BÌNH QUÂN GIA QUYỀN
        /// </summary>
        /// <param name="maNguyenLieu"></param>
        /// <returns></returns>
        public double tinhTienBinhQuanNguyenLieuNhap(int? maNguyenLieu)
        {
            double kq = 0;
            double index = 0;
            double tongTienNhap = 0;
            foreach (ctPhieuNhapKho ctPhieu in new qlCaPheEntities().ctPhieuNhapKhoes.Where(ct => ct.maNguyenLieu == maNguyenLieu).ToList())
            {
                index += (double)ctPhieu.soLuongNhap;
                tongTienNhap += (double)(ctPhieu.soLuongNhap * ctPhieu.donGiaNhap);
            }
            kq = tongTienNhap / index;
            return kq;
        }
    }
}