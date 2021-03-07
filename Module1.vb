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

    Public Sub InsertIntoPostgre(ByVal values() As String)
        Dim dayDate As Date = Date.Parse(values(0))
        Dim total As String = values(1) ' tady je to Decimal s tečkou ale posílá se v odbc s čárkou, takže to nefunguje v paramatrech.
        Dim dayCons As String = values(2) ' tady je to Decimal s tečkou ale posílá se v odbc s čárkou, takže to nefunguje v parametrech

        Dim status As String
        Dim connection As Odbc.OdbcConnection = New Odbc.OdbcConnection("DSN=HerokuDB")
        connection.Open()
        'System.Console.WriteLine("State: " + connection.State.ToString())
        status = connection.State.ToString()

        'Insert into table day_cons
        Dim cmd As New Odbc.OdbcCommand("INSERT INTO day_cons (item_date, total, day_cons) VALUES (?," & total & "," & dayCons & ")", connection) 'V parametrech dělají problém čárky ve floating typech
        cmd.Parameters.Add("item_date", Odbc.OdbcType.Date).Value = dayDate
        'cmd.Parameters.Add("total", Odbc.OdbcType.Decimal).Value = Val(total)
        'cmd.Parameters.Add("day_cons", Odbc.OdbcType.Decimal).Value = Val(dayCons)
        cmd.ExecuteNonQuery()
        cmd.Dispose()

        'Refresh materialized view day_MA7 (calculating 7 day MA)
        Dim refresh As New Odbc.OdbcCommand("REFRESH MATERIALIZED VIEW CONCURRENTLY day_ma7", connection)
        refresh.ExecuteNonQuery()
        refresh.Dispose()
        connection.Close()

    End Sub

    Public Function GetAPIdata(unixTime As Double) As String
        Dim endPoint As String = "https://api.visionq.cz/device_data.php?eui=0901288000016926&from=" & unixTime.ToString
        Dim webClient As New Net.WebClient
        Dim cc As System.Net.CredentialCache = New System.Net.CredentialCache
        cc.Add(New Uri(endPoint), "Basic", New System.Net.NetworkCredential("libor.knappek@gmail.com", "Liborknappek1+"))
        webClient.Credentials = cc
        Dim result As String = webClient.DownloadString(endPoint)
        Return result
    End Function

    Public Function CreateConsumptionTable() As DataTable
        Dim table As New DataTable
        table.Columns.Add("DateTime", GetType(Date))
        table.Columns.Add("Count", GetType(Decimal))
        table.Columns.Add("Source", GetType(Integer))
        Return table
    End Function

    Public Function CalculateConsumption(table As DataTable, metric As Integer) As DataTable
        'Select only low or hi data
        Dim Cons As DataTable = table.Select("[Source]='" + metric.ToString + "'").CopyToDataTable()
        Dim outputTable As DataTable = Cons.Clone()
        outputTable.Columns.Add("DailyCons", GetType(Decimal))


        'Calculate daily consumption
        Dim firstTick As Decimal = Cons.Rows(0).Item("Count") 'First datapoint
        Dim lastTick As Decimal = Cons.Rows(Cons.Rows.Count - 1).Item("Count") 'Last Datapoint

        For i As Integer = 0 To Cons.Rows.Count - 2
            Dim Date1 As Date = Cons.Rows(i)("DateTime") 'V jednom výrazu to nejede- nechápu proč- je to objekt a asi se to musí napřed naparsovat na datum, pokud se používá v kombi výrazu- na dim date se to převede samo asi
            Dim Date1Str As String = Date1.ToString("dd-MM-yyyy") 'ditto výše

            Dim Date2 As Date = Cons.Rows(i + 1)("DateTime") 'ditto výše
            Dim Date2Str As String = Date2.ToString("dd-MM-yyyy") 'ditto výše
            If Not Date1Str = Date2Str Then 'ditto výše
                Dim dayCons As Decimal = Cons.Rows(i).Item("Count") - firstTick
                Dim dayDate As Date = Date.Parse(Date1Str)
                firstTick = Cons.Rows(i).Item("Count")
                outputTable.Rows.Add(dayDate, firstTick, metric, dayCons)
            End If
        Next i

        Dim lastDate As Date = Cons.Rows(Cons.Rows.Count - 1)("DateTime") 'ditto výše
        outputTable.Rows.Add(Date.Parse(lastDate.ToString("dd-MM-yyyy")), lastTick, metric, lastTick - firstTick)


        Return outputTable
    End Function

    Public Sub ShowGraph()
        'decide on showing a graph

        Dim decide As Integer = MessageBox.Show("Show graph?", "Results", MessageBoxButtons.YesNo)
        If decide = 7 Then Return '7 is button value for "No"

        'Show Graph
        GraphForm.Show()
    End Sub
End Module
