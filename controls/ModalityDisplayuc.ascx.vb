
Imports System.Data

Partial Class controls_ModalityDisplayuc
    Inherits System.Web.UI.UserControl
    Public Property LinacName() As String
    Public Property Mode() As String
    Public Property ConnectionString As String
    Const EMPTYSTRING = ""
    Const UNAUTHORISEDSTATE As String = "Linac Unauthorised"
    Const ENGINEERINGAPPROVED As String = "Engineering Approved"
    Const CLINICALSTATE As String = "Clinical"
    Const SUSPENDED As String = "Suspended"
    Const FAULT As String = "Fault"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Not IsPostBack Then
        'Dim LastState As String = EMPTYSTRING
        'LastState = DavesCode.Reuse.GetLastState(LinacName, 0)
        'Select Case LastState
        '    Case UNAUTHORISEDSTATE
        '        MultiView1.SetActiveView(UnauthorisedView)
        '    Case ENGINEERINGAPPROVED
        '        MultiView1.SetActiveView(PreClinicalView)
        '    Case CLINICALSTATE, SUSPENDED
        '        BindEnergyDataClinical()
        '        MultiView1.SetActiveView(ClinicalView)
        '    Case FAULT
        'End Select
        Select Case Mode
                Case UNAUTHORISEDSTATE
                    MultiView1.SetActiveView(UnauthorisedView)
            Case ENGINEERINGAPPROVED
                BindEnergyDataClinical()
                MultiView1.SetActiveView(PreClinicalView)
                Case CLINICALSTATE, SUSPENDED
                    BindEnergyDataClinical()
                    MultiView1.SetActiveView(ClinicalView)
                Case FAULT
            End Select
        'End If


    End Sub
    Private Sub BindEnergyDataClinical()
        If Not LinacName Like "T#" Then
            Dim SqlDataSource1 As New SqlDataSource()
            'the distinct takes care of when suspended returns via pre-clinical because then there are two pre ids for one runup id
            'Dim query As String = "Select distinct handoverID, MV6, MV10, MeV6, MeV8, " & _
            '                          "MeV10, MeV12, MeV15, MeV18, MeV20, iView, XVI from HandoverEnergies e  left outer join clinicalhandover r On e.handoverid=r.ehandid where e.HandoverID  = (Select max(HandoverID) As lastrecord from HandoverEnergies where linac=@linac)"
            'Added isnull as per energy displayuc
            Dim query As String = "Select distinct handoverID, MV6, ISNULL(MV6FFF, 0) As ""MV6FFF"", MV10 ,ISNULL(MV10FFF, 0) As ""MV10FFF"",ISNULL(MeV4,0) As ""MeV4"", MeV6, MeV8, " &
                          "MeV10, MeV12, MeV15, MeV18, MeV20, iView, XVI from HandoverEnergies e  left outer join clinicalhandover r On e.handoverid=r.ehandid where r.CHandID  = (Select max(CHandID) As lastrecord from ClinicalHandover where linac=@linac)"

            SqlDataSource1 = QuerySqlConnection(query)
            GridView2.DataSource = SqlDataSource1
            GridView2.DataBind()

            GridView2.Columns(13).Visible = False
            Select Case LinacName
                Case "LA1"
                    For index As Integer = 1 To 4
                        GridView2.Columns(index).Visible = False
                    Next

                Case "LA2", "LA3"
                    For index As Integer = 1 To 4
                        GridView2.Columns(index).Visible = False
                    Next
                    GridView2.Columns(2).Visible = True
                Case "LA4"

                    For index As Integer = 1 To 11
                        GridView2.Columns(index).Visible = False
                    Next
                    GridView2.Columns(2).Visible = True
                    GridView2.Columns(13).Visible = True
                Case "E1", "E2", "B1", "B2"
                    GridView2.Columns(13).Visible = True
                    For index As Integer = 10 To 11
                        GridView2.Columns(index).Visible = False
                    Next
                Case Else
                    'All columns are valid and are displayed

            End Select
        Else
            GridView2.Visible = False
        End If

    End Sub
    Protected Function QuerySqlConnection(ByVal query As String) As SqlDataSource
        'This uses the sqldatasource instead of the individual conn definitions so reader can't be used
        'this solution is from http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.sqldatasource.select%28v=vs.90%29.aspx

        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = ConnectionString,
            .SelectCommand = query
        }
        SqlDataSource1.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", LinacName)
        Return SqlDataSource1


    End Function
    Protected Sub EnergyGridView_DataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView2.RowDataBound

        Dim headerRow As GridViewRow = e.Row
        Dim energy As Integer
        Dim query As String
        Dim SqlDataSource1 As New SqlDataSource()

        query = "Select distinct MV6,MV6FFF, MV10,MV10FFF,MeV4, MeV6, MeV8, " &
                  "MeV10, MeV12, MeV15, MeV18, MeV20, iView, XVI from HandoverEnergies e  left outer join clinicalhandover r on e.handoverid=r.ehandid where r.CHandID  = (Select max(CHandID) as lastrecord from ClinicalHandover where linac=@linac)"

        SqlDataSource1 = QuerySqlConnection(query)

        If headerRow.RowType = DataControlRowType.Header Then
            Try
                Dim dv As DataView
                dv = CType(SqlDataSource1.Select(DataSourceSelectArguments.Empty), DataView)
                'added 16march
                Dim colnum As Integer = dv.Table.Columns.Count - 1
                For count As Integer = 0 To dv.Table.Columns.Count - 1
                    'This will fail if for some reason the value n dv.Table.Rows(0)(count) is null
                    'so check for null and if it is put energy = 0
                    If IsDBNull(dv.Table.Rows(0)(count)) Then
                        energy = 0
                    Else
                        energy = CType(dv.Table.Rows(0)(count), Integer)
                    End If

                    Select Case energy
                        Case -1
                            headerRow.Cells(count).BackColor = System.Drawing.Color.Green
                        Case 0
                            headerRow.Cells(count).BackColor = System.Drawing.Color.Red
                    End Select

                Next

            Finally

            End Try
        End If

    End Sub
    Public Function FormatImage(ByVal energy As Boolean) As String

        Dim happyIcon As String = "Images/check_mark.jpg"

        Dim sadIcon As String = "Images/box_with_x.jpg"
        If energy Then
            Return happyIcon
        Else
            Return sadIcon
        End If
    End Function
End Class
