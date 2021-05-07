Imports System.Data.SqlClient
Imports System.Data

Partial Class controls_FaultTrackinguc
    Inherits System.Web.UI.UserControl

    'Private actionstate As String
    'Private technicalstate As String
    Private RadRow As DataTable
    Const CONCESSIONSELECTED As String = "CSelected"
    Const EMPTYSTRING As String = ""
    Const CONCESSION As String = "Concession"
    Const CLOSED As String = "Closed"
    Const NEWFAULT As String = "New"
    Const TECH As String = "Tech"
    Const OPEN As String = "Open"
    Private ConcessionDescriptionChanged As String
    Private ConcessionActionChanged As String
    Private ConcessionCommentChanged As String
    Private ParamApplication As String
    Private ConcessParamsTrial As DavesCode.ConcessionParameters = New DavesCode.ConcessionParameters()
    Private FaultParams As DavesCode.FaultParameters = New DavesCode.FaultParameters()
    Public Event UpdateClosedDisplays(ByVal EquipmentName As String)
    Public Event AddConcessionToDefectDropDownList(ByVal EquipmentName As String, ByVal incidentID As String)
    Public Event CloseFaultTracking(ByVal EquipmentName As String)
    Public Event UpdateOpenConcessions(ByVal EquipmentName As String)
    'Private faultstate As String
    Public Property LinacName() As String
    Public Property IncidentID As String
    Public Property ParentControl As String
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"
    Const LOCKELFSELECTED As String = "LockELFSelected"

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

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

        AddHandler WriteDatauc3.UserApproved, AddressOf UserApprovedEvent

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ConcessionDescriptionChanged = "ConcessionDescription" + LinacName
        ConcessionActionChanged = "ConcessionAction" + LinacName
        ConcessionCommentChanged = "ConcessionComment" + LinacName
        ParamApplication = "Params" + LinacName

        ConcessiondescriptionBoxC.BoxChanged = ConcessionDescriptionChanged
        ConcessionActionBox.BoxChanged = ConcessionActionChanged
        ConcessionCommentBox.BoxChanged = ConcessionCommentChanged
        Dim wrtctrl3 As WriteDatauc = CType(FindControl("WriteDatauc3"), WriteDatauc)
        wrtctrl3.LinacName = LinacName
        Select Case Me.DynamicControlSelection

            Case LOCKELFSELECTED
                Dim ObjLock As controls_UnLockElfuc = Page.LoadControl("controls\UnLockElfuc.ascx")

                ObjLock.LinacName = LinacName
                ObjLock.UserReason = ParentControl
                AddHandler ObjLock.HideUnlockPopUp, AddressOf HideUnlockElf
                'ObjLock.ID = "LockElf1"

                LockELFPlaceholder.Controls.Add(ObjLock)
                LockELFModalPopup.Show()


            Case Else
                'no dynamic controls need to be loaded...yet
        End Select

    End Sub


    Public Sub InitialiseFaultTracking(ByVal ConcessObject As DavesCode.ConcessionParameters)

        Application(ParamApplication) = ConcessObject
        SetupStatusTech(ConcessObject.IncidentID)
        Select Case ConcessObject.PresentFaultState
            Case NEWFAULT, OPEN
                SetUpOriginalFault(ConcessObject.IncidentID)
                MultiView_NewFaultConcessionDisplay.SetActiveView(NewFaultView)

            Case CONCESSION
                ConcessionHistoryuc1.BindConcessionHistoryGrid(ConcessObject.IncidentID)
                MultiView_NewFaultConcessionDisplay.SetActiveView(ConcessionHistoryView)
                FaultOptionList.Items.FindByValue("Open").Enabled = False
        End Select


        SaveAFault.Enabled = False
        CCommentPanel.Enabled = False
        CActionPanel.Enabled = False
        CDescriptionPanel.Enabled = False

        BindTrackingGrid(ConcessObject.IncidentID)

    End Sub
    Protected Sub BindTrackingGrid(ByVal IncidentID As String)
        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString,
            .SelectCommand = "select t.TrackingID, t.trackingcomment, t.AssignedTo,  t.LastUpDatedBy, t.LastUpDatedOn from FaultTracking t where t.incidentID=@incidentID order by t.TrackingID asc"
        }

        SqlDataSource1.SelectParameters.Add("@incidentID", System.Data.SqlDbType.Int)
        SqlDataSource1.SelectParameters.Add("incidentID", IncidentID)
        TrackingGrid.DataSource = SqlDataSource1
        TrackingGrid.DataBind()
    End Sub

    Protected Sub SetUpOriginalFault(ByVal incidentID As String)

        Dim objOriginalFault As UserControl = Page.LoadControl("ManyFaultGriduc.ascx")

        CType(objOriginalFault, ManyFaultGriduc).NewFault = True
        CType(objOriginalFault, ManyFaultGriduc).IncidentID = incidentID
        'to accomodate Tomo now need to pass equipment name?
        CType(objOriginalFault, ManyFaultGriduc).LinacName = LinacName
        PlaceHolderFaults.Controls.Add(objOriginalFault)


    End Sub


    Protected Sub SetupStatusTech(ByVal incidentID As String)
        Dim FaultDescription As New List(Of String)
        ConcessParamsTrial = Application(ParamApplication)
        FaultOptionList.SelectedIndex = -1
        AssignedToList.SelectedIndex = -1
        StatusLabel1.Text = ConcessParamsTrial.PresentFaultState
        ConcessiondescriptionBoxC.ResetCommentBox(ConcessParamsTrial.ConcessionDescription)
        ConcessionActionBox.ResetCommentBox(ConcessParamsTrial.ConcessionAction)
        'Make this default to Select now

        'AssignedToList.SelectedIndex = AssignedToList.Items.IndexOf(AssignedToList.Items.FindByText(ConcessParamsTrial.AssignedTo))
        'If ConcessParamsTrial.PresentFaultState = "Concession" Then
        '    FaultOptionList.Items.FindByValue("Open").Enabled = False

        If ConcessParamsTrial.PresentFaultState = NEWFAULT Then
            CancelButton.Enabled = False
        ElseIf ConcessParamsTrial.PresentFaultState = OPEN Then
            CancelButton.Enabled = False
        End If

        'Dim lockctrl As LockElfuc = CType(FindControl("LockElfuc1"), LockElfuc)
        'lockctrl.LinacName = LinacName

    End Sub


    Protected Sub FaultOptionList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles FaultOptionList.SelectedIndexChanged
        Dim updateFaultStatus As String
        Dim incidentid As String
        ConcessParamsTrial = Application(ParamApplication)
        Dim CDescriptionstring As String = ConcessParamsTrial.ConcessionDescription
        Dim AssignedTo As String = String.Empty
        updateFaultStatus = FaultOptionList.SelectedItem.Value
        incidentid = ConcessParamsTrial.IncidentID

        If Not updateFaultStatus = "Select" Then
            SaveAFault.Enabled = True
            SaveAFault.BackColor = Drawing.Color.Yellow
            CCommentPanel.Enabled = True
            ConcessParamsTrial.FutureFaultState = updateFaultStatus
            AssignedTo = AssignedToList.SelectedItem.Text

            ConcessParamsTrial.AssignedTo = AssignedTo
            If updateFaultStatus = "Concession" Then

                CActionPanel.Enabled = True
                If String.IsNullOrEmpty(CDescriptionstring) Then
                    CDescriptionPanel.Enabled = True
                End If
            Else
                CActionPanel.Enabled = False

            End If

        Else
            CDescriptionPanel.Enabled = False
            CActionPanel.Enabled = False
            CCommentPanel.Enabled = False
            SaveAFault.Enabled = False
            SaveAFault.BackColor = Drawing.Color.LightGray
            AssignedToList.SelectedIndex = -1
        End If
        'Basically puts new selected value into concessparams and Application
        Application(ParamApplication) = ConcessParamsTrial

    End Sub

    Protected Sub AssignedToList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles AssignedToList.SelectedIndexChanged
        ConcessParamsTrial = Application(ParamApplication)

        Dim AssignedTo As String = String.Empty
        AssignedTo = AssignedToList.SelectedItem.Text
        ConcessParamsTrial.AssignedTo = AssignedTo

    End Sub

    Protected Sub SetValidationControls(ByVal SetReset As String)
        If SetReset = "Reset" Then
            ConcessiondescriptionBoxC.SetValidation("", "")
            ConcessionActionBox.SetValidation("", "")
            ConcessionCommentBox.SetValidation("", "")
        Else
            Select Case SetReset

                Case CONCESSION
                    'Concessiondescription validator doesn't work if panel is disabled
                    If CDescriptionPanel.Enabled = True Then
                        ConcessiondescriptionBoxC.SetValidation("faulttracking", "Please Enter a Concession description")
                    End If
                    ConcessionActionBox.SetValidation("faulttracking", "Please Enter the Corrective Action")
                Case "Open"
                    If CCommentPanel.Enabled = True Then
                        ConcessionCommentBox.SetValidation("faulttracking", "Please Enter a Comment")
                    End If
            End Select
        End If
    End Sub

    Protected Sub SaveAFault_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SaveAFault.Click
        Dim validator As System.Web.UI.WebControls.BaseValidator
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc3"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)

        Dim strScript As String = "<script>"
        Dim incidentid As String
        'Dim UniqueC As Boolean
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        Dim SetValidation As String
        SetValidation = "Reset"
        SetValidationControls(SetValidation)
        SetValidation = FaultOptionList.SelectedItem.Text
        SetValidationControls(SetValidation)

        'this is concessparamstrial.incidentid
        incidentid = ConcessParamsTrial.IncidentID

        'This must be different validate key to others on the page!
        Page.Validate("faulttracking")
        If Page.IsValid Then

            wcbutton.Text = "Saving Fault Status"
            Session.Add("Actionstate", "Confirm")
            wctrl.Visible = True
            ForceFocus(wctext)
            SetValidationControls("Reset")

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

    Protected Sub UserApprovedEvent(ByVal Tabused As String, ByVal Userinfo As String)
        'faultstate = "OpenFault" + LinacName
        'actionstate = "ActionState" + LinacName
        'technicalstate = "techstate" + LinacName
        Dim TRACKINGID As Integer = 0

        Dim incidentID As String
        Dim Machine As String
        'Application(technicalstate) = Nothing
        CreateConcessParams(Userinfo)

        Machine = ConcessParamsTrial.Linac
        Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
        HttpContext.Current.Session.Remove("Actionstate")



        If Tabused = "incident" Then
            incidentID = ConcessParamsTrial.IncidentID
            'This stops it popping up again when it shouldn't

            Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc3"), WriteDatauc)
            wctrl.Visible = False

            If Action = "Confirm" Then
                Dim exists As Integer

                If incidentID <> 0 Then
                    'need to check if concession is new or not

                    If ConcessParamsTrial.FutureFaultState = "Concession" Then
                        'if new concession and everything works then exists = 0 so write new concession and tracking and skip to end
                        'if there is a problem then roll back and skip to end via -1 (what about rad row etc?)
                        'if exists = 1 or not 0 or -1 then concession already exists so update tracking. If it gets to there then insertnewconcession has worked
                        'ie returned exists = 1 or not 0 or -1 but hasn't done anything else. If concession updated ok but otherwise update tracking

                        If String.IsNullOrEmpty(ConcessParamsTrial.ConcessionNumber) Then

                            exists = DavesCode.NewFaultHandling.InsertNewConcession(ConcessParamsTrial)

                            If exists = -1 Then
                                RaiseError()
                            Else
                                Dim Returnstatus As String = DavesCode.NewFaultHandling.ReturnLastNonFaultState(incidentID)
                                Me.Page.GetType.InvokeMember("Updatestatedisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {Returnstatus})
                                UpdateState(Machine, Returnstatus, ParentControl, Userinfo)
                                RaiseEvent UpdateOpenConcessions(Machine)
                                RaiseEvent CloseFaultTracking(Machine)
                            End If
                        Else
                            TRACKINGID = DavesCode.NewFaultHandling.UpdateTracking(ConcessParamsTrial)
                            If TRACKINGID = -1 Then
                                RaiseError()
                            Else
                                RaiseEvent CloseFaultTracking(Machine)
                            End If
                        End If
                        Application(ParamApplication) = Nothing
                    Else
                        'Now modified to deal with state of Open fault
                        TRACKINGID = DavesCode.NewFaultHandling.UpdateTracking(ConcessParamsTrial)

                        If TRACKINGID = -1 Then
                            RaiseError()
                            Application(ParamApplication) = Nothing
                        Else
                            If ConcessParamsTrial.FutureFaultState = "Closed" Then
                                'Application(faultstate) = False
                                'introduce new set machine state so display is reset to previous state but only if not previously concession
                                'need to know what linac state was when fault was raised too. set lock to true so activity is not stopped
                                If Not ConcessParamsTrial.PresentFaultState = "Concession" Then
                                    Dim Returnstatus As String = DavesCode.NewFaultHandling.ReturnLastNonFaultState(incidentID)
                                    Me.Page.GetType.InvokeMember("Updatestatedisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {Returnstatus})
                                    UpdateState(Machine, Returnstatus, ParentControl, Userinfo)
                                End If
                                RaiseEvent UpdateClosedDisplays(Machine)
                                RaiseEvent CloseFaultTracking(Machine)
                                Application(ParamApplication) = Nothing
                            Else
                                'reset things?

                                StatusLabel1.Text = ConcessParamsTrial.FutureFaultState
                                ConcessionCommentBox.ResetCommentBox(EMPTYSTRING)
                                AssignedToList.SelectedIndex = -1
                                BindTrackingGrid(incidentID)

                            End If

                        End If

                    End If
                Else
                    RaiseError()
                    RaiseEvent CloseFaultTracking(Machine)
                End If

            End If
        End If
    End Sub

    Protected Sub UpdateState(ByVal linac As String, ByVal ReturnState As String, ParentControl As Integer, ByVal closingName As String)

        Dim time As DateTime
        time = Now()
        'Const REASON As Integer = 99

        Dim LinacStatusID As Integer
        Dim usergroupselected As Integer = DavesCode.Reuse.GetRole(closingName)
        Dim conn As SqlConnection
        Dim LoggedOn As Boolean = True
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        Dim Machinestatus As SqlCommand

        conn = New SqlConnection(connectionString)


        Machinestatus = New SqlCommand("INSERT INTO LinacStatus (state, DateTime, usergroup,userreason,linac, UserName, LoggedOn ) " &
                                    "VALUES (@state, @Datetime, @usergroup, @userreason, @linac, @UserName, @LoggedOn) SELECT SCOPE_IDENTITY()", conn)
        Machinestatus.Parameters.Add("@state", System.Data.SqlDbType.NVarChar, 50)
        Machinestatus.Parameters("@state").Value = ReturnState
        Machinestatus.Parameters.Add("@DateTime", System.Data.SqlDbType.DateTime)
        Machinestatus.Parameters("@DateTime").Value = time
        Machinestatus.Parameters.Add("@usergroup", System.Data.SqlDbType.Int)
        Machinestatus.Parameters("@usergroup").Value = usergroupselected
        Machinestatus.Parameters.Add("@userreason", System.Data.SqlDbType.Int)
        Machinestatus.Parameters("@userreason").Value = ParentControl
        Machinestatus.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
        Machinestatus.Parameters("@linac").Value = LinacName
        Machinestatus.Parameters.Add("@UserName", System.Data.SqlDbType.NVarChar, 50)
        Machinestatus.Parameters("@UserName").Value = closingName
        Machinestatus.Parameters.Add("@LoggedOn", System.Data.SqlDbType.Bit)
        Machinestatus.Parameters("@LoggedOn").Value = LoggedOn

        Try
            'To get the identity of the record just inserted from
            'http://www.aspsnippets.com/Articles/Return-Identity-Auto-Increment-Column-value-after-record-insert-in-SQL-Server-Database-using-ADONet-with-C-and-VBNet.aspx
            conn.Open()
            'commstatus.ExecuteNonQuery()

            Dim obj As Object = Machinestatus.ExecuteScalar()
            'Dim LinacStatusIDs As String = obj.ToString()
            LinacStatusID = CInt(obj)
            conn.Close()
            'This creates in the Activity table the entry for the start of an activity so long as it is not as a result of switching the user.

        Catch ex As Exception
            DavesCode.NewFaultHandling.LogError(ex)
        Finally
            conn.Close()
        End Try
    End Sub

    Protected Sub RaiseError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Updating Fault. Please call Engineer');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(SaveAFault, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Sub CreateConcessParams(ByVal UserInfo As String)
        ConcessParamsTrial = Application(ParamApplication)
        Dim ConcessionDescription As String
        Dim ConcessionAction As String
        Dim ConcessionComment As String
        Dim FaultState As String


        If (Not HttpContext.Current.Application(ConcessionDescriptionChanged) Is Nothing) Then
            ConcessionDescription = HttpContext.Current.Application(ConcessionDescriptionChanged).ToString
        Else
            ConcessionDescription = String.Empty
        End If

        If (Not HttpContext.Current.Application(ConcessionActionChanged) Is Nothing) Then
            ConcessionAction = HttpContext.Current.Application(ConcessionActionChanged).ToString
        Else
            ConcessionAction = String.Empty
        End If

        If (Not HttpContext.Current.Application(ConcessionCommentChanged) Is Nothing) Then
            ConcessionComment = HttpContext.Current.Application(ConcessionCommentChanged).ToString
        Else
            ConcessionComment = String.Empty
        End If

        FaultState = FaultOptionList.SelectedItem.Text
        ConcessParamsTrial.UserInfo = UserInfo
        ConcessParamsTrial.ConcessionAction = ConcessionAction
        ConcessParamsTrial.FutureFaultState = FaultState
        ConcessParamsTrial.ConcessionComment = ConcessionComment
        ConcessParamsTrial.ConcessionDescription = ConcessionDescription

        Application(ParamApplication) = ConcessParamsTrial
    End Sub


    Protected Sub ClearActionButton_Click(sender As Object, e As EventArgs) Handles ClearActionButton.Click
        ConcessParamsTrial = Application(ParamApplication)
        ConcessionActionBox.ResetCommentBox(EMPTYSTRING)
        ConcessParamsTrial.ConcessionAction = EMPTYSTRING
        Application(ParamApplication) = ConcessParamsTrial
        ConcessionHistoryuc1.BindConcessionHistoryGrid(ConcessParamsTrial.IncidentID)

    End Sub
    Protected Sub ClearCommentButton_Click(sender As Object, e As EventArgs) Handles ClearCommentButton.Click
        ConcessParamsTrial = Application(ParamApplication)
        ConcessionCommentBox.ResetCommentBox(EMPTYSTRING)
        ConcessParamsTrial.ConcessionComment = EMPTYSTRING

        Application(ParamApplication) = ConcessParamsTrial

        ConcessionHistoryuc1.BindConcessionHistoryGrid(ConcessParamsTrial.IncidentID)

    End Sub

    Protected Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click
        ConcessParamsTrial = Application(ParamApplication)
        RaiseEvent CloseFaultTracking(ConcessParamsTrial.Linac)

    End Sub

    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" + ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

    Protected Sub FormError()
        Dim strScript As String = "<script>"
        strScript += "alert('Please Correct Form Errors');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(SaveAFault, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub
    Protected Sub LogElf_Click(sender As Object, e As EventArgs) Handles LogElf.Click

        LockELFModalPopup.Hide()
        Dim username As String = "Lockuser"

        Dim comment As String = String.Empty
        Dim grdview As GridView
        Dim grdviewI As GridView
        Dim tabused As Integer = ParentControl
        Dim radioselect As Integer = 7
        Dim success As Boolean = False
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        tabused = ParentControl
        'has to be tablable to cope with either tab 1 or 9 control
        Select Case tabused
            Case 1, 9
                success = DavesCode.NewEngRunup.CommitRunup(grdview, grdviewI, LinacName, 1, "Lockuser", comment, False, False, True, FaultParams)
            Case 4, 5
                If DavesCode.NewFaultHandling.CheckForOpenFault(LinacName) Then
                    tabused = 5
                    success = DavesCode.NewWriteAux.WriteAuxTables(LinacName, username, comment, radioselect, tabused, True, True, FaultParams)
                Else
                    success = DavesCode.NewWriteAux.WriteAuxTables(LinacName, username, comment, radioselect, tabused, False, True, FaultParams)
                End If
        End Select


        If success Then
            Dim ObjLock As controls_UnLockElfuc = Page.LoadControl("controls\UnLockElfuc.ascx")
            ObjLock.LinacName = LinacName
            ObjLock.UserReason = ParentControl
            AddHandler ObjLock.HideUnlockPopUp, AddressOf HideUnlockElf

            LockELFPlaceholder.Controls.Add(ObjLock)
            DynamicControlSelection = LOCKELFSELECTED
            LockELFModalPopup.Show()

        Else
            RaiseLockError()
        End If
    End Sub
    Public Sub HideUnlockElf(ByVal Hide As Boolean)
        If Hide Then
            LockELFModalPopup.Hide()
            DynamicControlSelection = String.Empty
        Else
            LockELFModalPopup.Show()
            DynamicControlSelection = LOCKELFSELECTED
        End If
    End Sub
    Protected Sub RaiseLockError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Locking Elf. Please inform system administator');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(LogElf, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Sub TrackingGrid_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles TrackingGrid.PageIndexChanging

        Dim IncidentID As String
        IncidentID = TrackingGrid.DataKeys(0).Value.ToString
        TrackingGrid.PageIndex = e.NewPageIndex
        BindTrackingGrid(IncidentID)

    End Sub
End Class
