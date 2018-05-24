using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.Models;
using System.Collections;

namespace qlCaPhe.App_Start.Session
{
    public class cartHoaDonTam
    {
        private SortedList _item;
        public long tongTienDtb; //-----Lưu giữ tổng tiền trong database vào session
        public SortedList Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// Hàm dựng
        /// </summary>
        public cartHoaDonTam()
        {
            this._item = new SortedList();
        }
        /// <summary>
        /// Hàm thêm một sản phẩm vào hóa đơn tạm trong Session
        /// </summary>
        /// <returns>Trả về kết quả > 0 thì thêm thành công</returns>
        public int addCart(ctHoaDonTam x)
        {
            int kq = 0;
            try
            {
                ///--------Kiểm tra xem sản phẩm này đã chọn chưa. Nếu chưa thi..........
                if (!this.Item.ContainsKey(x.maSP))
                {
                    this.Item.Add(x.maSP, x);
                    kq++;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartHoaDonTam - Function: addCart", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm trả về danh sách các sản phẩm đã chọn có trong cart
        /// </summary>
        /// <returns></returns>
        public List<ctHoaDonTam> getList()
        {
            List<ctHoaDonTam> kq = new List<ctHoaDonTam>();
            try
            {
                foreach (ctHoaDonTam i in this.Item.Values)
                    kq.Add(i);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartHoaDonTam - Function: getList", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện xoá chi tiết trong session
        /// </summary>
        /// <param name="key">Mã sản phẩm của chi tiết cần xóa</param>
        /// <returns>Trả về kết quả > 0 thì thêm thành công</returns>
        public int removeItem(ctHoaDonTam ctXoa)
        {
            int kq = 0;
            try
            {
                //Nếu như tìm thấy chi tiết đó trong Session
                if (this.Item.ContainsKey(ctXoa.maSP))
                {
                    this.Item.Remove(ctXoa.maSP);
                    kq++;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartHoaDonTam - Function: removeItem", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm lấy 1 phần tử trong hóa đơn cart session theo mã sản phẩm
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ctHoaDonTam getItem(int maSP)
        {
            try
            {
                foreach (ctHoaDonTam ct in this.Item.Values)
                    if (ct.maSP == maSP)
                        return ct;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartHoaDonTam - Function: getItem", ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Hàm thực hiện cập nhật số lượng sản phẩm đã chọn trong SEssion
        /// </summary>
        /// <param name="hoaDon">Đối tượng hóa đơn cần cập nhật</param>
        /// <param name="soLuong">Số lượng sản phẩm cần cập nhật</param>
        public int updateItem(ctHoaDonTam hoaDon)
        {
            int kq = 0;
            try
            {
                ///--------Kiểm tra xem có trùng mã nguyên liệu, nếu trùng thì xóa và tạo lại chi tiết
                if (this.Item.ContainsKey(hoaDon.maSP))
                {
                    this.removeItem(hoaDon);
                    this.Item.Add(hoaDon.maSP, hoaDon);
                    kq++;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartNhapKho - Function: addCart", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// hàm thực hiện lấy tổng số tiền của hóa đơn có trong session
        /// </summary>
        /// <returns></returns>
        public long getTotalAmount()
        {
            long kq = 0;
            foreach (ctHoaDonTam i in this.Item.Values)
                kq += i.soLuong * i.donGia;
            return kq;
        }
    }
}