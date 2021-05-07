Imports AjaxControlToolkit
Imports System.Web.UI.Page
Partial Class ConfirmPage
    Inherits System.Web.UI.UserControl
    Private Action As String
    Dim modalpopupextendercom As New ModalPopupExtender
    Public Event ConfirmExit()

    Protected Sub page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim MyString As String
        MyString = "ModalPopupextenderconfirm"
        modalpopupextendercom.ID = MyString
        modalpopupextendercom.BehaviorID = MyString
        modalpopupextendercom.TargetControlID = "label1"
        modalpopupextendercom.PopupControlID = "Panel1"
        modalpopupextendercom.BackgroundCssClass = "modalBackground"
        PlaceHolder1.Controls.Add(modalpopupextendercom)
        modalpopupextendercom.Show()

    End Sub
    Protected Sub btnchkcancel_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnchkcancel.Click
        Dim cptrl As ConfirmPage = CType(Me.Parent.FindControl("ConfirmPage1"), ConfirmPage)
        If cptrl IsNot Nothing Then
            cptrl.Visible = False
        End If
    End Sub

    Protected Sub AcceptOK_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AcceptOK.Click

        Dim cptrl As ConfirmPage = CType(Me.Parent.FindControl("ConfirmPage1"), ConfirmPage)
        If cptrl IsNot Nothing Then
            cptrl.Visible = False
        End If
        RaiseEvent ConfirmExit()

    End Sub
End Class
