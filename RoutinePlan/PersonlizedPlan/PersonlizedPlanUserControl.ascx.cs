using Microsoft.SharePoint;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RoutinePlan.PersonlizedPlan
{
	public partial class PersonlizedPlanUserControl : UserControl
	{
		public PersonlizedPlan webObj { get; set; }
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				string rPlanList = webObj.RoutinePlanList;
				string siteUrl = webObj.SiteUrl;
				BindRoutinePlan(rPlanList, siteUrl);
			}
		}

		private void BindRoutinePlan(string rPlanList, string siteUrl)
		{
			SPSecurity.RunWithElevatedPrivileges(delegate ()
			{
				try
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
									ddlRPlans.DataTextField = "Title";
									ddlRPlans.DataValueField = "ID";
									ddlRPlans.DataSource = spList.GetItems();
									ddlRPlans.DataBind();
								}
							}
						}
					}
				}
				catch (Exception ex)
				{

					lbErr.Text = ex.ToString();
				}
			});
		}


		private void GetPersonalizedData(string planTitle, string planList, string siteUrl)
		{
			SPSecurity.RunWithElevatedPrivileges(delegate ()
		   {
			   try
			   {
				   using (SPSite spSite = new SPSite(siteUrl)) //找到网站集
				   {
					   using (SPWeb spWeb = spSite.OpenWeb())
					   {
						   SPList spList = spWeb.Lists.TryGetList(planList);
						   if (spList != null)
						   {
							   SPQuery qry = new SPQuery();
							   qry.Query = "";

						   }
					   }
				   }
			   }
			   catch (Exception ex)
			   {

				   lbErr.Text = ex.ToString();
			   }
		   });
		}
	}
}
