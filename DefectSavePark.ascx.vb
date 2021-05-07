Imports System.Data.SqlClient
Imports System.Data

Partial Class DefectSavePark
    Inherits System.Web.UI.UserControl

    Const RADIO As Integer = 103
    Const REPAIR As Integer = 5
    Private time As DateTime
    Const RecoverableFault As String = "Recoverable Fault"
    Const UnRecoverableFault As String = "UnRecoverable Fault"
    Const OtherFault As String = "Other Fault"
    Const UnRecoverableID As Integer = -23
    Const OtherfaultID As Integer = -25
    Const FaultAnswerNo As String = "No"
    Const FaultAnswerYes As String = "Yes"
    Const RadioIncidentAnswerYes As String = "Yes"
    Const RadioIncidentAnswerNo As String = "No"
    Const Concession As String = "Concession"
    Const Closed As String = "Closed"
    Const Radiographer As Integer = 3
    Const EMPTYSTRING As String = ""
    Const SELECTED As String = "Select"
    Private Valid As Boolean = False
    Dim ConcessionNumber As String = ""
    Private FaultDescriptionChanged As String
    Private RadActDescriptionChanged As String
    Private Comment As String
    Private RadActComment As String
    Dim SelectedIncident As Integer = 0
    Public Property LinacName() As String
    Public Property ParentControl() As String
    Public Property ParentControlComment() As String
    Public Event UpDateDefectDailyDisplay(ByVal EquipmentName As String)
    Public Event CloseReportFaultPopUp(ByVal EquipmentName As String, ByVal ErrorStatus As Boolean)
    Public Event UpdateViewOpenFaults(ByVal EquipmentName As String)
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Remove reference to this as no longer used after March 2016 done on 23/11/16
        'Added back in 26/3/18 see SPR
        AddHandler WriteDatauc1.UserApproved, AddressOf UserApprovedEvent

        FaultDescriptionChanged = "defectFault" + LinacName
        RadActDescriptionChanged = "radact" + LinacName
    End Sub

    Public Sub InitialiseDefectPage()
        Dim newFault As Boolean
        FaultDescription.BoxChanged = FaultDescriptionChanged
        RadActC.BoxChanged = RadActDescriptionChanged
        newFault = False
        SetFaults(newFault)
        RadioIncident.SelectedIndex = -1
        Dim clear As Button = FindControl("ClearButton")

    End Sub

    'No need to pass any references now or to have if statements. Analysis 23/11/16 Back in 29/03/18

    Protected Sub UserApprovedEvent(ByVal Tabused As String, ByVal Userinfo As String)
        Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
        HttpContext.Current.Session.Remove("Actionstate")

        Dim Result As Boolean = False
        ConcessionNumber = Defect.SelectedItem.ToString
        If ConcessionNumber.Contains("ELF") Then
            ConcessionNumber = Left(ConcessionNumber, 7)
        End If
        If Tabused = "Defect" Then
            Dim wctrl As WriteDatauc = CType(FindControl("WriteDatauc1"), WriteDatauc)
            wctrl.Visible = False
            If Action = "Confirm" Then
                Result = NewWriteRadReset(Userinfo, ConcessionNumber)
                'if there is a problem with result then when the Closereportfaultpopup event is raised it will pop up an error box.
                If Result Then
                    ClearsForm()
                    RaiseEvent CloseReportFaultPopUp(LinacName, Result)
                    RaiseEvent UpdateViewOpenFaults(LinacName)
                End If
            Else
                Result = True
                'This ensures that the pop up always closes successfully if the save is cancelled

            End If

        End If
    End Sub

    Protected Sub SaveDefectButton_Click(sender As Object, e As System.EventArgs) Handles SaveDefectButton.Click
        'Dim b As Button = TryCast(sender, Button)
        'Dim i As RadioButtonList
        'If b Is Nothing Then
        '    i = CType(sender, RadioButtonList)
        'End If
        If Defect.SelectedItem.Text = RecoverableFault Then
            FaultDescription.SetValidation("Tomodefect", "Please Enter a fault description")

        End If

        Page.Validate("Tomodefect")
        If Page.IsValid Then
            'If (TypeOf sender Is Button) Then
            Session.Add("ActionState", "Confirm")

            UserApprovedEvent("Defect", "")
        Else
            For Each validator In Page.Validators
                If (Not validator.IsValid) Then
                    'validator that failed found so set the focus to the control
                    'it validates and exit the loop
                    ForceFocus(validator.FindControl(validator.ControlToValidate))
                    Exit For
                End If
            Next validator

        End If

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        SaveDefectButton.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(SaveDefectButton, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        ClearButton.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(ClearButton, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")

        'WriteDatauc1 no longer used 23/11/16
        'Added back in for RAD RESET 26/3/18 SEE SPR
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        wctrl.LinacName = LinacName
        FaultDescription.BoxChanged = FaultDescriptionChanged
        RadActC.BoxChanged = RadActDescriptionChanged

    End Sub
    Public Sub ResetDefectDropDown(ByVal incidentid As String)

        Dim result As ListItem
        Dim newFault As Boolean

        result = Defect.Items.FindByValue(incidentid)
        Dim index As Integer
        index = Defect.Items.IndexOf(result)
        If index > 0 Then
            Defect.Items.RemoveAt(index)
            UpdatePanelDefectlist.Update()
        Else
            newFault = True
            SetFaults(newFault)
        End If
    End Sub
    Private Sub SetFaults(ByVal newfault As Boolean)

        Dim selectCommand As Boolean = newfault
        Dim Faults As New DataTable()
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        'formatting has to change between vs versions
        Dim connectionString1 As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        conn = New SqlConnection(connectionString1)
        If newfault Then
            comm = New SqlCommand("SELECT ConcessionNumber + ' ' + ConcessionDescription As Fault, IncidentID FROM [ConcessionTable] where linac=@linac and ConcessionActive = 'TRUE' and IncidentID = (Select max(IncidentID) from  [ConcessionTable] where linac=@linac and ConcessionActive = 'TRUE') order by ConcessionNumber", conn)
            comm.Parameters.AddWithValue("@Linac", LinacName)
        Else

            comm = New SqlCommand("SELECT  Defect as Fault, IncidentID From [DefectTable] where linacType in('T') and Active = 'True' UNION SELECT ConcessionNumber + ' ' + ConcessionDescription As Fault, IncidentID FROM [ConcessionTable] where linac=@linac and ConcessionActive = 'TRUE' order by IncidentID", conn)
            comm.Parameters.AddWithValue("@Linac", LinacName)

        End If
        'Don't need to open https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/populating-a-dataset-from-a-dataadapter
        'conn.Open()
        Dim da As New SqlDataAdapter(comm)
        da.Fill(Faults)

        Defect.DataSource = Faults
        Defect.DataTextField = "Fault"
        Defect.DataValueField = "IncidentID"
        Defect.DataBind()

    End Sub

    Protected Function QuerySqlConnection(ByVal MachineName As String, ByVal query As String) As SqlDataSource
        'This uses the sqldatasource instead of the individual conn definitions so reader can't be used
        'this solution is from http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.sqldatasource.select%28v=vs.90%29.aspx

        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString,
            .SelectCommand = (query)
        }
        SqlDataSource1.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", MachineName)
        Return SqlDataSource1

    End Function

    Protected Sub Defect_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles Defect.SelectedIndexChanged
        'Beef up error handling
        Dim incidentIDstring As String = ""
        Dim incidentID As Integer
        Dim conn1 As SqlConnection
        Dim comm1 As SqlCommand
        Dim DefectString As String = ""
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        'ClearsForm()
        incidentIDstring = Defect.SelectedItem.Value
        DefectString = Defect.SelectedItem.Text
        Dim result As ListItem
        result = Defect.Items.FindByValue(incidentIDstring)
        Dim index As Integer
        index = Defect.Items.IndexOf(result)
        If incidentIDstring = SELECTED Then
            ClearsForm()
        Else
            If Integer.TryParse(incidentIDstring, incidentID) Then

                SelectedIncidentID.Value = incidentID
                TimeFaultSelected.Value = Now().ToString

                conn1 = New SqlConnection(connectionString)
                If incidentID > 0 Then 'concession
                    comm1 = New SqlCommand("SELECT Action FROM ConcessionTable where incidentID=@incidentID", conn1) 'Corrective action

                    comm1.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                    comm1.Parameters("@incidentID").Value = incidentID

                    conn1.Open()

                    Dim sqlresult1 As Object = comm1.ExecuteScalar()

                    RadActC.ResetCommentBox(sqlresult1.ToString)

                    FaultClosedLabel.Visible = False
                    FaultOpenClosed.Visible = False
                    UnRecoverableSave.Visible = False
                    SaveDefectButton.Visible = True
                    SaveDefectButton.Enabled = True
                    SaveDefectButton.BackColor = Drawing.Color.Yellow
                    FaultPanel.Enabled = True
                    conn1.Close()

                ElseIf Not String.IsNullOrEmpty(DefectString) Then
                    'change to make other fault act like unrecoverable fault LA 21/05/20
                    'change if statement to case
                    Select Case DefectString
                        Case RecoverableFault
                            FaultClosedLabel.Visible = False
                            FaultOpenClosed.Visible = False
                            UnRecoverableSave.Visible = False
                            SaveDefectButton.Visible = True
                            SaveDefectButton.Enabled = True
                            SaveDefectButton.BackColor = Drawing.Color.Yellow
                            FaultPanel.Enabled = True
                            ActPanel.Enabled = False
                            RadioIncident.ClearSelection()
                        Case UnRecoverableFault, OtherFault
                            FaultClosedLabel.Visible = True
                            FaultOpenClosed.Visible = True
                            FaultOpenClosed.ClearSelection()
                            SaveDefectButton.Visible = False
                            UnRecoverableSave.Visible = False
                            'FaultTypeSave.SetActiveView(UnRecoverableView)
                            ActPanel.Enabled = False
                            FaultPanel.Enabled = True
                            RadioIncident.ClearSelection()
                        Case "Select"
                            ClearsForm()
                        Case Else
                            ClearsForm()
                            RaiseError()

                    End Select


                End If

            Else
                ClearsForm()
                RaiseError()
            End If
        End If

    End Sub

    Protected Sub ClearButton_Click(sender As Object, e As System.EventArgs) Handles ClearButton.Click

        RaiseEvent CloseReportFaultPopUp(LinacName, True)
    End Sub

    Protected Sub ClearsForm()

        FaultOpenClosed.ClearSelection()
        RadioIncident.ClearSelection()
        ErrorCode.Text = Nothing
        Accuray.Text = Nothing
        FaultDescription.ResetCommentBox(EMPTYSTRING)
        PatientIDBox.Text = Nothing
        RadActC.ResetCommentBox(EMPTYSTRING)
        SaveDefectButton.BackColor = Drawing.Color.LightGray
        SaveDefectButton.Enabled = False
        UnRecoverableSave.Visible = False
        FaultPanel.Enabled = False
        ActPanel.Enabled = False
        Defect.SelectedIndex = -1
    End Sub

    Function NewWriteRadReset(ByVal UserInfo As String, ByVal ConcessionNumber As String) As Boolean

        FaultDescriptionChanged = "defectFault" + LinacName
        RadActDescriptionChanged = "radact" + LinacName
        Dim Result As Boolean = False
        Dim usergroupselected As Integer = 0
        Dim FaultSelected As String = EMPTYSTRING
        Dim GridViewEnergy As GridView
        Dim GridViewImage As GridView
        Dim grdviewI As GridView = Me.Parent.FindControl("GridViewImage")
        Dim Status As String = EMPTYSTRING
        Dim FaultParams As DavesCode.FaultParameters = New DavesCode.FaultParameters()
        SelectedIncident = SelectedIncidentID.Value
        CreateFaultParams(UserInfo, FaultParams)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString

        If (SelectedIncident = UnRecoverableID) Or (SelectedIncident = OtherfaultID) Then
            usergroupselected = DavesCode.Reuse.GetRole(UserInfo)
            FaultSelected = FaultOpenClosed.SelectedItem.Text

            Select Case FaultSelected
                Case FaultAnswerYes

                    If usergroupselected.Equals(Radiographer) Then
                        Result = DavesCode.NewFaultHandling.InsertNewFault("Concession", FaultParams)
                        If Result Then
                            Status = Concession
                            SetFaults(True)
                            RaiseEvent UpdateViewOpenFaults(LinacName)

                        End If
                    Else
                        Result = DavesCode.NewFaultHandling.InsertNewFault("Closed", FaultParams)
                        If Result Then
                            RaiseEvent UpDateDefectDailyDisplay(LinacName)

                        End If
                    End If
                    'Unrecoverable fault isn't closed neither is other fault
                Case FaultAnswerNo

                    'Write equivalent of report fault assume only one fault at a time at the moment
                    'Close current tab

                    Dim ParentControlComment As String = Application("TabComment")
                    Dim iView As Boolean = False
                    Dim XVI As Boolean = False


                    Select Case ParentControl

                        Case 1, 9
                            Result = DavesCode.NewEngRunup.CommitRunup(GridViewEnergy, GridViewImage, LinacName, ParentControl, UserInfo, ParentControlComment, Valid, True, False, FaultParams)

                        Case 2
                            DavesCode.Reuse.ReturnImaging(iView, XVI, grdviewI, LinacName)
                            Result = DavesCode.NewPreClinRunup.CommitPreClin(LinacName, UserInfo, ParentControlComment, iView, XVI, Valid, True, FaultParams)
                        Case 3
                            Result = DavesCode.NewCommitClinical.CommitClinical(LinacName, UserInfo, True, FaultParams, False)


                        Case 4, 5, 6, 8
                            Result = DavesCode.NewWriteAux.WriteAuxTables(LinacName, UserInfo, ParentControlComment, RADIO, ParentControl, True, False, FaultParams)

                        Case Else
                            Result = False

                    End Select

                    If Result Then

                        Session.Add("name", UserInfo)
                        Session.Add("usergroup", usergroupselected)
                        Session.Add("userreason", REPAIR)
                        Dim returnstring As String = LinacName + "page.aspx?TabAction=Fault&NextTab="
                        Response.Redirect(returnstring & ParentControl & "&comment=" & "")

                    End If
                Case Else
            End Select

        Else 'This is a recoverable fault - So won't have concession number?

            Result = DavesCode.NewFaultHandling.InsertRepeatFault(FaultParams)

            If Result Then
                RaiseEvent UpDateDefectDailyDisplay(LinacName)
            Else


            End If
        End If
        Defect.SelectedIndex = -1
        Return Result
    End Function

    Protected Sub CreateFaultParams(ByVal UserInfo As String, ByRef FaultParams As DavesCode.FaultParameters)
        Dim laststate As String = String.Empty
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
        laststate = DavesCode.Reuse.GetLastState(LinacName, 0)
        If laststate = "Clinical" Then
            laststate = "Suspended"
        End If
        FaultParams.SelectedIncident = SelectedIncident
        FaultParams.Linac = LinacName
        FaultParams.DateInserted = DateTime.Parse(TimeFaultSelected.Value)
        FaultParams.UserInfo = UserInfo
        FaultParams.Area = Accuray.Text
        FaultParams.Energy = ErrorCode.Text
        FaultParams.GantryAngle = EMPTYSTRING
        FaultParams.CollimatorAngle = EMPTYSTRING
        FaultParams.PatientID = PatientIDBox.Text
        FaultParams.FaultDescription = Comment
        FaultParams.ConcessionNumber = ConcessionNumber
        FaultParams.RadAct = RadActComment
        FaultParams.RadioIncident = RadioIncident.SelectedItem.Value
        FaultParams.Activity = ParentControl
        FaultParams.LastState = laststate

    End Sub

    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" + ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

    Protected Sub FaultOpenClosed_SelectedIndexChanged(sender As Object, e As EventArgs) Handles FaultOpenClosed.SelectedIndexChanged
        Dim Selected As String = ""
        Selected = FaultOpenClosed.SelectedItem.Text
        Select Case Selected
            Case "Yes"
                ActPanel.Enabled = True
                RadActC.SetValidation("Tomodefect", "Please Enter the Corrective Action Taken")
                UnRecoverableSave.Visible = True
                UnRecoverableSave.Enabled = True
                UnRecoverableSave.BackColor = Drawing.Color.Yellow
            Case "No"
                RadActC.SetValidation("", "")
                ActPanel.Enabled = False
                UnRecoverableSave.Visible = True
                UnRecoverableSave.Enabled = True
                UnRecoverableSave.BackColor = Drawing.Color.Yellow
            Case Else
        End Select

    End Sub
    Protected Sub UnRecoverableSave_Click(sender As Object, e As EventArgs) Handles UnRecoverableSave.Click
        Dim wctrl As WriteDatauc = CType(FindControl("WriteDatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        Dim Selected As String = ""

        FaultDescription.SetValidation("Tomodefect", "Please Enter a fault description")

        Selected = FaultOpenClosed.SelectedItem.Text
        If Selected.Equals(FaultAnswerNo) Then
            AccurayValidation.Enabled = False
        Else
            'changed to false for now as unrecoverable doesn't seem to have to have a physicist approval
            AccurayValidation.Enabled = False
        End If
        Page.Validate("Tomodefect")
        If Page.IsValid Then

            UnRecoverableSave.BackColor = Drawing.Color.LightGray
            UnRecoverableSave.Enabled = False

            If Selected.Equals(FaultAnswerNo) Then
                wcbutton.Text = "Saving New Fault"
                Session.Add("ActionState", "Confirm")
                wctrl.Visible = True
                ForceFocus(wctext)
                'Open equivalent of fault
            ElseIf Selected.Equals(FaultAnswerYes) Then
                'Write equivalent of radreset

                wcbutton.Text = "Closing Fault"
                Session.Add("ActionState", "Confirm")

                wctrl.Visible = True
                ForceFocus(wctext)
            Else
                'Error action
            End If
            FaultDescription.SetValidation("", "")
            RadActC.SetValidation("", "")
        End If
    End Sub
    Protected Sub RaiseError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Updating Fault. Please call Engineering');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(ClearButton, Me.GetType(), "JSCR", (strScript.ToString()), False)

    End Sub
End Class
