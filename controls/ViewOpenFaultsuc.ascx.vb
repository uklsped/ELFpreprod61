Imports System.Data.SqlClient
Imports System.Data
Imports System.Drawing
Imports DavesCode

Partial Class controls_ViewOpenFaultsuc
    Inherits System.Web.UI.UserControl

    Private FaultStatus As String
    Private ClinicalButton As Button
    Private suspstate As String
    Private FaultOriginTab As String
    Private actionstate As String
    Private RunUpDone As String
    Private laststate As String
    Private faultviewstate As String
    Private technicalstate As String
    Private faultstate As String
    Private RadRow As DataTable
    Const REPEATFAULTSELECTED As String = "REPEAT"
    Const CONCESSIONSELECTED As String = "CSelected"
    Const VIEWFAULTSELECTED As String = "FSelected"
    Const VIEWSTATEKEY_DYNCONTROL As String = "DynamicControlSelection"
    Const EMPTYSTRING As String = ""
    Const CONCESSION As String = "Concession"
    Const CLOSED As String = "Closed"
    Const OPEN As String = "Open"

    Private ConcessionDescriptionChanged As String
    Private ConcessionActionChanged As String
    Private ConcessionCommentChanged As String
    Private ParamApplication As String
    Private ConcessParamsTrial As ConcessionParameters = New ConcessionParameters()
    Private FaultApplication As String
    Private FaultParams As FaultParameters = New FaultParameters()



    'from  
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

    Public Event ViewOpenFaults_UpdateClosedFaultDisplays(ByVal EquipmentName As String)

    Public Property ParentControl() As String
    Public Property LinacName() As String
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init

        ConcessionDescriptionChanged = "ConcessionDescription" + LinacName
        ConcessionActionChanged = "ConcessionAction" + LinacName
        ConcessionCommentChanged = "ConcessionComment" + LinacName
        ParamApplication = "Params" + LinacName
        FaultApplication = "FaultParams" + LinacName



    End Sub

    Protected Sub UpdateCloseDisplays(ByVal EquipmentID As String)
        If LinacName = EquipmentID Then
            RaiseEvent ViewOpenFaults_UpdateClosedFaultDisplays(LinacName)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Select Case Me.DynamicControlSelection

            Case CONCESSIONSELECTED

                Dim ConcessionPopup As controls_ConcessionPopUpuc = Page.LoadControl("controls\ConcessionPopUpuc.ascx")
                CType(ConcessionPopup, controls_ConcessionPopUpuc).ID = "ConcessionPopup"
                CType(ConcessionPopup, controls_ConcessionPopUpuc).LinacName = LinacName
                CType(ConcessionPopup, controls_ConcessionPopUpuc).ParentName = ParentControl
                CType(ConcessionPopup, controls_ConcessionPopUpuc).Visible = True
                AddHandler ConcessionPopup.CloseFaultTracking, AddressOf CloseTracking
                AddHandler ConcessionPopup.UpdateClosedDisplays, AddressOf UpdateCloseDisplays
                ConcessionPopupPlaceHolder.Controls.Add(ConcessionPopup)
                ConcessionPopup.SetUpFaultTracking()


            Case Else
                '        'no dynamic controls need to be loaded...yet
        End Select


        RadRow = HighlightRow()
        BindConcessionGrid()


    End Sub


    '    'from http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.buttonfield.commandname%28v=vs.110%29.aspx?cs-save-lang=1&cs-lang=vb#code-snippet-2

    Protected Sub CloseTracking(ByVal Linac As String)

        If Linac = LinacName Then
            Dim ConcessionPopup As controls_ConcessionPopUpuc = CType(FindControl("ConcessionPopup"), controls_ConcessionPopUpuc)
            ConcessionPopupPlaceHolder.Controls.Remove(ConcessionPopup)
            ViewState.Item(VIEWSTATEKEY_DYNCONTROL) = String.Empty
            ConcessionGrid.Enabled = True
            BindConcessionGrid()

            ConcessionSelectionPanel.Visible = True

            Application(technicalstate) = Nothing

            ConcessionGrid.Columns(5).Visible = True
            ConcessionGrid.Columns(6).Visible = True

        End If
    End Sub

    Protected Sub ConcessionGrid_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles ConcessionGrid.PageIndexChanging

        ConcessionGrid.PageIndex = e.NewPageIndex
        BindConcessionGrid()

    End Sub

    Sub FaultGridView_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        'this can be changed to pass concession number to report repeat fault? 02/11/18
        Dim success As Boolean = False

        If Not e.CommandName = "Page" Then
            Dim IncidentID As String
            ' Convert the row index stored in the CommandArgument
            ' property to an Integer.
            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            ' Retrieve the row that contains the button clicked 
            ' by the user from the Rows collection.
            Dim row As GridViewRow = ConcessionGrid.Rows(index)
            IncidentID = Server.HtmlDecode(row.Cells(0).Text)

            'This is where the concession number is found out
            Dim Concession As String = Server.HtmlDecode(row.Cells(1).Text)
            Dim Region As String
            Region = ""

            ' If multiple buttons are used in a GridView control, use the
            ' CommandName property to determine which button was clicked.
            Select Case e.CommandName
                Case "View"
                    Select Case ParentControl
                        Case "1", "4", "5"
                            success = ConcessParamsTrial.CreateObject(IncidentID)

                            If success Then
                                Application(ParamApplication) = ConcessParamsTrial

                                ConcessionGrid.Enabled = False
                                ConcessionSelectionPanel.Visible = False
                                DynamicControlSelection = CONCESSIONSELECTED

                                Dim ConcessionPopup As controls_ConcessionPopUpuc = Page.LoadControl("controls\ConcessionPopUpuc.ascx")
                                CType(ConcessionPopup, controls_ConcessionPopUpuc).ID = "ConcessionPopup"
                                CType(ConcessionPopup, controls_ConcessionPopUpuc).LinacName = LinacName
                                CType(ConcessionPopup, controls_ConcessionPopUpuc).ParentName = ParentControl
                                CType(ConcessionPopup, controls_ConcessionPopUpuc).Visible = True
                                AddHandler ConcessionPopup.CloseFaultTracking, AddressOf CloseTracking
                                AddHandler ConcessionPopup.UpdateClosedDisplays, AddressOf UpdateCloseDisplays
                                ConcessionPopupPlaceHolder.Controls.Add(ConcessionPopup)
                                ConcessionPopup.SetUpFaultTracking()

                            Else
                                RaiseError()
                            End If

                        Case Else
                            ConcessionHistoryuc1.BindConcessionHistoryGrid(IncidentID)
                            HideFaultsclinicalview.Visible = True
                            MultiView1.SetActiveView(View1)
                            ConcessionGrid.Enabled = False
                            ConcessionSelectionPanel.Visible = False
                    End Select


                    ''from http://www.sqlservercentral.com/Forums/Topic1416029-1292-1.aspx
                    'This displays associated faults - look at how it closes at the moment
                Case "Faults"
                    Dim objCon As ManyFaultGriduc = Page.LoadControl("ManyFaultGriduc.ascx")
                    CType(objCon, ManyFaultGriduc).NewFault = False
                    CType(objCon, ManyFaultGriduc).IncidentID = IncidentID
                    'to accomodate Tomo now need to pass equipment name?
                    CType(objCon, ManyFaultGriduc).LinacName = LinacName
                    PlaceHolderFaults.Controls.Add(objCon)
                    MultiView1.SetActiveView(View2)
                    ConcessionGrid.Enabled = False
                    Hidefaults.Visible = True
                    HideFaultsclinicalview.Visible = True
                    ConcessionSelectionPanel.Visible = False
            End Select

        End If

    End Sub

    Protected Sub FormError()
        Dim strScript As String = "<script>"
        strScript += "alert('Please Correct Form Errors');"
        strScript += "</script>"

    End Sub
    Function CheckUniqueConcession(ByVal ConcessionNumber As String) As Boolean
        Dim NewNumber As String = ConcessionNumber
        Dim ncount As Integer = 1
        Dim unique As Boolean = False
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim reader1 As SqlDataReader
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        conn = New SqlConnection(connectionString)
        comm = New SqlCommand("select Count(ConcessionNumber) as 'Count' from ConcessionTable where linac=@linac and ConcessionNumber = @ConcessionNumber", conn)
        comm.Parameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        comm.Parameters("@linac").Value = LinacName
        comm.Parameters.Add("@ConcessionNumber", System.Data.SqlDbType.NVarChar)
        comm.Parameters("@ConcessionNumber").Value = NewNumber

        Try
            conn.Open()
            reader1 = comm.ExecuteReader()
            If reader1.Read() Then
                ncount = reader1.Item("Count")
                reader1.Close()
            End If
        Finally
            conn.Close()
        End Try
        If ncount = 0 Then
            unique = True
        End If
        Return unique
    End Function

    Protected Sub OnPaging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles ConcessionGrid.PageIndexChanging
        ConcessionGrid.PageIndex = e.NewPageIndex
        ConcessionGrid.DataBind()
    End Sub
    Sub BindConcessionGrid()
        Dim SqlDataSource2 As New SqlDataSource With {
            .ID = "SqlDataSource2",
            .ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString,
            .SelectCommand = "select distinct f.incidentID,  f.Dateinserted, c.ConcessionDescription, c.ConcessionNumber, c.Action, f.linac " _
        & "from FaultIDTable f left outer join ConcessionTable c on c.ConcessionNumber = f.ConcessionNumber where f.linac=@linac and f.Status in('Concession') order by incidentid desc"
        }
        'Open was added to f.status allow use with singlemachinefaultuc it will only be appropriate for the repair page. TAken out on 5/8/15

        'SqlDataSource2.SelectCommand = "select r.FaultID, r.Description, r.ReportedBy, r.DateReported, r.FaultStatus, t.ConcessionNumber, r.linac " & _
        '   "from reportfault r left outer join Faulttracking t on r.FaultID=t.FaultID where r.linac=@linac and r.faultstatus=t.status and FaultStatus in('Concession') order by faultid desc"
        'SqlDataSource2.SelectCommand = "select * from reportfault where linac=@linac and FaultStatus in('Concession')"
        SqlDataSource2.SelectParameters.Add("@linac", System.Data.SqlDbType.NVarChar)
        SqlDataSource2.SelectParameters.Add("linac", LinacName)
        ConcessionGrid.DataSource = SqlDataSource2
        ConcessionGrid.DataBind()
        CheckEmptyGrid(ConcessionGrid)

    End Sub
    Protected Sub Hidefaults_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Hidefaults.Click, HideFaultsclinicalview.Click
        ConcessionGrid.Visible = True
        ConcessionGrid.Enabled = True
        ConcessionGrid.SelectedIndex = -1
        Hidefaults.Visible = False
        HideFaultsclinicalview.Visible = False
        ConcessionSelectionPanel.Visible = True

    End Sub

    'delete 3/7/18

    '    'from http://www.aspsnippets.com/Articles/Programmatically-add-items-to-DropDownList-on-Button-Click-in-ASPNet-using-C-and-VBNet.aspx
    '    'and http://www.yaldex.com/asp_tutorial/0596004877_progaspdotnet2-chp-5-sect-7.html

    Protected Sub Cancel()

        ConcessionGrid.Enabled = True
        ConcessionSelectionPanel.Visible = True
        Me.DynamicControlSelection = String.Empty
        Page_Load(Page, EventArgs.Empty)
    End Sub


    Protected Sub CheckEmptyGrid(ByVal grid As GridView)
        'If grid.Rows.Count = 0 And Not grid.DataSource Is Nothing Then
        '    Dim dt As Object = Nothing
        '    If grid.DataSource.GetType Is GetType(Data.DataSet) Then
        '        dt = New System.Data.DataSet
        '        dt = CType(grid.DataSource, System.Data.DataSet).Tables(0).Clone()
        '    ElseIf grid.DataSource.GetType Is GetType(Data.DataTable) Then
        '        dt = New System.Data.DataTable
        '        dt = CType(grid.DataSource, System.Data.DataTable).Clone()
        '    ElseIf grid.DataSource.GetType Is GetType(Data.DataView) Then
        '        dt = New System.Data.DataView
        '        dt = CType(grid.DataSource, System.Data.DataView).Table.Clone
        '    Else
        '        grid.DataSource
        '        dt = New System.Data.DataTable
        '        gr()
        '        dt = CType(grid.DataSource, System.Data.DataTable).Clone()
        '    End If
        '    dt.Rows.Add(dt.NewRow())
        '    grid.DataSource = dt
        '    grid.DataBind()
        '    Dim columnsCount As Integer
        '    Dim tCell As New TableCell()
        '    columnsCount = grid.Columns.Count
        '    grid.Rows(0).Cells.Clear()
        '    grid.Rows(0).Cells.Add(tCell)
        '    grid.Rows(0).Cells(0).ColumnSpan = columnsCount


        '    grid.Rows(0).Cells(0).Text = "No Records To Display"
        '    grid.Rows(0).Cells(0).HorizontalAlign = HorizontalAlign.Center
        '    grid.Rows(0).Cells(0).ForeColor = Drawing.Color.Black
        '    grid.Rows(0).Cells(0).Font.Bold = True


        '    'grid.Rows(0).Visible = False

        'End If
    End Sub

    Private Sub ForceFocus(ByVal ctrl As Control)
        ScriptManager.RegisterStartupScript(Me, Me.[GetType](), "FocusScript", "setTimeout(function(){$get('" + ctrl.ClientID + "').focus();}, 100);", True)
    End Sub

    Public Sub RebindViewFault()
        RadRow = HighlightRow()
        BindConcessionGrid()

    End Sub

    Protected Sub ConcessionGrid_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles ConcessionGrid.RowDataBound
        Select Case ParentControl
            Case "1", "4", "5"
                Dim nincid As String = ""
                If e.Row.RowType = DataControlRowType.DataRow Then
                    Dim SelectRow As String = e.Row.Cells(0).Text.ToString
                    If RadRow.Rows.Count > 0 Then
                        For Each dataRow As DataRow In RadRow.Rows
                            nincid = dataRow("incidentID")
                            If SelectRow = nincid Then
                                e.Row.BackColor = Color.FromName("#E56E94")
                            End If
                        Next
                    End If
                End If
        End Select
    End Sub
    Protected Function HighlightRow() As DataTable
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim connectionString1 As String = ConfigurationManager.ConnectionStrings("connectionstring").ConnectionString
        conn = New SqlConnection(connectionString1)
        comm = New SqlCommand("SELECT r.IncidentID From RadAckFault r Left outer join concessiontable c on r.Incidentid = c.Incidentid where r.Acknowledge = 'false' and linac=@linac", conn)
        comm.Parameters.AddWithValue("@linac", LinacName)
        conn.Open()
        Dim da As New SqlDataAdapter(comm)
        Dim dt As New DataTable()
        da.Fill(dt)
        Return dt

    End Function

    Protected Sub RaiseError()
        Dim strScript As String = "<script>"
        strScript += "alert('Problem Updating Fault. Please call Engineer');"
        strScript += "</script>"
        ScriptManager.RegisterStartupScript(Hidefaults, Me.GetType(), "JSCR", strScript.ToString(), False)
    End Sub

End Class
