Imports System.Data.SqlClient
Imports System.Transactions
Imports DavesCode

Partial Class Repairuc
    Inherits System.Web.UI.UserControl
    Private mpContentPlaceHolder As ContentPlaceHolder
    Private SelectCount As Boolean
    Private Radioselect As Integer = 102 'default to end of day

    Private objconToday As TodayClosedFault
    Private Todaydefect As DefectSave
    Private Todaydefectpark As DefectSavePark

    Private laststate As String
    Private lastuser As String
    Private lastusergroup As String
    Private isFault As Boolean
    Private BoxChanged As String
    Public Event BlankGroup(ByVal BlankUser As Integer)

    Private tabstate As String

    Dim FaultParams As DavesCode.FaultParameters = New DavesCode.FaultParameters()
    Private comment As String
    Const REPAIR As String = "5"
    Private MainFaultPanel As controls_MainFaultDisplayuc
    Private objReportFault As controls_ReportFaultPopUpuc
    Const NEWFAULTSELECTED As String = "NewFaultSelected"
    Const FAULTPOPUPSELECTED As String = "faultpopupupselected"
    Const QASELECTED As String = "ModalityQApopupselected"
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"
    Const EMPTYSTRING As String = ""
    Private ParamApplication As String
    Private ConcessParamsTrial As ConcessionParameters = New ConcessionParameters()
    Dim Repairlist As RadioButtonList
    Dim Modalities As controls_ModalityDisplayuc
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

    Public Property LinacName() As String
    Public Sub UpdateReturnButtonsHandler()
        'If Application(faultstate) <> True Then
        Dim ThereIsAFaultOpen As Boolean = NewFaultHandling.CheckForOpenFault(LinacName)

        If ThereIsAFaultOpen Then
            Reuse.GetLastTech(LinacName, 0, laststate, lastuser, lastusergroup)
            '    If (lastusergroup = 4) Then
            '        RadioButtonList1.Items.FindByValue(6).Enabled = True
            '    Else
            '        RadioButtonList1.Items.FindByValue(6).Enabled = False
            '    End If
            StateTextBox.Text = laststate
        End If
    End Sub

    Public Sub Repairlogon(ByVal connectionString As String)
        Dim breakdown As Boolean
        'Now find which user group is logged on
        'disabled to test removal of physics QA button 31 march 2016
        'This doesn't work if faultstate is corrupted so it will be best to test to see if there is an open fault instead
        'If Application(faultstate) <> True Then
        breakdown = Reuse.CheckForOpenFault(LinacName, connectionString)
        If Not breakdown Then
            'Application(faultstate) = False
            Reuse.GetLastTechNew(LinacName, 0, laststate, lastuser, lastusergroup, connectionString)
            '    If (lastusergroup = 4) Then
            '        RadioButtonList1.Items.FindByValue(6).Enabled = True
            '    Else
            '        RadioButtonList1.Items.FindByValue(6).Enabled = False
            '    End If
            StateTextBox.Text = laststate
        Else
            'Application(faultstate) = True
            Dim reloadnewfaultselected As controls_NewFaultPopUpuc
            reloadnewfaultselected = CType(FindControl("NewFaultPopup"), controls_NewFaultPopUpuc)
            If reloadnewfaultselected Is Nothing Then
                'ConcessParamsTrial = Application(ParamApplication)
                Dim NewFaultPopup As controls_NewFaultPopUpuc = Page.LoadControl("controls\NewFaultPopUpuc.ascx")
                CType(NewFaultPopup, controls_NewFaultPopUpuc).ID = "NewFaultPopup"
                CType(NewFaultPopup, controls_NewFaultPopUpuc).LinacName = LinacName

                CType(NewFaultPopup, controls_NewFaultPopUpuc).ParentName = REPAIR
                CType(NewFaultPopup, controls_NewFaultPopUpuc).Visible = True
                AddHandler NewFaultPopup.CloseFaultTracking, AddressOf CloseTracking
                AddHandler NewFaultPopup.UpdateClosedDisplays, AddressOf Update_FaultClosedDisplays
                AddHandler NewFaultPopup.UpdateOpenConcessions, AddressOf Update_ViewOpenFaults
                NewFaultPopupPlaceHolder.Controls.Add(NewFaultPopup)
                DynamicControlSelection = NEWFAULTSELECTED
            End If
        End If
    End Sub
    'This works to update closed faults and to remove concession from defect dropdown list.
    Protected Sub Update_FaultClosedDisplays(ByVal EquipmentID As String)
        MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
        MainFaultPanel.Update_FaultClosedDisplay(EquipmentID)

    End Sub

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
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Dim tabcontainer1 As TabContainer
        'Page = Me.Page
        'mpContentPlaceHolder =
        'CType(Page.Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
        'If Not mpContentPlaceHolder Is Nothing Then
        '    tabcontainer1 = CType(mpContentPlaceHolder.
        '        FindControl("tcl"), TabContainer)
        '    If Not tabcontainer1 Is Nothing Then
        '        Dim panelcontrol1 As TabPanel = tabcontainer1.FindControl("TabPanel5")
        '        'accontrol1 = panelcontrol1.FindControl("AcceptLinac5")
        '        'AddHandler accontrol1.Repairloaded, AddressOf Repairlogon

        '    End If
        'End If

        SetStates()
        ParamApplication = "Params" + LinacName

    End Sub

    Protected Sub SetStates()
        AddHandler WriteDatauc1.UserApproved, AddressOf UserApprovedEvent
        'AddHandler AutoApproved, AddressOf UserApprovedEvent
        'appstate = "LogOn" + LinacName
        'actionstate = "ActionState" + LinacName
        'suspstate = "Suspended" + LinacName
        'FaultOriginTab = "FOT" + LinacName
        'RunUpDone = "rppTab" + LinacName
        'faultviewstate = "Faultsee" + LinacName
        'atlasviewstate = "Atlassee" + LinacName
        'qaviewstate = "QAsee" + LinacName
        'faultstate = "OpenFault" + LinacName
        BoxChanged = "RepBoxChanged" + LinacName
        'tabstate = "ActTab" + LinacName
    End Sub
    Public Sub UserApprovedEvent(ByVal TabSet As String, ByVal Userinfo As String)
        Dim username As String = Userinfo
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim LinacStatusID As String = ""
        Dim LinacStateID As String = ""
        Dim Breakdown = False
        'Dim suspendvalue As String = Nothing
        'Dim RunUpBoolean As String = Nothing
        Dim FaultParams As DavesCode.FaultParameters = New DavesCode.FaultParameters()
        Dim EndofDay As Integer = 102
        Dim Recovery As Integer = 101
        Dim result As Boolean = False

        If TabSet = REPAIR Then
            SetStates()

            If (Not HttpContext.Current.Application(BoxChanged) Is Nothing) Then
                comment = HttpContext.Current.Application(BoxChanged).ToString
            Else
                comment = String.Empty
            End If
            Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
            HttpContext.Current.Session.Remove("Actionstate")
            'laststate = Reuse.GetLastState(LinacName, 0)
            'If laststate = GlobalConstants.SUSPENDED Then
            '    suspendvalue = 1
            'Else
            '    suspendvalue = 0
            'End If

            If Action = GlobalConstants.ENDOFDAY Then
                Radioselect = EndofDay
                Action = "Confirm"

            ElseIf Action = "Recover" Then
                Radioselect = Recovery
            Else
                Radioselect = RadioButtonList1.SelectedItem.Value
            End If
            'This can't be writing after new fault. That's done from defect save - should it be?
            result = NewWriteAux.WriteAuxTables(LinacName, username, comment, Radioselect, TabSet, False, False, FaultParams)
            If result Then
                If Action = "Confirm" Then
                    DynamicControlSelection = String.Empty
                    CommentBox.ResetCommentBox(String.Empty)

                    Dim strScript As String = "<script>"
                    strScript += "window.location='"
                    strScript += machinelabel
                    strScript += "</script>"
                    'This could probably be tidied up if clinical not 3,3

                    'If this fails it writes an error to file but carries on.
                    'DavesCode.NewWriteAux.WriteAuxTables(MachineName, username, comment, Radioselect, Tabused, False, suspendvalue, RunUpBoolean, False)
                    'DavesCode.Reuse.Writerep(MachineName, username, comment, LinacStatusID)
                    'Application(appstate) = 0
                    'Application(tabstate) = String.Empty
                    ' this is an instrumentation field that displays application number ie 0 or 1
                    'Dim output As String = Application(appstate)
                    'Me.Page.GetType.InvokeMember("UpdateHiddenLAField", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {output})
                    'SelectCount = False
                    'HttpContext.Current.Application(BoxChanged) = Nothing
                    Select Case Radioselect

                        Case 1

                            Dim returnstring As String = LinacName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)
                            Response.Redirect(returnstring)

                        Case 3

                            ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)

                        Case 4

                            Dim returnstring As String = LinacName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)
                            Response.Redirect(returnstring)
                        Case 6

                            Dim returnstring As String = LinacName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)
                            Response.Redirect(returnstring)
                        Case 102

                            ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                        Case 8

                            Dim returnstring As String = LinacName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)
                            Response.Redirect(returnstring)
                    End Select

                    RadioButtonList1.SelectedIndex = -1
                    LogOffButton.BackColor = Drawing.Color.AntiqueWhite
                Else

                    Dim returnstring As String = LinacName + "page.aspx?TabAction=Recovered&NextTab=" & TabSet
                    Response.Redirect(returnstring, False)
                End If
            Else
                RaiseLogOffError()
            End If
        End If

    End Sub
    Protected Sub RaiseLogOffError()
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Logging Off. Please inform Engineering');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub
    Protected Sub LogOffButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LogOffButton.Click
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wclabel As Label = CType(wctrl.FindControl("WarningLabel"), Label)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        Dim laststate As String = ""
        Dim lastusername As String = ""
        Dim lastusergroup As Integer
        Dim success As Boolean = True
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        Dim LogOffUser As String = "AutoLog"



        If laststate = GlobalConstants.FAULT Then
            success = WriteFaultIDTable()
        End If
        If success Then
            'this check isnt in pm and training
            If Not RadioButtonList1.SelectedItem Is Nothing Then
                Session.Add("Actionstate", "Confirm")
                Reuse.GetLastTechNew(LinacName, 0, laststate, lastusername, lastusergroup, connectionString)
                Session.Add("name", LogOffUser)
                Session.Add("usergroup", lastusergroup)
                Radioselect = RadioButtonList1.SelectedItem.Value
                Session.Add("userreason", Radioselect)
                Select Case Radioselect
                    Case 1, 4, 8
                        'wcbutton.Text = "Go To Engineering Run up"
                        UserApprovedEvent(REPAIR, LogOffUser)
                        'RaiseEvent AutoApproved(5, lastusername)

                    Case 3
                        wcbutton.Text = "Return to clinical"
                        WriteDatauc1.Visible = True
                        ForceFocus(wctext)
                        'Case 4
                        'wcbutton.Text = "Go To Planned Maintenance"
                        'RaiseEvent AutoApproved(5, lastusername)
                        'UserApprovedEvent(5, lastusername)
                    'Case 6
                    '    wcbutton.Text = "Go To Physics QA"
                    '    If lastusergroup = 4 Then
                    '        RaiseEvent AutoApproved(5, lastusername)
                    '    Else
                    '        WriteDatauc1.Visible = True
                    '        ForceFocus(wctext)
                    '    End If

                    Case 102
                        wclabel.Text = "Are you sure? This is the End of Day"
                        wcbutton.Text = "End of Day"
                        WriteDatauc1.Visible = True
                        ForceFocus(wctext)
                        'Case 8
                        'wcbutton.Text = "Go To Training/Development"
                        'RaiseEvent AutoApproved(5, lastusername)
                        'UserApprovedEvent(5, lastusername)
                End Select

            Else
                RaiseLogOffError()
            End If
        Else
            RaiseLogOffError()
        End If

    End Sub

    Protected Sub RadioButtonList1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButtonList1.SelectedIndexChanged
        'By this time the available buttons should have been set
        LogOffButton.Enabled = True
        LogOffButton.BackColor = Drawing.Color.Yellow
    End Sub

    Protected Sub PhysicsQA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles PhysicsQA.Click

        Dim ObjQA As controls_ModalityQAPopUpuc = Page.LoadControl("controls\ModalityQAPopUpuc.ascx")
        ObjQA.LinacName = LinacName
        ObjQA.ParentControl = REPAIR
        ObjQA.ID = "ModalityQAPopUpuc1"
        DynamicControlSelection = QASELECTED
        AddHandler ObjQA.CloseModalityQAPopUpTab, AddressOf Close_ModalityQAPopUp
        PlaceHolderModalities.Controls.Add(ObjQA)

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim success As Boolean = False
        WaitButtons("Tech")
        '    'The solution of how to pass parameter to dynamically loaded user control is from here:
        '    'http://weblogs.asp.net/aghausman/archive/2009/04/15/how-to-pass-parameters-to-the-dynamically-added-user-control.aspx

        CommentBox.BoxChanged = BoxChanged
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        Select Case Me.DynamicControlSelection

            Case NEWFAULTSELECTED
                Dim reloadnewfaultselected As controls_NewFaultPopUpuc
                reloadnewfaultselected = CType(FindControl("NewFaultPopup"), controls_NewFaultPopUpuc)
                If reloadnewfaultselected Is Nothing Then
                    Dim NewFaultPopup As controls_NewFaultPopUpuc = Page.LoadControl("controls\NewFaultPopUpuc.ascx")
                    CType(NewFaultPopup, controls_NewFaultPopUpuc).ID = "NewFaultPopup"
                    CType(NewFaultPopup, controls_NewFaultPopUpuc).LinacName = LinacName
                    'ConcessParamsTrial.Linac
                    CType(NewFaultPopup, controls_NewFaultPopUpuc).ParentName = REPAIR
                    CType(NewFaultPopup, controls_NewFaultPopUpuc).Visible = True
                    AddHandler NewFaultPopup.CloseFaultTracking, AddressOf CloseTracking
                    AddHandler NewFaultPopup.UpdateClosedDisplays, AddressOf Update_FaultClosedDisplays
                    AddHandler NewFaultPopup.UpdateOpenConcessions, AddressOf Update_ViewOpenFaults
                    NewFaultPopupPlaceHolder.Controls.Add(NewFaultPopup)
                End If
            Case LOCKELFSELECTED
                Dim ObjLock As controls_UnLockElfuc = Page.LoadControl("controls\UnLockElfuc.ascx")

                ObjLock.LinacName = LinacName
                ObjLock.UserReason = REPAIR
                AddHandler ObjLock.HideUnlockPopUp, AddressOf HideUnlockElf
                'ObjLock.ID = "LockElf1"

                LockELFPlaceholder.Controls.Add(ObjLock)
                LockELFModalPopup.Show()

            Case QASELECTED
                Dim ObjQA As controls_ModalityQAPopUpuc = Page.LoadControl("controls\ModalityQAPopUpuc.ascx")
                ObjQA.LinacName = LinacName
                ObjQA.ParentControl = REPAIR
                ObjQA.ID = "ModalityQAPopUpuc1"
                DynamicControlSelection = QASELECTED
                AddHandler ObjQA.CloseModalityQAPopUpTab, AddressOf Close_ModalityQAPopUp
                PlaceHolderModalities.Controls.Add(ObjQA)
            Case Else
                '        'no dynamic controls need to be loaded...yet
        End Select

        Dim objMFG As controls_MainFaultDisplayuc = Page.LoadControl("controls\MainFaultDisplayuc.ascx")
        CType(objMFG, controls_MainFaultDisplayuc).LinacName = LinacName
        CType(objMFG, controls_MainFaultDisplayuc).ID = "MainFaultDisplay"
        CType(objMFG, controls_MainFaultDisplayuc).ParentControl = REPAIR
        AddHandler objMFG.Mainfaultdisplay_UpdateClosedFaultDisplay, AddressOf Update_FaultClosedDisplays
        PlaceHolderFaults.Controls.Add(objMFG)

        Dim ReportFault As controls_ReportAFaultuc = CType(FindControl("ReportAFaultuc1"), controls_ReportAFaultuc)
        ReportFault.LinacName = LinacName
        ReportFault.ParentControl = REPAIR
        AddHandler ReportFault.ReportAFault_UpdateDailyDefectDisplay, AddressOf Update_DefectDailyDisplay
        AddHandler ReportFault.ReportAFault_UpDateViewOpenFaults, AddressOf Update_ViewOpenFaults

        Dim Vctrl As ViewCommentsuc = CType(FindControl("ViewCommentsuc1"), ViewCommentsuc)
        Vctrl.LinacName = LinacName
        Dim wrtctrl As WriteDatauc = CType(FindControl("WriteDatauc1"), WriteDatauc)
        wrtctrl.LinacName = LinacName

        'Dim lockctrl As LockElfuc = CType(FindControl("LockElfuc1"), LockElfuc)
        'lockctrl.LinacName = LinacName
        Modalities = Page.LoadControl("controls/ModalityDisplayuc.ascx")
        CType(Modalities, controls_ModalityDisplayuc).LinacName = LinacName
        CType(Modalities, controls_ModalityDisplayuc).ID = "ModalityDisplay"
        laststate = Reuse.GetLastState(LinacName, 0)
        If laststate = GlobalConstants.SUSPENDED Then
            CType(Modalities, controls_ModalityDisplayuc).Mode = "Suspended"
        Else
            CType(Modalities, controls_ModalityDisplayuc).Mode = "Linac Unauthorised"
        End If
        'Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        CType(Modalities, controls_ModalityDisplayuc).ConnectionString = connectionString
        ModalityPlaceholder.Controls.Add(Modalities)
        ModalityDisplayPanel.Visible = True

        If Not IsPostBack Then

            If Not LinacName Like "T?" Then
                PhysicsQA.Visible = True
            End If
            RadioButtonList1.Items.Add(New ListItem("Go To Engineering Run up", "1", False))
            RadioButtonList1.Items.Add(New ListItem("Hand Back to Clinical", "3", False))
            RadioButtonList1.Items.Add(New ListItem("Go To Planned Maintenance", "4", False))
            RadioButtonList1.Items.Add(New ListItem("Go To Training/Development", "8", False))
            RadioButtonList1.Items.Add(New ListItem("End of Day", "102", False))

            'Application(atlasviewstate) = 1
            'Application(qaviewstate) = 1
            Dim NumOpen As Integer = 0
            Dim IncidentID As Integer = 0
            Dim conn As SqlConnection
            Dim comm As SqlCommand
            Dim reader As SqlDataReader
            Dim connectionString1 As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            conn = New SqlConnection(connectionString1)
            comm = New SqlCommand("select IncidentID from FaultIDTable where Status in ('New', 'Open') and linac=@linac", conn)
            comm.Parameters.AddWithValue("@linac", LinacName)

            conn.Open()
            reader = comm.ExecuteReader()
            If Not FindControl("RadioButtonlist1") Is Nothing Then
                Repairlist = FindControl("RadioButtonlist1")
                If reader.Read() Then
                    IncidentID = reader.Item("IncidentID")
                    StateTextBox.Text = "Fault"
                    'Application(faultstate) = True
                    'isFault = Application(faultstate)
                    LogOffButton.Enabled = False
                    success = ConcessParamsTrial.CreateObject(IncidentID)

                    If success Then
                        Application(ParamApplication) = ConcessParamsTrial
                        'Dim NewFaultPopup As controls_NewFaultPopUpuc = Page.LoadControl("controls\NewFaultPopUpuc.ascx")
                        'CType(NewFaultPopup, controls_NewFaultPopUpuc).ID = "NewFaultPopup"
                        'CType(NewFaultPopup, controls_NewFaultPopUpuc).LinacName = LinacName

                        'CType(NewFaultPopup, controls_NewFaultPopUpuc).ParentName = REPAIR
                        'CType(NewFaultPopup, controls_NewFaultPopUpuc).Visible = True
                        'AddHandler NewFaultPopup.CloseFaultTracking, AddressOf CloseTracking
                        'AddHandler NewFaultPopup.UpdateClosedDisplays, AddressOf Update_FaultClosedDisplays
                        'AddHandler NewFaultPopup.UpdateOpenConcessions, AddressOf Update_ViewOpenFaults
                        'NewFaultPopupPlaceHolder.Controls.Add(NewFaultPopup)
                        'DynamicControlSelection = NEWFAULTSELECTED
                    Else
                        RaiseError()
                    End If
                Else

                    Application(ParamApplication) = Nothing
                    SetLeavingButtons()

                End If

            End If
            conn.Close()
        End If

    End Sub

    Protected Sub SetLeavingButtons()
        Reuse.GetLastTech(LinacName, 0, laststate, lastuser, lastusergroup)
        LogOffButton.Enabled = False
        If Not laststate = GlobalConstants.FAULT Then


            'Application(faultstate) = False
            'isFault = False
            Repairlist.Items.FindByValue(1).Enabled = True
            Repairlist.Items.FindByValue(4).Enabled = True

            Repairlist.Items.FindByValue(8).Enabled = True
            Repairlist.Items.FindByValue(102).Enabled = True
            'commented out next if because no Physics QA now 31 march 2016
            'If lastusergroup = 4 Then
            '    Repairlist.Items.FindByValue(6).Enabled = True
            'End If
            'Repairlist.Items(4).Selected = False
            'This next if check if got here via clinical suspend
            StateTextBox.Text = "Linac Unauthorised"
            'Dim ParentControl As String = DavesCode.NewFaultHandling.ReturnFaultActivity(LinacName)
            'If ParentControl IsNot Nothing Then
            '    Select Case ParentControl
            '        'Case 0
            '        '    If Application(RunUpDone) = 1 Then
            '        '        RadioButtonList1.Items.FindByValue(3).Enabled = True
            '        '        StateTextBox.Text = "Clinical - Not Treating"
            '        '    End If

            '        Case 3

            '            RadioButtonList1.Items.FindByValue(3).Enabled = True
            '            StateTextBox.Text = "Clinical - Not Treating"
            '        Case 4, 5, 8
            '            If laststate = GlobalConstants.SUSPENDED Then
            '                RadioButtonList1.Items.FindByValue(3).Enabled = True
            '                StateTextBox.Text = GlobalConstants.SUSPENDED
            '                'ElseIf Application(RunUpDone) = 1 Then
            '                '    RadioButtonList1.Items.FindByValue(3).Enabled = True
            '                '    StateTextBox.Text = "Clinical - Not Treating"
            '            End If
            '        Case Else
            '            'StateTextBox.Text = "Linac Unauthorised"
            '    End Select

            If laststate = GlobalConstants.SUSPENDED Then
                RadioButtonList1.Items.FindByValue(3).Enabled = True
                StateTextBox.Text = GlobalConstants.SUSPENDED
                '    Dim rtab As String = Application(RunUpDone)
                'ElseIf Application(RunUpDone) = 1 Then
                '    RadioButtonList1.Items.FindByValue(3).Enabled = True
                '    StateTextBox.Text = "Clinical - Not Treating"
            End If
        End If
    End Sub
    Protected Sub LockElf_Click(sender As Object, e As System.EventArgs) Handles LockElf.Click
        RemoteLockElf(True)
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

    Public Sub RemoteLockElf(ByVal realbut As Boolean)
        'Dim lockctrl As LockElfuc = CType(FindControl("LockElfuc1"), LockElfuc)
        'Dim lockctrltext As TextBox = CType(lockctrl.FindControl("txtchkUserName"), TextBox)
        'Dim suspendvalue As String
        'Dim RunUpBoolean As String
        Dim username As String = "Lockuser"
        comment = CommentBox.Currentcomment
        DynamicControlSelection = String.Empty
        LockELFModalPopup.Hide()
        'suspendvalue = Application(suspstate)
        'RunUpBoolean = Application(RunUpDone)
        Dim tabused = REPAIR
        Dim radioselect As Integer = 7
        Dim breakdown As Boolean = False
        Dim faultstate As String = Nothing

        'laststate = Reuse.GetLastState(LinacName, 0)
        'suspendvalue = laststate
        RaiseEvent BlankGroup(0)

        If Not realbut Then
            username = "System lock"
            comment = String.Empty
        End If
        'Dim lock As Boolean
        'Lock = Not lockctrl.Visible
        'If lock Then

        Dim success As Boolean = False
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        'has to be tablable to cope with either tab 1 or 7 control
        breakdown = Reuse.CheckForOpenFault(LinacName, connectionString)

        success = NewWriteAux.WriteAuxTables(LinacName, username, comment, radioselect, tabused, breakdown, True, FaultParams)

            If success Then
                RaiseEvent BlankGroup(0)
                Dim ObjLock As controls_UnLockElfuc = Page.LoadControl("controls\UnLockElfuc.ascx")
                ObjLock.LinacName = LinacName
                ObjLock.UserReason = REPAIR
                AddHandler ObjLock.HideUnlockPopUp, AddressOf HideUnlockElf
                LockELFPlaceholder.Controls.Add(ObjLock)
                DynamicControlSelection = LOCKELFSELECTED
                LockELFModalPopup.Show()
                'lockctrl.Visible = True
                'ForceFocus(lockctrltext)
            Else
                RaiseLockError()
            End If
        'lockctrl.Visible = True
        'End If
        'ForceFocus(lockctrltext)
    End Sub
    Protected Sub RaiseLockError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Locking Elf. Please inform system administator');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(LockElf, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Sub RaiseReadingFaultError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Intialising Tab. Please inform system administator');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(LockElf, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" +
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

    Public Function WriteFaultIDTable() As Boolean
        'There might have been multiple faults open while repair tab was opened so need to update all of them when leaving.
        Dim laststateid As Integer
        Dim Success As Boolean = False
        Dim conn As SqlConnection
        Try
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            Using myscope As TransactionScope = New TransactionScope()

                Dim updatefault As SqlCommand
                Dim getlaststateid As SqlCommand
                conn = New SqlConnection(connectionString)

                'need to close all faults that have been opened before repair page could be left. Find out what last stateid is then update all records with that time.
                'get last stateid
                getlaststateid = New SqlCommand(("SELECT TOP(1)  [StatusID] FROM FaultIDTable where Linac = @linac ORDER BY [IncidentID] DESC"), conn)
                getlaststateid.Parameters.AddWithValue("@linac", LinacName)


                conn.Open()

                laststateid = DirectCast(getlaststateid.ExecuteScalar(), Integer)
                conn.Close()

                updatefault = New SqlCommand("UPDATE  FaultIDTable SET ReportClosed = @ReportClosed where StatusID  = @statusId", conn)
                'updatefault = New SqlCommand("UPDATE  FaultIDTable SET ReportClosed = @ReportClosed where IncidentID  = (Select max(IncidentID) as lastrecord from FaultIDtable where linac=@linac)", conn)
                updatefault.Parameters.Add("@ReportClosed", Data.SqlDbType.DateTime)
                updatefault.Parameters("@ReportClosed").Value = Now()
                updatefault.Parameters.Add("@linac", Data.SqlDbType.NVarChar, 10)
                updatefault.Parameters("@linac").Value = LinacName
                updatefault.Parameters.Add("@statusID", Data.SqlDbType.Int)
                updatefault.Parameters("@statusID").Value = CInt(laststateid)

                conn.Open()
                updatefault.ExecuteNonQuery()
                myscope.Complete()
                Success = True

            End Using
        Catch ex As Exception
            RaiseLogOffError()
        End Try

        Return Success
    End Function
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

    Protected Sub RaiseError()
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Updating Fault. Please call Engineer');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(LockElf, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Sub CloseTracking(ByVal Linac As String)
        If Linac = LinacName Then
            Dim NewFaultPopup As controls_NewFaultPopUpuc = CType(FindControl("NewFaultPopup"), controls_NewFaultPopUpuc)
            NewFaultPopupPlaceHolder.Controls.Remove(NewFaultPopup)
            ViewState.Item(VIEWSTATEKEY_DYNCONTROL) = String.Empty
            If Not FindControl("RadioButtonlist1") Is Nothing Then
                Repairlist = FindControl("RadioButtonlist1")
                SetLeavingButtons()
            End If
        End If
    End Sub
    Protected Sub Close_ModalityQAPopUp(ByVal EquipmentId As String)
        If LinacName = EquipmentId Then
            DynamicControlSelection = String.Empty
            Dim ModalityQA As controls_ModalityQAPopUpuc = CType(FindControl("ModalityQAPopUpuc1"), controls_ModalityQAPopUpuc)
            PlaceHolderModalities.Controls.Remove(ModalityQA)
        End If
    End Sub

End Class
