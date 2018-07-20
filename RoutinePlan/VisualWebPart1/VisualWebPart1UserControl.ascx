<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VisualWebPart1UserControl.ascx.cs" Inherits="RoutinePlan.VisualWebPart1.VisualWebPart1UserControl" %>
<table >
    <tr>
        <th>
            周期
        </th>
        <th>
            模式
        </th>
        <th>
            次数
        </th>

    </tr>
    <tr>
        <td style="vertical-align:top;">
            <asp:RadioButtonList ID="rblistCycle" runat="server" OnSelectedIndexChanged="rblistCycle_SelectedIndexChanged">
                <asp:ListItem Value="rbDay">按天</asp:ListItem>
                <asp:ListItem Value="rbWeek">按周</asp:ListItem>
                <asp:ListItem Value="rbMonth">按月</asp:ListItem>
                <asp:ListItem Value="rbYear">按年</asp:ListItem>

</asp:RadioButtonList>
        </td>
        <td style="vertical-align:top;">

            <asp:RadioButtonList ID="rblistDayModel" runat="server">
                <asp:ListItem Value="mDays">每一天</asp:ListItem>
                <asp:ListItem Value="mWorkDays">每个工作日</asp:ListItem>
            </asp:RadioButtonList>

            <div id="WeekModel">
                每<asp:TextBox ID="tbweeksCount" runat="server" Width="20px"></asp:TextBox>周重复，重复时间：
                <asp:CheckBoxList ID="CheckBoxList1" runat="server" RepeatColumns="3" RepeatDirection="Horizontal">
                    <asp:ListItem Value="weekMon">周一</asp:ListItem>
                    <asp:ListItem Value="weekTues">周二</asp:ListItem>
                    <asp:ListItem Value="weekWed">周三</asp:ListItem>
                    <asp:ListItem Value="weekThur">周四</asp:ListItem>
                    <asp:ListItem Value="weekFri">周五</asp:ListItem>
                    <asp:ListItem Value="weekSat">周六</asp:ListItem>
                    <asp:ListItem Value="weekSun">周日</asp:ListItem>
                </asp:CheckBoxList>
            </div>
            <div>
                每<asp:TextBox ID="tbMonthCount1" runat="server" Width="20px"></asp:TextBox>个月的第<asp:TextBox ID="tbMonthDayCount1" runat="server" Width="20px"></asp:TextBox>日<br/>
                每<asp:TextBox ID="tbMonthCount2" runat="server" Width="20px"></asp:TextBox>个月的
                <asp:DropDownList ID="ddlWeekCount" runat="server">
                    <asp:ListItem Value="1">第一个</asp:ListItem>
                    <asp:ListItem Value="2">第二个</asp:ListItem>
                    <asp:ListItem Value="3">第三个</asp:ListItem>
                    <asp:ListItem Value="4">第四个</asp:ListItem>
                    <asp:ListItem Value="0">最后一个</asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="ddlWeekDay" runat="server">
                    <asp:ListItem Value="weekMon">周末</asp:ListItem>
                    <asp:ListItem Value="weekMon">工作日</asp:ListItem>
                    <asp:ListItem Value="weekMon">周一</asp:ListItem>
                    <asp:ListItem Value="weekTues">周二</asp:ListItem>
                    <asp:ListItem Value="weekWed">周三</asp:ListItem>
                    <asp:ListItem Value="weekThur">周四</asp:ListItem>
                    <asp:ListItem Value="weekFri">周五</asp:ListItem>
                    <asp:ListItem Value="weekSat">周六</asp:ListItem>
                    <asp:ListItem Value="weekSun">周日</asp:ListItem>
                </asp:DropDownList>
            </div>


        </td>
        <td style="vertical-align:top;">
            日期范围：
            开始日期：<SharePoint:DateTimeControl ID="dtStart" runat="server" DateOnly="True" />
            截止日期：<SharePoint:DateTimeControl ID="dtEnd" runat="server" DateOnly="True" />
            <br />
            无截止日期
            <br/>
            重复<asp:TextBox ID="tbCount" runat="server" Width="20px"></asp:TextBox>次

        </td>
    </tr>
</table>


