Imports System.Data
Imports System.Data.SqlClient

Partial Class controls_OriginalReportedfaultuc
    Inherits UserControl
    Public Property IncidentID() As String
    Public Property Device() As String
    'Public Property ReportFaultID() As String
    Private conn As SqlConnection
    Private comm As SqlCommand

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim connectionString1 As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        conn = New SqlConnection(connectionString1)
        If Device Like "T?" Then
            TomoLoad()
            MultiView1.SetActiveView(Tomo)
        Else
            LinacLoad()
            MultiView1.SetActiveView(Linac)
        End If

    End Sub

    Protected Sub LinacLoad()

        comm = New SqlCommand("select distinct r.FaultID, r.Description, r.ReportedBy, r.DateReported, f.Status, r.RadiationIncident, r.Area, r.Energy, r.GantryAngle, r.CollimatorAngle, ISNULL (r.BSUHID, '') as BSUHID,ISNULL(c.ConcessionNumber, '') as ConcessionNumber , ISNULL(c.concessiondescription, '') as ConcessionDescription, f.linac, f.IncidentID " _
       & "from reportfault r left outer join FaultIDTable f on f.OriginalFaultID = r.FaultID left outer join ConcessionTable c on f.ConcessionNumber=c.ConcessionNumber where f.incidentID = @incidentID", conn)

        comm.Parameters.AddWithValue("@incidentID", IncidentID)
        Try
            conn.Open()
            Dim da As New SqlDataAdapter(comm)
            Dim dt As New DataTable()

            da.Fill(dt)
            If dt.Rows.Count > 0 Then

                For Each dataRow As DataRow In dt.Rows
                    OriginalDescriptionBoxL.Text = dataRow("Description")
                    OriginalReportedBoxL.Text = dataRow("ReportedBy")
                    OriginalOpenDateBoxL.Text = dataRow("DateReported")
                    OriginalAreaBox.Text = dataRow("Area")
                    OriginalEnergyBox.Text = dataRow("Energy")
                    OriginalGantryBox.Text = dataRow("GantryAngle")
                    OriginalCollBox.Text = dataRow("CollimatorAngle")
                    OriginalPatientIDBoxL.Text = dataRow("BSUHID")
                    If dataRow.IsNull("RadiationIncident") Then
                        OriginalRadioIncident.Visible = False
                        RadiationIncidentLabel.Visible = False
                    Else
                        OriginalRadioIncident.SelectedValue = dataRow("RadiationIncident")
                    End If

                Next
            End If

        Finally
            conn.Close()
        End Try
    End Sub

    Protected Sub TomoLoad()
        comm = New SqlCommand("select distinct r.FaultID, r.Description, r.ReportedBy, r.DateReported, f.Status,r.RadiationIncident, r.Area, r.Energy, ISNULL (r.BSUHID, '') as BSUHID,ISNULL(c.ConcessionNumber, '') as ConcessionNumber , ISNULL(c.concessiondescription, '') as ConcessionDescription, f.linac, f.IncidentID " _
      & "from reportfault r left outer join FaultIDTable f on f.OriginalFaultID = r.FaultID left outer join ConcessionTable c on f.ConcessionNumber=c.ConcessionNumber where f.incidentID = @incidentID", conn)

        comm.Parameters.AddWithValue("@incidentID", IncidentID)
        Try
            conn.Open()
            Dim da As New SqlDataAdapter(comm)
            Dim dt As New DataTable()

            Dim FaultStatus As String


            da.Fill(dt)
            If dt.Rows.Count > 0 Then

                For Each dataRow As DataRow In dt.Rows

                    OriginalDescriptionBoxT.Text = dataRow("Description")
                    OriginalReportedBoxT.Text = dataRow("ReportedBy")
                    OriginalOpenDateBoxT.Text = dataRow("DateReported")
                    FaultStatus = dataRow("Status")
                    AccurayTextBox.Text = dataRow("Area")
                    ErrorTextBox.Text = dataRow("Energy")

                    OriginalPatientIDBoxT.Text = dataRow("BSUHID")
                    If dataRow.IsNull("RadiationIncident") Then
                        OriginalRadioIncidentT.Visible = False
                        RadiationIncidentLabelT.Visible = False
                    Else
                        OriginalRadioIncidentT.SelectedValue = dataRow("RadiationIncident")
                    End If
                Next
            End If

        Finally
            conn.Close()
        End Try
    End Sub
End Class
