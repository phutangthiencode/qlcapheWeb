using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.Models;
using System.Collections;
using qlCaPhe.Models.Business;

namespace qlCaPhe.App_Start.Session
{
    public class cartCongThuc
    {
        private SortedList _item;

        public SortedList Item
        {
            get { return _item; }
            set { _item = value; }
        }
        public cartCongThuc()
        {
            this.Item = new SortedList();
        }
        /// <summary>
        /// Hàm thêm chi tiết công thức đã chọn vào Session
        /// </summary>
        public void addCart(ctCongThuc x)
        {
            try
            {
                ///--------Kiểm tra xem có trùng mã nguyên liệu, nếu không thì mới cho thêm vào
                if (!this.Item.ContainsKey(x.maChiTiet))
                    this.Item.Add(x.maChiTiet, x);
                //---Nếu như nguyên liệu đã có trong session thì xóa và cập nhật nguyên liệu mới
                else
                {
                    //-----Xóa nguyên liệu 
                    this.removeItem(x.maChiTiet);
                    this.Item.Add(x.maNguyenLieu, x);
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartCongThuc- Function: addCart", ex.Message);
            }
        }
        /// <summary>
        /// Hàm trả về bảng danh sách các chi tiết công thức có trong sesion 
        /// </summary>
        /// <returns></returns>
        public List<ctCongThuc> getList()
        {
            List<ctCongThuc> kq = new List<ctCongThuc>();
            try
            {
                foreach (ctCongThuc i in this.Item.Values)
                    kq.Add(i);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartCongThuc - Function: getList", ex.Message);
            }
            return kq;
        }
        public ctCongThuc getItem(int key)
        {
            foreach (ctCongThuc i in this.Item.Values)
            {
                if (i.maChiTiet == key)
                    return i;
            }
            return null;
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
                xulyFile.ghiLoi("Class: cartCongThuc - Function: removeItem", ex.Message);
            }
        }

        /// <summary>
        /// hàm thực hiện lấy tổng số tiền của nguyên liệu tại thời điểm lập công thứccó trong session
        /// </summary>
        /// <returns></returns>
        public long getTotalPriceNguyenLieu()
        {
            long kq = 0;
            foreach (ctCongThuc i in this.Item.Values)
            {
                if (i.maNguyenLieu > 0) //--Nếu bước này có dùng nguyên liêu
                {
                    qlCaPheEntities db = new qlCaPheEntities();
                    //--------Sửa lại khi có bNhapKho
                    double donGiaNguyenLieu = new bNhapKho().tinhTienBinhQuanNguyenLieuNhap(i.maNguyenLieu);
                    //----Cộng dồn tổng tiền nguyên liệu = Số lượng sử dụng (với đơn vị lớn nhất (kg, lit)) * với đơn giá nguyên liệu
                    double soLuongSuDung = new bNguyenLieu().chuyenDoiDonViNhoSangLon(i.soLuongNguyenLieu, i.nguyenLieu);
                    kq += (long)(soLuongSuDung * donGiaNguyenLieu);
                }
            }
            return kq;
        }
        /// <summary>
        /// Hàm lấy danh sách các nguyên liệu sử dụng có trong công thức
        /// </summary>
        /// <returns></returns>
        public List<ctCongThuc> getListNguyenLieu()
        {
            List<ctCongThuc> kq = new List<ctCongThuc>();
            try
            {
                foreach (ctCongThuc i in this.Item.Values)
                    if (i.maNguyenLieu != 0) //Đã có nguyên liệu
                        kq.Add(i);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: cartCongThuc - Function: getListNguyenLieu", ex.Message);
            }
            return kq;
        }
        ///// <summary>
        ///// Hàm tính tiền bình quân nhập nguyên liệu
        ///// <para/> theo BÌNH QUÂN GIA QUYỀN
        ///// </summary>
        ///// <param name="maNguyenLieu"></param>
        ///// <returns></returns>
        //private double tinhTienBinhQuanNguyenLieuNhap(int maNguyenLieu)
        //{
        //    double kq = 0;
        //    double index = 0;
        //    double tongTienNhap = 0;
        //    foreach (ctPhieuNhapKho ctPhieu in new qlCaPheEntities().ctPhieuNhapKhoes.Where(ct => ct.maNguyenLieu == maNguyenLieu).ToList())
        //    {
        //        index += (double)ctPhieu.soLuongNhap;
        //        tongTienNhap += (double)(ctPhieu.soLuongNhap * ctPhieu.donGiaNhap);
        //    }
        //    kq = tongTienNhap / index;
        //    return kq;
        //}
    }
}