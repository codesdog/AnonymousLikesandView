using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Configuration;
using Microsoft.SharePoint.Client;
using System.Data;

namespace RoutinePlaning
{
    class Program
    {
        static void Main(string[] args)
        {
            string rpList = ConfigurationManager.AppSettings["RoutinePlanList"].ToString();
            string siteUrl = ConfigurationManager.AppSettings["siteUrl"].ToString();
            string planList = ConfigurationManager.AppSettings["PlanList"].ToString();
            int duringDays=int.Parse(ConfigurationManager.AppSettings["DuringDays"].ToString());
            string[] stateIndexs = ConfigurationManager.AppSettings["DuringDays"].ToString().Split(';');
            //GetRoutinePlan(rpList, siteUrl,planList,duringDays);
            DataView dv = DistinctProjectItem(siteUrl, planList, "Author");
            if (dv != null)
            {
                DataTable dt = dv.ToTable();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string userName = dt.Rows[i]["Author"].ToString();
                        GetUserStateFromPlan(userName,planList,siteUrl, stateIndexs);
                    }
                }
            }
        }


        #region 计算个人状态指标值
        private static void GetUserStateFromPlan(string userName,string planList,string siteUrl,string[] stateIndexs)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {

                try
                {
                    using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            spWeb.AllowUnsafeUpdates = true;

                            SPList spList = spWeb.Lists.TryGetList(planList);
                            if (spList != null)
                            {
                                SPQuery qry = new SPQuery();
                                qry.Query = @"<Where><Eq><FieldRef Name='Author' LookupId='True' /><Value Type='Text'>" + userName + "</Value></Eq></Where>";
                                SPListItemCollection pListItems = spList.GetItems(qry);
                                //var results = from t in spList.Items.Cast<SPListItem>()                                              select new { t.Fields("Author")};
                                //var disresults = Enumerable.Distinct(results);
                                if (pListItems.Count > 0)
                                {
                                    DataTable dt= pListItems.GetDataTable();
                                    for (int i = 0; i < stateIndexs.Length; i++)
                                    {


                                        string benchmarkList = ConfigurationSettings.AppSettings["BenchmarkList"].ToString();
                                        DataTable dtBenchmark = GetBenchmarkByIndex(stateIndexs[i], siteUrl, benchmarkList);
                                        if (dtBenchmark.Rows.Count > 0)
                                        {
                                            for (int j = 0; j <= dtBenchmark.Rows.Count; j++)
                                            {
                                                string aType = dtBenchmark.Rows[j]["活动类别"].ToString();
                                                string selectStr = "操作类别 ='" + aType + "'";
                                                DataRow[] drs = dt.Select(selectStr);
                                            }
                                        }


                                    }

                                }
                            }
                            else
                            {
                                Console.WriteLine("列表“" + planList + "”不存在！");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
        }

        /// <summary>
        /// 获取指定状态指标项的计分标准
        /// </summary>
        /// <param name="index">指标项</param>
        /// <param name="siteUrl">网址</param>
        /// <param name="listName">计分标准列表名称</param>
        /// <returns>计分标准表DataTable</returns>
        private static DataTable GetBenchmarkByIndex(string index, string siteUrl,string listName)
        {
            using (SPSite spSite = new SPSite(siteUrl))
            {
                using (SPWeb spWeb = spSite.OpenWeb())
                {
                    SPList MyList = spWeb.Lists.TryGetList(listName);
                    SPListItemCollection spListItem;
                    SPQuery qry = new SPQuery();
                    qry.Query = "<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>" + index + "</Value></Eq></Where>";
                    spListItem = MyList.GetItems(qry);

                    return spListItem.GetDataTable();
                }
            }
        }

        /// <summary>
        /// 选择指定列表中某个字段的
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="listName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static DataView DistinctProjectItem(string siteUrl,string listName, string fieldName)
        {
            DataView dvListItem = null;
            using (SPSite spSite = new SPSite(siteUrl))
            {
                using (SPWeb spWeb = spSite.OpenWeb())
                {
                    SPList MyList = spWeb.Lists.TryGetList(listName);
                    SPListItemCollection spListItem;
                    SPQuery spDistinctItem = new SPQuery();
                    spDistinctItem.Query = "<OrderBy><FieldRef Name='Author'/></Order By><FieldRef Name='" + fieldName + "'/>";
                    spListItem = MyList.GetItems(spDistinctItem);

                    DataTable dtDistinctProject = spListItem.GetDataTable();
                    try
                    {
                        DataTable dtListItem = dtDistinctProject.DefaultView.ToTable(true, fieldName);
                        //true 表示选择不重复的项
                        dvListItem = new DataView(dtListItem);
                        return dvListItem;
                    }
                    catch (Exception ex)
                    {
                        return dvListItem;
                    }
                }
            }

        }
        #endregion

        #region 例行计划生成


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


        #endregion

        /// <summary>
        /// 两个DataTable组合
        /// </summary>
        /// <param name="dtFirst">第一个DataTable</param>
        /// <param name="dtSecond">第二个DataTable</param>
        /// <param name="FJC">第一个DataTable列集合</param>
        /// <param name="SJC">第二个DataTable列集合</param>
        /// <returns></returns>
        public static DataTable JoinDataTable(DataTable dtFirst, DataTable dtSecond, DataColumn[] FJC, DataColumn[] SJC)
        {
            //创建一个新的DataTable
            DataTable table = new DataTable("Join");
            // Use a DataSet to leverage DataRelation
            using (DataSet ds = new DataSet())
            {
                //把DataTable Copy到DataSet中
                ds.Tables.AddRange(new DataTable[] { dtFirst.Copy(), dtSecond.Copy() });
                DataColumn[] parentcolumns = new DataColumn[FJC.Length];
                for (int i = 0; i < parentcolumns.Length; i++)
                {
                    parentcolumns[i] = ds.Tables[0].Columns[FJC[i].ColumnName];
                }
                DataColumn[] childcolumns = new DataColumn[SJC.Length];
                for (int i = 0; i < childcolumns.Length; i++)
                {
                    childcolumns[i] = ds.Tables[1].Columns[SJC[i].ColumnName];
                }
                //创建关联
                DataRelation r = new DataRelation(string.Empty, parentcolumns, childcolumns, false);
                ds.Relations.Add(r);

                //为关联表创建列
                for (int i = 0; i < dtFirst.Columns.Count; i++)
                {
                    table.Columns.Add(dtFirst.Columns[i].ColumnName, dtFirst.Columns[i].DataType);
                }
                for (int i = 0; i < dtSecond.Columns.Count; i++)
                {
                    //看看有没有重复的列，如果有在第二个DataTable的Column的列明后加_Second
                    if (!table.Columns.Contains(dtSecond.Columns[i].ColumnName))
                        table.Columns.Add(dtSecond.Columns[i].ColumnName, dtSecond.Columns[i].DataType);
                    else
                        table.Columns.Add(dtSecond.Columns[i].ColumnName + "_Second", dtSecond.Columns[i].DataType);
                }
                table.BeginLoadData();
                foreach (DataRow firstrow in ds.Tables[0].Rows)
                {
                    //得到行的数据
                    DataRow[] childrows = firstrow.GetChildRows(r);
                    if (childrows != null && childrows.Length > 0)
                    {
                        object[] parentarray = firstrow.ItemArray;
                        foreach (DataRow secondrow in childrows)
                        {
                            object[] secondarray = secondrow.ItemArray;
                            object[] joinarray = new object[parentarray.Length + secondarray.Length];
                            Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);
                            Array.Copy(secondarray, 0, joinarray, parentarray.Length, secondarray.Length);
                            table.LoadDataRow(joinarray, true);
                        }
                    }
                }
                table.EndLoadData();
            }
            return table;
        }


        /// <summary>
        /// 组合两个DataTable
        /// </summary>
        /// <param name="dt1">第一个DataTable</param>
        /// <param name="dt2">第二个DataTable</param>
        /// <param name="KeyColName">关联的列名</param>
        /// <returns></returns>
        public static DataTable MergeDataTable(DataTable dt1, DataTable dt2, String KeyColName)
        {
            //定义临时变量
            DataTable dtReturn = new DataTable();
            int i = 0;
            int j = 0;
            int k = 0;
            int colKey1 = 0;
            int colKey2 = 0;
            //设定表dtReturn的名字?
            dtReturn.TableName = dt1.TableName;
            //设定表dtReturn的列名
            for (i = 0; i < dt1.Columns.Count; i++)
            {
                if (dt1.Columns[i].ColumnName == KeyColName)
                {
                    colKey1 = i;
                }
                dtReturn.Columns.Add(dt1.Columns[i].ColumnName);
            }
            for (j = 0; j < dt2.Columns.Count; j++)
            {
                if (dt2.Columns[j].ColumnName == KeyColName)
                {
                    colKey2 = j;
                    continue;
                }
                dtReturn.Columns.Add(dt2.Columns[j].ColumnName);
            }
            //建立表的空间
            for (i = 0; i < dt1.Rows.Count; i++)
            {
                DataRow dr;
                dr = dtReturn.NewRow();
                dtReturn.Rows.Add(dr);
            }
            //将表dt1,dt2的数据写入dtReturn
            for (i = 0; i < dt1.Rows.Count; i++)
            {
                int m = -1;
                //表dt1的第i行数据拷贝到dtReturn中去
                for (j = 0; j < dt1.Columns.Count; j++)
                {
                    dtReturn.Rows[i][j] = dt1.Rows[i][j].ToString();
                }
                //查找的dt2中KeyColName的数据,与dt1相同的行
                for (k = 0; k < dt2.Rows.Count; k++)
                {
                    if (dt1.Rows[i][colKey1].ToString() == dt1.Rows[k][colKey2].ToString())
                    {
                        m = k;
                    }
                }
                //表dt2的第m行数据拷贝到dtReturn中去,且不要KeyColName(ID)列
                if (m != -1)
                {
                    for (k = 0; k < dt2.Columns.Count; k++)
                    {
                        if (k == colKey2)
                        {
                            continue;
                        }
                        dtReturn.Rows[i][j] = dt2.Rows[m][k].ToString();
                        j++;
                    }
                }
            }
            return dtReturn;
        }


    }
}
