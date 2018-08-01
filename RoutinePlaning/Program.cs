using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Configuration;

namespace RoutinePlaning
{
    class Program
    {


        static void Main(string[] args)
        {
            string rpList = ConfigurationSettings.AppSettings["RoutinePlanList"].ToString();
            string siteUrl = ConfigurationSettings.AppSettings["siteUrl"].ToString();
            string planList = ConfigurationSettings.AppSettings["PlanList"].ToString();
            int duringDays=int.Parse(ConfigurationSettings.AppSettings["DuringDays"].ToString());
            GetRoutinePlan(rpList, siteUrl,planList,duringDays);
        }

        private static void GetRoutinePlan(string rPlanList,string siteUrl,string panList,int duringDays)
        {

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        if (rPlanList != "")
                        {

                            SPList spList = spWeb.Lists.TryGetList(rPlanList);
                            if (spList != null)
                            {
                                SPQuery spQry = new SPQuery();
                                spQry.Query = "<OrderBy><FieldRef Name='PlanStart'/></OrderBy>";
                                SPListItemCollection items = spList.GetItems(spQry);
                                if (items.Count > 0)
                                {
                                    foreach (SPListItem item in items)
                                    {
                                        WritePlan(panList, siteUrl, item,duringDays);
                                        string title = item["标题"].ToString();
                                        Console.WriteLine(title);

                                    }
                                    Console.ReadLine();
                                }
                            }
                        }
                    }
                }
            });
        }

        private static void WritePlan(string planList, string siteUrl, SPListItem rPlanListItem,int duringDays)
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
                                for (int i = 0; i < duringDays; i++)
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
    }
}
