using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace AnonymousLikesandView.Likes
{
    [ToolboxItemAttribute(false)]
    public class Likes : WebPart
    {
        protected override void CreateChildControls()
        {
            string urlStr = Request.Url.ToString();
            string itemId = GetQueryString("ID", urlStr);
        }

        public SPListItem GetItemById(int itemId)
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPListItem item = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
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
            });
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
