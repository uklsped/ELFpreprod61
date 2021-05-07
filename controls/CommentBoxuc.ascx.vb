
Partial Class controls_CommentBoxuc
    Inherits UserControl
    Public Property Currentcomment() As String
    Public Property BoxChanged() As String
    Public Property MaxCount() As Integer = 150
    Public Property TextWidth As Integer = 345
    Public Property TextHeight As Integer = 120
    Public Property NoWrite As Boolean = False

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        TextBox.Width = Unit.Pixel(TextWidth)
        TextBox.Height = Unit.Pixel(TextHeight)
        'https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.clientscriptmanager?view=netframework-4.7.2
        'https://www.codeproject.com/Questions/125501/asp-net-object-Attributes-add-Method

        SetCount()

        If Not IsPostBack Then

            If Not HttpContext.Current.Application(BoxChanged) Is Nothing Then
                    Currentcomment = HttpContext.Current.Application(BoxChanged).ToString
                    TextBox.Text = Currentcomment

                End If

        End If
        If NoWrite Then
            TextBox.ReadOnly = True
            TextBox.ForeColor = Drawing.Color.DarkBlue
            TextBox.BackColor = Drawing.SystemColors.Window
            CommentWordCount.Text = String.Empty
        Else
            CommentWordCount.Text = MaxCount - Currentcomment.Length
        End If
        Currentcomment = TextBox.Text

    End Sub
    Public Sub SetCount()
        TextBox.Attributes.Add("onKeyUp", String.Format("Count(""{0}"", ""{1}"", ""{2}"")", TextBox.ClientID, CommentWordCount.ClientID, MaxCount))

        Dim csname2 As String = "TextBoxScript"
        Dim cstype As Type = Me.GetType()
        'Get a ClientScriptManager reference from the Page class.
        Dim cs As ClientScriptManager = Page.ClientScript
        If Not cs.IsClientScriptBlockRegistered(cstype, csname2) Then

            Dim cstext2 As StringBuilder = New StringBuilder()
            cstext2.Append("function Count(TextID, CountID, MaxCount) {")
            cstext2.Append("var Maxcom = parseInt(MaxCount) + 1;")
            cstext2.Append("var txt = document.getElementById(TextID);")
            cstext2.Append("var i = txt.value.length;")
            cstext2.Append("if (i < Maxcom) {document.getElementById(CountID).innerHTML = MaxCount - i;} ")
            cstext2.Append("else {document.getElementById(CountID).innerHTML = ""You have exceeded the max text please delete some characters"";} ")
            cstext2.Append("return;}")
            cs.RegisterClientScriptBlock(cstype, csname2, cstext2.ToString(), True)

        End If
        Currentcomment = TextBox.Text
        Application(BoxChanged) = Currentcomment
    End Sub
    Public Sub TextBox_TextChanged(sender As Object, e As EventArgs) Handles TextBox.TextChanged

        Currentcomment = TextBox.Text
            Application(BoxChanged) = Currentcomment
        'DavesCode.Reuse.ReturnApplicationState(BoxChanged)
    End Sub

    Public Sub ResetCommentBox(ByVal Comments As String)

        Currentcomment = Comments
        Application(BoxChanged) = Comments
        TextBox.Text = Currentcomment

        CommentWordCount.Text = MaxCount

    End Sub
    Public Sub SetValidation(ByVal ValidateGroup As String, ByVal ErrorMessage As String)
        CommentValidation.ValidationGroup = ValidateGroup
        CommentValidation.ErrorMessage = ErrorMessage
    End Sub

End Class
