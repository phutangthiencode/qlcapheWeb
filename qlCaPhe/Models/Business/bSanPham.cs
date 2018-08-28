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
                //-------------Lặp qua những bước có sử dụng nguyên liệu
                foreach (ctCongThuc ctCongThuc in congThucSanPham.ctCongThucs.Where(c => c.maNguyenLieu > 0).ToList())
                {
                    double soLuongSuDung = (double)ctCongThuc.soLuongNguyenLieu;
                    //--------Kiểm tra số lượng nguyên liệu trong kho có đủ để pha chế
                    /////////////-----------------Lấy thông tin tồn kho mới nhất-----------
                    TonKho tonKhoNew = db.TonKhoes.OrderByDescending(t => t.maSoKy).FirstOrDefault();
                    if (tonKhoNew != null)
                    {
                        /////////////-----------------Lấy danh sách nguyên liệu trong tồn kho
                        ctTonKho ctTon = tonKhoNew.ctTonKhoes.SingleOrDefault(ct => ct.maNguyenLieu == ctCongThuc.maNguyenLieu);
                        ////////////------------------Lấy danh sách xuất kho từ thời điểm kiểm kê kho đến nay=> để lấy Số lượng tồn
                        List<NguyenLieuXuat> listNguyenLieuXuat = new bNguyenLieu().layDanhSachNguyenLieuXuat(db, tonKhoNew.ngayKiem);
                        if (ctTon != null)
                        {
                            int soLuongThucTe = (int)ctTon.soLuongThucTe; //-----Lấy số lượng thực tế trong tồn kho
                            int soLuongXuat = 0; NguyenLieuXuat nguyenLieuXuat = listNguyenLieuXuat.SingleOrDefault(s => s.maNguyenLieu == ctTon.maNguyenLieu);
                            if (nguyenLieuXuat != null)
                                soLuongXuat = nguyenLieuXuat.soLuongXuat;
                            if (soLuongThucTe - soLuongXuat < soLuongSuDung)
                                return false; //-----Trả về khi số lượng không đủ
                        }
                        else
                            return false; //------trả về khi không có nguyên liệu trong kho
                    }
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