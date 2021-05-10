Imports AjaxControlToolkit
Imports System.Data.SqlClient
Imports System.Data

Partial Public Class LockElfuc
    Inherits System.Web.UI.UserControl
    Public Event AcceptHandler As EventHandler
    Private appstate As String
    Private suspstate As String
    Private isFault As Boolean
    Private RunUpDone As String
    Private faultstate As String
    Dim modalpopupextenderLock As New ModalPopupExtender
    'Public Event ClinicalApproved(ByVal sender As Object, ByVal e As CommandEventArgs)
    Public Event ClinicalApproved()
    Public Property Tabby() As String
    Public Property UserReason() As Integer
    Public Property LinacName() As String

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
        Dim modalpop As ModalPopupExtender
        Dim modalname As String = "modalpopupextenderLock" + Tabby
        modalpop = CType(FindControl(modalname), ModalPopupExtender)
        'We need to determine if the user is authenticated
        'Get the values entered by the user
        Dim loginUsername As String = username
        Dim loginPassword As String = userpassword
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        Dim Activity As String
        Dim su As String = Application(suspstate)
        Dim en As String = Application(RunUpDone)
        Activity = DavesCode.Reuse.ReturnActivity(UserReason)

        'When tidying up don't now need to pass linac name to successful logon
        'Dim usergroupselected As Integer = DavesCode.Reuse.SuccessfulLogin(loginUsername, loginPassword, UserReason, textboxUser, passwordUser, logerrorbox, modalpop)

        Dim usergroupselected As Integer = DavesCode.Reuse.SuccessfulLogin(loginUsername, loginPassword, UserReason, textboxUser, passwordUser, logerrorbox)
        If usergroupselected <> Nothing Then
            '    'what happens if machinestate fails?
            resetLogInscreen()
            DavesCode.Reuse.MachineStateNew(loginUsername, usergroupselected, LinacName, UserReason, True, connectionString)
            'DavesCode.Reuse.MachineState(loginUsername, usergroupselected, LinacName, UserReason, True)
            modalpop.Hide()
            'DavesCode.Reuse.ReturnApplicationState(Activity)
            Dim lockoff As LockElfuc = CType(Me.Parent.FindControl("lockElfuc1"), LockElfuc)
            lockoff.Visible = False
            '    'eg from http://dotnetbites.wordpress.com/2014/02/15/call-parent-page-method-from-user-control-using-reflection/
            Me.Page.GetType.InvokeMember("UpdateUserDisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {usergroupselected})
            Me.Page.GetType.InvokeMember("UpdateButtons", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {})

            'Else
            '    'If it gets to here something has gone wrong with SuccessfulLogin()
            '    modalidentifier = modalpopupextendergen.ID
            '    modalpopupextendergen.Show()
        End If

    End Sub

    Public Event LaunchControl(ByVal Control As Integer)
    Protected Sub page_init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        'AddHandler AcceptLinac.AcceptHandler, AddressOf BlankTabs

    End Sub
    'Protected Sub BlankTabs(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Response.Redirect("faultPage.aspx?val=LA1")
    'End Sub
    Protected Sub page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim button As Button = FindControl("UnlockElf")
        button.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(button, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        appstate = "LogOn" + LinacName
        suspstate = "Suspended" + LinacName
        RunUpDone = "rppTab" + LinacName
        faultstate = "OpenFault" + LinacName
        If Application(appstate) = 1 Then
            Dim MyString As String
            Dim Tabnumber As String
            MyString = "ModalPopupextenderLock"
            Tabnumber = Tabby
            MyString = MyString & Tabnumber

            modalpopupextenderLock.ID = MyString
            'Dim TabSel As String
            'TabSel = "FSet"
            'TabSel = "FSet" & Tabnumber
            'If Application(TabSel) = 1 Then
            modalpopupextenderLock.BehaviorID = MyString
            modalpopupextenderLock.TargetControlID = "label1"
            modalpopupextenderLock.PopupControlID = "Panel1"
            modalpopupextenderLock.BackgroundCssClass = "modalBackground"
            PlaceHolder1.Controls.Add(modalpopupextenderLock)
            modalpopupextenderLock.Show()
        End If


    End Sub

    Protected Sub resetLogInscreen()
        Dim textboxUser As TextBox = FindControl("txtchkUserName")
        Dim passwordUser As TextBox = FindControl("txtchkPWD")
        Dim logerrorbox As Label = FindControl("LoginErrordetails")
        textboxUser.Text = Nothing
        passwordUser.Text = Nothing
        logerrorbox.Text = Nothing
        modalpopupextenderLock.Hide()
    End Sub



End Class
