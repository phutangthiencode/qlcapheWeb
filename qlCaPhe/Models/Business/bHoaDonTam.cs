using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.App_Start;


namespace qlCaPhe.Models.Business
{
    /// <summary>
    /// Class xử lý nghiệp vụ liên quan đến bảng hoaDonTam và ctHoaDonTam
    /// </summary>
    public class bHoaDonTam
    {
        /// <summary>
        /// Hàm thêm mới 1 hóa đơn tạm vào bảng HoaDonTam trong database
        /// </summary>
        /// <param name="db">Object entities thực thi</param>
        /// <param name="maBan">Mã bàn cần thêm mới vào bảng</param>
        /// <param name="tenDangNhap">Tên đăng nhập người phục vụ (Người thêm)</param>
        /// <returns>1: Thêm thành công, 2: Thất bại</returns>
        public int themMoiHoaDonTam(qlCaPheEntities db, int maBan, string tenDangNhap)
        {
            int kq = 0;
            try
            {
                hoaDonTam hoaDonAdd = new hoaDonTam();
                hoaDonAdd.maBan = maBan;
                hoaDonAdd.trangThaiHoadon = 0;//-----Thiết lập trạng thái hóa đơn 0 - VỪA VÀO BÀN
                hoaDonAdd.trangthaiphucVu = -1;//------Thiết lập trạng thái phục vụ - Trước khi nhận order
                hoaDonAdd.thoiDiemDen = DateTime.Now;
                hoaDonAdd.nguoiPhucVu = tenDangNhap;
                db.hoaDonTams.Add(hoaDonAdd);
                kq = db.SaveChanges();
                if(kq>0)
                    xulyChung.ghiNhatKyDtb(2, "Đã tiếp nhận bàn");
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bHoaDonTam - Fucntion: themMoiHoaDonTam", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// Hàm CẬP NHẬT LẠI BÀN CHO HÓA ĐƠN TẠM <para />
        /// KHI ĐỔI BÀN
        /// </summary>
        /// <param name="db">Object thực thi entities</param>
        /// <param name="hoaDonCu">Object chứa các giá trị của hóa đơn cũ</param>
        /// <param name="maBanMoi">Mã bàn mới cần cập nhật sang</param>
        /// <returns></returns>
        public int capNhatHoaDonTam(qlCaPheEntities db, hoaDonTam hoaDonCu, int maBanMoi)
        {
            int kq = 0;
            try
            {
                //thiết lập lại các thuộc tính của bàn mới
                hoaDonTam hoaDonMoi = new hoaDonTam();
                hoaDonMoi.ghiChu = hoaDonCu.ghiChu;
                hoaDonMoi.ngayLap = hoaDonCu.ngayLap;
                hoaDonMoi.nguoiPhucVu = hoaDonCu.nguoiPhucVu;
                hoaDonMoi.thoiDiemDen = hoaDonCu.thoiDiemDen;
                hoaDonMoi.tongTien = hoaDonCu.tongTien;
                hoaDonMoi.trangThaiHoadon = hoaDonCu.trangThaiHoadon;
                hoaDonMoi.trangthaiphucVu = hoaDonCu.trangthaiphucVu;
                hoaDonMoi.maBan = maBanMoi;
                db.hoaDonTams.Add(hoaDonMoi);
                kq=  db.SaveChanges();
                if (kq > 0)
                {
                    this.capNhatChiTietHoaDonCuSangMoi(db, hoaDonCu.ctHoaDonTams.ToList(), maBanMoi);
                    this.xoaHoaDonTam(db, hoaDonCu);
                }           
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bHoaDonTam - Fucntion: capNhatHoaDonTam", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm chuyển đổi dữ liệu bảng ctHoaDonTam từ bảng hoaDonTam cũ sang mới
        /// </summary>
        /// <param name="db">Object thực thi entities</param>
        /// <param name="listCu">Danh sách ctHoaDonTam từ hoaDonCu</param>
        /// <param name="maBanMoi"></param>
        /// <returns></returns>
        private int capNhatChiTietHoaDonCuSangMoi(qlCaPheEntities db, List<ctHoaDonTam> listCu, int maBanMoi)
        {
            int kq = 0;
            try
            {
                //----Update tất chi tiết từ bàn cũ sang bàn mới
                foreach (ctHoaDonTam ct in listCu.ToList())
                {
                    ct.maBan = maBanMoi;
                    db.Entry(ct).State = System.Data.Entity.EntityState.Modified;
                    kq += db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bHoaDonTam - Fucntion: capNhatChiTietHoaDonCuSangMoi", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm xóa 1 hóa đơn tạm Khi DỌN BÀN
        /// </summary>
        /// <param name="db"></param>
        /// <param name="hoaDonXoa">Object hóa đơn cần xóa</param>
        /// <returns>1: Thành công, 2: Thất bại</returns>
        private int xoaHoaDonTam(qlCaPheEntities db, hoaDonTam hoaDonXoa)
        {
            int kq = 0;
            try
            {
                //------Dọn bàn cũ
                db.hoaDonTams.Remove(hoaDonXoa);
                kq=  db.SaveChanges();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bHoaDonTam - Fucntion: xoaHoaDonTam", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// Lấy tổng tiền của có trong hóa đơn tạm. 
        /// Chỉ tính những sản phẩm có khả năng bán.
        /// không tính tiền cho những sản phẩm có đề xuất thay thế
        /// </summary>
        /// <param name="hoaDon">Hóa đơn tạm cần lấy tổng tiền</param>
        /// <returns></returns>
        public long layTongTienSanPham(hoaDonTam hoaDon)
        {
            long kq = 0;
            try
            {
                //-------Lặp qua những sản phẩm có khả năng bán
                foreach (ctHoaDonTam ct in hoaDon.ctHoaDonTams.Where(c => c.trangThaiPhaChe != 4))
                    kq += ct.donGia * ct.soLuong;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bHoaDonTam - Fucntion: layTongTienSanPham", ex.Message);
            }
            return kq;
        }

        /// <summary>
        /// Hàm lấy tổng số lượng sản phẩm trong hóa đơn dựa vào trạng thái pha chế
        /// </summary>
        /// <param name="hoaDon">Hóa đơn cần lấy</param>
        /// <param name="trangThai">Trạng thái pha chế cần lấy<para/> -1: Chờ tiếp nhận pha chế - 0: Đã tiếp nhận pha chế - 1: Đang pha chế - 2: Đã pha chế xong</param>
        /// <returns></returns>
        public int layTongSoLuongSanPhamTrongHoaDon(hoaDonTam hoaDon, int trangThai)
        {
            int kq = 0;
            try
            {
                foreach (ctHoaDonTam ct in hoaDon.ctHoaDonTams.Where(c => c.trangThaiPhaChe == trangThai))
                    kq += ct.soLuong;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bHoaDonTam - Fucntion: layTongTienSanPham", ex.Message);
            }
            return kq;
        }
    }
}