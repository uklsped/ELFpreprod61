<%@ Control Language="VB" AutoEventWireup="false" CodeFile="NewFaultPopUpuc.ascx.vb" Inherits="controls_NewFaultPopUpuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register src="FaultTrackinguc.ascx" tagname="FaultTrackinguc" tagprefix="uc1" %>

<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<asp:Label ID="Label1" runat="server" style="display:none" causesvalidation="false" Visible="true"></asp:Label>

<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Height="600px" Width="970px" cssclass="modalPopup" style="display:none" >
  <div >
     <uc1:FaultTrackinguc ID="FaultTrackinguc1" runat="server" />
  </div>
  <asp:Label ID="LoginErrorDetails" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>
            
</asp:Panel>

</ContentTemplate>
</asp:UpdatePanel>

