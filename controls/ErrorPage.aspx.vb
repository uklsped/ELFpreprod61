
Imports System.IO

Partial Class controls_ErrorPage
    Inherits System.Web.UI.Page
    Dim errorMsg As String
    Dim pageErrorOccured As String
    Dim exceptionType As String
    Dim stackTrace As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            If (Not IsPostBack) Then

                ' 2. Grab the exception information from the session
                errorMsg = CStr(Session("ErrorMsg"))
                pageErrorOccured = CStr(Session("PageErrorOccured"))
                exceptionType = CStr(Session("ExceptionType"))
                stackTrace = CStr(Session("StackTrace"))
                LogError()

                ' 3. Clear the values from the session
                Session("ErrorMsg") = Nothing
                Session("PageErrorOccured") = Nothing
                Session("ExceptionType") = Nothing
                Session("StackTrace") = Nothing

                ' 4. Display a generic error message to the user
                lblMessage.Text = "We're sorry, but an unhandled " &
                "error has occurred.<br/><br/>"

                lblMessage.Text =
                String.Format("{0}To try again, " &
                "click <a href='{1}' class='linkgreen'> here</a>.<br/><br/>",
                lblMessage.Text, pageErrorOccured)

                ' 5. Add specific error information as 
                'HTML comments for you
                ' to view during development. 
                'You could also log the error to 
                ' the Windows event log here.
                lblMessage.Text = lblMessage.Text & "<!--" & Chr(10) &
                "Error Message: " & errorMsg & Chr(10) &
                "Page Error Occurred: " & pageErrorOccured & Chr(10) &
                "ExceptionType: " & exceptionType & Chr(10) &
                "Stack Trace: " & stackTrace & Chr(10) &
                "-->"

            End If
        Catch ex As Exception
            ' If an exception is thrown in the 
            ' above code output the message
            ' and stack trace to the screen
            lblMessage.Text = ex.Message & " " & ex.StackTrace
        End Try
    End Sub
    Protected Sub LogError()
        Dim message As String = String.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"))
        message += Environment.NewLine
        message += "-----------------------------------------------------------"
        message += Environment.NewLine
        message += String.Format("Message: {0}", errorMsg)
        message += Environment.NewLine
        message += String.Format("StackTrace: {0}", stackTrace)
        message += Environment.NewLine
        message += String.Format("Source: {0}", pageErrorOccured)
        message += Environment.NewLine
        message += String.Format("Exception Type: {0}", exceptionType)
        message += Environment.NewLine
        message += "-----------------------------------------------------------"
        message += Environment.NewLine

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
End Class
