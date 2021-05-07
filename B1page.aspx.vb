Imports System.Data.SqlClient
Imports AjaxControlToolkit
Imports System.Transactions
Imports GlobalConstants
Imports DavesCode
Partial Public Class B1page
    Inherits System.Web.UI.Page
    Private EquipmentID As String = "B1"

    Private tabclicked As String
    Private comment As String
    Private mpContentPlaceHolder As ContentPlaceHolder
    Private wctrl As WriteDatauc
    Private runupcontrolId As String = "ERunupUserControl1"
    Private preclincontrolID As String = "PreclinUserControl1"
    Private ClinicalUserControlID As String = "ClinicalUserControl1"
    Private PlannedMaintenanceControlID As String = "PlannedMaintenanceuc1"
    Private repcontrolId As String = "repairuc1"
    Private webusercontrol21ID As String = "webUserControl21"
    Private physicscontrolID As String = "PhysicsQAuc1"
    Private writedatacontrolID As String = "Writedatauc1"
    Private emergencycontrolID As String = "ERunupUserControl2"
    Private trainingcontrolID As String = "Traininguc1"
    Private TodayTraining As Traininguc
    Private TodayPM As Planned_Maintenanceuc
    Private TodayRep As Repairuc


    Private ParentControl As String
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"
    Const ACCEPTLINACSELECTED As String = "CAcceptLinac"
    Public Const NEXTTAB As String = "NextTab"
    Private Tabaction As String = String.Empty
    Public Const CLICKED As String = "Clicked"
    Public Const AUTOCLICKED As String = "Autoclicked"

    Const REPAIR As String = "5"


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


    'Protected Sub Update_ReturnButtons()

    '    Dim tabActive As Integer
    '    tabActive = tcl.ActiveTabIndex
    '    Dim containerID As String = "TabContent" & tabActive
    '    Dim panel As Panel = tcl.ActiveTab.FindControl(containerID)
    '    If (Not panel Is Nothing) Then
    '        Select Case tabActive
    '            Case 4
    '                TodayPM = tcl.ActiveTab.FindControl(PlannedMaintenanceControlID)
    '                TodayPM.UpdateReturnButtonsHandler()
    '            Case 5
    '                TodayRep = tcl.ActiveTab.FindControl(repcontrolId)
    '                TodayRep.UpdateReturnButtonsHandler()
    '            Case 8
    '                TodayTraining = tcl.ActiveTab.FindControl(trainingcontrolID)
    '                TodayTraining.UpdateReturnButtonsHandler()
    '        End Select

    '    End If

    'End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        mpContentPlaceHolder = CType(Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
        If Not mpContentPlaceHolder Is Nothing Then
            wctrl = CType(mpContentPlaceHolder.FindControl("Writedatauc1"), WriteDatauc)
            AddHandler wctrl.UserApproved, AddressOf UserApprovedEvent

        End If

    End Sub

    Protected Sub UserApprovedEvent(ByVal Tabset As String, ByVal Userinfo As String)

        If Tabset = ENDOFDAY Then
                Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
                mpContentPlaceHolder = CType(Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
                If Not mpContentPlaceHolder Is Nothing Then
                    wctrl = CType(mpContentPlaceHolder.FindControl("Writedatauc1"), WriteDatauc)
                    wctrl.Visible = False
                    If Action = "Confirm" Then
                        Try
                            Dim lastState As String
                        'tick looks at fault table make these consistent
                        lastState = Reuse.GetLastState(EquipmentID, 0)
                        Statelabel.Text = lastState

                        Reuse.SetStatus(Userinfo, UNAUTHORISED, 5, 102, EquipmentID, 0, False)

                        Dim returnstring As String = EquipmentID + "page.aspx"

                        Response.Redirect(returnstring)
                    Finally

                        'This should have some error handling


                    End Try

                End If
            End If
        End If

    End Sub

    Protected Sub ClinicalApprovedEvent()
        Dim tabActive As Integer
        tabActive = tcl.ActiveTabIndex
        Dim containerID As String = "TabContent" & tabActive
        Dim ClinicalUserControlid As String = "ClinicalUserControl1"
        Dim panel As Panel = tcl.ActiveTab.FindControl(containerID)
        Dim clincontrol As UserControl = tcl.ActiveTab.FindControl(ClinicalUserControlid)
        If (Not panel Is Nothing) Then
            clincontrol.Visible = True

        End If

    End Sub

    Protected Sub SetUser(ByVal usergroup As Integer)
        Dim usergroupname As String = ""
        Select Case usergroup
            Case 0
                usergroupname = String.Empty
            Case 2
                usergroupname = "Engineer"
            Case 3
                usergroupname = "Radiographer"
            Case 4
                usergroupname = "Physicist"
        End Select

        UserGroupLabel.Text = usergroupname

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Dim Reload As Boolean = False
        'Dim Fcancel As String = ""
        AddHandler PlannedMaintenanceuc1.BlankGroup, AddressOf SetUser
        AddHandler Repairuc1.BlankGroup, AddressOf SetUser
        AddHandler ErunupUserControl1.BlankGroup, AddressOf SetUser

        Dim ResetDay As String = Nothing

        Select Case Me.DynamicControlSelection

            Case ACCEPTLINACSELECTED

                Dim ObjAccept As AcceptLinacuc = Page.LoadControl("Controls/AcceptLinacuc.ascx")
                ObjAccept.LinacName = EquipmentID
                ObjAccept.ID = "AcceptLinac1"
                ObjAccept.UserReason = tcl.ActiveTab.ID.Substring(8)
                'ObjAccept.ActivePanel = tcl.ActiveTabIndex
                AcceptLinacPlaceholder.Controls.Add(ObjAccept)

            Case Else
                '        'no dynamic controls need to be loaded...yet
        End Select

        EndOfDayButton.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(EndOfDayButton, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")

        comment = Nothing
        tabclicked = Nothing
        Dim lastState As String = String.Empty
        Dim userGroup As Integer = 0
        Dim lastusername As String = String.Empty
        Reuse.GetLastTech(EquipmentID, 0, lastState, lastusername, userGroup)
        Statelabel.Text = lastState

        If Not IsPostBack Then

            If Not Request.QueryString("loadup") Is Nothing Then
                ResetDay = Reuse.GetLastTime(EquipmentID, 0)

                Select Case ResetDay
                    Case "Ignore"

                    Case ENDOFDAY
                        EndofDayElf(ResetDay)
                    Case "Error"
                        'Do nothing
                End Select
            End If
            If Request.QueryString("TabAction") Is Nothing Then

                Dim LoggedOn As Boolean = GetApplication.GetApplicationState(EquipmentID, 0)
                Dim ThereIsAFaultOpen As Boolean = NewFaultHandling.CheckForOpenFault(EquipmentID)

                If ThereIsAFaultOpen Then

                    If LoggedOn Then
                        ParentControl = GetApplication.Returnlastuserreason(EquipmentID, 0)
                        If ParentControl = 5 Then
                            ParentControl = NewFaultHandling.ReturnFaultActivity(EquipmentID)
                            Dim returnstring As String = EquipmentID + "page.aspx?TabAction=Fault&NextTab="
                            Response.Redirect(returnstring & ParentControl & "&comment=" & "")
                        Else

                            Dim returnstring As String = EquipmentID + "page.aspx?TabAction=Fault&NextTab="
                            Response.Redirect(returnstring & ParentControl & "&comment=" & "")
                        End If
                    Else
                        ParentControl = NewFaultHandling.ReturnFaultActivity(EquipmentID)
                        Session.Add("RecoverFault", True)
                        Dim returnstring As String = EquipmentID + "page.aspx?TabAction=Fault&NextTab="
                        Response.Redirect(returnstring & ParentControl & "&comment=" & "")

                    End If

                ElseIf LoggedOn Then

                    If lastState = "Fault" Then
                        ParentControl = 5
                        'setmachine state and then do nothing
                        lastState = NewFaultHandling.RecoverLastNonFaultState(EquipmentID)
                        Reuse.SetStatus(lastusername, lastState, userGroup, 5, EquipmentID, 0, 1)
                    Else
                        ParentControl = GetApplication.Returnlastuserreason(EquipmentID, 0)
                    End If
                    Session.Add("usergroup", userGroup)
                    Dim returnstring As String = EquipmentID + "page.aspx?TabAction=Recovered&NextTab="
                    Response.Redirect(returnstring & ParentControl & "&comment=" & "")

                ElseIf lastusername = "Lockuser" Then
                    Reuse.GetLastTech(EquipmentID, 1, lastState, lastusername, userGroup)
                    ParentControl = GetApplication.Returnlastuserreason(EquipmentID, 1)
                    'This is to handle fact that there is still a preclinical step next big job to remove
                    If ParentControl = 2 Then
                        ParentControl = 1
                    End If
                    Dim returnstring As String = EquipmentID + "page.aspx?TabAction=Reloaded&NextTab="
                    Response.Redirect(returnstring & ParentControl & "&comment=" & "")
                End If
                'Else there is no fault open that is not recorded so just carry on
            End If

            TabPanel0.Enabled = True
            EndOfDayButton.Visible = True

            Dim tabpicked As String = Nothing
            Dim RecoveredTab As String = Nothing
            If Not Request.QueryString("TabAction") Is Nothing Then
                Tabaction = Request.QueryString("TabAction").ToString

                Select Case Tabaction
                    Case CLICKED
                        tabpicked = Request.QueryString("NextTab").ToString
                    Case AUTOCLICKED
                        tabpicked = Request.QueryString("NextTab").ToString
                    Case FAULT
                        tabpicked = Request.QueryString("NextTab").ToString
                        If tabpicked = 3 Or tabpicked = 8 Then ' this is to prevent repair loading from clinical or training
                            tabpicked = Nothing
                            lastState = "Fault"
                        End If
                    Case "Recovered"
                        tabpicked = Request.QueryString("NextTab").ToString
                        RecoveredPopup(RecoveredTab)
                    Case "Reloaded"
                        RecoveredTab = Request.QueryString("NextTab").ToString
                        RecoveredPopup(RecoveredTab)
                End Select
            End If

            If Not tabpicked Is Nothing Then
                LaunchTab(Tabaction, tabpicked)
            ElseIf Not RecoveredTab Is Nothing Then
                SetActiveTab(RecoveredTab)
            Else

                Select Case lastState
                    Case UNAUTHORISED
                        TabPanel1.Enabled = "true"
                        TabPanel3.Enabled = "false"
                        TabPanel4.Enabled = "true"
                        TabPanel5.Enabled = "true"
                        TabPanel8.Enabled = "True"
                        TabPanel9.Enabled = "True"

                    Case "Fault"
                        TabPanel1.Enabled = "false"
                        TabPanel3.Enabled = "false"
                        TabPanel4.Enabled = "false"
                        TabPanel5.Enabled = "true"
                        TabPanel8.Enabled = "false"
                        TabPanel9.Enabled = "false"

                    Case SUSPENDED
                        TabPanel1.Enabled = "false"
                        TabPanel3.Enabled = "true"
                        TabPanel4.Enabled = "true"
                        TabPanel5.Enabled = "true"
                        TabPanel8.Enabled = "true"
                        TabPanel9.Enabled = "false"

                    Case Else

                End Select
            End If

        End If

    End Sub

    Protected Sub RecoveredPopup(ByVal RecoveredTab As String)
        Dim strScript As String = "<script>"
        Dim machinelabel As String = EquipmentID & "Page.aspx';"
        If Not RecoveredTab Is Nothing Then
            strScript += "alert('ELF has been reloaded.');"
            'strScript += "alert('ELF has been restored. Please Log in again to complete your action');"
        Else
            strScript += "alert('ELF has been restored.');"
        End If
        'strScript += "window.location='"
        'strScript += machinelabel
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(RestoreButton, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub
    'Protected Sub LoadTab(ByVal SetActiveTab As String)
    '    Dim container As TabContainer
    '    Dim TabId As String = "TabPanel" & SetActiveTab

    '    mpContentPlaceHolder = CType(Page.Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
    '    If Not mpContentPlaceHolder Is Nothing Then
    '        container = CType(mpContentPlaceHolder.FindControl("tcl"), TabContainer)
    '        If Not container Is Nothing Then
    '            For Each obj As Object In container.Controls
    '                If TypeOf obj Is TabPanel Then
    '                    Dim tabPanel As TabPanel = CType(obj, TabPanel)
    '                    Select Case tabPanel.ID
    '                        Case TabId,
    '                            tabPanel.Visible = True
    '                        Case Else
    '                            tabPanel.Enabled = False
    '                    End Select

    '                End If
    '            Next obj
    '        End If
    '    End If


    'End Sub
    Protected Sub SetActiveTab(ByVal SetActiveTab As String)
        Dim container As TabContainer
        Dim TabId As String = "TabPanel" & SetActiveTab
        Dim TabIDzero As String = "TabPanel0"
        mpContentPlaceHolder = CType(Page.Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
        If Not mpContentPlaceHolder Is Nothing Then
            container = CType(mpContentPlaceHolder.FindControl("tcl"), TabContainer)
            If Not container Is Nothing Then
                For Each obj As Object In container.Controls
                    If TypeOf obj Is TabPanel Then
                        Dim tabPanel As TabPanel = CType(obj, TabPanel)
                        Select Case tabPanel.ID
                            Case TabId, TabIDzero
                                tabPanel.Enabled = True
                            Case Else
                                tabPanel.Enabled = False
                        End Select

                    End If
                Next obj
            End If
        End If

    End Sub

    Protected Sub TabButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        'This is now panel click
        Launchmodal()

    End Sub
    'Protected Sub DealWithClick()
    '    Dim ActivePanelId As String = tcl.ActiveTab.ID.Substring(8)
    '    'Dim ActiveTab As Integer = tcl.ActiveTabIndex
    '    'Session.Add("userreason", ActiveTab)
    '    If ActivePanelId <> 0 Then
    '        Launchmodal()
    '    Else
    '        'LaunchTab(CLICKED, ActiveTab)
    '    End If
    'End Sub

    Protected Sub Launchmodal()

        AcceptLinacModalPopup.Hide()
        Dim UserReason As Integer = tcl.ActiveTab.ID.Substring(8)
        Dim returnstring As String = String.Empty
        If UserReason <> 0 Then
            If Not GetApplication.GetApplicationState(EquipmentID, 0) Then

                Dim ObjAccept As AcceptLinacuc = Page.LoadControl("Controls/AcceptLinacuc.ascx")
                ObjAccept.LinacName = EquipmentID
                ObjAccept.ID = "AcceptLinac1"
                ObjAccept.UserReason = UserReason
                AcceptLinacPlaceholder.Controls.Add(ObjAccept)
                DynamicControlSelection = ACCEPTLINACSELECTED
                AcceptLinacModalPopup.Show()
            Else
                Dim ReturnfromStatus As Boolean = HttpContext.Current.Session("returnFromLinacStatus")
                If ReturnfromStatus Then
                    Dim tab As String = GetApplication.Returnlastuserreason(EquipmentID, 0).ToString
                    'HttpContext.Current.Session.Remove("returnFromLinacStatus")
                    returnstring = EquipmentID + "page.aspx?TabAction=Clicked&NextTab=" + tab
                    Response.Redirect(returnstring)
                Else
                    'returnstring = EquipmentID + "Page.aspx"
                End If

            End If
        Else
            Session.Add("returnFromLinacStatus", True)
        End If

    End Sub


    Public Sub LaunchTab(ByVal TabAction As String, ByVal LiveTab As String)
        Dim Reload As Boolean = False
        Dim tabActive As Integer
        Dim lastState As String = String.Empty
        Dim lastuser As String = String.Empty
        Dim lastusergroup As Integer = 0
        Dim LoggedOn As Boolean = False
        Dim ActivePanel As String = String.Empty
        If Not IsPostBack Then
            LoggedOn = GetApplication.GetApplicationState(EquipmentID, 0)
            If Not LoggedOn Then
                Reload = True
            ElseIf LoggedOn Then
                If TabAction = "Recovered" Or TabAction = FAULT Then
                    Reload = True
                ElseIf LiveTab = 0 Then
                    Reload = False
                ElseIf HttpContext.Current.Session("returnFromLinacStatus").ToString Then
                    HttpContext.Current.Session.Remove("returnFromLinacStatus")
                    Reload = True
                Else
                    'This should autorecover
                    WriteRestore("when does here get to")
                    End If

            End If

            If Reload = True Then
                Try
                    Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
                    Using myscope As TransactionScope = New TransactionScope()

                        If Not LoggedOn Then
                            If TabAction = FAULT Then
                                Dim FaultTab As String = LiveTab
                                If Session("RecoverFault") Then
                                    'Check why this is this value
                                    FaultTab = 100
                                    Session.Remove("RecoverFault")

                                End If
                                Select Case FaultTab
                                    Case 1, 4, 5
                                        'This writes log on into linac status.Log on name is set to person logging fault
                                        lastState = Reuse.SetMachineState(EquipmentID, False, True, connectionString)

                                    Case Else
                                        'Do nothing for clinical, training or rad run up
                                End Select
                                'Now change active tab to repair
                                ActivePanel = Reuse.ReturnActivePanel(REPAIR)
                                LiveTab = REPAIR
                            Else
                                lastState = Reuse.SetMachineState(EquipmentID, False, True, connectionString)
                                ActivePanel = Reuse.ReturnActivePanel(LiveTab)
                            End If
                        Else
                            If TabAction = FAULT Then
                                ActivePanel = Reuse.ReturnActivePanel(REPAIR)
                                LiveTab = REPAIR
                            Else
                                ActivePanel = Reuse.ReturnActivePanel(LiveTab)
                            End If
                            lastState = Reuse.GetLastState(EquipmentID, 0)
                        End If
                        tcl.ActiveTabIndex = ActivePanel
                        Dim containerId As String = "TabContent" & LiveTab

                        Dim panel As Panel = tcl.ActiveTab.FindControl(containerId)
                        If (Not panel Is Nothing) Then
                            panel.Visible = True

                            Dim Activity As String = String.Empty
                            Dim User As String = String.Empty
                            'LoadTab(LiveTab)
                            Select Case LiveTab
                                Case 0

                                Case 1
                                    Dim rucontrol As ErunupUserControl = tcl.ActiveTab.FindControl(runupcontrolId)
                                    rucontrol.EngLogOnEvent(connectionString)
                                    rucontrol.Visible = True
                                    AcceptLinacModalPopup.Hide()
                                    DynamicControlSelection = String.Empty

                                Case 3
                                    Dim clinicalcontrol As ClinicalUserControl = tcl.ActiveTab.FindControl(ClinicalUserControlID)
                                    clinicalcontrol.Visible = True
                                    AcceptLinacModalPopup.Hide()
                                    DynamicControlSelection = String.Empty
                                    clinicalcontrol.ClinicalApprovedEvent(connectionString)

                                Case 4
                                    Dim plancontrol As Planned_Maintenanceuc = tcl.ActiveTab.FindControl(PlannedMaintenanceControlID)
                                    AcceptLinacModalPopup.Hide()
                                    DynamicControlSelection = String.Empty
                                    plancontrol.Visible = True

                                Case 5
                                    Dim repcontrol As Repairuc = tcl.ActiveTab.FindControl(repcontrolId)
                                    repcontrol.Repairlogon(connectionString)
                                    repcontrol.Visible = True
                                    AcceptLinacModalPopup.Hide()
                                    DynamicControlSelection = String.Empty

                                Case 9
                                    Dim emergencycontrol As ErunupUserControl = tcl.ActiveTab.FindControl(emergencycontrolID)
                                    emergencycontrol.EngLogOnEvent(connectionString)
                                    emergencycontrol.Visible = True
                                    AcceptLinacModalPopup.Hide()
                                    DynamicControlSelection = String.Empty

                                Case 8
                                    Dim trainingcontrol As Traininguc = tcl.ActiveTab.FindControl(trainingcontrolID)
                                    trainingcontrol.Visible = True
                                    AcceptLinacModalPopup.Hide()
                                    DynamicControlSelection = String.Empty

                            End Select
                            ActivityLabel.Text = Reuse.ReturnActivity(LiveTab)
                            Statelabel.Text = lastState
                            lastusergroup = HttpContext.Current.Session("usergroup")
                            SetUser(lastusergroup)
                            SetActiveTab(LiveTab)
                        End If
                        myscope.Complete()
                    End Using
                Catch ex As Exception
                    NewFaultHandling.LogError(ex)
                    'pop up message here?
                    'send back to page
                    Dim returnstring As String = EquipmentID + "page.aspx"
                    Response.Redirect(returnstring)
                End Try
            Else
                tabActive = tcl.ActiveTabIndex
                Dim containerId As String = "TabContent" & tabActive

            End If

        End If

    End Sub

    '    'eg from http://dotnetbites.wordpress.com/2014/02/15/call-parent-page-method-from-user-control-using-reflection/

    '    'From http://geekswithblogs.net/frankw/archive/2008/10/29/enable-back-button-support-in-asp.net-ajax-web-sites.aspx

    '    'http://stackoverflow.com/questions/17582081/how-to-open-aspx-web-pages-on-a-pop-up-window

    Protected Sub AcknowledgeEnergies()
        PopUpWindow("AcknowledgeEnergies.aspx", "Energies")
    End Sub
    Protected Sub PopUpWindow(ByVal Poppage As String, ByVal PageType As String)
        'http://www.aspsnippets.com/Articles/Open-New-Window-from-Server-Side-Code-Behind-in-ASPNet-using-C-and-VBNet.aspx
        Dim url As String = Poppage
        Dim PageName As String = PageType
        PageName = "popup_window_" & PageName
        'Dim DiagResult As Integer
        'DiagResult = Integer.Parse(inpHide.Value)
        Dim path As String = HttpContext.Current.Request.Url.AbsolutePath
        'from http://www.codestore.net/store.nsf/unid/DOMM-4PYJ3S?OpenDocument
        'Dim s As String = "window.open('" & url + "', 'PageName', 'width=800,height=700,left=100,top=100,resizable=yes, scrollbars=yes');"
        Dim s As String = "windowOpener('" & url + "', 'PageName', 'width=800,height=700,left=100,top=100,resizable=yes, scrollbars=yes');"
        'Dim s As String = "windowOpener('" & url + "', 'PageName', 'fullscreen=yes');"

        ClientScript.RegisterStartupScript(path.GetType(), PageName, s, True)

    End Sub

    Protected Sub EndOfDay_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EndOfDayButton.Click

        'Amended because a user could click button before it was hidden SPR 30

        EndOfDayButton.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(EndOfDayButton, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        Dim LoggedOn As Boolean = False
        Dim lastState As String

        Dim mpContentPlaceHolder As ContentPlaceHolder
        mpContentPlaceHolder = CType(Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
        If Not mpContentPlaceHolder Is Nothing Then
            LoggedOn = GetApplication.GetApplicationState(EquipmentID, 0)
            lastState = Reuse.GetLastState(EquipmentID, 0)

            If (LoggedOn) Or (lastState = "Fault") Then
                'tell user it can't be done
                Dim strScript As String = "<script>"

                If LoggedOn Then
                    strScript += "alert('Please complete current action first');"
                Else
                    strScript += "alert('Please Clear fault first');"
                End If
                strScript += "</script>"
                ScriptManager.RegisterStartupScript(EndOfDayButton, Me.GetType(), "JSCR", strScript.ToString(), False)
            Else
                wctrl = CType(mpContentPlaceHolder.FindControl("Writedatauc1"), WriteDatauc)
                Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
                wcbutton.Text = ENDOFDAY
                Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
                Session.Add("Actionstate", "Confirm")
                wctrl.Visible = True
                ForceFocus(wctext)
            End If
        End If

    End Sub

    ' this is an instrumentation field that displays application number ie 0 or 1
    'Public Sub UpdateHiddenLAField(ByVal message As String)
    '    LAHiddenFieldcontrol.Value = message
    '    Application1.Text = message
    '    'LAFieldcontrol.Text = message
    'End Sub
    Public Sub UpdateUserDisplay(ByVal message As Integer)
        SetUser(message)
    End Sub
    Public Sub UpdateDisplay(ByVal message As String)
        ActivityLabel.Text = message
    End Sub
    Public Sub UpdateButtons()
        'Update_ReturnButtons()
    End Sub
    'instrumentation code
    Public Sub Updateuserlabel(ByVal message As String)
        'UserLabel.Text = message
    End Sub

    Public Sub Updatestatedisplay(ByVal message As String)
        Statelabel.Text = message
    End Sub

    Protected Sub Timer1_Tick(sender As Object, e As System.EventArgs) Handles Timer1.Tick

        Dim ResetDay As String = Nothing
        ResetDay = Reuse.GetLastTime(EquipmentID, 0)

        Select Case ResetDay
            Case "Ignore"
                        'Ignore
            Case ENDOFDAY
                EndofDayElf(ResetDay)
            Case "Error"
                'Do nothing
        End Select

    End Sub
    'This run when end of day hasn't been completed the day before
    Protected Sub EndofDayElf(ByVal Caller As String)
        Dim returnstring As String = EquipmentID + "page.aspx"
        Dim mrucontrol As ErunupUserControl
        Dim mclincontrol As ClinicalUserControl
        Dim mplancontrol As Planned_Maintenanceuc
        Dim mrepcontrol As Repairuc
        Dim mtrainingcontrol As Traininguc
        Dim grdview As GridView
        Dim Logoffuser As String = "System"
        Dim lastState As String
        Dim activetab As Integer
        Dim suspendnull As String = Nothing
        Dim Successful As Boolean = False
        Dim ThereIsAFaultOpen As Boolean
        lastState = Reuse.GetLastState(EquipmentID, 0)
        ThereIsAFaultOpen = NewFaultHandling.CheckForOpenFault(EquipmentID)
        activetab = GetApplication.Returnlastuserreason(EquipmentID, 0)

        If GetApplication.GetApplicationState(EquipmentID, 0) Then

            Select Case activetab

                Case 1, 9
                    If activetab = 1 Then
                        If tcl.ActiveTab.FindControl(runupcontrolId) Is Nothing Then
                            mrucontrol = Page.LoadControl("ErunupUserControlCommon.ascx")
                            mrucontrol.ID = runupcontrolId
                            mrucontrol.LinacName = EquipmentID
                        Else
                            mrucontrol = tcl.ActiveTab.FindControl(runupcontrolId)
                        End If

                    Else
                        If tcl.ActiveTab.FindControl(emergencycontrolID) Is Nothing Then
                            mrucontrol = Page.LoadControl("ErunupUserControlCommon.ascx")
                            mrucontrol.ID = emergencycontrolID
                            mrucontrol.LinacName = EquipmentID
                        Else
                            mrucontrol = tcl.ActiveTab.FindControl(emergencycontrolID)
                        End If

                    End If

                    grdview = mrucontrol.FindControl("Gridview1")
                    Session.Add("ActionState", ENDOFDAY)
                    mrucontrol.UserApprovedEvent(activetab, Logoffuser)

                Case 3
                    If tcl.ActiveTab.FindControl(ClinicalUserControlID) Is Nothing Then
                        mclincontrol = Page.LoadControl("ClinicalUserControl.ascx")
                        mclincontrol.ID = ClinicalUserControlID
                        mclincontrol.LinacName = EquipmentID
                    Else
                        mclincontrol = tcl.ActiveTab.FindControl(ClinicalUserControlID)
                    End If
                    Session.Add("ActionState", ENDOFDAY)
                    mclincontrol.UserApprovedEvent(activetab, Logoffuser)

                Case 4
                    If tcl.ActiveTab.FindControl(PlannedMaintenanceControlID) Is Nothing Then
                        mplancontrol = Page.LoadControl("PlannedMaintenanceuc.ascx")
                        mplancontrol.ID = PlannedMaintenanceControlID
                        mplancontrol.LinacName = EquipmentID
                    Else
                        mplancontrol = tcl.ActiveTab.FindControl(PlannedMaintenanceControlID)
                    End If
                    Session.Add("Actionstate", ENDOFDAY)
                    mplancontrol.UserApprovedEvent(activetab, Logoffuser)

                Case 5
                    If tcl.ActiveTab.FindControl(repcontrolId) Is Nothing Then
                        mrepcontrol = Page.LoadControl("Repairuc.ascx")
                        mrepcontrol.ID = repcontrolId
                        mrepcontrol.LinacName = EquipmentID

                    Else
                        mrepcontrol = tcl.ActiveTab.FindControl(repcontrolId)
                    End If

                    If ThereIsAFaultOpen Then
                        'This means there are still open faults
                        If Caller = ENDOFDAY Then

                            mrepcontrol.RemoteLockElf(False)
                        Else
                            mrepcontrol.RemoteLockElf(False)
                        End If
                    Else 'dont think this gets called now.
                        If lastState = FAULT Then
                            'This means there were open faults but they have been closed so need to close them off.
                            mrepcontrol.WriteFaultIDTable()
                        End If

                        Session.Add("Actionstate", ENDOFDAY)
                        mrepcontrol.UserApprovedEvent(activetab, Logoffuser)
                    End If

                Case 8
                    If tcl.ActiveTab.FindControl(trainingcontrolID) Is Nothing Then
                        mtrainingcontrol = Page.LoadControl("Traininguc.ascx")
                        mtrainingcontrol.ID = trainingcontrolID
                        mtrainingcontrol.LinacName = EquipmentID
                    Else
                        mtrainingcontrol = tcl.ActiveTab.FindControl(trainingcontrolID)
                    End If

                    Session.Add("Actionstate", ENDOFDAY)
                    mtrainingcontrol.UserApprovedEvent(activetab, Logoffuser)

            End Select
        Else
            If ThereIsAFaultOpen = False Then
                'this is to make sure that equivalent of end of day happens
                'Only want this to happen if RunUpDone or suspended but no one is logged on.
                'If lastState = SUSPENDED Then
                Reuse.SetStatus(Logoffuser, UNAUTHORISED, 5, 102, EquipmentID, 10, False)
                'End If
            Else
                Reuse.SetStatus(Logoffuser, FAULT, 5, 102, EquipmentID, 10, False)
                NewFaultHandling.UpdateLastNonFaultState(EquipmentID)
            End If
        End If
        'If Not ThereIsAFaultOpen Then
        Response.Redirect(returnstring)
        'End If

    End Sub

    Protected Sub RestoreButton_Click(sender As Object, e As System.EventArgs) Handles RestoreButton.Click
        WriteRestore("Restore")
    End Sub

    'From http://www.pberblog.com/blog/set-focus-to-a-control-of-a-modalpopupextender-programmatically/
    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" + ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

    Private Sub recoverbuttonscript()
        Dim strScript As String = "<script>"
        strScript += "alert('Elf has been Restored');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(RestoreButton, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Sub WriteRestore(ByVal RestoreReason As String)
        Dim activetab As String
        Dim susstate As String = Nothing
        Dim RunUpBoolean As String = Nothing
        Dim Logoffuser As String = String.Empty
        Dim reader As SqlDataReader
        Dim Status As String = ""
        Dim Activity As Integer
        Dim Radio As String = "101"
        Dim conn As SqlConnection
        Dim conActivity As SqlCommand
        Dim connectionString As String = ConfigurationManager.ConnectionStrings(
        "connectionstring").ConnectionString
        Dim mpContentPlaceHolder As ContentPlaceHolder
        Dim grdview As GridView
        Dim breakdown As Boolean = False
        Dim MachineLabel As String = EquipmentID & "Page.aspx';"
        Dim strScript As String = "<script>"
        Dim Valid As Boolean = False
        Dim Comment As String = "Recovered"
        strScript += "window.location='"
        strScript += EquipmentID
        strScript += "</script>"
        Dim returnstring As String = String.Empty
        Dim mrucontrol As ErunupUserControl
        Dim mclincontrol As ClinicalUserControl
        Dim mplancontrol As Planned_Maintenanceuc
        Dim mrepcontrol As Repairuc
        Dim mtrainingcontrol As Traininguc

        If RestoreReason = "Restore" Then
            Logoffuser = "Restored"
        ElseIf RestoreReason = "Reload" Then
            Logoffuser = "Reload"
        ElseIf RestoreReason = "RecoverFault" Then
            Logoffuser = "Restored"
        End If
        Try

            mpContentPlaceHolder = CType(FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
            If Not mpContentPlaceHolder Is Nothing Then
                grdview = CType(mpContentPlaceHolder.FindControl("DummyGridview"), GridView)
            End If

            conn = New SqlConnection(connectionString)

            conActivity = New SqlCommand("SELECT state, userreason FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

            conActivity.Parameters.AddWithValue("@linac", EquipmentID)
            conn.Open()
            reader = conActivity.ExecuteReader()

            If reader.Read() Then
                Status = reader.Item("State")
                Activity = reader.Item("userreason")
            End If
            reader.Close()
            conn.Close()
            'If Not breakdown Then
            activetab = Activity
            If RestoreReason = "Reload" Or RestoreReason = "Restore" Then
                If GetApplication.GetApplicationState(EquipmentID, 0) Then
                    returnstring = EquipmentID + "page.aspx?TabAction=Recovered&NextTab=" & activetab
                Else
                    returnstring = EquipmentID + "page.aspx"
                End If
                Response.Redirect(returnstring, False)
                'Context.ApplicationInstance.CompleteRequest()
            ElseIf RestoreReason = "RecoverFault" Then

            Else

                Select Case activetab

                    Case 1
                        'only need dummy gridview when passing to commit run up not when using runup control
                        'tab 666 is for commit run up - same as for fault condition
                        If activetab = 1 Then
                            If tcl.ActiveTab.FindControl(runupcontrolId) Is Nothing Then
                                mrucontrol = Page.LoadControl("ErunupUserControlCommon.ascx")
                                mrucontrol.ID = runupcontrolId
                                mrucontrol.LinacName = EquipmentID
                            Else
                                mrucontrol = tcl.ActiveTab.FindControl(runupcontrolId)
                            End If

                        Else
                            If tcl.ActiveTab.FindControl(emergencycontrolID) Is Nothing Then
                                mrucontrol = Page.LoadControl("ErunupUserControlCommon.ascx")
                                mrucontrol.ID = emergencycontrolID
                                mrucontrol.LinacName = EquipmentID
                            Else
                                mrucontrol = tcl.ActiveTab.FindControl(emergencycontrolID)
                            End If

                        End If

                        Session.Add("Actionstate", "Cancel")
                        mrucontrol.UserApprovedEvent(activetab, Logoffuser)
                    Case 3
                        If tcl.ActiveTab.FindControl(ClinicalUserControlID) Is Nothing Then
                            mclincontrol = Page.LoadControl("ClinicalUserControl.ascx")
                            mclincontrol.ID = ClinicalUserControlID
                            mclincontrol.LinacName = EquipmentID
                        Else
                            mclincontrol = tcl.ActiveTab.FindControl(ClinicalUserControlID)
                        End If
                        Session.Add("Actionstate", "Recover")

                        mclincontrol.UserApprovedEvent(activetab, Logoffuser)

                    Case 4, 5, 6, 8
                        Dim laststate As String = Reuse.GetLastState(EquipmentID, 0)

                        Select Case activetab
                            Case 4
                                If tcl.ActiveTab.FindControl(PlannedMaintenanceControlID) Is Nothing Then
                                    mplancontrol = Page.LoadControl("PlannedMaintenanceuc.ascx")
                                    mplancontrol.ID = PlannedMaintenanceControlID
                                    mplancontrol.LinacName = EquipmentID
                                Else
                                    mplancontrol = tcl.ActiveTab.FindControl(PlannedMaintenanceControlID)
                                End If
                                Session.Add("Actionstate", "Recover")
                                mplancontrol.UserApprovedEvent(activetab, Logoffuser)

                            Case 5
                                If tcl.ActiveTab.FindControl(repcontrolId) Is Nothing Then
                                    mrepcontrol = Page.LoadControl("Repairuc.ascx")
                                    mrepcontrol.ID = repcontrolId
                                    mrepcontrol.LinacName = EquipmentID

                                Else
                                    mrepcontrol = tcl.ActiveTab.FindControl(repcontrolId)
                                End If
                                Session.Add("Actionstate", "Recover")
                                mrepcontrol.UserApprovedEvent(activetab, Logoffuser)
                            Case 8
                                If tcl.ActiveTab.FindControl(trainingcontrolID) Is Nothing Then
                                    mtrainingcontrol = Page.LoadControl("Traininguc.ascx")
                                    mtrainingcontrol.ID = trainingcontrolID
                                    mtrainingcontrol.LinacName = EquipmentID
                                Else
                                    mtrainingcontrol = tcl.ActiveTab.FindControl(trainingcontrolID)
                                End If
                                Session.Add("Actionstate", "Recover")
                                mtrainingcontrol.UserApprovedEvent(activetab, Logoffuser)
                        End Select

                    Case Else
                        'This caters for when the system is already idling as it were.

                        returnstring = EquipmentID + "page.aspx?TabAction=Recovered&NextTab=" & activetab
                        Response.Redirect(returnstring, False)

                        'Context.ApplicationInstance.CompleteRequest()
                End Select

            End If

        Catch ex As Exception
            NewFaultHandling.LogError(ex)
            'pop up error message here
        End Try
    End Sub

End Class
