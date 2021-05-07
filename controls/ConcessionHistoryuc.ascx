<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ConcessionHistoryuc.ascx.vb" Inherits="controls_ConcessionHistoryuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<asp:GridView ID="ConcessionHistoryGridView" runat="server" AllowPaging="True"
    AutoGenerateColumns="False" CellPadding="4" DataKeyNames="IncidentID" OnPageIndexChanging="ConcessionHistoryGridView_PageIndexChanging"
    EnableViewState="False" ForeColor="#333333" GridLines="None">
    <RowStyle CssClass="grows" />
    <%-- BackColor="#F7F6F3" ForeColor="#333333" />--%>
    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    <Columns>
        <asp:BoundField DataField="TrackingID" HeaderText="TrackingID"
            InsertVisible="False" ReadOnly="True" SortExpression="TrackingID" Visible="false" />
        <asp:BoundField DataField="IncidentID" HeaderText="IncidentID"
            InsertVisible="False" ReadOnly="True" SortExpression="IncidentID" Visible="false" />
        <asp:BoundField DataField="ConcessionNumber" HeaderText="Concession Number"
            SortExpression="ConcessionNumber" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField DataField="LastupdatedOn" HeaderText="LastupdatedOn"
            SortExpression="LastupdatedOn" />
        <asp:BoundField DataField="Status" HeaderText="Status"
            SortExpression="Status" />
        <asp:BoundField DataField="Action" HeaderText="Concession Action"
            SortExpression="Action" />
        <asp:BoundField DataField="TrackingComment" HeaderText="Comment"
            SortExpression="Comment" />
        <asp:BoundField DataField="AssignedTo" HeaderText="AssignedTo"
            SortExpression="AssignedTo" />
        <asp:BoundField DataField="LastupdatedBy" HeaderText="LastupdatedBy"
            SortExpression="LastupdatedBy" />
    </Columns>
    <EditRowStyle BackColor="#99FF33" />
    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
</asp:GridView>
