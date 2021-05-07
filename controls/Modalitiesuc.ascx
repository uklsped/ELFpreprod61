<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Modalitiesuc.ascx.vb" Inherits="Modalitiesuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register src="../WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc1" %>

<%--<%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc1" %>--%>

<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />
      
        <uc1:writedatauc ID="WriteDatauc1" LinacName="" UserReason="6"  
    Tabby="QA"  WriteName="PhysicsQA" visible="false" runat="server" />
        <h3>Table of Approved Energy and Imaging Modalities</h3>
<table >
    <tr><td colspan="4">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" OnRowEditing="GridView1_RowEditing"
            OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowUpdating="Gridview1_RowUpdating" onrowdatabound= "GridView1_RowDataBound"
            CellPadding="4" DataKeyNames="EnergyID" enableeventvalidation="false"
            ForeColor="#333333" GridLines="None">
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
            <%--From https://forums.asp.net/t/1255553.aspx?Dynamically+show+Edit+and+Delete+buttons+in+a+CommandField+in+a+GridView--%>
            <asp:TemplateField ShowHeader="False">
                    <EditItemTemplate>
                        <asp:LinkButton ID="LinkButton3" runat="server" CausesValidation="false" CommandName="Update"
                            Text="Update"></asp:LinkButton>
                        <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel"
                            Text="Cancel"></asp:LinkButton>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Visible="false"
                            Text="Edit"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="EnergyID" HeaderText="Modality ID" 
                    SortExpression="EnergyID" InsertVisible="False" ReadOnly="True" Visible="false"/>
                <asp:BoundField DataField="Energy" HeaderText="Modality" InsertVisible="False" ReadOnly="True"
                    SortExpression="Energy" />
                <asp:CheckBoxField DataField="Approved" HeaderText="Approved" 
                    SortExpression="Approved" />
                <asp:BoundField DataField="ApprovedBy" HeaderText="ApprovedBy" 
                    SortExpression="ApprovedBy" InsertVisible="True" ReadOnly="True" />
                <asp:BoundField DataField="DateApproved" HeaderText="DateApproved" InsertVisible="False" ReadOnly="True" 
                    SortExpression="DateApproved" />
                <asp:BoundField DataField="Comment" HeaderText="Comment" 
                    SortExpression="Comment" />
                <asp:BoundField DataField="Linac" HeaderText="Linac" SortExpression="Linac" InsertVisible="False" ReadOnly="True"/>
            </Columns>
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
</td></tr>
    <tr><td>

           <asp:Button ID="Close" runat="server" Text="Close" CausesValidation="false"/>
        </td>
        
    </tr>

    </table>
    

    