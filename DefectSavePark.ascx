<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DefectSavePark.ascx.vb" Inherits="DefectSavePark" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register src="controls/CommentBoxuc.ascx" tagname="CommentBoxuc" tagprefix="uc2" %>
<%--No need now for WriteDatauc Analysis 23/11/16 --%><%--Added back in 26/03/18 --%>
<%@ Register Src="WriteDatauc.ascx" TagName="WriteDatauc" TagPrefix="uc1" %>


<asp:HiddenField ID="SelectedIncidentID" Value="" runat="server" />
<asp:HiddenField ID="TimeFaultSelected" Value="" runat="server" />
<asp:HiddenField ID="AreaOrAccuray" Value="" runat="server" />
<%-- NO requirement 23/11/16 --%><%-- Added back in 26/03/18 --%>
<uc1:WriteDatauc ID="WriteDatauc1" LinacName="" UserReason="12" Tabby="Defect" WriteName="Defect" Visible="false" runat="server" />

<div style="width: 500px">
    <fieldset style="width:auto">
        <legend>Report Fault</legend>
        <table style="width: 401px;">
            <tr style="vertical-align:top">
                <td><asp:Label ID="FaultSelection" runat="server" Text="Select Fault: "></asp:Label></td>
                <td colspan="3"><asp:UpdatePanel ID="UpdatePanelDefectlist" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="Defect" runat="server" AutoPostBack="true"
                                    AppendDataBoundItems="True" DataValueField="IncidentID" DateTextField="Fault">
                                    <asp:ListItem>Select</asp:ListItem>
                                    </asp:DropDownList>
                                </ContentTemplate>
                    </asp:UpdatePanel>
                 </td>
            </tr>
            <tr>
                <td><asp:Label ID="ErrorCodeLabel" runat="server" Text="Error Code: "></asp:Label></td>
                <td colspan="3"><asp:TextBox ID="ErrorCode" runat="server" Text="" ReadOnly="false" EnableViewState="False" Visible="true"></asp:TextBox></td>
            </tr>
            <tr>
                <td><asp:Label ID="AccurayLabel" runat="server" Text="Physicist/Accuracy Job Number: "></asp:Label></td>
                <td colspan="3"><asp:TextBox ID="Accuray" runat="server" EnableViewState="false"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="AccurayValidation" runat="server" ControlToValidate="Accuray" forecolor="Red" ErrorMessage="Please enter a physicist name or Accuray job number" Display="None" validationgroup="Tomodefect" Enabled="false"></asp:RequiredFieldValidator></td>
                
            </tr>
            <tr>
                <td><asp:Label ID="FaultDescriptionLabel" runat="server" Text="Fault Description: "></asp:Label></td>
                <td colspan="3"> <asp:Panel ID="FaultPanel" runat="server" Enabled ="false">
                            <uc2:CommentBoxuc ID="FaultDescription" runat="server" />
                        </asp:Panel></td>
            </tr>
            <tr><td><asp:Label ID="RadIncidentLabel" runat="server" Text="Radiation Incident?: "></asp:Label></td>
                <td> <asp:RadioButtonList ID="RadioIncident" runat="server" AutoPostBack="false" enabled="true">
                                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                        <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                                    </asp:RadioButtonList>

                </td>
               <td><asp:Label ID="FaultClosedLabel" runat="server" Text="Fault Closed?" Visible="false"></asp:Label></td>
                <td>
                                    
                                    <asp:RadioButtonList ID="FaultOpenClosed" runat="server" AutoPostBack="True" Visible="false">
                                        <asp:ListItem Text="No" Value="-1"></asp:ListItem>
                                        <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                                    </asp:RadioButtonList>

                </td>
                
            </tr>
            <tr><td colspan="2"><asp:RequiredFieldValidator ID="RadioIncidentValidation" runat="server" ControlToValidate="RadioIncident" ErrorMessage="Please complete Radiation Incident Selection"
                            Display="None" validationgroup="Tomodefect" ></asp:RequiredFieldValidator></td></tr>
            <tr>
                <td><asp:Label ID="CorrectiveActionLabel" runat="server" Text="Corrective Action"></asp:Label></td>
                <td colspan="3"> <asp:Panel ID="ActPanel" Enabled="false" runat="server">
                            <uc2:CommentBoxuc ID="RadActC" runat="server" />
                        </asp:Panel></td>
            </tr>
            <tr><td><asp:Label ID="PatientIDLabel" runat="server" Text="Patient ID: "></asp:Label></td>
                <td colspan="3"><asp:TextBox ID="PatientIDBox" Text="" runat="server"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionPatient" runat="server" forecolor="Red" ControlToValidate="PatientIDBox" ValidationExpression="^\d{7}$" Display="None"  ErrorMessage="Please enter a BSUH ID" validationgroup="Tomodefect"></asp:RegularExpressionValidator> </td>
            
            </tr>
            
            <tr><td><asp:Button ID="SaveDefectButton" runat="server" Text="Save"  CausesValidation="False" Visible="false"/></td>
                <td><asp:Button ID="UnRecoverableSave" runat="server" CausesValidation="False"  Text="Save" Visible="false" /></td>
                <td><asp:Button ID="ClearButton" runat="server" Text="Close Without Saving" CausesValidation="False" CssClass="buttonmargin" /></td>
                <td></td>
            </tr>
            <tr><td colspan="3"><asp:ValidationSummary ID="ValidationSummary1" ForeColor="Red" HeaderText="You must enter a value in the following fields:" ValidationGroup="Tomodefect" ShowMessageBox="True" ShowSummary="True"  runat="server" /></td>
                
            </tr>
        </table>
    </fieldset>
</div>







