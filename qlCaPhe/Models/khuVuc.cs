//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace qlCaPhe.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class khuVuc
    {
        public khuVuc()
        {
            this.BanChoNgois = new HashSet<BanChoNgoi>();
        }
    
        public int maKhuVuc { get; set; }
        public string tenKhuVuc { get; set; }
        public string dienGiai { get; set; }
        public int dienTich { get; set; }
        public int tongSucChua { get; set; }
        public string ghiChu { get; set; }
    
        public virtual ICollection<BanChoNgoi> BanChoNgois { get; set; }
    }
}
