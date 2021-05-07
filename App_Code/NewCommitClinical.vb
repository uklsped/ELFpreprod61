Imports System.Data
Imports System.Data.SqlClient
Imports System.Transactions
Namespace DavesCode
    Public Class NewCommitClinical
        Public Shared Function CommitClinical(ByVal LinacName As String, ByVal LogOffName As String, ByVal Fault As Boolean, ByVal FaultParams As DavesCode.FaultParameters, ByVal EndofDay As Boolean) As Boolean
            Dim Successful As Boolean = False
            Try
                Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
                Using myscope As TransactionScope = New TransactionScope()
                    CommitClinicalNew(LinacName, LogOffName, Fault, connectionString, EndofDay)
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
        Shared Function CommitClinicalNew(ByVal LinacName As String, ByVal LogOutName As String, ByVal Fault As Boolean, ByVal connectionString As String, ByVal EndofDay As String) As String
            Dim LinacStatusID As Integer
            Dim logInStatusID As Integer
            Dim time As DateTime
            'Dim LinacName As String = LinacN
            'Dim logOutName As String = username
            Dim logInName As String = ""
            time = Now()
            Dim StartTime As DateTime
            Dim reader As SqlDataReader
            Dim CHID As Integer
            Dim breakdown As Boolean = Fault
            'Dim textbox As String = TextBoxc
            Dim conn As SqlConnection
            Dim SqlDataSource1 As New SqlDataSource()
            Dim commHAuthID As SqlCommand
            Dim contime As SqlCommand

            conn = New SqlConnection(connectionString)
            'This will get the time the linac was accepted for clinical
            contime = New SqlCommand("SELECT stateid, DateTime, UserName FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac And state='clinical')", conn)
            contime.Parameters.AddWithValue("@linac", LinacName)
            conn.Open()
            reader = contime.ExecuteReader()

            If reader.Read() Then
                logInStatusID = reader.Item("stateid")
                StartTime = reader.Item("DateTime")
                logInName = reader.Item("UserName")
            End If
            reader.Close()
            conn.Close()
            'This calculates the time between logging on and now - but why here?
            Dim minutesDuration As Decimal
            Dim duration As TimeSpan = time - StartTime

            minutesDuration = Decimal.Round(duration.TotalMinutes, 2, MidpointRounding.ToEven)

            'this gets the ID of the associated engineering handover
            commHAuthID = New SqlCommand("Select CHandID from ClinicalHandover where CHandID  = (Select max(CHandID) as lastrecord from ClinicalHandover where linac=@linac)", conn)
            commHAuthID.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
            commHAuthID.Parameters("@linac").Value = LinacName

            conn.Open()
            reader = commHAuthID.ExecuteReader()
            If reader.Read() Then
                CHID = reader.Item("CHandId")
                reader.Close()
                conn.Close()
            End If
            'Moved If Breakdown after this section, checking and writing treatment table to here because it needs to be checked both for breakdown and if recover button operated.
            'It's only if there is a breakdown or recover that there will be a null entry for treatmentstoptime Not true 22/11/17

            commHAuthID = New SqlCommand("select treatmentid, treatmentstoptime from treatmenttable where treatmentid = (select max(treatmentID) from treatmentTable where linac=@linac)", conn)
            commHAuthID.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
            commHAuthID.Parameters("@linac").Value = LinacName
            conn.Open()
            reader = commHAuthID.ExecuteReader()

            If reader.Read() Then 'this means there is an entry so
                'checking for null from http://www.triconsole.com/dotnet/sqldatareader_class.php#isdbnull
                If reader.IsDBNull(1) Then
                    reader.Close()
                    conn.Close()
                    SetTreatment("Not Treating", LinacName, logInStatusID, connectionString)
                Else
                    reader.Close()
                    conn.Close()
                End If
            Else
                reader.Close()
                conn.Close()
            End If

            'This is here because this sub is also called from the fault page in order to write the linacstatus and to write clinicalhandover table
            If breakdown Then
                LinacStatusID = Reuse.SetStatusNew(LogOutName, "Fault", 5, 103, LinacName, -1, False, connectionString)
            ElseIf LogOutName = "System" Then 'not a breakdown so if approved set clinical - not treating or back to engineering approved
                LinacStatusID = Reuse.SetStatusNew(LogOutName, "Linac Unauthorised", 5, 102, LinacName, 3, False, connectionString)
            ElseIf EndofDay Then
                LinacStatusID = Reuse.SetStatusNew(LogOutName, "Linac Unauthorised", 5, 102, LinacName, 3, False, connectionString)
            Else
                LinacStatusID = Reuse.SetStatusNew(LogOutName, "Suspended", 5, 7, LinacName, 3, False, connectionString)
            End If
            'http://www.mikesdotnetting.com/Article/53/Saving-a-user%27s-CheckBoxList-selection-and-re-populating-the-CheckBoxList-from-saved-data - used for imaging
            'This writes the clinicalstatus table

            Dim commaccept As SqlCommand
            commaccept = New SqlCommand("INSERT INTO ClinicalStatus ( PClinID, LogInDate, LogOutDate, linac, Duration, LogInName, LogOutName,LogOutStatusID, logInStatusID) " &
                                                "VALUES (@PClinID, @LogInDate, @LogOutDate, @linac, @Duration,@LogInName, @LogOutName, @LogOutStatusID, @logInStatusID)", conn)

            commaccept.Parameters.Add("@PClinID", SqlDbType.Int)
            commaccept.Parameters("@PClinID").Value = CHID
            commaccept.Parameters.Add("@LogInDate", SqlDbType.DateTime)
            commaccept.Parameters("@LogInDate").Value = StartTime
            commaccept.Parameters.Add("@LogOutDate", SqlDbType.DateTime)
            commaccept.Parameters("@LogOutDate").Value = time
            commaccept.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
            commaccept.Parameters("@linac").Value = LinacName
            commaccept.Parameters.Add("@Duration", SqlDbType.Decimal)
            commaccept.Parameters("@Duration").Value = minutesDuration
            commaccept.Parameters.Add("@LogInName", SqlDbType.NVarChar, 50)
            commaccept.Parameters("@LogInName").Value = logInName
            commaccept.Parameters.Add("@LogOutName", SqlDbType.NVarChar, 50)
            commaccept.Parameters("@LogOutName").Value = LogOutName
            commaccept.Parameters.Add("@LogOutStatusID", SqlDbType.Int)
            commaccept.Parameters("@LogOutStatusID").Value = LinacStatusID
            commaccept.Parameters.Add("@logInStatusID", SqlDbType.Int)
            commaccept.Parameters("@logInStatusID").Value = logInStatusID
            conn.Open()
            commaccept.ExecuteNonQuery()

            conn.Close()
            Reuse.UpdateActivityTable(LinacName, LinacStatusID, connectionString)

            Return LinacStatusID
        End Function
        Public Shared Sub SetTreatment(ByVal State As String, ByVal linacid As String, ByVal linacstate As String, ByVal connectionString As String)
            Dim time As DateTime

            ' Dim StateType As String = State

            Dim conn As SqlConnection
            'Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            '"connectionstring").ConnectionString
            Dim commstatus As SqlCommand
            time = Now()
            conn = New SqlConnection(connectionString)
            If State = "Treating" Then
                commstatus = New SqlCommand("INSERT INTO TreatmentTable ( TreatmentStartTime, LinacStatusId,linac) " &
                                            "VALUES ( @TreatmentStartTime, @LinacStatusID, @linac)", conn)

                commstatus.Parameters.Add("@TreatmentStartTime", SqlDbType.DateTime)
                commstatus.Parameters("@TreatmentStartTime").Value = time
                commstatus.Parameters.Add("@LinacStatusID", SqlDbType.NVarChar, 10)
                commstatus.Parameters("@LinacStatusId").Value = linacstate
                commstatus.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
                commstatus.Parameters("@linac").Value = linacid
            Else
                'Update command changed 12 April because it should update last row not all rows with same LinacStatusId
                'commstatus = New SqlCommand("UPDATE  TreatmentTable SET TreatmentStopTime = @TreatmentStopTime where LinacStatusid = @LinacStatusID", conn)
                commstatus = New SqlCommand("UPDATE  TreatmentTable SET TreatmentStopTime = @TreatmentStopTime where TreatmentID  = (Select max(treatmentID) as lastrecord from treatmenttable where linac=@linac)", conn)
                commstatus.Parameters.Add("@TreatmentStopTime", SqlDbType.DateTime)
                commstatus.Parameters("@TreatmentStopTime").Value = time
                commstatus.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
                commstatus.Parameters("@linac").Value = linacid

            End If
            'Try
            conn.Open()
            commstatus.ExecuteNonQuery()

            'Finally
            conn.Close()
            'End Try

        End Sub

        Public Shared Function NewWriteClinicalTable(ByVal LinacName As String, ByVal Clinicalcomment As String, ByVal connectionString As String) As Integer
            Dim time As DateTime
            time = Now()
            Dim reader As SqlDataReader
            Dim CID As Integer
            Dim CTID As Integer
            'Dim ClinD As Integer
            Dim StatusID As Integer = 0
            Dim UserName As String = String.Empty
            Dim conn As SqlConnection
            'Dim Clinicalcomment As String = String.Empty
            Dim commCAuthID As SqlCommand
            Dim commclin As SqlCommand

            'Clinicalcomment = Application(BoxChanged)
            conn = New SqlConnection(connectionString)

            commCAuthID = New SqlCommand("Select CHandID, LogOutStatusID from ClinicalHandover where CHandID  = (Select max(CHandID) as lastrecord from ClinicalHandover where linac=@linac)", conn)
            commCAuthID.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
            commCAuthID.Parameters("@linac").Value = LinacName
            conn.Open()
            CID = 0
            reader = commCAuthID.ExecuteReader()
            If reader.Read() Then
                CID = reader.Item("CHandID")
                CTID = reader.Item("LogOutStatusID")
                reader.Close()
                conn.Close()
            End If
            If Not CID = 0 Then
                'Remove user name as this is not the correct name anyway
                commCAuthID = New SqlCommand("Select StateID from LinacStatus where StateID  = (Select max(StateID) as lastrecord from LinacStatus where linac=@linac)", conn)
                commCAuthID.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
                commCAuthID.Parameters("@linac").Value = LinacName
                conn.Open()
                reader = commCAuthID.ExecuteReader()
                If reader.Read() Then
                    StatusID = reader.Item("StateID")

                    reader.Close()
                    conn.Close()
                End If

                commclin = New SqlCommand("INSERT INTO ClinicalTable (PreClinID,LinacStatusID, clinComment,linac, DateTime, Username) " &
                                    "VALUES ( @PreclinID, @LinacStatusID,@clincomment, @linac, @DateTime, @UserName)", conn)

                commclin.Parameters.Add("@PreClinID", SqlDbType.Int)
                commclin.Parameters("@PreclinID").Value = CID
                commclin.Parameters.Add("@LinacStatusID", SqlDbType.Int)
                commclin.Parameters("@LinacStatusID").Value = StatusID
                commclin.Parameters.Add("@clinComment", SqlDbType.NVarChar, 250)
                commclin.Parameters("@clinComment").Value = Clinicalcomment
                commclin.Parameters.Add("@linac", SqlDbType.NVarChar, 10)
                commclin.Parameters("@linac").Value = LinacName
                commclin.Parameters.Add("@DateTime", SqlDbType.DateTime)
                commclin.Parameters("@DateTime").Value = time
                commclin.Parameters.Add("@UserName", SqlDbType.NVarChar, 25)
                commclin.Parameters("@UserName").Value = String.Empty

                conn.Open()
                commclin.ExecuteNonQuery()

                conn.Close()

            End If

            Return StatusID

            'from http://msdn.microsoft.com/en-us/library/system.string.isnullorempty(v=vs.110).aspx

        End Function

    End Class
End Namespace
