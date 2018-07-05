using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace AnonViewandLikes.ViewandLikes
{
    [ToolboxItemAttribute(false)]
    public class ViewandLikes : WebPart
    {
        // 更改可视 Web 部件项目项后，Visual Studio 可能会自动更新此路径。
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/AnonViewandLikes/ViewandLikes/ViewandLikesUserControl.ascx";

        protected override void CreateChildControls()
        {
            ViewandLikesUserControl control = Page.LoadControl(_ascxPath) as ViewandLikesUserControl;
            if (control != null)
            {
                control.webObj = this;
            }
            Controls.Add(control);
        }

        #region 自定义设置

        private string _listName = "导师简介";
        [Personalizable, WebDisplayName("源列表名称"), WebDescription("web部件读取数据所在的列表名称"), WebBrowsable, Category("自定义设置")]
        public string ListName
        {
            get
            {
                return this._listName;
            }
            set
            {
                this._listName = value;
            }
        }

        private string _historyList = "访问历史";
        [Personalizable, WebDisplayName("历史列表名称"), WebDescription("列表访问历史列表名称"), WebBrowsable, Category("自定义设置")]
        public string HistoryList
        {
            get
            {
                return this._historyList;
            }
            set
            {
                this._historyList = value;
            }
        }

        private string _webUrl = "";
        [Personalizable, WebDisplayName("网站路径"), WebDescription("包含数据列表的网站路径"), WebBrowsable, Category("自定义设置")]
        public string WebUrl
        {
            get
            {
                return this._webUrl;
            }
            set
            {
                this._webUrl = value;
            }
        }
        #endregion

        #region 变量
        private string _connStr = "Database=VAExtension;Server=202.118.11.99;User ID=sa;Password=sasasasa;Trusted_Connection=False;";
        public string ConnStr
        {
            get
            {
                return this._connStr;
            }
            set
            {
                this._connStr = value;
            }
        }
        #endregion
    }
}
