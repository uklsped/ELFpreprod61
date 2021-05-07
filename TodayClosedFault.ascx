<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TodayClosedFault.ascx.vb" Inherits="TodayClosedFault" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%--<%@ Register src="controls/FaultTrackinguc.ascx" tagname="FaultTrackinguc" tagprefix="uc1" %>--%>

<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />

<script type="text/javascript">

    </script>

    <asp:HiddenField ID="HiddenField1" runat="server"/>
    <fieldset style="width:100%">
        <legend>
        <asp:Label ID="FaultClearedTodayLabel" runat="server" Text="Concessions Closed Today"></asp:Label></legend>
   <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Always" ><ContentTemplate>
       
    <asp:Table ID="Table1" runat="server" HorizontalAlign="Left" Width="100%" >
       <asp:TableRow HorizontalAlign="Left" VerticalAlign="Top">
       <asp:TableCell HorizontalAlign="Left" VerticalAlign="Top">   
       <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" 
                        DataKeyNames="incidentID" ForeColor="#333333" GridLines="None" AllowPaging="True"  PageSize="4">                

<RowStyle cssclass="grows" />
<%--    BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Left" />--%>
                                           
<Columns>
                            
<asp:BoundField DataField="incidentID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="incidentID" />
                                                               
<asp:BoundField DataField="Description" HeaderText="Original Fault Description" SortExpression="Description" />
                            
<asp:BoundField DataField="ConcessionNumber" HeaderText="Concession Number" SortExpression="ConcessionNumber" ItemStyle-HorizontalAlign="Left" >
                            
<%--<ItemStyle HorizontalAlign="Left" />--%>
                            
</asp:BoundField>
                            
<asp:BoundField DataField="ConcessionDescription" HeaderText="Concession Description" SortExpression="Description" />
                            
                        
</Columns>
                        

<FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />                       

<PagerSettings Mode="NumericFirstLast" />
                        

<PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        

<SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        

<HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Size="Small" />
                        

<EditRowStyle BackColor="#999999" />
                        
<AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    
</asp:GridView>

       
</asp:TableCell>
    
</asp:TableRow>
    </asp:Table>

</ContentTemplate>
    </asp:UpdatePanel>
        </fieldset>
