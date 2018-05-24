using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.Models.Entities
{
    /// <summary>
    /// Class chứa các thuộc tính của 1 menu tool trên trang quản trị
    /// </summary>
    public class MenuTools
    {
        private string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        private string idCha;

        public string IdCha
        {
            get { return idCha; }
            set { idCha = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string link;

        public string Link
        {
            get { return link; }
            set { link = value; }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        private string icon;

        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        private List<MenuTools> listMenuCon;

        public List<MenuTools> ListMenuCon
        {
            get { return listMenuCon; }
            set { listMenuCon = value; }
        }
        private string cssClass;

        public string CssClass
        {
            get { return cssClass; }
            set { cssClass = value; }
        }
        public MenuTools()
        {
            this.Id = "";
            this.IdCha = "";
            this.Link = "";
            this.Name = "";
            this.Description = "";
            this.Icon = "";
            this.CssClass = "";
            this.ListMenuCon = new List<MenuTools>();
        }
    }
}