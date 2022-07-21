
Public Class OpenArchive

    Private _tempFolder As String
    Private _fileList As List(Of String)
    Public ReadOnly Property fileList As List(Of String)
        Get
            Return _fileList
        End Get
    End Property
    Public ReadOnly Property tempFolder As String
        Get
            Return _tempFolder
        End Get
    End Property


    Public Sub New(ByVal mypath As String)
        _tempFolder = Path.Combine(Path.GetTempPath, Guid.NewGuid.ToString)
        Do While Directory.Exists(_tempFolder) Or File.Exists(_tempFolder)
            _tempFolder = Path.Combine(Path.GetTempPath, Guid.NewGuid.ToString)
        Loop

        If System.IO.Directory.Exists(_tempFolder) Then
            System.IO.Directory.Delete(_tempFolder, True)
        End If
        _fileList = New List(Of String)
        If File.Exists(mypath) Then
            OpenArchive(mypath)
        ElseIf Directory.Exists(mypath) Then
            OpenDirectory(mypath)
        End If
    End Sub

    Public Shared Function MakeRelativePath(ByVal fromPath As String, ByVal toPath As String) As String
        If String.IsNullOrEmpty(fromPath) Then
            Throw New ArgumentNullException("fromPath")
        End If
        If String.IsNullOrEmpty(toPath) Then
            Throw New ArgumentNullException("toPath")
        End If

        Dim fromUri As Uri = New Uri(fromPath)
        Dim toUri As Uri = New Uri(toPath)

        If fromUri.Scheme <> toUri.Scheme Then
            Return toPath
        End If
        ' path can't be made relative.

        Dim relativeUri As Uri = fromUri.MakeRelativeUri(toUri)
        Dim relativePath As String = Uri.UnescapeDataString(relativeUri.ToString())

        If toUri.Scheme.ToUpperInvariant() = "FILE" Then
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
        End If

        Return relativePath
    End Function

    Private Sub OpenDirectory(ByRef mypath As String)
        Try
            For Each entry As String In Directory.GetFiles(mypath, "*.*", SearchOption.AllDirectories)
                Dim high_prio As Boolean
                Dim visible As Boolean
                Dim rel_path As String = MakeRelativePath(mypath & "/", entry)
                If ShouldCopyArchive(rel_path, high_prio, visible) Then
                    Dim dir As String = Path.GetDirectoryName(Path.Combine(_tempFolder, Path.GetFileName(entry)))
                    If Not Directory.Exists(dir) Then
                        Directory.CreateDirectory(dir)
                    End If
                    File.Copy(entry, Path.Combine(dir, Path.GetFileName(entry)))
                    If visible Then
                        If high_prio Then
                            _fileList.Insert(0, Path.GetFileName(entry))
                        Else
                            _fileList.Add(Path.GetFileName(entry))
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "File read error")
        End Try
    End Sub

    Private Sub OpenArchive(ByRef mypath As String)
        Try
            Using archive As ZipArchive = ZipFile.OpenRead(mypath)
                For Each entry As ZipArchiveEntry In archive.Entries
                    Dim high_prio As Boolean
                    Dim visible As Boolean
                    If ShouldCopyArchive(entry.FullName, high_prio, visible) Then
                        Dim dir As String = Path.GetDirectoryName(Path.Combine(_tempFolder, entry.FullName))
                        If Not Directory.Exists(dir) Then
                            Directory.CreateDirectory(dir)
                        End If
                        entry.ExtractToFile(Path.Combine(_tempFolder, entry.FullName))
                        If visible Then
                            If high_prio Then
                                _fileList.Insert(0, entry.FullName)
                            Else
                                _fileList.Add(entry.FullName)
                            End If
                        End If
                    End If
                Next
            End Using
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "File read error")
            'Throw New NotImplementedException
        End Try
    End Sub

    Private Function ShouldCopyArchive(filename As String, ByRef high_prio As Boolean, ByRef visible As Boolean) As Boolean
        visible = True
        ' forced true
        high_prio = True
        If filename.EndsWith("KRC/R1/System/$config.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/Mada/$machine.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/BMW_App/A01_plc_User.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/BMW_App/a03_grp.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/BMW_App/a03_grp_user.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/BMW_App/A04_swp_global.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/BMW_App/A04_swp_user.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/BMW_App/a02_tch.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/BMW_App/a02_tch_global.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("KRC/R1/BMW_App/a02_tch_user.dat", StringComparison.OrdinalIgnoreCase) Then Return True
        If filename.EndsWith("ArchiveInfo.xml", StringComparison.OrdinalIgnoreCase) Then Return True

        high_prio = False
        ' FALSE
        If filename.StartsWith("c/krc", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.StartsWith("krc/r1/tp", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.StartsWith("KRC/STEU/", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.StartsWith("krc/r1/system", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.StartsWith("krc/r1/bmw_init", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.StartsWith("krc/r1/bmw_app", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("krc/r1/bmw_utilities/application_ini.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("KRC/R1/BMW_Utilities/Appl_OnActivate.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("KRC/R1/BMW_Utilities/Appl_stopm_ini.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("KRC/R1/BMW_Utilities/Appl_stopm_restart.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("KRC/R1/BMW_Utilities/Appl_stopm_stop.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("KRC/R1/BMW_Utilities/masref.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("KRC/R1/BMW_Utilities/masref_bmw.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("KRC/R1/Program/masref_user.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.EndsWith("KRC/R1/Program/tm_useraction.src", StringComparison.OrdinalIgnoreCase) Then Return False
        If filename.Contains("select_abortprog") Then Return False
        If filename.Contains("a03_grp_special") Then Return False

        ' TRUE
        If filename.EndsWith(".src", StringComparison.OrdinalIgnoreCase) Then Return True

        visible = False
        If filename.EndsWith(".dat", StringComparison.OrdinalIgnoreCase) Then Return True

        Return False
    End Function

    Protected Overrides Sub Finalize()
        If System.IO.Directory.Exists(_tempFolder) Then
            System.IO.Directory.Delete(_tempFolder, True)
        End If
        MyBase.Finalize()
    End Sub
End Class
