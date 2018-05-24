using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qlCaPhe.App_Start;

namespace qlCaPhe.Controllers
{
    public class AjaxController : Controller
    {

        private static string pathAnhGoc = "";
        private static string tenAnhGoc = "";
        /// <summary>
        /// Hàm thực hiện upload ảnh lên host
        /// </summary>
        /// <returns>Đường dẫn lưu trữ hình ảnh trên host</returns>
        [HttpPost]
        public string UploadFiles(string folder)
        {
            string srcAnhGoc = "";
            // Kiểm tra xem có file ảnh gửi về không 
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    //--Chỉ lấy 1 file duy nhất
                    HttpPostedFileBase file = files[0];
                    string tenTam;
                    tenTam = file.FileName;
                    //---Gán tên file tạm vào biến để thực hiện đọc file này và crop
                    tenAnhGoc = tenTam;
                    //---Gán đường dẫn thư mục ảnh gốc vừa up lên host cho tag img trên view
                    srcAnhGoc = "/pages/temp/" + folder + "/" + tenAnhGoc;
                    //---Xác định đường dẫn lưu trữ file ảnh gốc trên host
                    tenTam = Path.Combine(Server.MapPath("~/pages/temp/" + folder + "/"), tenTam); pathAnhGoc = tenTam;
                    //---Lưu ảnh lên host
                    file.SaveAs(tenTam);
                }
                catch (Exception ex)
                {
                    string mes = ex.Message;
                }
            }
            return srcAnhGoc;
        }
        /// <summary>
        /// Hàm thực hiện crop hình và lưu lại
        /// </summary>
        /// <param name="x">Vị trí tọa độ x</param>
        /// <param name="y">Vị trí tọa độ y</param>
        /// <param name="w">Chiều dài lựa chọn ảnh</param>
        /// <param name="h">Chiều cao lựa chọn ảnh</param>
        /// <returns>Đường dẫn file hình đã crop trên project: pages/temp/folder...../filehinh...
        /// và đường dẫn file hình trên đĩa đã mã hóa C:/Project/page......  </returns>
        public string CropAndSaveImage(string x, string y, string w, string h, string folder)
        {
            string kq = "";
            try
            {
                string tenHinhCrop = "crop_" + tenAnhGoc;
                //Xác định đường dẫn file hình đã crop trên host
                string duongDanHinhCrop = Path.Combine(Server.MapPath("~/pages/temp/" + folder + "/"), tenHinhCrop);
                //----Tiến hành crop hình ảnh
                Bitmap anhCrop = xulyFile.cropHinh(x, y, w, h, pathAnhGoc);
                //---Lưu hình ảnh đã crop trên host
                anhCrop.Save(duongDanHinhCrop);
                //----Lưu lại đường dẫn vào tập tin hình đã crop
             //   xulyChung.pathHinhAnhCrop = duongDanHinhCrop;
                //----Lưu lại đường dẫn hình để hiện lên view
                kq = "/pages/temp/" + folder + "/" + tenHinhCrop;
           //     xulyChung.srcHinhCrop = kq;
                kq += "||" + xulyMaHoa.Encrypt(duongDanHinhCrop);
            }
            catch (Exception ex)
            {

            }
            //---Xóa bỏ dữ liệu tạm
            tenAnhGoc = "";
            pathAnhGoc = "";
            return kq;
        }

        /// <summary>
        /// Hàm xử lý request và chuyển đến trang mong muốn <para /> Request sẽ được lưu vào session
        /// </summary>
        /// <param name="param"></param>
        /// <returns>đường dẫn trang cần chuyển đến</returns>
        public string xulyRequestVaGuiRequest(string param)
        {
            string url = "";
            try
            {
                if (param.Length > 0)
                {
                    //------Thay thế khoảng trắng thành dấu + khi truyền 2 tham số
                    param=param.Replace(" ", "+");
                    //--------Giải mã request
                    param = xulyMaHoa.DecryptWithKey(param, DateTime.Now.ToShortDateString());
                    //------Thiết lập lại session
                    Session.Remove("urlAction"); Session.Add("urlAction", "");
                    Session["urlAction"] = param;

                    string page = param.Split('|')[0];
                    page = page.Replace("page=", "");
                    //------Chuyển đến trang mong muốn
                    url = xulyChung.layTenMien() + page;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: AjaxController - Function: xulyRequest", ex.Message);
            }
            return url;
        }
        /// <summary>
        /// Hàm thêm request vào session
        /// </summary>
        /// <param name="param">Chuỗi cần truyền vào session</param>
        /// <returns>Trả về đường dẫn trang cần lấy sesion</returns>
        public string addSessionRequest(string param)
        {
            //------Thiết lập lại session
            Session.Remove("urlAction"); Session.Add("urlAction", "");
            Session["urlAction"] = param;
            string page = param.Split('|')[0];
            page= page.Replace("page=","");
            return xulyChung.layTenMien() +  page;
        }

        public void AddRequestToSession(string param)
        {
            try
            {
                if (param.Length > 0)
                {
                    //------Thay thế khoảng trắng thành dấu + khi truyền 2 tham số
                    param = param.Replace(" ", "+");
                    //--------Giải mã request
                    param = xulyMaHoa.DecryptWithKey(param, DateTime.Now.ToShortDateString());
                    //------Thiết lập lại session
                    Session.Remove("urlAction"); Session.Add("urlAction", "");
                    Session["urlAction"] = param;
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: AjaxController - Function: AddRequestToSession", ex.Message);
            }
        }
    }
}