'Imports OpenTK
''Imports OpenTK.Graphics
''Imports OpenTK.Graphics.OpenGL
'Imports System.Drawing.Drawing2D

'Public Class ArcBallCamera

'    Public Sub New(aspectRation As Single, lookAt As Vector3)
'        Me.New(aspectRation, MathHelper.PiOver4, lookAt, Vector3.Up, 0.1F, Single.MaxValue)
'    End Sub

'    Public Sub New(aspectRatio As Single, fieldOfView As Single, lookAt As Vector3, up As Vector3, nearPlane As Single, farPlane As Single)
'        Me.m_aspectRatio = aspectRatio
'        Me.m_fieldOfView = fieldOfView
'        Me.m_lookAt = lookAt
'        Me.m_nearPlane = nearPlane
'        Me.m_farPlane = farPlane
'    End Sub

'    ''' <summary>
'    ''' Recreates our view matrix, then signals that the view matrix
'    ''' is clean.
'    ''' </summary>
'    Private Sub ReCreateViewMatrix()
'        'Calculate the relative position of the camera                        
'        m_position = Vector3.Transform(Vector3.Backward, Matrix.CreateFromYawPitchRoll(m_yaw, m_pitch, 0))
'        'Convert the relative position to the absolute position
'        m_position *= m_zoom
'        m_position += m_lookAt

'        'Calculate a new viewmatrix
'        m_viewMatrix = Matrix.CreateLookAt(m_position, m_lookAt, Vector3.Up)
'        viewMatrixDirty = False
'    End Sub

'    ''' <summary>
'    ''' Recreates our projection matrix, then signals that the projection
'    ''' matrix is clean.
'    ''' </summary>
'    Private Sub ReCreateProjectionMatrix()
'        m_projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, AspectRatio, m_nearPlane, m_farPlane)
'        projectionMatrixDirty = False
'    End Sub

'#Region "HelperMethods"

'    ''' <summary>
'    ''' Moves the camera and lookAt at to the right,
'    ''' as seen from the camera, while keeping the same height
'    ''' </summary>        
'    Public Sub MoveCameraRight(amount As Single)
'        Dim right As Vector3 = Vector3.Normalize(LookAt - Position)
'        'calculate forward
'        right = Vector3.Cross(right, Vector3.Up)
'        'calculate the real right
'        right.Y = 0
'        right.Normalize()
'        LookAt += right * amount
'    End Sub

'    ''' <summary>
'    ''' Moves the camera and lookAt forward,
'    ''' as seen from the camera, while keeping the same height
'    ''' </summary>        
'    Public Sub MoveCameraForward(amount As Single)
'        Dim forward As Vector3 = Vector3.Normalize(LookAt - Position)
'        forward.Y = 0
'        forward.Normalize()
'        LookAt += forward * amount
'    End Sub

'#End Region

'#Region "FieldsAndProperties"
'    'We don't need an update method because the camera only needs updating
'    'when we change one of it's parameters.
'    'We keep track if one of our matrices is dirty
'    'and reacalculate that matrix when it is accesed.
'    Private viewMatrixDirty As Boolean = True
'    Private projectionMatrixDirty As Boolean = True

'    Public MinPitch As Single = -MathHelper.PiOver2 + 0.3F
'    Public MaxPitch As Single = MathHelper.PiOver2 - 0.3F
'    Private m_pitch As Single
'    Public Property Pitch() As Single
'        Get
'            Return m_pitch
'        End Get
'        Set(value As Single)
'            viewMatrixDirty = True
'            m_pitch = MathHelper.Clamp(value, MinPitch, MaxPitch)
'        End Set
'    End Property

'    Private m_yaw As Single
'    Public Property Yaw() As Single
'        Get
'            Return m_yaw
'        End Get
'        Set(value As Single)
'            viewMatrixDirty = True
'            m_yaw = value
'        End Set
'    End Property

'    Private m_fieldOfView As Single
'    Public Property FieldOfView() As Single
'        Get
'            Return m_fieldOfView
'        End Get
'        Set(value As Single)
'            projectionMatrixDirty = True
'            m_fieldOfView = value
'        End Set
'    End Property

'    Private m_aspectRatio As Single
'    Public Property AspectRatio() As Single
'        Get
'            Return m_aspectRatio
'        End Get
'        Set(value As Single)
'            projectionMatrixDirty = True
'            m_aspectRatio = value
'        End Set
'    End Property

'    Private m_nearPlane As Single
'    Public Property NearPlane() As Single
'        Get
'            Return m_nearPlane
'        End Get
'        Set(value As Single)
'            projectionMatrixDirty = True
'            m_nearPlane = value
'        End Set
'    End Property

'    Private m_farPlane As Single
'    Public Property FarPlane() As Single
'        Get
'            Return m_farPlane
'        End Get
'        Set(value As Single)
'            projectionMatrixDirty = True
'            m_farPlane = value
'        End Set
'    End Property

'    Public MinZoom As Single = 1
'    Public MaxZoom As Single = Single.MaxValue
'    Private m_zoom As Single = 1
'    Public Property Zoom() As Single
'        Get
'            Return m_zoom
'        End Get
'        Set(value As Single)
'            viewMatrixDirty = True
'            m_zoom = MathHelper.Clamp(value, MinZoom, MaxZoom)
'        End Set
'    End Property


'    Private m_position As Vector3
'    Public ReadOnly Property Position() As Vector3
'        Get
'            If viewMatrixDirty Then
'                ReCreateViewMatrix()
'            End If
'            Return m_position
'        End Get
'    End Property

'    Private m_lookAt As Vector3
'    Public Property LookAt() As Vector3
'        Get
'            Return m_lookAt
'        End Get
'        Set(value As Vector3)
'            viewMatrixDirty = True
'            m_lookAt = value
'        End Set
'    End Property
'#End Region

'#Region "ICamera Members"
'    Public ReadOnly Property ViewProjectionMatrix() As Matrix
'        Get
'            Return ViewMatrix * ProjectionMatrix
'        End Get
'    End Property

'    Private m_viewMatrix As Matrix
'    Public ReadOnly Property ViewMatrix() As Matrix
'        Get
'            If viewMatrixDirty Then
'                ReCreateViewMatrix()
'            End If
'            Return m_viewMatrix
'        End Get
'    End Property

'    Private m_projectionMatrix As Matrix
'    Public ReadOnly Property ProjectionMatrix() As Matrix
'        Get
'            If projectionMatrixDirty Then
'                ReCreateProjectionMatrix()
'            End If
'            Return m_projectionMatrix
'        End Get
'    End Property
'#End Region
'End Class