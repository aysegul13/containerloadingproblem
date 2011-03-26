Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Enum RotateHorizontal    'rotation in horizontal direction
    Left = -1       'in enum, -1 means left
    Center = 0
    Right = 1
End Enum

Public Enum RotateVertical      'rotation in vertical direction
    Up = -1         'in enum, -1 means up
    Center = 0
    Down = 1
End Enum

Public Enum CubeSides
    'side of cube, as we know, cube has 6 side, let say it: left,...,back
    'only orientation, so it's just a name...
    Left
    Right
    Top
    Bottom
    Front
    Back
End Enum

'cube is build by side, side build by point..
'to make multiple cube that arrange into order, we need to allocate the cube right on to position (x,y) first..
'definition of position is made from calculation (see that)
'it's quite easy.
'---
'how to...
'0 learn about position x,y; then the calculation --FINISH: all you need to do is fix the location, so.. transform location 3D --> location 2D
'1 build the container first (only: front, left, bottom) --FINISH: container has been installed, is quite easy to call container wall
'2 improve the class to transform 3D into position in 2D
'   cara paling ok adalah
'   -normalisasi dulu sesuai dengan keadaan..
'   -abis itu baru dibenerin lokasi nya...
'   -titik origin mending ditempatin ditengah kiri...
Public Class Cube
    Private FLocation As Point  'coordinate location of cube
    Private FHeight As Integer  'box:   height
    Private FWidth As Integer   '       width
    Private FDepth As Integer   '       depth
    Private FCenter As Point    '       center

    Private FPath As GraphicsPath       'path box
    Private FContainer As GraphicsPath  'path container

    Private FRotateX As RotateHorizontal
    Private FRotateY As RotateVertical


    'define property of box...
    Public Property Location() As Point 'location as point, how to get and how to update.
        Get
            Return FLocation
        End Get
        Set(ByVal Value As Point)
            FLocation = Value
        End Set
    End Property

    Public Property Height() As Integer
        Get
            Return FHeight
        End Get
        Set(ByVal Value As Integer)
            FHeight = Value
        End Set
    End Property

    Public Property Width() As Integer
        Get
            Return FWidth
        End Get
        Set(ByVal Value As Integer)
            FWidth = Value
        End Set
    End Property

    Public Property Depth() As Integer
        Get
            Return FDepth
        End Get
        Set(ByVal Value As Integer)
            FDepth = Value
        End Set
    End Property

    Public Property Center() As Point
        Get
            Return FCenter
        End Get
        Set(ByVal Value As Point)
            FCenter = Value
        End Set
    End Property

    '---property readonly
    Public ReadOnly Property Path() As GraphicsPath 'fpath just show not set...
        Get
            Return FPath
        End Get
    End Property

    Public Property RotateX() As RotateHorizontal
        Get
            Return FRotateX
        End Get
        Set(ByVal Value As RotateHorizontal)
            FRotateX = Value
        End Set
    End Property

    Public Property RotateY() As RotateVertical
        Get
            Return FRotateY
        End Get
        Set(ByVal Value As RotateVertical)
            FRotateY = Value
        End Set
    End Property

    Public ReadOnly Property X() As Integer
        Get
            Return FLocation.X
        End Get
    End Property

    Public ReadOnly Property Y() As Integer
        Get
            Return FLocation.Y
        End Get
    End Property
    '---end property readonly


    '---set another properties... include ethe calculation
    '---
    'return the rectangle that bounds the entire polygon
    'representing the cube
    Public ReadOnly Property BoundsRect() As Rectangle
        Get                                                         'how to get value for bouds rect... if fpath no nothing, then no rectangles
            If (FPath Is Nothing) Then                              'if fpath get, then there's a bound draw rectangles...
                Return New Rectangle(0, 0, 0, 0)
            Else
                Dim r As RectangleF = Path.GetBounds()
                ' Implicit conversion from single to integer,
                ' really only available in VB
                Return New Rectangle(r.X, r.Y, r.Width, r.Height)
            End If
        End Get
    End Property

    'Public Property Size() As CubeSize                              'note defined yet for size of cube
    '    Get
    '        Return New cubeSize(FWidth, FHeight, FDepth)
    '    End Get
    '    Set(ByVal Value As CubeSize)
    '        FWidth = Value.cWidth
    '        FHeight = Value.cHeight
    '        FDepth = Value.cDepth
    '    End Set
    'End Property

    Public ReadOnly Property Item(ByVal Index As CubeSides) As Point()  'item??? define property item...
        Get
            Select Case Index                                       'index for side, from enumeration technique
                Case CubeSides.Back
                    Return Back
                Case CubeSides.Front
                    Return Front
                Case CubeSides.Left
                    Return Left
                Case CubeSides.Right
                    Return Right
                Case CubeSides.Top
                    Return Top
                Case CubeSides.Bottom
                    Return Bottom
                Case Else
                    Return Front
            End Select
        End Get
    End Property

    Public ReadOnly Property Top() As Point()
        Get
            Return GetTop(Location, Height, Width, Depth, RotateX, RotateY)     'what is get top??? find in function method
        End Get
    End Property

    Public ReadOnly Property Bottom() As Point()
        Get
            Return GetBottom(Location, Height, Width, Depth, RotateX, RotateY)
        End Get
    End Property

    Public ReadOnly Property Left() As Point()
        Get
            Return GetLeft(Location, Height, Width, Depth, RotateX, RotateY)
        End Get
    End Property

    Public ReadOnly Property Right() As Point()
        Get
            Return GetRight(Location, Height, Width, Depth, RotateX, RotateY)
        End Get
    End Property

    Public ReadOnly Property Front() As Point()
        Get
            Return GetFront(Location, Height, Width, Depth, RotateX, RotateY)
        End Get
    End Property

    Public ReadOnly Property Back() As Point()
        Get
            Return GetBack(Location, Height, Width, Depth, RotateX, RotateY)
        End Get
    End Property


    'constructor of structure CUBE
    Public Sub New(ByVal X As Integer, ByVal Y As Integer, _
        ByVal height As Integer, ByVal width As Integer, ByVal depth As Integer, _
        ByVal rotateX As RotateHorizontal, ByVal rotateY As RotateVertical)

        FPath = New GraphicsPath                'set the value...
        FContainer = New GraphicsPath

        FLocation = New Point(X, Y)
        FHeight = height
        FWidth = width
        FDepth = depth
        FRotateX = rotateX
        FRotateY = rotateY
        FCenter = New Point(Location.X + (width + depth / 2 * rotateX) / 2, _
                            Location.Y + (height + depth / 2 * rotateY) / 2)
        'after defining the value, construct the path.
        ConstructPath()
        'ConstructContainer()
    End Sub

    Public Sub New(ByVal point As Point, _
                ByVal height As Integer, ByVal width As Integer, ByVal depth As Integer, _
                ByVal rotateX As RotateHorizontal, ByVal rotateY As RotateVertical)

        FPath = New GraphicsPath
        FContainer = New GraphicsPath

        FLocation = point
        FHeight = height
        FWidth = width
        FDepth = depth
        FRotateX = rotateX
        FRotateY = rotateY
        FCenter = New Point(Location.X + (width + depth / 2 * rotateX) / 2, _
                            Location.Y + (height + depth / 2 * rotateY) / 2)
        ConstructPath()
        'ConstructContainer()
    End Sub

    Public Sub New(ByVal x As Integer, ByVal Y As Integer, _
            ByVal height As Integer, ByVal width As Integer, ByVal depth As Integer)

        FPath = New GraphicsPath
        FContainer = New GraphicsPath

        FLocation = New Point(x, Y)
        FHeight = height
        FWidth = width
        FDepth = depth
        FRotateX = RotateHorizontal.Right
        FRotateY = RotateVertical.Up
        FCenter = New Point(Location.X + (width + depth / 2 * RotateX) / 2, _
                            Location.Y + (height + depth / 2 * RotateY) / 2)
        ConstructPath()
        'ConstructContainer()
    End Sub

    Public Sub New(ByVal point As Point, _
                    ByVal height As Integer, ByVal width As Integer, ByVal depth As Integer)

        FPath = New GraphicsPath
        FContainer = New GraphicsPath

        FLocation = point
        FHeight = height
        FWidth = width
        FDepth = depth
        FRotateX = RotateHorizontal.Right
        FRotateY = RotateVertical.Up
        FCenter = New Point(Location.X + (width + depth / 2 * RotateX) / 2, _
                            Location.Y + (height + depth / 2 * RotateY) / 2)
        ConstructPath()
        'ConstructContainer()
    End Sub

    'clone
    Sub New(ByVal cloneCube As Cube)
        FPath = New GraphicsPath
        FContainer = New GraphicsPath

        FLocation = New Point(cloneCube.FLocation.X, cloneCube.FLocation.Y)
        FHeight = cloneCube.FHeight
        FWidth = cloneCube.FWidth
        FDepth = cloneCube.FDepth
        FRotateX = cloneCube.FRotateX
        FRotateY = cloneCube.FRotateY
        FCenter = New Point(cloneCube.FCenter)
        ConstructPath()
    End Sub

    Private Sub Changed()
        ConstructPath()
        'ConstructContainer()
    End Sub


    'behaviour about construction the container
    'Private Sub ConstructContainer()
    '    'to build container list: front, bottom, left
    '    'container face to "BACK"

    '    FContainer = New GraphicsPath
    '    FContainer.AddLines(GetFront(Location, Height, Width, Depth, RotateX, RotateY))
    '    FContainer.AddLines(GetBottom(Location, Height, Width, Depth, RotateX, RotateY))
    '    FContainer.AddLines(GetLeft(Location, Height, Width, Depth, RotateX, RotateY))
    'End Sub

    'behaviour about construction the cube
    Private Sub ConstructPath()
        FPath = New GraphicsPath
        'construction means draw the sides of cube, which is back-front, top-bottom, left-right
        FPath.AddLines(GetBack(Location, Height, Width, Depth, RotateX, RotateY))
        FPath.AddLines(GetFront(Location, Height, Width, Depth, RotateX, RotateY))
        FPath.AddLines(GetTop(Location, Height, Width, Depth, RotateX, RotateY))
        FPath.AddLines(GetBottom(Location, Height, Width, Depth, RotateX, RotateY))
        FPath.AddLines(GetLeft(Location, Height, Width, Depth, RotateX, RotateY))
        FPath.AddLines(GetRight(Location, Height, Width, Depth, RotateX, RotateY))

        FContainer = New GraphicsPath
        FContainer.AddLines(GetFront(Location, Height, Width, Depth, RotateX, RotateY))
        FContainer.AddLines(GetBottom(Location, Height, Width, Depth, RotateX, RotateY))
        'FContainer.AddLines(GetRight(Location, Height, Width, Depth, RotateX, RotateY))
        FContainer.AddLines(GetLeft(Location, Height, Width, Depth, RotateX, RotateY))
        'FContainer.AddLines(GetBack(Location, Height, Width, Depth, RotateX, RotateY))
    End Sub

    Private Function GetXFromCenter(ByVal newCenter As Point) As Integer
        Return newCenter.X - (FWidth + FDepth / 2 * FRotateX) / 2
    End Function

    Private Function GetYFromCenter(ByVal newCenter As Point) As Integer
        Return newCenter.Y - (FHeight + FDepth / 2 * FRotateY) / 2
    End Function

    Public Sub FilleRectangle(ByVal boundingRect As Rectangle)
        Dim x As Integer
        If (FRotateX = RotateHorizontal.Right) Then
            x = 0
        Else
            x = Math.Abs(Depth / 2 * FRotateX)
        End If

        Dim y As Integer
        If (FRotateY = RotateVertical.Down) Then
            y = 0
        Else
            y = Math.Abs(Depth / 2 * RotateY)
        End If

        FLocation = New Point(x, y)
        FWidth = boundingRect.Width - Depth / 2 - 1
        FHeight = boundingRect.Height - Depth / 2 - 1
        ConstructPath()
        'ConstructContainer()
    End Sub

    Public Function GetCube() As GraphicsPath
        Return FPath                                'execution of draw path...
    End Function

    'Public Shared Function GetXYPos(ByVal var1 As Single, ByVal var2 As Single, ByVal rotate1 As RotateHorizontal, ByVal rotate2 As RotateVertical) As Integer
    '    Return Int(var1 + (var2 / 2 * rotate1) + (var2 / 2 * rotate2))
    'End Function

    Public Function GetContainer() As GraphicsPath
        Return FContainer
    End Function

    Public Function GetSides(ByVal theseSides As CubeSides()) As GraphicsPath
        Dim newPath As GraphicsPath = New GraphicsPath
        Dim I As Integer
        For I = 0 To theseSides.Length - 1
            newPath.AddLines(Item(theseSides(I)))
        Next I

        Return newPath
    End Function

    Public Shared Function GetXOffset(ByVal depth As Integer, ByVal rotateX As RotateHorizontal) As Integer
        Return depth / 2 * rotateX
    End Function

    Public Shared Function GetYOffset(ByVal depth As Integer, ByVal rotateY As RotateVertical) As Integer
        Return depth / 2 * rotateY
    End Function

    Public Shared Function GetTop(ByVal point As Point, _
                                  ByVal height As Integer, _
                                  ByVal width As Integer, _
                                  ByVal depth As Integer, _
                                  ByVal rotateX As RotateHorizontal, _
                                  ByVal rotateY As RotateVertical) _
                                        As Point()

        Return New Point() {New Point(point.X, point.Y), _
                                            New Point(point.X + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY)), _
                                            New Point(point.X + width + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY)), _
                                            New Point(point.X + width, point.Y), _
                                            New Point(point.X, point.Y)}
    End Function

    Public Shared Function GetLeft(ByVal point As Point, _
                                  ByVal height As Integer, _
                                  ByVal width As Integer, _
                                  ByVal depth As Integer, _
                                  ByVal rotateX As RotateHorizontal, _
                                  ByVal rotateY As RotateVertical) _
                                        As Point()

        Return New Point() {New Point(point.X, point.Y), _
                            New Point(point.X + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY)), _
                            New Point(point.X + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY) + height), _
                            New Point(point.X, point.Y + height), _
                            New Point(point.X, point.Y)}
    End Function

    Public Shared Function GetRight(ByVal point As Point, _
                                  ByVal height As Integer, _
                                  ByVal width As Integer, _
                                  ByVal depth As Integer, _
                                  ByVal rotateX As RotateHorizontal, _
                                  ByVal rotateY As RotateVertical) _
                                        As Point()

        Return New Point() {New Point(point.X + width, point.Y), _
                            New Point(point.X + width + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY)), _
                            New Point(point.X + width + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY) + height), _
                            New Point(point.X + width, point.Y + height), _
                            New Point(point.X + width, point.Y)}
    End Function

    Public Shared Function GetBottom(ByVal point As Point, _
                                  ByVal height As Integer, _
                                  ByVal width As Integer, _
                                  ByVal depth As Integer, _
                                  ByVal rotateX As RotateHorizontal, _
                                  ByVal rotateY As RotateVertical) _
                                        As Point()

        Return New Point() {New Point(point.X, point.Y + height), _
                            New Point(point.X + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY) + height), _
                            New Point(point.X + width + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY) + height), _
                            New Point(point.X + width, point.Y + height), _
                            New Point(point.X, point.Y + height)}
    End Function

    Public Shared Function GetFront(ByVal point As Point, _
                                  ByVal height As Integer, _
                                  ByVal width As Integer, _
                                  ByVal depth As Integer, _
                                  ByVal rotateX As RotateHorizontal, _
                                  ByVal rotateY As RotateVertical) _
                                        As Point()

        Return New Point() {point, _
                            New Point(point.X + width, point.Y), _
                            New Point(point.X + width, point.Y + height), _
                            New Point(point.X, point.Y + height), _
                            point}
    End Function

    Public Shared Function GetBack(ByVal point As Point, _
                                  ByVal height As Integer, _
                                  ByVal width As Integer, _
                                  ByVal depth As Integer, _
                                  ByVal rotateX As RotateHorizontal, _
                                  ByVal rotateY As RotateVertical) _
                                        As Point()

        Dim topLeft As Point = New Point(point.X + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY))
        Dim topRight As Point = New Point(point.X + width + GetXOffset(depth, rotateX), point.Y + GetYOffset(depth, rotateY))
        Dim bottomRight As Point = New Point(point.X + width + GetXOffset(depth, rotateX), point.Y + height + GetYOffset(depth, rotateY))
        Dim bottomLeft As Point = New Point(point.X + GetXOffset(depth, rotateX), point.Y + height + GetYOffset(depth, rotateY))

        Return New Point() {topLeft, topRight, bottomRight, bottomLeft, topLeft}
    End Function
End Class
