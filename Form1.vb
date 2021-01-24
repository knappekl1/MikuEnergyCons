Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class Form1
    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        'get Today
        Dim ActualDate As Date = Now.Date
        tbDate.Text = ActualDate.ToShortDateString


        'Get last day before today from database
        Dim query As String = "SELECT * FROM last_date"
        Dim outputVal(,) As String

        Dim response As DataTable = GetDBdata(query)
        ReDim outputVal(response.Rows.Count - 1, response.Columns.Count - 1)
        For j As Integer = 0 To response.Rows.Count - 1
            For i As Integer = 0 To response.Columns.Count - 1
                outputVal(j, i) = response.Rows(j).Item(i).ToString
            Next
        Next
        Dim lastDBdate As Date = response.Rows(0).Item(0)

        ' Get API Data
        tbConsumption.Text = response.Rows(0).Item(0)
        Dim targetDate As Date = lastDBdate.AddDays(-1)
        Dim unixDate As Double = (targetDate.ToUniversalTime - Date.Parse("1970-01-01 00:00:00")).TotalSeconds
        Dim APIresponse As String = GetAPIdata(unixDate)

        'Deserialize json
        Dim APIjson As JObject = JsonConvert.DeserializeObject(APIresponse)
        Dim keyVal As JToken
        Dim tokenValue As Decimal

        'Save values into Datatable
        Dim ConsumptionTable As DataTable = CreateConsumptionTable() 'Create table via function


        For Each row In APIjson("data").ToList 'convert JSON obj to list

            keyVal = row("timestamp") ' get and save value from list
            tokenValue = DirectCast(keyVal, JValue).Value
            Dim newDate As Date = Date.Parse("1970-01-01 00:00:00").AddSeconds(keyVal).ToLocalTime

            keyVal = row("value") ' get and save value from list
            tokenValue = DirectCast(keyVal, JValue).Value
            Dim cons As Decimal = keyVal

            keyVal = row("metric") ' get and save value from list
            tokenValue = DirectCast(keyVal, JValue).Value
            Dim metric As Integer = keyVal

            ConsumptionTable.Rows.Add(newDate, cons, metric) 'Add extracted values to Datatable
        Next row

        'Process Table

        Dim hiConsTable As DataTable = CalculateConsumption(ConsumptionTable, 1)
        Dim lowConsTable As DataTable = CalculateConsumption(ConsumptionTable, 2)


    End Sub
End Class
