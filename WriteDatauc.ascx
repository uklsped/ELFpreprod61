<%@ Control Language="VB" AutoEventWireup="false" CodeFile="WriteDatauc.ascx.vb" Inherits="WriteDatauc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<asp:Label ID="Label1" runat="server" style="display:none" causesvalidation="false" Visible="true"></asp:Label>

<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Panel ID="Panel1" runat="server" Height="150px" Width="350px" cssclass="modalPopup" style="display:none" defaultbutton="AcceptOK">
  <div >
     <table id="table" >
      <tr>
          <td>
            <asp:Label ID="WarningLabel" runat="server" Text=""></asp:Label>
          </td>
      </tr>
      <tr>
         <td class="style1">Username: </td>
         <td><asp:TextBox ID="txtchkUserName" runat="server" style="margin-left: 0px" /></td>
      </tr>
      <tr>
         <td class="style1">Password:  </td>
         <td><asp:TextBox ID="txtchkPWD" runat="server" TextMode="Password" /></td>
      </tr>
      <tr>
         <td class="style1"><asp:Button ID="AcceptOK" runat="server" Text="" causesvalidation="false" /></td>
         <td class="style1"><asp:Button ID="btnchkcancel" runat="server" causesvalidation="false" Text="Cancel" /></td>
      </tr>
         <tr>
             <td>
                 <asp:Label ID="LoginErrorDetails" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>
             </td>

         </tr>
    </table>
  </div>
  <%--<asp:Label ID="LoginErrorDetails" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>--%>
            
</asp:Panel>

</ContentTemplate>
</asp:UpdatePanel>
   
