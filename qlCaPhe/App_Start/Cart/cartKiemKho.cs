using qlCaPhe.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.App_Start.Cart
{
    public class cartKiemKho
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
        public cartKiemKho()
        {
            this.Info = new SortedList();
        }
        /// <summary>
        /// Hàm thêm mới một nguyên liệu cần kiểm tra vào giỏ
        /// </summary>
        /// <param name="x"></param>
        public void addCart(ctTonKho x)
        {
            try
            {
                //-----Kiểm tra xem nguyên liệu đã có trong giỏ chưa
                if (!this.Info.ContainsKey(x.maNguyenLieu))
                    this.Info.Add(x.maNguyenLieu, x);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartKiemKho - Function: addCart", ex.Message);
            }
        }
        /// <summary>
        /// Hàm xóa 1 nguyên liệu cần kiểm tra khỏi giỏ
        /// </summary>
        /// <param name="maNguyenLieu">Nguyên liệu cần xóa</param>
        public void removeItem(int maNguyenLieu)
        {
            try
            {
                //-------Nếu tìm thấy nguyên liệu cần xóa có trong giỏ
                if (this.Info.ContainsKey(maNguyenLieu))
                    this.Info.Remove(maNguyenLieu);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartKiemKho - Function: removeItem", ex.Message);
            }
        }
        /// <summary>
        /// Hàm lấy thông tin của nguyên liệu kiểm kho
        /// </summary>
        /// <param name="maNguyenLieu">Mã để xác định nguyên liệu cần lấy</param>
        /// <returns></returns>
        public ctTonKho getInfo(int maNguyenLieu)
        {
            try
            {
                foreach (ctTonKho item in this.Info.Values)
                    if (item.maNguyenLieu == maNguyenLieu)
                        return item;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartKiemKho - Function: getInfo", ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Hàm lấy tổng số tiền nguyên liệu có trong cart
        /// </summary>
        /// <returns></returns>
        public long getTotalPrice()
        {
            long kq = 0;
            foreach (ctTonKho item in this.Info.Values)
                kq += item.donGia;
            return kq;
        }
    }
}