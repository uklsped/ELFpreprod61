<%@ Control Language="VB" AutoEventWireup="false" CodeFile="EnergyDisplayuc.ascx.vb" Inherits="EnergyDisplayuc" %>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
    <div>

<asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" 
            DataKeyNames="HandoverId" 
        BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px" 
        CellPadding="4"  EditRowStyle-BorderStyle="NotSet" Width="906px" >
            <RowStyle BackColor="White" ForeColor="#330099" />
            <Columns>
                   <asp:TemplateField HeaderText="6 MV"  >
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MV6"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="6 MV FFF"  >
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MV6FFF"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="10 MV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MV10"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                     <asp:TemplateField HeaderText="10 MV FFF">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MV10FFF"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="4 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV4"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="6 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV6"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="8 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV8"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="10 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV10"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="12 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV12"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="15 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV15"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="18 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV18"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="20 MeV">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("MeV20"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="iView"  >
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("iView"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="XVI">
                    <ItemTemplate >
                    <img src= "<%#FormatImage(Eval("XVI"))%>" />
                    </ItemTemplate>
                    </asp:TemplateField>
                    
            </Columns>
            <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
            <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
            <HeaderStyle BackColor="#330000" Font-Bold="True" ForeColor="#FFFFCC" />
        </asp:GridView>
        </div>
         </ContentTemplate>
    </asp:UpdatePanel>