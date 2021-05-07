Imports System.Data.SqlClient
Partial Class faultPage

    Inherits System.Web.UI.Page
    Private tabIndex As String
    Private appstate As String
    Private suspstate As String
    Private actionstate As String
    Private failstate As String
    Private repairstate As String
    Private treatmentstate As String
    Private clinicalstate As String
    Private returnclinical As String
    Private machinename As String
    Public Event ClinicalApprovedEvent()

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AddHandler WriteDataucfr.UserApproved, AddressOf UserApprovedEvent

    End Sub

    Protected Sub UserApprovedEvent(ByVal Tabused As String, ByVal Userinfo As String)
        'Alter magic number later
        Dim faulttab As String = "-1"
        Dim radio As Integer = 103
        machinename = MachineLabel.Text
        appstate = "LogOn" + machinename
        actionstate = "ActionState" + machinename
        suspstate = "Suspended" + machinename
        failstate = "FailState" + machinename
        repairstate = "rppTab" + machinename
        clinicalstate = "ClinicalOn" + machinename
        treatmentstate = "Treatment" + machinename
        returnclinical = "ReturnClinical" + machinename
        Dim actionflag As String = actionstate
        Dim Action As String = Application(actionstate)
        Dim EnergyPicked As String
        If Tabused = "Report" Then

            Dim mpContentPlaceHolder As ContentPlaceHolder
            Dim grdview As GridView

            mpContentPlaceHolder = _
            CType(Me.Master.FindControl("ContentPlaceHolder1"),  _
            ContentPlaceHolder)
            If Not mpContentPlaceHolder Is Nothing Then
                grdview = CType(mpContentPlaceHolder.FindControl("DummyGridview"), GridView)
            End If


            If Action = "Confirm" Then
                Dim time As DateTime
                Dim LinacID As String = MachineLabel.Text
                Dim LastFault As Integer
                Dim LastIncident As Integer
                'reportTab is the tab from where the fault was reported
                Dim reportTab As String = HiddenField1.Value
                Dim comment As String = DummyCommentBox.Value 'want it here for commit run ups
                Dim breakdown As Boolean = True
                Dim LinacStatusID As String = ""
                Dim susstate As String = Application(suspstate)
                Dim repstate As String = Application(repairstate)
                time = Now()
                'This has the effect of logging off the linac

                '20 April
                Dim conn As SqlConnection
                Dim connectionString As String = ConfigurationManager.ConnectionStrings( _
                "connectionstring").ConnectionString
                Dim existingfault As SqlCommand

                Dim reader As SqlDataReader
                conn = New SqlConnection(connectionString)

                'existingfault = New SqlCommand("SELECT IncidentID FROM FaultIDTable where Linac = @linac and Status in ('New', 'Open')", conn)
                'existingfault = New SqlCommand("SELECT TOP(1) [IncidentID], [StatusID] FROM FaultIDTable where Linac = @linac and Status in ('New', 'Open') ORDER BY [IncidentID] DESC", conn)
                'previous query didn't work if a new fault was created after original faults were closed before moving on from repair.
                existingfault = New SqlCommand("SELECT TOP(1) [IncidentID], [StatusID] FROM [FaultIDTable] where Linac = @linac and ReportClosed is Null and statusid is not NULL ORDER BY [IncidentID] DESC", conn)
                existingfault.Parameters.AddWithValue("@linac", machinename)
                conn.Open()
                reader = existingfault.ExecuteReader()
                If reader.HasRows() Then
                    'Have to now actually read the rows
                    reader.Read()
                    LinacStatusID = reader.Item("StatusID").ToString()

                Else
                    
                    Select Case reportTab
                        Case 0
                            Dim Usergroup As Integer
                            Usergroup = DavesCode.Reuse.GetRole(Userinfo)
                            LinacStatusID = DavesCode.Reuse.SetStatus(Userinfo, "Fault", 5, radio, LinacID, faulttab)
                        Case 1, 7
                            LinacStatusID = DavesCode.Reuse.CommitRunup(grdview, LinacID, faulttab, Userinfo, comment, False, True, False)
                        Case 2
                            LinacStatusID = DavesCode.Reuse.CommitPreClin(LinacID, Userinfo, comment, False, False, False, True)
                        Case 3
                            LinacStatusID = DavesCode.Reuse.CommitClinical(LinacID, Userinfo, True)
                            Application(suspstate) = 1
                        Case 4, 5, 6, 8
                            LinacStatusID = DavesCode.Reuse.WriteAuxTables(LinacID, Userinfo, comment, radio, reportTab, True, susstate, repstate, False)

                        Case Else
                            Application(failstate) = reportTab
                            LinacStatusID = DavesCode.Reuse.WriteAuxTables(LinacID, Userinfo, comment, radio, reportTab, True, susstate, repstate, False)
                    End Select

                    Application(appstate) = Nothing
                End If
                reader.Close()
                conn.Close()
                Dim incidentfault As SqlCommand
                Dim commfault As SqlCommand

                'Will either be new fault then new linacstatusid or existing statusid
                incidentfault = New SqlCommand("INSERT INTO FaultIDTable (DateInserted, Linac, Status, originalFaultID, ConcessionNumber, StatusID) VALUES (@DateInserted, @Linac, @Status, @originalFaultID, @ConcessionNumber, @StatusID)", conn)
                incidentfault.Parameters.Add("@DateInserted", System.Data.SqlDbType.DateTime)
                incidentfault.Parameters("@DateInserted").Value = time
                incidentfault.Parameters.Add("@Status", System.Data.SqlDbType.NVarChar, 20)
                incidentfault.Parameters("@Status").Value = "New"
                incidentfault.Parameters.Add("@originalFaultID", System.Data.SqlDbType.Int)
                incidentfault.Parameters("@originalFaultID").Value = 0
                incidentfault.Parameters.Add("@ConcessionNumber", System.Data.SqlDbType.NVarChar, 10)
                incidentfault.Parameters("@ConcessionNumber").Value = ""
                incidentfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                incidentfault.Parameters("@Linac").Value = LinacID
                incidentfault.Parameters.Add("@StatusID", System.Data.SqlDbType.Int)
                incidentfault.Parameters("@StatusID").Value = LinacStatusID
                conn.Open()
                incidentfault.ExecuteNonQuery()
                conn.Close()

                Dim comm1 As SqlCommand
                Dim reader1 As SqlDataReader

                comm1 = New SqlCommand("Select IncidentID from FaultIDTable where IncidentId = (select max(IncidentId) from FaultIDTable where linac = @linac)", conn)
                comm1.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                comm1.Parameters("@Linac").Value = LinacID
                conn.Open()
                reader1 = comm1.ExecuteReader()
                If reader1.Read() Then
                    LastIncident = reader1.Item("IncidentID")
                    reader1.Close()
                    conn.Close()
                End If

                'EnergyPicked = DropDownListEnergy.SelectedItem.Text
                'If EnergyPicked = "Select" Then
                EnergyPicked = ""
                'End If
                commfault = New SqlCommand("INSERT INTO ReportFault (Description, ReportedBy, DateReported, Area, Energy, GantryAngle, CollimatorAngle,Linac, IncidentID, BSUHID) " _
                                          & "VALUES (@Description, @ReportedBy, @DateReported, @Area, @Energy,@GantryAngle,@CollimatorAngle, @Linac, @IncidentID, @BSUHID )", conn)
                commfault.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar, 250)
                commfault.Parameters("@Description").Value = TextBox4.Text
                commfault.Parameters.Add("@ReportedBy", System.Data.SqlDbType.NVarChar, 50)
                commfault.Parameters("@ReportedBy").Value = Userinfo
                commfault.Parameters.Add("@DateReported", System.Data.SqlDbType.DateTime)
                commfault.Parameters("@DateReported").Value = time
                commfault.Parameters.Add("@Area", System.Data.SqlDbType.NVarChar, 20)
                commfault.Parameters("@Area").Value = DropDownListArea.SelectedItem.Text
                commfault.Parameters.Add("@Energy", System.Data.SqlDbType.NVarChar, 10)
                commfault.Parameters("@Energy").Value = EnergyPicked
                commfault.Parameters.Add("@GantryAngle", System.Data.SqlDbType.NVarChar, 3)
                commfault.Parameters("@GantryAngle").Value = TextBox2.Text
                commfault.Parameters.Add("@CollimatorAngle", System.Data.SqlDbType.NVarChar, 3)
                commfault.Parameters("@CollimatorAngle").Value = TextBox3.Text
                commfault.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                commfault.Parameters("@Linac").Value = LinacID
                commfault.Parameters.Add("@IncidentID", System.Data.SqlDbType.Int)
                commfault.Parameters("@IncidentID").Value = LastIncident
                commfault.Parameters.Add("@BSUHID", System.Data.SqlDbType.NVarChar, 7)
                commfault.Parameters("@BSUHID").Value = PatientIDTextBox.Text
                    Try
                        conn.Open()
                        commfault.ExecuteNonQuery()
                        conn.Close()
                        'This reads the number of the newly created fault to put into the fault tracking database

                        comm1 = New SqlCommand("Select FaultID from ReportFault where FaultId = (select max(FaultId) from ReportFault where linac = @linac)", conn)
                        comm1.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                        comm1.Parameters("@Linac").Value = LinacID
                        conn.Open()
                        reader1 = comm1.ExecuteReader()
                        If reader1.Read() Then
                            LastFault = reader1.Item("FaultID")
                            reader1.Close()
                            conn.Close()
                        End If

                    Finally
                        conn.Close()

                    End Try

                    Dim commtrack As SqlCommand
                commtrack = New SqlCommand("Insert into FaultTracking (Trackingcomment, AssignedTo, Status, LastupdatedBy, Lastupdatedon, Linac, IncidentID) " _
                                            & "VALUES (@Trackingcomment, @AssignedTo, @Status, @LastupdatedBy, @Lastupdatedon, @Linac, @IncidentID)", conn)
                'The tracking comment should be "" at this stage 11/04/18 not TextBox4.Text as that is fault description
                commtrack.Parameters.Add("@Trackingcomment", System.Data.SqlDbType.NVarChar, 250)
                commtrack.Parameters("@Trackingcomment").Value = ""
                commtrack.Parameters.Add("@AssignedTo", Data.SqlDbType.NVarChar, 50)
                    commtrack.Parameters("@AssignedTo").Value = "Unassigned"
                    commtrack.Parameters.Add("@Status", Data.SqlDbType.NVarChar, 50)
                    commtrack.Parameters("@Status").Value = "New"
                    commtrack.Parameters.Add("@LastupdatedBy", System.Data.SqlDbType.NVarChar, 50)
                    commtrack.Parameters("@LastupdatedBy").Value = Userinfo
                    commtrack.Parameters.Add("@Lastupdatedon", System.Data.SqlDbType.DateTime)
                    commtrack.Parameters("@Lastupdatedon").Value = time
                    commtrack.Parameters.Add("@Linac", System.Data.SqlDbType.NVarChar, 10)
                    commtrack.Parameters("@Linac").Value = MachineLabel.Text
                    commtrack.Parameters.Add("@IncidentID", System.Data.SqlDbType.Int)
                    commtrack.Parameters("@IncidentID").Value = LastIncident
                    Try
                        conn.Open()
                        commtrack.ExecuteNonQuery()


                    Finally
                        conn.Close()

                    End Try

                    'update incident table with fault id then don't need it in fault tracking table?

                    incidentfault = New SqlCommand("Update FaultIDTable SET originalFaultID=@originalFaultID where incidentID=@incidentID", conn)
                    incidentfault.Parameters.Add("@originalFaultID", Data.SqlDbType.Int)
                    incidentfault.Parameters("@originalFaultID").Value = LastFault
                    incidentfault.Parameters.Add("@incidentID", Data.SqlDbType.Int)
                    incidentfault.Parameters("@incidentID").Value = LastIncident
                    conn.Open()
                    incidentfault.ExecuteNonQuery()
                    conn.Close()

                Application(treatmentstate) = "Yes"
                    'FailState is the flag that tells repair and view open faults which tab the fault was reported from
                    '18 April move this to page load to enable multiple faults from tab 0 after fault reported
                    'Application(failstate) = HiddenField1.Value
                    '21 april
                'Dim tab As Stringa
                    'This sets the return tab as repair - important that it is always tab 5!
                    Dim state As String = Application(appstate)
                    'tab = "5"
                    If reportTab = "5" Then
                    'Application(appstate) = 1
                        'Application(suspstate) = Nothing
                        'Application(repairstate) = Nothing
                        'Application(treatmentstate) = "Yes"

                    Else
                        'don't want to return saved value of comment here because this sets the fault page. Want empty string
                        comment = ""
                    End If
                    state = Application(appstate)

                'Return where it came from now
                    Dim returnstring As String = LinacID + "page.aspx?pageref=Fault&Tabindex="
                    Response.Redirect(returnstring & reportTab & "&comment=" & comment)

            End If

        End If


    End Sub
    Protected Sub Faultconfirmed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles confirmfault.Click
        'have to have the contentplaceholder step because the control is on a main page
        ' - this wouldn't be necessary if this was a user control
        Dim mpContentPlaceHolder As ContentPlaceHolder
        Dim wctrl As WriteDatauc
        Dim wcbutton As Button
        Dim wctext As TextBox
        Dim Areastatus As String
        Dim strScript As String = "<script>"
        Dim actionstate As String
        'This finds control because here because it needs to be disabled if select because there are instances where it can pop up again
        ' added page.is valid
        If Page.IsValid Then
            mpContentPlaceHolder = _
               CType(Me.Master.FindControl("ContentPlaceHolder1"),  _
               ContentPlaceHolder)
            If Not mpContentPlaceHolder Is Nothing Then
                wctrl = CType(mpContentPlaceHolder.FindControl("Writedataucfr"), WriteDatauc)
                machinename = MachineLabel.Text
                actionstate = "ActionState" + machinename
                Areastatus = DropDownListArea.SelectedItem.Text
                If Areastatus = "Select" Then
                    strScript += "alert('Please select an Area');"
                    strScript += "</script>"
                    wctrl.Visible = False
                    ScriptManager.RegisterStartupScript(confirmfault, Me.GetType(), "JSCR", strScript.ToString(), False)
                Else
                    wcbutton = CType(wctrl.FindControl("AcceptOK"), Button)
                    wctext = CType(wctrl.FindControl("txtchkUserName"), TextBox)
                    wcbutton.Text = "Saving Fault"
                    Application(actionstate) = "Confirm"
                    wctrl.Visible = True
                    ForceFocus(wctext)
                End If
            Else
                'need an error message here
            End If
        End If
    End Sub

    Protected Sub Cancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Cancel.Click
        'This solution came via http://yasserzaid.wordpress.com/2009/01/02/how-to-go-back-to-the-previous-page-in-aspnet/
        'Dim refUrl As Object = ViewState("RefUrl")
        'If (Not (refUrl) Is Nothing) Then
        '    Response.Redirect(CType(refUrl, String))
        'End If
        'Application("Someoneisloggedin") = 1
        Dim LinacID As String = MachineLabel.Text
        failstate = "FailState" + LinacID
        returnclinical = "ReturnClinical" + LinacID
        Dim tab As String

        tab = HiddenField1.Value

        Dim comment As String = DummyCommentBox.Value
        'if the cancel originated from the repair, maintenance or physics tabs then want to remember original tab. Otherwise set to nothing
        If tab = "5" Or tab = "4" Or tab = "6" Then
            'don't alter failstate
        Else
            Application(failstate) = Nothing
        End If

        If tab = "3" Then
            Application(returnclinical) = 1
        End If
        Dim check As String = Application(returnclinical)
        Dim state As String = Application(appstate)
        Dim returnstring As String = LinacID + "page.aspx?pageref=Fault&tabref="
        'Dim returnstring As String = LinacID + "page.aspx?pageref=Fault&Tabindex="
        'DavesCode.Reuse.ReturnApplicationState("Fault page")
        Response.Redirect(returnstring & tab & "&comment=" & comment)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'from http://blog.davidsz.nl/2012/03/04/previous-page-in-asp-net/
        'Cancel.Attributes.Add("onClick", "javascript:history.back();   return false;")
        'leave  cancel_click empty if you use this


        If Not IsPostBack Then
            ViewState("RefUrl") = Request.UrlReferrer.ToString()
            Dim viewrefurl As String = ViewState("RefUrl")
            machinename = Request.QueryString("val")
            Dim reportTab As String = Request.QueryString("Tabindex")
            HiddenField1.Value = reportTab

            MachineLabel.Text = machinename
            appstate = "LogOn" + machinename
            actionstate = "ActionState" + machinename
            suspstate = "Suspended" + machinename
            failstate = "FailState" + machinename
            repairstate = "rppTab" + machinename
            clinicalstate = "ClinicalOn" + machinename
            treatmentstate = "Treatment" + machinename
            returnclinical = "ReturnClinical" + machinename
            AddEnergyItem()
            
            'Some error handling for if a problem and fault already open
            Dim conn As SqlConnection
            Dim connectionString As String = ConfigurationManager.ConnectionStrings( _
            "connectionstring").ConnectionString
            Dim existingfault As SqlCommand

            Dim reader As SqlDataReader
            conn = New SqlConnection(connectionString)

            existingfault = New SqlCommand("SELECT IncidentID FROM FaultIDTable where Linac = @linac and Status in ('New', 'Open')", conn)
            existingfault.Parameters.AddWithValue("@linac", machinename)
            conn.Open()
            reader = existingfault.ExecuteReader()
            'this allows for a fault to be reported from the status page if there is already a fault open

            If reader.HasRows() Then
                If (reportTab = "5") OrElse (reportTab = "0") Then

                Else
                    Dim strScript As String = "<script>"
                    strScript += "alert('An open fault already exists');"
                    'strScript += "window.location='la3page.aspx';"
                    strScript += "</script>"
                    ScriptManager.RegisterStartupScript(confirmfault, Me.GetType(), "JSCR", strScript.ToString(), False)
                    Dim tab As String = "5"
                    Dim comment As String = ""

                    'don't want to return saved value of comment here because this sets the fault page. Want empty string
                    Dim returnstring As String = machinename + "page.aspx?pageref=Fault&Tabindex="
                    Response.Redirect(returnstring & tab & "&comment=" & comment)
                End If
            Else
                'FailState is the flag that tells repair and view open faults which tab the fault was reported from
                '18 April move this to page load to enable multiple faults from tab 0 after fault reported
                Application(failstate) = HiddenField1.Value
                'Dim reportTab As String = HiddenField1.Value
                Dim commentbox As String = Request.QueryString("commentbox")
                DummyCommentBox.Value = commentbox
            End If
            conn.Close()
        End If

    End Sub
    Protected Sub AddEnergyItem() 'this is also in ViewOpenFaults.ascx.vb
        'from http://www.aspsnippets.com/Articles/Programmatically-add-items-to-DropDownList-on-Button-Click-in-ASPNet-using-C-and-VBNet.aspx
        'and http://www.yaldex.com/asp_tutorial/0596004877_progaspdotnet2-chp-5-sect-7.html

        'This is a mad way of doing it but I don't know how to dim the energy list without constructing it at the same time
        Dim MachineName As String = MachineLabel.Text
        Select Case MachineName
            Case "LA1"
                Dim Energy1() As String = {"Select", "6 MV", "6 MeV", "8 MeV", "10 MeV", "12 MeV", "15 MeV", "18 MeV", "20 MeV"}
                ConstructEnergylist(Energy1)
            Case "LA2", "LA3"
                Dim Energy1() As String = {"Select", "6 MV", "10 MV", "6 MeV", "8 MeV", "10 MeV", "12 MeV", "15 MeV", "18 MeV", "20 MeV"}
                ConstructEnergylist(Energy1)
            Case "LA4"
                Dim Energy1() As String = {"Select", "6 MV", "10 MV"}
                ConstructEnergylist(Energy1)
                'Added 6/7/17
            Case "E1", "E2", "B1"
                Dim Energy1() As String = {"Select", "6 MV", "6 MV FFF", "10 MV", "10 MV FFF", "4 MeV", "6 MeV", "8 MeV", "10 MeV", "12 MeV", "15 MeV"}
                ConstructEnergylist(Energy1)
            Case Else
                'Don't show any energies
        End Select


    End Sub
    Protected Sub ConstructEnergylist(ByVal Energylist() As String)
        Dim energy() As String = Energylist
        Dim i As Integer
        For i = 0 To energy.GetLength(0) - 1
            DropDownListEnergy.Items.Add(New ListItem(energy(i)))
        Next
        DropDownListEnergy.SelectedIndex = -1
    End Sub
    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" + _
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub
End Class

