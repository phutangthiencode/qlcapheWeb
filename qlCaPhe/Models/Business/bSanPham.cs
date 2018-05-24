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
                        if (ctTon != null)
                        {
                            int soLuongThucTe = (int)ctTon.soLuongThucTe; //-----Lấy số lượng thực tế trong tồn kho
                            ////////////------------------Lấy danh sách xuất kho từ thời điểm kiểm kê kho đến nay=> để lấy Số lượng tồn
                            var listXuat = db.ctPhieuXuatKhoes.Where(ct => ct.phieuXuatKho.ngayXuat >= tonKhoNew.ngayKiem && ct.phieuXuatKho.ngayXuat <= DateTime.Now && ct.maNguyenLieu == ctCongThuc.maNguyenLieu).GroupBy(ct => ct.maNguyenLieu);
                            double soLuongXuat = 0;
                            //--------Lặp qua danh sách nguyên liệu đã xuất để tính tồn kho
                            foreach (var listXuatItem in listXuat)
                                foreach (var ctXuat in listXuatItem)
                                    soLuongXuat += (double)ctXuat.soLuongXuat; //-----Lấy tổng số lượng xuất kho
                            //----------Tính số lượng tồn = số lượng trong kho - số lượng đã xuất
                            if (soLuongThucTe - soLuongXuat < soLuongSuDung)
                                return false;
                        }
                        else
                            return false;
                    }
                }
            return true;
        }
    }
}