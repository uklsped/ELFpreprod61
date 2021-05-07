Imports System.Data.SqlClient
Imports DavesCode

Partial Class LinacStatusuc
    Inherits System.Web.UI.UserControl
    Private Objfault As ViewFaultsuc
    Private RegistrationState As String

    Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
    Dim Modalities As controls_ModalityDisplayuc


    Public Property LinacName() As String
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AddHandler WriteDatauc1.UserApproved, AddressOf UserApprovedEvent
        AddHandler RegisterUseruc1.ReloadTab, AddressOf PageLoad
        'Added handler so viewfaultsuc1 is reset to cope with nonmachinepage
        AddHandler ViewFaultsuc1.ReloadFaultsTab, AddressOf PageLoad

    End Sub
    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim Resetbutton As Button = CType(FindControl("Reset"), Button)
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        wctrl.LinacName = LinacName
        Dim fltctrl As ViewFaultsuc = CType(FindControl("ViewFaultsuc1"), ViewFaultsuc)
        fltctrl.LinacName = LinacName
        WaitButtons()

        Dim adminctrl As Administrationuc = CType(FindControl("Administrationuc1"), Administrationuc)
        adminctrl.LinacName = LinacName

        'Dim value As Boolean
        'value = HiddenField2.Value
        If LinacName = "nonmachine" Then
            Resetbutton.Visible = False
        End If

    End Sub
    Protected Sub PageLoad()
        Dim returnstring As String
        'Application(RegistrationState) = True
        'HiddenField2.Value = True


        If GetApplication.GetApplicationState(LinacName, 0) Then
            'Session.Add("returnFromLinacStatus", True)
            Dim tab As String = GetApplication.Returnlastuserreason(LinacName, 0).ToString
            returnstring = LinacName + "page.aspx?TabAction=Clicked&NextTab=" + tab

        Else
            returnstring = LinacName + "Page.aspx"
        End If

        Response.Redirect(returnstring)

    End Sub
    'This is reset control. Could rework now to actually recover state.
    Protected Sub UserApprovedEvent(ByVal Tabused As String, ByVal Userinfo As String)
        Dim tab As String = Tabused
        Dim reader As SqlDataReader
        Dim Status As String = ""
        Dim conn As SqlConnection
        Dim conActivity As SqlCommand
        Dim connectionString As String = ConfigurationManager.ConnectionStrings(
        "connectionstring").ConnectionString
        Dim returnstring As String

        If tab = "0" Then
            conn = New SqlConnection(connectionString)

            conActivity = New SqlCommand("SELECT state FROM [LinacStatus] where stateID = (Select max(stateID) as lastrecord from [LinacStatus] where linac=@linac)", conn)

            conActivity.Parameters.AddWithValue("@linac", LinacName)
            conn.Open()
            reader = conActivity.ExecuteReader()

            If reader.Read() Then
                Status = reader.Item("State")
            End If
            reader.Close()
            conn.Close()

            If Status = "Fault" Then
                If NewFaultHandling.CheckForOpenFault(LinacName) Then
                    DavesCode.Reuse.SetStatus(Userinfo, "Fault", 5, 5, LinacName, 5, False)

                Else
                    DavesCode.Reuse.SetStatus(Userinfo, "Linac Unauthorised", 5, 7, LinacName, 0, False)

                End If

            Else
                DavesCode.Reuse.SetStatus(Userinfo, "Linac Unauthorised", 5, 7, LinacName, 0, False)

            End If
            returnstring = LinacName + "page.aspx"

            Response.Redirect(returnstring)

        End If
    End Sub

    Protected Sub Reset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Reset.Click
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        wcbutton.Text = "Confirm Reset"
        Session.Add("Actionstate", "Confirm")
        WriteDatauc1.Visible = True
        ForceFocus(wctext)

    End Sub

    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" + _
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub


    Sub FaultButton_Click(s As Object, e As EventArgs)
        Dim faultcon As ViewFaultsuc
        faultcon = MultiView1.FindControl("ViewFaultsuc1")
        faultcon.Update()
        HiddenField1.Value = False
        MultiView1.Visible = True
        MultiView1.SetActiveView(View1)

    End Sub

    Sub RegisterButton_Click(s As Object, e As EventArgs)
        FaultButton.Visible = False
        RegisterButton.Visible = False
        AdminButton.Visible = False
        HiddenField1.Value = False
        MultiView1.Visible = True
        MultiView1.SetActiveView(View2)

    End Sub

    Sub AdminButton_Click(s As Object, e As EventArgs)

        HiddenField1.Value = True
        MultiView1.Visible = True
        MultiView1.SetActiveView(View3)
    End Sub
    Sub WaitButtons()
        Dim FaultButton As Button = FindControl("FaultButton")
        Dim RegisterButton As Button = FindControl("RegisterButton")
        Dim AdminButton As Button = FindControl("AdminButton")
        Dim HistoryButton As Button = FindControl("HistoryButton")

        If Not FaultButton Is Nothing Then
            FaultButton.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(FaultButton, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
        If Not RegisterButton Is Nothing Then
            RegisterButton.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(RegisterButton, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
        If Not AdminButton Is Nothing Then
            AdminButton.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(AdminButton, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
        If Not HistoryButton Is Nothing Then
            HistoryButton.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(HistoryButton, "") + ";this.value='Wait...';this.disabled = true; this.style.display='block';")
        End If
    End Sub

    Protected Sub HistoryButton_Click(sender As Object, e As EventArgs) Handles HistoryButton.Click

    End Sub
End Class
