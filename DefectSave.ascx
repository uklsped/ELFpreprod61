<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DefectSave.ascx.vb" Inherits="DefectSave" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register src="controls/CommentBoxuc.ascx" tagname="CommentBoxuc" tagprefix="uc2" %>

<%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc1" %>

<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<asp:PlaceHolder ID="PlaceHolderwrite" runat="server">
<uc1:WriteDatauc ID="WriteDatauc1" LinacName="" UserReason="11" Tabby="Defect" WriteName="Defect" Visible="false" runat ="server" />
<uc1:WriteDatauc ID="WriteDatauc2" LinacName="" UserReason="103" Tabby="Major" WriteName="Major"  Visible="false" runat="server" />
</asp:PlaceHolder>

<asp:HiddenField ID="SelectedIncidentID" Value="" runat="server" />

<asp:HiddenField ID="TimeFaultSelected" Value="" runat="server" />
<asp:HiddenField ID="AreaOrAccuray" Value="" runat="server" />

<div style="width:500px" >
   <fieldset style="width:auto">
       <legend>Report Fault</legend>
<%--Record Repeat Fault <br />
<span class="redcolor">* Mandatory Field</span> <br />--%>
     
    <table style="width: 401px;">
        <tr style="vertical-align:top;">
            <td>
                <asp:Label ID="FaultSelection" runat="server" Text="Select Fault"></asp:Label></td>
            <td colspan="3"><asp:UpdatePanel ID="UpdatePanelDefectList" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:DropDownList ID="Defect" runat="server" AutoPostBack="true" AppendDataBoundItems="true" DataValueField="IncidentID">
                        <asp:ListItem>Select</asp:ListItem>
                    </asp:DropDownList>
                </ContentTemplate></asp:UpdatePanel>
            </td>
            
        </tr>
        <tr>
            <td>Area: </td>
            <td>
                <asp:DropDownList ID="DropDownListArea" runat="server" Enabled="false" EnableViewState="false">
                    <asp:ListItem Text="Select" Value="Select"></asp:ListItem>
                    <asp:ListItem Text="Machine" Value="Machine"></asp:ListItem>
                    <asp:ListItem Text="iView" Value="iView"></asp:ListItem>
                    <asp:ListItem Text="XVI" Value="XVI"></asp:ListItem>
                    <asp:ListItem Text="IT" Value="IT"></asp:ListItem>
                    <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="AreaValidation" ControlToValidate="DropDownListArea" runat="server" InitialValue="Select" ErrorMessage="Please Select Area" Display="None"></asp:RequiredFieldValidator>
            </td>
        <%--</tr>--%>
        <%--<tr>--%>
            <td>Gantry Angle: </td>
            <td>
                <asp:TextBox ID="GantryAngleBox" runat="server" Width="30px"></asp:TextBox>
                <asp:CompareValidator ID="GantryAngleCheck" runat="server" ErrorMessage="Please enter Gantry Angle as an Integer" ControlToValidate="GantryAngleBox" Operator="DataTypeCheck" SetFocusOnError="true" Type="Integer" ValidationGroup="defect" Display="None"></asp:CompareValidator>
                <asp:RangeValidator ID="GantryRangeCheck" runat="server" ErrorMessage="Gantry Angle Range is 0 to 360 degrees" Type="Integer" SetFocusOnError="true" MaximumValue="360" MinimumValue="0" ControlToValidate="GantryAngleBox" ValidationGroup="defect" Display="None"></asp:RangeValidator>
            </td>
            
        </tr>
        <tr>
            <td>Energy: </td>
            <td><asp:DropDownList ID="DropDownListEnergy" runat="server"></asp:DropDownList></td>
        <%--</tr>--%>
        <%--<tr>--%>
            <td>Collimator Angle: </td>
            <td colspan="3">
                <asp:TextBox ID="CollimatorAngleBox" runat="server" Width="30px"></asp:TextBox>
                <asp:CompareValidator ID="CollimatorAngleCheck" runat="server" ErrorMessage="Please enter Collimator Angle as an Integer" ControlToValidate="CollimatorAngleBox" Operator="DataTypeCheck" SetFocusOnError="true" Type="Integer" ValidationGroup="defect" Display="None"></asp:CompareValidator>
                <asp:RangeValidator ID="CollimatorRangeCheck" runat="server" ErrorMessage="Collimator Angle Range is 0 to 360 degrees" Type="Integer" SetFocusOnError="true" MaximumValue="360" MinimumValue="0" ControlToValidate="CollimatorAngleBox" ValidationGroup="defect" Display="None"></asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <td>Fault Description: </td>
            <td colspan="3">
                <asp:Panel ID="FaultPanel" runat="server" Enabled="false">
                <uc2:CommentBoxuc runat="server" ID="FaultDescription" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>Corrective Action: </td>
            <td colspan="3">
                <asp:Panel ID="ActPanel" runat="server">
                <uc2:CommentBoxuc runat="server" ID="RadActC" />
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>Patient ID: </td>
            <td>
                <asp:TextBox ID="PatientIDBox" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RegularExpressionPatient" runat="server" ControlToValidate="PatientIDBox" ValidationExpression="^\d{7}$" Display="None" ValidationGroup="defect" ErrorMessage="Please enter a BSUH ID in the correct format" SetFocusOnError="true"></asp:RegularExpressionValidator>
            </td>
            <td colspan="2"></td>
        </tr>
        <tr>
            <td><asp:Label ID="RadIncidentlabel" runat="server" Text="Radiation Incident?: "></asp:Label></td>
            <td>
                <asp:RadioButtonList ID="RadioIncident" runat="server" AutoPostBack="false">
                <asp:ListItem Text="No" Value="False"></asp:ListItem>
                <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator ID="RadioIncidentValidation" runat="server" ControlToValidate="RadioIncident" ErrorMessage="Please complete Radiation Incident Selection" Display="None" ValidationGroup="defect"></asp:RequiredFieldValidator>
            </td>
        <%--</tr>--%>
        <%--<tr>--%>
            <td><asp:Button ID="SaveDefectButton" runat="server" Text="Save" CausesValidation="false" Enabled="false" /> </td>
            <td><asp:Button ID="ClearButton" runat="server" Text="Close without saving" CausesValidation="false"/></td>
        </tr>
        <tr><td colspan="3"><asp:ValidationSummary ID="ValidationSummarydefect" Forecolor="Red" HeaderText="Please correct the errors in the following fields:" ValidationGroup="defect" ShowMessageBox="True" ShowSummary="True" EnableClientScript="false" runat="server" /></td>
                
            </tr>
    </table>
   </fieldset>

       
</div>


