<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GraphForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New System.Windows.Forms.DataVisualization.Charting.ChartArea()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GraphForm))
        Dim Title1 As System.Windows.Forms.DataVisualization.Charting.Title = New System.Windows.Forms.DataVisualization.Charting.Title()
        Me.ChartDayCons = New System.Windows.Forms.DataVisualization.Charting.Chart()
        CType(Me.ChartDayCons, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ChartDayCons
        '
        Me.ChartDayCons.CausesValidation = False
        ChartArea1.Name = "ChartArea1"
        Me.ChartDayCons.ChartAreas.Add(ChartArea1)
        resources.ApplyResources(Me.ChartDayCons, "ChartDayCons")
        Me.ChartDayCons.Name = "ChartDayCons"
        Title1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.0!)
        Title1.Name = "Title1"
        Title1.Text = "Day Consumption (kWh)"
        Me.ChartDayCons.Titles.Add(Title1)
        '
        'GraphForm
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.ChartDayCons)
        Me.MinimizeBox = False
        Me.Name = "GraphForm"
        CType(Me.ChartDayCons, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ChartDayCons As DataVisualization.Charting.Chart
End Class
