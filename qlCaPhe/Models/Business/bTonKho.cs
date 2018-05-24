﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.Models;
using qlCaPhe.App_Start;

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
        /// CẦN OPTIMIZE
        /// Hàm lấy danh sách nguyên liệu tồn kho
        /// </summary>
        /// <param name="ngayNhap">Ngày nhập nguyên liệu muốn lấy.</param>
        public List<ctTonKho> layDanhSachTon(DateTime ngayNhap)
        {
            List<ctTonKho> listTonKho = new List<ctTonKho>();
            qlCaPheEntities db = new qlCaPheEntities();
            var listNhap = db.ctPhieuNhapKhoes.Where(ct => ct.phieuNhapKho.ngayNhap >= ngayNhap && ct.phieuNhapKho.ngayNhap <= DateTime.Now).GroupBy(ct => ct.maNguyenLieu).ToList();
            //==================================LẶP QUA DANH SÁCH NHẬP NGUYÊN LIỆU
            //------Lặp qua từng nguyên liệu đã được groupby
            foreach (var itemListNhap in listNhap)
            {
                ctPhieuNhapKho itemNhap = new ctPhieuNhapKho(); itemNhap.soLuongNhap = 0;
                double soLuongXuat = 0;
                //----------Lặp qua danh sách nguyên liệu có trong group
                foreach (var ctNhap in itemListNhap)
                {
                    itemNhap.maPhieu = ctNhap.maPhieu; itemNhap.maNguyenLieu = ctNhap.maNguyenLieu; itemNhap.maNhaCC = ctNhap.maNhaCC; itemNhap.donGiaNhap = ctNhap.donGiaNhap; itemNhap.ghiChu = ctNhap.ghiChu;
                    itemNhap.nguyenLieu = ctNhap.nguyenLieu;
                    itemNhap.soLuongNhap += ctNhap.soLuongNhap;
                }
                var listXuat = db.ctPhieuXuatKhoes.Where(ct => ct.phieuXuatKho.ngayXuat >= ngayNhap && ct.phieuXuatKho.ngayXuat <= DateTime.Now && ct.maNguyenLieu == itemNhap.maNguyenLieu).GroupBy(ct => ct.maNguyenLieu).ToList();
                if (listXuat.Count > 0)
                    //--------Lặp qua danh sách nguyên liệu đã xuất để tính tồn kho
                    foreach (var listXuatItem in listXuat)
                        if (listXuatItem != null)
                            //---------Lặp qua mỗi record nguyên liệu đã được groupby
                            foreach (var ctXuat in listXuatItem)
                                if (ctXuat != null)
                                    soLuongXuat += (double)ctXuat.soLuongXuat;
                //-----------Thêm vào danh sách để hiển thị lên giao diện                    
                listTonKho.Add(this.addCtTonKhoToList(itemNhap, db, soLuongXuat));
            }//------Quay lại lặp nguyên liệu tiếp theo
            return listTonKho;
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