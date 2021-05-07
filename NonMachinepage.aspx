<%@ Page Title="" Language="VB" MasterPageFile="~/Elf.master" AutoEventWireup="false" CodeFile="NonMachinepage.aspx.vb" Inherits="NonMachinepage"  %>
<%@ MasterType VirtualPath="~/Elf.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>





<%@ Register src="LinacStatusuc.ascx" tagname="LinacStatusuc" tagprefix="uc5" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<script type="text/javascript">
        // JScript File

        // finds a control that had the given server id, of a the given type
    // in the given parent.
    function windowOpener(url, name, args) {
        if (typeof (popupWin) != "object") {
            popupWin = window.open(url, name, args);
        } else {
            if (!popupWin.closed) {
                popupWin.location.href = url;
            } else {
                popupWin = window.open(url, name, args);
            }
        }
        popupWin.focus();
    }        
           
        function findControl(parent, tagName, serverId) {
            var items = parent.getElementsByTagName(tagName);

            // walk the items looking for the right guy
            for (var i = 0; i < items.length; i++) {
                var ctl = items[i];
                if (ctl && ctl.id) {
                    // check the end of the name.
                    //
                    var subId = ctl.id.substring(ctl.id.length - serverId.length);
                    if (subId == serverId) {
                        return ctl;
                    }
                }
            }
            return null;
        }


        
        function loadTabPanel(sender, e) {

            var tabContainer = sender;

            if (tabContainer) {
                var updateControlId = "TabButton" + tabContainer.get_activeTabIndex();
                // get the active tab and find our button
                //
                var activeTab = tabContainer.get_activeTab();


                // check to see if we've already loaded
                //
                if (findControl(activeTab.get_element(), "div", "TabContent" + tabContainer.get_activeTabIndex())) return;


                var updateControl = findControl(activeTab.get_element(), "input", updateControlId);

                if (updateControl) {

                    // fire the update

                    updateControl.click();


                  
                }

            }

        }

//      
       
      

</script>

 
    <asp:TabContainer ID="tcl" runat="server"  activetabindex="1" 
            height="930px" 
            OnClientActiveTabChanged="loadTabPanel" >
            
<asp:TabPanel runat="server" HeaderText="" ID="TabPanel0"><HeaderTemplate>

</HeaderTemplate>
<ContentTemplate>
<asp:UpdatePanel ID="LinacStatus" runat="server" >
    <ContentTemplate>

        <asp:UpdatePanel ID="UpdatePanel0" runat="server" >
            <ContentTemplate>
           <%-- <asp:Button ID="TabButton0" runat="server"  OnClick="TabButton_Click"  style="display:none;" CausesValidation="false"/>--%>
            <asp:Panel ID="Panel0" runat="server" >
            
                    <uc5:LinacStatusuc ID="LinacStatusuc1" LinacName="nonmachine" runat="server" />
                    </asp:Panel></ContentTemplate></asp:UpdatePanel>
</ContentTemplate>
</asp:UpdatePanel>
</ContentTemplate>
</asp:TabPanel>    




        
        





  
        


        

  
        
    </asp:TabContainer>
 
</asp:Content>

