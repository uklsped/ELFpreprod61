<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AcceptLinacuc.ascx.vb" Inherits="AcceptLinacuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%--<%@ Register src="EnergyDisplayuc.ascx" tagname="EnergyDisplayuc" tagprefix="uc1" %>--%>


<%@ Register src="../EnergyDisplayuc.ascx" tagname="EnergyDisplayuc" tagprefix="uc1" %>




<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

            
 <asp:Panel ID="AcceptLinacDisplay" runat="server" DefaultButton="AcceptOK" cssclass="modalPopup" Height="150px" Width="350px" Font-Underline="False">
                <div>
                    <asp:Label ID="AcceptTabLabel" runat="server" Text=""></asp:Label>
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
    

