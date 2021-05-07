<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Traininguc.ascx.vb" Inherits="Traininguc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="controls/ReportAFaultuc.ascx" tagname="ReportAFaultuc" tagprefix="uc1" %>
<%--<%@ Register src="DefectSavePark.ascx" tagname="DefectSavePark" tagprefix="uc1" %>--%>
<%@ Register Src="WriteDatauc.ascx" TagName="WriteDatauc" TagPrefix="uc3" %>
<%--<%@ Register Src="AtlasEnergyViewuc.ascx" TagName="AtlasEnergyViewuc" TagPrefix="uc4" %>
<%@ Register Src="Modalitiesuc.ascx" TagName="Modalitiesuc" TagPrefix="uc5" %>
<%@ Register Src="ViewOpenFaults.ascx" TagName="ViewOpenFaults" TagPrefix="uc6" %>--%>
<%@ Register Src="ViewCommentsuc.ascx" TagName="ViewCommentsuc" TagPrefix="uc7" %>
<%--<%@ Register src="AcceptLinac.ascx" tagname="AcceptLinac" tagprefix="uc8" %>--%>
<%--<%@ Register src="DefectSave.ascx" tagname="DefectSave" tagprefix="uc9" %>
<%@ Register src="TodayClosedFault.ascx" tagname="TodayClosedFault" tagprefix="uc10" %>--%>
<%@ Register src="controls/CommentBoxuc.ascx" tagname="CommentBoxuc" tagprefix="uc2" %>

<%@ Register src="controls/MainFaultDisplayuc.ascx" tagname="MainFaultDisplayuc" tagprefix="uc11" %>

<%@ Register src="controls/ReportFaultPopUpuc.ascx" tagname="ReportFaultPopUpuc" tagprefix="uc12" %>

<uc3:WriteDatauc ID="WriteDatauc1" LinacName="" UserReason="8" Tabby="8" Visible="false" runat="server" />
<div class="grid">
    <div class="col100 grey">

        <table id="HandoverTable">
            <tr style="vertical-align:top">
                <td style="width:300px"> <asp:Label ID="StateLabel" runat="server" Text="Last State:"></asp:Label><br />
                    <asp:TextBox ID="StateTextBox" runat="server"></asp:TextBox></td>
                <td><asp:Button ID="LogOffButton" runat="server" Text="Log Off" CausesValidation="false"
                        Enabled="False" Height="50px" Width="180px" /></td>
                
            </tr>
            <tr>
                <td ><asp:RadioButtonList ID="RadioButtonList1" CausesValidation="false" runat="server"
                        AutoPostBack="True">
                    </asp:RadioButtonList></td>
               
            </tr>
           
         <tr>          
           <td colspan="2" style="height: 92px">
               <table>
                 <tr>
                     <td><asp:Literal ID="Literal1" runat="server" Text="Training Comments"></asp:Literal></td>
                 </tr>
                 <tr>
                     <td><uc2:CommentBoxuc ID="CommentBox" runat="server" />
                         
                     </td>
                 </tr>
               </table>
           </td>                         
        </tr>
        <tr>
           <td colspan="2">
              <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <uc1:ReportAFaultuc ID="ReportAFaultuc1" runat="server" />
                 <%-- <asp:Label ID="Label2" runat="server" Text="Major Faults Cleared Today:"></asp:Label>
                  <asp:PlaceHolder ID="PlaceHolderTodaysclosedfaults" runat="server">
                  </asp:PlaceHolder>--%>
                </ContentTemplate>
              </asp:UpdatePanel>
           </td>              
        </tr>
      </table>

</div>
<%-- <div class="col200 blue" ><asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <asp:Button ID="ReportFaultButton" runat="server" Text="Report Fault" CausesValidation="false"/>
                            <asp:PlaceHolder ID="ReportFaultPopupPlaceHolder" runat="server"></asp:PlaceHolder>
                        </ContentTemplate>
                   </asp:UpdatePanel></div>--%>
    <div class="col300 green" ><asp:UpdatePanel ID="UpdatePanel2" runat="server" Visible="true" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolderFaults" runat="server"></asp:PlaceHolder>&nbsp;
        </ContentTemplate>
    </asp:UpdatePanel>
    </div>
    
    
    
</div>
        
