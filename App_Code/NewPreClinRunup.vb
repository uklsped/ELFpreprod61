Imports System.Data
Imports System.Data.SqlClient
Imports System.Transactions
Namespace DavesCode
    Public Class NewPreClinRunup
        Public Shared Function CommitPreClin(ByVal LinacName As String, ByVal LogOffName As String, ByVal textbox As String, ByVal iView As Boolean, ByVal XVI As Boolean, ByVal Valid As Boolean, ByVal Fault As Boolean, ByVal FaultParams As DavesCode.FaultParameters) As Boolean
            Dim Successful As Boolean = False
            Try
                Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
                Using myscope As TransactionScope = New TransactionScope()
                    CommitPreClinNew(LinacName, LogOffName, textbox, iView, XVI, Valid, Fault, connectionString)
                    If Fault Then
                        NewFaultHandling.InsertMajorFault(FaultParams, connectionString)
                    End If
                    myscope.Complete()
                    Successful = True
                End Using
            Catch ex As Exception
                NewFaultHandling.LogError(ex)
            End Try
            Return Successful
        End Function
        Shared Function CommitPreClinNew(ByVal LinacN As String, ByVal username As String, ByVal TextBoxp As String, ByVal Imgchk1 As Boolean, ByVal Imgchk2 As Boolean, ByVal Valid As Boolean, ByVal Fault As Boolean, connectionString As String) As String
            'Public Shared Function CommitPreClin(ByVal LinacN As String, ByVal username As String, ByVal TextBoxp As String, ByVal GridViewI As GridView, ByVal Valid As Boolean, ByVal Fault As Boolean) As String
            Dim LogOutStatusID As String
            Dim LogInStatusID As String
            Dim time As DateTime
            Dim LinacName As String = LinacN
            Dim logOutName As String = username
            Dim logInName As String = ""
            time = Now()
            Dim StartTime As DateTime
            Dim reader As SqlDataReader
            Dim EHID As Integer
            Dim Approved As Boolean = Valid
            Dim breakdown As Boolean = Fault
            Dim iView As Boolean = Imgchk1
            Dim XVI As Boolean = Imgchk2
            Dim textbox As String = TextBoxp
            Dim conn As SqlConnection
            Dim SqlDataSource1 As New SqlDataSource()
            Dim commHAuthID As SqlCommand
            Dim contime As SqlCommand
            Dim State As String = "Linac Unauthorised" 'default reason
            Dim UserReason As Integer = 7 'most common reason
            Dim Tab As Integer = 2 'most common tab
            Dim FaultState As String = String.Empty
            conn = New SqlConnection(connectionString)
            'This will get the time the linac was accepted for the pre-clinical
            contime = New SqlCommand("SELECT DateTime, username, stateID, State FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

            contime.Parameters.AddWithValue("@linac", LinacName)
            conn.Open()
            reader = contime.ExecuteReader()

            If reader.Read() Then
                StartTime = reader.Item("DateTime")
                logInName = reader.Item("username")
                LogInStatusID = reader.Item("stateID")
                FaultState = reader.Item("State")
            End If
            reader.Close()
            conn.Close()
            'This calculates the time between logging on and now - but why here?
            Dim minutesDuration As Decimal
            Dim duration As TimeSpan = time - StartTime

            minutesDuration = Decimal.Round(duration.TotalMinutes, 2, MidpointRounding.ToEven)


            'this gets the ID of the associated engineering handover
            commHAuthID = New SqlCommand("Select HandoverID from HandoverEnergies where HandoverID  = (Select max(HandoverID) as lastrecord from HandoverEnergies where linac=@linac)", conn)
            commHAuthID.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
            commHAuthID.Parameters("@linac").Value = LinacName

            conn.Open()
            reader = commHAuthID.ExecuteReader()
            If reader.Read() Then
                EHID = reader.Item("HandoverId")
                reader.Close()
                conn.Close()
            End If
            'This is here because this sub is also called from the fault page in order to write the linacstatus and to write clinicalhandover table
            If breakdown Then
                'changed to user reason 103
                'LogOutStatusID = DavesCode.Reuse.SetStatusNew(logOutName, "Fault", 5, 103, LinacName, -1, ConnectionString)
                State = "Fault"
                UserReason = 103
                iView = False
                XVI = False
                Tab = -1
            Else 'not a breakdown so if approved set clinical - not treating or back to engineering approved
                If Approved Then
                    'LinacStatusID = DavesCode.Reuse.SetStatus(loginName, "Clinical - Not Treating", 5, 7, LinacName, 2)
                    'October change
                    'changed august 21 to allow going to other states
                    'changed clinical to suspended to allow for cancel on clinical.  31 March 2016
                    'LogOutStatusID = DavesCode.Reuse.SetStatusNew(logOutName, "Suspended", 5, 7, LinacName, 2, ConnectionString)
                    State = "Suspended"
                Else
                    'added to make right for midnight check before it just left as engineering approved
                    If logOutName = "System" Then
                        UserReason = 102
                    ElseIf logOutName = "Restored" Then
                        If FaultState = "Fault" Then
                            State = "Fault"
                        End If
                        'Defaults ok
                    Else
                        'added for E1 and E2. Modified 9/4/19 because linac behaviour the same now and B2 included
                        'If LinacName = "E1" Or LinacName = "E2" Or LinacName = "B1" Or LinacName = "T1" Or LinacName = "T2" Then
                        '    Tab = 1
                        'Else
                        '    State = "Engineering Approved"
                        'End If
                        Tab = 1
                    End If

                End If
            End If
            LogOutStatusID = Reuse.SetStatusNew(logOutName, State, 5, UserReason, LinacName, Tab, False, connectionString)
            'http://www.mikesdotnetting.com/Article/53/Saving-a-user%27s-CheckBoxList-selection-and-re-populating-the-CheckBoxList-from-saved-data - used for imaging

            'This writes the clinicalhandover table - doesn't have the user in it

            Dim commaccept As SqlCommand
            commaccept = New SqlCommand("INSERT INTO ClinicalHandover ( CComment,Ehandid, LogOutDate, linac, LogInDate, Duration, iView, XVI, LogOutStatusID, Approved, LogInName, LogOutName, LogInStatusID) " &
                                        "VALUES (@CComment,@Ehandid, @LogOutDate, @linac, @LogInDate, @Duration, @iView, @XVI, @LogOutStatusID, @Approved, @LogInName, @LogOutName, @LogInStatusID)", conn)
            commaccept.Parameters.Add("@CComment", SqlDbType.NVarChar, 250)
            commaccept.Parameters("@CComment").Value = textbox
            commaccept.Parameters.Add("@Ehandid", SqlDbType.Int)
            commaccept.Parameters("@Ehandid").Value = EHID
            commaccept.Parameters.Add("@LogOutDate", SqlDbType.DateTime)
            commaccept.Parameters("@LogOutDate").Value = time
            commaccept.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
            commaccept.Parameters("@linac").Value = LinacName
            commaccept.Parameters.Add("@LogInDate", SqlDbType.DateTime)
            commaccept.Parameters("@LogInDate").Value = StartTime
            commaccept.Parameters.Add("@Duration", SqlDbType.Decimal)
            commaccept.Parameters("@Duration").Value = minutesDuration
            commaccept.Parameters.Add("@iView", SqlDbType.Bit)
            commaccept.Parameters("@iView").Value = iView
            commaccept.Parameters.Add("@XVI", SqlDbType.Bit)
            commaccept.Parameters("@XVI").Value = XVI
            commaccept.Parameters.Add("@LogOutStatusID", SqlDbType.Int)
            commaccept.Parameters("@LogOutStatusID").Value = LogOutStatusID
            commaccept.Parameters.Add("@Approved", SqlDbType.Bit)
            commaccept.Parameters("@Approved").Value = Approved
            commaccept.Parameters.Add("@LogInName", SqlDbType.NVarChar, 50)
            commaccept.Parameters("@LogInName").Value = logInName
            commaccept.Parameters.Add("@LogOutName", SqlDbType.NVarChar, 50)
            commaccept.Parameters("@LogOutName").Value = logOutName
            commaccept.Parameters.Add("@LogInStatusID", SqlDbType.Int)
            commaccept.Parameters("@LogInStatusID").Value = LogInStatusID


            'Try
            conn.Open()
            'Altered 17 March
            'commaccept.ExecuteNonQuery()
            Dim CHANDID As Integer
            Dim obj As Object = commaccept.ExecuteScalar()
            'Dim LinacStatusIDs As String = obj.ToString()
            CHANDID = CInt(obj)

            'Finally
            conn.Close()
            Reuse.UpdateActivityTable(LinacName, LogOutStatusID, connectionString)
            'End Try
            Return LogOutStatusID

        End Function
    End Class
End Namespace
