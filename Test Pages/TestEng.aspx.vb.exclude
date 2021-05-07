
Partial Class Test_Pages_TestEng
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim objcon As Object
        objcon = Page.LoadControl("~/controls/EngApproveuc.ascx")

        CType(objcon, controls_EngApproveuc).Device = "LA1"

        PlaceHolder1.Controls.Add(objcon)
    End Sub
End Class
