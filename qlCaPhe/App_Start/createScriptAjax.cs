using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.App_Start
{
    /// <summary>
    /// Class thực hiện tạo script ajax thêm vào trang
    /// </summary>
    public class createScriptAjax
    {
        /// <summary>
        /// Hàm thực hiện tạo script vào trang
        /// </summary>
        /// <param name="trang">Trang cần tạo script</param>
        /// <returns></returns>
        public static string taoScript(string trang)
        {
            string kq = "";
            //switch (trang)
            //{
            //    case "tk_TaoMoiTaiKhoan":
            //        kq = scriptTaoMoiThanhVien();
            //        break;
            //}
            return kq;
        }

        /// <summary>
        /// Hàm thực hiện lấy thông tin của 1 đối tượng sau khi lựa chọn trên combobox
        /// </summary>
        /// <param name="action">Phương thức và tham số truyền vào để lấy thông tin
        /// VD: getInfoThanhVienForCreateTaiKhoan?maTV=</param>
        /// <param name="controller">Tên controller chứa phương thức</param>
        /// <returns></returns>
        public static string scriptGetInfoComboboxClick(string idCombobox, string urlAction, string idVungHienThi)
        {
            string kq = "";
            kq += "<script>";
            kq += "     $(document).ready(function (e) {\n";
            kq += "         thucHienAjax();\n";
            kq += "     });\n";
            kq += "     function thucHienAjax() {\n";
            kq += "           $('#" + idCombobox + "').one('change', function (e) { \n";
            kq += "             var ts = $(this).val();\n";
            kq += scriptAjax(urlAction, "GET", "html", "\n  e.preventDefault();\n e.stopImmediatePropagation();\n", "$('#" + idVungHienThi + "').html(data);\n thucHienAjax();\n", "thucHienAjax();\n");
            kq += "         });";
            kq += "     }";
            kq += "</script>";
            return kq;
        }

        /// <summary>
        /// Hàm thực hiện nhúng script thực thi ajax xóa 1 đối tượng vào giao diện table
        /// </summary>
        /// <param name="action">Controller/Action?id=</param>
        /// <returns></returns>
        public static string scriptAjaxXoaDoiTuong(string action)
        {
            string kq = "";
            kq += "<script>";
            kq += "$(document).ready(function () {";
            kq += "     bindingCommand();";
            kq += "});";
            kq += "function bindingCommand() {";
            kq += "     $('.xoa').click(function () {";
            kq += "     var ts = $(this).attr('maXoa');";
            kq += "     $('#modalXoa').find('#xacNhanXoa').attr('maXoa', ts);";
            kq += "     $('#modalXoa').modal();";
            kq += "         return false;";
            kq += "     });";
            kq += "     $('#xacNhanXoa').click(function () {";
            kq += "         var ts = $(this).attr('maXoa');";
            kq+= scriptAjax(action,"GET", "html","$('#xacNhanXoa').unbind('click');\n","location.reload();\n bindingCommand();\n","bindingCommand();\n");
            kq += "     });";
            kq += "}";
            kq += "</script>";
            return kq;
        }
        /// <summary>
        /// Hàm thực hiện tạo script ajax thực hiện upload và crop hình đã up 
        /// </summary>
        /// <param name="folder">Tên folder chứa hình ảnh đã up và đã crop</param>
        /// <param name="widthMin">Chiều dài nhỏ nhất của hình cần cắt</param>
        /// <param name="heightMin">Chiều cao nhỏ nhất của hình cần cắt</param>
        /// <param name="widthMax">Chiều dài lớn nhất của hình cần cắt</param>
        /// <param name="heightMax">Chiều cao lớn nhất của hình cần cắt</param>
        /// <returns></returns>
        public static string scriptAjaxUpLoadAndCropImage(string folder, int widthMin, int heightMin, int widthMax, int heightMax)
        {
            string script = "";
            script += "$(document).ready(function () {\n";
            script += "     $('#btnUpload').click(function () {\n";
            // Checking whether FormData is available in browser
            script += "         if (window.FormData !== undefined) {\n";
            script += "             var fileUpload = $(\"#FileUpload1\").get(0);\n";
            script += "             var files = fileUpload.files;\n";
            // Create FormData object
            script += "             var fileData = new FormData();\n";
            // Looping over all files and add it to FormData object
            script += "             for (var i = 0; i < files.length; i++) {\n";
            script += "                 fileData.append(files[i].name, files[i]);\n";
            script += "             }\n";
            // Adding one more key to FormData object
            script += "             fileData.append('username', 'Manas');\n";
            script += "             $.ajax({\n";
            script += "                 url: '/Ajax/UploadFiles?folder=" + folder + "',\n";
            script += "                 type: \"POST\",\n";
            script += "                 contentType: false, \n";// Not to set any content header
            script += "                 processData: false,\n"; // Not to process data
            script += "                 data: fileData,\n";
            script += "                 success: function (result) {\n";
            script += "                     if (result != \"\")\n";
            script += "                         $('#imgCrop').attr('src', result);\n";
            script += "                     else\n";
            script += "                         $('#hinhDD').attr('src', '/images/gallery-upload-256.png');\n";
            script += "                 },\n";
            script += "                 error: function (err) {\n";
            script += "                     $('#hinhDD').attr('src', '/images/gallery-upload-256.png');\n";
            script += "                 }\n";
            script += "             });\n";
            script += "        } else {\n";
            script += "             alert(\"FormData is not supported.\");\n";
            script += "        }\n";
            script += "     });\n";
            script += "     $('#imgCrop').hover(function () {\n";
            script += "         $('#imgCrop').Jcrop({\n";
            //aspectRatio: 1,
            script += "             minSize: [" + widthMin.ToString() + ", " + heightMin.ToString() + "],\n";
            script += "             maxSize: [" + widthMax.ToString() + ", " + heightMax.ToString() + "],\n";
            script += "             onChange: SelectCropArea,\n";
            script += "             onSelect: SelectCropArea\n";
            script += "         });\n";
            script += "     });\n";
            //Sự kiện click vào nút crop
            script += "     $('#btnCrop').click(function () {\n";
            script += "         var x = $('#x').val();\n";
            script += "         var y = $('#y').val();\n";
            script += "         var w = $('#w').val();\n";
            script += "         var h = $('#h').val();\n";
            ///------Kiểm tra
            script += "         if (w == 0) {\n";
            script += "             $('#thongBao').text(\"Vui lòng lựa chọn vị trí cần lấy trên hình\");\n";
            script += "             return;\n";
            script += "         }\n";
            //Ajax thực hiện crop và save hình
            script += "         $.ajax({\n";
            script += "             url: getDuongDan() + 'Ajax/CropAndSaveImage?x=' + x + '&y=' + y + '&w=' + w + '&h=' + h + '&folder=" + folder + "',\n";
            script += "             beforeSend: function () { },\n";
            script += "             success: function (result) {\n";
            script += "                 $('#hinhDD').attr('src', result.split('||')[0]);\n"; //result.split('|')[0]) là Đường dẫn file hình đã crop trên project: pages/temp/folder...../filehinh... Dùng hiển thị trên giao diện
            script += "                 $('#pathHinh').val(result.split('||')[1])\n;"; //result.split('|')[1] là đường dẫn chứa file hình trên đĩa. Dùng để lưu vào DAtabase
            script += "                 $('#modalChonHinh').modal('hide');\n";
            script += "             },\n";
            script += "             error: function (xhr, textStatus, errorThrown) {\n";
            script += "             }\n";
            script += "         });\n";
            script += "     });\n";
            script += "});\n";
            //Hàm thực hiện gán các giá trị kích thước và vị trí cần lấy trên hình cho các input
            script += "function SelectCropArea(c) {\n";
            script += "     $('#x').val(parseInt(c.x));\n";
            script += "     $('#y').val(parseInt(c.y));\n";
            script += "     $('#w').val(parseInt(c.w));\n";
            script += "     $('#h').val(parseInt(c.h));\n";
            script += "}\n";
            return script;
        }
        /// <summary>
        /// Hàm thực hiện tạo script khi người dùng click vào classClick sẽ hiện modal với nội dung tương ứng
        /// </summary>
        /// <param name="classClick">Class nhận sự kiện click</param>
        /// <param name="idDoiTuong">Tên thuộc tính chứa mã của đối tượng cần lấy thông tin có trong tag classClick</param>
        /// <param name="urlAction">Url gọi đến action thực thi ajax. Có kèm tham số
        ///                         VD: 'DoUong/hienHinhDoUong?maDoUong=</param>
        /// <param name="vungHienThi">id vùng cần hiển thị thông tin trên modal</param>
        /// <param name="idModal">id modal cần hiển thị</param>
        /// <returns></returns>
        public static string scriptAjaxXemChiTietKhiClick(string classClick, string idDoiTuong, string urlAction, string vungHienThi, string idModal)
        {
            string script = "";
            script += "<script>";
            //Script Hiện hình ảnh sản phẩm khi click vào tên hình
            script += "     $(document).ready(function () {\n";
            script += "         $('." + classClick + "').click(function () {\n";
            script += "         var ts = $(this).attr('" + idDoiTuong + "');\n";
            //Thực hiện ajax lấy thông tin hình ảnh sản phẩm
            script += "         $.ajax({\n";
            script += "             url: getDuongDan() + '" + urlAction + "' + ts, \n";
            script += "             type: 'get', \n";
            script += "             beforeSend: function () {\n";
            script += "             },\n";
            script += "             success: function (data, textStatus, xhr) {\n";
            script += "                 $('#" + vungHienThi + "').html(data);\n";
            script += "                 $('#" + idModal + "').modal('show');\n";
            script += "             },\n";
            script += "             error: function (xhr, textStatus, errorThrown) {    \n";
            script += "             }\n";
            script += "         });\n";
            script += "         return false;\n";
            script += "    });\n";
            script += "});\n";
            script += "</script>";
            return script;
        }
        /// <summary>
        /// Hàm thực hiện tạo script để nhúng bản đồ và đánh dấu lên giao diện
        /// </summary>
        /// <param name="kinhDo">Kinh độ điểm cần đánh dấu</param>
        /// <param name="viDo">Vĩ độ điểm cần đánh dấu</param>
        /// <param name="data">Dữ liệu đối tượng cần nhúng. Chú ý gõ đúng GeoLat và GeoLong
        /// VD \"Id\": 1, \"tenKhoHang\": \"" + xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang) + "\", \"diaChi\": \"" + xulyDuLieu.traVeKyTuGoc(kh.diaChi) + "\", \"sdt\": \"" + xulyDuLieu.traVeKyTuGoc(kh.sdt) + "\", \"GeoLong\": \"" + xulyDuLieu.traVeKyTuGoc(kh.kinhDo) + "\", \"GeoLat\": \"" + xulyDuLieu.traVeKyTuGoc(kh.viDo) + "\"</param>
        /// <param name="content">Nội dung hiển thị trên bản đồ khi click vào marker.  Chú ý gõ đúng item.[key_json_data] //Key trong data
        ///  content: \"<div class='infoDiv'><h2>\" + item.tenKhoHang + \"</h2>\" + \"<div><h4>Địa chỉ: \" + item.diaChi + \"</h4><h4>Điện thoại: \" + item.sdt + \"</h4></div></div>\"\n</param>
        /// <returns></returns>
        public static string taoScriptNhungBanDo(string kinhDo, string viDo, string data, string content)
        {
            string script = "";
            script += "<script type=\"text/javascript\">\n";
            script += " $(document).ready(function () {\n";
            script += "     Initialize();\n";
            script += " });\n";
            // Hàm thực hiện tạo bản đồ
            script += " function Initialize() {\n";
            // Google has tweaked their interface somewhat - this tells the api to use that new UI
            script += "     google.maps.visualRefresh = true;\n";
            script += "     var zoneZoom = new google.maps.LatLng(" + kinhDo + ", " + viDo + ");\n";
            // These are options that set initial zoom level, where the map is centered globally to start, and the type of map to show
            script += "     var mapOptions = {\n";
            script += "         zoom: 14,\n";
            script += "         center: zoneZoom,\n";
            script += "         mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP\n";
            script += "     };\n";
            // This makes the div with id \"map_canvas\" a google map
            script += "     var map = new google.maps.Map(document.getElementById(\"map_canvas\"), mapOptions);\n";
            //Tạo dữ liệu thông tin kho hàng và hiển thị
            script += "     var data = [\n";
            // script += "         { \"Id\": 1, \"tenKhoHang\": \"" + xulyDuLieu.traVeKyTuGoc(kh.tenKhoHang) + "\", \"diaChi\": \"" + xulyDuLieu.traVeKyTuGoc(kh.diaChi) + "\", \"sdt\": \"" + xulyDuLieu.traVeKyTuGoc(kh.sdt) + "\", \"GeoLong\": \"" + xulyDuLieu.traVeKyTuGoc(kh.kinhDo) + "\", \"GeoLat\": \"" + xulyDuLieu.traVeKyTuGoc(kh.viDo) + "\" }\n";
            script += "         {" + data + "}";
            script += "     ];\n";
            // Using the JQuery \"each\" selector to iterate through the JSON list and drop marker pins
            script += "     $.each(data, function (i, item) {\n";
            script += "         var marker = new google.maps.Marker({\n";
            script += "         'position': new google.maps.LatLng(item.GeoLong, item.GeoLat),\n";
            script += "         'map': map,\n";
            script += "         'title': item.PlaceName\n";
            script += "     });\n";
            // Make the marker-pin blue!
            script += "     marker.setIcon('http://maps.google.com/mapfiles/ms/icons/blue-dot.png')\n";
            // Hiện thông tin kho hàng khi click vào marker
            script += "     var infowindow = new google.maps.InfoWindow({\n";
            //  script += "         content: \"<div class='infoDiv'><h2>\" + item.tenKhoHang + \"</h2>\" + \"<div><h4>Địa chỉ: \" + item.diaChi + \"</h4><h4>Điện thoại: \" + item.sdt + \"</h4></div></div>\"\n";
            script += content;
            script += "     });\n";
            // finally hook up an \"OnClick\" listener to the map so it pops up out info-window when the marker-pin is clicked!
            script += "     google.maps.event.addListener(marker, 'click', function () {\n";
            script += "         infowindow.open(map, marker);\n";
            script += "     });\n";
            script += "     });\n";
            script += "}\n";
            script += "</script>\n";
            return script;
        }
        /// <summary>
        /// hàm tạo script nhúng API key của Google để hiển thị bản đồ
        /// </summary>
        /// <returns></returns>
        public static string taoScriptGoogleMapAPIKey()
        {
            string key = "AIzaSyDo8qHJoEP_6gaZ0_HDcxeqoVTYhv6oLos";
            string script = "";
            script += " <script async defer src=\"https://maps.googleapis.com/maps/api/js?key=" + key + "&callback=initMap\"type=\"text/javascript\"></script>";
            return script;
        }
        /// <summary>
        /// Hàm thực hiện tạo $ajax cho các trang
        /// </summary>
        /// <param name="urlAction">Phương thức thực hiện ajax có dạng: Controller/fucntion?id=</param>
        /// <param name="method">Method thực hiện ajax là GET hay POST</param>
        /// <param name="dataType">Loại dữ liệu phục vụ cho ajax html hay xml</param>
        /// <param name="beforeSend">Những hành động xảy ra trước khi gửi vào ajax. Có thể là unbind</param>
        /// <param name="idVungHienThi">id vùng hiển thị dữ liệu sau khi đã ajax</param>
        /// <param name="sucess">Hành động xảy ra sau khi ajax thành công</param>
        /// <param name="error">Hành động xảy ra sau khi ajax thất bại</param>
        /// <returns>Script $.ajax dùng để nhúng vào script</returns>
        public static string scriptAjax(string urlAction, string method, string dataType, string beforeSend, string sucess, string error)
        {
            string kq = "";
            kq += "$.ajax({\n";
            kq += "     url: getDuongDan() + '" + urlAction + "' + ts,";
            kq += "     type: '"+method+"',\n";
     //       kq += "     data: 'qlChiNhanh='+ ts,\n";
            kq += "     context: this, \n";
            kq += "     dataType: '"+dataType+"',\n";
            kq += "     beforeSend: function () {\n";
            //kq += "$('#vungDL').html('<div class=\"bdgLoading\" style=\"display:block;\"></div>');\n";
            kq +=           beforeSend;
            kq += "     },\n";
            kq += "     success: function (data, textStatus, xhr) {\n";
            //kq += "         $('#"+idVungHienThi+"').html(data);\n";
            kq += "         "+sucess+"\n";
            kq += "     },\n";
            kq += "     error: function (xhr, textStatus, errorThrown) {\n";
            kq += "         "+error+";\n";
            kq += "     }\n";
            kq += "});\n";
            return kq;
        }
    }
}