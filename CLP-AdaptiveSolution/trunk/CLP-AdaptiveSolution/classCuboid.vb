Option Explicit On

Imports System
Imports System.Array
Imports System.Object
Imports System.Math

''' <summary>
''' Cuboid
''' </summary>
''' <remarks>
''' Input: Box, Number of Box, Emptyspace
''' Output: Coordinate of each box in emptyspace, which is the cuboid has the optimal value
''' 
''' Process:
''' - optimize the cuboid --&gt; find the cuboid that has the largest value, based on score calculatin
''' - build cuboid manual and automatically
''' 
''' 
''' *FTolerate hasn't functioned yet...
''' </remarks>
Public Class Cuboid
    'inherits
    Inherits Placement

    ''' <summary>
    ''' Box
    ''' </summary>
    Private FBox As Box
    ''' PROCESS
    ''' maximum cuboid
    Private FCapacityBox As Integer
    ''' <summary>
    ''' Maximum number of box in X axis
    ''' </summary>
    Private FLengthX As Integer
    ''' <summary>
    ''' Maximum number of box in Y axis
    ''' </summary>
    Private FLengthY As Integer
    ''' <summary>
    ''' Maximum number of box in Z axis
    ''' </summary>
    Private FLengthZ As Integer
    ''' <summary>
    ''' Score of cuboid
    ''' </summary>
    Private FScore As Double
    ''' <summary>
    ''' Direction of box
    ''' </summary>
    Private FDirection As Char(,)
    ''' <summary>
    ''' Maximum capacity with the best orientaion & side in container
    ''' </summary>
    Private FMaxBox As Integer
    ''' <summary>
    ''' Save all volume box
    ''' </summary>
    Private FVolBox() As Integer
    ''' <summary>
    ''' Position of each box in cuboid
    ''' </summary>
    Private FCoordBox() As Point3D
    ''' <summary>
    ''' Input number of box that will used
    ''' </summary>
    Private FNumberBox As Integer       'number box
    ''' <summary>h
    ''' Number of box used in cuboid
    ''' </summary>
    Private FUsedBox As Integer
    ''' <summary>
    ''' Number of box not used
    ''' </summary>
    Private FFreeBox As Integer
    ''' <summary>
    ''' Allow toleration
    ''' </summary>
    Private FTolerate As Boolean
    ''' <summary>
    ''' Pointing box and coordinate
    ''' </summary>
    Private FPointerBox() As Integer




    ''' <summary>
    ''' Simple constructor data
    ''' </summary>
    Sub New(ByVal DEmpty As Box, ByVal DBox As Box, ByVal DCount As Integer)
        'input data
        FBox = New Box(DBox)
        FNumberBox = DCount
        FEmptySpace = New Box(DEmpty)

        'process data
        FUsedBox = 0        'set 0 as initial value
        FFreeBox = FNumberBox   'set FNOBOX as initial value

        GetDirection()
        ReDim FVolBox(6)
    End Sub

    ''' <summary>
    ''' Default constructor data
    ''' </summary>
    Sub New(ByVal DEmpty As Box, ByVal InputBox() As Box)
        Dim i As Integer

        'input data
        FEmptySpace = New Box(DEmpty)

        ReDim FInput(InputBox.GetUpperBound(0))
        For i = 1 To InputBox.GetUpperBound(0)
            FInput(i) = New Box(InputBox(i))
        Next

        Recapitulation(FInput, FDataListInput)

        Dim bestType As Integer = GetMaxList()
        If bestType = 0 Then
            FPossiblePlacement = False
        Else
            FPossiblePlacement = True
            For i = 1 To FInput.GetUpperBound(0)
                If FInput(i).Type = bestType Then
                    FBox = New Box(FInput(i))
                    Exit For
                End If
            Next
            For i = 1 To FDataListInput.GetUpperBound(0)
                If FDataListInput(i).SType = bestType Then
                    FNumberBox = FDataListInput(i).SCount
                End If
            Next
        End If
        

        'process data
        FUsedBox = 0            'set 0 as initial value
        FFreeBox = FNumberBox   'set FNOBOX as initial value

        GetDirection()
        ReDim FVolBox(6)
    End Sub

    ''' <summary>
    ''' Set manually box
    ''' </summary>
    Public Property Box() As Box
        Get
            Return FBox
        End Get
        Set(ByVal Value As Box)
            FBox = Value
        End Set
    End Property

    ''' <summary>
    ''' Fitness value
    ''' </summary>
    Public ReadOnly Property Score() As Double
        Get
            Return FScore
        End Get
    End Property

    Public ReadOnly Property MaxBox() As Integer
        Get
            Return FMaxBox
        End Get
    End Property

    Public ReadOnly Property FreeBox() As Integer
        Get
            Return FFreeBox
        End Get
    End Property

    ''' <summary>
    ''' Number of box  that used
    ''' </summary>
    Public ReadOnly Property UsedBox() As Integer
        Get
            Return FUsedBox
        End Get
    End Property

    ''' <summary>
    ''' Output box
    ''' </summary>
    Public ReadOnly Property OutputBox() As Box()
        Get
            GetOutput()
            Return FOutput
        End Get
    End Property

    ''' <summary>
    ''' Output list
    ''' </summary>
    Public ReadOnly Property OutputList() As ListBox()
        Get
            GetOutput()
            Return FDataListOutput
        End Get
    End Property

    ''' <summary>
    ''' Coordinate for each box
    ''' </summary>
    Public ReadOnly Property PositionBoxInCuboid()
        Get
            Return FCoordBox
        End Get
    End Property

    ''' <summary>
    ''' Number of box for arrangement
    ''' </summary>
    Public ReadOnly Property NumberBox() As Integer
        Get
            Return FNumberBox
        End Get
    End Property

    ''' <summary>
    ''' Allow toleration
    ''' </summary>
    Public Property Tolerate() As Boolean
        Get
            Return FTolerate
        End Get
        Set(ByVal Value As Boolean)
            FTolerate = Value
        End Set
    End Property

    ''' <summary>
    ''' Output bounding box
    ''' </summary>
    Public ReadOnly Property OutputBoundingBox() As Box
        Get
            Return FBoundingCuboid
        End Get
    End Property

    ''' <summary>
    ''' Count size of maximum cuboid
    ''' </summary>
    Private Sub GetMaxSizeCuboid()
        'get maximum slot for each dimension emptyspace
        FLengthX = Int(FEmptySpace.Depth / FBox.Depth)
        FLengthY = Int(FEmptySpace.Width / FBox.Width)
        FLengthZ = Int(FEmptySpace.Height / FBox.Height)
        FCapacityBox = FLengthX * FLengthY * FLengthZ
    End Sub

    ''' <summary>
    ''' Count size of maximum cuboid
    ''' </summary>
    Private Sub GetMaxSizeLayer()
        'get maximum slot for each dimension emptyspace
        FLengthX = Int(FEmptySpace.Depth / FBox.Depth)
        FLengthY = Int(FEmptySpace.Width / FBox.Width)
        FLengthZ = 1
        FCapacityBox = FLengthX * FLengthY * FLengthZ
        FMaxBox = FCapacityBox
    End Sub

    ''' <summary>
    ''' Set side of cuboid automatically
    ''' </summary>
    Private Sub SetSideOrientation(ByVal side As Byte, ByVal orientation As Boolean)
        'set side of box
        If side = 1 Then
            FBox.Alpha = True
        ElseIf side = 2 Then
            FBox.Beta = True
        Else
            FBox.Gamma = True
        End If
        'set orientation of box
        FBox.Orientation = orientation

        'automatic: get maximum cuboid
        GetMaxSizeCuboid()
    End Sub

    ''' <summary>
    ''' Get single maximum cuboid based on score of cuboid --after trying all configuration
    ''' </summary>
    Public Sub GetMaxScoreCuboid(ByVal tolerate As Boolean)
        Dim i, j, k, n(3), count As Integer
        Dim direction(3) As Char
        Dim tside As Byte
        Dim torien As Boolean
        Dim tdirection As Byte

        'getting the direction
        n(1) = FLengthX
        n(2) = FLengthY
        n(3) = FLengthZ
        direction(1) = "D"
        direction(2) = "W"
        direction(3) = "H"

        'sorting --> getting list from minimum to maximum
        For i = 1 To n.GetUpperBound(0) - 1
            For j = 2 To n.GetUpperBound(0)
                If n(i) > n(j) Then
                    Swap(n(i), n(j))
                    Swap(direction(i), direction(j))
                End If
            Next
        Next

        'save best direction
        For i = 1 To n.GetUpperBound(0)
            FDirection(0, i) = direction(i)
        Next

        'calculate maximum cuboid
        FScore = GetScore(New Box())
        'Console.WriteLine("0 : " & FScore)

        count = 0
        For i = 1 To 3
            For j = 1 To 2
                If j = 1 Then
                    SetSideOrientation(i, True)
                Else
                    SetSideOrientation(i, False)
                End If

                For k = 1 To 6
                    'construct cuboid
                    FFreeBox = FNumberBox   'reset data
                    ConstructCuboid(FFreeBox, 3, FDirection(k, 3), FDirection(k, 2), FDirection(k, 1), New Point3D(0, 0, 0), True)

                    'best = maximum point
                    If FScore < GetScore(FBoundingCuboid) Then
                        FScore = GetScore(FBoundingCuboid)
                        tside = i
                        torien = True
                        tdirection = k
                        If j = 2 Then torien = False
                    End If
                    If FScore = 0 Then Exit For 'exit if minimum score = 0

                    'dump it if not necessary
                    count += 1
                    'Console.WriteLine(count & " : " & FScore & " , " & FCapacityBox)
                Next
            Next
            If FScore = 0 Then Exit For
        Next

        'set the best side & orientation
        SetSideOrientation(tside, torien)

        'construct cuboid
        FFreeBox = FNumberBox   'reset data
        ConstructCuboid(FFreeBox, 3, FDirection(tdirection, 3), FDirection(tdirection, 2), FDirection(tdirection, 1), New Point3D(0, 0, 0), True)

        'Console.WriteLine("BEST : " & FScore & " , " & FCapacityBox)

        'final calculation
        FUsedBox = FNumberBox - FFreeBox                    'calculate used box
        FScore = GetScore(FBoundingCuboid)                  'empty score
        FMethodStatus = "Max Score Cuboid"                  'give status report
    End Sub

    ''' <summary>
    ''' Get single maximum cuboid based on number of cuboid --after trying all configuration
    ''' </summary>
    Private Sub GetMaxNumberCuboid(ByVal tolerate As Boolean)
        Dim i, j As Integer
        Dim count As Byte
        Dim maxCub As Integer
        Dim tside As Byte
        Dim torien As Boolean

        'calculate maximum cuboid
        maxCub = 0
        count = 0
        For i = 1 To 3
            For j = 1 To 2
                If j = 1 Then
                    SetSideOrientation(i, True)
                Else
                    SetSideOrientation(i, False)
                End If

                count += 1
                FVolBox(count) = FCapacityBox

                'Console.WriteLine(tside & "+" & torien & "(" & count & ") :" & FLengthX & " x " & FLengthY & " x " & FLengthZ & " = " & FCapacityBox)

                If FCapacityBox > maxCub Then
                    maxCub = FCapacityBox
                    tside = i
                    torien = True
                    If j = 2 Then torien = False
                End If
            Next
        Next

        'get the maximum cuboid
        'set the best side & orientation
        SetSideOrientation(tside, torien)
        FFreeBox = FNumberBox
        FMaxBox = FCapacityBox
        FVolBox(0) = FMaxBox

        'construct cuboid
        ConstructCuboid(FFreeBox, 3, "D", "H", "W", New Point3D(0, 0, 0), tolerate)

        'final calculation
        '--give status report
        FUsedBox = FNumberBox - FFreeBox                    'calculate used box
        FScore = GetScore(FBoundingCuboid)                  'empty score
        FMethodStatus = "Max Number Cuboid"                 'give status report
    End Sub

    ''' <summary>
    ''' Get single maximum cuboid based on number of cuboid --after trying all configuration
    ''' </summary>
    Private Sub GetMaxNumberLayer(ByVal tolerate As Boolean)
        Dim j As Integer
        Dim count As Byte
        Dim maxCub As Integer
        Dim torien As Boolean

        'calculate maximum cuboid
        maxCub = 0
        count = 0
        For j = 1 To 2
            If j = 1 Then
                FBox.Orientation = True
                GetMaxSizeLayer()
            Else
                FBox.Orientation = False
                GetMaxSizeLayer()
            End If

            count += 1
            FVolBox(count) = FCapacityBox

            'Console.WriteLine(tside & "+" & torien & "(" & count & ") :" & FLengthX & " x " & FLengthY & " x " & FLengthZ & " = " & FCapacityBox)

            If FCapacityBox > maxCub Then
                maxCub = FCapacityBox
                torien = True
                If j = 2 Then torien = False
            End If
        Next

        'get the maximum cuboid
        'set the best side & orientation
        FBox.Orientation = torien
        GetMaxSizeLayer()
        FFreeBox = FNumberBox
        FMaxBox = FCapacityBox
        FVolBox(0) = FMaxBox

        'construct cuboid
        ConstructCuboid(FFreeBox, 3, "D", "H", "W", New Point3D(0, 0, 0), tolerate)

        'final calculation
        '--give status report
        FUsedBox = FNumberBox - FFreeBox                    'calculate used box
        FScore = GetScore(FBoundingCuboid)                  'empty score
        FMethodStatus = "Max Number Cuboid"                 'give status report
    End Sub

    ''' <summary>
    ''' Get score of cuboid in emptyspace
    ''' full explanation of this function, please see the excel files
    ''' - basic idea to scoring is from benefit/ratio fitness
    ''' - see this picture
    '''     ---------------------------------------------
    '''     |   COST         |                          |
    '''     |                |                          |
    '''     |=================        BENEFIT           |
    '''     |     BOX       ||                          |
    '''     |               ||                          |
    '''     ==================---------------------------
    ''' - the placement box on left-bottom, makint 1 varation of cost and benefit
    ''' - why cost? because the narrow aisle (cost area) is more difficult to do arrangement
    ''' - why benefit? because the placement of cost (area) will generate the more empty space
    ''' </summary>
    Private Function GetScore(ByVal Cuboid As Box) As Single
        Dim cost1, cost2, benefit1, benefit2, ratio1, ratio2 As Single
        Dim cost3, benefit3, ratio3 As Single
        Dim ratioX As Single

        'calculate totalRatio of benefit/cost
        cost1 = (FEmptySpace.Width - Cuboid.Width) * Cuboid.Depth
        benefit1 = (FEmptySpace.Depth - Cuboid.Depth) * FEmptySpace.Width

        cost2 = (FEmptySpace.Depth - Cuboid.Depth) * Cuboid.Width
        benefit2 = (FEmptySpace.Width - Cuboid.Width) * FEmptySpace.Depth

        If cost1 = 0 Then
            ratio1 = benefit1
        Else
            ratio1 = benefit1 / cost1
        End If
        If cost2 = 0 Then
            ratio2 = benefit2
        Else
            ratio2 = benefit2 / cost2
        End If

        'additional, just for experiment
        cost3 = Cuboid.Depth * Cuboid.Width
        benefit3 = (FEmptySpace.Depth * FEmptySpace.Width) - cost3
        If cost3 = 0 Then
            ratio3 = benefit3
        Else
            ratio3 = benefit3 / cost3
        End If

        If (Cuboid.Depth < Cuboid.Width) And (Cuboid.Depth > 0) And (Cuboid.Width > 0) Then
            ratioX = Cuboid.Depth / Cuboid.Width
        ElseIf (Cuboid.Depth > 0) And (Cuboid.Width > 0) Then
            ratioX = Cuboid.Width / Cuboid.Depth
        Else
            ratioX = 1
        End If

        'vol box of cuboid
        Dim vol As Integer
        Dim volEmptySpace
        vol = CInt(Cuboid.Depth / FBox.Depth) * CInt(Cuboid.Height / FBox.Height) * CInt(Cuboid.Width / FBox.Width)
        volEmptySpace = FEmptySpace.Depth * FEmptySpace.Height * FEmptySpace.Width

        'normalize ratioTotal
        '1: height of cuboid
        '2: box of cuboid

        'basic function
        'Return (ratio1 + ratio2) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol))))


        'Return (ratio1 + ratio2) * (Cuboid.Height / FEmptySpace.Height) * (1 - (Abs(min(FCapacityBox, FNumberBox) - vol) / min(FCapacityBox, FNumberBox)))

        'Return (ratio1 + ratio2 + ratio3) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(min(FCapacityBox, FNumberBox) - vol))))
        'Return (ratio1 + ratio2) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(min(FCapacityBox, FNumberBox) - vol)))) * ratioX
        'Return (ratio1 + ratio2 + ratio3) * (1 / (3 ^ Int(FEmptySpace.Height / Cuboid.Height))) * ((1 / (2 ^ Abs(min(FCapacityBox, FNumberBox) - vol)))) * ratioX

        'the best
        'Return (ratio1 + ratio2 + ratio3) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol)))) * ratioX
        Return (ratio1 + ratio2 + ratio3) * ((Cuboid.Height / FEmptySpace.Height) ^ 2) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol)))) * ratioX

        'experiment (+ utilization factor)
        'Return (ratio1 + ratio2 + ratio3) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol)))) * ratioX * (vol / volEmptySpace)
        'Return (ratio1 + ratio2 + ratio3) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol)))) * ratioX * (vol / volEmptySpace)
    End Function

    ''' <summary>
    ''' Construct cuboid : 1,2,3 dimension
    ''' </summary>
    Public Sub ConstructCuboid(ByRef noBox As Integer, ByVal axis As Byte, ByVal dir1 As Char, ByVal dir2 As Char, ByVal dir3 As Char, ByVal startPos As Point3D, ByVal tolerate As Boolean)
        Dim i, n(3) As Integer
        Dim direction(3) As Char

        If axis = 1 Then Construct1AxisCuboid(noBox, axis, dir1, startPos, tolerate)
        If axis = 2 Then Construct2AxisCuboid(noBox, axis, dir1, dir2, startPos, tolerate)
        If axis = 3 Then
            If (dir1 = dir2) And (dir2 = dir3) Then
                dir3 = direction(1)
                dir2 = direction(2)
                dir1 = direction(3)
            ElseIf (dir1 = dir3) Then
                For i = 1 To direction.GetUpperBound(0)
                    direction(0) = direction(i)
                    If dir2 = direction(i) Then direction(i) = "A"
                    If dir3 = direction(i) Then direction(i) = "A"
                    If direction(i) = "A" Then direction(0) = "A"
                Next
                dir1 = direction(0)
            ElseIf (dir2 = dir3) Then
                For i = 1 To direction.GetUpperBound(0)
                    direction(0) = direction(i)
                    If dir1 = direction(i) Then direction(i) = "A"
                    If dir3 = direction(i) Then direction(i) = "A"
                    If direction(i) = "A" Then direction(0) = "A"
                Next
                dir2 = direction(0)
            ElseIf (dir1 = dir2) Then
                For i = 1 To direction.GetUpperBound(0)
                    direction(0) = direction(i)
                    If dir2 = direction(i) Then direction(i) = "A"
                    If dir3 = direction(i) Then direction(i) = "A"
                    If direction(i) = "A" Then direction(0) = "A"
                Next
                dir1 = direction(0)
            End If
            Construct3AxisCuboid(noBox, axis, dir1, dir2, dir3, startPos, tolerate)
        End If

        'finalize
        ReDim Preserve FCoordBox(FNumberBox - noBox)
        GetBoundingCuboid()
    End Sub

    ''' <summary>
    ''' Construct 1 axis cuboid
    ''' - it will be proceed if number of used box is enough to area
    ''' </summary>
    ''' <param name="noBox">no.box to arrange</param>
    ''' <param name="axis">number of dimension involved</param>
    ''' <param name="direction">direction (W, D, H) --along width, depth, height</param>
    ''' <param name="startPos">start position in container</param>
    ''' <param name="tolerate">true = tolerate the construction --bounding box not perfect</param>
    Private Sub Construct1AxisCuboid(ByRef noBox As Integer, ByVal axis As Byte, _
                                     ByVal direction As Char, ByVal startPos As Point3D, ByVal Tolerate As Boolean)
        Dim i, j, n As Integer

        If direction = "W" Then
            n = FLengthY
        ElseIf direction = "H" Then
            n = FLengthZ
        Else
            n = FLengthX
        End If

        'set array size
        j = Min(n, noBox)
        If axis = 1 Then ReDim FCoordBox(j)

        If ((Tolerate = False) And (Max(n, noBox) >= n)) Or (Tolerate = True) Then
            For i = 1 To j
                noBox -= 1
                If direction = "W" Then
                    FCoordBox(FNumberBox - noBox) = New Point3D(startPos.X, (i - 1) * FBox.Width, startPos.Z)
                ElseIf direction = "H" Then
                    FCoordBox(FNumberBox - noBox) = New Point3D(startPos.X, startPos.Y, (i - 1) * FBox.Height)
                Else
                    FCoordBox(FNumberBox - noBox) = New Point3D((i - 1) * FBox.Depth, startPos.Y, startPos.Z)
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Construct 2 axis cuboid
    ''' </summary>
    ''' <param name="noBox">no.box to arrange</param>
    ''' <param name="axis">number of dimension involved</param>
    ''' <param name="dir1">direction 1 (W, D, H) --along width, depth, height</param>
    ''' <param name="dir2">direction 2 (W, D, H) --along width, depth, height</param>
    ''' <param name="startPos">start position in container</param>
    ''' <param name="tolerate">true = tolerate the construction --bounding box not perfect</param>
    Private Sub Construct2AxisCuboid(ByRef noBox As Integer, ByVal axis As Byte, _
                                     ByVal dir1 As Char, ByVal dir2 As Char, ByVal startPos As Point3D, ByVal tolerate As Boolean)
        Dim i, n, layer As Integer

        'get number of iteration
        If dir2 = "W" Then
            n = FLengthY
            If (dir1 = dir2) And (FLengthX <= FLengthZ) Then dir1 = "D"
            If (dir1 = dir2) And (FLengthX > FLengthZ) Then dir1 = "H"
        ElseIf dir2 = "H" Then
            n = FLengthZ
            If (dir1 = dir2) And (FLengthX <= FLengthY) Then dir1 = "D"
            If (dir1 = dir2) And (FLengthX > FLengthY) Then dir1 = "W"
        Else
            n = FLengthX
            If (dir1 = dir2) And (FLengthY <= FLengthZ) Then dir1 = "W"
            If (dir1 = dir2) And (FLengthY > FLengthZ) Then dir1 = "H"
        End If

        If dir1 = "W" Then
            layer = n * FLengthY
        ElseIf dir1 = "H" Then
            layer = n * FLengthZ
        Else
            layer = n * FLengthX
        End If

        'set array size
        If axis = 2 Then ReDim FCoordBox(layer)

        For i = 1 To n
            If dir2 = "W" Then
                Construct1AxisCuboid(noBox, axis, dir1, New Point3D(startPos.X, (i - 1) * FBox.Width, startPos.Z), tolerate)
            ElseIf dir2 = "H" Then
                Construct1AxisCuboid(noBox, axis, dir1, New Point3D(startPos.X, startPos.Y, (i - 1) * FBox.Height), tolerate)
            Else
                Construct1AxisCuboid(noBox, axis, dir1, New Point3D((i - 1) * FBox.Depth, startPos.Y, startPos.Z), tolerate)
            End If

            'no toleration
            If (tolerate = False) And (noBox < (layer - ((layer / n) * i))) Then Exit For
        Next
    End Sub

    ''' <summary>
    ''' Construct 3 axis cuboid
    ''' </summary>
    ''' <param name="noBox">no.box to arrange</param>
    ''' <param name="axis">number of dimension involved</param>
    ''' <param name="dir1">direction 1 (W, D, H) --along width, depth, height</param>
    ''' <param name="dir2">direction 2 (W, D, H) --along width, depth, height</param>
    ''' <param name="dir2">direction 3 (W, D, H) --along width, depth, height</param>
    ''' <param name="startPos">start position in container</param>
    ''' <param name="tolerate">true = tolerate the construction --bounding box not perfect</param>
    Private Sub Construct3AxisCuboid(ByRef noBox As Integer, ByVal axis As Byte, _
                                 ByVal dir1 As Char, ByVal dir2 As Char, ByVal dir3 As Char, _
                                 ByVal startPos As Point3D, ByVal tolerate As Boolean)
        Dim i, n, layer As Integer

        'get number of iteration
        If dir3 = "W" Then
            n = FLengthY
        ElseIf dir3 = "H" Then
            n = FLengthZ
        Else
            n = FLengthX
        End If

        If ((dir2 = "W") And (dir1 = "H")) Or ((dir2 = "H") And (dir1 = "W")) Then
            layer = FLengthY * FLengthZ
        ElseIf ((dir2 = "D") And (dir1 = "W")) Or ((dir2 = "W") And (dir1 = "D")) Then
            layer = FLengthX * FLengthY
        Else
            layer = FLengthX * FLengthZ
        End If

        'set array size
        If axis = 3 Then ReDim FCoordBox(FCapacityBox)

        For i = 1 To n
            If dir3 = "W" Then
                Construct2AxisCuboid(noBox, axis, dir1, dir2, New Point3D(startPos.X, (i - 1) * FBox.Width, startPos.Z), tolerate)
            ElseIf dir3 = "H" Then
                Construct2AxisCuboid(noBox, axis, dir1, dir2, New Point3D(startPos.X, startPos.Y, (i - 1) * FBox.Height), tolerate)
            Else
                Construct2AxisCuboid(noBox, axis, dir1, dir2, New Point3D((i - 1) * FBox.Depth, startPos.Y, startPos.Z), tolerate)
            End If

            'no toleration
            If (tolerate = False) And (noBox < (layer - ((layer / n) * i))) Then Exit For
        Next
    End Sub

    ''' <summary>
    ''' Get bounding box around cuboid
    ''' </summary>
    Private Sub GetBoundingCuboid()
        Dim i As Integer
        Dim longWidth, longHeight, longDepth As Single

        'get maximum value
        longWidth = 0 : longHeight = 0 : longDepth = 0
        For i = 1 To FCoordBox.GetUpperBound(0)
            If longDepth < (FCoordBox(i).X + FBox.Depth) Then longDepth = FCoordBox(i).X + FBox.Depth
            If longWidth < (FCoordBox(i).Y + FBox.Width) Then longWidth = FCoordBox(i).Y + FBox.Width
            If longHeight < (FCoordBox(i).Z + FBox.Height) Then longHeight = FCoordBox(i).Z + FBox.Height
        Next

        'get the bounding box
        If (longDepth > longWidth) Then
            FBoundingCuboid = New Box(-1, longDepth, longWidth, longHeight, False)
        Else
            FBoundingCuboid = New Box(-1, longDepth, longWidth, longHeight, True)
        End If
    End Sub

    ''' <summary>
    ''' Insert direction
    ''' </summary>
    Private Sub GetDirection()
        Dim i, numberComb As Integer

        'getting combination
        'ntar kasi prosedur yang lebih baik
        numberComb = 1
        For i = 1 To 3
            numberComb *= i
        Next
        ReDim FDirection(numberComb, 3)

        FDirection(1, 1) = "W"
        FDirection(1, 2) = "D"
        FDirection(1, 3) = "H"

        FDirection(2, 1) = "W"
        FDirection(2, 2) = "H"
        FDirection(2, 3) = "D"

        FDirection(3, 1) = "H"
        FDirection(3, 2) = "D"
        FDirection(3, 3) = "W"

        FDirection(4, 1) = "H"
        FDirection(4, 2) = "W"
        FDirection(4, 3) = "D"

        FDirection(5, 1) = "D"
        FDirection(5, 2) = "W"
        FDirection(5, 3) = "H"

        FDirection(6, 1) = "D"
        FDirection(6, 2) = "H"
        FDirection(6, 3) = "W"
    End Sub

    ''' <summary>
    ''' Get optimize of cuboid in emptyspace
    ''' This is the only method to get the ebst cuboid without thinking --i hope so.
    ''' </summary>
    Public Sub GetOptimizeCuboid(ByVal tolerate As Boolean)
        If FPossiblePlacement = False Then
            FScore = 0
        Else
            Dim i, j, k, l As Integer

            Dim FFeasible(FVolBox.GetUpperBound(0)) As Integer              'box feasible of variation orientation & side
            Dim FNumberOrientation(FVolBox.GetUpperBound(0)) As Integer     'number of variation orientation & side

            Dim count As Integer                                            'counting variable

            Dim tside As Byte                                               'best side
            Dim torien As Boolean                                           'best orientation
            Dim tX, tY, tZ As Integer                                       'best length in X, Y, Z

            'find the maximum cuboid
            GetMaxNumberCuboid(tolerate)                                    'calculate the maximum box of cuboid in empty space

            '---
            FFeasible(0) = 0                'reset data first

            j = 0
            If FNumberBox < FMaxBox Then    'if numberBOX > maximumBob --> no optimization, arrange as GetMaxNumberCuboid
                'calculate score initial    'else, find the best arrangement: this section is goint to optimize it!
                FScore = GetScore(New Box())
                'Console.WriteLine("0 : " & FScore)
                count = 0

                'part#1
                'finding orientation & side that nearest value of target + the above value of target
                'iteration is all around the vol.box --all possible orientation & side
                For i = 1 To FVolBox.GetUpperBound(0)
                    'find the nearest bellow value of target
                    If FFeasible(0) <= FVolBox(i) And FVolBox(i) <= FNumberBox Then
                        FFeasible(0) = FVolBox(i)
                        FNumberOrientation(0) = i
                    End If

                    'find the above value of target
                    If FVolBox(i) > FNumberBox Then
                        j += 1
                        FFeasible(j) = FVolBox(i)
                        FNumberOrientation(j) = i
                    End If
                Next

                'recapitulation of possible arrangement & fixing the array size
                For i = 1 To FVolBox.GetUpperBound(0)
                    If FVolBox(i) = FFeasible(0) Then
                        j += 1
                        FFeasible(j) = FVolBox(i)
                        FNumberOrientation(j) = i
                    End If
                Next
                ReDim Preserve FFeasible(j)
                ReDim Preserve FNumberOrientation(j)

                'part#2
                'optimization
                For i = 1 To FFeasible.GetUpperBound(0)
                    'different action give for value bellow and above target
                    'if above target, we must find score each combination of 3 axis multiply
                    'if bellow target, we must find score only the maximum cuboid

                    If ((FNumberOrientation(i) + 1) Mod 2 = 0) Then             'set orientation & side first
                        SetSideOrientation(Int((FNumberOrientation(i) + 1) / 2), True)
                    Else
                        SetSideOrientation(Int((FNumberOrientation(i) + 1) / 2), False)
                    End If

                    'If FFeasible(i) > FNumberBox Then                               'if ABOVE target, find each combination
                    'part#3
                    'optimization for ABOVE target
                    'starting constraint combination

                    For j = 1 To Min3(FLengthX, FNumberBox, FCapacityBox)           'minimum of lengthX, number box must be arrange, maximum box in cuboid
                        For k = 1 To Min(FLengthY, CInt(Min(FNumberBox, FCapacityBox) / j) + 1)
                            For l = Min(FLengthZ, Max(1, CInt(Min(FNumberBox, FCapacityBox) / (j * k)))) To (Min(FLengthZ, Max(1, CInt(Min(FNumberBox, FCapacityBox) / (j * k)))) + 1)
                                If (j <= Min3(FLengthX, FNumberBox, FCapacityBox)) And (k <= Min3(FLengthY, FNumberBox, FCapacityBox)) And (l <= Min3(FLengthZ, FNumberBox, FCapacityBox)) _
                                    And ((tolerate = True) Or ((tolerate = False) And ((j * k * l) <= Min(FNumberBox, FFeasible(i))))) Then
                                    GetBoundingCuboid2(j, k, l)                         'construct bounding cuboid

                                    ''tolerate and not tolerate
                                    ''--> means the cuboid will be full or not
                                    ''--> if not tolerate = need re-getboundingcuboid
                                    'If tolerate = False Then
                                    '    If (max3((j - 1) * k * l, j * (k - 1) * l, j * k * (l - 1)) = ((j - 1) * k * l)) And (j > 1) Then
                                    '        j -= 1
                                    '    ElseIf (max3((j - 1) * k * l, j * (k - 1) * l, j * k * (l - 1)) = ((j * (k - 1) * l))) And (k > 1) Then
                                    '        k -= 1
                                    '    ElseIf (max3((j - 1) * k * l, j * (k - 1) * l, j * k * (l - 1)) = (j * k * (l - 1))) And (l > 1) Then
                                    '        l -= 1
                                    '    End If

                                    '    GetBoundingCuboid2(j, k, l)
                                    'End If

                                    'best = maximum point
                                    If GetScore(FBoundingCuboid) > FScore Then
                                        FScore = GetScore(FBoundingCuboid)
                                        tside = Int((FNumberOrientation(i) + 1) / 2)
                                        If ((FNumberOrientation(i) + 1) Mod 2 = 0) Then
                                            torien = True
                                        Else
                                            torien = False
                                        End If

                                        tX = j
                                        tY = k
                                        tZ = l
                                        FNumberOrientation(0) = i
                                    End If

                                    count += 1
                                    'Console.WriteLine(count & ":" & GetScore(FBoundingCuboid) & " --" & FFeasible(i) & " (" & j & " x " & k & " x " & l & " = " & j * k * l & ")")
                                End If
                            Next
                        Next
                    Next
                    'Else
                    '    GetBoundingCuboid2(FLengthX, FLengthY, FLengthZ)                 'construct bounding cuboid

                    '    'best = maximum point --> dibuat dulu ya.
                    '    If GetScore(FBoundingCuboid) > FScore Then
                    '        FScore = GetScore(FBoundingCuboid)
                    '        tside = Int((FNumberOrientation(i) + 1) / 2)
                    '        If ((FNumberOrientation(i) + 1) Mod 2 = 0) Then
                    '            torien = True
                    '        Else
                    '            torien = False
                    '        End If
                    '        tX = FLengthX
                    '        tY = FLengthY
                    '        tZ = FLengthZ
                    '        FFeasible(0) = i
                    '    End If
                    'End If

                    count += 1
                    'Console.WriteLine(count & ":" & GetScore(FBoundingCuboid) & " --" & FFeasible(i) & " (" & FLengthX & " x " & FLengthY & " x " & FLengthZ & " = " & FLengthX * FLengthY * FLengthZ & ")")
                Next

                'part#4
                'construction the cuboid

                'set the orientation
                If ((FNumberOrientation(FNumberOrientation(0)) + 1) Mod 2) = 0 Then             'set orientation & side first
                    SetSideOrientation(Int(((FNumberOrientation(FNumberOrientation(0)) + 1) / 2)), True)
                Else
                    SetSideOrientation(Int(((FNumberOrientation(FNumberOrientation(0)) + 1) / 2)), False)
                End If


                'construct cuboid
                FFreeBox = FNumberBox   'reset data
                ConstructManual(FFreeBox, tX, tY, tZ, New Point3D(0, 0, 0))

                FScore = GetScore(FBoundingCuboid)

                'Console.WriteLine("BEST:" & GetScore(FBoundingCuboid) & " --" & FFeasible(0) & " = " & tX & " x " & tY & " x " & tZ)

                'final calculation
                FUsedBox = FNumberBox - FFreeBox                    'calculate used box
                FScore = GetScore(FBoundingCuboid)     'empty score
                FMethodStatus = "Max Optimize Cuboid"                  'give status report
            End If
        End If
    End Sub

    ''' <summary>
    ''' Constrctor data manual
    ''' </summary>
    Public Sub ConstructManual(ByRef noBox As Integer, ByVal dirX As Integer, ByVal dirY As Integer, ByVal dirZ As Integer, ByVal startPos As Point3D)
        Dim i, j, k, n As Integer

        ReDim FCoordBox(dirX * dirY * dirZ)
        n = 0
        For k = 1 To dirZ
            For i = 1 To dirX
                For j = 1 To dirY
                    n += 1
                    FCoordBox(n) = New Point3D((i - 1) * FBox.Depth, (j - 1) * FBox.Width, (k - 1) * FBox.Height)
                    If n = noBox Then Exit For
                Next
                If n = noBox Then Exit For
            Next
            If n = noBox Then Exit For
        Next
        noBox = noBox - n

        'finalize
        ReDim Preserve FCoordBox(FNumberBox - noBox)
        GetBoundingCuboid()
    End Sub

    ''' <summary>
    ''' Get bounding cuboid by different method
    ''' </summary>
    Private Sub GetBoundingCuboid2(ByVal nDepth As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer)
        Dim longDepth, longHeight, longWidth As Single

        'get maximum value

        longDepth = FBox.Depth * nDepth
        longWidth = FBox.Width * nWidth
        longHeight = FBox.Height * nHeight

        'get the bounding box
        If (longDepth > longWidth) Then
            FBoundingCuboid = New Box(-1, longDepth, longWidth, longHeight, False)
        Else
            FBoundingCuboid = New Box(-1, longDepth, longWidth, longHeight, True)
        End If
    End Sub

    ''' <summary>
    ''' Get optimize of cuboid in layer space
    ''' </summary>
    Public Sub GetOptimizeLayer(ByVal tolerate As Boolean)
        Dim i, j, k, l As Integer

        Dim count As Integer                                            'counting variable

        Dim torien As Boolean                                           'best orientation
        Dim tX, tY, tZ As Integer                                       'best length in X, Y, Z

        'find the maximum layer
        GetMaxNumberLayer(tolerate)                                    'calculate the maximum box of cuboid in empty space

        '---

        j = 0
        If FNumberBox < FMaxBox Then    'if numberBOX > maximumBob --> no optimization, arrange as GetMaxNumberCuboid
            'calculate score initial    'else, find the best arrangement: this section is goint to optimize it!
            FScore = GetScore(New Box())
            'Console.WriteLine("0 : " & FScore)
            count = 0

            'part#1
            'optimization
            For i = 1 To 2
                'different action give for value bellow and above target
                'if above target, we must find score each combination of 3 axis multiply
                'if bellow target, we must find score only the maximum cuboid

                If i = 1 Then
                    FBox.Orientation = True
                    GetMaxSizeLayer()
                Else
                    FBox.Orientation = False
                    GetMaxSizeLayer()
                End If

                'part#3
                'optimization for ABOVE target
                'starting constraint combination
                For j = 1 To Min3(FLengthX, FNumberBox, FCapacityBox)           'minimum of lengthX, number box must be arrange, maximum box in cuboid
                    For k = 1 To Min(FLengthY, CInt(Min(FNumberBox, FCapacityBox) / j) + 1)
                        For l = 1 To 1
                            If (j <= Min3(FLengthX, FNumberBox, FCapacityBox)) And (k <= Min3(FLengthY, FNumberBox, FCapacityBox)) _
                                And ((tolerate = True) Or ((tolerate = False) And ((j * k * l) <= Min(FNumberBox, FCapacityBox)))) Then
                                GetBoundingCuboid2(j, k, l)                         'construct bounding cuboid

                                'best = maximum point --> dibuat dulu ya.
                                If GetScore(FBoundingCuboid) > FScore Then
                                    FScore = GetScore(FBoundingCuboid)
                                    If i = 1 Then
                                        torien = True
                                    Else
                                        torien = False
                                    End If

                                    tX = j
                                    tY = k
                                    tZ = l
                                End If

                                count += 1
                                'Console.WriteLine(count & ":" & GetScore(FBoundingCuboid) & " --" & Min(FNumberBox, FCapacityBox) & " (" & j & " x " & k & " x " & l & " = " & j * k * l & ")")
                            End If
                        Next
                    Next
                Next
                count += 1
                'Console.WriteLine(count & ":" & GetScore(FBoundingCuboid) & " --" & Min(FNumberBox, FCapacityBox) & " (" & FLengthX & " x " & FLengthY & " x " & FLengthZ & " = " & FLengthX * FLengthY * FLengthZ & ")")
            Next

            'part#4
            'construction the cuboid

            'set the orientation
            If i = 1 Then 'set orientation & side first
                FBox.Orientation = True
                GetMaxSizeLayer()
            Else
                FBox.Orientation = False
                GetMaxSizeLayer()
            End If

            'construct cuboid
            FFreeBox = FNumberBox   'reset data
            ConstructManual(FFreeBox, tX, tY, tZ, New Point3D(0, 0, 0))

            FScore = GetScore(FBoundingCuboid)

            'Console.WriteLine("BEST:" & GetScore(FBoundingCuboid) & " --" & Min(FNumberBox, FCapacityBox) & " = " & tX & " x " & tY & " x " & tZ)

            'final calculation
            FUsedBox = FNumberBox - FFreeBox                    'calculate used box
            FScore = GetScore(FBoundingCuboid)                  'empty score
            FMethodStatus = "Max Optimize Layer"                'give status report
        End If
    End Sub

    ''' <summary>
    ''' Get best box from list
    ''' </summary>
    Private Function GetMaxList() As Integer
        Dim i, j As Integer
        Dim volBest As Single
        Dim typeBest As Integer

        volBest = 0
        For i = 1 To FDataListInput.GetUpperBound(0)
            For j = 1 To FInput.GetUpperBound(0)
                If (FInput(j).Type = FDataListInput(i).SType) AndAlso ((FDataListInput(i).SCount * FInput(j).Depth * FInput(j).Height * FInput(j).Width) > volBest) AndAlso _
                    (CheckPossiblePacking(FInput(j), FEmptySpace) = True) Then
                    volBest = FDataListInput(i).SCount * FInput(j).Depth * FInput(j).Height * FInput(j).Width
                    typeBest = FDataListInput(i).SType
                    Exit For
                End If
            Next
        Next
        Return typeBest
    End Function

    ''' <summary>
    ''' Finalization data
    ''' </summary>
    Private Sub GetOutput()
        Dim i As Integer

        'pointing box first
        GetPointingBox()

        'resize as used box
        ReDim FOutput(FInput.GetUpperBound(0))
        'cloning input --> output
        For i = 1 To FInput.GetUpperBound(0)
            FOutput(i) = New Box(FInput(i))
        Next

        'revision for box in cuboid only
        For i = 1 To FUsedBox
            'cloning manual by copy of resume from FB
            If FBox.Alpha = True Then FOutput(FPointerBox(i)).Alpha = FBox.Alpha
            If FBox.Beta = True Then FOutput(FPointerBox(i)).Beta = FBox.Beta
            If FBox.Gamma = True Then FOutput(FPointerBox(i)).Gamma = FBox.Gamma
            FOutput(FPointerBox(i)).Orientation = FBox.Orientation

            FOutput(FPointerBox(i)).LocationTemp = New Point3D(FCoordBox(i).X, FCoordBox(i).Y, FCoordBox(i).Z)
            FOutput(FPointerBox(i)).InContainer = True
        Next

        'recap data
        Recapitulation(FInput, FDataListOutput)
        For i = 1 To FDataListOutput.GetUpperBound(0)
            If FDataListOutput(i).SType = FBox.Type Then
                FDataListOutput(i).SCount = FUsedBox
            Else
                FDataListOutput(i).SCount = 0
            End If
        Next
    End Sub

    ''' <summary>
    ''' Get pointing box from FCoordBox
    ''' </summary>
    Private Sub GetPointingBox()
        Dim i, j As Integer

        'resize as fcoordbox
        ReDim FPointerBox(FUsedBox)
        j = 0
        For i = 1 To FInput.GetUpperBound(0)
            If FInput(i).Type = FBox.Type Then
                j += 1
                FPointerBox(j) = i
            End If
            If j = FUsedBox Then Exit For
        Next
    End Sub

    ''' <summary>
    ''' Check possible packing in emptyspace
    ''' </summary>
    Private Function CheckPossiblePacking(ByVal dataBox As Box, ByVal dataEmpty As Box) As Boolean
        Dim i, j As Integer
        Dim cek As Boolean

        cek = False
        For i = 1 To 3
            If i = 1 Then
                dataBox.Alpha = True
            ElseIf i = 2 Then
                dataBox.Beta = True
            Else
                dataBox.Gamma = True
            End If

            For j = 1 To 2
                If j = 1 Then
                    dataBox.Orientation = True
                Else
                    dataBox.Orientation = False
                End If
                If (dataBox.Depth <= dataEmpty.Depth) And (dataBox.Width <= dataEmpty.Width) And (dataBox.Height <= dataEmpty.Height) Then
                    cek = True
                End If
                If cek = True Then Exit For
            Next
            If cek = True Then Exit For
        Next

        Return cek
    End Function
End Class

'---
'using if necessary, old method to build cuboid max --> before substitute with "constructnumbercuboidmax" --forgot the method name
'---
'Private Sub BuildCuboidMax()
'    Dim i, j, k As Integer

'    'if to much box then it's easy to make cuboid directly
'    ReDim FCoordBox(FMaxBox)     'enlarge box to maxbox
'    FUsedBox = 0
'    If FMaxBox <= FNoBox Then
'        'set the coordinate for each box --> origin point(0,0,0)
'        For i = 1 To maxX
'            For j = 1 To maxY
'                For k = 1 To maxZ
'                    FUsedBox += 1       'adding one
'                    FCoordBox(FUsedBox) = New Point3D((i - 1) * FBox.Depth, (j - 1) * FBox.Width, (k - 1) * FBox.Height)
'                Next
'            Next
'        Next
'    Else
'        'set the coordinate for each box --> origin point(0,0,0)
'        'see the iteration, the Z is on first to guarantee the cuboid made the first block from bottom to above
'        For k = 1 To maxZ
'            For i = 1 To maxX
'                For j = 1 To maxY
'                    FUsedBox += 1       'adding one
'                    FCoordBox(FUsedBox) = New Point3D((i - 1) * FBox.Depth, (j - 1) * FBox.Width, (k - 1) * FBox.Height)
'                    If FUsedBox = FMaxBox Then Exit For
'                Next
'                If FUsedBox = FMaxBox Then Exit For
'            Next
'            If FUsedBox = FMaxBox Then Exit For
'        Next
'    End If
'    FFreeBox = FMaxBox - FUsedBox
'End Sub

'Private Function GetScore(ByVal Cuboid As Box, ByVal EmptySpace As Box) As Double
'    'Dim utilTotal, utilHeight, utilWidth, utilDepth As Single
'    Dim volTotal, volHeight, volWidth, volDepth As Single
'    'Dim spaceTotal, spaceHeight, spaceWidth, spaceDepth As Single
'    Dim weightHeight, weightDepth, weightWidth As Single

'    'calculate volume
'    volTotal = (EmptySpace.Width * EmptySpace.Height * EmptySpace.Depth) - (FBox.Width * FBox.Depth * FBox.Height * FUsedBox)
'    volHeight = (EmptySpace.Height - Cuboid.Height) * Cuboid.Width * Cuboid.Width
'    volDepth = (EmptySpace.Depth - Cuboid.Depth) * EmptySpace.Width * EmptySpace.Height
'    volWidth = (EmptySpace.Width - Cuboid.Width) * EmptySpace.Depth * EmptySpace.Height

'    ''calculate utilization
'    'utilTotal = (FBox.Width * FBox.Depth * FBox.Height * FUsedBox) / (EmptySpace.Width * EmptySpace.Height * EmptySpace.Depth)
'    'utilHeight = (Cuboid.Width * Cuboid.Height * Cuboid.Depth) / (Cuboid.Width * Cuboid.Depth * EmptySpace.Height)
'    'utilWidth = (Cuboid.Width * Cuboid.Height * Cuboid.Depth) / (Cuboid.Height * Cuboid.Depth * EmptySpace.Width)
'    'utilDepth = (Cuboid.Width * Cuboid.Height * Cuboid.Depth) / (Cuboid.Height * Cuboid.Width * EmptySpace.Depth)

'    '''calculate space
'    ''spaceTotal = 1 - utilTotal
'    ''spaceHeight = 1 - utilHeight
'    ''spaceWidth = 1 - utilWidth
'    ''spaceDepth = 1 - utilDepth

'    'calculate weight version2
'    'weight show us the ratio between dimension1 and 2...
'    'it's indicated that the near 1.0 make it shape like square (like it more)
'    If (EmptySpace.Depth - Cuboid.Depth) > EmptySpace.Width Then
'        weightDepth = (EmptySpace.Depth - Cuboid.Depth) / EmptySpace.Width
'    Else
'        weightDepth = EmptySpace.Width / (EmptySpace.Depth - Cuboid.Depth)
'    End If

'    If (EmptySpace.Width - Cuboid.Width) > EmptySpace.Depth Then
'        weightWidth = (EmptySpace.Width - Cuboid.Width) / EmptySpace.Depth
'    Else
'        weightWidth = EmptySpace.Depth / (EmptySpace.Width - Cuboid.Width)
'    End If

'    If Cuboid.Width > Cuboid.Depth Then
'        weightHeight = Cuboid.Width / Cuboid.Depth
'    Else
'        weightHeight = Cuboid.Depth / Cuboid.Width
'    End If

'    ''calculate weight each component
'    'weightHeight = 1 / (Cuboid.Width * Cuboid.Height) / (((Cuboid.Width * Cuboid.Height) + ((EmptySpace.Depth - Cuboid.Depth) * EmptySpace.Width) + ((EmptySpace.Width - Cuboid.Width) * EmptySpace.Depth)))
'    'weightWidth = 1 / ((EmptySpace.Width - Cuboid.Width) * EmptySpace.Depth) / (((Cuboid.Width * Cuboid.Height) + ((EmptySpace.Depth - Cuboid.Depth) * EmptySpace.Width) + ((EmptySpace.Width - Cuboid.Width) * EmptySpace.Depth)))
'    'weightDepth = 1 / ((EmptySpace.Depth - Cuboid.Depth) * EmptySpace.Width) / (((Cuboid.Width * Cuboid.Height) + ((EmptySpace.Depth - Cuboid.Depth) * EmptySpace.Width) + ((EmptySpace.Width - Cuboid.Width) * EmptySpace.Depth)))

'    ''total score
'    ''the largest score indicated worse
'    ''find the minimum one
'    'If (Cuboid.Height = 0) And (Cuboid.Width = 0) And (Cuboid.Depth = 0) Then
'    '    spaceTotal = 1 : spaceDepth = 1 : spaceHeight = 1 : spaceWidth = 1
'    '    Return spaceTotal + weightDepth * spaceDepth + weightHeight * spaceHeight + weightWidth * spaceWidth
'    'Else
'    '    Return spaceTotal + weightDepth * spaceDepth + weightHeight * spaceHeight + weightWidth * spaceWidth
'    'End If

'    'total score
'    'find minimum one
'    If (Cuboid.Height = 0) And (Cuboid.Width = 0) And (Cuboid.Depth = 0) Then
'        If EmptySpace.Width > EmptySpace.Depth Then
'            weightHeight = EmptySpace.Width / EmptySpace.Depth
'        Else
'            weightHeight = EmptySpace.Depth / EmptySpace.Width
'        End If
'        Return volTotal ^ (weightDepth + weightHeight + weightWidth) + volDepth ^ weightDepth + volHeight ^ weightHeight + volWidth ^ weightWidth
'    Else
'        Return volTotal ^ (weightDepth + weightHeight + weightWidth) + volDepth ^ weightDepth + volHeight ^ weightHeight + volWidth ^ weightWidth
'    End If
'End Function