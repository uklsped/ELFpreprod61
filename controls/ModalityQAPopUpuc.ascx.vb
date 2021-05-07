Imports AjaxControlToolkit
Imports DavesCode
Partial Class controls_ModalityQAPopUpuc
    Inherits System.Web.UI.UserControl
    Dim modalpopupextenderrf As New ModalPopupExtender
    Public Property ParentControl() As String
    Public Property LinacName() As String
    'Private ConcessParamsTrial As ConcessionParameters = New ConcessionParameters()

    Public Event CloseModalityQAPopUpTab(ByVal Linac As String)

    Protected Sub Page_load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim logerrorbox As Label = FindControl("LoginErrordetails")
        logerrorbox.Text = Nothing

        Dim objQA As Modalitiesuc = Page.LoadControl("controls\Modalitiesuc.ascx")
        objQA.ID = "QAModality"
        CType(objQA, Modalitiesuc).LinacName = LinacName
        CType(objQA, Modalitiesuc).TabName = ParentControl
        AddHandler objQA.CloseModalityQAPopup, AddressOf Close_ModalityQAPopUp
        PlaceHolderModalities.Controls.Add(objQA)

        Dim MyString As String
        Dim Tabnumber As String
        MyString = "ModalPopupextenderQA"
        Tabnumber = ParentControl
        MyString = MyString & Tabnumber
        modalpopupextenderrf.ID = MyString
        modalpopupextenderrf.BehaviorID = MyString
        modalpopupextenderrf.TargetControlID = "Label1"
        modalpopupextenderrf.PopupControlID = "Panel1"
        modalpopupextenderrf.BackgroundCssClass = "modalBackground"
        PlaceHolder1.Controls.Add(modalpopupextenderrf)
        modalpopupextenderrf.Show()

    End Sub
    Protected Sub Close_ModalityQAPopUp(ByVal Linac As String)
        If LinacName = Linac Then
            RaiseEvent CloseModalityQAPopUpTab(Linac)
        End If
    End Sub

End Class
