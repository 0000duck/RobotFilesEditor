Imports System.Windows.Forms
Public Class MainWindow
    Private _myContext As New MyData
    Declare Sub Inject Lib "InjectLib.dll" (ByRef FileName_ As String)
    Private Sub About_Click(sender As Object, e As RoutedEventArgs)
        Dim Window As Window = New WindowAbout() With {
    .ShowInTaskbar = False,
    .Topmost = True,
    .ResizeMode = ResizeMode.NoResize,
    .Owner = Application.Current.MainWindow
        }
        Window.ShowDialog()
    End Sub

    Private Sub Close_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
    End Sub

    Private Sub Browse_Click(sender As Object, e As RoutedEventArgs)
        Dim dlg = New FolderBrowserDialog() With {.SelectedPath = My.Settings.backupPath}
        Dim result As System.Windows.Forms.DialogResult = dlg.ShowDialog(Me.GetIWin32Window())
        If result = Forms.DialogResult.OK Then
            _myContext.BackupPath = dlg.SelectedPath
            _myContext.ScanForBackups()
            _myContext.GoToStep(2)
        End If
    End Sub

    Private Sub FindBackups_Click(sender As Object, e As RoutedEventArgs)
        _myContext.ScanForBackups()
        _myContext.GoToStep(2)
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DataContext = _myContext

    End Sub

    Private Sub SelectNone_Click(sender As Object, e As RoutedEventArgs)
        For Each item As BackupItem In _myContext.BackupList
            item.Selected = False
        Next
    End Sub

    Private Sub SelectAll_Click(sender As Object, e As RoutedEventArgs)
        For Each item As BackupItem In _myContext.BackupList
            item.Selected = True
        Next
    End Sub

    Private Sub LoadBackups_Click(sender As Object, e As RoutedEventArgs)
        _myContext.LoadSelectedBackups()
        _myContext.GoToStep(3)
    End Sub

    Private Sub SelectNoneLoads_Click(sender As Object, e As RoutedEventArgs)
        For Each item As MyRobot In _myContext.LoadProject
            For Each Load As LoadingCase In item.LoadCases
                Load.Selected = False
            Next
        Next
    End Sub

    Private Sub SelectAllLoads_Click(sender As Object, e As RoutedEventArgs)
        For Each item As MyRobot In _myContext.LoadProject
            For Each Load As LoadingCase In item.LoadCases
                Load.Selected = True
            Next
        Next
    End Sub

    Private Sub GetList_Click(sender As Object, e As RoutedEventArgs)
        _myContext.GoToStep(4)
    End Sub

    Private Sub Connect_Click(sender As Object, e As RoutedEventArgs)
        Try
            If _myContext.ConnectAndGetList() Then
                _myContext.AssignRobotsToList()
                _myContext.GoToStep(5)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    Private Sub GenerateXML_Click(sender As Object, e As RoutedEventArgs)
        If _myContext.SaveProject Then
            _myContext.GoToStep(6)
        End If
    End Sub

    Private Sub Connect2Step_Click(sender As Object, e As RoutedEventArgs)
        If _myContext.ConnectAndPrepareProject() Then
            _myContext.GoToStep(7)
        End If
    End Sub

    Protected Overrides Sub Finalize()
        My.Settings.Save()
        MyBase.Finalize()
    End Sub
End Class
