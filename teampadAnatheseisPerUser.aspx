<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="teampadAnatheseisPerUser.aspx.cs" Inherits="statistika_net4.statistika.teampadAnatheseisPerUser" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="FeaturedContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="CSV" Width="55px" />
    <asp:GridView ID="GridView1" runat="server">
    </asp:GridView>
</asp:Content>
