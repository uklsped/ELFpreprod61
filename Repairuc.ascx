<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Repairuc.ascx.vb" Inherits="Repairuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="controls/CommentBoxuc.ascx" tagname="CommentBoxuc" tagprefix="uc1" %>
<%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc2" %>
<%@ Register src="AtlasEnergyViewuc.ascx" tagname="AtlasEnergyViewuc" tagprefix="uc3" %>
<%@ Register src="controls/Modalitiesuc.ascx" tagname="Modalitiesuc" tagprefix="uc4" %>
<%@ Register src="controls/UnLockElfuc.ascx" tagname="UnLockElfuc" tagprefix="uc5" %>
<%--<%@ Register src="AcceptLinac.ascx" tagname="AcceptLinac" tagprefix="uc5" %>--%>
<%--<%@ Register src="ViewOpenFaults.ascx" tagname="ViewOpenFaults" tagprefix="uc5" %>--%>
<%@ Register src="ViewCommentsuc.ascx" tagname="ViewCommentsuc" tagprefix="uc6" %>
<%@ Register src="DefectSave.ascx" tagname="DefectSave" tagprefix="uc7" %>
<%--<%@ Register src="LockElfuc.ascx" tagname="LockElfuc" tagprefix="uc9" %>--%>
<%@ Register src="TodayClosedFault.ascx" tagname="TodayClosedFault" tagprefix="uc10" %>
<%@ Register src="DefectSavePark.ascx" tagname="DefectSavePark" tagprefix="uc11" %>

<%@ Register src="controls/ReportFaultPopUpuc.ascx" tagname="ReportFaultPopUpuc" tagprefix="uc8" %>

<%@ Register src="controls/MainFaultDisplayuc.ascx" tagname="MainFaultDisplayuc" tagprefix="uc12" %>

<%@ Register src="controls/ConcessionPopUpuc.ascx" tagname="ConcessionPopUpuc" tagprefix="uc13" %>

<%@ Register src="controls/NewFaultPopUpuc.ascx" tagname="NewFaultPopUpuc" tagprefix="uc14" %>

<%@ Register src="controls/ModalityQAPopUpuc.ascx" tagname="ModalityQAPopUpuc" tagprefix="uc15" %>

<%@ Register src="controls/ReportAFaultuc.ascx" tagname="ReportAFaultuc" tagprefix="uc16" %>

<%@ Register src="controls/ModalityDisplayuc.ascx" tagname="ModalityDisplayuc" tagprefix="uc17" %>

<uc2:WriteDatauc ID="WriteDatauc1" LinacName="" UserReason="5"  Tabby="5"  WriteName="Repair" visible="false" runat="server" />
<%--<uc9:LockElfuc ID="LockElfuc1" LinacName="" UserReason="5" Tabby="5" visible="false" runat="server" />--%>

<div class="clear" style="width:1863px">
    <asp:UpdatePanel ID="NewFaultPopUpUpdatePanel" runat="server">
        <ContentTemplate>
    <asp:Panel ID="NewFaultPopUpPanel" runat="server">
     <asp:PlaceHolder ID="NewFaultPopupPlaceHolder" runat="server"></asp:PlaceHolder>
    </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
</div>
<div class="grid">
<div class="col100 grey">
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
            <td style="width:300px">
                <asp:Label ID="StateLabel" runat="server" Text="Last State:"></asp:Label><br />
                <asp:TextBox ID="StateTextBox" ReadOnly="true" runat="server"></asp:TextBox>
            </td>
            <td style="width:145px">
                <asp:Button ID="LogOffButton" runat="server" Text="Log Off" causesvalidation="false" Enabled="false"  Height="50px" Width="183px" />
            </td>
        </tr>
        <tr>
            <td rowspan="4" style="width:300px">
                <asp:RadioButtonList ID="RadioButtonList1" CausesValidation="false" runat="server" AutoPostBack="True"></asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td style="width:145px;">
                <asp:Button ID="LockElf" runat="server" Text="Lock Elf/Switch User" causesvalidation="false"/>
            </td>
        </tr>
        <tr>
            <td style="width:145px;">
                <asp:Button ID="PhysicsQA" runat="server" Text="View Physics Energies/Imaging" Visible="false" Width="200px" CausesValidation="false"  />
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="height:92px">
                <fieldset>
                    <legend style="font-family: Arial, Arial, Helvetica, sans-serif; font-weight:bold">
                        Repair Comments
                    </legend>
                    <uc1:CommentBoxuc ID="CommentBox" runat="server" />
                </fieldset>
            </td>
        </tr>
         <tr><td colspan="2">
              <br />
              <br />
              <asp:UpdatePanel ID="ReportAFaultucUpdatePanel" runat="server">
<ContentTemplate>
    <uc16:ReportAFaultuc ID="ReportAFaultuc1" runat="server" />

</ContentTemplate>
</asp:UpdatePanel> 
              </td></tr>
       <tr>
           <td colspan="2">
               <asp:UpdatePanel ID="UpdatePanel4" runat="server" Visible="true">
<ContentTemplate>
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
    </asp:PlaceHolder>
        </ContentTemplate>
        
</asp:UpdatePanel>

           </td>              
        </tr>
        </table>
</div>

<div class="col300 green">
    <asp:UpdatePanel ID="PlaceHolderFaultsUpdatePanel" runat="server" Visible="true" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolderFaults" runat="server"></asp:PlaceHolder>&nbsp;
        </ContentTemplate>   
    </asp:UpdatePanel>
</div>
               
   
               
</div>
<div id="left">

<asp:UpdatePanel ID="PlaceHolderModalitiesUpdatePanel" runat="server" Visible="true">
<ContentTemplate>
    <asp:PlaceHolder ID="PlaceHolderModalities" runat="server"></asp:PlaceHolder>
</ContentTemplate>
</asp:UpdatePanel>
</div>
<div id="right"></div>

<uc6:ViewCommentsuc ID="ViewCommentsuc1" LinacName="" CommentSort="rp" runat="server" />
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











