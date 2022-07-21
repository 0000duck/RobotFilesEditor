Option Compare Text
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.IO
Imports System.Collections.ObjectModel
Imports System.IO.Compression
Imports System.Globalization
Imports System.Xml.Serialization
Imports System.Windows.Forms
Imports dll_KUKA_ParseModuleFile
Imports dll_KUKA_OpenArchive
Imports dll_KUKA_ParseModuleFile.KUKA
Imports IniParser
Imports IniParser.Model
Imports System.Text.RegularExpressions
Imports ParseModuleFile

Public MustInherit Class PropList
    Implements IXmlSerializable
    Implements INotifyPropertyChanged
    Public Shared isOldKUKALoad As Boolean
    Public Shared loadCaseNum As Integer

#Region " Events "
    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
#End Region

#Region " Methods "
    ' This method is called by the Set accessor of each property. 
    ' The CallerMemberName attribute that is applied to the optional propertyName 
    ' parameter causes the property name of the caller to be substituted as an argument. 
    Public Sub NotifyPropertyChanged(<CallerMemberName()> Optional ByVal propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
#End Region
#Region " XMLSerializable implementation "

    Public Function GetSchema() As Xml.Schema.XmlSchema Implements IXmlSerializable.GetSchema
        Return Nothing
    End Function

    Public Sub ReadXml(reader As Xml.XmlReader) Implements IXmlSerializable.ReadXml
        Throw New NotImplementedException
    End Sub

    Friend Shared Sub WriteElement(writer As Xml.XmlWriter, ByVal Name As String, Optional ByVal Value As String = Nothing)
        writer.WriteStartElement(Name)
        If Not String.IsNullOrWhiteSpace(Value) Then
            writer.WriteString(Value)
        End If
        writer.WriteEndElement()
    End Sub

    Public MustOverride Sub WriteXml(writer As Xml.XmlWriter) Implements IXmlSerializable.WriteXml
#End Region
End Class

<XmlRoot("Robot")> Public Class MyRobot
    Inherits PropList
    Private Shared _id As Integer = 0
    Private _myid As Integer
    Private _RobotName As String
    Private _RobotType As String
    Private _RobotID As RobotID
    Private _SerialNumber As Integer
    Private _LoadCases As LoadCaseList = New LoadCaseList

#Region " Fields "

#End Region
#Region " Properties "
    Public Property RobotName As String
        Get
            Return _RobotName
        End Get
        Set(value As String)
            _RobotName = value
            NotifyPropertyChanged()
        End Set
    End Property
    Public Property RobotType As String
        Get
            Return _RobotType
        End Get
        Set(value As String)
            _RobotType = value
            NotifyPropertyChanged()
        End Set
    End Property
    Public ReadOnly Property RobotIDListEmpty As Brush
        Get
            If _RobotID Is Nothing Then
                Return Brushes.Red
            Else
                Return Brushes.White
            End If
        End Get
    End Property
    Public Property RobotID As RobotID
        Get
            Return _RobotID
        End Get
        Set(value As RobotID)
            _RobotID = value
            NotifyPropertyChanged()
            NotifyPropertyChanged("RobotIDListEmpty")
        End Set
    End Property
    Public Property SerialNumber As Integer
        Get
            Return _SerialNumber
        End Get
        Set(value As Integer)
            _SerialNumber = value
            NotifyPropertyChanged()
        End Set
    End Property
    Public Property LoadCases As LoadCaseList
        Get
            Return _LoadCases
        End Get
        Set(value As LoadCaseList)
            _LoadCases = value
            NotifyPropertyChanged()
        End Set
    End Property
    Public ReadOnly Property ID As Integer
        Get
            Return _myid
        End Get
    End Property
#End Region

    Public Sub New()
        _id += 1
        _myid = _id
    End Sub

    Public Overrides Sub WriteXml(writer As Xml.XmlWriter)
        Dim krc2ids = New Integer() {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 42, 43, 44, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 66, 67, 68, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 108, 109, 110, 126, 127}

        If KUKALoad_Connector._isOldKUKALoad Then
            WriteElement(writer, "ID", _myid.ToString)
            WriteElement(writer, "robottype", _RobotName)
            WriteElement(writer, "RobotID", _RobotID.ID.ToString)
            WriteElement(writer, "StationID", "1")
            WriteElement(writer, "serialnumber", _SerialNumber.ToString)
            WriteElement(writer, "comment")
            WriteElement(writer, "application")
            If krc2ids.Contains(_myid) Then
                WriteElement(writer, "krcRelease", "V 5.6")
                WriteElement(writer, "ControlTechnologyID", "1")
            Else
                WriteElement(writer, "krcRelease", "V 8.3")
                WriteElement(writer, "ControlTechnologyID", "2")
            End If

            WriteElement(writer, "VoltageNetworkID", "2")
        Else
            WriteElement(writer, "RobotID", _RobotID.ID.ToString)
            WriteElement(writer, "Name", _RobotName)
            WriteElement(writer, "Controltechnologytype", KUKALoad_Connector._RobotGeneration)
            Dim voltage As String = ""
            Select Case KUKALoad_Connector._RobotGeneration
                Case "KR C2", "KR C4", "KR C4 smallsize 2"
                    voltage = "400"
                Case "KR C5"
                    voltage = "380 - 480"
                Case "KR C4 Compact"
                    voltage = "230"
                Case "KR C5 micro"
                    voltage = "230 - 240"
            End Select
            WriteElement(writer, "Voltagetype", voltage)
            WriteElement(writer, "SerialNumber", _SerialNumber.ToString)
            WriteElement(writer, "Application", "")
            WriteElement(writer, "Comment", "")
        End If
    End Sub
End Class

Public Class Load
    Inherits PropList
    Public Property m As Double
    Public Property cx As Double
    Public Property cy As Double
    Public Property cz As Double
    Public Property Ix As Double
    Public Property Iy As Double
    Public Property Iz As Double
    Public Sub New(ByVal load As KUKA.LOAD)
        _m = load.M
        _cx = load.cm_x
        _cy = load.cm_y
        _cz = load.cm_z
        _Ix = load.j_x
        _Iy = load.j_y
        _Iz = load.j_z
    End Sub
    Public Sub WriteData(writer As Xml.XmlWriter, Optional ByVal suffix As String = "", Optional ByVal axisNum As Integer = 0)
        If KUKALoad_Connector._isOldKUKALoad Then
            WriteElement(writer, "m" & suffix, Math.Max(0.0F, _m).ToString(CultureInfo.InvariantCulture))
            WriteElement(writer, "cx" & suffix, _cx.ToString(CultureInfo.InvariantCulture))
            WriteElement(writer, "cy" & suffix, _cy.ToString(CultureInfo.InvariantCulture))
            WriteElement(writer, "cz" & suffix, _cz.ToString(CultureInfo.InvariantCulture))
            WriteElement(writer, "Ix" & suffix, _Ix.ToString(CultureInfo.InvariantCulture))
            WriteElement(writer, "Iy" & suffix, _Iy.ToString(CultureInfo.InvariantCulture))
            WriteElement(writer, "Iz" & suffix, _Iz.ToString(CultureInfo.InvariantCulture))
        Else
            writer.WriteStartElement("AxisLoadParameter")
            Select Case axisNum
                Case 1, 2, 3
                    WriteElement(writer, "Axis", axisNum)
                    WriteElement(writer, "Mass" & suffix, Math.Max(0.0F, _m).ToString(CultureInfo.InvariantCulture))
                    writer.WriteStartElement("CenterOfMass")
                    WriteElement(writer, "Lx" & suffix, _cx.ToString(CultureInfo.InvariantCulture))
                    WriteElement(writer, "Ly" & suffix, _cy.ToString(CultureInfo.InvariantCulture))
                    WriteElement(writer, "Lz" & suffix, _cz.ToString(CultureInfo.InvariantCulture))
                    writer.WriteEndElement()
                    writer.WriteStartElement("Inertia")
                    WriteElement(writer, "InertiaVarianttype", "XYZ")
                    writer.WriteStartElement("InertiaVariantXYZ")
                    writer.WriteStartElement("XYZ")
                    WriteElement(writer, "Ix" & suffix, _Ix.ToString(CultureInfo.InvariantCulture))
                    WriteElement(writer, "Iy" & suffix, _Iy.ToString(CultureInfo.InvariantCulture))
                    WriteElement(writer, "Iz" & suffix, _Iz.ToString(CultureInfo.InvariantCulture))
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                Case 6
                    WriteElement(writer, "Axis", "6")
                    WriteElement(writer, "Mass" & suffix, Math.Max(0.0F, _m).ToString(CultureInfo.InvariantCulture))
                    writer.WriteStartElement("CenterOfMass")
                    WriteElement(writer, "Lx" & suffix, _cx.ToString(CultureInfo.InvariantCulture))
                    WriteElement(writer, "Ly" & suffix, _cy.ToString(CultureInfo.InvariantCulture))
                    WriteElement(writer, "Lz" & suffix, _cz.ToString(CultureInfo.InvariantCulture))
                    writer.WriteEndElement()
                    writer.WriteStartElement("Inertia")
                    WriteElement(writer, "InertiaVarianttype", "XYZ")
                    writer.WriteStartElement("InertiaVariantXYZ")
                    writer.WriteStartElement("XYZ")
                    WriteElement(writer, "Ix" & suffix, _Ix.ToString(CultureInfo.InvariantCulture))
                    WriteElement(writer, "Iy" & suffix, _Iy.ToString(CultureInfo.InvariantCulture))
                    WriteElement(writer, "Iz" & suffix, _Iz.ToString(CultureInfo.InvariantCulture))
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                    writer.WriteEndElement()
            End Select
            writer.WriteEndElement()
        End If
    End Sub

    Public Overrides Sub WriteXml(writer As Xml.XmlWriter)
        Throw New NotImplementedException
    End Sub
End Class

Public Class RobotID
    Inherits PropList

    Private _Selected As Boolean = True

    Public Property ID As Integer = -1
    Public Property Name As String = ""
    Public ReadOnly Property Value As String
        Get
            Return _Name & " (" & _ID.ToString & ")"
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Value
    End Function

    Public Property Selected As Boolean
        Get
            Return _Selected
        End Get
        Set(value As Boolean)
            _Selected = value
            NotifyPropertyChanged()
        End Set

    End Property

    Sub New(ByVal ID As Integer, ByVal Name As String)
        _ID = ID
        _Name = Name
    End Sub

    Public Overrides Sub WriteXml(writer As Xml.XmlWriter)
        Throw New NotImplementedException
    End Sub
End Class

Public Class LoadingCase
    Inherits PropList

    Private Shared _id As Integer = 0
    Private _myid As Integer
    Private _robotID As Integer

    Public ReadOnly Property Text As String
        Get
            Return String.Format(CultureInfo.InvariantCulture, "{0}={{M {1},CM {{X {2},Y {3},Z {4}}},J {{X {5},Y {6},Z {7}}}}}", _
                                 _Tool, Math.Round(_A6.m, 2), Math.Round(_A6.cx, 3), Math.Round(_A6.cy, 3), _
                                 Math.Round(_A6.cz, 3), Math.Round(_A6.Ix, 5), Math.Round(_A6.Iy, 5), Math.Round(_A6.Iz, 5))
        End Get
    End Property

    Private _selected As Boolean = False

    Public Property Selected As Boolean
        Get
            Return _selected
        End Get
        Set(value As Boolean)
            If value <> _selected Then
                _selected = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Tool As String
    Public Property A6 As Load
    Public Property A1 As Load
    Public Property A2 As Load
    Public Property A3 As Load

    Protected Sub New()

    End Sub

    Public Sub New(RobotID As Integer)
        _id += 1
        _myid = _id
        _robotID = RobotID
    End Sub

    Public Overrides Sub WriteXml(writer As Xml.XmlWriter)
        If KUKALoad_Connector._isOldKUKALoad Then
            WriteElement(writer, "ID", _myid.ToString)
            WriteElement(writer, "description")
            WriteElement(writer, "RobotID", _robotID.ToString)
            WriteElement(writer, "loadID", "20141006_165344_808")
            WriteElement(writer, "tool", _Tool)
            WriteElement(writer, "date", "2014-09-23T14:04:43.5947138+02:00")
            WriteElement(writer, "engineer", "Ersteller")
            A6.WriteData(writer)
            WriteElement(writer, "A", "0")
            WriteElement(writer, "B", "0")
            WriteElement(writer, "C", "0")
            A1.WriteData(writer, "A1")
            A2.WriteData(writer, "A2")
            A3.WriteData(writer, "A3")
            WriteElement(writer, "DeterminationID", "2")
            WriteElement(writer, "calibration_with_tool", "128")
            WriteElement(writer, "determinationInfo")
            WriteElement(writer, "valuation")
            WriteElement(writer, "toolNr", "1")
        Else
            WriteElement(writer, "Name", _Tool)
            WriteElement(writer, "Creator", "Ersteller")
            WriteElement(writer, "CreateDate", "0001-01-01T00:00:00Z")
            WriteElement(writer, "Number", loadCaseNum)
            WriteElement(writer, "Description")
            WriteElement(writer, "Determination", "Manual")
            WriteElement(writer, "DeterminationInfo")
            writer.WriteStartElement("Valuations")
            WriteElement(writer, "Valuation", "Robot OK")
            writer.WriteEndElement()
            writer.WriteStartElement("LoadingCase")
            A6.WriteData(writer,, 6)
            A3.WriteData(writer,, 3)
            A2.WriteData(writer,, 2)
            A1.WriteData(writer,, 1)
            writer.WriteEndElement()
        End If
    End Sub
End Class

Public Class LoadCaseList
    Inherits ObservableCollection(Of LoadingCase)
End Class

Public Class LoadProject
    Inherits ObservableCollection(Of MyRobot)
End Class

Public Class BackupList
    Inherits ObservableCollection(Of BackupItem)

End Class

Public Class BackupItem
    Inherits PropList
#Region " Constructor "
    Sub New(ByVal Name As String, ByVal Path As String, ByVal Serial As Integer, ByVal Type As String)
        Me._name = Name
        Me._path = Path
        Me._serial = Serial
        Me._type = Type
        Me._selected = False
    End Sub
#End Region

#Region " Properites "
    Public Property Serial As Integer
        Get
            Return _serial
        End Get
        Set(value As Integer)
            _serial = value
            NotifyPropertyChanged()
        End Set
    End Property

    Public Property Type As String
        Get
            Return _type
        End Get
        Set(value As String)
            _type = value
            NotifyPropertyChanged()
        End Set
    End Property

    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            NotifyPropertyChanged()
        End Set
    End Property

    Public Property Path As String
        Get
            Return _path
        End Get
        Set(value As String)
            _path = value
            NotifyPropertyChanged()
        End Set
    End Property

    Public Property Selected As Boolean
        Get
            Return _selected
        End Get
        Set(value As Boolean)
            _selected = value
            NotifyPropertyChanged()
        End Set
    End Property
#End Region

#Region " Fields "
    Private _name As String = ""
    Private _path As String = ""
    Private _selected As Boolean = False
    Private _serial As Integer = 0
    Private _type As String = ""
#End Region

    Public Overrides Sub WriteXml(writer As Xml.XmlWriter)
        Throw New NotImplementedException
    End Sub
End Class

<XmlRoot("KukaLoadProjectDataSet")> Public Class MyData
    Inherits PropList

#Region " Fields and properties "
    Private WithEvents _backupList As BackupList = New BackupList
    Private WithEvents _loadProject As LoadProject = New LoadProject
    Private WithEvents _robotIDList As ObservableCollection(Of RobotID) = New ObservableCollection(Of RobotID)
    Public Property BackupList As BackupList
        Get
            Return _backupList
        End Get
        Set(value As BackupList)
            _backupList = value
            NotifyPropertyChanged()
        End Set
    End Property
    Public Property LoadProject As LoadProject
        Get
            Return _loadProject
        End Get
        Set(value As LoadProject)
            _loadProject = value
            NotifyPropertyChanged()
        End Set
    End Property
    Public Property RobotIDList As ObservableCollection(Of RobotID)
        Get
            Return _robotIDList
        End Get
        Set(value As ObservableCollection(Of RobotID))
            _robotIDList = value
            NotifyPropertyChanged()
        End Set
    End Property

    Private _findBackups As IAsyncCommand
    Private connector As KUKALoad_Connector = New KUKALoad_Connector



#Region " Step enabling "
    Private _step1Enabled As Boolean = True
    Private _step2Enabled As Boolean = False
    Private _step3Enabled As Boolean = False
    Private _step4Enabled As Boolean = False
    Private _step5Enabled As Boolean = False
    Private _step6Enabled As Boolean = False
    Private _step7Enabled As Boolean = False
    Public Property Step1Enabled As Boolean
        Get
            Return _step1Enabled
        End Get
        Set(value As Boolean)
            If value <> _step1Enabled Then
                _step1Enabled = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step2Enabled As Boolean
        Get
            Return _step2Enabled
        End Get
        Set(value As Boolean)
            If value <> _step2Enabled Then
                _step2Enabled = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step3Enabled As Boolean
        Get
            Return _step3Enabled
        End Get
        Set(value As Boolean)
            If value <> _step3Enabled Then
                _step3Enabled = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step4Enabled As Boolean
        Get
            Return _step4Enabled
        End Get
        Set(value As Boolean)
            If value <> _step4Enabled Then
                _step4Enabled = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step5Enabled As Boolean
        Get
            Return _step5Enabled
        End Get
        Set(value As Boolean)
            If value <> _step5Enabled Then
                _step5Enabled = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step6Enabled As Boolean
        Get
            Return _step6Enabled
        End Get
        Set(value As Boolean)
            If value <> _step6Enabled Then
                _step6Enabled = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step7Enabled As Boolean
        Get
            Return _step7Enabled
        End Get
        Set(value As Boolean)
            If value <> _step7Enabled Then
                _step6Enabled = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
#End Region
#Region " Step expanding "
    Private _step1Expanded As Boolean = True
    Private _step2Expanded As Boolean = False
    Private _step3Expanded As Boolean = False
    Private _step4Expanded As Boolean = False
    Private _step5Expanded As Boolean = False
    Private _step6Expanded As Boolean = False
    Private _step7Expanded As Boolean = False
    Public Property Step1Expanded As Boolean
        Get
            Return _step1Expanded
        End Get
        Set(value As Boolean)
            If value <> _step1Expanded Then
                _step1Expanded = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step2Expanded As Boolean
        Get
            Return _step2Expanded
        End Get
        Set(value As Boolean)
            If value <> _step2Expanded Then
                _step2Expanded = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step3Expanded As Boolean
        Get
            Return _step3Expanded
        End Get
        Set(value As Boolean)
            If value <> _step3Expanded Then
                _step3Expanded = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step4Expanded As Boolean
        Get
            Return _step4Expanded
        End Get
        Set(value As Boolean)
            If value <> _step4Expanded Then
                _step4Expanded = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step5Expanded As Boolean
        Get
            Return _step5Expanded
        End Get
        Set(value As Boolean)
            If value <> _step5Expanded Then
                _step5Expanded = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step6Expanded As Boolean
        Get
            Return _step6Expanded
        End Get
        Set(value As Boolean)
            If value <> _step6Expanded Then
                _step6Expanded = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
    Public Property Step7Expanded As Boolean
        Get
            Return _step7Expanded
        End Get
        Set(value As Boolean)
            If value <> _step7Expanded Then
                _step6Expanded = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property
#End Region

    Private _backupPath As String = My.Settings.backupPath
    Public Property BackupPath As String
        Get
            Return _backupPath
        End Get
        Set(value As String)
            If _backupPath <> value Then
                _backupPath = value
                My.Settings.backupPath = value
                My.Settings.Save()
                NotifyPropertyChanged()
            End If
        End Set
    End Property
#End Region

#Region " Methods "


    Public Sub LoadSelectedBackups()
        _loadProject.Clear()
        Dim excludeList As New List(Of String)
        excludeList.Add("")
        Dim includeList As New List(Of String)
        includeList.Add("$config.dat")
        includeList.Add("a01_plc_user.dat")
        includeList.Add("ArchiveInfo.xml")
        'Dim Archive As MultiArchive = New MultiArchive(_backupPath, 1, excludeList, includeList)
        For Each item As BackupItem In _backupList
            If item.Selected Then
                Dim Archive As OpenArchive = New OpenArchive(item.Path)
                Dim list_of_files As List(Of String) = New List(Of String)
                For Each fileName As String In Archive.fileList
                    If fileName.EndsWith("$config.dat", StringComparison.InvariantCultureIgnoreCase) Then
                        list_of_files.Add(fileName)
                    ElseIf fileName.EndsWith("a01_plc_user.dat", StringComparison.InvariantCultureIgnoreCase) Then
                        list_of_files.Add(fileName)
                    End If
                Next
                If list_of_files.Count < 1 Then
                    Throw New NotImplementedException
                End If
                Dim robot As Robot = New Robot(Archive.tempFolder, list_of_files)

                Dim _newRob As MyRobot = New MyRobot
                _newRob.SerialNumber = item.Serial
                _newRob.RobotName = item.Name
                _newRob.RobotType = item.Type
                For Each tool As KeyValuePair(Of Integer, Object) In robot.Tools
                    Dim x As dll_KUKA_ParseModuleFile.KUKA.BMW.Tool = tool.Value
                    If x.HasData Then
                        Dim lc As LoadingCase = New LoadingCase(_newRob.ID)
                        lc.A1 = New Load(robot.LOAD_A1_DATA)
                        lc.A2 = New Load(robot.LOAD_A2_DATA)
                        lc.A3 = New Load(robot.LOAD_A3_DATA)
                        lc.A6 = New Load(x.Load)
                        lc.Tool = "T" & x.num.ToString & ". " & x.Name
                        _newRob.LoadCases.Add(lc)
                    End If
                Next
                For Each LoadVar As KeyValuePair(Of Integer, Object) In robot.LoadsPLC
                    Dim x As dll_KUKA_ParseModuleFile.KUKA.BMW.LoadDataPLC = LoadVar.Value
                    If x.HasData Then
                        Dim lc As LoadingCase = New LoadingCase(_newRob.ID)
                        lc.A1 = New Load(robot.LOAD_A1_DATA)
                        lc.A2 = New Load(robot.LOAD_A2_DATA)
                        lc.A3 = New Load(robot.LOAD_A3_DATA)
                        lc.A6 = New Load(x.load)
                        lc.Tool = "L" & x.num.ToString & ". " & x.Name
                        _newRob.LoadCases.Add(lc)
                    End If
                Next
                _loadProject.Add(_newRob)
            End If
        Next
    End Sub

    Public Sub GoToStep(ByVal StepNumber As Integer)
        If StepNumber = 1 Then
            Step1Enabled = True
            Step2Enabled = False
            Step3Enabled = False
            Step4Enabled = False
            Step5Enabled = False
            Step6Enabled = False
            Step7Enabled = False
        ElseIf StepNumber = 2 Then
            Step1Enabled = True
            Step2Enabled = True
            Step3Enabled = False
            Step4Enabled = False
            Step5Enabled = False
            Step6Enabled = False
            Step7Enabled = False
        ElseIf StepNumber = 3 Then
            Step1Enabled = True
            Step2Enabled = True
            Step3Enabled = True
            Step4Enabled = False
            Step5Enabled = False
            Step6Enabled = False
            Step7Enabled = False
        ElseIf StepNumber = 4 Then
            Step1Enabled = True
            Step2Enabled = True
            Step3Enabled = True
            Step4Enabled = True
            Step5Enabled = False
            Step6Enabled = False
            Step7Enabled = False
        ElseIf StepNumber = 5 Then
            Step1Enabled = True
            Step2Enabled = True
            Step3Enabled = True
            Step4Enabled = True
            Step5Enabled = True
            Step6Enabled = False
            Step7Enabled = False
        ElseIf StepNumber = 6 Then
            Step1Enabled = True
            Step2Enabled = True
            Step3Enabled = True
            Step4Enabled = True
            Step5Enabled = True
            Step6Enabled = True
            Step7Enabled = False
        ElseIf StepNumber = 7 Then
            Step1Enabled = True
            Step2Enabled = True
            Step3Enabled = True
            Step4Enabled = True
            Step5Enabled = True
            Step6Enabled = True
            Step7Enabled = True
        End If

        If StepNumber = 1 Then
            Step1Expanded = True
            Step2Expanded = False
            Step3Expanded = False
            Step4Expanded = False
            Step5Expanded = False
            Step6Expanded = False
            Step7Expanded = False
        ElseIf StepNumber = 2 Then
            Step1Expanded = False
            Step2Expanded = True
            Step3Expanded = False
            Step4Expanded = False
            Step5Expanded = False
            Step6Expanded = False
            Step7Expanded = False
        ElseIf StepNumber = 3 Then
            Step1Expanded = False
            Step2Expanded = False
            Step3Expanded = True
            Step4Expanded = False
            Step5Expanded = False
            Step6Expanded = False
            Step7Expanded = False
        ElseIf StepNumber = 4 Then
            Step1Expanded = False
            Step2Expanded = False
            Step3Expanded = False
            Step4Expanded = True
            Step5Expanded = False
            Step6Expanded = False
            Step7Expanded = False
        ElseIf StepNumber = 5 Then
            Step1Expanded = False
            Step2Expanded = False
            Step3Expanded = False
            Step4Expanded = False
            Step5Expanded = True
            Step6Expanded = False
            Step7Expanded = False
        ElseIf StepNumber = 6 Then
            Step1Expanded = False
            Step2Expanded = False
            Step3Expanded = False
            Step4Expanded = False
            Step5Expanded = False
            Step6Expanded = True
            Step7Expanded = False
        ElseIf StepNumber = 7 Then
            Step1Expanded = False
            Step2Expanded = False
            Step3Expanded = False
            Step4Expanded = False
            Step5Expanded = False
            Step6Expanded = False
            Step7Expanded = True
        End If
    End Sub

    Public Sub ScanForBackups()
        _backupList.Clear()
        If Directory.Exists(_backupPath) Then
            Dim ret As Boolean = False
            For Each zipPath As String In Directory.GetFiles(_backupPath, "*.zip", SearchOption.AllDirectories)
                Try
                    Using archive As ZipArchive = System.IO.Compression.ZipFile.OpenRead(zipPath)
                        Dim entry As ZipArchiveEntry = archive.Entries.FirstOrDefault(Function(x) x.Name.Equals("ArchiveInfo.xml"))
                        If (entry IsNot Nothing) Then
                            Dim xmldoc As Xml.XmlDocument = New Xml.XmlDocument()
                            xmldoc.Load(entry.Open)
                            Dim xmlnsManager As System.Xml.XmlNamespaceManager = New System.Xml.XmlNamespaceManager(xmldoc.NameTable)
                            xmlnsManager.AddNamespace("k", "KUKARoboter.Contracts.Archive")
                            Dim xmlnodeName As Xml.XmlNode = xmldoc.SelectSingleNode("/k:ArchiveInformation/k:Roboter/k:Name", xmlnsManager)
                            Dim xmlnodeSerial As Xml.XmlNode = xmldoc.SelectSingleNode("/k:ArchiveInformation/k:Roboter/k:Serialnumber", xmlnsManager)
                            Dim xmlnodeType As Xml.XmlNode = xmldoc.SelectSingleNode("/k:ArchiveInformation/k:Roboter/k:Type", xmlnsManager)
                            If xmlnodeName IsNot Nothing Then
                                _backupList.Add(New BackupItem(xmlnodeName.InnerText, zipPath, Convert.ToInt32(xmlnodeSerial.InnerText), xmlnodeType.InnerText))
                            End If
                        Else
                            Dim amIniEntry As ZipArchiveEntry = archive.Entries.FirstOrDefault(Function(x) x.Name.Equals("am.ini"))
                            Dim robcorEntry As ZipArchiveEntry = archive.Entries.FirstOrDefault(Function(x) x.FullName.Equals("KRC/R1/Mada/$robcor.dat"))
                            If amIniEntry IsNot Nothing AndAlso robcorEntry IsNot Nothing Then
                                ' rocorfile
                                Dim regex = New Regex("\""(.*)\""")
                                Dim line As String
                                Dim model As String = Nothing
                                Using stream As Stream = robcorEntry.Open
                                    Using textReader As TextReader = New StreamReader(stream)
                                        While True
                                            line = textReader.ReadLine
                                            If line Is Nothing Then
                                                Exit While
                                            End If
                                            If line.StartsWith("$MODEL_NAME[]=") AndAlso regex.IsMatch(line) Then
                                                model = regex.Match(line)?.Groups(1)?.Value
                                                Exit While
                                            End If
                                        End While
                                    End Using
                                End Using

                                If model Is Nothing Then Continue For

                                ' ini file
                                Dim iniFile As FileIniDataParser = New FileIniDataParser()
                                Dim iniData As IniData
                                Using stream As Stream = amIniEntry.Open
                                    Using streamreader As StreamReader = New StreamReader(stream)
                                        iniData = iniFile.ReadData(streamreader)
                                    End Using
                                End Using
                                If (iniData IsNot Nothing) Then
                                    Dim name As String = iniData("Roboter")("RobName")
                                    Dim serial As Integer = Convert.ToInt32(iniData("Roboter")("IRSerialNr"))

                                    _backupList.Add(New BackupItem(name, zipPath, Convert.ToInt32(serial), model))
                                End If
                            End If
                        End If
                    End Using
                Catch ex As Exception
                    MessageBox.Show("Something went wrong with reading file " + zipPath, "Error")

                End Try
            Next
        End If
    End Sub

    Public Function ConnectAndGetList() As Boolean
        If Not connector.DoStep1 Then Return False
        _robotIDList.Clear()
        For Each item As KeyValuePair(Of Integer, String) In connector.RobotList
            _robotIDList.Add(New RobotID(item.Key, item.Value))
        Next
        Return True
    End Function

    Private Function MatchARobotID(ByVal Name As String) As RobotID
        Name = Name.TrimStart("#").Replace(" ", "").Replace("/", "_")
        For Each item As RobotID In _robotIDList
            Dim test As String = item.Name.Replace(" ", "").Replace("-", "_").Replace("/", "_")
            If Name.StartsWith(test, StringComparison.InvariantCultureIgnoreCase) Then
                Return item
            End If
            If Name Like test Then
                Return item
            End If
        Next
        Return Nothing
    End Function

    Public Sub AssignRobotsToList()
        For Each Robot As MyRobot In _loadProject
            If Robot.RobotID Is Nothing Then
                Robot.RobotID = MatchARobotID(Robot.RobotType)
            End If
        Next
    End Sub

    Public Function SaveProject() As Boolean
        Using SF_SaveProject As SaveFileDialog = New SaveFileDialog With {.Title = "Save Project...", .DefaultExt = "*.xml", .Filter = "Project files (*.xml)|*.xml"}
            SF_SaveProject.FileName = IO.Path.GetFileName(My.Settings.projectFile)
            Try
                SF_SaveProject.InitialDirectory = IO.Path.GetDirectoryName(My.Settings.projectFile)
                SF_SaveProject.RestoreDirectory = True
            Catch ex As Exception
            End Try
            If SF_SaveProject.ShowDialog = Windows.Forms.DialogResult.OK Then
                My.Settings.projectFile = SF_SaveProject.FileName
                My.Settings.Save()
                Dim objStreamWriter As New IO.StreamWriter(SF_SaveProject.FileName)
                Dim x As New XmlSerializer(Me.GetType)
                x.Serialize(objStreamWriter, Me)
                objStreamWriter.Close()
                If (Not KUKALoad_Connector._isOldKUKALoad) Then
                    Dim stringList As String = ""
                    Dim reader As New StreamReader(SF_SaveProject.FileName)
                    While (Not reader.EndOfStream)
                        Dim line As String = reader.ReadLine()
                        If line.Contains("KukaLoadProjectDataSet") Then
                            line = line.Replace("KukaLoadProjectDataSet", "Project")
                        End If
                        stringList = stringList & line & Environment.NewLine
                    End While
                    reader.Close()
                    File.WriteAllText(SF_SaveProject.FileName, stringList)
                Else
                    Dim reader As New StreamReader(SF_SaveProject.FileName)
                    Dim filecontent As String = reader.ReadToEnd()
                    filecontent = filecontent & Environment.NewLine & "</KukaLoadProjectDataSet>"
                    reader.Close()
                    File.WriteAllText(SF_SaveProject.FileName, filecontent)
                End If
                Return True
            End If
        End Using
        Return False
    End Function

    Public Function ConnectAndPrepareProject() As Boolean
        If Not connector.DoStep2 Then Return False
        Return True
    End Function
#End Region

    Public Overrides Sub WriteXml(writer As Xml.XmlWriter)
        If KUKALoad_Connector._isOldKUKALoad Then
            'writer.WriteStartElement("KukaLoadProjectDataSet")
            writer.WriteStartElement("Factory")
            WriteElement(writer, "ID", "1")
            WriteElement(writer, "project", "some line")
            WriteElement(writer, "factory", "34.0")
            WriteElement(writer, "Information")
            WriteElement(writer, "customer", "BMW")
            writer.WriteEndElement()

            writer.WriteStartElement("Station")
            WriteElement(writer, "ID", "1")
            WriteElement(writer, "name", "Stx0")
            WriteElement(writer, "FactoryID", "1")
            WriteElement(writer, "description")
            writer.WriteEndElement()

            For Each rob As MyRobot In _loadProject
                If rob.RobotID IsNot Nothing Then
                    writer.WriteStartElement("Robot")
                    rob.WriteXml(writer)
                    writer.WriteEndElement()
                    For Each loadcase As LoadingCase In rob.LoadCases
                        If loadcase.Selected Then
                            writer.WriteStartElement("LoadingCase")
                            loadcase.WriteXml(writer)
                            writer.WriteEndElement()
                        End If
                    Next
                End If
            Next

            writer.WriteStartElement("Determination")
            WriteElement(writer, "ID", "1")
            WriteElement(writer, "determination", "CAD")
            writer.WriteEndElement()

            writer.WriteStartElement("Determination")
            WriteElement(writer, "ID", "2")
            WriteElement(writer, "determination", "Manuell")
            writer.WriteEndElement()

            writer.WriteStartElement("Determination")
            WriteElement(writer, "ID", "3")
            WriteElement(writer, "determination", "Lastdatenermittlung")
            'writer.WriteEndElement()
        Else
            'writer.WriteStartElement("Project")
            writer.WriteAttributeString("versioninfo", "KUKA.Load 5.0.31")
            writer.WriteAttributeString("xmlns", "http://kuka.com/kukaLoad/xsd/schemas/project")
            writer.WriteStartElement("ProjectEntry")
            WriteElement(writer, "Name", "MyProject")
            WriteElement(writer, "Factory", "MyFactory")
            WriteElement(writer, "Customer", "Customer")
            WriteElement(writer, "Information", "Info")
            writer.WriteStartElement("ProjectEntryStations")
            writer.WriteStartElement("ProjectEntryStation")
            WriteElement(writer, "Name", "Station1")
            WriteElement(writer, "Description", "MyDescr")
            writer.WriteStartElement("ProjectEntryStationRobots")

            For Each rob As MyRobot In _loadProject
                If rob.RobotID IsNot Nothing Then
                    writer.WriteStartElement("ProjectEntryStationRobot")
                    rob.WriteXml(writer)
                    writer.WriteStartElement("ProjectEntryStationRobotLoadingCases")
                    loadCaseNum = 1
                    For Each loadcase As LoadingCase In rob.LoadCases
                        If loadcase.Selected Then
                            writer.WriteStartElement("ProjectEntryStationRobotLoadingCase")
                            loadcase.WriteXml(writer)
                            writer.WriteEndElement()
                            loadCaseNum = loadCaseNum + 1
                        End If
                    Next
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                End If
            Next

            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndElement()
            writer.WriteEndElement()
            'writer.WriteEndElement()
        End If
    End Sub
End Class
