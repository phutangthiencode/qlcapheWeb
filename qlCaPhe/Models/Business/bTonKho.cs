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
    /// Class thực thi cách hành động liên quan đến kho
    /// </summary>
    public class bTonKho
    {
        /// <summary>
        /// Hàm lấy tổng số lượng của nguyên liệu thực tế đang tồn trong kho.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="maNguyenLieu">Mã nguyên liệu cần kiểm tra tồn kho</param>
        /// <returns></returns>
        public double laySoLuongNguyenLieuTonThucTeTrongKho(int maNguyenLieu, qlCaPheEntities db)
        {
            double soLuongTon = 0;
            //--------Lấy thông tin tồn kho mới nhất.
            TonKho tonKhoNew = db.TonKhoes.OrderByDescending(t => t.maSoKy).FirstOrDefault();
            if (tonKhoNew != null)
            {
                //------Lấy thông tin tồn kho của nguyên liệu
                ctTonKho ctTonKhoThucTe = tonKhoNew.ctTonKhoes.SingleOrDefault(ct => ct.maNguyenLieu == maNguyenLieu);
                if (ctTonKhoThucTe != null)
                {   //---------Lấy số lượng thực tế của nguyên liệu trong kỳ kiểm tra mới nhất.
                    double soLuongThucTe = (double)ctTonKhoThucTe.soLuongThucTe;
                    ////////////------------------3. Lấy danh sách xuất kho từ thời điểm kiểm kê kho đến nay
                    var listXuat = db.ctPhieuXuatKhoes.Where(ct => ct.phieuXuatKho.ngayXuat >= tonKhoNew.ngayKiem && ct.phieuXuatKho.ngayXuat <= DateTime.Now && ct.maNguyenLieu == maNguyenLieu).GroupBy(ct => ct.maNguyenLieu);
                    double soLuongDaXuat = 0;
                    //--------Lặp qua danh sách nguyên liệu đã xuất để tính tồn kho
                    foreach (var listXuatItem in listXuat)
                        foreach (var ctXuat in listXuatItem)
                            soLuongDaXuat += (double) ctXuat.soLuongXuat;
                    //-------Tính số lượng tồn kho
                    soLuongTon = soLuongThucTe - soLuongDaXuat;
                }
            }
            return soLuongTon;
        }
        /// <summary>
        /// Hàm lấy đơn giá nhập nguyên liệu tại thời điểm kiểm kê kho mới nhất
        /// </summary>
        /// <param name="maNguyenLieu"></param>
        /// <returns></returns>
        public long layDonGiaNguyenLieuTonKho(int maNguyenLieu)
        {
            try
            {
                //----------Lấy thông tin tồn kho mới nhất
                TonKho tonKhoNew = new qlCaPheEntities().TonKhoes.OrderByDescending(t => t.maSoKy).FirstOrDefault();
                if (tonKhoNew != null)
                {
                    //------Lấy thông tin nguyên liệu trong tồn kho
                    ctTonKho ctTonKhoNew = tonKhoNew.ctTonKhoes.SingleOrDefault(ct => ct.maNguyenLieu == maNguyenLieu);
                    if (ctTonKhoNew != null)
                        //---------Lấy số lượng thực tế của nguyên liệu trong kỳ kiểm tra mới nhất.
                        return ctTonKhoNew.donGia;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bTonKho - Function: layDonGiaNguyenLieuTonKho", ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// Hàm lấy danh sách nguyên liệu tồn kho thực tế
        /// </summary>
        /// <returns>Trả về danh sách các nguyên liệu tồn kho thực tế</returns>
        public List<ctTonKho> layDanhSachTon()
        {
            List<ctTonKho> kq = new List<ctTonKho>();
            try
            {
                qlCaPheEntities db = new qlCaPheEntities();
                //------Lấy thông tin đợt kiểm kho gần nhất
                TonKho tonKho = db.TonKhoes.OrderByDescending(t=>t.maSoKy).FirstOrDefault();
                if (tonKho != null)
                {
                    List<ctPhieuNhapKho> listNguyenLieuNhapTrongKy = new bNguyenLieu().layDanhSachNguyenLieuNhapTrongKy(db,tonKho.ngayKiem);
                    List<NguyenLieuXuat> listNguyenLieuXuat = new bNguyenLieu().layDanhSachNguyenLieuXuat(db, tonKho.ngayKiem);
                    //-------Đọc tất cả các nguyên liệu
                    foreach (nguyenLieu nlItem in db.nguyenLieux.Where(n=>n.trangThai==true).ToList())
                    {
                        double soLuongXuat = 0, soLuongNhap = 0, soLuongThucTe = 0, soLuongDauKy = 0, donGiaNhap=0;
                        ctTonKho ctKQ = new ctTonKho();
                        ctKQ.nguyenLieu = nlItem;
                        ctPhieuNhapKho nguyenLieuNhap = listNguyenLieuNhapTrongKy.SingleOrDefault(s => s.maNguyenLieu == nlItem.maNguyenLieu);
                        if (nguyenLieuNhap != null)
                        {
                            soLuongNhap = (double)nguyenLieuNhap.soLuongNhap;
                            donGiaNhap = nguyenLieuNhap.donGiaNhap;
                            ctKQ.maNguyenLieu = nguyenLieuNhap.maNguyenLieu;
                        }
                        //--------Kiểm tra để nguyên liệu đang duyệt có trong sổ kho trước đây
                        ctTonKho itemSoKho = tonKho.ctTonKhoes.SingleOrDefault(t => t.maNguyenLieu == nlItem.maNguyenLieu);
                        if (itemSoKho != null)
                        {
                            ctKQ.maNguyenLieu = itemSoKho.maNguyenLieu;
                            //------Số lượng dầu kỳ này là số lượng thực tế cuối kỳ trước
                            soLuongDauKy = (double) itemSoKho.soLuongThucTe;
                            soLuongThucTe = (double) itemSoKho.soLuongThucTe;
                            donGiaNhap = itemSoKho.donGia;
                            //----Kiểm tra nguyên liệu đang duyệt cần kiểm tra có được xuất chưa. 
                            NguyenLieuXuat nguyenLieuXuat = listNguyenLieuXuat.SingleOrDefault(s => s.maNguyenLieu == itemSoKho.maNguyenLieu);
                            if (nguyenLieuXuat != null)
                                soLuongXuat = nguyenLieuXuat.soLuongXuat;
                        }
                        ctKQ.donGia = (long) donGiaNhap;
                        ctKQ.soLuongDauKy = soLuongDauKy;
                        ctKQ.soLuongThucTe = soLuongThucTe;
                        //-------------Số lượng THỰC TẾ TRÊN LÝ THUYẾT là số lượng (CUỐI KỲ TRƯỚC + SỐ LƯỢNG NHẬP(Trong kỳ)) - SỐ LƯỢNG ĐÃ XUẤT(trong kỳ) //=============Trong kỳ: từ ngày kiểm kê cuối cùng đến nay
                        ctKQ.soLuongCuoiKyLyThuyet = (soLuongThucTe + soLuongNhap) - soLuongXuat;
                        ctKQ.tyLeHaoHut = ctKQ.soLuongDauKy - ctKQ.soLuongCuoiKyLyThuyet;
                        kq.Add(ctKQ);
                    }
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bTonKho - Function: layDanhSachTon", ex.Message);
            }
            return kq;
        }



        /// <summary>
        /// Hàm gán dữ liệu cho object cttonkho để thêm vào danh sách hiện lên giao diện
        /// </summary>
        /// <param name="itemNhap">ctPhieuNhap để lấy thông tin nguyên liệu, số lượng nhập.</param>
        /// <param name="db"></param>
        /// <param name="soLuongXuat">Tổng số lượng nguyên liệu đã xuất trong kỳ.</param>
        /// <returns></returns>
        private ctTonKho addCtTonKhoToList(ctPhieuNhapKho itemNhap, qlCaPheEntities db, double soLuongXuat)
        {
            int soLuongCuoiKyTruoc = 0;//, maKyTruoc=0;
            //--------Lấy thông tin tồn kho kỳ trước để xác định SỐ LƯỢNG TỒN CUỒI KỲ TRƯỚC
            TonKho tonKhoKyTruoc = db.TonKhoes.OrderByDescending(t => t.maSoKy).FirstOrDefault();
            if (tonKhoKyTruoc != null)
            {
                ctTonKho ctTonTruoc = tonKhoKyTruoc.ctTonKhoes.SingleOrDefault(c => c.TonKho.maSoKy == tonKhoKyTruoc.maSoKy && c.maNguyenLieu == itemNhap.maNguyenLieu);
                if (ctTonTruoc != null)
                    //----------Lấy số lượng tồn kho cuối kỳ trước làm đầu kỳ này. (Số lượng cuối kỳ trước là số lượng thực tế).
                    soLuongCuoiKyTruoc = (int)ctTonTruoc.soLuongThucTe;
            }

            ctTonKho ctTon = new ctTonKho();
            ctTon.maNguyenLieu = itemNhap.maNguyenLieu;
            ctTon.donGia = itemNhap.donGiaNhap;
            ctTon.soLuongDauKy = soLuongCuoiKyTruoc;
            ctTon.soLuongCuoiKyLyThuyet = itemNhap.soLuongNhap - soLuongXuat;
            ctTon.soLuongThucTe = itemNhap.soLuongNhap - soLuongXuat;
            ctTon.nguyenLieu = itemNhap.nguyenLieu;
            ctTon.tyLeHaoHut = 0;
            return ctTon;
        }

    }
}