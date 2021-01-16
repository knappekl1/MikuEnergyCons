Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class Form1
    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        'get Today
        Dim ActualDate As Date = Now.Date
        tbDate.Text = ActualDate.ToShortDateString


        'Get last day before today from database
        Dim query As String = "SELECT * FROM last_date"
        Dim outputVal(-1, -1) As String

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
        For Each row In APIjson("data").ToList
            keyVal = row("timestamp")
            tokenValue = DirectCast(keyVal, JValue).Value
            'DODĚLAT    ZDE
        Next row
        Dim test As String = APIjson("data")(0)("timestamp").ToString





    End Sub
End Class
