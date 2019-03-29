﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Apart.ApartUC
{
    public class ApartUCUserControl:UserControl
    {
        public ApartUC webObj;
        protected Label lbtxt;
        protected Button btn1;
        protected TextBox tbtxt;
        protected void Page_Load(object sender, EventArgs e)
        {
            string zj = webObj.HostName;
            lbtxt.Text = "主机名称：" + zj + "；现在时刻：" + DateTime.Now.ToString();
        }

        protected void btn1_Click(object sender, EventArgs e)
        {
            string zj = webObj.HostName;
            string txt = tbtxt.Text;
            lbtxt.Text = txt+"；主机名称：" + zj + "；现在时刻：" + DateTime.Now.ToString();
        }
    }
}
