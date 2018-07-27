using Microsoft.SharePoint;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RoutinePlan.VisualWebPart1
{
    public partial class VisualWebPart1UserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void GetRoutinePlan(string rPlanList)
        {
            string siteUrl = SPContext.Current.Site.Url;
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
                                spQry.Query="<OrderBy><FieldRef Name='PlanStart'/></OrderBy>";
                                SPListItemCollection items = spList.GetItems(spQry);
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
    }
}
