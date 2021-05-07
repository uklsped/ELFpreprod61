Imports System.Data.SqlClient
Imports System.Transactions
Namespace DavesCode
    Public Class NewWriteAux
        Public Shared Function WriteAuxTables(ByVal LinacName As String, ByVal LogOffName As String, ByVal comment As String, ByVal Radioselect As Integer, ByVal Tabused As Integer, ByVal NewFault As Boolean, ByVal lock As Boolean, ByVal FaultParams As DavesCode.FaultParameters) As Boolean
            Dim Successful As Boolean = False
            Dim NewIncidentID As Integer = 0
            Dim FaultParamsApplication As String
            FaultParamsApplication = "FaultParams" + LinacName
            Try
                Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
                Using myscope As TransactionScope = New TransactionScope()
                    WriteAuxTablesNew(LinacName, LogOffName, comment, Radioselect, Tabused, NewFault, lock, connectionString)
                    If NewFault And Not lock Then
                        NewIncidentID = NewFaultHandling.InsertMajorFault(FaultParams, connectionString)
                        FaultParams.SelectedIncident = NewIncidentID
                        HttpContext.Current.Application(FaultParamsApplication) = FaultParams
                    End If
                    myscope.Complete()
                    Successful = True
                End Using
            Catch ex As Exception
                NewFaultHandling.LogError(ex)
            End Try
            Return Successful
        End Function
        Public Shared Function WriteAuxTablesNew(ByVal LinacName As String, ByVal LogOffName As String, ByVal comment As String, ByVal Radioselect As Integer, ByVal Tabused As Integer, ByVal Fault As Boolean, ByVal lock As Boolean, ByVal connectionString As String) As String
            'writes the aux tables depending on the options picked. And writes the linac status table first.
            'When tidying this up look at whether radio is the same as tab used - no it's not
            Dim state As String = "Linac Unauthorised" 'most common value
            Dim time As DateTime
            time = Now()
            Dim LoginStatusID As String = ""
            Dim conn As SqlConnection
            Dim commpm As SqlCommand
            Dim StartTime As DateTime
            Dim LoginName As String = ""
            Dim LogOutStatusID As String = ""
            Dim contime As SqlCommand
            Dim reader As SqlDataReader
            Dim Activity As String = ""
            Dim userreason As Integer
            conn = New SqlConnection(connectionString)
            'Need to set  tab that function is on for query because of introduction of entry for fault close or concession raised.

            'Try
            '    Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            '    Using myscope As TransactionScope = New TransactionScope()

            'had to change this because Fault was getting confused with new fault with new popup.

            contime = New SqlCommand("SELECT stateID,State,DateTime, UserName FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
            'contime = New SqlCommand("SELECT StateID,DateTime, UserName, stateID FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac and userreason=@userreason)", conn)
            contime.Parameters.AddWithValue("@linac", LinacName)
            'contime.Parameters.AddWithValue("@userreason", Tabused)
            conn.Open()
            reader = contime.ExecuteReader()

            'If not new fault then state unchanged as Linac unauthorised or Suspended. If new fault then state is changed to fault.

            If reader.Read() Then
                StartTime = reader.Item("DateTime")
                LoginName = reader.Item("UserName")
                LoginStatusID = reader.Item("stateID")
                state = reader.Item("State")
            End If
            reader.Close()
            conn.Close()
            If Fault Then
                state = "Fault"
            End If



            Select Case Radioselect
                Case 1 'going back to run up should always reset state to unauthorised
                    userreason = 7
                    state = "Linac Unauthorised"
                Case 101, 103
                    userreason = Radioselect
                Case 102
                    userreason = Radioselect
                    If state = "Suspended" Then
                        state = "Linac Unauthorised"
                    End If
                Case Else
                    userreason = 7
            End Select
            'If Fault Then
            '    state = "Fault"
            '    'userreason = 103
            'Else
            '    'Radioselect determines how to SetStatus as a result of which radiobutton selected
            '    Select Case Radioselect
            '        Case 1, 102
            '            'STATE has already been set to unauthorised at top.
            '        Case 3, 4, 5, 6, 8, 101
            '            If suspstate = 1 Then
            '                state = "Suspended"
            '            End If

            '            '    ElseIf RunUpBoolean = 1 Then
            '            '        state = "Engineering Approved"
            '            '    Else

            '            '    End If

            '            ''Case 2
            '            ''    state = "Engineering Approved"
            '            'Case 3
            '            '    state = "Suspended"
            '            'Case 102


            '    End Select
            'End If

            LogOutStatusID = Reuse.SetStatusNew(LogOffName, state, 5, userreason, LinacName, Tabused, False, connectionString)
            'modified so not logged out of repair 25/06/20
            'If Tabused = 5 AndAlso Fault Then
            'don't want to log out of repair page

            'Else

            commpm = New SqlCommand("INSERT INTO AuxTable (Tab,LogInDate, LogOutDate, LogInName, LogOutName, Comment,linac, LogInStatusID, LogOutStatusID ) " &
                                               "VALUES (@Tab, @LogInDate, @LogOutDate, @LogInName, @LogOutName, @Comment,@linac, @LogInStatusID, @LogOutStatusID)", conn)

            commpm.Parameters.Add("@Tab", Data.SqlDbType.Int)
            commpm.Parameters("@Tab").Value = Tabused
            commpm.Parameters.Add("@LogInDate", Data.SqlDbType.DateTime)
            commpm.Parameters("@LogInDate").Value = StartTime
            commpm.Parameters.Add("@LogOutDate", Data.SqlDbType.DateTime)
            commpm.Parameters("@LogOutDate").Value = time
            commpm.Parameters.Add("@LogInName", Data.SqlDbType.NVarChar, 50)
            commpm.Parameters("@LogInName").Value = LoginName
            commpm.Parameters.Add("@LogOutName", Data.SqlDbType.NVarChar, 50)
            commpm.Parameters("@LogOutName").Value = LogOffName
            commpm.Parameters.Add("Comment", Data.SqlDbType.NVarChar, 250)
            commpm.Parameters("Comment").Value = comment
            commpm.Parameters.Add("@linac", Data.SqlDbType.NVarChar, 10)
            commpm.Parameters("@linac").Value = LinacName
            commpm.Parameters.Add("@LogInStatusID", Data.SqlDbType.NVarChar, 10)
            commpm.Parameters("@LogInStatusID").Value = LoginStatusID
            commpm.Parameters.Add("@LogOutStatusID", Data.SqlDbType.NVarChar, 10)
            commpm.Parameters("@LogOutStatusID").Value = LogOutStatusID

            conn.Open()
            commpm.ExecuteNonQuery()

            conn.Close()

            If Not lock Then
                Reuse.UpdateActivityTable(LinacName, LogOutStatusID, connectionString)
            Else
                'NewFaultHandling.UpdateLastNonFaultState(LinacName, connectionString)
            End If

            Return LogOutStatusID
        End Function

    End Class
End Namespace
