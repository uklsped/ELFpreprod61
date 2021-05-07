<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ErunupUserControlCommon.ascx.vb" Inherits="ErunupUserControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register src="AtlasEnergyViewuc.ascx" tagname="AtlasEnergyViewuc" tagprefix="uc2" %>

<%@ Register src="controls/CommentBoxuc.ascx" tagname="CommentBoxuc" tagprefix="uc3" %>

<%@ Register src="ViewCommentsuc.ascx" tagname="ViewCommentsuc" tagprefix="uc5" %>

<%@ Register src="DefectSave.ascx" tagname="DefectSave" tagprefix="uc6" %>

<%@ Register src="TodayClosedFault.ascx" tagname="TodayClosedFault" tagprefix="uc7" %>



<%@ Register src="controls/UnLockElfuc.ascx" tagname="UnLockElfuc" tagprefix="uc9" %>



<%--<%@ Register src="LockElfuc.ascx" tagname="LockElfuc" tagprefix="uc9" %>--%>

<%@ Register src="controls/Modalitiesuc.ascx" tagname="Modalitiesuc" tagprefix="uc10" %>

<%@ Register src="DefectSavePark.ascx" tagname="DefectSavePark" tagprefix="uc11" %>



<%--<%@ Register src="AcceptLinac.ascx" tagname="AcceptLinac" tagprefix="uc12" %>--%>

<%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc4" %>

<%@ Register src="ConfirmPage.ascx" tagname="ConfirmPage" tagprefix="uc1" %>

<%@ Register src="controls/MainFaultDisplayuc.ascx" tagname="MainFaultDisplayuc" tagprefix="uc8" %>

<%@ Register src="ManyFaultGriduc.ascx" tagname="ManyFaultGriduc" tagprefix="uc13" %>

<%@ Register src="controls/ReportFaultPopUpuc.ascx" tagname="ReportFaultPopUpuc" tagprefix="uc14" %>

<%@ Register src="controls/ModalityQAPopUpuc.ascx" tagname="ModalityQAPopUpuc" tagprefix="uc15" %>

<%@ Register src="controls/ReportAFaultuc.ascx" tagname="ReportAFaultuc" tagprefix="uc16" %>

<%@ Register src="controls/ModalityDisplayuc.ascx" tagname="ModalityDisplayuc" tagprefix="uc17" %>

<%--<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />--%>

<%--<%@ Register src="ViewOpenFaults.ascx" tagname="ViewOpenFaults" tagprefix="uc1" %>--%>

<%--<uc9:LockElfuc ID="LockElfuc1" LinacName="" UserReason="1" Tabby="1" visible="false" runat="server" />--%>
<%--<asp:PlaceHolder ID="PlaceHolderAcceptLinac" runat="server"></asp:PlaceHolder>--%>
 <asp:GridView ID="DummyGridView" runat="server">
        </asp:GridView>
<asp:GridView ID="DummyGridViewImaging" runat="server"></asp:GridView>

<div class="grid" >

   <div class="col100 grey" >
      <table id="EnergyTable">
           <%--<tr style="vertical-align:top">
            <td colspan="3">
                <asp:Panel ID="ModalityDisplayPanel" runat="server" Visible="false">
                       <asp:PlaceHolder ID="ModalityPlaceholder" runat="server">
                       </asp:PlaceHolder>
                   </asp:Panel>
            </td>
                </tr>--%>
        <tr style="vertical-align:top;">     
           <td  rowspan="6">
           <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" visible="false" Width="126px" >
            <Columns>
            <asp:BoundField DataField="Energy" HeaderText="Select All Energies" SortExpression="Energy" />
              <asp:TemplateField>  
               <HeaderTemplate>  
                <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="true" OncheckedChanged="checked" />  
               </HeaderTemplate>    
               <ItemTemplate>
                <asp:CheckBox runat="server" ID="RowlevelCheckBox" />
               </ItemTemplate>
             </asp:TemplateField>
            </Columns>
           </asp:GridView>
               
           </td>
           <td style="width: 182px" >
           <asp:GridView ID="GridViewImage" runat="server" AutoGenerateColumns="False" Visible="false" >
               <Columns>
                <asp:BoundField DataField="Energy" HeaderText="Select Imaging" SortExpression="Energy" />
                <asp:TemplateField>                      
                 <ItemTemplate>
                  <asp:CheckBox runat="server" ID="RowlevelCheckBoxImage" />
                </ItemTemplate>
                </asp:TemplateField>
               </Columns>
           </asp:GridView>
              
           </td>
            
        </tr>
        <tr>              
           <td style="width: 182px" >
               <asp:Button ID="engHandoverButton" runat="server" BackColor="#FFCC00" causesvalidation="false" Height="50px" Text="Approve for Clinical Use" />

           </td>               
        </tr>
          <tr>
              <td style="width: 182px" >
              <asp:Button ID="LogOffButton" runat="server" Text="Log Off Without Approving For Clinical Use" CausesValidation="False" />
          </td>
                  </tr>
          <tr><td style="width: 182px" >
              <asp:Button ID="LockElf" runat="server" Text="Lock Elf/Switch User" causesvalidation="false"/>
              </td></tr>
          <tr><td style="width: 182px" >
              <asp:Button ID="PhysicsQA" runat="server" Text="View Physics Energies/Imaging" Visible="false" CausesValidation="false" />
              </td></tr>
          <tr>
              <td style="width: 182px" >
            
          </tr>
              
        <tr>          
           <td colspan="2" style="height: 92px">
               <table>
                 <tr>
                     <td><asp:Literal ID="Literal1" runat="server" Text="Runup Comments"></asp:Literal></td>
                 </tr>
                 <tr>
                     <td><uc3:CommentBoxuc ID="CommentBox" runat="server" />
                         
                     </td>
                 </tr>
               </table>
           </td>                         
        </tr>
          <tr><td colspan="2">
              <br />
              <br />
              <asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>
    <uc16:ReportAFaultuc ID="ReportAFaultuc1" runat="server" />

</ContentTemplate>
</asp:UpdatePanel> 
              </td></tr>
       <tr>
           <td colspan="2">
               <asp:UpdatePanel ID="UpdatePanelQA" runat="server" Visible="true">
<ContentTemplate>
    <asp:PlaceHolder ID="PlaceHolderModalities" runat="server">
    </asp:PlaceHolder>
        </ContentTemplate>
        
</asp:UpdatePanel>

           </td>              
        </tr>
      </table>
   </div>                

   <div class="col300 green" >
            <asp:UpdatePanel ID="UpdatePanel3" runat="server" Visible="true" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:PlaceHolder ID="PlaceHolderFaults" runat="server"></asp:PlaceHolder>
                </ContentTemplate> 
            </asp:UpdatePanel>
       </div>

    

</div>

<div class="clear"></div>
   <%--<asp:Table ID="Table2" runat="server" CellSpacing="20" GridLines="Both" Width="1507px">
         <asp:TableRow ID ="t2r1" runat="server">
         <asp:TableCell ID="lock" runat="server" Width="160px" HorizontalAlign="Left">
         </asp:TableCell>
         <asp:TableCell ID="t2c1" runat="server" Width="160px" HorizontalAlign="Left">

               </asp:TableCell>
               <asp:TableCell ID="t2c2" runat="server" Width="50px">
               
               </asp:TableCell>
               <asp:TableCell ID="TableCell1" runat="server">
                   
                </asp:TableCell>
               <asp:TableCell ID="t2c3" runat="server">
               
               </asp:TableCell>
              
                        </asp:TableRow>
               </asp:Table>--%>

 <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
            <asp:PlaceHolder ID="PlaceHolderWriteData" runat="server">
            <uc4:WriteDatauc ID="WriteDatauc1" LinacName="Linac" UserReason="0"  Tabby="TabNumber"  WriteName="EngData"   Visible="false" runat="server" />
            </asp:PlaceHolder>
            </ContentTemplate>
            </asp:UpdatePanel>
                        
 <div style="background-color: #FFFF66; background-repeat: no-repeat; border-style: solid; border-width: thin">  
   
 <uc5:ViewCommentsuc ID="ViewCommentsuc1"  LinacName="" CommentSort="er" runat="server" />
</div>
 
<asp:PlaceHolder ID="PlaceHolderConfirmPage" runat="server">
        
            <uc1:ConfirmPage ID="ConfirmPage1" Visible="false" runat="server" />
         </asp:PlaceHolder>
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


   




   
