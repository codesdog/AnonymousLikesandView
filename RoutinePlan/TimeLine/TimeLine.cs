using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace RoutinePlan.TimeLine
{
    [ToolboxItemAttribute(false)]
    public class TimeLine : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/RoutinePlan/TimeLine/TimeLineUserControl.ascx";

        protected override void CreateChildControls()
        {
            TimeLineUserControl control = Page.LoadControl(_ascxPath) as TimeLineUserControl;
            if (control!=null)
            {
                control.webObj = this;
            }
            Controls.Add(control);
        }

        private string _siteUrl = "http://ws2018:19568/";
        [Personalizable, WebDisplayName("网址"), WebDescription("网站地址，如：http://va.neu.edu.cn"), WebBrowsable, Category("自定义设置")]
        public string SiteUrl
        {
            get
            {
                return this._siteUrl;
            }
            set
            {
                this._siteUrl = value;
            }
        }


        private string _planList = "个人学习助手";
        [Personalizable, WebDisplayName("助手列表名称"), WebDescription("个人学习助手列表名称，如：个人学习助手"), WebBrowsable, Category("自定义设置")]
        public string PlanList
        {
            get
            {
                return this._planList;
            }
            set
            {
                this._planList = value;
            }
        }

        private int _dayCount = 3;
        [Personalizable, WebDisplayName("天数"), WebDescription("时间轴跨度天数，如：3"), WebBrowsable, Category("自定义设置")]
        public int DayCount
        {
            get
            {
                return this._dayCount;
            }
            set
            {
                this._dayCount = value;
            }
        }

    }
}
