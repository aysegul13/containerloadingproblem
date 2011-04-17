Public Class MaximalSpace
    Private fContour(), fInitContour() As Line3D
    Private fOrigin As Point3D

    Private fSpace() As Kotak
    Private fSpaceBox() As Box

    ''' <summary>
    ''' Contour construct
    ''' Useful when a contour generate more than one differentiate contour
    ''' --0. Parameter set
    ''' --1. Preparation set: resize contour and copy data
    ''' --2. Get minimum point
    ''' --3. Sorting contour
    ''' --4. Get another contour if more than 2
    ''' --5. Get maximal space
    ''' </summary>
    Sub New(ByRef lineContour() As Line3D, ByVal AboveContour As Boolean)
        '(1)
        ReDim fContour(lineContour.GetUpperBound(0))
        ReDim fInitContour(lineContour.GetUpperBound(0))
        For i As Integer = 1 To lineContour.GetUpperBound(0)
            fContour(i) = New Line3D(lineContour(i))
            fInitContour(i) = New Line3D(lineContour(i))
        Next
        ReDim lineContour(0)

        '(2)
        fOrigin = GetOriginPoint(fContour, AboveContour)

        '(3)
        Dim count As Integer
        SortKontur(fContour, count)

        '(4)
        If count < fContour.GetUpperBound(0) Then
            '//Move unused loop to kontur --> tobe reexaminate again
            ReDim lineContour(fContour.GetUpperBound(0) - count)
            Dim i, j As Integer
            j = 0
            For i = count + 1 To fContour.GetUpperBound(0)
                j += 1
                lineContour(j) = New Line3D(fContour(i))
            Next

            '//Resize (to fix) currentcontour
            ReDim Preserve fContour(count)
            '//re-pointing + re-sorting >> important to determine looping
            SortKontur(fContour, count)
        End If

        '(5)
        If count > 0 Then GetMaximalSpace(fContour)
    End Sub


    ''' <summary>
    ''' Merging construct
    ''' Contour construct, special for merging
    ''' --0. Parameter set
    ''' --1. Getting line that has same height of start point
    ''' --2. Merging contour + normalization line
    ''' --3. Get minimum point >> !important to set foriginpoint after getcontour function!!
    ''' --4. Sorting contour
    ''' --5. Establish another contour if it define more than one close loop
    ''' --6. Copy to initial contour >> initContour: data initial of contour --before changing due box adding
    ''' --7. Get maximal space; otherwise: no maximal space
    ''' </summary>
    Sub New(ByRef restContour() As Line3D, ByVal startPoint As Point3D)
        '(1)
        fContour = GetSeparatedLine(restContour, startPoint, restContour)

        '//Continue process
        If fContour.GetUpperBound(0) > 0 Then
            '(2)
            '//Merging contour
            MergeContour(fContour)
            NormalizeLine(fContour)

            '(3)
            fOrigin = GetOriginPoint(fContour, True)

            '(4)
            Dim count As Integer
            SortKontur(fContour, count)

            '(5)
            '//Extract loops --if there are another loop
            If count < fContour.GetUpperBound(0) Then
                '//Move unused loop to restContour --> tobe reexaminate again
                Dim i, j As Integer

                j = restContour.GetUpperBound(0)
                ReDim Preserve restContour(j + fContour.GetUpperBound(0) - count)
                For i = count + 1 To fContour.GetUpperBound(0)
                    j += 1
                    restContour(j) = New Line3D(fContour(i))
                Next

                '//Resize (to fix) currentcontour
                ReDim Preserve fContour(count)
                '//re-pointing + re-sorting >> important to determine looping
                SortKontur(fContour, count)
            End If

            '(6)
            ReDim fInitContour(fContour.GetUpperBound(0))
            For i As Integer = 1 To fContour.GetUpperBound(0)
                fInitContour(i) = New Line3D(fContour(i))
            Next

            '(7)
            'Try
            If count > 0 Then GetMaximalSpace(fContour)
            'Catch ex As Exception
            '    MyForm.formMainMenu.txtConsole.Text = "get maximal space"
            '    Stop
            'End Try
        Else
            ReDim fSpace(Nothing)
        End If
    End Sub


    ''' <summary>
    ''' Box construct
    ''' Usefull when subset box has been placed in a contour --> there's a box above contour that must be build
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Separated box from different height
    ''' --3. Extract contour of selected box
    ''' --4. Get minimum point
    ''' --5. Sorting contour
    ''' --6. Establish another contour if it define more than one close loop
    ''' --7. Copy to initial contour >> initContour: data initial of contour --before changing due box adding
    ''' --8. Get maximal space; otherwise: no maximal space
    ''' </summary>
    Sub New(ByVal addBox() As Box, ByVal startPoint As Point3D, ByRef restContour() As Line3D)
        '(1)
        Dim tempBox(addBox.GetUpperBound(0)) As Box

        '(2)
        '//Getting box that has same height of start point
        tempBox = GetSeparatedBox(addBox, startPoint, True)

        '(3)
        '//Find contour, only for box that has same height of start point
        GetContour(fContour, tempBox, True)

        '//Continue process
        If fContour.GetUpperBound(0) > 0 Then
            '(4)
            fOrigin = GetOriginPoint(fContour, True)

            '(5)
            Dim count As Integer
            SortKontur(fContour, count)

            '(6)
            '//Extract loops --if there are another loop
            If count < fContour.GetUpperBound(0) Then
                '//Move unused loop to restContour --> tobe reexaminate again
                ReDim restContour(fContour.GetUpperBound(0) - count)
                Dim i, j As Integer
                j = 0
                For i = count + 1 To fContour.GetUpperBound(0)
                    j += 1
                    restContour(j) = New Line3D(fContour(i))
                Next

                '//Resize (to fix) currentcontour
                ReDim Preserve fContour(count)
                '//re-pointing + re-sorting >> important to determine looping
                SortKontur(fContour, count)
            End If

            '(7)
            ReDim fInitContour(fContour.GetUpperBound(0))
            For i As Integer = 1 To fContour.GetUpperBound(0)
                fInitContour(i) = New Line3D(fContour(i))
            Next

            '(8)
            'Try
            If count > 0 Then GetMaximalSpace(fContour)
            'Catch ex As Exception
            '    MyForm.formMainMenu.txtConsole.Text = "get maximal space"
            '    Stop
            'End Try
        Else
            ReDim fSpace(Nothing)
        End If
    End Sub

    ''' <summary>
    ''' Container construct
    ''' Usefull when placing first time container
    ''' </summary>
    Sub New(ByVal container As Box)
        '1. resize contour
        ReDim fContour(4), fInitContour(4)
        fContour(1) = New Line3D(container.AbsPos1.X, container.AbsPos1.Y, container.AbsPos1.Z, _
                                 container.AbsPos2.X, container.AbsPos1.Y, container.AbsPos1.Z)
        fContour(2) = New Line3D(container.AbsPos2.X, container.AbsPos1.Y, container.AbsPos1.Z, _
                                 container.AbsPos2.X, container.AbsPos2.Y, container.AbsPos1.Z)
        fContour(3) = New Line3D(container.AbsPos1.X, container.AbsPos2.Y, container.AbsPos1.Z, _
                                 container.AbsPos2.X, container.AbsPos2.Y, container.AbsPos1.Z)
        fContour(4) = New Line3D(container.AbsPos1.X, container.AbsPos1.Y, container.AbsPos1.Z, _
                                 container.AbsPos1.X, container.AbsPos2.Y, container.AbsPos1.Z)

        For i As Integer = 1 To fContour.GetUpperBound(0)
            fInitContour(i) = New Line3D(fContour(i))
        Next

        '2. get minimum point
        fOrigin = GetOriginPoint(fContour, False)

        '3. sorting contour
        Dim count As Integer
        SortKontur(fContour, count)

        '4. due new kontur --> get maximal space
        GetMaximalSpace(fContour)
    End Sub

    ''' <summary>
    ''' Clone constructor
    ''' </summary>
    Sub New(ByVal masterContour As MaximalSpace)
        Dim i As Integer

        '//Contour
        ReDim fContour(masterContour.fContour.GetUpperBound(0))
        For i = 1 To fContour.GetUpperBound(0)
            fContour(i) = New Line3D(masterContour.fContour(i))
        Next

        '//Space
        ReDim fSpace(masterContour.fSpace.GetUpperBound(0))
        For i = 1 To fSpace.GetUpperBound(0)
            fSpace(i) = New Kotak(masterContour.fSpace(i))
        Next

        '//InitContour
        ReDim fInitContour(masterContour.fInitContour.GetUpperBound(0))
        For i = 1 To fInitContour.GetUpperBound(0)
            fInitContour(i) = New Line3D(masterContour.fInitContour(i))
        Next

        '//Origin
        fOrigin = New Point3D(masterContour.fOrigin)
    End Sub

    ''' <summary>
    ''' Get origin of emptySpaceArea
    ''' </summary>
    Public ReadOnly Property OriginPoint() As Point3D
        Get
            Return fOrigin
        End Get
    End Property

    ''' <summary>
    ''' Get emptyspace that generated in this contour
    ''' </summary>
    Public ReadOnly Property Space() As Kotak()
        Get
            Return fSpace
        End Get
    End Property

    Public ReadOnly Property CountSpace() As Integer
        Get
            Dim i As Integer = 0
            If fSpace.GetUpperBound(0) > 0 Then i = fSpace.GetUpperBound(0)
            Return i
        End Get
    End Property

    Public ReadOnly Property Contour() As Line3D()
        Get
            Return fContour
        End Get
    End Property

    Public Property SpaceBox() As Box()
        Get
            Return fSpaceBox
        End Get
        Set(ByVal value() As Box)
            fSpaceBox = value
        End Set
    End Property

    ''' <summary>
    ''' Cek box in contour
    ''' -Don't be confuse >> it's talk about contour; one-contour consists of many-space-area
    ''' -What this algorithm cek is whether or not one box is touch fully in contour.
    '''     -that is why it's check box-corner in every space.
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Checking same height with area contour
    ''' --3. Return value
    ''' </summary>
    Public Function CheckBoxInContour(ByVal boxCompare As Box) As Boolean
        '(1)
        Dim i As Integer
        Dim cek(4) As Boolean

        '(2)
        '//Box in areaContour
        cek(1) = False : cek(2) = False : cek(3) = False : cek(4) = False
        For i = 1 To fSpace.GetUpperBound(0)
            If (fOrigin.Z = boxCompare.AbsPos1.Z) AndAlso _
                ((fSpace(i).Position.X <= boxCompare.AbsPos1.X) And (boxCompare.AbsPos1.X <= fSpace(i).Position2.X)) AndAlso _
                ((fSpace(i).Position.Y <= boxCompare.AbsPos1.Y) And (boxCompare.AbsPos1.Y <= fSpace(i).Position2.Y)) Then
                cek(1) = True
            End If
            If (fOrigin.Z = boxCompare.AbsPos1.Z) AndAlso _
                ((fSpace(i).Position.X <= boxCompare.AbsPos2.X) And (boxCompare.AbsPos2.X <= fSpace(i).Position2.X)) AndAlso _
                ((fSpace(i).Position.Y <= boxCompare.AbsPos2.Y) And (boxCompare.AbsPos2.Y <= fSpace(i).Position2.Y)) Then
                cek(2) = True
            End If
            If (fOrigin.Z = boxCompare.AbsPos1.Z) AndAlso _
                ((fSpace(i).Position.X <= boxCompare.AbsPos1.X) And (boxCompare.AbsPos1.X <= fSpace(i).Position2.X)) AndAlso _
                ((fSpace(i).Position.Y <= boxCompare.AbsPos2.Y) And (boxCompare.AbsPos2.Y <= fSpace(i).Position2.Y)) Then
                cek(3) = True
            End If
            If (fOrigin.Z = boxCompare.AbsPos1.Z) AndAlso _
                ((fSpace(i).Position.X <= boxCompare.AbsPos2.X) And (boxCompare.AbsPos2.X <= fSpace(i).Position2.X)) AndAlso _
                ((fSpace(i).Position.Y <= boxCompare.AbsPos1.Y) And (boxCompare.AbsPos1.Y <= fSpace(i).Position2.Y)) Then
                cek(4) = True
            End If
        Next

        '(3)
        '//We need only one coordinate inside 
        If (cek(1) = True) And (cek(2) = True) And (cek(3) = True) And (cek(4) = True) Then
            Return True
        Else
            Return False
        End If
    End Function


    ''' <summary>
    ''' Cek box touch contour
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Checking same height with area contour
    ''' --3. Return value
    ''' </summary>
    Public Function CheckBoxTouchContour(ByVal boxCompare As Box) As Boolean
        '(1)
        Dim i As Integer
        Dim cek As Boolean

        '(2)
        '//Box in areaContour
        cek = False
        For i = 1 To fSpace.GetUpperBound(0)
            '//Based on colission theory, contour is touched if
            If (fOrigin.Z = boxCompare.AbsPos1.Z) AndAlso _
                (boxCompare.AbsPos1.X < fSpace(i).Position2.X) And _
                (boxCompare.AbsPos2.X > fSpace(i).Position.X) And _
                (boxCompare.AbsPos1.Y < fSpace(i).Position2.Y) And _
                (boxCompare.AbsPos2.Y > fSpace(i).Position.Y) Then
                cek = True
            End If
        Next

        '(3)
        Return cek
    End Function

    ''' <summary>
    ''' Get box in contour
    ''' -Record box in contour
    ''' -Plus manipulate it, if only separate box in contour
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Checking same height with area contour
    ''' --3. Return value
    ''' </summary>
    Private Function GetBoxInContour(ByVal inputBox() As Box) As Box()

        Return Nothing
    End Function

    ''' <summary>
    ''' Get maximal space from contour
    ''' 1. Contour separation
    ''' 2. Generate fisibelPoint
    ''' 3. Getting parrarlpiped Width
    ''' 4. Get maximal space
    ''' </summary>
    Private Sub GetMaximalSpace(ByVal Kontur() As Line3D)
        'Try
        '(1)
        '//Contour separation --> lineWidth + lineDepth
        Dim lineWidth(Nothing), lineDepth(Nothing) As Line3D
        GetLineWidthDepth(Kontur, lineWidth, lineDepth)

        '(2)
        '//Generate fisibelPoint (closed-contour point or opened-contour point)
        Dim contourPoint(Nothing) As Point3D
        Dim fisibelPoint(Nothing) As Boolean
        GetFeasiblePoint(Kontur, contourPoint, fisibelPoint)

        '(3)
        '//Getting parrarelpiped Width --> dealing with lineWidth + intersection to lineDepth
        Dim pararelWidth(Nothing), pararelDepth(Nothing) As Line3D
        GetParrarelpiped(contourPoint, fisibelPoint, lineWidth, lineDepth, pararelWidth, pararelDepth)

        '(4)
        '//Get maximal space
        GenerateMaximalSpace(contourPoint, fisibelPoint, lineWidth, lineDepth, pararelWidth, pararelDepth, fSpace)
        'Catch ex As Exception
        '    MyForm.formMainMenu.txtConsole.Text = "error, di maximal space..."
        'End Try
    End Sub


    ''' <summary>
    ''' Sort kontur to get 
    ''' </summary>
    Private Sub SortKontur(ByRef lineContour() As Line3D, ByRef count As Integer)
        'find minimum point
        Dim i, j As Integer
        Dim maxPoint As Single

        'pointer minimum point
        maxPoint = 0
        For i = 1 To lineContour.GetUpperBound(0)
            If (lineContour(i).IsDepthLine = True) AndAlso _
            (fMin(lineContour(i).fPoint1.X, lineContour(i).fPoint2.X) = fOrigin.X) And _
            (fMin(lineContour(i).fPoint1.Y, lineContour(i).fPoint2.Y) = fOrigin.Y) Then
                j = i
            End If
            If lineContour(i).fPoint1.Y > maxPoint Then maxPoint = lineContour(i).fPoint1.Y
        Next

        'swap pointer --> array1 : minimum point
        procSwap(lineContour(1), lineContour(j))
        If lineContour(1).fPoint1.Y = maxPoint Then
            lineContour(1).fDirection = False
        Else
            lineContour(1).fDirection = True
        End If


        'reconfigurate kontur --> initial: lineDepth
        'sorting value
        For i = 2 To lineContour.GetUpperBound(0)
            For j = i To lineContour.GetUpperBound(0)
                'kalo line yang sekarang width, cari depth sekarang.
                If (lineContour(i - 1).fDirection = True) AndAlso _
                    ((lineContour(i - 1).fPoint2.IsEqualTo(lineContour(j).fPoint1) = True) Or _
                    (lineContour(i - 1).fPoint2.IsEqualTo(lineContour(j).fPoint2) = True)) Then
                    procSwap(lineContour(i), lineContour(j))
                    If lineContour(i - 1).fPoint2.IsEqualTo(lineContour(i).fPoint1) = True Then
                        lineContour(i).fDirection = True
                    Else
                        lineContour(i).fDirection = False
                    End If
                    Exit For
                End If

                If (lineContour(i - 1).fDirection = False) AndAlso _
                    ((lineContour(i - 1).fPoint1.IsEqualTo(lineContour(j).fPoint1) = True) Or _
                    (lineContour(i - 1).fPoint1.IsEqualTo(lineContour(j).fPoint2) = True)) Then
                    procSwap(lineContour(i), lineContour(j))
                    If lineContour(i - 1).fPoint1.IsEqualTo(lineContour(i).fPoint1) = True Then
                        lineContour(i).fDirection = True
                    Else
                        lineContour(i).fDirection = False
                    End If
                    Exit For
                End If
            Next
        Next

        'count used-contour
        i = 2

        Do Until _
                (lineContour(1).fDirection = False And lineContour(i).fDirection = False And lineContour(1).fPoint2.IsEqualTo(lineContour(i).fPoint1) = True) Or _
                (lineContour(1).fDirection = False And lineContour(i).fDirection = True And lineContour(1).fPoint2.IsEqualTo(lineContour(i).fPoint2) = True) Or _
                (lineContour(1).fDirection = True And lineContour(i).fDirection = False And lineContour(1).fPoint1.IsEqualTo(lineContour(i).fPoint1) = True) Or _
                (lineContour(1).fDirection = True And lineContour(i).fDirection = True And lineContour(1).fPoint1.IsEqualTo(lineContour(i).fPoint2) = True)
            i += 1
        Loop
        count = i
    End Sub

    ''' <summary>
    ''' GetLine Width and Depth
    ''' </summary>
    Private Sub GetLineWidthDepth(ByVal lineContour() As Line3D, ByRef lineWidth() As Line3D, ByRef lineDepth() As Line3D)
        Dim i, j, k As Integer

        'resize array size
        ReDim lineWidth(lineContour.GetUpperBound(0)), lineDepth(lineContour.GetUpperBound(0))

        'insert data lineWidth and lineDepth
        j = 0 : k = 0
        For i = 1 To lineContour.GetUpperBound(0)
            If lineContour(i).IsWidthLine = True Then
                j += 1
                lineWidth(j) = lineContour(i)
            End If
            If lineContour(i).IsDepthLine = True Then
                k += 1
                lineDepth(k) = lineContour(i)
            End If
        Next

        'refit array size
        ReDim Preserve lineWidth(j), lineDepth(k)
    End Sub

    ''' <summary>
    ''' Get minimal point of a contour
    ''' </summary>
    Private Function GetOriginPoint(ByVal lineContour() As Line3D, ByVal Above As Boolean) As Point3D
        Dim minPoint = New Point3D(lineContour(1).fPoint1)

        'getting minimum point
        For i As Integer = 1 To lineContour.GetUpperBound(0)
            If (lineContour(i).IsWidthLine = True) AndAlso (fMin(lineContour(i).fPoint1.Y, lineContour(i).fPoint2.Y) < minPoint.Y) Then
                minPoint.Y = fMin(lineContour(i).fPoint1.Y, lineContour(i).fPoint2.Y)
            End If
        Next
        For i = 1 To lineContour.GetUpperBound(0)
            If (lineContour(i).IsDepthLine = True) AndAlso (lineContour(i).fPoint1.Y = minPoint.Y) Then
                minPoint.X = fMin(lineContour(i).fPoint1.X, lineContour(i).fPoint2.X)
            End If
        Next

        'if perpendicular, minimum point in intersection area
        For i = 1 To lineContour.GetUpperBound(0) - 1
            For j = i + 1 To lineContour.GetUpperBound(0)
                If ((i <> j) And (lineContour(i).IsDepthLine = True) And (lineContour(j).IsDepthLine = True)) AndAlso _
                    (lineContour(i).GetUnion(lineContour(j)).Length > 0) Then
                    minPoint.X = fMax(lineContour(i).fPoint1.X, lineContour(j).fPoint1.X)
                    minPoint.Y = lineContour(i).fPoint1.Y
                    Exit For
                End If
            Next
        Next

        If Above = True Then
            minPoint.Z = lineContour(1).fPoint2.Z
        Else
            minPoint.Z = lineContour(1).fPoint1.Z
        End If

        'result
        Return minPoint
    End Function

    ''' <summary>
    ''' InsertNewBox
    ''' -Revise space if new box inserted
    ''' -Concept: get box from same height in same cuboid then build contour
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Separated box from different height
    ''' --3. Revise contour
    ''' --4. Normalizeline --> reduce line into normal number
    ''' --5. Preparation
    ''' --6. Get another contour if more than one closed-loop
    ''' --7. Get maximal space
    ''' </summary>
    Public Sub InsertNewBox(ByVal addBox() As Box, ByVal startPoint As Point3D, ByRef restContour() As Line3D)
        '(1)
        Dim tempBox() As Box

        '(2)
        tempBox = GetSeparatedBox(addBox, startPoint, False)

        '(3)
        'Try
        ReviseContour(fContour, tempBox)
        'Catch ex As Exception
        '    MyForm.formMainMenu.txtConsole.Text = "error di rebuild contour"
        '    Stop
        'End Try

        '(4)
        NormalizeLine(fContour)

        If fContour.GetUpperBound(0) > 0 Then
            '(5)
            '//Preparation
            fOrigin = GetOriginPoint(fContour, False)
            Dim count As Integer
            SortKontur(fContour, count)

            'Console.WriteLine("===")
            'Console.WriteLine("box packed")
            'For i = 1 To tempBox.GetUpperBound(0)
            '    Console.WriteLine(tempBox(i).AbsPos1.X & "," & tempBox(i).AbsPos1.Y & "," & addBox(i).AbsPos1.Z & " + " & tempBox(i).AbsPos2.X & "," & tempBox(i).AbsPos2.Y & "," & tempBox(i).AbsPos2.Z)
            'Next
            'Console.WriteLine("---")
            'Console.WriteLine("contour result (origin = " & fOrigin.X & "," & fOrigin.Y & "," & fOrigin.Z & ")")
            'For i = 1 To fContour.GetUpperBound(0)
            '    If fContour(i).FDirection = True Then
            '        Console.WriteLine(fContour(i).FPoint1.X & "," & fContour(i).FPoint1.Y & "," & fContour(i).FPoint1.Z & " --> " & fContour(i).FPoint2.X & "," & fContour(i).FPoint2.Y & "," & fContour(i).FPoint2.Z)
            '    Else
            '        Console.WriteLine(fContour(i).FPoint2.X & "," & fContour(i).FPoint2.Y & "," & fContour(i).FPoint2.Z & " --> " & fContour(i).FPoint1.X & "," & fContour(i).FPoint1.Y & "," & fContour(i).FPoint1.Z)
            '    End If
            'Next
            'Console.WriteLine("===")

            '(6)
            If count < fContour.GetUpperBound(0) Then
                '//Move unused loop to kontur --> tobe reexaminate again
                ReDim restContour(fContour.GetUpperBound(0) - count)
                Dim i, j As Integer
                j = 0
                For i = count + 1 To fContour.GetUpperBound(0)
                    j += 1
                    restContour(j) = New Line3D(fContour(i))
                Next

                '//Resize (to fix) currentcontour
                ReDim Preserve fContour(count)
                '//re-pointing + re-sorting >> important to determine looping
                SortKontur(fContour, count)
            End If

            '(7)
            'Try
            If count > 0 Then GetMaximalSpace(fContour)
            'Catch ex As Exception
            '    MyForm.formMainMenu.txtConsole.Text = "get maximal space"
            '    Stop
            'End Try
        Else
            ReDim fSpace(Nothing)
        End If
    End Sub


    ''' <summary>
    ''' Get pararelpiped of a contour
    ''' </summary>
    Private Sub GetParrarelpiped(ByVal contourPoint() As Point3D, ByVal fisibelPoint() As Boolean, ByVal lineWidth() As Line3D, ByVal lineDepth() As Line3D, ByRef pararelpipedWidth() As Line3D, ByRef pararelpipedDepth() As Line3D)
        Dim i, j, k As Integer
        Dim defaultValue, minDistance, maxDistance As Single
        Dim minPointer, maxPointer, defaultMinPointer, defaultMaxPointer As Integer
        Dim cekMin, cekMax, cekPararelpiped() As Boolean

        'find maximal value
        defaultValue = fMax3(CSng(MyForm.formMainMenu.txtDConDepth.Text), CSng(MyForm.formMainMenu.txtDConHeight.Text), CSng(MyForm.formMainMenu.txtDConWidth.Text))

        '=====
        'lineWidth
        'resize array
        ReDim pararelpipedWidth(lineWidth.GetUpperBound(0) * 3)
        '-iteration for each lineWidth
        k = 0
        For i = 1 To lineWidth.GetUpperBound(0)
            '--reset data
            minDistance = defaultValue : maxDistance = defaultValue
            For j = 1 To contourPoint.GetUpperBound(0)
                If lineWidth(i).fPoint1.IsEqualTo(contourPoint(j)) = True Then cekMin = fisibelPoint(j)
                If lineWidth(i).fPoint2.IsEqualTo(contourPoint(j)) = True Then cekMax = fisibelPoint(j)
            Next

            'geting min and max point
            For j = 1 To lineDepth.GetUpperBound(0)
                '--find max point from point2 + record distance
                If (cekMax = True) AndAlso _
                   (lineDepth(j).fPoint1.Y > lineWidth(i).fPoint2.Y) And _
                   ((lineDepth(j).fPoint1.Y - lineWidth(i).fPoint2.Y) < maxDistance) And _
                   ((lineDepth(j).fPoint1.X <= lineWidth(i).fPoint2.X) And (lineWidth(i).fPoint2.X <= lineDepth(j).fPoint2.X)) Then
                    maxDistance = lineDepth(j).fPoint1.Y - lineWidth(i).fPoint2.Y
                    maxPointer = j
                End If

                '--find min point from point1 + record distance
                If (cekMin = True) AndAlso _
                   (lineDepth(j).fPoint1.Y < lineWidth(i).fPoint1.Y) And _
                   ((lineWidth(i).fPoint1.Y - lineDepth(j).fPoint1.Y) < minDistance) And _
                   ((lineDepth(j).fPoint1.X <= lineWidth(i).fPoint1.X) And (lineWidth(i).fPoint1.X <= lineDepth(j).fPoint2.X)) Then
                    minDistance = lineWidth(i).fPoint1.Y - lineDepth(j).fPoint1.Y
                    minPointer = j
                End If

                '--get maxDefault
                If (lineDepth(j).fPoint1.Y = lineWidth(i).fPoint2.Y) AndAlso _
                    ((lineDepth(j).fPoint1.X <= lineWidth(i).fPoint2.X) And (lineWidth(i).fPoint2.X <= lineDepth(j).fPoint2.X)) Then
                    defaultMaxPointer = j
                End If

                '--get minDefault
                If (lineDepth(j).fPoint1.Y = lineWidth(i).fPoint1.Y) AndAlso _
                    ((lineDepth(j).fPoint1.X <= lineWidth(i).fPoint1.X) And (lineWidth(i).fPoint1.X <= lineDepth(j).fPoint2.X)) Then
                    defaultMinPointer = j
                End If
            Next

            If (cekMax = False) Then maxPointer = defaultMinPointer
            If (cekMin = False) Then minPointer = defaultMaxPointer

            'get pararelpiped
            If (lineWidth(i).IsEqualTo(New Line3D(New Point3D(lineWidth(i).fPoint1.X, lineDepth(minPointer).fPoint1.Y, lineWidth(i).fPoint1.Z), _
                                                  New Point3D(lineWidth(i).fPoint2.X, lineDepth(maxPointer).fPoint1.Y, lineWidth(i).fPoint2.Z))) = False) Then
                k += 1
                pararelpipedWidth(k) = New Line3D(New Point3D(lineWidth(i).fPoint1.X, lineDepth(minPointer).fPoint1.Y, lineWidth(i).fPoint1.Z), _
                                                  New Point3D(lineWidth(i).fPoint2.X, lineDepth(maxPointer).fPoint1.Y, lineWidth(i).fPoint2.Z))
            End If
            If cekMin = True Then
                k += 1
                pararelpipedWidth(k) = New Line3D(New Point3D(lineWidth(i).fPoint1.X, lineDepth(minPointer).fPoint1.Y, lineWidth(i).fPoint1.Z), _
                                                  New Point3D(lineWidth(i).fPoint2.X, lineDepth(defaultMinPointer).fPoint1.Y, lineWidth(i).fPoint2.Z))
            End If
            If cekMax = True Then
                k += 1
                pararelpipedWidth(k) = New Line3D(New Point3D(lineWidth(i).fPoint1.X, lineDepth(defaultMaxPointer).fPoint1.Y, lineWidth(i).fPoint1.Z), _
                                                  New Point3D(lineWidth(i).fPoint2.X, lineDepth(maxPointer).fPoint1.Y, lineWidth(i).fPoint2.Z))
            End If
        Next

        'update pararelpiped
        ReDim cekPararelpiped(k)
        For i = 1 To k - 1
            If (cekPararelpiped(i) = False) Then
                For j = i + 1 To k
                    If pararelpipedWidth(i).IsEqualTo(pararelpipedWidth(j)) = True Then
                        cekPararelpiped(j) = True
                    End If
                Next
            End If
        Next
        j = 0
        For i = 1 To k
            If (cekPararelpiped(i) = False) Then
                j += 1
                If (i <> j) Then pararelpipedWidth(j) = New Line3D(pararelpipedWidth(i))
            End If
        Next

        'resize array size
        ReDim Preserve pararelpipedWidth(j)
        '======
        '======
        'lineDepth
        'resize array
        ReDim pararelpipedDepth(lineDepth.GetUpperBound(0) * 3)
        '-iteration for each lineWidth
        k = 0
        For i = 1 To lineDepth.GetUpperBound(0)
            '--reset data
            minDistance = defaultValue : maxDistance = defaultValue
            For j = 1 To contourPoint.GetUpperBound(0)
                If lineDepth(i).fPoint1.IsEqualTo(contourPoint(j)) = True Then cekMin = fisibelPoint(j)
                If lineDepth(i).fPoint2.IsEqualTo(contourPoint(j)) = True Then cekMax = fisibelPoint(j)
            Next

            'geting min and max point
            For j = 1 To lineWidth.GetUpperBound(0)
                '--find max point from point2 + record distance
                If (cekMax = True) AndAlso _
                   (lineWidth(j).fPoint1.X > lineDepth(i).fPoint2.X) And _
                   ((lineWidth(j).fPoint1.X - lineDepth(i).fPoint2.X) < maxDistance) And _
                   ((lineWidth(j).fPoint1.Y <= lineDepth(i).fPoint2.Y) And (lineDepth(i).fPoint2.Y <= lineWidth(j).fPoint2.Y)) Then
                    maxDistance = lineWidth(j).fPoint1.X - lineDepth(i).fPoint2.X
                    maxPointer = j
                End If

                '--find min point from point1 + record distance
                If (cekMin = True) AndAlso _
                   (lineWidth(j).fPoint1.X < lineDepth(i).fPoint1.X) And _
                   ((lineDepth(i).fPoint1.X - lineWidth(j).fPoint1.X) < minDistance) And _
                   ((lineWidth(j).fPoint1.Y <= lineDepth(i).fPoint1.Y) And (lineDepth(i).fPoint1.Y <= lineWidth(j).fPoint2.Y)) Then
                    minDistance = lineDepth(i).fPoint1.X - lineWidth(j).fPoint1.X
                    minPointer = j
                End If

                '--get max default
                If (lineWidth(j).fPoint1.X = lineDepth(i).fPoint2.X) AndAlso _
                    ((lineWidth(j).fPoint1.Y <= lineDepth(i).fPoint2.Y) And (lineDepth(i).fPoint2.Y <= lineWidth(j).fPoint2.Y)) Then
                    defaultMaxPointer = j
                End If

                '--get min default
                If (lineWidth(j).fPoint1.X = lineDepth(i).fPoint1.X) AndAlso _
                   ((lineWidth(j).fPoint1.Y <= lineDepth(i).fPoint1.Y) And (lineDepth(i).fPoint1.Y <= lineWidth(j).fPoint2.Y)) Then
                    defaultMinPointer = j
                End If
            Next

            If (cekMax = False) Then maxPointer = defaultMinPointer
            If (cekMin = False) Then minPointer = defaultMaxPointer

            'get pararelpiped
            If (lineDepth(i).IsEqualTo(New Line3D(New Point3D(lineWidth(minPointer).fPoint1.X, lineDepth(i).fPoint1.Y, lineDepth(i).fPoint1.Z), _
                                                  New Point3D(lineWidth(maxPointer).fPoint1.X, lineDepth(i).fPoint2.Y, lineDepth(i).fPoint2.Z))) = False) Then
                k += 1
                pararelpipedDepth(k) = New Line3D(New Point3D(lineWidth(minPointer).fPoint1.X, lineDepth(i).fPoint1.Y, lineDepth(i).fPoint1.Z), _
                                                  New Point3D(lineWidth(maxPointer).fPoint1.X, lineDepth(i).fPoint2.Y, lineDepth(i).fPoint2.Z))
            End If
            If cekMin = True Then
                k += 1
                pararelpipedDepth(k) = New Line3D(New Point3D(lineWidth(minPointer).fPoint1.X, lineDepth(i).fPoint1.Y, lineDepth(i).fPoint1.Z), _
                                                  New Point3D(lineWidth(defaultMinPointer).fPoint1.X, lineDepth(i).fPoint2.Y, lineDepth(i).fPoint2.Z))
            End If
            If cekMax = True Then
                k += 1
                pararelpipedDepth(k) = New Line3D(New Point3D(lineWidth(defaultMaxPointer).fPoint1.X, lineDepth(i).fPoint1.Y, lineDepth(i).fPoint1.Z), _
                                                  New Point3D(lineWidth(maxPointer).fPoint1.X, lineDepth(i).fPoint2.Y, lineDepth(i).fPoint2.Z))
            End If
        Next

        'update pararelpiped
        ReDim cekPararelpiped(k)
        For i = 1 To k - 1
            If (cekPararelpiped(i) = False) Then
                For j = i + 1 To k
                    If pararelpipedDepth(i).IsEqualTo(pararelpipedDepth(j)) = True Then
                        cekPararelpiped(j) = True
                    End If
                Next
            End If
        Next
        j = 0
        For i = 1 To k
            If (cekPararelpiped(i) = False) Then
                j += 1
                If (i <> j) Then pararelpipedDepth(j) = New Line3D(pararelpipedDepth(i))
            End If
        Next

        'resize array size
        ReDim Preserve pararelpipedDepth(j)
    End Sub

    ''' <summary>
    ''' Get contour
    ''' -Start from initial point
    ''' --0. Parameter set
    ''' --1. Varuable set
    ''' --2. Generate all line
    ''' --3. Substract line that coincidence
    ''' --4. Normalize line
    ''' </summary>
    Public Sub GetContour(ByRef lineContour() As Line3D, ByVal contourBox() As Box, ByVal Above As Boolean)
        '(1)
        Dim i As Integer

        '(2)
        ReDim lineContour(contourBox.GetUpperBound(0) * 4)
        If Above = True Then
            For i = 1 To contourBox.GetUpperBound(0)
                With contourBox(i)
                    lineContour(((i - 1) * 4) + 1) = New Line3D(.AbsPos1.X, .AbsPos1.Y, .AbsPos2.Z, _
                                                                .AbsPos2.X, .AbsPos1.Y, .AbsPos2.Z)
                    lineContour(((i - 1) * 4) + 2) = New Line3D(.AbsPos2.X, .AbsPos1.Y, .AbsPos2.Z, _
                                                                .AbsPos2.X, .AbsPos2.Y, .AbsPos2.Z)
                    lineContour(((i - 1) * 4) + 3) = New Line3D(.AbsPos1.X, .AbsPos2.Y, .AbsPos2.Z, _
                                                                .AbsPos2.X, .AbsPos2.Y, .AbsPos2.Z)
                    lineContour(((i - 1) * 4) + 4) = New Line3D(.AbsPos1.X, .AbsPos1.Y, .AbsPos2.Z, _
                                                                .AbsPos1.X, .AbsPos2.Y, .AbsPos2.Z)
                End With
            Next
        Else
            For i = 1 To contourBox.GetUpperBound(0)
                With contourBox(i)
                    lineContour(((i - 1) * 4) + 1) = New Line3D(.AbsPos1.X, .AbsPos1.Y, .AbsPos1.Z, _
                                                                .AbsPos2.X, .AbsPos1.Y, .AbsPos1.Z)
                    lineContour(((i - 1) * 4) + 2) = New Line3D(.AbsPos2.X, .AbsPos1.Y, .AbsPos1.Z, _
                                                                .AbsPos2.X, .AbsPos2.Y, .AbsPos1.Z)
                    lineContour(((i - 1) * 4) + 3) = New Line3D(.AbsPos1.X, .AbsPos2.Y, .AbsPos1.Z, _
                                                                .AbsPos2.X, .AbsPos2.Y, .AbsPos1.Z)
                    lineContour(((i - 1) * 4) + 4) = New Line3D(.AbsPos1.X, .AbsPos1.Y, .AbsPos1.Z, _
                                                                .AbsPos1.X, .AbsPos2.Y, .AbsPos1.Z)
                End With
            Next
        End If
        

        '//kayanya ga usah deh
        'NormalizeLine(lineContour)

        '(3)
        MergeContour(lineContour)

        '(4)
        '#normalize line final
        NormalizeLine(lineContour)
    End Sub

    ''' <summary>
    ''' Separation box from target box and not
    ''' -Getting box that has same target height
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Separation process
    ''' --3. Return value
    ''' </summary>
    Private Function GetSeparatedBox(ByVal addBox() As Box, ByVal startPoint As Point3D, ByVal IsNewContour As Boolean) As Box()
        '(1)
        Dim i, j As Integer
        Dim tempBox(addBox.GetUpperBound(0)) As Box

        '(2)
        j = 0
        For i = 1 To addBox.GetUpperBound(0)
            If (IsNewContour = True) AndAlso (addBox(i).AbsPos2.Z = startPoint.Z) Then
                j += 1
                tempBox(j) = New Box(addBox(i))
            End If
            If (IsNewContour = False) AndAlso (addBox(i).AbsPos1.Z = startPoint.Z) Then
                j += 1
                tempBox(j) = New Box(addBox(i))
            End If

        Next
        ReDim Preserve tempBox(j)

        '(3)
        Return tempBox
    End Function

    ''' <summary>
    ''' Separation line from target line and not
    ''' -Getting line that has same target height
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Separation process
    ''' --3. Return value
    ''' </summary>
    Private Function GetSeparatedLine(ByVal lineContour() As Line3D, _
                                      ByVal startPoint As Point3D, _
                                      ByRef restContour() As Line3D) As Line3D()
        '(1)
        Dim i, j, k As Integer
        Dim tempContour(lineContour.GetUpperBound(0)) As Line3D
        ReDim restContour(lineContour.GetUpperBound(0))

        '(2)
        j = 0
        k = 0
        For i = 1 To lineContour.GetUpperBound(0)
            If (lineContour(i).IsHeightLine = False) And (lineContour(i).fPoint1.Z = startPoint.Z) Then
                j += 1
                tempContour(j) = New Line3D(lineContour(i))
            Else
                k += 1
                restContour(k) = New Line3D(lineContour(i))
            End If
        Next
        ReDim Preserve tempContour(j)
        ReDim Preserve restContour(k)

        '(3)
        Return tempContour
    End Function

    ''' <summary>
    ''' Generate maximal space
    ''' </summary>
    ''' <remarks>
    ''' Basic idea:
    ''' - maximal space is generated by pararelpipedLine and complementer line...
    ''' - for example, a pararelpiped width line will be complemented by depth line... if pararelpiped line has fix distance, the complemented line is choosen from minimum distance of both line.
    ''' 
    ''' - for example
    ''' |   |
    ''' |   |
    ''' |=|
    ''' |   |
    ''' |   |
    ''' 
    ''' if = --&gt; pararelpiped line then "|" complementer line.
    ''' the maximal space will be"=".distance * min("|").distance
    ''' </remarks>
    Private Sub GenerateMaximalSpace(ByVal contourPoint() As Point3D, _
                                     ByVal fisibelPoint() As Boolean, _
                                     ByVal lineWidth() As Line3D, _
                                     ByVal lineDepth() As Line3D, _
                                     ByVal pararelWidth() As Line3D, _
                                     ByVal pararelDepth() As Line3D, _
                                     ByRef empSpace() As Kotak)
        'if there's no pararel line --> maximalspace = a whole area
        If (pararelDepth.GetUpperBound(0) > 0) And (pararelWidth.GetUpperBound(0) > 0) Then
            'variable
            Dim i, j As Integer
            Dim compLine1, compLine2, complementLine As Line3D
            Dim pararelPoint1, pararelPoint2 As Point3D
            Dim pLine1Min, pLine1Max, pLine2Min, pLine2Max As Point3D
            Dim dLine1Min, dLine1Max, dLine2Min, dLine2Max As Single
            Dim emptyspaceDepth(pararelDepth.GetUpperBound(0)), emptyspaceWidth(pararelWidth.GetUpperBound(0)) As Kotak

            'reset data
            pLine1Min = New Point3D(0, 0, 0) : pLine2Min = New Point3D(0, 0, 0)
            pLine1Max = New Point3D(0, 0, 0) : pLine2Max = New Point3D(0, 0, 0)

            'pararelpiped Width line --> complementer = depthLine
            '===
            'iteration for each p.width line
            For i = 1 To pararelWidth.GetUpperBound(0)
                'get pararel point
                pararelPoint1 = New Point3D(pararelWidth(i).fPoint1)
                pararelPoint2 = New Point3D(pararelWidth(i).fPoint2)

                dLine1Min = fMax3(CSng(MyForm.formMainMenu.txtDConDepth.Text), _
                                        CSng(MyForm.formMainMenu.txtDConWidth.Text), _
                                        CSng(MyForm.formMainMenu.txtDConHeight.Text))
                dLine1Max = dLine1Min
                dLine2Min = dLine1Min
                dLine2Max = dLine1Min

                'get complementer line (top-bottom)
                For j = 1 To lineWidth.GetUpperBound(0)
                    With lineWidth(j)
                        'get complementerBottomLine
                        If (.fPoint1.Y <= pararelPoint1.Y) And (pararelPoint1.Y < .fPoint2.Y) Then
                            'get left (to minimum)
                            If (.fPoint1.X < pararelPoint1.X) And ((pararelPoint1.X - .fPoint1.X) < dLine1Min) Then
                                dLine1Min = pararelPoint1.X - .fPoint1.X
                                pLine1Min = New Point3D(.fPoint1.X, pararelPoint1.Y, pararelPoint1.Z)
                            End If
                            'get right (to maximum)
                            If (.fPoint1.X > pararelPoint1.X) And ((.fPoint1.X - pararelPoint1.X) < dLine1Max) Then
                                dLine1Max = .fPoint1.X - pararelPoint1.X
                                pLine1Max = New Point3D(.fPoint1.X, pararelPoint1.Y, pararelPoint1.Z)
                            End If
                        End If

                        'get complementerTopLine
                        If (.fPoint1.Y < pararelPoint2.Y) And (pararelPoint2.Y <= .fPoint2.Y) Then
                            'get left (to minimum)
                            If (.fPoint1.X < pararelPoint2.X) And ((pararelPoint2.X - .fPoint1.X) < dLine2Min) Then
                                dLine2Min = pararelPoint2.X - .fPoint1.X
                                pLine2Min = New Point3D(.fPoint1.X, pararelPoint2.Y, pararelPoint2.Z)
                            End If
                            'get right (to maximum)
                            If (.fPoint1.X > pararelPoint2.X) And ((.fPoint1.X - pararelPoint2.X) < dLine2Max) Then
                                dLine2Max = .fPoint1.X - pararelPoint2.X
                                pLine2Max = New Point3D(.fPoint1.X, pararelPoint2.Y, pararelPoint1.Z)
                            End If
                        End If
                    End With
                Next

                'establish complementer line
                compLine1 = New Line3D(pLine1Min, pLine1Max)
                compLine2 = New Line3D(pLine2Min, pLine2Max)
                complementLine = compLine1.GetIntersection(compLine2)

                'get maximal space
                emptyspaceWidth(i) = New Kotak(pararelWidth(i).Length, complementLine.Length, fOrigin.Z)
                emptyspaceWidth(i).Position = New Point3D(complementLine.fPoint1.X, pararelWidth(i).fPoint1.Y, pararelWidth(i).fPoint2.Z)
            Next

            'pararelpiped Depth line --> complementer = widthLine
            '===
            'iteration for each p.depth line
            For i = 1 To pararelDepth.GetUpperBound(0)
                'get pararel point
                pararelPoint1 = New Point3D(pararelDepth(i).fPoint1)
                pararelPoint2 = New Point3D(pararelDepth(i).fPoint2)

                dLine1Min = fMax3(CSng(MyForm.formMainMenu.txtDConDepth.Text), _
                                        CSng(MyForm.formMainMenu.txtDConWidth.Text), _
                                        CSng(MyForm.formMainMenu.txtDConHeight.Text))
                dLine1Max = dLine1Min
                dLine2Min = dLine1Min
                dLine2Max = dLine1Min

                'get complementer line (left-right)
                For j = 1 To lineDepth.GetUpperBound(0)
                    With lineDepth(j)
                        'get complementerLeftLine
                        If (.fPoint1.X <= pararelPoint1.X) And (pararelPoint1.X < .fPoint2.X) Then
                            'get bottom (to minimum)
                            If (.fPoint1.Y < pararelPoint1.Y) And ((pararelPoint1.Y - .fPoint1.Y) < dLine1Min) Then
                                dLine1Min = pararelPoint1.Y - .fPoint1.Y
                                pLine1Min = New Point3D(pararelPoint1.X, .fPoint1.Y, pararelPoint1.Z)
                            End If
                            'get top (to maximum)
                            If (.fPoint1.Y > pararelPoint1.Y) And ((.fPoint1.Y - pararelPoint1.Y) < dLine1Max) Then
                                dLine1Max = .fPoint1.Y - pararelPoint1.Y
                                pLine1Max = New Point3D(pararelPoint1.X, .fPoint1.Y, pararelPoint1.Z)
                            End If
                        End If

                        'get complementerRightLine
                        If (.fPoint1.X < pararelPoint2.X) And (pararelPoint2.X <= .fPoint2.X) Then
                            'get bottom (to minimum)
                            If (.fPoint1.Y < pararelPoint2.Y) And ((pararelPoint2.Y - .fPoint1.Y) < dLine2Min) Then
                                dLine2Min = pararelPoint2.Y - .fPoint1.Y
                                pLine2Min = New Point3D(pararelPoint2.X, .fPoint1.Y, pararelPoint2.Z)
                            End If
                            'get top (to maximum)
                            If (.fPoint1.Y > pararelPoint2.Y) And ((.fPoint1.Y - pararelPoint2.Y) < dLine2Max) Then
                                dLine2Max = .fPoint1.Y - pararelPoint2.Y
                                pLine2Max = New Point3D(pararelPoint2.X, .fPoint1.Y, pararelPoint1.Z)
                            End If
                        End If
                    End With
                Next

                'establish complementer line
                compLine1 = New Line3D(pLine1Min, pLine1Max)
                compLine2 = New Line3D(pLine2Min, pLine2Max)
                complementLine = compLine1.GetIntersection(compLine2)

                'get maximal space
                emptyspaceDepth(i) = New Kotak(complementLine.Length, pararelDepth(i).Length, fOrigin.Z)
                emptyspaceDepth(i).Position = New Point3D(pararelDepth(i).fPoint1.X, complementLine.fPoint1.Y, pararelDepth(i).fPoint2.Z)
            Next

            'return value of emptyspace
            ReDim empSpace(emptyspaceDepth.GetUpperBound(0) + emptyspaceWidth.GetUpperBound(0))
            For i = 1 To emptyspaceWidth.GetUpperBound(0)
                empSpace(i) = New Kotak(emptyspaceWidth(i).Width, emptyspaceWidth(i).Depth, emptyspaceWidth(i).Height)
                empSpace(i).Position = New Point3D(emptyspaceWidth(i).Position)
            Next
            For i = 1 To emptyspaceDepth.GetUpperBound(0)
                empSpace(emptyspaceWidth.GetUpperBound(0) + i) = New Kotak(emptyspaceDepth(i).Width, emptyspaceDepth(i).Depth, emptyspaceDepth(i).Height)
                empSpace(emptyspaceWidth.GetUpperBound(0) + i).Position = New Point3D(emptyspaceDepth(i).Position)
            Next

            ''additional emptyspace (multiply from pararelpipedWidth * pararelpipedDepth
            'ReDim Preserve empSpace(emptyspaceDepth.GetUpperBound(0) + emptyspaceWidth.GetUpperBound(0) + (pararelDepth.GetUpperBound(0) * pararelWidth.GetUpperBound(0)))
            'k = emptyspaceDepth.GetUpperBound(0) + emptyspaceWidth.GetUpperBound(0)
            'For i = 1 To pararelDepth.GetUpperBound(0)
            '    For j = 1 To pararelWidth.GetUpperBound(0)
            '        k += 1
            '        empSpace(k) = New Kotak(pararelWidth(j).Length, pararelDepth(i).Length, FOrigin.Z)
            '        empSpace(k).Position = New Point3D(pararelDepth(i).FPoint1.X, pararelWidth(j).FPoint1.Y, pararelDepth(i).FPoint2.Z)
            '    Next
            'Next

            'harusnya pas disini ada filtrasi.. kalo misalnya ada empty space yang berada didalem.. tp sekarang mending uji coba dulu deh
            NormalizeMaximalSpace(contourPoint, fisibelPoint, lineWidth, lineDepth, empSpace)
        Else
            ReDim empSpace(1)
            empSpace(1) = New Kotak(lineWidth(1).Length, lineDepth(1).Length, fOrigin.Z)
            empSpace(1).Position = New Point3D(lineDepth(1).fPoint1.X, lineWidth(1).fPoint1.Y, lineDepth(1).fPoint2.Z)
        End If

    End Sub

    ''' <summary>
    ''' Normalize maximal space >> eliminated if inbound maximal space
    ''' </summary>
    Private Sub NormalizeMaximalSpace(ByVal contourPoint() As Point3D, ByVal fisibelPoint() As Boolean, ByVal lineWidth() As Line3D, ByVal lineDepth() As Line3D, ByRef empSpace() As Kotak)
        Dim i, j, k As Integer
        Dim notFisibel(empSpace.GetUpperBound(0)) As Boolean       'default value = false
        Dim lineContour(4) As Line3D

        'find intersection-perpendicular section
        For i = 1 To empSpace.GetUpperBound(0)
            'generate all line --> for 1 box
            With empSpace(i)
                lineContour(1) = New Line3D(.Position.X, .Position.Y, .Position.Z, _
                                            .Position2.X, .Position.Y, .Position.Z)
                lineContour(2) = New Line3D(.Position2.X, .Position.Y, .Position.Z, _
                                            .Position2.X, .Position2.Y, .Position.Z)
                lineContour(3) = New Line3D(.Position.X, .Position2.Y, .Position.Z, _
                                            .Position2.X, .Position2.Y, .Position.Z)
                lineContour(4) = New Line3D(.Position.X, .Position.Y, .Position.Z, _
                                            .Position.X, .Position2.Y, .Position.Z)
            End With

            For j = 1 To 4
                If notFisibel(i) = False Then
                    '-compare to linewidth --> intersection in perpendicular --> emptyspace default (not fisibel) = true
                    For k = 1 To lineWidth.GetUpperBound(0)
                        If ((lineContour(j).IsDepthLine = True) And (notFisibel(i) = False)) AndAlso _
                            (lineContour(j).fPoint1.X < lineWidth(k).fPoint1.X) And (lineWidth(k).fPoint1.X < lineContour(j).fPoint2.X) And _
                            (lineWidth(k).fPoint1.Y < lineContour(j).fPoint1.Y) And (lineContour(j).fPoint1.Y < lineWidth(k).fPoint2.Y) Then
                            notFisibel(i) = True
                            Exit For
                        End If
                    Next

                    '-compare to linedepth --> intersection in perpendicular --> emptyspace default (not fisibel) = true
                    For k = 1 To lineDepth.GetUpperBound(0)
                        If ((lineContour(j).IsWidthLine = True) And (notFisibel(i) = False)) AndAlso _
                            (lineDepth(k).fPoint1.X < lineContour(j).fPoint1.X) And (lineContour(j).fPoint1.X < lineDepth(k).fPoint2.X) And _
                            (lineContour(j).fPoint1.Y < lineDepth(k).fPoint1.Y) And (lineDepth(k).fPoint1.Y < lineContour(j).fPoint2.Y) Then
                            notFisibel(i) = True
                            Exit For
                        End If
                    Next

                    '-compare linewidth + linedepth through closed-point
                    For k = 1 To contourPoint.GetUpperBound(0)
                        If ((lineContour(j).IsDepthLine = True) And (notFisibel(i) = False) And (fisibelPoint(k) = False)) AndAlso _
                            (lineContour(j).fPoint1.X < contourPoint(k).X) And (contourPoint(k).X < lineContour(j).fPoint2.X) And _
                            (lineContour(j).fPoint1.Y = contourPoint(k).Y) Then
                            notFisibel(i) = True
                            Exit For
                        End If
                        If ((lineContour(j).IsWidthLine = True) And (notFisibel(i) = False) And (fisibelPoint(k) = False)) AndAlso _
                            (lineContour(j).fPoint1.X = contourPoint(k).X) And _
                            (lineContour(j).fPoint1.Y < contourPoint(k).Y) And (contourPoint(k).Y < lineContour(j).fPoint2.Y) Then
                            notFisibel(i) = True
                            Exit For
                        End If
                    Next
                End If
            Next
        Next

        'find overlap area --> in bound area
        For i = 1 To empSpace.GetUpperBound(0) - 1
            For j = i + 1 To empSpace.GetUpperBound(0)
                If ((notFisibel(i) = False) And (notFisibel(j) = False)) AndAlso _
                    (functCheckAreaInBound(empSpace(i), empSpace(j)) = True) Then
                    If (empSpace(i).Depth * empSpace(i).Width) >= (empSpace(j).Depth * empSpace(j).Width) Then
                        notFisibel(j) = True
                    Else
                        notFisibel(i) = True
                    End If
                End If
                If ((notFisibel(i) = False) And (notFisibel(j) = False)) AndAlso _
                    (empSpace(i).IsEqualTo(empSpace(j)) = True) Then
                    notFisibel(j) = True
                End If
            Next
        Next

        'update emptyspace
        j = 0
        For i = 1 To empSpace.GetUpperBound(0)
            If (notFisibel(i) = False) Then
                j += 1
                If (i <> j) Then
                    empSpace(j) = New Kotak(empSpace(i).Width, empSpace(i).Depth, empSpace(i).Height)
                    empSpace(j).Position = New Point3D(empSpace(i).Position)
                End If
            End If
        Next
        ReDim Preserve empSpace(j)
    End Sub


    ''' <summary>
    ''' ReviseContour
    ''' -Revising old contour by adding or subtracting new lines
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Resize contour
    ''' --3. Generate all line (1 box)
    ''' </summary>
    Private Sub ReviseContour(ByRef oldContour() As Line3D, ByVal addBox() As Box)
        '(1)
        Dim lineContour(4), tempLine() As Line3D
        Dim cek(4) As Boolean
        Dim i, j, k, count, fixcount, rest As Integer

        For j = 1 To addBox.GetUpperBound(0)
            '//Resize contour
            count = oldContour.GetUpperBound(0)

            '//Generate all line --> for 1 box
            With addBox(j)
                lineContour(1) = New Line3D(.AbsPos1.X, .AbsPos1.Y, .AbsPos1.Z, _
                                            .AbsPos2.X, .AbsPos1.Y, .AbsPos1.Z)
                lineContour(2) = New Line3D(.AbsPos1.X, .AbsPos2.Y, .AbsPos1.Z, _
                                            .AbsPos2.X, .AbsPos2.Y, .AbsPos1.Z)
                lineContour(3) = New Line3D(.AbsPos1.X, .AbsPos1.Y, .AbsPos1.Z, _
                                            .AbsPos1.X, .AbsPos2.Y, .AbsPos1.Z)
                lineContour(4) = New Line3D(.AbsPos2.X, .AbsPos1.Y, .AbsPos1.Z, _
                                            .AbsPos2.X, .AbsPos2.Y, .AbsPos1.Z)
            End With

            '//Reset data
            For k = 1 To 4
                cek(k) = False
            Next
            rest = 4
            fixcount = count

            '//Substract line
            For i = 1 To fixcount
                For k = 1 To 4
                    If ((lineContour(k).IsIntersectionWith(oldContour(i)) = True) And (cek(k) = False)) AndAlso _
                        (lineContour(k).GetIntersection(oldContour(i)).Length > 0) Then
                        tempLine = oldContour(i).SubstractSpecial(lineContour(k))
                        If tempLine.GetUpperBound(0) = 1 Then
                            oldContour(i) = New Line3D(tempLine(1))
                        Else
                            '//If intersection occur in middle, result 2 new line
                            ReDim Preserve oldContour(oldContour.GetUpperBound(0) + 1)
                            oldContour(i) = New Line3D(tempLine(1))
                            count += 1
                            oldContour(oldContour.GetUpperBound(0)) = New Line3D(tempLine(2))
                        End If

                        cek(k) = True
                        rest -= 1
                    End If
                Next

                For k = 1 To count
                    If ((i <> k) And (oldContour(k).IsIntersectionWith(oldContour(i)) = True)) AndAlso _
                        (oldContour(k).GetIntersection(oldContour(i)).Length > 0) Then
                        tempLine = oldContour(i).SubstractSpecial(oldContour(k))
                        If tempLine.GetUpperBound(0) = 1 Then
                            oldContour(i) = New Line3D(tempLine(1))
                            oldContour(k) = New Line3D(0, 0, 0, 0, 0, 0)
                        Else
                            '//If intersection occur in middle, result 2 new line
                            oldContour(i) = New Line3D(tempLine(1))
                            oldContour(k) = New Line3D(tempLine(2))
                        End If
                    End If
                Next
            Next

            '//Add new line to contour
            ReDim Preserve oldContour(count + rest)
            For k = 1 To 4
                If cek(k) = False Then
                    count += 1
                    oldContour(count) = New Line3D(lineContour(k))
                End If
            Next

            '//Normalize data
            NormalizeLine(oldContour)
        Next
    End Sub

    ''' <summary>
    ''' Merge contour
    ''' -Merge by eliminating unused contour
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Merge by substact it
    ''' </summary>
    Private Sub MergeContour(ByRef lineContour() As Line3D)
        '(1)
        Dim i, j As Integer

        '(2)
        For i = 1 To (lineContour.GetUpperBound(0) - 1)
            For j = i + 1 To lineContour.GetUpperBound(0)
                lineContour(i).MergeSpecial(lineContour(j))
            Next
        Next

        'Dim tempLine() As Line3D
        'For i = 1 To (lineContour.GetUpperBound(0) - 1)
        '    For j = (i + 1) To lineContour.GetUpperBound(0)
        '        If (lineContour(j).IsIntersectionWith(lineContour(i)) = True) AndAlso _
        '           (lineContour(j).GetIntersection(lineContour(i)).Length > 0) Then
        '            tempLine = lineContour(i).SubstractSpecial(lineContour(j))
        '            If tempLine.GetUpperBound(0) = 1 Then
        '                lineContour(i) = New Line3D(tempLine(1))
        '                lineContour(j) = New Line3D(0, 0, 0, 0, 0, 0)
        '            Else
        '                'if intersection occur in middle, result 2 new line
        '                lineContour(i) = New Line3D(tempLine(1))
        '                lineContour(j) = New Line3D(tempLine(2))
        '            End If
        '        End If
        '    Next
        'Next
    End Sub


    ''' <summary>
    ''' Normalize line
    ''' -Eliminated unused line
    ''' </summary>
    ''' <remarks>
    ''' What can be normalize:
    ''' 1. Adding line (same orientation, neighborhood line) >> suppose to be add into 1 line
    ''' 2. Identical line
    ''' 3. Intersection line
    ''' 4. Zero length-line
    ''' </remarks>
    Private Sub NormalizeLine(ByRef lineContour() As Line3D)
        Dim i, j As Integer
        Dim notFisibel(lineContour.GetUpperBound(0)) As Boolean

        '+NO LENGTH = 0
        '//No length = 0
        For i = 1 To lineContour.GetUpperBound(0)
            If lineContour(i).Length = 0 Then notFisibel(i) = True
        Next

        '//Update contour
        j = 0
        For i = 1 To lineContour.GetUpperBound(0)
            If (notFisibel(i) = False) Then
                j += 1
                If (i <> j) Then
                    lineContour(j) = New Line3D(lineContour(i))
                End If
            End If
        Next
        ReDim Preserve lineContour(j)
        ReDim notFisibel(lineContour.GetUpperBound(0))

        '===
        '+ADD LINE
        '//Copy lineContour
        Dim backupContour(lineContour.GetUpperBound(0))
        For i = 1 To lineContour.GetUpperBound(0)
            backupContour(i) = New Line3D(lineContour(i))
        Next

        '//Adding same line
        For i = 1 To lineContour.GetUpperBound(0) - 1
            For j = 1 To lineContour.GetUpperBound(0)
                If lineContour(j).Length = 0 Then
                    notFisibel(j) = True
                End If
                If ((i <> j) And (notFisibel(i) = False) And (notFisibel(j) = False)) AndAlso _
                    (lineContour(i).IsEqualTo(lineContour(j)) = True) Then
                    notFisibel(j) = True
                End If
                If ((i <> j) And (notFisibel(i) = False) And (notFisibel(j) = False)) AndAlso _
                    (lineContour(i).GetUnion(lineContour(j)).Length > 0) Then
                    lineContour(i) = lineContour(i).GetUnion(lineContour(j))
                    notFisibel(j) = True
                End If
            Next
        Next

        '//Update contour
        j = 0
        For i = 1 To lineContour.GetUpperBound(0)
            If (notFisibel(i) = False) Then
                j += 1
                If (i <> j) Then
                    lineContour(j) = New Line3D(lineContour(i))
                End If
            End If
        Next
        ReDim Preserve lineContour(j)
        ReDim notFisibel(lineContour.GetUpperBound(0))

        '===
        '+PERPENDICULAR
        '!!this procedure is a trouble maker...
        '//If number line mod 2 = 0 then --> it's contour
        '//Failed normalization
        If (j Mod 2) = 1 Then
            ReDim lineContour(backupContour.GetUpperBound(0))
            For i = 1 To backupContour.GetUpperBound(0)
                lineContour(i) = New Line3D(backupContour(i))
            Next
        End If

        '//Cek for perpendicular...
        '//Extract contour if perpendicular exist
        For i = 1 To lineContour.GetUpperBound(0) - 1
            For j = i + 1 To lineContour.GetUpperBound(0)
                If (i <> j) AndAlso _
                    (lineContour(i).IsDepthLine = Not lineContour(j).IsDepthLine) AndAlso _
                    (lineContour(i).fPoint1.X < lineContour(j).fPoint1.X) And (lineContour(j).fPoint1.X < lineContour(i).fPoint2.X) And _
                    (lineContour(j).fPoint1.Y < lineContour(i).fPoint1.Y) And (lineContour(i).fPoint1.Y < lineContour(j).fPoint2.Y) Then

                    'if perpendicular exist --> draw 4 new line
                    ReDim Preserve lineContour(lineContour.GetUpperBound(0) + 2)
                    lineContour(lineContour.GetUpperBound(0) - 1) = New Line3D(lineContour(i))
                    lineContour(lineContour.GetUpperBound(0)) = New Line3D(lineContour(j))

                    lineContour(i).fPoint2.X = lineContour(j).fPoint1.X
                    lineContour(j).fPoint2.Y = lineContour(i).fPoint1.Y

                    lineContour(lineContour.GetUpperBound(0) - 1).fPoint1.X = lineContour(j).fPoint1.X
                    lineContour(lineContour.GetUpperBound(0)).fPoint1.Y = lineContour(i).fPoint1.Y
                    Exit For
                End If

                If (i <> j) AndAlso _
                    (lineContour(j).IsDepthLine = Not lineContour(i).IsDepthLine) AndAlso _
                    (lineContour(j).fPoint1.X < lineContour(i).fPoint1.X) And (lineContour(i).fPoint1.X < lineContour(j).fPoint2.X) And _
                    (lineContour(i).fPoint1.Y < lineContour(j).fPoint1.Y) And (lineContour(j).fPoint1.Y < lineContour(i).fPoint2.Y) Then

                    'if perpendicular exist --> draw 4 new line
                    ReDim Preserve lineContour(lineContour.GetUpperBound(0) + 2)
                    lineContour(lineContour.GetUpperBound(0) - 1) = New Line3D(lineContour(j))
                    lineContour(lineContour.GetUpperBound(0)) = New Line3D(lineContour(i))

                    lineContour(j).fPoint2.X = lineContour(i).fPoint1.X
                    lineContour(i).fPoint2.Y = lineContour(j).fPoint1.Y

                    lineContour(lineContour.GetUpperBound(0) - 1).fPoint1.X = lineContour(i).fPoint1.X
                    lineContour(lineContour.GetUpperBound(0)).fPoint1.Y = lineContour(j).fPoint1.Y
                    Exit For
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' Get closed-contour-point and opened-contour-point
    ''' </summary>
    Private Sub GetFeasiblePoint(ByVal lineContour() As Line3D, ByRef contourPoint() As Point3D, ByRef fisibelPoint() As Boolean)
        'get point and possibility to packing
        ReDim fisibelPoint(lineContour.GetUpperBound(0))
        ReDim contourPoint(lineContour.GetUpperBound(0))

        'depthLine = true --> is depth line,
        'UP = depthline(false), istrue(true)
        'DOWN  = depthline(false), istrue(false)
        'RIGHT = depthline(true), istrue(true)
        'LEFT = depthline(true), istrue(false)
        Dim IsDepthLine1, IsDepthLine2, IsDirection1, IsDirection2 As Boolean
        For i = 1 To lineContour.GetUpperBound(0)
            If lineContour(i).fDirection = True Then
                contourPoint(i) = New Point3D(lineContour(i).fPoint1)
            Else
                contourPoint(i) = New Point3D(lineContour(i).fPoint2)
            End If
            fisibelPoint(i) = False

            'set define UP, DOWN, RIGHT, LEFT --> depthLine+direction
            If i = 1 Then
                IsDepthLine1 = lineContour(lineContour.GetUpperBound(0)).IsDepthLine
                IsDirection1 = lineContour(lineContour.GetUpperBound(0)).fDirection
                IsDepthLine2 = lineContour(1).IsDepthLine
                IsDirection2 = lineContour(1).fDirection
            Else
                IsDepthLine1 = lineContour(i - 1).IsDepthLine
                IsDirection1 = lineContour(i - 1).fDirection
                IsDepthLine2 = lineContour(i).IsDepthLine
                IsDirection2 = lineContour(i).fDirection
            End If

            'UP = isdepthline=false + fdirection=true
            'DOWN = isdepthline=false + fdirection=false
            'RIGHT = isdepthline=true + direction=true
            'LEFT = isdepthline=true + direction=false

            'UP-RIGHT
            'RIGHT-DOWN
            'DOWN-LEFT
            'LEFT-UP
            If ((IsDepthLine1 = False) And (IsDirection1 = True) And (IsDepthLine2 = True) And (IsDirection2 = True)) Or _
                ((IsDepthLine1 = True) And (IsDirection1 = True) And (IsDepthLine2 = False) And (IsDirection2 = False)) Or _
                ((IsDepthLine1 = False) And (IsDirection1 = False) And (IsDepthLine2 = True) And (IsDirection2 = False)) Or _
                ((IsDepthLine1 = True) And (IsDirection1 = False) And (IsDepthLine2 = False) And (IsDirection2 = True)) Then
                fisibelPoint(i) = True
            End If
        Next
    End Sub


    ''' <summary>
    ''' ReviseContour
    ''' -Revising old contour by adding or subtracting new lines
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Generate all line from contourbox
    ''' --3. Merge old and new contour
    ''' --4. Merge all contour
    ''' --5. Normalize contour
    ''' </summary>
    Private Sub ReviseContourTemp(ByRef lineContour() As Line3D, ByVal contourBox() As Box)
        '(1)
        Dim i, j As Integer
        Dim tempLine(Nothing) As Line3D

        '(2)
        ReDim tempLine(contourBox.GetUpperBound(0) * 4)
        j = 0
        For i = 1 To contourBox.GetUpperBound(0)
            With contourBox(i)
                If .AbsPos1.Z = lineContour(1).fPoint1.Z Then
                    j += 1
                    tempLine(((i - 1) * 4) + 1) = New Line3D(.AbsPos1.X, .AbsPos1.Y, .AbsPos2.Z, _
                                                             .AbsPos2.X, .AbsPos1.Y, .AbsPos2.Z)
                    tempLine(((i - 1) * 4) + 2) = New Line3D(.AbsPos2.X, .AbsPos1.Y, .AbsPos2.Z, _
                                                             .AbsPos2.X, .AbsPos2.Y, .AbsPos2.Z)
                    tempLine(((i - 1) * 4) + 3) = New Line3D(.AbsPos1.X, .AbsPos2.Y, .AbsPos2.Z, _
                                                             .AbsPos2.X, .AbsPos2.Y, .AbsPos2.Z)
                    tempLine(((i - 1) * 4) + 4) = New Line3D(.AbsPos1.X, .AbsPos1.Y, .AbsPos2.Z, _
                                                             .AbsPos1.X, .AbsPos2.Y, .AbsPos2.Z)
                End If
            End With
        Next
        ReDim Preserve tempLine(j * 4)

        '(3)
        j = lineContour.GetUpperBound(0)
        ReDim Preserve lineContour(j + tempLine.GetUpperBound(0))
        For i = j + 1 To j + tempLine.GetUpperBound(0)
            lineContour(i) = New Line3D(tempLine(i - j))
        Next

        '(4)
        MergeContour(lineContour)

        '(5)
        NormalizeLine(lineContour)
    End Sub
End Class


''===
''get new contour
''===
''separated lineWidth and lineDepth
'Dim lineWidth(Nothing), lineDepth(Nothing) As Line3D
'        GetLineWidthDepth(lineContour, lineWidth, lineDepth)

''getting contour
'Dim IsLineDepth, stopIteration As Boolean
'Dim currentPoint, point1, point2 As Point3D         '--> as initial
'        ReDim FContour(contourBox.GetUpperBound(0) * 4)

''reset data
'        point2 = New Point3D(-1, -1, -1)
'        j = 0
'        IsLineDepth = True
'        currentPoint = startPoint

''start iteration
'        Do Until point2.IsEqualTo(startPoint)
''reset data
'            stopIteration = False

''beginning operation
'            point1 = New Point3D(currentPoint)

''find maxPoint --> same Line
'            Do Until stopIteration = True
'                If IsLineDepth = True Then
'                    For i = 1 To lineDepth.GetUpperBound(0)
'                        If (lineDepth(i).FPoint1.IsEqualTo(currentPoint) = True) Or _
'                            (lineDepth(i).FPoint2.IsEqualTo(currentPoint) = True) Then
'                            If lineDepth(i).FPoint1.IsEqualTo(currentPoint) = True Then
'                                currentPoint = lineDepth(i).FPoint2
'                            Else
'                                currentPoint = lineDepth(i).FPoint1
'                            End If
'                            stopIteration = True
'                            Exit For
'                        Else
'                            stopIteration = False
'                        End If
'                    Next
'                Else
'                    For i = 1 To lineWidth.GetUpperBound(0)
'                        If (lineWidth(i).FPoint1.IsEqualTo(currentPoint) = True) Or _
'                            (lineWidth(i).FPoint2.IsEqualTo(currentPoint) = True) Then
'                            If lineWidth(i).FPoint1.IsEqualTo(currentPoint) = True Then
'                                currentPoint = lineWidth(i).FPoint2
'                            Else
'                                currentPoint = lineWidth(i).FPoint1
'                            End If
'                            stopIteration = True
'                            Exit For
'                        Else
'                            stopIteration = False
'                        End If
'                    Next
'                End If
'            Loop

''ending operation
'            point2 = New Point3D(currentPoint)

''get line
'            j += 1
'            FContour(j) = New Line3D(New Point3D(point1), New Point3D(point2))
'            IsLineDepth = Not IsLineDepth
'        Loop
