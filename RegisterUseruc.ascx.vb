Imports System.Net.Mail
Imports System.Web.Security
Partial Class RegisterUseruc
    Inherits System.Web.UI.UserControl

    Public Event ReloadTab()
    'Public Property UpdateMode() As UpdatePanelUpdateMode
    '    Get
    '        Return Me.UpdatePanel1.UpdateMode
    '    End Get
    '    Set(ByVal value As UpdatePanelUpdateMode)
    '        Me.UpdatePanel1.UpdateMode = value
    '    End Set
    'End Property

    'Public Sub Update()
    '    Me.UpdatePanel1.Update()
    'End Sub
    Public Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Dim myControl1 As Control = FindControl("TextBox2")
        'If (Not myControl1 Is Nothing) Then
        '    ' Get control's parent. 
        '    Dim myControl2 As Control = myControl1.Parent
        '    Response.Write("Parent of the text box is : " & myControl2.ID)
        'Else
        '    Response.Write("Control not found.....")
        'End If



        'Dim UserNameTextBox As TextBox
        'UserNameTextBox = ChangePwd.ChangePasswordTemplateContainer.FindControl("UserName")
        'UserNameTextBox.Text = Nothing

        'next if was there for delete user demo
        'If Not IsPostBack Then
        '    DropDownListDataBind()

        'End If
        PasswordRecovery1.UserName = String.Empty
        ChangePwd.UserName = String.Empty
        WaitButtons()
        'Dim machinename As String
        'from http://blog.davidsz.nl/2012/03/04/previous-page-in-asp-net/
        'Cancel.Attributes.Add("onClick", "javascript:history.back();   return false;")
        'leave  cancel_click empty if you use this
        'If Not IsPostBack Then
        'This next is if register opens as new page, but at the moment using
        'http://stackoverflow.com/questions/17582081/how-to-open-aspx-web-pages-on-a-pop-up-window


        'ViewState("RefUrl") = Request.UrlReferrer.ToString()
        'Dim viewrefurl As String = ViewState("RefUrl")
        'machinename = Request.QueryString("val")
        'HiddenField1.Value = Request.QueryString("Tabindex")

        'End If
        'End If




    End Sub
 
    Protected Sub CreateUserWizard1_CreatedUser(ByVal sender As Object, ByVal e As System.EventArgs) Handles CreateUserWizard1.CreatedUser
        'Protected Sub CreateUser_click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim selectedrole As DropDownList = CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserRole")
        Roles.AddUserToRole(CreateUserWizard1.UserName, selectedrole.SelectedItem.Value)
        'this is from http://www.4guysfromrolla.com/articles/070506-1.aspx
        Dim UserNameTextBox As TextBox
        UserNameTextBox = CreateUserWizardStep1.ContentTemplateContainer.FindControl("UserName")
        Dim DataSource As SqlDataSource
        DataSource = CreateUserWizardStep1.ContentTemplateContainer.FindControl("InsertExtraInfo")
        Dim User As MembershipUser
        User = Membership.GetUser(UserNameTextBox.Text)
        Dim UserGuid As Object
        UserGuid = User.ProviderUserKey()
        Dim guid As String = UserGuid.ToString()

        DataSource.InsertParameters.Add("UserID", UserGuid.ToString())
        DataSource.Insert()

        Dim UserEmailtextbox As TextBox = CreateUserWizardStep1.ContentTemplateContainer.FindControl("Email")
        Dim UserEmail As String
        UserEmail = UserEmailtextbox.Text.ToString
        Dim smtpClient As SmtpClient = New SmtpClient()
        Dim message As MailMessage = New MailMessage()
        Try
            Dim fromAddress As New MailAddress("VISIRSERVER@VISIRSERVER.bsuh.nhs.uk", "ELF")
            Dim toAddress As New MailAddress("david.spendley@nhs.net")
            message.From = fromAddress
            message.To.Add(toAddress)
            message.Subject = "ELF registration"
            message.Body = "Someone has registered for ELF"
            smtpClient.Host = "10.216.10.47"
            smtpClient.Send(message)

            Dim regAddress As New MailAddress(UserEmail)
            message.From = fromAddress
            message.To.Add(regAddress)
            message.Subject = "ELF registration"
            message.Body = "Thank you for registering with ELF. You will be notified when your account has been approved"
            smtpClient.Host = "10.216.10.47"
            smtpClient.Send(message)

        Catch ex As Exception
            DavesCode.NewFaultHandling.LogError(ex)
            'should be an error action here
        End Try
    End Sub

    'Protected Sub ExitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ExitButton.Click
    '    'This if opening as full page not as popup page

    '    'Dim tab As String
    '    'tab = HiddenField1.Value
    '    'Response.Redirect("LA1page.aspx?pageref=RegisterUser&Tabindex=" & tab)

    '    ClientScript.RegisterClientScriptBlock(Page.GetType(), "script", "window.close();", True)
    'End Sub

    Protected Sub continue_click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Multiviewlist As MultiView

        If Not Me.Parent.FindControl("Multiview1") Is Nothing Then
            Multiviewlist = Me.Parent.FindControl("Multiview1")
            Multiviewlist.ActiveViewIndex = 0
        End If
        RaiseEvent ReloadTab()
        'ClientScript.RegisterClientScriptBlock(Page.GetType(), "script", "window.close();", True)
    End Sub

    '  protected void Page_Load(object sender, System.EventArgs e) {  
    '    if(!Page.IsPostBack){  
    '        DropDownListDataBind();  
    '    }  
    '}  

    'Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Membership.DeleteUser(DropDownList1.SelectedItem.Text, True)
    '    Label1.Text = "User deleted successfully!"
    '    DropDownListDataBind()
    'End Sub
    Protected Sub ClearPage(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button4.Click
        PasswordRecovery1.UserName = String.Empty
        ChangePwd.UserName = String.Empty
        RaiseEvent ReloadTab()
        'Dim Multiviewlist As MultiView

        'If Not Me.Parent.FindControl("Multiview1") Is Nothing Then
        '    Multiviewlist = Me.Parent.FindControl("Multiview1")
        '    Multiviewlist.ActiveViewIndex = 0
        'End If

    End Sub

    'Protected Sub DropDownListDataBind()
    '    DropDownList1.DataSource = Membership.GetAllUsers()
    '    DropDownList1.DataBind()
    'End Sub

    Protected Sub OnCreateUserError(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.CreateUserErrorEventArgs)

        Select Case (e.CreateUserError)

            Case MembershipCreateStatus.DuplicateUserName
                Label1.Text = "Username already exists. Please enter a different user name."


            Case MembershipCreateStatus.DuplicateEmail
                Label1.Text = "A username for that e-mail address already exists. Please enter a different e-mail address."


            Case MembershipCreateStatus.InvalidPassword
                Label1.Text = "The password provided is invalid. Please enter a valid password value."


            Case MembershipCreateStatus.InvalidEmail
                Label1.Text = "The e-mail address provided is invalid. Please check the value and try again."


            Case MembershipCreateStatus.InvalidAnswer
                Label1.Text = "The password retrieval answer provided is invalid. Please check the value and try again."


            Case MembershipCreateStatus.InvalidQuestion
                Label1.Text = "The password retrieval question provided is invalid. Please check the value and try again."


            Case MembershipCreateStatus.InvalidUserName
                Label1.Text = "The user name provided is invalid. Please check the value and try again."


            Case MembershipCreateStatus.ProviderError
                Label1.Text = "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator."


            Case MembershipCreateStatus.UserRejected
                Label1.Text = "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator."

            Case Else
                Label1.Text = "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator."

        End Select

    End Sub

    Protected Sub WaitButtons()
        Dim Create As Button = CreateUserWizardStep1.CustomNavigationTemplateContainer.FindControl("StepNextButton")
        If Not Create Is Nothing Then
            Create.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Create, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
    End Sub

End Class
