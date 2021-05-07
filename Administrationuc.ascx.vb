Imports System.Net.Mail
Imports System.Data.SqlClient

Partial Class Administrationuc
    Inherits System.Web.UI.UserControl
    Private MachineName As String
    Private mpContentPlaceHolder As ContentPlaceHolder
    Private wctrl As WriteDatauc
    Private Operated As Boolean
    Private actionstate As String
    Private hiddenfield As HiddenField
    Private hiddenvalue As Boolean
    Public Property LinacName() As String
        Get
            Return MachineName
        End Get
        Set(ByVal value As String)
            MachineName = value
        End Set
    End Property
    ''' <summary>
    ''' This is a sample documentation bit
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim PlaceHolder As PlaceHolder
        PlaceHolder = CType(FindControl("Placeholder2"), PlaceHolder)
        If Not PlaceHolder Is Nothing Then
            wctrl = CType(PlaceHolder2.FindControl("Writedatauc1"), WriteDatauc)
            AddHandler wctrl.UserApproved, AddressOf UserApprovedEvent

        End If




    End Sub
    Protected Sub UserApprovedEvent(ByVal Tabset As String, ByVal Userinfo As String)

        Dim tabcontrol As String = Tabset
        Dim Action As String = Application(actionstate)
        If tabcontrol = "Admin" Then


            wctrl = CType(PlaceHolder2.FindControl("Writedatauc1"), WriteDatauc)
            wctrl.Visible = False
            If Action = "Confirm" Then
                MultiView1.SetActiveView(LoggedIn)
            Else
                'close the page because the user has cancelled
                If Not Me.Parent.FindControl("HiddenField1") Is Nothing Then
                    hiddenfield = Me.Parent.FindControl("HiddenField1")
                    hiddenfield.Value = False
                End If
                'Response.Redirect(Request.Url.ToString(), True)
                'Dim returnstring As String
                'returnstring = MachineName + "page.aspx"
                'Response.Redirect(returnstring)
                ' last result from http://forums.asp.net/t/1248126.aspx?How+to+close+browser+window+from+codebehind+
                'ScriptManager.RegisterClientScriptBlock(Page, GetType(Page), "close", "window.close();", True)
                'ClientScript.RegisterClientScriptBlock(Page.GetType(), "script", "window.close();", True)

            End If
        End If
        'End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Dim objCon As UserControl = Page.LoadControl("CreateUseruc.ascx")
        'CType(objCon, CreateUseruc).LinacName = MachineName
        'PlaceHolder1.Controls.Add(objCon)
        actionstate = "ActionState" + MachineName
        wctrl = CType(PlaceHolder2.FindControl("Writedatauc1"), WriteDatauc)
        wctrl.LinacName = MachineName
        If Not Me.Parent.FindControl("HiddenField1") Is Nothing Then
            hiddenfield = Me.Parent.FindControl("HiddenField1")
            hiddenvalue = hiddenfield.Value
        End If
        RoleName.Text = String.Empty

        If Not IsPostBack Then
            'MultiView1.SetActiveView(ShowNowt)
            'BindUserGrid()
            'DisplayRolesInGrid()
            '
            'Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
            'wcbutton.Text = "Admin Sign-In"
            'Application(actionstate) = "Confirm"
            'wctrl.Visible = True
            'hiddenfield.Value = True
            SetSignIn()
        ElseIf hiddenvalue = False Then
            'MultiView1.SetActiveView(ShowNowt)
            'BindUserGrid()
            'DisplayRolesInGrid()
            'wctrl = CType(PlaceHolder2.FindControl("Writedatauc1"), WriteDatauc)
            'Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
            'wcbutton.Text = "Admin Sign-In"
            'Application(actionstate) = "Confirm"
            'wctrl.Visible = True
            'hiddenfield.Value = True
            SetSignIn()
        End If


        'End If
        'added 23 feb
        'MachineName = Request.QueryString("val")
    End Sub
    Private Sub SetSignIn()
        MultiView1.SetActiveView(ShowNowt)
        BindUserGrid()
        DisplayRolesInGrid()
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        wcbutton.Text = "Admin Sign-In"
        Application(actionstate) = "Confirm"
        wctrl.Visible = True
        hiddenfield.Value = True
    End Sub
    Private Sub BindUserGrid()
        'Dim allUsers As MembershipUserCollection = Membership.GetAllUsers()
        'UserGrid.DataSource = allUsers
        Dim SqlDataSourceUser As New SqlDataSource()
        SqlDataSourceUser.ID = "SqlDataSourceUser"
        SqlDataSourceUser.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("userstring").ConnectionString

        SqlDataSourceUser.SelectCommand = "Select ur.firstname, ur.lastname, m.Email,u.UserName, n.RoleName, m.isapproved from aspnet_membership m left outer join aspnet_UsersInRoles r on m.UserId=r.UserId left outer join aspnet_users u on u.UserId=m.UserID left outer join aspnet_Roles n on n.RoleId = r.RoleId left outer join firstlastname ur on ur.UserId=m.UserId order by ur.lastname, ur.firstname"

        UserGrid.DataSource = SqlDataSourceUser

        UserGrid.DataBind()
    End Sub
    Protected Sub UserGrid_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles UserGrid.RowEditing
        ' Set the grid's EditIndex and rebind the data

        UserGrid.EditIndex = e.NewEditIndex
        BindUserGrid()
    End Sub

    Protected Sub UserGrid_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles UserGrid.RowCancelingEdit
        ' Revert the grid's EditIndex to -1 and rebind the data
        UserGrid.EditIndex = -1
        BindUserGrid()
    End Sub

    Protected Sub UserGrid_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles UserGrid.RowUpdating
        ' Exit if the page is not valid
        If Not Page.IsValid Then
            Exit Sub
        End If

        ' Determine the username of the user we are editing
        Dim UserName As String
        UserName = UserGrid.DataKeys(e.RowIndex).Value.ToString()
        'It is very important that this is not bound on every page load and page load is called
        'before the row updating command so put the bind in isnot postback on page load
        'from http://www.codeproject.com/Questions/139076/not-getting-new-entered-values-on-grid-view-row-up

        ' Read in the entered information and update the user
        Dim EmailTextBox As TextBox = CType(UserGrid.Rows(e.RowIndex).FindControl("Email"), TextBox)
        Dim approvedbox As CheckBox = CType(UserGrid.Rows(e.RowIndex).FindControl("Checkbox1"), CheckBox)
        ' Return information about the user
        '
        Dim UserInfo As MembershipUser
        UserInfo = Membership.GetUser(UserName)

        ' Update the User account information
        UserInfo.Email = EmailTextBox.Text.Trim()
        UserInfo.IsApproved = approvedbox.Checked
        Membership.UpdateUser(UserInfo)
        ' Revert the grid's EditIndex to -1 and rebind the data
        UserGrid.EditIndex = -1
        BindUserGrid()

        ' This should email to say registration complete
        Dim smtpClient As SmtpClient = New SmtpClient()
        Dim message As MailMessage = New MailMessage()
        Try
            Dim fromAddress As New MailAddress("VISIRSERVER@VISIRSERVER.bsuh.nhs.uk", "ELF")
            Dim regemail As String = EmailTextBox.Text.ToString.Trim

            Dim regAddress As New MailAddress(regemail)
            message.From = fromAddress
            message.To.Add(regAddress)
            message.Subject = "ELF registration"
            message.Body = "Your registration for ELF is now complete."
            smtpClient.Host = "10.216.10.47"
            smtpClient.Send(message)

        Catch ex As Exception
            'should be an error action here
        End Try
    End Sub

    Protected Sub CreateRoleButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CreateRoleButton.Click
        Dim newRoleName As String = RoleName.Text.Trim()
        Dim SqlDataSourceUser As New SqlDataSource()

        Try
            If Not Roles.RoleExists(newRoleName) Then
                ' Create the role
                Roles.CreateRole(newRoleName)
                ' Refresh the RoleList Grid
                Dim conn As SqlConnection

                Dim connectionString As String = ConfigurationManager.ConnectionStrings( _
                "userstring").ConnectionString

                Dim UpdateNow As SqlCommand
                conn = New SqlConnection(connectionString)
                UpdateNow = New SqlCommand("Update aspnet_Roles set Description = '0' where RoleName = @RoleName", conn)
                UpdateNow.Parameters.Add("@RoleName", System.Data.SqlDbType.NVarChar)
                UpdateNow.Parameters("@RoleName").Value = newRoleName


                conn.Open()
                UpdateNow.ExecuteNonQuery()
                conn.Close()


                DisplayRolesInGrid()

            End If
        Catch ex As System.ArgumentException

            Dim strScript As String = "<script>"
            strScript += "alert('Role Name must not be empty or contain commas');"
            strScript += "</script>"
            ScriptManager.RegisterStartupScript(CreateRoleButton, Me.GetType(), "JSCR", strScript.ToString(), False)

        Finally
            RoleName.Text = String.Empty
        End Try

    End Sub

    Private Sub DisplayRolesInGrid()
        'Dim rolesArray() As String
        'rolesArray = Roles.GetAllRoles()
        'RoleList.DataSource = rolesArray
        ''RoleList.DataSource = Roles.GetAllRoles()
        'RoleList.DataBind()


        Dim SqlDataSourceUser As New SqlDataSource()
        SqlDataSourceUser.ID = "SqlDataSourceUser"
        SqlDataSourceUser.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("userstring").ConnectionString

        SqlDataSourceUser.SelectCommand = "Select RoleName, description from aspnet_Roles"

        RoleList.DataSource = SqlDataSourceUser

        RoleList.DataBind()
    End Sub

    'Protected Sub RoleList_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles RoleList.RowDeleting
    '    ' Get the RoleNameLabel
    '    Dim RoleNameLabel As Label = CType(RoleList.Rows(e.RowIndex).FindControl("RoleNameLabel"), Label)

    '    ' Delete the role
    '    Roles.DeleteRole(RoleNameLabel.Text, False)

    '    ' Rebind the data to the RoleList grid
    '    DisplayRolesInGrid()

    'End Sub



    Protected Sub CloseButton_Click(sender As Object, e As System.EventArgs) Handles CloseButton.Click
        RoleName.Text = String.Empty
        Dim Multiviewlist As MultiView

        If Not Me.Parent.FindControl("Multiview1") Is Nothing Then
            Multiviewlist = Me.Parent.FindControl("Multiview1")
            Dim setView As View = Multiviewlist.FindControl("View0")
            Multiviewlist.SetActiveView(setView)
            Dim hidfield As HiddenField = Multiviewlist.FindControl("HiddenField1")
            hidfield.Value = False

        End If
       
        'Response.Redirect(Request.Url.ToString(), True)
    End Sub

    Protected Sub Rolelist_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles RoleList.RowEditing
        ' Set the grid's EditIndex and rebind the data

        RoleList.EditIndex = e.NewEditIndex
        'Dim selectcheckbox As CheckBox = CType(UserGrid.Rows(e.RowIndex).FindControl("Checkbox1"), CheckBox)
        DisplayRolesInGrid()
    End Sub
    Protected Sub Rolelist_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles RoleList.RowCancelingEdit
        ' Revert the grid's EditIndex to -1 and rebind the data
        RoleList.EditIndex = -1
        DisplayRolesInGrid()
    End Sub
    Protected Sub RoleList_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles RoleList.RowUpdating
        ' Exit if the page is not valid
        If Not Page.IsValid Then
            Exit Sub
        End If

        ' Determine the RoleName of the user we are editing
        Dim Rolename As String
        Rolename = RoleList.DataKeys(e.RowIndex).Value.ToString()
        'It is very important that this is not bound on every page load and page load is called
        'before the row updating command so put the bind in isnot postback on page load
        'from http://www.codeproject.com/Questions/139076/not-getting-new-entered-values-on-grid-view-row-up

        '' Read in the entered information and update the user
        'Dim EmailTextBox As TextBox = CType(UserGrid.Rows(e.RowIndex).FindControl("Email"), TextBox)
        Dim approvedbox As CheckBox = CType(RoleList.Rows(e.RowIndex).FindControl("Checkbox1"), CheckBox)
        Dim archived As String
        If approvedbox.Checked = True Then
            archived = "1"
        Else
            archived = "0"
        End If
        Dim conn As SqlConnection

        Dim connectionString As String = ConfigurationManager.ConnectionStrings( _
        "userstring").ConnectionString

        Dim UpdateNow As SqlCommand
        conn = New SqlConnection(connectionString)
        UpdateNow = New SqlCommand("Update aspnet_Roles set Description = @Description where RoleName = @RoleName", conn)
        UpdateNow.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar)
        UpdateNow.Parameters("@Description").Value = archived
        UpdateNow.Parameters.Add("@RoleName", System.Data.SqlDbType.NVarChar)
        UpdateNow.Parameters("@RoleName").Value = Rolename


        conn.Open()
        UpdateNow.ExecuteNonQuery()
        conn.Close()

        '' Return information about the user
        ''
        'Dim UserInfo As MembershipUser
        'UserInfo = Membership.GetUser(UserName)

        '' Update the User account information
        'UserInfo.Email = EmailTextBox.Text.Trim()
        'UserInfo.IsApproved = approvedbox.Checked
        'Membership.UpdateUser(UserInfo)
        ' Revert the grid's EditIndex to -1 and rebind the data
        RoleList.EditIndex = -1
        DisplayRolesInGrid()

        Try


        Catch ex As Exception
            'should be an error action here
        End Try
    End Sub
End Class
