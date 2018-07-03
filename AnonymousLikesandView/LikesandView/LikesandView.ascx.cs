
using System;
using System.Web.UI
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace AnonymousLikesandView.LikesandView
{
    [ToolboxItemAttribute(false)]
    public partial class LikesandView : WebPart
    {
        // 仅当使用检测方法对场解决方案进行性能分析时才取消注释以下 SecurityPermission
        // 特性，然后在代码准备进行生产时移除 SecurityPermission 特性
        // 特性。因为 SecurityPermission 特性会绕过针对您的构造函数的调用方的
        // 安全检查，不建议将它用于生产。
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public LikesandView()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string urlStr =Request.Url.ToString();
            string itemId = GetQueryString("ID", urlStr);
        }

        public void GetLikes(SPListItem item)
        {

        }
        public void GetViews(SPListItem item)
        {

        }

        public SPListItem GetItemById(int itemId, string listName)
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPListItem item = null;
            //SPSecurity.RunWithElevatedPrivileges(delegate ()
            //{
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList spList = spWeb.Lists.TryGetList(listName);
                        if (spList != null)
                        {
                            item = spList.GetItemById(itemId);
                        }
                    }
                }
            //});
            return item;
        }


        /// <summary>
        /// 获取url字符串参数，返回参数值字符串
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="url">url字符串</param>
        /// <returns></returns>
        public string GetQueryString(string name, string url)
        {
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", System.Text.RegularExpressions.RegexOptions.Compiled);
            System.Text.RegularExpressions.MatchCollection mc = re.Matches(url);
            foreach (System.Text.RegularExpressions.Match m in mc)
            {
                if (m.Result("$2").Equals(name))
                {
                    return m.Result("$3");
                }
            }
            return "";
        }

    }
}
