Imports System.Data.SqlClient
Imports System.Data
Partial Class TodayClosedFault
    Inherits UserControl

    Public Property LinacName() As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            SetGrid()
        End If
    End Sub

    Public Sub Update_ClosedDisplays(ByVal Linac As String)
        If Linac = LinacName Then
            SetGrid()
        End If
    End Sub

    Public Sub SetGrid()
        Dim today As Date = DateTime.Today
        Dim todayplusone As DateTime = today.AddDays(1)
        Dim dt As DataTable = New DataTable()
        Dim conn As SqlConnection
        Dim connectionstring As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        Dim querystring As String
        Dim adapter As SqlDataAdapter


        conn = New SqlConnection(connectionstring)
        querystring = "select distinct f.incidentID, r.description ,c.ConcessionNumber, c.ConcessionDescription " &
        "from FaultIDTable f left outer join ConcessionTable c on c.ConcessionNumber = f.ConcessionNumber left outer join reportfault r on r.faultid = f.OriginalFaultID where f.linac=@linac and c.ConcessionNumber is not NULL and f.dateclosed between @StartDate and @EndDate order by f.incidentid desc"

        adapter = New SqlDataAdapter()
        Dim command As SqlCommand = New SqlCommand(querystring, conn)
        adapter.SelectCommand = command

        command.Parameters.AddWithValue("@linac", SqlDbType.NVarChar).Value = LinacName
        command.Parameters.Add("@StartDate", System.Data.SqlDbType.DateTime).Value = today
        command.Parameters.Add("@EndDate", System.Data.SqlDbType.DateTime).Value = todayplusone

        Try
            ' Connect to the database and run the query.
            ' Fill the DataSet.
            adapter.Fill(dt)

        Catch ex As Exception
            DavesCode.NewFaultHandling.LogError(ex)
            ' The connection failed. Display an error message.

        End Try

        ViewState("FaultsDataTable") = dt
        GridView1.DataSource = dt
        GridView1.DataBind()

        CheckEmptyGrid(GridView1)
    End Sub
    Public Sub CheckEmptyGrid(ByVal grid As WebControls.GridView)
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

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Dim dt As New DataTable

        dt = ViewState("FaultsDataTable")
        Session("SelectedIndex") = Nothing
        GridView1.PageIndex = e.NewPageIndex
        GridView1.DataSource = dt
        GridView1.DataBind()
    End Sub

End Class


