<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="genikoSinolo.aspx.cs" Inherits="statistika_net4.statistika.genikoSinolo" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ID="MainContent" ContentPlaceHolderID="MainContent">
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
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="CSV" Width="56px" />
        <asp:Label ID="Label1" runat="server" Text="Σύνδεση στη Βάση:"></asp:Label>
		&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="DropDownListDatabase" runat="server" Height="26px" Width="117px">
            <asp:ListItem Value="gkt116">KT1-16</asp:ListItem>
            <asp:ListItem Value="gkt118">KT1-18</asp:ListItem>
            <asp:ListItem Value="gkt207">KT2-07</asp:ListItem>
            <asp:ListItem Value="gkt210">KT2-10</asp:ListItem>
            <asp:ListItem Value="gkt501">KT5-01</asp:ListItem>
            <asp:ListItem Value="gkt505">KT5-05</asp:ListItem>
            <asp:ListItem Value="gkt507">KT5-07</asp:ListItem>
        </asp:DropDownList> &nbsp; 
		<asp:Button ID="btnSubmit" runat="server" Text="Προβολή" OnClick="Button1_Click" Width="96px" />
        <br />
        <br />
        <asp:Label ID="Label2" runat="server"></asp:Label>
        <asp:GridView ID="GridViewGeotemaxia" runat="server" Height="155px" Width="342px">
        </asp:GridView>
        <br />
        <br />
        <asp:Label ID="Label3" runat="server"></asp:Label>
        <asp:GridView ID="GridViewDikaiomaton" runat="server" Height="155px" Width="342px">
        </asp:GridView>
        



    <div class="loading" align="center">
    υπολογισμός στατιστικών. Παρακαλώ περιμένετε.<br />
        <asp:Image ID="Image1" runat="server" Height="68px" ImageUrl="~/loader.gif" Width="100px" />
</div>


</asp:Content>
