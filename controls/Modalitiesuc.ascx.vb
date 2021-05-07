Imports System.Data.SqlClient
Imports System.Data

Partial Class Modalitiesuc
    Inherits System.Web.UI.UserControl

    Private keyfieldvalue As Integer
    Private actionstate As String
    Private energyrow As String
    Private keyfield As String
    Public Property LinacName() As String
    Public Property TabName() As String
    Public Event CloseModalityQAPopup(ByVal Linac As String)

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        AddHandler WriteDatauc1.UserApproved, AddressOf UserApprovedEvent
        'added specific application states so explicit to linac
        actionstate = "ActionState" + LinacName
        energyrow = "EnergyQA" + LinacName
        keyfield = "keyfield" + LinacName


    End Sub
    Protected Sub UserApprovedEvent(ByVal Tabused As String, ByVal Userinfo As String)
        Dim Repairlist As RadioButtonList
        Dim row As Integer = Application(energyrow)
        Dim erow As GridViewRow
        Dim cb As CheckBox
        Dim strScript As String
        Dim usergroupselected As Integer
        'Dim alertstring As String

        If Tabused = "QA" Then
            usergroupselected = DavesCode.Reuse.GetRole(Userinfo)
            erow = GridView1.Rows(row)
            cb = erow.Cells(3).Controls(0)
            If Not cb.Checked Or (cb.Checked And usergroupselected = 4) Then
                If Not Me.Parent.FindControl("RadioButtonlist1") Is Nothing Then
                    Repairlist = Me.Parent.FindControl("RadioButtonlist1")
                    'modified for E1 etc that don't have pre-clin now 9/10/17
                    If LinacName Like "LA?" Then
                        Repairlist.Items.FindByValue(2).Enabled = False
                    End If
                    Repairlist.Items.FindByValue(3).Enabled = False
                    If Not Repairlist.Items.FindByValue(4) Is Nothing Then
                        Repairlist.Items.FindByValue(4).Enabled = False
                    End If
                    If Not Repairlist.Items.FindByValue(5) Is Nothing Then
                        Repairlist.Items.FindByValue(5).Enabled = False
                    End If
                    Repairlist.Items.FindByValue(8).Enabled = False
                End If
                Dim Action As String = Application(actionstate)
                Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
                wctrl.Visible = False
                If Action = "Confirm" Then
                    Dim username As String = Userinfo
                    'Dim strScript As String = "<script>"
                    'Dim alertstring As String
                    Dim time As DateTime
                    time = Now()
                    Dim conn As SqlConnection
                    Dim connectionString As String = ConfigurationManager.ConnectionStrings(
                    "connectionstring").ConnectionString
                    conn = New SqlConnection(connectionString)
                    Dim commupdate As New SqlCommand("update PhysicsEnergies set Approved=@Approved, ApprovedBy=@ApprovedBy, DateApproved=@DateApproved, Comment=@Comment, linac=@linac where EnergyID=@EnergyID", conn)
                    'Dim row As Integer = Application(energyrow)
                    'Dim erow As GridViewRow
                    'erow = GridView1.Rows(row)

                    Dim tenergy As String
                    tenergy = erow.Cells(2).ToString

                    'Dim cb As CheckBox = erow.Cells(3).Controls(0)
                    Dim txtComment As TextBox = erow.Cells(6).Controls(0)
                    keyfieldvalue = Application(keyfield)
                    'Added 01/12/16 to write changed energy to new archive table
                    DavesCode.Reuse.ArchiveEnergies(keyfieldvalue)

                    conn.Open()
                    commupdate.Parameters.Add("@EnergyID", System.Data.SqlDbType.Int).Value = keyfieldvalue
                    commupdate.Parameters.Add("@Approved", SqlDbType.Bit).Value = cb.Checked
                    commupdate.Parameters.Add("@ApprovedBy", SqlDbType.VarChar).Value = username
                    commupdate.Parameters.Add("@DateApproved", SqlDbType.DateTime).Value = Now
                    commupdate.Parameters.Add("@Comment", SqlDbType.VarChar).Value = txtComment.Text
                    commupdate.Parameters.Add("@linac", SqlDbType.NVarChar).Value = LinacName
                    commupdate.ExecuteNonQuery()
                    'Message.Text = txtApprovedby.Text
                    conn.Close()
                    GridView1.EditIndex = -1
                    BindGridData()

                    Application(energyrow) = String.Empty
                    Application(keyfield) = String.Empty
                    Application(actionstate) = String.Empty


                    strScript = "<script>alert('Modality Updated. Please go to Eng Run up or End of Day after Raising/Closing concession');</script>"
                    ScriptManager.RegisterStartupScript(Close, Me.GetType(), "JSCR", strScript.ToString(), False)
                End If
            Else
                Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
                wctrl.Visible = False

                GridView1.EditIndex = -1
                BindGridData()
                strScript = "<script>alert('You do not have permission to reinstate a modality. Modality Not Updated');</script>"
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "JSCR", strScript.ToString(), False)

            End If

        Else

        End If

    End Sub


    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' if this is not here then before the grid is updated the old values are retrieved and new are overwritten
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        'WriteDatauc1.LinacName = MachineName
        wctrl.LinacName = LinacName
        'comment postback out when used with ModalityQAPoupUpuc
        'If Not IsPostBack Then
        'commented out showeditbutton 2/12/16
        BindGridData()

    End Sub

    Protected Sub GridView1_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs) Handles GridView1.RowEditing
        GridView1.EditIndex = e.NewEditIndex
        'Disable other edit buttons
        BindGridData()

    End Sub
    Protected Sub GridView1_RowCancelingEdit(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs) Handles GridView1.RowCancelingEdit
        GridView1.EditIndex = -1
        BindGridData()
    End Sub
    Protected Sub GridView1_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs) Handles GridView1.RowUpdating
        Dim strScript As String
        strScript = "<script>alert('Remember to Raise/Close Concession');</script>"

        ScriptManager.RegisterStartupScript(Close, Me.GetType(), "JSCR", strScript.ToString(), False)
        keyfieldvalue = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values("EnergyID").ToString())
        Dim row As Integer
        row = Convert.ToInt32(e.RowIndex)
        Application(energyrow) = row
        Application(keyfield) = keyfieldvalue
        Application(actionstate) = "Confirm"
        Dim wctrl As WriteDatauc = CType(FindControl("Writedatauc1"), WriteDatauc)
        Dim wcbutton As Button = CType(wctrl.FindControl("AcceptOK"), Button)
        wcbutton.Text = "Update Modality"
        Dim wctext As TextBox = CType(wctrl.FindControl("txtchkUserName"), TextBox)
        WriteDatauc1.Visible = True
        ForceFocus(wctext)

        'The event handler stuff used to be done here

    End Sub

    Private Sub BindGridData()
        Dim SqlDataSource1 As New SqlDataSource()
        SqlDataSource1.ID = "SqlDataSource1"

        SqlDataSource1.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
        SqlDataSource1.SelectCommand = "SELECT * FROM [PhysicsEnergies] where linac= @linac order by EnergyID"
        SqlDataSource1.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource1.SelectParameters.Add("linac", LinacName)
        GridView1.DataSource = SqlDataSource1
        GridView1.DataBind()
    End Sub

    'From https://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.gridview.rowdatabound(v=vs.110).aspx
    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs)

        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim lnk As LinkButton = TryCast(e.Row.FindControl("linkbutton1"), LinkButton)
            If lnk IsNot Nothing Then
                If TabName = 5 Then
                    lnk.Visible = True
                End If
            End If
        End If
    End Sub

    'Add force focus 9/10/17
    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" +
        ctrl.ClientID + "').focus();}, 100);", True)
    End Sub
    Protected Sub Close_Click(sender As Object, e As EventArgs) Handles Close.Click
        RaiseEvent CloseModalityQAPopup(LinacName)
    End Sub
End Class
