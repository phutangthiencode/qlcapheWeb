using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities.Services
{
    public class svKhuVuc
    {
        public int maKhuVuc { get; set; }
        public string tenKhuVuc { get; set; }
        public string dienGiai { get; set; }
        public int dienTich { get; set; }
        public int tongSucChua { get; set; }
        public string ghiChu { get; set; }
    }
}