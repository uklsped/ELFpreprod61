Imports System.Data
Imports System.Data.SqlClient

Namespace DavesCode

    Public Class EngRunup
        Private _Data As DataTable
#Region "DataTable encapsulation"
        Public ReadOnly Property Data() As IList
            Get
                Return _Data.DefaultView
            End Get
        End Property

        Sub New()
            _Data = New DataTable("EngUp")
        End Sub

        Public Function NewRunup() As DataRow
            Return _Data.NewRow()
        End Function

        Public Sub Update(ByVal adapter As SqlDataAdapter)
            adapter.Update(_Data)
        End Sub
#End Region

#Region "Business Logic"
        'Public Sub AddEngRunup(ByVal comment As String, ByVal LogOutName As String, ByVal LogOutDate As DateTime, ByVal Machinename As String, ByVal LogInDate As String
        '    Dim NewEngRunup As DataRow
        '    NewEngRunup = Me.NewRunup()
        'NewEngRunup
        'comm.Parameters.Add("@Comment", System.Data.SqlDbType.NVarChar, 250)
        'comm.Parameters("@Comment").Value = TextBox
        'comm.Parameters.Add("@LogOutName", System.Data.SqlDbType.NVarChar, 10)
        'comm.Parameters("@LogOutName").Value = commitusername 'This will have to find real user name
        'comm.Parameters.Add("@LogOutDate", System.Data.SqlDbType.DateTime)
        'comm.Parameters("@LogOutDate").Value = time
        'comm.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar, 10)
        'comm.Parameters("@linac").Value = machinename
        'comm.Parameters.Add("@LogInDate", System.Data.SqlDbType.DateTime)
        'comm.Parameters("@LogInDate").Value = StartTime
        'comm.Parameters.Add("@Duration", System.Data.SqlDbType.Decimal)
        'comm.Parameters("@Duration").Value = minutesDuration
        'comm.Parameters.Add("@LogInStatusID", System.Data.SqlDbType.NVarChar, 10)
        'comm.Parameters("@LogInStatusID").Value = LogOnStateID
        'comm.Parameters.Add("@LogOutStatusID", System.Data.SqlDbType.NVarChar, 10)
        'comm.Parameters("@LogOutStatusID").Value = LogOffStateID
        'comm.Parameters.Add("@Approved", System.Data.SqlDbType.Bit)
        'comm.Parameters("@Approved").Value = Approved
        'comm.Parameters.Add("LogInName", SqlDbType.NVarChar, 50)
        'comm.Parameters("LogInName").Value = LoginName

        'End Sub
#End Region

    End Class

End Namespace
