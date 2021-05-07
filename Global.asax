<%@ Application Language="VB" %>

<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        'Application("LA1loaded") = 0
        'Application("LA2loaded") = 0
        'Application("LA3loaded") = 0
        'Application("LA4loaded") = 0
        '' AppConfig.Init()
        'Dim sf As BaseBallTeam = New BaseBallTeam("AppTrial", 9)
        'Application("LA9") = sf
        'Dim E1 As LinacState = New LinacState()
        'Application("E1") = E1
        ' Code that runs on application startup
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
        'Application("LA1loaded") = 0
        'Application("LA2loaded") = 0
        'Application("LA3loaded") = 0
        'Application("LA4loaded") = 0
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        Try
            ' set default values
            Session("ErrorMsg") =
            "No error information was available"
            Session("ExceptionType") = String.Empty
            Session("PageErrorOccured") =
              Request.CurrentExecutionFilePath
            Session("StackTrace") = String.Empty

            ' 2. Catch the last exception thrown
            Dim lastError As Exception = Server.GetLastError()

            ' 3. Pull out the InnerException
            If (Not lastError Is Nothing) Then
                lastError = lastError.InnerException
                ' 4. Place a few key values in the session for 
                ' retrieval on the custom error page
                Session("ErrorMsg") = lastError.Message
                Session("ExceptionType") = lastError.GetType.ToString
                Session("StackTrace") = lastError.StackTrace

            End If
            ' 5. Clear the error from the server
            Server.ClearError()
            ' 6. Redirect to the custom error page
            Response.Redirect("~/controls/ErrorPage.aspx")
        Catch ex As Exception
            ' if we end up here, 
            'error handling has thrown an error.
            ' do nothing - don't want to create an infinite loop
            Response.Write("We apologize, but an" &
        " unrecoverable error has occurred. Please click the " &
        " back button on your browser and try again.")
        End Try

    End Sub


    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub

</script>