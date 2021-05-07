
Imports System.Data

Partial Class controls_MainFaultDisplayuc
    Inherits System.Web.UI.UserControl
    Public Property LinacName() As String
    Public Property ParentControl As String
    Private Property MainFaultID As String = "MainFaultDisplay"
    Public Event Mainfaultdisplay_UpdateClosedFaultDisplay(ByVal LinacName As String)

    Const REPEATFAULT As Integer = 0


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim objMFG As UserControl = Page.LoadControl("ManyFaultGriduc.ascx")
        CType(objMFG, ManyFaultGriduc).IncidentID = REPEATFAULT
        'to accomodate Tomo now need to pass equipment name?
        CType(objMFG, ManyFaultGriduc).LinacName = LinacName
        CType(objMFG, ManyFaultGriduc).ID = "ManyFaultGriduc"
        PlaceHolderFaults.Controls.Add(objMFG)

        Dim objconToday As TodayClosedFault = Page.LoadControl("TodayClosedFault.ascx")
        objconToday = Page.LoadControl("TodayClosedFault.ascx")
        objconToday.ID = "TodayClosedfault"
        objconToday.LinacName = LinacName
        PlaceHolderTodaysclosedfaults.Controls.Add(objconToday)

        Dim objCon As controls_ViewOpenFaultsuc = Page.LoadControl("controls\ViewOpenFaultsuc.ascx")
        CType(objCon, controls_ViewOpenFaultsuc).LinacName = LinacName
        CType(objCon, controls_ViewOpenFaultsuc).ParentControl = ParentControl
        CType(objCon, controls_ViewOpenFaultsuc).ID = "ViewOpenFaultsuc"
        'AddHandler CType(objCon, controls_ViewOpenFaultsuc).ResetViewOpenFaults, AddressOf Update_FaultClosedDisplays
        'AddHandler CType(objCon, ViewOpenFaults).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay
        AddHandler CType(objCon, controls_ViewOpenFaultsuc).ViewOpenFaults_UpdateClosedFaultDisplays, AddressOf UpdateMasterClosedFaultDisplay
        PlaceHolderViewOpenFaults.Controls.Add(objCon)
    End Sub

    Protected Sub UpdateMasterClosedFaultDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            RaiseEvent Mainfaultdisplay_UpdateClosedFaultDisplay(LinacName)
        End If
    End Sub
    Public Sub Update_defectsToday(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            Dim TodayDefectClosed As ManyFaultGriduc = PlaceHolderFaults.FindControl("ManyFaultGriduc")
            TodayDefectClosed.BindGridClosedTodayFault()
        End If
    End Sub

    Public Sub Update_OpenConcessions(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            Dim OpenConcessions As controls_ViewOpenFaultsuc = PlaceHolderViewOpenFaults.FindControl("ViewOpenFaultsuc")
            OpenConcessions.RebindViewFault()
        End If
    End Sub

    Public Sub Update_FaultClosedDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            Dim ClosedFaults As TodayClosedFault = PlaceHolderTodaysclosedfaults.FindControl("TodayClosedfault")
            ClosedFaults.Update_ClosedDisplays(EquipmentID)
        End If
    End Sub



End Class
