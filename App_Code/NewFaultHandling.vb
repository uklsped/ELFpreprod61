Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Namespace DavesCode

    Public Class NewFaultHandling
        'used in DeviceRepeatfault, DeviceSave,DeviceSavePark
        'Public Shared Function InsertRepeatFault(ByVal Description As String, ByVal ReportedBy As String, ByVal DateReported As DateTime, ByVal Area As String, ByVal Energy As String, ByVal GantryAngle As String, ByVal CollimatorAngle As String, ByVal Device As String, ByVal IncidentID As Integer, ByVal PatientID As String, ByVal ConcessionNumber As String, ByVal Reportable As Boolean) As Integer
        Public Shared Function InsertRepeatFault(ByVal FaultP As DavesCode.FaultParameters) As Boolean
            Dim Result As Boolean = False
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            conn = New SqlConnection(connectionString)

            Using (conn)

                Dim incidentfault As New SqlCommand With {
                    .Connection = conn
                }

                Dim ObjTransaction As SqlTransaction
                ObjTransaction = Nothing

                Try
                    conn.Open()
                    ObjTransaction = conn.BeginTransaction()
                    incidentfault.CommandText = "usp_ReportFault"
                    incidentfault.CommandType = CommandType.StoredProcedure
                    incidentfault.Transaction = ObjTransaction
                    incidentfault.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar, 250)
                    incidentfault.Parameters("@Description").Value = FaultP.FaultDescription
                    incidentfault.Parameters.Add("@ReportedBy", System.Data.SqlDbType.NVarChar, 50)
                    incidentfault.Parameters("@ReportedBy").Value = FaultP.UserInfo
                    incidentfault.Parameters.Add("@DateReported", System.Data.SqlDbType.DateTime)
                    incidentfault.Parameters("@DateReported").Value = FaultP.DateInserted
                    incidentfault.Parameters.Add("@Area", System.Data.SqlDbType.NVarChar, 20)
                    incidentfault.Parameters("@Area").Value = FaultP.Area
                    incidentfault.Parameters.Add("@Energy", System.Data.SqlDbType.NVarChar, 10)
                    incidentfault.Parameters("@Energy").Value = FaultP.Energy
                    incidentfault.Parameters.Add("@GantryAngle", System.Data.SqlDbType.NVarChar, 3)
                    incidentfault.Parameters("@GantryAngle").Value = FaultP.GantryAngle
                    incidentfault.Parameters.Add("@CollimatorAngle", System.Data.SqlDbType.NVarChar, 3)
                    incidentfault.Parameters("@CollimatorAngle").Value = FaultP.CollimatorAngle
                    incidentfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                    incidentfault.Parameters("@Linac").Value = FaultP.Linac
                    incidentfault.Parameters.Add("@IncidentID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@IncidentID").Value = FaultP.SelectedIncident
                    incidentfault.Parameters.Add("@BSUHID", System.Data.SqlDbType.VarChar, 7)
                    incidentfault.Parameters("@BSUHID").Value = FaultP.PatientID
                    incidentfault.Parameters.Add("@ConcessionNumber", System.Data.SqlDbType.NVarChar, 25)
                    incidentfault.Parameters("@ConcessionNumber").Value = FaultP.ConcessionNumber
                    incidentfault.Parameters.Add("@OriginalFaultID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@OriginalFaultID").Value = -1
                    incidentfault.Parameters.Add("@RadiationIncident", System.Data.SqlDbType.Bit)
                    incidentfault.Parameters("@RadiationIncident").Value = FaultP.RadioIncident
                    incidentfault.Parameters.Add("@Activity", System.Data.SqlDbType.NVarChar, 3)
                    incidentfault.Parameters("@Activity").Value = FaultP.Activity
                    incidentfault.Parameters.Add("@LastState", System.Data.SqlDbType.NVarChar, 25)
                    incidentfault.Parameters("@LastState").Value = FaultP.LastState
                    incidentfault.ExecuteNonQuery()
                    incidentfault.Parameters.Clear()

                    ObjTransaction.Commit()
                    Result = True

                Catch ex As Exception

                    ObjTransaction.Rollback()
                    LogError(ex)
                    Result = False
                Finally
                    incidentfault.Parameters.Clear()
                    conn.Close()

                End Try
            End Using
            Return Result
        End Function
        'Used in ViewOpenFaults to change to concession or to update concession
        'Public Shared Function InsertNewConcession(ByVal ConcessionDescription As String, ByVal LinacName As String, ByVal IncidentID As Integer, ByVal ReportedBy As String, ByVal ConcessionAction As String) As Integer
        Public Shared Function CreateObject(ByVal IncidentID As Integer, ByVal Linac As String, ByRef ConcessP As DavesCode.ConcessionParameters) As Integer
            Dim success As Boolean = False
            Dim conn As SqlConnection
            Dim comm As SqlCommand
            Dim connectionString1 As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            conn = New SqlConnection(connectionString1)
            comm = New SqlCommand("select distinct f.status, ISNULL(c.ConcessionNumber, '') as ConcessionNumber , ISNULL(c.concessiondescription, '') as ConcessionDescription, ISNULL(c.action, '') as Action, ISNULL(c.AssignedTo, 'Unassigned') as AssignedTo " _
            & "from FaultIDTable f left outer join ConcessionTable c on f.ConcessionNumber=c.ConcessionNumber where f.incidentID = @incidentID", conn)
            comm.Parameters.AddWithValue("@incidentID", IncidentID)

            Try
                conn.Open()
                Dim da As New SqlDataAdapter(comm)
                Dim dt As New DataTable()

                da.Fill(dt)
                If dt.Rows.Count > 0 Then

                    For Each dataRow As DataRow In dt.Rows

                        ConcessP.PresentFaultState = dataRow("Status")
                        ConcessP.ConcessionNumber = dataRow("ConcessionNumber")
                        ConcessP.ConcessionDescription = dataRow("ConcessionDescription")
                        ConcessP.ConcessionAction = dataRow("action")
                        ConcessP.AssignedTo = dataRow("AssignedTo")

                    Next
                    ConcessP.IncidentID = IncidentID
                    ConcessP.Linac = Linac
                    ConcessP.UserInfo = String.Empty
                    success = True
                Else
                    success = False
                End If

            Catch ex As Exception
                    LogError(ex)
                success = False
            Finally
                    conn.Close()

                End Try
            'End Using

            Return success
        End Function
        Public Shared Function InsertNewConcession(ByVal ConcessP As DavesCode.ConcessionParameters) As Integer
            Dim Countcommandtext As String = "select count(*) from Concessiontable where incidentID=@incidentID"
            Dim Getfaultstatus As String = "select Status From FaultIDTable where incidentID = @incidentID"
            Dim Status As String
            Dim exists As Integer
            Dim time As DateTime = Now()
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            conn = New SqlConnection(connectionString)

            Using (conn)
                Dim incidentfault As New SqlCommand With {
                    .Connection = conn
                }

                Dim ObjTransaction As SqlTransaction
                ObjTransaction = Nothing
                Try
                    conn.Open()
                    ObjTransaction = conn.BeginTransaction()
                    incidentfault.CommandText = Getfaultstatus
                    incidentfault.CommandType = CommandType.Text
                    incidentfault.Transaction = ObjTransaction
                    incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@incidentID").Value = ConcessP.IncidentID
                    Status = CType(incidentfault.ExecuteScalar(), String)
                    incidentfault.Parameters.Clear()

                    incidentfault.CommandText = Countcommandtext
                    incidentfault.CommandType = CommandType.Text
                    incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@incidentID").Value = ConcessP.IncidentID
                    exists = CInt(incidentfault.ExecuteScalar())
                    incidentfault.Parameters.Clear()

                    If (exists = 0) And ((Status = "Open") Or (Status = "New")) Then
                        'commconcess.ExecuteNonQuery()
                        'from http://www.dotnetperls.com/string-format-vbnet

                        incidentfault.CommandText = "usp_CreateNewConcession"
                        incidentfault.CommandType = CommandType.StoredProcedure
                        incidentfault.Parameters.Add("@ConcessionDescription", System.Data.SqlDbType.NVarChar, 25)
                        incidentfault.Parameters("@ConcessionDescription").Value = ConcessP.ConcessionDescription
                        incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                        incidentfault.Parameters("@incidentID").Value = ConcessP.IncidentID
                        incidentfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                        incidentfault.Parameters("@Linac").Value = ConcessP.Linac
                        incidentfault.Parameters.Add("@ConcessionActive", System.Data.SqlDbType.Bit)
                        incidentfault.Parameters("@ConcessionActive").Value = 1
                        incidentfault.Parameters.Add("@Action", System.Data.SqlDbType.NVarChar, 250)
                        incidentfault.Parameters("@Action").Value = ConcessP.ConcessionAction
                        incidentfault.Parameters.Add("@AssignedTo", System.Data.SqlDbType.NVarChar, 50)
                        incidentfault.Parameters("@AssignedTo").Value = ConcessP.AssignedTo
                        incidentfault.Parameters.Add("@ReportedBy", System.Data.SqlDbType.NVarChar, 250)
                        incidentfault.Parameters("@ReportedBy").Value = ConcessP.UserInfo
                        incidentfault.Parameters.Add("@Lastupdatedon", System.Data.SqlDbType.DateTime)
                        incidentfault.Parameters("@Lastupdatedon").Value = time
                        incidentfault.Parameters.Add("@TrackingComment", System.Data.SqlDbType.NVarChar, 250)
                        incidentfault.Parameters("@TrackingComment").Value = ConcessP.ConcessionComment
                        Dim outPutParameter = New SqlParameter With {
                        .ParameterName = "@TrackingNum",
                        .SqlDbType = System.Data.SqlDbType.Int,
                        .Direction = System.Data.ParameterDirection.Output
                        }
                        incidentfault.Parameters.Add(outPutParameter)
                        incidentfault.ExecuteNonQuery()
                        ObjTransaction.Commit()
                        incidentfault.Parameters.Clear()

                    ElseIf (((exists = 0) And (Status = "Concession")) Or ((exists = 1) And (Not Status = "Concession"))) Then
                        exists = -1
                        ObjTransaction.Rollback()
                        LogAnomaly(ConcessP.Linac, "InsertNewConcession", "Error exists = 0 but Status = Concession")
                    End If
                Catch ex As Exception
                    ObjTransaction.Rollback()
                    exists = -1
                    LogError(ex)

                Finally
                    conn.Close()
                End Try
            End Using

            Return exists
        End Function
        'Used to update tracking in ViewOpenFaults
        'Public Shared Function UpdateTracking(ByVal TrackingComment As String, ByVal Assigned As String, ByVal Status As String, ByVal ReportedBy As String, ByVal LinacName As String, ByVal Action As String, ByVal IncidentID As Integer, ByVal concess As Integer) As String
        Public Shared Function UpdateTracking(ByVal ConcessP As DavesCode.ConcessionParameters) As String

            Dim time As DateTime = Now()
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            conn = New SqlConnection(connectionString)
            Dim trackingID As Integer = 0
            Dim ConcessionActive As Integer = 0
            If (ConcessP.FutureFaultState = "Closed") AndAlso (ConcessP.PresentFaultState = "Concession") Then
                ConcessionActive = 1
            End If

            Using (conn)

                Dim incidentfault As New SqlCommand With {
                    .Connection = conn
                }

                Dim ObjTransaction As SqlTransaction
                ObjTransaction = Nothing

                Try
                    conn.Open()
                    ObjTransaction = conn.BeginTransaction()
                    incidentfault.CommandText = "usp_ClassicFaultTracking"
                    incidentfault.CommandType = CommandType.StoredProcedure
                    incidentfault.Transaction = ObjTransaction
                    'incidentfault.Parameters.Add("@Trackingcomment", System.Data.SqlDbType.NVarChar, 250)
                    'incidentfault.Parameters("@Trackingcomment").Value = TrackingComment
                    'incidentfault.Parameters.Add("@AssignedTo", Data.SqlDbType.NVarChar, 50)
                    'incidentfault.Parameters("@AssignedTo").Value = Assigned
                    'incidentfault.Parameters.Add("@Status", Data.SqlDbType.NVarChar, 50)
                    'incidentfault.Parameters("@Status").Value = Status
                    'incidentfault.Parameters.Add("@LastupdatedBy", System.Data.SqlDbType.NVarChar, 50)
                    'incidentfault.Parameters("@LastupdatedBy").Value = ReportedBy
                    'incidentfault.Parameters.Add("@Lastupdatedon", System.Data.SqlDbType.DateTime)
                    'incidentfault.Parameters("@Lastupdatedon").Value = time
                    'incidentfault.Parameters.Add("@linacName", System.Data.SqlDbType.NVarChar, 10)
                    'incidentfault.Parameters("@linacName").Value = LinacName
                    'incidentfault.Parameters.Add("@action", System.Data.SqlDbType.NVarChar, 250)
                    'incidentfault.Parameters("@action").Value = Action
                    'incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                    'incidentfault.Parameters("@incidentID").Value = IncidentID
                    incidentfault.Parameters.Add("@Trackingcomment", System.Data.SqlDbType.NVarChar, 250)
                    incidentfault.Parameters("@Trackingcomment").Value = ConcessP.ConcessionComment
                    incidentfault.Parameters.Add("@AssignedTo", Data.SqlDbType.NVarChar, 50)
                    incidentfault.Parameters("@AssignedTo").Value = ConcessP.AssignedTo
                    incidentfault.Parameters.Add("@Status", Data.SqlDbType.NVarChar, 50)
                    incidentfault.Parameters("@Status").Value = ConcessP.FutureFaultState
                    incidentfault.Parameters.Add("@LastupdatedBy", System.Data.SqlDbType.NVarChar, 50)
                    incidentfault.Parameters("@LastupdatedBy").Value = ConcessP.UserInfo
                    incidentfault.Parameters.Add("@Lastupdatedon", System.Data.SqlDbType.DateTime)
                    incidentfault.Parameters("@Lastupdatedon").Value = time
                    incidentfault.Parameters.Add("@linacName", System.Data.SqlDbType.NVarChar, 10)
                    incidentfault.Parameters("@linacName").Value = ConcessP.Linac
                    incidentfault.Parameters.Add("@action", System.Data.SqlDbType.NVarChar, 250)
                    incidentfault.Parameters("@action").Value = ConcessP.ConcessionAction
                    incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@incidentID").Value = ConcessP.IncidentID
                    incidentfault.Parameters.Add("@concess", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@concess").Value = ConcessionActive
                    incidentfault.Parameters.Add("@ConcessionDescription", System.Data.SqlDbType.NVarChar, 250)
                    incidentfault.Parameters("@ConcessionDescription").Value = ConcessP.ConcessionDescription
                    Dim outPutParameter = New SqlParameter With {
                        .ParameterName = "@TrackingID",
                        .SqlDbType = System.Data.SqlDbType.Int,
                        .Direction = System.Data.ParameterDirection.Output
                        }
                    incidentfault.Parameters.Add(outPutParameter)
                    incidentfault.ExecuteNonQuery()
                    Dim NewTrackID As String = outPutParameter.Value.ToString
                    trackingID = CInt(NewTrackID)
                    incidentfault.Parameters.Clear()
                    outPutParameter.ParameterName = Nothing

                    incidentfault.CommandText = "usp_SetRadAckFault"
                    incidentfault.CommandType = CommandType.StoredProcedure
                    incidentfault.Transaction = ObjTransaction
                    incidentfault.Parameters.Add("@IncidentID", Data.SqlDbType.Int)
                    incidentfault.Parameters("@IncidentID").Value = ConcessP.IncidentID
                    incidentfault.Parameters.Add("@TrackingID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@TrackingID").Value = trackingID
                    incidentfault.Parameters.Add("@Acknowledge", Data.SqlDbType.Bit)
                    incidentfault.Parameters("@Acknowledge").Value = True
                    incidentfault.ExecuteNonQuery()
                    incidentfault.Parameters.Clear()

                    ObjTransaction.Commit()


                Catch ex As Exception

                    ObjTransaction.Rollback()
                    LogError(ex)
                    trackingID = -1
                Finally
                    conn.Close()
                End Try

            End Using


            Return trackingID
        End Function
        'Public Shared Function InsertNewFault(ByVal State As String, ByVal LinacName As String, ByVal DateInserted As DateTime, ByVal Description As String, ByVal ReportedBy As String, ByVal Area As String, ByVal Energy As String, ByVal GantryAngle As String, ByVal CollimatorAngle As String, ByVal PatientID As String, ByVal ConcessionDescription As String, ByVal ConcessionAction As String, ByVal RadiationIncident As Boolean) As Integer
        Public Shared Function InsertNewFault(ByVal State As String, ByVal FaultP As DavesCode.FaultParameters) As Integer
            Dim time As DateTime = Now()
            Dim IncidentID As Integer = 0
            Dim Result As Boolean = False
            Dim LastFault As Integer = 0
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            conn = New SqlConnection(connectionString)
            Dim ConcessionNumber = ""
            Dim logInStatusID As Integer = 0
            Dim constateid As SqlCommand
            constateid = New SqlCommand("SELECT stateid FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

            constateid.Parameters.AddWithValue("@linac", FaultP.Linac)
            Dim readers As SqlDataReader
            conn.Open()
            readers = constateid.ExecuteReader()

            If readers.Read() Then
                logInStatusID = CInt(readers.Item("stateid"))
            End If
            readers.Close()
            conn.Close()

            Using (conn)

                Dim incidentfault As New SqlCommand With {
                    .Connection = conn
                }

                Dim ObjTransaction As SqlTransaction
                ObjTransaction = Nothing

                Try
                    conn.Open()
                    ObjTransaction = conn.BeginTransaction()
                    incidentfault.CommandText = "usp_InsertNewFault"
                    incidentfault.CommandType = CommandType.StoredProcedure
                    incidentfault.Transaction = ObjTransaction

                    incidentfault.Parameters.Add("@DateInserted", System.Data.SqlDbType.DateTime)
                    incidentfault.Parameters("@DateInserted").Value = FaultP.DateInserted
                    incidentfault.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, 20)
                    incidentfault.Parameters("@Status").Value = State
                    incidentfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                    incidentfault.Parameters("@Linac").Value = FaultP.Linac
                    incidentfault.Parameters.Add("@StateID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@StateID").Value = logInStatusID

                    Dim outPutParameter = New SqlParameter With {
                        .ParameterName = "@IncidentID",
                        .SqlDbType = System.Data.SqlDbType.Int,
                        .Direction = System.Data.ParameterDirection.Output
                    }
                    incidentfault.Parameters.Add(outPutParameter)

                    incidentfault.ExecuteNonQuery()
                    Dim NewIncident As String = outPutParameter.Value.ToString
                    IncidentID = CInt(NewIncident)

                    incidentfault.Parameters.Clear()
                    outPutParameter.ParameterName = Nothing

                    incidentfault.CommandText = "usp_ReportFault"
                    incidentfault.CommandType = CommandType.StoredProcedure
                    incidentfault.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar, 250)
                    incidentfault.Parameters("@Description").Value = FaultP.FaultDescription
                    incidentfault.Parameters.Add("@ReportedBy", System.Data.SqlDbType.NVarChar, 50)
                    incidentfault.Parameters("@ReportedBy").Value = FaultP.UserInfo
                    incidentfault.Parameters.Add("@DateReported", System.Data.SqlDbType.DateTime)
                    incidentfault.Parameters("@DateReported").Value = FaultP.DateInserted
                    incidentfault.Parameters.Add("@Area", System.Data.SqlDbType.NVarChar, 20)
                    incidentfault.Parameters("@Area").Value = FaultP.Area
                    incidentfault.Parameters.Add("@Energy", System.Data.SqlDbType.NVarChar, 10)
                    incidentfault.Parameters("@Energy").Value = FaultP.Energy
                    incidentfault.Parameters.Add("@GantryAngle", System.Data.SqlDbType.NVarChar, 3)
                    incidentfault.Parameters("@GantryAngle").Value = FaultP.GantryAngle
                    incidentfault.Parameters.Add("@CollimatorAngle", System.Data.SqlDbType.NVarChar, 3)
                    incidentfault.Parameters("@CollimatorAngle").Value = FaultP.CollimatorAngle
                    incidentfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                    incidentfault.Parameters("@Linac").Value = FaultP.Linac
                    incidentfault.Parameters.Add("@IncidentID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@IncidentID").Value = IncidentID
                    incidentfault.Parameters.Add("@BSUHID", System.Data.SqlDbType.VarChar, 7)
                    incidentfault.Parameters("@BSUHID").Value = FaultP.PatientID
                    incidentfault.Parameters.Add("@ConcessionNumber", System.Data.SqlDbType.NVarChar, 25)
                    incidentfault.Parameters("@ConcessionNumber").Value = FaultP.ConcessionNumber
                    incidentfault.Parameters.Add("@OriginalFaultID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@OriginalFaultID").Value = 0
                    incidentfault.Parameters.Add("@RadiationIncident", System.Data.SqlDbType.Bit)
                    incidentfault.Parameters("@RadiationIncident").Value = FaultP.RadioIncident
                    incidentfault.Parameters.Add("@Activity", System.Data.SqlDbType.NVarChar, 3)
                    incidentfault.Parameters("@Activity").Value = FaultP.Activity
                    incidentfault.Parameters.Add("@LastState", System.Data.SqlDbType.NVarChar, 25)
                    incidentfault.Parameters("@LastState").Value = FaultP.LastState
                    incidentfault.ExecuteNonQuery()

                    incidentfault.Parameters.Clear()

                    incidentfault.CommandText = "usp_NewFaultTracking"
                    incidentfault.CommandType = CommandType.StoredProcedure
                    'incidentfault.Parameters.Add("@Trackingcomment", System.Data.SqlDbType.NVarChar, 250)
                    'incidentfault.Parameters("@Trackingcomment").Value = TrackingComment
                    'incidentfault.Parameters.Add("@AssignedTo", Data.SqlDbType.NVarChar, 50)
                    'incidentfault.Parameters("@AssignedTo").Value = Assigned
                    incidentfault.Parameters.Add("@Status", Data.SqlDbType.NVarChar, 50)
                    incidentfault.Parameters("@Status").Value = "New"
                    incidentfault.Parameters.Add("@LastupdatedBy", System.Data.SqlDbType.NVarChar, 50)
                    incidentfault.Parameters("@LastupdatedBy").Value = FaultP.UserInfo
                    incidentfault.Parameters.Add("@Lastupdatedon", System.Data.SqlDbType.DateTime)
                    incidentfault.Parameters("@Lastupdatedon").Value = FaultP.DateInserted
                    incidentfault.Parameters.Add("@linacName", System.Data.SqlDbType.NVarChar, 10)
                    incidentfault.Parameters("@linacName").Value = FaultP.Linac
                    'incidentfault.Parameters.Add("@action", System.Data.SqlDbType.NVarChar, 250)
                    'incidentfault.Parameters("@action").Value = Action
                    incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                    incidentfault.Parameters("@incidentID").Value = IncidentID
                    incidentfault.ExecuteNonQuery()
                    incidentfault.Parameters.Clear()

                    Select Case State
                        Case "New"
                            'No further action except to commit transaction
                        Case "Concession"
                            incidentfault.CommandText = "usp_CreateNewConcession"
                            incidentfault.CommandType = CommandType.StoredProcedure
                            incidentfault.Parameters.Add("@ConcessionDescription", System.Data.SqlDbType.NVarChar, 25)
                            incidentfault.Parameters("@ConcessionDescription").Value = String.Empty
                            incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                            incidentfault.Parameters("@incidentID").Value = IncidentID
                            incidentfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                            incidentfault.Parameters("@Linac").Value = FaultP.Linac
                            incidentfault.Parameters.Add("@ConcessionActive", System.Data.SqlDbType.Bit)
                            incidentfault.Parameters("@ConcessionActive").Value = 1
                            incidentfault.Parameters.Add("@Action", System.Data.SqlDbType.NVarChar, 250)
                            incidentfault.Parameters("@Action").Value = FaultP.RadAct
                            incidentfault.Parameters.Add("@AssignedTo", System.Data.SqlDbType.NVarChar, 50)
                            incidentfault.Parameters("@AssignedTo").Value = "Unassigned"
                            incidentfault.Parameters.Add("@ReportedBy", System.Data.SqlDbType.NVarChar, 250)
                            incidentfault.Parameters("@ReportedBy").Value = FaultP.UserInfo
                            incidentfault.Parameters.Add("@Lastupdatedon", System.Data.SqlDbType.DateTime)
                            incidentfault.Parameters("@Lastupdatedon").Value = time
                            outPutParameter.ParameterName = "@TrackingNum"
                            outPutParameter.SqlDbType = System.Data.SqlDbType.Int
                            outPutParameter.Direction = System.Data.ParameterDirection.Output
                            incidentfault.Parameters.Add(outPutParameter)
                            incidentfault.ExecuteNonQuery()

                            incidentfault.Parameters.Clear()
                            incidentfault.CommandText = "usp_SetRadAckFault"
                            incidentfault.Parameters.Add("@IncidentID", Data.SqlDbType.Int)
                            incidentfault.Parameters("@IncidentID").Value = IncidentID
                            incidentfault.Parameters.Add("@TrackingID", System.Data.SqlDbType.Int)
                            incidentfault.Parameters("@TrackingID").Value = 0
                            incidentfault.Parameters.Add("@Acknowledge", Data.SqlDbType.Bit)
                            incidentfault.Parameters("@Acknowledge").Value = False
                            incidentfault.ExecuteNonQuery()
                            incidentfault.Parameters.Clear()
                        Case "Closed"
                            incidentfault.CommandText = "usp_NewFaultTracking"
                            incidentfault.CommandType = CommandType.StoredProcedure
                            incidentfault.Parameters.Add("@Status", Data.SqlDbType.NVarChar, 50)
                            incidentfault.Parameters("@Status").Value = "Closed"
                            incidentfault.Parameters.Add("@LastupdatedBy", System.Data.SqlDbType.NVarChar, 50)
                            incidentfault.Parameters("@LastupdatedBy").Value = FaultP.UserInfo
                            incidentfault.Parameters.Add("@Lastupdatedon", System.Data.SqlDbType.DateTime)
                            incidentfault.Parameters("@Lastupdatedon").Value = time
                            incidentfault.Parameters.Add("@linacName", System.Data.SqlDbType.NVarChar, 10)
                            incidentfault.Parameters("@linacName").Value = FaultP.Linac
                            incidentfault.Parameters.Add("@action", System.Data.SqlDbType.NVarChar, 250)
                            incidentfault.Parameters("@action").Value = FaultP.RadAct
                            incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                            incidentfault.Parameters("@incidentID").Value = IncidentID
                            incidentfault.ExecuteNonQuery()
                            incidentfault.Parameters.Clear()

                    End Select

                    incidentfault.Parameters.Clear()
                    ObjTransaction.Commit()
                    Result = True
                Catch ex As Exception

                    ObjTransaction.Rollback()
                    LogError(ex)
                Finally
                    conn.Close()
                End Try

            End Using
            Return Result
        End Function

        Public Shared Function InsertMajorFault(FaultP As DavesCode.FaultParameters, ByVal connectionString As String) As Integer
            Const STATE = "New"
            Const ORIGINALFAULTID = 0

            Dim time As DateTime = Now()
            Dim IncidentID As Integer = 0
            Dim LastFault As Integer = 0
            Dim conn As SqlConnection
            'Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
            conn = New SqlConnection(connectionString)
            'Dim incidentfault As SqlCommand
            'Const CONCESSIONNUMBER = ""
            Dim logInStatusID As Integer = 0

            Dim constateid As SqlCommand
            constateid = New SqlCommand("SELECT stateid FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

            constateid.Parameters.AddWithValue("@linac", FaultP.Linac)
            Dim readers As SqlDataReader
            conn.Open()
            readers = constateid.ExecuteReader()

            If readers.Read() Then
                logInStatusID = readers.Item("stateid")

            End If
            readers.Close()
            conn.Close()

            Using (conn)

                Dim incidentfault As New SqlCommand With {
                    .Connection = conn
                }

                'Dim ObjTransaction As SqlTransaction
                'ObjTransaction = Nothing

                'Try
                conn.Open()
                'ObjTransaction = conn.BeginTransaction()
                incidentfault.CommandText = "usp_InsertNewFault"
                incidentfault.CommandType = CommandType.StoredProcedure
                'incidentfault.Transaction = ObjTransaction

                incidentfault.Parameters.Add("@DateInserted", System.Data.SqlDbType.DateTime)
                incidentfault.Parameters("@DateInserted").Value = FaultP.DateInserted
                incidentfault.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, 20)
                incidentfault.Parameters("@Status").Value = STATE
                incidentfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                incidentfault.Parameters("@Linac").Value = FaultP.Linac
                incidentfault.Parameters.Add("@StateID", System.Data.SqlDbType.Int)
                incidentfault.Parameters("@StateID").Value = logInStatusID

                Dim outPutParameter = New SqlParameter With {
                        .ParameterName = "@IncidentID",
                        .SqlDbType = System.Data.SqlDbType.Int,
                        .Direction = System.Data.ParameterDirection.Output
                    }
                incidentfault.Parameters.Add(outPutParameter)

                incidentfault.ExecuteNonQuery()
                Dim NewIncident As String = outPutParameter.Value.ToString
                IncidentID = CInt(NewIncident)

                incidentfault.Parameters.Clear()
                outPutParameter.ParameterName = Nothing

                incidentfault.CommandText = "usp_ReportFault"
                incidentfault.CommandType = CommandType.StoredProcedure
                incidentfault.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar, 250)
                incidentfault.Parameters("@Description").Value = FaultP.FaultDescription
                incidentfault.Parameters.Add("@ReportedBy", System.Data.SqlDbType.NVarChar, 50)
                incidentfault.Parameters("@ReportedBy").Value = FaultP.UserInfo
                incidentfault.Parameters.Add("@DateReported", System.Data.SqlDbType.DateTime)
                incidentfault.Parameters("@DateReported").Value = FaultP.DateInserted
                incidentfault.Parameters.Add("@Area", System.Data.SqlDbType.NVarChar, 20)
                incidentfault.Parameters("@Area").Value = FaultP.Area
                incidentfault.Parameters.Add("@Energy", System.Data.SqlDbType.NVarChar, 10)
                incidentfault.Parameters("@Energy").Value = FaultP.Energy
                incidentfault.Parameters.Add("@GantryAngle", System.Data.SqlDbType.NVarChar, 3)
                incidentfault.Parameters("@GantryAngle").Value = FaultP.GantryAngle
                incidentfault.Parameters.Add("@CollimatorAngle", System.Data.SqlDbType.NVarChar, 3)
                incidentfault.Parameters("@CollimatorAngle").Value = FaultP.CollimatorAngle
                incidentfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                incidentfault.Parameters("@Linac").Value = FaultP.Linac
                incidentfault.Parameters.Add("@IncidentID", System.Data.SqlDbType.Int)
                incidentfault.Parameters("@IncidentID").Value = IncidentID
                incidentfault.Parameters.Add("@BSUHID", System.Data.SqlDbType.VarChar, 7)
                incidentfault.Parameters("@BSUHID").Value = FaultP.PatientID
                incidentfault.Parameters.Add("@ConcessionNumber", System.Data.SqlDbType.NVarChar, 25)
                incidentfault.Parameters("@ConcessionNumber").Value = FaultP.ConcessionNumber
                incidentfault.Parameters.Add("@OriginalFaultID", System.Data.SqlDbType.Int)
                incidentfault.Parameters("@OriginalFaultID").Value = ORIGINALFAULTID
                incidentfault.Parameters.Add("@RadiationIncident", System.Data.SqlDbType.Bit)
                incidentfault.Parameters("@RadiationIncident").Value = FaultP.RadioIncident
                incidentfault.Parameters.Add("@Activity", System.Data.SqlDbType.NVarChar, 3)
                incidentfault.Parameters("@Activity").Value = FaultP.Activity
                incidentfault.Parameters.Add("@LastState", System.Data.SqlDbType.NVarChar, 25)
                incidentfault.Parameters("@LastState").Value = FaultP.LastState
                incidentfault.ExecuteNonQuery()

                incidentfault.Parameters.Clear()

                incidentfault.CommandText = "usp_NewFaultTracking"
                incidentfault.CommandType = CommandType.StoredProcedure
                'incidentfault.Parameters.Add("@Trackingcomment", System.Data.SqlDbType.NVarChar, 250)
                'incidentfault.Parameters("@Trackingcomment").Value = TrackingComment
                'incidentfault.Parameters.Add("@AssignedTo", Data.SqlDbType.NVarChar, 50)
                'incidentfault.Parameters("@AssignedTo").Value = Assigned
                incidentfault.Parameters.Add("@Status", Data.SqlDbType.NVarChar, 50)
                incidentfault.Parameters("@Status").Value = STATE
                incidentfault.Parameters.Add("@LastupdatedBy", System.Data.SqlDbType.NVarChar, 50)
                incidentfault.Parameters("@LastupdatedBy").Value = FaultP.UserInfo
                incidentfault.Parameters.Add("@Lastupdatedon", System.Data.SqlDbType.DateTime)
                incidentfault.Parameters("@Lastupdatedon").Value = FaultP.DateInserted
                incidentfault.Parameters.Add("@linacName", System.Data.SqlDbType.NVarChar, 10)
                incidentfault.Parameters("@linacName").Value = FaultP.Linac
                'incidentfault.Parameters.Add("@action", System.Data.SqlDbType.NVarChar, 250)
                'incidentfault.Parameters("@action").Value = Action
                incidentfault.Parameters.Add("@incidentID", System.Data.SqlDbType.Int)
                incidentfault.Parameters("@incidentID").Value = IncidentID
                incidentfault.ExecuteNonQuery()
                incidentfault.Parameters.Clear()



                incidentfault.Parameters.Clear()
                '    ObjTransaction.Commit()

                'Catch ex As Exception

                '    ObjTransaction.Rollback()
                '    LogError(ex)
                'Finally
                '    conn.Close()
                'End Try

            End Using
            Return IncidentID
        End Function

        Public Shared Function ReturnNewIncidentID(ByVal Linacname As String) As String
            Dim IncidentID As Integer = 0
            'Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            '"connectionstring").ConnectionString
            'Dim sql As String = "select IncidentID from ReportFault where incidentID in (select IncidentID from FaultIDTable where linac=@linac and Status in ('New', 'Open')) order by IncidentID desc"
            'Using conn As New SqlConnection(connectionString)
            '    Dim cmd As New SqlCommand(sql, conn)
            '    cmd.Parameters.Add("@Linac", SqlDbType.VarChar)
            '    cmd.Parameters("@Linac").Value = Linacname
            '    Try
            '        conn.Open()
            '        IncidentID = Convert.ToInt64(cmd.ExecuteScalar())
            '    Catch ex As Exception
            '        LogError(ex)
            '    End Try
            'End Using

            'Return IncidentID

            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            Dim existingfault As SqlCommand
            Dim LinacStatusID As String = ""
            Dim reader As SqlDataReader
            conn = New SqlConnection(connectionString)

            existingfault = New SqlCommand("SELECT TOP(1) [IncidentID] FROM [FaultIDTable] where Linac = @linac and ReportClosed is Null and statusid is not NULL ORDER BY [IncidentID] DESC", conn)
            existingfault.Parameters.AddWithValue("@linac", Linacname)
            conn.Open()
            reader = existingfault.ExecuteReader()
            If reader.HasRows() Then
                'Have to now actually read the rows
                reader.Read()
                IncidentID = reader.Item("IncidentID").ToString()

            End If

            Return IncidentID

        End Function

        Public Shared Function CheckForOpenFault(ByVal machinename As String) As Boolean
            Dim openfault As Boolean = False
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
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

            Return openfault
        End Function

        Public Shared Function ReturnFaultActivity(ByVal machinename As String) As String
            Dim faultActivity As String = String.Empty
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            Dim existingfault As SqlCommand
            Dim reader As SqlDataReader
            conn = New SqlConnection(connectionString)

            existingfault = New SqlCommand("SELECT TOP(1)[FaultID], [Activity] FROM [ReportFault] where Linac = @linac ORDER BY [FaultID] DESC", conn)
            existingfault.Parameters.AddWithValue("@linac", machinename)
            conn.Open()
            reader = existingfault.ExecuteReader()
            If reader.HasRows() Then
                'Have to now actually read the rows
                reader.Read()
                faultActivity = reader.Item("Activity").ToString()
            End If

            Return faultActivity
        End Function
        Public Shared Sub UpdateLastNonFaultState(ByVal Linac As String)
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            Dim conn As SqlConnection
            Dim Faultid As String = String.Empty
            Dim existingfault As SqlCommand
            Dim LastState As String = "Linac Unauthorised"
            Dim reader As SqlDataReader
            conn = New SqlConnection(connectionString)
            existingfault = New SqlCommand("SELECT TOP(1) [FaultID] FROM [ReportFault] where Linac = @Linac ORDER BY [FaultID] DESC", conn)
            existingfault.Parameters.AddWithValue("@Linac", Linac)
            conn.Open()
            reader = existingfault.ExecuteReader()
            If reader.HasRows() Then
                'Have to now actually read the rows
                reader.Read()
                Faultid = reader.Item("FaultID").ToString()
            End If
            conn.Close()
            conn.Open()
            existingfault = New SqlCommand("Update [ReportFault] SET LastState=@LastState WHERE FaultID=@FaultID", conn)
            existingfault.Parameters.AddWithValue("@LastState", LastState)
            existingfault.Parameters.AddWithValue("@FaultID", Faultid)

            existingfault.ExecuteNonQuery()

            conn.Close()
        End Sub
        Public Shared Function ReturnLastNonFaultState(ByVal incidentid As String) As String
            Dim LastNonFaultState As String = String.Empty
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            Dim existingfault As SqlCommand
            Dim reader As SqlDataReader
            conn = New SqlConnection(connectionString)

            existingfault = New SqlCommand("SELECT [LastState] FROM [ReportFault] where incidentid = @incidentid", conn)
            existingfault.Parameters.AddWithValue("@incidentID", incidentid)
            conn.Open()
            reader = existingfault.ExecuteReader()
            If reader.HasRows() Then
                'Have to now actually read the rows
                reader.Read()
                LastNonFaultState = reader.Item("LastState").ToString()
            End If
            If LastNonFaultState = "Clinical" Then
                LastNonFaultState = "Suspended"
            End If
            Return LastNonFaultState
        End Function

        Public Shared Function RecoverLastNonFaultState(ByVal Linac As String) As String
            Dim LastNonFaultState As String = String.Empty
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings(
            "connectionstring").ConnectionString
            Dim existingfault As SqlCommand
            Dim reader As SqlDataReader
            conn = New SqlConnection(connectionString)

            existingfault = New SqlCommand("SELECT TOP(1) [LastState] FROM [ReportFault] where Linac = @Linac ORDER BY [FaultID] DESC", conn)
            existingfault.Parameters.AddWithValue("@Linac", Linac)
            conn.Open()
            reader = existingfault.ExecuteReader()
            If reader.HasRows() Then
                'Have to now actually read the rows
                reader.Read()
                LastNonFaultState = reader.Item("LastState").ToString()
            End If
            If LastNonFaultState = "Clinical" Then
                LastNonFaultState = "Suspended"
            End If
            Return LastNonFaultState
        End Function
        Public Shared Sub LogError(ex As Exception)
            Dim message As String = String.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"))
            message += Environment.NewLine
            message += "-----------------------------------------------------------"
            message += Environment.NewLine
            message += String.Format("Message: {0}", ex.Message)
            message += Environment.NewLine
            message += String.Format("StackTrace: {0}", ex.StackTrace)
            message += Environment.NewLine
            message += String.Format("Source: {0}", ex.Source)
            message += Environment.NewLine
            message += String.Format("TargetSite: {0}", ex.TargetSite.ToString())
            message += Environment.NewLine
            message += "-----------------------------------------------------------"
            message += Environment.NewLine
            'Dim path As String = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog/ErrorLog.txt")
            Dim path As String = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog/")
            If (Not Directory.Exists(path)) Then
                Directory.CreateDirectory(path)
            End If
            Dim shortfilename As String = DateTime.Today.ToString("dd-MM-yy") + ".txt"
            path = path + shortfilename ' Text File Name
            If (Not File.Exists(path)) Then
                File.Create(path).Dispose()
            End If
            Using writer As New StreamWriter(path, True)
                writer.WriteLine(message)
                writer.Close()
            End Using
            'Send email
            'Dim smtpClient As SmtpClient = New SmtpClient()
            'Dim emessage As MailMessage = New MailMessage()

            'Dim fromAddress As New MailAddress("VISIRSERVER@VISIRSERVER.bsuh.nhs.uk", "ELF")
            'Dim toAddress As New MailAddress("david.spendley@bsuh.nhs.uk")
            'emessage.From = fromAddress
            'emessage.To.Add(toAddress)
            'emessage.Subject = "ELF system error"
            'emessage.Body = "Error file name: " + shortfilename
            'smtpClient.Host = "10.216.8.19"
            'smtpClient.Send(emessage)

        End Sub

        Public Shared Sub LogError(ByVal ex As Exception, ByVal texbox As String)
            Dim message As String = String.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"))
            message += Environment.NewLine
            message += "-----------------------------------------------------------"
            message += Environment.NewLine
            message += String.Format("Message: {0}", ex.Message)
            message += Environment.NewLine
            message += String.Format("TextBox: {0}", texbox)
            message += Environment.NewLine
            message += String.Format("StackTrace: {0}", ex.StackTrace)
            message += Environment.NewLine
            message += String.Format("Source: {0}", ex.Source)
            message += Environment.NewLine
            message += String.Format("TargetSite: {0}", ex.TargetSite.ToString())
            message += Environment.NewLine
            message += "-----------------------------------------------------------"
            message += Environment.NewLine
            'Dim path As String = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog/ErrorLog.txt")
            Dim path As String = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog/")
            If (Not Directory.Exists(path)) Then
                Directory.CreateDirectory(path)
            End If
            Dim shortfilename As String = DateTime.Today.ToString("dd-MM-yy") + ".txt"
            path = path + shortfilename ' Text File Name
            If (Not File.Exists(path)) Then
                File.Create(path).Dispose()
            End If
            Using writer As New StreamWriter(path, True)
                writer.WriteLine(message)
                writer.Close()
            End Using

        End Sub

        Public Shared Sub LogAnomaly(ByVal LinacName As String, ByVal Procedure As String, ByVal Anomaly As String)
            Dim message As String = String.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"))
            message += Environment.NewLine
            message += "-----------------------------------------------------------"
            message += Environment.NewLine
            message += String.Format("Linac: {0}", LinacName)
            message += Environment.NewLine
            message += String.Format("Calling Procedure: {0}", Procedure)
            message += Environment.NewLine
            message += String.Format("Anomaly: {0}", Anomaly)
            message += Environment.NewLine
            message += String.Format("Anomaly occurred on page: {0}", System.Web.HttpContext.Current.Request.Url.ToString)
            message += Environment.NewLine
            message += "-----------------------------------------------------------"
            message += Environment.NewLine
            'Dim path As String = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog/ErrorLog.txt")
            Dim path As String = System.Web.HttpContext.Current.Server.MapPath("~/ErrorLog/")
            If (Not Directory.Exists(path)) Then
                Directory.CreateDirectory(path)
            End If
            path = path + DateTime.Today.ToString("dd-MM-yy") + ".txt" ' Text File Name
            If (Not File.Exists(path)) Then
                File.Create(path).Dispose()
            End If
            Using writer As New StreamWriter(path, True)
                writer.WriteLine(message)
                writer.Close()
            End Using
        End Sub

    End Class

End Namespace
