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
        Dim i, j As Integer
        Dim alphaR(Nothing) As AlphaRatio
        Dim utilBest, utilTemp As Single
        Dim cek As Boolean
        Dim currentEmptySpace, tempPlacement(Nothing) As Box
        Dim originPoint As New Point3D(FEmptySpace.LocationTemp)

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
        utilBest = 0
        For i = 1 To FLengthDepth.GetUpperBound(0)
            'calculate alpha fill ratio
            ReDim alphaR(boxlist.GetUpperBound(0))
            alphaR = GetAlphaRatio(boxlist, FLengthDepth(i))

            'reset emptyspace
            currentEmptySpace = New Box(-1, FEmptySpace.Width, FEmptySpace.Height, FLengthDepth(i), CByte(1))
            cek = False
            ReDim tempPlacement(Nothing)

            GetLayer(currentEmptySpace, alphaR, FDataListInput, tempPlacement)

            'catet bestPlacement.... :)
            'pertama itung dulu lah, utilization yang bisa di pack berapa
            utilTemp = 0
            For j = 1 To tempPlacement.GetUpperBound(0)
                utilTemp += tempPlacement(j).Width * tempPlacement(j).Depth * tempPlacement(j).Height
            Next
            utilTemp = utilTemp / (currentEmptySpace.Width * currentEmptySpace.Depth * currentEmptySpace.Height)

            If utilBest < utilTemp Then
                utilBest = utilTemp
                ReDim FBox(tempPlacement.GetUpperBound(0))
                For j = 1 To tempPlacement.GetUpperBound(0)
                    FBox(j) = New Box(tempPlacement(j))
                Next
            End If

            '    '=== obsolete dulu ya..
            '    'iteration only for largest alpha ratio as references box
            '    For j = 1 To alphaR.GetUpperBound(0)
            '        If alphaR(j).Ratio = maxRatio Then
            '            'reset emptyspace
            '            currentEmptySpace = New Box(-1, FEmptySpace.Width, FEmptySpace.Height, FEmptySpace.Depth)
            '            cek = False

            '            'recursive for each alphaR.getupperbound --> until there's no empty space
            '            GetLayer(currentEmptySpace, alphaR, FDataListInput, tempPlacement, tempVol)

            '            'get best placement
            '            If volBest < tempVol Then
            '                volBest = tempVol
            '                ReDim FBox(tempPlacement.GetUpperBound(0))
            '                For k = 1 To FBox.GetUpperBound(0)
            '                    FBox(k) = New Box(tempPlacement(k))
            '                    FBox(k).LocationContainer = New Point3D(tempPlacement(k).LocationTemp.X + FEmptySpace.LocationContainer.X, _
            '                                                            tempPlacement(k).LocationTemp.Y + FEmptySpace.LocationContainer.Y, _
            '                                                            tempPlacement(k).LocationTemp.Z + FEmptySpace.LocationContainer.Z)
            '                Next
            '            End If
            '        End If
            '    Next

        Next

        'get best placement + write output
        FUtilization = utilBest
        GetOutput()
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
    Private Sub GetLayer(ByVal currentEmptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As ListBox, ByRef bestPlacement() As Box)
        'variable
        Dim i, j, count As Integer
        Dim cek(alphaR.GetUpperBound(0)) As Boolean
        Dim maxRatio As Single
        Dim currentPointer(Nothing), bestPointer(Nothing) As Integer
        Dim bestEmptySpace As Box = Nothing
        Dim originPoint As New Point3D(currentEmptySpace.LocationTemp)

        '#stopping rule of recursive
        'cek there's availability box
        cek(0) = CheckAvailabilityBox(alphaR, dataList, New Point3D(currentEmptySpace.Depth, currentEmptySpace.Width, currentEmptySpace.Height))

        'if cek = false --> there's a box that can be fit in emptyspace
        '#begin recursive.
        If cek(0) = True Then
            'reset data
            count = bestPlacement.GetUpperBound(0)
            Dim tempNumber, bestRatio, maxHeight, maxWidth As Single
            Dim tempEmptySpace, boxPlacement(Nothing), tempBoxPlacement(Nothing) As Box

            cek = GetAvailabilityBox(alphaR, dataList, New Point3D(currentEmptySpace.Depth, currentEmptySpace.Width, currentEmptySpace.Height))

            '1. find maximum ratio as reference box
            maxRatio = 0
            For i = 1 To alphaR.GetUpperBound(0)
                If (maxRatio < alphaR(i).Ratio) And (cek(i) = True) Then maxRatio = alphaR(i).Ratio
            Next

            '-finding alternative ratio, from join knapsack
            bestRatio = 0
            tempNumber = currentEmptySpace.Depth
            GetKnapsackJoinBox(currentEmptySpace, alphaR, dataList, 0, currentPointer, tempNumber, bestRatio, bestPointer)

            '-select references of box
            If maxRatio < bestRatio Then
                'prepare for placing strip (horizontal and vertical) + update data list + placing reference box
                maxWidth = 0 : maxHeight = 0 : bestRatio = 0
                ReDim Preserve bestPlacement(count + bestPointer.GetUpperBound(0))
                For i = 1 To bestPointer.GetUpperBound(0)
                    If alphaR(bestPointer(i)).Box.Width > maxWidth Then maxWidth = alphaR(bestPointer(i)).Box.Width
                    If alphaR(bestPointer(i)).Box.Height > maxHeight Then maxHeight = alphaR(bestPointer(i)).Box.Height

                    For j = 1 To dataList.GetUpperBound(0)
                        If alphaR(bestPointer(i)).Box.Type = dataList(j).SType Then dataList(j).SCount -= 1
                    Next

                    'copy references box
                    bestPlacement(count + i) = New Box(alphaR(bestPointer(i)).Box)
                    If i = 1 Then
                        bestPlacement(count + i).LocationTemp = New Point3D(originPoint)
                    Else
                        bestPlacement(count + i).LocationTemp = New Point3D(bestPlacement(count + i - 1).LocationTemp.X + bestPlacement(count + i - 1).Depth, _
                                                                            originPoint.Y, _
                                                                            originPoint.Z)
                    End If

                Next

                'start to fill strip
                'i=1 --> horizontal strip
                'i=2 --> vertical strip
                For i = 1 To 2
                    If i = 1 Then
                        ReDim boxPlacement(Nothing)
                        tempEmptySpace = New Box(-1, currentEmptySpace.Width - maxWidth, maxHeight, currentEmptySpace.Depth, CByte(1))
                        GetStrip(False, tempEmptySpace, alphaR, dataList, New Point3D(originPoint.X, originPoint.Y + maxWidth, originPoint.Z), boxPlacement, tempNumber)
                    Else
                        ReDim boxPlacement(Nothing)
                        tempEmptySpace = New Box(-1, maxWidth, currentEmptySpace.Height - maxHeight, currentEmptySpace.Depth, CByte(1))
                        GetStrip(True, tempEmptySpace, alphaR, dataList, New Point3D(originPoint.X, originPoint.Y, originPoint.Z + maxHeight), boxPlacement, tempNumber)
                    End If

                    'record best value
                    If bestRatio < tempNumber Then
                        maxHeight = 0 : maxWidth = 0
                        ReDim Preserve bestPlacement(bestPointer.GetUpperBound(0) + boxPlacement.GetUpperBound(0))
                        For j = 1 To boxPlacement.GetUpperBound(0)
                            bestPlacement(count + bestPointer.GetUpperBound(0) + j) = New Box(boxPlacement(j))
                            bestPlacement(count + bestPointer.GetUpperBound(0) + j).LocationTemp = New Point3D(boxPlacement(j).LocationTemp)
                            If maxHeight < boxPlacement(j).Height Then maxHeight = boxPlacement(j).Height
                            If maxWidth < boxPlacement(j).Width Then maxWidth = boxPlacement(j).Width
                        Next
                        bestRatio = tempNumber

                        'update empty space
                        'i = 1 --> horizontal strip.. change height
                        'i = 2 --> vertical strip.. change width
                        If i = 1 Then
                            bestEmptySpace = New Box(-1, currentEmptySpace.Width, currentEmptySpace.Height - maxHeight, currentEmptySpace.Depth, CByte(1))
                            bestEmptySpace.LocationTemp = New Point3D(originPoint.X, originPoint.Y, originPoint.Z + maxHeight)
                        Else
                            bestEmptySpace = New Box(-1, currentEmptySpace.Width - maxWidth, currentEmptySpace.Height, currentEmptySpace.Depth, CByte(1))
                            bestEmptySpace.LocationTemp = New Point3D(originPoint.X, originPoint.Y + maxWidth, originPoint.Z)
                        End If
                    End If
                Next

                '---finishing
                'after find the best one...
                'update datalist
                For i = count + 1 To bestPlacement.GetUpperBound(0)
                    For j = 1 To dataList.GetUpperBound(0)
                        If bestPlacement(i).Type = dataList(j).SType Then
                            dataList(j).SCount -= 1
                            Exit For
                        End If
                    Next
                Next

                'goto the next layer
                GetLayer(bestEmptySpace, alphaR, dataList, bestPlacement)


            Else
                'uda di uji coba semua aja.. daripada pusink2.. bener ga?
                bestRatio = 0
                For j = 1 To alphaR.GetUpperBound(0)
                    If alphaR(j).Ratio = maxRatio Then

                        'prepare for placing strip (horizontal and vertical)
                        maxWidth = alphaR(j).Box.Width
                        maxHeight = alphaR(j).Box.Height

                        'get strip
                        For i = 1 To 2
                            If i = 1 Then
                                ReDim boxPlacement(Nothing)
                                tempEmptySpace = New Box(-1, currentEmptySpace.Width - maxWidth, maxHeight, currentEmptySpace.Depth, CByte(1))
                                GetStrip(False, tempEmptySpace, alphaR, dataList, New Point3D(originPoint.X, originPoint.Y + maxWidth, originPoint.Z), boxPlacement, tempNumber)
                            Else
                                ReDim boxPlacement(Nothing)
                                tempEmptySpace = New Box(-1, maxWidth, currentEmptySpace.Height - maxHeight, currentEmptySpace.Depth, CByte(1))
                                GetStrip(True, tempEmptySpace, alphaR, dataList, New Point3D(originPoint.X, originPoint.Y, originPoint.Z + maxHeight), boxPlacement, tempNumber)
                            End If

                            'update best
                            If bestRatio < tempNumber Then
                                ReDim Preserve bestPlacement(count + boxPlacement.GetUpperBound(0) + 1)
                                maxHeight = 0 : maxWidth = 0

                                bestPlacement(count + 1) = New Box(alphaR(j).Box)
                                bestPlacement(count + 1).LocationTemp = New Point3D(originPoint)

                                For k = 1 To boxPlacement.GetUpperBound(0)
                                    bestPlacement(count + k + 1) = New Box(boxPlacement(k))
                                    bestPlacement(count + k + 1).LocationTemp = New Point3D(boxPlacement(k).LocationTemp)
                                    If maxHeight < boxPlacement(k).Height Then maxHeight = boxPlacement(k).Height
                                    If maxWidth < boxPlacement(k).Width Then maxWidth = boxPlacement(k).Width
                                Next
                                bestRatio = tempNumber

                                'update empty space
                                'i = 1 --> horizontal strip.. change height
                                'i = 2 --> vertical strip.. change width
                                If i = 1 Then
                                    bestEmptySpace = New Box(-1, currentEmptySpace.Width, currentEmptySpace.Height - maxHeight, currentEmptySpace.Depth, CByte(1))
                                    bestEmptySpace.LocationTemp = New Point3D(originPoint.X, originPoint.Y, originPoint.Z + maxHeight)
                                Else
                                    bestEmptySpace = New Box(-1, currentEmptySpace.Width - maxWidth, currentEmptySpace.Height, currentEmptySpace.Depth, CByte(1))
                                    bestEmptySpace.LocationTemp = New Point3D(originPoint.X, originPoint.Y + maxWidth, originPoint.Z)
                                End If
                            End If
                        Next

                    End If
                Next

                '---finishing
                'after find the best one...
                'update datalist
                For i = count + 1 To bestPlacement.GetUpperBound(0)
                    For j = 1 To dataList.GetUpperBound(0)
                        If bestPlacement(i).Type = dataList(j).SType Then
                            dataList(j).SCount -= 1
                            Exit For
                        End If
                    Next
                Next

                'goto the next layer
                GetLayer(bestEmptySpace, alphaR, dataList, bestPlacement)

            End If

        End If





        ''===obsolete dulu ya... bikin stress soalnya.







        'cek = False
        'Do Until cek = True
        '    'reset data
        '    cek = True
        '    tempNumber = FEmptySpace.Depth
        '    ReDim pointerBestSolution(alphaR.GetUpperBound(0))
        '    bestRatio = 0
        '    count = bestPlacement.GetUpperBound(0)

        '    'choose maximal join box --> to get alphaMax ratio near 1





        '    'cek there's no box that can come to emptyspace
        '    For i = 1 To alphaR.GetUpperBound(0)
        '        If (alphaR(i).Box.Depth <= currentEmptySpace.Depth) And _
        '            (alphaR(i).Box.Width <= currentEmptySpace.Depth) And _
        '            (alphaR(i).Box.Height <= currentEmptySpace.Depth) Then
        '            For j = 1 To dataList.GetUpperBound(0)
        '                If (alphaR(i).Box.Type = dataList(j).SType) And (dataList(j).SCount > 0) Then
        '                    cek = False
        '                    Exit For
        '                End If
        '            Next
        '            If cek = False Then Exit For
        '        End If
        '    Next
        'Loop

        ''all solution have been gathered at here.
        ''calculate placement
        'volTotal = 0
        'For i = 1 To bestPlacement.GetUpperBound(0)
        '    volTotal += bestPlacement(i).Depth * bestPlacement(i).Width * bestPlacement(i).Height
        'Next

    End Sub

    ''' <summary>
    ''' Get join box in order to improve fill ratio
    ''' </summary>
    Private Sub GetKnapsackJoinBox(ByVal currentEmptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As ListBox, ByVal lastLevel As Integer, ByVal currentPointer() As Integer, _
                                   ByVal resDepth As Single, ByRef bestRatio As Single, ByRef bestPointer() As Integer)
        'variable
        Dim i, j As Integer
        Dim cek(0) As Boolean

        '#define stopping rule
        'stop1: availability box
        cek(0) = CheckAvailabilityBox(alphaR, dataList, New Point3D(resDepth, currentEmptySpace.Width, currentEmptySpace.Height))

        'stop2: maxRatio maksimum
        If (cek(0) = True) And (bestRatio = 1) Then
            cek(0) = False
        End If


        '#start recursive
        If cek(0) = True Then
            'get availability box
            cek = GetAvailabilityBox(alphaR, dataList, New Point3D(resDepth, currentEmptySpace.Width, currentEmptySpace.Height))

            For i = 1 To alphaR.GetUpperBound(0)
                If (cek(i) = True) And (bestRatio < 1) Then
                    For j = 1 To dataList.GetUpperBound(0)
                        If (dataList(j).SType = alphaR(i).Box.Type) Then
                            dataList(j).SCount -= 1
                            Exit For
                        End If
                    Next

                    'update current pointer
                    If currentPointer.GetUpperBound(0) = lastLevel Then ReDim Preserve currentPointer(lastLevel + 1)
                    currentPointer(lastLevel + 1) = i

                    'go to the next level
                    GetKnapsackJoinBox(currentEmptySpace, alphaR, dataList, lastLevel + 1, currentPointer, (resDepth - alphaR(i).Box.Depth), bestRatio, bestPointer)

                    'after rekursif back... data must be back to normal
                    dataList(j).SCount += 1
                End If
            Next

        Else

            'calculate best ratio
            Dim maxWidth, maxHeight, currentRatio As Single
            currentRatio = 0 : maxHeight = 0 : maxWidth = 0

            For i = 1 To lastLevel
                currentRatio += (alphaR(currentPointer(i)).Box.Width * alphaR(currentPointer(i)).Box.Height * alphaR(currentPointer(i)).Box.Depth)
                If maxWidth < alphaR(currentPointer(i)).Box.Width Then maxWidth = alphaR(currentPointer(i)).Box.Width
                If maxHeight < alphaR(currentPointer(i)).Box.Height Then maxHeight = alphaR(currentPointer(i)).Box.Height
            Next
            currentRatio = currentRatio / (maxWidth * maxHeight * currentEmptySpace.Depth)

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
        End If
    End Sub

    ''' <summary>
    ''' Fill strip --horizontal or vertical
    ''' True = vertical
    ''' False = horizontal
    ''' </summary>
    Private Sub GetStrip(ByVal direction As Boolean, ByVal currentEmptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As ListBox, ByVal originPoint As Point3D, ByRef boxPlacement() As Box, ByRef volTotal As Single)
        'variable
        Dim cek As Boolean = True
        Dim pointerCoordinate(Nothing) As Point3D

        Dim pointerBestSolution(alphaR.GetUpperBound(0)), _
            currentPointer(Nothing), bestPointer(Nothing), _
            count As Integer

        Dim bestRatio, tempResDepth, _
            wEmpty, dEmpty, hEmpty, _
            maxHeight, maxWidth As Single

        'reset data
        wEmpty = currentEmptySpace.Width
        dEmpty = currentEmptySpace.Depth
        hEmpty = currentEmptySpace.Height

        'start iteration
        Do Until cek = False
            '1. reset data
            bestRatio = 0
            tempResDepth = currentEmptySpace.Depth

            '2. choose maximal join box
            GetKnapsackJoinBox(currentEmptySpace, alphaR, dataList, 0, currentPointer, tempResDepth, bestRatio, bestPointer)

            '3. find coordinate of join box
            ReDim pointerCoordinate(bestPointer(0))
            For i = 1 To bestPointer(0)
                If i = 1 Then
                    pointerCoordinate(i) = New Point3D(originPoint.X, originPoint.Y, originPoint.Z)
                Else
                    pointerCoordinate(i) = New Point3D(pointerCoordinate(i - 1).X + alphaR(i - 1).Box.Depth, originPoint.Y, originPoint.Z)
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
                    If alphaR(bestPointer(i)).Box.Type = dataList(j).SType Then
                        dataList(j).SCount -= 1
                        Exit For
                    End If
                Next
            Next

            '5. set for next emptyspace
            'true --> vertical, emptyspace = wfix, hres, dfix
            'false --> horizontal = wres, hfix, dfix
            If direction = True Then
                hEmpty -= maxHeight
                originPoint.Z += maxHeight
            Else
                wEmpty -= maxWidth
                originPoint.Y += maxWidth
            End If
            currentEmptySpace = New Box(-1, wEmpty, hEmpty, dEmpty, CByte(1))

            '6. find there's more dimension that can be adequate in empty space
            cek = CheckAvailabilityBox(alphaR, dataList, New Point3D(dEmpty, wEmpty, hEmpty))
        Loop

        '7. get utilization
        volTotal = 0
        For i = 1 To boxPlacement.GetUpperBound(0)
            volTotal += boxPlacement(i).Depth * boxPlacement(i).Width * boxPlacement(i).Height
        Next
    End Sub

    ''' <summary>
    ''' Checking availability box
    ''' cek = true --> there's box that available
    ''' cek = false --> no box that available
    ''' </summary>
    Private Function CheckAvailabilityBox(ByVal alphaR() As AlphaRatio, ByVal dataList() As ListBox, ByVal emptySpaceDim As Point3D) As Boolean
        Dim cek As Boolean = False
        For i = 1 To alphaR.GetUpperBound(0)
            If (alphaR(i).Box.Depth <= emptySpaceDim.X) And _
                (alphaR(i).Box.Width <= emptySpaceDim.Y) And _
                (alphaR(i).Box.Height <= emptySpaceDim.Z) Then
                For j = 1 To dataList.GetUpperBound(0)
                    If (alphaR(i).Box.Type = dataList(j).SType) And (dataList(j).SCount > 0) Then
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
    ''' Get availability box --array boolean
    ''' </summary>
    Private Function GetAvailabilityBox(ByVal alphaR() As AlphaRatio, ByVal dataList() As ListBox, ByVal emptySpaceDim As Point3D) As Boolean()
        Dim cek(alphaR.GetUpperBound(0)) As Boolean

        For i = 1 To alphaR.GetUpperBound(0)
            If (alphaR(i).Box.Depth <= emptySpaceDim.X) And _
                (alphaR(i).Box.Width <= emptySpaceDim.Y) And _
                (alphaR(i).Box.Height <= emptySpaceDim.Z) Then
                For j = 1 To dataList.GetUpperBound(0)
                    If (alphaR(i).Box.Type = dataList(j).SType) And (dataList(j).SCount > 0) Then
                        cek(i) = True
                    End If
                Next
            End If
        Next

        Return cek
    End Function

    ''' <summary>
    ''' Finalize arrangement
    ''' </summary>
    Private Sub GetOutput()
        Dim i As Integer

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


End Class
