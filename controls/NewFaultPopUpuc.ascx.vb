Imports AjaxControlToolkit
Imports DavesCode
Partial Class controls_NewFaultPopUpuc
    Inherits System.Web.UI.UserControl

    Private IncidentID As String
    Dim modalpopupextendernf As New ModalPopupExtender
    Public Property ParentName() As String
    Public Property LinacName() As String
    Private ParamApplication As String
    Private ConcessParamsTrial As ConcessionParameters = New ConcessionParameters()
    Private FaultParamsApplication As String
    Public Event CloseFaultTracking(ByVal Linac As String)
    Public Event UpdateOpenConcessions(ByVal Linac As String)
    Public Event UpdateClosedDisplays(ByVal Linac As String)
    'Public Event UpdateClosedDisplays(ByVal Linac As String, ByVal IncidentID As String)
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        ParamApplication = "Params" + LinacName
        AddHandler FaultTrackinguc1.CloseFaultTracking, AddressOf CloseTracking
        'AddHandler FaultTrackinguc1.UpdateClosedDisplays, AddressOf CloseDisplays
        FaultParamsApplication = "FaultParams" + LinacName
    End Sub


    Private Sub CloseTracking(ByVal LinacName As String)
        RaiseEvent CloseFaultTracking(LinacName)
    End Sub

    Private Sub Update_OpenConcessions(ByVal LinacName As String)
        RaiseEvent UpdateOpenConcessions(LinacName)
    End Sub

    Private Sub Update_ClosedDisplays(ByVal LinacName As String)
        RaiseEvent UpdateClosedDisplays(LinacName)
    End Sub


    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim success As Boolean = False
        Dim logerrorbox As Label = FindControl("LoginErrordetails")
        logerrorbox.Text = Nothing


        If Not Application(ParamApplication) Is Nothing Then
                ConcessParamsTrial = Application(ParamApplication)
            Else
                If NewFaultHandling.CheckForOpenFault(LinacName) Then

                    ConcessParamsTrial.Linac = LinacName
                    ConcessParamsTrial.IncidentID = NewFaultHandling.ReturnNewIncidentID(LinacName)
                    Application(ParamApplication) = ConcessParamsTrial
                Else
                    RaiseError()
                End If

            End If


        If Not ConcessParamsTrial Is Nothing Then

            Dim FaultTracking As controls_FaultTrackinguc = CType(FindControl("FaultTrackinguc1"), controls_FaultTrackinguc)
                FaultTracking.LinacName = ConcessParamsTrial.Linac
            FaultTracking.IncidentID = ConcessParamsTrial.IncidentID
            FaultTracking.ParentControl = ParentName
            FaultTracking.InitialiseFaultTracking(ConcessParamsTrial)
            AddHandler FaultTrackinguc1.UpdateOpenConcessions, AddressOf Update_OpenConcessions
                AddHandler FaultTrackinguc1.UpdateClosedDisplays, AddressOf Update_ClosedDisplays
                AddHandler FaultTracking.CloseFaultTracking, AddressOf CloseTracking

                Dim MyString As String
                Dim Tabnumber As String
                MyString = "ModalPopupextendernf"
                Tabnumber = ParentName
                MyString = MyString & Tabnumber
                modalpopupextendernf.ID = MyString
                modalpopupextendernf.BehaviorID = MyString
                modalpopupextendernf.TargetControlID = "label1"
                modalpopupextendernf.PopupControlID = "Panel1"
                modalpopupextendernf.BackgroundCssClass = "modalBackground"
                PlaceHolder1.Controls.Add(modalpopupextendernf)
                modalpopupextendernf.Show()
            Else
                RaiseError()
            End If

    End Sub

    Protected Sub RaiseError()
        Dim strScript As String = "<script>"
        strScript += "alert('Problem loading Fault. Please call Engineer');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(Label1, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Function GetNewIncident() As String

        Dim IncidentID As String = String.Empty
        IncidentID = NewFaultHandling.ReturnNewIncidentID(LinacName)
        Return IncidentID
    End Function

End Class
