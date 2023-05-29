Imports System.Globalization
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.Data
Partial Class OpsLoguc
    Inherits UserControl
    Private machinename As String
    Public Property LinacName() As String
        Get
            Return machinename
        End Get
        Set(ByVal value As String)
            machinename = value
        End Set
    End Property
    Public Event ReloadFaultsTab()

    Protected Sub ViewOpsButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ViewOpsButton.Click

        '    'Date selection from http://aspsnippets.com/Articles/ASP.Net-AJAX-CalendarExtender---Get-selected-date-from-ReadOnly-TextBox.aspx
        If Page.IsValid Then
            GetFaultSelection()
            ViewOpsButton.Visible = False
        End If

    End Sub
    Protected Sub GetFaultSelection()
        Dim MyCultureInfo As New CultureInfo("en-GB")
        Dim machineSelected As String
        Dim StopDate As String
        Dim EndingDate As Date
        Dim returntable As New DataTable()
        ' the checking of the date is very problematic if view button is selected without radiobutton. So decided to control view button

        Dim bul As Boolean = RequiredFieldValidatorstart.IsValid
        Dim bul1 As Boolean = RequiredFieldValidatorstop.IsValid
        Dim com As Boolean = CompareValidator1.IsValid
        machineSelected = dropLinac.SelectedValue
        Dim OpsType As String = RadioButtonOps.SelectedValue
        StopDate = Request.Form(EndDate.UniqueID)


        Dim StartingDate As Date

        If bul And bul1 And com Then
            Dim BeginDate As String = Request.Form(StartDate.UniqueID)
            If BeginDate IsNot Nothing Then
                StartingDate = DateTime.Parse(BeginDate, MyCultureInfo).ToShortDateString
                StartDate_CalendarExtender.SelectedDate = StartingDate
            End If

            If StopDate IsNot Nothing Then
                EndingDate = DateTime.Parse(StopDate, MyCultureInfo).ToShortDateString
                EndDate_CalendarExtender.SelectedDate = EndingDate
            End If
        End If
        StartDate.Enabled = False
        EndDate.Enabled = False


        returntable = SetGrid(machineSelected, OpsType, StartingDate, EndingDate)

        ViewState("OpsDataTable") = returntable
        If OpsType = "1" Then
            GridView3.DataSource = returntable
            GridView3.Caption = StringBuilderOps(StartingDate, EndingDate)
            ''have to set page index before binding data
            GridView3.PageIndex = 0
            GridView3.DataBind()
            CheckEmptyGrid(GridView3)
            'Else
            '    GridView2.DataSource = returntable
            '    GridView2.Caption = StringBuilderOps(StartingDate, EndingDate)
            '    ''have to set page index before binding data
            '    GridView2.PageIndex = 0
            '    GridView2.DataBind()
            '    CheckEmptyGrid(GridView2)
        End If
        RadioButtonOps.SelectedIndex = -1

    End Sub

    Protected Sub CheckEmptyGrid(ByVal grid As WebControls.GridView)
        If grid.Rows.Count = 0 And Not grid.DataSource Is Nothing Then
            Dim dt As Object = Nothing
            If grid.DataSource.GetType Is GetType(Data.DataSet) Then
                dt = New System.Data.DataSet
                dt = CType(grid.DataSource, System.Data.DataSet).Tables(0).Clone()
            ElseIf grid.DataSource.GetType Is GetType(Data.DataTable) Then
                dt = New System.Data.DataTable
                dt = CType(grid.DataSource, System.Data.DataTable).Clone()
            ElseIf grid.DataSource.GetType Is GetType(Data.DataView) Then
                dt = New System.Data.DataView
                dt = CType(grid.DataSource, System.Data.DataView).Table.Clone
            End If
            dt.Rows.Add(dt.NewRow())
            grid.DataSource = dt
            grid.DataBind()
            Dim columnsCount As Integer
            Dim tCell As New TableCell()
            columnsCount = grid.Columns.Count
            grid.Rows(0).Cells.Clear()
            grid.Rows(0).Cells.Add(tCell)
            grid.Rows(0).Cells(0).ColumnSpan = columnsCount


            grid.Rows(0).Cells(0).Text = "No Records To Display"
            grid.Rows(0).Cells(0).HorizontalAlign = HorizontalAlign.Center
            grid.Rows(0).Cells(0).ForeColor = Drawing.Color.Black
            grid.Rows(0).Cells(0).Font.Bold = True

        End If
    End Sub

    Function StringBuilderOps(ByVal BeginDate As String, StopDate As String) As String
        Dim builder As New StringBuilder
        Dim statement As String
        Dim StartDate As String = BeginDate
        Dim EndDate As String = StopDate
        Dim ending As String
        Dim Opslog As String = "Operations Log for "
        Dim Dates As String = StartDate & " To " & EndDate

        ending = "</td></tr></table>"
        statement = "<table border='1' width='100%' height='40px' cellpadding='0' cellspacing='0' bgcolor='#66d9ff'><tr><td>"

        builder.Append(statement)
        builder.Append(Opslog)
        builder.Append(Dates)
        builder.Append(ending)
        Return builder.ToString

    End Function

    Protected Sub ExitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ExitButton.Click

        Update()
        Dim Multiviewlist As MultiView

        If Parent.FindControl("Multiview1") IsNot Nothing Then
            Multiviewlist = Me.Parent.FindControl("Multiview1")
            Multiviewlist.ActiveViewIndex = 0
        End If
        'Added this event to reset if nonmachine page used
        RaiseEvent ReloadFaultsTab()


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            StartDate.Attributes.Add("readonly", "readonly")
            EndDate.Attributes.Add("readonly", "readonly")
            GridView3.DataSource = Nothing
            GridView3.DataBind()
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


    Protected Sub GridView3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridView3.SelectedIndexChanged
        Dim StateID As String
        Dim ReasonDescription As String
        Dim selectedRowIndex As Integer
        Dim oldselectedRowIndex As Integer

        selectedRowIndex = GridView3.SelectedIndex
        Dim row As GridViewRow
        If (Session("SelectedIndex") IsNot Nothing) Then
            oldselectedRowIndex = Convert.ToInt32(Session("SelectedIndex"))
            row = GridView3.Rows(oldselectedRowIndex)
            row.BackColor = (Session("selectedcolour"))

        End If
        row = GridView3.Rows(selectedRowIndex)
        StateID = row.Cells(1).Text
        ReasonDescription = row.Cells(3).Text

        Session("selectedcolour") = row.BackColor
        row.BackColor = ColorTranslator.FromHtml("#A1DCF2")
        Session("selectedindex") = GridView3.SelectedIndex
        GridView3.SelectedIndex = -1

    End Sub

    Protected Sub GridView3_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView3.PageIndexChanging
        Dim dt As New DataTable

        dt = ViewState("OpsDataTable")
        Session("SelectedIndex") = Nothing
        GridView3.PageIndex = e.NewPageIndex
        GridView3.DataSource = dt
        GridView3.DataBind()
    End Sub

    Function SetGrid(ByVal machineselected As String, ByVal radioselection As Integer, StartDate As DateTime, EndDate As DateTime) As DataTable
        Dim dt As New DataTable()
        Dim linac As String = machineselected
        Dim adapter As SqlDataAdapter

        adapter = New SqlDataAdapter()
        Dim conn As SqlConnection
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        conn = New SqlConnection(connectionString)

        Using (conn)
            Dim OpsLog As New SqlCommand With {
                .Connection = conn
            }
            Try
                conn.Open()

                Select Case radioselection
                    Case 1
                        OpsLog.CommandText = "usp_OpsLogCreation"
                        OpsLog.CommandType = CommandType.StoredProcedure
                        adapter.SelectCommand = OpsLog
                        OpsLog.Parameters.AddWithValue("@linac", SqlDbType.NVarChar).Value = linac
                        OpsLog.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime).Value = StartDate
                        OpsLog.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime).Value = EndDate
                        adapter.Fill(dt)

                End Select

            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)

            Finally
                conn.Close()
            End Try

        End Using

        Return dt


    End Function

    Protected Sub RadioButtonFault_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles RadioButtonOps.SelectedIndexChanged
        Dim DummyDate As DateTime = Now()
        Session("SelectedIndex") = Nothing
        GridView3.DataSource = Nothing
        GridView3.DataBind()
        Dim machineSelected As String = dropLinac.SelectedValue

        DatePanel.Visible = True
        StartDate_CalendarExtender.SelectedDate = Nothing
        EndDate_CalendarExtender.SelectedDate = Nothing
        StartDate.Enabled = True
        EndDate.Enabled = True
        RequiredFieldValidatorstart.Enabled = True
        RequiredFieldValidatorstop.Enabled = True
        ViewOpsButton.Visible = True

    End Sub

    Protected Sub DropLinac_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dropLinac.SelectedIndexChanged
        If Not dropLinac.SelectedValue = "Select" Then
            UpdatePanel2.Visible = True
        Else
            UpdatePanel2.Visible = False
        End If
        RadioButtonOps.SelectedIndex = -1
        DatePanel.Visible = False
        GridView3.DataSource = Nothing
        GridView3.DataBind()
    End Sub

    Public Sub Update()

        GridView3.DataSource = Nothing
        GridView3.DataBind()
        RadioButtonOps.SelectedIndex = -1
        If machinename = "nonmachine" Then
            machinename = "Select"
        End If

        dropLinac.SelectedValue = machinename
    End Sub

    Sub WaitButtons()
        Dim VF As Button = FindControl("ViewOpsButton")
        Dim Leave As Button = FindControl("ExitButton")


        If Not VF Is Nothing Then
            VF.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(VF, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
        If Not Leave Is Nothing Then
            Leave.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Leave, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
    End Sub


End Class
