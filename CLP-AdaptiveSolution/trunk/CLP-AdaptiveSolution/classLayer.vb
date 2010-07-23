Imports System
Imports System.Collections.Generic

''' <summary>
''' Construction of layering box
''' </summary>
Public Class Layer
    Inherits Placement

    ''' <summary>
    ''' Allow placement in tolerate area
    ''' </summary>
    Private FTolerate As Boolean
    ''' <summary>
    ''' Detail placement of area tolerate
    ''' </summary>
    Private FSideTolerate As SideRectangle
    Private FGroupData() As CollectData

    ''' <summary>
    ''' Defining area structure
    ''' </summary>
    Public Structure CollectData
        Dim SNumber As Single
        Dim SCount As Integer
        Dim SFitness As Single
    End Structure

    Public Structure SideRectangle
        Dim Left, Right, Up, Bottom As Boolean
    End Structure

    ''' <summary>
    ''' Standar construction
    ''' </summary>
    Sub New(ByVal DataEmptySpace As Box, ByVal InputBox() As Box)
        'input data
        FInput = InputBox
        FEmptySpace = DataEmptySpace
        FTolerate = False               'false --> no toleration
        FSideTolerate.Up = False
        FSideTolerate.Bottom = False
        FSideTolerate.Left = False
        FSideTolerate.Right = False

        'recapitulation data
        Recapitulation(FInput, FDataListInput)
    End Sub

    ''' <summary>
    ''' Full construction
    ''' </summary>
    Sub New(ByVal DataEmptySpace As Box, ByVal InputBox() As Box, ByVal Tolerate As Boolean, ByVal Left As Boolean, ByVal Right As Boolean, ByVal Up As Boolean, ByVal Bottom As Boolean)
        'input data
        FInput = InputBox
        FEmptySpace = DataEmptySpace

        FTolerate = Tolerate
        FSideTolerate.Up = Up
        FSideTolerate.Bottom = Bottom
        FSideTolerate.Left = Left
        FSideTolerate.Right = Right

        'recapitulation data
        Recapitulation(FInput, FDataListInput)
    End Sub

    Public Property Tolerate() As Boolean
        Get
            Return FTolerate
        End Get
        Set(ByVal Value As Boolean)
            FTolerate = Value
        End Set
    End Property

    ''' <summary>
    ''' Get layering optimization
    ''' </summary>
    Public Sub GetOptimizeLayer()
        'variable counter
        Dim i, j, k, l As Integer

        'ide(penyusunan)
        '- kumpulin box dengan ketinggian yang sama terlebih dulu
        '- kumpulin dulu yang sama, buat cuboid 1 lapis
        '- cari yang box lain yang ketinggiannya sama
        '- coba di plot di area tersebut, termasuk kemungkinan melanggar.
        '- arahnya semakin ke-X atau ke-Y nilai fitness semakin bagus.. tapi ini perlu dibuat heuristiknya juga sih...

        '#kumpulin box
        '1. cari data ketinggian dari semua box
        GetGroupingHeight()
        '2. sorting box tersebut --> max to min
        sortingGroup(FGroupData)
        reverseGroup(FGroupData)

        '#mulai iterasi buat packing
        'variable data
        Dim VPointerBox(Nothing) As Integer
        Dim VTempBox(Nothing), VTemp2Box(Nothing), VTempCuboid(Nothing) As Box
        Dim VListBox(Nothing) As ListBox
        Dim VCuboid(Nothing) As Cuboid
        Dim VPointerCuboid(Nothing, Nothing) As Integer
        Dim upperBound As Single = 0

        'if iteration above fitness, do iteration
        i = 1
        Do Until FGroupData(i).SFitness < upperBound
            'for feasibility, only height below the empty space.height
            If (FEmptySpace.Height - FGroupData(i).SNumber) >= 0 Then
                '-----
                'get pointer, box dengan ketinggian yang sama
                GetPointerBox(FGroupData(i).SNumber, VPointerBox)

                'copy data to temporary box --> dipakai sementara (lokalisasi bentar)
                ReDim VTempBox(VPointerBox.GetUpperBound(0))
                For j = 1 To VPointerBox.GetUpperBound(0)
                    VTempBox(j) = FInput(VPointerBox(j))
                Next

                'rekap box --> list; untuk tahu komposisi box-nya (walau ketinggian sama, tetep aja, bisa jadi type beda)
                Recapitulation(VTempBox, VListBox)

                'resizing cuboid --> hasil rekap box (list)
                ReDim VTempCuboid(VListBox.GetUpperBound(0))
                ReDim VCuboid(VListBox.GetUpperBound(0))

                'get pointer cuboid --> cuboid tersebut menunjuk ke box mana saja
                GetPointerCuboid(VListBox, VPointerCuboid)

                'get cuboid
                For j = 1 To VListBox.GetUpperBound(0)
                    '1. grouping cuboid --> dari box yang tipe-nya sama
                    ReDim VTemp2Box(VListBox(j).SCount)
                    l = 0
                    For k = 1 To VTempBox.GetUpperBound(0)
                        If VListBox(j).SType = VTempBox(k).Type Then
                            l += 1
                            VTemp2Box(l) = VTempBox(k)
                        End If
                        If l = VListBox(j).SCount Then Exit For
                    Next

                    '2. build cuboid
                    VCuboid(j) = New Cuboid(FEmptySpace, VTemp2Box)
                    '3. optimasi plot posisi cuboid di area zone
                    VCuboid(j).GetOptimizeLayer(False)
                    '4. get cuboid bounding box
                    VTempCuboid(j) = VCuboid(j).BoundingCuboid
                    VTempCuboid(j).Type = VListBox(j).SType
                Next

                '#layering plotting
                GetOptimizePlacement(FEmptySpace, VTempCuboid, VTempBox, VPointerCuboid)


                '#harusnya disini keluar semua tipe box, orientasi, juga penempatannya gmn...
                ' apa disini dikeluarin tipe box, orientasi, penempatannya aja ya?
                ' trus disimpen sebagai the best..
                ' trus yang disimpen cuma yang box aja. berhubung yang cuboid sendiri kan sebenernya cuma bentuk dari box
                ' jadi disimpen data best placement:
                ' 1. box 
                ' 2. orientasi (side dan orientasi)
                ' 3. placement 
                ' +++ kalo begitu yang cuboid harus dipecah dulu pas disini... berarti data cuboid harus dikembalikan lagi donk
                ' +++ kalo data cuboid harus dikembalikan lagi, berarti harus ada data yang menjelaskan data mana saja yang terpakai dan tidak

            End If
            'continue to the next group
            i += 1



        Loop

    End Sub

    ''' <summary>
    ''' Gathering the same height
    ''' </summary>
    Private Sub GetGroupingHeight()
        Dim i, j, counter As Integer
        Dim Height, Area As Single

        'enlarge the array
        ReDim FGroupData(FInput.GetUpperBound(0) * 3)
        'iterate for all box
        For i = 1 To FInput.GetUpperBound(0)
            For j = 1 To 3
                Select Case j
                    Case 1
                        Height = FInput(i).Depth
                        Area = FInput(i).Width * FInput(i).Height
                    Case 2
                        Height = FInput(i).Width
                        Area = FInput(i).Depth * FInput(i).Height
                    Case 3
                        Height = FInput(i).Height
                        Area = FInput(i).Depth * FInput(i).Width
                End Select

                If CheckHeight(Height) = True Then
                    counter += 1
                    FGroupData(counter).SNumber = Height
                    FGroupData(counter).SCount = 1
                    FGroupData(counter).SFitness = Area
                Else
                    FGroupData(FindHeight(Height)).SCount += 1
                    FGroupData(FindHeight(Height)).SFitness += Area
                End If
            Next
        Next
        'fix the array size
        ReDim Preserve FGroupData(counter)
    End Sub


    ''' <summary>
    ''' Sorting of group data
    ''' </summary>
    Private Sub sortingGroup(ByVal group() As CollectData)
        Dim i, j As Integer

        For i = 1 To group.GetUpperBound(0) - 1
            For j = i To group.GetUpperBound(0)
                If (group(i).SFitness > group(j).SFitness) Or _
                    ((group(i).SFitness = group(j).SFitness) And (group(i).SCount > group(j).SCount)) Or _
                    ((group(i).SFitness = group(j).SFitness) And (group(i).SCount = group(j).SCount) And (group(i).SNumber > group(j).SNumber)) _
                    Then Swap(group(i), group(j))
            Next
        Next
    End Sub

    ''' <summary>
    ''' Reverse sorting of group data
    ''' </summary>
    Private Sub reverseGroup(ByRef group() As CollectData)
        Dim temp() As CollectData
        Dim i As Integer

        temp = ShallowClone(group)
        For i = 1 To group.GetUpperBound(0)
            group(i) = temp(group.GetUpperBound(0) - i + 1)
        Next
    End Sub

    ''' <summary>
    ''' Check height
    ''' </summary>
    Private Function CheckHeight(ByVal height As Single) As Boolean
        Dim i As Integer

        'reset data first
        CheckHeight = True          'set the data true, until find the same date --> replace with false

        For i = 1 To FGroupData.GetUpperBound(0)
            If height = FGroupData(i).SNumber Then
                CheckHeight = False
                Exit For
            End If
        Next
    End Function

    ''' <summary>
    ''' Find height
    ''' </summary>
    Private Function FindHeight(ByVal height As Single) As Integer
        Dim i As Integer

        For i = 1 To FGroupData.GetUpperBound(0)
            If height = FGroupData(i).SNumber Then
                Return i
                Exit For
            End If
        Next
    End Function

    ''' <summary>
    ''' Find pointer box
    ''' </summary>
    Private Sub GetPointerBox(ByVal height As Single, ByRef pointerBox() As Integer)
        Dim i, j, count As Integer
        'reset data
        ReDim pointerBox(FInput.GetUpperBound(0))
        count = 0

        For i = 1 To FInput.GetUpperBound(0)
            If (FInput(i).Depth = height) Or (FInput(i).Height = height) Or (FInput(i).Width = height) Then
                count += 1
                'set pointer
                pointerBox(count) = i
                For j = 1 To 3
                    Select Case j
                        Case 1 : FInput(i).Alpha = True
                        Case 2 : FInput(i).Beta = True
                        Case 3 : FInput(i).Gamma = True
                    End Select
                    If FInput(i).Height = height Then Exit For 'exit if the side has been side rightly
                Next
            End If
        Next
        'resize for last
        ReDim Preserve pointerBox(count)
    End Sub

    ''' <summary>
    ''' Find pointer cuboid, same like pointer box
    ''' </summary>
    Private Sub GetPointerCuboid(ByVal ListBox() As ListBox, ByRef pointerCuboid(,) As Integer)
        Dim i, j, count As Integer
        'resize array
        ReDim pointerCuboid(ListBox.GetUpperBound(0), Nothing)
        're pointing
        For i = 1 To ListBox.GetUpperBound(0)
            'input data scount
            pointerCuboid(i, 0) = ListBox(i).SType
            'resize the 2nd dimension
            ReDim Preserve pointerCuboid(i, ListBox(i).SCount)
            'find the pointer
            count = 0
            For j = 1 To FInput.GetUpperBound(0)
                If FInput(j).Type = ListBox(i).SType Then
                    count += 1
                    pointerCuboid(i, count) = j
                End If
                If count = ListBox(i).SCount Then Exit For
            Next
        Next
    End Sub

    ''' <summary>
    ''' Placement box in layer
    ''' </summary>
    ''' <remarks>
    ''' - Placing box in empty-space
    ''' - Start from the largest area
    ''' 
    ''' - Insert the next area, with criteria
    ''' - Trying all position in all area,
    ''' - Trying orientation in all area,
    ''' - Finding the largest maximal-space in there
    ''' </remarks>
    Private Sub GetOptimizePlacement(ByVal emptySpace As Box, ByRef CuboidBox() As Box, ByRef BoxSingle() As Box, ByVal PointerCuboid(,) As Integer)
        Dim i, j, k, l As Integer

        '#get empty area 
        Dim emptyArea As New Kotak(emptySpace.Width, emptySpace.Depth, emptySpace.Height)

        '#get data cuboid box, box single --> transform to 2D
        '--enlarge array size
        Dim box2D(CuboidBox.GetUpperBound(0) + BoxSingle.GetUpperBound(0)) As Kotak
        Dim boxFisibel(CuboidBox.GetUpperBound(0) + BoxSingle.GetUpperBound(0)) As Boolean

        '--transform to 2D
        j = 0
        For i = 1 To (CuboidBox.GetUpperBound(0) + BoxSingle.GetUpperBound(0))
            'reset data
            boxFisibel(i) = True

            'build area class
            If i <= CuboidBox.GetUpperBound(0) Then
                'area class: cuboid
                j += 1
                box2D(i) = New Kotak(j, True, CuboidBox(j).Width, CuboidBox(j).Depth, CuboidBox(j).Height, New Point3D(0, 0, 0))
                If j = CuboidBox.GetUpperBound(0) Then j = 0 'reset j = 0, to counter the next box single
            Else
                j += 1
                'find pointer for single box
                For k = 1 To CuboidBox.GetUpperBound(0)
                    If CuboidBox(k).Type = BoxSingle(j).Type Then
                        l = k
                        Exit For
                    End If
                Next
                box2D(i) = New Kotak(l, False, BoxSingle(j).Width, BoxSingle(j).Depth, BoxSingle(j).Height, New Point3D(0, 0, 0))
            End If
        Next

        '#sorting area from max to min
        sortingArea(box2D)

        '#placing in 2DPlot
        Dim plotting As New Plot2D(emptyArea, box2D)
        Dim tempScore, tempScore2, tempBest As Single

        Dim historyFitness(plotting.CountKotak) As Single
        Dim historyBox(plotting.CountKotak) As Integer
        Dim historyPosition(plotting.CountKotak) As Point3D

        Dim bestOrientation As Boolean
        Dim bestPlacement As Integer

        '#placement --kalo bisa semua di akses langsung dari classplotting2D
        tempBest = 0
        l = 0
        '- placing order from larger area to minimum area
        For i = 1 To plotting.CountKotak
            If boxFisibel(i) = True Then
                '---
                '- plotting 1 kotak
                '- iteration for all position/coordinate

                'reset data
                tempScore = 0
                tempScore2 = 0
                For j = 1 To plotting.CountPlacement
                    '- 1 coordinate, 2 trying --different orientation (true and false)
                    For k = 1 To 2
                        If k = 1 Then
                            plotting.OrientationKotak(i) = True
                        Else
                            plotting.OrientationKotak(i) = False
                        End If
                    Next
                    '- plotting kotak i in area j --> get the best plotting
                    tempScore = plotting.GetScorePositionRectangle(i, plotting.GetPlacement(j))
                    If tempScore2 < tempScore Then
                        tempScore2 = tempScore
                        bestPlacement = j
                        bestOrientation = plotting.OrientationKotak(i)
                    End If
                Next

                If tempScore2 > 0 Then
                    If tempBest < tempScore2 Then tempBest = tempScore2

                    '- record history
                    l += 1
                    historyBox(l) = i
                    historyFitness(l) = tempScore2
                    historyPosition(l) = plotting.GetPlacement(bestPlacement)

                    '- do the placement
                    plotting.KotakInEmptySpace(i) = True
                    plotting.SetPlacement(bestPlacement, plotting.GetPlacement(bestPlacement), bestOrientation)

                    '- update feasibility box
                    boxFisibel(i) = False
                    For k = 1 To boxFisibel.GetUpperBound(0)
                        If (box2D(k).Pointer = box2D(i).Pointer) AndAlso (box2D(k).DefineCuboid = False) Then boxFisibel(k) = False
                    Next

                End If
                '---
            End If
        Next

        '- resize the history
        ReDim Preserve historyBox(l), historyFitness(l), historyPosition(l)

        '#reallign the orientation of box
        '- choose the best score
        Dim bestFitness As Single = 0
        Dim pointer As Integer
        For i = 1 To historyFitness.GetUpperBound(0)
            If historyFitness(i) > bestFitness Then
                bestFitness = historyFitness(i)
                pointer = i
            End If
        Next

        '- rewrite box allignment
        ' arghhh ga ada ide... ini sebenernya cuma tinggal mindahin aja tapi koq ga bisa ya.
        ' kesulitan:
        ' 1. gw harus update cuboid, dan box sekaligus
        ' 2. gw bingung untuk selanjutnya nanti si cuboid dan box akan diupdate seperti apa
        ' 3. untuk output paling akhir, bentuknya akan seperti apa.

        For i = 1 To pointer

        Next

    End Sub

    ''' <summary>
    ''' Sorting of box data
    ''' </summary>
    Private Sub sortingArea(ByRef area() As Kotak)
        Dim i, j As Integer

        For i = 1 To area.GetUpperBound(0) - 1
            For j = i To area.GetUpperBound(0)
                If (area(i).Width * area(i).Depth) <= (area(j).Width * area(j).Depth) Then
                    Swap(area(i), area(j))
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' Reverse sorting of box data
    ''' </summary>
    Private Sub reverseArea(ByRef area() As Kotak)
        Dim temp() As Kotak
        Dim i As Integer

        temp = ShallowClone(area)
        For i = 1 To area.GetUpperBound(0)
            area(i) = temp(area.GetUpperBound(0) - i + 1)
        Next

    End Sub
End Class

' jangan lupa, ntar bikin langsung prosedur rekursif..
' ga susah koq, pokoknya limit rekursif selese saat ga ada lagi yang bisa ditumpuk karn ketinggian dan luas tumpukan melebihi tolerasnsi
' selain itu, variabel bisa diakses langsung  biar mudah...
' pada intinya nanti ada back tracking untuk ngumpulin hasilnya...
' paling enak ud disiapin, ranking dan urutan area zone untuk ditempati.. apakah oke ao ga...
' trus pas masang perlu ditempatin juga, penempatan tiap balok di area xone yang paling efektif, dan efisien
' apalagi ya... gw sebenernya juga masi bingung, mo coding tapi gw ud ngantuk..
' jangan lupa ke kedutaan, buat dapet visa...




'Private Function CompareDataGroup(ByVal x As CollectData, ByVal y As CollectData) As Integer
'    If (x.SCount > y.SCount) Or _
'        ((x.SCount = y.SCount) And (x.SNumber > y.SNumber)) Then
'        Return -1
'    Else
'        Return 0
'    End If
'End Function