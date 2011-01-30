
Public Class Kotak

    ''' <summary>
    ''' Width
    ''' </summary>
    Private FWidth As Single
    ''' <summary>
    ''' Height --&gt; the height of box, not height from origin
    ''' </summary>
    Private FHeight As Single
    ''' <summary>
    ''' Depth
    ''' </summary>
    Private FDepth As Single
    Private FOrientation As Boolean
    ''' <summary>
    ''' Coordinate position of rectangle
    ''' </summary>
    Private FPos As Point3D
    ''' <summary>
    ''' Coordinate position 2
    ''' </summary>
    Private FPos2 As Point3D
    Private FPointerBox As Integer
    ''' <summary>
    ''' Define Cuboid/Box --&gt; true = cuboid, false = box
    ''' </summary>
    Private FDefineCuboid As Boolean
    ''' <summary>
    ''' Standard input data rectangle
    ''' </summary>
    Sub New(ByVal width As Single, ByVal depth As Single, ByVal height As Single)
        FPointerBox = 0

        FWidth = width
        FDepth = depth
        FHeight = height

        If width >= depth Then
            FOrientation = True
        Else
            FOrientation = False
        End If

        FPos = New Point3D(0, 0, 0)
        UpdatePosition()
    End Sub

    ''' <summary>
    ''' Full input data rectangle
    ''' </summary>
    Sub New(ByVal pointer As Integer, ByVal definecuboid As Boolean, ByVal width As Single, ByVal depth As Single, ByVal height As Single, ByVal pos As Point3D)
        FPointerBox = pointer
        FDefineCuboid = definecuboid

        FWidth = width
        FDepth = depth
        FHeight = height

        If width >= depth Then
            FOrientation = True
        Else
            FOrientation = False
        End If

        FPos = pos
        UpdatePosition()
    End Sub

    ''' <summary>
    ''' Get and set boolean
    ''' </summary>
    Public Property Orientation() As Boolean
        Get
            Return FOrientation
        End Get
        Set(ByVal Value As Boolean)
            FOrientation = Value
            UpdateOrientation()
            UpdatePosition()
        End Set
    End Property

    ''' <summary>
    ''' Get position
    ''' </summary>
    Public Property Position() As Point3D
        Get
            Return FPos
        End Get
        Set(ByVal Value As Point3D)
            FPos = Value
            UpdatePosition()
        End Set
    End Property

    ''' <summary>
    ''' Get position2
    ''' </summary>
    Public ReadOnly Property Position2() As Point3D
        Get
            Return FPos2
        End Get
    End Property

    Public ReadOnly Property Depth() As Single
        Get
            Return FDepth
        End Get
    End Property

    Public Property Height() As Single
        Get
            Return FHeight
        End Get
        Set(ByVal Value As Single)
            FHeight = Value
        End Set
    End Property

    Public ReadOnly Property Width() As Single
        Get
            Return FWidth
        End Get
    End Property

    ''' <summary>
    ''' Pointer
    ''' </summary>
    Public ReadOnly Property Pointer() As Integer
        Get
            Return FPointerBox
        End Get
    End Property

    ''' <summary>
    ''' Defining cuboid or single box
    ''' </summary>
    Public ReadOnly Property DefineCuboid() As Boolean
        Get
            Return FDefineCuboid
        End Get
    End Property

    ''' <summary>
    ''' Update orientation
    ''' </summary>
    Private Sub UpdateOrientation()
        If ((FOrientation = True) And (FDepth > FWidth)) Or _
            ((FOrientation = False) And (FWidth > FDepth)) Then
            procSwap(FDepth, FWidth)
        End If
    End Sub

    ''' <summary>
    ''' Update position
    ''' </summary>
    Private Sub UpdatePosition()
        FPos2 = New Point3D(FPos.X + FDepth, FPos.Y + FWidth, FHeight)
    End Sub

    ''' <summary>
    ''' Cek it's same area or not
    ''' </summary>
    Public Function IsEqualTo(ByVal areaCompare As Kotak) As Boolean
        With areaCompare
            If ((FDepth = .Depth) And (FHeight = .Height) And (FWidth = .Width)) AndAlso _
                ((FOrientation = .Orientation) And (FPos.IsEqualTo(.Position)) And (FPos2.IsEqualTo(.FPos2))) Then
                Return True
            Else
                Return False
            End If
        End With
    End Function



End Class
