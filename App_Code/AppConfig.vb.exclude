Imports System.Xml

Public Class AppConfig
        Implements IConfigurationSectionHandler
        Private Shared E1Status As String

        Public Shared ReadOnly Property E1State() As String
            Get
                Return E1Status
            End Get

        End Property
        Public Shared Sub Init()

        'System.Configuration.ConfigurationSettings.GetConfig("AppConfig")

    End Sub
    Public Function Create(parent As Object, configContext As Object, section As XmlNode) As Object Implements IConfigurationSectionHandler.Create
            Dim nv As NameValueCollection
            Dim sh As NameValueSectionHandler
            sh = New NameValueSectionHandler()
            nv = CType(sh.Create(parent, configContext, section), NameValueCollection)
            E1Status = nv.Item("E1State")
            Throw New NotImplementedException()
        End Function
    End Class

