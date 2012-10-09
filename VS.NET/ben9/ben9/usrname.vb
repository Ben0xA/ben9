Public Module usrname
    Declare Function GetUserName Lib "advapi32.dll" Alias "GetUserNameA" (ByVal lpBuffer As String, ByRef nSize As Integer) As Integer

    Public Function RetrieveUserName() As String
        Try
            Dim RetVal As Integer
            Dim UserName As String
            Dim Buffer As String
            Buffer = New String(CChar(" "), 25)
            RetVal = GetUserName(Buffer, 25)
            UserName = Strings.Left(Buffer, InStr(Buffer, Chr(0)) - 1)
            RetrieveUserName = UserName
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Module
