<%@ Page Title="" Language="VB" MasterPageFile="~/Elf.master" AutoEventWireup="false" CodeFile="Oops.aspx.vb" Inherits="Error_pages_oops" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<h2>An Error Has Occurred</h2>
<p>An unexpected error has occurred with ELF.</p>

<p>The ELF administrator has been informed</p>
<li>
<asp:HyperLink ID="LnkHome" runat="server" NavigateUrl="~/LA1Page.aspx">Return to LA1</asp:HyperLink>
</li>
    
</asp:Content>

