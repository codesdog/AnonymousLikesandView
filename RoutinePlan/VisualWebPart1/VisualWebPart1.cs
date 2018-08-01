﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace RoutinePlan.VisualWebPart1
{
    [ToolboxItemAttribute(false)]
    public class VisualWebPart1 : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/RoutinePlan/VisualWebPart1/VisualWebPart1UserControl.ascx";

        protected override void CreateChildControls()
        {
            VisualWebPart1UserControl control = Page.LoadControl(_ascxPath) as VisualWebPart1UserControl;
            if (control != null)
            {
                control.webObj = this;
            }
            Controls.Add(control);
        }

        #region 自定义设置

        private string _routinePlanList = "例行计划";
        [Personalizable, WebDisplayName("例行计划列表名称"), WebDescription("例行计划列表名称，如：例行计划"), WebBrowsable, Category("自定义设置")]
        public string RoutinePlanList
        {
            get
            {
                return this._routinePlanList;
            }
            set
            {
                this._routinePlanList = value;
            }
        }

        private string _planList = "个人学习助手";
        [Personalizable, WebDisplayName("助手列表名称"), WebDescription("个人学习助手列表名称，如：个人学习助手"), WebBrowsable, Category("自定义设置")]
        public string PlanList
        {
            get
            {
                return this._planList;
            }
            set
            {
                this._planList = value;
            }
        }

        private string _logList = "例行计划生成历史";
        [Personalizable, WebDisplayName("例行计划生成历史"), WebDescription("例行计划生成历史列表名称，如：个人学习助手"), WebBrowsable, Category("自定义设置")]
        public string LogList
        {
            get
            {
                return this._logList;
            }
            set
            {
                this._logList = value;
            }
        }

        private string _siteUrl = "http://ws2018:19568/";
        [Personalizable, WebDisplayName("网址"), WebDescription("网站地址，如：http://va.neu.edu.cn"), WebBrowsable, Category("自定义设置")]
        public string SiteUrl
        {
            get
            {
                return this._siteUrl;
            }
            set
            {
                this._siteUrl = value;
            }
        }

        private int _days =2;
        [Personalizable, WebDisplayName("一次生成天数"), WebDescription("一次生成数据的天数，如：2"), WebBrowsable, Category("自定义设置")]
        public int Days
        {
            get
            {
                return this._days;
            }
            set
            {
                this._days = value;
            }
        }
        #endregion
    }
}
