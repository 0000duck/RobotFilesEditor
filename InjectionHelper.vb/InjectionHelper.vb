Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Windows.Interop
Imports System.IO
Imports System.Text.RegularExpressions

Namespace InjectionHelper
    Public Class InjectionHelper
        Private _hWnd As IntPtr
        Private _hWndProject As IntPtr
        Private _returnData As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String)
        Private _projectTreeView As TreeView
        Private _projectNodes As Windows.Forms.TreeNodeCollection
        Private _loaddataGroupBox As Windows.Forms.GroupBox
        Private _payloadControl As Windows.Forms.Control
        Private _dynamicControl As Windows.Forms.Control
        Private _staticsTableLayoutPanel As Windows.Forms.TableLayoutPanel

        Public ReadOnly Property ReturnData As Dictionary(Of Integer, String)
            Get
                Return _returnData
            End Get
        End Property
        Public Sub New(hWnd As IntPtr)
            'Debugger.Launch()
            _hWnd = hWnd
        End Sub
        Public Sub New(hWndMain As IntPtr, hWndProject As IntPtr)
            'Debugger.Launch()
            _hWnd = hWndMain
            _hWndProject = hWndProject
        End Sub
        Friend Function GetTreeViewElems() As Boolean
            Dim window As System.Windows.Forms.Control = System.Windows.Forms.Control.FromHandle(_hWnd)
            'Dim win2 As Windows.Media.Visual = HwndSource.FromHwnd(_hWnd).RootVisual
            'Dim test As Windows.Forms.TreeView = window.RobottypeSelectorView.Controls(0).Controls(0).Controls(9).Controls(1).Nodes
            If window IsNot Nothing Then
                If window.GetType.Name <> "KukaLoadGUI" And Not window.GetType.FullName.StartsWith("KukaLoadGUI") Then
                    MsgBox("Wrong type of window: " & window.GetType.Name & ". Should be ""KukaLoadGUI"".")
                    Return False
                End If
                ' 1 type - Groupbox
                If window.Controls.Count < 17 And Not window.Controls.Count = 2 Then
                    MsgBox("Not enough controls in main window.")
                    Return False
                End If
                If window.Controls.Count > 2 Then
                    If window.Controls(14).GetType.Name <> "GroupBox" Then
                        MsgBox("15th control in mainwindow is not a GroupBox")
                        Return False
                    End If
                    Dim groupbox1 As Windows.Forms.GroupBox = window.Controls(14)
                    If groupbox1.Text <> "Robotertyp" And groupbox1.Text <> "Robot type" Then
                        MsgBox("15th control (GroupBox) has wrong name: " & groupbox1.Text & ". Should be ""Robotertyp"" or ""Robot type"".")
                        Return False
                    End If

                    ' 2. type - TreeView
                    If groupbox1.Controls.Count < 1 Then
                        MsgBox("Not enough controls in ""Robotertyp"" or ""Robot type"" GroupBox.")
                        Return False
                    End If

                    If groupbox1.Controls(0).GetType.Name <> "TreeView" Then
                        MsgBox("1st control in ""Robotertyp"" or ""Robot type"" groupbox is not a TreeView")
                        Return False
                    End If
                    Dim treeview As Windows.Forms.TreeView = groupbox1.Controls(0)
                    For Each node As TreeNode In treeview.Nodes
                        _returnData.Add(Convert.ToInt32(node.Tag), node.Text)
                    Next
                    Return True
                Else
                    Dim treeview2 As Windows.Forms.TreeView = window.Controls(0).Controls(5).Controls(0).Controls(0).Controls(9).Controls(1)
                    For Each node As TreeNode In treeview2.Nodes
                        _returnData.Add(Convert.ToInt32(node.Tag.ID), node.Text)
                    Next
                    Return True
                End If

                'ElseIf win2 IsNot Nothing Then
                '    Return True
            Else
                MsgBox("Window is Nothing, handle: " & _hWnd.ToString)
                Return False
            End If
        End Function

        Private Function PrepareProjectStep1() As Boolean
            Dim window As System.Windows.Forms.Control = System.Windows.Forms.Control.FromHandle(_hWndProject)
            If window IsNot Nothing Then
                If window.GetType.Name <> "RoboterProject" And window.GetType.Name <> "ProjectForm" Then
                    MsgBox("Wrong type of window: " & window.GetType.Name & ". Should be ""RoboterProject"".")
                    Return False
                End If
                ' 1. type - GroupBox no Text
                If window.Controls.Count < 4 Then
                    MsgBox("Not enough controls in main window.")
                    Return False
                End If
                If Not window.Controls(3).GetType.Name.ToLower().Contains("groupbox") Then
                    MsgBox("4th control in mainwindow is not a GroupBox")
                    Return False
                End If
                Dim groupbox1 As Windows.Forms.GroupBox = window.Controls(3)

                ' 2. type - GroupBox Robotertypen
                If groupbox1.Controls.Count < 6 Then
                    MsgBox("Not enough controls in main groupbox.")
                    Return False
                End If
                If groupbox1.Controls(5).GetType.Name <> "GroupBox" Then
                    MsgBox("3rd control in mainwindow is not a GroupBox")
                    Return False
                End If
                Dim groupbox2 As Windows.Forms.GroupBox = groupbox1.Controls(5)
                If groupbox2.Text <> "Robotertypen" And groupbox2.Text <> "Robot types" Then
                    MsgBox("6th control (GroupBox) has wrong name: " & groupbox2.Text & ". Should be ""Robotertypen"" or ""Robot types"".")
                    Return False
                End If

                ' 3. type - TreeView nodes.count > 0
                ' 3. type - GroupBox Lastfallübertragung auf KukaLoadGUI
                If groupbox2.Controls.Count < 10 Then
                    MsgBox("Not enough controls in Robotertypen/Robot types groupbox.")
                    Return False
                End If

                If (groupbox2.Controls.Count > 12) Then
                    If groupbox2.Controls(13).GetType.Name <> "TreeView" Then
                        MsgBox("7th control in Robotertypen/Robot types groupbox is not a TreeView")
                        Return False
                    End If
                ElseIf groupbox2.Controls(6).GetType.Name <> "TreeView" Then
                    MsgBox("7th control in Robotertypen/Robot types groupbox is not a TreeView")
                    Return False
                End If

                If (groupbox2.Controls(6).GetType.Name = "TreeView") Then
                    _projectTreeView = groupbox2.Controls(6)
                Else
                    _projectTreeView = groupbox2.Controls(13)
                End If
                If _projectTreeView.Nodes.Count < 1 Then
                    MsgBox("No items in treeview. Do you have loaded a project?")
                    Return False
                End If
                _projectTreeView.SelectedNode = _projectTreeView.Nodes(0)
                _projectNodes = _projectTreeView.Nodes

                If groupbox2.Controls.Count > 16 Then
                    If groupbox2.Controls(17).GetType.Name <> "GroupBox" Then
                        MsgBox("10th control in Robotertypen/Robot types groupbox is not a GroupBox")
                        Return False
                    End If
                ElseIf groupbox2.Controls(9).GetType.Name <> "GroupBox" Then
                    MsgBox("10th control in Robotertypen/Robot types groupbox is not a GroupBox")
                    Return False
                End If
                Dim groupbox3 As Windows.Forms.GroupBox
                If groupbox2.Controls(9).GetType.Name = "GroupBox" Then
                    groupbox3 = groupbox2.Controls(9)
                Else
                    groupbox3 = groupbox2.Controls(17)
                End If

                ' 4. type - CheckBox Projektverbindung
                If groupbox3.Controls.Count < 2 Then
                    MsgBox("Not enough controls in Lastfalluebertragung.../Transfer... groupbox.")
                    Return False
                End If
                If groupbox3.Controls(1).GetType.Name <> "CheckBox" Then
                    MsgBox("2nd control in Lastfalluebertragung.../Transfer... groupbox is not a CheckBox")
                    Return False
                End If
                Dim checkbox1 As Windows.Forms.CheckBox = groupbox3.Controls(1)
                Try
                    checkbox1.CheckState = CheckState.Checked
                Catch
                End Try
                'checkbox1.Checked = True
                'window.Controls(3).Controls(5).Controls(9).Controls(1)
            Else
                MsgBox("Window is Nothing, handle: " & _hWnd.ToString)
                Return False
            End If
            Return True
        End Function

        Private Function GetControlImage(ByVal ctl As Control) As Bitmap
            Dim bm As New Bitmap(ctl.Width, ctl.Height)
            ctl.DrawToBitmap(bm, New Rectangle(0, 0, ctl.Width, ctl.Height))
            Return bm
        End Function

        Private Function PrepareProjectStep2() As Boolean
            Dim window As System.Windows.Forms.Control = System.Windows.Forms.Control.FromHandle(_hWnd)
            'Debugger.Launch()
            If window IsNot Nothing Then
                If window.GetType.Name <> "KukaLoadGUI" And Not window.GetType.FullName.StartsWith("KukaLoadGUI") Then
                    MsgBox("Wrong type of window: " & window.GetType.Name & ". Should be ""KukaLoadGUI"".")
                    Return False
                End If
                ' 1 type - Groupbox
                If window.Controls.Count < 17 And Not window.Controls.Count = 2 Then
                    MsgBox("Not enough controls in main window.")
                    Return False
                End If
                If window.Controls.Count > 2 Then
                    If window.Controls(8).GetType.Name <> "GroupBox" Then
                        MsgBox("9th control in mainwindow is not a GroupBox")
                        Return False
                    End If
                    _loaddataGroupBox = window.Controls(8)

                    If window.Controls(3).GetType.Name <> "TabControl" Then
                        MsgBox("4th control in mainwindow is not a TabControl")
                        Return False
                    End If
                    Dim tabControl As TabControl = window.Controls(3)
                    If tabControl.TabCount < 4 Then
                        MsgBox("Not enough tabs in in TabControl")
                        Return False
                    End If

                    Dim payloadTabPage As TabPage = tabControl.TabPages(0)
                    Dim dynamicTabPage As TabPage = tabControl.TabPages(3)

                    If payloadTabPage.Controls.Count < 1 Then
                        MsgBox("Not enough controls in in payload TabPage")
                        Return False
                    End If
                    _payloadControl = payloadTabPage.Controls(0)

                    If dynamicTabPage.Controls.Count < 1 Then
                        MsgBox("Not enough controls in in dynamic TabPage")
                        Return False
                    End If
                    _dynamicControl = dynamicTabPage.Controls(0)

                    If window.Controls(16).GetType.Name <> "GroupBox" Then
                        MsgBox("17th control in mainwindow is not a GroupBox")
                        Return False
                    End If
                    Dim groupbox1 As GroupBox = window.Controls(16)

                    If groupbox1.Controls.Count < 1 Then
                        MsgBox("Not enough controls in BelastungsAnalyse parent GroupBox")
                        Return False
                    End If
                    If groupbox1.Controls(0).GetType.Name <> "GroupBox" Then
                        MsgBox("1st control in BelastungsAnalyse parent GroupBox is not a GroupBox")
                        Return False
                    End If
                    Dim groupbox2 As GroupBox = groupbox1.Controls(0)

                    If groupbox2.Controls.Count < 1 Then
                        MsgBox("Not enough controls in BelastungsAnalyse GroupBox")
                        Return False
                    End If
                    If groupbox2.Controls(0).GetType.Name <> "GroupBox" Then
                        MsgBox("1st control in BelastungsAnalyse GroupBox is not a GroupBox")
                        Return False
                    End If
                    Dim groupbox3 As GroupBox = groupbox2.Controls(0)

                    If groupbox3.Controls.Count < 1 Then
                        MsgBox("Not enough controls in Statikauswetrung GroupBox")
                        Return False
                    End If
                    If groupbox3.Controls(0).GetType.Name <> "TableLayoutPanel" Then
                        MsgBox("1st control in Statikauswetrung GroupBox is not a TableLayoutPanel")
                        Return False
                    End If
                    _staticsTableLayoutPanel = groupbox3.Controls(0)
                    Return True
                Else
                    If window.Controls(0).Controls(1).Controls(0).GetType.Name <> "GroupBox" Then
                        MsgBox("9th control in mainwindow is not a GroupBox")
                        Return False
                    End If
                    _loaddataGroupBox = window.Controls(0).Controls(1).Controls(0)
                    If window.Controls(0).Controls(4).Controls(1).Controls(0).GetType.Name <> "TabControl" Then
                        MsgBox("4th control in mainwindow is not a TabControl")
                        Return False
                    End If
                    Dim tabControl As TabControl = window.Controls(0).Controls(4).Controls(1).Controls(0)
                    If tabControl.TabCount < 4 Then
                        MsgBox("Not enough tabs in in TabControl")
                        Return False
                    End If

                    Dim payloadTabPage As TabPage = tabControl.TabPages(0)
                    Dim dynamicTabPage As TabPage = tabControl.TabPages(3)

                    If payloadTabPage.Controls.Count < 1 Then
                        MsgBox("Not enough controls in in payload TabPage")
                        Return False
                    End If
                    _payloadControl = payloadTabPage.Controls(0)

                    If dynamicTabPage.Controls.Count < 1 Then
                        MsgBox("Not enough controls in in dynamic TabPage")
                        Return False
                    End If
                    _dynamicControl = dynamicTabPage.Controls(0)

                    If window.Controls(0).Controls(3).Controls(0).Controls(0).GetType.Name <> "GroupBox" Then
                        MsgBox("17th control in mainwindow is not a GroupBox")
                        Return False
                    End If
                    Dim groupbox1 As GroupBox = window.Controls(0).Controls(3).Controls(0).Controls(0)

                    If groupbox1.Controls.Count < 1 Then
                        MsgBox("Not enough controls in BelastungsAnalyse parent GroupBox")
                        Return False
                    End If
                    'If groupbox1.Controls(0).GetType.Name <> "GroupBox" Then
                    '    MsgBox("1st control in BelastungsAnalyse parent GroupBox is not a GroupBox")
                    '    Return False
                    'End If
                    'Dim groupbox2 As GroupBox = groupbox1.Controls(0)

                    'If groupbox2.Controls.Count < 1 Then
                    '    MsgBox("Not enough controls in BelastungsAnalyse GroupBox")
                    '    Return False
                    'End If
                    'If groupbox2.Controls(0).GetType.Name <> "GroupBox" Then
                    '    MsgBox("1st control in BelastungsAnalyse GroupBox is not a GroupBox")
                    '    Return False
                    'End If
                    'Dim groupbox3 As GroupBox = groupbox2.Controls(0)

                    'If groupbox3.Controls.Count < 1 Then
                    '    MsgBox("Not enough controls in Statikauswetrung GroupBox")
                    '    Return False
                    'End If
                    If groupbox1.Controls(0).GetType.Name <> "TableLayoutPanel" Then
                        MsgBox("1st control in Statikauswetrung GroupBox is not a TableLayoutPanel")
                        Return False
                    End If
                    _staticsTableLayoutPanel = groupbox1.Controls(0)
                    Return True
                End If


            Else
                MsgBox("Window is Nothing, handle: " & _hWnd.ToString)
                Return False
            End If
        End Function

        Friend Function PrepareProject() As Boolean
            If PrepareProjectStep1() Then
                If _projectNodes Is Nothing Then
                    MsgBox("Nodes in project treeview not found.")
                    Return False
                End If
                If Not PrepareProjectStep2() Then
                    Return False
                End If
                If _loaddataGroupBox Is Nothing Then
                    MsgBox("LoadData Groupbox not found")
                    Return False
                End If
                If _payloadControl Is Nothing Then
                    MsgBox("Payload Control not found")
                    Return False
                End If
                If _dynamicControl Is Nothing Then
                    MsgBox("Dynamic Control not found")
                    Return False
                End If
                If _staticsTableLayoutPanel Is Nothing Then
                    MsgBox("Statics TableLayoutPanel not found")
                    Return False
                End If

                'Dim result As DialogResult = MessageBox.Show("Generate pictures for online (yes) or offline (no)?", "Online/Offline", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                'Dim add_str As String
                'If result = DialogResult.Yes Then
                '    add_str = "_robotload_online_"
                'Else
                '    add_str = "_robotload_offline_"
                'End If

                MessageBox.Show("Select a folder to write the images.", "Select folder", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Dim dlg = New FolderBrowserDialog() With {.Description = "Select a folder to writedown the images:"}
                Dim xresult As System.Windows.Forms.DialogResult = dlg.ShowDialog()

                If xresult = DialogResult.OK Then
                    If Not Directory.Exists(Path.Combine(dlg.SelectedPath, "BodyInWhite")) Then
                        Directory.CreateDirectory(Path.Combine(dlg.SelectedPath, "BodyInWhite"))
                    End If

                    For Each parentnode As TreeNode In _projectNodes
                        Dim robot_name As String = parentnode.Text
                        Dim plcName As String = New Regex("(?<=^\s*)\w{6}", RegexOptions.IgnoreCase).Match(robot_name).ToString()
                        Dim stName As String = New Regex("(?<=^\s*[a-zA-Z0-9]+_)ST\d+", RegexOptions.IgnoreCase).Match(robot_name).ToString()
                        Dim robotName As String = New Regex("(?<=^\s*[a-zA-Z0-9]+_ST\d+_)IR\d+", RegexOptions.IgnoreCase).Match(robot_name).ToString()
                        If Not Directory.Exists(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName)) Then
                            Directory.CreateDirectory(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName))
                        End If
                        If Not Directory.Exists(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName)) Then
                            Directory.CreateDirectory(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName))
                        End If
                        If Not Directory.Exists(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName, "Pictures")) Then
                            Directory.CreateDirectory(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName, "Pictures"))
                        End If
                        If Not Directory.Exists(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName, "Pictures", "Online")) Then
                            Directory.CreateDirectory(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName, "Pictures", "Online"))
                        End If
                        If Not Directory.Exists(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName, "Pictures", "Online", "ToolLoad")) Then
                            Directory.CreateDirectory(Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName, "Pictures", "Online", "ToolLoad"))
                        End If

                        For Each node As TreeNode In parentnode.Nodes
                            Dim load_name As String = node.Text
                            _projectTreeView.SelectedNode = node
                            Dim window As System.Windows.Forms.Control = System.Windows.Forms.Control.FromHandle(_hWnd)
                            Dim tabControl As TabControl = window.Controls(0).Controls(4).Controls(1).Controls(0)
                            Dim loaddata As Image = GetControlImage(_loaddataGroupBox)
                            tabControl.SelectedIndex = 0
                            Dim payload As Image = GetControlImage(_payloadControl)
                            tabControl.SelectedIndex = 3
                            Dim dynamic As Image = GetControlImage(_dynamicControl)
                            Dim statics As Image = GetControlImage(_staticsTableLayoutPanel)
                            Dim outputBitmap As Image = New Bitmap(700, 640, System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                            Dim g As Graphics = Graphics.FromImage(outputBitmap)
                            g.Clear(Color.White)
                            g.DrawImage(payload, New Rectangle(0, 380, 350, 205))
                            g.DrawImage(dynamic, New Rectangle(350, 380, 350, 205))
                            g.DrawImageUnscaled(loaddata, New Point(0, 0))
                            g.DrawImageUnscaled(statics, New Point(500, 100))
                            outputBitmap.Save(System.IO.Path.Combine(dlg.SelectedPath, "BodyInWhite", plcName, stName + "_" + robotName, "Pictures", "Online", "ToolLoad", robot_name & load_name & ".jpg"), System.Drawing.Imaging.ImageFormat.Jpeg)
                            Thread.Sleep(50)
                        Next

                    Next
                Else
                    Return False
                End If
            Else
                Return False
            End If
            Return True
        End Function

        Public Shared Function GetRobots(hWnd As IntPtr) As Dictionary(Of Integer, String)
            Dim tmp As InjectionHelper = New InjectionHelper(hWnd)
            tmp.GetTreeViewElems()
            Return tmp.ReturnData
        End Function

        Public Shared Function PrepareData(hWndMain As IntPtr, hWndProject As IntPtr) As List(Of Boolean)
            Dim tmp As InjectionHelper = New InjectionHelper(hWndMain, hWndProject)
            Return New List(Of Boolean) From {tmp.PrepareProject()}
        End Function

        Public Shared Function GetKukaLoadVersion(hWnd As IntPtr) As Boolean
            Dim window As System.Windows.Forms.Control = System.Windows.Forms.Control.FromHandle(hWnd)
            If window.Controls.Count > 2 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function GetRobotGeneration(hWnd As IntPtr) As String
            Dim _isOldKukaLoad As Boolean = GetKukaLoadVersion(hWnd)
            If _isOldKukaLoad Then
                Return "KR C4"
            End If
            Dim window As System.Windows.Forms.Control = System.Windows.Forms.Control.FromHandle(hWnd)
            Return window.Controls(0).Controls(5).Controls(0).Controls(0).Controls(2).Text
        End Function

    End Class
End Namespace