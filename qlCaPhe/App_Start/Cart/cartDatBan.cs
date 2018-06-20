using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using  qlCaPhe.Models;

namespace qlCaPhe.App_Start.Cart
{
    /// <summary>
    /// Class xử lý nghiệp vụ lưu trữ đặt bàn tại trang public
    /// </summary>
    public class cartDatBan
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
        public cartDatBan()
        {
            this._info = new SortedList();
        }
        /// <summary>
        /// Hàm thêm mới 1 bàn vào giỏ
        /// </summary>
        /// <param name="x"></param>
        public void addCart(ctDatBan x)
        {
            try
            {
                //------Nếu bàn cần đặt chưa có trong giỏ
                if (!this.Info.ContainsKey(x.maBan))
                    this.Info.Add(x.maBan, x);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartDatBan - Function: addCart", ex.Message);
            }
        }

        /// <summary>
        /// Hàm xóa 1 bàn đã đặt khỏi giỏ
        /// </summary>
        /// <param name="maBan">Mã ca làm việc cần xóa</param>
        public void removeItem(int maBan)
        {
            try
            {
                //-------Nếu tìm thấy nguyên liệu cần xóa có trong giỏ
                if (this.Info.ContainsKey(maBan))
                    this.Info.Remove(maBan);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartDatBan - Function: removeItem", ex.Message);
            }
        }
        /// <summary>
        /// Hàm lấy danh sách object các bàn đã đặt
        /// </summary>
        /// <returns></returns>
        public List<ctDatBan> getList()
        {
            List<ctDatBan> kq = new List<ctDatBan>();
            try
            {
                foreach (ctDatBan item in this.Info.Values)
                    kq.Add(item);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartDatBan - Function: removeItem", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Lấy tồng sức chứa của bàn đã đặt
        /// </summary>
        /// <returns>Số tổng sức chứa của các bàn</returns>
        public int getTotalCapacity()
        {
            int kq = 0;
            try
            {
                foreach (ctDatBan item in this.Info.Values)
                    kq += item.BanChoNgoi.sucChua;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartDatBan - Function: getTotalCapacity", ex.Message);
            }
            return kq;
        }

    }
}