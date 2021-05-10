<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Preclinusercontrol.ascx.vb" Inherits="Preclinusercontrol" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>  
<%--<%@ Register src="ViewOpenFaults.ascx" tagname="ViewOpenFaults" tagprefix="uc1" %>--%>
<%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc2" %>
<%@ Register src="ConfirmPage.ascx" tagname="ConfirmPage" tagprefix="uc3" %>
<%@ Register src="ViewCommentsuc.ascx" tagname="ViewCommentsuc" tagprefix="uc5" %>
<%--<%@ Register src="DefectSave.ascx" tagname="DefectSave" tagprefix="uc6" %>
<%@ Register src="TodayClosedFault.ascx" tagname="TodayClosedFault" tagprefix="uc7" %>--%>
<%@ Register src="AcceptLinac.ascx" tagname="AcceptLinac" tagprefix="uc8" %>
<%@ Register src="controls/CommentBoxuc.ascx" tagname="CommentBoxuc" tagprefix="uc4" %>
<%@ Register Src="~/controls/CommentBoxuc.ascx" TagPrefix="uc1" TagName="CommentBoxuc" %>

   <%-- <%@ Register src="ManyFaultGriduc.ascx" tagname="ManyFaultGriduc" tagprefix="uc10" %>--%>

  <%@ Register src="controls/MainFaultDisplayuc.ascx" tagname="MainFaultDisplayuc" tagprefix="uc11" %>

    <%@ Register src="controls/ReportFaultPopUpuc.ascx" tagname="ReportFaultPopUpuc" tagprefix="uc9" %>

    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
            DataKeyNames="HandoverId"
        BackColor="White" BorderColor="#CC9966" BorderStyle="Solid" BorderWidth="1px" 
        CellPadding="4"  EditRowStyle-BorderStyle="NotSet" Width="1867px">
            <RowStyle BackColor="White" ForeColor="#330099" />
            <Columns>
            <asp:BoundField DataField="HandoverId" HeaderText="HandoverId" InsertVisible="False" ReadOnly="True" SortExpression="HandoverId" />
                   <asp:TemplateField HeaderText="6 MV"  >
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MV6"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="6 MV FFF"  >
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MV6FFF"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="10 MV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MV10"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="10 MV FFF">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MV10FFF"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="4 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV4"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="6 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV6"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="8 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV8"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="10 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV10"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="12 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV12"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="15 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV15"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="18 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV18"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                   <asp:TemplateField HeaderText="20 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV20"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
             <asp:BoundField DataField="LogOutName" HeaderText="Approved By" SortExpression="LogOutName" />
             <asp:BoundField DataField="LogOutDate" HeaderText="Date Approved" SortExpression="LogOutDate" />
             <asp:BoundField DataField="Comment" HeaderText=" Engineering Comment" SortExpression="CComment" />
             <asp:BoundField DataField="linac" HeaderText="linac" SortExpression="linac" />                  
             </Columns>
             <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
             <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
             <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
             <HeaderStyle BackColor="#330000" Font-Bold="True" ForeColor="#FFFFCC" />
        </asp:GridView>

    <div class="clear" style="width: 1863px"></div>
<div class="grid">
    <div class="col100 green">
        <table id="HandoverTable">
            <tr style="vertical-align:top">
                <td style="width:300px" rowspan="2"> 
                 <asp:GridView ID="GridViewImage" runat="server" AutoGenerateColumns="False" >
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
                <td style="width:145px">
                    <asp:Button ID="clinHandoverButton" Height="50px"  runat="server" 
                Text="Approve For Clinical Use" CausesValidation="false" BackColor="#FFCC00" />
                </td>
                </tr>
            <tr><td>
                <asp:Button ID="LogOff" runat="server" Text="Log Off Without Approving For Clinical Use" CausesValidation="False" />
                </td></tr>
           <tr>
                <td colspan="3" style="height:92px"><fieldset>
                    <legend style="font-family: Arial, Helvetica, sans-serif; font-weight: bold">
                        Pre-Clinical Comments
                   </legend>
            
                  <uc4:CommentBoxuc ID="CommentBox" runat="server" />
                   </fieldset></td>
                            </tr>

        </table>
</div>
    <div class="col200 blue">
        <asp:UpdatePanel ID="UpdatePanel8" runat="server">
            <ContentTemplate>
                <asp:Button ID="ReportFaultButton" runat="server" Text="Report Fault" CausesValidation="false"/>
                            <asp:PlaceHolder ID="ReportFaultPopupPlaceHolder" runat="server"></asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="col300 green">
                         <asp:PlaceHolder ID="PlaceHolderFaults" runat="server"></asp:PlaceHolder>
     
    </div>
</div>
<div class="clear">
    
    
</div>
<asp:PlaceHolder ID="PlaceHolder2" runat="server">
  <uc2:WriteDatauc ID="WriteDatauc1" runat="server" LinacName="" Tabby="2" UserReason="2" visible="false" WriteName="PreClinData" />
  <uc3:ConfirmPage ID="ConfirmPage1" runat="server" Visible="false" />
</asp:PlaceHolder>

<uc5:ViewCommentsuc ID="ViewCommentsuc1" linacName="" CommentSort="pcr" runat="server" />
