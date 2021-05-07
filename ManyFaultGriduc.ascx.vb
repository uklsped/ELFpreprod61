
Imports System.Data

Partial Class ManyFaultGriduc
    Inherits System.Web.UI.UserControl

    'Private technicalstate As String
    Const DISPLAYREPEATFAULT As Integer = 0

    'Public Property Settech() As Boolean
    Public Property LinacName() As String
    Public Property NewFault() As Boolean
    Public Property IncidentID() As String

    'Protected Sub Page_init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
    '    technicalstate = "techstate" + MachineName
    'End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'technicalstate = "techstate" + MachineName
        If CInt(IncidentID) = DISPLAYREPEATFAULT Then
            'This is display of faults closed today
            BindGridClosedTodayFault()
        Else
            'This is Display of all faults associated with a concession
            BindGridViewConcessionFaults()
        End If
        If LinacName Like "T?" Then
            MultiView1.SetActiveView(Tomo)
        Else
            MultiView1.SetActiveView(Linacs)
        End If

    End Sub
    Public Sub BindGridClosedTodayFault()
        If LinacName Like "T?" Then
            BindDefectDataTomo()
        Else
            BindDefectDataLinac()
        End If
    End Sub

    Protected Sub BindDefectDataTomo()
        Dim SqlDataSource1 As New SqlDataSource()
        Dim query As String = "SELECT ConcessionNumber as 'IncidentID', CASE WHEN RadiationIncident = 1 THEN 'Yes' When RadiationIncident = 0 then 'No' Else 'Not Recorded' END AS RadiationIncident, Description, ReportedBy,RIGHT(CONVERT(VARCHAR, DateReported, 100),7) as 'DateReported',Area,Energy from ReportFault where Cast(DateReported As Date) = Cast(GetDate() As Date) And linac=@linac order by FaultID desc"
        SqlDataSource1 = QuerySqlConnection(query)
        GridViewTomo.DataSource = SqlDataSource1
        GridViewTomo.DataBind()
        CheckEmptyGrid(GridViewTomo)
    End Sub
    Public Sub BindGridViewConcessionFaults()
        If LinacName Like "T?" Then
            BindGridViewTomoVEF()
        Else
            BindGridViewVEF()
        End If
    End Sub
    Protected Sub BindGridViewVEF()
        'Added IncidentID = 0 to represent repeat faults recorded today
        Dim SqlDataSource4 As New SqlDataSource With {
            .ID = "SqlDataSource3",
            .ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        }
        'If NewFault Then
        '    SqlDataSource4.SelectCommand = "select IncidentID, CASE WHEN RadiationIncident = 1 THEN 'Yes' When RadiationIncident = 0 then 'No' Else 'Not Recorded' END AS RadiationIncident, Description, ReportedBy,DateReported,Area,Energy,GantryAngle,CollimatorAngle,Linac from ReportFault where incidentID in (select IncidentID from FaultIDTable where linac=@linac and Status in ('New'))"
        '    SqlDataSource4.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        '    SqlDataSource4.SelectParameters.Add("linac", LinacName)

        'Else

        SqlDataSource4.SelectCommand = "select IncidentID, CASE WHEN RadiationIncident = 1 THEN 'Yes' When RadiationIncident = 0 then 'No' Else 'Not Recorded' END AS RadiationIncident, Description, ReportedBy,DateReported,Area,Energy,GantryAngle,CollimatorAngle from ReportFault where incidentID = @IncidentID order by DateReported desc"
            'SqlDataSource4.SelectCommand = "select IncidentID, FaultID,ConcessionNumber, RadiationIncident, Description, ReportedBy,DateReported,Area,Energy,GantryAngle,CollimatorAngle,Linac from ReportFault where incidentID = @IncidentID order by DateReported desc"
            SqlDataSource4.SelectParameters.Add("@incidentID", SqlDbType.NVarChar)
            SqlDataSource4.SelectParameters.Add("incidentID", IncidentID)
        'End If

        GridViewLinac.DataSource = SqlDataSource4
        GridViewLinac.DataBind()
        'Because new machine column removed then columns(10) becomes columns(9)
        'If NewFault Then
        '    If Application(technicalstate) = True Then
        '        GridViewLinac.Columns(9).Visible = False
        '    Else
        '        GridViewLinac.Columns(9).Visible = True
        '    End If

        'End If


    End Sub

    Protected Sub BindGridViewTomoVEF()

        Dim SqlDataSource4 As New SqlDataSource With {
            .ID = "SqlDataSource3",
            .ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        }
        'If NewFault Then
        '    SqlDataSource4.SelectCommand = "select IncidentID, FaultID, Description, ReportedBy,DateReported,Area,Energy from ReportFault where incidentID in (select IncidentID from FaultIDTable where linac=@linac and Status in ('New', 'Open')) order by IncidentID desc"
        '    SqlDataSource4.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        '    SqlDataSource4.SelectParameters.Add("linac", MachineName)
        'Else
        SqlDataSource4.SelectCommand = "select IncidentID, FaultID, CASE WHEN RadiationIncident = 1 THEN 'Yes' When RadiationIncident = 0 then 'No' Else 'Not Recorded' END AS RadiationIncident, Description, ReportedBy,DateReported,Area,Energy from ReportFault where incidentID = @IncidentID order by DateReported desc"
        SqlDataSource4.SelectParameters.Add("@incidentID", System.Data.SqlDbType.NVarChar)
            SqlDataSource4.SelectParameters.Add("incidentID", IncidentID)
        'End If
        GridViewTomo.DataSource = SqlDataSource4
        GridViewTomo.DataBind()

        'If NewFault Then
        '    If Application(technicalstate) = True Then
        '        GridViewTomo.Columns(7).Visible = False
        '    Else
        '        GridViewTomo.Columns(7).Visible = True
        '    End If

        'End If


    End Sub

    Public Sub BindDefectDataLinac()
        Dim SqlDataSource1 As New SqlDataSource()
        Dim query As String = "SELECT 'IncidentID'= CASE WHEN ConcessionNumber = '' THEN  'Major Fault' ELSE ConcessionNumber END , CASE WHEN RadiationIncident = 1 THEN 'Yes' When RadiationIncident = 0 then 'No' Else 'Not Recorded' END AS RadiationIncident, Description, ReportedBy,RIGHT(CONVERT(VARCHAR, DateReported, 100),7) as 'DateReported',Area,Energy,GantryAngle,CollimatorAngle from ReportFault where Cast(DateReported As Date) = Cast(GetDate() As Date) And linac=@linac order by FaultID desc"
        SqlDataSource1 = QuerySqlConnection(query)
        GridViewLinac.DataSource = SqlDataSource1
        GridViewLinac.DataBind()
        CheckEmptyGrid(GridViewLinac)

    End Sub

    Public Sub CheckEmptyGrid(ByVal grid As WebControls.GridView)
        If grid.Rows.Count = 0 And Not grid.DataSource Is Nothing And Not IncidentID Is Nothing Then
            'Doesn't work like todayclosedfault checkemptygrid because of sqldatasource
            'From https://www.devexpress.com/Support/Center/Question/Details/A2624/how-to-access-the-gridviewinfo-object-of-the-gridview-class-in-xtragrid
            Dim dt As DataTable = CType(grid.DataSource.Select(DataSourceSelectArguments.Empty), DataView).Table

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
    Protected Function QuerySqlConnection(ByVal query As String) As SqlDataSource
        'This uses the sqldatasource instead of the individual conn definitions so reader can't be used
        'this solution is from http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.sqldatasource.select%28v=vs.90%29.aspx

        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString,
            .SelectCommand = (query)
        }
        SqlDataSource1.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", LinacName)
        Return SqlDataSource1

    End Function
    Public Sub UpDateDefectsEventHandler()
        'BindDefectData()
    End Sub


    'Sub NewFaultGridView_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)

    '    Dim IncidentID As String
    '    Dim index As Integer = Convert.ToInt32(e.CommandArgument)
    '    Dim row As GridViewRow
    '    If LinacName Like "T#" Then
    '        row = GridViewTomo.Rows(index)
    '    Else
    '        row = GridViewLinac.Rows(index)
    '    End If

    '    IncidentID = Server.HtmlDecode(row.Cells(0).Text)

    '    ' If multiple buttons are used in a GridView control, use the
    '    ' CommandName property to determine which button was clicked.
    '    Select Case e.CommandName
    '        Case "View"
    '            'RaiseEvent ShowFault(incidentNumber)
    '            'added 05April2016
    '            Application(technicalstate) = Nothing
    '            RaiseEvent ShowFault(IncidentID)
    '    End Select

    'End Sub

    Protected Sub OnPaging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewLinac.PageIndexChanging, GridViewTomo.PageIndexChanging
        If sender.ID = GridViewLinac.ID Then
            GridViewLinac.PageIndex = e.NewPageIndex
            GridViewLinac.DataBind()
        Else
            GridViewTomo.PageIndex = e.NewPageIndex
            GridViewTomo.DataBind()
        End If


    End Sub

End Class
