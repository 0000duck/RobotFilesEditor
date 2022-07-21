Imports System.Runtime.InteropServices
Imports System.Text
Imports Bds.Inject


Public Class KUKALoad_Connector

#Region " Fields "
    Public Shared _isOldKUKALoad As Boolean
    Private _hWndMain As IntPtr
    Private _hWndProject As IntPtr
    Private _RobotList As Dictionary(Of Integer, String)
    Public Shared _RobotGeneration As String
    Private _FailureFirstConn As Boolean = True
    Private _FailureSecondConn As Boolean = True
#End Region

#Region " Properties "
    Public ReadOnly Property RobotList As Dictionary(Of Integer, String)
        Get
            Return _RobotList
        End Get
    End Property
    Public ReadOnly Property RobotGeneration As String
        Get
            Return _RobotGeneration
        End Get
    End Property
    Public ReadOnly Property FailureFirstConn As Boolean
        Get
            Return _FailureFirstConn
        End Get
    End Property
    Public ReadOnly Property FailureSecondConn As Boolean
        Get
            Return _FailureSecondConn
        End Get
    End Property
#End Region

#Region "Native"
    ' User-defined as the maximum treeview item text length.
    ' If an items text exceeds this value when calling GetTVItemText
    ' there could be problems...
    Private Const MAX_ITEM = 256
    Private Delegate Function EnumWindowProcess(ByVal Handle As IntPtr, ByVal Parameter As IntPtr) As Boolean
    <DllImport("gdi32.dll")> _
    Private Shared Function BitBlt(ByVal hdcDest As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer, ByVal dwRop As System.Int32) As Boolean
    End Function
    <DllImport("user32.dll")> _
    Private Shared Function ReleaseDC(ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    <DllImport("User32.dll")> _
    Public Shared Function GetWindowDC(ByVal hWnd As IntPtr) As IntPtr
    End Function
    <DllImport("user32.dll")> _
    Private Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As Rect) As Boolean
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function SendInput(ByVal nInputs As Integer, ByVal pInputs() As Input, ByVal cbSize As Integer) As Integer
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As Boolean
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> Private Shared Function GetClassName(ByVal hWnd As System.IntPtr, ByVal lpClassName As System.Text.StringBuilder, ByVal nMaxCount As Integer) As Integer
    End Function
    <DllImport("user32.dll", SetLastError:=True)> _
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As SetWindowPosFlags) As Boolean
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function GetWindowTextLength(ByVal hwnd As IntPtr) As Integer
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function GetWindowText(ByVal hwnd As IntPtr, ByVal lpString As System.Text.StringBuilder, ByVal cch As Integer) As Integer
    End Function
    <DllImport("User32.dll")> _
    Private Shared Function EnumChildWindows(ByVal WindowHandle As IntPtr, ByVal Callback As EnumWindowProcess, ByVal lParam As IntPtr) As Boolean
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function FindWindow(ByVal lpClassName As String, ByVal lpWindowName As String) As IntPtr
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function FindWindowEx(ByVal hWndParent As Long, ByVal hWndChildAfter As Long, ByVal lpszClassName As String, ByVal lpszWindowName As String) As IntPtr
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As TVITEM) As IntPtr
    End Function

    Private Const KEYEVENTF_KEYUP As Integer = &H2
    Private Const INPUT_MOUSE As Integer = 0
    Private Const INPUT_KEYBOARD As Integer = 1
    Private Const INPUT_HARDWARE As Integer = 2

    Private Const VK_APPS As Short = &H5D
    Private Const VK_RETURN As Short = &HD
    Private Const VK_DOWN As Short = &H28
    Private Const VK_LBUTTON As Short = &H1
    Private Const VK_RBUTTON As Short = &H2

    Private Const SRCCOPY As Integer = &HCC0020

    Private Structure MOUSEINPUT
        Public dx As Integer
        Public dy As Integer
        Public mouseData As Integer
        Public dwFlags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure

    Private Structure KEYBDINPUT
        Public wVk As Short
        Public wScan As Short
        Public dwFlags As Integer
        Public time As Integer
        Public dwExtraInfo As IntPtr
    End Structure

    Private Structure HARDWAREINPUT
        Public uMsg As Integer
        Public wParamL As Short
        Public wParamH As Short
    End Structure

    <StructLayout(LayoutKind.Explicit)> _
    Private Structure INPUT
        <FieldOffset(0)> _
        Public type As Integer
        <FieldOffset(4)> _
        Public mi As MOUSEINPUT
        <FieldOffset(4)> _
        Public ki As KEYBDINPUT
        <FieldOffset(4)> _
        Public hi As HARDWAREINPUT
    End Structure
    <StructLayout(LayoutKind.Sequential)> _
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure


    ' ===========================================================================
    ' treeview definitions defined in Commctrl.h at:
    ' http://premium.microsoft.com/msdn/library/sdkdoc/c67_4c8m.htm

    Private Structure TVITEM   ' was TV_ITEM
        Dim mask As Long
        Dim hItem As Long
        Dim State As Long
        Dim stateMask As Long
        Dim pszText As String   ' Long   ' pointer
        Dim cchTextMax As Long
        Dim iImage As Long
        Dim iSelectedImage As Long
        Dim cChildren As Long
        Dim lParam As Long
    End Structure

    Private Enum TVITEM_mask
        TVIF_TEXT = &H1
        TVIF_IMAGE = &H2
        TVIF_PARAM = &H4
        TVIF_STATE = &H8
        TVIF_HANDLE = &H10
        TVIF_SELECTEDIMAGE = &H20
        TVIF_CHILDREN = &H40
#If (Win32_IE >= &H400) Then   ' WIN32_IE = 1024 (>= Comctl32.dll v4.71)
        TVIF_INTEGRAL = &H80
#End If
        TVIF_DI_SETITEM = &H1000   ' Notification
    End Enum

    Private Enum SetWindowPosFlags
        NONE = &H0
        NOSIZE = &H1
        NOMOVE = &H2
        NOZORDER = &H4
        NOREDRAW = &H8
        NOACTIVATE = &H10
        DRAWFRAME = &H20
        FRAMECHANGED = &H20
        SHOWWINDOW = &H40
        HIDEWINDOW = &H80
        NOCOPYBITS = &H100
        NOOWNERZORDER = &H200
        NOREPOSITION = &H200
        NOSENDCHANGING = &H400
        DEFERERASE = &H2000
        ASYNCWINDOWPOS = &H4000
    End Enum

    'Private Const TVE_COLLAPSE = &H1
    'Private Const TVE_COLLAPSERESET = &H8000
    'Private Const TVE_EXPAND = &H2
    'Private Const TVE_EXPANDPARTIAL = &H4000
    'Private Const TVE_TOGGLE = &H3
    'Private Const TV_FIRST = &H1100
    'Private Const TVM_EXPAND = (TV_FIRST + 2)
    'Private Const TVM_GETNEXTITEM = (TV_FIRST + 10)
    'Private Const TVGN_ROOT = &H0
    'Private Const TVGN_NEXTVISIBLE = &H6
    'Private Const TVGN_CHILD = 4


    ' TVM_GETNEXTITEM wParam values
    Private Enum TVGN_Flags
        TVGN_ROOT = &H0
        TVGN_NEXT = &H1
        TVGN_PREVIOUS = &H2
        TVGN_PARENT = &H3
        TVGN_CHILD = &H4
        TVGN_FIRSTVISIBLE = &H5
        TVGN_NEXTVISIBLE = &H6
        TVGN_PREVIOUSVISIBLE = &H7
        TVGN_DROPHILITE = &H8
        TVGN_CARET = &H9
#If (Win32_IE >= &H400) Then   ' >= Comctl32.dll v4.71
        TVGN_LASTVISIBLE = &HA
#End If
    End Enum

    Private Enum TVMessages
        TV_FIRST = &H1100
#If UNICODE Then
        TVM_INSERTITEM = (TV_FIRST + 50)
#Else
        TVM_INSERTITEM = (TV_FIRST + 0)
#End If
        TVM_DELETEITEM = (TV_FIRST + 1)
        TVM_EXPAND = (TV_FIRST + 2)
        TVM_GETITEMRECT = (TV_FIRST + 4)
        TVM_GETCOUNT = (TV_FIRST + 5)
        TVM_GETINDENT = (TV_FIRST + 6)
        TVM_SETINDENT = (TV_FIRST + 7)
        TVM_GETIMAGELIST = (TV_FIRST + 8)
        TVM_SETIMAGELIST = (TV_FIRST + 9)
        TVM_GETNEXTITEM = (TV_FIRST + 10)
        TVM_SELECTITEM = (TV_FIRST + 11)
#If UNICODE Then
        TVM_GETITEM = (TV_FIRST + 62)
        TVM_SETITEM = (TV_FIRST + 63)
        TVM_EDITLABEL = (TV_FIRST + 65)
#Else
        TVM_GETITEM = (TV_FIRST + 12)
        TVM_SETITEM = (TV_FIRST + 13)
        TVM_EDITLABEL = (TV_FIRST + 14)
#End If
        TVM_GETEDITCONTROL = (TV_FIRST + 15)
        TVM_GETVISIBLECOUNT = (TV_FIRST + 16)
        TVM_HITTEST = (TV_FIRST + 17)
        TVM_CREATEDRAGIMAGE = (TV_FIRST + 18)
        TVM_SORTCHILDREN = (TV_FIRST + 19)
        TVM_ENSUREVISIBLE = (TV_FIRST + 20)
        TVM_SORTCHILDRENCB = (TV_FIRST + 21)
        TVM_ENDEDITLABELNOW = (TV_FIRST + 22)
#If UNICODE Then
        TVM_GETISEARCHSTRING = (TV_FIRST + 64)
#Else
        TVM_GETISEARCHSTRING = (TV_FIRST + 23)
#End If
#If (Win32_IE >= &H300) Then
        TVM_SETTOOLTIPS = (TV_FIRST + 24)
        TVM_GETTOOLTIPS = (TV_FIRST + 25)
#End If    ' 0x0300
#If (Win32_IE >= &H400) Then
        TVM_SETINSERTMARK = (TV_FIRST + 26)
        TVM_SETUNICODEFORMAT = CCM_SETUNICODEFORMAT
        TVM_GETUNICODEFORMAT = CCM_GETUNICODEFORMAT
        TVM_SETITEMHEIGHT = (TV_FIRST + 27)
        TVM_GETITEMHEIGHT = (TV_FIRST + 28)
        TVM_SETBKCOLOR = (TV_FIRST + 29)
        TVM_SETTEXTCOLOR = (TV_FIRST + 30)
        TVM_GETBKCOLOR = (TV_FIRST + 31)
        TVM_GETTEXTCOLOR = (TV_FIRST + 32)
        TVM_SETSCROLLTIME = (TV_FIRST + 33)
        TVM_GETSCROLLTIME = (TV_FIRST + 34)
        TVM_SETINSERTMARKCOLOR = (TV_FIRST + 37)
        TVM_GETINSERTMARKCOLOR = (TV_FIRST + 38)
#End If   ' 0x0400

    End Enum   ' TVMessages

    Private Enum TVM_EXPAND_wParam
        TVE_COLLAPSE = &H1
        TVE_EXPAND = &H2
        TVE_TOGGLE = &H3
#If (Win32_IE >= &H300) Then
        TVE_EXPANDPARTIAL = &H4000
#End If
        TVE_COLLAPSERESET = &H8000
    End Enum

    ' Expands or collapses the list of child items, if any, associated with the specified parent item.
    ' Returns TRUE if successful or FALSE otherwise.
    ' (docs say TVM_EXPAND does not send the TVN_ITEMEXPANDING and
    ' TVN_ITEMEXPANDED notification messages to the parent window...?)
    Private Shared Function TreeView_Expand(hWnd As IntPtr, hItem As IntPtr, flag As TVM_EXPAND_wParam) As Boolean
        Return SendMessage(hWnd, TVMessages.TVM_EXPAND, flag, hItem)
    End Function

    ' Retrieves the tree-view item that bears the specified relationship to a specified item.
    ' Returns the handle to the item if successful or 0 otherwise.
    Private Shared Function TreeView_GetNextItem(hWnd As IntPtr, hItem As IntPtr, flag As Long) As IntPtr
        Return SendMessage(hWnd, TVMessages.TVM_GETNEXTITEM, flag, hItem)
    End Function

    ' Retrieves the first child item. The hitem parameter must be NULL.
    ' Returns the handle to the item if successful or 0 otherwise.
    Private Shared Function TreeView_GetChild(hWnd As IntPtr, hItem As IntPtr) As IntPtr
        Return TreeView_GetNextItem(hWnd, hItem, TVGN_Flags.TVGN_CHILD)
    End Function
    Private Shared Function TreeView_HasChildren(hWnd As IntPtr, hItem As IntPtr) As Boolean
        Return If(TreeView_GetNextItem(hWnd, hItem, TVGN_Flags.TVGN_CHILD) <> IntPtr.Zero, True, False)
    End Function

    ' Retrieves the next sibling item.
    ' Returns the handle to the item if successful or 0 otherwise.
    Private Shared Function TreeView_GetNextSibling(hWnd As IntPtr, hItem As IntPtr) As IntPtr
        Return TreeView_GetNextItem(hWnd, hItem, TVGN_Flags.TVGN_NEXT)
    End Function

    ' Retrieves the previous sibling item.
    ' Returns the handle to the item if successful or 0 otherwise.
    Private Shared Function TreeView_GetPrevSibling(hWnd As IntPtr, hItem As IntPtr) As IntPtr
        Return TreeView_GetNextItem(hWnd, hItem, TVGN_Flags.TVGN_PREVIOUS)
    End Function

    ' Retrieves the parent of the specified item.
    ' Returns the handle to the item if successful or 0 otherwise.
    Private Shared Function TreeView_GetParent(hWnd As IntPtr, hItem As IntPtr) As IntPtr
        Return TreeView_GetNextItem(hWnd, hItem, TVGN_Flags.TVGN_PARENT)
    End Function

    ' Retrieves the first visible item.
    ' Returns the handle to the item if successful or 0 otherwise.
    Private Shared Function TreeView_GetFirstVisible(hWnd As IntPtr) As IntPtr
        Return TreeView_GetNextItem(hWnd, 0, TVGN_Flags.TVGN_FIRSTVISIBLE)
    End Function

    ' Retrieves the topmost or very first item of the tree-view control.
    ' Returns the handle to the item if successful or 0 otherwise.
    Public Shared Function TreeView_GetRoot(hWnd As IntPtr) As IntPtr
        Return TreeView_GetNextItem(hWnd, 0, TVGN_Flags.TVGN_ROOT)
    End Function

    Private Shared Function TreeView_Select(hWnd As IntPtr, hItem As IntPtr) As Boolean
        Return SendMessage(hWnd, TVMessages.TVM_SELECTITEM, TVGN_Flags.TVGN_CARET, hItem)
    End Function

    Private Shared Function GetChildWindows(ByVal ParentHandle As IntPtr) As IntPtr()
        Dim ChildrenList As New List(Of IntPtr)
        Dim ListHandle As GCHandle = GCHandle.Alloc(ChildrenList)
        Try
            EnumChildWindows(ParentHandle, AddressOf EnumWindow, GCHandle.ToIntPtr(ListHandle))
        Finally
            If ListHandle.IsAllocated Then ListHandle.Free()
        End Try
        Return ChildrenList.ToArray
    End Function

    Private Shared Function EnumWindow(ByVal Handle As IntPtr, ByVal Parameter As IntPtr) As Boolean
        Dim ChildrenList As List(Of IntPtr) = GCHandle.FromIntPtr(Parameter).Target
        If ChildrenList Is Nothing Then Throw New Exception("GCHandle Target could not be cast as List(Of IntPtr)")
        ChildrenList.Add(Handle)
        Return True
    End Function
#End Region

#Region " Helper Methods "
    Private Function GetProcess() As Process
        For Each iProcess As Process In Process.GetProcesses()
            'If iProcess.ProcessName.StartsWith("KukaLoadGUI") Or iProcess.ProcessName.StartsWith("KUKA.Load") Then
            If iProcess.ProcessName.StartsWith("KukaLoadGUI") Then
                Return iProcess
            End If
        Next
        Return Nothing
    End Function

    Private Function GetClassName(hWnd As IntPtr) As String
        Dim className As StringBuilder = New StringBuilder(100)
        If GetClassName(hWnd, className, className.Capacity) > 0 Then
            Return className.ToString
        End If
        Return String.Empty
    End Function

    Public Function GetText(ByVal hWnd As IntPtr) As String
        Dim length As Integer
        If hWnd.ToInt32 = 0 Then
            Return Nothing
        End If
        length = GetWindowTextLength(hWnd)
        If length = 0 Then
            Return Nothing
        End If
        Dim sb As New System.Text.StringBuilder("", length)

        GetWindowText(hWnd, sb, sb.Capacity + 1)
        Return sb.ToString()
    End Function

    Private Function GetTreeViewElems(ByVal hWnd As IntPtr)
        Dim ctrl As System.Windows.Forms.Control = System.Windows.Forms.Control.FromHandle(hWnd)
        Dim treeview As System.Windows.Forms.TreeView = ctrl
        'System.Windows.Forms.NativeWindow.(hWnd)
        Return False
    End Function

#End Region

    Public Function DoStep1() As Boolean
        Dim x As Process = GetProcess()
        If x IsNot Nothing Then
            If x.MainWindowTitle <> "KukaLoadGUI" And Not x.MainModule.FileName.Contains("KukaLoadGUI") Then
                MsgBox("Title of window is wrong: " & x.MainWindowTitle & ". Should be KukaLoadGUI or KUKA.Load 5.x.x.x.")
                Return False
            End If
            _hWndMain = x.MainWindowHandle
            _RobotList = Injector.InvokeRemote(_hWndMain, "InjectionHelper.dll", "InjectionHelper.InjectionHelper.InjectionHelper", "GetRobots", New Object() {_hWndMain})
            _isOldKUKALoad = Injector.InvokeRemote(_hWndMain, "InjectionHelper.dll", "InjectionHelper.InjectionHelper.InjectionHelper", "GetKukaLoadVersion", New Object() {_hWndMain})
            _RobotGeneration = Injector.InvokeRemote(_hWndMain, "InjectionHelper.dll", "InjectionHelper.InjectionHelper.InjectionHelper", "GetRobotGeneration", New Object() {_hWndMain})
            If _RobotList Is Nothing OrElse _RobotList.Count = 0 Then
                MsgBox("No robot type found in KukaLoadGUI")
                Return False
            End If
            Return True
        Else
            MsgBox("Could not find KukaLoadGUI process.")
            Return False
        End If
    End Function

    Public Function DoStep2() As Boolean
        Dim x As Process = GetProcess()
        If x IsNot Nothing Then
            If x.MainWindowTitle <> "KukaLoadGUI" And Not x.MainWindowTitle.StartsWith("KUKA.Load") And Not x.MainWindowTitle.StartsWith("KukaLoad Robot Project") And Not x.ProcessName = "KukaLoadGUI" Then
                MsgBox("Title of window is wrong: " & x.MainWindowTitle & ". Should be KukaLoadGUI or KUKA.Load 5.x.x.x.")
                Return False
            End If
            _hWndProject = x.MainWindowHandle
            Dim aready As Object = Injector.InvokeRemote(_hWndProject, "InjectionHelper.dll", "InjectionHelper.InjectionHelper.InjectionHelper", "PrepareData", New Object() {_hWndMain, _hWndProject})
            If TypeOf aready Is List(Of Boolean) Then
                Dim ready As List(Of Boolean) = aready
                If ready Is Nothing OrElse ready.Count = 0 Or ready(0) = False Then
                    MsgBox("Could not prepare the project.")
                    Return False
                End If
            Else
                MsgBox("Connected has finished his work")
            End If
            Return True
        Else
            MsgBox("Could not find KukaLoadGUI process.")
            Return False
        End If
    End Function

    Public Shared Function IsOldKukaLoad() As Boolean
        Return _isOldKUKALoad
    End Function
End Class
