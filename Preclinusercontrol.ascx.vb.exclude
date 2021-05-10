Imports System.Data.SqlClient
Imports System.Data
Imports AjaxControlToolkit
Imports System.Web.UI.Page
Partial Class Preclinusercontrol
    Inherits System.Web.UI.UserControl
    Private mpContentPlaceHolder As ContentPlaceHolder
    Private appstate As String
    Private suspstate As String
    Private actionstate As String
    Private faultviewstate As String
    Private LinacFlag As String
    'Private objconToday As TodayClosedFault
    'Private Todaydefect As DefectSave
    Private BoxChanged As String
    Private tabstate As String
    Dim accontrol As AcceptLinac
    Public Property LinacName() As String
    Public Property DataName() As String
    Dim comment As String
    Const PRECLIN As String = "2"
    Const REPEATFAULT As Integer = 0
    Const FAULTPOPUPSELECTED As String = "faultpopupupselected"
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"
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

    Public Function FormatImage(ByVal energy As Boolean) As String
        'Dim happyIcon As String = "Images/happy.gif"
        Dim happyIcon As String = "Images/check_mark.jpg"
        'Dim sadIcon As String = "Images/sad.gif"
        Dim sadIcon As String = "Images/box_with_x.jpg"

        If (energy) Then
            Return happyIcon
        Else
            Return sadIcon
        End If

    End Function
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AddHandler WriteDatauc1.UserApproved, AddressOf UserApprovedEvent
        AddHandler ConfirmPage1.ConfirmExit, AddressOf ConfirmExitEvent ' this is if imaging wasn't selected
        Dim tabcontainer1 As TabContainer
        Page = Me.Page
        mpContentPlaceHolder =
        CType(Page.Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
        If Not mpContentPlaceHolder Is Nothing Then
            tabcontainer1 = CType(mpContentPlaceHolder.
                FindControl("tcl"), TabContainer)
            If Not tabcontainer1 Is Nothing Then
                Dim panelcontrol As TabPanel = tabcontainer1.FindControl("TabPanel2")
                'accontrol = panelcontrol.FindControl("AcceptLinac2")
                'AddHandler accontrol.PreRunuploaded, AddressOf Preclinloaded

            End If
        End If
        appstate = "LogOn" + LinacName
        actionstate = "ActionState" + LinacName
        suspstate = "Suspended" + LinacName
        faultviewstate = "Faultsee" + LinacName
        LinacFlag = "State" + LinacName
        BoxChanged = "PreCBoxChanged" + LinacName
        tabstate = "ActTab" + LinacName

    End Sub
    Public Sub Preclinloaded(ByVal connectionString As String)
        BindGridview2(connectionString)
        'BindComments(connectionString)
        SetImaging(connectionString)

    End Sub
    Private Sub BindGridview2(ByVal connectionString As String)
        Dim SqlDateSourceGridView As New SqlDataSource()
        Dim query As String = "select HandoverId, MV6,ISNULL(MV6FFF, 0) as ""MV6FFF"",MV10,ISNULL(MV10FFF, 0) as ""MV10FFF"",ISNULL(MeV4,0) as ""MeV4"", MEV6, MEV8, MeV10, MeV12, MeV15, MeV18, MeV20, Comment, LogOutName, LogOutDate, linac, LogInDate, Duration, LogInStatusID, Approved, LogInName, LogOutStatusID From handoverenergies Where handoverid = (Select Max(handoverid) As mancount from [handoverenergies] where linac=@linac)"
        SqlDateSourceGridView = QuerySqlConnection(LinacName, query, connectionString)
        GridView2.DataSource = SqlDateSourceGridView
        GridView2.DataBind()
    End Sub
    'Private Sub BindComments(ByVal connectionString As String)
    '    Dim SqlDateSourceComment As New SqlDataSource()
    '    Dim query As String = "select e.comment from handoverenergies e  where e.handoverid = (Select Max(handoverid) as mancount from [handoverenergies] where linac=@linac)"
    '    SqlDateSourceComment = QuerySqlConnection(LinacName, query, connectionString)
    '    GridViewComments.DataSource = SqlDateSourceComment
    '    GridViewComments.DataBind()

    'End Sub
    Protected Sub SetImaging(ByVal connectionString As String)
        'This had to be changed to add the imaging modalities for E1 and E2 - added 44 and 45 and 56 and 57. Altered this 2/10 because energyids are not always
        'sequential. Changed to check energy instead which is what is used successfully elsewhere
        'SqlDataSource1.SelectCommand = "SELECT * FROM [physicsenergies] where linac= @linac and EnergyID in (29,30,31,32,33,44,45, 56,57)"
        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = connectionString,
            .SelectCommand = "SELECT * FROM [physicsenergies] where linac=@linac and Energy in ('iView','XVI')"
        }

        SqlDataSource1.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", LinacName)

        GridViewImage.DataSource = SqlDataSource1
        GridViewImage.DataBind()

        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim reader As SqlDataReader
        Dim count As Integer = 0

        conn = New SqlConnection(connectionString)
        'This had to be changed to add the imaging modalities for E1 and E2 - added 44 and 45 and 56 and 57. Altered this 2/10 because energyids are not always
        'sequential. Changed to check energy instead which is what is used successfully elsewhere
        'comm = New SqlCommand("SELECT EnergyID, Approved FROM physicsenergies where linac=@linac and EnergyID in (29,30,31,32,33, 44, 45, 56,57)", conn)
        comm = New SqlCommand("SELECT EnergyID, Approved FROM physicsenergies where linac=@linac and Energy in ('iView','XVI')", conn)


        comm.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
        comm.Parameters("@linac").Value = LinacName
        'Try
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
                Dim cb As CheckBox = CType(GridViewImage.Rows(count).FindControl("RowLevelCheckBoxImage"), CheckBox)
                cb.Enabled = False
                cb.Visible = False
            End If

            count = count + 1
        End While
        reader.Close()

    End Sub

    Protected Sub Update_DefectDailyDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            'Don't need if because report fault pop up is the same for both defects now
            'If LinacName Like "T?" Then
            'If LinacName Like "T?" Then
            'Todaydefectpark = PlaceHolderDefectSave.FindControl("DefectDisplay")
            'Todaydefectpark.UpDateDefectsEventHandler()
            'Else
            'Todaydefect = PlaceHolderDefectSave.FindControl("DefectDisplay")
            'Todaydefect.UpDateDefectsEventHandler()
            'MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            'MainFaultPanel.Update_defectsToday(LinacName)
            'ReportFaultPopUpuc1.Visible = False
            'End If

        End If
    End Sub

    Protected Sub Update_ViewOpenFaults(ByVal EquipmentID As String)
        'If LinacName = EquipmentID Then
        '    Dim updatefault As ViewOpenFaults = FindControl("ViewOpenFaults")
        '    updatefault.RebindViewFault()
        'End If
    End Sub

    Protected Sub ConfirmExitEvent()
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        wcbutton.Text = "Confirm Pre-Clinical"
        Application(actionstate) = "Confirm"
        WriteDatauc1.Visible = True
        ForceFocus(wctext)
    End Sub

    Public Sub UserApprovedEvent(ByVal Tabused As String, ByVal Userinfo As String)
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim username As String = Userinfo
        'Set these specifically to false 2/12/16
        Dim Valid As Boolean = False
        Dim iView As Boolean = False
        Dim XVI As Boolean = False
        Dim Successful As Boolean = False
        If Tabused = "2" Then
            If (Not HttpContext.Current.Application(BoxChanged) Is Nothing) Then
                comment = HttpContext.Current.Application(BoxChanged).ToString
            Else
                comment = String.Empty
            End If
            Dim strScript As String = "<script>"
            Dim Action As String = Application(actionstate)
            Dim grdviewI As GridView = FindControl("GridViewImage")
            Dim FaultP As DavesCode.FaultParameters = New DavesCode.FaultParameters()
            'this changed 21 aug to allow to move on to other states so suspstate is made to be suspended
            Application(appstate) = 0

            If Action = "Confirm" Then
                Application(LinacFlag) = "Clinical"
                Valid = True
                DavesCode.Reuse.ReturnImaging(iView, XVI, grdviewI, LinacName)
                Successful = DavesCode.NewPreClinRunup.CommitPreClin(LinacName, username, comment, iView, XVI, Valid, False, FaultP)
                If Successful Then
                    Dim returnstring As String = LinacName + "page.aspx?tabref=3"
                    Application(tabstate) = String.Empty
                    CommentBox.ResetCommentBox(String.Empty)
                    Application(suspstate) = 1
                    Response.Redirect(returnstring)
                Else
                    RaiseError()
                End If

            Else
                Application(LinacFlag) = "Engineering Approved"
                Valid = False
                strScript += "alert('No Pre-clinical RunUp Logging Off');"
                Successful = DavesCode.NewPreClinRunup.CommitPreClin(LinacName, username, comment, iView, XVI, Valid, False, FaultP)
                If Successful Then
                    If Not Userinfo = "Restored" Then
                        CommentBox.ResetCommentBox(String.Empty)
                    End If
                    strScript += "window.location='"
                    strScript += machinelabel
                    strScript += "</script>"
                    ScriptManager.RegisterStartupScript(LogOff, Me.GetType(), "JSCR", strScript.ToString(), False)
                Else
                    RaiseError()
                End If
            End If
        End If
    End Sub
    Protected Sub RaiseError()
        Dim message As String
        message = "alert('Problem recording Pre-clin run up. Logging off without Approving for Clinical');"

        Dim strScript As String = "<script>"
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Application(LinacFlag) = "Linac Unauthorised"
        Application(appstate) = 0
        CommentBox.ResetCommentBox(String.Empty)
        Application(tabstate) = String.Empty
        strScript += message
        strScript += "window.location='"
        strScript += machinelabel
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(clinHandoverButton, Me.GetType(), "JSCR", strScript.ToString(), False)

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        WaitButtons("Rad")
        'TodayRepeatFaultDisplay = Page.LoadControl("controls\RepeatFaultDisplayuc.ascx")
        'TodayRepeatFaultDisplay.ID = "TodayRepeatFaultDisplay"
        'TodayRepeatFaultDisplay.LinacName = LinacName
        'PlaceHolderTodaysRepeatFaultsDisplay.Controls.Add(TodayRepeatFaultDisplay)


        'Dim objMFG As UserControl = Page.LoadControl("ManyFaultGriduc.ascx")
        'CType(objMFG, ManyFaultGriduc).NewFault = False
        'CType(objMFG, ManyFaultGriduc).IncidentID = REPEATFAULT
        ''to accomodate Tomo now need to pass equipment name?
        'CType(objMFG, ManyFaultGriduc).MachineName = LinacName
        'PlaceHolderFaults.Controls.Add(objMFG)
        Select Case Me.DynamicControlSelection
        '    Case REPEATFAULTSELECTED
            '        LoadRepeatFaultTable(HiddenIncidentID.Value, HiddenConcessionNumber.Value)
            Case FAULTPOPUPSELECTED
                '        'LoadFaultTable(Label2.Text)
                'ReloadConcessionPopUp()

                Dim objReportFault As controls_ReportFaultPopUpuc = Page.LoadControl("controls\ReportFaultPopUpuc.ascx")
                objReportFault.LinacName = LinacName
                objReportFault.ID = "ReportFaultPopupuc"
                objReportFault.ParentControl = PRECLIN
                'objReportFault.Visible = False
                AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay
                AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).UpdateViewOpenFaults, AddressOf Update_ViewOpenFaults
                AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).CloseReportFaultPopUp, AddressOf Close_ReportFaultPopUp
                ReportFaultPopupPlaceHolder.Controls.Add(objReportFault)

            Case Else
                '        'no dynamic controls need to be loaded...yet
        End Select


        Dim objMFG As controls_MainFaultDisplayuc = Page.LoadControl("controls\MainFaultDisplayuc.ascx")
        CType(objMFG, controls_MainFaultDisplayuc).LinacName = LinacName
        PlaceHolderFaults.Controls.Add(objMFG)

        'objconToday = Page.LoadControl("TodayClosedFault.ascx")
        'objconToday.ID = "Todaysfaults"
        'objconToday.LinacName = LinacName
        'PlaceHolderTodaysclosedfaults.Controls.Add(objconToday)

        'Dim objCon As ViewOpenFaults = Page.LoadControl("ViewOpenFaults.ascx")
        'CType(objCon, ViewOpenFaults).LinacName = LinacName
        'CType(objCon, ViewOpenFaults).ParentControl = PRECLIN
        'CType(objCon, ViewOpenFaults).ID = "ViewOpenFaults"
        'AddHandler CType(objCon, ViewOpenFaults).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay

        Dim button1 As Button = FindControl("clinHandoverButton")
        Dim button2 As Button = FindControl("LogOff")
        'PlaceHolderViewOpenFaults.Controls.Add(objCon)

        'Dim objDefect As UserControl = Page.LoadControl("DefectSave.ascx")
        'CType(objDefect, DefectSave).ID = "DefectDisplay"
        'CType(objDefect, DefectSave).LinacName = LinacName
        'CType(objDefect, DefectSave).ParentControl = PRECLIN
        'PlaceHolderDefectSave.Controls.Add(objDefect)
        'AddHandler CType(objDefect, DefectSave).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay

        'AddHandler CType(objDefect, DefectSave).UpdateViewOpenFaults, AddressOf Update_ViewOpenFaults

        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        wctrl.LinacName = LinacName
        Dim Vctrl As ViewCommentsuc = CType(FindControl("ViewCommentsuc1"), ViewCommentsuc)
        Vctrl.LinacName = LinacName

        'The solution of how to pass parameter to dynamically loaded user control is from here:
        'http://weblogs.asp.net/aghausman/archive/2009/04/15/how-to-pass-parameters-to-the-dynamically-added-user-control.aspx

        PlaceHolder2.Visible = True
        CommentBox.BoxChanged = BoxChanged
        If Not IsPostBack Then

            Select Case LinacName
                Case "LA1"
                    For index As Integer = 2 To 5
                        GridView2.Columns(index).Visible = False
                    Next

                Case "LA2", "LA3"
                    For index As Integer = 2 To 5
                        GridView2.Columns(index).Visible = False
                    Next
                    GridView2.Columns(3).Visible = True
                Case "LA4"

                    For index As Integer = 2 To 12
                        GridView2.Columns(index).Visible = False
                    Next
                    GridView2.Columns(3).Visible = True
                Case "E1", "E2", "B1"

                    For index As Integer = 11 To 12
                        GridView2.Columns(index).Visible = False
                    Next
                Case Else
                    'All columns are valid and are displayed

            End Select
            GridView2.Columns(0).Visible = False

            Application(faultviewstate) = 1

        End If

    End Sub
    Protected Sub RaiseLoadError()
        Dim machinelabel As String = LinacName & "Page.aspx';"

        Application(appstate) = 0
        HttpContext.Current.Application(BoxChanged) = Nothing
        Application(tabstate) = String.Empty
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Loading Page. Please call Engineer');"
        strScript += "window.location='"
        strScript += machinelabel
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(clinHandoverButton, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Sub EnergyGridView_DataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound

        Dim headerRow As GridViewRow = e.Row
        Dim energy As Integer
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim reader As SqlDataReader
        Dim count As Integer
        Dim connectionString1 As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        conn = New SqlConnection(connectionString1)
        'This query has to be changed to accommodate E1 and LA6
        comm = New SqlCommand("Select  MV6, MV6FFF, MV10, MV10FFF, MeV4, MeV6, MeV8, MeV10, MeV12, MeV15, MeV18, MeV20 from HandoverEnergies where HandoverID  = (Select max(HandoverID) As lastrecord from HandoverEnergies where linac=@linac)", conn)
        comm.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
        comm.Parameters("@linac").Value = LinacName
        If headerRow.RowType = DataControlRowType.Header Then
            'Try
            conn.Open()
            reader = comm.ExecuteReader()
            While reader.Read()
                For count = 0 To reader.FieldCount - 1
                    'modified to account for dbnull 5/7/17
                    If Not reader.GetValue(count) Is System.DBNull.Value Then
                        energy = reader.GetValue(count)
                    Else
                        energy = 0
                    End If
                    Select Case energy
                        Case -1
                            headerRow.Cells(count + 1).BackColor = System.Drawing.Color.Green
                        Case 0
                            headerRow.Cells(count + 1).BackColor = System.Drawing.Color.Red
                    End Select

                Next
            End While

            reader.Close()

            conn.Close()

        End If

    End Sub

    Protected Sub ClinHandoverButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles clinHandoverButton.Click

        Dim counter As Integer = 0

        For Each grv As GridViewRow In GridViewImage.Rows

            Dim checktick As CheckBox = CType(grv.FindControl("RowlevelCheckBoxImage"), CheckBox)
            If checktick.Checked = True Then
                counter = counter + 1
            End If
        Next
        If counter <> 0 Then
            Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
            Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
            Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
            wcbutton.Text = "Confirm Pre-Clinical"
            Application(actionstate) = "Confirm"
            WriteDatauc1.Visible = True
            ForceFocus(wctext)

        Else
            Dim cptrl As ConfirmPage = CType(FindControl("ConfirmPage1"), ConfirmPage)
            Dim cpbutton As Button = CType(cptrl.FindControl("AcceptOK"), Button)
            cpbutton.Text = "Confirm No Imaging"
            ConfirmPage1.Visible = True

        End If

    End Sub
    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" +
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

    Protected Sub LogOff_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LogOff.Click
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        wcbutton.Text = "Log Off"
        Application(actionstate) = "Cancel"
        WriteDatauc1.Visible = True
        ForceFocus(wctext)
    End Sub

    Protected Function QuerySqlConnection(ByVal MachineName As String, ByVal query As String, ByVal connectionString As String) As SqlDataSource
        'This uses the sqldatasource instead of the individual conn definitions so reader can't be used
        'this solution is from http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.sqldatasource.select%28v=vs.90%29.aspx

        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = connectionString,
            .SelectCommand = (query)
        }
        SqlDataSource1.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", MachineName)

        Return SqlDataSource1

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

    Protected Sub Close_ReportFaultPopUp(ByVal EquipmentId As String, ByVal ErrorStatus As Boolean)
        If LinacName = EquipmentId Then
            'Don't need if because report fault pop up is the same for both defects now
            'If LinacName Like "T?" Then
            'Todaydefectpark = PlaceHolderDefectSave.FindControl("DefectDisplay")
            'Todaydefectpark.UpDateDefectsEventHandler()
            'Else
            'Todaydefect = PlaceHolderDefectSave.FindControl("DefectDisplay")
            'Todaydefect.UpDateDefectsEventHandler()
            DynamicControlSelection = String.Empty
            Dim ReportFault As controls_ReportFaultPopUpuc = CType(FindControl("ReportFaultPopupuc"), controls_ReportFaultPopUpuc)
            ReportFaultPopupPlaceHolder.Controls.Remove(ReportFault)
            'End If

        End If
    End Sub

    Protected Sub ReportFaultButton_Click(sender As Object, e As EventArgs) Handles ReportFaultButton.Click
        'Need to load reportfaultpopupuc here to pass comment box
        Dim CommentControl As controls_CommentBoxuc = FindControl("CommentBox")
        Dim DaTxtBox As TextBox = CommentControl.FindControl("TextBox")
        Dim Comment As String = DaTxtBox.Text
        Application("TabComment") = Comment

        Dim objReportFault As controls_ReportFaultPopUpuc = Page.LoadControl("controls\ReportFaultPopUpuc.ascx")
        objReportFault.LinacName = LinacName
        objReportFault.ID = "ReportFaultPopupuc"
        objReportFault.ParentControl = PRECLIN
        DynamicControlSelection = FAULTPOPUPSELECTED

        AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).UpDateDefectDailyDisplay, AddressOf Update_DefectDailyDisplay
        AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).UpdateViewOpenFaults, AddressOf Update_ViewOpenFaults
        AddHandler CType(objReportFault, controls_ReportFaultPopUpuc).CloseReportFaultPopUp, AddressOf Close_ReportFaultPopUp
        ReportFaultPopupPlaceHolder.Controls.Add(objReportFault)

    End Sub

End Class
