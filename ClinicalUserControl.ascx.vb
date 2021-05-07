Imports System.Data.SqlClient
Imports System.Drawing
Imports AjaxControlToolkit
Imports System.Transactions


Partial Class ClinicalUserControl
    Inherits UserControl

    Private mpContentPlaceHolder As ContentPlaceHolder
    Private CurrentCID As Integer
    Private colourstart As Color = Color.FromArgb(255, 204, 0)
    Private colourstop As Color = Color.FromArgb(102, 153, 153)
    Private StateLabel As Label
    Private ActivityLabel As Label
    Private appstate As String
    Private suspstate As String
    Private actionstate As String
    Private FaultOriginTab As String
    Private RunUpDone As String
    Private faultviewstate As String
    Private clinicalstate As String
    Private returnclinical As String
    Private LinacFlag As String
    Private objconToday As TodayClosedFault
    Private Todaydefect As DefectSave
    Private TodaydefectPark As DefectSavePark
    Dim BoxChanged As String
    Dim RunupBoxChanged As String
    'Dim accontrol As AcceptLinac
    Private tabstate As String
    Private TodayComment As controls_CommentBoxuc
    Const CLINICAL As String = "3"
    Const FAULTPOPUPSELECTED As String = "faultpopupupselected"
    Const MODALITYDISPLAY As String = "ModalityDisplayLoaded"
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"
    Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
    Dim Modalities As controls_ModalityDisplayuc
    Private MainFaultPanel As controls_MainFaultDisplayuc
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

    Public Function FormatImage(ByVal energy As Boolean) As String

        Dim happyIcon As String = "Images/check_mark.jpg"

        Dim sadIcon As String = "Images/box_with_x.jpg"
        If energy Then
            Return happyIcon
        Else
            Return sadIcon
        End If
    End Function

    Protected Sub Update_ClosedFaultDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_FaultClosedDisplay(LinacName)
        End If
    End Sub


    ' This updates the defect display on defectsave etc when repeat fault from viewopenfaults
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
        'The method of finding acceptlinac3 started from here http://forums.asp.net/t/1380670.aspx?Access+Master+page+properties+from+User+Control

        Dim tabcontainer1 As TabContainer
        Page = Me.Page
        mpContentPlaceHolder =
        CType(Page.Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
        If Not mpContentPlaceHolder Is Nothing Then
            tabcontainer1 = CType(mpContentPlaceHolder.
                FindControl("tcl"), TabContainer)
            If Not tabcontainer1 Is Nothing Then
                Dim panelcontrol As TabPanel = tabcontainer1.FindControl("TabPanel3")

            End If
        End If

        AddHandler WriteDatauc2.UserApproved, AddressOf UserApprovedEvent

        If Not mpContentPlaceHolder Is Nothing Then
            StateLabel = CType(mpContentPlaceHolder.
                FindControl("StateLabel"), Label)
            ActivityLabel = CType(mpContentPlaceHolder.FindControl("ActivityLabel"), Label)
        End If

        BoxChanged = "ClinBoxChanged" + LinacName
        RunupBoxChanged = "RUBoxChanged" + LinacName

    End Sub

    Public Sub ClinicalApprovedEvent(ByVal connectionString As String)
        'This is the point that the gridviews are displayed and Clinical Table Data is written
        'BindEnergyData()

        'This looks to see if BoxChanged has a value. if it has the comment has not been saved.
        If Not HttpContext.Current.Application(BoxChanged) Is Nothing Then
            HiddenFieldLinacState.Value = DavesCode.NewCommitClinical.NewWriteClinicalTable(LinacName, HttpContext.Current.Application(BoxChanged), connectionString)
            CommentBox.ResetCommentBox(String.Empty)
        End If
        BindComments()
        BindRunUpComments(connectionString)
        'ModalityDisplays(connectionString)
        HiddenFieldModalityVisible.Value = True
    End Sub

    Protected Sub ModalityDisplays(ByVal connectionString As String)
        Modalities = Page.LoadControl("controls/ModalityDisplayuc.ascx")
        CType(Modalities, controls_ModalityDisplayuc).LinacName = LinacName
        CType(Modalities, controls_ModalityDisplayuc).ID = "ModalityDisplayClinical"
        CType(Modalities, controls_ModalityDisplayuc).Mode = "Clinical"
        CType(Modalities, controls_ModalityDisplayuc).ConnectionString = connectionString
        ModalityPlaceholder.Controls.Add(Modalities)
        ModalityDisplayPanel.Visible = True
        DynamicControlSelection = MODALITYDISPLAY
    End Sub

    Public Sub UserApprovedEvent(ByVal TabSet As String, ByVal Userinfo As String)

        Dim machinelabel As String = LinacName & "Page.aspx"
        Dim username As String = Userinfo

        Dim Result As Boolean = False
        Dim EndofDay As Boolean = False

        If TabSet = CLINICAL Or TabSet = "Recover" Then
            Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
            HttpContext.Current.Session.Remove("Actionstate")
            Dim FaultParams As DavesCode.FaultParameters = New DavesCode.FaultParameters()
            If Action = "EndofDay" Then
                EndofDay = True
            End If
            Result = DavesCode.NewCommitClinical.CommitClinical(LinacName, username, False, FaultParams, EndofDay)
            If Result Then

                CommentBox.ResetCommentBox(String.Empty)

                DynamicControlSelection = String.Empty
                If Action = "Recover" Then
                    Dim returnstring As String = LinacName + "page.aspx?TabAction=Recovered&NextTab=" & TabSet
                    Response.Redirect(returnstring)
                Else
                    Response.Redirect(machinelabel)
                End If

            Else
                RaiseLogOffError()
                If Action = "Recover" Then
                    'if this is from restore then don't want to go back to recovery
                    Response.Redirect("/Errorpages/Oops.aspx")
                End If
            End If
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SaveText.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(SaveText, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        WaitButtons("Rad")

        'Select Case Me.DynamicControlSelection

        '    Case MODALITYDISPLAY
        '        ModalityDisplays(connectionString)
        '    Case Else
        'End Select

        Dim ReportFault As controls_ReportAFaultuc = CType(FindControl("ReportAFaultuc1"), controls_ReportAFaultuc)
        ReportFault.LinacName = LinacName
        ReportFault.ParentControl = CLINICAL
        AddHandler ReportFault.ReportAFault_UpdateDailyDefectDisplay, AddressOf Update_DefectDailyDisplay
        AddHandler ReportFault.ReportAFault_UpDateViewOpenFaults, AddressOf Update_ViewOpenFaults
        Dim objMFG As controls_MainFaultDisplayuc = Page.LoadControl("controls\MainFaultDisplayuc.ascx")
        CType(objMFG, controls_MainFaultDisplayuc).LinacName = LinacName
        CType(objMFG, controls_MainFaultDisplayuc).ID = "MainFaultDisplay"
        CType(objMFG, controls_MainFaultDisplayuc).ParentControl = CLINICAL
        PlaceHolderFaults.Controls.Add(objMFG)
        Dim Vctrl As ViewCommentsuc = CType(FindControl("ViewCommentsuc1"), ViewCommentsuc)
        Vctrl.LinacName = LinacName

        Dim wctrl2 As WriteDatauc = CType(FindControl("Writedatauc2"), WriteDatauc)
        wctrl2.LinacName = LinacName
        CommentBox.BoxChanged = BoxChanged
        RunUpCommentBox.BoxChanged = RunupBoxChanged
        Dim conn As SqlConnection
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        conn = New SqlConnection(connectionString)
        Modalities = Page.LoadControl("controls/ModalityDisplayuc.ascx")
        CType(Modalities, controls_ModalityDisplayuc).LinacName = LinacName
        CType(Modalities, controls_ModalityDisplayuc).ID = "ModalityDisplayClinical"
        CType(Modalities, controls_ModalityDisplayuc).Mode = "Clinical"
        CType(Modalities, controls_ModalityDisplayuc).ConnectionString = connectionString
        ModalityPlaceholder.Controls.Add(Modalities)
        If HiddenFieldModalityVisible.Value Then
            ModalityDisplayPanel.Visible = True
        End If
        'If Not IsPostBack Then
        '    Select Case Me.DynamicControlSelection

        '        Case MODALITYDISPLAY
        '            ModalityDisplays(connectionString)
        '        Case Else
        '    End Select
        'End If

    End Sub
    Protected Sub BindRunUpComments(ByVal connectionString As String)
        Dim RunupComment As String
        Dim con As SqlConnection = New SqlConnection(connectionString)
        con.Open()
        Dim comm As SqlCommand = New SqlCommand("select e.comment from handoverenergies e where e.handoverid = (Select Max(handoverid) as mancount from [handoverenergies] where linac=@linac and Comment is not NULL)", con)
        comm.Parameters.AddWithValue("@linac", LinacName)
        RunupComment = CStr(comm.ExecuteScalar())
        RunUpCommentBox.ResetCommentBox(RunupComment)

        con.Close()

    End Sub

    Private Sub BindComments()

        Dim SqlDateSourceComment As New SqlDataSource()

        Dim query As String = "select convert(Varchar(5),c.DateTime, 108) as DateTime, c.ClinComment from handoverenergies e left outer join clinicalhandover r on e.handoverid=r.ehandid " &
        "Left outer join ClinicalTable c on c.PreClinID = r.CHandID where e.handoverid = (Select Max(handoverid) as mancount from [handoverenergies] where linac=@linac) and " &
        "c.PreClinID = (Select Max(CHandID) as mancount from [ClinicalHandover] where linac=@linac and not c.Clincomment = '') order by c.datetime desc"

        SqlDateSourceComment = QuerySqlConnection(LinacName, query)

        GridViewComments.DataSource = SqlDateSourceComment
        GridViewComments.DataBind()

    End Sub


    Protected Function QuerySqlConnection(ByVal MachineName As String, ByVal query As String) As SqlDataSource
        'This uses the sqldatasource instead of the individual conn definitions so reader can't be used
        'this solution is from http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.sqldatasource.select%28v=vs.90%29.aspx

        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString,
            .SelectCommand = query
        }
        SqlDataSource1.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", MachineName)
        Return SqlDataSource1


    End Function

    Protected Sub LinacHandover_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LogOffButton.Click
        'This hands over linac to enable repair, planned maintenance or Physics QA to take place
        'But it needs to allow existing engineering and pre-clinical run up to be retained
        'It needs a log out as well+
        'WriteClinicalTable()
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc2"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wclabel As Label = CType(wctrl.FindControl("WarningLabel"), Label)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        wcbutton.Text = "Log off Linac"
        wclabel.Text = String.Empty

        Session.Add("Actionstate", "Confirm")
        WriteDatauc2.Visible = True
        ForceFocus(wctext)


    End Sub
    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" +
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

    Protected Sub SaveText_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveText.Click
        Dim statusid As Integer
        Try
            Using myscope As TransactionScope = New TransactionScope()
                Dim connectionString1 As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
                statusid = DavesCode.NewCommitClinical.NewWriteClinicalTable(LinacName, HttpContext.Current.Application(BoxChanged), connectionString1)
                BindComments()
                myscope.Complete()
            End Using

            HiddenFieldLinacState.Value = statusid

        Catch ex As Exception
            DavesCode.NewFaultHandling.LogError(ex, Application(BoxChanged))
            RaiseWriteError()
        End Try
        CommentBox.ResetCommentBox(String.Empty)
    End Sub

    Protected Sub Tstart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Tstart.Click
        Dim linacstatusid As String
        'http://www.javascripter.net/faq/hextorgb.htm to convert from hex to argb
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString

        If Not StateLabel Is Nothing Then
            linacstatusid = HiddenFieldLinacState.Value

            Dim tval As String = Tstart.Text
            Try
                Using myscope As TransactionScope = New TransactionScope()
                    If tval = "Start Treatment" Then

                        DavesCode.NewCommitClinical.SetTreatment("Treating", LinacName, linacstatusid, connectionString)
                        Tstart.Text = "Stop Treatment"
                        Tstart.BackColor = colourstop

                        ActivityLabel.Text = "Clinical - Treating"


                    Else

                        DavesCode.NewCommitClinical.SetTreatment("Not Treating", LinacName, linacstatusid, connectionString)
                        Tstart.Text = "Start Treatment"
                        Tstart.BackColor = colourstart

                        ActivityLabel.Text = "Clinical - Not Treating"
                    End If

                    myscope.Complete()
                End Using
                StateLabel.Text = "Clinical"
            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)
                RaiseStartError()
            End Try
        End If

    End Sub
    Protected Sub RaiseLogOffError()
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Logging Off. Please inform Engineering');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(Tstart, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub
    Protected Sub RaiseStartError()
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim strScript As String = "<script>"
        strScript += "alert('Problem setting Start Treatment. Please inform Engineering');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(Tstart, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub
    Protected Sub RaiseLoadError()
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Logging On. Please call Administrator');"
        strScript += "window.location='"
        strScript += machinelabel
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(Tstart, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Sub RaiseWriteError()
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Writing Comment. Please call Administrator');"

        strScript += "</script>"
        ScriptManager.RegisterStartupScript(Tstart, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub



    '    'from http://msdn.microsoft.com/en-us/library/system.string.isnullorempty(v=vs.110).aspx

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
                Dim LogOff As Button = FindControl("LogOffButton")
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



    Protected Sub EndofDayButton_Click(sender As Object, e As EventArgs) Handles EndofDayButton.Click

        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc2"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wclabel As Label = CType(wctrl.FindControl("WarningLabel"), Label)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)


        Session.Add("Actionstate", "EndofDay")
        wclabel.Text = "Are you sure? This is the End of Day"
        wcbutton.Text = "End of Day"
        WriteDatauc2.Visible = True
        ForceFocus(wctext)

    End Sub
End Class
