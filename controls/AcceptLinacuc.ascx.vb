Partial Public Class AcceptLinacuc
    Inherits System.Web.UI.UserControl
    Dim usergroupselected As Integer = Nothing
    Public Property UserReason() As Integer
    Public Property LinacName() As String

    Public ReadOnly Property Username() As String
        Get
            Username = txtchkUserName.Text.Trim()
        End Get

    End Property
    Public ReadOnly Property Userpassword() As String
        Get
            Userpassword = txtchkPWD.Text.Trim()
        End Get
    End Property

    Public Sub AcceptOK_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AcceptOK.Click

        Dim textboxUser As TextBox = FindControl("txtchkUserName") 'This gets username textbox to pass to login
        Dim passwordUser As TextBox = FindControl("txtchkPWD")  'This gets password textbox to pass to login
        Dim logerrorbox As Label = FindControl("LoginErrordetails") 'This gets error label to pass to login

        usergroupselected = DavesCode.Reuse.SuccessfulLogin(Username, Userpassword, UserReason, textboxUser, passwordUser, logerrorbox)

        If usergroupselected <> Nothing Then

            Try
                AddDataToSession()
                textboxUser.Text = String.Empty
                Dim returnstring As String = LinacName & "page.aspx?TabAction=Clicked&NextTab=" & UserReason
                Response.Redirect(returnstring, False)

            Catch ex As Exception
                DavesCode.NewFaultHandling.LogError(ex)
                RaiseLoadError()
            End Try
        Else
            textboxUser.Text = String.Empty

        End If

    End Sub
    Protected Sub AddDataToSession()
        Session.Add("name", Username)
        Session.Add("usergroup", usergroupselected)
        Session.Add("userreason", UserReason)
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

    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ForceFocus(txtchkUserName)
        WaitButtons("Acknowledge")

        AcceptTabLabel.Text = "Log on to " & DavesCode.Reuse.ReturnActivity(UserReason)

        If UserReason = 3 Then

            If Not LinacName Like "T#" Then
                Dim objCon As UserControl = Page.LoadControl("EnergyDisplayuc.ascx")
                CType(objCon, EnergyDisplayuc).LinacName = LinacName
                PlaceHolder2.Controls.Add(objCon)
                AcceptLinacDisplay.Width = 950
                AcceptLinacDisplay.Height = 210
                AcceptOK.Text = "Acknowledge Energies and Accept Linac"
            End If
        End If

    End Sub

    Public Sub Btnchkcancel_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnchkcancel.Click
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        AcceptOK.Visible = False

        Dim returnstring As String
        returnstring = LinacName & "page.aspx"
        Response.Redirect(returnstring)

    End Sub
    Private Sub WaitButtons(ByVal WaitType As String)

        Select Case WaitType
            Case "Acknowledge"
                Dim Accept As Button = FindControl("AcceptOK")
                Dim Cancel As Button = FindControl("btnchkcancel")
                If Not FindControl("AcceptOK") Is Nothing Then
                    Accept.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Accept, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not FindControl("btnchkcancel") Is Nothing Then
                    Cancel.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Cancel, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

            Case "Tech"
                Dim Eng As Button = FindControl("engHandoverButton")
                Dim LogOff As Button = FindControl("LogOffButton")
                Dim Lock As Button = FindControl("LockElf")
                Dim Physics As Button = FindControl("PhysicsQA")
                Dim Atlas As Button = FindControl("ViewAtlasButton")
                Dim FaultPanel As Button = FindControl("FaultPanelButton")
                If Not Eng Is Nothing Then
                    Eng.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Eng, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not LogOff Is Nothing Then
                    LogOff.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(LogOff, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Lock Is Nothing Then
                    Lock.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Lock, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Physics Is Nothing Then
                    Physics.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Physics, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not Atlas Is Nothing Then
                    Atlas.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(Atlas, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not FaultPanel Is Nothing Then
                    FaultPanel.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(FaultPanel, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

            Case "Rad"
                Dim clin As Button = FindControl("clinHandoverButton")
                Dim LogOff As Button = FindControl("LogOff")
                Dim TStart As Button = FindControl("TStart")
                If Not clin Is Nothing Then
                    clin.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(clin, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not LogOff Is Nothing Then
                    LogOff.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(LogOff, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If
                If Not TStart Is Nothing Then
                    TStart.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(TStart, "") & ";this.value='Wait...';this.disabled = true; this.style.display='block';")
                End If

        End Select

    End Sub
    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" +
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

End Class
