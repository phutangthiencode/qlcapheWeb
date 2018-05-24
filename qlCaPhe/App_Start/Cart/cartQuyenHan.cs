using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace qlCaPhe.App_Start.Cart
{
    public class cartQuyenHan
    {
        private SortedList _item;

        public SortedList Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public cartQuyenHan()
        {
            this.Item = new SortedList();
        }

        /// <summary>
        /// Hàm thêm một quyền hạn và session
        /// 2|201&7
        /// </summary>
        /// <param name="key">2</param>
        /// <param name="value">201&7</param>
        public void addCart(string key, string value)
        {
            if(!this.Item.ContainsKey(key))
            {
                this.Item.Add(key, value);
            }
            else
            {                //----Trùng key, trùng value, trùng cả quyền hạn (Bỏ chọn 1 chức năng)
                //if (this.Item[key].ToString().Contains(value))
                //{
                //    string rep = this.Item[key].ToString(); //----nhận dữ liệu cũ
                //    rep= rep.Replace(value, "");//------Xóa bỏ dữ liệu cũ
                //    this.Item[key] = rep;
                //}
                //else
                //{
                //    //--------trùng key, trùng value, khác quyền
                //    string idpage = value.Split('&')[0];//----idpage = 201
                //    if (this.Item[key].ToString().Contains(idpage))
                //    {
                //        string vesau = idpage.Split(idpage.ToCharArray())[1]; ;//////&4-3|301&2:.......
                //        string  strSoQuyen= vesau.Substring(1, 1);
                //        int quyen = xulyDuLieu.doiChuoiSangInteger(strSoQuyen);
                //        quyen++;
                //        value = value.
                //    }
                //    //----trùng key nhưng khác value (Có thêm 1 trang mới của nhóm)
                //    value = ":" + value; //-----value = :202&4
                //    this.Item[key] += value; //------value of Item = 201&7:202&4
                //}
            }
        }
    }
} 