Imports System.Data
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Data.SqlClient


Partial Class AtlasEnergyViewuc
    Inherits UserControl
    Private MachineName As String

    Public Property LinacName() As String
        Get
            Return MachineName
        End Get
        Set(ByVal value As String)
            MachineName = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim machine As String
        ' Inserted this extra step because LA3 licence has been re-allocated to E1. 10/7/17
        'This is in machine table in atlas
        If MachineName = "E1" Then
            machine = 3
        ElseIf MachineName = "E2" Then
            machine = 5
        ElseIf MachineName = "B1" Then
            machine = 6
        Else
            'This next step strips out the machine number because that's what atlas uses beware that the energies database needs the full machine name
            machine = MachineName.Last
        End If

        'Added energyid less than 29 becuase imaging modalities have been added
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("AtlasConnectionString").ConnectionString
        Dim con As New SqlConnection(connectionString)
        con.Open()
        Dim cmd As New SqlCommand("select m.machinename,cast(e.energyvalue as varchar(10)) + ' ' + e.units as Beam,  mt.machinetestname as 'Test Name', b.result, cast(b.created as datetime) as 'Test Date', b.comment, b.accountname from atlas.dbo.[BeamTestResult] b " &
        "left outer join machinetemplate mt on b.machinetemplateid=mt.machinetemplateid " &
        "left outer join machine m on m.machineid=mt.machineid " &
        "left outer join energy e on e.energyid=mt.energyid " &
        "where substring((Cast(b.created as char)),1,11) = substring((Cast(getdate() as char)),1,11) " &
        "and mt.machineid= @machineid and beamtestresultid= " &
        "(SELECT MAX(btr.beamtestresultid) FROM beamtestresult as btr where btr.machinetemplateid=b.machinetemplateid) " &
        "order by e.energyid, mt.machinetestname", con)
        cmd.Parameters.AddWithValue("@machineid", machine)
        Dim da As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        GridView4.DataSource = dt
        GridView4.DataBind()
        CheckEmptyGrid(GridView4)

        Dim time As String
        time = Now.ToString("d")

    End Sub
    Public Sub CheckEmptyGrid(ByVal grid As GridView)
        If grid.Rows.Count = 10 And Not grid.DataSource Is Nothing Then
            Dim dt As Object = Nothing
            If grid.DataSource.GetType Is GetType(DataSet) Then
                dt = New DataSet
                dt = CType(grid.DataSource, DataSet).Tables(0).Clone()
            ElseIf grid.DataSource.GetType Is GetType(DataTable) Then
                dt = New DataTable
                dt = CType(grid.DataSource, DataTable).Clone()
            ElseIf grid.DataSource.GetType Is GetType(DataView) Then
                dt = New DataView
                dt = CType(grid.DataSource, DataView).Table.Clone
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
End Class
