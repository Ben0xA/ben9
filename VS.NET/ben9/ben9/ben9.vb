Public Module ben9

#Region " Private Variables "
    Private WithEvents clt As New client
#End Region

    Sub Main()
        clt.Connect("127.0.0.1", 4343)
        System.Threading.Thread.Sleep(200)
    End Sub

    Private Sub clt_ClientEvent(ByVal EventType As client.ClientEventType, ByVal Message As String) Handles clt.ClientEvent
        Console.WriteLine("[" & EventType.ToString & "] " & Message)
    End Sub
End Module
