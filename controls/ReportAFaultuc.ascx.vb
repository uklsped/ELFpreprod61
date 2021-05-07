
Partial Class controls_ReportAFaultuc
    Inherits System.Web.UI.UserControl

    Public Property LinacName() As String
    Public Property ParentControl() As String
    Const FAULTPOPUPSELECTED As String = "faultpopupupselected"
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"
    Const ENG As String = "1"
    Public Event ReportAFault_UpdateDailyDefectDisplay(ByVal Linac As String)
    Public Event ReportAFault_UpDateViewOpenFaults(ByVal Linac As String)

    Private Property DynamicControlSelection() As String
        Get
            Dim result As String = ViewState.Item(VIEWSTATEKEY_DYNCONTROL)
            If result Is Nothing Then
                'doing things like this lets us access this property without
                'worrying about this property returning null/Nothing
                Return String.Empty
            Else
                Return result
            End If
        End Get
        Set(ByVal value As String)
            ViewState.Item(VIEWSTATEKEY_DYNCONTROL) = value
        End Set
    End Property
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim ThereIsAnOpenFault As Boolean = True
        'If DavesCode.NewFaultHandling.CheckForOpenFault(LinacName) Then
        '    ReportFaultButton.Enabled = False
        'Else
        '    ReportFaultButton.Enabled = True
        'End If
        Select Case Me.DynamicControlSelection
        '    Case REPEATFAULTSELECTED
            '        LoadRepeatFaultTable(HiddenIncidentID.Value, HiddenConcessionNumber.Value)
            Case FAULTPOPUPSELECTED
                Dim CommentControl As controls_CommentBoxuc = Parent.FindControl("CommentBox")
                Dim DaTxtBox As TextBox = CommentControl.FindControl("TextBox")
                Dim Comment As String = DaTxtBox.Text
                Application("TabComment") = Comment
                Dim objReportFault As controls_ReportFaultPopUpuc = Page.LoadControl("controls\ReportFaultPopUpuc.ascx")
                CType(objReportFault, controls_ReportFaultPopUpuc).LinacName = LinacName
                CType(objReportFault, controls_ReportFaultPopUpuc).ID = "ReportFaultPopupuc"
                CType(objReportFault, controls_ReportFaultPopUpuc).ParentControl = ParentControl

                AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay
                AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).UpdateViewOpenFaults, AddressOf Update_ViewOpenFaults
                AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).CloseReportFaultPopUp, AddressOf Close_ReportFaultPopUp
                ReportFaultPopupPlaceHolder.Controls.Add(objReportFault)
                objReportFault.SetUpReportFault()
            Case Else
                'no dynamic controls need to be loaded...yet
        End Select
    End Sub
    Protected Sub ReportFaultButton_Click(sender As Object, e As EventArgs) Handles ReportFaultButton.Click
        'To bodge a solution this needs to check if there is an open fault already that hasn't been handled because of a problem.
        'Need to load reportfaultpopupuc here to pass comment box

        Dim CommentControl As controls_CommentBoxuc = Parent.FindControl("CommentBox")
            Dim DaTxtBox As TextBox = CommentControl.FindControl("TextBox")
            Dim Comment As String = DaTxtBox.Text
            Application("TabComment") = Comment

            Dim objReportFault As controls_ReportFaultPopUpuc = Page.LoadControl("controls\ReportFaultPopUpuc.ascx")
            CType(objReportFault, controls_ReportFaultPopUpuc).LinacName = LinacName
            CType(objReportFault, controls_ReportFaultPopUpuc).ID = "ReportFaultPopupuc"
            CType(objReportFault, controls_ReportFaultPopUpuc).ParentControl = ParentControl
            DynamicControlSelection = FAULTPOPUPSELECTED

            AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay
            AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).UpdateViewOpenFaults, AddressOf Update_ViewOpenFaults
            AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).CloseReportFaultPopUp, AddressOf Close_ReportFaultPopUp
            ReportFaultPopupPlaceHolder.Controls.Add(objReportFault)
            objReportFault.SetUpReportFault()

    End Sub

    Protected Sub Update_DefectDailyDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            RaiseEvent ReportAFault_UpdateDailyDefectDisplay(LinacName)
        End If
    End Sub

    Protected Sub Update_ViewOpenFaults(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            RaiseEvent ReportAFault_UpDateViewOpenFaults(LinacName)

        End If
    End Sub
    Protected Sub Close_ReportFaultPopUp(ByVal EquipmentId As String, ByVal ErrorState As Boolean)
        If LinacName = EquipmentId Then
            If Not ErrorState Then
                RaiseError()
            End If
            DynamicControlSelection = String.Empty
            Dim ReportFault As controls_ReportFaultPopUpuc = CType(FindControl("ReportFaultPopupuc"), controls_ReportFaultPopUpuc)
            ReportFaultPopupPlaceHolder.Controls.Remove(ReportFault)

        End If

    End Sub

    Protected Sub RaiseError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Updating Fault. Please call Engineering');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(ReportFaultButton, Me.GetType(), "JSCR", (strScript.ToString()), False)

    End Sub
End Class
