Imports System.Data
Imports System.Data.SqlClient
Imports DavesCode

Partial Class Controls_DeviceRepeatFaultuc
    Inherits System.Web.UI.UserControl
    Public Property IncidentID() As String
    Public Property LinacName() As String
    Public Property ConcessionN() As String
    Private conn As SqlConnection
    Private comm As SqlCommand
    Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
    Dim commfault As SqlCommand
    'Private ConcessionNumber As String
    Const RepeatFault As String = "Repeatfault"
    Const CancelFaultReturn As String = "Cancel"
    Const EMPTYSTRING As String = ""
    Private FaultDescriptionChanged As String
    Private RadActDescriptionChanged As String
    Private FaultDescriptionChangedT As String
    Private RadActDescriptionChangedT As String
    Private Comment As String
    Private RadActComment As String
    Private FaultApplication As String
    Private FaultParam As FaultParameters = New FaultParameters()
    Public Event UpdateRepeatFault(ByVal LinacName As String)
    Public Event CloseRepeatFault(ByVal LinacName As String)

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        FaultDescriptionChanged = "defectFault" + LinacName
        RadActDescriptionChanged = "radact" + LinacName
        FaultApplication = "FaultParams" + LinacName
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If LinacName Like "T*" Then
            '    TomoLoad()
            '    MultiView1.SetActiveView(Tomo)
            FaultDescriptionT.BoxChanged = FaultDescriptionChanged
            RadActCT.BoxChanged = RadActDescriptionChanged
        Else
            FaultDescription.BoxChanged = FaultDescriptionChanged
            RadActC.BoxChanged = RadActDescriptionChanged
            '    LinacLoad()
            '    MultiView1.SetActiveView(Linac)
        End If

    End Sub
    Public Sub InitialiseRepeatFault(ByVal FaultObject As FaultParameters)
        'because T has trailing blanks
        If LinacName Like "T*" Then
            TomoLoad(FaultObject)
            MultiView1.SetActiveView(Tomo)
        Else
            LinacLoad(FaultObject)
            MultiView1.SetActiveView(Linac)
        End If
        ConcessionNumber.Text = FaultObject.ConcessionNumber
    End Sub
    Protected Sub LinacLoad(ByVal FaultObject As FaultParameters)


        AreaBox.Text = FaultObject.Area
        GantryAngleBox.Text = String.Empty
        CollimatorAngleBox.Text = String.Empty
        RadActC.ResetCommentBox(FaultObject.RadAct)
        'DescriptionBox.Text = String.Empty
        PatientIDBox.Text = String.Empty
        RadioIncident.SelectedIndex = -1
        ConcessionNumber.Text = FaultObject.ConcessionNumber
        Application(FaultApplication) = FaultObject
        'conn = New SqlConnection(connectionString)
        ''from http://www.sqlservercentral.com/Forums/Topic1416029-1292-1.aspx
        'comm = New SqlCommand("SELECT Area from ReportFault where incidentID=@incidentID", conn)
        'comm.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
        'comm.Parameters("@incidentID").Value = IncidentID
        'conn.Open()
        'AreaBox.Text = comm.ExecuteScalar()
        'comm = New SqlCommand("SELECT Action FROM ConcessionTable where incidentID=@incidentID", conn) 'Corrective action
        'comm.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
        'comm.Parameters("@incidentID").Value = IncidentID
        'Dim sqlresult1 As Object = comm.ExecuteScalar()
        'RadActCT.ResetCommentBox(sqlresult1.ToString)
        'conn.Close()

        AddEnergyItem()

    End Sub
    Protected Sub TomoLoad(ByVal FaultObject As FaultParameters)
        Application(FaultApplication) = FaultObject
        RadActCT.ResetCommentBox(FaultObject.RadAct)
        'FaultDescriptionT.BoxChanged = FaultDescriptionChanged
        'RadActCT.BoxChanged = RadActDescriptionChanged
        'DescriptionBoxT.Text = String.Empty
        PatientIDBoxT.Text = String.Empty
        ErrorTextBox.Text = String.Empty
        Accuray.Text = String.Empty
        'RadAct.Text = String.Empty
        RadioIncident.SelectedIndex = -1
        'conn = New SqlConnection(connectionString)
        ''from http://www.sqlservercentral.com/Forums/Topic1416029-1292-1.aspx
        'comm = New SqlCommand("SELECT Action FROM ConcessionTable where incidentID=@incidentID", conn) 'Corrective action
        'comm.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
        'comm.Parameters("@incidentID").Value = IncidentID
        'conn.Open()
        'Dim sqlresult1 As Object = comm.ExecuteScalar()
        'RadActCT.ResetCommentBox(sqlresult1.ToString)

        'conn.Close()

    End Sub

    Protected Sub AddEnergyItem()
        'from http://www.aspsnippets.com/Articles/Programmatically-add-items-to-DropDownList-on-Button-Click-in-ASPNet-using-C-and-VBNet.aspx
        'and http://www.yaldex.com/asp_tutorial/0596004877_progaspdotnet2-chp-5-sect-7.html
        DropDownListEnergy.Items.Clear()
        'This is a mad way of doing it but I don't know how to dim the energy list without constructing it at the same time
        Select Case LinacName
            Case "LA1"
                Dim Energy1() As String = {"Select", "6 MV", "6 MeV", "8 MeV", "10 MeV", "12 MeV", "15 MeV", "18 MeV", "20 MeV"}
                ConstructEnergylist(Energy1)
            Case "LA2", "LA3"
                Dim Energy1() As String = {"Select", "6 MV", "10 MV", "6 MeV", "8 MeV", "10 MeV", "12 MeV", "15 MeV", "18 MeV", "20 MeV"}
                ConstructEnergylist(Energy1)
            Case "LA4"
                Dim Energy1() As String = {"Select", "6 MV", "10 MV"}
                ConstructEnergylist(Energy1)
            Case "E1", "E2", "B1"
                Dim Energy1() As String = {"Select", "6 MV", "6 MV FFF", "10 MV", "10 MV FFF", "4 MeV", "6 MeV", "8 MeV", "10 MeV", "12 MeV", "15 MeV"}
                ConstructEnergylist(Energy1)
            Case Else
                'Don't show any energies
        End Select

    End Sub
    Protected Sub ConstructEnergylist(ByVal Energylist() As String)
        Dim energy() As String = Energylist
        Dim i As Integer
        For i = 0 To energy.GetLength(0) - 1
            DropDownListEnergy.Items.Add(New ListItem(energy(i)))
        Next
        DropDownListEnergy.SelectedIndex = -1
    End Sub
    Private Sub SaveRepeatFault_Click1(sender As Object, e As EventArgs) Handles SaveRepeatFault.Click

        Page.Validate("Repeat")
        If Page.IsValid Then
            SaveRepeatFaultbutton()
        End If
    End Sub
    Public Sub SaveRepeatFaultbutton()
        FaultParam = Application(FaultApplication)
        Dim UserInfo As String = String.Empty
        CreateFaultParams(FaultParam)
        Dim Result As Boolean = False

        Result = DavesCode.NewFaultHandling.InsertRepeatFault(FaultParam)
        If Result Then
            'This triggers UserApprovedEvent in ViewOpenFaults
            'RaiseEvent CloseRepeatFault(LinacName)
            RaiseEvent UpdateRepeatFault(LinacName)
        Else
            RaiseError()
        End If
    End Sub
    Protected Sub CreateFaultParams(ByRef FaultP As DavesCode.FaultParameters)
        If LinacName Like "T*" Then
            If (Not HttpContext.Current.Application(FaultDescriptionChanged) Is Nothing) Then
                Comment = HttpContext.Current.Application(FaultDescriptionChanged).ToString
            Else
                Comment = String.Empty
            End If

            If (Not HttpContext.Current.Application(RadActDescriptionChanged) Is Nothing) Then
                RadActComment = HttpContext.Current.Application(RadActDescriptionChanged).ToString
            Else
                RadActComment = String.Empty
            End If
            FaultP.Area = ErrorTextBox.Text
            FaultP.FaultDescription = Comment
            FaultP.RadAct = RadActComment
            FaultP.PatientID = PatientIDBoxT.Text
        Else
            If (Not HttpContext.Current.Application(FaultDescriptionChanged) Is Nothing) Then
                Comment = HttpContext.Current.Application(FaultDescriptionChanged).ToString
            Else
                Comment = String.Empty
            End If

            If (Not HttpContext.Current.Application(RadActDescriptionChanged) Is Nothing) Then
                RadActComment = HttpContext.Current.Application(RadActDescriptionChanged).ToString
            Else
                RadActComment = String.Empty
            End If
            Dim Energy As String = DropDownListEnergy.SelectedItem.Text
            If Energy = "Select" Then
                Energy = String.Empty
            End If
            FaultP.Energy = Energy
            FaultP.Area = AreaBox.Text
            FaultP.GantryAngle = GantryAngleBox.Text
            FaultP.CollimatorAngle = CollimatorAngleBox.Text
            FaultP.FaultDescription = Comment
            FaultP.RadAct = RadActComment
            FaultP.PatientID = PatientIDBox.Text

        End If
        'FaultP.SelectedIncident = IncidentID
        'FaultP.Linac = LinacName
        'FaultP.UserInfo = String.Empty
        FaultP.DateInserted = Now()
        'FaultP.ConcessionNumber = ConcessionN
        FaultP.RadioIncident = RadioIncident.SelectedItem.Value
        Application(FaultApplication) = FaultP
    End Sub

    Protected Sub RaiseError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Updating Fault. Please call Engineering');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(SaveRepeatFault, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub
    Protected Sub CancelFault_Click(sender As Object, e As EventArgs) Handles CancelFault.Click

        RaiseEvent UpdateRepeatFault(LinacName)
    End Sub
    Protected Sub ViewExistingFaults_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ViewExistingFaults.Click
        FaultParam = Application(FaultApplication)
        Dim objCon As UserControl = Page.LoadControl("ManyFaultGriduc.ascx")
        'CType(objCon, ManyFaultGriduc).NewFault = False
        CType(objCon, ManyFaultGriduc).IncidentID = FaultParam.SelectedIncident
        'to accomodate Tomo now need to pass equipment name?
        CType(objCon, ManyFaultGriduc).LinacName = FaultParam.Linac
        PlaceHolderVEF.Controls.Add(objCon)
        VEFUpdatePanel.Visible = True
    End Sub
End Class
