Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Net.Mail


Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim userIP As String = Request.UserHostAddress

        Dim bc As HttpBrowserCapabilities = Request.Browser()
        Dim browserType As String = bc.Browser.ToString()
        If browserType = "Firefox" Then
            Response.Write("<p>ELF does not work with Firefox. Please open with Internet Explorer, Edge or Chrome</p>")
        Else

            If String.IsNullOrEmpty(Request.QueryString("machinekey")) Then

                Response.Redirect("NonMachinepage.aspx")
            Else
                Dim machinepage As String = Request.QueryString("machinekey")
                Dim returnstring As String = machinepage + "page.aspx?loadup=1"
                Dim loadedstring As String = machinepage + "loaded"

                Select Case machinepage
                    Case "LA1", "LA2", "LA3", "LA4", "E1", "E2", "B1", "B2", "T1", "T2", "T3"

                        Response.Redirect(returnstring)
                    Case Else
                        Response.Redirect("NonMachinepage.aspx")
                End Select

            End If
        End If
    End Sub

End Class
