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
''' classStack.vb
'''
''' Input: Box, Number of Box, Emptyspace
''' Output: Coordinate of each box in emptyspace as wall form which has optimal value
''' 
''' Process:
''' - follow process of Gehring & Borfeldth
'''
''' </summary>
Public Class Stack
    Inherits Placement

    ''' <summary>
    ''' Best placement of stack building
    ''' </summary>
    Private fBox() As Box

    ''' <summary>
    ''' Limiting number of co-tower
    ''' </summary>
    Private fLimit As Single

    ''' <summary>
    ''' #New
    ''' -Default constructor wall
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Input data: space, box
    ''' --3. Recapitulation
    ''' </summary>
    Sub New(ByVal DSpace As Box, ByVal InputBox() As Box, ByVal limitDepth As Single)
        '(1)
        Dim i As Integer

        '(2)
        fVolume = 0
        fCompactness = 0
        fUtilization = 0
        fLimit = limitDepth

        fSpace = New Box(DSpace)
        ReDim fInput(InputBox.GetUpperBound(0))
        For i = 1 To InputBox.GetUpperBound(0)
            fInput(i) = New Box(InputBox(i))
        Next

        '(3)
        algRecapitulation(fInput, fListInput)
    End Sub


    ''' <summary>
    ''' #GetOutput
    ''' -Get output box as final result
    ''' ++
    ''' --1. Variable set
    ''' --2. Copy fInput --> fOutput
    ''' --3. Record detail-data
    ''' --4. Create boundingBox
    ''' --5. Recapitulation fOutput
    ''' --6. Calculate utilization + compactness + fitness
    ''' </summary>
    Private Sub GetOutput()
        '(1)
        Dim i, j As Integer

        '(2)
        procBoxClone(fInput, fOutput)

        '(3)
        For i = 1 To fBox.GetUpperBound(0)
            For j = 1 To fOutput.GetUpperBound(0)
                If (fBox(i).Type = fOutput(j).Type) And (fOutput(j).InContainer = False) Then
                    fOutput(j) = New Box(fBox(i))
                    fOutput(j).RelPos1 = New Point3D(fBox(i).AbsPos1)
                    fOutput(j).InContainer = True
                    Exit For
                End If
            Next
        Next

        '(4)
        fBoundingBox = GetBoundingStack(fBox)
        fBoundingBox.RelPos1 = New Point3D(fSpace.RelPos1)

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
        fVolume = 0
        For i = 1 To fBox.GetUpperBound(0)
            fVolume += fBox(i).Depth * fBox(i).Width * fBox(i).Height
        Next
        fUtilization = fVolume / (fSpace.Depth * fSpace.Width * fSpace.Height)
        fCompactness = fVolume / (fBoundingBox.Depth * fBoundingBox.Width * fSpace.Height)
    End Sub


    ''' <summary>
    ''' #GetOptimizeStack
    ''' -Get optimize all stack
    ''' -Finding the best one
    ''' -Based on Gehring & Bortfeldt algorithm
    ''' -Output: 1 tower contain largest volume on it.
    ''' ++
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get preliminary tower + sort tower
    ''' --3. Preparation variable for each iteration
    ''' --4. Create area plot3D --> each tower
    ''' --5. Fill tower: tempscore, temputil = 0
    ''' --6. Get best tower and placement (score and util)
    ''' --7. Get result to fBox
    ''' --8. Get output
    ''' </summary>
    Public Sub GetOptimizeStack()
        '(1)
        Dim i As Integer
        Dim tempScore, bestScore, tempUtil, bestUtil As Single
        Dim preTower(Nothing), freeBox(Nothing), tempBox(Nothing), bestBox(Nothing) As Box

        '(2)
        '(1)
        '//Set default value 0.1 --if percentM outside (0,1]
        If (fLimit <= 0) And (fLimit > 1) Then
            fLimit = 0.1
        End If

        preTower = GetPreTower(fInput, fSpace)
        SortTower(preTower)
        LimitTower(preTower, fLimit)

        '(3)
        Dim Tower(preTower.GetUpperBound(0))
        procBoxClone(fInput, freeBox)
        bestScore = 0
        For i = 1 To preTower.GetUpperBound(0)
            '(4)
            Tower(i) = New Plot3D(preTower(i).Depth, preTower(i).Width, fSpace.Height)

            '(5)
            '//Default percentM = 0.1
            tempScore = 0
            tempUtil = 0
            FillTower(preTower(i), _
                      1, _
                      Tower(i), _
                      freeBox, _
                      tempUtil, _
                      tempBox, _
                      fLimit)

            '(6)
            tempScore = GetScore(GetBoundingStack(tempBox))
            If (tempUtil > bestUtil) Or _
               ((tempUtil = bestUtil) And (tempScore > bestScore)) Then
                procBoxClone(tempBox, bestBox)
                bestUtil = tempUtil
                bestScore = tempScore
            End If
        Next

        '(7)
        procBoxClone(bestBox, fBox)

        '(8).
        If bestUtil > 0 Then GetOutput()
    End Sub


    ''' <summary>
    ''' #FillTower
    ''' -Fill the tower step-by-step
    ''' --0. Parameter set
    ''' --1. Add box + record position
    ''' ----. Modify: tempBox, addBox
    ''' --2. Revision freeBox
    ''' --3. Generate space 
    ''' --4. Sorting space from high-Z to low-Z --> data in pointer variable
    ''' --5. Select box + space
    ''' --6. Record bestTower
    ''' </summary>
    Private Sub FillTower(ByVal addBox As Box, _
                          ByVal pointerSpace As Integer, _
                          ByRef towerPack As Plot3D, _
                          ByVal freeBox() As Box, _
                          ByRef bestScore As Single, _
                          ByRef bestBox() As Box, _
                          ByVal percentM As Single)
        '//Preparation
        '(1)
        Dim tempBox(1) As Box
        tempBox(1) = New Box(addBox)
        tempBox(1).InContainer = True
        tempBox(1).RelPos1 = New Point3D(towerPack.Space(pointerSpace).RelPos1)

        addBox.RelPos1 = New Point3D(towerPack.Space(pointerSpace).RelPos1)

        '(2)
        GetRevisionFreeBox(addBox, freeBox)

        '(3)
        towerPack.InsertNewBoxes2(towerPack.Space(pointerSpace), _
                                  tempBox, _
                                  addBox)
        towerPack.GetSpace()


        '//Filling Tower
        Dim cek As Boolean = False
        If freeBox.GetUpperBound(0) > 0 Then
            '(4)
            Dim pSpace(towerPack.CountSpace) As Integer
            Dim volSpace(towerPack.CountSpace) As Single
            For i = 1 To towerPack.CountSpace
                volSpace(i) = towerPack.Space(i).Depth * towerPack.Space(i).Width * towerPack.Space(i).Height
                pSpace(i) = i
            Next
            If towerPack.CountSpace > 1 Then
                For i = 1 To towerPack.CountSpace - 1
                    For j = i + 1 To towerPack.CountSpace
                        If (towerPack.Space(pSpace(i)).AbsPos1.Z < towerPack.Space(pSpace(j)).AbsPos1.Z) Or _
                            ((towerPack.Space(pSpace(i)).AbsPos1.Z = towerPack.Space(pSpace(j)).AbsPos1.Z) And (volSpace(pSpace(i)) < volSpace(pSpace(j)))) Then
                            procSwap(volSpace(pSpace(i)), volSpace(pSpace(j)))
                            procSwap(pSpace(i), pSpace(j))
                        End If
                    Next
                Next
            End If

            '(5)
            Dim preTower(Nothing) As Box
            Dim tempTower As Plot3D
            For i = 1 To towerPack.CountSpace
                preTower = GetPreTower(freeBox, towerPack.Space(pSpace(i)))
                SortTower(preTower)
                LimitTower(preTower, percentM)

                If preTower.GetUpperBound(0) > 0 Then
                    For j = 1 To preTower.GetUpperBound(0)
                        tempTower = New Plot3D(towerPack)
                        FillTower(preTower(j), _
                                  pSpace(i), _
                                  tempTower, _
                                  freeBox, _
                                  bestScore, _
                                  bestBox, _
                                  percentM)
                    Next
                    cek = True
                    Exit For
                End If
            Next
        End If

        '(6)
        '//If no box can be placed in tower --> finish the solution
        If cek = False Then
            '//Save solution if better
            If towerPack.Utilization > bestScore Then
                ReDim bestBox(towerPack.Output.GetUpperBound(0))
                For i = 1 To towerPack.Output.GetUpperBound(0)
                    bestBox(i) = New Box(towerPack.Output(i))
                Next
                bestScore = towerPack.Utilization
            End If
        End If
    End Sub


    ''' <summary>
    ''' #GetPreTower
    ''' -Get preliminary tower
    ''' --0. Paramater set
    ''' --1. Variable set
    ''' --2. Prepare box
    ''' --3. Generate tower
    ''' ---3a. Set orientation box
    ''' ---3b. Filter feasibility in space
    ''' --4. Record data (if feasible) + resize data
    ''' --5. Return value
    ''' </summary>
    Private Function GetPreTower(ByVal inpBox() As Box, ByVal emptySpace As Box) As Box()
        '(1)
        Dim i, j, count As Integer
        Dim inpList(Nothing) As strBoxList
        algRecapitulation(inpBox, inpList)
        Dim boxList(inpList.GetUpperBound(0)), _
            preTow(6 * inpList.GetUpperBound(0)) As Box

        '(2)
        For i = 1 To inpList.GetUpperBound(0)
            For j = 1 To inpBox.GetUpperBound(0)
                If inpList(i).SType = inpBox(j).Type Then
                    boxList(i) = New Box(inpBox(j))
                    Exit For
                End If
            Next
        Next

        '(3)
        count = 0
        For i = 1 To boxList.GetUpperBound(0)
            For j = 1 To 6
                '//If meet rotation allow
                If (((j = 1) Or (j = 2)) And (boxList(i).RotAlpha = True)) Or _
                   (((j = 3) Or (j = 4)) And (boxList(i).RotBeta = True)) Or _
                   (((j = 5) Or (j = 6)) And (boxList(i).RotGamma = True)) Then
                    '(3a)
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
                    '(3b)
                    '//If box feasible in space
                    If (boxList(i).Depth <= emptySpace.Depth) And _
                        (boxList(i).Width <= emptySpace.Width) And _
                        (boxList(i).Height <= emptySpace.Height) Then

                        '(4)
                        '//Record data if feasible
                        count += 1
                        preTow(count) = New Box(boxList(i))
                    End If
                End If
            Next
        Next
        '//Resize preTower data
        ReDim Preserve preTow(count)

        '(5)
        '//Return value
        Return preTow
    End Function

    ''' <summary>
    ''' #GetBoundingStack
    ''' -Attention: This method uses absPosition variable to deterimine position
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Find longest position
    ''' --3. Create boundingbox
    ''' --4. Return value
    ''' </summary>
    Private Function GetBoundingStack(ByVal rawBox() As Box) As Box
        '(1)
        Dim i As Integer
        Dim boundBox As Box
        Dim longWidth, longDepth, longHeight As Single

        '(2)
        longWidth = 0 : longDepth = 0 : longHeight = 0
        For i = 1 To rawBox.GetUpperBound(0)
            With rawBox(i).AbsPos2
                If .X > longDepth Then longDepth = .X
                If .Y > longWidth Then longWidth = .Y
                If .Z > longHeight Then longHeight = .Z
            End With
        Next

        '(3)
        boundBox = New Box(-1, longDepth, longWidth, longHeight)
        boundBox.RelPos1 = New Point3D(0, 0, 0)

        '(4)
        Return boundBox
    End Function

    ''' <summary>
    ''' #GetRevisionUsedBox
    ''' -Fill the tower step-by-step
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Find reference box --> that want to be dump
    ''' --3. Revision freeBox
    ''' --4. Fix array size
    ''' </summary>
    Private Sub GetRevisionFreeBox(ByVal refBox As Box, ByRef freeBox() As Box)
        '(1)
        Dim i, j As Integer

        If freeBox.GetUpperBound(0) > 1 Then
            '(2)
            For i = 1 To freeBox.GetUpperBound(0)
                If refBox.Type = freeBox(i).Type Then Exit For
            Next

            '(3)
            If i < freeBox.GetUpperBound(0) Then
                For j = i + 1 To freeBox.GetUpperBound(0)
                    freeBox(j - 1) = New Box(freeBox(j))
                Next
            End If
        End If

        '(4)
        ReDim Preserve freeBox(freeBox.GetUpperBound(0) - 1)
    End Sub

    ''' <summary>
    ''' #sortTower
    ''' -Sort from large to smaller
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get volume
    ''' --3. Sorting process
    Private Sub SortTower(ByRef preTower() As Box)
        '(1)
        Dim volTower(preTower.GetUpperBound(0)) As Single
        Dim tempTower As Box

        '(2)
        For i = 1 To preTower.GetUpperBound(0)
            volTower(i) = preTower(i).Depth * preTower(i).Width * preTower(i).Height
        Next

        '(3)
        For i = 1 To preTower.GetUpperBound(0) - 1
            For j = i + 1 To preTower.GetUpperBound(0)
                If volTower(i) < volTower(j) Then
                    procSwap(volTower(i), volTower(j))
                    tempTower = New Box(preTower(i))
                    preTower(i) = New Box(preTower(j))
                    preTower(j) = New Box(tempTower)
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' #sortTower
    ''' -Sort from large to smaller
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get volume
    ''' --3. Find limit volume
    ''' --4. Limiting tower
    Private Sub LimitTower(ByRef preTower() As Box, ByVal percentM As Single)
        '(1)
        Dim i, j, nTargetVol As Integer
        Dim volTower(preTower.GetUpperBound(0)) As Single

        If preTower.GetUpperBound(0) > 0 Then
            '(2)
            j = 0
            For i = 1 To preTower.GetUpperBound(0)
                volTower(0) = preTower(i).Depth * preTower(i).Width * preTower(i).Height
                If (i = 1) Or (volTower(0) <> volTower(j)) Then
                    j += 1
                    volTower(j) = volTower(0)
                End If
            Next

            '(3)
            nTargetVol = CInt(j * percentM)
            If nTargetVol = 0 Then nTargetVol = 1

            j = preTower.GetUpperBound(0)
            For i = 1 To preTower.GetUpperBound(0)
                If (preTower(i).Depth * preTower(i).Width * preTower(i).Height < volTower(nTargetVol)) Then
                    If i - 1 > 0 Then
                        j = i - 1
                    Else
                        j = 1
                    End If
                    Exit For
                End If
            Next

            '(4)
            ReDim Preserve preTower(j)
        End If
        
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
    Private Function GetScore(ByVal BoundingBox As Box) As Single
        '(1)
        Dim cost1, cost2, benefit1, benefit2, ratio1, ratio2 As Single
        Dim cost3, benefit3, ratio3 As Single
        Dim ratioX As Single

        '(2)
        cost1 = (fSpace.Width - BoundingBox.Width) * BoundingBox.Depth
        benefit1 = (fSpace.Depth - BoundingBox.Depth) * fSpace.Width

        cost2 = (fSpace.Depth - BoundingBox.Depth) * BoundingBox.Width
        benefit2 = (fSpace.Width - BoundingBox.Width) * fSpace.Depth

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
        cost3 = BoundingBox.Depth * BoundingBox.Width
        benefit3 = (fSpace.Depth * fSpace.Width) - cost3
        If cost3 = 0 And benefit3 = 0 Then
            ratio3 = fSpace.Depth * fSpace.Width
        ElseIf cost3 = 0 Then
            ratio3 = benefit3
        Else
            ratio3 = benefit3 / cost3
        End If

        If (BoundingBox.Depth < BoundingBox.Width) And (BoundingBox.Depth > 0) And (BoundingBox.Width > 0) Then
            ratioX = BoundingBox.Depth / BoundingBox.Width
        ElseIf (BoundingBox.Depth > 0) And (BoundingBox.Width > 0) Then
            ratioX = BoundingBox.Width / BoundingBox.Depth
        Else
            ratioX = 1
        End If

        '(4)
        '//Best result ..haha.. --> I don't where's this come from
        Return (ratio1 + ratio2 + ratio3) * ((BoundingBox.Height / fSpace.Height) ^ 2) * ratioX

    End Function
End Class


' jangan lupa, ntar bikin langsung prosedur rekursif..
' ga susah koq, pokoknya limit rekursif selese saat ga ada lagi yang bisa ditumpuk karn ketinggian dan luas tumpukan melebihi tolerasnsi
' selain itu, variabel bisa diakses langsung  biar mudah...
' pada intinya nanti ada back tracking untuk ngumpulin hasilnya...
' paling enak ud disiapin, ranking dan urutan area zone untuk ditempati.. apakah oke ao ga...
' trus pas masang perlu ditempatin juga, penempatan tiap balok di area xone yang paling efektif, dan efisien
' apalagi ya... gw sebenernya juga masi bingung, mo coding tapi gw ud ngantuk..
' jangan lupa ke kedutaan, buat dapet visa...




'1.cek limit height ada ato ga... kl ada, berarti bisa masuk, kl ga, berarti ga masuk.
'2.cek fisibilitas ranking, ada yang fisibel ato ga... kl ada yang fisibel berarti lakukan packing
'3.proses packing dilakukan dengan mengambil box dari ranking yang paling besar terdahulu, kemudian satu-per-satu dimasukkan
'
' - yang masi membingungkan adalah bagaimana caranya supaya proses packing tidak rekursif ke belakang, tampaknya harus dibuat mekanisme control untuk mengatur prioritas antar prosedur
'
' 
'pengennya prosesnya kira2 seperti ini
'
'#cek apakah posisi sekarang masi ada balok yang ketinggiannya mencukupi (dibawah) ketinggian batas
'#bila cek=true
'   #re-fitness balok dengan kondisi area yang ada, cek fisibilitas dari masing2 balok
'   #sorting balok menurut fitness
'   #iterasi sampai balok tidak ada yang bisa masuk lagi
'       #packing tiap balok berdasarkan aturan (gunakan nilai packing fitness terbesar)
'   #generate area zone, berdasarkan prinsip maximal space --> mungkin ini yang okeh
'   #list area zone sehingga bisa digunakan
'       #cek sisi mana saja yang boleh tertoleransi, sisi mana yang tidak boleh tertoleransi
'   #rekapitulasi lagi, balok yang telah terpakai dan blon terpakai
'   #rekapitulasi urutan prioritas area zone yang akan dipacking
'   #iterasi panggilan prosedur untuk area zone yang telah diprioritaskan, mulai dari prioritas paling besar
'       #prioritas paling besar: paling rendah ketinggiannya, paling besar ukuran
'       #lanjutkan memanggil urutan yang lain


'''' <summary>
'''' Optimizing stacking
'''' </summary>
'Public Sub GetOptimizeStack()
'    'it's better working with tempBox
'    '1. looping until (nobox = true) or (noemptyspace = true)
'    '   --each looping
'    '   2. find ranking dimension for box
'    '   3. let layer class handling to filling box
'    '   4. update filled box, update rest box
'    '   5. generate empty-space + update empty-space

'    '#variable
'    Dim i, j, k As Integer

'    '#preparing step
'    'copy emptyspace to list + fdatalistoutput
'    Dim emptySpaceList(1) As Box
'    emptySpaceList(1) = New Box(-1, fSpace.Depth, fSpace.Width, fSpace.Height)

'    algRecapitulation(fInput, fListOutput)
'    For i = 1 To fListOutput.GetUpperBound(0)
'        fListOutput(i).SCount = 0
'    Next

'    'prepare cek as boolean for FInput controller
'    'default cek = false --> box(i) hasn't placed yet
'    Dim cek(fInput.GetUpperBound(0)) As Boolean

'    Do Until (emptySpaceList.GetUpperBound(0) = 0) Or (cek(0) = True)
'        '#update cek(0)
'        cek(0) = True
'        For i = 1 To cek.GetUpperBound(0)
'            If cek(i) = False Then
'                cek(0) = False
'                Exit For
'            End If
'        Next

'        '#update emptySpaceList
'        'cek(0) = false --> there's still others box
'        'check 1-by-1 fisibility emptySpace to fill at empty-Space
'        If cek(0) = False Then
'            For i = 1 To emptySpaceList.GetUpperBound(0)
'                For j = 1 To fListInput.GetUpperBound(0)
'                    'checking possibilities of emptyspace only if box.count > 0
'                    If fListOutput(j).SCount > 0 Then
'                        For k = 1 To fInput.GetUpperBound(0)
'                            If (fListInput(j).SType = fInput(k).Type) Then

'                            End If
'                        Next
'                    End If


'                Next
'            Next

'        End If
'    Loop

'End Sub

