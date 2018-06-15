using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.App_Start
{
    /// <summary>
    /// Class thực hiện tạo các đoạn html và nhúng vào giao diện
    /// </summary>
    public class createHTML
    {
        public static int pageSize = 10; //-----Thiết lập số lượng phần tử có trên trang
        /// <summary>
        /// hàm thực hiện tạo nội dung thông báo 
        /// </summary>
        /// <param name="doiTuong">Đối tượng cần Lưu </param>
        /// <param name="tenDoiTuong">Tên của đối tượng vừa lưu</param>
        /// <param name="link">Đường dẫn trên thông báo muốn trỏ tới</param>
        /// <returns>Chuỗi html chứa nội dung</returns>
        public static string taoNoiDungThongBao(string doiTuong, string tenDoiTuong, string link)
        {
            string kq = "";
            kq += doiTuong + " &quot;<b>" + tenDoiTuong + "</b>&quot; đã lưu thành công. ";
            kq += "&quot;<a href=\"" + link + "\" class=\"col-red font-bold\">Click vào đây </a>&quot; ";
            kq += "để chuyển đến trang danh sách " + doiTuong;
            return kq;
        }

        /// <summary>
        /// hàm thực hiện tạo modal thông báo thành công kho Thêm mới một đối tượng
        /// </summary>
        /// <param name="noiDung"></param>
        /// <returns></returns>
        public static string taoThongBaoLuu(string noiDung)
        {
            string kq = "";
            kq += scriptHienModal();
            kq += "<div class=\"modal fade\" id=\"modalThongBao\" role=\"dialog\">";
            kq += "     <div class=\"modal-dialog\">";
            kq += "         <div class=\"modal-content\">";
            kq += "                 <div class=\"modal-header\">";
            kq += "                     <button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button>";
            kq += "                     <h4 class=\"modal-title\">Thông báo</h4>";
            kq += "                 </div>";
            kq += "                 <div class=\"modal-body\">";
            kq += "                     <div>" + noiDung;
            kq += "                     </div>";
            kq += "                 </div>";
            kq += "                 <div class=\"modal-footer\">";
            kq += "                     <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Đóng lại</button>";
            kq += "                 </div>";
            kq += "         </div>";
            kq += "     </div>";
            kq += "</div>";
            kq += "<div id=\"hienModal\" data-toggle=\"modal\" data-target=\"#modalThongBao\"></div>";
            return kq;
        }


        /// <summary>
        /// hàm thực hiện tạo đoạn script để hiển thị modal
        /// </summary>
        /// <returns>Chuỗi script thực thi</returns>
        private static string scriptHienModal()
        {
            string kq = "";
            kq += "<script>";
            kq += "     $(function () {";
            kq += "         $('#hienModal').trigger('click');";
            kq += "     });";
            kq += "</script>";
            return kq;
        }

        /// <summary>
        /// Hàm thực hiện tạo modal cảnh báo khi xóa 1 đối tượng
        /// </summary>
        /// <param name="actionController">Action thực hiện việc xóa trong Controller kèm theo id của đối tượng: VD NhomTaiKhoan/xoaNhomTaiKhoan?maNhom=1</param>
        /// <param name="doiTuong">Đối tượng cần xóa</param>
        /// <returns></returns>
        public static string taoCanhBaoXoa(string doiTuong)
        {
            string kq = "";
            kq += "<div id=\"modalXoa\" class=\"modal fade in\" tabindex=\"-1\" role=\"dialog\" style=\"display: none;\" aria-hidden=\"true\">";
            kq += "     <div class=\"modal-dialog\">";
            kq += "         <div class=\"modal-content\">";
            kq += "             <div class=\"modal-header\">";
            kq += "                 <button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button>";
            kq += "                 <h4 class=\"modal-title\">Xác nhận</h4>";
            kq += "             </div>";
            kq += "             <div class=\"modal-body\">";
            kq += "                 <p>Bạn có đồng ý xóa " + doiTuong + " này không?</p>";
            kq += "             </div>";
            kq += "             <div class=\"modal-footer\">";
            kq += "                 <button class=\"btn btn-primary cancel\" data-dismiss=\"modal\" aria-hidden=\"true\"";
            kq += "                         id=\"xacNhanXoa\" maXoa=\"\">";
            kq += "                         Đồng ý";
            kq += "                 </button>";
            kq += "                 <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Đóng</button>";
            kq += "             </div>";
            kq += "         </div>";
            kq += "     </div>";
            kq += "</div>";
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện tạo modal up hình
        /// </summary>
        /// <returns></returns>
        public static string taoModalUpHinh()
        {
            string html = "";
            html += " <div id=\"modalChonHinh\" class=\"modal fade\" role=\"dialog\">";
            html += "       <div class=\"modal-dialog\" style=\"width:800px\">";
            html += "           <div class=\"modal-content\">";
            html += "               <div class=\"modal-header\">";
            html += "                   <button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button>";
            html += "                   <h4 class=\"modal-title\">Chọn hình</h4>";
            html += "               </div>";
            html += "               <div class=\"modal-body\">";
            html += "                   <label for=\"FileUpload1\" class=\"custom-btn-file-upload \">";
            html += "                      <i class=\"fa fa-cloud-upload\"></i> Tải hình";
            html += "                   </label>";
            html += "                   <input type=\"file\" id=\"FileUpload1\" multiple accept=\".png,.jpg,.jpeg,.gif,.tif\" style=\"display:none\" />";
            html += "                   <b class=\"col-red\">Vui lòng chọn hình có kích thước nhỏ hơn 400px và 300px</b>";
            html += "                   <br />";
            html += "                   <input type=\"button\" name=\"btnUpload\" value=\"Upload\" id=\"btnUpload\" class=\"btn btn-success\">";
            html += "                   <br />";
            html += "                   <img src=\"\" id=\"imgCrop\" alt=\"\" />";
            html += "               </div>";
            html += "               <div class=\"modal-footer\">";
            html += "                   <p id=\"thongBao\"></p>";
            html += "                   <input type=\"button\" name=\"btnCrop\" value=\"Hoàn tất\" class=\"btn btn-primary\" data-dismiss=\"modal\" id=\"btnCrop\">";
            html += "                   <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">Close</button>";
            html += "               </div>";
            html += "           </div>";
            html += "           <input type=\"hidden\" name=\"x\" id=\"x\" />";
            html += "           <input type=\"hidden\" name=\"y\" id=\"y\" />";
            html += "           <input type=\"hidden\" name=\"w\" id=\"w\" />";
            html += "           <input type=\"hidden\" name=\"h\" id=\"h\" />";
            html += "       </div>";
            html += "</div>";
            return html;
        }
        /// <summary>
        /// Hảm thực hiện tạo modal chi tiết khi người dùng click vào một thành phần trên giao diện
        /// </summary>
        /// <param name="idModal">Đặt tên id cho modal chi tiết cần tạo</param>
        /// <param name="idVungChiTiet">đặt id cho vùng hiển thị nội dung</param>
        /// <param name="loaiModal">Loại modal cần tạo: 1: NHỎ - 2: VỪA - 3: LỚN - 4: LỚN HẾT CỠ</param>
        /// <returns></returns>
        public static string taoModalChiTiet(string idModal, string idVungChiTiet, int loaiModal)
        {
            string kq = "";
            kq += "<div class=\"modal fade in\" id=\"" + idModal + "\" tabindex=\"-3\" role=\"dialog\" style=\"display: none; padding-right: 17px;\">";
            kq += "     <div class=\"row\">";
            if (loaiModal == 1)
                kq += "     <div class=\"modal-dialog modal-sm\" role=\"document\">";
            else if (loaiModal == 2)
                kq += "     <div class=\"modal-dialog\" role=\"document\">";
            else if (loaiModal == 3)
                kq += "     <div class=\"modal-dialog modal-lg\" role=\"document\">";
            else //Modal maximun
                kq += "     <div class=\"modal-dialog modal-lg\" style=\"width:90%\" role=\"document\">";
            kq += "             <div class=\"modal-content\" id=\"" + idVungChiTiet + "\">";
            kq += "             <!--Vùng hiện thông tin, hình ảnh-->";
            kq += "             </div>";
            kq += "         </div>";
            kq += "     </div>";
            kq += "</div>";
            return kq;
        }

        /// <summary>
        /// Hàm thực hiện tạo chuỗi html cho việc phân trang
        /// </summary>
        /// <param name="soPhanTu">Số lượng tất cả các phần tử có trong list cần phân trang</param>
        /// <param name="soLuongTrenTrang">Số lượng phần từ có trên 1 trang</param>
        /// <param name="soTrangHienHanh">Số trang hiện tại dang đứng</param>
        /// <param name="url">Đường dẫn đến với trang trong danh sách</param>
        /// <returns>Chuỗi html danh sách trang</returns>
        public static string taoPhanTrang(int soPhanTu, int soLuongTrenTrang, int soTrangHienHanh, string url)
        {
            string kq = "";
            if (soPhanTu > soLuongTrenTrang)
            {
                //-----Tính số trang chứa đủ 10 phần tử
                int soLuongTrang = soPhanTu / soLuongTrenTrang;
                //-----tính trang chứa phần tử lẻ còn lại
                if (soPhanTu % soLuongTrenTrang > 0)
                    soLuongTrang++;

                //-------Tạo chuỗi html dựa trên số trang đã xác định
                kq += "<ul class=\"pagination\">";
                //-----Nếu trang hiện hành lớn hơn hoặc bằng 1 thì hiện previous
                int previousPage = soTrangHienHanh - 1;//------Lấy số trang trước đó
                kq += previousPage >= 1 ? "<li class=\"previous\"><a href=\"" + url + "?page=" + previousPage.ToString() + "\">Previous</a></li>" : "";
                for (int i = 1; i <= soLuongTrang; i++)
                    kq += "    <li class=\"" + (i == soTrangHienHanh ? " active" : "") + " \"><a href=\"" + url + "?page=" + i.ToString() + "\">" + i.ToString() + "</a></li>";
                //------------Cấu hình hiện nút next
                int nextPage = soTrangHienHanh + 1;
                kq += soTrangHienHanh < soLuongTrang ? "<li class=\"next\"><a href=\"" + url + "?page=" + nextPage.ToString() + "\">Next</a></li>" : "";
                kq += "</ul>";
            }
            return kq;
        }
        /// <summary>
        /// Hàm tạo 1 toast thông báo để hiển thị
        /// Thông báo được cấu hình hiển thị trong 5s trong javascript
        /// </summary>
        /// <param name="noiDung">Nội dung thông báo</param>
        /// <param name="classMauNen">Class chứa màu nền cho thông báo</param>
        /// <returns></returns>
        public static string taoToastThongBao(string noiDung, string classMauNen)
        {
            string kq = "";
            kq += "<div id=\"notify\" class=\"bootstrap-notify-container alert alert-dismissible p-r-35 animated fadeInDown "+classMauNen+"\"";
            kq += "     style=\"display: none; margin: 0px auto; position: fixed; transition: all 0.5s ease-in-out; z-index: 1031; bottom: 20px; left: 20px;\">";
            kq += "    <span data-notify=\"message\">"+noiDung+"</span>";
            kq += "</div>";
            return kq;
        }
    }
}