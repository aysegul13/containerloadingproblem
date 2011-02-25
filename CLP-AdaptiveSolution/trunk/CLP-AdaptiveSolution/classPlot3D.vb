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
        fSpaceArea(1) = New Contour(fSpaceBox(1))
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
            fSpaceArea(i) = New Contour(masterPlot3D.fSpaceArea(i))
        Next

        '//SpaceBox
        Try
            procBoxClone(masterPlot3D.fSpaceBox, fSpaceBox)
        Catch ex As Exception
        End Try

    End Sub


    ''' <summary>
    ''' #InsertNewBoxes
    ''' -If input new box added to 3D-space
    ''' -Include output-mechanism
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
        UpdateSpace(inputBox, boundingBox)
    End Sub

    ''' <summary>
    ''' #InsertNewBoxes
    ''' -If input new box added to 3D-space
    ''' -Include output-mechanism
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
        UpdateSpace(insertBox, boundingBox)
    End Sub

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
    Private Sub UpdateSpace(ByVal inputBox() As Box, _
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

        fSpaceArea(fSpaceArea.GetUpperBound(0)) = New Contour(inputBox, _
                                                              New Point3D(boundingBox.AbsPos1.X, _
                                                                          boundingBox.AbsPos1.Y, _
                                                                          boundingBox.AbsPos2.Z), _
                                                              restContour)

        Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
            ReDim Preserve fSpaceArea(fSpaceArea.GetUpperBound(0) + 1)
            fSpaceArea(fSpaceArea.GetUpperBound(0)) = New Contour(restContour, True)
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
                        fSpaceArea(fSpaceArea.GetUpperBound(0)) = New Contour(restContour, False)
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
            j += fSpaceArea(i).EmptySpace.GetUpperBound(0)
        Next

        '(3)
        ReDim fSpaceBox(j)

        '(4)
        k = 0
        For i = 1 To fSpaceArea.GetUpperBound(0)
            With fSpaceArea(i)
                For j = 1 To .EmptySpace.GetUpperBound(0)
                    k += 1
                    fSpaceBox(k) = New Box(-1, _
                                           .EmptySpace(j).Depth, _
                                           .EmptySpace(j).Width, _
                                           (fInitialSpace.Z - .EmptySpace(j).Height))
                    fSpaceBox(k).AbsPos1 = New Point3D(.EmptySpace(j).Position)
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
    Private fSpaceArea() As Contour

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
    ''' Output data for printing
    ''' </summary>
    Public ReadOnly Property Output() As Box()
        Get
            Return fBox
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