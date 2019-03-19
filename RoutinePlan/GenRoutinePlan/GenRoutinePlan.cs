﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace RoutinePlan.GenRoutinePlan
{
    [ToolboxItemAttribute(false)]
    public class GenRoutinePlan : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/15/RoutinePlan/GenRoutinePlan/GenRoutinePlanUserControl.ascx";

        protected override void CreateChildControls()
        {
            GenRoutinePlanUserControl control = Page.LoadControl(_ascxPath) as GenRoutinePlanUserControl;
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

        private int _minValue =1;
        [Personalizable, WebDisplayName("最小生成天数"), WebDescription("一次生成数据的最小天数，如：1"), WebBrowsable, Category("自定义设置")]
        public int MinValue
        {
            get
            {
                return this._minValue;
            }
            set
            {
                this._minValue = value;
            }
        }


        private int _maxValue = 7;
        [Personalizable, WebDisplayName("最大生成天数"), WebDescription("一次生成数据的最大天数，如：7"), WebBrowsable, Category("自定义设置")]
        public int MaxValue
        {
            get
            {
                return this._maxValue;
            }
            set
            {
                this._maxValue = value;
            }
        }

        private int _colCount = 4;
        [Personalizable, WebDisplayName("列数"), WebDescription("多选框选项分布列数，如：4"), WebBrowsable, Category("自定义设置")]
        public int ColCount
        {
            get
            {
                return this._colCount;
            }
            set
            {
                this._colCount = value;
            }
        }

        #endregion
    }
}
