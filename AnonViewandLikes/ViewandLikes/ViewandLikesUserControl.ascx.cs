using Microsoft.SharePoint;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;

namespace AnonViewandLikes.ViewandLikes
{
    public partial class ViewandLikesUserControl : UserControl
    {
        public ViewandLikes webObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            string urlStr = Request.Url.ToString();
            int itemId = int.Parse(GetQueryString("ID", urlStr));
            string listName = webObj.ListName;
            GetandUpdateItemById(itemId, listName);
        }

        public void GetandUpdateItemById(int itemId, string listName)
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
                            spList.AllowEveryoneViewItems = true;
                            item = spList.GetItemById(itemId);
                            spWeb.AllowUnsafeUpdates = true;
                            if (item != null)
                            {
                                string likeCount= item["点赞数"] != null ? item["点赞数"].ToString() : "0";

                                string hitCount = item["阅读量"] != null ? item["阅读量"].ToString() : "0";
                                item["阅读量"] = int.Parse(hitCount) + 1;
                                item.Update();
                                lbViews.Text = (int.Parse(hitCount) + 1).ToString();
                                lbLikes.Text = likeCount;
                            }
                            else
                            {
                                ultools.Visible = false;
                            }
                        }
                    }
                }
            });
        }

        public void UpdateItemById()
        {

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
