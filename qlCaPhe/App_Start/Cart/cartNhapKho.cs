using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.Models;
using qlCaPhe.Models.Business;

namespace qlCaPhe.App_Start
{
    public class cartNhapKho
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
        public cartNhapKho()
        {
            this._item = new SortedList();
        }
        /// <summary>
        /// Hàm thêm chi tiết phiếu nhập đã chọn vào Session
        /// </summary>
        public void addCart(ctPhieuNhapKho x)
        {
            try
            {
                ///--------Kiểm tra xem có trùng mã nguyên liệu, nếu không thì mới cho thêm vào
                if(!this.Item.ContainsKey(x.maNguyenLieu))
                    this.Item.Add(x.maNguyenLieu, x);
                //---Nếu như nguyên liệu đã có trong session thì xóa và cập nhật nguyên liệu mới
                else
                {
                    //-----Xóa nguyên liệu 
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
        public List<ctPhieuNhapKho> getListForTable()
        {
            List<ctPhieuNhapKho> kq = new List<ctPhieuNhapKho>();
            try
            {
                foreach (ctPhieuNhapKho i in this.Item.Values)
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
        /// hàm thực hiện lấy tổng  số tiền nhập nguyên liệu có trong session
        /// </summary>
        /// <returns></returns>
        public long getTotalPrice()
        {
            long kq=0;
            foreach (ctPhieuNhapKho i in this.Item.Values)
            {
                //kq += i.donGiaNhap * (int)(i.soLuongNhap / i.nguyenLieu.tyLeChuyenDoi);  //------tổng tiền = đơn giá * số lượng gốc (đơn vị hiển thị kg, lit....)
                double soLuongNhap = new bNguyenLieu().chuyenDoiDonViNhoSangLon(i.soLuongNhap, i.nguyenLieu);
                kq += (long)(i.donGiaNhap * soLuongNhap);
            }
            return kq;
        }


    }
}