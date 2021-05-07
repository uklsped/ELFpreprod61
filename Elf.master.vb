Imports System.Web.Services
Imports System.Data.SqlClient
Imports System.Configuration




Partial Class Elf

    Inherits System.Web.UI.MasterPage
    <WebMethod()> _
    Public Shared Sub AbandonSession()
        HttpContext.Current.Session.Abandon()
    End Sub
    Private MachineName As String
    Private Reason As Integer

    Public Property UserReason() As Integer
        Get
            Return Reason
        End Get
        Set(ByVal value As Integer)
            Reason = value
        End Set
    End Property


    Public Property LinacName() As String
        Get
            Return MachineName
        End Get
        Set(ByVal value As String)
            MachineName = value
        End Set
    End Property

    'Public Sub CheckUser_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckUser.Load
    '    If (DavesCode.Reuse.help(username, userpassword, "LA1", 1)) = True Then
    '        Application("RULoggedin") = 1
    'ModalPopupExtender1.Show()
    '        ErunupUserControl1.Visible = True
    '        'Else
    '        '    Dim strScript As String = "<script>"
    '        '    strScript += "alert('You are not allowed to perform that action');"
    '        '    strScript += "window.location='default.aspx';"
    '        '    strScript += "</script>"
    '        '    Me.ClientScript.RegisterStartupScript(Me.[GetType](), "Startup", strScript)
    '    End If
    'End Sub
    'Public ReadOnly Property username() As String
    '    Get
    '        username = txtchkUserName.Text
    '    End Get

    'End Property
    'Public ReadOnly Property userpassword() As String
    '    Get
    '        userpassword = txtchkPWD.Text
    '    End Get
    'End Property

    'Public Sub ShowLogon()
    '    usetab.Show()
    'End Sub
    'Protected Sub button1_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
    '    If (DavesCode.Reuse.help(username, userpassword, MachineName, UserReason)) = True Then
    '        '        Application("RULoggedin") = 1


    '        usetab.Hide()



    'RaiseEvent MyEvent(Me, New EventArgs())
    '        'ErunupUserControl1.Visible = True
    '        '        'Else
    '        '        '    Dim strScript As String = "<script>"
    '        '        '    strScript += "alert('You are not allowed to perform that action');"
    '        '        '    strScript += "window.location='default.aspx';"
    '        '        '    strScript += "</script>"
    '        '        '    Me.ClientScript.RegisterStartupScript(Me.[GetType](), "Startup", strScript)
    '    End If
    'End Sub

    Public Event MyEventMaster As EventHandler

    'Public Sub LogOn()
    '    programmaticModalPopup.Show()
    'End Sub

    'Public Sub SendOK_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SendOK.Click

    '    '    'RaiseEvent MyEventMasterPage(Me, New EventArgs())
    '    programmaticModalPopup.Hide()
    '    RaiseEvent MyEventMaster(Me, EventArgs.Empty)
    'End Sub

    'Protected Sub applogoff_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles applogoff.Click
    '    DavesCode.Reuse.SetStatus("Linac Unauthorised", 5, 7, "LA4", 7)
    '    Server.Transfer("default.aspx")
    'End Sub

    'Protected Sub LA1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LA1.Click

    '    Response.Redirect("LA1page.aspx")


    'End Sub

    'Protected Sub LA2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LA2.Click
    '    Response.Redirect("LA2page.aspx")
    'End Sub

    'Public Sub LoginUser()
    '    ModalPopupextender1.Show()
    'End Sub

End Class

