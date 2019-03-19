using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Apart.ApartUC
{
    public partial class ApartUCUserControl : UserControl
    {
        public ApartUC webObj;
        protected Label lbtxt;
        protected void Page_Load(object sender, EventArgs e)
        {
            string zj = webObj.HostName;
            lbtxt.Text ="主机名称："+zj+"；现在时刻："+ DateTime.Now.ToString();
        }
    }
}
