Imports System.Data
Imports System.Data.SqlClient

Namespace DavesCode

    Public Class FaultParameters
        'auto-implemented properties https://docs.microsoft.com/en-us/dotnet/visual-basic/programming-guide/language-features/procedures/auto-implemented-properties
        Public SelectedIncident As Integer = 0
        Public Linac As String = String.Empty
        Public DateInserted As DateTime
        Public FaultDescription As String = String.Empty
        Public UserInfo As String = String.Empty
        Public Area As String = String.Empty
        Public Energy As String = String.Empty
        Public GantryAngle As String = String.Empty
        Public CollimatorAngle As String = String.Empty
        Public PatientID As String = String.Empty
        Public RadAct As String = String.Empty
        Public ConcessionNumber As String = String.Empty
        Public RadioIncident As String = String.Empty
        Public Activity As String = String.Empty
        Public LastState As String = String.Empty
        'Added activity to store Parent control ie tab where fault was reported
        'This is not used and refers to physicsenergydev

        'Public Function CreateObject(ByVal IncidentID As String) As Boolean
        '    Dim Success As Boolean = False
        '    Dim conn As SqlConnection
        '    Dim comm As SqlCommand
        '    Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        '    conn = New SqlConnection(connectionString)
        '    'from http://www.sqlservercentral.com/Forums/Topic1416029-1292-1.aspx
        '    comm = New SqlCommand("Select distinct ISNULL(c.ConcessionNumber, '') as ConcessionNumber , ISNULL(c.action, '') as Action, ISNULL(f.Area, '') as Area, c.linac " _
        '    & "From [PhysicsEnergyDev].[dbo].[ConcessionTable] c  left outer Join [PhysicsEnergyDev].[dbo].[ReportFault] f On f.incidentID=c.incidentID Where f.incidentID = @incidentid", conn)
        '    comm.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
        '    comm.Parameters("@incidentID").Value = IncidentID
        '    Try
        '        conn.Open()

        '        Dim da As New SqlDataAdapter(comm)
        '        Dim dt As New DataTable()

        '        da.Fill(dt)
        '        If dt.Rows.Count > 0 Then

        '            For Each dataRow As DataRow In dt.Rows
        '                ConcessionNumber = dataRow("ConcessionNumber")
        '                RadAct = dataRow("action")
        '                Area = dataRow("Area")
        '                Linac = dataRow("linac")

        '            Next


        '            SelectedIncident = IncidentID
        '            DateInserted = Now()
        '            Success = True
        '        Else
        '            Dim message As String = String.Format("Data row count is zero in faultparameters.vb create object. IncidentID: {0}", IncidentID)
        '            Dim myEx As New Exception(message)

        '            Throw myEx

        '        End If

        '    Catch ex As Exception
        '        NewFaultHandling.LogError(ex)
        '        Success = False


        '    Finally
        '        conn.Close()

        '    End Try

        '    Return Success
        'End Function
    End Class

End Namespace
