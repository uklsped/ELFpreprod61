Imports System.Globalization
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Drawing
Imports System.Data
Partial Class ViewFaultsuc
    Inherits System.Web.UI.UserControl
    Private machinename As String
    Public Property LinacName() As String
        Get
            Return MachineName
        End Get
        Set(ByVal value As String)
            MachineName = value
        End Set
    End Property
    Public Event ReloadFaultsTab()

    Protected Sub ViewFaultsButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewFaultsButton.Click
        'Also have to select linac
        'Dim machineSelected As String
        'Dim faultType As String
        'Dim BeginDate As String
        'Dim StopDate As String

        ''If RequiredFieldValidatorstart.IsValid And RequiredFieldValidatorstop.IsValid Then
        'machineSelected = dropLinac.SelectedValue
        'faultType = RadioButtonFault.SelectedValue
        'BeginDate = Request.Form(StartDate.UniqueID)
        'StopDate = Request.Form(EndDate.UniqueID)
        'If (BeginDate <> "") And (StopDate <> "") Then

        '    updatepanel3.Visible = True

        '    'Date selection from http://aspsnippets.com/Articles/ASP.Net-AJAX-CalendarExtender---Get-selected-date-from-ReadOnly-TextBox.aspx
        If Page.IsValid Then
            GetFaultSelection()
            ViewFaultsButton.Visible = False
        End If

        'End If
        'End If
    End Sub
    'Protected Sub GetFaultSelection(ByVal machineSelected As String, ByVal faultType As String, ByVal BeginDate As String, ByVal StopDate As String)
    Protected Sub GetFaultSelection()
        Dim MyCultureInfo As CultureInfo = New CultureInfo("en-GB")
        Dim machineSelected As String
        Dim faultType As String
        Dim BeginDate As String
        Dim StartingDate As DateTime
        Dim StopDate As String
        Dim EndingDate As DateTime
        Dim returntable As DataTable = New DataTable()
        ' the checking of the date is very problematic if view button is selected without radiobutton. So decided to control view button

        Dim bul As Boolean = RequiredFieldValidatorstart.IsValid
        Dim bul1 As Boolean = RequiredFieldValidatorstop.IsValid
        Dim com As Boolean = CompareValidator1.IsValid
        machineSelected = dropLinac.SelectedValue
        faultType = RadioButtonFault.SelectedValue
        BeginDate = Request.Form(StartDate.UniqueID)
        StopDate = Request.Form(EndDate.UniqueID)


        Select Case faultType
            Case 5, 6, 7
                If bul And bul1 And com Then
                    If Not BeginDate Is Nothing Then
                        StartingDate = DateTime.Parse(BeginDate, MyCultureInfo).ToShortDateString
                        StartDate_CalendarExtender.SelectedDate = StartingDate
                    End If

                    If Not StopDate Is Nothing Then
                        EndingDate = DateTime.Parse(StopDate, MyCultureInfo).ToShortDateString
                        EndDate_CalendarExtender.SelectedDate = EndingDate
                        EndingDate = EndingDate.AddDays(1)
                    End If
                End If
                StartDate.Enabled = False
                EndDate.Enabled = False
        End Select

        'This resets details tables between activations of view fault buttons
        resetTables()

        'Dim SqlDataSource1 As New SqlDataSource()
        Dim fconc, fclosed As String
        Dim elfconcession As Boolean = False
        Select Case faultType
            Case "1"
                fconc = "concession"
                fclosed = "closed"
            Case "2"
                fconc = "closed"
                fclosed = "closed"
            Case "3"
                fconc = "concession"
                fclosed = "concession"
            Case "4"
                elfconcession = True
            Case Else
        End Select


        returntable = SetGrid(machineSelected, faultType, StartingDate, EndingDate)
        ViewState("FaultsDataTable") = returntable
        If faultType = "7" Then

            GridView2.DataSource = returntable
            GridView2.Caption = DavesCode.Reuse.StringBuilder(faultType, StartingDate, EndingDate)
            'have to set page index before binding data
            GridView2.PageIndex = 0
            GridView2.DataBind()
        Else
            GridView1.DataSource = returntable
            GridView1.Caption = DavesCode.Reuse.StringBuilder(faultType, StartingDate, EndingDate)
            'have to set page index before binding data
            GridView1.PageIndex = 0
            GridView1.DataBind()
        End If
        RadioButtonFault.SelectedIndex = -1

    End Sub

    Function GetconcessionData(ByVal machineselected As String) As DataTable
        Dim dt As DataTable = New DataTable()
        Dim conn As SqlConnection
        Dim connectionstring As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        Dim querystring As String
        Dim adapter As SqlDataAdapter

        conn = New SqlConnection(connectionstring)
        querystring = "select distinct f.incidentID,  f.Dateinserted, f.DateClosed, f.status, r.description ,c.ConcessionNumber, c.ConcessionDescription,  f.linac " &
        "from FaultIDTable f left outer join ConcessionTable c on c.ConcessionNumber = f.ConcessionNumber left outer join reportfault r on r.faultid = f.OriginalFaultID where f.linac = @linac and c.ConcessionNumber != '' order by c.concessionnumber"
        adapter = New SqlDataAdapter()
        Dim command As SqlCommand = New SqlCommand(querystring, conn)
        adapter.SelectCommand = command

        command.Parameters.AddWithValue("@linac", SqlDbType.NVarChar).Value = machineselected


        Try
            ' Connect to the database and run the query.
            ' Fill the DataSet.
            adapter.Fill(dt)

        Catch ex As Exception

            ' The connection failed. Display an error message.

        End Try

        Return dt

    End Function

    Protected Sub ExitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ExitButton.Click
        'UpdatePanel2.Visible = False
        'updatepanel3.Visible = False
        'GridView1.SelectedIndex = -1
        Update()
        Dim Multiviewlist As MultiView

        If Not Me.Parent.FindControl("Multiview1") Is Nothing Then
            Multiviewlist = Me.Parent.FindControl("Multiview1")
            Multiviewlist.ActiveViewIndex = 0
        End If
        'Added this event to reset if nonmachine page used
        RaiseEvent ReloadFaultsTab()
        'ClientScript.RegisterClientScriptBlock(Page.GetType(), "script", "window.close();", True)

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            StartDate.Attributes.Add("readonly", "readonly")
            EndDate.Attributes.Add("readonly", "readonly")
            'machinename = Request.QueryString("id")
            'If String.IsNullOrEmpty(Request.QueryString("id")) Then
            '    machinename = "1"
            'End If
            GridView1.DataSource = Nothing
            GridView1.DataBind()
            GridView2.DataSource = Nothing
            GridView2.DataBind()
            DatePanel.Visible = False
            If machinename = "nonmachine" Then
                'UpdatePanel2.Visible = True
            Else
                dropLinac.Items.FindByValue(machinename).Selected = True
                If Not machinename = "Select" Then
                    UpdatePanel2.Visible = True
                End If
            End If
            WaitButtons()
        End If
    End Sub

    Sub Listfaults(ByVal IncidentID As String)

        Dim IncidentNumber As String = IncidentID
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim reader As SqlDataReader
        Dim connectionstring As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        conn = New SqlConnection(connectionstring)
        comm = New SqlCommand("Select Top 1 IncidentID, Description, ReportedBy, DateReported, Area, Energy, GantryAngle,CollimatorAngle, BSUHID from ReportFault where IncidentID = @IncidentID order by DateReported", conn)
        comm.Parameters.Add("IncidentID", System.Data.SqlDbType.Int)
        comm.Parameters.Item("IncidentID").Value = Convert.ToInt16(IncidentNumber)
        Try
            conn.Open()
            reader = comm.ExecuteReader()
            DatalistFaults.DataSource = reader
            DatalistFaults.DataBind()
            reader.Close()
        Finally
            conn.Close()
        End Try

    End Sub

    Protected Sub GridView1_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles GridView1.SelectedIndexChanged
        Dim IncidentID As String
        Dim selectedRowIndex As Integer
        Dim oldselectedRowIndex As Integer

        selectedRowIndex = GridView1.SelectedIndex
        Dim row As GridViewRow
        If (Session("SelectedIndex") IsNot Nothing) Then
            oldselectedRowIndex = Convert.ToInt32(Session("SelectedIndex"))
            row = GridView1.Rows(oldselectedRowIndex)
            row.BackColor = (Session("selectedcolour"))

        End If
        row = GridView1.Rows(selectedRowIndex)
        IncidentID = row.Cells(0).Text
        Listfaults(IncidentID)
        ListHistory(IncidentID)
        ListMultipleFaults(IncidentID)
        Session("selectedcolour") = row.BackColor
        row.BackColor = ColorTranslator.FromHtml("#A1DCF2")
        Session("selectedindex") = GridView1.SelectedIndex
        GridView1.SelectedIndex = -1
        updatepanel3.Visible = True
    End Sub

    Sub ListHistory(ByVal IncidentID As String)
        Dim IncidentNumber As String = IncidentID
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim reader As SqlDataReader
        Dim connectionstring As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        conn = New SqlConnection(connectionstring)
        comm = New SqlCommand("select t.TrackingID, t.trackingcomment, t.AssignedTo, t.Status, t.LastUpDatedBy, t.LastUpDatedOn, t.linac, t.action " &
            "from FaultTracking t  where t.incidentID=@incidentID order by t.trackingid asc", conn)
        comm.Parameters.Add("IncidentID", System.Data.SqlDbType.Int)
        comm.Parameters.Item("IncidentID").Value = Convert.ToInt16(IncidentNumber)

        Try
            conn.Open()
            reader = comm.ExecuteReader()
            trackingHistory.DataSource = reader
            trackingHistory.DataBind()
            reader.Close()
        Finally
            conn.Close()
        End Try
    End Sub

    Sub ListMultipleFaults(ByVal IncidentId As String)
        Dim IncidentNumber As String = IncidentId
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim reader As SqlDataReader
        Dim connectionstring As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        conn = New SqlConnection(connectionstring)
        comm = New SqlCommand("SELECT FaultID ,Description,ReportedBy,DateReported, Area, Energy, GantryAngle, CollimatorAngle, Linac, IncidentID, BSUHID, ConcessionNumber " &
        "FROM ReportFault where incidentID=@incidentID and FaultID not in (Select OriginalFaultID from FaultIDTable where incidentID=@incidentID) order by faultid asc", conn)
        comm.Parameters.Add("IncidentID", System.Data.SqlDbType.Int)
        comm.Parameters.Item("IncidentID").Value = Convert.ToInt16(IncidentNumber)

        Try
            conn.Open()
            reader = comm.ExecuteReader()
            Multifault.DataSource = reader
            Multifault.DataBind()
            reader.Close()
        Finally
            conn.Close()
        End Try
    End Sub
    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Dim dt As New DataTable
        resetTables()
        dt = ViewState("FaultsDataTable")
        Session("SelectedIndex") = Nothing
        GridView1.PageIndex = e.NewPageIndex
        GridView1.DataSource = dt
        GridView1.DataBind()
    End Sub
    Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView2.PageIndexChanging
        Dim dt As New DataTable
        resetTables()
        dt = ViewState("FaultsDataTable")
        Session("SelectedIndex") = Nothing
        GridView2.PageIndex = e.NewPageIndex
        GridView2.DataSource = dt
        GridView2.DataBind()
    End Sub


    Function GetData(ByVal linac As String, ByVal concession As String, closed As String, ByVal StartDate As DateTime, ByVal EndDate As DateTime) As DataTable
        Dim dt As DataTable = New DataTable()
        Dim conn As SqlConnection
        Dim connectionstring As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        Dim querystring As String
        Dim adapter As SqlDataAdapter

        conn = New SqlConnection(connectionstring)
        adapter = New SqlDataAdapter()
        Dim command As SqlCommand

        querystring = "select distinct f.incidentID,  f.Dateinserted, f.DateClosed, f.status, r.description ,c.ConcessionNumber, c.ConcessionDescription,  f.linac " &
        "from FaultIDTable f left outer join ConcessionTable c on c.ConcessionNumber = f.ConcessionNumber left outer join reportfault r on r.faultid = f.OriginalFaultID where f.linac=@linac and f.Status in (@closed, @concession) and f.dateinserted between @StartDate and @EndDate order by Dateinserted desc"

        command = New SqlCommand(querystring, conn)
        adapter.SelectCommand = command

        command.Parameters.AddWithValue("@linac", SqlDbType.NVarChar).Value = linac
        command.Parameters.AddWithValue("@closed", System.Data.SqlDbType.NVarChar).Value = closed
        command.Parameters.AddWithValue("@concession", System.Data.SqlDbType.NVarChar).Value = concession
        command.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime).Value = StartDate
        command.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime).Value = EndDate

        Try
            ' Connect to the database and run the query.
            ' Fill the DataSet.
            adapter.Fill(dt)

        Catch ex As Exception

            ' The connection failed. Display an error message.

        End Try

        Return dt
    End Function

    Function SetGrid(ByVal machineselected As String, ByVal radioselection As Integer, StartDate As DateTime, EndDate As DateTime) As DataTable
        Dim dt As DataTable = New DataTable()
        Dim linac As String = machineselected
        Dim conn As SqlConnection
        Dim connectionstring As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        Dim querystring As String
        Dim adapter As SqlDataAdapter
        Dim closed As String
        Dim concession As String

        conn = New SqlConnection(connectionstring)
        adapter = New SqlDataAdapter()
        Dim command As SqlCommand

        Select Case radioselection
            Case 1, 2, 4
                If radioselection = 1 Then
                    closed = "closed"
                    concession = "concession"
                ElseIf radioselection = 2 Then
                    closed = "closed"
                    concession = "closed"
                Else
                    closed = "concession"
                    concession = "concession"
                End If
                querystring = "select distinct f.incidentID,  f.Dateinserted, f.DateClosed, f.status, r.description ,c.ConcessionNumber, c.ConcessionDescription,  f.linac " &
                "from FaultIDTable f left outer join ConcessionTable c on c.ConcessionNumber = f.ConcessionNumber left outer join reportfault r on r.faultid = f.OriginalFaultID where f.linac=@linac and f.Status in (@closed, @concession) order by Dateinserted desc"

                command = New SqlCommand(querystring, conn)
                adapter.SelectCommand = command
                command.Parameters.AddWithValue("@linac", SqlDbType.NVarChar).Value = linac
                command.Parameters.AddWithValue("@closed", System.Data.SqlDbType.NVarChar).Value = closed
                command.Parameters.AddWithValue("@concession", System.Data.SqlDbType.NVarChar).Value = concession
            Case 3
                querystring = "select distinct f.incidentID,  f.Dateinserted, f.DateClosed, f.status, r.description ,c.ConcessionNumber, c.ConcessionDescription,  f.linac " &
                "from FaultIDTable f left outer join ConcessionTable c on c.ConcessionNumber = f.ConcessionNumber left outer join reportfault r on r.faultid = f.OriginalFaultID where f.linac=@linac and c.ConcessionNumber like 'ELF%'  order by Dateinserted desc"
                command = New SqlCommand(querystring, conn)
                adapter.SelectCommand = command
                command.Parameters.AddWithValue("@linac", SqlDbType.NVarChar).Value = linac
            Case 5, 6

                If radioselection = 5 Then
                    querystring = "select distinct f.incidentID,  f.Dateinserted, f.DateClosed, f.status, r.description ,c.ConcessionNumber, c.ConcessionDescription,  f.linac " &
                    "from FaultIDTable f left outer join ConcessionTable c on c.ConcessionNumber = f.ConcessionNumber left outer join reportfault r on r.faultid = f.OriginalFaultID where f.linac=@linac and f.dateinserted between @StartDate and @EndDate order by Dateinserted desc"
                Else
                    querystring = "select distinct f.incidentID,  f.Dateinserted, f.DateClosed, f.status, r.description ,c.ConcessionNumber, c.ConcessionDescription,  f.linac " &
                    "from FaultIDTable f left outer join ConcessionTable c on c.ConcessionNumber = f.ConcessionNumber left outer join reportfault r on r.faultid = f.OriginalFaultID where f.linac=@linac and f.dateclosed between @StartDate and @EndDate order by Dateinserted desc"
                End If
                command = New SqlCommand(querystring, conn)
                adapter.SelectCommand = command
                command.Parameters.AddWithValue("@linac", SqlDbType.NVarChar).Value = linac
                command.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime).Value = StartDate
                command.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime).Value = EndDate
            Case 7
                querystring = "select concessionnumber, Description, DateReported from ReportFault where incidentid <1 and linac=@linac and Datereported between @StartDate and @EndDate order by DateReported desc"
                command = New SqlCommand(querystring, conn)
                adapter.SelectCommand = command
                command.Parameters.AddWithValue("@linac", SqlDbType.NVarChar).Value = linac
                command.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime).Value = StartDate
                command.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime).Value = EndDate
        End Select
        Try
            ' Connect to the database and run the query.
            ' Fill the DataSet.
            adapter.Fill(dt)

        Catch ex As Exception

            ' The connection failed. Display an error message.

        End Try

        Return dt


    End Function
    Sub resetTables()
        Listfaults("0")
        ListHistory("0")
        ListMultipleFaults("0")
        updatepanel3.Visible = False

    End Sub

    Protected Sub RadioButtonFault_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles RadioButtonFault.SelectedIndexChanged

        Dim faulttype As Integer
        Dim DummyDate As DateTime = Now()
        Dim returntable As DataTable = New DataTable()
        Session("SelectedIndex") = Nothing
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        GridView2.DataSource = Nothing
        GridView2.DataBind()
        updatepanel3.Visible = False
        Dim machineSelected As String = dropLinac.SelectedValue
        faulttype = RadioButtonFault.SelectedValue

        Select Case faulttype
            Case 1, 2, 3, 4
                DatePanel.Visible = False
                StartDate.Enabled = False
                EndDate.Enabled = False
                RequiredFieldValidatorstart.Enabled = False
                RequiredFieldValidatorstop.Enabled = False
                returntable = SetGrid(machineSelected, faulttype, DummyDate, DummyDate)
                ViewState("FaultsDataTable") = returntable
                GridView1.DataSource = returntable
                GridView1.Caption = DavesCode.Reuse.StringBuilder(faulttype, DummyDate, DummyDate)
                'have to set page index before binding data
                GridView1.PageIndex = 0
                GridView1.DataBind()
                'updatepanel3.Visible = True
            Case 5, 6, 7
                DatePanel.Visible = True
                StartDate_CalendarExtender.SelectedDate = Nothing
                EndDate_CalendarExtender.SelectedDate = Nothing
                StartDate.Enabled = True
                EndDate.Enabled = True
                RequiredFieldValidatorstart.Enabled = True
                RequiredFieldValidatorstop.Enabled = True
                ViewFaultsButton.Visible = True

        End Select

    End Sub

    Protected Sub dropLinac_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dropLinac.SelectedIndexChanged
        If Not dropLinac.SelectedValue = "Select" Then
            UpdatePanel2.Visible = True
        Else
            UpdatePanel2.Visible = False
        End If
        RadioButtonFault.SelectedIndex = -1
        resetTables()
        DatePanel.Visible = False
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        GridView2.DataSource = Nothing
        GridView2.DataBind()
    End Sub

    Public Sub Update()
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        GridView2.DataSource = Nothing
        GridView2.DataBind()
        resetTables()
        RadioButtonFault.SelectedIndex = -1
        'ViewFaultsButton.Enabled = False
        If machinename = "nonmachine" Then
            machinename = "Select"
        End If

        dropLinac.SelectedValue = machinename
    End Sub

    Sub WaitButtons()
        Dim VF As Button = FindControl("ViewFaultsButton")
        Dim Leave As Button = FindControl("ExitButton")


        If Not VF Is Nothing Then
            VF.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(VF, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
        If Not Leave Is Nothing Then
            Leave.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Leave, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
    End Sub
End Class
