<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ReportFaultPopUpuc.ascx.vb" Inherits="controls_ReportFaultPopUpuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register src="../DefectSave.ascx" tagname="DefectSave" tagprefix="uc1" %>

<%@ Register src="../DefectSavePark.ascx" tagname="DefectSavePark" tagprefix="uc2" %>

<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<asp:Label ID="Label1" runat="server" style="display:none" causesvalidation="false" Visible="true"></asp:Label>
<asp:HiddenField ID="ParentTabComment" runat="server" />
<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Height="700px" Width="510px" cssclass="modalPopup" style="display:none" >
   <asp:UpdatePanel ID="UpdatePanel3" runat="Server" UpdateMode="Conditional">
                <ContentTemplate>
  <div >
      <%--<asp:PlaceHolder ID="PlaceHolderDefectSave" runat="server"></asp:PlaceHolder>--%>
      <asp:MultiView ID="DefectView" runat="server" ActiveViewIndex="0">
          <asp:View ID="LinacView" runat="server">
              <asp:PlaceHolder ID="PlaceHolderDefectSave" runat="server"></asp:PlaceHolder>
                
          </asp:View>
          <asp:View ID="TomoView" runat="server">
              <asp:PlaceHolder ID="PlaceHolderDefectSavePark" runat="server"></asp:PlaceHolder>
                <%--<uc2:DefectSavePark ID="DefectSavePark1" runat="server" />--%>
           </asp:View>
      </asp:MultiView>
  </div>
  <asp:Label ID="Label2" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>
      </ContentTemplate>
                </asp:UpdatePanel>
  <asp:Label ID="LoginErrorDetails" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>
            
</asp:Panel>

</ContentTemplate>
</asp:UpdatePanel>




