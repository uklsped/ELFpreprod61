<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PlannedMaintenanceuc.ascx.vb" Inherits="Planned_Maintenanceuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="controls/UnLockElfuc.ascx" tagname="UnLockElfuc" tagprefix="uc1" %>
<%--<%@ Register src="LockElfuc.ascx" tagname="LockElfuc" tagprefix="uc1" %>--%>
<%@ Register src="DefectSavePark.ascx" tagname="DefectSavePark" tagprefix="uc10" %>
<%@ Register src="controls/CommentBoxuc.ascx" tagname="CommentBoxuc" tagprefix="uc2" %>
<%@ Register Src="WriteDatauc.ascx" TagName="WriteDatauc" TagPrefix="uc3" %>
<%@ Register Src="AtlasEnergyViewuc.ascx" TagName="AtlasEnergyViewuc" TagPrefix="uc4" %>
<%@ Register src="controls/Modalitiesuc.ascx" tagname="Modalitiesuc" tagprefix="uc5" %>
<%--<%@ Register Src="ViewOpenFaults.ascx" TagName="ViewOpenFaults" TagPrefix="uc6" %>--%>
<%@ Register Src="ViewCommentsuc.ascx" TagName="ViewCommentsuc" TagPrefix="uc7" %>
<%@ Register src="DefectSave.ascx" tagname="DefectSave" tagprefix="uc8" %>
<%@ Register src="TodayClosedFault.ascx" tagname="TodayClosedFault" tagprefix="uc9" %>
<%@ Register src="controls/MainFaultDisplayuc.ascx" tagname="MainFaultDisplayuc" tagprefix="uc11" %>
<%@ Register src="ManyFaultGriduc.ascx" tagname="ManyFaultGriduc" tagprefix="uc12" %>
<%@ Register src="controls/ReportFaultPopUpuc.ascx" tagname="ReportFaultPopUpuc" tagprefix="uc13" %>

<%@ Register src="controls/ModalityQAPopUpuc.ascx" tagname="ModalityQAPopUpuc" tagprefix="uc14" %>

<%@ Register src="controls/ReportAFaultuc.ascx" tagname="ReportAFaultuc" tagprefix="uc15" %>

<%@ Register src="controls/ModalityDisplayuc.ascx" tagname="ModalityDisplayuc" tagprefix="uc16" %>

<%--<div class="clear" style="width:1863px">--%>
    
<%--</div>--%>
<div class="grid" >
    <div class="col100 grey" >
         
        <table id="Handover">
            <tr style="vertical-align:top">
            <td colspan="3">
                <asp:Panel ID="ModalityDisplayPanel" runat="server" Visible="false">
                       <asp:PlaceHolder ID="ModalityPlaceholder" runat="server">
                       </asp:PlaceHolder>
                   </asp:Panel>
            </td>
                </tr>
            <tr style="vertical-align:top">
                <td style="width: 300px">
                  <asp:Label ID="StateLabel" runat="server" Text="Last State:"></asp:Label><br />
                    <asp:TextBox ID="StateTextBox" runat="server" ReadOnly="True"></asp:TextBox>
                  </td>
                <td style="width:145px">
                     <asp:Button ID="LogOffButton" runat="server" Text="Log Off" 
                         CausesValidation="false" Enabled="False" Height="50px" Width="183px" />
                </td>
                </tr>
            <tr><td rowspan="4" style="width:300px">
                <asp:RadioButtonList ID="RadioButtonList1" CausesValidation="false" runat="server" AutoPostBack="True"></asp:RadioButtonList>
                </td></tr>
            <tr>
                <td style="width: 145px">
            <asp:Button ID="LockElf" runat="server" Text="Lock Elf/Switch User" causesvalidation="false"/>                            
                </td>
            </tr>
            <tr>
                <td style="width: 145px">
                    <asp:Button ID="PhysicsQA" runat="server" Text="View Physics Energies/Imaging" visible="false"
                        CausesValidation="false" Width="200px" />
                </td>
            </tr>
            <tr>
                <td>
                    <%--<asp:Button ID="ViewAtlasButton" runat="server" Text="View Atlas Energies" CausesValidation="false" />--%>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="height:92px"><fieldset>
                    <legend style="font-family: Arial, Helvetica, sans-serif; font-weight: bold">
                        Maintenance Comments
                   </legend>
                  <uc2:CommentBoxuc ID="CommentBox" runat="server" />
                    
                   </fieldset></td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <br />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc15:ReportAFaultuc ID="ReportAFaultuc1" runat="server" />
                            
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>

            </table>
    </div>
   <%-- <div class="col200 blue" ><asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <asp:Button ID="ReportFaultButton" runat="server" Text="Report Fault" CausesValidation="false"/>
                            <uc13:ReportFaultPopUpuc ID="ReportFaultPopUpuc1" LinacName="" Visible="false" ParentControl="4" runat ="server" />
                        <asp:PlaceHolder ID="ReportFaultPopupPlaceHolder" runat="server"></asp:PlaceHolder>
                        </ContentTemplate>
                   </asp:UpdatePanel></div>--%>
    <div class="col300 green" ><asp:UpdatePanel ID="UpdatePanel2" runat="server" Visible="true" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolderFaults" runat="server"></asp:PlaceHolder>&nbsp;
        </ContentTemplate>
    </asp:UpdatePanel>
         <asp:UpdatePanel ID="UpdatePanelQA" runat="server" Visible="true">
        <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolderModalities" runat="server"></asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
    </div>
</div>
        <uc3:WriteDatauc ID="WriteDatauc1" LinacName="" UserReason="4" Tabby="4" Visible="false" runat="server" />
       <%-- <uc1:lockelfuc ID="LockElfuc1" LinacName="" UserReason="4" Tabby="4"  visible="false" runat="server" />--%>
<%--<div id="left">--%>
    <%--<asp:UpdatePanel ID="UpdatePanelAtlas" runat="server" Visible="false" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolder2" runat="server"></asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
   <%-- <asp:UpdatePanel ID="UpdatePanelQA" runat="server" Visible="true">
        <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolderModalities" runat="server"></asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
<%--</div>
<div id="right">
</div>--%>
<div>
    
<uc7:ViewCommentsuc ID="ViewCommentsuc1" LinacName="" CommentSort="pm" runat="server" />
</div>
 <asp:Label ID="TargetControl" runat="server" style="display:none" causesvalidation="false" Visible="true" ></asp:Label>
    <asp:ModalPopupExtender ID="LockELFModalPopup" runat="server"
                TargetControlID = "TargetControl"
                PopupControlID = "LockELFPopup"
                BackgroundCssClass = "modalBackground"
                >
</asp:ModalPopupExtender>

   <asp:Panel ID="LockELFPopup" runat="server" style="display:none" CssClass="modalPopup" Height="150px" 
            Width="350px" Font-Underline="False" >
        
            <asp:UpdatePanel ID="UpdatePanel10" runat="server" >
            <ContentTemplate>
                <asp:PlaceHolder ID="LockELFPlaceholder" runat="server"></asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
       
    </asp:Panel>

