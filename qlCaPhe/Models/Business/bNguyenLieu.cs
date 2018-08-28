using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.Models;
using qlCaPhe.App_Start;
using qlCaPhe.Models.Entities;

namespace qlCaPhe.Models.Business
{
    /// <summary>
    /// Class chứa các hành động liên quan đến nguyenLieu object
    /// </summary>
    public class bNguyenLieu
    {
        /// <summary>
        /// Hàm chuyển đổi số lượng nguyên liệu từ đơn vị nhỏ sang đơn vị lớn <para/>
        /// VD: 1000 gam => 1 kg
        /// </summary>
        /// <param name="nl">Nguyên liệu cần chuyển đổi</param>
        /// <param name="soLuongCu">Số lượng cũ trước khi chuyển đổi</param>
        /// <returns></returns>
        public double chuyenDoiDonViNhoSangLon(double? soLuongCu, nguyenLieu nl)
        {
            return (double) (soLuongCu / nl.tyLeChuyenDoi);
        }
        /// <summary>
        /// Hàm chuyển đổi số lượng nguyên liệu từ đơn vị lớn sang đơn vị nhỏ
        /// <para/> VD: 1 kg => 1000 gam
        /// </summary>
        /// <param name="soLuongCu">Số lượng cũ trước khi chuyển đổi</param>
        /// <param name="nl">Nguyên liệu cần chuyển đổi</param>
        /// <returns></returns>
        public double chuyenDoiDonViTuLonSangNho(double? soLuongCu, nguyenLieu nl)
        {
            return (double) (soLuongCu * nl.tyLeChuyenDoi);
        }

        /// <summary>
        /// Hàm lấy danh sách các sản phẩm được pha chế từ 1 nguyên liệu
        /// </summary>
        /// <param name="maNguyenLieu">Mã nguyên liệu cần xem</param>
        /// <returns>List object chứa thông tin nguyên liệu, sản phẩm, công thức</returns>
        public List<NguyenLieuOfSanPham> laySanPhamSuDungNguyenLieu(int maNguyenLieu)
        {
            List<NguyenLieuOfSanPham> listKQ = new List<NguyenLieuOfSanPham>();
            IEnumerable<object> listQuery = new qlCaPheEntities().laySanPhamTheoMaNguyenLieu(maNguyenLieu);
            foreach (object x in listQuery.ToList())
            {
                NguyenLieuOfSanPham nlsp = new NguyenLieuOfSanPham();
                nlsp.maSanPham = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(x, "maSanPham"));
                nlsp.tenSanPham = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenSanPham");
                nlsp.maLoai = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(x, "maLoai"));
                nlsp.tenLoai = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenLoai");
                nlsp.moTaSanPham = xulyDuLieu.layThuocTinhTrongMotObject(x, "moTa");
                nlsp.donGia = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(x, "donGia"));
                nlsp.hinhAnh = xulyDuLieu.layThuocTinhByteArrayMotObject(x, "hinhAnh");
                nlsp.thoiGianPhaChe = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(x, "thoiGianPhaChe"));
                nlsp.trangThaiSanPham = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(x, "trangThai"));
                nlsp.ghiChuSanPham = xulyDuLieu.layThuocTinhTrongMotObject(x, "ghiChu");
                nlsp.maCongThuc = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(x, "maCongThuc"));
                nlsp.tenCongThuc = xulyDuLieu.layThuocTinhTrongMotObject(x, "tenCongThuc");
                nlsp.dienGiaiCongThuc = xulyDuLieu.layThuocTinhTrongMotObject(x, "dienGiai");
                listKQ.Add(nlsp);
            }
            return listKQ;
        }

        /// <summary>
        /// Hàm lấy tất cả các nguyên liệu đã xuất kho từ ngày ... đến ngày hiện tại
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ngayLay">Ngày bắt đầu cần lấy</param>
        /// <returns>List object chứa các nguyên liệu đã xuất</returns>
        public List<NguyenLieuXuat> layDanhSachNguyenLieuXuat(qlCaPheEntities db,DateTime ngayLay)
        {
            List<NguyenLieuXuat> listNguyenLieuXuat = new List<NguyenLieuXuat>();
            //------Lấy danh sách nguyên liệu đã xuất từ ngày đã kiểm kho đến ngày hiện tại
            IEnumerable<object> listNguyenLieuXuatStore = db.laySoLuongNguyenLieuXuatTuNgay(ngayLay);
            //-----Đọc dữ liệu đã lấy và thêm vảo listobject nguyên liệu xuất
            foreach (object itemXuat in listNguyenLieuXuatStore)
            {
                int maNguyenLieuGet = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(itemXuat, "maNguyenLieu"));
                int soLuongXuatGet = xulyDuLieu.doiChuoiSangInteger(xulyDuLieu.layThuocTinhTrongMotObject(itemXuat, "soLuongXuat"));
                long tongTienXuatGet = xulyDuLieu.doiChuoiSangLong(xulyDuLieu.layThuocTinhTrongMotObject(itemXuat, "tongTienXuat"));
                NguyenLieuXuat nguyenLieuXuatGet = new NguyenLieuXuat(maNguyenLieuGet, soLuongXuatGet, tongTienXuatGet);
                listNguyenLieuXuat.Add(nguyenLieuXuatGet);
            }
            return listNguyenLieuXuat;
        }
    }
}