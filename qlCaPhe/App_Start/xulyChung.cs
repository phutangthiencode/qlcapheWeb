using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using qlCaPhe.Models;
using System.Net;
using System.Net.Sockets;

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

        #region Nhóm hàm phục vụ ghi nhật ký sử dụng
        /// <summary>
        /// Hàm thực hiện thêm nhật ký vào CSDL
        /// </summary>
        /// <param name="status">Trạng thái thực hiện <para/> 1..6: Truy cập, Thêm, Xóa, Sửa, Xem, Đăng xuất</param>
        /// <param name="noiDung">Nội dung, chức năng thực hiện</param>
        public static void ghiNhatKyDtb(int status, string noiDung)
        {
            try
            {
                taiKhoan tkLogin = (taiKhoan)new HttpSessionStateWrapper(HttpContext.Current.Session)["login"];
                nhatKy nkAdd = new nhatKy();
                nkAdd.tenDangNhap = tkLogin.tenDangNhap;
                nkAdd.thoiDiem = DateTime.Now;
                nkAdd.IP = getLocalIPAddress();
                System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;
                nkAdd.trinhDuyet = browser.Browser;
                nkAdd.OS = GetUserPlatform(HttpContext.Current.Request);
                nkAdd.chucNang = getChucNang(status) + xulyDuLieu.xulyKyTuHTML(noiDung);
                //----Thêm nhật ký vào CSDL
                qlCaPheEntities db = new qlCaPheEntities();
                db.nhatKies.Add(nkAdd);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: xulyChung - Function: ghiNhatKy", ex.Message);
            }
        }
        /// <summary>
        /// Hàm lấy địa chi ip của người dùng truy cập
        /// </summary>
        /// <returns></returns>
        private static string getLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        /// <summary>
        /// Hàm lấy OS người dùng truy cập
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetUserPlatform(HttpRequest request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

            if (ua.Contains("iPad"))
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("iPhone"))
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            //fallback to basic platform:
            return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
        }
        /// <summary>
        /// Hàm lấy OS thiết bị di động
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private static string GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }

        /// <summary>
        /// Hàm xác định chức năng thực hiện của người dùng
        /// </summary>
        /// <param name="status">Trạng thái thực hiện trên chức năng</param>
        /// <returns></returns>
        private static string getChucNang(int status)
        {
            string kq = "( ";
            switch (status)
            {
                case 1: kq += "Truy cập"; break;
                case 2: kq += "Thêm mới"; break;
                case 3: kq += "Xóa bỏ"; break;
                case 4: kq += "Chỉnh sửa"; break;
                case 5: kq += "Xem"; break;
                case 6: kq += "Đăng xuất"; break;
                default: kq += ""; break;
            }
            kq += " ) ";
            return kq;
        }
        #endregion
    }
  
    
}