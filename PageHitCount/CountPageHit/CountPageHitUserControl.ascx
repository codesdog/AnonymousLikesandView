<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CountPageHitUserControl.ascx.cs" Inherits="PageHitCount.CountPageHit.CountPageHitUserControl" %>
<style type="text/css">
.GridViewStyle
{
    border-right: 1px solid #A7A6AA;
    border-bottom: 1px solid #A7A6AA;
    border-left: 1px solid white;
    border-top: 1px solid white;
    padding: 5px;
    text-align:center;
}

.GridViewStyle a
{
    color: #FFFFFF;
}

.GridViewHeaderStyle th
{
    border-left: 1px solid #EBE9ED;
    border-right: 1px solid #EBE9ED;
}

.GridViewHeaderStyle
{
    background-color: #A7A6AA;
    font-weight: bold;
    color: White;
}

.GridViewFooterStyle
{
    background-color: #5D7B9D;
    font-weight: bold;
    color: White;
}

.GridViewRowStyle
{
    background-color: #F7F6F3;
    color: #808080;
}

.GridViewAlternatingRowStyle
{
    background-color: #FFFFFF;
    color: #808080;
}

.GridViewRowStyle td, .GridViewAlternatingRowStyle td
{
    border: 1px solid #EBE9ED;
    padding: 2px;
}

.GridViewSelectedRowStyle
{
    background-color: #E2DED6;
    font-weight: bold;
    color: #333333;
}

.GridViewPagerStyle
{
    background-color: #284775;
    color: #FFFFFF;
}

.GridViewPagerStyle table /**//* to center the paging links*/
{
    margin: 0 auto 0 auto;
}
</style>

<asp:Label ID="lbmsg" runat="server" Text=""></asp:Label>

<asp:GridView ID="GridView1" runat="server" CssClass="GridViewStyle">
     <FooterStyle CssClass="GridViewFooterStyle" />
    <RowStyle CssClass="GridViewRowStyle" />
    <SelectedRowStyle CssClass="GridViewSelectedRowStyle" />
    <PagerStyle CssClass="GridViewPagerStyle" />
    <AlternatingRowStyle CssClass="GridViewAlternatingRowStyle" />
    <HeaderStyle CssClass="GridViewHeaderStyle" />

</asp:GridView>
