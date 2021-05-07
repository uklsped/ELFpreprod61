<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ViewCommentsuc.ascx.vb" Inherits="ViewCommentsuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<link href="App_Themes/Blue/calendar.css" rel="stylesheet" type="text/css" />
 <%--From http://www.dotnetcurry.com/showarticle.aspx?ID=149--%>
 <script language="javascript" type="text/javascript">
//     function datecheck(sender, args) {
//         if (sender._selectedDate > new Date()) {
//             alert("You cannot select a day later than today!");
//             sender._selectedDate = new Date();
//             // set the date back to the current date
//             sender._textbox.set_Value(sender._selectedDate.format(sender._format))
//         }
     //     }



     //This function prevents return key usage from http://www.felgall.com/jstip43.htm
     function kH(e) {
         var pK = e ? e.which : window.event.keyCode;
         return pK != 13;
     }
     document.onkeypress = kH;
     if (document.layers) document.captureEvents(Event.KEYPRESS);

    </script>




  <div style="background-color: #FFFFCC; border: thin solid #FFFF00">    
       Start Date:
        <asp:TextBox ID="txtStartDate" runat="server" ></asp:TextBox>&nbsp;&nbsp;
<asp:RequiredFieldValidator ID="RequiredFieldValidatorstart" runat="server" ErrorMessage="Please Enter Start Date" Display="Dynamic" ControlToValidate="txtStartDate" ValidationGroup="CommentDates"></asp:RequiredFieldValidator>
        <asp:CalendarExtender ID="ceStartDate" TargetControlID="txtStartDate" 

                 PopupPosition="BottomRight" 

                Format="dd/MM/yyyy"  runat="server" TodaysDateFormat="d M" CssClass="cal_Theme1">

                </asp:CalendarExtender>

        Stop Date:

        <asp:TextBox ID="StopBox1" runat="server" ></asp:TextBox>
<asp:RequiredFieldValidator ID="RequiredFieldValidatorstop" runat="server" ErrorMessage="Please Enter End Date" Display="Dynamic" ControlToValidate="StopBox1" ValidationGroup="CommentDates"></asp:RequiredFieldValidator>

        <asp:CalendarExtender ID="CalendarExtender2" Format="dd/MM/yyyy"

         PopupPosition="BottomRight" 

         TargetControlID="StopBox1" TodaysDateFormat="d M" CssClass="cal_Theme1"  runat="server" >

        </asp:CalendarExtender>
        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="txtStartDate"
        ControlToValidate="StopBox1" ErrorMessage="Start Date must be before Stop Date"
        Operator="GreaterThanEqual" Type="Date" ValidationGroup="CommentDates"></asp:CompareValidator>

    <asp:Button ID="submitButton" runat="server" Text="View History" 
            OnClientClick='"submitButton_Click"' CausesValidation="false" />
        
<asp:UpdatePanel ID="UpdatePanel1" visible="false"  runat="server">
<ContentTemplate>
    <asp:Panel ID="Panel1" scrollbars="vertical" Height="400px" runat="server">
    
<asp:GridView ID="GridView1"    runat="server" BackColor="White" BorderColor="#DEDFDE" 
        BorderStyle="None" BorderWidth="1px" CellPadding="4" ForeColor="Black" 
        GridLines="Vertical">
        
    <RowStyle BackColor="#F7F7DE" />
    <FooterStyle BackColor="#CCCC99" />
    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
    <AlternatingRowStyle BackColor="White" />
</asp:GridView>
</asp:Panel>
</ContentTemplate>
</asp:UpdatePanel>
</div>




       
    
 
 


