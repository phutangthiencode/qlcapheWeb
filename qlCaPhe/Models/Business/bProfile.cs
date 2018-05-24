using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.Models.Business
{
    /// <summary>
    /// Class xử lý nghiệp vụ liên quan đến profile thành viên hệ thống
    /// </summary>
    public class bProfile
    {
        /// <summary>
        /// Hàm kiểm tra đăng nhập <para/>
        /// Khi thành viên đăng nhập vào hệ thống. Thực hiện giải mã mật khẩu và kiểm tra
        /// </summary>
        /// <param name="tenDangNhap">Tên tài khoản đăng nhập của nhân viên</param>
        /// <param name="matKhau">Mật khẩu đăng nhập của tài khoản (MẬT KHẨU GỐC CHƯA MÃ HÓA)</param>
        /// <returns>0: Đăng nhập thất bại <para/> 1: Đăng nhập thành công <para/> 2: Tài khoản bị cấm</returns>
        public static int kiemTraDangNhap(string tenDangNhap, string matKhau)
        {
            int kq = 0;
            try
            {
                matKhau = xulyMaHoa.Encrypt(matKhau);
                taiKhoan tk = new qlCaPheEntities().taiKhoans.SingleOrDefault(t => t.tenDangNhap == tenDangNhap && t.matKhau == matKhau);
                //-------Kiểm tra đăng nhập
                if (tk != null)
                    kq = tk.trangThai ? 1 : 2; //-----1 Thành công 2 thất bại
                else //--------2. Thất bại-----------------
                    kq = 0;
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bProfile - Function: kiemTraDangNhap", ex.Message);
            }
            return kq;
        }

    }
}