Imports System

''' <summary>
''' Construction of wall box
''' </summary>
Public Class Wall
    Inherits Placement

    'variable in class
    Private FFrequency1() As FrequencyOccurence
    Private FFrequency2() As FrequencyOccurence
    Private FFrequency3() As FrequencyOccurence
    Private FMDepthPrioritize() As FrequencyOccurence
    ''' <summary>
    ''' Length depth of wall
    ''' </summary>
    Private FLengthDepth As Single()
    ''' <summary>
    ''' Best placement of wall building
    ''' </summary>
    Private FBox() As Box

    ''' <summary>
    ''' Structure for ranking occurence
    ''' </summary>
    Private Structure FrequencyOccurence
        Dim Dimension As Single
        Dim Count As Integer
    End Structure

    ''' <summary>
    ''' Structure for ranking occurence
    ''' </summary>
    Private Structure AlphaRatio
        Dim Ratio As Single
        Dim Box As Box
    End Structure

    ''' <summary>
    ''' Default constructor data
    ''' </summary>
    Sub New(ByVal DEmpty As Box, ByVal InputBox() As Box)
        Dim i As Integer

        'input data --empty space, input box
        FEmptySpace = New Box(DEmpty)

        ReDim FInput(InputBox.GetUpperBound(0))
        For i = 1 To InputBox.GetUpperBound(0)
            FInput(i) = New Box(InputBox(i))
        Next

        'recapitulation box
        Recapitulation(FInput, FDataListInput)
    End Sub

    ''' <summary>
    ''' Get frequency of boxes
    ''' --get different with update
    ''' --basic idea --> connect to all dimension in box... catch them all
    ''' </summary>
    Private Sub GetFrequency(ByVal boxList() As Box, ByRef tempDim() As Single, ByVal dataList() As ListBox)
        'reset data first
        Dim i, j, k, count As Integer
        Dim cek As Boolean
        Dim temp(3) As String

        '#get all dimension
        ReDim tempDim(boxList.GetUpperBound(0) * 3)
        count = 0
        For i = 1 To boxList.GetUpperBound(0)
            temp(1) = boxList(i).Dim1
            temp(2) = boxList(i).Dim2
            temp(3) = boxList(i).Dim3

            'cek dimension temp(i) to database
            For j = 1 To 3
                If temp(j) > 0 Then
                    'reset data, if cek = false --> there's no dimension on there
                    cek = False
                    For k = 1 To tempDim.GetUpperBound(0)
                        If temp(j) = tempDim(k) Then
                            cek = True
                            Exit For
                        End If
                    Next
                    'update data
                    If (cek = False) Then
                        count += 1
                        tempDim(count) = temp(j)
                    End If
                End If
            Next
        Next

        'resize array dimension + sort
        ReDim Preserve tempDim(count)
        ReDim FFrequency1(count)
        ReDim FFrequency2(count)
        ReDim FFrequency3(count)

        'array sort (largest to smalesst)
        Array.Sort(tempDim)

        'copy dimension to frequency + finding occurence
        For i = 1 To tempDim.GetUpperBound(0)
            'reset and copy data
            FFrequency1(i).Count = 0
            FFrequency2(i).Count = 0
            FFrequency3(i).Count = 0

            FFrequency1(i).Dimension = tempDim(i)
            FFrequency2(i).Dimension = tempDim(i)
            FFrequency3(i).Dimension = tempDim(i)

            For j = 1 To boxList.GetUpperBound(0)
                temp(1) = boxList(j).Dim1
                temp(2) = boxList(j).Dim2
                temp(3) = boxList(j).Dim3
                Array.Sort(temp)

                'find function1 occurence (is there dimension)
                If (temp(1) = tempDim(i)) Or _
                    (temp(2) = tempDim(i)) Or _
                    (temp(3) = tempDim(i)) Then _
                    FFrequency1(i).Count += dataList(j).SCount

                'find function2 occurence (max dimension)
                If (temp(1) = tempDim(i)) Then _
                    FFrequency2(i).Count += dataList(j).SCount

                'find function3 occurence (min dimension)
                If (temp(3) <> 0) And (temp(3) = tempDim(i)) Then
                    FFrequency3(i).Count += dataList(j).SCount
                ElseIf (temp(3) = 0) And (temp(2) <> 0) And (temp(2) = tempDim(i)) Then
                    FFrequency3(i).Count += dataList(j).SCount
                ElseIf (temp(3) = 0) And (temp(3) = 0) And (temp(1) = tempDim(3)) Then
                    FFrequency3(i).Count += dataList(j).SCount
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' Optimize wall construct
    ''' </summary>fs
    ''' <remarks>
    ''' Wall construct is developed by Pisinger heuristic model.
    ''' ---
    ''' basic algorithm
    ''' 1. choose depth of wall --&gt; based on prioritize function
    '''     - provid number of depth that can be used

    ''' 2. filling layer by using strip-by-strip (horizontal / vertical)
    ''' 
    ''' to optimize,
    ''' it's better to calculate with M1 variety of depth.
    ''' </remarks>
    Public Sub GetOptimizeWall()
        'set default %dimension to get
        'default = 10%

        '==preparation
        'variable
        Dim i, j, k As Integer
        Dim alphaR(Nothing) As AlphaRatio
        Dim maxRatio, volBest, tempVol As Single
        Dim cek As Boolean
        Dim currentEmptySpace, tempPlacement(Nothing) As Box

        'update variety dimension
        Dim boxlist(FDataListInput.GetUpperBound(0)) As Box
        For i = 1 To FDataListInput.GetUpperBound(0)
            For j = 1 To FInput.GetUpperBound(0)
                If FInput(j).Type = FDataListInput(i).SType Then
                    boxlist(i) = New Box(FInput(j))
                    Exit For
                End If
            Next
        Next

        'get length of depth
        FLengthDepth = GetLengthDepth(boxList, 0.1)

        'begin iteration
        volBest = 0
        For i = 1 To FLengthDepth.GetUpperBound(0)
            'calculate alpha fill ratio
            ReDim alphaR(boxlist.GetUpperBound(0))
            alphaR = GetAlphaRatio(boxlist, FLengthDepth(i))

            'get largest alpha fill ratio
            maxRatio = alphaR(1).Ratio

            'iteration only for largest alpha ratio as references box
            For j = 1 To alphaR.GetUpperBound(0)
                If alphaR(j).Ratio = maxRatio Then
                    'reset emptyspace
                    currentEmptySpace = New Box(-1, FEmptySpace.Width, FEmptySpace.Height, FEmptySpace.Depth)
                    cek = False

                    'recursive for each alphaR.getupperbound --> until there's no empty space
                    GetLayer(currentEmptySpace, alphaR, FDataListInput, tempPlacement, tempVol)

                    'get best placement
                    If volBest < tempVol Then
                        volBest = tempVol
                        ReDim FBox(tempPlacement.GetUpperBound(0))
                        For k = 1 To FBox.GetUpperBound(0)
                            FBox(k) = New Box(tempPlacement(k))
                            FBox(k).LocationContainer = New Point3D(tempPlacement(k).LocationTemp.X + FEmptySpace.LocationContainer.X, _
                                                                    tempPlacement(k).LocationTemp.Y + FEmptySpace.LocationContainer.Y, _
                                                                    tempPlacement(k).LocationTemp.Z + FEmptySpace.LocationContainer.Z)
                        Next
                    End If
                End If
            Next
        Next

        'get best placement
        FUtilization = volBest / (FEmptySpace.Width * FEmptySpace.Depth * FEmptySpace.Height)
    End Sub

    ''' <summary>
    ''' Get prioritize of depth
    ''' </summary>
    Private Sub GetPrioritize(ByRef tempDim() As Single, ByVal percentM As Single)
        'get number dimension for each prioritize
        Dim nTargetDimension As Integer = CInt(tempDim.GetUpperBound(0) * percentM)
        If nTargetDimension = 0 Then nTargetDimension = 1

        'variable
        Dim i, j, count As Integer
        Dim priorityDimension(9, nTargetDimension) As Single
        Dim targetDim As Single
        Dim targetCount As Integer
        Dim cek(Nothing) As Boolean

        '===
        'get prioritize 1
        'the largest dimension
        count = 0
        ReDim cek(tempDim.GetUpperBound(0))
        Do Until count = nTargetDimension
            'reset from null
            targetDim = 0

            'find largest k-dimension
            For i = 1 To tempDim.GetUpperBound(0)
                If (targetDim < tempDim(i)) And (cek(i) = False) Then
                    targetDim = tempDim(i)
                    j = i
                End If
            Next

            'record data
            count += 1
            priorityDimension(1, count) = targetDim
            cek(j) = True
        Loop


        'get prioritize 5
        'the largest frequency dimension
        count = 0
        ReDim cek(tempDim.GetUpperBound(0))
        Do Until count = nTargetDimension
            'reset from null
            targetCount = 0

            'find largest k-dimension
            For i = 1 To FFrequency1.GetUpperBound(0)
                If ((targetCount < FFrequency1(i).Count) And (cek(i) = False)) Or _
                    ((targetCount = FFrequency1(i).Count) And (cek(i) = False) And (targetDim < FFrequency1(i).Dimension)) Then
                    targetDim = FFrequency1(i).Dimension
                    targetCount = FFrequency1(i).Count
                    j = i
                End If
            Next

            'record data
            count += 1
            priorityDimension(5, count) = FFrequency1(j).Dimension
            cek(j) = True
        Loop

        'recapitulation
        'for temporary only
        ReDim tempDim(2 * nTargetDimension)
        count = 0
        For i = 1 To 2
            For j = 1 To nTargetDimension
                If i = 1 Then
                    count += 1
                    tempDim(count) = priorityDimension(i, j)
                End If
                If i = 2 Then
                    count += 1
                    tempDim(count) = priorityDimension(5, j)
                End If
            Next
        Next

        'after recapitulation --> remove same dimension
        j = 0
        For i = 1 To tempDim.GetUpperBound(0)
            For j = 1 To tempDim.GetUpperBound(0)
                If (i <> j) And (tempDim(i) > 0) And (tempDim(i) = tempDim(j)) Then
                    tempDim(j) = 0
                End If
            Next
        Next

        'update data
        j = 0
        For i = 1 To tempDim.GetUpperBound(0)
            If (tempDim(i) > 0) Then
                j += 1
                If (i <> j) Then
                    tempDim(j) = tempDim(i)
                End If
            End If
        Next
        ReDim Preserve tempDim(j)
    End Sub

    ''' <summary>
    ''' Update frequency and prioritize
    ''' </summary>
    Private Function GetLengthDepth(ByVal boxList() As Box, ByVal percentM As Single) As Single()
        'set default value 0.1 --if percentM outside (0,1]
        If (0 >= percentM) And (percentM > 1) Then
            percentM = 0.1
        End If

        'get frequency
        Dim varietyDimension(Nothing) As Single
        GetFrequency(boxList, varietyDimension, FDataListInput)

        'get rank
        GetPrioritize(varietyDimension, percentM)

        'return value
        Return varietyDimension
    End Function

    ''' <summary>
    ''' Get alpha ratio as mentioned in Pisinger paper
    ''' </summary>
    Private Function GetAlphaRatio(ByVal boxList() As Box, ByVal lengthDepth As Single) As AlphaRatio()
        Dim i, j, count As Integer

        'reset array size
        Dim tempAlpha(6 * boxList.GetUpperBound(0)) As AlphaRatio

        'get alpha ratio
        count = 0
        For i = 1 To boxList.GetUpperBound(0)
            For j = 1 To 6
                'set orientation box first
                Select Case j
                    Case 1
                        boxList(i).Alpha = True
                        boxList(i).Orientation = True
                    Case 2
                        boxList(i).Alpha = True
                        boxList(i).Orientation = False
                    Case 3
                        boxList(i).Beta = True
                        boxList(i).Orientation = True
                    Case 4
                        boxList(i).Beta = True
                        boxList(i).Orientation = False
                    Case 5
                        boxList(i).Gamma = True
                        boxList(i).Orientation = True
                    Case 6
                        boxList(i).Gamma = True
                        boxList(i).Orientation = False
                End Select

                'next process if box is feasible in emptyspace
                If (boxList(i).Depth <= lengthDepth) And _
                    (boxList(i).Width <= FEmptySpace.Width) And _
                    (boxList(i).Height <= FEmptySpace.Height) Then

                    'record data if box feasible
                    count += 1
                    tempAlpha(count).Box = New Box(boxList(i))
                    tempAlpha(count).Ratio = boxList(i).Depth / lengthDepth
                End If
            Next
        Next
        'resize
        ReDim Preserve tempAlpha(count)

        'sorting alpha value
        For i = 1 To tempAlpha.GetUpperBound(0) - 1
            For j = i + 1 To tempAlpha.GetUpperBound(0)
                If (tempAlpha(i).Ratio < tempAlpha(j).Ratio) Or _
                    ((tempAlpha(i).Ratio = tempAlpha(j).Ratio) And _
                     ((tempAlpha(i).Box.Width * tempAlpha(i).Box.Depth * tempAlpha(i).Box.Height) < (tempAlpha(j).Box.Width * tempAlpha(j).Box.Depth * tempAlpha(j).Box.Height))) _
                     Then Swap(tempAlpha(i), tempAlpha(j))
            Next
        Next

        'return value
        Return tempAlpha
    End Function

    ''' <summary>
    ''' Recursive function
    ''' </summary>
    Private Sub GetLayer(ByVal currentEmptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As ListBox, ByRef bestPlacement() As Box, ByRef volTotal As Single)
        'variable
        Dim i, j, count As Integer
        Dim cek As Boolean

        Dim tempNumber, bestRatio, maxHeight, maxWidth As Single
        Dim pointerBestSolution(Nothing), currentPointer(Nothing), bestPointer(Nothing) As Integer
        Dim tempEmptySpace, boxPlacement(Nothing), tempBoxPlacement(Nothing) As Box
        Dim originPoint As New Point3D(0, 0, 0)

        cek = False
        Do Until cek = True
            'reset data
            cek = True
            tempNumber = FEmptySpace.Depth
            ReDim pointerBestSolution(alphaR.GetUpperBound(0))
            bestRatio = 0
            count = bestPlacement.GetUpperBound(0)

            'choose maximal join box
            GetKnapsackJoinBox(currentEmptySpace, alphaR, dataList, 0, currentPointer, tempNumber, bestRatio, bestPointer)

            'prepare for horizontal strip and vertical strip
            maxWidth = 0 : maxHeight = 0 : bestRatio = 0
            For i = 1 To bestPointer.GetUpperBound(0)
                If alphaR(bestPointer(i)).Box.Width > maxWidth Then maxWidth = alphaR(bestPointer(i)).Box.Width
                If alphaR(bestPointer(i)).Box.Height > maxHeight Then maxHeight = alphaR(bestPointer(i)).Box.Height
            Next

            'start to fill strip
            'i=1 --> horizontal strip
            'i=2 --> vertical strip
            For i = 1 To 2
                If i = 1 Then
                    tempEmptySpace = New Box(-1, currentEmptySpace.Width, maxHeight, FEmptySpace.Depth, CByte(1))
                    originPoint.Y += maxWidth
                    GetStrip(False, tempEmptySpace, alphaR, dataList, originPoint, boxPlacement, tempNumber)
                Else
                    tempEmptySpace = New Box(-1, maxWidth, currentEmptySpace.Height, FEmptySpace.Depth, CByte(1))
                    originPoint.Z += maxHeight
                    GetStrip(True, tempEmptySpace, alphaR, dataList, originPoint, boxPlacement, tempNumber)
                End If

                'record best value
                If bestRatio < tempNumber Then
                    maxHeight = 0 : maxWidth = 0
                    ReDim Preserve bestPlacement(count + boxPlacement.GetUpperBound(0))
                    For j = 1 To bestPlacement.GetUpperBound(0)
                        bestPlacement(count + j) = New Box(boxPlacement(j))
                        bestPlacement(count + j).LocationTemp = New Point3D(boxPlacement(j).LocationTemp)
                        If maxHeight < boxPlacement(j).Height Then maxHeight = boxPlacement(j).Height
                        If maxWidth < boxPlacement(j).Width Then maxWidth = boxPlacement(j).Width
                    Next
                    bestRatio = tempNumber

                    'update empty space
                    If i = 1 Then
                        currentEmptySpace = New Box(-1, currentEmptySpace.Width, currentEmptySpace.Height - maxHeight, currentEmptySpace.Depth, CByte(1))
                    Else
                        currentEmptySpace = New Box(-1, currentEmptySpace.Width - maxWidth, currentEmptySpace.Height, currentEmptySpace.Depth, CByte(1))
                    End If
                End If
            Next

            'cek there's no box that can come to emptyspace
            For i = 1 To alphaR.GetUpperBound(0)
                If (alphaR(i).Box.Depth <= currentEmptySpace.Depth) And _
                    (alphaR(i).Box.Width <= currentEmptySpace.Depth) And _
                    (alphaR(i).Box.Height <= currentEmptySpace.Depth) Then
                    For j = 1 To dataList.GetUpperBound(0)
                        If (alphaR(i).Box.Type = dataList(j).SType) And (dataList(j).SCount > 0) Then
                            cek = False
                            Exit For
                        End If
                    Next
                    If cek = False Then Exit For
                End If
            Next
        Loop

        'all solution have been gathered at here.
        'calculate placement
        volTotal = 0
        For i = 1 To bestPlacement.GetUpperBound(0)
            volTotal += bestPlacement(i).Depth * bestPlacement(i).Width * bestPlacement(i).Height
        Next

    End Sub

    ''' <summary>
    ''' Get join box in order to improve fill ratio
    ''' </summary>
    Private Sub GetKnapsackJoinBox(ByVal emptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As ListBox, ByVal lastLevel As Integer, ByVal currentPointer() As Integer, _
                                   ByRef resDepth As Single, ByRef bestRatio As Single, ByRef bestPointer() As Integer)
        Dim i As Integer
        Dim cek As Boolean = True
        Dim maxWidth, maxHeight As Single
        Dim currentRatio As Single = 0

        'filter = exist box in bound of (emptyspace --width and heigh + resdepth)
        For i = 1 To alphaR.GetUpperBound(0)
            If (alphaR(i).Box.Depth <= resDepth) And _
                (alphaR(i).Box.Width <= emptySpace.Width) And _
                (alphaR(i).Box.Height <= emptySpace.Height) Then
                For j = 1 To dataList.GetUpperBound(0)
                    If (alphaR(i).Box.Type = dataList(j).SType) And (dataList(j).SCount > 0) Then
                        cek = False
                        Exit For
                    End If
                Next
                If cek = False Then Exit For
            End If
        Next

        'cek = true --> there's no alpha that can be added
        If cek = True Then
            'calculate best ratio
            currentRatio = 0
            maxHeight = 0
            maxWidth = 0
            For i = 1 To lastLevel
                currentRatio += (alphaR(currentPointer(i)).Box.Width * alphaR(currentPointer(i)).Box.Height * alphaR(currentPointer(i)).Box.Depth)
                If maxWidth < alphaR(currentPointer(i)).Box.Width Then maxWidth = alphaR(currentPointer(i)).Box.Width
                If maxHeight < alphaR(currentPointer(i)).Box.Height Then maxHeight = alphaR(currentPointer(i)).Box.Height
            Next
            currentRatio = currentRatio / (maxWidth * maxHeight * emptySpace.Depth)

            'update best ratio
            If currentRatio > bestRatio Then
                bestRatio = currentRatio
                ReDim bestPointer(lastLevel)
                For i = 1 To lastLevel
                    bestPointer(i) = currentPointer(i)
                Next
                'number of last pointer
                bestPointer(0) = lastLevel
            End If

        Else

            For i = 1 To alphaR.GetUpperBound(0)
                'cek alpha(i) and other prerequisit feasible?
                For j = 1 To dataList.GetUpperBound(0)
                    If (dataList(j).SType = alphaR(i).Box.Type) And _
                        (dataList(j).SCount > 0) And _
                        (alphaR(i).Box.Depth <= resDepth) And _
                        (alphaR(i).Box.Width <= emptySpace.Width) And _
                        (alphaR(i).Box.Height <= emptySpace.Height) Then
                        dataList(j).SCount -= 1

                        If currentPointer.GetUpperBound(0) = lastLevel Then _
                            ReDim Preserve currentPointer(lastLevel + 1)
                        currentPointer(lastLevel + 1) = i

                        'go to the next level
                        GetKnapsackJoinBox(emptySpace, alphaR, dataList, lastLevel + 1, currentPointer, (resDepth - alphaR(i).Box.Depth), bestRatio, bestPointer)

                        'after rekursif back... data must be back to normal
                        dataList(j).SCount += 1
                    End If
                Next
            Next
        End If
    End Sub

    ''' <summary>
    ''' Fill strip --horizontal or vertical
    ''' </summary>
    Private Sub GetStrip(ByVal direction As Boolean, ByVal emptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As ListBox, ByVal originPoint As Point3D, ByRef boxPlacement() As Box, ByRef volTotal As Single)
        Dim cek As Boolean
        Dim pointerBestSolution(alphaR.GetUpperBound(0)) As Integer
        Dim bestRatio, tempResDepth As Single
        Dim currentPointer(Nothing), bestPointer(Nothing) As Integer
        Dim count As Integer
        Dim currentCoordinate, pointerCoordinate(Nothing) As Point3D
        Dim wEmpty, dEmpty, hEmpty, maxHeight, maxWidth As Single

        'reset data
        cek = False
        currentCoordinate = New Point3D(originPoint)
        wEmpty = emptySpace.Width
        dEmpty = emptySpace.Depth
        hEmpty = emptySpace.Height

        'start iteration
        Do Until cek = True
            cek = True
            '1. reset data
            bestRatio = 0
            tempResDepth = emptySpace.Depth

            '2. choose maximal join box
            GetKnapsackJoinBox(emptySpace, alphaR, dataList, 0, currentPointer, tempResDepth, bestRatio, bestPointer)

            '3. find coordinate of join box
            ReDim pointerCoordinate(bestPointer(0))
            For i = 1 To bestPointer(0)
                If i = 1 Then
                    pointerCoordinate(i) = New Point3D(currentCoordinate.X, currentCoordinate.Y, currentCoordinate.Z)
                Else
                    pointerCoordinate(i) = New Point3D(pointerCoordinate(i - 1).X + alphaR(i - 1).Box.Depth, currentCoordinate.Y, currentCoordinate.Z)
                End If
            Next

            '4. record coordinate + get max dimension + update datalist
            count = boxPlacement.GetUpperBound(0)
            maxHeight = 0 : maxWidth = 0
            ReDim Preserve boxPlacement(boxPlacement.GetUpperBound(0) + bestPointer(0))
            For i = 1 To bestPointer(0)
                'record coord
                count += 1
                boxPlacement(count) = New Box(alphaR(bestPointer(i)).Box)
                boxPlacement(count).LocationTemp = New Point3D(pointerCoordinate(i))

                'get max dimension
                If maxHeight < alphaR(bestPointer(i)).Box.Height Then maxHeight = alphaR(bestPointer(i)).Box.Height
                If maxWidth < alphaR(bestPointer(i)).Box.Width Then maxWidth = alphaR(bestPointer(i)).Box.Width

                'update datalist
                For j = 1 To dataList.GetUpperBound(0)
                    If alphaR(bestPointer(i)).Box.Type = dataList(j).SType Then dataList(j).SCount -= 1
                Next
            Next

            '5. set for next emptyspace
            'true --> vertical, emptyspace = wfix, hres, dfix
            'false --> horizontal = wres, hfix, dfix
            If direction = True Then
                hEmpty -= maxHeight
            Else
                wEmpty -= maxWidth
            End If
            emptySpace = New Box(-1, wEmpty, hEmpty, dEmpty, CByte(1))

            '6. find there's more dimension that can be adequate in empty space
            For i = 1 To alphaR.GetUpperBound(0)
                If (alphaR(i).Box.Depth <= emptySpace.Depth) And _
                    (alphaR(i).Box.Width <= emptySpace.Width) And _
                    (alphaR(i).Box.Height <= emptySpace.Height) Then
                    For j = 1 To dataList.GetUpperBound(0)
                        If (alphaR(i).Box.Type = dataList(j).SType) And (dataList(j).SCount > 0) Then
                            cek = False
                            Exit For
                        End If
                    Next
                    If cek = False Then Exit For
                End If
            Next
        Loop

        '7. get utilization
        volTotal = 0
        For i = 1 To boxPlacement.GetUpperBound(0)
            volTotal += boxPlacement(i).Depth * boxPlacement(i).Width * boxPlacement(i).Height
        Next
    End Sub
End Class
