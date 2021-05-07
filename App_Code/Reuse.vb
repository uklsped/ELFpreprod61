Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.UI.Page
Imports AjaxControlToolkit
Imports System.Text
Imports System.IO
Imports System.Transactions
Imports GlobalConstants


Namespace DavesCode

    Public Class Reuse

        Public Shared Function GetIPAddress() As String
            Dim context As System.Web.HttpContext = System.Web.HttpContext.Current
            Dim sIPAddress As String = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(sIPAddress) Then
                Return context.Request.ServerVariables("REMOTE_ADDR")
            Else
                Dim ipArray As String() = sIPAddress.Split(New [Char]() {","c})
                Return ipArray(0)
            End If
        End Function
        Public Shared Function ReturnState(ByVal State As String, ByVal Output As Integer) As String
            Dim Statedictionary As New Dictionary(Of Integer, String) From {
                {1, "Linac Unauthorised"},
                {2, "Clinical"},
                {3, "Suspended"}
                }
            If Output <> 0 Then
                Return Statedictionary.Item(State)
            Else
                Return Statedictionary.Keys(Output)

            End If


        End Function
        Public Shared Function ReturnActivity(ByVal activity As String) As String
            Dim Activitydictionary As New Dictionary(Of Integer, String) From {
                {1, "Engineering Run up"},
                {2, "Pre-Clinical Run up"},
                {3, "Clinical"},
                {4, "Planned Maintenance"},
                {5, "Repair"},
                {6, "Physics QA"},
                {7, "Logged Off"},
                {8, "Development/Training"},
                {9, "Emergency Run Up"},
                {100, "Administration"},
                {101, "Lock Elf"},
                {102, "End of Day"},
                {103, "Report Fault"}
            }


            Return Activitydictionary.Item(activity)

        End Function
        Public Shared Function ReturnActivePanel(ByVal userreason As String) As String
            Dim ActivePanelDictionary As New Dictionary(Of Integer, Integer) From {
                {0, 0},
                {1, 1},
                {3, 2},
                {4, 3},
                {5, 4},
                {8, 5},
                {9, 6}
            }
            Return ActivePanelDictionary.Item(userreason)
        End Function


        Public Shared Sub writeLogFile(ByVal useage As Integer, ByVal user As String, ByVal onoff As Boolean)
            'Instrumentation to write to log file

            Dim builder As New StringBuilder
            Dim Reason As Integer = useage
            Dim loginUsername As String = user
            Dim polarity As Boolean = onoff
            Dim statement As String
            If polarity Then
                statement = "linac logged on for "
            Else
                statement = "linac logged off for "
            End If
            ' Append the time to the stringBuilder
            builder.Append(statement)
            builder.Append(Reason)
            ' Append the comment to the StringBuilder
            builder.Append(" by ")
            ' Append a line break
            builder.Append(loginUsername)
            builder.Append(" at ")
            builder.Append(DateTime.Now.ToString("h:mm:ss"))
            'builder.Append(DateTime.Now.ToString("h:mm tt"))
            'see date patterns at http://www.geekzilla.co.uk/View00FF7904-B510-468C-A2C8-F859AA20581F.htm

            'Using streamWriter As StreamWriter = File.AppendText("C:\ELF\TestRun1.txt")
            '    streamWriter.WriteLine(builder)
            'End Using


            'this is the end of the file try


        End Sub


        Public Shared Function StringBuilder(ByVal faulttype As String, ByVal BeginDate As String, StopDate As String) As String
            Dim builder As New StringBuilder
            Dim statement As String
            Dim StartDate As String = BeginDate
            Dim EndDate As String = StopDate
            Dim ending As String
            Dim TypeofFault As String = ""
            Dim Dates As String = StartDate & " And " & EndDate

            ending = "</td></tr></table>"
            statement = "<table border='1' width='100%' height='40px' cellpadding='0' cellspacing='0' bgcolor='#66d9ff'><tr><td>"
            Select Case faulttype
                Case 1
                    TypeofFault = "All Faults"
                Case 2
                    TypeofFault = "All Closed Faults"
                Case 3
                    TypeofFault = "All Concessions"
                Case 4
                    TypeofFault = "All Open Concessions"
                Case 5
                    TypeofFault = "All Faults Opened Between: "
                Case 6
                    TypeofFault = "All Faults Closed Between: "
                Case 7
                    TypeofFault = "All User Closed Faults Between: "
            End Select

            builder.Append(statement)
            builder.Append(TypeofFault)

            Select Case faulttype
                Case 5, 6, 7
                    builder.Append(Dates)
            End Select

            builder.Append(ending)
            Return builder.ToString

        End Function


        '    'from http://www.sqlservercentral.com/Forums/Topic1416029-1292-1.aspx

        Public Shared Function SuccessfulLogin(ByVal username As String, ByVal userpassword As String, ByVal Need As Integer, ByVal Texbox As TextBox, ByVal pasword As TextBox, ByVal logerror As Label) As Integer

            'We need to determine if the user is authenticated and set e.Authenticated accordingly
            'Get the values entered by the user
            'If anything is invalid the popup remains until corrected or the cancel button on the popup is selected
            Dim loginUsername As String = username
            Dim loginPassword As String = userpassword
            Dim Reason As Integer = Need
            Dim textboxUser As TextBox = Texbox
            Dim textboxPass As TextBox = pasword
            Dim LoginErrorDetail As Label = logerror
            'Dim modalpopupident As ModalPopupExtender = modalp
            'First check if user name and password are correct
            If Membership.ValidateUser(loginUsername, loginPassword) Then
                'Find out which user group user is in
                'Only roles that are valid will get to here
                Dim usergroupselected As Integer = GetRole(loginUsername)
                'No database action here
                If LogOn(usergroupselected, Reason) Then
                    Return usergroupselected
                Else
                    'Tell them they don't have permission

                    If (Not textboxUser Is Nothing) Then
                        textboxUser.Text = String.Empty
                        LoginErrorDetail.Text = "You don't have permission to perform that action."
                        'modalpopupident.Show()

                    End If

                End If

            Else
                'See if this user exists in the database
                Dim userInfo As MembershipUser = Nothing
                Try
                    userInfo = Membership.GetUser(loginUsername)
                Catch e As ArgumentException
                    LoginErrorDetail.Text = "Your username is invalid."
                    'Return Nothing
                End Try
                If userInfo Is Nothing Then
                    'The user entered an invalid username...

                    If (Not textboxUser Is Nothing) Then
                        If textboxUser.Text = String.Empty Then
                            LoginErrorDetail.Text = "Please Enter Your User Name."
                        Else
                            textboxUser.Text = String.Empty
                            LoginErrorDetail.Text = "Your username is invalid."
                        End If

                    End If
                    'Return Nothing
                Else
                    'The password was incorrect (don't show anything, the Login control already describes the problem)

                    If (Not textboxPass Is Nothing) Then
                        If textboxPass.Text = String.Empty Then
                            LoginErrorDetail.Text = "Please Enter Your Password."
                        Else
                            textboxPass.Text = String.Empty
                            LoginErrorDetail.Text = "Your Password is invalid."
                            ' modalpopupident.Show()

                        End If

                    End If
                    'Return Nothing
                End If
            End If
        End Function ' Don't want to return if invalid user name or password. Want to leave pop up until correct value or cancel is selected.
        'Used Finds out which user group user is in
        Public Shared Function GetRole(ByVal user As String) As Integer
            Dim loginUsername As String = user
            'This won't work if new users are added.
            Dim Roledictionary As New Dictionary(Of String, Integer) From {
                {"Administrator", 1},
                {"Engineer", 2},
                {"Radiographer", 3},
                {"Physicist", 4},
                {"None", 5}
            }
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("userstring").ConnectionString
            Dim con As New SqlConnection(connectionString)
            con.Open()
            Dim cmd As New SqlCommand("select r.rolename from aspnet_Roles r " &
            "left outer join aspnet_UsersInRoles ur on ur.RoleId = r.RoleId " &
            "left outer join aspnet_Users u on u.UserId=ur.UserId " &
            "where u.UserName = @UserName", con)
            cmd.Parameters.AddWithValue("@UserName", loginUsername)
            Dim da As New SqlDataAdapter(cmd)
            Dim dt As New DataTable()
            Dim userrolename As String
            da.Fill(dt)
            If dt.Rows.Count > 0 Then

                For Each dataRow As DataRow In dt.Rows

                    userrolename = dataRow("RoleName")
                Next
            End If
            con.Close()
            'if userrolename does not exist in the dictionary then the error page is called and returned to calling page
            'So only returned value will be a valid value from this function
            Return Roledictionary.Item(userrolename)
        End Function
        'Used
        Public Shared Function LogOn(ByVal usergroup As Integer, ByVal userreason As Integer) As Boolean
            ' this checks for valid usergroup for this log in for a particular reason. The linac entry is obsolete and is removed.
            Dim usergroupselected As Integer = usergroup
            Dim reason As Integer = userreason

            Select Case reason
                Case 1, 4, 5, 101

                    If usergroupselected = 2 Or usergroupselected = 4 Then

                        Return True

                    Else
                        Return False

                    End If


                Case 2, 3, 9

                    If usergroupselected = 3 Then

                        Return True
                    Else
                        Return False
                    End If

                Case 6
                    If usergroupselected = 4 Or usergroupselected = 2 Then

                        Return True
                    Else
                        Return False
                    End If
                    '104 is recovery and at the moment let anyone do that
                    'change 11 to only allow rad to select RAD RESET in defect uc
                    'Added 12 to allow for tomo unrecoverable fault
                Case 8, 10, 12, 102, 103, 104
                    Return True

                Case 11
                    If usergroupselected = 3 Then
                        Return True
                    Else
                        Return False
                    End If

                Case 100
                    If usergroupselected = 1 Then
                        Return True
                    Else
                        Return False
                    End If

                Case Else
                    Return False
            End Select

        End Function


        '        'http://www.aspsnippets.com/Articles/Return-Identity-Auto-Increment-Column-value-after-record-insert-in-SQL-Server-Database-using-ADONet-with-C-and-VBNet.aspx
        Public Shared Function SetMachineState(ByVal linacName As String, ByVal unlock As Boolean, ByVal LoggedOn As Boolean, ConnectionString As String) As String

            Dim time As DateTime
            time = Now()
            Dim LinacStatusID As Integer
            Dim reader As SqlDataReader
            Dim nowstatus As String
            Dim conn As SqlConnection
            Dim loginname As String = HttpContext.Current.Session("name").ToString
            Dim Possessreason As Integer = HttpContext.Current.Session("userreason")
            Dim usergroup As Integer = HttpContext.Current.Session("usergroup")
            'Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            '"connectionstring").ConnectionString
            Dim Machinestatus As SqlCommand
            Dim StatusNow As SqlCommand

            conn = New SqlConnection(ConnectionString)
            'Added because state wasn't being changed from Suspended to Clinical 31 March 2016 Bug 7
            If Possessreason = 3 Then
                nowstatus = "Clinical"
            Else
                StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                StatusNow.Parameters.AddWithValue("@linac", linacName)
                conn.Open()
                reader = StatusNow.ExecuteReader()

                If reader.Read() Then
                    nowstatus = reader.Item("state").ToString()
                Else
                    'this caters for the case where this is the first record for the linac
                    nowstatus = "Linac Unauthorised"
                End If
                reader.Close()
                conn.Close()
            End If
            'commented out code removed 15 April 2016

            Machinestatus = New SqlCommand("INSERT INTO LinacStatus (state, DateTime, usergroup,userreason,linac, UserName, LoggedOn ) " &
                                        "VALUES (@state, @Datetime, @usergroup, @userreason, @linac, @UserName, @LoggedOn) SELECT SCOPE_IDENTITY()", conn)
            Machinestatus.Parameters.Add("@state", System.Data.SqlDbType.NVarChar, 50)
            Machinestatus.Parameters("@state").Value = nowstatus
            Machinestatus.Parameters.Add("@DateTime", System.Data.SqlDbType.DateTime)
            Machinestatus.Parameters("@DateTime").Value = time
            Machinestatus.Parameters.Add("@usergroup", System.Data.SqlDbType.Int)
            Machinestatus.Parameters("@usergroup").Value = usergroup
            Machinestatus.Parameters.Add("@userreason", System.Data.SqlDbType.Int)
            Machinestatus.Parameters("@userreason").Value = Possessreason
            Machinestatus.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
            Machinestatus.Parameters("@linac").Value = linacName
            Machinestatus.Parameters.Add("@UserName", System.Data.SqlDbType.NVarChar, 50)
            Machinestatus.Parameters("@UserName").Value = loginname
            Machinestatus.Parameters.Add("@LoggedOn", System.Data.SqlDbType.Bit)
            Machinestatus.Parameters("@LoggedOn").Value = LoggedOn

            Try
                'To get the identity of the record just inserted from
                'http://www.aspsnippets.com/Articles/Return-Identity-Auto-Increment-Column-value-after-record-insert-in-SQL-Server-Database-using-ADONet-with-C-and-VBNet.aspx
                conn.Open()
                'commstatus.ExecuteNonQuery()

                Dim obj As Object = Machinestatus.ExecuteScalar()
                'Dim LinacStatusIDs As String = obj.ToString()
                LinacStatusID = CInt(obj)
                conn.Close()
                'This creates in the Activity table the entry for the start of an activity so long as it is not as a result of switching the user.
                If Not unlock Then
                    WriteActivityTableNew(LinacStatusID, time, Possessreason, linacName, ConnectionString)
                End If
                Return nowstatus
            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)
            Finally
                conn.Close()
            End Try
        End Function

        Public Shared Sub MachineStateNew(ByVal loginuser As String, ByVal usergroup As Integer, ByVal linac As String, ByVal possessreason As Integer, ByVal unlock As Boolean, ByVal LoggedOn As Boolean, ConnectionString As String)
            'Need Some Error Handling in this function
            Dim loginName As String = loginuser
            Dim time As DateTime
            time = Now()

            Dim LinacStatusID As Integer
            Dim reader As SqlDataReader
            Dim nowstatus As String
            Dim linacName As String = linac
            Dim conn As SqlConnection
            'Dim logname As String = HttpContext.Current.Session("name").ToString

            'Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            '"connectionstring").ConnectionString
            Dim Machinestatus As SqlCommand
            Dim StatusNow As SqlCommand

            conn = New SqlConnection(ConnectionString)
            'Added because state wasn't being changed from Suspended to Clinical 31 March 2016 Bug 7
            If possessreason = 3 Then
                nowstatus = "Clinical"
            Else
                StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                StatusNow.Parameters.AddWithValue("@linac", linacName)
                conn.Open()
                reader = StatusNow.ExecuteReader()

                If reader.Read() Then
                    nowstatus = reader.Item("state").ToString()
                Else
                    'this caters for the case where this is the first record for the linac
                    nowstatus = "Linac Unauthorised"
                End If
                reader.Close()
                conn.Close()
            End If
            'commented out code removed 15 April 2016

            Machinestatus = New SqlCommand("INSERT INTO LinacStatus (state, DateTime, usergroup,userreason,linac, UserName, LoggedOn ) " &
                                        "VALUES (@state, @Datetime, @usergroup, @userreason, @linac, @UserName, @LoggedOn) SELECT SCOPE_IDENTITY()", conn)
            Machinestatus.Parameters.Add("@state", System.Data.SqlDbType.NVarChar, 50)
            Machinestatus.Parameters("@state").Value = nowstatus
            Machinestatus.Parameters.Add("@DateTime", System.Data.SqlDbType.DateTime)
            Machinestatus.Parameters("@DateTime").Value = time
            Machinestatus.Parameters.Add("@usergroup", System.Data.SqlDbType.Int)
            Machinestatus.Parameters("@usergroup").Value = usergroup
            Machinestatus.Parameters.Add("@userreason", System.Data.SqlDbType.Int)
            Machinestatus.Parameters("@userreason").Value = possessreason
            Machinestatus.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
            Machinestatus.Parameters("@linac").Value = linacName
            Machinestatus.Parameters.Add("@UserName", System.Data.SqlDbType.NVarChar, 50)
            Machinestatus.Parameters("@UserName").Value = loginName
            Machinestatus.Parameters.Add("@LoggedOn", System.Data.SqlDbType.Bit)
            Machinestatus.Parameters("@LoggedOn").Value = LoggedOn

            Try
                'To get the identity of the record just inserted from
                'http://www.aspsnippets.com/Articles/Return-Identity-Auto-Increment-Column-value-after-record-insert-in-SQL-Server-Database-using-ADONet-with-C-and-VBNet.aspx
                conn.Open()
                'commstatus.ExecuteNonQuery()

                Dim obj As Object = Machinestatus.ExecuteScalar()
                'Dim LinacStatusIDs As String = obj.ToString()
                LinacStatusID = CInt(obj)
                conn.Close()
                'This creates in the Activity table the entry for the start of an activity so long as it is not as a result of switching the user.
                If Not unlock Then
                    WriteActivityTableNew(LinacStatusID, time, possessreason, linacName, ConnectionString)
                End If
            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)
            Finally
                conn.Close()
            End Try


        End Sub

        Public Shared Sub WriteActivityTableNew(ByVal StateID As Integer, ByVal timestamp As DateTime, ByVal possessreason As Integer, ByVal linacname As String, ByVal ConnectionString As String)
            Dim laststateid As Integer = 0
            Dim lastState As String = ""
            Dim Activestatus As SqlCommand
            Dim conn As SqlConnection
            Dim lastusername As String = Nothing
            Dim lastusergroup As Integer = Nothing
            'Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            'Try
            laststateid = GetLastTechNew(linacname, 1, lastState, lastusername, lastusergroup, ConnectionString)


            conn = New SqlConnection(ConnectionString)

            Activestatus = New SqlCommand("INSERT INTO ActiveTime (userreason, StartID, StartTime, Linac, PreviousStateID ) " &
                                        "VALUES (@userreason,@StartID, @StartTime, @Linac, @PreviousStateID)", conn)
            Activestatus.Parameters.Add("@userreason", System.Data.SqlDbType.Int)
            Activestatus.Parameters("@userreason").Value = possessreason
            Activestatus.Parameters.Add("@StartID", System.Data.SqlDbType.Int)
            Activestatus.Parameters("@StartID").Value = StateID
            Activestatus.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime)
            Activestatus.Parameters("@StartTime").Value = timestamp
            Activestatus.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
            Activestatus.Parameters("@linac").Value = linacname
            Activestatus.Parameters.Add("@PreviousStateID", System.Data.SqlDbType.Int)
            Activestatus.Parameters("@PreviousStateID").Value = laststateid


            conn.Open()

            Activestatus.ExecuteNonQuery()
            'Catch ex As Exception
            '    DavesCode.NewFaultHandling.LogError(ex)

            'Finally
            conn.Close()
            'End Try

        End Sub
        Public Shared Sub UpdateActivityTable(ByVal linac As String, ByVal StopID As String, ByVal connectionString As String)

            Dim reader As SqlDataReader
            Dim ActID As String
            Dim linacName As String = linac
            Dim conn As SqlConnection

            'Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            '"connectionstring").ConnectionString

            Dim StatusNow As SqlCommand
            Dim UpdateNow As SqlCommand
            conn = New SqlConnection(connectionString)
            'Added because state wasn't being changed from Suspended to Clinical 31 March 2016 Bug 7
            'Try

            StatusNow = New SqlCommand("SELECT ActID FROM [ActiveTime] where ActID = (Select max(ActID) as lastrecord from [ActiveTime] where linac=@linac)", conn)
            StatusNow.Parameters.AddWithValue("@linac", linacName)
            conn.Open()
            reader = StatusNow.ExecuteReader()

            If reader.Read() Then
                ActID = reader.Item("ActID").ToString
            End If
            reader.Close()
            conn.Close()

            UpdateNow = New SqlCommand("Update ActiveTime SET StopID=@StopID,StopTime=@StopTime WHERE ActID=@ActID", conn)
            UpdateNow.Parameters.Add("@StopID", System.Data.SqlDbType.Int)
            UpdateNow.Parameters("@StopID").Value = StopID
            UpdateNow.Parameters.Add("@StopTime", System.Data.SqlDbType.DateTime)
            UpdateNow.Parameters("@StopTime").Value = Now()
            UpdateNow.Parameters.Add("@ActID", System.Data.SqlDbType.Int)
            UpdateNow.Parameters("@ActID").Value = ActID
            conn.Open()
            UpdateNow.ExecuteNonQuery()
            'Catch ex As Exception
            '    DavesCode.NewFaultHandling.LogError(ex)

            'Finally
            conn.Close()
            'End Try
        End Sub



        '    'http://www.mikesdotnetting.com/Article/53/Saving-a-user%27s-CheckBoxList-selection-and-re-populating-the-CheckBoxList-from-saved-data - used for imaging




        '    http://www.mikesdotnetting.com/Article/53/Saving-a-user%27s-CheckBoxList-selection-and-re-populating-the-CheckBoxList-from-saved-data - used for imaging


        Public Shared Function CommitClinical(ByVal LinacN As String, ByVal username As String, ByVal Fault As Boolean) As String

            '        'checking for null from http://www.triconsole.com/dotnet/sqldatareader_class.php#isdbnull

            '    http://www.mikesdotnetting.com/Article/53/Saving-a-user%27s-CheckBoxList-selection-and-re-populating-the-CheckBoxList-from-saved-data - used for imaging

            '    Return LinacStatusID
        End Function


        'Used
        Public Shared Function SetStatus(ByVal loginName As String, ByVal State As String, ByVal UserGroup As Integer, ByVal Userreason As Integer, ByVal linacid As String, ByVal Activity As Integer, LoggedOn As Boolean) As Integer
            Dim time As DateTime
            Dim userName As String = loginName
            Dim StateType As String = State
            Dim UserType As Integer = UserGroup
            Dim ReasonType As Integer = Userreason
            Dim MachineType As String = linacid
            Dim minutesDuration As Decimal
            Dim StartTime As DateTime
            Dim LinacStatusID As String
            time = Now()
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            'Dim commstatus As SqlCommand

            If Activity <> 0 Then
                CalculateDuration(MachineType, time, StartTime, minutesDuration)
            End If

            conn = New SqlConnection(connectionString)
            Using (conn)
                Dim commstatus As New SqlCommand With {
                .Connection = conn
                }
                Dim ObjTransaction As SqlTransaction
                ObjTransaction = Nothing
                Try
                    conn.Open()
                    ObjTransaction = conn.BeginTransaction()
                    commstatus.CommandText = "usp_InsertLinacStatus"
                    commstatus.CommandType = CommandType.StoredProcedure
                    commstatus.Transaction = ObjTransaction
                    'commstatus = New SqlCommand("INSERT INTO LinacStatus ( State, DateTime, Usergroup, Userreason, linac, UserName) " &
                    '"VALUES ( @State, @Datetime, @Usergroup, @Userreason, @linac, @UserName) SELECT SCOPE_IDENTITY()", conn)
                    commstatus.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 50)
                    commstatus.Parameters("@State").Value = StateType
                    commstatus.Parameters.Add("@DateTime", System.Data.SqlDbType.DateTime)
                    commstatus.Parameters("@DateTime").Value = time
                    commstatus.Parameters.Add("@usergroup", System.Data.SqlDbType.Int)
                    commstatus.Parameters("@usergroup").Value = UserType
                    commstatus.Parameters.Add("@userreason", System.Data.SqlDbType.Int)
                    commstatus.Parameters("@userreason").Value = ReasonType
                    commstatus.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
                    commstatus.Parameters("@linac").Value = MachineType
                    commstatus.Parameters.Add("@UserName", System.Data.SqlDbType.NVarChar, 50)
                    commstatus.Parameters("@UserName").Value = userName
                    commstatus.Parameters.Add("@LoggedOn", System.Data.SqlDbType.NVarChar, 50)
                    commstatus.Parameters("@LoggedOn").Value = LoggedOn
                    Dim outPutParameter = New SqlParameter With {
                        .ParameterName = "@LinacStatusID",
                        .SqlDbType = System.Data.SqlDbType.Int,
                        .Direction = System.Data.ParameterDirection.Output
                        }
                    commstatus.Parameters.Add(outPutParameter)
                    commstatus.ExecuteNonQuery()
                    ObjTransaction.Commit()
                    commstatus.Parameters.Clear()
                    Dim Newstatusid As String = outPutParameter.Value.ToString
                    LinacStatusID = CInt(Newstatusid)
                    'Try
                    'To get the identity of the record just inserted from
                    'http://www.aspsnippets.com/Articles/Return-Identity-Auto-Increment-Column-value-after-record-insert-in-SQL-Server-Database-using-ADONet-with-C-and-VBNet.aspx
                    'conn.Open()
                    'commstatus.ExecuteNonQuery()

                    'Dim obj As Object = commstatus.ExecuteScalar()
                    '    'Dim LinacStatusIDs As String = obj.ToString()
                    '    LinacStatusID = CInt(obj)
                    '    conn.Close()
                    If Activity > 0 Then
                        WriteDurationnew(MachineType, Activity, time, StartTime, minutesDuration, LinacStatusID)
                    End If
                Catch ex As Exception
                    ObjTransaction.Rollback()
                    LinacStatusID = -1
                    NewFaultHandling.LogError(ex)
                Finally
                    conn.Close()
                End Try
            End Using
            Return LinacStatusID
        End Function
        Public Shared Function SetStatusNew(ByVal loginName As String, ByVal State As String, ByVal UserGroup As Integer, ByVal Userreason As Integer, ByVal linacid As String, ByVal Activity As Integer, ByVal LoggedOn As Boolean, ByVal ConnectionString As String) As Integer
            Dim time As DateTime
            Dim userName As String = loginName
            Dim StateType As String = State
            Dim UserType As Integer = UserGroup
            Dim ReasonType As Integer = Userreason
            Dim MachineType As String = linacid
            Dim minutesDuration As Decimal
            Dim StartTime As DateTime
            Dim LinacStatusID As String
            time = Now()
            Dim conn As SqlConnection
            'Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            '"connectionstring").ConnectionString
            'Dim commstatus As SqlCommand

            If Activity <> 0 Then
                'CalculateDuration(MachineType, time, StartTime, minutesDuration)
            End If

            conn = New SqlConnection(ConnectionString)
            Using (conn)
                Dim commstatus As New SqlCommand With {
                .Connection = conn
                }
                Try
                    'Dim ObjTransaction As SqlTransaction
                    'ObjTransaction = Nothing
                    'Try
                    conn.Open()
                    'ObjTransaction = conn.BeginTransaction()
                    commstatus.CommandText = "usp_InsertLinacStatus"
                    commstatus.CommandType = CommandType.StoredProcedure
                    'commstatus.Transaction = ObjTransaction
                    'commstatus = New SqlCommand("INSERT INTO LinacStatus ( State, DateTime, Usergroup, Userreason, linac, UserName) " &
                    '"VALUES ( @State, @Datetime, @Usergroup, @Userreason, @linac, @UserName) SELECT SCOPE_IDENTITY()", conn)
                    commstatus.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 50)
                    commstatus.Parameters("@State").Value = StateType
                    commstatus.Parameters.Add("@DateTime", System.Data.SqlDbType.DateTime)
                    commstatus.Parameters("@DateTime").Value = time
                    commstatus.Parameters.Add("@usergroup", System.Data.SqlDbType.Int)
                    commstatus.Parameters("@usergroup").Value = UserType
                    commstatus.Parameters.Add("@userreason", System.Data.SqlDbType.Int)
                    commstatus.Parameters("@userreason").Value = ReasonType
                    commstatus.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
                    commstatus.Parameters("@linac").Value = MachineType
                    commstatus.Parameters.Add("@UserName", System.Data.SqlDbType.NVarChar, 50)
                    commstatus.Parameters("@UserName").Value = userName
                    commstatus.Parameters.Add("@LoggedOn", System.Data.SqlDbType.Bit)
                    commstatus.Parameters("@LoggedOn").Value = LoggedOn
                    Dim outPutParameter = New SqlParameter With {
                            .ParameterName = "@LinacStatusID",
                            .SqlDbType = System.Data.SqlDbType.Int,
                            .Direction = System.Data.ParameterDirection.Output
                            }
                    commstatus.Parameters.Add(outPutParameter)
                    commstatus.ExecuteNonQuery()
                    'ObjTransaction.Commit()
                    commstatus.Parameters.Clear()
                    Dim Newstatusid As String = outPutParameter.Value.ToString
                    LinacStatusID = CInt(Newstatusid)
                    'Try
                    'To get the identity of the record just inserted from
                    'http://www.aspsnippets.com/Articles/Return-Identity-Auto-Increment-Column-value-after-record-insert-in-SQL-Server-Database-using-ADONet-with-C-and-VBNet.aspx
                    'conn.Open()
                    'commstatus.ExecuteNonQuery()

                    'Dim obj As Object = commstatus.ExecuteScalar()
                    '    'Dim LinacStatusIDs As String = obj.ToString()
                    '    LinacStatusID = CInt(obj)
                    '    conn.Close()
                    If Activity > 0 Then
                        'WriteDurationnew(MachineType, Activity, time, StartTime, minutesDuration, LinacStatusID)
                    End If
                Catch ex As Exception

                    LinacStatusID = -1
                    NewFaultHandling.LogError(ex)
                Finally
                    conn.Close()
                End Try
            End Using
            Return LinacStatusID
        End Function
        'Used
        Public Shared Sub WriteDurationnew(ByVal Linac As String, ByVal ActivityIn As Integer, ByVal EndTime As DateTime, ByVal StartingTime As DateTime, ByVal Duration As Decimal, ByVal LinacStatusID As Integer)
            Dim time As DateTime
            time = EndTime
            Dim StartTime As DateTime
            StartTime = StartingTime
            Dim LinacName As String = Linac
            Dim Activity As Integer = ActivityIn
            Dim minutesDuration As Decimal
            minutesDuration = Duration
            Dim LinacStateID As Integer
            LinacStateID = LinacStatusID
            Dim conn As SqlConnection
            Dim comm As SqlCommand

            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString

            conn = New SqlConnection(connectionString)
            comm = New SqlCommand("INSERT INTO WriteDuration (Linac, Activity, EndTime,  StartTime, Duration, StatusID) " &
                                        "VALUES (@linac, @Activity, @EndTime, @StartTime, @Duration, @StatusID)", conn)
            comm.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
            comm.Parameters("@linac").Value = LinacName
            comm.Parameters.Add("@Activity", System.Data.SqlDbType.Int)
            comm.Parameters("@Activity").Value = Activity
            comm.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime)
            comm.Parameters("@EndTime").Value = time
            comm.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime)
            comm.Parameters("@StartTime").Value = StartTime
            comm.Parameters.Add("@Duration", System.Data.SqlDbType.Decimal)
            comm.Parameters("@Duration").Value = minutesDuration
            comm.Parameters.Add("@StatusID", System.Data.SqlDbType.Int)
            comm.Parameters("@StatusID").Value = LinacStateID

            conn.Open()
            Try
                comm.ExecuteNonQuery()
            Catch ex As Exception
                'Continue
            End Try

            conn.Close()
        End Sub

        Public Shared Sub WriteDuration(ByVal Linac As String, ByVal EndTime As DateTime)
            Dim time As DateTime
            time = EndTime
            Dim reader As SqlDataReader
            Dim StartTime As DateTime
            Dim LinacName As String = Linac
            'Dim Activity As Integer = ActivityIn
            Dim Activity As Integer
            Dim conn As SqlConnection
            Dim comm As SqlCommand
            Dim contime As SqlCommand
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString

            conn = New SqlConnection(connectionString)

            contime = New SqlCommand("SELECT DateTime, userreason FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

            contime.Parameters.AddWithValue("@linac", LinacName)
            conn.Open()
            reader = contime.ExecuteReader()

            If reader.Read() Then
                StartTime = reader.Item("DateTime")
                Activity = reader.Item("userreason")
            End If
            reader.Close()
            conn.Close()

            Dim minutesDuration As Decimal
            Dim duration As TimeSpan = time - StartTime

            minutesDuration = Decimal.Round(duration.TotalMinutes, 2, MidpointRounding.ToEven)


            comm = New SqlCommand("INSERT INTO WriteDuration (Linac, Activity, EndTime,  StartTime, Duration) " &
                                        "VALUES (@linac, @Activity, @EndTime, @StartTime, @Duration)", conn)
            comm.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
            comm.Parameters("@linac").Value = LinacName
            comm.Parameters.Add("@Activity", System.Data.SqlDbType.Int)
            comm.Parameters("@Activity").Value = Activity
            comm.Parameters.Add("@EndTime", System.Data.SqlDbType.DateTime)
            comm.Parameters("@EndTime").Value = time
            comm.Parameters.Add("@StartTime", System.Data.SqlDbType.DateTime)
            comm.Parameters("@StartTime").Value = StartTime
            comm.Parameters.Add("@Duration", System.Data.SqlDbType.Decimal)
            comm.Parameters("@Duration").Value = minutesDuration

            conn.Open()
            comm.ExecuteNonQuery()
            conn.Close()

        End Sub

        Public Shared Sub CalculateDuration(ByVal Linac As String, ByVal EndTime As DateTime, ByRef StartTime As DateTime, ByRef minutesDuration As Decimal)
            Dim time As DateTime
            time = EndTime
            Dim reader As SqlDataReader
            Dim LinacName As String = Linac
            Dim conn As SqlConnection
            Dim contime As SqlCommand
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString

            conn = New SqlConnection(connectionString)

            contime = New SqlCommand("SELECT DateTime FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

            contime.Parameters.AddWithValue("@linac", LinacName)
            conn.Open()
            reader = contime.ExecuteReader()

            If reader.Read() Then
                StartTime = reader.Item("DateTime")
            End If
            reader.Close()
            conn.Close()

            Dim duration As TimeSpan = time - StartTime

            minutesDuration = Decimal.Round(duration.TotalMinutes, 2, MidpointRounding.ToEven)

        End Sub

        'Used
        Public Shared Function GetLastState(ByVal linac As String, ByVal index As Integer) As String
            Dim time As DateTime
            time = Now()
            Dim PreviousState As Integer = index
            Dim reader As SqlDataReader
            Dim nowstatus As String
            Dim linacName As String = linac
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            'Dim Machinestatus As SqlCommand
            Dim StatusNow As SqlCommand
            conn = New SqlConnection(connectionString)
            Select Case PreviousState
                Case 0
                    StatusNow = New SqlCommand("SELECT state, username, usergroup, stateID FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                Case 1
                    StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)
                Case 2
                    StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 3 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)
            End Select
            'If PreviousState = 0 Then
            '    StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
            'Else
            '    'This doesn't work it just gets penultimate record irrespective of linac
            '    'StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select (max(stateID)-1) as penultimaterecord from [LinacStatus] where linac=@linac)", conn)
            '    'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
            '    StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

            'End If
            StatusNow.Parameters.AddWithValue("@linac", linacName)
            conn.Open()
            reader = StatusNow.ExecuteReader()

            If reader.Read() Then
                nowstatus = reader.Item("state").ToString()
            Else
                'added for problem of when new linac so no previous state 6/7/17
                nowstatus = "Linac Unauthorised"
            End If
            reader.Close()
            conn.Close()
            'nowstatus = "Fault"
            Return nowstatus
        End Function

        Public Shared Function GetLastStateNew(ByVal linac As String, ByVal index As Integer, ByVal connectionString As String) As String
            Dim time As DateTime
            time = Now()
            Dim PreviousState As Integer = index
            Dim reader As SqlDataReader
            Dim nowstatus As String
            Dim linacName As String = linac
            Dim conn As SqlConnection
            Dim StatusNow As SqlCommand
            conn = New SqlConnection(connectionString)

            'If PreviousState = 0 Then
            '    StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
            'Else
            '    'This doesn't work it just gets penultimate record irrespective of linac
            '    'StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select (max(stateID)-1) as penultimaterecord from [LinacStatus] where linac=@linac)", conn)
            '    'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
            '    StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

            'End If
            'StatusNow.Parameters.AddWithValue("@linac", linacName)
            'conn.Open()
            'reader = StatusNow.ExecuteReader()

            'If reader.Read() Then
            '    nowstatus = reader.Item("state").ToString()
            'Else
            '    'added for problem of when new linac so no previous state 6/7/17
            '    nowstatus = Nothing
            'End If
            'reader.Close()
            'conn.Close()

            'Return nowstatus
            nowstatus = "Linac Unauthorised"
        End Function

        Public Shared Function GetLastTime(ByVal linac As String, ByVal index As Integer) As String
            Dim time As DateTime
            time = Now()
            Dim PreviousState As Integer = index
            Dim reader As SqlDataReader
            Dim nowstatus As String = "Error"
            Dim linacName As String = linac
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            'Dim Machinestatus As SqlCommand
            Dim StatusNow As SqlCommand
            Dim ResetDayCom As SqlCommand
            Dim oldtime As DateTime
            Dim activity As String = String.Empty
            Dim StatusID As String = String.Empty
            Dim oldDayofyear As Integer
            Dim newDayofyear As Integer
            Dim Status As String = String.Empty
            Dim LoggedOn As Boolean ' this is set to 100 to detect if Appstate is null later.
            'Dim LogOn As String
            'Dim LiveTab As String
            'Dim SuspValue As String
            'Dim RunupVal As String

            'LogOn = "LogOn" + linacName
            'LiveTab = "ActTab" + linacName
            'SuspValue = "Suspended" + linacName
            'RunupVal = "rppTab" + linacName

            conn = New SqlConnection(connectionString)

            'DavesCode.Reuse.RecordStates(linacName, 0, "getlasttime1", 0)

            'If (Not HttpContext.Current.Application(LogOn) Is Nothing) Then
            '    AppState = CInt(HttpContext.Current.Application(LogOn))
            'End If

            'DavesCode.Reuse.RecordStates(linacName, 0, "getlasttime2", 0)

            If PreviousState = 0 Then

                'StatusNow = New SqlCommand("SELECT dateadd(dd,0, datediff(dd,0,datetime)) FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

                'StatusNow = New SqlCommand("SELECT stateid, datetime, userreason FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                StatusNow = New SqlCommand("SELECT TOP 1 stateid, state, datetime, userreason, LoggedOn FROM [LinacStatus] where linac=@linac order by StateID desc", conn)
            Else
                'This doesn't work it just gets penultimate record irrespective of linac
                'StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select (max(stateID)-1) as penultimaterecord from [LinacStatus] where linac=@linac)", conn)
                'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
                StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

            End If
            StatusNow.Parameters.AddWithValue("@linac", linacName)
            conn.Open()
            reader = StatusNow.ExecuteReader()



            If reader.Read() Then
                oldtime = reader.Item("datetime")
                activity = reader.Item("userreason")
                StatusID = reader.Item("stateid")
                Status = reader.Item("state")
                If Not Convert.IsDBNull(reader.Item("LoggedOn")) Then
                    LoggedOn = reader.Item("LoggedOn")
                Else
                    LoggedOn = False
                End If

                'oldtime = oldtime.Date.AddDays(-1) 'test line
                oldDayofyear = oldtime.DayOfYear
                    newDayofyear = time.DayOfYear


                    If activity = "102" Then
                        nowstatus = "Ignore"
                    ElseIf Not newDayofyear = oldDayofyear Then
                        nowstatus = ENDOFDAY
                    ElseIf newDayofyear = oldDayofyear Then
                        nowstatus = "Ignore"
                    End If
                Else
                    nowstatus = "Error"
            End If
            reader.Close()
            conn.Close()

            'This line checks to see if Appstate was null, if it was it will still be 100 from the start of the sub.
            'If it is null then the application states are reset depending on the last entry in the database.
            If nowstatus IsNot "Error" Then
                'If AppState = 200 Then
                '    Select Case activity
                '        Case 101, 102, 7, 103
                '            HttpContext.Current.Application(LogOn) = 0
                '        Case Else
                '            HttpContext.Current.Application(LogOn) = 1
                '            HttpContext.Current.Application(LiveTab) = activity

                '    End Select
                '    Select Case Status
                '        Case "Suspended"
                '            HttpContext.Current.Application(SuspValue) = 1
                '        Case "Engineering Approved"
                '            HttpContext.Current.Application(RunupVal) = 1

                '    End Select
                'End If
                'DavesCode.Reuse.RecordStates(linacName, 0, "getlasttime3", 0)

                'this an instrumentation table it could be removed at a later update.
                If nowstatus = GlobalConstants.ENDOFDAY Then
                    ResetDayCom = New SqlCommand("INSERT INTO ResetDay (DateCreated,StateID, State, ApplicationState, Activity, OldDayofYear, newDayofYear,nowstatus, Linac) VALUES (@DateCreated,@StateID, @State, @ApplicationState, @Activity, @OldDayofYear, @newDayofYear,@nowstatus, @Linac)", conn)
                    ResetDayCom.Parameters.Add("@DateCreated", System.Data.SqlDbType.DateTime)
                    ResetDayCom.Parameters("@DateCreated").Value = time
                    ResetDayCom.Parameters.Add("@StateID", System.Data.SqlDbType.Int)
                    ResetDayCom.Parameters("@StateID").Value = StatusID
                    ResetDayCom.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 50)
                    ResetDayCom.Parameters("@State").Value = Status
                    ResetDayCom.Parameters.Add("@ApplicationState", System.Data.SqlDbType.Bit)
                    ResetDayCom.Parameters("@ApplicationState").Value = LoggedOn
                    ResetDayCom.Parameters.Add("@Activity", System.Data.SqlDbType.Int)
                    ResetDayCom.Parameters("@Activity").Value = activity
                    ResetDayCom.Parameters.Add("@OldDayofYear", System.Data.SqlDbType.Int)
                    ResetDayCom.Parameters("@OldDayofYear").Value = oldDayofyear
                    ResetDayCom.Parameters.Add("@newDayofYear", System.Data.SqlDbType.Int)
                    ResetDayCom.Parameters("@newDayofYear").Value = newDayofyear
                    ResetDayCom.Parameters.Add("@nowstatus", System.Data.SqlDbType.NVarChar, 10)
                    ResetDayCom.Parameters("@nowstatus").Value = nowstatus
                    ResetDayCom.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
                    ResetDayCom.Parameters("@linac").Value = linacName

                    conn.Open()
                    ResetDayCom.ExecuteNonQuery()
                    conn.Close()
                End If
            End If
            Return nowstatus
        End Function

        Public Shared Sub RecordStates(ByVal linac As String, ByVal Setuptab As Integer, ByVal Callingfunction As String, ByVal index As Integer)
            Dim time As DateTime
            time = Now()
            Dim PreviousState As Integer = index
            Dim reader As SqlDataReader
            Dim linacName As String = linac
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            'Dim Machinestatus As SqlCommand
            Dim StatusNow As SqlCommand
            Dim RecordStatesCom As SqlCommand
            Dim activity As Integer = 0
            Dim StateID As Integer = 0
            Dim Status As String = ""
            Dim AppState As Integer = 99 ' this is set to 100 to detect if Appstate is null later.
            'Dim Appstate As Integer
            Dim LogOn As String
            Dim LiveTab As String
            Dim SuspValue As String
            Dim RunupVal As String
            Dim LoggedOn As Boolean
            LoggedOn = DavesCode.GetApplication.GetApplicationState(linac, 0)
            LogOn = "LogOn" + linacName
            LiveTab = "ActTab" + linacName
            SuspValue = "Suspended" + linacName
            RunupVal = "rppTab" + linacName
            Dim ActiveTab As Integer
            Dim Runup As Integer
            Dim suspended As Integer
            conn = New SqlConnection(connectionString)



            'If (Not HttpContext.Current.Application(LogOn) Is Nothing) Then
            '    AppState = CInt(HttpContext.Current.Application(LogOn))
            'Else
            '    AppState = HttpContext.Current.Application(LogOn)
            'End If
            If (Not HttpContext.Current.Application(LiveTab) Is Nothing) Then
                ActiveTab = CInt(HttpContext.Current.Application(LogOn))
            End If
            If (Not HttpContext.Current.Application(SuspValue) Is Nothing) Then
                suspended = CInt(HttpContext.Current.Application(SuspValue))
            End If
            If (Not HttpContext.Current.Application(RunupVal) Is Nothing) Then
                Runup = CInt(HttpContext.Current.Application(RunupVal))
            End If


            If PreviousState = 0 Then

                'StatusNow = New SqlCommand("SELECT dateadd(dd,0, datediff(dd,0,datetime)) FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

                'StatusNow = New SqlCommand("SELECT stateid, datetime, userreason FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                StatusNow = New SqlCommand("SELECT TOP 1 stateid, state, datetime, userreason FROM [LinacStatus] where linac=@linac order by StateID desc", conn)
            Else
                'This doesn't work it just gets penultimate record irrespective of linac
                'StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select (max(stateID)-1) as penultimaterecord from [LinacStatus] where linac=@linac)", conn)
                'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
                StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

            End If
            StatusNow.Parameters.AddWithValue("@linac", linacName)
            conn.Open()
            reader = StatusNow.ExecuteReader()



            If reader.Read() Then
                activity = reader.Item("userreason")
                StateID = reader.Item("StateID")
                Status = reader.Item("State")


            Else

            End If
            reader.Close()
            RecordStatesCom = New SqlCommand("INSERT INTO RecordStates (Callingfunction,Setuptab,State, LogOn,Suspended, rppTab, ActTab,StateID,DateTime,Linac) VALUES (@Callingfunction,@Setuptab,@State, @LogOn,@Suspended, @rppTab, @ActTab,@StateID,@DateTime,@Linac)", conn)

            RecordStatesCom.Parameters.Add("@Callingfunction", System.Data.SqlDbType.NVarChar, 50)
            RecordStatesCom.Parameters("@Callingfunction").Value = Callingfunction
            RecordStatesCom.Parameters.Add("@Setuptab", System.Data.SqlDbType.Int)
            RecordStatesCom.Parameters("@Setuptab").Value = Setuptab
            RecordStatesCom.Parameters.Add("@State", System.Data.SqlDbType.NVarChar, 50)
            RecordStatesCom.Parameters("@State").Value = Status
            RecordStatesCom.Parameters.Add("@LogOn", System.Data.SqlDbType.Bit)
            RecordStatesCom.Parameters("@LogOn").Value = LoggedOn
            RecordStatesCom.Parameters.Add("@Suspended", System.Data.SqlDbType.Int)
            RecordStatesCom.Parameters("@Suspended").Value = suspended
            RecordStatesCom.Parameters.Add("@rppTab", System.Data.SqlDbType.Int)
            RecordStatesCom.Parameters("@rppTab").Value = Runup
            RecordStatesCom.Parameters.Add("@ActTab", System.Data.SqlDbType.Int)
            RecordStatesCom.Parameters("@ActTab").Value = activity
            RecordStatesCom.Parameters.Add("@StateID", System.Data.SqlDbType.Int)
            RecordStatesCom.Parameters("@StateID").Value = StateID
            RecordStatesCom.Parameters.Add("@DateTime", System.Data.SqlDbType.DateTime)
            RecordStatesCom.Parameters("@DateTime").Value = time
            RecordStatesCom.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
            RecordStatesCom.Parameters("@linac").Value = linacName


            RecordStatesCom.ExecuteNonQuery()
            conn.Close()
        End Sub

        Public Shared Sub ListParameters(ByVal linac As String, ByVal index As Integer)
            Dim time As DateTime
            time = Now()
            Dim PreviousState As Integer = index
            Dim reader As SqlDataReader
            Dim nowstatus As String = "Error"
            Dim linacName As String = linac
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            'Dim Machinestatus As SqlCommand
            Dim StatusNow As SqlCommand
            Dim ResetDayCom As SqlCommand
            Dim oldtime As DateTime
            Dim activity As String = ""
            Dim StateID As String = ""
            Dim oldDayofyear As Integer
            Dim newDayofyear As Integer
            Dim Status As String = ""
            Dim AppState As Integer = 100 ' this is set to 100 to detect if Appstate is null later.
            Dim LogOn As String
            Dim LiveTab As String
            Dim SuspValue As String
            Dim RunupVal As String

            LogOn = "LogOn" + linacName
            LiveTab = "ActTab" + linacName
            SuspValue = "Suspended" + linacName
            RunupVal = "rppTab" + linacName
            Dim ActiveTab As Integer
            Dim Runup As Integer
            Dim suspended As Integer
            conn = New SqlConnection(connectionString)



            'If (Not HttpContext.Current.Application(LogOn) Is Nothing) Then
            '    AppState = CInt(HttpContext.Current.Application(LogOn))
            'End If
            If (Not HttpContext.Current.Application(LiveTab) Is Nothing) Then
                ActiveTab = CInt(HttpContext.Current.Application(LogOn))
            End If
            If (Not HttpContext.Current.Application(SuspValue) Is Nothing) Then
                suspended = CInt(HttpContext.Current.Application(SuspValue))
            End If
            If (Not HttpContext.Current.Application(RunupVal) Is Nothing) Then
                Runup = CInt(HttpContext.Current.Application(RunupVal))
            End If



            If PreviousState = 0 Then

                'StatusNow = New SqlCommand("SELECT dateadd(dd,0, datediff(dd,0,datetime)) FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

                'StatusNow = New SqlCommand("SELECT stateid, datetime, userreason FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                StatusNow = New SqlCommand("SELECT TOP 1 stateid, state, datetime, userreason FROM [LinacStatus] where linac=@linac order by StateID desc", conn)
            Else
                'This doesn't work it just gets penultimate record irrespective of linac
                'StatusNow = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select (max(stateID)-1) as penultimaterecord from [LinacStatus] where linac=@linac)", conn)
                'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
                StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

            End If
            StatusNow.Parameters.AddWithValue("@linac", linacName)
            conn.Open()
            reader = StatusNow.ExecuteReader()



            If reader.Read() Then
                oldtime = reader.Item("datetime")
                activity = reader.Item("userreason")
                StateID = reader.Item("StateID")
                Status = reader.Item("State")
                'oldtime = oldtime.Date.AddDays(-1) 'test line
                oldDayofyear = oldtime.DayOfYear
                newDayofyear = time.DayOfYear

                activity = reader.Item("userreason")
                If activity = "102" Then
                    nowstatus = "Ignore"
                ElseIf Not newDayofyear = oldDayofyear Then
                    nowstatus = "EndDay"
                ElseIf newDayofyear = oldDayofyear Then
                    nowstatus = "Ignore"
                End If
            Else
                nowstatus = "Error"
            End If
            reader.Close()
            conn.Close()



        End Sub

        Public Shared Function GetLastTech(ByVal linac As String, ByVal index As Integer, ByRef lastState As String, ByRef lastusername As String, ByRef lastusergroup As Integer) As Integer
            'Changed this to a function to return the linac state ID where necessary
            Dim laststateid As Integer = 0
            Dim PreviousState As Integer = index
            Dim reader As SqlDataReader
            Dim linacName As String = linac
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            'Dim Machinestatus As SqlCommand
            Dim StatusNow As SqlCommand

            conn = New SqlConnection(connectionString)
            Try
                Select Case PreviousState
                    Case 0
                        StatusNow = New SqlCommand("SELECT state, username, usergroup, stateID FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                    Case 1
                        StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)
                    Case 2
                        StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 3 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)
                End Select
                'If PreviousState = 0 Then
                '    StatusNow = New SqlCommand("SELECT state, username, usergroup, stateID FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
                'Else
                '    'This doesn't work it just gets penultimate record irrespective of linac
                '    'StatusNow = New SqlCommand("SELECT state, username, usergroup FROM [LinacStatus] where stateID = (Select (max(stateID)-1) as penultimaterecord from [LinacStatus] where linac=@linac)", conn)
                '    'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
                '    StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

                'End If
                StatusNow.Parameters.AddWithValue("@linac", linacName)
                conn.Open()
                reader = StatusNow.ExecuteReader()

                If reader.Read() Then
                    lastState = reader.Item("state").ToString()
                    lastusername = reader.Item("username").ToString()
                    lastusergroup = reader.Item("usergroup")
                    laststateid = reader.Item("stateID")
                End If
            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)
                'RaiseLoadError()

            Finally
                reader.Close()
                conn.Close()
            End Try

            Return laststateid
        End Function
        Public Shared Function GetLastTechNew(ByVal linac As String, ByVal index As Integer, ByRef lastState As String, ByRef lastusername As String, ByRef lastusergroup As Integer, ByVal ConnectionString As String) As Integer
            'Changed this to a function to return the linac state ID where necessary
            Dim laststateid As Integer = 0
            Dim PreviousState As Integer = index
            Dim reader As SqlDataReader
            Dim linacName As String = linac
            Dim conn As SqlConnection
            'Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            '"connectionstring").ConnectionString
            'Dim keyestatus As SqlCommand
            Dim StatusNow As SqlCommand

            conn = New SqlConnection(ConnectionString)

            If PreviousState = 0 Then
                StatusNow = New SqlCommand("SELECT state, username, usergroup, stateID FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)
            Else
                'This doesn't work it just gets penultimate record irrespective of linac
                'StatusNow = New SqlCommand("SELECT state, username, usergroup FROM [LinacStatus] where stateID = (Select (max(stateID)-1) as penultimaterecord from [LinacStatus] where linac=@linac)", conn)
                'from http://stackoverflow.com/questions/8198962/taking-the-second-last-row-with-only-one-select-in-sql-server
                StatusNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

            End If
            StatusNow.Parameters.AddWithValue("@linac", linacName)
            conn.Open()
            reader = StatusNow.ExecuteReader()

            If reader.Read() Then
                lastState = reader.Item("state").ToString()
                lastusername = reader.Item("username").ToString()
                lastusergroup = reader.Item("usergroup")
                laststateid = reader.Item("stateID")
            End If
            reader.Close()
            conn.Close()

            Return laststateid
        End Function

        Public Shared Function CheckForOpenFault(ByVal machinename As String, ByVal connectionString As String) As Boolean
            Dim openfault As Boolean = False
            Dim conn As SqlConnection
            Dim existingfault As SqlCommand
            Dim LinacStatusID As String = ""
            Dim reader As SqlDataReader
            conn = New SqlConnection(connectionString)

            existingfault = New SqlCommand("SELECT TOP(1) [IncidentID], [StatusID] FROM [FaultIDTable] where Linac = @linac and ReportClosed is Null and statusid is not NULL ORDER BY [IncidentID] DESC", conn)
            existingfault.Parameters.AddWithValue("@linac", machinename)
            conn.Open()
            reader = existingfault.ExecuteReader()
            If reader.HasRows() Then
                'Have to now actually read the rows
                reader.Read()
                LinacStatusID = reader.Item("StatusID").ToString()
                openfault = True

            End If
            conn.Close()
            Return openfault

        End Function

        Public Shared Sub ArchiveEnergies(ByVal EnergyIndex As Integer)
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            Dim archive As SqlCommand

            conn = New SqlConnection(connectionString)

            archive = New SqlCommand("Insert INTO  [PhysicsEnergiesArchive] Select * from [PhysicsEnergies] where EnergyID = @EnergyID", conn)
            archive.Parameters.AddWithValue("@EnergyID", EnergyIndex)
            conn.Open()
            archive.ExecuteNonQuery()
            conn.Close()

        End Sub

        Public Shared Sub ReturnImaging(ByRef iView As Boolean, ByRef XVi As Boolean, ByVal GridViewImage As GridView, ByVal MachineName As String)
            Dim cb As CheckBox
            Select Case MachineName

                Case "LA1", "LA2", "LA3"
                    cb = CType(GridViewImage.Rows(0).FindControl("RowLevelCheckBoxImage"), CheckBox)
                    iView = cb.Checked
                    'added E1 and LA6 for eastbourne 4/7/17
                Case Else
                    cb = CType(GridViewImage.Rows(0).FindControl("RowLevelCheckBoxImage"), CheckBox)
                    iView = cb.Checked
                    cb = CType(GridViewImage.Rows(1).FindControl("RowLevelCheckBoxImage"), CheckBox)
                    XVi = cb.Checked
            End Select
        End Sub

        'Public Shared Function GetlastActivity(ByVal Linac As String, Index As Integer) As String

        '    Dim reader As SqlDataReader
        '    Dim activityNow As SqlCommand
        '    Dim linacName As String = Linac
        '    Dim conn As SqlConnection
        '    Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        '    Dim activity As String = String.Empty


        '    conn = New SqlConnection(connectionString)
        '    Select Case Index
        '        Case 0
        '            activityNow = New SqlCommand("SELECT TOP 1  userreason FROM [LinacStatus] where linac=@linac order by StateID desc", conn)
        '        Case 1
        '            activityNow = New SqlCommand("SELECT TOP 1 * From (select Top 2 * from (select * from [LinacStatus] where linac=@linac) as a ORDER BY a.stateid DESC)  as x ORDER BY x.stateid", conn)

        '    End Select

        '    activityNow.Parameters.AddWithValue("@linac", linacName)
        '            conn.Open()
        '            reader = activityNow.ExecuteReader()

        '    If reader.Read() Then
        '                activity = reader.Item("userreason")

        '            End If
        '            reader.Close()
        '            conn.Close()
        '            Return activity
        'End Function

    End Class

End Namespace
