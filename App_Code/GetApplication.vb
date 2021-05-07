Imports System.Data.SqlClient
Namespace DavesCode

    Public Class GetApplication
        Public Shared Function GetApplicationState(ByVal linac As String, ByVal PreviousState As Integer) As Boolean
            'Changed this to a function to return the linac state ID where necessary
            Dim LoggedOn As Boolean
            Dim reader As Data.SqlClient.SqlDataReader

            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            'Dim Machinestatus As SqlCommand
            Dim StatusNow As SqlCommand

            conn = New SqlConnection(connectionString)
            Try
                If PreviousState = 0 Then
                    'StatusNow = New SqlCommand("SELECT LoggedOn FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                    StatusNow = New SqlCommand("SELECT TOP 1 * FROM [LinacStatus] where linac=@linac order by StateID desc", conn)

                Else
                    'This doesn't work it just gets penultimate record irrespective of linac
                    'StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select (max(stateID)-1) as penultimaterecord from [LinacStatus] where linac=@linac)", conn)
                    'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
                    StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

                End If
                StatusNow.Parameters.AddWithValue("@linac", linac)
                conn.Open()
                reader = StatusNow.ExecuteReader()

                If reader.Read() Then
                    LoggedOn = reader.Item("LoggedOn")

                End If
            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)


            Finally
                reader.Close()
                conn.Close()
            End Try

            Return LoggedOn
        End Function

        Public Shared Function Returnlastuserreason(ByVal linac As String, ByVal PreviousState As Integer) As Integer
            Dim Userreason As Integer
            Dim reader As Data.SqlClient.SqlDataReader
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            Dim StatusNow As SqlCommand

            conn = New SqlConnection(connectionString)
            Try
                If PreviousState = 0 Then

                    StatusNow = New SqlCommand("SELECT TOP 1 * FROM [LinacStatus] where linac=@linac order by StateID desc", conn)

                Else

                    'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
                    StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

                End If
                StatusNow.Parameters.AddWithValue("@linac", linac)
                conn.Open()
                reader = StatusNow.ExecuteReader()

                If reader.Read() Then
                    Userreason = reader.Item("userreason")

                End If
            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)


            Finally
                reader.Close()
                conn.Close()
            End Try

            Return Userreason
        End Function

    End Class
End Namespace
