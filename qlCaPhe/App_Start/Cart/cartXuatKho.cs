using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using qlCaPhe.App_Start;
using qlCaPhe.Models;
using qlCaPhe.Models.Business;

namespace qlCaPhe.App_Start.Cart
{
    public class cartXuatKho
    {
        
        private SortedList _item;

        public SortedList Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        /// Hàm dựng
        /// </summary>
        public cartXuatKho()
        {
            this._item = new SortedList();
        }
        /// <summary>
        /// Hàm thêm chi tiết phiếu xuất đã chọn vào Session
        /// </summary>
        public void addCart(ctPhieuXuatKho x)
        {
            try
            {
                ///--------Kiểm tra xem có trùng mã nguyên liệu, nếu không thì mới cho thêm vào
                if(!this.Item.ContainsKey(x.maNguyenLieu))
                    this.Item.Add(x.maNguyenLieu, x);
                //---Nếu như nguyên liệu đã có trong session thì xóa và cập nhật nguyên liệu mới
                else
                {
                    //-----Xóa nguyên liệu trong cart
                    this.removeItem(x.maNguyenLieu);
                    this.Item.Add(x.maNguyenLieu, x);
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartNhapKho - Function: addCart", ex.Message);
            }
        }
        /// <summary>
        /// Hàm trả về bảng danh sách các chi tiết phiếu nhập kho có trong sesion 
        /// </summary>
        /// <returns></returns>
        public List<ctPhieuXuatKho> getList()
        {
            List<ctPhieuXuatKho> kq = new List<ctPhieuXuatKho>();
            try
            {
                foreach (ctPhieuXuatKho i in this.Item.Values)
                    kq.Add(i);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartNhapKho - Function: getListForTable", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện xoá chi tiết trong session
        /// </summary>
        /// <param name="key">Mã nguyên liệu của chi tiết cần xóa</param>
        public void removeItem(int key)
        {
            try
            {
                //Nếu như tìm thấy đợt thi đó trong Session
                if (this.Item.ContainsKey(key))
                    this.Item.Remove(key);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartNhapKho - Function: removeItem", ex.Message);
            }
        }

        /// <summary>
        /// hàm thực hiện lấy tổng số tiền xuất nguyên liệu có trong session
        /// </summary>
        /// <returns></returns>
        public double getTotalPrice()
        {
            double kq = 0; bNguyenLieu bNguyenLieu = new bNguyenLieu();
            foreach (ctPhieuXuatKho i in this.Item.Values)
            {
                //--------Chuyển đổi ra đơn vị chính để tính tiền
                double soLuongXuat = (double) bNguyenLieu.chuyenDoiDonViNhoSangLon((double)i.soLuongXuat, i.nguyenLieu);
                kq += i.donGiaXuat * soLuongXuat;
            }
            return kq;
        }

    }
}