<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ConfirmPage.ascx.vb" Inherits="ConfirmPage" %>
<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />
<asp:Label ID="Label1" runat="server" style="display:none" causesvalidation="false" Visible="true"></asp:Label>

<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">

<ContentTemplate>
        <%--This is the log in popup panel  --%>
            <asp:Panel ID="Panel1" runat="server" Height="150px" Width="350px" cssclass="modalPopup" style="display:none" defaultbutton="AcceptOK">
                <div >
                    <table width="300px">
                        <tr>
                            <p>Are you sure you don't want to confirm an Imaging Modality?</p>
                        </tr>
                        <tr>
                            <td class="style1">
                                <asp:Button ID="AcceptOK" runat="server" Text="" causesvalidation="false"/>
                                                                </td>
                                                                <td></td>
                                <td class="style1">  
                                <asp:Button ID="btnchkcancel" runat="server" causesvalidation="false" 
                                    Text="Cancel" />
                            </td>
                        </tr>
                    </table>
                </div>
                
</asp:Panel>
    
</ContentTemplate>

</asp:UpdatePanel>