<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.txtArchive = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.btnOpen = New System.Windows.Forms.Button()
        Me.btnDirectory = New System.Windows.Forms.Button()
        Me.lbFiles = New System.Windows.Forms.ListBox()
        Me.cms_lbFiles = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PathEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DatEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnSelectAll = New System.Windows.Forms.Button()
        Me.btnScan = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemAutoOpen = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemAutoScanAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.NotepadOptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UseNotepadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectPathToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ThAxisHelperToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.btnA01 = New System.Windows.Forms.Button()
        Me.btnA02 = New System.Windows.Forms.Button()
        Me.btnA03 = New System.Windows.Forms.Button()
        Me.btnA04 = New System.Windows.Forms.Button()
        Me.ElementHost2 = New System.Windows.Forms.Integration.ElementHost()
        Me.RobotMain1 = New dll_KUKA_ParseModuleFile.KUKA.RobotMain()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.OpenFileDialog2 = New System.Windows.Forms.OpenFileDialog()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.cms_lbFiles.SuspendLayout()
        Me.TableLayoutPanel3.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.FlowLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.GroupBox1, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.lbFiles, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.TableLayoutPanel3, 0, 4)
        Me.TableLayoutPanel1.Controls.Add(Me.MenuStrip1, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.FlowLayoutPanel1, 1, 5)
        Me.TableLayoutPanel1.Controls.Add(Me.ElementHost2, 1, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 6
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(1044, 576)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'GroupBox1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.GroupBox1, 3)
        Me.GroupBox1.Controls.Add(Me.TableLayoutPanel2)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(3, 27)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(1038, 80)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Select Archive:"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.txtArchive, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnBrowse, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnOpen, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.btnDirectory, 2, 0)
        Me.TableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(3, 16)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(1032, 61)
        Me.TableLayoutPanel2.TabIndex = 0
        '
        'txtArchive
        '
        Me.txtArchive.Dock = System.Windows.Forms.DockStyle.Fill
        Me.txtArchive.Location = New System.Drawing.Point(3, 3)
        Me.txtArchive.MinimumSize = New System.Drawing.Size(4, 20)
        Me.txtArchive.Multiline = True
        Me.txtArchive.Name = "txtArchive"
        Me.txtArchive.Size = New System.Drawing.Size(826, 24)
        Me.txtArchive.TabIndex = 0
        '
        'btnBrowse
        '
        Me.btnBrowse.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnBrowse.Location = New System.Drawing.Point(835, 3)
        Me.btnBrowse.MinimumSize = New System.Drawing.Size(0, 20)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(94, 24)
        Me.btnBrowse.TabIndex = 1
        Me.btnBrowse.Text = "Archive..."
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'btnOpen
        '
        Me.TableLayoutPanel2.SetColumnSpan(Me.btnOpen, 3)
        Me.btnOpen.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOpen.Location = New System.Drawing.Point(3, 33)
        Me.btnOpen.MinimumSize = New System.Drawing.Size(0, 20)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(1026, 25)
        Me.btnOpen.TabIndex = 2
        Me.btnOpen.Text = "Open"
        Me.btnOpen.UseVisualStyleBackColor = True
        '
        'btnDirectory
        '
        Me.btnDirectory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnDirectory.Location = New System.Drawing.Point(935, 3)
        Me.btnDirectory.MinimumSize = New System.Drawing.Size(0, 20)
        Me.btnDirectory.Name = "btnDirectory"
        Me.btnDirectory.Size = New System.Drawing.Size(94, 24)
        Me.btnDirectory.TabIndex = 1
        Me.btnDirectory.Text = "Directory..."
        Me.btnDirectory.UseVisualStyleBackColor = True
        '
        'lbFiles
        '
        Me.lbFiles.ContextMenuStrip = Me.cms_lbFiles
        Me.lbFiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lbFiles.FormattingEnabled = True
        Me.lbFiles.Location = New System.Drawing.Point(3, 113)
        Me.lbFiles.Name = "lbFiles"
        Me.TableLayoutPanel1.SetRowSpan(Me.lbFiles, 2)
        Me.lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.lbFiles.Size = New System.Drawing.Size(202, 424)
        Me.lbFiles.TabIndex = 2
        '
        'cms_lbFiles
        '
        Me.cms_lbFiles.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PathEditorToolStripMenuItem, Me.DatEditorToolStripMenuItem})
        Me.cms_lbFiles.Name = "cms_lbFiles"
        Me.cms_lbFiles.Size = New System.Drawing.Size(133, 48)
        '
        'PathEditorToolStripMenuItem
        '
        Me.PathEditorToolStripMenuItem.Name = "PathEditorToolStripMenuItem"
        Me.PathEditorToolStripMenuItem.Size = New System.Drawing.Size(132, 22)
        Me.PathEditorToolStripMenuItem.Text = "Path Editor"
        '
        'DatEditorToolStripMenuItem
        '
        Me.DatEditorToolStripMenuItem.Name = "DatEditorToolStripMenuItem"
        Me.DatEditorToolStripMenuItem.Size = New System.Drawing.Size(132, 22)
        Me.DatEditorToolStripMenuItem.Text = "Dat Editor"
        '
        'TableLayoutPanel3
        '
        Me.TableLayoutPanel3.AutoSize = True
        Me.TableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.TableLayoutPanel3.ColumnCount = 2
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel3.Controls.Add(Me.btnSelectAll, 0, 0)
        Me.TableLayoutPanel3.Controls.Add(Me.btnScan, 1, 0)
        Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel3.Location = New System.Drawing.Point(3, 543)
        Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
        Me.TableLayoutPanel3.RowCount = 1
        Me.TableLayoutPanel1.SetRowSpan(Me.TableLayoutPanel3, 2)
        Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel3.Size = New System.Drawing.Size(202, 30)
        Me.TableLayoutPanel3.TabIndex = 3
        '
        'btnSelectAll
        '
        Me.btnSelectAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectAll.Location = New System.Drawing.Point(3, 3)
        Me.btnSelectAll.Name = "btnSelectAll"
        Me.btnSelectAll.Size = New System.Drawing.Size(95, 23)
        Me.btnSelectAll.TabIndex = 0
        Me.btnSelectAll.Text = "Select All"
        Me.btnSelectAll.UseVisualStyleBackColor = True
        '
        'btnScan
        '
        Me.btnScan.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnScan.Location = New System.Drawing.Point(104, 3)
        Me.btnScan.Name = "btnScan"
        Me.btnScan.Size = New System.Drawing.Size(95, 23)
        Me.btnScan.TabIndex = 1
        Me.btnScan.Text = "Scan selected"
        Me.btnScan.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.MenuStrip1, 3)
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OptionsToolStripMenuItem, Me.ToolsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1044, 24)
        Me.MenuStrip1.TabIndex = 9
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItemAutoOpen, Me.ToolStripMenuItemAutoScanAll, Me.NotepadOptionsToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(61, 20)
        Me.OptionsToolStripMenuItem.Text = "&Options"
        '
        'ToolStripMenuItemAutoOpen
        '
        Me.ToolStripMenuItemAutoOpen.CheckOnClick = True
        Me.ToolStripMenuItemAutoOpen.Name = "ToolStripMenuItemAutoOpen"
        Me.ToolStripMenuItemAutoOpen.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemAutoOpen.Size = New System.Drawing.Size(217, 22)
        Me.ToolStripMenuItemAutoOpen.Text = "&Open Automaticly"
        '
        'ToolStripMenuItemAutoScanAll
        '
        Me.ToolStripMenuItemAutoScanAll.CheckOnClick = True
        Me.ToolStripMenuItemAutoScanAll.Name = "ToolStripMenuItemAutoScanAll"
        Me.ToolStripMenuItemAutoScanAll.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S), System.Windows.Forms.Keys)
        Me.ToolStripMenuItemAutoScanAll.Size = New System.Drawing.Size(217, 22)
        Me.ToolStripMenuItemAutoScanAll.Text = "Auto &Scan All Items"
        '
        'NotepadOptionsToolStripMenuItem
        '
        Me.NotepadOptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UseNotepadToolStripMenuItem, Me.SelectPathToolStripMenuItem})
        Me.NotepadOptionsToolStripMenuItem.Name = "NotepadOptionsToolStripMenuItem"
        Me.NotepadOptionsToolStripMenuItem.Size = New System.Drawing.Size(217, 22)
        Me.NotepadOptionsToolStripMenuItem.Text = "Notepad++ Options"
        '
        'UseNotepadToolStripMenuItem
        '
        Me.UseNotepadToolStripMenuItem.CheckOnClick = True
        Me.UseNotepadToolStripMenuItem.Name = "UseNotepadToolStripMenuItem"
        Me.UseNotepadToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.UseNotepadToolStripMenuItem.Text = "Use Notepad++"
        '
        'SelectPathToolStripMenuItem
        '
        Me.SelectPathToolStripMenuItem.Name = "SelectPathToolStripMenuItem"
        Me.SelectPathToolStripMenuItem.Size = New System.Drawing.Size(158, 22)
        Me.SelectPathToolStripMenuItem.Text = "Select Path..."
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ThAxisHelperToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(48, 20)
        Me.ToolsToolStripMenuItem.Text = "&Tools"
        '
        'ThAxisHelperToolStripMenuItem
        '
        Me.ThAxisHelperToolStripMenuItem.Name = "ThAxisHelperToolStripMenuItem"
        Me.ThAxisHelperToolStripMenuItem.Size = New System.Drawing.Size(153, 22)
        Me.ThAxisHelperToolStripMenuItem.Text = "7th Axis Helper"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.FlowLayoutPanel1, 2)
        Me.FlowLayoutPanel1.Controls.Add(Me.Button1)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnA01)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnA02)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnA03)
        Me.FlowLayoutPanel1.Controls.Add(Me.btnA04)
        Me.FlowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(211, 543)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(830, 30)
        Me.FlowLayoutPanel1.TabIndex = 11
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(3, 3)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(149, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "MADA"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnA01
        '
        Me.btnA01.Enabled = False
        Me.btnA01.Location = New System.Drawing.Point(158, 3)
        Me.btnA01.Name = "btnA01"
        Me.btnA01.Size = New System.Drawing.Size(150, 23)
        Me.btnA01.TabIndex = 3
        Me.btnA01.Text = "Robot + (A01) PLC Config"
        Me.btnA01.UseVisualStyleBackColor = True
        '
        'btnA02
        '
        Me.btnA02.Enabled = False
        Me.btnA02.Location = New System.Drawing.Point(314, 3)
        Me.btnA02.Name = "btnA02"
        Me.btnA02.Size = New System.Drawing.Size(150, 23)
        Me.btnA02.TabIndex = 1
        Me.btnA02.Text = "(A02) ToolChanger Config"
        Me.btnA02.UseVisualStyleBackColor = True
        '
        'btnA03
        '
        Me.btnA03.Enabled = False
        Me.btnA03.Location = New System.Drawing.Point(470, 3)
        Me.btnA03.Name = "btnA03"
        Me.btnA03.Size = New System.Drawing.Size(150, 23)
        Me.btnA03.TabIndex = 0
        Me.btnA03.Text = "(A03) Gripper Config"
        Me.btnA03.UseVisualStyleBackColor = True
        '
        'btnA04
        '
        Me.btnA04.Enabled = False
        Me.btnA04.Location = New System.Drawing.Point(626, 3)
        Me.btnA04.Name = "btnA04"
        Me.btnA04.Size = New System.Drawing.Size(150, 23)
        Me.btnA04.TabIndex = 2
        Me.btnA04.Text = "(A04) SpotWeld Config"
        Me.btnA04.UseVisualStyleBackColor = True
        '
        'ElementHost2
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.ElementHost2, 2)
        Me.ElementHost2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ElementHost2.Location = New System.Drawing.Point(211, 113)
        Me.ElementHost2.Name = "ElementHost2"
        Me.TableLayoutPanel1.SetRowSpan(Me.ElementHost2, 2)
        Me.ElementHost2.Size = New System.Drawing.Size(830, 424)
        Me.ElementHost2.TabIndex = 12
        Me.ElementHost2.Text = "ElementHost2"
        Me.ElementHost2.Child = Me.RobotMain1
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.DefaultExt = "zip"
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        Me.OpenFileDialog1.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*"
        '
        'OpenFileDialog2
        '
        Me.OpenFileDialog2.DefaultExt = "exe"
        Me.OpenFileDialog2.FileName = "OpenFileDialog2"
        Me.OpenFileDialog2.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1044, 576)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "frmMain"
        Me.Text = "KUKA OpenArchive"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.cms_lbFiles.ResumeLayout(False)
        Me.TableLayoutPanel3.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.FlowLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents txtArchive As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents btnOpen As System.Windows.Forms.Button
    Friend WithEvents lbFiles As System.Windows.Forms.ListBox
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnDirectory As System.Windows.Forms.Button
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents TableLayoutPanel3 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnSelectAll As System.Windows.Forms.Button
    Friend WithEvents btnScan As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemAutoOpen As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemAutoScanAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NotepadOptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UseNotepadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SelectPathToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OpenFileDialog2 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents FlowLayoutPanel1 As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnA01 As System.Windows.Forms.Button
    Friend WithEvents btnA02 As System.Windows.Forms.Button
    Friend WithEvents btnA03 As System.Windows.Forms.Button
    Friend WithEvents btnA04 As System.Windows.Forms.Button
    Friend WithEvents cms_lbFiles As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents PathEditorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents DatEditorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ToolsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ThAxisHelperToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ElementHost2 As System.Windows.Forms.Integration.ElementHost
    Friend RobotMain1 As dll_KUKA_ParseModuleFile.KUKA.RobotMain

End Class
