using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.App_Start
{
    /// <summary>
    /// Class chứa các thuộc tính cấu hình lên nút chức năng cho bảng
    /// </summary>
    public class NutChucNang
    {
        private string idNut;

        public string IdNut
        {
            get { return idNut; }
            set { idNut = value; }
        }

        private int thuTuSapXep;

        public int ThuTuSapXep
        {
            get { return thuTuSapXep; }
            set { thuTuSapXep = value; }
        }

        private int loaiNut;

        public int LoaiNut
        {
            get { return loaiNut; }
            set { loaiNut = value; }
        }
        private string icon;

        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string thamSo;

        public string ThamSo
        {
            get { return thamSo; }
            set { thamSo = value; }
        }
        private string urlAction;

        public string UrlAction
        {
            get { return urlAction; }
            set { urlAction = value; }
        }
        /// <summary>
        /// Hàm tạo một nút chức năng
        /// </summary>
        //public static string taoNutSua(string urlAction, string thamSo, string )
        //{

        //}

        /// <summary>
        /// Hàm tạo nút chức năng được cấu hình với 7 tham số
        /// </summary>
        /// <param name="idNut">Tên nút để xác định nút [Bắt buộc] <para/> VD: "chinhSua"</param>
        /// <param name="thuTu">Thứ tự để sắp xếp nút trên buttonList <para/> VD: 1, 2..</param>
        /// <param name="loaiNut">Loại nút để cấu hình chức năng: <para/> 1: Chỉnh sửa, 2: Chuyển trạng thái, 3: Xóa bỏ</param>
        /// <param name="icon">iconname cho nút <para/> VD: mode_edit</param>
        /// <param name="title">Tên hiển thị cho nút <para/> VD: Chỉnh sửa, xóa bỏ..</param>
        /// <param name="thamSo">Tham số truyền vào request </param>
        /// <param name="urlAction">Action thực hiện hành động khi nhấn vào nút</param>
        public NutChucNang(string idNut, int thuTu, int loaiNut, string icon, string title, string thamSo, string urlAction)
        {
            this.IdNut = idNut;
            this.ThuTuSapXep = thuTu;
            this.LoaiNut = loaiNut;
            this.Icon = icon;
            this.Title = title;
            this.ThamSo = thamSo;
            this.UrlAction = urlAction;
        }
        /// <summary>
        /// Hàm tạo nút chức năng với 6 tham số
        /// </summary>
        /// <param name="idNut">Tên nút để xác định nút [Bắt buộc] <para/> VD: "chinhSua"</param>
        /// <param name="thuTu">Thứ tự để sắp xếp nút trên buttonList <para/> VD: 1, 2..</param>
        /// <param name="loaiNut">Loại nút để cấu hình chức năng: <para/> 1: Chỉnh sửa, 2: Chuyển trạng thái, 3: Xóa bỏ</param>
        /// <param name="icon">iconname cho nút <para/> VD: mode_edit</param>
        /// <param name="title">Tên hiển thị cho nút <para/> VD: Chỉnh sửa, xóa bỏ..</param>
        /// <param name="thamSo">Tham số truyền vào request </param>
        public NutChucNang(string idNut, int thuTu, int loaiNut, string icon, string title, string thamSo)
        {
            this.IdNut = idNut;
            this.ThuTuSapXep = thuTu;
            this.LoaiNut = loaiNut;
            this.Icon = icon;
            this.Title = title;
            this.ThamSo = thamSo;
        }
    }
}