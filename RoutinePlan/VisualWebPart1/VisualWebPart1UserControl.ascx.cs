using Microsoft.SharePoint;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Text.RegularExpressions;

namespace RoutinePlan.VisualWebPart1
{
    public partial class VisualWebPart1UserControl : UserControl
    {
        public VisualWebPart1 webObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SPContext.Current.Site.OpenWeb().CurrentUser == null)
            {
                Panel1.Visible = false;
            }
            else
            {
                Panel1.Visible = true;
            }
        }
        protected void btnMakePlans_Click(object sender, EventArgs e)
        {
            string rPlanList = webObj.RoutinePlanList;
            string siteUrl = webObj.SiteUrl;
            string planList = webObj.PlanList;
            string logList = webObj.LogList;
            if (IsIntNum(tbDays.Text.Trim(),1,7))
            {
                int days = int.Parse(tbDays.Text.Trim());
                string refStr=GetRoutinePlan(logList,rPlanList, siteUrl, planList, days);
                if (refStr==days.ToString())
                {
                    lbErr.Text = "从今天起，"+days+ "天内的例行计划已生成，无需重复生成！";
                }
                else if (refStr=="0")
                {
                    DateTime dtNow = DateTime.Now.Date;
                    lbErr.Text = "本次操作共生成了从 " + dtNow.ToShortDateString() + "起， " + days + " 天的例行计划！";
                }
                else
                {
                    lbErr.Text = refStr;
                }
            }
            else
            {
                lbErr.Text = "请输入1-7的整数；";
            }
        }

        public static bool IsIntNum(string str,int minValue,int maxValue)
        {
            bool ismatch;
            if (string.IsNullOrEmpty(str))
            {
                ismatch = false;
            }
            else
            {
                Regex reg= new Regex(@"^[-]?[1-9]{1}\d*$|^[0]{1}$");
                ismatch = reg.IsMatch(str);
                if (ismatch)
                {
                    if (int.Parse(str) >= minValue && int.Parse(str) <= maxValue)
                    {
                        ismatch = true;
                    }
                    else
                    {
                        ismatch = false;
                    }
                }
            }
            return ismatch;
        }
        private static string GetRoutinePlan(string LogList, string rPlanList, string siteUrl, string planList, int days)
        {
            string refStr="";
            SPUser currentUser = SPContext.Current.Site.OpenWeb().CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                try
                {
                    int k = 0;
                    using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            spWeb.AllowUnsafeUpdates = true;
                            SPList logList = spWeb.Lists.TryGetList(LogList);
                            if (LogList != null)
                            {
                                SPQuery qry = new SPQuery();
                                qry.Query = "<Where><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Integer'>" + currentUser.ID + "</Value></Eq></Where>";
                                SPListItemCollection logListItems = logList.GetItems(qry);
                                DateTime dtNow = DateTime.Now;
                                if (logListItems.Count > 0)
                                {
                                    SPListItem logItem = logListItems[0];
                                    DateTime dtEnd = (DateTime)logItem["截止日期"];
                                    int n = GetNumberInt(logItem["标题"].ToString());
                                    if (dtEnd.Date > dtNow.Date)
                                    {
                                        int leftdays = days - DateDiff(dtNow, dtEnd) - 1;
                                        if (leftdays > 0)
                                        {
                                            k = DateDiff(dtNow, dtEnd) + 1;
                                            n = n + 1;

                                            logItem["标题"] = currentUser.Name + "第" + n + "次操作";
                                            logItem["开始日期"] = dtNow;
                                            logItem["截止日期"] = dtEnd.AddDays(leftdays);
                                            logItem.Update();
                                            refStr = k.ToString();
                                        }
                                        else
                                        {
                                            k = days;
                                            refStr = days.ToString();
                                        }
                                    }
                                    else
                                    {
                                        k = 0;
                                        refStr ="0";
                                        n = n + 1;
                                        logItem["标题"] = currentUser.Name + "第" + n + "次操作";
                                        logItem["开始日期"] = dtNow;
                                        logItem["截止日期"] = dtNow.AddDays(days - 1);
                                        logItem.Update();
                                    }
                                }
                                else
                                {
                                    k = 0;
                                    refStr = "0";
                                    SPListItem logItem = logList.AddItem();
                                    logItem["标题"] = currentUser.Name + "第1次操作";
                                    logItem["开始日期"] = dtNow;
                                    logItem["截止日期"] = dtNow.AddDays(days - 1);
                                    logItem["创建者"] = currentUser.ID;
                                    logItem["修改者"] = currentUser.ID;
                                    logItem.Update();
                                }
                            }
                            SPList spList = spWeb.Lists.TryGetList(rPlanList);
                            if (spList != null)
                            {
                                SPListItemCollection rPlanListItems = spList.GetItems();
                                if (rPlanListItems.Count > 0)
                                {
                                    foreach (SPListItem rPlanListItem in rPlanListItems)
                                    {
                                        SPList pList = spWeb.Lists.TryGetList(planList);
                                        if (pList != null)
                                        {
                                            for (int i = k; i < days; i++)
                                            {
                                                SPListItem pitem = pList.AddItem();
                                                pitem["对象"] = rPlanListItem["标题"];
                                                DateTime dtStart = (DateTime)rPlanListItem["计划开始"];
                                                pitem["计划开始"] = DateTime.Now.AddDays(i).Date + dtStart.TimeOfDay;
                                                pitem["计划时长"] = rPlanListItem["计划时长"];

                                                pitem["活动操作"] = rPlanListItem["操作"];
                                                pitem["创建者"] = currentUser.ID;
                                                pitem["修改者"] = currentUser.ID;
                                                pitem.Update();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    refStr = ex.ToString();
                }
            });
            return refStr;
        }

        private static void WritePlan(string planList, string siteUrl, SPListItem rPlanListItem, int days)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        if (planList != "")
                        {

                            SPList spList = spWeb.Lists.TryGetList(planList);
                            if (spList != null)
                            {
                                for (int i = 0; i < days; i++)
                                {
                                    SPListItem item = spList.AddItem();
                                    item["标题"] = rPlanListItem["标题"];
                                    DateTime dtStart = (DateTime)rPlanListItem["计划开始"];
                                    item["计划开始"] = DateTime.Now.AddDays(i).Date + dtStart.TimeOfDay;
                                    item["计划时长"] = rPlanListItem["计划时长"];

                                    item["活动操作"] = rPlanListItem["操作"];
                                    item["创建者"] = spWeb.EnsureUser(spWeb.CurrentUser.LoginName);
                                    item["修改者"] = spWeb.CurrentUser;
                                    item.Update();
                                }
                            }
                        }
                    }
                }
            });
        }

        protected void rblistCycle_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rbValue = rblistCycle.SelectedValue;
            switch (rbValue)
            {
                case "rbDay":
                    DayModel.Visible = true;
                    WeekModel.Visible = false;
                    MonthModel.Visible = false;
                    YearModel.Visible = false;
                    break;
                case "rbWeek":
                    DayModel.Visible = false;
                    WeekModel.Visible = true;
                    MonthModel.Visible = false;
                    YearModel.Visible = false;
                    break;
                case "rbMonth":
                    DayModel.Visible = false;
                    WeekModel.Visible = false;
                    MonthModel.Visible = true;
                    YearModel.Visible = false;
                    break;
                case "rbYear":
                    DayModel.Visible = false;
                    MonthModel.Visible = false;
                    WeekModel.Visible = false;
                    YearModel.Visible = true;
                    break;
                default:
                    DayModel.Visible = true;
                    WeekModel.Visible = false;
                    MonthModel.Visible = false;
                    YearModel.Visible = false;
                    break;
            }
        }

        protected void rbEndDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEndDate.Checked)
            {
                rbEndTimes.Checked = false;
                rbNoEndDate.Checked = false;
            }
        }

        protected void rbEndTimes_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEndTimes.Checked)
            {
                rbEndDate.Checked = false;
                rbNoEndDate.Checked = false;
            }
        }

        protected void rbNoEndDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbNoEndDate.Checked)
            {
                rbEndDate.Checked = false;
                rbEndTimes.Checked = false;
            }
        }

        protected void rbDayPerNDay_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDayPerNDay.Checked)
            {
                rbDayPerWorkDay.Checked = false;
            }
        }

        protected void rbDayPerWorkDay_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDayPerWorkDay.Checked)
            {
                rbDayPerNDay.Checked = false;
            }
        }

        protected void rbMonth1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMonth1.Checked)
            {
                rbMonth2.Checked = false;
            }
        }

        protected void rbMonth2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMonth2.Checked)
            {
                rbMonth1.Checked = false;
            }
        }

        protected void rbYear1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbYear1.Checked)
            {
                rbYear2.Checked = false;
            }
        }

        protected void rbYear2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbYear2.Checked)
            {
                rbYear1.Checked = false;
            }
        }

        /// <summary>
        /// 计算开始时间和结束时间之间相差的天数
        /// </summary>
        /// <param name="dateStart">开始时间</param>
        /// <param name="dateEnd">结束时间</param>
        /// <returns></returns>
        private static int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());

            TimeSpan sp = end.Subtract(start);

            return sp.Days;
        }
        /// <summary>
        /// 获取字符串中的整数数字
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>数字</returns>
        public static int GetNumberInt(string str)
        {
            int result = 0;
            if (str != null && str != string.Empty)
            {
                // 正则表达式剔除非数字字符（不包含小数点.）
                str = Regex.Replace(str, @"[^\d.\d]", "");
                // 如果是数字，则转换为decimal类型
                if (Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$"))
                {
                    result = int.Parse(str);
                }
            }
            return result;
        }
    }
}
