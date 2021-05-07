Partial Class Traininguc
    Inherits System.Web.UI.UserControl
    Private MachineName As String
    Private SelectCount As Boolean
    Private Radioselect As Integer
    Private laststate As String
    Private lastuser As String
    Private lastusergroup As String
    Private MainFaultPanel As controls_MainFaultDisplayuc
    Dim Master As Object
    Dim BoxChanged As String
    Private Event AutoApproved(ByVal Tab As String, ByVal UserName As String)
    Private tabstate As String
    Dim comment As String
    Const TRAINING As String = "8"
    Const FAULTPOPUPSELECTED As String = "faultpopupupselected"
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"

    Private Property DynamicControlSelection() As String
        Get
            Dim result As String = ViewState.Item(VIEWSTATEKEY_DYNCONTROL)
            If result Is Nothing Then
                'doing things like this lets us access this property without
                'worrying about this property returning null/Nothing
                Return String.Empty
            Else
                Return result
            End If
        End Get
        Set(ByVal value As String)
            ViewState.Item(VIEWSTATEKEY_DYNCONTROL) = value
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


    Public Sub UpdateReturnButtonsHandler()
        'removed reference to LA 9/4/19
        If Not IsPostBack Then
            RadioButtonList1.Items.Add(New ListItem("Go To Engineering Run up", "1", False))
            RadioButtonList1.Items.Add(New ListItem("Hand Back to Clinical", "3", False))
            RadioButtonList1.Items.Add(New ListItem("Go to Planned Maintenance", "4", False))
            RadioButtonList1.Items.Add(New ListItem("Go To Repair", "5", False))
            RadioButtonList1.Items.Add(New ListItem("End of Day", "102", True))
            'End If
        End If

        DavesCode.Reuse.GetLastTech(MachineName, 0, laststate, lastuser, lastusergroup)
        If laststate = GlobalConstants.SUSPENDED Then
            RadioButtonList1.Items.FindByValue(3).Enabled = True
            If (lastusergroup = 2) Or (lastusergroup = 4) Then
                RadioButtonList1.Items.FindByValue(4).Enabled = True
                RadioButtonList1.Items.FindByValue(5).Enabled = True


            End If
            '5 caters for autosign from last tab
        Else

            If ((lastusergroup = 2) Or (lastusergroup = 4) Or (lastusergroup = 5)) Then
                RadioButtonList1.Items.FindByValue(1).Enabled = True
                RadioButtonList1.Items.FindByValue(4).Enabled = True
                RadioButtonList1.Items.FindByValue(5).Enabled = True


            End If
        End If
        StateTextBox.Text = laststate

    End Sub
    Protected Sub Update_DefectDailyDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_defectsToday(LinacName)

        End If
    End Sub

    Protected Sub Update_ViewOpenFaults(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_OpenConcessions(LinacName)

        End If
    End Sub

    Protected Sub Update_ClosedFaultDisplay(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
            MainFaultPanel.Update_FaultClosedDisplay(LinacName)
        End If
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AddHandler WriteDatauc1.UserApproved, AddressOf UserApprovedEvent

        BoxChanged = "TABoxChanged" + MachineName

    End Sub

    Public Sub UserApprovedEvent(ByVal TabSet As String, ByVal Userinfo As String)
        Dim machinelabel As String = MachineName & "Page.aspx';"
        Dim username As String = Userinfo
        Dim LinacStateID As String = ""


        Dim FaultParams As DavesCode.FaultParameters = New DavesCode.FaultParameters()
        Dim EndofDay As Integer = 102
        Dim Recovery As Integer = 101

        If TabSet = TRAINING Then
            Dim strScript As String = "<script>"
            strScript += "window.location='"
            strScript += machinelabel
            strScript += "</script>"
            Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
            wctrl.Visible = False

            Dim result As Boolean = False
            Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
            HttpContext.Current.Session.Remove("Actionstate")

            If (Not HttpContext.Current.Application(BoxChanged) Is Nothing) Then
                comment = HttpContext.Current.Application(BoxChanged).ToString
            Else
                comment = String.Empty
            End If
            If Action = GlobalConstants.ENDOFDAY Then
                Radioselect = EndofDay 'local constant
                Action = "Confirm"

            ElseIf Action = "Recover" Then
                Radioselect = Recovery
            Else
                Radioselect = RadioButtonList1.SelectedItem.Value
            End If

            result = DavesCode.NewWriteAux.WriteAuxTables(MachineName, username, comment, Radioselect, TabSet, False, False, FaultParams)

            If result Then
                If Action = "Confirm" Then
                    CommentBox.ResetCommentBox(String.Empty)
                    Select Case Radioselect
                        Case 1

                            If lastusergroup <> 3 Then
                                Dim returnstring As String = MachineName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)
                                Response.Redirect(returnstring)
                            Else
                                ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                            End If

                        Case 3

                            If lastusergroup = 3 Then
                                Dim returnstring As String = MachineName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)
                                Response.Redirect(returnstring)
                            Else
                                ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                            End If
                        Case 4
                            If lastusergroup <> 3 Then
                                Dim returnstring As String = MachineName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)

                                Response.Redirect(returnstring)
                            Else
                                ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                            End If
                        Case 5

                            If lastusergroup <> 3 Then
                                Dim returnstring As String = MachineName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)
                                Response.Redirect(returnstring)
                            Else
                                ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                            End If
                        Case 6

                            If lastusergroup <> 3 Then
                                Dim returnstring As String = MachineName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)
                                Response.Redirect(returnstring)
                            Else
                                ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                            End If
                        Case 102

                            ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                    End Select

                    RadioButtonList1.SelectedIndex = -1
                    LogOffButton.BackColor = Drawing.Color.AntiqueWhite
                Else

                    Dim returnstring As String = LinacName + "page.aspx?TabAction=Recovered&NextTab=" & TabSet
                    Response.Redirect(returnstring, False)


                End If
            End If
        End If


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        WaitButtons("Tech")

        Dim ReportFault As controls_ReportAFaultuc = CType(FindControl("ReportAFaultuc1"), controls_ReportAFaultuc)
        ReportFault.LinacName = LinacName
        ReportFault.ParentControl = TRAINING
        AddHandler ReportFault.ReportAFault_UpdateDailyDefectDisplay, AddressOf Update_DefectDailyDisplay
        AddHandler ReportFault.ReportAFault_UpDateViewOpenFaults, AddressOf Update_ViewOpenFaults
        Dim objMFG As controls_MainFaultDisplayuc = Page.LoadControl("controls\MainFaultDisplayuc.ascx")
        CType(objMFG, controls_MainFaultDisplayuc).LinacName = LinacName
        CType(objMFG, controls_MainFaultDisplayuc).ID = "MainFaultDisplay"
        CType(objMFG, controls_MainFaultDisplayuc).ParentControl = TRAINING
        AddHandler objMFG.Mainfaultdisplay_UpdateClosedFaultDisplay, AddressOf Update_ClosedFaultDisplay
        PlaceHolderFaults.Controls.Add(objMFG)

        CommentBox.BoxChanged = BoxChanged
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        wctrl.LinacName = MachineName

        If Not IsPostBack Then

            RadioButtonList1.Items.Add(New ListItem("Go To Engineering Run up", "1", False))
            RadioButtonList1.Items.Add(New ListItem("Hand Back to Clinical", "3", False))
            RadioButtonList1.Items.Add(New ListItem("Go to Planned Maintenance", "4", False))
            RadioButtonList1.Items.Add(New ListItem("Go To Repair", "5", False))
            RadioButtonList1.Items.Add(New ListItem("End of Day", "102", True))
        End If
        DavesCode.Reuse.GetLastTech(MachineName, 0, laststate, lastuser, lastusergroup)

        If laststate = GlobalConstants.SUSPENDED Then
            RadioButtonList1.Items.FindByValue(3).Enabled = True
            If (lastusergroup = 2) Or (lastusergroup = 4) Then
                RadioButtonList1.Items.FindByValue(4).Enabled = True
                RadioButtonList1.Items.FindByValue(5).Enabled = True


            End If
            '5 caters for autosign from last tab
        Else
            If ((lastusergroup = 2) Or (lastusergroup = 4) Or (lastusergroup = 5)) And (laststate = "Linac Unauthorised") Then
                RadioButtonList1.Items.FindByValue(1).Enabled = True
                RadioButtonList1.Items.FindByValue(4).Enabled = True
                RadioButtonList1.Items.FindByValue(5).Enabled = True

            ElseIf (laststate = "Engineering Approved") Then

                If (lastusergroup = 2) Or (lastusergroup = 4) Then
                    RadioButtonList1.Items.FindByValue(4).Enabled = True
                    RadioButtonList1.Items.FindByValue(5).Enabled = True

                End If
            End If
        End If
        StateTextBox.Text = laststate

    End Sub

    Protected Sub LogOffButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LogOffButton.Click
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wclabel As Label = CType(wctrl.FindControl("WarningLabel"), Label)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        Dim LogOffUser As String = "AutoLog"
        DavesCode.Reuse.GetLastTech(MachineName, 0, laststate, lastuser, lastusergroup)
        Radioselect = RadioButtonList1.SelectedItem.Value
        Session.Add("name", LogOffUser)
        Session.Add("usergroup", lastusergroup)
        Session.Add("userreason", Radioselect)
        Session.Add("Actionstate", "Confirm")

        Select Case Radioselect
            Case 1

                If lastusergroup <> 3 Then
                    UserApprovedEvent(TRAINING, lastuser)
                Else
                    wcbutton.Text = "Go To Engineering Run up"
                    WriteDatauc1.Visible = True
                    ForceFocus(wctext)
                End If

            Case 3

                If lastusergroup = 3 Then
                    UserApprovedEvent(TRAINING, lastuser)
                Else
                    wcbutton.Text = "Return to clinical"
                    WriteDatauc1.Visible = True
                    ForceFocus(wctext)
                End If

            Case 4

                If lastusergroup <> 3 Then
                    UserApprovedEvent(TRAINING, lastuser)
                Else
                    wcbutton.Text = "Go To PM"
                    WriteDatauc1.Visible = True
                    ForceFocus(wctext)
                End If
            Case 5

                If lastusergroup <> 3 Then
                    UserApprovedEvent(TRAINING, lastuser)
                Else
                    wcbutton.Text = "Go To Repair"
                    WriteDatauc1.Visible = True
                    ForceFocus(wctext)
                End If

            Case 102
                wclabel.Text = "Are you sure? This is the End of Day"
                wcbutton.Text = "End of Day"
                WriteDatauc1.Visible = True
                ForceFocus(wctext)
        End Select


    End Sub


    Protected Sub RadioButtonList1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadioButtonList1.SelectedIndexChanged

        LogOffButton.Enabled = True
        LogOffButton.BackColor = Drawing.Color.Yellow

    End Sub

    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" +
        ctrl.ClientID + "').focus();}, 100);", True)
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
