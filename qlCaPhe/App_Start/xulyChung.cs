using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using qlCaPhe.Models;

namespace qlCaPhe.App_Start
{
    public class xulyChung
    {
        /// <summary>
        /// Hàm thực hiện trả về đường dẫn chứa domain name hiện tại
        /// </summary>
        /// <returns></returns>
        public static string layTenMien()
        {
            return HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";
        }
        /// <summary>
        /// Hàm thực hiện lấy đường dẫn hiện tại trên host của project
        /// </summary>
        /// <returns></returns>
        public static string layDuongDanHost()
        {
            return AppDomain.CurrentDomain.BaseDirectory;

        }
        /// <summary>
        /// Hàm tạo chuỗi xử lý nghiệp vụ khi cần chuyển đổi sang trang khác <para/> Có dạng: page=ten_Trang_Can_Den|request=gia_Tri_Tham_So_Can_Truyen
        /// <para /> Chuỗi đã được mã hóa với khóa là ngày hiện tại <para/> Có thể gán vào thuộc tính tag của 1 htmltag
        /// </summary>
        /// <param name="page">Tên trang cần chuyển đến <para/> ví dụ: /DoUong/du_ChinhSuaDoUong</param>
        /// <param name="request">Tham số cần truyền đi</param>
        /// <returns>Chuỗi urlAction đã mã hóa để gán vào task</returns>
        public static string taoUrlCoTruyenThamSo(string page, string request)
        {
            string kq = "";
            //------kq có dạng page=/DoUong/du_ChinhSuaDoUong|request=req1;req2...
            kq+="page=" + page + "|request=" + request;
            //------Mã hóa chuỗi url
            kq = xulyMaHoa.EncryptWithKey(kq, DateTime.Now.ToShortDateString());
            return kq; //----Chuỗi đã mã hóa
        }
        /// <summary>
        /// Hàm nhận tham số từ sesison request để xác định tham số cần thực hiện....
        /// </summary>
        /// <returns>Chuỗi tham số</returns>
        public static string nhanThamSoTrongSession()
        {
            //---------Lấy session
            HttpSessionStateBase Session = new HttpSessionStateWrapper(HttpContext.Current.Session);
            string urlAction = (string)Session["urlAction"]; //urlAction có dạng: page=tv_ChinhSuaThanhVien|request=maTV            
            //-----Xử lý request trong session
            urlAction = urlAction.Split('|')[1];  //urlAction có dạng: request=maTV
            urlAction = urlAction.Replace("request=", ""); //urlAction có dạng: maTV
            return urlAction;
        }

        /// <summary>
        /// Hàm kiểm tra quyền hạn tài khoản đăng nhập có được phép truy cập vào trang
        /// </summary>
        /// <param name="idMenu">ID menu của trang dựa có trong file menuTools</param>
        /// <returns>True: Được truy cập <para/> False: không được truy cập</returns>
        public static bool duocTruyCap(string idMenu)
        {
            try
            {
                HttpSessionStateBase Session = new HttpSessionStateWrapper(HttpContext.Current.Session);
                taiKhoan tkLogin = (taiKhoan)Session["login"];
                if (tkLogin.tenDangNhap != null)
                {
                    string quyenHan = xulyDuLieu.traVeKyTuGoc(tkLogin.nhomTaiKhoan.quyenHan);
                    if (quyenHan.Contains(":" + idMenu)) //------Nếu như quyền hạn có menu trang cần truy cập
                        return true;
                }
                //-------Chuyển đến trang báo lỗi quyền truy cập khi không có quyền hạn
                HttpContext.Current.Response.Redirect(xulyChung.layTenMien() + "/Home/h_AccessDenied", true);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: xulyChung - Function: duocTruyCap", ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Hàm kiểm tra quyền hạn tài khoản đăng nhập được phép cập nhật thông tin
        /// </summary>
        /// <param name="idMenu">ID menu của trang dựa có trong file menuTools</param>
        /// <param name="soQuyen">Số quyền cập nhật: <para/> 2: Cập nhật <para/> 3: Cập nhật - xóa <para/> 6: Cập nhật - Đọc</param>
        /// <returns>True: Được phép thực thi <para/> false: không được phép</returns>
        public static bool duocCapNhat(string idMenu, string soQuyen)
        {
            try
            {
                if (soQuyen.Equals("6") || soQuyen.Equals("2") || soQuyen.Equals("3") || soQuyen.Equals("7"))
                {
                    HttpSessionStateBase Session = new HttpSessionStateWrapper(HttpContext.Current.Session);
                    taiKhoan tkLogin = (taiKhoan)Session["login"];
                    if (tkLogin.tenDangNhap != null)
                    {
                        string quyenHan = xulyDuLieu.traVeKyTuGoc(tkLogin.nhomTaiKhoan.quyenHan);
                        if (quyenHan.Contains(":" + idMenu + "&" + soQuyen)) //------Nếu như quyền hạn có menu trang cần truy cập
                            return true;
                    }
                    //-------Chuyển đến trang báo lỗi quyền truy cập khi không có quyền hạn
                    HttpContext.Current.Response.Redirect(xulyChung.layTenMien() + "/Home/h_AccessDenied", false);
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: xulyChung - Function: duocCapNhat", ex.Message);

            }
            return false;
        }

        /// <summary>
        /// Hàm thực hiện tạo giao diện thông tin thành viên
        /// </summary>
        /// <param name="thanhVien">Object chứa thông tin thành viên</param>
        /// <returns>Chuỗi html tạo hình ảnh thành viên - Họ tên, Phone</returns>
        public static string DrawInforThanhVien(thanhVien thanhVien)
        {
            string html = "";
            html += "<img id=\"hinhDD\" class='img img-responsive img-thumbnail'src=\"" + string.Format("data:image/png;base64,{0}", Convert.ToBase64String(thanhVien.hinhDD)) + "\" width=\"250px\" height=\"auto\" />";
            html += "<br />";
            html += "<p class=\"font-bold col-pink\">Thành viên: " + xulyDuLieu.traVeKyTuGoc(thanhVien.hoTV) + " " + xulyDuLieu.traVeKyTuGoc(thanhVien.tenTV) + "</p>";
            html += "<p class=\"font-bold col-blue\">Phone: " + xulyDuLieu.traVeKyTuGoc(thanhVien.soDT) + "</p>";
            return html;
        }

        /// <summary>
        /// Hàm thực hiện tao phần giao diện cho thành viên dựa tên tài khoản
        /// </summary>
        /// <param name="tk">Object tài khoản chứa thông tin thành viên</param>
        /// <returns>Chuỗi html tạo hình ảnh thành viên - Họ tên - Phone - Tên nhóm tài khoản</returns>
        public static string DrawInforThanhVienWithTaiKhoan(taiKhoan tk)
        {
            string html = "";
            html += DrawInforThanhVien(tk.thanhVien);
            html += "<p class=\"font-bold col-orange \">Chức vụ: " + xulyDuLieu.traVeKyTuGoc(tk.nhomTaiKhoan.tenNhom) + "</p>";
            return html;
        }
        /// <summary>
        /// Hàm thực hiện thêm tham số vào session urlAction
        /// </summary>
        /// <param name="page">Trang cần chuyển</param>
        /// <param name="request">Tham số truyền vào trang</param>
        public static void addSessionUrlAction(string page, string request)
        {
            string param = "page=" + page + "|request=" + request;
            //------Thiết lập lại session
            HttpContext.Current.Session.Remove("urlAction");
            HttpContext.Current.Session.Add("urlAction", "");
            //ssion.Remove("urlAction"); Session.Add("urlAction", "");
            HttpContext.Current.Session["urlAction"] = param;
        }
    }
  
    
}