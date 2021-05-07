Imports System.Data.SqlClient

Partial Class ErunupUserControl
    Inherits UserControl

    Private mpContentPlaceHolder As ContentPlaceHolder
    Private Todaydefect As DefectSave
    Private TodaydefectPark As DefectSavePark
    Private MainFaultPanel As controls_MainFaultDisplayuc
    Private BoxChanged As String
    Public Event BlankGroup(ByVal BlankUser As Integer)
    Dim comment As String
    Private Obpage As Page
    Dim FaultParams As DavesCode.FaultParameters = New DavesCode.FaultParameters()
    Const ENG As String = "1"
    Const RAD As String = "9"
    Const ENDOFDAY As String = "EndofDay"
    Public Property DataName() As String
    Public Property LinacName() As String
    Public Property UserReason() As Integer
    Const FAULTPOPUPSELECTED As String = "faultpopupupselected"
    Const QASELECTED As String = "ModalityQApopupselected"
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"
    Const LOCKELFSELECTED As String = "LockELFSelected"
    Private Modalities As controls_ModalityDisplayuc


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

    Protected Sub Close_ModalityQAPopUp(ByVal EquipmentId As String)
        If LinacName = EquipmentId Then
            DynamicControlSelection = String.Empty
            Dim ModalityQA As controls_ModalityQAPopUpuc = CType(FindControl("ModalityQAPopUpuc1"), controls_ModalityQAPopUpuc)
            PlaceHolderModalities.Controls.Remove(ModalityQA)
        End If
    End Sub

    Public Function FormatImage(ByVal energy As Boolean) As String
        Dim happyIcon As String = "Images/happy.gif"
        Dim sadIcon As String = "Images/sad.gif"
        If (energy) Then
            Return happyIcon
        Else
            Return sadIcon
        End If
    End Function

    Protected Sub Checked(ByVal sender As Object, ByVal e As EventArgs)
        Dim toggle As CheckBox = CType(GridView1.HeaderRow.FindControl("chkSelectAll"), CheckBox)

        If toggle.Checked = True Then
            For Each grv As GridViewRow In GridView1.Rows
                Dim cb As CheckBox = CType(grv.FindControl("RowlevelCheckBox"), CheckBox)
                If cb.Enabled = True Then
                    cb.Checked = True
                End If
            Next
        Else
            For Each grv As GridViewRow In GridView1.Rows
                Dim cb As CheckBox = CType(grv.FindControl("RowlevelCheckBox"), CheckBox)
                cb.Checked = False
            Next
        End If

    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

        BoxChanged = "EngBoxChanged" + LinacName ' used but leave for now

        AddHandler WriteDatauc1.UserApproved, AddressOf UserApprovedEvent
        AddHandler ConfirmPage1.ConfirmExit, AddressOf ConfirmExitEvent ' this is if imaging wasn't selected

    End Sub

    Public Sub EngLogOnEvent(connectionString As String)

        BoxChanged = "EngBoxChanged" + LinacName


        If Not LinacName Like "T?" Then
            SetEnergies(connectionString)
            SetImaging(connectionString)
            PhysicsQA.Visible = True
        End If

        GridView1.Visible = True
        GridViewImage.Visible = True

    End Sub

    ' This updates the defect display on defectsave etc when repeat fault from defectsave
    Protected Sub Update_DefectDailyDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_defectsToday(LinacName)

        End If
    End Sub

    Protected Sub Update_ViewOpenFaults(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_OpenConcessions(LinacName)

        End If
    End Sub

    Protected Sub Update_ClosedFaultDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_FaultClosedDisplay(LinacName)
        End If
    End Sub

    Public Sub UserApprovedEvent(ByVal Tabset As String, ByVal Userinfo As String)

        Dim ClinicalTab As String = "3"

        If (Tabset = ENG) Or (Tabset = "666") Or (Tabset = RAD) Then

            Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
            HttpContext.Current.Session.Remove("Actionstate")
            Dim machinelabel As String = LinacName & "Page.aspx';"
            Dim Valid As Boolean = False
            Dim strScript As String = "<script>"
            Dim grdview As GridView = FindControl("Gridview1")
            Dim grdviewI As GridView = FindControl("GridViewImage")
            If (Not HttpContext.Current.Application(BoxChanged) Is Nothing) Then
                comment = HttpContext.Current.Application(BoxChanged).ToString
            Else
                comment = String.Empty
            End If

            Dim Successful As Boolean = False

            If Action = "Confirm" Then
                Valid = True
                Successful = DavesCode.NewEngRunup.CommitRunup(grdview, grdviewI, LinacName, Tabset, Userinfo, comment, Valid, False, False, FaultParams)
                If Successful Then
                    CommentBox.ResetCommentBox(String.Empty)
                    Dim returnstring As String = LinacName + "page.aspx"
                    Response.Redirect(returnstring)
                Else
                    RaiseError("WriteEnergies")
                End If

            Else
                Valid = False
                If Tabset = "666" Then
                    grdview = CType(mpContentPlaceHolder.FindControl("DummyGridview"), GridView)
                    grdviewI = CType(mpContentPlaceHolder.FindControl("DummyGridViewImaging"), GridView)
                End If
                Successful = DavesCode.NewEngRunup.CommitRunup(grdview, grdviewI, LinacName, Tabset, Userinfo, comment, Valid, False, False, FaultParams)
                If Successful Then

                    If Not Userinfo = "Restored" Then
                        CommentBox.ResetCommentBox(String.Empty)
                        strScript += "alert('Not Approved For Clinical Use.Logging Off');"
                        strScript += "window.location='"
                        strScript += machinelabel
                        strScript += "</script>"
                        ScriptManager.RegisterStartupScript(engHandoverButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                    Else
                        'Dim returnstring As String = LinacName + "page.aspx?TabAction=Recovered&NextTab=" & Tabset
                        Dim returnstring As String = LinacName + "page.aspx"
                        Response.Redirect(returnstring, False)
                        Context.ApplicationInstance.CompleteRequest()
                    End If
                Else
                    RaiseError("WriteEnergies")
                End If
            End If
        End If

    End Sub
    Protected Sub RaiseLoadError()
        Dim machinelabel As String = LinacName & "Page.aspx';"

        HttpContext.Current.Application(BoxChanged) = Nothing

        Dim strScript As String = "<script>"
        strScript += "alert('Problem Loading Values. Please call Engineer');"
        strScript += "window.location='"
        strScript += machinelabel
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(engHandoverButton, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub
    Protected Sub RaiseError(ByVal message As String)

        Select Case message
            Case "Images"
                message = "alert('Problem Loading Imaging. Logging off without Approving Energies');"
            Case "WriteEnergies"
                message = "alert('Problem Recording Energies. Logging off without Approving Energies');"
            Case "Radconcess"
                message = "alert('Problem checking Rad Concessions. Logging off without Approving Energies');"
        End Select

        Dim strScript As String = "<script>"
        Dim machinelabel As String = LinacName & "Page.aspx';"

        strScript += message
        strScript += "window.location='"
        strScript += machinelabel
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(engHandoverButton, Me.GetType(), "JSCR", strScript.ToString(), False)

    End Sub
    Protected Sub RaiseLockError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Locking Elf. Please inform system administator');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(LockElf, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Sub Page_load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        WaitButtons("Tech")
        Dim SuccessEnergy As Boolean = False
        Dim SuccessImage As Boolean = False

        Dim objMFG As controls_MainFaultDisplayuc = Page.LoadControl("controls\MainFaultDisplayuc.ascx")
        CType(objMFG, controls_MainFaultDisplayuc).LinacName = LinacName
        CType(objMFG, controls_MainFaultDisplayuc).ID = "MainFaultDisplay"
        AddHandler objMFG.Mainfaultdisplay_UpdateClosedFaultDisplay, AddressOf Update_ClosedFaultDisplay

        'Sets up user comments
        Dim Vctrl As ViewCommentsuc = CType(FindControl("ViewCommentsuc1"), ViewCommentsuc)
        Vctrl.LinacName = LinacName
        'Sets up view open faults and Lock Elf

        If UserReason = 1 Then
            CType(objMFG, controls_MainFaultDisplayuc).ParentControl = ENG

        Else
            'Hide lock elf button if rad run up
            LockElf.Visible = False
            CType(objMFG, controls_MainFaultDisplayuc).ParentControl = RAD

        End If
        PlaceHolderFaults.Controls.Add(objMFG)

        Dim ReportFault As controls_ReportAFaultuc = CType(FindControl("ReportAFaultuc1"), controls_ReportAFaultuc)
        ReportFault.LinacName = LinacName
        ReportFault.ParentControl = ENG
        AddHandler ReportFault.ReportAFault_UpdateDailyDefectDisplay, AddressOf Update_DefectDailyDisplay
        AddHandler ReportFault.ReportAFault_UpDateViewOpenFaults, AddressOf Update_ViewOpenFaults

        Select Case Me.DynamicControlSelection

            Case LOCKELFSELECTED
                Dim ObjLock As controls_UnLockElfuc = Page.LoadControl("controls\UnLockElfuc.ascx")

                ObjLock.LinacName = LinacName
                ObjLock.UserReason = ENG
                AddHandler ObjLock.HideUnlockPopUp, AddressOf HideUnlockElf
                'ObjLock.ID = "LockElf1"

                LockELFPlaceholder.Controls.Add(ObjLock)
                LockELFModalPopup.Show()

            Case QASELECTED
                Dim ObjQA As controls_ModalityQAPopUpuc = Page.LoadControl("controls\ModalityQAPopUpuc.ascx")
                ObjQA.LinacName = LinacName
                ObjQA.ParentControl = ENG
                ObjQA.ID = "ModalityQAPopUpuc1"
                DynamicControlSelection = QASELECTED
                AddHandler ObjQA.CloseModalityQAPopUpTab, AddressOf Close_ModalityQAPopUp
                PlaceHolderModalities.Controls.Add(ObjQA)
            Case Else
                'no dynamic controls need to be loaded...yet
        End Select

        'Wire up the event (UserApproved) to the event handler (UserApprovedEvent)
        'The solution of how to pass parameter to dynamically loaded user control is from here:
        'http://weblogs.asp.net/aghausman/archive/2009/04/15/how-to-pass-parameters-to-the-dynamically-added-user-control.aspx

        Dim ControlName As String = DataName
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        wctrl.LinacName = LinacName
        wctrl.UserReason = UserReason
        If UserReason = 9 Then
            wctrl.Tabby = RAD
        Else
            wctrl.Tabby = ENG
        End If

        CommentBox.BoxChanged = BoxChanged

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
    Protected Sub SetEnergies(ByVal connectionString As String)
        Dim SelCommand As String = ""
        If UserReason = ENG Then
            'added imaging
            SelCommand = "SELECT * FROM [physicsenergies] where linac= @linac and Energy not in ('iView','XVI')"
        Else
            SelCommand = "SELECT * FROM [physicsenergies] where linac= @linac and Energy in ('6 MV', '10 MV')"
        End If
        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = connectionString,
            .SelectCommand = SelCommand
        }
        SqlDataSource1.SelectParameters.Add("@linac", Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", LinacName)

        GridView1.DataSource = SqlDataSource1
        GridView1.DataBind()


        ''This makes visible checkboxes for those energies that are approved
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim reader As SqlDataReader
        Dim count As Integer = 0

        conn = New SqlConnection(connectionString)
        If UserReason = ENG Then
            'added imaging
            comm = New SqlCommand("SELECT EnergyID, Approved FROM physicsenergies where linac=@linac and Energy not in ('iView','XVI')", conn)
        Else
            comm = New SqlCommand("SELECT EnergyID, Approved FROM physicsenergies where linac=@linac and Energy in ('6 MV', '10 MV')", conn)
        End If
        comm.Parameters.Add("@linac", Data.SqlDbType.NVarChar, 10)
        comm.Parameters("@linac").Value = LinacName

        conn.Open()
        reader = comm.ExecuteReader()
        While reader.Read()
            'This will fall over if approved is null so needs error handling
            'Added handling 4/7/17
            If Not IsDBNull(reader.Item("Approved")) Then
                If Not reader.Item("Approved") Then
                    Dim cb As CheckBox = CType(GridView1.Rows(count).FindControl("RowLevelCheckBox"), CheckBox)
                    cb.Enabled = False
                    cb.Visible = False
                End If
            Else
                Dim cb As CheckBox = CType(GridView1.Rows(count).FindControl("RowLevelCheckBox"), CheckBox)
                cb.Enabled = False
                cb.Visible = False
            End If
            count = count + 1
        End While
        reader.Close()
        conn.Close()

    End Sub
    Protected Function SetImaging(ByVal connectionString As String) As Boolean
        Dim count As Integer = 0
        Dim conn As SqlConnection
        Dim Success As Boolean = False
        Dim Scommand As String
        If UserReason = ENG Then
            Scommand = "SELECT * FROM [physicsenergies] where linac=@linac and Energy in ('iView','XVI') order by energy"
        Else
            Scommand = "SELECT * FROM [physicsenergies] where linac=@linac and Energy in ('iView', 'XVI') order by energy"
        End If

        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = connectionString,
            .SelectCommand = Scommand
        }

        SqlDataSource1.SelectParameters.Add("@linac", Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", LinacName)

        GridViewImage.DataSource = SqlDataSource1
        GridViewImage.DataBind()

        Dim comm As SqlCommand
        Dim reader As SqlDataReader

        conn = New SqlConnection(connectionString)
        If UserReason = ENG Then
            comm = New SqlCommand("SELECT EnergyID, Approved FROM physicsenergies where linac=@linac and Energy in ('iView','XVI') order by energy", conn)
        Else
            comm = New SqlCommand("SELECT EnergyID, Approved FROM physicsenergies where linac=@linac and Energy in ('iView') order by energy", conn)
        End If
        comm.Parameters.Add("@linac", Data.SqlDbType.NVarChar, 10)
        comm.Parameters("@linac").Value = LinacName

        conn.Open()
        reader = comm.ExecuteReader()
        While reader.Read()
            'This will fall over if approved is null so needs error handling
            'Same fix as Engineering run up energies 4/7/17
            If Not IsDBNull(reader.Item("Approved")) Then
                If Not reader.Item("Approved") Then
                    Dim cb As CheckBox = CType(GridViewImage.Rows(count).FindControl("RowLevelCheckBoxImage"), CheckBox)
                    cb.Enabled = False
                    cb.Visible = False
                End If
            Else
                CType(GridViewImage.Rows(count).FindControl("RowLevelCheckBoxImage"), CheckBox).Enabled = False
                CType(GridViewImage.Rows(count).FindControl("RowLevelCheckBoxImage"), CheckBox).Visible = False
            End If

            count = count + 1
        End While
        reader.Close()

        conn.Close()
        'disables XVI for emergency run up
        If Not UserReason = ENG Then
            CType(GridViewImage.Rows(1).FindControl("RowLevelCheckBoxImage"), CheckBox).Enabled = False
        End If

        Return Success

    End Function

    Protected Sub ChkSelectAll_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        For Each grv As GridViewRow In GridView1.Rows
            Dim cb As CheckBox = CType(grv.FindControl("RowlevelCheckBox"), CheckBox)
            If cb.Enabled = True Then
                cb.Checked = True
            End If
        Next

    End Sub

    Protected Sub EngHandoverButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles engHandoverButton.Click
        Dim strScript As String = "<script>"
        'count if there are unacknowledged rad concessions first

        'Next 4 lines inserted 7/2/20 to allow rads to run up emergency even if open rad concessions

        Dim Radcount As Boolean = True
        If UserReason = ENG Then
            Radcount = ConfirmNoRadConcession()
        End If

        If Radcount Then
            If LinacName Like "T?" Then
                ConfirmExitEvent()
            Else
                Dim counter As Integer = 0
                For Each grv As GridViewRow In GridView1.Rows
                    Dim checktick As CheckBox = CType(grv.FindControl("RowlevelCheckBox"), CheckBox)
                    If checktick.Checked = True Then
                        counter = counter + 1
                    End If
                Next
                Select Case counter
                    Case Is <> 0

                        Dim icounter As Integer = 0

                        For Each grv As GridViewRow In GridViewImage.Rows

                            Dim checktick As CheckBox = CType(grv.FindControl("RowlevelCheckBoxImage"), CheckBox)
                            If checktick.Checked = True Then
                                icounter = icounter + 1
                            End If
                        Next
                        If icounter <> 0 Then
                            ConfirmExitEvent()
                        Else
                            Dim cptrl As ConfirmPage = CType(FindControl("ConfirmPage1"), ConfirmPage)
                            Dim cpbutton As Button = CType(cptrl.FindControl("AcceptOK"), Button)
                            cpbutton.Text = "Confirm No Imaging"
                            ConfirmPage1.Visible = True

                        End If

                    Case Else
                        strScript += "alert('Select at least one energy');"
                        strScript += "</script>"
                        ScriptManager.RegisterStartupScript(engHandoverButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                End Select
            End If

        Else
            strScript += "alert('There are unacknowledged Rad Concessions');"
            strScript += "</script>"
            ScriptManager.RegisterStartupScript(engHandoverButton, Me.GetType(), "JSCR", strScript.ToString(), False)
        End If
    End Sub

    Protected Function ConfirmNoRadConcession() As Boolean
        Dim comm As SqlCommand
        Dim conn As SqlConnection
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        Dim reader As SqlDataReader
        Dim NumOpen As Integer
        Try
            conn = New SqlConnection(connectionString)
            comm = New SqlCommand("SELECT Count(*) as NumOpen From RadAckFault r Left outer join concessiontable c on r.Incidentid = c.Incidentid where r.Acknowledge = 'false' and linac=@linac", conn)
            comm.Parameters.AddWithValue("@linac", LinacName)

            conn.Open()
            reader = comm.ExecuteReader()

            If reader.Read() Then
                NumOpen = reader.Item("NumOpen")
                If NumOpen <> 0 Then 'there are open faults
                    Return False
                Else
                    Return True
                End If
                'No need for this for if there is no record there has been a sql error
                'Else
                '    Return True

            End If
        Catch ex As Exception
            DavesCode.NewFaultHandling.LogError(ex)
            RaiseError("Radconcess")
        Finally
            conn.Close()
        End Try

    End Function


    Protected Sub LogOff_Click(ByVal sender As Object, ByVal e As EventArgs) Handles LogOffButton.Click
        DavesCode.Reuse.RecordStates(LinacName, 1, "Log off click", 0)
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        wcbutton.Text = "Log Off"
        Session.Add("ActionState", "Cancel")
        WriteDatauc1.Visible = True
        ForceFocus(wctext)

    End Sub


    '15 April Added this control as a result of Bug 11
    Protected Sub LockElf_Click(sender As Object, e As EventArgs) Handles LockElf.Click

        Dim grdview As GridView = FindControl("Gridview1")
        Dim grdviewI As GridView = FindControl("GridViewImage")
        comment = CommentBox.Currentcomment
        DynamicControlSelection = String.Empty
        LockELFModalPopup.Hide()
        Dim success As Boolean = False
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        'has to be able to cope with either tab 1 or 7 control
        success = DavesCode.NewEngRunup.CommitRunup(grdview, grdviewI, LinacName, ENG, "Lockuser", comment, False, False, True, FaultParams)

        If success Then
            RaiseEvent BlankGroup(0)

            Dim ObjLock As controls_UnLockElfuc = Page.LoadControl("controls\UnLockElfuc.ascx")
            ObjLock.LinacName = LinacName
            ObjLock.UserReason = ENG
            AddHandler ObjLock.HideUnlockPopUp, AddressOf HideUnlockElf

            LockELFPlaceholder.Controls.Add(ObjLock)
            DynamicControlSelection = LOCKELFSELECTED
            LockELFModalPopup.Show()

        Else
            RaiseLockError()
        End If

    End Sub

    '15 April comment. Added this control as a result of Bug 11
    Protected Sub PhysicsQA_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PhysicsQA.Click
        Dim ObjQA As controls_ModalityQAPopUpuc = Page.LoadControl("controls\ModalityQAPopUpuc.ascx")
        ObjQA.LinacName = LinacName
        ObjQA.ParentControl = ENG
        ObjQA.ID = "ModalityQAPopUpuc1"
        DynamicControlSelection = QASELECTED
        AddHandler ObjQA.CloseModalityQAPopUpTab, AddressOf Close_ModalityQAPopUp
        PlaceHolderModalities.Controls.Add(ObjQA)

    End Sub

    '15 April Comment added as a result of Bug 6
    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" +
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub
    Private Sub WaitButtons(ByVal WaitType As String)

        Select Case WaitType
            Case "Acknowledge"
                Dim Accept As Button = FindControl("AcceptOK")
                Dim Cancel As Button = FindControl("btnchkcancel")
                If Not FindControl("AcceptOK") Is Nothing Then
                    Accept.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Accept, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not FindControl("btnchkcancel") Is Nothing Then
                    Cancel.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Cancel, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

            Case "Tech"
                Dim Eng As Button = FindControl("engHandoverButton")
                Dim LogOff As Button = FindControl("LogOffButton")
                Dim Lock As Button = FindControl("LockElf")
                Dim Physics As Button = FindControl("PhysicsQA")
                Dim Atlas As Button = FindControl("ViewAtlasButton")
                Dim FaultPanel As Button = FindControl("FaultPanelButton")
                If Not Eng Is Nothing Then
                    Eng.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Eng, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not LogOff Is Nothing Then
                    LogOff.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(LogOff, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Lock Is Nothing Then
                    Lock.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Lock, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Physics Is Nothing Then
                    Physics.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Physics, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Atlas Is Nothing Then
                    Atlas.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Atlas, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not FaultPanel Is Nothing Then
                    FaultPanel.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(FaultPanel, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

            Case "Rad"
                Dim clin As Button = FindControl("clinHandoverButton")
                Dim LogOff As Button = FindControl("LogOff")
                Dim TStart As Button = FindControl("TStart")
                If Not clin Is Nothing Then
                    clin.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(clin, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not LogOff Is Nothing Then
                    LogOff.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(LogOff, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not TStart Is Nothing Then
                    TStart.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(TStart, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

        End Select

    End Sub
    Protected Sub ConfirmExitEvent()
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        wcbutton.Text = "Confirm for Clinical Use"
        Session.Add("ActionState", "Confirm")
        WriteDatauc1.Visible = True
        ForceFocus(wctext)
    End Sub

End Class