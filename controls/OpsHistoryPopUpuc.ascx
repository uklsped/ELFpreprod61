<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OpsHistoryPopUpuc.ascx.vb" Inherits="controls_OpsHistoryPopUpuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<asp:Label ID="Label1" runat="server" style="display:none" causesvalidation="false" Visible="true"></asp:Label>
<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Height="700px" Width="600px" cssclass="modalPopup" style="display:none" >
   <asp:UpdatePanel ID="UpdatePanel3" runat="Server" UpdateMode="Conditional">
                <ContentTemplate>
  <div >
             <asp:PlaceHolder ID="PlaceHolderOpsHistory" runat="server">
    </asp:PlaceHolder>
  </div>
  <asp:Label ID="Label2" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>
      </ContentTemplate>
                </asp:UpdatePanel>
  <asp:Label ID="LoginErrorDetails" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>
            
</asp:Panel>

</ContentTemplate>
</asp:UpdatePanel>


























