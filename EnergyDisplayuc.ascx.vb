Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Data
Imports System.Web.UI.Page
Imports System.Drawing
Imports AjaxControlToolkit
Imports System.Text
Partial Class EnergyDisplayuc
    Inherits System.Web.UI.UserControl
    Private MachineName As String
    Public Property LinacName() As String
        Get
            Return MachineName
        End Get
        Set(ByVal value As String)
            MachineName = value
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

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        BindEnergyData()
    End Sub
    Private Sub BindEnergyData()
        Dim SqlDataSource1 As New SqlDataSource()
        'the distinct takes care of when suspended returns via pre-clinical because then there are two pre ids for one runup id
        'Dim query As String = "Select distinct handoverID, MV6, MV10, MeV6, MeV8, " & _
        '                          "MeV10, MeV12, MeV15, MeV18, MeV20, iView, XVI from HandoverEnergies e  left outer join clinicalhandover r on e.handoverid=r.ehandid where e.HandoverID  = (Select max(HandoverID) as lastrecord from HandoverEnergies where linac=@linac)"

        'Dim query As String = "Select distinct handoverID, MV6, MV6fff,  MV10,MV10FFF, MeV4,  MeV6, MeV8, " &
        '                  "MeV10, MeV12, MeV15, MeV18, MeV20, iView, XVI from HandoverEnergies e  left outer join clinicalhandover r on e.handoverid=r.ehandid where r.CHandID  = (Select max(CHandID) as lastrecord from ClinicalHandover where linac=@linac)"
        Dim query As String = "select HandoverId, MV6,ISNULL(MV6FFF, 0) as ""MV6FFF"",MV10,ISNULL(MV10FFF, 0) as ""MV10FFF"",ISNULL(MeV4,0) as ""MeV4"", MEV6" & _
        ", MEV8, MeV10, MeV12, MeV15, MeV18, MeV20, iView, XVI" & _
        " from HandoverEnergies e  left outer join clinicalhandover r on e.handoverid=r.ehandid where r.CHandID  = (Select max(CHandID) as lastrecord from ClinicalHandover where linac=@linac)"
        'Dim query As String = "SELECT *  FROM [HandoverEnergies] where handoverid = (Select Max(handoverid) as mancount from [handoverenergies] where linac=@linac)"
        SqlDataSource1 = QuerySqlConnection(MachineName, query)
        GridView2.DataSource = SqlDataSource1
        GridView2.DataBind()
        'amended 13/12/16 to cater for instances with no XVi
        'GridView2.Columns(10).Visible = False

        'Select Case MachineName
        '    Case "LA1"
        '        GridView2.Columns(1).Visible = False
        '    Case "LA4"
        '        GridView2.Columns(10).Visible = True
        '        For index As Integer = 2 To 8
        '            GridView2.Columns(index).Visible = False
        '        Next
        '    Case Else
        '        'All columns are valid and are displayed
        'End Select
        GridView2.Columns(13).Visible = False
        Select Case MachineName
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
    End Sub
    Protected Function QuerySqlConnection(ByVal MachineName As String, ByVal query As String) As SqlDataSource
        'This uses the sqldatasource instead of the individual conn definitions so reader can't be used
        'this solution is from http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.sqldatasource.select%28v=vs.90%29.aspx

        Dim SqlDataSource1 As New SqlDataSource()
        SqlDataSource1.ID = "SqlDataSource1"
        SqlDataSource1.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        SqlDataSource1.SelectCommand = (query)
        SqlDataSource1.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", MachineName)
        Return SqlDataSource1


    End Function
End Class
