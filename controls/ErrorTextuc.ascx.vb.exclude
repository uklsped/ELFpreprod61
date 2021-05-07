Imports System.IO

Partial Class controls_ErrorText
    Inherits System.Web.UI.UserControl

    Protected Sub BtnRead_Click(sender As Object, e As EventArgs)


        Dim path As String = fileupload1.PostedFile.FileName
        If Not (String.IsNullOrEmpty(path)) Then

            Dim readText() As String = File.ReadAllLines(path)
            Dim strbuild As StringBuilder = New StringBuilder()
            For Each value As String In readText

                strbuild.Append(value)
                strbuild.AppendLine()

                textBoxContents.Text = strbuild.ToString()
            Next
        End If
    End Sub


End Class
