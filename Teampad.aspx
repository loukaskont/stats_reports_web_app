<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Teampad.aspx.cs" Inherits="Statistika.statistika.Teampad" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ID="MainContent" ContentPlaceHolderID="MainContent">

        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="CSV" Width="57px" />

        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
        <br />
        <br />
        <asp:Panel ID="Panel1" runat="server" Height="113px" Width="1032px">
        </asp:Panel>

</asp:Content>
