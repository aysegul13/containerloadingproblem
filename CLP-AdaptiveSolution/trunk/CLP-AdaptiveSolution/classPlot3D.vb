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
''' classPlot3D.vb
'''
''' </summary>

Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Class Plot3D
    
    ''' <summary>
    ''' #New
    ''' -Default constructor
    ''' --0. Parameter set: container dimension
    ''' --1. Set initial container
    ''' --2. Set initial data
    ''' --3. Set first space
    ''' </summary>
    Sub New(ByVal dCon As Single, ByVal wCon As Single, ByVal hCon As Single)
        '(1)
        fInitialSpace = New Point3D(dCon, wCon, hCon)

        '(2)
        '//Initial data --> container (pointer-0)
        ReDim fBox(0)
        fBox(0) = New Box(-1, dCon, wCon, hCon)

        '(3)
        '//Make first space box
        ReDim fSpaceBox(1)
        fSpaceBox(1) = New Box(0, dCon, wCon, hCon)
        fSpaceBox(1).AbsPos1 = New Point3D(0, 0, 0)
        fSpaceBox(1).RelPos1 = New Point3D(0, 0, 0)
        '//Make first space arena
        ReDim fSpaceArea(1)
        fSpaceArea(1) = New MaximalSpace(fSpaceBox(1))
    End Sub

    ''' <summary>
    ''' #New
    ''' -Clone constructor
    ''' </summary>
    Sub New(ByVal masterPlot3D As Plot3D)
        Dim i As Integer

        '//BoundingBox
        Try
            procBoxClone(masterPlot3D.fBoundingBox, fBoundingBox)
        Catch ex As Exception
        End Try

        '//Box
        Try
            procBoxClone(masterPlot3D.fBox, fBox)
        Catch ex As Exception
        End Try

        '//Colission
        fCollision = masterPlot3D.fCollision

        '//InitialSpace
        fInitialSpace = New Point3D(masterPlot3D.fInitialSpace)

        '//OutContainer
        fOutContainer = masterPlot3D.fOutContainer

        '//SpaceArea
        ReDim fSpaceArea(masterPlot3D.fSpaceArea.GetUpperBound(0))
        For i = 1 To fSpaceArea.GetUpperBound(0)
            fSpaceArea(i) = New MaximalSpace(masterPlot3D.fSpaceArea(i))
        Next

        '//SpaceBox
        Try
            procBoxClone(masterPlot3D.fSpaceBox, fSpaceBox)
        Catch ex As Exception
        End Try

    End Sub


    ''' <summary>
    ''' #InsertNewBoxes1
    ''' -If input new box added to 3D-space
    ''' -Include output-mechanism
    ''' -Use UPDATESPACE #1
    ''' --0. Paramater set
    ''' --1. Variable set
    ''' --2. Separating input-output box; inputbox --> incontainer, outbox --> not incontainer
    ''' --3. Update position in container: box + bounding box
    ''' --4. Insert new boxes
    ''' ---4a. Adding new fBox (from inputBox)
    ''' ---4b. Adding new boundingBox
    ''' --5. Update space
    ''' </summary>
    Public Sub InsertNewBoxes1(ByVal emptySpace As Box, _
                         ByVal inputBox() As Box, _
                         ByVal boundingBox As Box, _
                         ByRef outputBox() As Box)
        '(1)
        Dim i, j As Integer
        '(2)
        GetSeparateBox(inputBox, outputBox)
        '(3)
        UpdatePosCont(New Point3D(emptySpace.AbsPos1.X, emptySpace.AbsPos1.Y, emptySpace.AbsPos1.Z), inputBox)
        boundingBox.AbsPos1 = New Point3D(boundingBox.AbsPos1.X + emptySpace.AbsPos1.X, _
                                          boundingBox.AbsPos1.Y + emptySpace.AbsPos1.Y, _
                                          boundingBox.AbsPos1.Z + emptySpace.AbsPos1.Z)
        '(4)
        '//Preparation
        j = fBox.GetUpperBound(0)
        ReDim Preserve fBox(fBox.GetUpperBound(0) + inputBox.GetUpperBound(0))
        ReDim Preserve fBoundingBox(fBoundingBox.GetUpperBound(0) + 1)
        '(4a)
        For i = 1 To inputBox.GetUpperBound(0)
            fBox(j + i) = New Box(inputBox(i))
        Next
        '(4b)
        fBoundingBox(fBoundingBox.GetUpperBound(0)) = New Box(boundingBox)

        '(5)
        UpdateSpace1(inputBox, boundingBox)
    End Sub

    ''' <summary>
    ''' #InsertNewBoxes1
    ''' -If input new box added to 3D-space
    ''' -Include output-mechanism
    ''' -Use UPDATESPACE #1
    ''' --0. Paramater set
    ''' --1. Variable set
    ''' --2. Separating input-output box; inputbox --> incontainer, outbox --> not incontainer
    ''' --3. Update position in container: box + bounding box
    ''' --4. Insert new boxes
    ''' ---4a. Adding new fBox (from inputBox)
    ''' ---4b. Adding new boundingBox
    ''' --5. Update space
    ''' </summary>
    Public Sub InsertNewBoxes2(ByVal emptySpace As Box, _
                         ByVal inputBox() As Box, _
                         ByVal boundingBox As Box, _
                         ByRef outputBox() As Box)
        '(1)
        Dim i, j As Integer
        '(2)
        GetSeparateBox(inputBox, outputBox)
        '(3)
        UpdatePosCont(New Point3D(emptySpace.AbsPos1.X, emptySpace.AbsPos1.Y, emptySpace.AbsPos1.Z), inputBox)
        boundingBox.AbsPos1 = New Point3D(boundingBox.AbsPos1.X + emptySpace.AbsPos1.X, _
                                          boundingBox.AbsPos1.Y + emptySpace.AbsPos1.Y, _
                                          boundingBox.AbsPos1.Z + emptySpace.AbsPos1.Z)
        '(4)
        '//Preparation
        j = fBox.GetUpperBound(0)
        ReDim Preserve fBox(fBox.GetUpperBound(0) + inputBox.GetUpperBound(0))
        ReDim Preserve fBoundingBox(fBoundingBox.GetUpperBound(0) + 1)
        '(4a)
        For i = 1 To inputBox.GetUpperBound(0)
            fBox(j + i) = New Box(inputBox(i))
        Next
        '(4b)
        fBoundingBox(fBoundingBox.GetUpperBound(0)) = New Box(boundingBox)

        '(5)
        UpdateSpace2(inputBox, boundingBox)
    End Sub


    ''' <summary>
    ''' #InsertNewBoxes 3
    ''' -If input new box added to 3D-space
    ''' -exclude output-mechanism
    ''' -Use UPDATESPACE #2
    ''' --0. Paramater set
    ''' --1. Variable set
    ''' --2. Separating input-output box; inputbox --> incontainer, outbox --> not incontainer
    ''' --3. Update position in container: box + bounding box
    ''' --4. Insert new boxes
    ''' ---4a. Adding new fBox (from inputBox)
    ''' ---4b. Adding new boundingBox
    ''' --5. Update space
    ''' </summary>
    Public Sub InsertNewBoxes3(ByVal emptySpace As Box, _
                               ByVal insertBox() As Box, _
                               ByVal boundingBox As Box)
        '(1)
        Dim i, j As Integer

        '(2)
        UpdatePosCont(New Point3D(emptySpace.AbsPos1.X, emptySpace.AbsPos1.Y, emptySpace.AbsPos1.Z), insertBox)
        boundingBox.AbsPos1 = New Point3D(boundingBox.AbsPos1.X + emptySpace.AbsPos1.X, _
                                          boundingBox.AbsPos1.Y + emptySpace.AbsPos1.Y, _
                                          boundingBox.AbsPos1.Z + emptySpace.AbsPos1.Z)
        '(4)
        '//Preparation
        j = fBox.GetUpperBound(0)
        ReDim Preserve fBox(fBox.GetUpperBound(0) + insertBox.GetUpperBound(0))
        ReDim Preserve fBoundingBox(fBoundingBox.GetUpperBound(0) + 1)
        '(4a)
        For i = 1 To insertBox.GetUpperBound(0)
            fBox(j + i) = New Box(insertBox(i))
        Next
        '(4b)
        fBoundingBox(fBoundingBox.GetUpperBound(0)) = New Box(boundingBox)

        '(5)
        UpdateSpace2(insertBox, boundingBox)
    End Sub



    ''' <summary>
    ''' #Update space
    ''' -Renew space due to added boxes
    ''' -Generate new space
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Grouping inputBox depending on height
    ''' --3. Build new contour
    ''' --4. Merge new-old contour
    ''' --5. Revise old contour
    ''' </summary>
    ''' <remarks>
    ''' Explanation:
    ''' For determine spacial condition (after box(es) added), we use following algorithm:
    ''' 1. Mapping 2D-area based on height of boxes
    '''   -diketahui box yang berada pada ketinggian nol
    '''   -diketahui box dan spacial pada ketinggian tertentu; paling enak sih sebenernya kalo pas pertama aja identifikasinya
    '''   ++logika sederhana
    '''   ---saat pemetaan maka terbentuk 2 spacial: 1 spacial pada ketinggian Zmin, dan 1 spacial pada ketinggian Zmax
    '''   ---spacial pada ketinggian tersebut dijadikan acuan
    '''   ---yang masalah kalo spacial pada ketinggian yang sama cara nyatuinnya gimana?
    '''   ---bisa diketahui dari empty space yang digunakan; jadi bisa ditrace itu masuk ke area2D yang mana
    '''   ---ok.. tapi ngasi taunya itu cuboid sehingga perlu bisa digabung gimana ya?
    ''' - kalo asal jadi bisa sih.. tinggal masukkin input cuboid, aja... biar ga bingung...
    ''' - jadi pas akhirnya dibuat cuboid box aja
    '''
    ''' 2. Placement as usual in 2D space
    '''   -ini perlu perubahan kalo misalnya box mengakomodasi adanya toleransi
    '''
    ''' 3. Generate empty space and update array
    '''</remarks>
    Private Sub UpdateSpace1(ByVal inputBox() As Box, _
                             ByVal boundingBox As Box)
        '(1)
        Dim i, j, k, l, m As Integer
        Dim restContour(Nothing) As Line3D
        Dim GroupBox(Nothing)(), tempBox(Nothing), useBox(Nothing) As Box
        Dim varHeight() As Single

        '(2)
        '//Grouping
        GroupBox = fGetGroupBox(inputBox)

        '//Working per group
        For k = 1 To GroupBox.GetUpperBound(0)
            '(3)
            '//Get variation height in specific group !!(height position2,  not position1)
            varHeight = fGetVarHeightBox(GroupBox(k), True)

            For i = 1 To varHeight.GetUpperBound(0)

                '//Build (New Contour(resize + construct))
                '//Merging all box into new contour --> usually, contour of on top-face of boxes
                ReDim restContour(Nothing)
                ReDim Preserve fSpaceArea(fSpaceArea.GetUpperBound(0) + 1)

                fSpaceArea(fSpaceArea.GetUpperBound(0)) = New MaximalSpace(GroupBox(k), _
                                                                            New Point3D(0, _
                                                                                        0, _
                                                                                        varHeight(i)), _
                                                                            restContour)
                '//Continue after all box located (as contour) in space
                Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
                    ReDim Preserve fSpaceArea(fSpaceArea.GetUpperBound(0) + 1)
                    fSpaceArea(fSpaceArea.GetUpperBound(0)) = New MaximalSpace(restContour, True)
                Loop

                'Catch ex As Exception
                '    MyForm.formMainMenu.txtConsole.Text = "error di buildnew contour --> " & MyForm.formMainMenu.txtConsole.Text
                '    Stop
                'End Try
            Next
        Next

        '(5) 
        '//Revise contour (for empty space area)
        For k = 1 To GroupBox.GetUpperBound(0)
            Dim cekRepeat, cekFinal As Boolean

            cekFinal = False
            Do Until cekFinal = True
                cekRepeat = False
                For i = 1 To fSpaceArea.GetUpperBound(0)
                    '//Check if same height >> continue --to minimize counting
                    If (fSpaceArea(i).OriginPoint.Z = GroupBox(k)(1).AbsPos1.Z) Then
                        l = 0
                        ReDim useBox(l)
                        For j = 1 To GroupBox(k).GetUpperBound(0)
                            '//which box >> not whole box but partially, so we must divide it first
                            tempBox = GetBoxWhenOverlap2D(fSpaceArea(i), GroupBox(k)(j))

                            '//join box
                            If tempBox.GetUpperBound(0) > 0 Then
                                ReDim Preserve useBox(l + tempBox.GetUpperBound(0))
                                For m = l + 1 To l + tempBox.GetUpperBound(0)
                                    useBox(m) = New Box(tempBox(m - l))
                                Next
                                l += tempBox.GetUpperBound(0)
                            End If
                        Next

                        '//at here i get box that overlap,
                        '//what i need to do is join all the box before enter insertbox
                        If useBox.GetUpperBound(0) > 0 Then
                            '//Insert in new box and rebuild contour
                            '//!!! boundingbox.abbpos1 --> harus diganti biar ga bingung
                            fSpaceArea(i).InsertNewBox(useBox, _
                                                        New Point3D(useBox(1).AbsPos1), _
                                                        restContour)

                            '//Iterate until no contour left
                            Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
                                ReDim Preserve fSpaceArea(fSpaceArea.GetUpperBound(0) + 1)
                                fSpaceArea(fSpaceArea.GetUpperBound(0)) = New MaximalSpace(restContour, False)
                            Loop

                            '//repeat until no contour left
                            cekRepeat = True
                        End If
                    End If
                Next
                If cekRepeat = False Then cekFinal = True
            Loop

            'Catch ex As Exception
            '    MyForm.formMainMenu.txtConsole.Text = "error di revise old contour --> " & MyForm.formMainMenu.txtConsole.Text
            'End Try
        Next

        '(4)
        '//Merge space area
        fSpaceArea = GetMergeSpaceArea(fSpaceArea)
    End Sub

    ''' <summary>
    ''' #Merge space
    ''' -Merging space if it can be merge
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Calculate all line contour
    ''' --3. Merge all line contour into one
    ''' --4. Start iteration of merging process
    ''' --5. Return value
    ''' </summary>
    Private Function GetMergeSpaceArea(ByVal inputSpace() As MaximalSpace) As MaximalSpace()
        '(1)
        Dim i, j, k As Integer
        Dim mergeSpace(Nothing) As MaximalSpace

        '(2)
        j = 0
        For i = 1 To inputSpace.GetUpperBound(0)
            j += inputSpace(i).Contour.GetUpperBound(0)
        Next

        '(3)
        Dim lineContour(j) As Line3D
        k = 0
        For i = 1 To inputSpace.GetUpperBound(0)
            For j = 1 To inputSpace(i).Contour.GetUpperBound(0)
                k += 1
                lineContour(k) = New Line3D(inputSpace(i).Contour(j))
            Next
        Next

        '(4)
        '//Merging all box into new contour --> usually, contour of on top-face of boxes
        '//Continue after all box located (as contour) in space
        Do Until (lineContour.GetUpperBound(0) = 0) Or (lineContour Is Nothing)
            ReDim Preserve mergeSpace(mergeSpace.GetUpperBound(0) + 1)
            mergeSpace(mergeSpace.GetUpperBound(0)) = New MaximalSpace(lineContour, _
                                                                       New Point3D(lineContour(1).fPoint1))
        Loop

        Return mergeSpace
    End Function

    ''' <summary>
    ''' #Update space
    ''' -Renew space due to added boxes
    ''' -Generate new space
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Build new contour
    ''' --3. Resize contour
    ''' </summary>
    ''' <remarks>
    ''' Explanation:
    ''' For determine spacial condition (after box(es) added), we use following algorithm:
    ''' 1. Mapping 2D-area based on height of boxes
    '''   -diketahui box yang berada pada ketinggian nol
    '''   -diketahui box dan spacial pada ketinggian tertentu; paling enak sih sebenernya kalo pas pertama aja identifikasinya
    '''   ++logika sederhana
    '''   ---saat pemetaan maka terbentuk 2 spacial: 1 spacial pada ketinggian Zmin, dan 1 spacial pada ketinggian Zmax
    '''   ---spacial pada ketinggian tersebut dijadikan acuan
    '''   ---yang masalah kalo spacial pada ketinggian yang sama cara nyatuinnya gimana?
    '''   ---bisa diketahui dari empty space yang digunakan; jadi bisa ditrace itu masuk ke area2D yang mana
    '''   ---ok.. tapi ngasi taunya itu cuboid sehingga perlu bisa digabung gimana ya?
    ''' - kalo asal jadi bisa sih.. tinggal masukkin input cuboid, aja... biar ga bingung...
    ''' - jadi pas akhirnya dibuat cuboid box aja
    '''
    ''' 2. Placement as usual in 2D space
    '''   -ini perlu perubahan kalo misalnya box mengakomodasi adanya toleransi
    '''
    ''' 3. Generate empty space and update array
    '''</remarks>
    Private Sub UpdateSpace2(ByVal inputBox() As Box, _
                            ByVal boundingBox As Box)
        '(1)
        Dim i, j As Integer
        '//Start manipulation --for temporary
        '//But if this method works well --> we can use it on the future.
        ReDim inputBox(1)
        inputBox(1) = New Box(boundingBox)

        '(2) 
        '//Build (New Contour(resize + construct))
        Dim restContour(Nothing) As Line3D
        ReDim Preserve fSpaceArea(fSpaceArea.GetUpperBound(0) + 1)

        fSpaceArea(fSpaceArea.GetUpperBound(0)) = New MaximalSpace(inputBox, _
                                                              New Point3D(boundingBox.AbsPos1.X, _
                                                                          boundingBox.AbsPos1.Y, _
                                                                          boundingBox.AbsPos2.Z), _
                                                              restContour)

        Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
            ReDim Preserve fSpaceArea(fSpaceArea.GetUpperBound(0) + 1)
            fSpaceArea(fSpaceArea.GetUpperBound(0)) = New MaximalSpace(restContour, True)
        Loop

        'Catch ex As Exception
        '    MyForm.formMainMenu.txtConsole.Text = "error di buildnew contour --> " & MyForm.formMainMenu.txtConsole.Text
        '    Stop
        'End Try

        '(3) 
        '//Resize contour (for empty space area)
        For i = 1 To fSpaceArea.GetUpperBound(0)
            For j = 1 To inputBox.GetUpperBound(0)
                If fSpaceArea(i).CheckBoxInContour(inputBox(j)) = True Then
                    '//Insert in new box
                    fSpaceArea(i).InsertNewBox(inputBox, _
                                            New Point3D(boundingBox.AbsPos1), _
                                            restContour)
                    '//Iterate until no contour left
                    Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
                        ReDim Preserve fSpaceArea(fSpaceArea.GetUpperBound(0) + 1)
                        fSpaceArea(fSpaceArea.GetUpperBound(0)) = New MaximalSpace(restContour, False)
                    Loop
                    Exit For
                End If
            Next
        Next

        'Catch ex As Exception
        '    MyForm.formMainMenu.txtConsole.Text = "error di revise old contour --> " & MyForm.formMainMenu.txtConsole.Text
        'End Try
    End Sub

    ''' <summary>
    ''' #GetBox when Overlap
    ''' -If box overlap maximal space, we need to reduce into several number box in order to fit it on that max.space
    '''     -It's kinda manipulation to get easier calculation
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Start iteration
    ''' </summary>
    Private Function GetBoxWhenOverlap2D(ByVal currSpace As MaximalSpace, ByVal mainBox As Box) As Box()
        '(1)
        Dim resultBox(Nothing) As Box

        '(2)
        '//if box touch contour --> insert new box
        If currSpace.CheckBoxTouchContour(mainBox) = True Then
            '(3)
            '//if box in contour
            '//else box touch contour, but not whole box in contour
            If currSpace.CheckBoxInContour(mainBox) = True Then
                ReDim resultBox(1)
                resultBox(1) = New Box(mainBox)
            Else
                Dim i, j, k, l As Integer
                Dim fContour(Nothing), restContour(Nothing) As Line3D
                Dim cekRepeat, cekFinal As Boolean
                Dim tempRect As Rectangle
                Dim tempBox(1) As Box

                '//Change box into maximal space
                '//Get Contour
                currSpace.GetContour(fContour, resultBox)
                '//Build variable
                Dim currBox(1) As MaximalSpace
                currBox(1) = New MaximalSpace(fContour, False)

                '//Start iteration
                cekFinal = False
                l = 0
                Do Until cekFinal = True
                    cekRepeat = False
                    For i = 1 To currSpace.CountSpace
                        For j = 1 To currBox.GetUpperBound(0)
                            For k = 1 To currBox(j).CountSpace
                                '//if there is intersection >> get the area that intersect
                                tempRect = currBox(j).Space(k).Rectangle
                                If (tempRect.IntersectsWith(currSpace.Space(i).Rectangle) = True) Then
                                    '//get intersect area
                                    tempRect.Intersect(currSpace.Space(i).Rectangle)

                                    '//get intersect box
                                    l += 1
                                    ReDim Preserve resultBox(l)
                                    resultBox(l) = New Box(mainBox.Type, tempRect.Width, tempRect.Height, mainBox.Height)
                                    resultBox(l).AbsPos1 = New Point3D(tempRect.X, tempRect.Y, mainBox.AbsPos1.Z)

                                    '//repeat again
                                    cekRepeat = True
                                End If
                                '//exit
                                If cekRepeat = True Then Exit For
                            Next
                            '//exit
                            If cekRepeat = True Then Exit For
                        Next
                        '//exit
                        If cekRepeat = True Then Exit For
                    Next

                    '//revise currBox form
                    If cekRepeat = True Then
                        tempBox(1) = New Box(resultBox(l))
                        currBox(j).InsertNewBox(tempBox, _
                                                New Point3D(tempBox(1).AbsPos1), _
                                                restContour)

                        '//Iterate until no contour left
                        Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
                            ReDim Preserve currBox(currBox.GetUpperBound(0) + 1)
                            currBox(currBox.GetUpperBound(0)) = New MaximalSpace(restContour, False)
                        Loop
                    Else
                        '//Iteration finish if no repeat process anymore
                        cekFinal = True
                    End If
                Loop
            End If
        End If

        Return resultBox
    End Function

    ''' <summary>
    ''' #GetSpace
    ''' -Get emptySpace from emptyArea (raw)
    ''' --1. Variable set
    ''' --2. Count total space 
    ''' --3. Resize number of space
    ''' --4. Fill space 
    ''' --5. Remove space that volume = 0 (null)
    ''' </summary>
    Public Sub GetSpace()
        '(1)
        Dim i, j, k As Integer

        '(2)
        j = 0
        For i = 1 To fSpaceArea.GetUpperBound(0)
            j += fSpaceArea(i).Space.GetUpperBound(0)
        Next

        '(3)
        ReDim fSpaceBox(j)

        '(4)
        k = 0
        For i = 1 To fSpaceArea.GetUpperBound(0)
            With fSpaceArea(i)
                For j = 1 To .Space.GetUpperBound(0)
                    k += 1
                    fSpaceBox(k) = New Box(-1, _
                                           .Space(j).Depth, _
                                           .Space(j).Width, _
                                           (fInitialSpace.Z - .Space(j).Height))
                    fSpaceBox(k).AbsPos1 = New Point3D(.Space(j).Position)
                    fSpaceBox(k).RelPos1 = New Point3D(0, 0, 0)
                Next
            End With
        Next

        '(5)
        j = 0
        For i = 1 To fSpaceBox.GetUpperBound(0)
            With fSpaceBox(i)
                If (.Depth * .Width * .Height) > 0 Then
                    j += 1
                    If i <> j Then
                        fSpaceBox(j) = New Box(-1, _
                                               .Depth, _
                                               .Width, _
                                               .Height)
                        fSpaceBox(j).AbsPos1 = New Point3D(fSpaceBox(i).AbsPos1)
                    End If
                End If
            End With
        Next
        ReDim Preserve fSpaceBox(j)
    End Sub

    ''' <summary>
    ''' Get utilization
    ''' </summary>
    Private Function GetVolume() As Single
        Dim i As Integer
        Dim vol As Single = 0
        For i = 1 To fBox.GetUpperBound(0)
            vol += fBox(i).Width * fBox(i).Depth * fBox(i).Height
        Next
        Return vol
    End Function

    ''' <summary>
    ''' #UpdatePosCont
    ''' -Get position boxes in container (as absolute position)
    ''' </summary>
    Private Sub UpdatePosCont(ByVal position As Point3D, ByRef dataBox() As Box)
        For i As Integer = 1 To dataBox.GetUpperBound(0)
            dataBox(i).AbsPos1 = New Point3D(position.X + dataBox(i).RelPos1.X, position.Y + dataBox(i).RelPos1.Y, position.Z + dataBox(i).RelPos1.Z)
        Next
    End Sub

    ''' <summary>
    ''' 1. Check collision position in container
    ''' 2. Check position box in container in container
    ''' Strategi: kumpulin semua koordinat, cek apakah masuk dalam area orang lain
    ''' </summary>
    Public Sub GetValidation()
        Dim i, j As Integer
        Dim cek As Boolean
        i = 0 : j = 0 : cek = False

        '(1)
        '// perhitungan hanya dilakukan bila minimal ada 2 box sudah masuk
        If fBox.GetUpperBound(0) < 2 Then
            fCollision = False
        Else
            Do Until (cek = True) Or (i = fBox.GetUpperBound(0))
                i += 1
                j = i
                Do Until (cek = True) Or (j = fBox.GetUpperBound(0))
                    j += 1
                    cek = functCheckCollision3D(fBox(i), fBox(j))
                Loop
            Loop
            If cek = True Then
                fCollision = True
            Else
                fCollision = False
            End If
        End If

        '(2)
        If fBox.GetUpperBound(0) = 0 Then
            fOutContainer = False
        Else
            For i = 1 To fBox.GetUpperBound(0)
                If functCheckBoxInBound(fBox(i), fBox(0)) = False Then
                    fOutContainer = True
                    Exit For
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' #SeparateBox
    ''' -Separating inputBox and outputBox --based on 'in-Container' variable
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' </summary>
    Private Sub GetSeparateBox(ByRef inpBox() As Box, ByRef outBox() As Box)
        '(1)
        Dim i, j, l As Integer

        '(2)
        For i = 1 To inpBox.GetUpperBound(0)
            If inpBox(i).InContainer = True Then
                j += 1
                inpBox(j) = New Box(inpBox(i))
            Else
                l += 1
                outBox(l) = New Box(inpBox(i))
            End If
        Next
        ReDim Preserve inpBox(j)
        ReDim Preserve outBox(l)
    End Sub


    '//
    '// FIELD
    '//
    ''' <summary>
    ''' Emptyspace in container (box type)
    ''' </summary>
    Private fSpaceBox() As Box

    ''' <summary>
    ''' Box in container
    ''' Box that placed in container
    ''' </summary>
    Private fBox() As Box

    ''' <summary>
    ''' Initial emptyspace --> container
    ''' </summary>
    Private fInitialSpace As Point3D

    ''' <summary>
    ''' Contour empty space
    ''' </summary>
    Private fSpaceArea() As MaximalSpace

    ''' <summary>
    ''' BoundingBox in container --> similiar with box
    ''' </summary>
    Private fBoundingBox(Nothing) As Box

    ''' <summary>
    ''' Colission validation
    ''' </summary>
    Private fCollision As Boolean

    ''' <summary>
    ''' Number box that out of container
    ''' </summary>
    Private fOutContainer As Integer


    '//
    '//PROPERTY
    '//
    ''' <summary>
    ''' #Get utilization
    ''' -Utilization container
    ''' </summary>
    Public ReadOnly Property Utilization() As Single
        Get
            Return (VolumeBox / VolumeContainer)
        End Get
    End Property

    ''' <summary>
    ''' #Read a box
    ''' -Call box data
    ''' </summary>
    Public ReadOnly Property Box(ByVal i As Integer) As Box
        Get
            Return fBox(i)
        End Get
    End Property

    ''' <summary>
    ''' #Volume box
    ''' -Calculate volume box
    ''' </summary>
    Public ReadOnly Property VolumeBox() As Single
        Get
            Return GetVolume()
        End Get
    End Property

    ''' <summary>
    ''' #Volume container
    ''' -Calculate volume container
    ''' </summary>
    Public ReadOnly Property VolumeContainer() As Single
        Get
            Return (fInitialSpace.X * fInitialSpace.Y * fInitialSpace.Z)
        End Get
    End Property

    ''' <summary>
    ''' #BoxinColission
    ''' -Get box in collision: TRUE --> collision, FALSE --> free of colision
    ''' </summary>
    Public ReadOnly Property Validation() As Boolean
        Get
            If (fCollision = False) Or (fOutContainer = False) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    ''' <summary>
    ''' #EmptySpace
    ''' -Get space
    ''' </summary>
    Public ReadOnly Property Space(ByVal i As Integer) As Box
        Get
            Return fSpaceBox(i)
        End Get
    End Property

    ''' <summary>
    ''' #CountSpace
    ''' -Calculate number space result
    ''' </summary>
    Public ReadOnly Property CountSpace() As Integer
        Get
            Return fSpaceBox.GetUpperBound(0)
        End Get
    End Property

    ''' <summary>
    ''' #CountBox
    ''' -Calculate number box result
    ''' </summary>
    Public ReadOnly Property CountBox() As Integer
        Get
            Return fBox.GetUpperBound(0)
        End Get
    End Property

    ''' <summary>
    ''' Output data for printing
    ''' </summary>
    Public ReadOnly Property OutputBox() As Box()
        Get
            Return fBox
        End Get
    End Property

    ''' <summary>
    ''' Output space for printing
    ''' </summary>
    Public ReadOnly Property OutputSpace() As Box()
        Get
            Return fSpaceBox
        End Get
    End Property
End Class

'''' <summary>
'''' #SetNewBox
'''' -If box maps-in a space --> recalculate and configure new position
'''' --0. Parameter set
'''' --1. Update coordinate
'''' --2. Insert new box
'''' </summary>
'Public Sub SetNewBox(ByVal Space As Box, _
'                     ByVal inputBox() As Box)
'    '(1)
'    UpdatePosCont(New Point3D(Space.AbsPos1.X, Space.AbsPos1.Y, Space.AbsPos1.Z), inputBox)

'    '(2)
'    For i As Integer = (fBox.GetUpperBound(0) + 1) To (fBox.GetUpperBound(0) + inputBox.GetUpperBound(0))
'        fBox(i) = New Box(inputBox(i - fBox.GetUpperBound(0)))
'    Next
'End Sub