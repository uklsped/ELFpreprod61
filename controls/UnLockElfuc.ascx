<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UnLockElfuc.ascx.vb" Inherits="controls_UnLockElfuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<asp:Label ID="Label1" runat="server" style="display:none" causesvalidation="false" Visible="true"></asp:Label>

<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
       <asp:UpdatePanel ID="UpdatePanel1" runat="server">

<ContentTemplate>
        <%--This is the log in popup panel  --%>
    
            <asp:Panel ID="Panel1" runat="server"   DefaultButton="UnlockElf"  Height="150px" Width="350px" >
                <div>
                    <table>
                        <tr>
                            <td class="style1">
                                Username:
                                <asp:TextBox ID="txtchkUserName" runat="server" style="margin-left: 0px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="style1">
                                Password:
                                <asp:TextBox ID="txtchkPWD" runat="server" TextMode="Password" />
                            </td>

                        </tr>
                        <tr>
                            <td class="style1">
                                <asp:Button ID="UnlockElf" runat="server" Text="Unlock Elf/Switch User" causesvalidation="false"/>
                            </td>
                            
                        </tr>
                    </table>
                </div>
                <asp:Label ID="LoginErrorDetails" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>



</asp:Panel>
    
</ContentTemplate>

</asp:UpdatePanel>