<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ModalityDisplayuc.ascx.vb" Inherits="controls_ModalityDisplayuc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:UpdatePanel ID="MultiviewUpdatePanel" runat="server"><ContentTemplate>
<asp:MultiView ID="MultiView1"  runat="server" ActiveViewIndex="0">
    <asp:View ID="UnauthorisedView" runat="server">
        <asp:Label ID="UnauthorisedLabel" runat="server" Text="No Modalities Authorised"></asp:Label>
    </asp:View>
    <asp:View ID="PreClinicalView" runat="server">PreclinicalView</asp:View>
    <asp:View ID="ClinicalView" runat="server">
    <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False"
                                DataKeyNames="HandoverId"
                                BackColor="White" BorderColor="#CC9966" BorderStyle="None" BorderWidth="1px"
                                CellPadding="4" EditRowStyle-BorderStyle="NotSet" Width="785px">
                                <RowStyle BackColor="White" ForeColor="#330099" />
                                <Columns>
                                    <asp:TemplateField HeaderText="6 MV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MV6"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="6 MV FFF">
                                        <ItemTemplate>
                                            <img src=" <%#FormatImage(Eval("MV6FFF")) %>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="10 MV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MV10"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="10 MV FFF">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MV10FFF"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="4 MeV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MeV4"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="6 MeV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MeV6"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="8 MeV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MeV8"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="10 MeV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MeV10"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="12 MeV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MeV12"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="15 MeV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MeV15"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="18 MeV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MeV18"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="20 MeV">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("MeV20"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="iView">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("iView"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="XVI">
                                        <ItemTemplate>
                                            <img src="<%#FormatImage(Eval("XVI"))%>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                                <FooterStyle BackColor="#FFFFCC" ForeColor="#330099" />
                                <PagerStyle BackColor="#FFFFCC" ForeColor="#330099" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="#663399" />
                                <HeaderStyle BackColor="#330000" Font-Bold="True" ForeColor="#FFFFCC" />
                            </asp:GridView>
    </asp:View>
    </asp:MultiView>
</ContentTemplate></asp:UpdatePanel>