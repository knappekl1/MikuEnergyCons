Module Module1
    Public Function GetDBdata(query As String) As DataTable
        Dim status As String
        Dim connection As Odbc.OdbcConnection = New Odbc.OdbcConnection("DSN=HerokuDB")
        connection.Open()
        System.Console.WriteLine("State: " + connection.State.ToString())
        status = connection.State.ToString()
        Dim command As Odbc.OdbcCommand = New Odbc.OdbcCommand(query, connection)
        Dim reader As Odbc.OdbcDataReader = command.ExecuteReader(CommandBehavior.CloseConnection)
        Dim results As New DataTable
        results.Load(reader)

        reader.Close()
        connection.Close()
        Return results
    End Function

    Public Function GetAPIdata(unixTime As Double) As String
        Dim endPoint As String = "https://api.visionq.cz/device_data.php?eui=0901288000016926&from=" & unixTime.ToString
        Dim webClient As New Net.WebClient
        Dim cc As System.Net.CredentialCache = New System.Net.CredentialCache
        cc.Add(New Uri(endPoint), "Basic", New System.Net.NetworkCredential("libor.knappek@gmail.com", "Liborknappek1+"))
        webClient.Credentials = cc
        Dim result As String = webClient.DownloadString(endPoint)
        Return result
    End Function

End Module
