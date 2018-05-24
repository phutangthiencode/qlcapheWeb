using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace qlCaPhe.Models
{
    /// <summary>
    /// Class thực hiện nhận và xử lý chuỗi trên Editor textbox
    /// </summary>
    public class EditorHTML
    {

        /// <summary>
        /// Thuộc tính chứa dữ liệu có cả html nhập vào textbox editor
        /// </summary>
        [Display(Name = "EditorHTML Details")]
        [Required(ErrorMessage = "EditorHTML Details is required.")]
        [AllowHtml]
        public string noiDungHTML { get; set; }
    }
}