using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities
{
    public class QuyenHan
    {
        private string idMenuCha;

        public string IdMenuCha
        {
            get { return idMenuCha; }
            set { idMenuCha = value; }
        }
        private string idMenuCon;

        public string IdMenuCon
        {
            get { return idMenuCon; }
            set { idMenuCon = value; }
        }
        private int soQuyen;

        public int SoQuyen
        {
            get { return soQuyen; }
            set { soQuyen = value; }
        }

        public QuyenHan()
        {
            this.idMenuCha = "";
            this.idMenuCon = "";
            this.soQuyen = 0;
        }
    }
}