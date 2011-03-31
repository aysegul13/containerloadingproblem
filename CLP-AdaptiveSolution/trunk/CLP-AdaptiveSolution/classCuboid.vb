Imports System.Math
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
''' classCuboid.vb
'''
''' Input: Box, Number of Box, Emptyspace
''' Output: Coordinate of each box in emptyspace as cuboid form which has optimal value
''' 
''' Process:
''' - optimize the cuboid --> find the cuboid that has the largest value, based on score calculating
''' - build cuboid manual and automatically
''' 
''' 
''' *FTolerate hasn't functioned yet...
''' </summary>
Public Class Cuboid
    '//Inherits
    Inherits Placement

    ''' <summary>
    ''' Box
    ''' </summary>
    Private fBox As Box

    ''' <summary>
    ''' Number of capacity box in container
    ''' </summary>
    Private fCapacityBox As Integer

    ''' <summary>
    ''' Maximum number box in X axis
    ''' </summary>
    Private fLengthX As Integer

    ''' <summary>
    ''' Maximum number box in Y axis
    ''' </summary>
    Private fLengthY As Integer

    ''' <summary>
    ''' Maximum number box in Z axis
    ''' </summary>
    Private fLengthZ As Integer

    ''' <summary>
    ''' Score of cuboid
    ''' </summary>
    Private fScore As Double

    ''' <summary>
    ''' Box direction
    ''' </summary>
    Private fDirection As Char(,)

    ''' <summary>
    ''' Maximum capacity with the best orientation & side in container
    ''' </summary>
    Private fMaxBox As Integer

    ''' <summary>
    ''' Save all volume box
    ''' </summary>
    Private fVolBox() As Integer

    ''' <summary>
    ''' Relative position box in cuboid
    ''' </summary>
    Private fCoordBox() As Point3D

    ''' <summary>
    ''' Input number box that will used
    ''' </summary>
    Private fNumberBox As Integer

    ''' <summary>h
    ''' Number box used in cuboid
    ''' </summary>
    Private fUsedBox As Integer

    ''' <summary>
    ''' Number box not used
    ''' </summary>
    Private fFreeBox As Integer

    ''' <summary>
    ''' Allow toleration
    ''' </summary>
    Private fTolerate As Boolean

    ''' <summary>
    ''' Pointing box and coordinate
    ''' </summary>
    Private fPointerBox() As Integer

    ''' <summary>
    ''' Pointing box and coordinate
    ''' </summary>
    Private fListMaxVolBox() As strBoxList

    ''' <summary>
    ''' Simple constructor data
    ''' Used (once) in dummy variable
    ''' --0. Parameter set
    ''' --1. Input dataBox
    ''' --2. Recapitulation
    ''' --3. Reset data
    ''' </summary>
    Sub New(ByVal DEmpty As Box, ByVal DBox As Box, ByVal DCount As Integer)
        '(1)
        fVolume = 0
        fCompactness = 0
        fUtilization = 0

        fBox = New Box(DBox)
        fNumberBox = DCount
        fSpace = New Box(DEmpty)

        '(2)
        algRecapitulation(fInput, fListInput)

        '(3)
        fUsedBox = 0            'set 0 as initial value
        fFreeBox = fNumberBox   'set FNOBOX as initial value
    End Sub

    ''' <summary>
    ''' #New
    ''' -Default constructor data
    ''' -Used most in programming
    ''' --0. Parameter set
    ''' --1. Input data
    ''' ---1a. Empty space
    ''' ---1b. Box
    ''' --3. Recapitulation
    ''' --4. Create max box list
    ''' </summary>
    Sub New(ByVal DEmpty As Box, ByVal InputBox() As Box)
        fVolume = 0
        fCompactness = 0
        fUtilization = 0

        '(1a)
        fSpace = New Box(DEmpty)
        '(1b)
        procBoxClone(InputBox, fInput)

        '(2)
        algRecapitulation(fInput, fListInput)

        '(3)
        '//masih harus di uji lagi takutnya banyak error
        GetMaxList()
    End Sub

    ''' <summary>
    ''' Set manually box
    ''' </summary>
    Public Property Box() As Box
        Get
            Return fBox
        End Get
        Set(ByVal Value As Box)
            fBox = Value
        End Set
    End Property

    ''' <summary>
    ''' Fitness value
    ''' </summary>
    Public ReadOnly Property Score() As Double
        Get
            Return fScore
        End Get
    End Property

    Public ReadOnly Property MaxBox() As Integer
        Get
            Return fMaxBox
        End Get
    End Property

    Public ReadOnly Property FreeBox() As Integer
        Get
            Return fFreeBox
        End Get
    End Property

    ''' <summary>
    ''' Number box  that used
    ''' </summary>
    Public ReadOnly Property UsedBox() As Integer
        Get
            Return fUsedBox
        End Get
    End Property

    ''' <summary>
    ''' Coordinate for each box
    ''' </summary>
    Public ReadOnly Property PositionBoxInCuboid()
        Get
            Return fCoordBox
        End Get
    End Property

    ''' <summary>
    ''' Number box for arrangement
    ''' </summary>
    Public ReadOnly Property NumberBox() As Integer
        Get
            Return fNumberBox
        End Get
    End Property

    ''' <summary>
    ''' Allow toleration
    ''' </summary>
    Public Property Tolerate() As Boolean
        Get
            Return fTolerate
        End Get
        Set(ByVal Value As Boolean)
            fTolerate = Value
        End Set
    End Property


    ''' <summary>
    ''' #GetOptimizeCuboid
    ''' -Get optimize all cuboid
    ''' -Finding the best one
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Preparation set
    ''' --3. Iteration to all feasible box in list
    ''' --3a. Get data box
    ''' --3b. Reset box
    ''' --4. Start optimization
    ''' --5. Record best box (score + orientation)
    ''' --6. Update list (so it don't need to calculate all)
    ''' --7. Reconstruct best box + orientaton
    ''' --8. Get output
    ''' </summary>
    Public Sub GetOptimizeCuboid(ByVal tolerate As Boolean)
        '(1)
        Dim i As Integer
        Dim bestBox As Box = New Box(fInput(1))
        Dim bestBounding As Box = New Box(fInput(1))
        Dim bestScore As Single = 0
        Dim bestUsed, bestNumber, bX, bY, bZ As Integer

        '(2)
        GetDirection()
        ReDim fVolBox(6)

        '(3)
        '//Do algorithm if there is possible placement --at least one.
        If fPossiblePlacement = True Then
            For i = 1 To fListMaxVolBox.GetUpperBound(0)
                If (fListMaxVolBox(i).SType <> -1) Then
                    '(3a)
                    fBox = New Box(fGetBoxFromList(fListMaxVolBox(i).SType, fInput))
                    '(3b)
                    fNumberBox = fListMaxVolBox(i).SCount
                    fUsedBox = 0                            'set 0 as initial value
                    fFreeBox = fNumberBox                   'set FFreeBox = FNumberBox

                    '(4)
                    GetOptimalArrangement(tolerate)

                    '(5)
                    If (bestScore < fScore) Then
                        bestBox = New Box(fBox)
                        bestScore = fScore
                        bestBounding = New Box(fBoundingBox)
                        bestNumber = fNumberBox
                        bestUsed = fUsedBox
                        bX = fLengthX
                        bY = fLengthY
                        bZ = fLengthZ
                    End If

                    '(6)
                    '//nanti bisa dibikin lah.. sekarang ditest dlu aja
                End If
            Next

            '(7)
            fBox = New Box(bestBox)
            fScore = bestScore
            fNumberBox = bestNumber
            fFreeBox = fNumberBox

            '//Set Orientation
            If fBox.IsAlpha = True Then SetRotation(1, fBox.Orientation)
            If fBox.IsBeta = True Then SetRotation(2, fBox.Orientation)
            If fBox.IsGamma = True Then SetRotation(3, fBox.Orientation)

            '//Construct cuboid
            ConstructManual(fFreeBox, _
                            bX, bY, bZ, _
                            New Point3D(0, 0, 0))
            fUsedBox = fNumberBox - fFreeBox

            'Console.WriteLine("BEST:" & GetScore(FBoundingCuboid) & " --" & FFeasible(0) & " = " & tX & " x " & tY & " x " & tZ)

            fMethod = "Max Optimized Cuboid"                     'give status report
        Else
            fScore = 0
        End If

        '(8)
        If fScore > 0 Then GetOutput()
    End Sub

    ''' <summary>
    ''' #GetOptimalArrangement
    ''' -Most important optimization program in this class
    ''' -Works automatically to find box with best dimension and orientation
    ''' -Call if possiblePlacement = true
    ''' --0. Parameter set; tolerate = false --> real cube
    ''' --1. Variable set
    ''' --2. Get maximum cuboid
    ''' --3. if numberBox >= maxBox then "construct as maxcuboid"
    ''' --3a. Construct cuboid
    ''' --3b. Finalization
    ''' --4. if numberbox < maxBox then "find best arrangement"
    ''' --4a. Find rotation that result the nearest (below & above) value of target 
    ''' --    Iteration is all around the vol.box --all possible orientation & side
    ''' --    Recapitulation result
    ''' --4b. Optimization
    ''' --    Difference below/above --> different method ??? (kayanya ga juga deh..)
    ''' --4c. Decoding value
    ''' --4d. Optimization
    ''' --4d1. Starting combination
    ''' --4d2. Construct cuboid
    ''' --4d3. Record best value
    ''' --4e. Construct best combination + orientation
    ''' --5. Finalization
    ''' </summary>
    Private Sub GetOptimalArrangement(ByVal tolerate As Boolean)
        '(1)
        Dim i, j, k, l As Integer
        Dim FFeasible(fVolBox.GetUpperBound(0)) As Integer              'box feasible of variation orientation & side
        Dim FNumberOrientation(fVolBox.GetUpperBound(0)) As Integer     'number of variation orientation & side
        Dim count As Integer                                            'counting variable
        Dim tside As Byte                                               'best side
        Dim torien As Boolean                                           'best orientation
        Dim tX, tY, tZ As Integer                                       'best length in X, Y, Z

        '(2)
        GetMaxCuboid(tolerate)

        '(3)
        If fNumberBox >= fMaxBox Then    'if numberBox > maxBox --> no optimization, arrange as GetMaxNumberCuboid
            '(3a)
            ConstructCuboid(fFreeBox, _
                            3, _
                            "D", "H", "W", _
                            New Point3D(0, 0, 0), _
                            tolerate)
            '(3b)
            fUsedBox = fNumberBox - fFreeBox    'calculate used box
            fScore = GetScore(fBoundingBox)     'empty score
            fMethod = "Max Number Cuboid"       'give status report
        Else
            '//Reset data
            FFeasible(0) = 0
            j = 0
            fScore = 0

            'Console.WriteLine("0 : " & FScore)
            count = 0

            '++
            '(4a)
            For i = 1 To fVolBox.GetUpperBound(0)
                '//Find value that bellow target
                If (FFeasible(0) <= fVolBox(i)) And (fVolBox(i) <= fNumberBox) Then
                    FFeasible(0) = fVolBox(i)
                    FNumberOrientation(0) = i
                End If
                '//Find value that above target
                If (fVolBox(i) > fNumberBox) Then
                    j += 1
                    FFeasible(j) = fVolBox(i)
                    FNumberOrientation(j) = i
                End If
            Next
            '//Recap possible arrangement + fix array size
            For i = 1 To fVolBox.GetUpperBound(0)
                If fVolBox(i) = FFeasible(0) Then
                    j += 1
                    FFeasible(j) = fVolBox(i)
                    FNumberOrientation(j) = i
                End If
            Next
            ReDim Preserve FFeasible(j)
            ReDim Preserve FNumberOrientation(j)

            '++
            '(4b)
            For i = 1 To FFeasible.GetUpperBound(0)
                '(4c)
                If ((FNumberOrientation(i) + 1) Mod 2 = 0) Then
                    SetRotation(Int((FNumberOrientation(i) + 1) / 2), True)
                Else
                    SetRotation(Int((FNumberOrientation(i) + 1) / 2), False)
                End If

                '(4d)
                '//if valus is above target --> find score from combination of 3 axis
                '//if value is bellow target --> find score in maximum cuboid
                '//starting constraint combination 
                For j = 1 To fMin3(fLengthX, _
                                   fNumberBox, _
                                   fCapacityBox)           'minimum of lengthX, number box must be arrange, maximum box in cuboid
                    For k = 1 To fMin(fLengthY, _
                                      CInt(fMin(fNumberBox, fCapacityBox) / j) + 1)
                        For l = fMin(fLengthZ, _
                                     fMax(1, CInt(fMin(fNumberBox, fCapacityBox) / (j * k)))) _
                                     To (fMin(fLengthZ, _
                                              fMax(1, CInt(fMin(fNumberBox, fCapacityBox) / (j * k)))) + 1)
                            If (j <= fMin3(fLengthX, _
                                           fNumberBox, _
                                           fCapacityBox)) And (k <= fMin3(fLengthY, _
                                                                          fNumberBox, _
                                                                          fCapacityBox)) And _
                                                                (l <= fMin3(fLengthZ, _
                                                                            fNumberBox, _
                                                                            fCapacityBox)) _
                                                        And ((tolerate = True) Or ((tolerate = False) _
                                                                And ((j * k * l) <= fMin(fNumberBox, FFeasible(i))))) Then
                                '//Construct bounding box
                                GetBoundingCuboid2(j, k, l)

                                '//Record best value
                                If GetScore(fBoundingBox) > fScore Then
                                    fScore = GetScore(fBoundingBox)
                                    tside = Int((FNumberOrientation(i) + 1) / 2)
                                    If ((FNumberOrientation(i) + 1) Mod 2 = 0) Then
                                        torien = True
                                    Else
                                        torien = False
                                    End If

                                    tX = j
                                    tY = k
                                    tZ = l
                                    FNumberOrientation(0) = FNumberOrientation(i)
                                End If

                                count += 1
                                'Console.WriteLine(count & ":" & GetScore(FBoundingCuboid) & " --" & FFeasible(i) & " (" & j & " x " & k & " x " & l & " = " & j * k * l & ")")
                            End If
                        Next
                    Next
                Next

                count += 1
                'Console.WriteLine(count & ":" & GetScore(FBoundingCuboid) & " --" & FFeasible(i) & " (" & FLengthX & " x " & FLengthY & " x " & FLengthZ & " = " & FLengthX * FLengthY * FLengthZ & ")")
            Next

            '(4e)
            '//Set Orientation
            If ((FNumberOrientation(0) + 1) Mod 2 = 0) Then
                SetRotation(Int((FNumberOrientation(0) + 1) / 2), True)
            Else
                SetRotation(Int((FNumberOrientation(0) + 1) / 2), False)
            End If
            '//Construct cuboid + calculate score
            fFreeBox = fNumberBox
            ConstructManual(fFreeBox, tX, tY, tZ, New Point3D(0, 0, 0))
            fScore = GetScore(fBoundingBox)

            'Console.WriteLine("BEST:" & GetScore(FBoundingCuboid) & " --" & FFeasible(0) & " = " & tX & " x " & tY & " x " & tZ)

            '(5)
            fLengthX = tX
            fLengthY = tY
            fLengthZ = tZ
            fUsedBox = fNumberBox - fFreeBox                    'calculate used box
            fScore = GetScore(fBoundingBox)                     'empty score
            fMethod = "Max Optimize Cuboid"                     'give status report
        End If
    End Sub

    ''' <summary>
    ''' #GetMaxSizeCuboid
    ''' -Count size of maximum cuboid
    ''' </summary>
    Private Sub GetMaxSizeCuboid()
        fLengthX = Int(fSpace.Depth / fBox.Depth)
        fLengthY = Int(fSpace.Width / fBox.Width)
        fLengthZ = Int(fSpace.Height / fBox.Height)
        fCapacityBox = fLengthX * fLengthY * fLengthZ
    End Sub

    ''' <summary>
    ''' Count size of maximum cuboid
    ''' </summary>
    Private Sub GetMaxSizeLayer()
        'get maximum slot for each dimension emptyspace
        fLengthX = Int(fSpace.Depth / fBox.Depth)
        fLengthY = Int(fSpace.Width / fBox.Width)
        fLengthZ = 1
        fCapacityBox = fLengthX * fLengthY * fLengthZ
        fMaxBox = fCapacityBox
    End Sub

    ''' <summary>
    ''' #SetRotation
    ''' -Set box to a rotation
    ''' -Rotation set based on box
    ''' --0. Parameter set
    ''' --1. Set rotation
    ''' --2. Set orientation
    ''' --3. Get max size cuboid
    ''' </summary>
    Private Sub SetRotation(ByVal side As Byte, ByVal orientation As Boolean)
        '(1)
        If (side = 1) And (fBox.RotAlpha = True) Then fBox.IsAlpha = True
        If (side = 2) And (fBox.RotBeta = True) Then fBox.IsBeta = True
        If (side = 3) And (fBox.RotGamma = True) Then fBox.IsGamma = True

        '(2)
        fBox.Orientation = orientation

        '(3)
        GetMaxSizeCuboid()
    End Sub

    ''' <summary>
    ''' #GetMaxCuboid
    ''' -Calculate single cuboid in space
    ''' -Also create coordinate for each box in cuboid
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Each orientation that feasible, calculate maximum cuboid
    ''' --2a. Set rotation and get max cuboid
    ''' --2b. Record temp.Best cuboid
    ''' --2c. Set best orientation
    ''' </summary>
    Private Sub GetMaxCuboid(ByVal tolerate As Boolean)
        '(1)
        Dim i, j As Integer
        Dim count As Byte = 0
        Dim maxCuboid As Integer = 0
        Dim tside As Byte
        Dim torien As Boolean

        '(2)
        For i = 1 To 3
            '//Reset
            fVolBox(i) = 0

            If ((i = 1) AndAlso (fBox.RotAlpha = True)) Or _
                ((i = 2) AndAlso (fBox.RotBeta = True)) Or _
                ((i = 3) AndAlso (fBox.RotGamma = True)) Then

                '(2a)
                For j = 1 To 2
                    If (j = 1) Then SetRotation(i, False)
                    If (j = 2) Then SetRotation(i, True)
                    fVolBox(i * 2 - j + 1) = fCapacityBox

                    '(2b)
                    'Console.WriteLine(tside & "+" & torien & "(" & count & ") :" & FLengthX & " x " & FLengthY & " x " & FLengthZ & " = " & FCapacityBox)
                    If fCapacityBox > maxCuboid Then
                        maxCuboid = fCapacityBox
                        tside = i
                        If j = 1 Then torien = False
                        If j = 2 Then torien = True
                    End If
                Next
            End If

        Next

        '(2c)
        SetRotation(tside, torien)
        fFreeBox = fNumberBox
        fMaxBox = fCapacityBox
        fVolBox(0) = fMaxBox
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
                fBox.Orientation = True
                GetMaxSizeLayer()
            Else
                fBox.Orientation = False
                GetMaxSizeLayer()
            End If

            count += 1
            fVolBox(count) = fCapacityBox

            'Console.WriteLine(tside & "+" & torien & "(" & count & ") :" & FLengthX & " x " & FLengthY & " x " & FLengthZ & " = " & FCapacityBox)

            If fCapacityBox > maxCub Then
                maxCub = fCapacityBox
                torien = True
                If j = 2 Then torien = False
            End If
        Next

        'get the maximum cuboid
        'set the best side & orientation
        fBox.Orientation = torien
        GetMaxSizeLayer()
        fFreeBox = fNumberBox
        fMaxBox = fCapacityBox
        fVolBox(0) = fMaxBox

        'construct cuboid
        ConstructCuboid(fFreeBox, 3, "D", "H", "W", New Point3D(0, 0, 0), tolerate)

        'final calculation
        '--give status report
        fUsedBox = fNumberBox - fFreeBox                    'calculate used box
        fScore = GetScore(fBoundingBox)                  'empty score
        fMethod = "Max Number Cuboid"                 'give status report
    End Sub

    ''' <summary>
    ''' #GetScore
    ''' -Calculate score for placement cuboid in space
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Calculate totalRatio of benefit/cost
    ''' --3. Additional calculation --> for experiment
    ''' --4. Vol box of cuboid
    ''' --5. Return value
    ''' </summary>
    ''' <remarks>
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
    ''' </remarks>
    Private Function GetScore(ByVal Cuboid As Box) As Single
        '(1)
        Dim cost1, cost2, benefit1, benefit2, ratio1, ratio2 As Single
        Dim cost3, benefit3, ratio3 As Single
        Dim ratioX As Single

        '(2)
        cost1 = (fSpace.Width - Cuboid.Width) * Cuboid.Depth
        benefit1 = (fSpace.Depth - Cuboid.Depth) * fSpace.Width

        cost2 = (fSpace.Depth - Cuboid.Depth) * Cuboid.Width
        benefit2 = (fSpace.Width - Cuboid.Width) * fSpace.Depth

        If cost1 = 0 And benefit1 = 0 Then
            ratio1 = fSpace.Depth * fSpace.Width
        ElseIf cost1 = 0 Then
            ratio1 = benefit1
        Else
            ratio1 = benefit1 / cost1
        End If

        If cost2 = 0 And benefit2 = 0 Then
            ratio2 = fSpace.Depth * fSpace.Width
        ElseIf cost2 = 0 Then
            ratio2 = benefit2
        Else
            ratio2 = benefit2 / cost2
        End If

        '(3)
        cost3 = Cuboid.Depth * Cuboid.Width
        benefit3 = (fSpace.Depth * fSpace.Width) - cost3
        If cost3 = 0 And benefit3 = 0 Then
            ratio3 = fSpace.Depth * fSpace.Width
        ElseIf cost3 = 0 Then
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

        '(4)
        Dim vol As Integer
        Dim volEmptySpace
        vol = CInt(Cuboid.Depth / fBox.Depth) * CInt(Cuboid.Height / fBox.Height) * CInt(Cuboid.Width / fBox.Width)
        volEmptySpace = fSpace.Depth * fSpace.Height * fSpace.Width

        '(5)
        '//Best result ..haha.. --> I don't where's this come from
        'Return (ratio1 + ratio2 + ratio3) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol)))) * ratioX
        Return (ratio1 + ratio2 + ratio3) * ((Cuboid.Height / fSpace.Height) ^ 2) * ((1 / (2 ^ Abs(fMin(fCapacityBox, fNumberBox) - vol)))) * ratioX

        '---
        'normalize ratioTotal
        '1: height of cuboid
        '2: box of cuboid

        'basic function
        'Return (ratio1 + ratio2) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol))))


        'Return (ratio1 + ratio2) * (Cuboid.Height / FEmptySpace.Height) * (1 - (Abs(min(FCapacityBox, FNumberBox) - vol) / min(FCapacityBox, FNumberBox)))

        'Return (ratio1 + ratio2 + ratio3) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(min(FCapacityBox, FNumberBox) - vol))))
        'Return (ratio1 + ratio2) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(min(FCapacityBox, FNumberBox) - vol)))) * ratioX
        'Return (ratio1 + ratio2 + ratio3) * (1 / (3 ^ Int(FEmptySpace.Height / Cuboid.Height))) * ((1 / (2 ^ Abs(min(FCapacityBox, FNumberBox) - vol)))) * ratioX

        'experiment (+ utilization factor)
        'Return (ratio1 + ratio2 + ratio3) * (Cuboid.Height / FEmptySpace.Height) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol)))) * ratioX * (vol / volEmptySpace)
        'Return (ratio1 + ratio2 + ratio3) * ((1 / (2 ^ Abs(Min(FCapacityBox, FNumberBox) - vol)))) * ratioX * (vol / volEmptySpace)
    End Function

    ''' <summary>
    ''' #Construct cuboid
    ''' -Make cuboid in 1/2/3 dimension
    ''' -
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Cuboid construction for each dimension
    ''' --3. Get box in cuboid + boundingCuboid
    ''' </summary>
    Public Sub ConstructCuboid(ByRef FreeBox As Integer, _
                               ByVal axis As Byte, _
                               ByVal dir1 As Char, ByVal dir2 As Char, ByVal dir3 As Char, _
                               ByVal startPos As Point3D, _
                               ByVal tolerate As Boolean)
        '(1)
        Dim i, n(3) As Integer
        Dim direction(3) As Char

        '(2)
        If axis = 1 Then Construct1AxisCuboid(FreeBox, axis, dir1, startPos, tolerate)
        If axis = 2 Then Construct2AxisCuboid(freeBox, axis, dir1, dir2, startPos, tolerate)
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
            Construct3AxisCuboid(FreeBox, axis, dir1, dir2, dir3, startPos, tolerate)
        End If

        '(3)
        ReDim Preserve fCoordBox(fNumberBox - freeBox)
        GetBoundingCuboid()
    End Sub

    ''' <summary>
    ''' #Construct 1-Axis
    ''' -Get constuction in 1axis, based on direction given
    ''' -Proceed only_if usedBox is enough
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get direction
    ''' ---2a. Get direction + get iteration could make
    ''' ---2b. Set array size
    ''' --3. Write coordinate
    ''' </summary>
    ''' <param name="noBox">no.box to arrange</param>
    ''' <param name="axis">number of dimension involved</param>
    ''' <param name="direction">direction (W, D, H) --along width, depth, height</param>
    ''' <param name="startPos">start position in container</param>
    ''' <param name="tolerate">true = tolerate the construction --bounding box not perfect</param>
    Private Sub Construct1AxisCuboid(ByRef FreeBox As Integer, _
                                     ByVal axis As Byte, _
                                     ByVal direction As Char, _
                                     ByVal startPos As Point3D, _
                                     ByVal Tolerate As Boolean)
        '(1)
        Dim i, j, n As Integer

        '(2)
        '(2a)
        If direction = "W" Then
            n = fLengthY
        ElseIf direction = "H" Then
            n = fLengthZ
        Else
            n = fLengthX
        End If

        '(2b)
        j = fMin(n, FreeBox)
        If axis = 1 Then ReDim fCoordBox(j)

        '(3)
        If ((Tolerate = False) And (fMax(n, FreeBox) >= n)) Or _
            (Tolerate = True) Then
            For i = 1 To j
                FreeBox -= 1
                If direction = "W" Then
                    fCoordBox(fNumberBox - FreeBox) = New Point3D(startPos.X, (i - 1) * fBox.Width, startPos.Z)
                ElseIf direction = "H" Then
                    fCoordBox(fNumberBox - FreeBox) = New Point3D(startPos.X, startPos.Y, (i - 1) * fBox.Height)
                Else
                    fCoordBox(fNumberBox - FreeBox) = New Point3D((i - 1) * fBox.Depth, startPos.Y, startPos.Z)
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' #Construct 2-Axis
    ''' -Get constuction in 2axis, based on direction given
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get direction
    ''' ---2a. Get 1st direction + get iteration could make
    ''' ---2b. Get 2nd direction + get layer sizet
    ''' ---2c. Set array size
    ''' --3. Continue for next recursive
    ''' </summary>
    ''' <param name="noBox">no.box to arrange</param>
    ''' <param name="axis">number of dimension involved</param>
    ''' <param name="dir1">direction 1 (W, D, H) --along width, depth, height</param>
    ''' <param name="dir2">direction 2 (W, D, H) --along width, depth, height</param>
    ''' <param name="startPos">start position in container</param>
    ''' <param name="tolerate">true = tolerate the construction --bounding box not perfect</param>
    Private Sub Construct2AxisCuboid(ByRef FreeBox As Integer, _
                                     ByVal axis As Byte, _
                                     ByVal dir1 As Char, ByVal dir2 As Char, _
                                     ByVal startPos As Point3D, _
                                     ByVal tolerate As Boolean)
        '(1)
        Dim i, n, layer As Integer

        '(2)
        '(2a)
        If dir2 = "W" Then
            n = fLengthY
            If (dir1 = dir2) And (fLengthX <= fLengthZ) Then dir1 = "D"
            If (dir1 = dir2) And (fLengthX > fLengthZ) Then dir1 = "H"
        ElseIf dir2 = "H" Then
            n = fLengthZ
            If (dir1 = dir2) And (fLengthX <= fLengthY) Then dir1 = "D"
            If (dir1 = dir2) And (fLengthX > fLengthY) Then dir1 = "W"
        Else
            n = fLengthX
            If (dir1 = dir2) And (fLengthY <= fLengthZ) Then dir1 = "W"
            If (dir1 = dir2) And (fLengthY > fLengthZ) Then dir1 = "H"
        End If

        '(2b)
        If dir1 = "W" Then
            layer = n * fLengthY
        ElseIf dir1 = "H" Then
            layer = n * fLengthZ
        Else
            layer = n * fLengthX
        End If

        '(2c)
        If axis = 2 Then ReDim fCoordBox(layer)

        '(3)
        For i = 1 To n
            If dir2 = "W" Then
                Construct1AxisCuboid(FreeBox, _
                                     axis, _
                                     dir1, _
                                     New Point3D(startPos.X, (i - 1) * fBox.Width, startPos.Z), _
                                     tolerate)
            ElseIf dir2 = "H" Then
                Construct1AxisCuboid(FreeBox, _
                                     axis, _
                                     dir1, _
                                     New Point3D(startPos.X, startPos.Y, (i - 1) * fBox.Height), _
                                     tolerate)
            Else
                Construct1AxisCuboid(FreeBox, _
                                     axis, _
                                     dir1, _
                                     New Point3D((i - 1) * fBox.Depth, startPos.Y, startPos.Z), _
                                     tolerate)
            End If

            'No_Toleration = True
            If (tolerate = False) And (FreeBox < (layer - ((layer / n) * i))) Then Exit For
        Next
    End Sub

    ''' <summary>
    ''' #Construct 3-Axis
    ''' -Get constuction in 3axis, based on direction given
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get direction
    ''' ---2a. Get 1st direction + get iteration could make
    ''' ---2b. Get 2nd direction + get layer sizet
    ''' ---2c. Set array size
    ''' --3. Continue for next recursive
    ''' Construct 3 axis cuboid
    ''' </summary>
    ''' <param name="noBox">no.box to arrange</param>
    ''' <param name="axis">number of dimension involved</param>
    ''' <param name="dir1">direction 1 (W, D, H) --along width, depth, height</param>
    ''' <param name="dir2">direction 2 (W, D, H) --along width, depth, height</param>
    ''' <param name="dir2">direction 3 (W, D, H) --along width, depth, height</param>
    ''' <param name="startPos">start position in container</param>
    ''' <param name="tolerate">true = tolerate the construction --bounding box not perfect</param>
    Private Sub Construct3AxisCuboid(ByRef FreeBox As Integer, _
                                     ByVal axis As Byte, _
                                     ByVal dir1 As Char, ByVal dir2 As Char, ByVal dir3 As Char, _
                                     ByVal startPos As Point3D, ByVal tolerate As Boolean)
        '(1)
        Dim i, n, layer As Integer

        '(2)
        '(2a)
        If dir3 = "W" Then
            n = fLengthY
        ElseIf dir3 = "H" Then
            n = fLengthZ
        Else
            n = fLengthX
        End If

        '(2b)
        If ((dir2 = "W") And (dir1 = "H")) Or ((dir2 = "H") And (dir1 = "W")) Then
            layer = fLengthY * fLengthZ
        ElseIf ((dir2 = "D") And (dir1 = "W")) Or ((dir2 = "W") And (dir1 = "D")) Then
            layer = fLengthX * fLengthY
        Else
            layer = fLengthX * fLengthZ
        End If

        'set array size
        If axis = 3 Then ReDim fCoordBox(fCapacityBox)

        For i = 1 To n
            If dir3 = "W" Then
                Construct2AxisCuboid(FreeBox, _
                                     axis, _
                                     dir1, dir2, _
                                     New Point3D(startPos.X, (i - 1) * fBox.Width, startPos.Z), _
                                     tolerate)
            ElseIf dir3 = "H" Then
                Construct2AxisCuboid(FreeBox, _
                                     axis, _
                                     dir1, dir2, _
                                     New Point3D(startPos.X, startPos.Y, (i - 1) * fBox.Height), _
                                     tolerate)
            Else
                Construct2AxisCuboid(FreeBox, _
                                     axis, _
                                     dir1, dir2, _
                                     New Point3D((i - 1) * fBox.Depth, startPos.Y, startPos.Z), _
                                     tolerate)
            End If

            '//No_Toleration = True
            If (tolerate = False) And (FreeBox < (layer - ((layer / n) * i))) Then Exit For
        Next
    End Sub

    ''' <summary>
    ''' #GetBoundingCuboid
    ''' --1. Variable set
    ''' --2. Get bounding dimension
    ''' --3. Create boundingBox
    ''' </summary>
    Private Sub GetBoundingCuboid()
        '(1)
        Dim i As Integer
        Dim longWidth, longHeight, longDepth As Single

        '(2)
        longWidth = 0 : longHeight = 0 : longDepth = 0
        For i = 1 To fCoordBox.GetUpperBound(0)
            If longDepth < (fCoordBox(i).X + fBox.Depth) Then longDepth = fCoordBox(i).X + fBox.Depth
            If longWidth < (fCoordBox(i).Y + fBox.Width) Then longWidth = fCoordBox(i).Y + fBox.Width
            If longHeight < (fCoordBox(i).Z + fBox.Height) Then longHeight = fCoordBox(i).Z + fBox.Height
        Next

        '(3)
        fBoundingBox = New Box(-1, longDepth, longWidth, longHeight)
    End Sub

    ''' <summary>
    ''' #GetBoundingCuboid -METHOD2
    ''' -Different method
    ''' --1. Variable set
    ''' --2. Get maximum value
    ''' --3. Create boundingBox
    ''' </summary>
    Private Sub GetBoundingCuboid2(ByVal nDepth As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer)
        '(1)
        Dim longDepth, longHeight, longWidth As Single

        '(2)
        longDepth = fBox.Depth * nDepth
        longWidth = fBox.Width * nWidth
        longHeight = fBox.Height * nHeight

        '(3)
        fBoundingBox = New Box(-1, longDepth, longWidth, longHeight)
    End Sub

    ''' <summary>
    ''' #Get Rotation
    ''' -Define combination direction for box arrangement
    ''' --0. Parameter set
    ''' </summary>
    Private Sub GetDirection()
        ReDim fDirection(6, 3)

        fDirection(1, 1) = "W"
        fDirection(1, 2) = "D"
        fDirection(1, 3) = "H"

        fDirection(2, 1) = "W"
        fDirection(2, 2) = "H"
        fDirection(2, 3) = "D"

        fDirection(3, 1) = "H"
        fDirection(3, 2) = "D"
        fDirection(3, 3) = "W"

        fDirection(4, 1) = "H"
        fDirection(4, 2) = "W"
        fDirection(4, 3) = "D"

        fDirection(5, 1) = "D"
        fDirection(5, 2) = "W"
        fDirection(5, 3) = "H"

        fDirection(6, 1) = "D"
        fDirection(6, 2) = "H"
        fDirection(6, 3) = "W"
    End Sub

    ''' <summary>
    ''' #ConstructManual
    ''' -Manual box construction
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Construct box manually
    ''' --3. Finalization
    ''' </summary>
    Public Sub ConstructManual(ByRef FreeBox As Integer, _
                               ByVal dirX As Integer, ByVal dirY As Integer, ByVal dirZ As Integer, _
                               ByVal startPos As Point3D)
        '(1)
        Dim i, j, k, n As Integer

        '(2)
        ReDim fCoordBox(dirX * dirY * dirZ)
        n = 0
        For k = 1 To dirZ
            For i = 1 To dirX
                For j = 1 To dirY
                    n += 1
                    fCoordBox(n) = New Point3D((i - 1) * fBox.Depth, (j - 1) * fBox.Width, (k - 1) * fBox.Height)
                    If n = FreeBox Then Exit For
                Next
                If n = FreeBox Then Exit For
            Next
            If n = FreeBox Then Exit For
        Next
        FreeBox = FreeBox - n

        '(3)
        ReDim Preserve fCoordBox(fNumberBox - FreeBox)
        GetBoundingCuboid()
    End Sub


    ''' <summary>
    ''' #GetMaxList
    ''' -Sort box from list from the max first
    ''' --1. Variable set
    ''' --2. Insert data
    ''' --3. Sort data
    ''' --Additional variable fPossiblePlacement = true, if at least one box can get into fPossiblePlacement
    ''' </summary>
    Private Sub GetMaxList()
        '(1)
        Dim i, j As Integer
        Dim volList(fListInput.GetUpperBound(0)) As Single
        Dim tempBox As Box
        ReDim fListMaxVolBox(fListInput.GetUpperBound(0))
        fPossiblePlacement = False

        '(2)
        For i = 1 To fListInput.GetUpperBound(0)
            tempBox = New Box(fGetBoxFromList(fListInput(i).SType, fInput))
            If (CheckPossiblePacking(tempBox, fSpace) = False) Then
                fListMaxVolBox(i).SType = -1
                fListMaxVolBox(i).SCount = -1
                volList(i) = 0
            Else
                fListMaxVolBox(i).SType = fListInput(i).SType
                fListMaxVolBox(i).SCount = fListInput(i).SCount
                volList(i) = fListInput(i).SCount * tempBox.Depth * tempBox.Height * tempBox.Width
                fPossiblePlacement = True
            End If
        Next

        '(3)
        For i = 1 To (fListInput.GetUpperBound(0) - 1)
            For j = i + 1 To fListInput.GetUpperBound(0)
                If volList(i) < volList(j) Then
                    procSwap(volList(i), volList(j))
                    procSwap(fListMaxVolBox(i).SCount, fListMaxVolBox(j).SCount)
                    procSwap(fListMaxVolBox(i).SType, fListMaxVolBox(j).SType)
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' #GetOutput
    ''' -Finalization data
    ''' -Write output to box as final output
    ''' --1. Variable set
    ''' --2. Pointing box
    ''' --3. Cloning box all
    ''' --4. Revision for box in cuboid (manual)
    ''' --5. Recapitulation
    ''' --6. Get utilization
    ''' </summary>
    Private Sub GetOutput()
        '(1)
        Dim i As Integer

        '(2)
        GetPointingBox()

        '(3)
        procBoxClone(fInput, fOutput)

        '(4)
        For i = 1 To fUsedBox
            If fBox.IsAlpha = True Then fOutput(fPointerBox(i)).IsAlpha = fBox.IsAlpha
            If fBox.IsBeta = True Then fOutput(fPointerBox(i)).IsBeta = fBox.IsBeta
            If fBox.IsGamma = True Then fOutput(fPointerBox(i)).IsGamma = fBox.IsGamma
            fOutput(fPointerBox(i)).Orientation = fBox.Orientation
            fOutput(fPointerBox(i)).RelPos1 = New Point3D(fCoordBox(i).X, fCoordBox(i).Y, fCoordBox(i).Z)
            fOutput(fPointerBox(i)).InContainer = True
        Next

        '(5)
        algRecapitulation(fInput, fListOutput)
        For i = 1 To fListOutput.GetUpperBound(0)
            If fListOutput(i).SType = fBox.Type Then
                fListOutput(i).SCount = fUsedBox
            Else
                fListOutput(i).SCount = 0
            End If
        Next

        '(6)
        fVolume = fBoundingBox.Depth * fBoundingBox.Width * fBoundingBox.Height
        fUtilization = fVolume / (fSpace.Depth * fSpace.Width * fSpace.Height)
        fCompactness = fVolume / (fBoundingBox.Depth * fBoundingBox.Width * fSpace.Height)
    End Sub

    ''' <summary>
    ''' #GetPointingBox
    ''' -Pointing box from coordBox
    ''' --1. Variabel set
    ''' --2. Pointing box
    ''' </summary>
    Private Sub GetPointingBox()
        '(1)
        Dim i, j As Integer
        ReDim fPointerBox(fUsedBox)

        '(2)
        j = 0
        For i = 1 To fInput.GetUpperBound(0)
            If fInput(i).Type = fBox.Type Then
                j += 1
                fPointerBox(j) = i
            End If
            If j = fUsedBox Then Exit For
        Next
    End Sub

    ''' <summary>
    ''' #CheckPossiblePacking
    ''' -Check possible packing in emptyspace
    ''' -Minimum there is one feasible orientation to place box in space
    ''' -!ACCOMODATED ROTATION RESTRICTION
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. For all orientation that feasible
    ''' --3. Check dimension box placed in space
    ''' </summary>
    Private Function CheckPossiblePacking(ByVal dataBox As Box, ByVal dataSpace As Box) As Boolean
        '(1)
        Dim i, j As Integer
        Dim cek As Boolean

        '(2)
        cek = False
        For i = 1 To 3
            If ((i = 1) AndAlso (dataBox.RotAlpha = True)) Or _
                ((i = 2) AndAlso (dataBox.RotBeta = True)) Or _
                ((i = 3) AndAlso (dataBox.RotGamma = True)) Then

                If i = 1 Then dataBox.IsAlpha = True
                If i = 2 Then dataBox.IsBeta = True
                If i = 3 Then dataBox.IsGamma = True

                For j = 1 To 2
                    If j = 1 Then dataBox.Orientation = True
                    If j = 2 Then dataBox.Orientation = False

                    '(3)
                    If (dataBox.Depth <= dataSpace.Depth) And _
                        (dataBox.Width <= dataSpace.Width) And _
                        (dataBox.Height <= dataSpace.Height) Then
                        cek = True
                        Exit For
                    End If
                Next
                If cek = True Then Exit For
            End If
        Next
        Return cek
    End Function


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
        If fNumberBox < fMaxBox Then    'if numberBOX > maximumBob --> no optimization, arrange as GetMaxNumberCuboid
            'calculate score initial    'else, find the best arrangement: this section is goint to optimize it!
            fScore = 0
            'Console.WriteLine("0 : " & FScore)
            count = 0

            'part#1
            'optimization
            For i = 1 To 2
                'different action give for value bellow and above target
                'if above target, we must find score each combination of 3 axis multiply
                'if bellow target, we must find score only the maximum cuboid

                If i = 1 Then
                    fBox.Orientation = True
                    GetMaxSizeLayer()
                Else
                    fBox.Orientation = False
                    GetMaxSizeLayer()
                End If

                'part#3
                'optimization for ABOVE target
                'starting constraint combination
                For j = 1 To fMin3(fLengthX, fNumberBox, fCapacityBox)           'minimum of lengthX, number box must be arrange, maximum box in cuboid
                    For k = 1 To fMin(fLengthY, CInt(fMin(fNumberBox, fCapacityBox) / j) + 1)
                        For l = 1 To 1
                            If (j <= fMin3(fLengthX, fNumberBox, fCapacityBox)) And (k <= fMin3(fLengthY, fNumberBox, fCapacityBox)) _
                                And ((tolerate = True) Or ((tolerate = False) And ((j * k * l) <= fMin(fNumberBox, fCapacityBox)))) Then
                                GetBoundingCuboid2(j, k, l)                         'construct bounding cuboid

                                'best = maximum point --> dibuat dulu ya.
                                If GetScore(fBoundingBox) > fScore Then
                                    fScore = GetScore(fBoundingBox)
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
                fBox.Orientation = True
                GetMaxSizeLayer()
            Else
                fBox.Orientation = False
                GetMaxSizeLayer()
            End If

            'construct cuboid
            fFreeBox = fNumberBox   'reset data
            ConstructManual(fFreeBox, tX, tY, tZ, New Point3D(0, 0, 0))

            fScore = GetScore(fBoundingBox)

            'Console.WriteLine("BEST:" & GetScore(FBoundingCuboid) & " --" & Min(FNumberBox, FCapacityBox) & " = " & tX & " x " & tY & " x " & tZ)

            'final calculation
            fUsedBox = fNumberBox - fFreeBox                    'calculate used box
            fScore = GetScore(fBoundingBox)                  'empty score
            fMethod = "Max Optimize Layer"                'give status report
        End If
    End Sub
End Class



'//Sementara dan tidak jelas ini apaan.. gw lupa
'''' <summary>
'''' Get single maximum cuboid based on score of cuboid --after trying all configuration
'''' </summary>
'Public Sub GetMaxScoreCuboid(ByVal tolerate As Boolean)
'    Dim i, j, k, n(3), count As Integer
'    Dim direction(3) As Char
'    Dim tside As Byte
'    Dim torien As Boolean
'    Dim tdirection As Byte

'    'getting the direction
'    n(1) = fLengthX
'    n(2) = fLengthY
'    n(3) = fLengthZ
'    direction(1) = "D"
'    direction(2) = "W"
'    direction(3) = "H"

'    'sorting --> getting list from minimum to maximum
'    For i = 1 To n.GetUpperBound(0) - 1
'        For j = 2 To n.GetUpperBound(0)
'            If n(i) > n(j) Then
'                procSwap(n(i), n(j))
'                procSwap(direction(i), direction(j))
'            End If
'        Next
'    Next

'    'save best direction
'    For i = 1 To n.GetUpperBound(0)
'        fDirection(0, i) = direction(i)
'    Next

'    'calculate maximum cuboid
'    fScore = GetScore(New Box())
'    'Console.WriteLine("0 : " & FScore)

'    count = 0
'    For i = 1 To 3
'        For j = 1 To 2
'            If j = 1 Then
'                SetRotation(i, True)
'            Else
'                SetRotation(i, False)
'            End If

'            For k = 1 To 6
'                'construct cuboid
'                fFreeBox = fNumberBox   'reset data
'                ConstructCuboid(fFreeBox, 3, fDirection(k, 3), fDirection(k, 2), fDirection(k, 1), New Point3D(0, 0, 0), True)

'                'best = maximum point
'                If fScore < GetScore(fBoundingBox) Then
'                    fScore = GetScore(fBoundingBox)
'                    tside = i
'                    torien = True
'                    tdirection = k
'                    If j = 2 Then torien = False
'                End If
'                If fScore = 0 Then Exit For 'exit if minimum score = 0

'                'dump it if not necessary
'                count += 1
'                'Console.WriteLine(count & " : " & FScore & " , " & FCapacityBox)
'            Next
'        Next
'        If fScore = 0 Then Exit For
'    Next

'    'set the best side & orientation
'    SetRotation(tside, torien)

'    'construct cuboid
'    fFreeBox = fNumberBox   'reset data
'    ConstructCuboid(fFreeBox, 3, fDirection(tdirection, 3), fDirection(tdirection, 2), fDirection(tdirection, 1), New Point3D(0, 0, 0), True)

'    'Console.WriteLine("BEST : " & FScore & " , " & FCapacityBox)

'    'final calculation
'    fUsedBox = fNumberBox - fFreeBox                    'calculate used box
'    fScore = GetScore(fBoundingBox)                  'empty score
'    fMethod = "Max Score Cuboid"                  'give status report
'End Sub
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