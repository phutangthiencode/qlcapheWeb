using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.Models;

namespace qlCaPhe.App_Start.Cart
{
    /// <summary>
    /// Class xử lý cho giỏ mục tiêu đánh giá
    /// </summary>
    public class cartMucTieu
    {
         private SortedList _info;

        public SortedList Info
        {
            get { return _info; }
            set { _info = value; }
        }
        /// <summary>
        /// Hàm dựng
        /// </summary>
        public cartMucTieu()
        {
            this.Info = new SortedList();
        }
        /// <summary>
        /// Hàm thêm mới một mục tiêu đánh giá vào giỏ
        /// </summary>
        /// <param name="x">Object mục tiêu cần thêm</param>
        public void addCart(mucTieuDanhGia x)
        {
            try
            {
                //-----Kiểm tra xem nguyên liệu đã có trong giỏ chưa
                if (!this.Info.ContainsKey(x.maMucTieu))
                    this.Info.Add(x.maMucTieu, x);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartMucTieu - Function: addCart", ex.Message);
            }
        }

        /// <summary>
        /// Hàm xóa 1 mục tiêu đánh giá khỏi giỏ
        /// </summary>
        /// <param name="maMucTieu">Mã mục tiêu  cần xóa</param>
        public void removeItem(int maMucTieu)
        {
            try
            {
                //-------Nếu tìm thấy nguyên liệu cần xóa có trong giỏ
                if (this.Info.ContainsKey(maMucTieu))
                    this.Info.Remove(maMucTieu);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartMucTieu - Function: removeItem", ex.Message);
            }
        }

        /// <summary>
        /// Hàm lấy thông tin của 1 mục tiêu đánh giá
        /// </summary>
        /// <param name="maMucTieu">Mã mục tiêu cần lấy thông tin</param>
        /// <returns></returns>
        public mucTieuDanhGia getInfo(int maMucTieu)
        {
            try
            {
                foreach (mucTieuDanhGia item in this.Info.Values)
                    if (item.maMucTieu == maMucTieu)
                        return item;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartMucTieu - Function: getInfo", ex.Message);
            }
            return null;
        }
    }
}