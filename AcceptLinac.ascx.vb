Imports AjaxControlToolkit
Imports System.Data.SqlClient
Imports System.Data
Imports System.Transactions

Partial Public Class AcceptLinac
    Inherits System.Web.UI.UserControl
    Public Event AcceptHandler As EventHandler
    Private MachineName As String
    Private Reason As Integer
    Private tablabel As String
    Private appstate As String
    Private suspstate As String
    Dim modalpopupextendergen As New ModalPopupExtender
    Public Event ClinicalApproved(ByVal connectionString As String)
    Public Event AcknowledgeEnergies()
    Public Event UpdateReturnButtons()
    Public Event ShowName(ByVal LastUserGroup As Integer)
    Public Event EngRunuploaded(ByVal connectionString As String)
    Public Event PreRunuploaded(ByVal connectionString As String)
    Public Event SetModalities(ByVal connectionString As String)
    Public Event Repairloaded(ByVal connectionString As String)
    Public Event CloseAcceptlinac(ByVal LinacName As String)


    'Private LinacObj As LinacState

    Public Property Tabby() As String
        Get
            Return tablabel
        End Get
        Set(ByVal value As String)
            tablabel = value
        End Set
    End Property
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

    Public ReadOnly Property username() As String
        Get
            username = txtchkUserName.Text.Trim()
        End Get

    End Property
    Public ReadOnly Property Userpassword() As String
        Get
            Userpassword = txtchkPWD.Text.Trim()
        End Get
    End Property

    Public Sub AcceptOK_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AcceptOK.Click
        Dim output As String
        Dim strScript As String = "<script>"
        Dim textboxUser As TextBox = FindControl("txtchkUserName") 'This gets username textbox to pass to login
        Dim passwordUser As TextBox = FindControl("txtchkPWD")  'This gets password textbox to pass to login
        Dim logerrorbox As Label = FindControl("LoginErrordetails") 'This gets error label to pass to login
        Dim modalpop As ModalPopupExtender
        Dim modalname As String = "Modalpopupextendergen" & Tabby
        Dim usergroupname As String = ""
        modalpop = CType(FindControl(modalname), ModalPopupExtender)
        'We need to determine if the user is authenticated
        'Get the values entered by the user
        Dim loginUsername As String = username
        Dim loginPassword As String = Userpassword
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        Dim Activity As String
        Dim modalidentifier As String
        appstate = "LogOn" + MachineName
        suspstate = "Suspended" + MachineName
        Dim reload As String
        Dim clinstate As String = "Clinical"
        'LinacObj = Application(LinacName)
        'reload = LinacObj.LinacStatus
        reload = DavesCode.Reuse.GetLastState(MachineName, 0)
        'from http://spacetech.dk/vb-net-string-compare-not-equal.html
        If Not (reload.Equals(clinstate)) Then
            Dim myAppState As Integer = CInt(Application(appstate))
            'myAppState = 1
            DavesCode.Reuse.RecordStates(LinacName, Tabby, "AcceptOK", 0)
            If myAppState = 0 Then
                Activity = DavesCode.Reuse.ReturnActivity(Reason)

                'This can only return here if there is a valid log in
                'Dim usergroupselected As Integer = DavesCode.Reuse.SuccessfulLogin(loginUsername, loginPassword, Reason, textboxUser, passwordUser, logerrorbox, modalpop)
                Dim usergroupselected As Integer = DavesCode.Reuse.SuccessfulLogin(loginUsername, loginPassword, Reason, textboxUser, passwordUser, logerrorbox)

                If usergroupselected <> Nothing Then

                    Try
                        'Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
                        Using myscope As TransactionScope = New TransactionScope()
                            'DavesCode.Reuse.MachineState(loginUsername, usergroupselected, MachineName, Reason, False)
                            DavesCode.Reuse.MachineStateNew(loginUsername, usergroupselected, LinacName, Reason, False, connectionString)
                            Select Case Tabby
                                Case 1, 7
                                    'RaiseEvent EngRunuploaded(connectionString)
                                    'RaiseEvent SetModalities(connectionString)

                                Case 2
                                    RaiseEvent PreRunuploaded(connectionString)
                                Case 3
                                    Me.Page.GetType.InvokeMember("LaunchTab", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {})
                                    output = "Clinical"
                                    Me.Page.GetType.InvokeMember("Updatestatedisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {output})

                                    RaiseEvent ClinicalApproved(connectionString)
                                    'RaiseEvent SetModalities(connectionString)
                                Case 4, 8
                                    RaiseEvent UpdateReturnButtons()
                                    'RaiseEvent ShowName(usergroupselected)
                                Case 5
                                    RaiseEvent Repairloaded(connectionString)
                                    'RaiseEvent Repairloaded(connectionString)
                            End Select
                            output = connectionString
                            'Me.Page.GetType.InvokeMember("SetModalities", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {output})
                            myscope.Complete()
                        End Using

                        modalidentifier = modalpopupextendergen.ID
                        textboxUser.Text = String.Empty
                        modalpop.Hide()
                        'moved if to case 3 above
                        'If usergroupselected = 3 Then
                        '    Me.Page.GetType.InvokeMember("LaunchTab", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {})
                        'End If

                        Application.Lock()
                        Application(appstate) = 1
                        Application.UnLock()
                        output = Application(appstate)
                        'eg from http://dotnetbites.wordpress.com/2014/02/15/call-parent-page-method-from-user-control-using-reflection/
                        ' this is an instrumentation field that displays application number ie 0 or 1
                        'Me.Page.GetType.InvokeMember("UpdateHiddenLAField", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {output})
                        Me.Page.GetType.InvokeMember("UpdateDisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {Activity})
                        'this is instrumentation code that displays current username
                        'Me.Page.GetType.InvokeMember("Updateuserlabel", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {loginUsername})
                        'If Tabby = 3 Then
                        '    output = "Clinical"
                        '    Me.Page.GetType.InvokeMember("Updatestatedisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {output})
                        '    RaiseEvent ClinicalApproved()
                        'End If
                        'moved following to using above
                        'Select Case Tabby
                        'Case 1
                        '    RaiseEvent EngRunuploaded(connectionString)
                        'Case 3
                        '    output = "Clinical"
                        '    Me.Page.GetType.InvokeMember("Updatestatedisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {output})
                        '    RaiseEvent ClinicalApproved()

                        'Case 4, 5, 8
                        '    RaiseEvent UpdateReturnButtons()
                        '    RaiseEvent ShowName(usergroupselected)
                        'End Select
                        RaiseEvent ShowName(usergroupselected)
                        RaiseEvent CloseAcceptlinac(MachineName)
                        '    myscope.Complete()
                        'End Using
                    Catch ex As Exception
                        DavesCode.NewFaultHandling.LogError(ex)
                        RaiseLoadError()
                    End Try
                Else
                    'If it gets to here something has gone wrong with SuccessfulLogin()
                    modalidentifier = modalpopupextendergen.ID
                    textboxUser.Text = "Round we go"
                    modalpopupextendergen.Show()

                End If
            Else
                textboxUser.Text = ""
                Application(appstate) = 0
                modalidentifier = modalpopupextendergen.ID
                'modalpop.Hide()
                modalpop.Dispose()
                modalidentifier = modalpop.ID
            End If
        Else
            Try
                If Tabby = 3 Then
                    Using myscope As TransactionScope = New TransactionScope()
                        RaiseEvent ClinicalApproved(connectionString)
                        output = "Clinical"
                        Me.Page.GetType.InvokeMember("Updatestatedisplay", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {output})
                        modalidentifier = modalpopupextendergen.ID
                        modalpop.Hide()
                        myscope.Complete()
                    End Using
                End If
            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)
                RaiseLoadError()
            End Try
        End If
    End Sub
    Protected Sub RaiseLoadError()
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Logging On. Please call Administrator');"
        strScript += "window.location='"
        strScript += machinelabel
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(AcceptOK, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Public Event LaunchControl(ByVal Control As Integer)
    Protected Sub Page_init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        'AddHandler AcceptLinac.AcceptHandler, AddressOf BlankTabs
        appstate = "LogOn" + MachineName
        suspstate = "Suspended" + MachineName

    End Sub
    'Protected Sub BlankTabs(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Response.Redirect("faultPage.aspx?val=LA1")
    'End Sub

    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DavesCode.Reuse.RecordStates(LinacName, Tabby, "AcceptPageLoad", 0)
        Dim reload As String
        Dim clinstate As String = "Clinical"
        reload = DavesCode.Reuse.GetLastState(MachineName, 0)
        If reload = Nothing Then
            reload = clinstate
        End If

        appstate = "LogOn" + MachineName
        suspstate = "Suspended" + MachineName
        'from http://spacetech.dk/vb-net-string-compare-not-equal.html
        If Not (reload.Equals(clinstate)) Then
            Dim placeholder As PlaceHolder

            Dim modalpop As ModalPopupExtender
            Dim modalname As String = "Modalpopupextendergen" & Tabby
            placeholder = CType(FindControl("PlaceHolder1"), PlaceHolder)

            modalpop = CType(placeholder.FindControl(modalname), ModalPopupExtender)



            If Not modalpop Is Nothing Then
                Dim id As String = modalpop.ID
            End If
            WaitButtons("Acknowledge")
            If Application(appstate) = 0 Then
                Dim MyString As String
                Dim Tabnumber As String
                If Tabby = 3 Then
                    If Not MachineName Like "T#" Then
                        Dim objCon As UserControl = Page.LoadControl("EnergyDisplayuc.ascx")
                        CType(objCon, EnergyDisplayuc).LinacName = MachineName
                        PlaceHolder2.Controls.Add(objCon)
                        'PlaceHolder2.Visible = True
                        Panel1.Width = 1000
                        Panel1.Height = 200
                        AcceptOK.Text = "Acknowledge Energies and Accept Linac"
                    End If
                End If
                MyString = "ModalPopupextendergen"
                Tabnumber = tablabel
                MyString = MyString & Tabnumber
                modalpopupextendergen.ID = MyString
                modalpopupextendergen.BehaviorID = "B" & MyString
                modalpopupextendergen.TargetControlID = "label1"
                modalpopupextendergen.PopupControlID = "Panel1"
                modalpopupextendergen.BackgroundCssClass = "modalBackground"
                PlaceHolder1.Controls.Add(modalpopupextendergen)
                modalpopupextendergen.Show()
                Dim textboxUser As TextBox = FindControl("txtchkUserName")
                If Tabnumber = 1 Then
                    textboxUser.Text = Tabnumber
                    AcceptOK.Text = "Why the fuck doesn't this work"
                End If
                'placeholder = CType(FindControl("PlaceHolder1"), PlaceHolder)

                'modalpop = CType(placeholder.FindControl(modalname), ModalPopupExtender)
            Else
                PlaceHolder2.Visible = False
            End If
        Else
            PlaceHolder2.Visible = False
        End If


    End Sub

    Protected Sub LAQA_handle(ByVal sender As Object, ByVal e As System.EventArgs)

    End Sub

    Public Sub Btnchkcancel_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnchkcancel.Click
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        AcceptOK.Visible = False
        appstate = "LogOn" + MachineName
        suspstate = "Suspended" + MachineName
        Dim reload As String
        Dim clinstate As String = "Clinical"
        reload = DavesCode.Reuse.GetLastState(MachineName, 0)
        'from http://spacetech.dk/vb-net-string-compare-not-equal.html
        If Not (reload.Equals(clinstate)) Then
            Dim myAppState As Integer = CInt(Application(appstate))

            If myAppState = 0 Then
                Dim loginUsername As String = username
                Dim returnstring As String
                Application(appstate) = 0
                'If Application(suspstate) = 1 Then
                'DavesCode.Reuse.SetStatus(loginUsername, "Clinical - Not Treating", 3, 3, MachineName, 7)
                'DavesCode.Reuse.SetStatus(loginUsername, "Suspended", 5, 7, MachineName, 7) 'This line of code removed after testing 28th may as the state shouldn't be written to.
                'modalpopupextendergen.Hide()
                If Tabby = 3 Then
                    Application(suspstate) = 1
                End If
                returnstring = MachineName + "page.aspx"
                Response.Redirect(returnstring)
            Else
                Dim modalpop As ModalPopupExtender
                Dim modalname As String = "Modalpopupextendergen" & Tabby
                Dim usergroupname As String = ""
                modalpop = CType(FindControl(modalname), ModalPopupExtender)
                modalpop.Hide()
            End If
        Else
            RaiseEvent ClinicalApproved(connectionString)
        End If
    End Sub
    Private Sub WaitButtons(ByVal WaitType As String)

        Select Case WaitType
            Case "Acknowledge"
                Dim Accept As Button = FindControl("AcceptOK")
                Dim Cancel As Button = FindControl("btnchkcancel")
                If Not FindControl("AcceptOK") Is Nothing Then
                    Accept.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Accept, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not FindControl("btnchkcancel") Is Nothing Then
                    Cancel.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Cancel, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

            Case "Tech"
                Dim Eng As Button = FindControl("engHandoverButton")
                Dim LogOff As Button = FindControl("LogOffButton")
                Dim Lock As Button = FindControl("LockElf")
                Dim Physics As Button = FindControl("PhysicsQA")
                Dim Atlas As Button = FindControl("ViewAtlasButton")
                Dim FaultPanel As Button = FindControl("FaultPanelButton")
                If Not Eng Is Nothing Then
                    Eng.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Eng, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not LogOff Is Nothing Then
                    LogOff.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(LogOff, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Lock Is Nothing Then
                    Lock.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Lock, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Physics Is Nothing Then
                    Physics.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Physics, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Atlas Is Nothing Then
                    Atlas.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Atlas, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not FaultPanel Is Nothing Then
                    FaultPanel.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(FaultPanel, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

            Case "Rad"
                Dim clin As Button = FindControl("clinHandoverButton")
                Dim LogOff As Button = FindControl("LogOff")
                Dim TStart As Button = FindControl("TStart")
                If Not clin Is Nothing Then
                    clin.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(clin, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not LogOff Is Nothing Then
                    LogOff.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(LogOff, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not TStart Is Nothing Then
                    TStart.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(TStart, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

        End Select

    End Sub

End Class
