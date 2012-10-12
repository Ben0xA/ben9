Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class server

#Region " Private Variables "
    Private srvr As TcpListener
    Private clnt As TcpClient
    Private strm As NetworkStream
    Private host As String
    Private port As Integer
    Private msg As String
    Private running As Boolean = False
#End Region

#Region " Private WithEvents Variables "
    Private WithEvents tmrStream As New System.Timers.Timer
#End Region

#Region " Public Events "
    Public Event ServerEvent(ByVal EventTYpe As ServerEventType, ByVal Message As String)
#End Region

#Region " Public Enum "
    Public Enum ServerEventType
        Listening = 0
        ConnectionEstablished
        StreamReceived
        MessageReceived
        ConnectionClosed
        ServerStopped
        SystemError
        Waiting
    End Enum
#End Region

#Region " Private Methods "
    Private Sub srvListen()
        Try
            RaiseEvent ServerEvent(ServerEventType.Listening, "Service started on " & host & " on port " & port & "...")
            srvr = New TcpListener(IPAddress.Parse(host), port)
            srvr.Start()
            tmrStream.Interval = 100 ' 1 Second
            Console.WriteLine("[*]Server started.")
            Do While Running
                srvWaitForConnection()
            Loop
        Catch iex As IO.IOException
            srvr.Server.Disconnect(True)
            srvr.Stop()
            srvr = Nothing
            RaiseEvent ServerEvent(ServerEventType.ServerStopped, "")
        Catch ex As Exception
            RaiseEvent ServerEvent(ServerEventType.SystemError, ex.Message)
        End Try
    End Sub

    Private Sub srvWaitForConnection()
        Try
            RaiseEvent ServerEvent(ServerEventType.Listening, "Listening...")
            clnt = srvr.AcceptTcpClient
            If clnt.Connected Then
                RaiseEvent ServerEvent(ServerEventType.ConnectionEstablished, CType(clnt.Client.RemoteEndPoint, Net.IPEndPoint).Address.ToString)
                srvWaitForStream()
            End If
        Catch iex As IO.IOException
            strm.Close()
            clnt.Close()
            srvr = Nothing
            clnt = Nothing
            RaiseEvent ServerEvent(ServerEventType.ConnectionClosed, "")
        Catch ex As Exception
            RaiseEvent ServerEvent(ServerEventType.SystemError, ex.Message)
        End Try
    End Sub

    Private Sub srvWaitForStream()
        Try
            strm = clnt.GetStream
            tmrStream.Enabled = True
        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
    End Sub
#End Region

#Region " Private Events "
    Private Sub tmrStream_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles tmrStream.Elapsed
        Try
            Dim BytesRead As Integer = -1
            If running And clnt.Connected Then
                Try
                    Dim Bytes(clnt.ReceiveBufferSize) As Byte
                    If strm.CanRead And strm.DataAvailable Then
                        tmrStream.Enabled = False
                        BytesRead = strm.Read(Bytes, 0, CInt(clnt.ReceiveBufferSize))
                        Do While BytesRead > 0
                            msg &= Encoding.ASCII.GetString(Bytes)

                            'Clear Byte array otherwise it will retain largest message
                            Array.Clear(Bytes, 0, Bytes.Length)
                            ReDim Bytes(-1)

                            'Redimension Byte array
                            ReDim Bytes(clnt.ReceiveBufferSize)
                            If strm.CanRead And strm.DataAvailable Then
                                BytesRead = strm.Read(Bytes, 0, CInt(clnt.ReceiveBufferSize))
                            Else
                                BytesRead = -1
                            End If
                        Loop

                        msg = msg.Replace(Chr(0), "")
                        msg = Now.ToString("MM/dd/yyyy hh:mm:ss") & "|" & clnt.Client.RemoteEndPoint.ToString & "|" & msg
                        If Not msg = "" Then
                            RaiseEvent ServerEvent(ServerEventType.MessageReceived, msg)
                        End If
                        msg = ""
                        tmrStream.Enabled = True
                    End If
                Catch iex As IO.IOException
                    strm.Close()
                End Try
            Else
                tmrStream.Enabled = False
                strm.Close()
                clnt.Close()
                strm = Nothing
                clnt = Nothing
                RaiseEvent ServerEvent(ServerEventType.ConnectionClosed, "")
            End If
        Catch ex As Exception
            RaiseEvent ServerEvent(ServerEventType.SystemError, ex.Message)
        End Try
    End Sub
#End Region

#Region " Public Methods "
    Public Sub Listen(Optional ByVal HostIP As String = "127.0.0.1", Optional ByVal HostPort As Integer = 4343)
        host = HostIP
        port = HostPort
        running = True
        srvListen()
    End Sub
#End Region

End Class
