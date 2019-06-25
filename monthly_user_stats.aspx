<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="monthly_user_stats.aspx.cs" Inherits="statistika_net4.statistika.monthly_user_stats" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 331px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    &nbsp&nbsp<br />
&nbsp;&nbsp;&nbsp;    
    <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="CSV" Width="55px" />
    <asp:Label ID="Label5" runat="server" Text="Σύνδεση στη βάση:"></asp:Label>
&nbsp;&nbsp;
    <asp:DropDownList ID="DropDownListDatabase" runat="server" Height="26px" Width="117px" AutoPostBack="True">
        <asp:ListItem Value="gkt116">KT1-16</asp:ListItem>
        <asp:ListItem Value="gkt118">KT1-18</asp:ListItem>
        <asp:ListItem Value="gkt207">KT2-07</asp:ListItem>
        <asp:ListItem Value="gkt210">KT2-10</asp:ListItem>
        <asp:ListItem Value="gkt501">KT5-01</asp:ListItem>
        <asp:ListItem Value="gkt505">KT5-05</asp:ListItem>
        <asp:ListItem Value="gkt507">KT5-07</asp:ListItem>
    </asp:DropDownList>
    <br />
&nbsp;<table style="width:100%;">
        <tr>
            <td class="auto-style1">
                <asp:Label ID="Label1" runat="server" Text="Από:"></asp:Label>
&nbsp;&nbsp;&nbsp;
                <asp:Label ID="LabelApoDate" runat="server" Text="_"></asp:Label>
            </td>
            <td>
                <asp:Label ID="Label3" runat="server" Text="Έως:"></asp:Label>
&nbsp;&nbsp;&nbsp;
                <asp:Label ID="LabelEosDate" runat="server" Text="_"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="auto-style1">
                <asp:Calendar ID="CalendarStart" runat="server">
                </asp:Calendar>
            </td>
            <td>
                <asp:Calendar ID="CalendarEnd" runat="server">
                </asp:Calendar>
            </td>
        </tr>
    </table>
    &nbsp;&nbsp;&nbsp;

<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<asp:Button ID="Button1" runat="server" Text="Προβολή" OnClick="Button1_Click" Width="100px" />
    <br />
    <br />
    <asp:GridView ID="GridView1" runat="server">
    </asp:GridView>
<br />

</asp:Content>
