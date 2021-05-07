<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ReportAFaultuc.ascx.vb" Inherits="controls_ReportAFaultuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register src="ReportFaultPopUpuc.ascx" tagname="ReportFaultPopUpuc" tagprefix="uc1" %>

<%@ Register src="CommentBoxuc.ascx" tagname="CommentBoxuc" tagprefix="uc2" %>

<div>
    <asp:Button ID="ReportFaultButton" runat="server" Text="Report a Fault" CausesValidation="false" Height="100px" Width="300px"  />
    <asp:PlaceHolder ID="ReportFaultPopupPlaceHolder" runat="server"></asp:PlaceHolder>
</div>

