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
            if (!IsPostBack)
            {
                GetandUpdateItemById(itemId,sourceList, historyList);
            }
            //imgBtnLikes.Click += likeClick();
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

        private static string GetIP()
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "ipconfig.exe";//设置程序名
            cmd.StartInfo.Arguments = "/all";  //参数
                                               //重定向标准输出
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.CreateNoWindow = true;//不显示窗口（控制台程序是黑屏）
            //cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//暂时不明白什么意思
            /*收集一下 有备无患.关于:ProcessWindowStyle.Hidden隐藏后如何再显示？hwndWin32Host = Win32Native.FindWindow(null, win32Exinfo.windowsName);Win32Native.ShowWindow(hwndWin32Host, 1);
             * //先FindWindow找到窗口后再ShowWindow*/
            cmd.Start();
            string info = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();
            cmd.Close();
            return info;
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
        /// 获得客户端外网IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        private static string GetClientInternetIP()
        {
            //获取本机外网ip的url
            string getIpUrl = "http://www.ipip.net/ip.html";//网上获取ip地址的网站
            string tempip = "";
            WebRequest wr = WebRequest.Create(getIpUrl);

            Stream s = wr.GetResponse().GetResponseStream();
            using (StreamReader sr = new StreamReader(s, Encoding.UTF8))
            {
                string all = sr.ReadToEnd(); //读取网站的数据

                //解析出需要的数据
                int start = all.IndexOf("<th colspan=\"3\">您的当前IP: <span style=\"color: rgb(243, 102, 102);\">");
                int end = all.IndexOf("</span></th>");
                tempip = all.Substring(start, end - start).Replace("<th colspan=\"3\">您的当前IP: <span style=\"color: rgb(243, 102, 102);\">", "");
                sr.Close();
                s.Close();
            }
            return tempip;
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
