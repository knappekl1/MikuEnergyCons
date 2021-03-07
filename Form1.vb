Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class lbOutput
    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        'get Today

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

        'Check if any data to process
        tbConsumption.Text = response.Rows(0).Item(0)
        If (Today - lastDBdate).TotalDays < 2 Then
            MessageBox.Show("No New data to process, DB already updated")
            ShowGraph()
            Return
        End If

        ' Get API Data
        Dim targetDate As Date = lastDBdate 'The date can be moved if needed by AddDays(-1) method
        Dim unixDate As Double = (targetDate.ToUniversalTime - Date.Parse("1970-01-01 00:00:00")).TotalSeconds
        Dim APIresponse As String = GetAPIdata(unixDate) 'function returns json string

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

        Dim hiConsTable As DataTable = CalculateConsumption(ConsumptionTable, 1) 'Processes high rate consumption
        Dim lowConsTable As DataTable = CalculateConsumption(ConsumptionTable, 2) 'Processes low rate consumption

        'Join results on Date
        Dim results = From t1 In hiConsTable.AsEnumerable() Join t2 In lowConsTable.AsEnumerable() On (t1("DateTime").ToString) Equals (t2("DateTime").ToString)
                      Select New With {.Date = t1.Field(Of Date)("DateTime"),
                        .Count = t1.Field(Of Decimal)("Count") + t2.Field(Of Decimal)("Count"),
                         .DayCons = t1.Field(Of Decimal)("DailyCons") + t2.Field(Of Decimal)("DailyCons")}

        'Create table to Assign result into
        Dim OutputTable As New DataTable()
        OutputTable.Columns.Add("Date", GetType(Date))
        OutputTable.Columns.Add("Count", GetType(Decimal))
        OutputTable.Columns.Add("DayCons", GetType(Decimal))

        For Each row In results 'Add results to Output Table
            OutputTable.Rows.Add(row.Date, row.Count, row.DayCons)
        Next row

        'Remove first row from Outputs (as it formed only base for calculation and is not needed)
        OutputTable.Rows.RemoveAt(0)

        'Remove last Date if today (as it is only partial consumption as shall not be included in the database)
        Dim lastDay As Date = OutputTable.Rows(OutputTable.Rows.Count - 1)("Date") 'Just in case the object cannot be parsed to date directly in If below
        If Today = lastDay Then
            OutputTable.Rows.RemoveAt(OutputTable.Rows.Count - 1)
        End If

        dgvOutput.DataSource = OutputTable 'Show result in form (UI)


        'Insert data into DB (Postgresql on Heroku)
        Dim values(2) As String
        For Each row As DataRow In OutputTable.Rows
            values(0) = Date.Parse(row.Item("Date")).ToString("yyyy-MM-dd")
            Dim temp As Decimal = row.Item("Count")
            values(1) = temp.ToString("F3", Globalization.CultureInfo.CreateSpecificCulture("en-US")) ' je to jedno, udělá se tam čárka až když se to posílá do Postgresql
            temp = row.Item("DayCons")
            values(2) = temp.ToString("F2", Globalization.CultureInfo.CreateSpecificCulture("en-US")) ' je to jedno, udělá se tam čárka až když se to posílá do Postgresql
            InsertIntoPostgre(values)
        Next row

        ShowGraph()


    End Sub


End Class
