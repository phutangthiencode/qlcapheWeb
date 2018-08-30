using qlCaPhe.App_Start;
using qlCaPhe.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Business
{
    /// <summary>
    /// Class chứa các phương thức liên quan với sanPham
    /// </summary>
    public class bSanPham
    {
        /// <summary>
        /// Hàm kiểm tra sản phẩm có khả thi để tiếp nhận
        /// </summary>
        /// <param name="sp">Sản phẩm cần kiểm tra</param>
        /// <returns>True: Khả thi, False: không khả thi (hết nguyên liệu)</returns>
        public bool kiemTraSanPhamKhaThi(sanPham sp)
        {
            qlCaPheEntities db = new qlCaPheEntities();
            //------Lấy công thức của sản phẩm còn sử dụng
            congThucPhaChe congThucSanPham = sp.congThucPhaChes.SingleOrDefault(c => c.trangThai == true);
            //---------Kiểm tra công thức của sản phẩm
            if (congThucSanPham != null)
            {
                //--------------Lấy số lượng thực tế trong kho
                List<ctTonKho> listThucTe = new bTonKho().layDanhSachTon();
                List<ctCongThuc> listBuocCongThuc = congThucSanPham.ctCongThucs.Where(c => c.maNguyenLieu > 0).ToList();
                //-------------Lặp qua những bước có sử dụng nguyên liệu
                foreach (ctCongThuc ctCongThuc in listBuocCongThuc)
                {
                    double soLuongSuDung = (double)ctCongThuc.soLuongNguyenLieu;                    
                    //-------Lặp qua các nguyên liệu tồn kho cần sử dụng cho công thức
                    ctTonKho nguyenLieuTonThucTe = listThucTe.SingleOrDefault(ct=>ct.maNguyenLieu==ctCongThuc.maNguyenLieu);
                    if (nguyenLieuTonThucTe != null)
                    {
                        if (nguyenLieuTonThucTe.soLuongCuoiKyLyThuyet < soLuongSuDung)
                            return false;
                    }
                    else
                        return false;//-----Trả về khi không có nguyên liệu trong kho
                }
                return true; //----Trả về khi đã kiểm tra số lượng nguyên liệu pha chế sản phẩm đủ
            }
            return false; //------Trả về khi không có công thức pha chế của sản phẩm
        }

        /// <summary>
        /// Hàm Thực hiện thêm mới lịch sử giá vào CSDL
        /// </summary>
        /// <param name="donGia">Đơn giá cần thêm</param>
        /// <param name="donGiaGoc">Đơn giá gốc dự vào tiền nguyên liệu hoặc giá cũ</param>
        /// <param name="ghiChu">Ghi chú cho việc thêm mới lịch sử giá</param>
        /// <param name="maSP">Mã sản phẩm cần thêm giá</param>
        /// <param name="db"></param>
        /// <returns>1: Thêm thành công - 0: Thêm thất bại</returns>
        public int themMoiLichSuGiaVaoDtb(int maSP, long donGia, long donGiaGoc, string ghiChu, qlCaPheEntities db)
        {
            int kq = 0;
            try
            {
                //------Khởi tạo đối tượng lịch sử giá
                lichSuGia lichSuAdd = new lichSuGia();
                lichSuAdd.donGia = donGia;
                lichSuAdd.donGiaGoc = donGiaGoc;
                lichSuAdd.ghiChu = ghiChu;
                lichSuAdd.maSanPham = maSP;
                lichSuAdd.ngayCapNhat = DateTime.Now;
                lichSuAdd.nguoiTao = xulyChung.layTenDangNhap();
                //----Thêm lịch sử giá vào CDSL
                db.lichSuGias.Add(lichSuAdd);
                kq = db.SaveChanges();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: DoUongController - Function: luuLichSuGia", ex.Message);
                throw new Exception(ex.Message);
            }
            return kq;
        }
    }
}