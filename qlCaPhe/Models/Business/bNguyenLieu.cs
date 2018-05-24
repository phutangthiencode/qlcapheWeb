using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.Models;

namespace qlCaPhe.Models.Business
{
    /// <summary>
    /// Class chứa các hành động liên quan đến nguyenLieu object
    /// </summary>
    public class bNguyenLieu
    {
        /// <summary>
        /// Hàm chuyển đổi số lượng nguyên liệu từ đơn vị nhỏ sang đơn vị lớn <para/>
        /// VD: 1000 gam => 1 kg
        /// </summary>
        /// <param name="nl">Nguyên liệu cần chuyển đổi</param>
        /// <param name="soLuongCu">Số lượng cũ trước khi chuyển đổi</param>
        /// <returns></returns>
        public double chuyenDoiDonViNhoSangLon(double? soLuongCu, nguyenLieu nl)
        {
            return (double) (soLuongCu / nl.tyLeChuyenDoi);
        }
        /// <summary>
        /// Hàm chuyển đổi số lượng nguyên liệu từ đơn vị lớn sang đơn vị nhỏ
        /// <para/> VD: 1 kg => 1000 gam
        /// </summary>
        /// <param name="soLuongCu">Số lượng cũ trước khi chuyển đổi</param>
        /// <param name="nl">Nguyên liệu cần chuyển đổi</param>
        /// <returns></returns>
        public double chuyenDoiDonViTuLonSangNho(double? soLuongCu, nguyenLieu nl)
        {
            return (double) (soLuongCu * nl.tyLeChuyenDoi);
        }
    }
}