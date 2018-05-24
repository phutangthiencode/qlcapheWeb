using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.App_Start
{
    public class createTableData
    {
        /// <summary>
        /// Hàm tạo li item cho nút chỉnh sửa trên bảng.
        /// <para/> Nút đã được cấu hình icon mode_edit và màu sắc:BLUE
        /// </summary>
        /// <param name="urlAction">action đền view chỉnh sửa <para/> VD: /DoUong/du_ChinhSuaDoUong</param>
        /// <param name="thamSo">Tham số cần truyền vào <para/> VD: 1 (maDoUong)</param>
        /// <returns></returns>
        public static string taoNutChinhSua(string urlAction, string thamSo)
        {
            string kq = "";
            kq+= "<li><a task=\"" + xulyChung.taoUrlCoTruyenThamSo(urlAction, thamSo) + "\" class=\"guiRequest col-blue\"><i class=\"material-icons\">mode_edit</i>Chỉnh sửa</a></li>";
            return kq;
        }
        /// <summary>
        /// Hàm tạo li item cho nút CẬP NHẬT TRẠNG THÁI
        /// <para/> Nút chưa được cấu hình màu và icon
        /// </summary>
        /// <param name="urlAction">Action thực hiện cập nhật <para/> VD: /DoUong/capNhatTrangThai</param>
        /// <param name="thamSo">tham số để xác định dữ liệu cần cập nhật <para/> VD: 1&2 == maDoUong&trangThai</param>
        /// <param name="classColor">Class màu sắc cho nút</param>
        /// <param name="icon">icon hiển thị bên trái nút</param>
        /// <param name="title">Tên nút</param>
        /// <returns>Chuỗi li nút cập nhật</returns>
        public static string taoNutCapNhat(string urlAction, string thamSo, string classColor, string icon, string title)
        {
            string kq = "";
            kq+= "<li><a task=\"" + xulyChung.taoUrlCoTruyenThamSo(urlAction, thamSo) + "\" class=\"guiRequest "+classColor+"\"><i class=\"material-icons\">"+icon+"</i>"+title+"</a></li>";
            return kq;
        }
        /// <summary>
        /// Hàm tạo li item cho nút XÓA BỎ trên bảng <para/> Nút đã được cấu hình icon delete và màu RED
        /// </summary>
        /// <param name="thamSo">Tham số để xác định xóa bỏ dữ liệu <para/> VD: 1 maDoUong</param>
        /// <returns></returns>
        public static string taoNutXoaBo(string thamSo)
        {
            string kq = "";
            kq+= "<li><a  maXoa=\"" + thamSo + "\" href=\"#\" class=\"xoa col-red\"><i class=\"material-icons\">delete</i>Xoá bỏ</a></li>";
            return kq;
        }
    }
}