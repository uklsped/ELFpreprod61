﻿<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AcceptLinac.ascx.vb" Inherits="AcceptLinac" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register src="EnergyDisplayuc.ascx" tagname="EnergyDisplayuc" tagprefix="uc1" %>


<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />
<asp:Label ID="Label1" runat="server" style="display:none" causesvalidation="false" Visible="true" ></asp:Label>




<asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
       <asp:UpdatePanel ID="UpdatePanel1" runat="server">

<ContentTemplate>
        <%--This is the log in popup panel  --%>
            
            <asp:Panel ID="Panel1" runat="server"  style="display:none" 
            DefaultButton="AcceptOK" CssClass="modalPopup" Height="150px" 
            Width="350px" Font-Underline="False">
                <div>
                    <table>
                        
                        <tr>
                         <td>
                            <asp:PlaceHolder ID="PlaceHolder2" runat="server"></asp:PlaceHolder>
                            </td>
                        </tr>
                        <tr>
                            <td class="style1">
                                Username:
                                <asp:TextBox ID="txtchkUserName" runat="server" style="margin-left: 0px" />
                            </td>
                            <td></td>
                        </tr>
                        
                        <tr>
                            <td class="style1">
                                Password:
                                <asp:TextBox ID="txtchkPWD" runat="server" TextMode="Password" />
                            </td>

                        </tr>
                        <tr>
                            <td class="style1">
                                <asp:Button ID="AcceptOK" runat="server" Text="Accept Linac" causesvalidation="false"/>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  
                                <asp:Button ID="btnchkcancel" runat="server" causesvalidation="false" 
                                    Text="Cancel" />
                            </td>
                           
                        </tr>
                    </table>
                </div>
                <asp:Label ID="LoginErrorDetails" runat="server" Font-Italic="True" ForeColor="Red"></asp:Label>



</asp:Panel>
    
</ContentTemplate>

</asp:UpdatePanel>
<%--<uc1:EnergyDisplayuc ID="EnergyDisplayuc1" runat="server" />--%>
