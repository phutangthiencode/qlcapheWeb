﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace qlCaPhe.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class qlCaPheEntities : DbContext
    {
        public qlCaPheEntities()
            : base("name=qlCaPheEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<baiViet> baiViets { get; set; }
        public virtual DbSet<BanChoNgoi> BanChoNgois { get; set; }
        public virtual DbSet<BangGiaoViec> BangGiaoViecs { get; set; }
        public virtual DbSet<caLamViec> caLamViecs { get; set; }
        public virtual DbSet<cauHinh> cauHinhs { get; set; }
        public virtual DbSet<congThucPhaChe> congThucPhaChes { get; set; }
        public virtual DbSet<ctBangGiaoViec> ctBangGiaoViecs { get; set; }
        public virtual DbSet<ctCongThuc> ctCongThucs { get; set; }
        public virtual DbSet<ctCungCap> ctCungCaps { get; set; }
        public virtual DbSet<ctDanhGia> ctDanhGias { get; set; }
        public virtual DbSet<ctDatBan> ctDatBans { get; set; }
        public virtual DbSet<ctHoaDonOrder> ctHoaDonOrders { get; set; }
        public virtual DbSet<ctHoaDonTam> ctHoaDonTams { get; set; }
        public virtual DbSet<ctPhieuNhapKho> ctPhieuNhapKhoes { get; set; }
        public virtual DbSet<ctPhieuXuatKho> ctPhieuXuatKhoes { get; set; }
        public virtual DbSet<ctTonKho> ctTonKhoes { get; set; }
        public virtual DbSet<danhGiaNhanVien> danhGiaNhanViens { get; set; }
        public virtual DbSet<datBanOnline> datBanOnlines { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<hoaDonOrder> hoaDonOrders { get; set; }
        public virtual DbSet<hoaDonTam> hoaDonTams { get; set; }
        public virtual DbSet<khoHang> khoHangs { get; set; }
        public virtual DbSet<khuVuc> khuVucs { get; set; }
        public virtual DbSet<lichSuGia> lichSuGias { get; set; }
        public virtual DbSet<loaiNguyenLieu> loaiNguyenLieux { get; set; }
        public virtual DbSet<loaiSanPham> loaiSanPhams { get; set; }
        public virtual DbSet<mucTieuDanhGia> mucTieuDanhGias { get; set; }
        public virtual DbSet<nguyenLieu> nguyenLieux { get; set; }
        public virtual DbSet<nhaCungCap> nhaCungCaps { get; set; }
        public virtual DbSet<nhatKy> nhatKies { get; set; }
        public virtual DbSet<nhomTaiKhoan> nhomTaiKhoans { get; set; }
        public virtual DbSet<phieuNhapKho> phieuNhapKhoes { get; set; }
        public virtual DbSet<phieuXuatKho> phieuXuatKhoes { get; set; }
        public virtual DbSet<sanPham> sanPhams { get; set; }
        public virtual DbSet<slide> slides { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<taiKhoan> taiKhoans { get; set; }
        public virtual DbSet<thanhVien> thanhViens { get; set; }
        public virtual DbSet<thongBao> thongBaos { get; set; }
        public virtual DbSet<TonKho> TonKhoes { get; set; }
    
        public virtual int getNextIdentify(string tenTable)
        {
            var tenTableParameter = tenTable != null ?
                new ObjectParameter("tenTable", tenTable) :
                new ObjectParameter("tenTable", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("getNextIdentify", tenTableParameter);
        }
    
        public virtual int sp_alterdiagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_alterdiagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_creatediagram(string diagramname, Nullable<int> owner_id, Nullable<int> version, byte[] definition)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var versionParameter = version.HasValue ?
                new ObjectParameter("version", version) :
                new ObjectParameter("version", typeof(int));
    
            var definitionParameter = definition != null ?
                new ObjectParameter("definition", definition) :
                new ObjectParameter("definition", typeof(byte[]));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_creatediagram", diagramnameParameter, owner_idParameter, versionParameter, definitionParameter);
        }
    
        public virtual int sp_demoSoluong(Nullable<System.DateTime> ngayXuat)
        {
            var ngayXuatParameter = ngayXuat.HasValue ?
                new ObjectParameter("ngayXuat", ngayXuat) :
                new ObjectParameter("ngayXuat", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_demoSoluong", ngayXuatParameter);
        }
    
        public virtual int sp_dropdiagram(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_dropdiagram", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_getListNhapKho(Nullable<System.DateTime> ngayXuat, Nullable<System.DateTime> ngayNhap)
        {
            var ngayXuatParameter = ngayXuat.HasValue ?
                new ObjectParameter("ngayXuat", ngayXuat) :
                new ObjectParameter("ngayXuat", typeof(System.DateTime));
    
            var ngayNhapParameter = ngayNhap.HasValue ?
                new ObjectParameter("ngayNhap", ngayNhap) :
                new ObjectParameter("ngayNhap", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_getListNhapKho", ngayXuatParameter, ngayNhapParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagramdefinition_Result> sp_helpdiagramdefinition(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagramdefinition_Result>("sp_helpdiagramdefinition", diagramnameParameter, owner_idParameter);
        }
    
        public virtual ObjectResult<sp_helpdiagrams_Result> sp_helpdiagrams(string diagramname, Nullable<int> owner_id)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_helpdiagrams_Result>("sp_helpdiagrams", diagramnameParameter, owner_idParameter);
        }
    
        public virtual int sp_renamediagram(string diagramname, Nullable<int> owner_id, string new_diagramname)
        {
            var diagramnameParameter = diagramname != null ?
                new ObjectParameter("diagramname", diagramname) :
                new ObjectParameter("diagramname", typeof(string));
    
            var owner_idParameter = owner_id.HasValue ?
                new ObjectParameter("owner_id", owner_id) :
                new ObjectParameter("owner_id", typeof(int));
    
            var new_diagramnameParameter = new_diagramname != null ?
                new ObjectParameter("new_diagramname", new_diagramname) :
                new ObjectParameter("new_diagramname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_renamediagram", diagramnameParameter, owner_idParameter, new_diagramnameParameter);
        }
    
        public virtual int sp_upgraddiagrams()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_upgraddiagrams");
        }
    
        public virtual ObjectResult<thongKeDoanhThuMotSanPhamTheoNgay_Result> thongKeDoanhThuMotSanPhamTheoNgay(Nullable<System.DateTime> ngay, Nullable<int> maSP)
        {
            var ngayParameter = ngay.HasValue ?
                new ObjectParameter("ngay", ngay) :
                new ObjectParameter("ngay", typeof(System.DateTime));
    
            var maSPParameter = maSP.HasValue ?
                new ObjectParameter("maSP", maSP) :
                new ObjectParameter("maSP", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuMotSanPhamTheoNgay_Result>("thongKeDoanhThuMotSanPhamTheoNgay", ngayParameter, maSPParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuMotSanPhamTheoThang_Result> thongKeDoanhThuMotSanPhamTheoThang(Nullable<int> thang, Nullable<int> maSP)
        {
            var thangParameter = thang.HasValue ?
                new ObjectParameter("thang", thang) :
                new ObjectParameter("thang", typeof(int));
    
            var maSPParameter = maSP.HasValue ?
                new ObjectParameter("maSP", maSP) :
                new ObjectParameter("maSP", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuMotSanPhamTheoThang_Result>("thongKeDoanhThuMotSanPhamTheoThang", thangParameter, maSPParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuMotSanPhamTheoTuan_Result> thongKeDoanhThuMotSanPhamTheoTuan(Nullable<System.DateTime> startDate, Nullable<int> maSP)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var maSPParameter = maSP.HasValue ?
                new ObjectParameter("maSP", maSP) :
                new ObjectParameter("maSP", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuMotSanPhamTheoTuan_Result>("thongKeDoanhThuMotSanPhamTheoTuan", startDateParameter, maSPParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuTheoSanPhamTheoNam_Result> thongKeDoanhThuTheoSanPhamTheoNam(Nullable<int> nam)
        {
            var namParameter = nam.HasValue ?
                new ObjectParameter("nam", nam) :
                new ObjectParameter("nam", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuTheoSanPhamTheoNam_Result>("thongKeDoanhThuTheoSanPhamTheoNam", namParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuTheoSanPhamTheoNgay_Result> thongKeDoanhThuTheoSanPhamTheoNgay(Nullable<System.DateTime> ngay)
        {
            var ngayParameter = ngay.HasValue ?
                new ObjectParameter("ngay", ngay) :
                new ObjectParameter("ngay", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuTheoSanPhamTheoNgay_Result>("thongKeDoanhThuTheoSanPhamTheoNgay", ngayParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuTheoSanPhamTheoQuy_Result> thongKeDoanhThuTheoSanPhamTheoQuy(Nullable<int> quy)
        {
            var quyParameter = quy.HasValue ?
                new ObjectParameter("quy", quy) :
                new ObjectParameter("quy", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuTheoSanPhamTheoQuy_Result>("thongKeDoanhThuTheoSanPhamTheoQuy", quyParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuTheoSanPhamTheoThang_Result> thongKeDoanhThuTheoSanPhamTheoThang(Nullable<int> thang)
        {
            var thangParameter = thang.HasValue ?
                new ObjectParameter("thang", thang) :
                new ObjectParameter("thang", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuTheoSanPhamTheoThang_Result>("thongKeDoanhThuTheoSanPhamTheoThang", thangParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuTheoSanPhamTheoTuan_Result> thongKeDoanhThuTheoSanPhamTheoTuan(Nullable<System.DateTime> startDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuTheoSanPhamTheoTuan_Result>("thongKeDoanhThuTheoSanPhamTheoTuan", startDateParameter);
        }
    
        public virtual ObjectResult<thongKeHoaDonTheoNam_Result> thongKeHoaDonTheoNam()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeHoaDonTheoNam_Result>("thongKeHoaDonTheoNam");
        }
    
        public virtual ObjectResult<thongKeHoaDonTheoQuy_Result> thongKeHoaDonTheoQuy(Nullable<int> nam)
        {
            var namParameter = nam.HasValue ?
                new ObjectParameter("nam", nam) :
                new ObjectParameter("nam", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeHoaDonTheoQuy_Result>("thongKeHoaDonTheoQuy", namParameter);
        }
    
        public virtual ObjectResult<thongKeHoaDonTheoThang_Result> thongKeHoaDonTheoThang(Nullable<int> nam)
        {
            var namParameter = nam.HasValue ?
                new ObjectParameter("nam", nam) :
                new ObjectParameter("nam", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeHoaDonTheoThang_Result>("thongKeHoaDonTheoThang", namParameter);
        }
    
        public virtual ObjectResult<thongKeHoaDonTheoTuan_Result> thongKeHoaDonTheoTuan(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeHoaDonTheoTuan_Result>("thongKeHoaDonTheoTuan", startDateParameter, endDateParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuMotSanPhamTheoQuy_Result> thongKeDoanhThuMotSanPhamTheoQuy(Nullable<int> quy, Nullable<int> maSP)
        {
            var quyParameter = quy.HasValue ?
                new ObjectParameter("quy", quy) :
                new ObjectParameter("quy", typeof(int));
    
            var maSPParameter = maSP.HasValue ?
                new ObjectParameter("maSP", maSP) :
                new ObjectParameter("maSP", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuMotSanPhamTheoQuy_Result>("thongKeDoanhThuMotSanPhamTheoQuy", quyParameter, maSPParameter);
        }
    
        public virtual ObjectResult<thongKeDoanhThuMotSanPhamTheoNam_Result> thongKeDoanhThuMotSanPhamTheoNam(Nullable<int> nam, Nullable<int> maSP)
        {
            var namParameter = nam.HasValue ?
                new ObjectParameter("nam", nam) :
                new ObjectParameter("nam", typeof(int));
    
            var maSPParameter = maSP.HasValue ?
                new ObjectParameter("maSP", maSP) :
                new ObjectParameter("maSP", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeDoanhThuMotSanPhamTheoNam_Result>("thongKeDoanhThuMotSanPhamTheoNam", namParameter, maSPParameter);
        }
    
        public virtual ObjectResult<thongKeHoaDonTheoNgay_Result> thongKeHoaDonTheoNgay(Nullable<System.DateTime> ngay)
        {
            var ngayParameter = ngay.HasValue ?
                new ObjectParameter("ngay", ngay) :
                new ObjectParameter("ngay", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeHoaDonTheoNgay_Result>("thongKeHoaDonTheoNgay", ngayParameter);
        }
    
        public virtual ObjectResult<thongKeTongTienNhapKhoTheoNgay_Result> thongKeTongTienNhapKhoTheoNgay(Nullable<System.DateTime> ngay)
        {
            var ngayParameter = ngay.HasValue ?
                new ObjectParameter("ngay", ngay) :
                new ObjectParameter("ngay", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeTongTienNhapKhoTheoNgay_Result>("thongKeTongTienNhapKhoTheoNgay", ngayParameter);
        }
    
        public virtual ObjectResult<thongKeSoLuongNhapKhoTheoNgay_Result> thongKeSoLuongNhapKhoTheoNgay(Nullable<System.DateTime> ngay)
        {
            var ngayParameter = ngay.HasValue ?
                new ObjectParameter("ngay", ngay) :
                new ObjectParameter("ngay", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeSoLuongNhapKhoTheoNgay_Result>("thongKeSoLuongNhapKhoTheoNgay", ngayParameter);
        }
    
        public virtual ObjectResult<thongKeTongTienNhapKhoTheoTuan_Result> thongKeTongTienNhapKhoTheoTuan(Nullable<System.DateTime> startDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeTongTienNhapKhoTheoTuan_Result>("thongKeTongTienNhapKhoTheoTuan", startDateParameter);
        }
    
        public virtual ObjectResult<thongKeSoLuongNhapKhoTheoTuan_Result> thongKeSoLuongNhapKhoTheoTuan(Nullable<System.DateTime> startDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeSoLuongNhapKhoTheoTuan_Result>("thongKeSoLuongNhapKhoTheoTuan", startDateParameter);
        }
    
        public virtual ObjectResult<thongKeTongTienNhapKhoTheoThang_Result> thongKeTongTienNhapKhoTheoThang(Nullable<int> nam)
        {
            var namParameter = nam.HasValue ?
                new ObjectParameter("nam", nam) :
                new ObjectParameter("nam", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeTongTienNhapKhoTheoThang_Result>("thongKeTongTienNhapKhoTheoThang", namParameter);
        }
    
        public virtual ObjectResult<thongKeSoLuongNhapKhoTheoThang_Result> thongKeSoLuongNhapKhoTheoThang(Nullable<byte> thang)
        {
            var thangParameter = thang.HasValue ?
                new ObjectParameter("thang", thang) :
                new ObjectParameter("thang", typeof(byte));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeSoLuongNhapKhoTheoThang_Result>("thongKeSoLuongNhapKhoTheoThang", thangParameter);
        }
    
        public virtual ObjectResult<thongKeTongTienXuatKhoTheoNgay_Result> thongKeTongTienXuatKhoTheoNgay(Nullable<System.DateTime> ngay)
        {
            var ngayParameter = ngay.HasValue ?
                new ObjectParameter("ngay", ngay) :
                new ObjectParameter("ngay", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeTongTienXuatKhoTheoNgay_Result>("thongKeTongTienXuatKhoTheoNgay", ngayParameter);
        }
    
        public virtual ObjectResult<thongKeSoLuongXuatKhoTheoNgay_Result> thongKeSoLuongXuatKhoTheoNgay(Nullable<System.DateTime> ngay)
        {
            var ngayParameter = ngay.HasValue ?
                new ObjectParameter("ngay", ngay) :
                new ObjectParameter("ngay", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeSoLuongXuatKhoTheoNgay_Result>("thongKeSoLuongXuatKhoTheoNgay", ngayParameter);
        }
    
        public virtual ObjectResult<thongKeSoLuongXuatKhoTheoTuan_Result> thongKeSoLuongXuatKhoTheoTuan(Nullable<System.DateTime> startDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeSoLuongXuatKhoTheoTuan_Result>("thongKeSoLuongXuatKhoTheoTuan", startDateParameter);
        }
    
        public virtual ObjectResult<thongKeTongTienXuatKhoTheoTuan_Result> thongKeTongTienXuatKhoTheoTuan(Nullable<System.DateTime> startDate)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<thongKeTongTienXuatKhoTheoTuan_Result>("thongKeTongTienXuatKhoTheoTuan", startDateParameter);
        }
    }
}
