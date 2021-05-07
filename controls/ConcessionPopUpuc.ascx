<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ConcessionPopUpuc.ascx.vb" Inherits="controls_ConcessionPopUpuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="FaultTrackinguc.ascx" tagname="FaultTrackinguc" tagprefix="uc1" %>
<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<asp:Label ID="Label1" runat="server" style="display:none" causesvalidation="false" Visible="true"></asp:Label>

<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Height="600px" Width="970px" cssclass="modalPopup" style="display:none" >
            <asp:UpdatePanel ID="udpInnerUpdatePanel" runat="Server" UpdateMode="Conditional">
                <ContentTemplate>
  <div >
      <asp:PlaceHolder ID="FaultTrackingPlaceHolder" runat="server"></asp:PlaceHolder>
      <%--<uc1:FaultTrackinguc ID="FaultTrackinguc1" runat="server" />--%>
  </div>
  <asp:Label ID="LoginErrorDetails" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>
      </ContentTemplate>
                </asp:UpdatePanel>
</asp:Panel>

</ContentTemplate>
</asp:UpdatePanel>


