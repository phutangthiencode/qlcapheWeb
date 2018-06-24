using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using qlCaPhe.App_Start;
using qlCaPhe.Models;

namespace qlCaPhe.App_Start
{
    /// <summary>
    /// Class tạo các biểu đồ 
    /// </summary>
    public class createScriptCarvas
    {
        /// <summary>
        /// Hàm tạo script để load biểu đồ
        /// </summary>
        /// <param name="data">Mảng danh sách các biểu đồ cần tạo</param>
        /// <returns></returns>
        public static string loadCarvas(string[] listCanvas)
        {
            string kq = "";
            kq += "   window.onload = function () {";
            foreach (string i in listCanvas)
                kq += i;
            kq += "   };";
            return kq;
        }
        /// <summary>
        /// Hàm khởi tạo các biểu đồ 
        /// </summary>
        /// <param name="idCarvas">id của div hiện biểu đồ</param>
        /// <param name="nameDataConfig">Tên biến config dữ liệu biểu đồ</param>
        /// <returns></returns>
        public static string khoiTaoCarvas(string idCarvas, string nameDataConfig)
        {
            string kq = "";
            kq += " var " + idCarvas + " = document.getElementById(\"" + idCarvas + "\").getContext(\"2d\");";
            kq += "   window.myLine = new Chart(" + idCarvas + ", " + nameDataConfig + ");";
            return kq;
        }
        /// <summary>
        /// Hàm tạo dữ liệu cho biểu đồ dạng MỘT ĐƯỜNG  
        /// </summary>
        /// <param name="idNameConfig">Tên biến tạo cấu hình</param>
        /// <param name="labels">Mảng danh sách các label chứa giá trị <para/> sẽ hiển thị dưới mỗi item đường</param>
        /// <param name="labelDataSets">Tên của mỗi đường khi người dùng HOVER vào 1 item giá trị trên biểu đồ</param>
        /// <param name="mauSac">Màu sắc cho đường trên bản đồ <para /> VD: red, blue, yellow, pink</param>
        /// <param name="datas">Mảng danh sách dữ liệu để vẽ lên biểu đồ</param>
        /// <param name="xLabel">Tên hoành độ</param>
        /// <param name="yLabel">Tên trung độ</param>
        /// <returns></returns>
        public static string taoBieuDoMotDuong(string idNameConfig, List<string> labels, string labelDataSets, string mauSac, List<string> datas, string xLabel, string yLabel)
        {
            string kq = "";
            kq += "var " + idNameConfig + " = {";
            kq += "   type: 'line',";
            kq += "     data: {";
            kq += "         labels: [";
            foreach (string label in labels)
                kq += "\"" + label + "\",";
            kq += "         ],";
            kq += "         datasets: [{";
            kq += "         label: \"" + labelDataSets + "\",";
            kq += "         backgroundColor: window.chartColors." + mauSac + ",";
            kq += "         borderColor: window.chartColors." + mauSac + ",";
            kq += "         data: [";
            foreach (string data in datas)
                kq += data + ",";
            kq += "         ],";
            kq += "         fill: false,}]";
            kq += "     },";
            kq += "     options: {";
            kq += "         responsive: true,";
            kq += "         tooltips: {";
            kq += "             mode: 'index', intersect: false,";
            kq += "         },";
            kq += "         hover: {";
            kq += "             mode: 'nearest', intersect: true";
            kq += "         },";
            kq += "         scales: {";
            kq += "             xAxes: [{";
            kq += "                 display: true,";
            kq += "                 scaleLabel: {";
            kq += "                     display: true, labelString: '" + xLabel + "'";
            kq += "                  }";
            kq += "              }],";
            kq += "         yAxes: [{";
            kq += "             display: true,";
            kq += "             scaleLabel: {";
            kq += "                 display: true,labelString: '" + yLabel + "'";
            kq += "             }";
            kq += "         }]";
            kq += "     }";
            kq += " }";
            kq += "};";
            return kq;
        }

        /// <summary>
        /// Hàm tạo script ajax gửi lên request yêu cầu lấy danh sách hóa đơn 
        /// </summary>
        /// <param name="url">Url ứng với request yêu cầu lấy danh sách</param>
        /// <param name="chartID"></param>
        /// <returns></returns>
        public static string ScriptAjaxThongKeDoanhThu(string url, string chartID)
        {
            string kq = "";
            kq += "<script>";
            kq += "    function jsonThongKeDoanhThuTheoTuan(handleData, ts) {";
            kq += "        $.ajax({";
            kq += "            url: \""+url+"\",";
            kq += "            data: 'param=' + ts,";
            kq += "            type: \"GET\",";
            kq += "            contentType: \"application/json; charset=utf-8\",";
            kq += "            dataType: \"json\",";
            kq += "            async: true,";
            kq += "            beforeSend: function () {";
            kq += "                $('#"+chartID+"').html('');";
            kq += "                $('.page-loader-wrapper').attr('style', 'display:block');";
            kq += "            },";
            kq += "            success: function (result) {";
            kq += "                handleData(result);";
            kq += "                $('.page-loader-wrapper').attr('style', 'display:none');";
            kq += "            },";
            kq += "            error: function (errormessage) {";
            kq += "            }";
            kq += "        });";
            kq += "        return false;";
            kq += "    }";
            kq += "</script>";
            return kq;
        }
    }
}