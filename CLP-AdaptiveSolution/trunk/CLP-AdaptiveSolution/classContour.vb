
Public Class Contour
    Private FContour(), FInitialContour() As Line3D
    Private FOrigin As Point3D
    Private FBox() As Box
    Private FEmptySpace() As Kotak

    ''' <summary>
    ''' Insert data from contour
    ''' Useful when a contour generate more than one differentiate contour
    ''' </summary>
    Sub New(ByRef Kontur() As Line3D, ByVal AboveContour As Boolean)
        '1. resize contour & copy data
        ReDim FContour(Kontur.GetUpperBound(0))
        ReDim FInitialContour(Kontur.GetUpperBound(0))
        For i As Integer = 1 To Kontur.GetUpperBound(0)
            FContour(i) = New Line3D(Kontur(i))
            FInitialContour(i) = New Line3D(Kontur(i))
        Next
        ReDim Kontur(0)

        '2. get minimum point
        FOrigin = GetOriginPoint(AboveContour)

        '3. sorting contour
        Dim count As Integer
        SortKontur(count)

        '4. build another contour if it result more than 1 contour
        If count < FContour.GetUpperBound(0) Then
            'resize contour
            ReDim Kontur(FContour.GetUpperBound(0) - count)
            Dim i, j As Integer
            j = 0
            For i = count + 1 To FContour.GetUpperBound(0)
                j += 1
                Kontur(j) = New Line3D(FContour(i))
            Next

            'resize (to fix) currentcontour
            ReDim Preserve FContour(count)
        End If

        '5. get maximal space
        If count > 0 Then GetMaximalSpace()
    End Sub

    ''' <summary>
    ''' Box construct
    ''' Usefull when subset box has been placed in a contour --> there's a box above contour that must be build
    ''' </summary>
    Sub New(ByVal addBox() As Box, ByVal startPoint As Point3D)
        'variable
        Dim tempBox(addBox.GetUpperBound(0)) As Box

        '1. separated box from different height
        tempBox = GetSeparatedBox(addBox, startPoint, True)

        '2. find contour from tempBox
        GetContour(tempBox, startPoint)

        '3. get minimum point
        FOrigin = GetOriginPoint(True)

        '4. sorting contour
        Dim count As Integer
        SortKontur(count)

        '5. copy to initial contour
        ReDim FInitialContour(FContour.GetUpperBound(0))
        For i As Integer = 1 To FContour.GetUpperBound(0)
            FInitialContour(i) = New Line3D(FContour(i))
        Next

        '6. due new kontur --> get maximal space
        GetMaximalSpace()
    End Sub

    ''' <summary>
    ''' Container construct
    ''' Usefull when placing first time container
    ''' </summary>
    Sub New(ByVal container As Box)
        '1. resize contour
        ReDim FContour(4), FInitialContour(4)
        FContour(1) = New Line3D(container.LocationContainer.X, container.LocationContainer.Y, container.LocationContainer.Z, _
                                 container.LocationContainer2.X, container.LocationContainer.Y, container.LocationContainer.Z)
        FContour(2) = New Line3D(container.LocationContainer2.X, container.LocationContainer.Y, container.LocationContainer.Z, _
                                 container.LocationContainer2.X, container.LocationContainer2.Y, container.LocationContainer.Z)
        FContour(3) = New Line3D(container.LocationContainer.X, container.LocationContainer2.Y, container.LocationContainer.Z, _
                                 container.LocationContainer2.X, container.LocationContainer2.Y, container.LocationContainer.Z)
        FContour(4) = New Line3D(container.LocationContainer.X, container.LocationContainer.Y, container.LocationContainer.Z, _
                                 container.LocationContainer.X, container.LocationContainer2.Y, container.LocationContainer.Z)

        For i As Integer = 1 To FContour.GetUpperBound(0)
            FInitialContour(i) = New Line3D(FContour(i))
        Next

        '2. get minimum point
        FOrigin = GetOriginPoint(False)

        '3. sorting contour
        Dim count As Integer
        SortKontur(count)

        '4. due new kontur --> get maximal space
        GetMaximalSpace()
    End Sub

    ''' <summary>
    ''' Get origin of emptySpaceArea
    ''' </summary>
    Public ReadOnly Property OriginPoint() As Point3D
        Get
            Return FOrigin
        End Get
    End Property

    ''' <summary>
    ''' Get emptyspace that generated in this contour
    ''' </summary>
    Public ReadOnly Property EmptySpace() As Kotak()
        Get
            Return FEmptySpace
        End Get
    End Property

    ''' <summary>
    ''' Cek box in contour
    ''' </summary>
    Public Function CheckBoxInContour(ByVal boxCompare As Box) As Boolean
        Dim i As Integer
        Dim cek(4) As Boolean

        'only cek for same height with areaContour
        cek(1) = False : cek(2) = False : cek(3) = False : cek(4) = False
        For i = 1 To FEmptySpace.GetUpperBound(0)
            If (FOrigin.Z = boxCompare.LocationContainer.Z) AndAlso _
                ((FEmptySpace(i).Position.X <= boxCompare.LocationContainer.X) And (boxCompare.LocationContainer.X <= FEmptySpace(i).Position2.X)) AndAlso _
                ((FEmptySpace(i).Position.Y <= boxCompare.LocationContainer.Y) And (boxCompare.LocationContainer.Y <= FEmptySpace(i).Position2.Y)) Then
                cek(1) = True
            End If
            If (FOrigin.Z = boxCompare.LocationContainer.Z) AndAlso _
                ((FEmptySpace(i).Position.X <= boxCompare.LocationContainer2.X) And (boxCompare.LocationContainer2.X <= FEmptySpace(i).Position2.X)) AndAlso _
                ((FEmptySpace(i).Position.Y <= boxCompare.LocationContainer2.Y) And (boxCompare.LocationContainer2.Y <= FEmptySpace(i).Position2.Y)) Then
                cek(2) = True
            End If
            If (FOrigin.Z = boxCompare.LocationContainer.Z) AndAlso _
                ((FEmptySpace(i).Position.X <= boxCompare.LocationContainer.X) And (boxCompare.LocationContainer.X <= FEmptySpace(i).Position2.X)) AndAlso _
                ((FEmptySpace(i).Position.Y <= boxCompare.LocationContainer2.Y) And (boxCompare.LocationContainer2.Y <= FEmptySpace(i).Position2.Y)) Then
                cek(3) = True
            End If
            If (FOrigin.Z = boxCompare.LocationContainer.Z) AndAlso _
                ((FEmptySpace(i).Position.X <= boxCompare.LocationContainer2.X) And (boxCompare.LocationContainer2.X <= FEmptySpace(i).Position2.X)) AndAlso _
                ((FEmptySpace(i).Position.Y <= boxCompare.LocationContainer.Y) And (boxCompare.LocationContainer.Y <= FEmptySpace(i).Position2.Y)) Then
                cek(4) = True
            End If
        Next

        'return value
        If (cek(1) = True) And (cek(2) = True) And (cek(3) = True) And (cek(4) = True) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Get maximal space from contour
    ''' </summary>
    Private Sub GetMaximalSpace()
        'Try
        '1. contour separation --> lineWidth + lineDepth
        Dim lineWidth(Nothing), lineDepth(Nothing) As Line3D
        GetLineWidthDepth(FContour, lineWidth, lineDepth)

        '2. generate fisibelPoint (closed-contour point or opened-contour point)
        Dim contourPoint(Nothing) As Point3D
        Dim fisibelPoint(Nothing) As Boolean
        GetFeasiblePoint(contourPoint, fisibelPoint)

        '3. getting parrarelpiped Width --> dealing with lineWidth + intersection to lineDepth
        Dim pararelWidth(Nothing), pararelDepth(Nothing) As Line3D
        GetParrarelpiped(contourPoint, fisibelPoint, lineWidth, lineDepth, pararelWidth, pararelDepth)

        '4. get maximal space
        GenerateMaximalSpace(contourPoint, fisibelPoint, lineWidth, lineDepth, pararelWidth, pararelDepth, FEmptySpace)
        'Catch ex As Exception
        '    MyForm.formMainMenu.txtConsole.Text = "error, di maximal space..."
        'End Try

    End Sub


    ''' <summary>
    ''' Sort kontur to get 
    ''' </summary>
    Private Sub SortKontur(ByRef count As Integer)
        'find minimum point
        Dim i, j As Integer

        'pointer minimum point
        For i = 1 To FContour.GetUpperBound(0)
            If (FContour(i).IsDepthLine = True) AndAlso _
            (Min(FContour(i).FPoint1.X, FContour(i).FPoint2.X) = FOrigin.X) And (Min(FContour(i).FPoint1.Y, FContour(i).FPoint2.Y) = FOrigin.Y) Then
                j = i
            End If
        Next

        'swap pointer --> array1 : minimum point
        Swap(FContour(1), FContour(j))
        FContour(1).FDirection = True

        'reconfigurate kontur --> initial: lineDepth
        'sorting value
        For i = 2 To FContour.GetUpperBound(0)
            For j = i To FContour.GetUpperBound(0)
                'kalo line yang sekarang width, cari depth sekarang.
                If (FContour(i - 1).FDirection = True) AndAlso _
                    ((FContour(i - 1).FPoint2.IsEqualTo(FContour(j).FPoint1) = True) Or _
                    (FContour(i - 1).FPoint2.IsEqualTo(FContour(j).FPoint2) = True)) Then
                    Swap(FContour(i), FContour(j))
                    If FContour(i - 1).FPoint2.IsEqualTo(FContour(i).FPoint1) = True Then
                        FContour(i).FDirection = True
                    Else
                        FContour(i).FDirection = False
                    End If
                    Exit For
                End If

                If (FContour(i - 1).FDirection = False) AndAlso _
                    ((FContour(i - 1).FPoint1.IsEqualTo(FContour(j).FPoint1) = True) Or _
                    (FContour(i - 1).FPoint1.IsEqualTo(FContour(j).FPoint2) = True)) Then
                    Swap(FContour(i), FContour(j))
                    If FContour(i - 1).FPoint1.IsEqualTo(FContour(i).FPoint1) = True Then
                        FContour(i).FDirection = True
                    Else
                        FContour(i).FDirection = False
                    End If
                    Exit For
                End If
            Next
        Next

        'count used-contour
        i = 2
        Do Until ((FContour(i).FDirection = True) And (FContour(1).FPoint1.IsEqualTo(FContour(i).FPoint2) = True)) Or _
                 ((FContour(i).FDirection = False) And (FContour(1).FPoint1.IsEqualTo(FContour(i).FPoint1) = True))
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
    Private Function GetOriginPoint(ByVal Above As Boolean) As Point3D
        Dim minPoint = New Point3D(FContour(1).FPoint1)

        'getting minimum point
        For i As Integer = 1 To FContour.GetUpperBound(0)
            If (FContour(i).IsWidthLine = True) AndAlso (Min(FContour(i).FPoint1.Y, FContour(i).FPoint2.Y) < minPoint.Y) Then
                minPoint.Y = Min(FContour(i).FPoint1.Y, FContour(i).FPoint2.Y)
            End If
        Next
        For i = 1 To FContour.GetUpperBound(0)
            If (FContour(i).IsDepthLine = True) AndAlso (FContour(i).FPoint1.Y = minPoint.Y) Then
                minPoint.X = Min(FContour(i).FPoint1.X, FContour(i).FPoint2.X)
            End If
        Next
        If Above = True Then
            minPoint.Z = FContour(1).FPoint2.Z
        Else
            minPoint.Z = FContour(1).FPoint1.Z
        End If

        'result
        Return minPoint
    End Function

    ''' <summary>
    ''' Concept --&gt; get box from same height in same cuboid then build contour...
    ''' </summary>
    Public Sub SetNewBox(ByVal addBox() As Box, ByVal startPoint As Point3D, ByRef restContour() As Line3D)
        'variable
        Dim tempBox() As Box

        '1. separated box from different height
        tempBox = GetSeparatedBox(addBox, startPoint, False)

        '2. rebuild contour
        Try
            RebuildContour(FContour, tempBox)
        Catch ex As Exception
            MyForm.formMainMenu.txtConsole.Text = "error di rebuild contour"
            Stop
        End Try


        '4. origin + sort data
        FOrigin = GetOriginPoint(False)
        Dim count As Integer
        SortKontur(count)

        '5. normalize
        NormalizeLine(FContour)

        Console.WriteLine("===")
        Console.WriteLine("box packed")
        For i = 1 To tempBox.GetUpperBound(0)
            Console.WriteLine(tempBox(i).LocationContainer.X & "," & tempBox(i).LocationContainer.Y & "," & addBox(i).LocationContainer.Z & " + " & tempBox(i).LocationContainer2.X & "," & tempBox(i).LocationContainer2.Y & "," & tempBox(i).LocationContainer2.Z)
        Next
        Console.WriteLine("---")
        Console.WriteLine("contour result (origin = " & FOrigin.X & "," & FOrigin.Y & "," & FOrigin.Z & ")")
        For i = 1 To FContour.GetUpperBound(0)
            If FContour(i).FDirection = True Then
                Console.WriteLine(FContour(i).FPoint1.X & "," & FContour(i).FPoint1.Y & "," & FContour(i).FPoint1.Z & " --> " & FContour(i).FPoint2.X & "," & FContour(i).FPoint2.Y & "," & FContour(i).FPoint2.Z)
            Else
                Console.WriteLine(FContour(i).FPoint2.X & "," & FContour(i).FPoint2.Y & "," & FContour(i).FPoint2.Z & " --> " & FContour(i).FPoint1.X & "," & FContour(i).FPoint1.Y & "," & FContour(i).FPoint1.Z)
            End If
        Next
        Console.WriteLine("===")

        '6. build another contour if it result more than 1 contour
        If count < FContour.GetUpperBound(0) Then
            'resize restcontour
            ReDim restContour(FContour.GetUpperBound(0) - count)
            Dim i, j As Integer
            j = 0
            For i = count + 1 To FContour.GetUpperBound(0)
                j += 1
                restContour(j) = New Line3D(FContour(i))
            Next

            'resize (to fix) currentcontour
            ReDim Preserve FContour(count)
        End If

        '7. getmaximal space
        Try
            If count > 0 Then GetMaximalSpace()
        Catch ex As Exception
            MyForm.formMainMenu.txtConsole.Text = "get maximal space"
            Stop
        End Try
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
        defaultValue = Max3(CSng(MyForm.formMainMenu.txtDConDepth.Text), CSng(MyForm.formMainMenu.txtDConHeight.Text), CSng(MyForm.formMainMenu.txtDConWidth.Text))

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
                If lineWidth(i).FPoint1.IsEqualTo(contourPoint(j)) = True Then cekMin = fisibelPoint(j)
                If lineWidth(i).FPoint2.IsEqualTo(contourPoint(j)) = True Then cekMax = fisibelPoint(j)
            Next

            'geting min and max point
            For j = 1 To lineDepth.GetUpperBound(0)
                '--find max point from point2 + record distance
                If (cekMax = True) AndAlso _
                   (lineDepth(j).FPoint1.Y > lineWidth(i).FPoint2.Y) And _
                   ((lineDepth(j).FPoint1.Y - lineWidth(i).FPoint2.Y) < maxDistance) And _
                   ((lineDepth(j).FPoint1.X <= lineWidth(i).FPoint2.X) And (lineWidth(i).FPoint2.X <= lineDepth(j).FPoint2.X)) Then
                    maxDistance = lineDepth(j).FPoint1.Y - lineWidth(i).FPoint2.Y
                    maxPointer = j
                End If

                '--find min point from point1 + record distance
                If (cekMin = True) AndAlso _
                   (lineDepth(j).FPoint1.Y < lineWidth(i).FPoint1.Y) And _
                   ((lineWidth(i).FPoint1.Y - lineDepth(j).FPoint1.Y) < minDistance) And _
                   ((lineDepth(j).FPoint1.X <= lineWidth(i).FPoint1.X) And (lineWidth(i).FPoint1.X <= lineDepth(j).FPoint2.X)) Then
                    minDistance = lineWidth(i).FPoint1.Y - lineDepth(j).FPoint1.Y
                    minPointer = j
                End If

                '--get maxDefault
                If (lineDepth(j).FPoint1.Y = lineWidth(i).FPoint2.Y) AndAlso _
                    ((lineDepth(j).FPoint1.X <= lineWidth(i).FPoint2.X) And (lineWidth(i).FPoint2.X <= lineDepth(j).FPoint2.X)) Then
                    defaultMaxPointer = j
                End If

                '--get minDefault
                If (lineDepth(j).FPoint1.Y = lineWidth(i).FPoint1.Y) AndAlso _
                    ((lineDepth(j).FPoint1.X <= lineWidth(i).FPoint1.X) And (lineWidth(i).FPoint1.X <= lineDepth(j).FPoint2.X)) Then
                    defaultMinPointer = j
                End If
            Next

            If cekMax = False Then maxPointer = defaultMinPointer
            If cekMin = False Then minPointer = defaultMaxPointer

            'get pararelpiped
            If (lineWidth(i).IsEqualTo(New Line3D(New Point3D(lineWidth(i).FPoint1.X, lineDepth(minPointer).FPoint1.Y, lineWidth(i).FPoint1.Z), _
                                                  New Point3D(lineWidth(i).FPoint2.X, lineDepth(maxPointer).FPoint1.Y, lineWidth(i).FPoint2.Z))) = False) Then
                k += 1
                pararelpipedWidth(k) = New Line3D(New Point3D(lineWidth(i).FPoint1.X, lineDepth(minPointer).FPoint1.Y, lineWidth(i).FPoint1.Z), _
                                                  New Point3D(lineWidth(i).FPoint2.X, lineDepth(maxPointer).FPoint1.Y, lineWidth(i).FPoint2.Z))
            End If
            If cekMin = True Then
                k += 1
                pararelpipedWidth(k) = New Line3D(New Point3D(lineWidth(i).FPoint1.X, lineDepth(minPointer).FPoint1.Y, lineWidth(i).FPoint1.Z), _
                                                  New Point3D(lineWidth(i).FPoint2.X, lineDepth(defaultMinPointer).FPoint1.Y, lineWidth(i).FPoint2.Z))
            End If
            If cekMax = True Then
                k += 1
                pararelpipedWidth(k) = New Line3D(New Point3D(lineWidth(i).FPoint1.X, lineDepth(defaultMaxPointer).FPoint1.Y, lineWidth(i).FPoint1.Z), _
                                                  New Point3D(lineWidth(i).FPoint2.X, lineDepth(maxPointer).FPoint1.Y, lineWidth(i).FPoint2.Z))
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
                If lineDepth(i).FPoint1.IsEqualTo(contourPoint(j)) = True Then cekMin = fisibelPoint(j)
                If lineDepth(i).FPoint2.IsEqualTo(contourPoint(j)) = True Then cekMax = fisibelPoint(j)
            Next

            'geting min and max point
            For j = 1 To lineWidth.GetUpperBound(0)
                '--find max point from point2 + record distance
                If (cekMax = True) AndAlso _
                   (lineWidth(j).FPoint1.X > lineDepth(i).FPoint2.X) And _
                   ((lineWidth(j).FPoint1.X - lineDepth(i).FPoint2.X) < maxDistance) And _
                   ((lineWidth(j).FPoint1.Y <= lineDepth(i).FPoint2.Y) And (lineDepth(i).FPoint2.Y <= lineWidth(j).FPoint2.Y)) Then
                    maxDistance = lineWidth(j).FPoint1.X - lineDepth(i).FPoint2.X
                    maxPointer = j
                End If

                '--find min point from point1 + record distance
                If (cekMin = True) AndAlso _
                   (lineWidth(j).FPoint1.X < lineDepth(i).FPoint1.X) And _
                   ((lineDepth(i).FPoint1.X - lineWidth(j).FPoint1.X) < minDistance) And _
                   ((lineWidth(j).FPoint1.Y <= lineDepth(i).FPoint1.Y) And (lineDepth(i).FPoint1.Y <= lineWidth(j).FPoint2.Y)) Then
                    minDistance = lineDepth(i).FPoint1.X - lineWidth(j).FPoint1.X
                    minPointer = j
                End If

                '--get max default
                If (lineWidth(j).FPoint1.X = lineDepth(i).FPoint2.X) AndAlso _
                    ((lineWidth(j).FPoint1.Y <= lineDepth(i).FPoint2.Y) And (lineDepth(i).FPoint2.Y <= lineWidth(j).FPoint2.Y)) Then
                    defaultMaxPointer = j
                End If

                '--get min default
                If (lineWidth(j).FPoint1.X = lineDepth(i).FPoint1.X) AndAlso _
                   ((lineWidth(j).FPoint1.Y <= lineDepth(i).FPoint1.Y) And (lineDepth(i).FPoint1.Y <= lineWidth(j).FPoint2.Y)) Then
                    defaultMinPointer = j
                End If
            Next

            If cekMax = False Then maxPointer = defaultMinPointer
            If cekMin = False Then minPointer = defaultMaxPointer

            'get pararelpiped
            If (lineDepth(i).IsEqualTo(New Line3D(New Point3D(lineWidth(minPointer).FPoint1.X, lineDepth(i).FPoint1.Y, lineDepth(i).FPoint1.Z), _
                                                  New Point3D(lineWidth(maxPointer).FPoint1.X, lineDepth(i).FPoint2.Y, lineDepth(i).FPoint2.Z))) = False) Then
                k += 1
                pararelpipedDepth(k) = New Line3D(New Point3D(lineWidth(minPointer).FPoint1.X, lineDepth(i).FPoint1.Y, lineDepth(i).FPoint1.Z), _
                                                  New Point3D(lineWidth(maxPointer).FPoint1.X, lineDepth(i).FPoint2.Y, lineDepth(i).FPoint2.Z))
            End If
            If cekMin = True Then
                k += 1
                pararelpipedDepth(k) = New Line3D(New Point3D(lineWidth(minPointer).FPoint1.X, lineDepth(i).FPoint1.Y, lineDepth(i).FPoint1.Z), _
                                                  New Point3D(lineWidth(defaultMinPointer).FPoint1.X, lineDepth(i).FPoint2.Y, lineDepth(i).FPoint2.Z))
            End If
            If cekMax = True Then
                k += 1
                pararelpipedDepth(k) = New Line3D(New Point3D(lineWidth(defaultMaxPointer).FPoint1.X, lineDepth(i).FPoint1.Y, lineDepth(i).FPoint1.Z), _
                                                  New Point3D(lineWidth(maxPointer).FPoint1.X, lineDepth(i).FPoint2.Y, lineDepth(i).FPoint2.Z))
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
    ''' Get contour from box, start from initial point
    ''' </summary>
    Private Sub GetContour(ByVal contourBox() As Box, ByVal startPoint As Point3D)
        Dim i, j As Integer
        '===
        'preparing stage
        '===

        'generate all line
        Dim lineContour(contourBox.GetUpperBound(0) * 4) As Line3D
        For i = 1 To contourBox.GetUpperBound(0)
            With contourBox(i)
                lineContour(((i - 1) * 4) + 1) = New Line3D(.LocationContainer.X, .LocationContainer.Y, .LocationContainer2.Z, _
                                                            .LocationContainer2.X, .LocationContainer.Y, .LocationContainer2.Z)
                lineContour(((i - 1) * 4) + 2) = New Line3D(.LocationContainer2.X, .LocationContainer.Y, .LocationContainer2.Z, _
                                                            .LocationContainer2.X, .LocationContainer2.Y, .LocationContainer2.Z)
                lineContour(((i - 1) * 4) + 3) = New Line3D(.LocationContainer.X, .LocationContainer2.Y, .LocationContainer2.Z, _
                                                            .LocationContainer2.X, .LocationContainer2.Y, .LocationContainer2.Z)
                lineContour(((i - 1) * 4) + 4) = New Line3D(.LocationContainer.X, .LocationContainer.Y, .LocationContainer2.Z, _
                                                            .LocationContainer.X, .LocationContainer2.Y, .LocationContainer2.Z)
            End With
        Next

        'normalize line first
        NormalizeLine(lineContour)

        'substract line
        Dim tempLine() As Line3D
        For i = 1 To (lineContour.GetUpperBound(0) - 1)
            For j = (i + 1) To lineContour.GetUpperBound(0)
                If (lineContour(j).IsIntersectionWith(lineContour(i)) = True) AndAlso _
                   (lineContour(j).GetIntersectionOnPlanarWith(lineContour(i)).Length > 0) Then
                    tempLine = lineContour(i).SubstractSpecial(lineContour(j))
                    If tempLine.GetUpperBound(0) = 1 Then
                        lineContour(i) = New Line3D(tempLine(1))
                    Else
                        'if intersection occur in middle, result 2 new line
                        ReDim Preserve lineContour(lineContour.GetUpperBound(0) + 1)
                        lineContour(i) = New Line3D(tempLine(1))
                        lineContour(lineContour.GetUpperBound(0)) = New Line3D(tempLine(2))
                    End If
                End If
            Next
        Next

        '#normalize line final
        NormalizeLine(lineContour)


        '===
        'get new contour
        '===
        'separated lineWidth and lineDepth
        Dim lineWidth(Nothing), lineDepth(Nothing) As Line3D
        GetLineWidthDepth(lineContour, lineWidth, lineDepth)

        'getting contour
        Dim IsLineDepth, stopIteration As Boolean
        Dim currentPoint, point1, point2 As Point3D         '--> as initial
        ReDim FContour(contourBox.GetUpperBound(0) * 4)

        'reset data
        point2 = New Point3D(-1, -1, -1)
        j = 0
        IsLineDepth = True
        currentPoint = startPoint

        'start iteration
        Do Until point2.IsEqualTo(startPoint)
            'reset data
            stopIteration = False

            'beginning operation
            point1 = New Point3D(currentPoint)

            'find maxPoint --> same Line
            Do Until stopIteration = True
                If IsLineDepth = True Then
                    For i = 1 To lineDepth.GetUpperBound(0)
                        If (lineDepth(i).FPoint1.IsEqualTo(currentPoint) = True) Or _
                            (lineDepth(i).FPoint2.IsEqualTo(currentPoint) = True) Then
                            If lineDepth(i).FPoint1.IsEqualTo(currentPoint) = True Then
                                currentPoint = lineDepth(i).FPoint2
                            Else
                                currentPoint = lineDepth(i).FPoint1
                            End If
                            stopIteration = True
                            Exit For
                        Else
                            stopIteration = False
                        End If
                    Next
                Else
                    For i = 1 To lineWidth.GetUpperBound(0)
                        If (lineWidth(i).FPoint1.IsEqualTo(currentPoint) = True) Or _
                            (lineWidth(i).FPoint2.IsEqualTo(currentPoint) = True) Then
                            If lineWidth(i).FPoint1.IsEqualTo(currentPoint) = True Then
                                currentPoint = lineWidth(i).FPoint2
                            Else
                                currentPoint = lineWidth(i).FPoint1
                            End If
                            stopIteration = True
                            Exit For
                        Else
                            stopIteration = False
                        End If
                    Next
                End If
            Loop

            'ending operation
            point2 = New Point3D(currentPoint)

            'get line
            j += 1
            FContour(j) = New Line3D(New Point3D(point1), New Point3D(point2))
            IsLineDepth = Not IsLineDepth
        Loop

        'resize array contour
        ReDim Preserve FContour(j)

        'normalize
        NormalizeLine(FContour)
        '===
    End Sub

    ''' <summary>
    ''' Separation box from target box and not
    ''' </summary>
    Private Function GetSeparatedBox(ByVal addBox() As Box, ByVal startPoint As Point3D, ByVal IsNewContour As Boolean) As Box()
        'variable
        Dim i, j As Integer
        Dim tempBox(addBox.GetUpperBound(0)) As Box

        'separated box from different height
        j = 0
        For i = 1 To addBox.GetUpperBound(0)
            If (IsNewContour = True) AndAlso _
                (addBox(i).LocationContainer2.Z = startPoint.Z) Then
                j += 1
                tempBox(j) = New Box(addBox(i))
            End If
            If (IsNewContour = False) AndAlso _
                (addBox(i).LocationContainer.Z = startPoint.Z) Then
                j += 1
                tempBox(j) = New Box(addBox(i))
            End If

        Next
        'resize array
        ReDim Preserve tempBox(j)

        'return value
        Return tempBox
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
    Private Sub GenerateMaximalSpace(ByVal contourPoint() As Point3D, ByVal fisibelPoint() As Boolean, ByVal lineWidth() As Line3D, ByVal lineDepth() As Line3D, ByVal pararelWidth() As Line3D, ByVal pararelDepth() As Line3D, ByRef empSpace() As Kotak)
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
                pararelPoint1 = New Point3D(pararelWidth(i).FPoint1)
                pararelPoint2 = New Point3D(pararelWidth(i).FPoint2)

                dLine1Min = Max3(CSng(MyForm.formMainMenu.txtDConDepth.Text), _
                                        CSng(MyForm.formMainMenu.txtDConWidth.Text), _
                                        CSng(MyForm.formMainMenu.txtDConHeight.Text))
                dLine1Max = dLine1Min
                dLine2Min = dLine1Min
                dLine2Max = dLine1Min

                'get complementer line (top-bottom)
                For j = 1 To lineWidth.GetUpperBound(0)
                    With lineWidth(j)
                        'get complementerBottomLine
                        If (.FPoint1.Y <= pararelPoint1.Y) And (pararelPoint1.Y < .FPoint2.Y) Then
                            'get left (to minimum)
                            If (.FPoint1.X < pararelPoint1.X) And ((pararelPoint1.X - .FPoint1.X) < dLine1Min) Then
                                dLine1Min = pararelPoint1.X - .FPoint1.X
                                pLine1Min = New Point3D(.FPoint1.X, pararelPoint1.Y, pararelPoint1.Z)
                            End If
                            'get right (to maximum)
                            If (.FPoint1.X > pararelPoint1.X) And ((.FPoint1.X - pararelPoint1.X) < dLine1Max) Then
                                dLine1Max = .FPoint1.X - pararelPoint1.X
                                pLine1Max = New Point3D(.FPoint1.X, pararelPoint1.Y, pararelPoint1.Z)
                            End If
                        End If

                        'get complementerTopLine
                        If (.FPoint1.Y < pararelPoint2.Y) And (pararelPoint2.Y <= .FPoint2.Y) Then
                            'get left (to minimum)
                            If (.FPoint1.X < pararelPoint2.X) And ((pararelPoint2.X - .FPoint1.X) < dLine2Min) Then
                                dLine2Min = pararelPoint2.X - .FPoint1.X
                                pLine2Min = New Point3D(.FPoint1.X, pararelPoint2.Y, pararelPoint2.Z)
                            End If
                            'get right (to maximum)
                            If (.FPoint1.X > pararelPoint2.X) And ((.FPoint1.X - pararelPoint2.X) < dLine2Max) Then
                                dLine2Max = .FPoint1.X - pararelPoint2.X
                                pLine2Max = New Point3D(.FPoint1.X, pararelPoint2.Y, pararelPoint1.Z)
                            End If
                        End If
                    End With
                Next

                'establish complementer line
                compLine1 = New Line3D(pLine1Min, pLine1Max)
                compLine2 = New Line3D(pLine2Min, pLine2Max)
                complementLine = compLine1.GetIntersectionOnPlanarWith(compLine2)

                'get maximal space
                emptyspaceWidth(i) = New Kotak(pararelWidth(i).Length, complementLine.Length, FOrigin.Z)
                emptyspaceWidth(i).Position = New Point3D(complementLine.FPoint1.X, pararelWidth(i).FPoint1.Y, pararelWidth(i).FPoint2.Z)
            Next

            'pararelpiped Depth line --> complementer = widthLine
            '===
            'iteration for each p.depth line
            For i = 1 To pararelDepth.GetUpperBound(0)
                'get pararel point
                pararelPoint1 = New Point3D(pararelDepth(i).FPoint1)
                pararelPoint2 = New Point3D(pararelDepth(i).FPoint2)

                dLine1Min = Max3(CSng(MyForm.formMainMenu.txtDConDepth.Text), _
                                        CSng(MyForm.formMainMenu.txtDConWidth.Text), _
                                        CSng(MyForm.formMainMenu.txtDConHeight.Text))
                dLine1Max = dLine1Min
                dLine2Min = dLine1Min
                dLine2Max = dLine1Min

                'get complementer line (left-right)
                For j = 1 To lineDepth.GetUpperBound(0)
                    With lineDepth(j)
                        'get complementerLeftLine
                        If (.FPoint1.X <= pararelPoint1.X) And (pararelPoint1.X < .FPoint2.X) Then
                            'get bottom (to minimum)
                            If (.FPoint1.Y < pararelPoint1.Y) And ((pararelPoint1.Y - .FPoint1.Y) < dLine1Min) Then
                                dLine1Min = pararelPoint1.Y - .FPoint1.Y
                                pLine1Min = New Point3D(pararelPoint1.X, .FPoint1.Y, pararelPoint1.Z)
                            End If
                            'get top (to maximum)
                            If (.FPoint1.Y > pararelPoint1.Y) And ((.FPoint1.Y - pararelPoint1.Y) < dLine1Max) Then
                                dLine1Max = .FPoint1.Y - pararelPoint1.Y
                                pLine1Max = New Point3D(pararelPoint1.X, .FPoint1.Y, pararelPoint1.Z)
                            End If
                        End If

                        'get complementerRightLine
                        If (.FPoint1.X < pararelPoint2.X) And (pararelPoint2.X <= .FPoint2.X) Then
                            'get bottom (to minimum)
                            If (.FPoint1.Y < pararelPoint2.Y) And ((pararelPoint2.Y - .FPoint1.Y) < dLine2Min) Then
                                dLine2Min = pararelPoint2.Y - .FPoint1.Y
                                pLine2Min = New Point3D(pararelPoint2.X, .FPoint1.Y, pararelPoint2.Z)
                            End If
                            'get top (to maximum)
                            If (.FPoint1.Y > pararelPoint2.Y) And ((.FPoint1.Y - pararelPoint2.Y) < dLine2Max) Then
                                dLine2Max = .FPoint1.Y - pararelPoint2.Y
                                pLine2Max = New Point3D(pararelPoint2.X, .FPoint1.Y, pararelPoint1.Z)
                            End If
                        End If
                    End With
                Next

                'establish complementer line
                compLine1 = New Line3D(pLine1Min, pLine1Max)
                compLine2 = New Line3D(pLine2Min, pLine2Max)
                complementLine = compLine1.GetIntersectionOnPlanarWith(compLine2)

                'get maximal space
                emptyspaceDepth(i) = New Kotak(complementLine.Length, pararelDepth(i).Length, FOrigin.Z)
                emptyspaceDepth(i).Position = New Point3D(pararelDepth(i).FPoint1.X, complementLine.FPoint1.Y, pararelDepth(i).FPoint2.Z)
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
            empSpace(1) = New Kotak(lineWidth(1).Length, lineDepth(1).Length, FOrigin.Z)
            empSpace(1).Position = New Point3D(lineDepth(1).FPoint1.X, lineWidth(1).FPoint1.Y, lineDepth(1).FPoint2.Z)
        End If

    End Sub

    ''' <summary>
    ''' Normalize maximal space --&gt; eliminated if inbound maximal space
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
                            (lineContour(j).FPoint1.X < lineWidth(k).FPoint1.X) And (lineWidth(k).FPoint1.X < lineContour(j).FPoint2.X) And _
                            (lineWidth(k).FPoint1.Y < lineContour(j).FPoint1.Y) And (lineContour(j).FPoint1.Y < lineWidth(k).FPoint2.Y) Then
                            notFisibel(i) = True
                            Exit For
                        End If
                    Next

                    '-compare to linedepth --> intersection in perpendicular --> emptyspace default (not fisibel) = true
                    For k = 1 To lineDepth.GetUpperBound(0)
                        If ((lineContour(j).IsWidthLine = True) And (notFisibel(i) = False)) AndAlso _
                            (lineDepth(k).FPoint1.X < lineContour(j).FPoint1.X) And (lineContour(j).FPoint1.X < lineDepth(k).FPoint2.X) And _
                            (lineContour(j).FPoint1.Y < lineDepth(k).FPoint1.Y) And (lineDepth(k).FPoint1.Y < lineContour(j).FPoint2.Y) Then
                            notFisibel(i) = True
                            Exit For
                        End If
                    Next

                    '-compare linewidth + linedepth through closed-point
                    For k = 1 To contourPoint.GetUpperBound(0)
                        If ((lineContour(j).IsDepthLine = True) And (notFisibel(i) = False) And (fisibelPoint(k) = False)) AndAlso _
                            (lineContour(j).FPoint1.X < contourPoint(k).X) And (contourPoint(k).X < lineContour(j).FPoint2.X) And _
                            (lineContour(j).FPoint1.Y = contourPoint(k).Y) Then
                            notFisibel(i) = True
                            Exit For
                        End If

                        If ((lineContour(j).IsWidthLine = True) And (notFisibel(i) = False) And (fisibelPoint(k) = False)) AndAlso _
                            (lineContour(j).FPoint1.X = contourPoint(k).X) And _
                            (lineContour(j).FPoint1.Y < contourPoint(k).Y) And (contourPoint(k).Y < lineContour(j).FPoint2.Y) Then
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
                    (CheckingAreaInBound2D(empSpace(i), empSpace(j)) = True) Then
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
    ''' Rebuilding contour due to add-subtract
    ''' </summary>
    Private Sub RebuildContour(ByRef oldContour() As Line3D, ByVal addBox() As Box)
        Dim lineContour(4), tempLine() As Line3D
        Dim cek(4) As Boolean
        Dim i, j, k, count, fixcount, rest As Integer

        For j = 1 To addBox.GetUpperBound(0)
            '#resize contour
            count = oldContour.GetUpperBound(0)

            'generate all line --> for 1 box
            With addBox(j)
                lineContour(1) = New Line3D(.LocationContainer.X, .LocationContainer.Y, .LocationContainer.Z, _
                                            .LocationContainer2.X, .LocationContainer.Y, .LocationContainer.Z)
                lineContour(2) = New Line3D(.LocationContainer.X, .LocationContainer2.Y, .LocationContainer.Z, _
                                            .LocationContainer2.X, .LocationContainer2.Y, .LocationContainer.Z)
                lineContour(3) = New Line3D(.LocationContainer.X, .LocationContainer.Y, .LocationContainer.Z, _
                                            .LocationContainer.X, .LocationContainer2.Y, .LocationContainer.Z)
                lineContour(4) = New Line3D(.LocationContainer2.X, .LocationContainer.Y, .LocationContainer.Z, _
                                            .LocationContainer2.X, .LocationContainer2.Y, .LocationContainer.Z)
            End With

            'reset data
            For k = 1 To 4
                cek(k) = False
            Next
            rest = 4
            fixcount = count

            'substract line
            For i = 1 To fixcount
                For k = 1 To 4
                    If ((lineContour(k).IsIntersectionWith(oldContour(i)) = True) And (cek(k) = False)) AndAlso _
                        (lineContour(k).GetIntersectionOnPlanarWith(oldContour(i)).Length > 0) Then
                        tempLine = oldContour(i).SubstractSpecial(lineContour(k))
                        If tempLine.GetUpperBound(0) = 1 Then
                            oldContour(i) = New Line3D(tempLine(1))
                        Else
                            'if intersection occur in middle, result 2 new line
                            ReDim Preserve oldContour(oldContour.GetUpperBound(0) + 1)
                            oldContour(i) = New Line3D(tempLine(1))
                            count += 1
                            oldContour(oldContour.GetUpperBound(0)) = New Line3D(tempLine(2))
                        End If

                        cek(k) = True
                        rest -= 1
                    End If
                Next

                If count > fixcount Then
                    For k = 1 To count
                        If ((i <> k) And (oldContour(k).IsIntersectionWith(oldContour(i)) = True)) AndAlso _
                            (oldContour(k).GetIntersectionOnPlanarWith(oldContour(i)).Length > 0) Then
                            tempLine = oldContour(i).SubstractSpecial(oldContour(k))
                            If tempLine.GetUpperBound(0) = 1 Then
                                oldContour(i) = New Line3D(tempLine(1))
                                oldContour(k) = New Line3D(0, 0, 0, 0, 0, 0)
                            Else
                                'if intersection occur in middle, result 2 new line
                                oldContour(i) = New Line3D(tempLine(1))
                                oldContour(k) = New Line3D(tempLine(2))
                            End If
                        End If
                    Next
                End If
            Next

            'add new line to contour
            ReDim Preserve oldContour(count + rest)
            For k = 1 To 4
                If cek(k) = False Then
                    count += 1
                    oldContour(count) = New Line3D(lineContour(k))
                End If
            Next

            '#normalize data
            NormalizeLine(oldContour)
        Next
    End Sub

    ''' <summary>
    ''' Normalize line --&gt; eliminated unused line
    ''' </summary>
    ''' <remarks>
    ''' What can be normalize:
    ''' 1. Adding line (same orientation, neighborhood line) --&gt; suppose to be add into 1 line
    ''' 2. Identical line
    ''' 3. Intersection line
    ''' 4. Zero length-line
    ''' </remarks>
    Private Sub NormalizeLine(ByRef lineContour() As Line3D)
        Dim i, j As Integer
        Dim notFisibel(lineContour.GetUpperBound(0)) As Boolean

        '- no length = 0
        '- adding same line
        For i = 1 To lineContour.GetUpperBound(0) - 1
            If lineContour(i).Length = 0 Then
                notFisibel(i) = True
            Else
                For j = i + 1 To lineContour.GetUpperBound(0)
                    If lineContour(j).Length = 0 Then
                        notFisibel(j) = True
                    End If
                    If ((i <> j) And (notFisibel(i) = False) And (notFisibel(j) = False)) AndAlso _
                        (lineContour(i).IsEqualTo(lineContour(j)) = True) Then
                        notFisibel(j) = True
                    End If
                    If ((i <> j) And (notFisibel(i) = False) And (notFisibel(j) = False)) AndAlso _
                        (lineContour(i).Add(lineContour(j)).Length > 0) Then
                        lineContour(i) = lineContour(i).Add(lineContour(j))
                        notFisibel(j) = True
                    End If
                Next
            End If
        Next

        'update contour
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
    End Sub

    ''' <summary>
    ''' Get closed-contour-point and opened-contour-point
    ''' </summary>
    Private Sub GetFeasiblePoint(ByRef contourPoint() As Point3D, ByRef fisibelPoint() As Boolean)
        'get point and possibility to packing
        ReDim fisibelPoint(FContour.GetUpperBound(0))
        ReDim contourPoint(FContour.GetUpperBound(0))

        'depthLine = true --> is depth line,
        'UP = depthline(false), istrue(true)
        'DOWN  = depthline(false), istrue(false)
        'RIGHT = depthline(true), istrue(true)
        'LEFT = depthline(true), istrue(false)
        Dim IsDepthLine1, IsDepthLine2, IsDirection1, IsDirection2 As Boolean
        For i = 1 To FContour.GetUpperBound(0)
            If FContour(i).FDirection = True Then
                contourPoint(i) = New Point3D(FContour(i).FPoint1)
            Else
                contourPoint(i) = New Point3D(FContour(i).FPoint2)
            End If
            fisibelPoint(i) = False

            'set define UP, DOWN, RIGHT, LEFT --> depthLine+direction
            If i = 1 Then
                IsDepthLine1 = FContour(FContour.GetUpperBound(0)).IsDepthLine
                IsDirection1 = FContour(FContour.GetUpperBound(0)).FDirection
                IsDepthLine2 = FContour(1).IsDepthLine
                IsDirection2 = FContour(1).FDirection
            Else
                IsDepthLine1 = FContour(i - 1).IsDepthLine
                IsDirection1 = FContour(i - 1).FDirection
                IsDepthLine2 = FContour(i).IsDepthLine
                IsDirection2 = FContour(i).FDirection
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
End Class
