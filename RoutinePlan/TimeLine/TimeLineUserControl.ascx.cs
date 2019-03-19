using Microsoft.SharePoint;
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RoutinePlan.TimeLine
{
    public partial class TimeLineUserControl : UserControl
    {
        public TimeLine webObj { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = GetData();

            if (dt!=null)
            {
                DataTable newDt = DtSelectTop(10, dt);
                GenHtml(newDt);
            }
            else
            {
                historyDiv.InnerHtml = "近 <b>"+webObj.DayCount+"</b> 日无任何活动记录！";
            }

        }



        private void GenHtml(DataTable dt)
        {
            DateTime dtNow = DateTime.Now.Date;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < webObj.DayCount; i++)
            {
                DataRow[] drs = dt.Select("PlanStart>='" + dtNow.AddDays(-i) + "' and PlanStart<'" + dtNow.AddDays(1-i) + "'");
                if (drs.Length>0)
                {
                    HtmlGenericControl NewControl = new HtmlGenericControl("div");

                    NewControl.ID = "NewControl";
                    NewControl.Attributes["class"] = "history-date";
                    sb.AppendLine("<div class='history-date'>");
                    sb.AppendLine("<ul>");
                    sb.AppendLine("<h2 class='first'><a href='#nogo'>" + dtNow.AddDays(i).GetDateTimeFormats('M')[0] + "</a></h2>");
                    for (int j = 0; j < drs.Length; j++)
                    {
                        sb.AppendLine("<li>");
                        DateTime dtplanStart = (DateTime)drs[j]["PlanStart"];
                        string panDuring = drs[j]["PlanDuring"].ToString();
                        sb.AppendLine("<h3>" + dtplanStart.GetDateTimeFormats('t')[0] + "<span>" + panDuring + " 分钟</span></h3>");
                        string rectype = drs[j]["Type"].ToString();
                        string action = drs[j]["CustAction"] + "&nbsp;" + drs[j]["Title"];
                        sb.AppendLine("<dl><dt>" + rectype + "<span>" + action + "</span></dt></dl>");
                        sb.AppendLine("</li>");
                    }
                    sb.AppendLine("</ul>");
                    sb.AppendLine("</div>");
                    //NewControl.InnerHtml = sb.ToString();
                }


                //historyDiv.Controls.Add(NewControl);
            }
            historyDiv.InnerHtml = sb.ToString();
        }

        private DataTable GetData()
        {
            DataTable dtReturn = new DataTable();
            SPSecurity.RunWithElevatedPrivileges(delegate ()
           {
               try
               {
                   using (SPSite spSite = new SPSite(webObj.SiteUrl)) //找到网站集
                   {
                       using (SPWeb spWeb = spSite.OpenWeb())
                       {
                           SPList pList = spWeb.Lists.TryGetList(webObj.PlanList);
                           //int days = webObj.DayCount;
                           //DateTime dt = DateTime.Now.Date.AddDays(-days).ToUniversalTime();
                           //SPQuery qry = new SPQuery()
                           //{
                           //    Query = "<Where><And><Geq><FieldRef Name='PlanStart' /><Value Type='DateTime'>"+dt+"</Value></Geq><Eq><FieldRef Name='Author' /><Value Type='Integer'><UserID /></Value></Eq></And></Where>"
                           //};
                           ////个人学习助手表
                           //dtReturn = pList.GetItems(qry).GetDataTable();
                           dtReturn = pList.GetItems().GetDataTable();
                       }
                   }
               }
               catch (Exception ex)
               {

                   historyDiv.InnerHtml = ex.ToString();
                   dtReturn = null;
               }
           });
            return dtReturn;
        }

        #region 获取DataTable前几条数据
        /// <summary>
        /// 获取DataTable前几条数据
        /// </summary>
        /// <param name="TopItem">前N条数据</param>
        /// <param name="oDT">源DataTable</param>
        /// <returns></returns>
        public DataTable DtSelectTop(int TopItem, DataTable oDT)
        {
            if (oDT.Rows.Count < TopItem) return oDT;

            DataTable NewTable = oDT.Clone();
            DataRow[] rows = oDT.Select("1=1");
            for (int i = 0; i < TopItem; i++)
            {
                NewTable.ImportRow((DataRow)rows[i]);
            }
            return NewTable;
        }
        #endregion
    }
}
