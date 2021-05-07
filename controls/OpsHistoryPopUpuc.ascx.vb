Imports AjaxControlToolkit
Imports DavesCode
Partial Class controls_OpsHistoryPopUpuc
    Inherits System.Web.UI.UserControl
    Dim modalpopupextenderops As New ModalPopupExtender
    Public Property ParentControl() As String
    Public Property LinacName() As String
    'Private ConcessParamsTrial As ConcessionParameters = New ConcessionParameters()

    Public Event CloseOpsHistoryPopUpTab(ByVal Linac As String)

    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim logerrorbox As Label = FindControl("LoginErrordetails")
        logerrorbox.Text = Nothing

        'Dim objQA As Modalitiesuc = Page.LoadControl("controls\Modalitiesuc.ascx")
        'objQA.ID = "QAModality"
        'CType(objQA, Modalitiesuc).LinacName = LinacName
        'CType(objQA, Modalitiesuc).TabName = ParentControl
        'AddHandler objQA.CloseModalityQAPopup, AddressOf Close_ModalityQAPopUp
        'PlaceHolderModalities.Controls.Add(objQA)

        Dim MyString As String
        Dim Tabnumber As String
        MyString = "ModalPopupextenderOps"
        Tabnumber = ParentControl
        MyString = MyString & Tabnumber
        modalpopupextenderops.ID = MyString
        modalpopupextenderops.BehaviorID = MyString
        modalpopupextenderops.TargetControlID = "Label1"
        modalpopupextenderops.PopupControlID = "Panel1"
        modalpopupextenderops.BackgroundCssClass = "modalBackground"
        PlaceHolder1.Controls.Add(modalpopupextenderops)
        modalpopupextenderops.Show()

    End Sub
    Protected Sub Close_ModalityQAPopUp(ByVal Linac As String)
        If LinacName = Linac Then
            RaiseEvent CloseOpsHistoryPopUpTab(Linac)
        End If
    End Sub

End Class
