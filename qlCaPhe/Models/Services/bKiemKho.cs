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
public class bKiemKho : System.Web.Services.WebService
{
    /// <summary>
    /// Hàm lấy số lượng lý thuyết tất cả nguyên liệu tồn kho tại thời điểm hiện tại đến trước đây 1 tháng
    /// </summary>
    /// <returns>List object CtTonKho</returns>
    [WebMethod]
    public List<svCtTonKho> layTonKhoLyThuyet()
    {
        List<svCtTonKho> kq = new List<svCtTonKho>();
        try
        {
            List<ctTonKho> listTon = new bTonKho().layDanhSachTon();
            //------Lặp qua danh sách tồn kho theo tháng
            foreach (ctTonKho item in listTon)
            {
                svCtTonKho ctKQ = new svCtTonKho();
                ctKQ.donGia = item.donGia;
                ctKQ.maNguyenLieu = item.maNguyenLieu;
                ctKQ.maSoKy = item.maSoKy;
                ctKQ.nguyenNhanHaoHut = item.nguyenNhanHaoHut;
                ctKQ.soLuongCuoiKyLyThuyet = (int)item.soLuongCuoiKyLyThuyet;
                ctKQ.soLuongDauKy = (int)item.soLuongDauKy;
                ctKQ.soLuongThucTe = (int)item.soLuongThucTe;
                ctKQ.tenNguyenLieu = item.nguyenLieu.tenNguyenLieu;
                ctKQ.tyLeHaoHut = (double)item.tyLeHaoHut;
                ctKQ.hinhNguyenLieu = item.nguyenLieu.hinhAnh;
                ctKQ.donViPhaChe = item.nguyenLieu.donViPhaChe;
                ctKQ.donViHienThi = item.nguyenLieu.donViHienThi;
                kq.Add(ctKQ);
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bKiemKho  - Function: layTonKhoLyThuyet", ex.Message);
        }
        return kq;
    }

    /// <summary>
    /// Hàm tạo services thêm mới 1 tồn kho mới trong CSDL
    /// </summary>
    /// <param name="tenDangNhap">Tên đăng nhập: người kiểm kho</param>
    /// <returns>Mã số kỳ mới nhất vừa được thêm</returns>
    [WebMethod]
    public int themMoiTonKho(string tenDangNhap)
    {
        int kq = 0;
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            TonKho tonKho = new TonKho();
            tonKho.ngayKiem = DateTime.Now;
            tonKho.tongTien = 0;
            tonKho.nguoiKiem = tenDangNhap;
            tonKho.ghiChu = "";
            db.TonKhoes.Add(tonKho);
            int kqLuu = db.SaveChanges();
            if (kqLuu > 0)
                kq = tonKho.maSoKy;
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bKiemKho  - Function: themMoiTonKho", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm tạo một services cho phép thêm mới 1 dòng vào bảng ctTonKho
    /// </summary>
    /// <param name="maSoKy">Mã số kỳ cần thêm</param>
    /// <param name="maNguyenLieu">mã nguyên liệu cần thêm</param>
    /// <param name="donGia">Đơn giá của sản phẩm vừa thêm</param>
    /// <param name="soLuongDauKy">Số lượng tồn kho đầu kỳ</param>
    /// <param name="soLuongCuoiKyLyThuyet">Số lượng tồn kho lý thuyết</param>
    /// <param name="soLuongThucTe">Số lượng tồn kho thực tế</param>
    /// <param name="tyLeHaoHut">Tỷ lệ phần trăm hao hụt</param>
    /// <param name="nguyenNhan">Nguyên nhân bị hao hụt</param>
    /// <returns>2: Thành công </returns>
    [WebMethod]
    public int themMoiChiTietTonKho(int maSoKy, int maNguyenLieu, long donGia, double soLuongDauKy, double soLuongCuoiKyLyThuyet, double soLuongThucTe, double tyLeHaoHut, string nguyenNhan)
    {
        int kq = 0;
        try
        {
            qlCaPheEntities db = new qlCaPheEntities();
            ctTonKho ctAdd = new ctTonKho();
            ctAdd.maSoKy = maSoKy;
            ctAdd.maNguyenLieu = maNguyenLieu;
            ctAdd.donGia = donGia;
            ctAdd.soLuongDauKy = soLuongDauKy;
            ctAdd.soLuongCuoiKyLyThuyet = soLuongCuoiKyLyThuyet;
            ctAdd.soLuongThucTe = soLuongThucTe;
            ctAdd.tyLeHaoHut = tyLeHaoHut;
            ctAdd.nguyenNhanHaoHut = nguyenNhan;
            db.ctTonKhoes.Add(ctAdd);
            kq = db.SaveChanges();
            kq += this.capNhatTongTienTonKho(ctAdd, db);
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bKiemKho  - Function: themMoiChiTietTonKho", ex.Message);
        }
        return kq;
    }
    /// <summary>
    /// Hàm thực hiện cập nhật lại tổng tiền tồn kho.
    /// </summary>
    /// <param name="ct">Object chi tiết tồn kho để lấy các thuộc tính cần thiết</param>
    /// <param name="db"></param>
    /// <returns>1: Cập nhật thành công - 2: thất bại</returns>
    public int capNhatTongTienTonKho(ctTonKho ct, qlCaPheEntities db)
    {
        int kq = 0;
        try
        {
            TonKho tkUpdate = db.TonKhoes.SingleOrDefault(s => s.maSoKy == ct.maSoKy);
            if (tkUpdate != null)
            {
                ct.nguyenLieu = db.nguyenLieux.SingleOrDefault(s => s.maNguyenLieu == ct.maNguyenLieu);
                double soLuongTinh = new bNguyenLieu().chuyenDoiDonViNhoSangLon(ct.soLuongThucTe, ct.nguyenLieu);
                tkUpdate.tongTien += (long)(ct.donGia * soLuongTinh);
                db.Entry(tkUpdate).State = System.Data.Entity.EntityState.Modified;
                kq = db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bKiemKho  - Function: capNhatTongTienTonKho", ex.Message);
        }
        return kq;
    }

    /// <summary>
    /// Hàm tạo service nhận thông báo nhắc nhở kiểm kho gửi đến app
    /// </summary>
    /// <returns></returns>
    [WebMethod]
    public List<svThongBao> getListThongBaoKiemKho()
    {
        List<svThongBao> kq = new List<svThongBao>();
        try
        {
            cauHinh ch = new qlCaPheEntities().cauHinhs.First();
            if (ch != null)
                if (DateTime.Now >= ch.batDauKiem && DateTime.Now <= ch.ketThucKiem)
                {
                    svThongBao itemKQ = new svThongBao();
                    itemKQ.daXem = false;
                    itemKQ.ghiChu = "Thông báo kiểm kho";
                    itemKQ.maThongBao = 1;
                    itemKQ.ndThongBao = "Đến đợt kiểm kê kho hàng " + ch.batDauKiem.ToString() + " - " + ch.ketThucKiem.ToString();
                    kq.Add(itemKQ);
                }
        }
        catch (Exception ex)
        {
            xulyFile.ghiLoi("Class: bKiemKho - Function: getListThongBaoKiemKho", ex.Message);
        }
        return kq;
    }
}
