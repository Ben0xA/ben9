Public Module ben9listener

#Region " Private Variables "
    Private WithEvents srv As New server
#End Region

    Sub Main()
        Console.WriteLine("Starting server.")
        srv.Listen("localhost", 4343)
        Console.ReadLine()
    End Sub

    Private Sub srv_ServerEvent(ByVal EventType As server.ServerEventType, ByVal Message As String) Handles srv.ServerEvent
        Select Case EventType
            Case server.ServerEventType.MessageReceived
                Console.WriteLine(Message)
            Case Else
                Console.WriteLine("[" & EventType.ToString & "] " & Message)
        End Select
    End Sub
End Module
