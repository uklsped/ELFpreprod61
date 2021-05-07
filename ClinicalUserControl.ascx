<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ClinicalUserControl.ascx.vb" Inherits="ClinicalUserControl" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="WriteDatauc.ascx" TagName="WriteDatauc" TagPrefix="uc2" %>

<%--<%@ Register Src="AcceptLinac.ascx" TagName="AcceptLinac" TagPrefix="uc3" %>--%>

<%@ Register Src="ViewCommentsuc.ascx" TagName="ViewCommentsuc" TagPrefix="uc4" %>

<%@ Register Src="DefectSave.ascx" TagName="DefectSave" TagPrefix="uc5" %>

<%@ Register Src="TodayClosedFault.ascx" TagName="TodayClosedFault" TagPrefix="uc6" %>


<%@ Register Src="DefectSavePark.ascx" TagName="DefectSavePark" TagPrefix="uc7" %>


<%@ Register Src="controls/CommentBoxuc.ascx" TagName="CommentBoxuc" TagPrefix="uc8" %>


<%@ Register src="controls/ReportFaultPopUpuc.ascx" tagname="ReportFaultPopUpuc" tagprefix="uc9" %>


<%@ Register src="controls/MainFaultDisplayuc.ascx" tagname="MainFaultDisplayuc" tagprefix="uc10" %>


<%@ Register src="controls/ReportAFaultuc.ascx" tagname="ReportAFaultuc" tagprefix="uc11" %>


<%@ Register src="controls/ModalityDisplayuc.ascx" tagname="ModalityDisplayuc" tagprefix="uc12" %>


<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />
<%--<%@ Register Src="ViewOpenFaults.ascx" TagName="ViewOpenFaults" TagPrefix="uc1" %>--%>
<asp:HiddenField ID="HiddenFieldModalityVisible" Value="False" runat="server" />
<div class="grid">
    <div class="col100 grey">
        <table id="HandoverTable">
            <tr style="vertical-align: top">
                <td colspan="3">
                    <asp:Panel ID="ModalityDisplayPanel" runat="server" Visible="false">
                        <asp:PlaceHolder ID="ModalityPlaceholder" runat="server"></asp:PlaceHolder>
                    </asp:Panel>
                </td>
            </tr>
            <tr style="vertical-align: top">
                <td colspan="4">
                    <table>
                        <tr>
                            <td>
                                <asp:Button ID="Tstart" runat="server"
                                    Text="Start Treatment" CausesValidation="false"
                                    BackColor="#FFCC00"
                                    Height="150px" Width="120px" />
                            </td>
                            <td>
                                <asp:Button ID="LogOffButton" runat="server" Text="Log Off" Height="150px"
                                    CausesValidation="false" />
                            </td>
                            <td>
                                <asp:Button ID="EndofDayButton" runat="server" Text="End of Day" Height="150px"
                                    CausesValidation="false" BackColor="#FF3300" ForeColor="White" />
                            </td>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <uc11:ReportAFaultuc ID="ReportAFaultuc1" runat="server" />
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                  
                </td>
            </tr>
            <tr>
                <td colspan="2" style="height: 92px">
                    <table>
                        <tr>
                            <td>
                                <asp:Literal ID="Literal1" runat="server" Text="Runup Comments"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Panel ID="Panel1" Enabled="false" runat="server">
                                    <uc8:CommentBoxuc ID="RunUpCommentBox" TextHeight="60" runat="server" NoWrite="true" />
                                </asp:Panel>
                            </td><td>

                                 </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <table style="width: 100%;">
                                        <tr>
                                            <td>Clinical Comment
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:UpdatePanel ID="UpdatePanelcomments" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <uc8:CommentBoxuc ID="CommentBox" TextHeight="60" runat="server" />
                                                        <br />
                                                        <asp:Button ID="SaveText" runat="server" Text="Save" CausesValidation="False" />
                                                    </ContentTemplate>
                                                    <Triggers>
                                                        <asp:AsyncPostBackTrigger ControlID="SaveText" EventName="click" />
                                                    </Triggers>
                                                </asp:UpdatePanel>
                                            </td>
                                           
                                        </tr>
                                        <tr>
                                            <td>
                                                <div style="background-color: Green; height: 30px; width: 355px; margin: 0; padding: 0">
                                                    <table cellspacing="0" cellpadding="0" rules="all" border="1" id="Table2"
                                                        style="font-family: Arial; font-size: 10pt; width: 355px; color: white; border-collapse: collapse; height: 100%;">
                                                        <tr>
                                                            <td style="width: 60px; text-align: center">Time</td>
                                                            <td style="width: 240px; text-align: center">Clinical Comment</td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                </td>
                                                    </tr>
                                                    <tr><td>
                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                                    <ContentTemplate>

                                                        <div style="height: 175px; width: 355px; overflow: auto;">
                                                            <asp:GridView ID="GridViewComments" runat="server" AutoGenerateColumns="False" ShowHeader="false"
                                                                DataKeyNames="Clincomment" BackColor="White"
                                                                BorderColor="#336666" BorderStyle="Double" BorderWidth="3px" CellPadding="4"
                                                                GridLines="Horizontal">
                                                                <RowStyle BackColor="White" ForeColor="#333333" />
                                                                <Columns>
                                                                    <asp:BoundField DataField="DateTime" ItemStyle-Width="60px"
                                                                        SortExpression="Time" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top"
                                                                        HtmlEncode="False" HtmlEncodeFormatString="True" />
                                                                    <asp:BoundField DataField="Clincomment" ItemStyle-Width="350px"
                                                                        SortExpression="Clinical" ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top"
                                                                        HtmlEncode="False" HtmlEncodeFormatString="True" />

                                                                </Columns>
                                                                <FooterStyle BackColor="White" ForeColor="#333333" />
                                                                <PagerStyle BackColor="#336666" ForeColor="White" HorizontalAlign="Center" />
                                                                <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                                                                <HeaderStyle BackColor="#336666" Font-Bold="True" ForeColor="White" />
                                                            </asp:GridView>
                                                        </div>

                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

        </table>
    </div>

    <div class="col300 green">
        <asp:UpdatePanel ID="UpdatePanel5" runat="server" Visible="true" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:PlaceHolder ID="PlaceHolderFaults" runat="server"></asp:PlaceHolder>
                &nbsp;
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<div>
    <uc2:WriteDatauc ID="WriteDatauc2" LinacName="" UserReason="3" Tabby="3" Visible="false" runat="server" />
</div>

<asp:HiddenField ID="HiddenFieldLinacState" Visible="true" runat="server" />
<div style="background-color: #FFFF66; background-repeat: no-repeat; border-style: solid; border-width: thin">
    <uc4:ViewCommentsuc ID="ViewCommentsuc1" LinacName="" CommentSort="pcr" runat="server" />
</div>