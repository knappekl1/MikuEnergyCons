Public Class GraphForm
    Private Sub GraphForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Get graph data from DB
        Dim query As String = "SELECT item_date, day_cons, ma7 FROM day_ma7 ORDER BY item_date"
        Dim GraphData As DataTable = GetDBdata(query)
        Console.WriteLine(GraphData.Rows.Count)

        'Insert into Series
        ChartDayCons.Series.Clear()
        'Chart day consumption values
        Dim Series1 = New DataVisualization.Charting.Series With {
                .Name = "DayCons",
                .Color = System.Drawing.Color.Blue,
                .IsVisibleInLegend = False,
                .IsXValueIndexed = True,
                .ChartType = DataVisualization.Charting.SeriesChartType.Line,
                .BorderWidth = 2
            }
        ChartDayCons.Series.Add(Series1)

        For Each row As DataRow In GraphData.Rows
            Series1.Points.AddXY(row.Item("item_date"), row.Item("day_cons"))
        Next row

        'Chart moving average
        Dim Series2 = New DataVisualization.Charting.Series With {
                .Name = "MA_7",
                .Color = System.Drawing.Color.Red,
                .IsVisibleInLegend = False,
                .IsXValueIndexed = True,
                .ChartType = DataVisualization.Charting.SeriesChartType.Line
            }
        ChartDayCons.Series.Add(Series2)

        For Each row As DataRow In GraphData.Rows
            Series2.Points.AddXY(row.Item("item_date"), row.Item("ma7"))
        Next row

    End Sub
End Class