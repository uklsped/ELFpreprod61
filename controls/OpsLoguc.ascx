<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OpsLoguc.ascx.vb" Inherits="OpsLoguc" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>


    <%--From http://www.dotnetcurry.com/showarticle.aspx?ID=149--%>

    <link href="App_Themes/Blue/calendar.css" rel="stylesheet" type="text/css" />
    
       

 
 <script type="text/javascript">
     
     function showDate(sender, args) {

         if (sender._textbox.get_element().value == "") {

             var todayDate = new Date();

             sender._selectedDate = todayDate;

         }

     }
 
 
    function checkDate(sender,args)
{
 if (sender._selectedDate > new Date())
            {
                alert("You cannot select a day later than today!");
                sender._selectedDate = new Date(); 
                // set the date back to the current date
sender._textbox.set_Value(sender._selectedDate.format(sender._format))
            }
        }

        function SelectToday(sender, e) {
            var format = sender._todaysDateFormat;
            var date = new Date();
            var formateddate = date.format(format);
            var daycells = $get(sender.get_id() + "_body").getElementsByTagName("DIV");
            for (var i = 0; i < daycells.length; i++) {
                if (daycells[i].title.indexOf(formateddate) > -1) {
                    daycells[i].style.backgroundColor = "Grey";
                    daycells[i].style.color = "White";
                    break;
                }
            }
        }
 
//This function prevents return key usage from http://www.felgall.com/jstip43.htm
        function kH(e) {
           var pK = e ? e.which : window.event.keyCode;
            return pK != 13;
        }
        document.onkeypress = kH;
        if (document.layers) document.captureEvents(Event.KEYPRESS);

    </script>
    

    
  <div id="container" style="background-color: #66FFFF; float: none; padding: 0px; margin: 0px auto 0px auto; overflow: hidden;" enableviewstate="false">
        <div id="leftcoll" style="float: left; width: 260px;">
<fieldset>
    <asp:Label ID="linacLabel" runat="server" Text="Please Select Linac:"></asp:Label> <br /><br />
<asp:DropDownList id="dropLinac" AutoPostBack="true" runat="server">
<asp:ListItem>Select</asp:ListItem>
<asp:ListItem>E1</asp:ListItem>
<asp:ListItem>E2</asp:ListItem>
<asp:ListItem>B1</asp:ListItem>
<asp:ListItem>B2</asp:ListItem>
<asp:ListItem>T1</asp:ListItem>
<asp:ListItem>T2</asp:ListItem>
<asp:ListItem>T3</asp:ListItem>
</asp:DropDownList>  
    
</fieldset>
    &nbsp;


    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional" Visible = "false"><ContentTemplate>
    
    <asp:RadioButtonList ID="RadioButtonOps" runat="server" AutoPostBack="true">
        <asp:ListItem Text="Operational Log" Value="1"></asp:ListItem>
        
        
    </asp:RadioButtonList>
        <br />
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel2" DynamicLayout="false">
<ProgressTemplate>
Processing Request Please Wait...
</ProgressTemplate>
    </asp:UpdateProgress>
        <br />
        <asp:UpdatePanel ID="DatePanel" runat="server" Visible="false">
        <ContentTemplate>
        
        <asp:Table ID="Table2" runat="server">
        <asp:TableRow>
        <asp:TableCell>
        <asp:Label ID="StartLabel" runat="server" Text="Start Date:" Width="50px"></asp:Label>
        </asp:TableCell>
        <asp:TableCell>
    <asp:TextBox ID="StartDate"  runat="server" ViewStateMode="Disabled" Text=""></asp:TextBox>
    
<asp:RequiredFieldValidator ID="RequiredFieldValidatorstart" runat="server" ErrorMessage="Please Enter Start Date" Display="dynamic" ControlToValidate="StartDate" Enabled="False" ForeColor="#FF3300" validationgroup="selection" ></asp:RequiredFieldValidator>
    
<asp:CalendarExtender ID="StartDate_CalendarExtender" runat="server" 
        Enabled="True" Format="dd/MM/yyyy" TargetControlID="StartDate" 
         CssClass="cal_Theme1">
    </asp:CalendarExtender>
    </asp:TableCell>
    </asp:TableRow> 
<asp:TableRow>
<asp:TableCell>
        <asp:Label ID="EndLabel" runat="server" Text="End Date:"></asp:Label>
</asp:TableCell>
<asp:TableCell>
    <asp:TextBox ID="EndDate"  runat="server"  EnableViewState="False" ViewStateMode="Disabled"></asp:TextBox>
    
    <asp:RequiredFieldValidator ID="RequiredFieldValidatorstop" runat="server" 
            ErrorMessage="Please Enter End Date" Display="Dynamic" 
            ControlToValidate="EndDate" Enabled="false" ForeColor="#FF3300" validationgroup="selection" ></asp:RequiredFieldValidator>
   
<asp:CalendarExtender ID="EndDate_CalendarExtender" runat="server" 
        Enabled="True" TargetControlID="EndDate" Format="dd/MM/yyyy" 
         CssClass="cal_Theme1"  >
    </asp:CalendarExtender>
    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="StartDate"
        ControlToValidate="EndDate" ErrorMessage="Start Date must be before End Date"
        Operator="GreaterThanEqual" Type="Date" validationgroup="selection"></asp:CompareValidator>     
        <br />
      </asp:TableCell>
    </asp:TableRow> 
    </asp:Table>
    </ContentTemplate></asp:UpdatePanel> 
    <asp:Table runat="server">
   <asp:TableRow>
   <asp:TableCell>
    <asp:Button ID="ViewOpsButton" runat="server" Text="View" 
        CausesValidation="true" BackColor="#33CC33" Height="56px" Width="105px" Enabled ="true" validationgroup="selection" Visible="false" />
        </asp:TableCell>
        <asp:TableCell>
        <asp:Button ID="ExitButton" runat="server" Text="Exit" CausesValidation="false" 
        Height="56px" Width="105px" />
        </asp:TableCell>
        </asp:TableRow>
        </asp:Table>
   </ContentTemplate>
        <Triggers>
        <asp:AsyncPostBackTrigger  ControlID="RadioButtonOps" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="ViewOpsButton" EventName="click" />
        </Triggers>
    </asp:UpdatePanel>
      </div>
 
   <div id="centrecoll" 
            style= "float: left; width: 710px;  margin-right: 46px;">  
   <asp:UpdatePanel ID="UpdatePanel1" runat="server" >
   <ContentTemplate>
     
        <asp:GridView ID="GridView3" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="StateID"
                        style="top: 670px; left: 10px; height: 162px; width: 617px;" 
                        ForeColor="#333333" GridLines="None" AllowPaging="True"  
           PageSize="20"
            Caption='<table border="1" width="100%" height="40px" cellpadding="0" cellspacing="0" bgcolor="white"><tr><td>Grid Heading</td></tr></table>'

CaptionAlign="Top" >
                        

<RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" />
     <Columns>

    <asp:BoundField DataField="Linac" HeaderText="Linac" InsertVisible="False" 
                                ReadOnly="True" SortExpression="linac" />
                                
<asp:BoundField DataField="StateID" HeaderText="State ID" 
                                SortExpression="StateID" />
                                
<asp:BoundField DataField="Date" HeaderText="Date" 
                                SortExpression="Date" />
                                
<asp:BoundField DataField="ReasonDescription" HeaderText="Reason" 
                                SortExpression="ReasonDescription" />
                                
<asp:BoundField DataField="StartTime" HeaderText="Start Time" 
                                SortExpression="StartTime" />
                            
<asp:BoundField DataField="Stoptime" HeaderText="Stop Time" 
                                SortExpression="Stoptime" ItemStyle-HorizontalAlign="Center" >
                            
<ItemStyle HorizontalAlign="Center" />
                            
</asp:BoundField>
<asp:BoundField DataField="Duration/min" HeaderText="Duration/min" 
                                SortExpression="duration/min" />
                            
<asp:BoundField DataField="User type" HeaderText="User Group" 
                                SortExpression="Usertype" />
                            
<asp:BoundField DataField="Username" HeaderText="User Name" 
                                SortExpression="username" />

<asp:BoundField DataField="Comments" HeaderText="Comments" 
                                SortExpression="Comments" />
                        
</Columns>                                            

                        

<FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                        

           <PagerSettings Mode="NumericFirstLast" />
                        

<PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        

<SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        

<HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        

<EditRowStyle BackColor="#999999" />
                        
<AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    
</asp:GridView>


         <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="StateID"
                        style="top: 670px; left: 10px; height: 162px; width: 617px;" 
                        ForeColor="#333333" GridLines="None" AllowPaging="True"  
           PageSize="20"
            Caption='<table border="1" width="100%" height="40px" cellpadding="0" cellspacing="0" bgcolor="white"><tr><td>Grid Heading</td></tr></table>'

CaptionAlign="Top" >
                        

<RowStyle BackColor="#F7F6F3" ForeColor="#333333" HorizontalAlign="Center" />
                          

<FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
                        

           <PagerSettings Mode="NumericFirstLast" />
                        

<PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        

<SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        

<HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        

<EditRowStyle BackColor="#999999" />
                        
<AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    
</asp:GridView>
       
 
</ContentTemplate>
</asp:UpdatePanel>

</div>
      </div>

   


