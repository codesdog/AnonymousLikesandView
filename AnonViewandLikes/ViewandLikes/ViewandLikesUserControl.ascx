<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewandLikesUserControl.ascx.cs" Inherits="AnonViewandLikes.ViewandLikes.ViewandLikesUserControl" %>
<style type="text/css">
    .ultools {
       list-style:none; /* 去掉ul前面的符号 */
        margin: 0px; /* 与外界元素的距离为0 */
        padding: 0px; /* 与内部元素的距离为0 */
        width: auto; /* 宽度根据元素内容调整 */
    }
    .ultools li{
        list-style:none;
        float:left; /* 向左漂移，将竖排变为横排 */
        text-align:center;
        padding:5px 10px 5px 10px;
        border:1px solid #bfcbd6;

    }
    .ultools li:hover{
        background-color: #bfcbd6; /* 背景色 */
        color: #465c71; /* 文字颜色 */
        text-decoration: none; /* 不显示超链接下划线 */
    }

    .viewspan {
        padding-left:20px;
        padding-top:-2px;
        line-height: 25px;
        background-position:left center;
        font-size: 14px;
    }
</style>
<span class="viewspan" style="background-image: url(../_layouts/15/images/view.png);background-repeat: no-repeat;" id="viewspan" runat="server"></span>
<span class="viewspan" id="viewtimespan" runat="server"  style="background-image: url(../_layouts/15/images/viewtime.png);background-repeat: no-repeat;" ></span>


