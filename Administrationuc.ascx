<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Administrationuc.ascx.vb" Inherits="Administrationuc" %>

<%--<%@ Register src="CreateUseruc.ascx" tagname="CreateUseruc" tagprefix="uc1" %>--%>

<%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc2" %>
<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />
<%--<link rel="stylesheet" type="text/css" href="css\twocol.css" />--%>
    <script type="text/javascript"> 
//This function prevents return key usage from http://www.felgall.com/jstip43.htm
        function kH(e) {
           var pK = e ? e.which : window.event.keyCode;
            return pK != 13;
        }
        document.onkeypress = kH;
        if (document.layers) document.captureEvents(Event.KEYPRESS);
</script>
    <div>
   <%-- changed 23rd feb--%>
    <%--<asp:Button ID="Button1" runat="server" Text="Close" onclientclick="window.close();" xmlns:asp="#unknown" causesvalidation="false"/>--%>
    
</div>

<div>
<asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
             <asp:PlaceHolder ID="PlaceHolder2" runat="server">
            <uc2:WriteDatauc ID="WriteDatauc1" LinacName="" UserReason="100"  Tabby="Admin"  WriteName="AdminData"   Visible="False" runat="server" />
            </asp:PlaceHolder>
            </ContentTemplate>
            </asp:UpdatePanel>
</div>
<asp:UpdatePanel ID ="Display" runat="server">
    <ContentTemplate>
     </ContentTemplate>
        </asp:UpdatePanel>
        <asp:MultiView ID="MultiView1" runat="server">
       
            <asp:View ID="ShowNowt" runat="server">
            </asp:View>
            <asp:View ID="LoggedIn" runat="server">
            <div id="container" style="background-color: #66FFFF; float: none; padding: 0px; margin: 0px auto 0px auto; overflow: hidden;">
            <div id="leftcoll" style="float: left; width: auto;">
<fieldset >
            <legend>User Details</legend>
                <asp:Panel ID="panelContainer" runat="server" Height="300px" Width="100%"  ScrollBars="Vertical">
                                <asp:GridView ID="UserGrid" runat="server" AutoGenerateColumns="False" 
                                DataKeyNames="UserName" cssclass="Grid">
       <Columns>
        <asp:CommandField ShowEditButton="True" />
        
        <asp:BoundField DataField="FirstName" HeaderText="First Name" ReadOnly="True" />
        <asp:BoundField DataField="LastName" HeaderText="Last Name" ReadOnly="True" />
        <asp:TemplateField HeaderText="Email">
        <ItemTemplate>
            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Email")%>'></asp:Label>
        </ItemTemplate>
        <EditItemTemplate>
        <asp:TextBox runat="server" ID="Email" Text='<%# Bind("Email")%>'></asp:TextBox>
        </EditItemTemplate></asp:TemplateField>
        <asp:BoundField DataField="UserName" HeaderText="User Name" ReadOnly="True" />
        <asp:BoundField DataField="RoleName" HeaderText="User Type" SortExpression="Role"  />
        <asp:TemplateField HeaderText="Approved">
            <EditItemTemplate>
                <asp:CheckBox ID="CheckBox1" runat="server" 
                    Checked='<%# Bind("IsApproved") %>' />
            </EditItemTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("IsApproved") %>' 
                    Enabled="true" AutoPostBack="True" />
            </ItemTemplate>
        </asp:TemplateField>  
    </Columns>
</asp:GridView>
</asp:Panel>
</fieldset>
    </div>
     <div id="centrecoll"style="float: left; width: auto;">

<fieldset>
            <legend>Role Administration</legend>
  <fieldset >
            <legend>Create Role</legend>          
<b>Create a New Role:</b>
<asp:TextBox ID="RoleName" runat="server" EnableViewState="false" ViewStateMode="Disabled" ></asp:TextBox>
<br />
<asp:Button ID="CreateRoleButton" runat="server" Text="Create Role" />

    </fieldset>
<p>
    &nbsp;</p>
    <fieldset >
            <legend>Archive Role</legend>
            <asp:GridView ID="RoleList" runat="server" AutoGenerateColumns="False" 
                                DataKeyNames="RoleName" cssclass="Grid">
       <Columns>
        <asp:CommandField ShowEditButton="True" />
        
        
        <asp:BoundField DataField="RoleName" HeaderText="Role" SortExpression="Role"  />
        <asp:TemplateField HeaderText="Archived">
            <EditItemTemplate>
                <asp:CheckBox ID="CheckBox1" runat="server" 
                    Checked='<%# Bind("Description") %>' />
            </EditItemTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Bind("Description") %>' 
                    Enabled="false" AutoPostBack="false" />
            </ItemTemplate>
        </asp:TemplateField>  
    </Columns>
</asp:GridView>

    <%--<asp:GridView ID="RoleList" autogeneratecolumns="False" runat="server">
    <Columns>
        <asp:CommandField EditText="Archive Role" ShowEditButton="True" />
 <asp:TemplateField HeaderText="Role">
 <ItemTemplate>
  
 <asp:Label runat="server" ID="RoleNameLabel" Text='<%# Container.DataItem %>' />
 </ItemTemplate>
 </asp:TemplateField>

 <asp:TemplateField HeaderText="Archived">
            <EditItemTemplate>
                 
            <asp:CheckBox runat="server" ID="RowlevelCheckBox" />
            
            </EditItemTemplate>
            <ItemTemplate>
                <asp:CheckBox ID="CheckBox1" runat="server"  
                    Enabled="true" AutoPostBack="True" />
            </ItemTemplate>
        </asp:TemplateField>
 </Columns>

    </asp:GridView>--%>
    </fieldset>
    
   </fieldset>   
</div>
<div id="rightcoll">
<asp:Button ID="CloseButton" Text="Exit" runat="server" causesvalidation="false" width="200px" Height="100px"/>
            </div>
            </div>
            </asp:View>
         </asp:MultiView>
            
                
     


   

    
    
    
    
   
        
    
   


