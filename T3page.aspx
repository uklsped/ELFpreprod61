<%@ Page Title="" Language="VB" MasterPageFile="~/Elf.master"  AutoEventWireup="false" CodeFile="T3page.aspx.vb" Inherits="T3page"  %>
<%@ MasterType VirtualPath="~/Elf.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="ErunupUserControlCommon.ascx" tagname="ErunupUserControlCommon" tagprefix="uc1" %>
<%--<%@ Register src="Preclinusercontrol.ascx" tagname="Preclinusercontrol" tagprefix="uc2" %>--%>
<%@ Register src="ClinicalUserControl.ascx" tagname="ClinicalUserControl" tagprefix="uc3" %>
<%@ Register src="LinacStatusuc.ascx" tagname="LinacStatusuc" tagprefix="uc5" %>
<%@ Register src="PlannedMaintenanceuc.ascx" tagname="PlannedMaintenanceuc" tagprefix="uc6" %>
<%@ Register src="Repairuc.ascx" tagname="Repairuc" tagprefix="uc7" %>
<%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc9" %>
<%@ Register src="Traininguc.ascx" tagname="Traininguc" tagprefix="uc12" %>
<%@ Register src="controls/ModalityDisplayuc.ascx" tagname="ModalityDisplayuc" tagprefix="uc8" %>
<%@ Register src="controls/AcceptLinacuc.ascx" tagname="AcceptLinacuc" tagprefix="uc10" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

  <%--  <script type="text/javascript" src="TabLoading.js"></script>--%>
  
    
 <div class="gridheader">
	

    <asp:UpdatePanel ID="UpdatePanel8" runat="server">
        <ContentTemplate>
    <div>
	
        <table style="width: 100%;">
            <tr>
                <td style="width: 260px"> <asp:Label ID="CurrentStateLabel" runat="server" Text="Current Linac State: " 
																				  
                                Height="50px"  Font-Size="XX-Large" BackColor="White" ForeColor="Black">
                                 </asp:Label></td>
                <td style="width: 350px">
                    <asp:Label ID="Statelabel" runat="server" BackColor="White" 
                     Font-Size="XX-Large" ForeColor="Black" Height="50px" Width="339px"></asp:Label></td>
                <td style="width: 150px">
                    <asp:Label ID="CurrentActivityLabel" runat="server" BackColor="White" 
                     Font-Size="Large" ForeColor="Black" Height="30px" Text="Current Activity: " 
                     Width="130px"></asp:Label></td>
                <td style="width: 200px"><asp:Label ID="ActivityLabel" runat="server" BackColor="White" 
                     Font-Size="Large" ForeColor="Black" Height="30px" Text=""></asp:Label></td>
                <td style="width: 117px"><asp:Label ID="CurrentUserGroupLabel" runat="server" BackColor="White" 
                     Font-Size="Large" ForeColor="Black" Height="30px" Text="Current User: "></asp:Label></td>
                <td style="width: 149px"><asp:Label ID="UserGroupLabel" runat="server" BackColor="White" 
                     Font-Size="Large" ForeColor="Black" Height="30px" Text=""></asp:Label></td>
                <td><asp:Button ID="EndOfDayButton" runat="server" Text="End of Day"  causesvalidation="false"/><br /><br />
                    <asp:Button ID="RestoreButton" runat="server" visible="true" CausesValidation="False" style="height: 26px" Text="RESTORE ELF" />
                </td>
                <td rowspan="2"><asp:Image ID="ELFImage" runat="server" AlternateText="ELF" Height="80px" 
												  
                     ImageUrl="~/Images/if_elf_62126.png" Width="100px" />
								  
                 <asp:Label ID="SoftwareVersion" runat="server" Text="Software Version 6.1"></asp:Label></td>
            </tr>
           
        </table>
       
								
 <asp:HiddenField ID="LAHiddenFieldcontrol" runat="server" />

</div>
		
      </ContentTemplate>
    </asp:UpdatePanel>
  </div>   
      <asp:Timer ID="Timer1" runat="server" Interval="7200000"></asp:Timer>
	
    <asp:UpdatePanel ID="UpdatePanelTimer" runat="server">
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
    </Triggers>
    <ContentTemplate>
	   
    </ContentTemplate>
        
       </asp:UpdatePanel>

	
  <input id="inpHide" type="hidden" runat="server" value="9" />

        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
             <asp:PlaceHolder ID="PlaceHolder4" runat="server">
            <uc9:WriteDatauc ID="WriteDatauc1" LinacName="T3" UserReason="10"  Tabby="EndofDay"  WriteName="EndDayData"   Visible="False" runat="server" />
           
            </asp:PlaceHolder>
            </ContentTemplate>
            </asp:UpdatePanel>


  <asp:TabContainer ID="tcl" runat="server" AutoPostback="true" activetabindex="0" OnActiveTabChanged="TabButton_Click" Ondemandmode="Once"  height="930px" >    
<asp:TabPanel runat="server" HeaderText="T3 Status" ID="TabPanel0"><HeaderTemplate>
T3 Status
</HeaderTemplate>
<ContentTemplate>
<asp:UpdatePanel ID="linacstatus" runat="server" >
    <ContentTemplate>

        <asp:UpdatePanel ID="UpdatePanel0" runat="server" >
            <ContentTemplate>
            <asp:Button ID="TabButton0" runat="server"  OnClick="TabButton_Click"  style="display:none;" CausesValidation="false"/>
            <asp:Panel ID="Panel0" runat="server" >
           
                    <uc5:LinacStatusuc ID="LinacStatusuc1" LinacName="T3" runat="server" />
                    </asp:Panel></ContentTemplate></asp:UpdatePanel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:TabPanel>

<%--This is the engineering run up tab--%>
        
<asp:TabPanel ID="TabPanel1" runat="server" HeaderText="T3 Runup" DynamicContextKey='Engrunup' CssClass="ajax__tab_header"><ContentTemplate>
<asp:UpdatePanel ID="signin" runat="server"
><ContentTemplate>
<asp:UpdatePanel ID="UpdatePanel1" runat="server" >
<ContentTemplate>
<asp:Button ID="TabButton1" runat="server"  OnClick="TabButton_Click"  style="display:none;" CausesValidation="false"/>
<asp:Panel ID="TabContent1" runat="server" Visible="False">

    <uc1:ErunupUserControlCommon ID="ErunupUserControl1" LinacName="T3" Tabby = "1" UserReason = "1" DataName="EngData" visible="false" runat="server" />

</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
    

</asp:TabPanel>


<%-- This is the Clinical Tab --%>
        
<asp:TabPanel ID="TabPanel3" runat="server" HeaderText="T3 Clinical">
        <ContentTemplate>
<asp:UpdatePanel ID="UpdatePanel3" runat="server" >
<ContentTemplate><asp:UpdatePanel ID="UpdatePanelClinical" Updatemode="Conditional" runat="server">
<ContentTemplate>
<asp:Button ID="TabButton3" runat="server" OnClick="TabButton_click" Style="Display: none" CausesValidation="false"/>
<asp:Panel ID="TabContent3" runat="server" Visible="false">
    <uc3:ClinicalUserControl ID="ClinicalUserControl1"  LinacName="T3" DataName="ClinData" runat="server" visible="false"/>
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:TabPanel>
        
        
<asp:TabPanel ID="TabPanel4" runat="server" HeaderText="T3 Planned Maintenance" >
        <ContentTemplate>
<asp:UpdatePanel ID="UpdatePanel4" runat="server" >
<ContentTemplate>
<asp:UpdatePanel ID="UpdatePanelMaintenance" runat="server">
<ContentTemplate><asp:Button ID="TabButton4" runat="server" OnClick="TabButton_click" Style="Display: none" CausesValidation="false"/>
<asp:Panel ID="TabContent4" runat="server" Visible="false">

<uc6:PlannedMaintenanceuc ID="PlannedMaintenanceuc1" linacname="T3" runat="server" Visible="false" />
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:TabPanel>
  
        
  <asp:TabPanel ID="TabPanel5" runat="server" HeaderText="T3 Repair" >
        <ContentTemplate>
<asp:UpdatePanel ID="UpdatePanel5" runat="server" >
<ContentTemplate>
<asp:UpdatePanel ID="UpdateRepair" runat="server">
<ContentTemplate>
<asp:Button ID="TabButton5" runat="server" OnClick="TabButton_click" Style="Display: none" CausesValidation="false"/>
<asp:Panel ID="TabContent5" runat="server" Visible="false">

<uc7:Repairuc ID="Repairuc1" LinacName="T3" runat="server" Visible="false" />
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:TabPanel>

 

<asp:TabPanel ID="TabPanel8" runat="server" HeaderText="T3 Development/Training" DynamicContextKey='Devel' tabindex="8" CssClass="ajax__tab_header"><ContentTemplate>
<asp:UpdatePanel ID="UpdateDevel" runat="server">
<ContentTemplate>
<asp:Button ID="TabButton8" runat="server"  OnClick="TabButton_Click"  style="display:none;" CausesValidation="false"/>
<asp:Panel ID="TabContent8" runat="server" Visible="False">

<uc12:Traininguc ID="Traininguc1" LinacName = "T3" Tabby="8" UserReason="8" Visible="false" runat="server" />
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:TabPanel> 

    
    </asp:TabContainer>
    <asp:Label ID="TargetControl" runat="server" style="display:none" causesvalidation="false" Visible="true" ></asp:Label>
    <asp:ModalPopupExtender ID="AcceptLinacModalPopup" runat="server"
                TargetControlID = "TargetControl"
                PopupControlID = "AcceptLinacPopup"
                BackgroundCssClass = "modalBackground"
                >
</asp:ModalPopupExtender>

   <asp:Panel ID="AcceptLinacPopup" runat="server" style="display:none"  Font-Underline="False" >
        
            <asp:UpdatePanel ID="UpdatePanel10" runat="server" >
            <ContentTemplate>
                <asp:PlaceHolder ID="AcceptLinacPlaceholder" runat="server"></asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
       
    </asp:Panel>

    
</asp:Content>

