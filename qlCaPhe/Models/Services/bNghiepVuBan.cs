using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using qlCaPhe.App_Start;
using qlCaPhe.Models.Entities.Services;
using qlCaPhe.Models.Business;
using qlCaPhe.Models;
using System.Xml.Serialization;

/// <summary>
/// Summary description for bNhanVien
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[SerializableAttribute]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class bNghiepVuBan : System.Web.Services.WebService
{
    /// <summary>
    /// Hàm lấy danh sách các bàn trống dựa trên khu vực
    /// </summary>
    /// <param name="maKV">Mã khu vực cần lấy danh sách</param>
    /// <returns></returns>
    [WebMethod]
    public List<svBanChoNgoi> layDanhSachBanTrong(int maKV)
    {
        List<svBanChoNgoi> kq = new List<svBanChoNgoi>();
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            //-------Lặp qua danh sách bàn KHÔNG có trong hóa đơn tạm. Với trạng thái BÀN ĐƯỢC PHÉP SỬ DỤNG 
            foreach (BanChoNgoi b in db.BanChoNgois.Where(b => b.maKhuVuc == maKV && b.trangThai == 1 && !db.hoaDonTams.Any(hd => hd.maBan == b.maBan)))
            {
                svBanChoNgoi temp = new svBanChoNgoi();
                temp.trangThai = (int)b.trangThai;
                temp.maBan = b.maBan;
                temp.tenBan = b.tenBan;
                temp.ghiChu = b.ghiChu;
                temp.gioiThieu = b.gioiThieu;
                temp.hinhAnh = b.hinhAnh;
                temp.maKhuVuc = b.maKhuVuc;
                temp.sucChua = b.sucChua;
                kq.Add(temp);
            }

        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: layDanhSachBanTrong", ex.Message);
        }
        return kq;
    }

    /// <summary>
    /// Hàm lấy danh sách bàn đã có khách ngồi - ĐANG ĐƯỢC SỬ DỤNG TRONG QUÁN
    /// </summary>
    /// <param name="maKV">Khu vực cần lấy danh sách bàn</param>
    /// <param name="trangThai">Trạng thái cần lấy <para/> 0: Bàn vừa tiếp nhận - 1: Bàn đã order - 2: Bàn chờ thanh toán - 3: Bàn đã thanh toán</param>
    /// <returns>List chứa danh sách object svBanChoNgoi</returns>
    [WebMethod]
    public List<svBanChoNgoi> layDanhSachBanTheoTrangThai(int maKV, int trangThai)
    {
        List<svBanChoNgoi> kq = new List<svBanChoNgoi>();
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            if (trangThai >= 0 && trangThai <= 3)
                //-------Lặp qua danh sách bàn KHÔNG có trong hóa đơn tạm. Với trạng thái BÀN ĐƯỢC PHÉP SỬ DỤNG 
                foreach (hoaDonTam hd in db.hoaDonTams.ToList().Where(t => t.trangThaiHoadon == trangThai && t.BanChoNgoi.maKhuVuc == maKV))
                {
                    svBanChoNgoi temp = new svBanChoNgoi();
                    temp.maBan = hd.BanChoNgoi.maBan;
                    temp.tenBan = hd.BanChoNgoi.tenBan;
                    temp.ghiChu = hd.BanChoNgoi.ghiChu;
                    temp.gioiThieu = hd.BanChoNgoi.gioiThieu;
                    temp.hinhAnh = hd.BanChoNgoi.hinhAnh;
                    temp.maKhuVuc = hd.BanChoNgoi.maKhuVuc;
                    temp.sucChua = hd.BanChoNgoi.sucChua;
                    kq.Add(temp);
                }

        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: layDanhSachBanTheoTrangThai", ex.Message);
        }
        return kq;
    }

    /// <summary>
    /// Hàm lấy tất cả các khu vực khả dụng để hiện lên app
    /// </summary>
    /// <returns></returns>
    [WebMethod]

    public List<svKhuVuc> layDanhSachKhuVuc()
    {
        List<svKhuVuc> kq = new List<svKhuVuc>();
        try
        {
            foreach (khuVuc kv in new qlCaPheEntities().khuVucs.ToList())
            {
                svKhuVuc temp = new svKhuVuc();
                temp.dienGiai = kv.dienGiai;
                temp.dienTich = kv.dienTich;
                temp.ghiChu = kv.ghiChu;
                temp.maKhuVuc = kv.maKhuVuc;
                temp.tenKhuVuc = kv.tenKhuVuc;
                temp.tongSucChua = kv.tongSucChua;
                kq.Add(temp);
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: layDanhSachKhuVuc", ex.Message);
        }
        return kq;
    }

    /// <summary>
    /// Hàm cập nhật TIẾP NHẬN BÀN danh cho webservice
    /// </summary>
    /// <param name="maBan">Mã bàn cần thêm mới vào bảng hoaDonTam</param>
    /// <param name="tenDangNhap">Tên tài khoản thành viên tiếp nhận bàn</param>
    /// <returns>1: Thành công, 2: Thất bại</returns>
    [WebMethod]
    public int tiepNhanBanMoi(int maBan, string tenDangNhap)
    {
        int kq = 0;
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            hoaDonTam hoaDonTam = db.hoaDonTams.SingleOrDefault(s => s.maBan == maBan);
            if(hoaDonTam==null) //-----Bàn này chưa có hóa đơn trong hệ thống (Bàn trống)
                kq=  new bHoaDonTam().themMoiHoaDonTam(db, maBan, tenDangNhap);
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Fucntion: tiepNhanBanMoi", ex.Message);
        }
        return kq;
    }

    /// <summary>
    /// Hàm lấy danh sách tất cả các loại sản phẩm có thể order
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public List<svLoaiSanPham> layDanhSachLoaiSP()
    {
        List<svLoaiSanPham> kq = new List<svLoaiSanPham>();
        try
        {
            //-------Lặp qua danh sách loại có số lượng sản phẩm > 0
            foreach(loaiSanPham l in new qlCaPheEntities().loaiSanPhams.ToList().Where(l=>l.sanPhams.Count>0))
            {
                svLoaiSanPham item = new svLoaiSanPham();
                item.dienGiai = l.dienGiai;
                item.ghiChu = l.ghiChu;
                item.maLoai = l.maLoai;
                item.tenLoai = l.tenLoai;
                kq.Add(item);
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan_service - Function: layDanhSaachLoaiSP", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm lấy danh sách các sản phẩm có thể order dựa vào loại sản phẩm
    /// </summary>
    /// <param name="maLoai"></param>
    /// <returns></returns>
    [WebMethod]
    public List<svSanPham> layDanhSachSanPhamTheoLoai(int maLoai)
    {
        List<svSanPham> kq = new List<svSanPham>();
        try
        {
            //--------Lặp qua danh sách sản phẩm có khả năng cung cấp
            foreach (sanPham sp in new qlCaPheEntities().sanPhams.Where(s => s.maLoai == maLoai && s.trangThai == 1))
            {
                //-----Kiểm tra sản phẩm còn nguyên liêu
                if(new bSanPham().kiemTraSanPhamKhaThi(sp))
                {
                    svSanPham item = new svSanPham();
                    item.donGia = sp.donGia;
                    item.ghiChu = sp.ghiChu;
                    item.hinhAnh = sp.hinhAnh;
                    item.maLoai = sp.maLoai;
                    item.maSanPham = sp.maSanPham;
                    item.moTa = sp.moTa;
                    item.tenSanPham = sp.tenSanPham;
                    item.thoiGianPhaChe = (int) sp.thoiGianPhaChe;
                    item.trangThai = sp.trangThai;
                    kq.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan_service - Function: layDanhSaachLoaiSP", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm cập nhật một hóa đơn đã tiếp nhận sang trạng thái đã order để tiếp nhận pha chế
    /// </summary>
    /// <param name="maBan">Mã bàn (mã hóa đơn tạm) cần cập nhật</param>
    /// <param name="tongTien">Tổng tiền mới cần cập nhật</param>
    /// <param name="ghiChu">Ghi chú của khách hàng về order</param>
    /// <returns>1: Cập nhật thành công</returns>
    [WebMethod]
    public int capNhatDaOrder(int maBan, long tongTien, string ghiChu)
    {
        int kq = -1;
        try {
            qlCaPheEntities db = new qlCaPheEntities();
            hoaDonTam hoaDonSua = db.hoaDonTams.SingleOrDefault(s => s.maBan == maBan);
            if (hoaDonSua != null)
            {
                hoaDonSua.trangThaiHoadon = 1; //--------Cập nhật trạng thái hóa đơn bàn đã order
                hoaDonSua.ngayLap = DateTime.Now; //-----Cập nhật ngày lập hóa đơn
                hoaDonSua.trangthaiphucVu = 0;//---------cập nhật trạng thái pha chế - CHỜ TIẾP NHẬN - Để hiện trong danh sách tiếp nhận pha chế
                hoaDonSua.tongTien = tongTien;
                hoaDonSua.ghiChu = xulyDuLieu.xulyKyTuHTML(ghiChu);
                db.Entry(hoaDonSua).State = System.Data.Entity.EntityState.Modified;
                kq = db.SaveChanges();
                if (kq > 0)
                    //----------Xóa tất cả chi tiết trong hóa đơn
                    foreach (ctHoaDonTam ctXoa in hoaDonSua.ctHoaDonTams.ToList())
                    {
                        db.ctHoaDonTams.Remove(ctXoa);
                        db.SaveChanges();
                    }
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: capNhatDaOrder", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm thêm mới 1 sản phẩm đã order vào bảng ctHoaDonTam
    /// </summary>
    /// <param name="maBan">Mã bàn, mã hóa đơn cần thêm</param>
    /// <param name="maSP">Mã sản phẩm cần thêm</param>
    /// <param name="donGia">Đơn giá của sản phẩm tại thời điểm thêm</param>
    /// <param name="soLuong">Số lượng sản phẩm đã order</param>
    /// <param name="trangThaiPhaChe">Trạng thái pha chế của sản phẩm: Default 0</param>
    /// <returns>1: Thêm thành công</returns>
    [WebMethod]
    public int themChiTietHoaDonTam(int maBan, int maSP, long donGia, int soLuong, int trangThaiPhaChe)
    {
        int kq = 0;
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            ctHoaDonTam ctAdd = new ctHoaDonTam();
            //-----Gán các thuộc tính từ session vào object để thêm vào database
            ctAdd.maSP = maSP;
            ctAdd.donGia = donGia;
            ctAdd.soLuong = soLuong;
            ctAdd.trangThaiPhaChe = trangThaiPhaChe;
            ctAdd.maBan = maBan;
            db.ctHoaDonTams.Add(ctAdd);
            kq = db.SaveChanges();
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: themChiTietHoaDonTam", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm lấy tổng tiền của 1 hóa đơn tạm
    /// </summary>
    /// <param name="maBan"></param>
    /// <returns></returns>
    [WebMethod]
    public long layTongTienHoaDonTam(int maBan)
    {
        long kq = 0;
        try
        {
            kq = (long) new qlCaPheEntities().hoaDonTams.SingleOrDefault(h => h.maBan == maBan).tongTien;
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuban - Function: layTongTienHoaDonTam", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm lấy danh sách các sản phẩm có trong hoaDonTam
    /// </summary>
    /// <param name="maBan">Mã hóa đơn tạm cần lấy</param>
    /// <returns>List object chi tiết hoadonTam</returns>
    [WebMethod]
    public List<svCtHoaDonTam> layCtHoaDonTam(int maBan)
    {
        List<svCtHoaDonTam> kq = new List<svCtHoaDonTam>();
        try
        {
            this.addListCtHoaDonTam(kq, -2, maBan);//------Lấy tất cả các sản phẩm có trong bảng chi tiết của hóa đơn cần xem
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Fucntion: layCtHoaDonTam", ex.Message);
        }

        return kq;
    }
    /// <summary>
    /// Hàm cập nhật thanh toán cho 1 bàn trong hoaDonTam
    /// </summary>
    /// <param name="maBan">Mã bàn cần cập nhật</param>
    /// <returns>1: Thành công - 0 Thất bại</returns>
    [WebMethod]
    public int tiepNhanThanhToan(int maBan)
    {
        int kq = 0;
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            hoaDonTam hdSua = db.hoaDonTams.SingleOrDefault(b => b.maBan == maBan);
            if (hdSua != null)
                //------Xét trạng thái hóa đơn ĐÃ ORDER thì mới được phép cập nhật
                if (hdSua.trangThaiHoadon == 1)
                {
                    hdSua.trangThaiHoadon = 2;//--Cập nhật trạng thái hóa đơn thành đang chờ thanh toán - Để hiển thị trong danh sách hóa đơn chờ thanh toán.
                    db.Entry(hdSua).State = System.Data.Entity.EntityState.Modified;
                    kq = db.SaveChanges();
                }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Fucntion: tiepNhanThanhToan", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm reset bàn sau khi đã thanh toán
    /// </summary>
    /// <param name="maBan">Mã bàn cần reset</param>
    /// <returns>1: Thành công - 2: Thất bại</returns>
    [WebMethod]
    public int resetBan(int maBan)
    {
        int kq = 0;
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            hoaDonTam hoaDonXoa = db.hoaDonTams.SingleOrDefault(h => h.maBan == maBan);
            if (hoaDonXoa != null)
            {
                //-----Nếu trạng thái VỪA TIẾP NHẬN hoặc ĐÃ THANH TOÁN thì mới cho xóa
                if (hoaDonXoa.trangThaiHoadon == 0 || hoaDonXoa.trangThaiHoadon == 3)
                {
                    //------thực hiện xóa dữ liệu trong bảng ctHoaDonTam
                    foreach (ctHoaDonTam ctXoa in db.ctHoaDonTams.ToList().Where(c => c.maBan == hoaDonXoa.maBan))
                    {
                        //-----Xóa chi tiết
                        db.ctHoaDonTams.Remove(ctXoa);
                        db.SaveChanges();
                    }
                    //-----Xóa hoaDonTam
                    db.hoaDonTams.Remove(hoaDonXoa);
                    kq=db.SaveChanges();
                }
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Fucntion: resetBan", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm thực thi đổi bàn
    /// Dựa vào thông tin hoaDonTam Cũ => Tạo mới hoaDonTam mới từ hoaDonTam cũ
    /// Xóa hoaDonTam cũ
    /// không thể chỉnh sửa mã bàn vì ảnh hưởng đến bảng ctHoaDonTam
    /// </summary>
    /// <param name="maBanCu">Mã hóa đơn tạm cũ </param>
    /// <param name="maBanMoi">mã hóa đơn tạm mới</param>
    /// <returns>1: Thành công = 0 Thất bại</returns>
    [WebMethod]
    public int doiBan(int maBanCu, int maBanMoi)
    {
        int kq = 0;
        try {
            qlCaPheEntities db = new qlCaPheEntities();
            hoaDonTam hdCu = db.hoaDonTams.SingleOrDefault(h => h.maBan == maBanCu);
            hoaDonTam hdMoi = db.hoaDonTams.SingleOrDefault(h => h.maBan == maBanMoi);
            //--------Kiểm tra nếu bàn cũ tồn tại và bàn mới không tồn tại trong hoaDonTam
            if (hdCu != null && hdMoi == null)
            {
                hdMoi = new hoaDonTam();
                if (this.themHoaDonChuyenDoi(maBanMoi, hdCu, hdMoi, db) > 0)
                    kq=this.xoaHoaDonCuSauChuyenDoi(db, hdCu);
            }
        
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: doiBan", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm thực hiện chuyển toàn bộ dữ liệu của bảng hoaDonTam từ cũ sang mới
    /// Thực hiện tạo mới hoaDonTam mới và xóa hoaDonTam cũ
    /// </summary>
    /// <param name="maBanMoi">Mã bàn mới dành cho hoaDonTam mới</param>
    /// <param name="hdCu">Object chứa hoaDonTam cũ cần chuyển sang</param>
    /// <param name="hdMoi">Object chứa dữ liệu từ bảng cũ chuyển sang</param>
    /// <param name="db"></param>
    /// <returns>2: Thành công</returns>
    private int themHoaDonChuyenDoi(int maBanMoi, hoaDonTam hdCu, hoaDonTam hdMoi, qlCaPheEntities db)
    {
        int kq = 0;
        hdMoi.ghiChu = hdCu.ghiChu;
        hdMoi.ngayLap = hdCu.ngayLap;
        hdMoi.nguoiPhucVu = hdCu.nguoiPhucVu;
        hdMoi.thoiDiemDen = hdCu.thoiDiemDen;
        hdMoi.tongTien = hdCu.tongTien;
        hdMoi.trangThaiHoadon = hdCu.trangThaiHoadon;
        hdMoi.trangthaiphucVu = hdCu.trangthaiphucVu;
        hdMoi.maBan = maBanMoi;
        db.hoaDonTams.Add(hdMoi);
        kq = db.SaveChanges();
        if(kq>0)
        {
            //-----------Thêm mới dữ liệu chi tiết của hoaDonTam cũ sang hoaDonTam mới
            foreach (ctHoaDonTam ctCu in hdCu.ctHoaDonTams.ToList())
            {
                ctHoaDonTam ctMoi = new ctHoaDonTam();
                ctMoi.donGia = ctCu.donGia;
                ctMoi.maCtTam = ctCu.maCtTam;
                ctMoi.maSP = ctCu.maSP;
                ctMoi.soLuong = ctCu.soLuong;
                ctMoi.trangThaiPhaChe = ctCu.trangThaiPhaChe;
                ctMoi.maBan = hdMoi.maBan;
                db.ctHoaDonTams.Add(ctMoi);
                kq = db.SaveChanges();                
            }
        }
        return kq;
    }
    /// <summary>
    /// Hàm xóa tất cả dữ liệu của hoaDonTam cũ và bảng chi tiết
    /// </summary>
    /// <param name="db"></param>
    /// <param name="hdCu">Hóa đơn cũ cần xóa</param>
    /// <returns>1: Thành công - 2: Thất bại</returns>
    private int xoaHoaDonCuSauChuyenDoi(qlCaPheEntities db, hoaDonTam hdCu)
    {
        int kq = 0, kqXoaCt=0;
        //----------Xóa bảng chi tiết hoaDonTam Cũ
        List<ctHoaDonTam> listChiTietCu = hdCu.ctHoaDonTams.ToList();
        foreach (ctHoaDonTam ctXoa in listChiTietCu)
        {
            db.ctHoaDonTams.Remove(ctXoa);
            kqXoaCt += db.SaveChanges();
        }
        //---------Nếu đã xóa xong tất cả trong chi tiết
        //-------------Xóa bảng hoaDonTam--------------
        if (kqXoaCt == listChiTietCu.Count)
        {
            db.hoaDonTams.Remove(hdCu);
            kq = db.SaveChanges();
        }
        return kq;
    }
    /// <summary>
    /// Service lấy danh sách tất cả các hóa đơn đã pha chế xong và chờ giao cho khách
    /// </summary>
    /// <returns>List object svHoaDonTam</returns>
    [WebMethod]
    public List<svHoaDonTam> layDanhSachPhucVu()
    {
        List<svHoaDonTam> kq = new List<svHoaDonTam>();
        try
        {//----Lặp qua danh sách hoaDonTam có trangthaiphucVu=2 Đã pha chế Danh sách được sort theo ngày lập hóa đơn tăng dần.
            foreach (hoaDonTam hdTam in new qlCaPheEntities().hoaDonTams.Where(h => h.trangthaiphucVu == 2).OrderBy(h => h.ngayLap).ToList())
            {
                svHoaDonTam itemKQ = new svHoaDonTam();
                itemKQ.ghiChu = hdTam.ghiChu;
                itemKQ.maBan = hdTam.maBan;
                itemKQ.ngayLap = (DateTime)hdTam.ngayLap;
                itemKQ.nguoiPhucVu = hdTam.nguoiPhucVu;
                itemKQ.tenBan = hdTam.BanChoNgoi.tenBan;
                itemKQ.hinhAnhBan = hdTam.BanChoNgoi.hinhAnh;
                itemKQ.thoiDiemDen = (DateTime)hdTam.thoiDiemDen;
                itemKQ.tongTien = (long)hdTam.tongTien;
                itemKQ.trangThaiHoadon = (int) hdTam.trangThaiHoadon;
                itemKQ.trangthaiphucVu = (int)hdTam.trangthaiphucVu;
                itemKQ.soLuongSanPham = new bHoaDonTam().layTongSoLuongSanPhamTrongHoaDon(hdTam, 2);
                itemKQ.dienGiaiChiTiet = "";
                //-------Lấy tên sản phẩm có trong hdTam
                foreach (ctHoaDonTam ctTam in hdTam.ctHoaDonTams.ToList())
                    itemKQ.dienGiaiChiTiet += ctTam.sanPham.tenSanPham + " " ; 
                if(itemKQ.dienGiaiChiTiet.Length>57)
                    //-------cắt chuỗi diển giải chỉ lấy 
                    itemKQ.dienGiaiChiTiet = itemKQ.dienGiaiChiTiet.Substring(0, 57) + "..."; //-----Chỉ lấy 60 ký tự trong chuỗi

                kq.Add(itemKQ);

            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: layDanhSachPhucVu", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm lấy danh sách các món cần phục vụ có trong hóa đơn tạm đã chọn
    /// </summary>
    /// <param name="maBan">Mã bàn, mã hóa đơn tạm cần xem chi tiết</param>
    /// <returns></returns>
    [WebMethod]
    public List<svCtHoaDonTam> layChiTietPhucVu(int maBan)
    {
        List<svCtHoaDonTam> kq = new List<svCtHoaDonTam>();
        try{
            //----------Lấy danh sách các món đã pha chế xong
            this.addListCtHoaDonTam(kq, 2, maBan);
        }catch(Exception ex){
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: layChiTietPhucVu", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm lấy danh sách dữ liệu có trong bảng ctHoaDonTam theo trangThaiPhaChe của các item và gán vào List object srCtHoaDonTam
    /// </summary>
    /// <param name="list">List object cần gán dữ liệu vào</param>
    /// <param name="trangThaiPhaChe">Trạng thái pha chế của sản phẩm cần lấy danh sách <para/> -2: Lấy tất cả sản phẩm có trong hóa đơn - >= -1 Lấy theo trạng thái pha chế</param>
    /// <param name="maBan">Mã bàn, mã hóa đơn tạm cần lấy chi tiết</param>
    private void addListCtHoaDonTam(List<svCtHoaDonTam> list, int trangThaiPhaChe, int maBan)
    {
        List<ctHoaDonTam> listChiTiet = new List<ctHoaDonTam>();
        if (trangThaiPhaChe < -1) //----Nếu lấy tất cả sản phẩm trong chi tiết mà không có điều kiện về trangThaiPhaChe
            listChiTiet = new qlCaPheEntities().ctHoaDonTams.Where(c => c.maBan == maBan).ToList();
        else
            listChiTiet = new qlCaPheEntities().ctHoaDonTams.Where(c => c.maBan == maBan && c.trangThaiPhaChe == trangThaiPhaChe).ToList();
        //-------Lặp qua danh sách chi tiết
        foreach (ctHoaDonTam ct in listChiTiet)
        {
            svCtHoaDonTam itemKQ = new svCtHoaDonTam();
            itemKQ.donGia = ct.donGia;
            itemKQ.hinhAnh = ct.sanPham.hinhAnh;
            itemKQ.maBan = ct.maBan;
            itemKQ.maCtTam = ct.maCtTam;
            itemKQ.maSP = ct.maSP;
            itemKQ.soLuong = ct.soLuong;
            itemKQ.tenSP = ct.sanPham.tenSanPham;
            itemKQ.trangThaiPhaChe = ct.trangThaiPhaChe;
            list.Add(itemKQ);
        }
    }
    /// <summary>
    /// Hàm cập nhật 1 hoaDonTam sang trạng thái đã giao sản phẩm
    /// </summary>
    /// <param name="maBan">Mã bàn - Mã hóa đơn cần cập nhật chi tiết</param>
    /// <returns>1: Cập nhật thành công - 0 Thất bại</returns>
    [WebMethod]
    public int capNhatDaGiao(int maBan)
    {
        int kq = 0; ;
        try
        {
            //-----Cập nhật dữ liệu trong chi tiết
            qlCaPheEntities db = new qlCaPheEntities();
            hoaDonTam hdSua = db.hoaDonTams.SingleOrDefault(s => s.maBan == maBan);
            if (hdSua != null)
            {
                int kqLuuChiTiet = 0;
                List<ctHoaDonTam> listChiTiet = hdSua.ctHoaDonTams.Where(c => c.trangThaiPhaChe == 2).ToList();
                //------Chỉ lấy những sản phẩm đã pha chế và cập nhật
                foreach (ctHoaDonTam ct in listChiTiet)
                {
                    ct.trangThaiPhaChe = 3;//Cập nhật sang sản phẩm đã giao
                    db.Entry(ct).State = System.Data.Entity.EntityState.Modified;
                    kqLuuChiTiet+= db.SaveChanges();
                }
                //------Nếu đã cập nhật tất cả chi tiết trong hóa đơn thành công
                if (kqLuuChiTiet == listChiTiet.Count)
                {
                    //------Cập nhật trạng thái phục vụ của hóa đơn
                    hdSua.trangthaiphucVu = -1;//------Cập nhật bàn sang trạng thái chờ order tiếp theo
                    db.Entry(hdSua).State = System.Data.Entity.EntityState.Modified;
                    kq = db.SaveChanges();
                }
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: capNhatDaGiao", ex.Message);
        }
        return kq;
    }

    /// <summary>
    /// Hàm lấy danh sách tất cả số lượng bàn có trong quán theo trạng thái
    /// </summary>
    /// <returns>Danh sách số lượng bàn theo trạng thái theo thứ tự: Bàn trống, chờ order, đã order, chờ thanh toán, đã thanh toán</returns>
    [WebMethod]
    public List<int> thongKeBanTheoTrangThai()
    {
        List<int> kq = new List<int>();
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            //--------Lấy tổng số bàn trống có trong quán
            //-------Lặp qua danh sách bàn KHÔNG có trong hóa đơn tạm. Với trạng thái BÀN ĐƯỢC PHÉP SỬ DỤNG 
            int soluongBanTrong = db.BanChoNgois.Where(b => b.trangThai == 1 && !db.hoaDonTams.Any(hd => hd.maBan == b.maBan)).Count();
            kq.Add(soluongBanTrong);
            //------------Lấy tổng số bàn theo các trạng thái phục vụ
            //-------Lặp qua danh sách trạng thái của hóa đơn tạm để lấy số lượng bàn: Chờ order, đã order, chờ thanh toán, đã thanh toán
            for (int i = 0; i <= 3; i++)
            {
                int soLuongBan = db.hoaDonTams.Where(t => t.trangThaiHoadon == i).Count();
                kq.Add(soLuongBan);
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bNghiepVuBan - Function: thongKeBanTheoTrangThai", ex.Message);
        }
        return kq;
    }
}
