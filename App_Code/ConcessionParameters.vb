Imports System.Data
Imports System.Data.SqlClient

Namespace DavesCode

    Public Class ConcessionParameters
        Public ConcessionDescription As String = String.Empty
        Public ConcessionAction As String = String.Empty
        Public ConcessionComment As String = String.Empty
        Public Linac As String = String.Empty
        Public UserInfo As String = String.Empty
        Public AssignedTo As String = String.Empty
        Public IncidentID As Integer = 0
        Public PresentFaultState As String = String.Empty
        Public FutureFaultState As String = String.Empty
        Public ConcessionNumber As String = String.Empty

        Public Function CreateObject(ByVal Incident As String) As Boolean
            Dim Success As Boolean = False
            Dim conn As SqlConnection
            Dim comm As SqlCommand
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            conn = New SqlConnection(connectionString)
            comm = New SqlCommand("select distinct f.status, ISNULL(c.ConcessionNumber, '') as ConcessionNumber , ISNULL(c.concessiondescription, '') as ConcessionDescription, ISNULL(c.action, '') as Action, f.IncidentID, f.linac, ISNULL(c.AssignedTo, 'Unassigned') as AssignedTo " _
            & "from FaultIDTable f left outer join ConcessionTable c on f.ConcessionNumber=c.ConcessionNumber where f.incidentID = @incidentID", conn)
            comm.Parameters.AddWithValue("@incidentID", Incident)
            Try
                conn.Open()
                Dim da As New SqlDataAdapter(comm)
                Dim dt As New DataTable()

                da.Fill(dt)
                If dt.Rows.Count > 0 Then

                    For Each dataRow As DataRow In dt.Rows

                        PresentFaultState = dataRow("Status")
                        ConcessionNumber = dataRow("ConcessionNumber")
                        ConcessionDescription = dataRow("ConcessionDescription")
                        ConcessionAction = dataRow("action")
                        AssignedTo = dataRow("AssignedTo")
                        Linac = dataRow("linac")

                    Next


                    IncidentID = Incident
                UserInfo = String.Empty
                    FutureFaultState = String.Empty
                    Success = True
                Else
                    Dim message As String = String.Format("Data row count is zero in ConcessionParameters.vb create object. IncidentID: {0}", IncidentID)
                    Dim myEx As New Exception(message)

                    Throw myEx
                End If

            Catch ex As Exception
                NewFaultHandling.LogError(ex)
                Success = False


            Finally
                conn.Close()

            End Try

            Return Success
        End Function
    End Class

End Namespace
