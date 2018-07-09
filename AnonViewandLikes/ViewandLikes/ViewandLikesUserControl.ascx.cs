using Microsoft.SharePoint;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

namespace AnonViewandLikes.ViewandLikes
{
    public partial class ViewandLikesUserControl : UserControl
    {
        public ViewandLikes webObj { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string urlStr = Request.Url.ToString();
            int itemId = int.Parse(GetQueryString("ID", urlStr));
            string sourceList = webObj.ListName;
            string historyList = webObj.HistoryList;
            GetandUpdateItemById(itemId,sourceList, historyList);
        }

        public void GetandUpdateItemById(int itemId, string sourceList, string historyList)
        {
            string siteUrl = SPContext.Current.Site.Url;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        if (historyList != "")
                        {

                            SPList spList = spWeb.Lists.TryGetList(historyList);
                            if (spList != null)
                            {
                                spList.AllowEveryoneViewItems = true;
                                string sourceUrl = siteUrl+spList.DefaultViewUrl;
                                string[] slistData = getSourceList(sourceList, itemId,sourceUrl);

                                SPQuery qry = new SPQuery();
                                qry.Query = @" <Where><And><Eq><FieldRef Name='ItemID' /><Value Type='Text'>" + itemId + "</Value></Eq><Eq><FieldRef Name='ListGuid' /><Value Type='Text'>" + slistData[0] + "</Value></Eq></And></Where>";
                                SPListItemCollection listItems = spList.GetItems(qry);
                                if (listItems.Count>0)
                                {
                                    SPListItem item = listItems[0];

                                    //获取访问次数，并显示
                                    string hitCount = item["访问次数"] != null ? item["访问次数"].ToString() : "0";
                                    viewspan.InnerText = "总访问次数：" + (int.Parse(hitCount) + 1).ToString();
                                    string visittime= item["最后访问时间"] != null ? item["最后访问时间"].ToString() : DateTime.Now.ToString();
                                    viewtimespan.InnerText = "最后访问时间：" + visittime;
                                    //更新访问次数历史
                                    spWeb.AllowUnsafeUpdates = true;
                                    item["访问次数"] = int.Parse(hitCount) + 1;
                                    item["最后访问者"] = GetCurrentAccount();
                                    item["最后访问者IP"] = GetClientIPAddr();
                                    item["最后访问时间"] = DateTime.Now;
                                    item.Update();

                                }
                                else
                                {
                                    viewspan.InnerText = "总访问次数：1";
                                    viewtimespan.InnerText = "最后访问时间：" + DateTime.Now.ToString();
                                    spWeb.AllowUnsafeUpdates = true;
                                    SPListItem item = spList.AddItem();
                                    item["标题"] = slistData[2];
                                    item["项目ID"] = itemId;
                                    item["列表Guid"] = slistData[0];
                                    item["列表名称"] = slistData[1];
                                    item["访问次数"] =  1;
                                    item["最后访问者"] = GetCurrentAccount();
                                    item["最后访问者IP"] = GetClientIPAddr();
                                    item["最后访问时间"] = DateTime.Now;

                                    var urlField = spList.Fields["项目Url"] as SPFieldUrl;
                                    SPFieldUrlValue urlFieldValue = new SPFieldUrlValue()
                                    {
                                        Description = slistData[2],
                                        Url = slistData[3]
                                    };
                                    urlField.ValidateParseAndSetValue(item, urlFieldValue.ToString());
                                    item.Update();
                                }
                            }
                        }
                    }
                }
            });
        }


        public string[] getSourceList(string sourceListName, int itemId,string sourceUrl)
        {
            string[] sslist = new string[4];
            string siteUrl = SPContext.Current.Site.Url;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb(SPContext.Current.Web.ID))
                    {
                        SPList sList = spWeb.Lists.TryGetList(sourceListName);
                        if (sList != null)
                        {

                            sslist[0]= sList.ID.ToString();
                            sslist[1] = spWeb.Title+"-"+sourceListName;
                            sslist[2] = sList.GetItemById(itemId)["标题"] != null ? sList.GetItemById(itemId)["标题"].ToString():"无标题";
                            sslist[3] = sList.DefaultDisplayFormUrl+"?ID="+itemId+ "&Source="+ sourceUrl;
                        }
                    }
                }
            });
            return sslist;
        }
        /// <summary>
        /// 获取当前登录账户的账户名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAccount()
        {
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            if (currentUser!=null)
            {
                string loginName = currentUser.LoginName;
                //if (currentUser.Name != "系统帐户")
                loginName = loginName.Substring(loginName.IndexOf('\\') + 1);
                string account = loginName.Replace(@"i:0#.w|", "");
                return account;
            }
            else
            {
                return "Anonymous";
            }

        }


        private static string[] GetLinkField(SPListItem item,string fieldName)
        {
            string[] urlfieldValue = new string[2];
            //Hyperlink or Picture 类型
            SPFieldUrlValue urlfield = new SPFieldUrlValue(item[fieldName].ToString());
            urlfieldValue[0] = urlfield.Description;
            urlfieldValue[1] = urlfield.Url;
            return urlfieldValue;
        }


        ///<summary>
        ///获取客户端IP地址
        ///</summary>
        ///<returns>IP地址</returns>
        public static string GetClientIPAddr()
        {
            string ipAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddr))
                ipAddr = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(ipAddr))
                ipAddr = HttpContext.Current.Request.UserHostAddress;

            return ipAddr;
        }

        /// <summary>
        /// 获取url字符串参数，返回参数值字符串
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="url">url字符串</param>
        /// <returns></returns>
        public static string GetQueryString(string name, string url)
        {
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(url);
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
