Partial Public Class controls_UnLockElfuc
    Inherits System.Web.UI.UserControl

    Public Event HideUnlockPopUp(ByVal Hide As Boolean)
    Public Property UserReason() As Integer
    Public Property LinacName() As String
    Dim usergroupselected As Integer = Nothing

    Public ReadOnly Property username() As String
        Get
            username = txtchkUserName.Text.Trim()
        End Get

    End Property
    Public ReadOnly Property userpassword() As String
        Get
            userpassword = txtchkPWD.Text.Trim()
        End Get
    End Property


    Protected Sub UnlockElf_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UnlockElf.Click

        Dim strScript As String = "<script>"
        Dim textboxUser As TextBox = FindControl("txtchkUserName") 'This gets username textbox to pass to login
        Dim passwordUser As TextBox = FindControl("txtchkPWD")  'This gets password textbox to pass to login
        Dim logerrorbox As Label = FindControl("LoginErrordetails") 'This gets error label to pass to login

        'We need to determine if the user is authenticated
        'Get the values entered by the user
        Dim loginUsername As String = username
        Dim loginPassword As String = userpassword
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString

        usergroupselected = DavesCode.Reuse.SuccessfulLogin(loginUsername, loginPassword, UserReason, textboxUser, passwordUser, logerrorbox)
        If usergroupselected <> Nothing Then
            '    'what happens if machinestate fails?
            resetLogInscreen()
            DavesCode.Reuse.MachineStateNew(loginUsername, usergroupselected, LinacName, UserReason, True, True, connectionString)

            '    'eg from http://dotnetbites.wordpress.com/2014/02/15/call-parent-page-method-from-user-control-using-reflection/
            Me.Page.GetType.InvokeMember("UpdateUserDisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {usergroupselected})
            'Me.Page.GetType.InvokeMember("UpdateButtons", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {})
            RaiseEvent HideUnlockPopUp(True)
        Else
            RaiseEvent HideUnlockPopUp(False)

        End If

    End Sub

    Protected Sub page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim Accept As Button = FindControl("UnlockElf")

        If Not FindControl("UnlockElf") Is Nothing Then
            Accept.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Accept, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If

    End Sub

    Protected Sub resetLogInscreen()
        Dim textboxUser As TextBox = FindControl("txtchkUserName")
        Dim passwordUser As TextBox = FindControl("txtchkPWD")
        Dim logerrorbox As Label = FindControl("LoginErrordetails")
        textboxUser.Text = Nothing
        passwordUser.Text = Nothing
        logerrorbox.Text = Nothing

    End Sub

End Class
