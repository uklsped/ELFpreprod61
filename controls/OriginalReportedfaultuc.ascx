<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OriginalReportedfaultuc.ascx.vb" Inherits="controls_OriginalReportedfaultuc" %>
<legend>Reported Fault Details</legend>
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="Linac" runat="server">
                <table style="width:300px;">                
        <tr>           
            <td class="style1">
                Area:</td>
            <td>
    <asp:TextBox ID="OriginalAreaBox" runat="server" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="style1">
                Energy:</td>
            <td>      

    <asp:TextBox ID="OriginalEnergyBox" runat="server" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="style1">
                Gantry Angle:</td>
            <td>
    <asp:TextBox ID="OriginalGantryBox" ReadOnly="True" runat="server"></asp:TextBox>

            </td>
        </tr>
        <tr>
                        <td class="style1">
                                Collimator Angle:</td>
            <td>
                
    <asp:TextBox ID="OriginalCollBox" runat="server" ReadOnly="True"></asp:TextBox>

            </td>
            </tr>
            <tr>
                
        <td class="style1">
                
                Fault Description:</td>
            <td>
                
              <asp:TextBox ID="OriginalDescriptionBoxL" runat="server" MaxLength="250" ReadOnly="True" 
                    Rows="5" TextMode="MultiLine"></asp:TextBox>
              </td>
              </tr>
               <tr>
                
        <td class="style1">
                
                Patient ID:</td>
            <td>
                
              <asp:TextBox ID="OriginalPatientIDBoxL" runat="server" ReadOnly="True"></asp:TextBox>
              </td>
              </tr>
              <tr>

              <td>
                  
              Reported By:</td>
              <td>
                  <asp:TextBox ID="OriginalReportedBoxL" runat="server" ReadOnly="True"></asp:TextBox>
                </td>
              </tr>
              <tr>
              <td>
                  
              Date Open:</td>
              <td>
                  <asp:TextBox ID="OriginalOpenDateBoxL" runat="server" ReadOnly="True"></asp:TextBox>
              </td>
        </tr>
                     <tr>
                  <td> <asp:Label ID="RadiationIncidentLabel" runat="server" Text="Radiation Incident"></asp:Label></td>
                  <td>
                       <asp:RadioButtonList ID="OriginalRadioIncident" runat="server" AutoPostBack="false" enabled="false">
                                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                        <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                  </td>
              </tr>
    </table>
        </asp:View>
    <asp:View ID="Tomo" runat="server">
          <table style="width:300px;">                
        <tr>           
            <td class="style1">
                Physics/Accuray:</td>
            <td>
    <asp:TextBox ID="AccurayTextBox" runat="server" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
         <tr>
            
            <td class="style1">

                Error code:</td>
            <td>      

    <asp:TextBox ID="ErrorTextBox" runat="server" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
       
            <tr>
                
        <td class="style1">
                
                Fault Description:</td>
            <td>
                
              <asp:TextBox ID="OriginalDescriptionBoxT" runat="server" MaxLength="250" ReadOnly="True" 
                    Rows="5" TextMode="MultiLine"></asp:TextBox>
              </td>
              </tr>
               <tr>
                
        <td class="style1">
                
                Patient ID:</td>
            <td>
                
              <asp:TextBox ID="OriginalPatientIDBoxT" runat="server" ReadOnly="True"></asp:TextBox>
              </td>
              </tr>
              <tr>

              <td>
                  
              Reported By:</td>
              <td>
                  <asp:TextBox ID="OriginalReportedBoxT" runat="server" ReadOnly="True"></asp:TextBox>
                </td>
              </tr>
              <tr>
              <td>
                  
              Date Open:</td>
              <td>
                  <asp:TextBox ID="OriginalOpenDateBoxT" runat="server" ReadOnly="True"></asp:TextBox>
              </td>
        </tr>
              <tr>
                  <td> <asp:Label ID="RadiationIncidentLabelT" runat="server" Text="Radiation Incident"></asp:Label></td>
                  <td>
                       <asp:RadioButtonList ID="OriginalRadioIncidentT" runat="server" AutoPostBack="false" enabled="false">
                                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
                                        <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
                                    </asp:RadioButtonList>
                  </td>
              </tr>
    </table>
    </asp:View>
    </asp:MultiView>
