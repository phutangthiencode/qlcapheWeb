using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace qlCaPhe.App_Start
{
    public class xulyFile
    {
        /// <summary>
        /// hàm thực hiện ghi lỗi vào tập tin
        /// </summary>
        /// <param name="viTri">Vị trí xảy ra lỗi: Class, Function</param>
        /// <param name="message">Thông báo lỗi</param>
        public static void ghiLoi(string viTri, string message)
        {
            try
            {
                string path = xulyChung.layDuongDanHost() + "\\pages\\nhatKy\\";
                TextWriter tsw = new StreamWriter(path + "baoLoi.txt", true);
                string kq = "";
                kq += "-----------------------" + DateTime.Now.ToString() + "----------\r\n";
                kq += "Vị trí: " + viTri + " \r\n Lỗi: " + message;
                kq += "\r\n---------------------------------------------------------------\r\n";

                //Writing text to the file.
                tsw.WriteLine(kq);
                //Close the file.
                tsw.Close();
            }
            catch { }
        }
        /// <summary>
        /// Hàm thực hiện chuyển file từ thư mục tạm vào thư mục gốc.
        /// </summary>
        /// <param name="nguon">Đường dẫn thư mục chứa tập tin nguồn</param>
        /// <param name="dich">Đường dẫn thư mục chứa tập tin đích</param>
        public static void saoCheoFile(string nguon, string dich)
        {
            try
            {
                string[] arrNguon = Directory.GetFiles(nguon);
                foreach (string file in arrNguon)
                {
                    FileInfo info = new FileInfo(file);
                    if (!File.Exists(info.Name))
                    {
                        bool exists = System.IO.Directory.Exists(dich);

                        if (!exists)
                            System.IO.Directory.CreateDirectory(dich);
                        File.Copy(file, dich + info.Name);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Hàm xử lý xóa tất cả tập tin trong thư mục và thư mục con của nó
        /// </summary>
        /// <param name="dd">Đường dẫn tuyệt đối cần xóa</param>
        public static void donDepTM(string dd)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(dd);
            try
            {
                if (System.IO.Directory.Exists(dd))
                {

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: xulyFile - Function: donDepTM", ex.Message);
            }
        }


        /// <summary>
        /// hàm thực hiện cop hình ảnh được tải lên host
        /// </summary>
        /// <param name="x">Vị trí tọa độ x</param>
        /// <param name="y">Vị trí tọa độ y</param>
        /// <param name="w">Chiều dài lựa chọn ảnh</param>
        /// <param name="h">Chiều cao lựa chọn ảnh</param>
        /// <param name="urlAnhGoc">Đường dẫn lưu trữ ảnh gốc</param>
        /// <returns>Trả về Bitmap hỉnh ảnh đã crop</returns>
        public static Bitmap cropHinh(string x, string y, string w, string h, string urlAnhGoc)
        {
            Bitmap bitMap = null;
            try
            {
                //Kiểm tra xem hình ảnh gốc còn tồn tại
                if (File.Exists(urlAnhGoc))
                {
                    //---Chuyển file hình gốc thành Image
                    System.Drawing.Image orgImg = System.Drawing.Image.FromFile(urlAnhGoc);

                    //----Khu vực crop hình
                    Rectangle CropArea = new Rectangle(Convert.ToInt32(x),
                        Convert.ToInt32(y), Convert.ToInt32(w), Convert.ToInt32(h));
                    try
                    {
                        //---Crop hình
                        bitMap = new Bitmap(CropArea.Width, CropArea.Height);
                        using (Graphics g = Graphics.FromImage(bitMap))
                        {
                            g.DrawImage(orgImg, new Rectangle(0, 0, bitMap.Width, bitMap.Height), CropArea, GraphicsUnit.Pixel);
                        }
                        return bitMap;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: xulyFile - Function: cropHinh", ex.Message);
            }
            return bitMap;

        }
    }
}