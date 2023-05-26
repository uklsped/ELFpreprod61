Imports System.Data.SqlClient
Imports System.Drawing
Imports AjaxControlToolkit
Imports System.Transactions


Partial Public Class ClinicalUserControl
    Inherits UserControl

    Private mpContentPlaceHolder As ContentPlaceHolder
    Private ReadOnly colourstart As Color = Color.FromArgb(255, 204, 0)
    Private ReadOnly colourstop As Color = Color.FromArgb(102, 153, 153)
    Private StateLabel As Label
    Private ActivityLabel As Label
    Private BoxChanged As String
    Private RunupBoxChanged As String
    Private Const CLINICAL As String = "3"
    Private ReadOnly connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
    Private Modalities As controls_ModalityDisplayuc
    Private MainFaultPanel As controls_MainFaultDisplayuc

    Public Property LinacName() As String

    Public Function FormatImage(energy As Boolean) As String

        Dim happyIcon As String = "Images/check_mark.jpg"
        Dim sadIcon As String = "Images/box_with_x.jpg"
        If energy Then
            Return happyIcon
        Else
            Return sadIcon
        End If
    End Function

    Protected Sub Update_ClosedFaultDisplay(EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_FaultClosedDisplay(LinacName)
        End If
    End Sub


    ' This updates the defect display on defectsave etc when repeat fault from viewopenfaults
    Protected Sub Update_DefectDailyDisplay(EquipmentID As String)

        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_defectsToday(LinacName)

        End If

    End Sub

    Protected Sub Update_ViewOpenFaults(EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_OpenConcessions(LinacName)

        End If
    End Sub

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'The method of finding acceptlinac3 started from here http://forums.asp.net/t/1380670.aspx?Access+Master+page+properties+from+User+Control

        Dim tabcontainer1 As TabContainer
        Page = Me.Page
        mpContentPlaceHolder =
        CType(Page.Master.FindControl("ContentPlaceHolder1"), ContentPlaceHolder)
        'If mpContentPlaceHolder IsNot Nothing Then
        '    tabcontainer1 = CType(mpContentPlaceHolder.
        '        FindControl("tcl"), TabContainer)
        '    If tabcontainer1 IsNot Nothing Then
        '        Dim panelcontrol As TabPanel = tabcontainer1.FindControl("TabPanel3")

        '    End If
        'End If

        AddHandler WriteDatauc2.UserApproved, AddressOf UserApprovedEvent

        If mpContentPlaceHolder IsNot Nothing Then
            StateLabel = CType(mpContentPlaceHolder.
                FindControl("StateLabel"), Label)
            ActivityLabel = CType(mpContentPlaceHolder.FindControl("ActivityLabel"), Label)
        End If

        BoxChanged = "ClinBoxChanged" + LinacName
        RunupBoxChanged = "RUBoxChanged" + LinacName

    End Sub

    Public Sub ClinicalApprovedEvent(connectionString As String)
        'This is the point that the gridviews are displayed and Clinical Table Data is written
        'BindEnergyData()

        'This looks to see if BoxChanged has a value. if it has the comment has not been saved.
        If HttpContext.Current.Application(BoxChanged) IsNot Nothing Then
            HiddenFieldLinacState.Value = DavesCode.NewCommitClinical.NewWriteClinicalTable(LinacName, HttpContext.Current.Application(BoxChanged), connectionString)
            CommentBox.ResetCommentBox(String.Empty)
        End If
        BindComments()
        BindRunUpComments(connectionString)

        HiddenFieldModalityVisible.Value = True
    End Sub

    Public Sub UserApprovedEvent(TabSet As String, Userinfo As String)

        Dim machinelabel As String = LinacName & "Page.aspx"
        Dim username As String = Userinfo
        Dim EndofDay As Boolean = False

        If TabSet = CLINICAL Or TabSet = "Recover" Then
            Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
            HttpContext.Current.Session.Remove("Actionstate")
            Dim FaultParams As New DavesCode.FaultParameters()
            If Action = "EndofDay" Then
                EndofDay = True
            End If
            Dim Result As Boolean = DavesCode.NewCommitClinical.CommitClinical(LinacName, username, False, FaultParams, EndofDay)
            If Result Then

                CommentBox.ResetCommentBox(String.Empty)

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
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        SaveText.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(SaveText, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        WaitButtons("Rad")

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

    End Sub
    Protected Sub BindRunUpComments(connectionString As String)
        Dim RunupComment As String
        Dim con As New SqlConnection(connectionString)
        con.Open()
        Dim comm As New SqlCommand("select e.comment from handoverenergies e where e.handoverid = (Select Max(handoverid) as mancount from [handoverenergies] where linac=@linac and Comment is not NULL)", con)
        comm.Parameters.AddWithValue("@linac", LinacName)
        RunupComment = CStr(comm.ExecuteScalar())
        RunUpCommentBox.ResetCommentBox(RunupComment)

        con.Close()

    End Sub

    Private Sub BindComments()

        Dim SqlDateSourceComment As New SqlDataSource()

        'Dim query As String = "select convert(Varchar(5),c.DateTime, 108) as DateTime, c.ClinComment from handoverenergies e left outer join clinicalhandover r on e.handoverid=r.ehandid " &
        '"Left outer join ClinicalTable c on c.PreClinID = r.CHandID where e.handoverid = (Select Max(handoverid) as mancount from [handoverenergies] where linac=@linac) and " &
        '"c.PreClinID = (Select Max(CHandID) as mancount from [ClinicalHandover] where linac=@linac and not c.Clincomment = '') order by c.datetime desc"
        Dim query As String = "select convert(Varchar(5),c.DateTime, 108) as DateTime, c.ClinComment " &
        "from ClinicalTable c where c.EHID = (Select Max(handoverid) as mancount from [handoverenergies]) and not c.Clincomment = '' and linac=@linac " &
         "order by c.datetime desc"

        SqlDateSourceComment = QuerySqlConnection(LinacName, query)

        GridViewComments.DataSource = SqlDateSourceComment
        GridViewComments.DataBind()

    End Sub

    Protected Function QuerySqlConnection(MachineName As String, query As String) As SqlDataSource
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

    Protected Sub LinacHandover_Click(sender As Object, e As System.EventArgs) Handles LogOffButton.Click
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
    Private Sub ForceFocus(ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" +
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

    Protected Sub SaveText_Click(sender As Object, e As System.EventArgs) Handles SaveText.Click
        Dim statusid As Integer
        Try
            Using myscope As New TransactionScope()
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

    Protected Sub Tstart_Click(sender As Object, e As System.EventArgs) Handles Tstart.Click
        Dim linacstatusid As String
        'http://www.javascripter.net/faq/hextorgb.htm to convert from hex to argb
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString

        If StateLabel IsNot Nothing Then
            linacstatusid = HiddenFieldLinacState.Value

            Dim tval As String = Tstart.Text
            Try
                Using myscope As New TransactionScope()
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

    Private Sub WaitButtons(WaitType As String)

        Select Case WaitType
            Case "Acknowledge"
                Dim Accept As Button = FindControl("AcceptOK")
                Dim Cancel As Button = FindControl("btnchkcancel")
                If FindControl("AcceptOK") IsNot Nothing Then
                    Accept.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Accept, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If FindControl("btnchkcancel") IsNot Nothing Then
                    Cancel.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Cancel, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

            Case "Tech"
                Dim Eng As Button = FindControl("engHandoverButton")
                Dim LogOff As Button = FindControl("LogOffButton")
                Dim Lock As Button = FindControl("LockElf")
                Dim Physics As Button = FindControl("PhysicsQA")
                Dim Atlas As Button = FindControl("ViewAtlasButton")
                Dim FaultPanel As Button = FindControl("FaultPanelButton")
                If Eng IsNot Nothing Then
                    Eng.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Eng, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If LogOff IsNot Nothing Then
                    LogOff.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(LogOff, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Lock IsNot Nothing Then
                    Lock.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Lock, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Physics IsNot Nothing Then
                    Physics.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Physics, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Atlas IsNot Nothing Then
                    Atlas.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Atlas, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If FaultPanel IsNot Nothing Then
                    FaultPanel.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(FaultPanel, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

            Case "Rad"
                Dim clin As Button = FindControl("clinHandoverButton")
                Dim LogOff As Button = FindControl("LogOffButton")
                Dim TStart As Button = FindControl("TStart")
                If clin IsNot Nothing Then
                    clin.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(clin, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If LogOff IsNot Nothing Then
                    LogOff.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(LogOff, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If TStart IsNot Nothing Then
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
