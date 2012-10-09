Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class client

#Region " Private Variables "
    Private clnt As TcpClient
    Private strm As NetworkStream
    Private host As String
    Private port As Integer
    Private rspn As String
#End Region

#Region " Public Enum "
    Public Enum ClientEventType
        Connecting = 0
        ConnectionEstablished
        SentMessage
        StreamReceived
        MessageReceived
        ClosingConnection
        ConnectionClosed
        Waiting
        SystemError
    End Enum
#End Region

#Region " Public Events "
    Public Event ClientEvent(ByVal EventType As ClientEventType, ByVal Message As String)
#End Region

#Region " Private Methods "
    Private Sub cltConnect()
        Try
            RaiseEvent ClientEvent(ClientEventType.Connecting, "Attempting to connect.")
            clnt = New TcpClient
            clnt.Connect(host, port)
            RaiseEvent ClientEvent(ClientEventType.ConnectionEstablished, "")
            cltWaitForStream()
        Catch ex As Exception
            RaiseEvent ClientEvent(ClientEventType.ConnectionClosed, ex.Message)
        End Try
    End Sub

    Private Sub cltWaitForStream()
        Try
            strm = clnt.GetStream()
            RaiseEvent ClientEvent(ClientEventType.StreamReceived, "")
            cltTransmit()
        Catch ex As Exception
            RaiseEvent ClientEvent(ClientEventType.ConnectionClosed, ex.Message)
        End Try
    End Sub

    Private Sub cltTransmit()
        Try
            If strm.CanWrite And strm.CanRead Then
                Dim SendBytes() As Byte = Nothing

                Dim txmsg As String = Now.ToString("d/M/y hh:mm:ss") & "|" & My.User.Name & "|" & GetIPAddress() & "|" & Dns.GetHostName
                Console.WriteLine(txmsg)
                Console.ReadLine()
                ClearBuffer()
                SendBytes = Encoding.ASCII.GetBytes(txmsg)
                strm.Write(SendBytes, 0, SendBytes.Length)
                RaiseEvent ClientEvent(ClientEventType.SentMessage, "")
            End If
        Catch ex As Exception
            RaiseEvent ClientEvent(ClientEventType.SystemError, ex.Message)
        End Try
    End Sub

    Private Sub ClearBuffer()
        Try
            'Clear Out Buffer
            If strm.DataAvailable Then
                Dim Bytes(clnt.ReceiveBufferSize) As Byte
                strm.Read(Bytes, 0, CInt(clnt.ReceiveBufferSize))
                Bytes = Nothing
            End If
        Catch ex As Exception
            RaiseEvent ClientEvent(ClientEventType.SystemError, ex.Message)
        End Try
    End Sub
#End Region

#Region " Private Functions "
    Private Function Disconnected() As Boolean
        Dim rtn As Boolean = False
        Try
            If Not clnt Is Nothing Then
                If Not strm Is Nothing And Not clnt Is Nothing Then
                    Dim BytesRead As Integer = -1, Bytes() As Byte
                    If strm.CanWrite And strm.CanRead Then
                        Dim nullBytes() As Byte = Encoding.ASCII.GetBytes(Chr(0))
                        strm.Write(nullBytes, 0, nullBytes.Length)

                        ReDim Bytes(clnt.ReceiveBufferSize)
                        If strm.CanRead And strm.DataAvailable Then
                            BytesRead = strm.Read(Bytes, 0, CInt(clnt.ReceiveBufferSize))
                        Else
                            BytesRead = -1
                        End If
                    Else
                        rtn = True
                    End If
                Else
                    rtn = True
                End If
            Else
                rtn = True
            End If
        Catch ex As Exception
            rtn = True
        End Try
        Return rtn
    End Function

    Private Function adtWaitForResponse() As String
        Try
            Dim Bytes(clnt.ReceiveBufferSize) As Byte
            strm.Read(Bytes, 0, CInt(clnt.ReceiveBufferSize))
            rspn = Encoding.ASCII.GetString(Bytes)
            If rspn.Chars(0) = Nothing Then
                'Attempt to read again
                strm.Read(Bytes, 0, CInt(clnt.ReceiveBufferSize))
                rspn = Encoding.ASCII.GetString(Bytes)
            End If
            Return rspn
        Catch ex As Exception
            RaiseEvent ClientEvent(ClientEventType.SystemError, ex.Message)
            Return ""
        End Try
    End Function

    Private Function GetIPAddress() As String
        Try
            Dim ipEntry As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName)
            Dim ipAddr As IPAddress() = ipEntry.AddressList
            Dim ips As String = ""
            For Each ip As IPAddress In ipAddr
                ips &= ip.ToString & ","
            Next
            ips = ips.Substring(0, ips.Length - 1)
            Return ips
        Catch ex As Exception
            RaiseEvent ClientEvent(ClientEventType.SystemError, ex.Message)
            Return ""
        End Try
    End Function
#End Region

#Region " Public Methods "
    Public Sub Connect(Optional ByVal HostIP As String = "127.0.0.1", Optional ByVal HostPort As Integer = 4343)
        host = HostIP
        port = HostPort
        cltConnect()
    End Sub
#End Region

End Class
