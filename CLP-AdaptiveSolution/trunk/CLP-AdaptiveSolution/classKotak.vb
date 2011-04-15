Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class Kotak
    ''' <summary>
    ''' Width
    ''' </summary>
    Private fWidth As Single

    ''' <summary>
    ''' Height --> the height of box, not height from origin
    ''' </summary>
    Private fHeight As Single

    ''' <summary>
    ''' Depth
    ''' </summary>
    Private fDepth As Single

    Private fOrientation As Boolean

    ''' <summary>
    ''' Coordinate position of rectangle
    ''' </summary>
    Private fPos As Point3D

    ''' <summary>
    ''' Coordinate position 2
    ''' </summary>
    Private fPos2 As Point3D

    Private fPointerBox As Integer

    ''' <summary>
    ''' Define Cuboid/Box --&gt; true = cuboid, false = box
    ''' </summary>
    Private fDefineCuboid As Boolean

    ''' <summary>
    ''' Standard input data rectangle
    ''' </summary>
    Sub New(ByVal width As Single, ByVal depth As Single, ByVal height As Single)
        fPointerBox = 0

        fWidth = width
        fDepth = depth
        fHeight = height

        If width >= depth Then
            fOrientation = True
        Else
            fOrientation = False
        End If

        fPos = New Point3D(0, 0, 0)
        UpdatePosition()
    End Sub

    ''' <summary>
    ''' Full input data rectangle
    ''' </summary>
    Sub New(ByVal pointer As Integer, _
            ByVal definecuboid As Boolean, _
            ByVal width As Single, _
            ByVal depth As Single, _
            ByVal height As Single, _
            ByVal pos As Point3D)
        fPointerBox = pointer
        fDefineCuboid = definecuboid

        fWidth = width
        fDepth = depth
        fHeight = height

        If width >= depth Then
            fOrientation = True
        Else
            fOrientation = False
        End If

        fPos = pos
        UpdatePosition()
    End Sub

    ''' <summary>
    ''' Clone data rectangle
    ''' </summary>
    Sub New(ByVal masterKotak As Kotak)
        '//DefineCuboid
        fDefineCuboid = masterKotak.fDefineCuboid
        '//Depth
        fDepth = masterKotak.fDepth
        '//Height
        fHeight = masterKotak.fHeight
        '//Width
        fWidth = masterKotak.fWidth
        '//Orientation
        fOrientation = masterKotak.fOrientation
        '//PointerBox
        fPointerBox = masterKotak.fPointerBox
        '//Pos
        fPos = New Point3D(masterKotak.fPos)
        '//Pos2
        fPos2 = New Point3D(masterKotak.fPos2)
    End Sub

    ''' <summary>
    ''' Get and set boolean
    ''' </summary>
    Public Property Orientation() As Boolean
        Get
            Return fOrientation
        End Get
        Set(ByVal Value As Boolean)
            fOrientation = Value
            UpdateOrientation()
            UpdatePosition()
        End Set
    End Property

    ''' <summary>
    ''' Get position
    ''' </summary>
    Public Property Position() As Point3D
        Get
            Return fPos
        End Get
        Set(ByVal Value As Point3D)
            fPos = Value
            UpdatePosition()
        End Set
    End Property

    ''' <summary>
    ''' Get position2
    ''' </summary>
    Public ReadOnly Property Position2() As Point3D
        Get
            Return fPos2
        End Get
    End Property

    Public ReadOnly Property Depth() As Single
        Get
            Return fDepth
        End Get
    End Property

    Public Property Height() As Single
        Get
            Return fHeight
        End Get
        Set(ByVal Value As Single)
            fHeight = Value
        End Set
    End Property

    Public ReadOnly Property Width() As Single
        Get
            Return fWidth
        End Get
    End Property

    ''' <summary>
    ''' Pointer
    ''' </summary>
    Public ReadOnly Property Pointer() As Integer
        Get
            Return fPointerBox
        End Get
    End Property

    ''' <summary>
    ''' Defining cuboid or single box
    ''' </summary>
    Public ReadOnly Property DefineCuboid() As Boolean
        Get
            Return fDefineCuboid
        End Get
    End Property

    ''' <summary>
    ''' Transform kotak structure as simple 'rectangle' structure
    ''' </summary>
    Public ReadOnly Property Rectangle() As Rectangle
        Get
            Return New Rectangle(Me.fPos.X, Me.fPos2.Y, Me.Depth, Me.Width)
        End Get
    End Property

    ''' <summary>
    ''' Update orientation
    ''' </summary>
    Private Sub UpdateOrientation()
        If ((fOrientation = True) And (fDepth > fWidth)) Or _
            ((fOrientation = False) And (fWidth > fDepth)) Then
            procSwap(fDepth, fWidth)
        End If
    End Sub

    ''' <summary>
    ''' Update position
    ''' </summary>
    Private Sub UpdatePosition()
        fPos2 = New Point3D(fPos.X + fDepth, fPos.Y + fWidth, fHeight)
    End Sub

    ''' <summary>
    ''' Cek it's same area or not
    ''' </summary>
    Public Function IsEqualTo(ByVal areaCompare As Kotak) As Boolean
        With areaCompare
            If ((fDepth = .Depth) And (fHeight = .Height) And (fWidth = .Width)) AndAlso _
                ((fOrientation = .Orientation) And (fPos.IsEqualTo(.Position)) And (fPos2.IsEqualTo(.fPos2))) Then
                Return True
            Else
                Return False
            End If
        End With
    End Function
End Class
