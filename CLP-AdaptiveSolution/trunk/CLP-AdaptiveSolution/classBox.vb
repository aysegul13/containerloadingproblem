''' <summary>
''' CLP Adaptive Solution - Flexible Heuristic Computation for CLP
''' Copyright (C) 2010-2011, Hardian Prabianto,
''' Production System Laboratory, Management and Industrial Engineering at Bandung Institute of Technology, Indonesia
'''
''' This library is free software; you can redistribute it and/or 
''' modify it under the terms of the GNU General Public License, 
''' Version 2, as published by the Free Software Foundation.
'''
''' This library is distributed in the hope that it will be useful, 
''' but WITHOUT ANY WARRANTY; without even the implied warranty of 
''' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
''' GNU General Public License for more details.
'''
''' +++
''' Box and Container
''' ---IMPROVEMENT:  ACCOMODATED ROTATION
''' </summary>
''' <remarks>
''' for container
''' - the coordinate, side and orientation is fix.
''' - please be aware of dimension of container. it should be fix from begining.
''' 
''' for box
''' - the dynamic orientation and side of box still must be arrange by using container origin
''' 
''' for empty space in container
''' - it could be generate by an arrangement of empty space
''' - the coordinate in container maybe fix, include the side and orientation
''' </remarks>
Public Class Box

    ''' <summary>
    ''' Type of box
    ''' </summary>
    Private fType As Integer

    ''' <summary>
    ''' One of input dimension
    ''' </summary>
    Private fDim1 As Single

    ''' <summary>
    ''' One of input dimension
    ''' </summary>
    Private fDim2 As Single

    ''' <summary>
    ''' One of input dimension
    ''' </summary>
    Private fDim3 As Single

    ''' <summary>
    ''' Width of box, depend on orientation-side
    ''' </summary>
    Private fLengthY As Single

    ''' <summary>
    ''' Depth of box, depend on orientation-side
    ''' </summary>
    Private fLengthX As Single

    ''' <summary>
    ''' Height of box, depend on orientation-side
    ''' </summary>
    Private fLengthZ As Single

    ''' <summary>
    ''' Coordinate 1 of relative position
    ''' </summary>
    Private fRelPos1 As Point3D

    ''' <summary>
    ''' Coordinate 2 of relative position
    ''' </summary>
    Private fRelPos2 As Point3D

    ''' <summary>
    ''' Coordinate 1 of absolute position in container
    ''' </summary>
    Private fAbsPos1 As Point3D

    ''' <summary>
    ''' Coordinate 2 of absolute position in container
    ''' </summary>
    Private fAbsPos2 As Point3D

    ''' <summary>
    ''' Orientation box
    ''' </summary>
    Private fOrientation As Boolean

    ''' <summary>
    ''' Side of d1-d2
    ''' </summary>
    Private fTopIsAlpha As Boolean

    ''' <summary>
    ''' Side of d1-d3
    ''' </summary>
    Private fTopIsBeta As Boolean

    ''' <summary>
    ''' Side of d2-d3
    ''' </summary>
    Private fTopIsGamma As Boolean

    ''' <summary>
    ''' Box in container
    ''' </summary>
    Private fInContainer As Boolean

    ''' <summary>
    ''' Allow orienting to side, along dim-1
    ''' True = able to rotate as up
    ''' </summary>
    Private fUpIsDim1 As Boolean

    ''' <summary>
    ''' Allow orienting to side , along dim-2
    ''' True = able to rotate as up
    ''' </summary>
    Private fUpIsDim2 As Boolean

    ''' <summary>
    ''' Allow orienting to side , along dim-3
    ''' True = able to rotate as up
    ''' </summary>
    Private fUpIsDim3 As Boolean


    ''' <summary>
    ''' Standar construction
    ''' </summary>
    ''' <param name="boxType">Type</param>
    ''' <param name="d1">Dimension1</param>
    ''' <param name="d2">Dimension2</param>
    ''' <param name="d3">Dimension3</param>
    Sub New(ByVal boxType As Integer, _
                    ByVal d1 As Single, ByVal d2 As Single, ByVal d3 As Single, _
                    ByVal r1 As Boolean, ByVal r2 As Boolean, ByVal r3 As Boolean)
        '//Insert Value
        fType = boxType

        fDim1 = d1
        fDim2 = d2
        fDim3 = d3

        fUpIsDim1 = r1
        fUpIsDim2 = r2
        fUpIsDim3 = r3

        '//Put assumption
        fRelPos1 = New Point3D(0, 0, 0)
        fAbsPos1 = New Point3D(0, 0, 0)
        fInContainer = False
        fOrientation = True

        '//Default face
        If r3 = True Then
            fTopIsAlpha = True
            fTopIsBeta = False
            fTopIsGamma = False
        ElseIf (r3 = False) And (r2 = True) Then
            fTopIsAlpha = False
            fTopIsBeta = True
            fTopIsGamma = False
        Else
            fTopIsAlpha = False
            fTopIsBeta = False
            fTopIsGamma = True
        End If

        UpdateAll()
    End Sub

    ''' <summary>
    ''' Construction for validate
    ''' Use (often) for bounding-box and empty-space
    ''' </summary>
    Sub New(ByVal boxType As Integer, _
                    ByVal depth As Single, ByVal width As Single, ByVal height As Single)
        fType = boxType

        fRelPos1 = New Point3D(0, 0, 0)
        fAbsPos1 = New Point3D(0, 0, 0)

        fDim1 = depth
        fDim2 = width
        fDim3 = height

        fUpIsDim1 = False
        fUpIsDim2 = False
        fUpIsDim3 = True

        fTopIsAlpha = True
        fTopIsBeta = False
        fTopIsGamma = False

        fInContainer = False

        If width > depth Then
            fOrientation = True
        Else
            fOrientation = False
        End If

        UpdateAll()
    End Sub

    ''' <summary>
    ''' Construction from Clone
    ''' </summary>
    Sub New(ByVal cloneBox As Box)
        fType = cloneBox.Type

        fUpIsDim1 = cloneBox.fUpIsDim1
        fUpIsDim2 = cloneBox.fUpIsDim2
        fUpIsDim3 = cloneBox.fUpIsDim3

        fDim1 = cloneBox.fDim1
        fDim2 = cloneBox.fDim2
        fDim3 = cloneBox.fDim3

        fRelPos1 = New Point3D(cloneBox.fRelPos1)
        fAbsPos1 = New Point3D(cloneBox.fAbsPos1)

        fTopIsAlpha = cloneBox.fTopIsAlpha
        fTopIsBeta = cloneBox.fTopIsBeta
        fTopIsGamma = cloneBox.fTopIsGamma

        fInContainer = cloneBox.fInContainer
        fOrientation = cloneBox.fOrientation

        UpdateAll()
    End Sub

    ''' <summary>
    ''' Depth
    ''' </summary>
    Public ReadOnly Property Depth() As Single
        Get
            Return fLengthX
        End Get
    End Property

    ''' <summary>
    ''' Width
    ''' </summary>
    Public ReadOnly Property Width() As Single
        Get
            Return fLengthY
        End Get
    End Property

    ''' <summary>
    ''' Height
    ''' </summary>
    Public ReadOnly Property Height() As Single
        Get
            Return fLengthZ
        End Get
    End Property

    ''' <summary>
    ''' Defining side alpha
    ''' </summary>
    Public Property IsAlpha() As Boolean
        Get
            Return fTopIsAlpha
        End Get
        Set(ByVal Value As Boolean)
            If (fUpIsDim3 = True) Then
                fTopIsAlpha = True
                fTopIsBeta = False
                fTopIsGamma = False
                UpdateAll()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Defining side beta
    ''' </summary>
    Public Property IsBeta() As Boolean
        Get
            Return fTopIsBeta
        End Get
        Set(ByVal Value As Boolean)
            If (fUpIsDim2 = True) Then
                fTopIsAlpha = False
                fTopIsBeta = True
                fTopIsGamma = False
                UpdateAll()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Defining side gamma
    ''' </summary>
    Public Property IsGamma() As Boolean
        Get
            Return fTopIsGamma
        End Get
        Set(ByVal Value As Boolean)
            If (fUpIsDim1 = True) Then
                fTopIsAlpha = False
                fTopIsBeta = False
                fTopIsGamma = True
                UpdateAll()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Location box temporary
    ''' </summary>
    Public Property AbsPos1() As Point3D
        Get
            Return fAbsPos1
        End Get
        Set(ByVal Value As Point3D)
            fAbsPos1 = New Point3D(Value)
            UpdateAll()
        End Set
    End Property

    ''' <summary>
    ''' Location box in container
    ''' </summary>
    Public Property RelPos1() As Point3D
        Get
            Return fRelPos1
        End Get
        Set(ByVal Value As Point3D)
            fRelPos1 = New Point3D(Value)
            UpdateAll()
        End Set
    End Property

    ''' <summary>
    ''' Box in container
    ''' </summary>
    Public Property InContainer() As Boolean
        Get
            Return fInContainer
        End Get
        Set(ByVal Value As Boolean)
            fInContainer = Value
        End Set
    End Property

    ''' <summary>
    ''' Location box 2 temporary
    ''' </summary>
    Public ReadOnly Property AbsPos2() As Point3D
        Get
            Return fAbsPos2
        End Get
    End Property

    ''' <summary>
    ''' Location box 2 in container
    ''' </summary>
    Public ReadOnly Property RelPos2() As Point3D
        Get
            Return fRelPos2
        End Get
    End Property

    ''' <summary>
    ''' Orientation of box
    ''' </summary>
    Public Property Orientation() As Boolean
        Get
            Return fOrientation
        End Get
        Set(ByVal Value As Boolean)
            fOrientation = Value
            UpdateAll()
        End Set
    End Property

    ''' <summary>
    ''' Type of box
    ''' </summary>
    Public Property Type() As Integer
        Get
            Return fType
        End Get
        Set(ByVal Value As Integer)
            fType = Value
        End Set
    End Property

    ''' <summary>
    ''' Length of dim1
    ''' </summary>
    Public Property Dim1() As Single
        Get
            Return fDim1
        End Get
        Set(ByVal Value As Single)
            fDim1 = Value
        End Set
    End Property

    ''' <summary>
    ''' Length of dim2
    ''' </summary>
    Public Property Dim2() As Single
        Get
            Return fDim2
        End Get
        Set(ByVal Value As Single)
            fDim2 = Value
        End Set
    End Property

    ''' <summary>
    ''' Length of dim3
    ''' </summary>
    Public Property Dim3() As Single
        Get
            Return fDim3
        End Get
        Set(ByVal Value As Single)
            fDim3 = Value
        End Set
    End Property

    ''' <summary>
    ''' Rotation feasibility gamma-side (dim1)
    ''' </summary>
    Public ReadOnly Property RotGamma() As Boolean
        Get
            Return fUpIsDim1
        End Get
    End Property

    ''' <summary>
    ''' Rotation feasibility beta-side (dim2)
    ''' </summary>
    Public ReadOnly Property RotBeta() As Boolean
        Get
            Return fUpIsDim2
        End Get
    End Property

    ''' <summary>
    ''' Rotation feasibility alpha-side (dim3)
    ''' </summary>
    Public ReadOnly Property RotAlpha() As Boolean
        Get
            Return fUpIsDim3
        End Get
    End Property

    ''' <param name="xx">Dimension1</param>
    ''' <param name="yy">Dimension2</param>
    ''' <param name="zz">Dimension3</param>
    Public Sub SetDim(ByVal xx As Single, ByVal yy As Single, ByVal zz As Single, _
                            ByVal r1 As Boolean, ByVal r2 As Boolean, ByVal r3 As Boolean)
        fDim1 = xx
        fDim2 = yy
        fDim3 = zz

        fUpIsDim1 = r1
        fUpIsDim2 = r2
        fUpIsDim3 = r3

        '//Select default face
        If r3 = True Then
            fTopIsAlpha = True
        ElseIf (r3 = False) And (r2 = True) Then
            fTopIsBeta = True
        Else
            fTopIsGamma = True
        End If

        UpdateAll()
    End Sub

    ''' <summary>
    ''' Update every changed that correlated
    ''' </summary>
    Public Sub UpdateAll()
        UpdateDim()
        fRelPos2 = GetCoord2(fRelPos1)
        fAbsPos2 = GetCoord2(fAbsPos1)
    End Sub

    ''' <summary>
    ''' Update dimension
    ''' </summary>
    Private Sub UpdateDim()
        Dim t1, t2, t3 As Integer

        If fTopIsAlpha = True Then
            t1 = fDim1
            t2 = fDim2
            t3 = fDim3
        ElseIf fTopIsBeta = True Then
            t1 = fDim1
            t2 = fDim3
            t3 = fDim2
        Else
            t1 = fDim2
            t2 = fDim3
            t3 = fDim1
        End If

        'return dimension
        fLengthY = t1
        fLengthX = t2
        fLengthZ = t3

        'orientation fixation
        If ((fOrientation = True) And (fLengthY < fLengthX)) Or _
            ((fOrientation = False) And (fLengthY > fLengthX)) _
                Then procSwap(fLengthY, fLengthX)
    End Sub

    ''' <summary>
    ''' Get coordinate for position-2
    ''' </summary>
    Private Function GetCoord2(ByVal pos As Point3D) As Point3D
        Return New Point3D(pos.X + fLengthX, pos.Y + fLengthY, pos.Z + fLengthZ)
    End Function
End Class


'''' <summary>
'''' Dimension construction
'''' </summary>
'Sub New(ByVal boxType As Integer, _
'                ByVal width As Single, ByVal height As Single, ByVal depth As Single, _
'                ByVal side As Byte)
'    fType = boxType

'    fRelPos1 = New Point3D(0, 0, 0)
'    fAbsPos1 = New Point3D(0, 0, 0)

'    If side = 1 Then
'        fTopIsAlpha = True
'    ElseIf side = 2 Then
'        fTopIsBeta = True
'    Else
'        fTopIsGamma = True
'    End If

'    fInContainer = False

'    If width > depth Then
'        fOrientation = True
'    Else
'        fOrientation = False
'    End If

'    fDim1 = depth
'    fDim2 = width
'    fDim3 = height
'    'FWidth = width
'    'FHeight = height
'    'FDepth = depth

'    'for basic default --> set all rotation feasible = true
'    fUpIsDim1 = True
'    fUpIsDim2 = True
'    fUpIsDim3 = True

'    UpdateAll()
'    'FPosTemp2 = GetCoordinate2(FPosTemp)
'    'FPosContainer2 = GetCoordinate2(FPosContainer)
'End Sub


''' <summary>
''' Full construction
''' </summary>
''' <param name="boxType">Type</param>
''' <param name="d1">Dimension1</param>
''' <param name="d2">Dimension2</param>
''' <param name="d3">Dimension3</param>
''' <param name="orientation">Orientation</param>
''' <param name="cekcontainer">Box in container</param>
''' <param name="alpha">Side alpha</param>
''' <param name="beta">Side beta</param>
''' <param name="gamma">Side gamma</param>
'Sub New(ByVal boxType As Integer, _
'                ByVal d1 As Single, ByVal d2 As Single, ByVal d3 As Single, _
'                ByVal orientation As Boolean, _
'                ByVal cekcontainer As Boolean, _
'                ByVal alpha As Boolean, ByVal beta As Boolean, ByVal gamma As Boolean)
'    fType = boxType

'    fRelPos1 = New Point3D(0, 0, 0)
'    fAbsPos1 = New Point3D(0, 0, 0)

'    If (alpha = False) And (beta = True) And (gamma = False) Then
'        fTopIsAlpha = False
'        fTopIsBeta = True
'        fTopIsGamma = False
'    ElseIf (alpha = False) And (beta = False) And (gamma = True) Then
'        fTopIsAlpha = False
'        fTopIsBeta = False
'        fTopIsGamma = True
'    Else
'        fTopIsAlpha = True
'        fTopIsBeta = False
'        fTopIsGamma = False
'    End If

'    fInContainer = cekcontainer
'    fOrientation = orientation

'    fDim1 = d1
'    fDim2 = d2
'    fDim3 = d3

'    '---
'    '!for basic default --> set all rotation feasible = true
'    fUpIsDim1 = True
'    fUpIsDim2 = True
'    fUpIsDim3 = True
'    '---

'    UpdateAll()
'End Sub