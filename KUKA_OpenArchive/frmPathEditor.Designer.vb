<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPathEditor
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
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.UcDataGrid1 = New dll_KUKA_ParseModuleFile.KUKA.ucDataGrid()
        Me.SuspendLayout()
        '
        'Timer1
        '
        '
        'UcDataGrid1
        '
        Me.UcDataGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UcDataGrid1.Location = New System.Drawing.Point(0, 0)
        Me.UcDataGrid1.Name = "UcDataGrid1"
        Me.UcDataGrid1.Size = New System.Drawing.Size(956, 548)
        Me.UcDataGrid1.TabIndex = 2
        '
        'frmPathEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(956, 548)
        Me.Controls.Add(Me.UcDataGrid1)
        Me.Name = "frmPathEditor"
        Me.Text = "Path Editor"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents UcDataGrid1 As dll_KUKA_ParseModuleFile.KUKA.ucDataGrid
End Class
