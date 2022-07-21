Imports System.IO
Imports dll_KUKA_ParseModuleFile
Imports dll_KUKA_ParseModuleFile.KUKA

Public Class frmMain
    Private robot As Robot
    Private notepadPlusPath As String
    Private scanned As Boolean = False
    Private Archive As OpenArchive
    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.lastFile = txtArchive.Text
        My.Settings.AutoOpen = ToolStripMenuItemAutoOpen.Checked
        My.Settings.AutoScan = ToolStripMenuItemAutoScanAll.Checked
        My.Settings.UseNotepadPlus = UseNotepadToolStripMenuItem.Checked
        My.Settings.NotepadPlusPath = notepadPlusPath
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        txtArchive.Text = My.Settings.lastFile
        ToolStripMenuItemAutoOpen.Checked = My.Settings.AutoOpen
        ToolStripMenuItemAutoScanAll.Checked = My.Settings.AutoScan
        UseNotepadToolStripMenuItem.Checked = My.Settings.UseNotepadPlus
        notepadPlusPath = My.Settings.NotepadPlusPath
        'ElementHost1.Child = New dll_KUKA_ParseModuleFile.RobotControl
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        OpenFileDialog1.FileName = txtArchive.Text
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            txtArchive.Text = OpenFileDialog1.FileName
            If ToolStripMenuItemAutoOpen.Checked Then btnOpen_Click(Me, Nothing)
        End If
    End Sub

    Private Sub btnDirectory_Click(sender As Object, e As EventArgs) Handles btnDirectory.Click
        Try
            FolderBrowserDialog1.SelectedPath = Path.GetDirectoryName(txtArchive.Text)
        Catch ex As Exception
            FolderBrowserDialog1.SelectedPath = ""
        End Try
        If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            txtArchive.Text = FolderBrowserDialog1.SelectedPath
            If ToolStripMenuItemAutoOpen.Checked Then btnOpen_Click(Me, Nothing)
        End If
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        dll_WarningHelper.WarningHelper.ResetCounter()
        Archive = New OpenArchive(txtArchive.Text)
        lbFiles.Items.Clear()
        For Each item As String In Archive.fileList

            lbFiles.Items.Add(item)
        Next
        If ToolStripMenuItemAutoScanAll.Checked Then
            btnSelectAll_Click(Me, Nothing)
            btnScan_Click(Me, Nothing)
        End If
    End Sub

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        For i As Integer = 0 To lbFiles.Items.Count - 1
            lbFiles.SetSelected(i, True)
        Next i
    End Sub

    Private Sub btnScan_Click(sender As Object, e As EventArgs) Handles btnScan.Click
        btnA01.Enabled = False
        btnA02.Enabled = False
        btnA03.Enabled = False
        btnA04.Enabled = False
        'dgvSummary.SuspendLayout()
        'dgvWarnings.SuspendLayout()
        'dgvSummary.Rows.Clear()
        'dgvWarnings.Rows.Clear()

        Dim list As List(Of String) = New List(Of String)
        For Each item As String In lbFiles.SelectedItems
            list.Add(item)
        Next

        'robot = New Robot(Archive.tempFolder, list)

        'For Each item As KeyValuePair(Of String, KUKA.Program) In robot.Programs
        '    Dim bi As ProgramBaseInfo = item.Value.BaseInfo
        '    Dim colllist As List(Of String) = New List(Of String)
        '    For Each colitem As KeyValuePair(Of Integer, String) In bi.collList
        '        colllist.Add("[" & colitem.Key.ToString & ":""" & colitem.Value & """]")
        '    Next

        '    Dim areaList As List(Of String) = New List(Of String)
        '    For Each areaitem As KeyValuePair(Of Integer, String) In bi.areaList
        '        areaList.Add("[" & areaitem.Key.ToString & ":""" & areaitem.Value & """]")
        '    Next
        '    'dgvSummary.Rows.Add(Path.GetFileName(item.Key), String.Join(", ", bi.jobList), bi.jobListOK, String.Join(", ", areaList), bi.areaListOK, String.Join(", ", bi.plcList), bi.plclistOK, String.Join(", ", colllist), bi.colllistOK, String.Join(", ", bi.homeList), bi.homeListOK)
        'Next

        'For Each warning As dll_WarningHelper.Warning In robot.Warnings
        'Dim warning As dll_WarningHelper.Warning = warn
        'If warning.Type = 101 Then
        'dgvWarnings.Rows.Add(warning.ID, Path.GetFileName(warning.File), warning.Line + 1, warning.Topic, warning.Text, warning.extraText, warning.File, warning.Type)
        'ElseIf warning.Type = 103 Then
        'dgvWarnings.Rows.Add(warn.Key, Path.GetFileName(warning.File), warning.Line + 1, warning.Topic, warning.Text, warning.extraText, Path.ChangeExtension(warning.File, "dat"), warning.Type)
        'End If
        'Next

        btnA01.Enabled = robot.Appl.A01_Plc
        btnA02.Enabled = robot.Appl.A02_Tch
        btnA03.Enabled = robot.Appl.A03_Grp
        btnA04.Enabled = robot.Appl.A04_Swp

        scanned = True

        'RobotControl.myContent.Data = robot
        'RobotMain1.UseNotepadPP = My.Settings.UseNotepadPlus
        'RobotMain1.PathNotepadPP = notepadPlusPath
        RobotMain1.myData = robot
        'dgvSummary.ResumeLayout()
        'dgvWarnings.ResumeLayout()
    End Sub

    Private Sub lbFiles_DoubleClick(sender As Object, e As EventArgs) Handles lbFiles.DoubleClick
        If lbFiles.SelectedIndex < 0 Then Exit Sub
        Clipboard.SetText(lbFiles.SelectedItem)
        Dim p As New System.Diagnostics.Process
        Dim s As System.Diagnostics.ProcessStartInfo
        If My.Settings.UseNotepadPlus Then
            s = New System.Diagnostics.ProcessStartInfo(notepadPlusPath, """" & Path.Combine(Archive.tempFolder, lbFiles.SelectedItem) & """")
        Else
            s = New System.Diagnostics.ProcessStartInfo(Path.Combine(Archive.tempFolder, lbFiles.SelectedItem))
        End If
        s.UseShellExecute = True
        s.WindowStyle = ProcessWindowStyle.Normal
        p.StartInfo = s
        p.Start()
    End Sub

    Private Sub SelectPathToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectPathToolStripMenuItem.Click
        OpenFileDialog2.FileName = notepadPlusPath
        If OpenFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            notepadPlusPath = OpenFileDialog2.FileName
            My.Settings.NotepadPlusPath = notepadPlusPath
        End If
    End Sub

    Private Sub UseNotepadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UseNotepadToolStripMenuItem.Click
        My.Settings.UseNotepadPlus = UseNotepadToolStripMenuItem.Checked
    End Sub

    '    Private Sub dgvWarnings_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
    '        If e.RowIndex < 0 Then Exit Sub
    '        Dim row As DataGridViewRow = dgvWarnings.Rows(e.RowIndex)
    '#If DEBUG Then
    '        If e.ColumnIndex = 0 Then
    '            dll_WarningHelper.WarningHelper.stop_at_warning = row.Cells(0).Value
    '            MsgBox("Next stop at warning " & dll_WarningHelper.WarningHelper.stop_at_warning.ToString)
    '        End If
    '#End If
    '        If row IsNot Nothing Then
    '            Dim p As New System.Diagnostics.Process
    '            Dim s As System.Diagnostics.ProcessStartInfo
    '            If My.Settings.UseNotepadPlus Then
    '                s = New System.Diagnostics.ProcessStartInfo(notepadPlusPath, """" & Path.Combine(Archive.tempFolder, row.Cells("xFilename").Value) & """ -n" & (row.Cells("xLine").Value).ToString)
    '            Else
    '                s = New System.Diagnostics.ProcessStartInfo(Path.Combine(Archive.tempFolder, lbFiles.SelectedItem))
    '            End If
    '            s.UseShellExecute = True
    '            s.WindowStyle = ProcessWindowStyle.Normal
    '            p.StartInfo = s
    '            p.Start()
    '        End If
    '    End Sub

    Private Sub btnA01_Click(sender As Object, e As EventArgs) Handles btnA01.Click
        Dim window As frmA01PlcAndRobot = New frmA01PlcAndRobot(robot)
        window.ShowDialog()
    End Sub

    Private Sub btnA02_Click(sender As Object, e As EventArgs) Handles btnA02.Click
        Dim window As frmA02Tch = New frmA02Tch(robot)
        window.ShowDialog()
    End Sub
    Private Sub btnA03_Click(sender As Object, e As EventArgs) Handles btnA03.Click
        Dim window As frmA03Gripper = New frmA03Gripper(robot)
        window.ShowDialog()
    End Sub

    Private Sub btnA04_Click(sender As Object, e As EventArgs) Handles btnA04.Click
        Dim window As frmA04Swp = New frmA04Swp(robot)
        window.ShowDialog()
    End Sub

    Private Sub PathEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PathEditorToolStripMenuItem.Click
        If Not scanned Then
            MsgBox("Please scan all elements first.", MsgBoxStyle.Information)
            Return
        End If
        If lbFiles.SelectedIndex < 0 Then Exit Sub
        If Not robot.Programs.HasProgram(lbFiles.SelectedItem) Then Exit Sub
        Dim path As Dictionary(Of Integer, Object) = KUKA.PathFold.ToPathFold(robot.Programs.ByName(lbFiles.SelectedItem).Parser.src.folds)
        Dim frm As frmPathEditor = New frmPathEditor(path)
        frm.ShowDialog()
    End Sub

    Private Sub DatEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DatEditorToolStripMenuItem.Click
        If Not scanned Then
            MsgBox("Please scan all elements first.", MsgBoxStyle.Information)
            Return
        End If
        If lbFiles.SelectedIndex < 0 Then Exit Sub
        'If Not lbFiles.SelectedItem.ToString.ToLowerInvariant.EndsWith(".dat") Then Exit Sub
        If Not robot.Programs.HasProgram(lbFiles.SelectedItem.ToString) Then Exit Sub
        Dim window As frmDatEditor = New frmDatEditor(robot.Programs.ByName(lbFiles.SelectedItem.ToString))
        window.ShowDialog()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim window As frmMADAConfig = New frmMADAConfig(robot)
        window.ShowDialog()
    End Sub

    Private Sub ThAxisHelperToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ThAxisHelperToolStripMenuItem.Click
        Dim window As frm7AxisHelper = New frm7AxisHelper()
        window.ShowDialog()
    End Sub
End Class
