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
        fSpace = New Box(DEmpty)

        ReDim fInput(InputBox.GetUpperBound(0))
        For i = 1 To InputBox.GetUpperBound(0)
            fInput(i) = New Box(InputBox(i))
        Next

        'recapitulation box
        algRecapitulation(fInput, fListInput)
    End Sub

    ''' <summary>
    ''' Output box
    ''' </summary>
    Public ReadOnly Property OutputBox() As Box()
        Get
            GetOutput()
            Return fOutput
        End Get
    End Property

    ''' <summary>
    ''' Output list
    ''' </summary>
    Public ReadOnly Property OutputList() As strBoxList()
        Get
            GetOutput()
            Return fListOutput
        End Get
    End Property

    ''' <summary>
    ''' Output bounding box
    ''' </summary>
    Public ReadOnly Property OutputBoundingBox() As Box
        Get
            GetOutput()
            Return fBoundingBox
        End Get
    End Property

    ''' <summary>
    ''' Get frequency of boxes
    ''' --get different with update
    ''' --basic idea --> connect to all dimension in box... catch them all
    ''' </summary>
    Private Sub GetFrequency(ByVal boxList() As Box, ByRef tempDim() As Single, ByVal dataList() As strBoxList)
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
                'Array.Sort(temp)

                'find function1 occurence (is there dimension)
                If (temp(1) = tempDim(i)) Or _
                    (temp(2) = tempDim(i)) Or _
                    (temp(3) = tempDim(i)) Then _
                    FFrequency1(i).Count += dataList(j).SCount

                'find function2 occurence (max dimension)
                If (fMax3(temp(1), temp(2), temp(3)) = tempDim(i)) Then _
                    FFrequency2(i).Count += dataList(j).SCount

                'find function3 occurence (min dimension)
                If (fMin3(temp(1), temp(2), temp(3)) = tempDim(i)) Then _
                    FFrequency3(i).Count += dataList(j).SCount

                'If (temp(3) <> 0) And (temp(3) = tempDim(i)) Then
                '    FFrequency3(i).Count += dataList(j).SCount
                'ElseIf (temp(3) = 0) And (temp(2) <> 0) And (temp(2) = tempDim(i)) Then
                '    FFrequency3(i).Count += dataList(j).SCount
                'ElseIf (temp(3) = 0) And (temp(3) = 0) And (temp(1) = tempDim(3)) Then
                '    FFrequency3(i).Count += dataList(j).SCount
                'End If
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
        Dim currentEmptySpace, tempPlacement(Nothing) As Box
        Dim originPoint As New Point3D(fSpace.RelPos1)

        'update variety dimension
        Dim boxlist(fListInput.GetUpperBound(0)) As Box
        For i = 1 To fListInput.GetUpperBound(0)
            For j = 1 To fInput.GetUpperBound(0)
                If fInput(j).Type = fListInput(i).SType Then
                    boxlist(i) = New Box(fInput(j))
                    Exit For
                End If
            Next
        Next

        'get length of depth
        FLengthDepth = GetLengthDepth(fSpace.Depth, boxlist, 0.1)

        'begin iteration
        utilBest = 0
        For i = 1 To FLengthDepth.GetUpperBound(0)
            'process if flengthdepth < femptyspace.depth
            If FLengthDepth(i) <= fSpace.Depth Then
                'calculate alpha fill ratio
                ReDim alphaR(boxlist.GetUpperBound(0))
                alphaR = GetAlphaRatio(boxlist, FLengthDepth(i))

                'reset emptyspace
                currentEmptySpace = New Box(-1, FLengthDepth(i), fSpace.Width, fSpace.Height)
                ReDim tempPlacement(Nothing)

                GetLayer(currentEmptySpace, alphaR, fListInput, tempPlacement, 0)

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
            End If
        Next

        'get best placement + write output
        fUtilization = utilBest
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
    Private Function GetLengthDepth(ByVal filterDepth As Single, ByVal boxList() As Box, ByVal percentM As Single) As Single()
        'set default value 0.1 --if percentM outside (0,1]
        If (0 >= percentM) And (percentM > 1) Then
            percentM = 0.1
        End If

        'get frequency
        Dim varietyDimension(Nothing) As Single
        GetFrequency(boxList, varietyDimension, fListInput)

        'get filter
        GetFilterFrequency(filterDepth, varietyDimension)

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
                        boxList(i).IsAlpha = True
                        boxList(i).Orientation = True
                    Case 2
                        boxList(i).IsAlpha = True
                        boxList(i).Orientation = False
                    Case 3
                        boxList(i).IsBeta = True
                        boxList(i).Orientation = True
                    Case 4
                        boxList(i).IsBeta = True
                        boxList(i).Orientation = False
                    Case 5
                        boxList(i).IsGamma = True
                        boxList(i).Orientation = True
                    Case 6
                        boxList(i).IsGamma = True
                        boxList(i).Orientation = False
                End Select

                'next process if box is feasible in emptyspace
                If (boxList(i).Depth <= lengthDepth) And _
                    (boxList(i).Width <= fSpace.Width) And _
                    (boxList(i).Height <= fSpace.Height) Then

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
                     Then procSwap(tempAlpha(i), tempAlpha(j))
            Next
        Next

        'return value
        Return tempAlpha
    End Function

    ''' <summary>
    ''' Recursive function
    ''' </summary>
    Private Sub GetLayer(ByVal currentEmptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As strBoxList, ByRef bestPlacement() As Box, ByVal lastLevel As Integer)
        'variable
        Dim cek(alphaR.GetUpperBound(0)) As Boolean

        '#stopping rule of recursive
        'cek there's availability box
        cek(0) = CheckAvailabilityBox(alphaR, dataList, New Point3D(currentEmptySpace.Depth, currentEmptySpace.Width, currentEmptySpace.Height))
        'If currentEmptySpace Is Nothing Then
        '    cek(0) = False
        'Else
        '    cek(0) = CheckAvailabilityBox(alphaR, dataList, New Point3D(currentEmptySpace.Depth, currentEmptySpace.Width, currentEmptySpace.Height))
        'End If

        'if cek = false --> there's a box that can be fit in emptyspace
        '#begin recursive.
        If cek(0) = True Then
            'reset data
            Dim i, j, count As Integer
            Dim currentPointer(Nothing), bestPointer(Nothing) As Integer
            Dim bestEmptySpace As Box = Nothing
            Dim originPoint = New Point3D(currentEmptySpace.RelPos1)
            Dim tempNumber, pointerVol, bestRatio, maxRatio, maxHeight, maxWidth As Single
            Dim tempEmptySpace, boxPlacement(Nothing), tempBoxPlacement(Nothing) As Box
            Dim tempList(Nothing) As strBoxList

            count = bestPlacement.GetUpperBound(0)

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
                maxWidth = 0 : maxHeight = 0 : bestRatio = 0 : pointerVol = 0
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
                        bestPlacement(count + i).RelPos1 = New Point3D(originPoint)
                    Else
                        bestPlacement(count + i).RelPos1 = New Point3D(bestPlacement(count + i - 1).RelPos1.X + bestPlacement(count + i - 1).Depth, _
                                                                            originPoint.Y, originPoint.Z)
                    End If

                    'calculate pointerVol
                    pointerVol += alphaR(bestPointer(i)).Box.Width * alphaR(bestPointer(i)).Box.Depth * alphaR(bestPointer(i)).Box.Height
                Next

                'start to fill strip
                'i=1 --> horizontal strip
                'i=2 --> vertical strip
                For i = 1 To 2
                    'copy tempList
                    ReDim tempList(dataList.GetUpperBound(0))
                    Array.Copy(dataList, tempList, dataList.Length)

                    If i = 1 Then
                        ReDim boxPlacement(Nothing)
                        tempEmptySpace = New Box(-1, currentEmptySpace.Depth, currentEmptySpace.Width - maxWidth, maxHeight)
                        GetStrip(False, tempEmptySpace, alphaR, tempList, New Point3D(originPoint.X, originPoint.Y + maxWidth, originPoint.Z), boxPlacement, tempNumber)
                    Else
                        ReDim boxPlacement(Nothing)
                        tempEmptySpace = New Box(-1, currentEmptySpace.Depth, maxWidth, currentEmptySpace.Height - maxHeight)
                        GetStrip(True, tempEmptySpace, alphaR, tempList, New Point3D(originPoint.X, originPoint.Y, originPoint.Z + maxHeight), boxPlacement, tempNumber)
                    End If

                    'tempNumber = reference vol + strip vol
                    tempNumber += pointerVol

                    'record best value
                    If bestRatio < tempNumber Then
                        maxHeight = 0 : maxWidth = 0

                        ReDim Preserve bestPlacement(count + bestPointer.GetUpperBound(0) + boxPlacement.GetUpperBound(0))
                        For j = 1 To boxPlacement.GetUpperBound(0)
                            bestPlacement(count + bestPointer.GetUpperBound(0) + j) = New Box(boxPlacement(j))
                            bestPlacement(count + bestPointer.GetUpperBound(0) + j).RelPos1 = New Point3D(boxPlacement(j).RelPos1)
                        Next

                        For j = 1 To bestPointer.GetUpperBound(0) + boxPlacement.GetUpperBound(0)
                            If maxHeight < bestPlacement(count + j).Height Then maxHeight = bestPlacement(count + j).Height
                            If maxWidth < bestPlacement(count + j).Width Then maxWidth = bestPlacement(count + j).Width
                        Next
                        bestRatio = tempNumber

                        'update empty space
                        'i = 1 --> horizontal strip.. change height
                        'i = 2 --> vertical strip.. change width
                        If i = 1 Then
                            bestEmptySpace = New Box(-1, currentEmptySpace.Depth, currentEmptySpace.Width, currentEmptySpace.Height - maxHeight)
                            bestEmptySpace.RelPos1 = New Point3D(originPoint.X, originPoint.Y, originPoint.Z + maxHeight)
                        Else
                            bestEmptySpace = New Box(-1, currentEmptySpace.Depth, currentEmptySpace.Width - maxWidth, currentEmptySpace.Height)
                            bestEmptySpace.RelPos1 = New Point3D(originPoint.X, originPoint.Y + maxWidth, originPoint.Z)
                        End If
                    End If
                Next

                '---finishing
                'update datalist
                For i = count + bestPointer.GetUpperBound(0) + 1 To bestPlacement.GetUpperBound(0)
                    For j = 1 To dataList.GetUpperBound(0)
                        If bestPlacement(i).Type = dataList(j).SType Then
                            dataList(j).SCount -= 1
                            Exit For
                        End If
                    Next
                Next

                'goto the next layer
                GetLayer(bestEmptySpace, alphaR, dataList, bestPlacement, lastLevel + 1)


            Else
                'uda di uji coba semua aja.. daripada pusink2.. bener ga?
                bestRatio = 0
                For j = 1 To alphaR.GetUpperBound(0)
                    If (alphaR(j).Ratio = maxRatio) And (cek(j) = True) Then

                        Console.Write(j & "-")

                        'prepare for placing strip (horizontal and vertical)
                        maxWidth = alphaR(j).Box.Width
                        maxHeight = alphaR(j).Box.Height

                        'get strip
                        For i = 1 To 2
                            'reset tempList
                            ReDim tempList(dataList.GetUpperBound(0))
                            Array.Copy(dataList, tempList, dataList.Length)

                            'fix data list due to references box
                            For k = 1 To tempList.GetUpperBound(0)
                                If tempList(k).SType = alphaR(j).Box.Type Then tempList(k).SCount -= 1
                            Next

                            If i = 1 Then
                                ReDim boxPlacement(Nothing)
                                tempEmptySpace = New Box(-1, currentEmptySpace.Depth, currentEmptySpace.Width - maxWidth, maxHeight)
                                GetStrip(False, tempEmptySpace, alphaR, tempList, New Point3D(originPoint.X, originPoint.Y + maxWidth, originPoint.Z), boxPlacement, tempNumber)
                            Else
                                ReDim boxPlacement(Nothing)
                                tempEmptySpace = New Box(-1, currentEmptySpace.Depth, maxWidth, currentEmptySpace.Height - maxHeight)
                                GetStrip(True, tempEmptySpace, alphaR, tempList, New Point3D(originPoint.X, originPoint.Y, originPoint.Z + maxHeight), boxPlacement, tempNumber)
                            End If

                            'tempNumber = reference vol + strip vol
                            tempNumber += alphaR(j).Box.Width * alphaR(j).Box.Depth * alphaR(j).Box.Height

                            'update best
                            If bestRatio < tempNumber Then
                                maxHeight = 0 : maxWidth = 0

                                ReDim Preserve bestPlacement(count + 1 + boxPlacement.GetUpperBound(0))
                                bestPlacement(count + 1) = New Box(alphaR(j).Box)
                                bestPlacement(count + 1).RelPos1 = New Point3D(originPoint)

                                For k = 1 To boxPlacement.GetUpperBound(0)
                                    bestPlacement(count + k + 1) = New Box(boxPlacement(k))
                                    bestPlacement(count + k + 1).RelPos1 = New Point3D(boxPlacement(k).RelPos1)
                                Next

                                For k = 1 To boxPlacement.GetUpperBound(0) + 1
                                    If maxHeight < bestPlacement(count + k).Height Then maxHeight = bestPlacement(count + k).Height
                                    If maxWidth < bestPlacement(count + k).Width Then maxWidth = bestPlacement(count + k).Width
                                Next
                                bestRatio = tempNumber

                                'update empty space
                                'i = 1 --> horizontal strip.. change height
                                'i = 2 --> vertical strip.. change width
                                If i = 1 Then
                                    bestEmptySpace = New Box(-1, currentEmptySpace.Depth, currentEmptySpace.Width, currentEmptySpace.Height - maxHeight)
                                    bestEmptySpace.RelPos1 = New Point3D(originPoint.X, originPoint.Y, originPoint.Z + maxHeight)
                                Else
                                    bestEmptySpace = New Box(-1, currentEmptySpace.Depth, currentEmptySpace.Width - maxWidth, currentEmptySpace.Height)
                                    bestEmptySpace.RelPos1 = New Point3D(originPoint.X, originPoint.Y + maxWidth, originPoint.Z)
                                End If
                            End If
                        Next

                    End If
                Next

                '---finishing
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
                GetLayer(bestEmptySpace, alphaR, dataList, bestPlacement, lastLevel + 1)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Get join box in order to improve fill ratio
    ''' </summary>
    Private Sub GetKnapsackJoinBox(ByVal currentEmptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As strBoxList, ByVal lastLevel As Integer, ByVal currentPointer() As Integer, _
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
    Private Sub GetStrip(ByVal direction As Boolean, ByVal currentEmptySpace As Box, ByVal alphaR() As AlphaRatio, ByVal dataList() As strBoxList, ByVal originPoint As Point3D, ByRef boxPlacement() As Box, ByRef volTotal As Single)
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
                    pointerCoordinate(i) = New Point3D(pointerCoordinate(i - 1).X + alphaR(bestPointer(i - 1)).Box.Depth, originPoint.Y, originPoint.Z)
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
                boxPlacement(count).RelPos1 = New Point3D(pointerCoordinate(i))

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
            currentEmptySpace = New Box(-1, dEmpty, wEmpty, hEmpty)

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
    Private Function CheckAvailabilityBox(ByVal alphaR() As AlphaRatio, ByVal dataList() As strBoxList, ByVal emptySpaceDim As Point3D) As Boolean
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
    Private Function GetAvailabilityBox(ByVal alphaR() As AlphaRatio, ByVal dataList() As strBoxList, ByVal emptySpaceDim As Point3D) As Boolean()
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
        Dim minPoint, maxPoint As Point3D

        'resize as used box
        ReDim fOutput(fInput.GetUpperBound(0))
        'cloning input --> output
        For i = 1 To fInput.GetUpperBound(0)
            fOutput(i) = New Box(fInput(i))
        Next

        'get container coordinate + bounding box
        minPoint = New Point3D(fSpace.RelPos2)
        maxPoint = New Point3D(fSpace.RelPos1)
        For i = 1 To FBox.GetUpperBound(0)
            FBox(i).RelPos1 = New Point3D(fSpace.RelPos1.X + FBox(i).RelPos1.X, _
                                                    fSpace.RelPos1.Y + FBox(i).RelPos1.Y, _
                                                    fSpace.RelPos1.Z + FBox(i).RelPos1.Z)

            If FBox(i).RelPos1.X < minPoint.X Then minPoint.X = FBox(i).RelPos1.X
            If FBox(i).RelPos1.Y < minPoint.Y Then minPoint.Y = FBox(i).RelPos1.Y
            If FBox(i).RelPos1.Z < minPoint.Z Then minPoint.Z = FBox(i).RelPos1.Z
            If FBox(i).RelPos2.X > maxPoint.X Then maxPoint.X = FBox(i).RelPos2.X
            If FBox(i).RelPos2.Y > maxPoint.Y Then maxPoint.Y = FBox(i).RelPos2.Y
            If FBox(i).RelPos2.Z > maxPoint.Z Then maxPoint.Z = FBox(i).RelPos2.Z
        Next
        fBoundingBox = New Box(-1, maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y, maxPoint.Z - minPoint.Z)
        fBoundingBox.RelPos1 = New Point3D(minPoint)

        'revision for box in wall only
        For i = 1 To FBox.GetUpperBound(0)
            For j = 1 To fOutput.GetUpperBound(0)
                If (FBox(i).Type = fOutput(j).Type) And (fOutput(j).InContainer = False) Then
                    fOutput(j) = New Box(FBox(i))
                    fOutput(j).RelPos1 = New Point3D(FBox(i).RelPos1)
                    fOutput(j).InContainer = True
                    Exit For
                End If
            Next
        Next

        'recap data
        algRecapitulation(fInput, fListOutput)
        For i = 1 To fListOutput.GetUpperBound(0)
            fListOutput(i).SCount = 0
        Next
        For i = 1 To fOutput.GetUpperBound(0)
            For j = 1 To fListOutput.GetUpperBound(0)
                If fOutput(i).Type = fListOutput(j).SType Then
                    fListOutput(j).SCount += 1
                    Exit For
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' Filter frequency
    ''' </summary>
    Private Sub GetFilterFrequency(ByVal filterDepth As Single, ByRef tempDim() As Single)
        Dim i, j As Integer
        Dim cek(tempDim.GetUpperBound(0))

        'filtering
        For i = 1 To tempDim.GetUpperBound(0)
            If tempDim(i) <= filterDepth Then cek(i) = True
        Next

        'update data
        j = 0
        For i = 1 To tempDim.GetUpperBound(0)
            If (cek(i) = True) And (i <> j + 1) Then
                j += 1
                tempDim(j) = tempDim(i)
                procSwap(FFrequency1(i), FFrequency1(j))
                procSwap(FFrequency2(i), FFrequency2(j))
                procSwap(FFrequency3(i), FFrequency3(j))
            End If
        Next
        ReDim Preserve tempDim(j)
        ReDim Preserve FFrequency1(j)
        ReDim Preserve FFrequency2(j)
        ReDim Preserve FFrequency3(j)
    End Sub
End Class
