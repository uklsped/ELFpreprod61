<%@ Page Title="" Language="VB" MasterPageFile="~/Elf.master" AutoEventWireup="false" CodeFile="faultPage.aspx.vb" Inherits="faultPage" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
 

<%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script type="text/javascript">
        //This function prevents return key usage from http://www.felgall.com/jstip43.htm
        //        function kH(e) {
        //           var pK = e ? e.which : window.event.keyCode;
        //            return pK != 13;
        //        }
        //        document.onkeypress = kH;
        //        if (document.layers) document.captureEvents(Event.KEYPRESS);
</script>

    <asp:UpdatePanel ID="faultupdate" runat="server"  Visible="true">
    <ContentTemplate >
   <%--removed refererence to LinacName = LA1 although this is reset in the codebehind--%>
    <uc1:writedatauc ID="WriteDataucfr" LinacName="" UserReason="103" Tabby="Report"  WriteName="Record" visible="false" runat="server" />
<%--changed all validation displays to static to stop having to press submit button twice. Changed submit button to causes validation Added validation group http://stackoverflow.com/questions/7086471/requiredfieldvalidator-requires-user-to-click-twice--%>

    
    <br />
    Fault Details For 
    <asp:Label ID="MachineLabel" runat="server" Text=""></asp:Label>
    <br />
    <span class = "redcolor"> * Mandatory Field </span>
    <br />
    <table style="width:300px;">
        <tr>
        <td  >
           Area: <span class = "redcolor"> * </span> </td>
        <td>
            <asp:DropDownList ID="DropDownListArea" runat="server">
                <asp:ListItem>Select</asp:ListItem>
                <asp:ListItem>Machine</asp:ListItem>
                <asp:ListItem>iView</asp:ListItem>
                <asp:ListItem>XVI</asp:ListItem>
                <asp:ListItem>IT</asp:ListItem>
                <asp:ListItem>Other</asp:ListItem>
            </asp:DropDownList>
             <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
            ControlToValidate="DropDownListArea" SetFocusOnError="True"
            InitialValue="Select" ErrorMessage="Please select Area" ValidationGroup="ValGroup1" >
           </asp:RequiredFieldValidator>
        </td>
        </tr>
        <tr>
            <td class="style1">
                Energy:</td>
            <td>
    <%--<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
            </td>--%>
            <asp:DropDownList ID="DropDownListEnergy" runat="server">
    </asp:DropDownList></td>
    
        </tr>
        <tr>
            <td class="style1">
                Gantry Angle:</td>
            <td>
    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    <asp:CompareValidator ID="GantryAngleCheck"
        runat="server" ErrorMessage="Please enter angle as integer" 
                    ControlToValidate="TextBox2" Operator="DataTypeCheck" SetFocusOnError="True" 
                    Type="Integer" Display="Static"></asp:CompareValidator>
                <asp:RangeValidator ID="GantryRangeCheck" runat="server" 
                    ErrorMessage="Range is 0 to 360 degrees" Type="Integer" SetFocusOnError="True" 
                    MaximumValue="360" MinimumValue="0" ControlToValidate="TextBox2" ValidationGroup="ValGroup1"
                    Display="Dynamic"></asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <td class="style1">
                Collimator Angle:</td>
                
            <td>
    <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
     <asp:CompareValidator ID="CollimatorAngleCheck"
        runat="server" ErrorMessage="Please enter angle as integer" 
                ControlToValidate="TextBox3" Operator="DataTypeCheck" SetFocusOnError="True" 
                Type="Integer" Display="Static"></asp:CompareValidator>
                <asp:RangeValidator ID="CollimatorRangeCheck" runat="server" 
                ErrorMessage="Range is 0 to 360 degrees" Type="Integer" SetFocusOnError="True" 
                MaximumValue="360" MinimumValue="0" ControlToValidate="TextBox3" ValidationGroup="ValGroup1"
                Display="Dynamic"></asp:RangeValidator>
            </td>
            </tr>
            <tr>
        <td class="style1">
                Fault Description:</td>
            <td>
              <asp:TextBox ID="TextBox4" runat="server" MaxLength="250" TextMode="MultiLine"></asp:TextBox>
              </td> 
        </tr>
        <tr>
            <td class="style1">
                Patient ID:</td>
            <td>
                <asp:TextBox ID="PatientIDTextBox" Text="" runat="server"></asp:TextBox>
                
                <asp:RegularExpressionValidator ID="RegularExpressionPatient" ValidationGroup="ValGroup1" runat="server" ControlToValidate="PatientIDTextBox" validationexpression="^\d{7}$" Display="Static" ErrorMessage="Please enter a 7 digit BSUH ID" SetFocusOnError="True"></asp:RegularExpressionValidator>
                
            </td>
        </tr>
    </table>
    <br />
    
    <br />
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <asp:HiddenField ID="DummyConcessionNumber"  runat="server" Value="" />
        <asp:HiddenField ID="DummyConcessionDescription" runat="server" Value="" />
        <asp:HiddenField ID="DummyCommentBox" runat="server" Value="" />
        <asp:GridView ID="DummyGridView" runat="server">
        </asp:GridView>
    <br />
    <table style="width:300px;">
        <tr>
            <td class="style1">
        <asp:Button ID="confirmfault" runat="server" Text="Confirm Fault" ValidationGroup="ValGroup1" causesvalidation="true"/>
        </td>
        <td><asp:Button ID="Cancel" runat="server" Text="Cancel" CausesValidation="false" />
        </td>
        </tr>
        </table>
    <br />
    
    <br />
    <br />
    <br />
    <br />
    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
        BackColor="LightGoldenrodYellow" BorderColor="Tan" BorderWidth="1px" 
        CellPadding="2" DataKeyNames="FaultId" 
        EnableViewState="False" ForeColor="Black" GridLines="None" 
    HorizontalAlign="Left" Width="446px">
        <Columns>
            <asp:BoundField DataField="FaultId" HeaderText="FaultId" InsertVisible="False" 
                ReadOnly="True" SortExpression="FaultId" />
            <asp:BoundField DataField="Description" HeaderText="Description" 
                SortExpression="Description" />
            <asp:BoundField DataField="ReportedBy" HeaderText="ReportedBy" 
                SortExpression="ReportedBy" />
            <asp:BoundField DataField="DateReported" HeaderText="DateReported" 
                SortExpression="DateReported" />
            <asp:BoundField DataField="FaultStatus" HeaderText="FaultStatus" 
                SortExpression="FaultStatus" />
            <asp:BoundField DataField="Energy" HeaderText="Energy" 
                SortExpression="Energy" />
            <asp:BoundField DataField="GantryAngle" HeaderText="Gantry Angle" 
                SortExpression="GantryAngle" />
            <asp:BoundField DataField="CollimatorAngle" HeaderText="Collimator Angle" 
                SortExpression="CollimatorAngle" />
                <asp:BoundField DataField="Linac" HeaderText="Linac" 
                SortExpression="Linac" />
        </Columns>
        <FooterStyle BackColor="Tan" />
        <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" 
            HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
        <HeaderStyle BackColor="Tan" Font-Bold="True" />
        <AlternatingRowStyle BackColor="PaleGoldenrod" />
    </asp:GridView>

    <%--<asp:ModalPopupExtender ID="FaultButton_ModalPopupExtender" 
        runat="server" Enabled="True" 
        TargetControlID="confirmfault"
        popupcontrolId="Panel1"
        popupdraghandlecontrolid="PopupHeader"
        drag="true" BackgroundCssClass="modalBackground" DropShadow="true" 
        OnCancelScript="ClearUI();" 
        >
    </asp:ModalPopupExtender>--%>
         <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">

<ContentTemplate>--%>
        <%--This is the log in popup panel  --%>
            <%--<asp:Panel ID="Panel1" runat="server" cssclass="modalPopup" style="display:none" >
                <div>
                    <table>
                        <tr>
                            <td class="style1">
                                Username:
                                <asp:TextBox ID="txtUserName" runat="server" style="margin-left: 0px" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="txtUserNamervf" runat="server" 
                                    ControlToValidate="txtUserName" Display="dynamic" 
                                    ErrorMessage="Please enter your username" SetFocusOnError="True"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="style1">
                                Password:
                                <asp:TextBox ID="txtPWD" runat="server" TextMode="Password" />
                                <asp:RequiredFieldValidator ID="txtPWDrvf" runat="server" 
                                    ControlToValidate="txtPWD" Display="Dynamic" 
                                    ErrorMessage="Please enter your password" SetFocusOnError="True"></asp:RequiredFieldValidator>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="style1">
                                <asp:Button ID="FaultConfirmed" runat="server" Text="Confirm Fault" causesvalidation="false" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;  
                                <asp:Button ID="btnchkcancel" runat="server" OnClientClick="return DoClose();" 
                                    Text="Cancel" />
                            </td>
                        </tr>
                    </table>
                </div>
</asp:Panel>--%>
<%--</ContentTemplate>
</asp:UpdatePanel>
    --%>
    <%--<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
        ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
        
        
        SelectCommand="SELECT FaultId, Description,ReportedBy,DateReported,FaultStatus,Energy,GantryAngle,CollimatorAngle, Linac  FROM ReportFault WHERE (FaultID = (SELECT Max(FaultID) AS lastrecord FROM ReportFault AS Fault_1 where Linac=machinename))">
    </asp:SqlDataSource>--%>
    <br />
    <%--<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
        DataKeyNames="TrackingID" DataSourceID="SqlDataSourcet" CellPadding="4" 
        ForeColor="#333333" GridLines="None">
        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
        <Columns>
            <asp:BoundField DataField="TrackingID" HeaderText="TrackingID" 
                InsertVisible="False" ReadOnly="True" SortExpression="TrackingID" />
            <asp:BoundField DataField="TrackingComment" HeaderText="TrackingComment" 
                SortExpression="TrackingComment" />
            <asp:BoundField DataField="AssignedTo" HeaderText="AssignedTo" 
                SortExpression="AssignedTo" />
            <asp:BoundField DataField="Status" HeaderText="Status" 
                SortExpression="Status" />
            <asp:BoundField DataField="LastupdatedBy" HeaderText="LastupdatedBy" 
                SortExpression="LastupdatedBy" />
            <asp:BoundField DataField="LastupdatedOn" HeaderText="LastupdatedOn" 
                SortExpression="LastupdatedOn" />
            <asp:BoundField DataField="FaultID" HeaderText="FaultID" 
                SortExpression="FaultID" />
        </Columns>
        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#999999" />
        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    </asp:GridView>--%>
    <asp:SqlDataSource ID="SqlDataSourcet" runat="server" 
        ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
        SelectCommand="SELECT * FROM [FaultTracking]"></asp:SqlDataSource>
    <br />
  
  </ContentTemplate></asp:UpdatePanel>  
  
</asp:Content>

