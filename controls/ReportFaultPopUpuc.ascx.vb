Imports AjaxControlToolkit
Imports DavesCode
Partial Class controls_ReportFaultPopUpuc
    Inherits System.Web.UI.UserControl
    Dim modalpopupextenderrf As New ModalPopupExtender
    Public Property ParentControl() As String
    Public Property LinacName() As String
    Public Property ParentCommentBox() As String
    Private ParamApplication As String
    Private ConcessParamsTrial As ConcessionParameters = New ConcessionParameters()
    Public Event UpDateDefectDailyDisplay(ByVal Linac As String)
    Public Event UpdateViewOpenFaults(ByVal Linac As String)
    Public Event CloseReportFaultPopUp(ByVal Linac As String, ByVal ErrorStatus As Boolean)
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        ParamApplication = "Params" + LinacName

    End Sub

    Private Sub Update_DefectDailyDisplay(ByVal linacName As String)
        RaiseEvent UpDateDefectDailyDisplay(linacName)
    End Sub
    Private Sub Update_ViewOpenFaults(ByVal linacName As String)
        RaiseEvent UpdateViewOpenFaults(linacName)
    End Sub

    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim logerrorbox As Label = FindControl("LoginErrordetails")
        logerrorbox.Text = Nothing

        Dim MyString As String
        Dim Tabnumber As String
        MyString = "ModalPopupextenderrf"
        Tabnumber = ParentControl
        MyString = MyString & Tabnumber
        modalpopupextenderrf.ID = MyString
        modalpopupextenderrf.BehaviorID = MyString
        modalpopupextenderrf.TargetControlID = "Label1"
        modalpopupextenderrf.PopupControlID = "Panel1"
        modalpopupextenderrf.BackgroundCssClass = "modalBackground"
        PlaceHolder1.Controls.Add(modalpopupextenderrf)
        modalpopupextenderrf.Show()

    End Sub

    Public Sub SetUpReportFault()
        'ConcessParamsTrial = Application(ParamApplication)

        If Not LinacName Like "T#" Then
            Dim objDefect As DefectSave = Page.LoadControl("DefectSave.ascx")
            CType(objDefect, DefectSave).ID = "DefectDisplay"
            CType(objDefect, DefectSave).LinacName = LinacName
            CType(objDefect, DefectSave).ParentControl = ParentControl
            CType(objDefect, DefectSave).ParentControlComment = ParentCommentBox
            AddHandler CType(objDefect, DefectSave).CloseReportFaultPopUp, AddressOf Close_ReportFaultPopUp
            AddHandler CType(objDefect, DefectSave).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay
            AddHandler CType(objDefect, DefectSave).UpdateViewOpenFaults, AddressOf Update_ViewOpenFaults
            objDefect.InitialiseDefectPage()
            PlaceHolderDefectSave.Controls.Add(objDefect)

        Else
            Dim objDefectPark As DefectSavePark = Page.LoadControl("DefectSavePark.ascx")
            CType(objDefectPark, DefectSavePark).ID = "DefectDisplaypark"
            CType(objDefectPark, DefectSavePark).LinacName = LinacName
            CType(objDefectPark, DefectSavePark).ParentControl = ParentControl
            CType(objDefectPark, DefectSavePark).ParentControlComment = ParentCommentBox
            AddHandler CType(objDefectPark, DefectSavePark).CloseReportFaultPopUp, AddressOf Close_ReportFaultPopUp
            AddHandler CType(objDefectPark, DefectSavePark).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay
            AddHandler CType(objDefectPark, DefectSavePark).UpdateViewOpenFaults, AddressOf Update_ViewOpenFaults
            objDefectPark.InitialiseDefectPage()
            PlaceHolderDefectSavePark.Controls.Add(objDefectPark)
            DefectView.ActiveViewIndex = 1
        End If
    End Sub
    Protected Sub Close_ReportFaultPopUp(ByVal Linac As String, ByVal ErrorStatus As Boolean)
        If LinacName = Linac Then
            RaiseEvent CloseReportFaultPopUp(Linac, ErrorStatus)
        End If
    End Sub

End Class
