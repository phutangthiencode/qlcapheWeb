using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.Models;

namespace qlCaPhe.App_Start.Cart
{
    public class cartDieuPhoi
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
        public cartDieuPhoi()
        {
            this.Info = new SortedList();
        }
        /// <summary>
        /// Hàm thêm mới 1 ca làm việc vào giỏ
        /// </summary>
        /// <param name="x"></param>
        public void addCart(ctBangGiaoViec x)
        {
            try
            {
                //-------Kiểm tra xem ca đã có trong giỏ chưa
                if (!this.Info.ContainsKey(x.maCa))
                    this.Info.Add(x.maCa, x);
                else //-------Update ca đã chọn
                {
                    this.removeItem(x.maCa);
                    this.Info.Add(x.maCa, x);
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartDieuPhoi - Function: addCart", ex.Message);
            }
        }

        /// <summary>
        /// Hàm xóa 1 ca làm việc khỏi giỏ
        /// </summary>
        /// <param name="maCa">Mã ca làm việc cần xóa</param>
        public void removeItem(int maCa)
        {
            try
            {
                //-------Nếu tìm thấy nguyên liệu cần xóa có trong giỏ
                if (this.Info.ContainsKey(maCa))
                    this.Info.Remove(maCa);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartDieuPhoi - Function: removeItem", ex.Message);
            }
        }
        /// <summary>
        /// Hàm lấy danh sách object chi tiết có trong giỏ
        /// </summary>
        /// <returns></returns>
        public List<ctBangGiaoViec> getList()
        {
            List<ctBangGiaoViec> kq = new List<ctBangGiaoViec>();
            try
            {
                foreach (ctBangGiaoViec item in this.Info.Values)
                    kq.Add(item);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartDieuPhoi - Function: removeItem", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// Hàm lấy thông tin của 1 ca làm việc đã có trong giỏ
        /// </summary>
        /// <param name="maCa">Mã ca làm việc cần lấy</param>
        /// <returns></returns>
        public ctBangGiaoViec getInfo(int maCa)
        {
            try
            {
                foreach (ctBangGiaoViec item in this.Info.Values)
                    if (item.maCa== maCa)
                        return item;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartDieuPhoi - Function: getInfo", ex.Message);
            }
            return null;
        }
    }
}