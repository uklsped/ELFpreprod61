Imports DavesCode
Imports GlobalConstants
Partial Class Planned_Maintenanceuc
    Inherits System.Web.UI.UserControl

    Private SelectCount As Boolean
    Private Radioselect As Integer
    Private objconToday As TodayClosedFault
    Private Todaydefect As DefectSave
    Private Todaydefectpark As DefectSavePark
    Private MainFaultPanel As controls_MainFaultDisplayuc
    Dim Master As Object
    Private laststate As String
    Private lastuser As String
    Private lastusergroup As String
    Private BoxChanged As String
    Private Event AutoApproved(ByVal Tab As String, ByVal UserName As String)
    Public Event BlankGroup(ByVal BlankUser As Integer)
    Private tabstate As String
    Dim FaultParams As DavesCode.FaultParameters = New DavesCode.FaultParameters()
    Public Property LinacName() As String
    Private comment As String
    Const PM As String = "4"
    Dim objReportFault As controls_ReportFaultPopUpuc
    Private WithEvents objdefect As DefectSave = New DefectSave
    Const FAULTPOPUPSELECTED As String = "faultpopupupselected"
    Const QASELECTED As String = "ModalityQApopupselected"
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"

    Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
    Dim Modalities As controls_ModalityDisplayuc
    Const ENDOFDAY102 As Integer = 102
    Const RECOVERY101 As Integer = 101
    Const LOCKELFSELECTED As String = "LockELFSelected"

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

    Protected Sub Close_ModalityQAPopUp(ByVal EquipmentId As String)
        If LinacName = EquipmentId Then
            DynamicControlSelection = String.Empty
            Dim ModalityQA As controls_ModalityQAPopUpuc = CType(FindControl("ModalityQAPopUpuc1"), controls_ModalityQAPopUpuc)
            PlaceHolderModalities.Controls.Remove(ModalityQA)
        End If
    End Sub

    Protected Sub Update_FaultClosedDisplays(ByVal EquipmentID As String)
        MainFaultPanel = PlaceHolderFaults.FindControl("MainFaultDisplay")
        MainFaultPanel.Update_FaultClosedDisplay(EquipmentID)

    End Sub


    ' This updates the defect display on defectsave etc when repeat fault from viewopenfaults
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

    Public Sub UpdateReturnButtonsHandler()

        'getlasttech returns laststate by ref!!!!
        Reuse.GetLastTech(LinacName, 0, laststate, lastuser, lastusergroup)

        StateTextBox.Text = laststate

    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AddHandler WriteDatauc1.UserApproved, AddressOf UserApprovedEvent
        AddHandler AutoApproved, AddressOf UserApprovedEvent

        BoxChanged = "PMBoxChanged" + LinacName


    End Sub
    Public Sub UserApprovedEvent(ByVal TabSet As String, ByVal Userinfo As String)
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim username As String = Userinfo
        Dim LinacStateID As String = ""

        Dim result As Boolean = False
        'This is here to cater for when system is recovering at end of day so no tab is actually loaded.

        If TabSet = PM Then

            If (Not HttpContext.Current.Application(BoxChanged) Is Nothing) Then
                comment = HttpContext.Current.Application(BoxChanged).ToString
            Else
                comment = String.Empty
            End If
            Dim strScript As String = "<script>"
            strScript += "window.location='"
            strScript += machinelabel
            strScript += "</script>"
            Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
            wctrl.Visible = False
            Dim Action As String = HttpContext.Current.Session("Actionstate").ToString
            HttpContext.Current.Session.Remove("Actionstate")

            If Action = ENDOFDAY Then
                Radioselect = ENDOFDAY102 'local constant = 102
                Action = "Confirm"

            ElseIf Action = "Recover" Then
                Radioselect = RECOVERY101 'local constant = 101
            Else
                Radioselect = RadioButtonList1.SelectedItem.Value
            End If

            result = NewWriteAux.WriteAuxTables(LinacName, username, comment, Radioselect, TabSet, False, False, FaultParams)
            If result Then
                If Action = "Confirm" Then

                    CommentBox.ResetCommentBox(String.Empty)
                    Select Case Radioselect
                        Case 1

                            Dim returnstring As String = LinacName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)

                            Response.Redirect(returnstring)
                            'Case 2

                        Case 3

                            ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                        Case 5
                            Dim returnstring As String = LinacName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)

                            Response.Redirect(returnstring)
                        Case 6
                            Dim returnstring As String = LinacName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)

                            Response.Redirect(returnstring)
                        Case 102

                            ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
                        Case 8
                            Dim returnstring As String = LinacName + "page.aspx?TabAction=Autoclicked&NextTab=" & Convert.ToString(Radioselect)

                            Response.Redirect(returnstring)
                    End Select
                    LogOffButton.BackColor = Drawing.Color.AntiqueWhite
                    RadioButtonList1.SelectedIndex = -1

                Else

                    'Dim returnstring As String = LinacName + "page.aspx?TabAction=Recovered&NextTab=" & TabSet
                    Dim returnstring As String = LinacName + "page.aspx"
                    Response.Redirect(returnstring, False)
                End If
            Else
                RaiseLogOffError()
            End If
        End If

    End Sub
    Protected Sub RaiseLogOffError()
        Dim machinelabel As String = LinacName & "Page.aspx';"
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Logging Off. Please inform Engineering');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(LogOffButton, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        laststate = Reuse.GetLastState(LinacName, 0)
        WaitButtons("Tech")
        CommentBox.BoxChanged = BoxChanged

        Dim objMFG As controls_MainFaultDisplayuc = Page.LoadControl("controls\MainFaultDisplayuc.ascx")
        CType(objMFG, controls_MainFaultDisplayuc).LinacName = LinacName
        CType(objMFG, controls_MainFaultDisplayuc).ID = "MainFaultDisplay"
        CType(objMFG, controls_MainFaultDisplayuc).ParentControl = PM
        AddHandler objMFG.Mainfaultdisplay_UpdateClosedFaultDisplay, AddressOf Update_FaultClosedDisplays

        PlaceHolderFaults.Controls.Add(objMFG)

        Dim Vctrl As ViewCommentsuc = CType(FindControl("ViewCommentsuc1"), ViewCommentsuc)
        Vctrl.LinacName = LinacName

        Dim ReportFault As controls_ReportAFaultuc = CType(FindControl("ReportAFaultuc1"), controls_ReportAFaultuc)
        ReportFault.LinacName = LinacName
        ReportFault.ParentControl = PM
        AddHandler ReportFault.ReportAFault_UpdateDailyDefectDisplay, AddressOf Update_DefectDailyDisplay
        AddHandler ReportFault.ReportAFault_UpDateViewOpenFaults, AddressOf Update_ViewOpenFaults

        Modalities = Page.LoadControl("controls/ModalityDisplayuc.ascx")
        CType(Modalities, controls_ModalityDisplayuc).LinacName = LinacName
        CType(Modalities, controls_ModalityDisplayuc).ID = "ModalityDisplay"
        If laststate = SUSPENDED Then
            CType(Modalities, controls_ModalityDisplayuc).Mode = "Suspended"
        Else
            CType(Modalities, controls_ModalityDisplayuc).Mode = "Linac Unauthorised"
        End If
        CType(Modalities, controls_ModalityDisplayuc).ConnectionString = connectionString
        ModalityPlaceholder.Controls.Add(Modalities)
        ModalityDisplayPanel.Visible = True

        Select Case Me.DynamicControlSelection

            Case LOCKELFSELECTED
                Dim ObjLock As controls_UnLockElfuc = Page.LoadControl("controls\UnLockElfuc.ascx")

                ObjLock.LinacName = LinacName
                ObjLock.UserReason = PM
                AddHandler ObjLock.HideUnlockPopUp, AddressOf HideUnlockElf
                'ObjLock.ID = "LockElf1"

                LockELFPlaceholder.Controls.Add(ObjLock)
                LockELFModalPopup.Show()
            Case QASELECTED
                Dim ObjQA As controls_ModalityQAPopUpuc = Page.LoadControl("controls\ModalityQAPopUpuc.ascx")
                ObjQA.LinacName = LinacName
                ObjQA.ParentControl = PM
                ObjQA.ID = "ModalityQAPopUpuc1"
                DynamicControlSelection = QASELECTED
                AddHandler ObjQA.CloseModalityQAPopUpTab, AddressOf Close_ModalityQAPopUp
                PlaceHolderModalities.Controls.Add(ObjQA)
            Case Else
                '        'no dynamic controls need to be loaded...yet
        End Select
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        wctrl.LinacName = LinacName
        If Not IsPostBack Then
            If Not LinacName Like "T?" Then
                PhysicsQA.Visible = True
            End If
            RadioButtonList1.Items.Add(New ListItem("Go To Engineering Run up", "1", True))
            RadioButtonList1.Items.Add(New ListItem("Hand Back to Clinical", "3", False))
            RadioButtonList1.Items.Add(New ListItem("Go To Repair", "5", True))
            RadioButtonList1.Items.Add(New ListItem("Go To Training/Development", "8", True))
            RadioButtonList1.Items.Add(New ListItem("End of Day", "102", True))

            StateTextBox.Text = "Linac Unauthorised"

            If laststate = SUSPENDED Then
                RadioButtonList1.Items.FindByValue(3).Enabled = True
                StateTextBox.Text = "Suspended"
            End If

        End If

    End Sub

    Public Sub HideUnlockElf(ByVal Hide As Boolean)
        If Hide Then
            LockELFModalPopup.Hide()
            DynamicControlSelection = String.Empty
        Else
            LockELFModalPopup.Show()
            DynamicControlSelection = LOCKELFSELECTED
        End If
    End Sub
    Protected Sub LogOffButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LogOffButton.Click
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        Dim wclabel As Label = CType(wctrl.FindControl("WarningLabel"), Label)
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        Dim laststate As String = String.Empty
        Dim lastusername As String = String.Empty
        Dim lastusergroup As Integer
        Dim LogOffUser As String = "AutoLog"
        Radioselect = RadioButtonList1.SelectedItem.Value
        Reuse.GetLastTech(LinacName, 0, laststate, lastusername, lastusergroup)
        Session.Add("name", LogOffUser)
        Session.Add("usergroup", lastusergroup)
        Session.Add("userreason", Radioselect)
        Session.Add("Actionstate", "Confirm")

        Select Case Radioselect
            Case 1, 5, 8

                UserApprovedEvent(PM, LogOffUser)
            'Case 2

            Case 3
                wcbutton.Text = "Return to clinical"
                WriteDatauc1.Visible = True
                ForceFocus(wctext)

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

    Protected Sub PhysicsQA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles PhysicsQA.Click
        Dim ObjQA As controls_ModalityQAPopUpuc = Page.LoadControl("controls\ModalityQAPopUpuc.ascx")
        ObjQA.LinacName = LinacName
        ObjQA.ParentControl = PM
        ObjQA.ID = "ModalityQAPopUpuc1"
        DynamicControlSelection = QASELECTED
        AddHandler ObjQA.CloseModalityQAPopUpTab, AddressOf Close_ModalityQAPopUp
        PlaceHolderModalities.Controls.Add(ObjQA)

    End Sub
    Protected Sub LockElf_Click(sender As Object, e As EventArgs) Handles LockElf.Click

        comment = CommentBox.Currentcomment
        DynamicControlSelection = String.Empty
        LockELFModalPopup.Hide()
        Dim tabused = PM
        Dim radioselect As Integer = 7
        Dim success As Boolean = False
        Dim username As String = "Lockuser"
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        success = NewWriteAux.WriteAuxTables(LinacName, username, comment, radioselect, tabused, False, True, FaultParams)

        If success Then
            RaiseEvent BlankGroup(0)
            Dim ObjLock As controls_UnLockElfuc = Page.LoadControl("controls\UnLockElfuc.ascx")
            ObjLock.LinacName = LinacName
            ObjLock.UserReason = PM
            AddHandler ObjLock.HideUnlockPopUp, AddressOf HideUnlockElf
            LockELFPlaceholder.Controls.Add(ObjLock)
            DynamicControlSelection = LOCKELFSELECTED
            LockELFModalPopup.Show()

        Else
            RaiseLockError()
        End If

    End Sub

    Protected Sub RaiseLockError()
        Dim strScript As String = "<script>"

        strScript += "alert('Problem Locking Elf. Please inform system administator');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(LockElf, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" + _
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
