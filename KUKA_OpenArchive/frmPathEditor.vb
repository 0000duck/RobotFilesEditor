'Imports OpenTK
'Imports OpenTK.Graphics
'Imports OpenTK.Graphics.OpenGL
Imports dll_KUKA_ParseModuleFile

Public Class frmPathEditor
    'Private myScene As Scene
    Private _myList As Dictionary(Of Integer, Object)
    Dim loaded As Boolean = False
    Public ReadOnly Property myList As Dictionary(Of Integer, Object)
        Get
            Return _myList
        End Get
    End Property
#Region "Constructors"
    Public Sub New()
        InitializeComponent()
        'InitData()
    End Sub
    Public Sub New(ByRef myList As Dictionary(Of Integer, Object))
        _myList = myList
        InitializeComponent()
        If myList IsNot Nothing Then UcDataGrid1.Value = _myList
        'InitData()
    End Sub
#End Region
    'Private Sub InitData()
    '    ' calculate size of path
    '    For Each item As KeyValuePair(Of Integer, Object) In _myList
    '        Dim i As KUKA.PathFold = item.Value
    '        If i.motion <> KUKA.MoveType.None Then
    '            If i.X < pathMinPos.X Then pathMinPos.X = i.X
    '            If i.Y < pathMinPos.Y Then pathMinPos.Y = i.Y
    '            If i.Z < pathMinPos.Z Then pathMinPos.Z = i.Z
    '            If i.X > pathMaxPos.X Then pathMaxPos.X = i.X
    '            If i.Y > pathMaxPos.Y Then pathMaxPos.Y = i.Y
    '            If i.Z > pathMaxPos.Z Then pathMaxPos.Z = i.Z
    '        End If
    '    Next
    '    cameraTarget = Vector3.Lerp(pathMinPos, pathMaxPos, 0.5)
    '    Timer1.Enabled = True
    'End Sub
#Region "3D"
    'Private pathMinPos As Vector3 = New Vector3(Single.MaxValue, Single.MaxValue, Single.MaxValue)
    'Private pathMaxPos As Vector3 = New Vector3(Single.MinValue, Single.MinValue, Single.MinValue)
    'Private cameraTarget As Vector3

    'Private Sub GlControl1_Load(sender As Object, e As EventArgs)
    '    loaded = True
    '    GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)
    '    GL.Disable(EnableCap.CullFace)
    '    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha)
    '    GL.Disable(EnableCap.Lighting)
    '    GL.ShadeModel(ShadingModel.Flat)
    '    GL.Enable(EnableCap.LineSmooth) ' // This is Optional 
    '    GL.Enable(EnableCap.Normalize) '  // These is critical to have
    '    GL.Enable(EnableCap.RescaleNormal)

    '    GL.ClearColor(Color.DarkGray)
    '    myScene = New Scene(GlControl1.Size)
    '    'AddHandler Application.Idle, AddressOf Application_Idle
    'End Sub
    'Private Sub GlControl1_KeyPress(sender As Object, e As Windows.Forms.KeyPressEventArgs)
    '    'Select Case e.KeyChar
    '    '    Case ChrW(Keys.Escape) : Me.ActiveControl = Nothing
    '    '    Case "w" : camera.Move(0.0F, 0.1F, 0.0F)
    '    '    Case "a" : camera.Move(-0.1F, 0.0F, 0.0F)
    '    '    Case "s" : camera.Move(0.0F, -0.1F, 0.0F)
    '    '    Case "d" : camera.Move(0.1F, 0.0F, 0.0F)
    '    '    Case "q" : camera.Move(0.0F, 0.0F, 0.1F)
    '    '    Case "e" : camera.Move(0.0F, 0.0F, -0.1F)
    '    'End Select
    '    GlControl1.Invalidate()
    '    GlControl1.Update()
    'End Sub
    'Private Sub GlControl1_MouseMove(sender As Object, e As MouseEventArgs)
    '    If myScene.isRotating Or myScene.isDragging Then
    '        myScene.MousePos = e.Location
    '        GlControl1.Invalidate()
    '        GlControl1.Update()
    '    End If
    'End Sub
    'Private Sub GlControl1_MouseDown(sender As Object, e As MouseEventArgs)
    '    If e.Button = Windows.Forms.MouseButtons.Middle Then
    '        myScene.MousePos = e.Location
    '        myScene.isRotating = True
    '        GlControl1.Invalidate()
    '        GlControl1.Update()
    '    ElseIf e.Button = Windows.Forms.MouseButtons.Left Then
    '        myScene.MousePos = e.Location
    '        myScene.isDragging = True
    '        GlControl1.Invalidate()
    '        GlControl1.Update()
    '    End If
    'End Sub
    'Private Sub GlControl1_MouseUp(sender As Object, e As MouseEventArgs)
    '    If e.Button = Windows.Forms.MouseButtons.Middle Then
    '        myScene.isRotating = False
    '    ElseIf e.Button = Windows.Forms.MouseButtons.Left Then
    '        myScene.isDragging = False
    '    End If
    'End Sub
    'Private Sub GlControl1_Enter(sender As Object, e As EventArgs)
    '    'ResetMouse()
    'End Sub
    'Private Sub GlControl1_Paint(sender As Object, e As PaintEventArgs)
    '    If Not loaded Then Return
    '    Render()
    'End Sub
    'Private Sub GlControl1_Resize(sender As Object, e As EventArgs)
    '    myScene.SetupViewPortAndProjection = GlControl1.Size
    '    GlControl1.Invalidate(True)
    'End Sub

    'Private Sub DrawPath()
    '    GL.Color3(1.0F, 1.0F, 0.0F)
    '    GL.Begin(PrimitiveType.LineStrip)
    '    For Each item As KeyValuePair(Of Integer, Object) In _myList
    '        Dim i As KUKA.PathFold = item.Value
    '        If i.motion <> KUKA.MoveType.None Then
    '            GL.Vertex3(i.X / 100.0F, i.Y / 100.0F, i.Z / 100.0F)
    '        End If
    '    Next
    '    GL.End()
    'End Sub

    'Public Sub DrawSphere(Center As Vector3, Radius As Single, Precision As Integer)
    '    If Radius < 0.0F Then _
    '            Radius = -Radius
    '    If Radius = 0.0F Then _
    '            Throw New DivideByZeroException("DrawSphere: Radius cannot be 0f.")
    '    If Precision = 0 Then _
    '            Throw New DivideByZeroException("DrawSphere: Precision of 8 or greater is required.")

    '    Const HalfPI As Single = Math.PI * 0.5
    '    Dim OneThroughPrecision As Single = 1.0F / Precision
    '    Dim TwoPIThroughPrecision As Single = Math.PI * 2.0 * OneThroughPrecision

    '    Dim theta1, theta2, theta3 As Single
    '    Dim Normal, Position As Vector3

    '    For j As Integer = 0 To Precision - 1
    '        theta1 = (j * TwoPIThroughPrecision) - HalfPI
    '        theta2 = ((j + 1) * TwoPIThroughPrecision) - HalfPI

    '        GL.Begin(PrimitiveType.TriangleStrip)
    '        For i As Integer = 0 To Precision

    '            theta3 = i * TwoPIThroughPrecision

    '            Normal.X = (Math.Cos(theta2) * Math.Cos(theta3))
    '            Normal.Y = Math.Sin(theta2)
    '            Normal.Z = (Math.Cos(theta2) * Math.Sin(theta3))
    '            Position.X = Center.X + Radius * Normal.X
    '            Position.Y = Center.Y + Radius * Normal.Y
    '            Position.Z = Center.Z + Radius * Normal.Z

    '            GL.Normal3(Normal)
    '            GL.TexCoord2(i * OneThroughPrecision, 2.0F * (j + 1) * OneThroughPrecision)
    '            GL.Vertex3(Position)

    '            Normal.X = (Math.Cos(theta1) * Math.Cos(theta3))
    '            Normal.Y = Math.Sin(theta1)
    '            Normal.Z = (Math.Cos(theta1) * Math.Sin(theta3))
    '            Position.X = Center.X + Radius * Normal.X
    '            Position.Y = Center.Y + Radius * Normal.Y
    '            Position.Z = Center.Z + Radius * Normal.Z

    '            GL.Normal3(Normal)
    '            GL.TexCoord2(i * OneThroughPrecision, 2.0F * j * OneThroughPrecision)
    '            GL.Vertex3(Position)
    '        Next
    '        GL.End()
    '    Next
    'End Sub
    'Private Sub DrawZones()
    '    GL.Color4(0.6F, 0.8F, 0.6F, 0.2F)
    '    GL.Enable(EnableCap.Blend)
    '    'GL.DepthMask(False)
    '    For Each item As KeyValuePair(Of Integer, Object) In _myList
    '        Dim i As KUKA.PathFold = item.Value
    '        If i.motion <> KUKA.MoveType.None And i.zone > 0 Then
    '            DrawSphere(New Vector3(i.X / 100.0F, i.Y / 100.0F, i.Z / 100.0F), i.zone / 100, 20)
    '        End If
    '    Next
    '    GL.Disable(EnableCap.Blend)
    '    'GL.DepthMask(True)
    'End Sub

    'Private Sub DrawAxes()
    '    GL.Begin(PrimitiveType.Lines)
    '    GL.Color3(1.0F, 0.0F, 0.0F)
    '    GL.Vertex3(0, 0, 0)
    '    GL.Vertex3(300, 0, 0)

    '    GL.Color3(0.0F, 0.0F, 1.0F)
    '    GL.Vertex3(0, 0, 0)
    '    GL.Vertex3(0, 300, 0)

    '    GL.Color3(0.0F, 1.0F, 0.0F)
    '    GL.Vertex3(0, 0, 0)
    '    GL.Vertex3(0, 0, 300)
    '    GL.End()
    'End Sub

    'Private Sub DrawTriangle()
    '    GL.Begin(PrimitiveType.Triangles)
    '    GL.Color3(1.0F, 0.0F, 0.0F)
    '    GL.Vertex3(-1.0F, -1.0F, 4.0F)

    '    GL.Color3(0.0F, 1.0F, 0.0F)
    '    GL.Vertex3(1.0F, -1.0F, 4.0F)

    '    GL.Color3(0.0F, 0.0F, 1.0F)
    '    GL.Vertex3(0.0F, 1.0F, 4.0F)
    '    GL.End()
    'End Sub

    'Private Sub DrawLines()
    '    GL.Begin(PrimitiveType.Lines)
    '    Dim z As Single = 0
    '    Dim x, y As Single
    '    For angle As Single = 0 To Math.PI Step Math.PI / 20
    '        x = 50 * Math.Sin(angle)
    '        y = 50 * Math.Cos(angle)
    '        GL.Vertex3(x, y, z)
    '        x = 50 * Math.Sin(angle + Math.PI)
    '        y = 50 * Math.Cos(angle + Math.PI)
    '        GL.Vertex3(x, y, z)
    '    Next
    '    GL.End()
    'End Sub

    'Private Sub RenderGrid(rows As Integer, columns As Integer)
    '    GL.Color3(0.3F, 0.3F, 0.3F)
    '    GL.Begin(PrimitiveType.Lines)
    '    For i As Integer = 0 - rows / 2 To rows / 2
    '        'If i = 0 Then Continue For
    '        GL.Vertex3(-columns / 2, i, 0)
    '        GL.Vertex3(columns / 2, i, 0)
    '    Next
    '    For i As Integer = 0 - columns / 2 To columns / 2
    '        'If i = 0 Then Continue For
    '        GL.Vertex3(i, -rows / 2, 0)
    '        GL.Vertex3(i, rows / 2, 0)
    '    Next
    '    GL.End()
    'End Sub

    'Private Sub Render()
    '    'void OnRenderFrame(FrameEventArgs e)
    '    GL.Clear(ClearBufferMask.ColorBufferBit Or ClearBufferMask.DepthBufferBit)
    '    'Compute()

    '    myScene.SetCamera()

    '    RenderGrid(10, 10)
    '    DrawAxes()
    '    DrawTriangle()
    '    If _myList IsNot Nothing Then
    '        DrawPath()
    '        DrawZones()
    '    End If
    '    GlControl1.SwapBuffers()
    '    Return


    '    GlControl1.SwapBuffers()

    'End Sub

#End Region



    'Private Function getArcBallVector(px As Point) As Vector3
    '    Dim P As Vector3 = New Vector3(1.0F * px.X / GlControl1.Width * 2 - 1.0F, 1.0F * px.Y / GlControl1.Height * 2 - 1.0F, 0)
    '    P.Y = -P.Y
    '    Dim op_squared As Single = P.X * P.X + P.Y * P.Y
    '    If op_squared <= 1 Then
    '        P.Z = Math.Sqrt(1.0F - op_squared)
    '    Else
    '        P.Normalize()
    '    End If
    '    Return P
    'End Function
    'Public Shared Function Mult(ByRef left As Matrix3, ByRef right As Vector3) As Vector3
    '    Mult.X = left.M11 * right.X + left.M21 * right.Y + left.M31 * right.Z
    '    Mult.Y = left.M12 * right.X + left.M22 * right.Y + left.M32 * right.Z
    '    Mult.Z = left.M13 * right.X + left.M23 * right.Y + left.M33 * right.Z
    'End Function



    'Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
    '    Dim text As String = ""
    '    text &= "_current = " & myScene.current_translation.ToString & vbNewLine
    '    text &= "_temp = " & myScene.temp_translation.ToString & vbNewLine
    '    TextBox1.Text = text
    'End Sub
End Class

Public Class Scene
    '    Const rad2angle As Single = 180.0F / Math.PI
    '    Private _projection As Matrix4
    '    Private _viewport As Size
    '    Private _modelview As Matrix4 = Matrix4.LookAt(New Vector3(0.0F, -7.0F, 7.0F), New Vector3(0.0F, 0.0F, 0.0F), Vector3.UnitY)

    '    Private _current_rotation As Quaternion = Quaternion.Identity
    '    Public Property current_translation As Vector3 = Vector3.Zero
    '    Public Property temp_translation As Vector3 = Vector3.Zero

    '    Private oldPos As Vector2
    '    Private newPos As Vector2

    '    Private _rotating As Boolean
    '    Private _dragging As Boolean
    '    Public WriteOnly Property MousePos As Point
    '        Set(value As Point)
    '            newPos.X = value.X
    '            newPos.Y = value.Y
    '        End Set
    '    End Property
    '    Public Property isRotating As Boolean
    '        Get
    '            Return _rotating
    '        End Get
    '        Set(value As Boolean)
    '            _rotating = value
    '            If value Then oldPos = newPos
    '        End Set
    '    End Property
    '    Public Property isDragging As Boolean
    '        Get
    '            Return _dragging
    '        End Get
    '        Set(value As Boolean)
    '            _dragging = value
    '            If value Then
    '                '_temp_translation = Vector3.Zero
    '                oldPos = newPos
    '            Else
    '                _current_translation += _temp_translation
    '                _temp_translation = Vector3.Zero
    '            End If
    '        End Set
    '    End Property

    'Public WriteOnly Property SetupViewPortAndProjection As Size
    '    Set(value As Size)
    '        _viewport = value
    '        _projection = Matrix4.CreatePerspectiveFieldOfView(Math.PI / 4, value.Width / value.Height, 1.0F, 64.0F)
    '        GL.Viewport(_viewport)
    '        GL.MatrixMode(MatrixMode.Projection)
    '        GL.LoadMatrix(_projection)
    '    End Set
    'End Property
    'Public Shared Function UnProject(ByRef projection As Matrix4, view As Matrix4, viewport As Size, mouse As Vector2) As Vector4
    '    Dim vec As Vector4

    '    vec.X = 2.0F * mouse.X / Convert.ToSingle(viewport.Width) - 1
    '    vec.Y = -(2.0F * mouse.Y / Convert.ToSingle(viewport.Height) - 1)
    '    vec.Z = 0
    '    vec.W = 1.0F

    '    Dim viewInv As Matrix4 = Matrix4.Invert(view)
    '    Dim projInv As Matrix4 = Matrix4.Invert(projection)

    '    vec = Vector4.Transform(vec, projInv)
    '    vec = Vector4.Transform(vec, viewInv)

    '    If vec.W > Single.Epsilon Or vec.W < Single.Epsilon Then
    '        vec.X /= vec.W
    '        vec.Y /= vec.W
    '        vec.Z /= vec.W
    '    End If

    '    Return vec
    'End Function

    'Public Sub New(size As Size)
    '    SetupViewPortAndProjection = size
    'End Sub

    'Public Sub Zoom(ByVal value As Single)
    '    ' na linii miedzy _eye a _target
    '    Throw New NotImplementedException

    'End Sub

    'Private Sub arcBall()
    '    'Arc ball rotation using Quaternion
    '    ' http://cgmath.blogspot.de/2009/03/arc-ball-rotation-using-quaternion.html


    '    Dim vtFrom As Vector3 = UnProject(_projection, _modelview, _viewport, oldPos).Xyz
    '    Dim vtTo As Vector3 = UnProject(_projection, _modelview, _viewport, newPos).Xyz
    '    'vtFrom.Y = -vtFrom.Y
    '    'vtTo.Y = -vtTo.Y
    '    vtFrom.Normalize()
    '    vtTo.Normalize()

    '    Dim vAxis As Vector3 = Vector3.Cross(vtFrom, vtTo)
    '    vAxis.Normalize()

    '    Dim theta As Single = Math.Acos(Vector3.Dot(vtFrom, vtTo))
    '    If theta > 0.001F Then
    '        Dim qTmp As Quaternion = Quaternion.FromAxisAngle(vAxis, theta)
    '        qTmp.Normalize()

    '        _current_rotation *= qTmp
    '        _current_rotation.Normalize()
    '        oldPos = newPos
    '    End If
    'End Sub

    'Private Sub Drag()
    '    Dim vtFrom As Vector3 = UnProject(_projection, _modelview, _viewport, oldPos).Xyz
    '    Dim vtTo As Vector3 = UnProject(_projection, _modelview, _viewport, newPos).Xyz
    '    _temp_translation = vtTo
    '    oldPos = newPos
    'End Sub

    'Public Sub SetCamera()
    '    If _dragging Then
    '        Drag()
    '    End If
    '    If _rotating Then
    '        arcBall()
    '    End If
    '    GL.MatrixMode(MatrixMode.Modelview)
    '    GL.LoadIdentity()
    '    GL.LoadMatrix(_modelview)
    '    Dim vectora As Vector4 = _current_rotation.ToAxisAngle()
    '    '_current_rotation.ToAxisAngle(vectora.Xyz, vectora.W)
    '    'GL.Translate(_current_translation)
    '    'GL.Translate(_temp_translation)
    '    GL.Rotate(vectora.W * (rad2angle) * 4, vectora.Xyz)
    'End Sub
End Class

Public Class Camera
    '    Private Transform As Matrix4 = Matrix4.Identity
    '    Private lastRotation As Matrix3 = Matrix3.Identity
    '    Private thisRotation As Matrix3 = Matrix3.Identity
    '    Private mousePt As PointF
    '    Private isLClicked As Boolean = False
    '    Private isRClicked As Boolean = False
    '    Private isDragging As Boolean = False

    '    Public Position As Vector3 = Vector3.Zero
    '    Public Orientation As Vector3 = New Vector3(Math.PI, 0.0F, 0.0F)
    '    Public MoveSpeed As Single = 0.2F
    '    Public MouseSensitivity As Single = 0.01F
    '    Public Function GetViewMatrix() As Matrix4
    '        Dim lookat As Vector3 = New Vector3(0, 0, -1.0F)

    '        lookat.X = (Math.Sin(Orientation.X) * Math.Cos(Orientation.Y))
    '        lookat.Y = Math.Sin(Orientation.Y)
    '        lookat.Z = (Math.Cos(Orientation.X) * Math.Cos(Orientation.Y))

    '        Return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY)
    '    End Function
    '    Public Sub Move(x As Single, y As Single, z As Single)
    '        Dim offset As Vector3 = New Vector3()

    '        Dim forward As Vector3 = New Vector3(Math.Sin(Orientation.X), 0, Math.Cos(Orientation.X))
    '        Dim Right As Vector3 = New Vector3(-forward.Z, 0, forward.X)

    '        offset += x * Right
    '        offset += y * forward
    '        offset.Y += z

    '        offset.NormalizeFast()
    '        offset = Vector3.Multiply(offset, MoveSpeed)

    '        Position += offset
    '    End Sub
    '    Public Sub AddRotation(x As Single, y As Single)
    '        x = x * MouseSensitivity
    '        y = y * MouseSensitivity

    '        Orientation.X = (Orientation.X + x) Mod (Math.PI * 2.0F)
    '        Orientation.Y = Math.Max(Math.Min(Orientation.Y + y, Math.PI / 2.0F - 0.1F), -Math.PI / 2.0F + 0.1F)
    '    End Sub
End Class

