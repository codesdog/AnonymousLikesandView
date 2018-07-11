using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace PageHitCount.CountPageHit
{
    public partial class CountPageHitUserControl : UserControl
    {
        #region  全局变量
        public static string Login_ID = ""; //定义全局变量，记录当前登录的用户编号
        public static string Login_Name = ""; //定义全局变量，记录当前登录的用户名
        public static SqlConnection My_con;  //定义一个SqlConnection类型的公共变量My_con，用于判断数据库是否连接成功
        public static string M_str_sqlcon = "Database=VAExtension;Server=202.118.11.99;User ID=sa;Password=sasasasa;Trusted_Connection=False;";

        public CountPageHit webObj { get; set; }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string urlStr = Request.Url.ToString();
            //countMe();
            //using (DataSet tmpDs = new DataSet())
            //{
            //    tmpDs.ReadXml(Server.MapPath("~/counter.xml"));
            //    this.lbCounter.Text = tmpDs.Tables[0].Rows[0]["hits"].ToString();
            //}
            long pageId = GetCurrentPageId();
            if (ConnectSuccess())
            {
                CountHit(pageId);
                ShowHit(pageId);
            }
            else
            {
                lbmsg.Text = "无法访问数据库！";
            }
        }

        private void countMe()
        {
            DataSet tmpDs = new DataSet();
            tmpDs.ReadXml(Server.MapPath("~/counter.xml"));

            int hits = Int32.Parse(tmpDs.Tables[0].Rows[0]["hits"].ToString());

            hits += 1;

            tmpDs.Tables[0].Rows[0]["hits"] = hits.ToString();

            tmpDs.WriteXml(Server.MapPath("~/counter.xml"));

        }

        private void ShowHit(long pageId)
        {
            string sqlstr = "SELECT * FROM GetSPPageHits where HostName Like '%" + webObj.HostName + "%' order by Hited desc";
            DataTable hitdt= GetDataSet(sqlstr, "网站访问历史").Tables[0];
            //hitdt.DefaultView.Sort = "Hited DESC";
            DataTable dtResult = GetDTResult();

            DataRow dr=dtResult.NewRow();
            dr[0] = "本页";
            //今日点击量
            dr[1] = hitdt.Compute("sum(HitToday)", "Hited >= '" + DateTime.Now.Date + "' and PageID= " + pageId);

            //今日匿名点击量
            dr[2] = hitdt.Compute("sum(HitToday)", "Hited >= '" + DateTime.Now.Date + "' and HitUser = 'Anonymous'  and PageID= " + pageId);
            //总点击量
            dr[3] = hitdt.Compute("sum(HitCount)", "PageID= " + pageId);
            //总匿名点击量
            dr[4] = hitdt.Compute("sum(HitCount)", "PageID= " + pageId + " and HitUser = 'Anonymous'");
            for (int i =1; i < dtResult.Columns.Count; i++)
            {
                dr[i] = Convert.IsDBNull(dr[i]) ? 0 : dr[i];
            }
            dtResult.Rows.Add(dr);

            dr = dtResult.NewRow();
            dr[0] = "本站";
            //今日点击量
            dr[1] = hitdt.Compute("sum(HitToday)", "Hited >= '" + DateTime.Now.Date + "'");
            //今日匿名点击量
            dr[2] = hitdt.Compute("sum(HitToday)", "Hited >= '" + DateTime.Now.Date + "' and HitUser = 'Anonymous'");
            //总点击量
            dr[3] = hitdt.Compute("sum(HitCount)", "1=1");
            //总匿名点击量
            dr[4] = hitdt.Compute("sum(HitCount)", "HitUser = 'Anonymous'");
            for (int i = 1; i < dtResult.Columns.Count; i++)
            {
                dr[i] = Convert.IsDBNull(dr[i]) ? 0 : dr[i];
            }
            dtResult.Rows.Add(dr);

            GridView1.DataSource = dtResult;
            GridView1.DataBind();

        }

        /// <summary>
        /// 初始化汇总结果表
        /// </summary>
        /// <returns></returns>
        public DataTable GetDTResult()
        {
            DataTable dt = new DataTable();//指标列表

            using (DataColumn dataColumn = new DataColumn("点击量", typeof(string)))
            {
                dt.Columns.Add(dataColumn);
            }

            using (DataColumn dataColumn = new DataColumn("今日", typeof(int)))
            {
                dt.Columns.Add(dataColumn);
            }
            using (DataColumn dataColumn = new DataColumn("今日匿名", typeof(int)))
            {
                dt.Columns.Add(dataColumn);
            }
            using (DataColumn dataColumn = new DataColumn("总量", typeof(int)))
            {
                dt.Columns.Add(dataColumn);
            }

            using (DataColumn dataColumn = new DataColumn("匿名总量", typeof(int)))
            {
                dt.Columns.Add(dataColumn);
            }
            return dt;
        }

        private void CountHit(long pageId)
        {
            int hitCount = 0;
            int hitToday = 0;
            string sqlstr = @"SELECT HitID,HitCount,Hited,HitToday FROM SPPageHits where PageID=" + pageId + " and FromIP='" + GetClientIPAddr() + "'";
            SqlDataReader sqlDr = GetSqlDataReader(sqlstr);
            bool isRead = sqlDr.Read();
            if (isRead)//该页面，对应的IP已有访问记录
            {
                long hitId =sqlDr.GetInt64(0);
                hitCount = sqlDr.GetInt32(1) +1;
                DateTime hited = sqlDr.GetDateTime(2);
                if (hited.Date==DateTime.Now.Date)
                {
                    hitToday = sqlDr.GetInt32(3)+1;
                }
                sqlstr = @"UPDATE SPPageHits SET Hited = '" + DateTime.Now + "',HitUser ='" + GetCurrentAccount() + "',HitCount =" + hitCount + ",HitToday =" + hitToday + " WHERE HitID =" +hitId;
                GetSqlItemId(sqlstr);
            }
            else//尚未有访问记录
            {
                sqlstr = @"INSERT INTO SPPageHits (PageID,Hited,HitUser,HitCount,HitToday,FromIP) VALUES (" + pageId+",'"+DateTime.Now+"','"+GetCurrentAccount()+"',1,1,'"+GetClientIPAddr()+"')";
                long hitId = GetSqlItemId(sqlstr);
            }

            con_close();
        }


        private long GetCurrentPageId()
        {
            long pageId = 0;
            string pageUrl = Request.Url.ToString();
            if (pageUrl.Split('?').Length>0)
            {
                pageUrl = pageUrl.Split('?')[0];
            }
            //pageUrl=SPAlternateUrlCollection.GetRedirectUrl();
            string sqlstr = "SELECT * FROM SPPages where PageUrl='"+pageUrl+"'";
            SqlDataReader sqlDr = GetSqlDataReader(sqlstr);
            bool isRead = sqlDr.Read();
            if (isRead)
            {
                pageId=sqlDr.GetInt64(0);
            }
            else
            {
                string siteUrl = SPContext.Current.Site.Url;
                string hostName = SPContext.Current.Site.HostName;
                Guid webGuid = SPContext.Current.Web.ID;
                DateTime created = DateTime.Now;
                sqlstr = @"INSERT INTO SPPages (PageUrl,HostName,SiteUrl,WebGuid,Created) VALUES ('" + pageUrl + "','" + hostName + "','" + siteUrl+"','"+webGuid+"','"+created+"')";
                pageId = GetSqlItemId(sqlstr);
            }
            con_close();
            return pageId;
        }
        /// <summary>
        /// 获取当前登录账户的账户名
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAccount()
        {
            SPUser currentUser = SPContext.Current.Web.CurrentUser;
            if (currentUser != null)
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
        #region 数据库操作


        #region  建立数据库连接
        /// <summary>
        /// 建立数据库连接.
        /// </summary>
        /// <returns>返回SqlConnection对象</returns>
        public static SqlConnection Getcon()
        {
            My_con = new SqlConnection(M_str_sqlcon);   //用SqlConnection对象与指定的数据库相连接
            try
            {
                My_con.Open();  //打开数据库连接
                if (My_con.State == ConnectionState.Open)
                {
                    return My_con;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
            //finally
            //{
            //    My_con.Close();
            //}
             //返回SqlConnection对象的信息
        }
        #endregion

        #region 判断数据库是否连接成功
        /// <summary>
        /// 数据库连接是否成功
        /// </summary>
        /// <returns></returns>
        public bool ConnectSuccess()
        {
            bool result = false;
            //创建连接对象
            My_con = new SqlConnection(M_str_sqlcon);
            try
            {
                My_con.Open();
                if (My_con.State == ConnectionState.Open)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                My_con.Close();
            }
            return result;
        }
        #endregion

        #region  测试数据库是否赋加
        /// <summary>
        /// 测试数据库是否赋加
        /// </summary>
        public void con_open()
        {
            Getcon();
            //con_close();
        }
        #endregion

        #region  关闭数据库连接
        /// <summary>
        /// 关闭于数据库的连接.
        /// </summary>
        public void con_close()
        {
            if (My_con.State == ConnectionState.Open)   //判断是否打开与数据库的连接
            {
                My_con.Close();   //关闭数据库的连接
                My_con.Dispose();   //释放My_con变量的所有空间
            }
        }
        #endregion

        #region  执行查询sql命令
        /// <summary>
        /// 执行查询sql命令.
        /// </summary>
        /// <param name="SQLstr">SQL语句</param>
        /// <returns>返回SqlDataReader</returns>
        public SqlDataReader GetSqlDataReader(string SQLstr)
        {
            Getcon();   //打开与数据库的连接
            if (My_con.State==ConnectionState.Open)
            {
                SqlCommand Mycom = My_con.CreateCommand(); //创建一个SqlCommand对象，用于执行SQL语句
                Mycom.CommandText = SQLstr;    //获取指定的SQL语句
                SqlDataReader Myread = Mycom.ExecuteReader(); //执行SQL语名句，生成一个SqlDataReader对象
                return Myread;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 执行增加、更新、删除等Sql命令
        /// <summary>
        /// 执行增加、更新、删除等Sql命令
        /// </summary>
        /// <param name="SQLstr">Sql语句</param>
        /// <returns>返回执行结果是sql语句中的数据项ID</returns>
        public long GetSqlItemId(string SQLstr)
        {
            long itemId = 0;
            Getcon();   //打开与数据库的连接
            SqlCommand SQLcom = new SqlCommand(SQLstr, My_con); //创建一个SqlCommand对象，用于执行SQL语句
            itemId = SQLcom.ExecuteNonQuery();   //执行SQL语句
            SQLcom.Dispose();   //释放所有空间
            con_close();    //调用con_close()方法，关闭与数据库的连接
            return itemId;
        }
        #endregion

        #region  执行Sql命令，返回指定名称的数据集
        /// <summary>
        /// 执行Sql命令，返回指定名称的数据集
        /// </summary>
        /// <param name="SQLstr">SQL命令语句</param>
        /// <param name="tableName">数据集表名</param>
        /// <returns>返回数据集DataSet对象</returns>
        public DataSet GetDataSet(string SQLstr, string tableName)
        {
            Getcon();   //打开与数据库的连接
            SqlDataAdapter SQLda = new SqlDataAdapter(SQLstr, My_con);  //创建一个SqlDataAdapter对象，并获取指定数据表的信息
            DataSet My_DataSet = new DataSet(); //创建DataSet对象
            SQLda.Fill(My_DataSet, tableName);  //通过SqlDataAdapter对象的Fill()方法，将数据表信息添加到DataSet对象中
            //DataTable dt = My_DataSet.Tables[0];//将数据集转化为数据表
            con_close();    //关闭数据库的连接
            return My_DataSet;  //返回DataSet对象的信息
        }
        #endregion
        #endregion

        #region 第三方方法


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

            if (string.IsNullOrEmpty(ipAddr))
            {
                ipAddr = "127.0.0.1";
            }
            if (ipAddr.Contains(","))
            {
                ipAddr = ipAddr.Substring(ipAddr.LastIndexOf(',') + 1, ipAddr.Length - ipAddr.LastIndexOf(',') - 1);
            }
            //最后判断获取是否成功，并检查IP地址的格式（检查其格式非常重要）
            if (!IsIP(ipAddr))
            {
                ipAddr = "127.0.0.1";
            }
            return ipAddr;
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ipaddr"></param>
        /// <returns></returns>
        public static bool IsIP(string ipaddr)
        {
            return Regex.IsMatch(ipaddr, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        #endregion
    }
}
