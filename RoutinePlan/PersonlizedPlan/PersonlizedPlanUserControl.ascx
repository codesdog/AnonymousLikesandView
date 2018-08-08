<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonlizedPlanUserControl.ascx.cs" Inherits="RoutinePlan.PersonlizedPlan.PersonlizedPlanUserControl" %>

<style type="text/css">
    .Appraise {
    box-shadow: 0 1px 3px #ccc;
    background-color: white;
    background-color: rgba(255, 255, 255, 0.6);
    padding:10px 20px 10px 10px;
    margin-right: 10px;
    margin-top: 5px;
    width:100%;
    max-width:420px;
    line-height:30px;
    font-size:14px;

}
    .Appraise .title {
    height: 50px;
    line-height: 50px;
    font-family: "微软雅黑", "PingFang SC", sans;
    font-weight: 600;
    color: #000;
    font-size: 20px;
    border-bottom: 1px solid #ccc;
}

        .formul{
        list-style:none;
        margin-left:auto;
        }
        .formul li{
        line-height:25px;
        padding-bottom:10px;
        }
        .formul li button{
        font-size:14px;
        }
        .att{
            background-color:#cfcfcf;
            color:red;
            font-size:14px;
        }
.button{
        width: 50px;
        text-align: center;
        font-weight: bold;
        color: #000;
        border-radius: 5px;
        margin:0 5px 5px 0;
        overflow: hidden;
        cursor:pointer;
    }
     </style>

<div class="Appraise">
     <div class="title">个性化生成例行计划</div>
<asp:DropDownList ID="ddlRPlans" runat="server"></asp:DropDownList>
<br/>
<asp:Label ID="lbErr" runat="server" ForeColor="red" Text=""></asp:Label>
</div>