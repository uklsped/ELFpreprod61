<%@ Control Language="VB" AutoEventWireup="false" CodeFile="LinacStatusuc.ascx.vb" Inherits="LinacStatusuc" %>

 <%@ Register src="WriteDatauc.ascx" tagname="WriteDatauc" tagprefix="uc1" %>

 <%@ Register src="ViewFaultsuc.ascx" tagname="viewfaultsuc" tagprefix="uc2" %>

 <%@ Register src="RegisterUseruc.ascx" tagname="RegisterUseruc" tagprefix="uc3" %>

<%@ Register src="Administrationuc.ascx" tagname="administrationuc" tagprefix="uc4" %>

<%@ Register src="controls/ModalityDisplayuc.ascx" tagname="ModalityDisplayuc" tagprefix="uc5" %>

<%--<%@ Register src="controls/OpsHistoryPopUpuc.ascx" tagname="OpsHistoryPopUpuc" tagprefix="uc6" %>--%>

<%@ Register src="ViewCommentsuc.ascx" tagname="ViewCommentsuc" tagprefix="uc7" %>

<%-- <link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />--%>
<%--<link rel="stylesheet" type="text/css" href="css\twocol.css" />--%>
 <%--<asp:TextBox ID="TextBox1" runat="server" Width="168px"></asp:TextBox>--%>
<%--    <br />
    <br />
    <br />--%>
<asp:HiddenField ID="HiddenField1" runat="server" Visible="False" Value="False" />
<asp:HiddenField ID="HiddenField2" runat="server" Visible="False" Value="False" />
    
            <asp:PlaceHolder ID="PlaceHolder1" runat="server">
           <uc1:WriteDatauc ID="WriteDatauc1" LinacName = "" UserReason = "100" Tabby="0" WriteName="Status" Visible ="false" runat="server" />
            
         </asp:PlaceHolder>
&nbsp;<br />
        <asp:GridView ID="DummyGridView" runat="server">
    </asp:GridView>
    
       
   <br />

   <div>
    <asp:Button id="FaultButton" runat="server" Text="View Fault History" causesvalidation="false" OnClick="FaultButton_Click" /> &nbsp;
    <asp:Button id="RegisterButton" runat="server" Text="Register" causesvalidation="false" OnClick="RegisterButton_Click" /> &nbsp; 
    <asp:Button id="AdminButton" runat="server" Text="Administration" causesvalidation="false" OnClick="AdminButton_Click" />  &nbsp;
    <asp:Button ID="HistoryButton" runat="server" Text="View Operation History" visible="false" causesvalidation="false" /> &nbsp;
    <asp:Button ID="Reset" runat="server" Text="Reset ELF" causesvalidation="false"/>
            
       </div>    
           
            <div id="container">
           <%-- <uc6:OpsHistoryPopUpuc ID="OpsHistoryPopUpuc1" runat="server" />--%>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
            <asp:View ID="View0" runat="server">
            </asp:View>
                <asp:View ID="View1" runat="server">
                                
                    <uc2:ViewFaultsuc ID="ViewFaultsuc1" LinacName='' runat="server" />
                    </asp:View>
                   <asp:View ID="View2" runat="server">
                  <asp:PlaceHolder ID="PlaceHolder2" runat="server">
                     <uc3:RegisterUseruc ID="RegisterUseruc1" updatemode="conditional" runat="server" />
                    </asp:PlaceHolder>
                    
                     </asp:View>
                <asp:View ID="View3" runat="server" >
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
                    <ContentTemplate>
                    
                   <uc4:Administrationuc ID="Administrationuc1" linac='' runat="server" />
                   </ContentTemplate>
                   </asp:UpdatePanel>
                </asp:View>
            </asp:MultiView>
            
            </div>
    <br />
  
    
<%--<uc7:ViewCommentsuc ID="ViewCommentsuc1" runat="server" LinacName="T1" CommentSort="pcr" />--%>

   




    


   




    
