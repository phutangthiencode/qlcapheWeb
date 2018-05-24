using qlCaPhe.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using qlCaPhe.App_Start;

namespace qlCaPhe.Models.Business
{
    public class bMenuTools
    {
        /// <summary>
        /// Hàm thêm các thuộc tính vào object menuTools
        /// </summary>
        /// <param name="menu">Object menu cần thêm thuộc tính</param>
        /// <param name="xmlReader">object xml để lấy thuộc tính trong xml</param>
        /// <param name="listAdd">Danh sách menu cần thêm vào</param>
        public void addAttributesToObjectMenu(MenuTools menu, XmlReader xmlReader, List<MenuTools> listAdd)
        {
            menu.Id = xmlReader.GetAttribute("id");
            menu.IdCha = xmlReader.GetAttribute("idCha");
            menu.CssClass = xmlReader.GetAttribute("cssClass");
            menu.Link = xmlReader.GetAttribute("link");
            menu.Name = xmlReader.GetAttribute("name");
            menu.Description = xmlReader.GetAttribute("description");
            menu.Icon = xmlReader.GetAttribute("icon");
            listAdd.Add(menu);
        }
        /// <summary>
        /// Hàm thêm các thuộc tính vào object menuTools
        /// </summary>
        /// <param name="menu">Object menu cần thêm thuộc tính</param>
        /// <param name="XmlNode">object xml để lấy thuộc tính trong xml</param>
        /// <param name="listAdd">Danh sách menu cần thêm vào</param>
        public void addAttributesToObjectMenu(MenuTools menu, XmlNode node, List<MenuTools> listAdd)
        {
            menu.Id = node.Attributes["id"].Value;
            menu.IdCha = node.Attributes["idCha"].Value;
            menu.CssClass = node.Attributes["cssClass"].Value;
            menu.Link = node.Attributes["link"].Value;
            menu.Name = node.Attributes["name"].Value;
            menu.Description = node.Attributes["description"].Value;
            menu.Icon = node.Attributes["icon"].Value;
            listAdd.Add(menu);
        }
        /// <summary>
        /// Hàm thêm các thuộc tính vào object menuTools
        /// </summary>
        /// <param name="menu">Object menu cần thêm thuộc tính</param>
        /// <param name="XmlElement">object xml để lấy thuộc tính trong xml</param>
        /// <param name="listAdd">Danh sách menu cần thêm vào</param>
        public void addAttributesToObjectMenu(MenuTools menu, XmlElement xmlElement, List<MenuTools> listAdd)
        {
            menu.Id = xmlElement.GetAttribute("id");
            menu.IdCha = xmlElement.GetAttribute("idCha");
            menu.CssClass = xmlElement.GetAttribute("cssClass");
            menu.Link = xmlElement.GetAttribute("link");
            menu.Name = xmlElement.GetAttribute("name");
            menu.Description = xmlElement.GetAttribute("description");
            menu.Icon = xmlElement.GetAttribute("icon");
            listAdd.Add(menu);
        }


        /// <summary>
        /// Hàm trả về danh sách menu dựa vào QUYÊN HẠN
        /// </summary>
        /// <param name="listQuyenHan">Có dạng: item[0] = 1|101&7:102&4 <para/> item[1] = 2|201&5</param>
        /// <returns></returns>
        public List<MenuTools> readMenuToolsWithPermission(List<string> listQuyenHan)
        {
            List<MenuTools> kq = new List<MenuTools>();
            try
            {
                foreach (string groupPage in listQuyenHan) //groupPage =  1|101&7:102&4 LẶP QUA TỪNG CỤM MENU CHA
                {
                    string idCha = groupPage.Split('|')[0];//idCha = 1
                    string strListCon = groupPage.Split('|')[1];//--strListCon = 101&7:102&4 
                    string[] itemCon = strListCon.Split(':'); //-------item[0] = 101&7
                    List<string> listStringCon = new List<string>();
                    foreach (string i in itemCon) //--------LẶP ĐỂ THÊM VÀO DANH SÁCH CÁC MENU CON 
                        listStringCon.Add(i.Split('&')[0]); //------listStringCon[0] = 101

                    //---------đọc xml
                    string pathXMLFile = xulyChung.layDuongDanHost() + "pages/settings/menuTools.xml";
                    XmlDocument xml = new XmlDocument(); xml.Load(pathXMLFile);
                    foreach (XmlNode nodeCha in xml.SelectNodes("/root/menuItem[@id=" + idCha + "]")) //---------Lặp qua danh sách node cha
                    {
                        MenuTools menuCha = new MenuTools();
                        this.addAttributesToObjectMenu(menuCha, nodeCha, kq); //----Thêm menu cha vào danh sách menu
                        //-----------Lặp qua danh sách các page con 
                        foreach (string idPageChild in listStringCon)
                        {
                            if (!idPageChild.Equals(""))
                            {
                                XmlElement elementChild = (XmlElement)xml.SelectSingleNode("/root/menuItem/menuItem[@id=" + idPageChild + "]");
                                if (elementChild != null)
                                {
                                    MenuTools menuCon = new MenuTools();
                                    this.addAttributesToObjectMenu(menuCon, elementChild, menuCha.ListMenuCon); //--------Thêm menu con vào listmenucon của menu cha
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                xulyFile.ghiLoi("Class: bMenuTools - Function: readMenuToolsWithPermission", ex.Message);
            }
            return kq;
        }
        /// <summary>
        /// Hàm lấy tất cả danh sách menu 
        /// </summary>
        /// <returns></returns>
        public List<MenuTools> readAllMenuTools()
        {
            List<MenuTools> listMenu = new List<MenuTools>();
            string pathXMLFile = xulyChung.layDuongDanHost() + "/pages/settings/menuTools.xml";
            XmlReader xmlReader = XmlReader.Create(pathXMLFile);

            MenuTools menuCha = null;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "menuItem"))
                {
                    //------Lấy danh sách menu cha
                    if (xmlReader.HasAttributes)
                    {
                        if (xmlReader.GetAttribute("idCha") == "0") //--------Là cha
                        {
                            menuCha = new MenuTools();
                            this.addAttributesToObjectMenu(menuCha, xmlReader, listMenu);
                        }
                        if (xmlReader.GetAttribute("idCha") == menuCha.Id) //--------Là con
                        {
                            MenuTools menuCon = new MenuTools();
                            this.addAttributesToObjectMenu(menuCon, xmlReader, menuCha.ListMenuCon);
                        }
                    }
                }
            }
            return listMenu;
        }
    }
}