using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace PageHitCount.CountPageHit
{
    [ToolboxItemAttribute(false)]
    public class CountPageHit : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/PageHitCount/CountPageHit/CountPageHitUserControl.ascx";

        protected override void CreateChildControls()
        {
            CountPageHitUserControl control = Page.LoadControl(_ascxPath) as CountPageHitUserControl;
            if (control != null)
            {
                control.webObj = this;
            }
            Controls.Add(control);
        }

        #region 自定义设置

        private string _hostName = "va";
        [Personalizable, WebDisplayName("主机名称"), WebDescription("场所在服务器主机名称，比如：va,fsc"), WebBrowsable, Category("自定义设置")]
        public string HostName
        {
            get
            {
                return this._hostName;
            }
            set
            {
                this._hostName = value;
            }
        }
        #endregion
    }
}
