<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="monthly_general_stats.aspx.cs" Inherits="statistika_net4.statistika.monthly_general_stats" %>





<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .auto-style1 {
            width: 68px;
        }
        .auto-style2 {
            width: 68px;
            height: 62px;
        }
        .auto-style3 {
            height: 62px;
        }
    </style>

    <style type="text/css">
    .modal
    {
        position: fixed;
        top: 0;
        left: 0;
        background-color: black;
        z-index: 99;
        opacity: 0.8;
        filter: alpha(opacity=80);
        -moz-opacity: 0.8;
        min-height: 100%;
        width: 100%;
    }
    .loading
    {
        font-family: Arial;
        font-size: 10pt;
        border: 5px solid #67CFF5;
        width: 200px;
        height: 100px;
        display: none;
        position: fixed;
        background-color: White;
        z-index: 999;
    }
</style>


    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script type="text/javascript">
    function ShowProgress() {
        setTimeout(function () {
            var modal = $('<div />');
            modal.addClass("modal");
            $('body').append(modal);
            var loading = $(".loading");
            loading.show();
            var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
            var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
            loading.css({ top: top, left: left });
        }, 200);
    }
    $('form').live("submit", function () {
        ShowProgress();
    });
</script>




</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Label ID="Label3" runat="server" Text="Σύνδεση στη βάση:"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:DropDownList ID="DropDownListDatabase" runat="server" Height="26px" Width="117px">
        <asp:ListItem Value="gkt116">KT1-16</asp:ListItem>
        <asp:ListItem Value="gkt118">KT1-18</asp:ListItem>
        <asp:ListItem Value="gkt207">KT2-07</asp:ListItem>
        <asp:ListItem Value="gkt210">KT2-10</asp:ListItem>
        <asp:ListItem Value="gkt501">KT5-01</asp:ListItem>
        <asp:ListItem Value="gkt505">KT5-05</asp:ListItem>
        <asp:ListItem Value="gkt507">KT5-07</asp:ListItem>
    </asp:DropDownList>
    <br />
    <br />

&nbsp;&nbsp;<asp:Button ID="Button2" runat="server" Height="33px" Text="CSV" Width="56px" OnClick="Button2_Click1" />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <table style="width:100%;">
        <tr>
            <td class="auto-style1">
    <asp:Label ID="LabelApo" runat="server" Text="Από:"></asp:Label>
            &nbsp;&nbsp;&nbsp;
                <asp:Label ID="LabelApoDate" runat="server" Text="_"></asp:Label>
            </td>
            <td>
    <asp:Label ID="LabelEos" runat="server" Text="Έως:"></asp:Label>
            &nbsp;&nbsp;&nbsp;
                <asp:Label ID="LabelEosDate" runat="server" Text="_"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="auto-style2">
                    <asp:Calendar ID="CalendarStart" runat="server">
                    </asp:Calendar>
                    </td>
            <td class="auto-style3">
                    <asp:Calendar ID="CalendarEnd" runat="server">
                    </asp:Calendar>
                </td>
        </tr>
    </table>
    <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnSubmit" runat="server" Text="Προβολή" OnClick="Button1_Click" Width="96px" />
        <br />
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
    <br />
    <br />
                <br />



        <div class="loading" align="center">
    υπολογισμός στατιστικών. Παρακαλώ περιμένετε.<br />
    <asp:Image ID="Image1" runat="server" Height="68px" ImageUrl="~/loader.gif" Width="100px" />
</div>


</asp:Content>
