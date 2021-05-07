
Partial Class controls_ConcessionHistoryuc
    Inherits System.Web.UI.UserControl

    Public Sub BindConcessionHistoryGrid(ByVal incidentID As String)
        Dim SqlDataSource1 As New SqlDataSource With {
            .ID = "SqlDataSource1",
            .ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString,
            .SelectCommand = "select t.TrackingID, t.incidentid, t.action, t.trackingcomment, t.AssignedTo, t.Status, t.LastUpDatedBy, t.LastUpDatedOn, ConcessionNumber  " _
        & "from FaultTracking t left outer join ConcessionTable c on c.incidentID=t.incidentID where t.incidentID=@incidentID order by t.TrackingID asc"
        }

        SqlDataSource1.SelectParameters.Add("@incidentID", System.Data.SqlDbType.Int)
        SqlDataSource1.SelectParameters.Add("incidentID", incidentID)
        ConcessionHistoryGridView.DataSource = SqlDataSource1
        ConcessionHistoryGridView.DataBind()
    End Sub
    Protected Sub ConcessionHistoryGridView_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles ConcessionHistoryGridView.PageIndexChanging

        Dim IncidentID As String
        IncidentID = ConcessionHistoryGridView.DataKeys(0).Value.ToString
        ConcessionHistoryGridView.PageIndex = e.NewPageIndex
        BindConcessionHistoryGrid(IncidentID)

    End Sub
End Class
