Imports System

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
''' classWall.vb
'''
''' Input: Box, Number of Box, Emptyspace
''' Output: Coordinate of each box in emptyspace as wall form which has optimal value
''' 
''' Process:
''' - optimize wall --> by Pisinger algorithm
''' </summary>
Public Class Wall
    Inherits Placement

    ''' <summary>
    ''' Frequency 1 -- number of occurrences of dimension k among free-boxes (all dimension)
    ''' </summary>
    Private fFreq1() As FrequencyOccurence

    ''' <summary>
    ''' Frequency 2 -- number of occurrences of dimension k among free-box, considering only largest dimension
    ''' </summary>
    Private fFreq2() As FrequencyOccurence

    ''' <summary>
    ''' Frequency 3 -- number of occurrences of dimension k among free-box, considering only smallest dimension
    ''' </summary>
    Private fFreq3() As FrequencyOccurence

    ''' <summary>
    ''' Frequency Depth Prioritize
    ''' </summary>
    Private fMDepthPrior() As FrequencyOccurence

    ''' <summary>
    ''' Length depth of wall
    ''' </summary>
    Private fDepth As Single()

    ''' <summary>
    ''' Best placement of wall building
    ''' </summary>
    Private fBox() As Box

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
    ''' #New
    ''' -Default constructor wall
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Input data: space, box
    ''' --3. Recapitulation
    ''' </summary>
    Sub New(ByVal DSpace As Box, ByVal InputBox() As Box)
        '(1)
        Dim i As Integer

        fVolume = 0
        fCompactness = 0
        fUtilization = 0

        '(2)
        fSpace = New Box(DSpace)
        ReDim fInput(InputBox.GetUpperBound(0))
        For i = 1 To InputBox.GetUpperBound(0)
            fInput(i) = New Box(InputBox(i))
        Next

        '(3)
        algRecapitulation(fInput, fListInput)
    End Sub








    ''' <summary>
    ''' #GetOptimizeWall
    ''' -Wall optimization construction
    ''' -Construct by Pisinger heuristic model.
    ''' -Basic algorithm
    ''' -1.Choose depth of wall (based on prioritize function)
    ''' --Provide number of depth that can be used
    ''' -2.Fill wall (layer) by using strip-by-strip (horizontal / vertical)
    ''' -3.Optimization
    ''' --Calculating M1 variety of depth.
    ''' ++
    ''' --0. Parameter set --> default %dimension =  10%
    ''' --1. Variable set
    ''' --2. Update variety dimension
    ''' --3. Get 'depth' dimension (length)
    ''' --4. Start iteration --only fLength <= fSpace.depth
    ''' ---4a. Find alphaRatio
    ''' ---4b. Reset data
    ''' ---4c. Fills wall
    ''' ---4d. Calculate utilization
    ''' ---4e. Record best utilization
    ''' --5. Finalize result --> best placement + write output
    ''' --6. Get output
    ''' </summary>
    Public Sub GetOptimizeWall()
        '(1)
        Dim i, j As Integer
        Dim alphaR(Nothing) As AlphaRatio
        Dim utilBest, utilTemp, volBest, volTemp As Single
        Dim currentEmptySpace, tempPlacement(Nothing) As Box
        Dim originPoint As New Point3D(fSpace.RelPos1)

        '(2)
        Dim boxlist(fListInput.GetUpperBound(0)) As Box
        For i = 1 To fListInput.GetUpperBound(0)
            For j = 1 To fInput.GetUpperBound(0)
                If fInput(j).Type = fListInput(i).SType Then
                    boxlist(i) = New Box(fInput(j))
                    Exit For
                End If
            Next
        Next

        '(3)
        '//Set default %dimension to get
        '//Default = 10%
        fDepth = GetLengthDepth(fSpace.Depth, boxlist, 0.1)

        '(4)
        '//Iteration find best utilization
        utilBest = 0
        For i = 1 To fDepth.GetUpperBound(0)
            '//If flengthdepth <= femptyspace.depth --> Run
            If fDepth(i) <= fSpace.Depth Then
                '(4a)
                ReDim alphaR(boxlist.GetUpperBound(0))
                alphaR = GetAlphaRatio(boxlist, fDepth(i))

                '(4b)
                currentEmptySpace = New Box(-1, fDepth(i), fSpace.Width, fSpace.Height)
                ReDim tempPlacement(Nothing)

                '(4c)
                GetWall(currentEmptySpace, _
                        alphaR, _
                        fListInput, _
                        tempPlacement, _
                        0)

                '(4d)
                '//Calculate total volume box in wall
                utilTemp = 0
                volTemp = 0
                For j = 1 To tempPlacement.GetUpperBound(0)
                    volTemp += tempPlacement(j).Width * tempPlacement(j).Depth * tempPlacement(j).Height
                Next
                '//Calculate 
                utilTemp = volTemp / (currentEmptySpace.Width * currentEmptySpace.Depth * currentEmptySpace.Height)

                '(4e)
                If utilBest < utilTemp Then
                    volBest = volTemp
                    utilBest = utilTemp
                    procBoxClone(tempPlacement, fBox)
                End If
            End If
        Next

        '(5)
        fVolume = volBest

        '(6)
        If fVolume > 0 Then GetOutput()
    End Sub


    ''' <summary>
    ''' #GetFrequency
    ''' -Pisinger defines several frequency that used to select depth of wall
    ''' -Basic idea --> connect all dimension in box to catch them all
    '''
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get dimensions
    ''' --3. Resize and sort dimension
    ''' --4. Record frequency
    ''' </summary>
    Private Sub GetFrequency(ByVal boxList() As Box, _
                             ByRef tempDim() As Single, _
                             ByVal dataList() As strBoxList)
        '(1)
        Dim i, j, k, count As Integer
        Dim cek As Boolean
        Dim temp(3) As Single

        '(2)
        ReDim tempDim(boxList.GetUpperBound(0) * 3)
        count = 0
        For i = 1 To boxList.GetUpperBound(0)
            temp(1) = boxList(i).Dim1
            temp(2) = boxList(i).Dim2
            temp(3) = boxList(i).Dim3

            '//Cek dimension temp(i) to database
            For j = 1 To 3
                If temp(j) > 0 Then
                    '//Reset data, if cek = false --> there's no dimension on there
                    cek = False
                    For k = 1 To tempDim.GetUpperBound(0)
                        If temp(j) = tempDim(k) Then
                            cek = True
                            Exit For
                        End If
                    Next
                    '//Update data
                    If (cek = False) Then
                        count += 1
                        tempDim(count) = temp(j)
                    End If
                End If
            Next
        Next

        '(3)
        ReDim Preserve tempDim(count)
        ReDim fFreq1(count)
        ReDim fFreq2(count)
        ReDim fFreq3(count)
        '//Array sort (largest to smalesst)
        Array.Sort(tempDim)

        '(4)
        For i = 1 To tempDim.GetUpperBound(0)
            '//Reset and copy data
            fFreq1(i).Count = 0
            fFreq2(i).Count = 0
            fFreq3(i).Count = 0

            fFreq1(i).Dimension = tempDim(i)
            fFreq2(i).Dimension = tempDim(i)
            fFreq3(i).Dimension = tempDim(i)

            For j = 1 To boxList.GetUpperBound(0)
                '//Occurence is count if meet rotation constraint
                With boxList(j)
                    '//Function-1 occurence (if dimension exist and rotation meet)
                    If ((.Dim1 = tempDim(i)) And ((.RotBeta = True) Or (.RotGamma = True))) Or _
                        ((.Dim2 = tempDim(i)) And ((.RotAlpha = True) Or (.RotGamma = True))) Or _
                        ((.Dim3 = tempDim(i)) And ((.RotAlpha = True) Or (.RotBeta = True))) Then
                        fFreq1(i).Count += dataList(j).SCount
                    End If

                    '//Function-2 occurence (if max dimension)
                    If (fMax3(.Dim1, .Dim2, .Dim3) = tempDim(i)) And _
                        (((.Dim1 = tempDim(i)) And ((.RotBeta = True) Or (.RotGamma = True))) Or _
                        ((.Dim2 = tempDim(i)) And ((.RotAlpha = True) Or (.RotGamma = True))) Or _
                        ((.Dim3 = tempDim(i)) And ((.RotAlpha = True) Or (.RotBeta = True)))) Then
                        fFreq2(i).Count += dataList(j).SCount
                    End If

                    '//Function-3 occurence (if min dimension)
                    If (fMin3(.Dim1, .Dim2, .Dim3) = tempDim(i)) And _
                        (((.Dim1 = tempDim(i)) And ((.RotBeta = True) Or (.RotGamma = True))) Or _
                        ((.Dim2 = tempDim(i)) And ((.RotAlpha = True) Or (.RotGamma = True))) Or _
                        ((.Dim3 = tempDim(i)) And ((.RotAlpha = True) Or (.RotBeta = True)))) Then
                        fFreq3(i).Count += dataList(j).SCount
                    End If
                End With

                '--kalo uda ga dipake dibuang aja.. bikin bingung soalnya.
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
    ''' #GetLengthDepth
    ''' -Update frequency and depth of wall on current
    ''' --0. Parameter set
    ''' --1. Set default value M(%) --> take Depth as top (%) value
    ''' --2. Get frequency
    ''' --3. Get filter
    ''' --4. Get Rank
    ''' --5. Return value
    ''' </summary>
    Private Function GetLengthDepth(ByVal filterDepth As Single, _
                                    ByVal boxList() As Box, _
                                    ByVal percentM As Single) As Single()
        '(1)
        '//Set default value 0.1 --if percentM outside (0,1]
        If (0 >= percentM) And (percentM > 1) Then
            percentM = 0.1
        End If

        '(2)
        Dim varietyDimension(Nothing) As Single
        GetFrequency(boxList, varietyDimension, fListInput)

        '(3)
        GetFilterFrequency(filterDepth, varietyDimension)

        '(4)
        GetPrioritize(varietyDimension, percentM)

        '(5)
        Return varietyDimension
    End Function

    ''' <summary>
    ''' #GetPrioritize
    ''' -Calculate occurence into prioritize
    ''' --0. Parameter set
    ''' --1. Get number dimension
    ''' --2. Variable set
    ''' --3. Get prioritize1 (largest dimension)
    ''' --4. Get prioritize5 (largest frequency dimension)
    ''' --5. Recapitulation
    ''' --6. Update data
    ''' </summary>
    ''' <remarks>??? still confuse why using prioritize#1 and #5 only to determine --need to read paper again...</remarks>
    Private Sub GetPrioritize(ByRef tempDim() As Single, _
                              ByVal percentM As Single)
        '(1)
        Dim nTargetDimension As Integer = CInt(tempDim.GetUpperBound(0) * percentM)
        If nTargetDimension = 0 Then nTargetDimension = 1

        '(2)
        Dim i, j, count As Integer
        Dim priorityDimension(9, nTargetDimension) As Single
        Dim targetDim As Single
        Dim targetCount As Integer
        Dim cek(Nothing) As Boolean

        '(3)
        '//P1 = largest dimension
        count = 0
        ReDim cek(tempDim.GetUpperBound(0))
        Do Until count = nTargetDimension
            '//Reset from null
            targetDim = 0
            '//Find largest k-dimension
            For i = 1 To tempDim.GetUpperBound(0)
                If (targetDim < tempDim(i)) And (cek(i) = False) Then
                    targetDim = tempDim(i)
                    j = i
                End If
            Next
            '//Record data
            count += 1
            priorityDimension(1, count) = targetDim
            cek(j) = True
        Loop

        '(4)
        '//P5 = largest frequency dimension
        count = 0
        ReDim cek(tempDim.GetUpperBound(0))
        Do Until count = nTargetDimension
            '//Reset from null
            targetCount = 0
            '//Find largest k-dimension
            For i = 1 To fFreq1.GetUpperBound(0)
                If ((targetCount < fFreq1(i).Count) And (cek(i) = False)) Or _
                    ((targetCount = fFreq1(i).Count) And (cek(i) = False) And (targetDim < fFreq1(i).Dimension)) Then
                    targetDim = fFreq1(i).Dimension
                    targetCount = fFreq1(i).Count
                    j = i
                End If
            Next
            '//Record data
            count += 1
            priorityDimension(5, count) = fFreq1(j).Dimension
            cek(j) = True
        Loop

        '(5)
        '//??For temporary only??
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
        '//After recapitulation --> remove same dimension
        j = 0
        For i = 1 To tempDim.GetUpperBound(0)
            For j = 1 To tempDim.GetUpperBound(0)
                If (i <> j) And (tempDim(i) > 0) And (tempDim(i) = tempDim(j)) Then
                    tempDim(j) = 0
                End If
            Next
        Next

        '(6)
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
    ''' #GetAlphaRatio
    ''' -AlphaRatio (in Pisinger) --> how well box fills the depth of wall
    ''' -BetaRatio (in Pisinger) --> how well combination of someboxes fills the depth of wall\
    ''' -Rotation accomodated
    '''
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get alpha ratio
    ''' --3. Sort alpha ratio
    ''' --4. Return value
    ''' </summary>
    Private Function GetAlphaRatio(ByVal boxList() As Box, ByVal lengthDepth As Single) As AlphaRatio()
        '(1)
        Dim i, j, count As Integer
        '//Reset array size
        Dim tempAlpha(6 * boxList.GetUpperBound(0)) As AlphaRatio

        '(2)
        count = 0
        For i = 1 To boxList.GetUpperBound(0)
            For j = 1 To 6
                '//If meet rotation allow
                If (((j = 1) Or (j = 2)) And (boxList(i).RotAlpha = True)) Or _
                   (((j = 3) Or (j = 4)) And (boxList(i).RotBeta = True)) Or _
                   (((j = 5) Or (j = 6)) And (boxList(i).RotGamma = True)) Then

                    '//Set orientation box first
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

                    '//If box feasible in space
                    If (boxList(i).Depth <= lengthDepth) And _
                        (boxList(i).Width <= fSpace.Width) And _
                        (boxList(i).Height <= fSpace.Height) Then
                        '//Record data if feasible
                        count += 1
                        tempAlpha(count).Box = New Box(boxList(i))
                        tempAlpha(count).Ratio = boxList(i).Depth / lengthDepth
                    End If
                End If
            Next
        Next
        '//Resize alphaRatio data
        ReDim Preserve tempAlpha(count)

        '(3)
        For i = 1 To tempAlpha.GetUpperBound(0) - 1
            For j = i + 1 To tempAlpha.GetUpperBound(0)
                If (tempAlpha(i).Ratio < tempAlpha(j).Ratio) Or _
                    ((tempAlpha(i).Ratio = tempAlpha(j).Ratio) And _
                     ((tempAlpha(i).Box.Width * tempAlpha(i).Box.Depth * tempAlpha(i).Box.Height) < (tempAlpha(j).Box.Width * tempAlpha(j).Box.Depth * tempAlpha(j).Box.Height))) _
                     Then procSwap(tempAlpha(i), tempAlpha(j))
            Next
        Next

        '(4)
        Return tempAlpha
    End Function


    ''' <summary>
    ''' #GetLayer
    ''' -Recursive function to fill the wall with selected depth of wall
    ''' -Characteristic recursive:
    ''' --Define-well stopping rule
    ''' --Consumptive of resources
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Stopping rule procedure
    ''' --3. Continue (start) recursive (If stopping rule not fulfilled)
    ''' ---3a. Reset data
    ''' ---3b. Get available box
    ''' ---3c. Find max ratio as references (alphaMaxRatio --> references for merging box)
    ''' ---3d. Find alternatif ratio (use Join-Knapsack)
    ''' --4. If (maxRatio < bestRatio) --> select reference box
    ''' ---4a. Prepare variable for placing strip
    ''' ---4b. Get references box
    ''' ----4b1. Count references box
    ''' ----4b2. Set rel.pos (only for references box)
    ''' ----4b3. Calculate pointerVol  (??? don't know what for)
    ''' ---4c. Fill strip (horizontal/vertical)
    ''' ----4c1. Copy temp--> for comparation strip horizontal/vertical
    ''' ----4c2. Get strip
    ''' ----4c3. ??? Unknown things --> really don't know what for!!
    ''' ----4c4. Record value
    ''' ----4c5. Update empty space
    ''' ---4d. Update datalist
    ''' ---4e. Goto next strip
    ''' --5. If (maxRatio >= bestRatio) --> use alphaRatio directly
    ''' ---5a. Preparing place a strip (horizontal and vertical)
    ''' ---5b. Get strip
    ''' ----5b1. Reset data
    ''' ----5b2. Fix datalist due references box ??
    ''' ----5b3. Get strip
    ''' ----5b4. ??? Unknown things --> really don't what for!!
    ''' ----5b5. Record value
    ''' ----5b6. Update empty space
    ''' ---5c. Update datalist
    ''' ---5d. Goto next strip
    ''' </summary>
    Private Sub GetWall(ByVal currentEmptySpace As Box, _
                        ByVal alphaR() As AlphaRatio, _
                        ByVal dataList() As strBoxList, _
                        ByRef bestPlacement() As Box, _
                        ByVal lastLevel As Integer)
        '(1)
        Dim cek(alphaR.GetUpperBound(0)) As Boolean

        '(2)
        '//Check box availability
        cek(0) = CheckAvailBox(alphaR, _
                                dataList, _
                                New Point3D(currentEmptySpace.Depth, _
                                            currentEmptySpace.Width, _
                                            currentEmptySpace.Height))

        '(3)
        '//No available box (cek = false) --> there's no box that fit in space
        If cek(0) = True Then
            '(3a)
            Dim i, j, count As Integer
            Dim currentPointer(Nothing), bestPointer(Nothing) As Integer
            Dim bestEmptySpace As Box = Nothing
            Dim originPoint = New Point3D(currentEmptySpace.RelPos1)
            Dim tempNumber, pointerVol, bestRatio, maxRatio, maxHeight, maxWidth As Single
            Dim tempEmptySpace, boxPlacement(Nothing), tempBoxPlacement(Nothing) As Box
            Dim tempList(Nothing) As strBoxList
            count = bestPlacement.GetUpperBound(0)

            '(3b)
            cek = GetAvailBox(alphaR, _
                              dataList, _
                              New Point3D(currentEmptySpace.Depth, _
                                          currentEmptySpace.Width, _
                                          currentEmptySpace.Height))
            '(3c)
            '//Find maximum ratio
            maxRatio = 0
            For i = 1 To alphaR.GetUpperBound(0)
                If (maxRatio < alphaR(i).Ratio) And (cek(i) = True) Then maxRatio = alphaR(i).Ratio
            Next
            '(3d)
            '//Merge box to find alternative ratio (use join-knapsack)
            bestRatio = 0
            tempNumber = currentEmptySpace.Depth
            GetKnapsackJoinBox(currentEmptySpace, _
                               alphaR, _
                               dataList, _
                               0, _
                               currentPointer, _
                               tempNumber, _
                               bestRatio, _
                               bestPointer)

            '(4)
            If (maxRatio < bestRatio) Then
                '(4a)
                '//strip (horizontal, vertical) + update data list + placing reference box
                maxWidth = 0 : maxHeight = 0 : bestRatio = 0 : pointerVol = 0
                ReDim Preserve bestPlacement(count + bestPointer.GetUpperBound(0))

                '(4b)
                For i = 1 To bestPointer.GetUpperBound(0)
                    '(4b1)
                    If alphaR(bestPointer(i)).Box.Width > maxWidth Then maxWidth = alphaR(bestPointer(i)).Box.Width
                    If alphaR(bestPointer(i)).Box.Height > maxHeight Then maxHeight = alphaR(bestPointer(i)).Box.Height
                    For j = 1 To dataList.GetUpperBound(0)
                        If alphaR(bestPointer(i)).Box.Type = dataList(j).SType Then dataList(j).SCount -= 1
                    Next
                    '(4b2)
                    bestPlacement(count + i) = New Box(alphaR(bestPointer(i)).Box)
                    If i = 1 Then
                        bestPlacement(count + i).RelPos1 = New Point3D(originPoint)
                    Else
                        bestPlacement(count + i).RelPos1 = New Point3D(bestPlacement(count + i - 1).RelPos1.X + bestPlacement(count + i - 1).Depth, _
                                                                       originPoint.Y, _
                                                                       originPoint.Z)
                    End If
                    '(4b3)
                    pointerVol += alphaR(bestPointer(i)).Box.Width * _
                                  alphaR(bestPointer(i)).Box.Depth * _
                                  alphaR(bestPointer(i)).Box.Height
                Next

                '(4c)
                '//i=1 --> horizontal (false) / i=2 --> vertical (true)
                For i = 1 To 2
                    '(4c1)
                    '//Copy tempList --> for comparation which the best strip is.
                    ReDim tempList(dataList.GetUpperBound(0))
                    Array.Copy(dataList, tempList, dataList.Length)

                    '(4c2)
                    ReDim boxPlacement(Nothing)
                    If i = 1 Then
                        tempEmptySpace = New Box(-1, _
                                                 currentEmptySpace.Depth, _
                                                 currentEmptySpace.Width - maxWidth, _
                                                 maxHeight)
                        GetStrip(False, _
                                 tempEmptySpace, _
                                 alphaR, _
                                 tempList, _
                                 New Point3D(originPoint.X, _
                                             originPoint.Y + maxWidth, _
                                             originPoint.Z), _
                                 boxPlacement, _
                                 tempNumber)
                    Else
                        tempEmptySpace = New Box(-1, _
                                                 currentEmptySpace.Depth, _
                                                 maxWidth, _
                                                 currentEmptySpace.Height - maxHeight)
                        GetStrip(True, _
                                 tempEmptySpace, _
                                 alphaR, _
                                 tempList, _
                                 New Point3D(originPoint.X, _
                                             originPoint.Y, _
                                             originPoint.Z + maxHeight), _
                                 boxPlacement, _
                                 tempNumber)
                    End If

                    '(4c3)
                    '//tempNumber = reference vol + strip vol --> total number box that put in a strip
                    tempNumber += pointerVol

                    '//bestRatio ???  don't know what for
                    If bestRatio < tempNumber Then
                        '(4c4)
                        '//Record best value
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
                        '(4c5)
                        '//i=1 (horizontal) --> change height : i=2 (vertical) --> change width
                        If i = 1 Then
                            bestEmptySpace = New Box(-1, _
                                                     currentEmptySpace.Depth, _
                                                     currentEmptySpace.Width, _
                                                     currentEmptySpace.Height - maxHeight)
                            bestEmptySpace.RelPos1 = New Point3D(originPoint.X, _
                                                                 originPoint.Y, _
                                                                 originPoint.Z + maxHeight)
                        Else
                            bestEmptySpace = New Box(-1, _
                                                     currentEmptySpace.Depth, _
                                                     currentEmptySpace.Width - maxWidth, _
                                                     currentEmptySpace.Height)
                            bestEmptySpace.RelPos1 = New Point3D(originPoint.X, _
                                                                 originPoint.Y + maxWidth, _
                                                                 originPoint.Z)
                        End If
                    End If
                Next

                '//Finishing
                '(4d)
                For i = count + bestPointer.GetUpperBound(0) + 1 To bestPlacement.GetUpperBound(0)
                    For j = 1 To dataList.GetUpperBound(0)
                        If bestPlacement(i).Type = dataList(j).SType Then
                            dataList(j).SCount -= 1
                            Exit For
                        End If
                    Next
                Next
                '(4e)
                GetWall(bestEmptySpace, _
                        alphaR, _
                        dataList, _
                        bestPlacement, _
                        lastLevel + 1)
            Else
                '(5)
                '//If maxRatio >= bestRatio
                '//Trial all ratio
                bestRatio = 0
                For j = 1 To alphaR.GetUpperBound(0)
                    If (alphaR(j).Ratio = maxRatio) And (cek(j) = True) Then
                        Console.Write(j & "-")
                        '(5a)
                        '//Set data
                        maxWidth = alphaR(j).Box.Width
                        maxHeight = alphaR(j).Box.Height
                        '(5b)
                        For i = 1 To 2
                            '(5b1)
                            ReDim tempList(dataList.GetUpperBound(0))
                            Array.Copy(dataList, tempList, dataList.Length)
                            '(5b2)
                            For k = 1 To tempList.GetUpperBound(0)
                                If tempList(k).SType = alphaR(j).Box.Type Then tempList(k).SCount -= 1
                            Next
                            '(5b3)
                            ReDim boxPlacement(Nothing)
                            If i = 1 Then
                                tempEmptySpace = New Box(-1, _
                                                         currentEmptySpace.Depth, _
                                                         currentEmptySpace.Width - maxWidth, _
                                                         maxHeight)
                                GetStrip(False, _
                                         tempEmptySpace, _
                                         alphaR, _
                                         tempList, _
                                         New Point3D(originPoint.X, _
                                                     originPoint.Y + maxWidth, _
                                                     originPoint.Z), _
                                         boxPlacement, _
                                         tempNumber)
                            Else
                                tempEmptySpace = New Box(-1, _
                                                         currentEmptySpace.Depth, _
                                                         maxWidth, _
                                                         currentEmptySpace.Height - maxHeight)
                                GetStrip(True, _
                                         tempEmptySpace, _
                                         alphaR, _
                                         tempList, _
                                         New Point3D(originPoint.X, _
                                                     originPoint.Y, _
                                                     originPoint.Z + maxHeight), _
                                         boxPlacement, _
                                         tempNumber)
                            End If

                            '(5b4)
                            '//??? what for --> i don't understand
                            '//tempNumber = reference vol + strip vol
                            tempNumber += alphaR(j).Box.Width * alphaR(j).Box.Depth * alphaR(j).Box.Height

                            '?? what for (bestRatio < tempNumber)???
                            If bestRatio < tempNumber Then
                                '//Reset data
                                maxHeight = 0 : maxWidth = 0
                                '(5b5)
                                '//Record data
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

                                '(5b6)
                                'i = 1 --> horizontal strip.. change height
                                'i = 2 --> vertical strip.. change width
                                If i = 1 Then
                                    bestEmptySpace = New Box(-1, _
                                                             currentEmptySpace.Depth, _
                                                             currentEmptySpace.Width, _
                                                             currentEmptySpace.Height - maxHeight)
                                    bestEmptySpace.RelPos1 = New Point3D(originPoint.X, _
                                                                         originPoint.Y, _
                                                                         originPoint.Z + maxHeight)
                                Else
                                    bestEmptySpace = New Box(-1, _
                                                             currentEmptySpace.Depth, _
                                                             currentEmptySpace.Width - maxWidth, _
                                                             currentEmptySpace.Height)
                                    bestEmptySpace.RelPos1 = New Point3D(originPoint.X, _
                                                                         originPoint.Y + maxWidth, _
                                                                         originPoint.Z)
                                End If
                            End If
                        Next

                    End If
                Next
                '//Finishing
                '(5c)
                For i = count + 1 To bestPlacement.GetUpperBound(0)
                    For j = 1 To dataList.GetUpperBound(0)
                        If bestPlacement(i).Type = dataList(j).SType Then
                            dataList(j).SCount -= 1
                            Exit For
                        End If
                    Next
                Next
                '(5d)
                GetWall(bestEmptySpace, alphaR, dataList, bestPlacement, lastLevel + 1)
            End If
        End If
    End Sub

    ''' <summary>
    ''' #GetKnapsack
    ''' -Join some boxes to improve alpha ratio
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Stopping rules
    ''' ---2a. No box available
    ''' ---2b. maxRatio reach maximum
    ''' --3. If recursive continue (only stopping rule not satisfy)
    ''' ---3a. Get available box
    ''' ---3b. For each available box
    ''' ---3b1. -1 record data
    ''' ---3b2. Update current pointer
    ''' ---3b3. Jump into recursive
    ''' ---3b4. +1 record data (after rekursif back --> data must be back to normal)
    ''' --4. If recursive stop (stopping rule satisfy)
    ''' ---4a. Prepare storing variable
    ''' ---4b. Calculate best ratio
    ''' ---4c. Record best ratio
    ''' </summary>
    Private Sub GetKnapsackJoinBox(ByVal currentEmptySpace As Box, _
                                   ByVal alphaR() As AlphaRatio, _
                                   ByVal dataList() As strBoxList, _
                                   ByVal lastLevel As Integer, _
                                   ByVal currentPointer() As Integer, _
                                   ByVal resDepth As Single, _
                                   ByRef bestRatio As Single, _
                                   ByRef bestPointer() As Integer)
        '(1)
        Dim i, j As Integer
        Dim cek(0) As Boolean

        '(2)
        '#Stopping rule
        '(2a)
        cek(0) = CheckAvailBox(alphaR, _
                               dataList, _
                               New Point3D(resDepth, _
                                           currentEmptySpace.Width, _
                                           currentEmptySpace.Height))
        '(2b)
        If (cek(0) = True) And (bestRatio = 1) Then
            cek(0) = False
        End If

        '(3)
        If cek(0) = True Then
            '(3a)
            cek = GetAvailBox(alphaR, _
                              dataList, _
                              New Point3D(resDepth, _
                                          currentEmptySpace.Width, _
                                          currentEmptySpace.Height))
            '(3b)
            For i = 1 To alphaR.GetUpperBound(0)
                '(3b1)
                If (cek(i) = True) And (bestRatio < 1) Then
                    For j = 1 To dataList.GetUpperBound(0)
                        If (dataList(j).SType = alphaR(i).Box.Type) Then
                            dataList(j).SCount -= 1
                            Exit For
                        End If
                    Next
                    '(3b2)
                    If (currentPointer.GetUpperBound(0) = lastLevel) Then ReDim Preserve currentPointer(lastLevel + 1)
                    currentPointer(lastLevel + 1) = i
                    '(3b3)
                    GetKnapsackJoinBox(currentEmptySpace, _
                                       alphaR, _
                                       dataList, _
                                       lastLevel + 1, _
                                       currentPointer, _
                                       (resDepth - alphaR(i).Box.Depth), bestRatio, bestPointer)
                    '(3b4)
                    dataList(j).SCount += 1
                End If
            Next
        Else
            '(4)
            '(4a)
            Dim maxWidth, maxHeight, currentRatio As Single
            currentRatio = 0 : maxHeight = 0 : maxWidth = 0
            '(4b)
            For i = 1 To lastLevel
                currentRatio += (alphaR(currentPointer(i)).Box.Width * _
                                 alphaR(currentPointer(i)).Box.Height * _
                                 alphaR(currentPointer(i)).Box.Depth)
                If maxWidth < alphaR(currentPointer(i)).Box.Width Then maxWidth = alphaR(currentPointer(i)).Box.Width
                If maxHeight < alphaR(currentPointer(i)).Box.Height Then maxHeight = alphaR(currentPointer(i)).Box.Height
            Next
            currentRatio = currentRatio / (maxWidth * maxHeight * currentEmptySpace.Depth)
            '(4c)
            If (currentRatio > bestRatio) Then
                bestRatio = currentRatio
                ReDim bestPointer(lastLevel)
                For i = 1 To lastLevel
                    bestPointer(i) = currentPointer(i)
                Next

                '//Return value --> number of last pointer
                bestPointer(0) = lastLevel
            End If
        End If
    End Sub

    ''' <summary>
    ''' #GetStrip
    ''' -Fill a strip (horizontal/vertical)
    ''' -Boolean val: true = vertical, false = horizontal
    ''' +
    ''' ?? !some bug (couldbe)
    ''' ?? what: there is no checking box availability, after 'join knapsack'
    ''' ?? effect: might error if the process continue without free-box
    ''' ?? where: see sign
    ''' +
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Reset 'global' data 
    ''' --3. Iteration (until cek = false)
    ''' ---3a. Reset 'iteration' data: ratio, resDepth
    ''' ---3b. Find maximal join box
    ''' ---3c. Find coordinate join box
    ''' ---3d. Record coordinate + get max dimension + update datalist
    ''' ---3e. Set next space
    ''' ---3f. Find more box that able put in empty space
    ''' --4. Get utilization
    ''' </summary>
    Private Sub GetStrip(ByVal direction As Boolean, _
                         ByVal currentEmptySpace As Box, _
                         ByVal alphaR() As AlphaRatio, _
                         ByVal dataList() As strBoxList, _
                         ByVal originPoint As Point3D, _
                         ByRef boxPlacement() As Box, _
                         ByRef volTotal As Single)
        '(1)
        Dim cek As Boolean = True
        Dim pointerCoordinate(Nothing) As Point3D
        Dim pointerBestSolution(alphaR.GetUpperBound(0)), currentPointer(Nothing), _
            bestPointer(Nothing), count As Integer
        Dim bestRatio, tempResDepth, _
            wEmpty, dEmpty, hEmpty, _
            maxHeight, maxWidth As Single

        '(2)
        wEmpty = currentEmptySpace.Width
        dEmpty = currentEmptySpace.Depth
        hEmpty = currentEmptySpace.Height

        '(3)
        '?? potency bug1: no check available box
        Do Until cek = False
            '(3a)
            bestRatio = 0
            tempResDepth = currentEmptySpace.Depth
            '(3b)
            GetKnapsackJoinBox(currentEmptySpace, _
                               alphaR, _
                               dataList, _
                               0, _
                               currentPointer, _
                               tempResDepth, _
                               bestRatio, _
                               bestPointer)
            '(3c)
            '?? potency bug2: continuing process without know box availability
            ReDim pointerCoordinate(bestPointer(0))
            For i = 1 To bestPointer(0)
                If i = 1 Then
                    pointerCoordinate(i) = New Point3D(originPoint.X, _
                                                       originPoint.Y, _
                                                       originPoint.Z)
                Else
                    pointerCoordinate(i) = New Point3D(pointerCoordinate(i - 1).X + alphaR(bestPointer(i - 1)).Box.Depth, _
                                                       originPoint.Y, _
                                                       originPoint.Z)
                End If
            Next
            '(3d)
            '//Reset data
            count = boxPlacement.GetUpperBound(0)
            maxHeight = 0 : maxWidth = 0
            ReDim Preserve boxPlacement(boxPlacement.GetUpperBound(0) + bestPointer(0))
            For i = 1 To bestPointer(0)
                '//Record coord
                count += 1
                boxPlacement(count) = New Box(alphaR(bestPointer(i)).Box)
                boxPlacement(count).RelPos1 = New Point3D(pointerCoordinate(i))
                '//Get max dimension
                If maxHeight < alphaR(bestPointer(i)).Box.Height Then maxHeight = alphaR(bestPointer(i)).Box.Height
                If maxWidth < alphaR(bestPointer(i)).Box.Width Then maxWidth = alphaR(bestPointer(i)).Box.Width
                '//Update datalist
                For j = 1 To dataList.GetUpperBound(0)
                    If alphaR(bestPointer(i)).Box.Type = dataList(j).SType Then
                        dataList(j).SCount -= 1
                        Exit For
                    End If
                Next
            Next
            '(3e)
            '//True = vertical --> space = wfix, hres, dfix
            '//False = horizontal --> space = wres, hfix, dfix
            If direction = True Then
                hEmpty -= maxHeight
                originPoint.Z += maxHeight
            Else
                wEmpty -= maxWidth
                originPoint.Y += maxWidth
            End If
            currentEmptySpace = New Box(-1, dEmpty, wEmpty, hEmpty)
            '(3f)
            cek = CheckAvailBox(alphaR, _
                                dataList, _
                                New Point3D(dEmpty, wEmpty, hEmpty))
        Loop

        '(4)
        volTotal = 0
        For i = 1 To boxPlacement.GetUpperBound(0)
            volTotal += boxPlacement(i).Depth * boxPlacement(i).Width * boxPlacement(i).Height
        Next
    End Sub

    ''' <summary>
    ''' #CheckAvailabilityBox function
    ''' -Cek = TRUE --> at least a box available
    ''' -Cek = FALSE --> no available box 
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Check availability
    ''' --3. Return value
    ''' </summary>
    Private Function CheckAvailBox(ByVal alphaR() As AlphaRatio, _
                                   ByVal dataList() As strBoxList, _
                                   ByVal emptySpaceDim As Point3D) As Boolean
        '(1)
        Dim cek As Boolean = False

        '(2)
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

        '(3)
        Return cek
    End Function

    ''' <summary>
    ''' #GetAvailBox
    ''' -Get box that available to fill-in
    ''' --0. Parameter set: alphaRatio, listBox, space
    ''' --1. Variable set
    ''' --2. Get available box --using array boolean
    ''' --3. Return value
    ''' </summary>
    Private Function GetAvailBox(ByVal alphaR() As AlphaRatio, _
                                 ByVal dataList() As strBoxList, _
                                 ByVal emptySpaceDim As Point3D) As Boolean()
        '(1)
        Dim cek(alphaR.GetUpperBound(0)) As Boolean

        '(2)
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

        '(3)
        Return cek
    End Function

    ''' <summary>
    ''' #GetOutput
    ''' -Finalize arrangement
    ''' --1. Variable set
    ''' --2. Get output box (by cloning it from input box)
    ''' --3. Get container coordinate + bounding box
    ''' --4. Revision cloning box --only for box in wall only
    ''' --5. Recapitulation result
    ''' --6. Calculate compactness + utilization
    ''' </summary>
    Private Sub GetOutput()
        '(1)
        Dim i As Integer
        Dim minPoint, maxPoint As Point3D

        '(2)
        ReDim fOutput(fInput.GetUpperBound(0))
        For i = 1 To fInput.GetUpperBound(0)
            fOutput(i) = New Box(fInput(i))
        Next

        '(3)
        minPoint = New Point3D(fSpace.RelPos2)
        maxPoint = New Point3D(fSpace.RelPos1)
        For i = 1 To fBox.GetUpperBound(0)
            fBox(i).RelPos1 = New Point3D(fSpace.RelPos1.X + fBox(i).RelPos1.X, _
                                                    fSpace.RelPos1.Y + fBox(i).RelPos1.Y, _
                                                    fSpace.RelPos1.Z + fBox(i).RelPos1.Z)

            If fBox(i).RelPos1.X < minPoint.X Then minPoint.X = fBox(i).RelPos1.X
            If fBox(i).RelPos1.Y < minPoint.Y Then minPoint.Y = fBox(i).RelPos1.Y
            If fBox(i).RelPos1.Z < minPoint.Z Then minPoint.Z = fBox(i).RelPos1.Z
            If fBox(i).RelPos2.X > maxPoint.X Then maxPoint.X = fBox(i).RelPos2.X
            If fBox(i).RelPos2.Y > maxPoint.Y Then maxPoint.Y = fBox(i).RelPos2.Y
            If fBox(i).RelPos2.Z > maxPoint.Z Then maxPoint.Z = fBox(i).RelPos2.Z
        Next
        fBoundingBox = New Box(-1, maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y, maxPoint.Z - minPoint.Z)
        fBoundingBox.RelPos1 = New Point3D(minPoint)

        '(4)
        For i = 1 To fBox.GetUpperBound(0)
            For j = 1 To fOutput.GetUpperBound(0)
                If (fBox(i).Type = fOutput(j).Type) And (fOutput(j).InContainer = False) Then
                    fOutput(j) = New Box(fBox(i))
                    fOutput(j).RelPos1 = New Point3D(fBox(i).RelPos1)
                    fOutput(j).InContainer = True
                    Exit For
                End If
            Next
        Next

        '(5)
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

        '(6)
        fUtilization = fVolume / (fSpace.Depth * fSpace.Width * fSpace.Height)
        fCompactness = fVolume / (fBoundingBox.Depth * fBoundingBox.Width * fSpace.Height)
    End Sub

    ''' <summary>
    ''' #GetFilter Frequency
    ''' -Filtering
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Filtering procedure: depth-box < depth-container
    ''' --3. Sort data
    ''' --4. Update variable-length 
    ''' </summary>
    Private Sub GetFilterFrequency(ByVal filterDepth As Single, _
                                   ByRef tempDim() As Single)
        '(1)
        Dim i, j As Integer
        Dim cek(tempDim.GetUpperBound(0))

        '(2)
        For i = 1 To tempDim.GetUpperBound(0)
            cek(i) = False
            If tempDim(i) <= filterDepth Then cek(i) = True
        Next

        '(3)
        j = 0
        'And (i <> j + 1) 
        For i = 1 To tempDim.GetUpperBound(0)
            If (cek(i) = True) Then
                j += 1
                tempDim(j) = tempDim(i)
                procSwap(fFreq1(i), fFreq1(j))
                procSwap(fFreq2(i), fFreq2(j))
                procSwap(fFreq3(i), fFreq3(j))
            End If
        Next
        ReDim Preserve tempDim(j)
        ReDim Preserve fFreq1(j)
        ReDim Preserve fFreq2(j)
        ReDim Preserve fFreq3(j)
    End Sub
End Class