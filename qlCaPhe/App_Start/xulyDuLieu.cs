using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace qlCaPhe.App_Start
{
    public class xulyDuLieu
    {
        ///<summary>
        /// Hàm xử lý các ký tự ( ", <, ,>, ') thành dạng chuẩn cho html
        /// </summary>
        /// <param name="s">Chuổi cần chuyển đổi</param>
        /// <returns></returns>
        public static string xulyKyTuHTML(string s)
        {
            return s.Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("'", "&apos;").Trim();
        }
        /// <summary>
        /// Hàm thực hiện chuyển mã ký tự html thành ký hiệu chuẩn
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string traVeKyTuGoc(string s)
        {
            if (s != null)
                return s.Replace("&quot;", "\"").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&apos;", "'");
            else
                return "";
        }
        /// <summary>
        /// Hàm thực hiện xử lý cắt chuỗi đối với chuỗi dài hơn length ký tự
        /// </summary>
        /// <param name="s">Chuỗi cần cắt</param>
        /// <param name="lenght">Độ dài ký tự cần cắt</param>
        /// <returns>Trả về chuỗi đã cắt.
        /// Nếu chuỗi ngắn hơn lenght ký tự thì trả về chuỗi bình thường
        /// Ngược lại thực hiện lấy length ký tự và thêm vào dấu "..."</returns>
        public static string xulyCatChuoi(string s, int lenght)
        {
            if (s.Length < lenght)
                return s;
            return s.Substring(0,lenght) + "...";
        }
        /// <summary>
        /// Hàm thực hiện chuyển đổi 1 chuỗi thành kiểu Integer
        /// Nếu không chuyển đổi được thì trả về giá trị là 0
        /// </summary>
        /// <param name="s">Chuỗi cần chuyển đổi</param>
        /// <returns>Số Integer đã đổi</returns>
        public static int doiChuoiSangInteger(string s)
        {
            int tempVal;
            int? val = Int32.TryParse(s, out tempVal) ? Int32.Parse(s) : (int?)0;
            return (int) val;
        }
        /// <summary>
        /// Hàm đổi chuỗi sang double
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double doiChuoiSangDouble(string s)
        {
            double tempVal;
            double? val = Double.TryParse(s, out tempVal) ? Double.Parse(s) : (double?)0;
            return (double)val;
        }
        /// <summary>
        /// Hàm thực hiện chuyển đổi chuổi thành datetime
        /// </summary>
        /// <param name="s"></param>
        /// <returns>Trả về kiểu DateTime, nếu không parse được thì trả về Datetime có giá trị 1/1/1900</returns>
        public static DateTime doiChuoiSangDateTime(string s)
        {
            DateTime kq;
            try
            {
                kq = DateTime.Parse(s);
            }
            catch (Exception ex)
            {
                kq = new DateTime(1900, 1, 1);
            }
            return kq;
        }
        /// <summary>
        /// Hàm chuyển chuổi thành dạng Long
        /// </summary>
        /// <param name="s">Chuỗi cần chuyển đổi thành số</param>
        /// <returns>Số đã được chuyển, Nếu chuyển thất bại thì trả về 0</returns>
        public static long doiChuoiSangLong(string s)
        {
            long tempVal;
            long ? kq = Int64.TryParse(s, out tempVal) ? Int64.Parse(s) : (long?)0;
            return (long)kq;
        }

        /// <summary>
        /// Hàm thực hiện chuyển file hình ảnh sang byte array
        /// </summary>
        /// <param name="path">Đường dẫn lưu trữ hình ảnh từ máy tính cục bộ</param>
        /// <returns>Mảng byte chứa hình ảnh</returns>
        public static byte[] chuyenDoiHinhSangByteArray(string path)
        {
            byte[] kq = null;
            try
            {             
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                kq = br.ReadBytes((int)fs.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: xulyDuLieu - Function: chuyenDoiHinhSangByteArray", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm chuyển đổi byte Array thành file hình ảnh và lưu lại
        /// </summary>
        /// <param name="bytes">Byte array hình ảnh của sản phẩm cần chuyển đổi</param>
        /// <param name="path">Đường dẫn đến thư mục lưu trữ hình ảnh (Có kèm tên file hình)</param>
        public static void chuyenByteArrayThanhHinhAndSave(byte[] bytes, string path)
        {
            try
            {
                byte[] imageBytes = bytes;
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true, true);
                Bitmap bmt = (Bitmap)image;
                //Lưu lại đường dẫn hình ảnh trong thư mục tạm
                string pathTemp = path + ".png";
                // Save the image as a png
                bmt.Save(pathTemp, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: xulyDuLieu - Function: chuyenByteArrayThanhHinh", ex.Message);
            }
        }

        /// <summary>
        /// Hàm thực hiện chuyển byte array hình ảnh thành đường dẫn hình cho tag img để hiển thị trên web
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string chuyenByteHinhThanhSrcImage(byte[] bytes)
        {
            return string.Format("data:image/png;base64,{0}", Convert.ToBase64String(bytes));
        }
        /// <summary>
        /// Hàm chuyển đổi chuỗi base64 của hình ảnh thành byte array
        /// </summary>
        /// <param name="strBase64"></param>
        /// <returns></returns>
        public static byte[] chuyenStringBase64ThanhByteArray(string strBase64)
        {
            return Convert.FromBase64String(strBase64);
        }
    }
}