Public Class GraphForm
    Private Sub GraphForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Get graph data from DB
        Dim query As String = "SELECT item_date, day_cons, sum FROM running_sum"
        Dim GraphData As DataTable = GetDBdata(query)
        Console.WriteLine(GraphData.Rows.Count)

        'Insert into Series
        ChartDayCons.Series.Clear()
        Dim Series1 = New DataVisualization.Charting.Series With {
                .Name = "DayCons",
                .Color = System.Drawing.Color.Blue,
                .IsVisibleInLegend = False,
                .IsXValueIndexed = True,
                .ChartType = DataVisualization.Charting.SeriesChartType.Line
            }
        ChartDayCons.Series.Add(Series1)

        For Each row As DataRow In GraphData.Rows
            Series1.Points.AddXY(row.Item("item_date"), row.Item("day_cons"))
        Next row



    End Sub
End Class