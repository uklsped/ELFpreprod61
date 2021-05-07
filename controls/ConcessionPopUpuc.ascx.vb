Imports AjaxControlToolkit
Imports DavesCode
Partial Class controls_ConcessionPopUpuc
    Inherits System.Web.UI.UserControl
    Dim modalpopupextenderft As New ModalPopupExtender
    Public Property ParentName() As String
    Public Property LinacName() As String
    Private ParamApplication As String
    Private ConcessParamsTrial As ConcessionParameters = New ConcessionParameters()
    Public Event CloseFaultTracking(ByVal Linac As String)

    Public Event UpdateClosedDisplays(ByVal Linac As String)
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        ParamApplication = "Params" + LinacName


    End Sub
    Private Sub CloseTracking(ByVal LinacName As String)
        RaiseEvent CloseFaultTracking(LinacName)
    End Sub
    Private Sub CloseDisplays(ByVal LinacName As String)
        RaiseEvent UpdateClosedDisplays(LinacName)
    End Sub
    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim logerrorbox As Label = FindControl("LoginErrordetails")
        logerrorbox.Text = Nothing


        Dim MyString As String
            Dim Tabnumber As String
            MyString = "ModalPopupextenderft"
            Tabnumber = ParentName
            MyString = MyString & Tabnumber
            modalpopupextenderft.ID = MyString
            modalpopupextenderft.BehaviorID = MyString
            modalpopupextenderft.TargetControlID = "label1"
            modalpopupextenderft.PopupControlID = "Panel1"
            modalpopupextenderft.BackgroundCssClass = "modalBackground"
            PlaceHolder1.Controls.Add(modalpopupextenderft)
            modalpopupextenderft.Show()


    End Sub
    Public Sub SetUpFaultTracking()

        ConcessParamsTrial = Application(ParamApplication)
            If Not ConcessParamsTrial Is Nothing Then

                Dim FaultTracking As controls_FaultTrackinguc = Page.LoadControl("controls\FaultTrackinguc.ascx")
                CType(FaultTracking, controls_FaultTrackinguc).LinacName = ConcessParamsTrial.Linac
            FaultTracking.IncidentID = ConcessParamsTrial.IncidentID
            FaultTracking.ParentControl = ParentName
            FaultTracking.InitialiseFaultTracking(ConcessParamsTrial)
            AddHandler FaultTracking.CloseFaultTracking, AddressOf CloseTracking
            AddHandler FaultTracking.UpdateClosedDisplays, AddressOf CloseDisplays
            FaultTrackingPlaceHolder.Controls.Add(FaultTracking)
            End If

    End Sub
End Class
