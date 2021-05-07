Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Data
Imports System.Configuration
'http://www.mikesdotnetting.com/Article/80/Create-PDFs-in-ASP.NET-getting-started-with-iTextSharp
'Imports iTextSharp.text

'Imports iTextSharp.text.pdf

'Imports iTextSharp.text.html

'Imports iTextSharp.text.html.simpleparser

Partial Class ViewCommentsuc
    Inherits System.Web.UI.UserControl
    Private MachineName As String
    Private CommentType As String

    Public Property LinacName() As String
        Get
            Return MachineName
        End Get
        Set(ByVal value As String)
            MachineName = value
        End Set
    End Property
    Public Property CommentSort() As String
        Get
            Return CommentType
        End Get
        Set(ByVal value As String)
            CommentType = value
        End Set
    End Property


    Protected Sub submitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles submitButton.Click
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        'have to have validate on date fields
        'Reads start and stop date from page
        'If RequiredFieldValidatorstart.IsValid And RequiredFieldValidatorstop.IsValid Then
        Page.Validate("CommentDates")
        If Page.IsValid Then
            If UpdatePanel1.Visible Then
                submitButton.Text = "View History"
                UpdatePanel1.Visible = False
                txtStartDate.Text = ""
                StopBox1.Text = ""

            Else

                Dim StartDate As DateTime = DateTime.Parse(txtStartDate.Text)
                Dim StopDate As DateTime = DateTime.Parse(StopBox1.Text)


                'Dims new dataset and the sql adapter
                Dim dataset As New DataSet
                Dim adapter As SqlDataAdapter

                'Dim Intent As String = DropDownList1.Text.ToString


                StopDate = StopDate.AddDays(1)

                Dim connectionString As String = ConfigurationManager.ConnectionStrings(
                "connectionstring").ConnectionString

                conn = New SqlConnection(connectionString)
                'Open connection to the database
                Select Case CommentType
                    Case "pm"
                        comm = New SqlCommand("Select * from AuxTable where linac=@linac and LogOutDate between @StartDate and @StopDate and Tab=4", conn)
                    Case "rp"
                        comm = New SqlCommand("Select * from AuxTable where linac=@linac and LogOutDate between @StartDate and @StopDate and Tab=5", conn)
                    Case "er"
                        comm = New SqlCommand("Select * from HandoverEnergies where linac=@linac and LogOutDate between @StartDate and @StopDate", conn)
                    Case "pcr"
                        comm = New SqlCommand("Select * from ClinicalHandover where linac=@linac and LogOutDate between @StartDate and @StopDate", conn)
                    Case "pqa"
                        comm = New SqlCommand("Select * from AuxTable where linac=@linac and LogOutDate between @StartDate and @StopDate and Tab=6", conn)

                End Select
                comm.Parameters.Add("@Linac", Data.SqlDbType.NVarChar)
                comm.Parameters("@linac").Value = MachineName
                comm.Parameters.AddWithValue("@StartDate", StartDate.ToString("dd MMM yyyy"))

                comm.Parameters.AddWithValue("@StopDate", StopDate.ToString("dd MMM yyyy"))
                'comm.Parameters.AddWithValue("@EndWeek", EndWeek.ToString("dd MMM yyyy"))

                adapter = New SqlDataAdapter(comm)
                adapter.Fill(dataset, "referrals")

                GridView1.DataSource = dataset
                GridView1.DataBind()
                UpdatePanel1.Visible = True
                submitButton.Text = "Hide History"

            End If
        End If



    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim csname1 As String = "datecheck"
        Dim cstype As Type = Me.GetType()

        ' Get a ClientScriptManager reference from the Page class. 
        Dim cs As ClientScriptManager = Page.ClientScript


        If Not IsPostBack Then
            UpdatePanel1.Visible = False

            ' Check to see if the client script is already registered. 
            If (Not cs.IsClientScriptBlockRegistered(cstype, csname1)) Then

                Dim cstext2 As New StringBuilder()
                cstext2.Append("<script type=""text/javascript""> function DoClick(sender, args) {")
                cstext2.Append("if (sender._selectedDate > new Date()) {")
                cstext2.Append("alert('You cannot select a day later than today!');")
                cstext2.Append("sender._selectedDate = new Date();")
                'set the date back to the current date
                cstext2.Append("sender._textbox.set_Value(sender._selectedDate.format(sender._format))}} </")
                cstext2.Append("script>")
                cs.RegisterClientScriptBlock(cstype, csname1, cstext2.ToString())

            End If

        End If


    End Sub
End Class
