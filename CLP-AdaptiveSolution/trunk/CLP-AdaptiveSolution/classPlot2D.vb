
Public Class Plot2D

    ''' <summary>
    ''' Empty space
    ''' </summary>
    Private FEmptyArea As Kotak
    ''' <summary>
    ''' Kotak
    ''' </summary>
    Private FKotak() As Kotak

    ''' <summary>
    ''' Boolean, true = kotak in area, false = kotak in outside
    ''' </summary>
    Private FKotakInEmptyspace() As Boolean
    Private FUtilization As Double
    Private FMaximalSpace As Double
    ''' <summary>
    ''' Placement position
    ''' </summary>
    Private FPlacement() As Point3D

    ''' <summary>
    ''' Bounding area for 2D plot
    ''' </summary>
    Private FBoundingArea As Kotak

    ''' <summary>
    ''' Input data in area
    ''' </summary>
    Sub New(ByVal empty As Kotak, ByVal area() As Kotak)
        FEmptyArea = empty
        FKotak = area

        ReDim FKotakInEmptyspace(FKotak.GetUpperBound(0))
        For i = 1 To FKotak.GetUpperBound(0)
            FKotakInEmptyspace(i) = False           'default value
        Next

        ReDim FPlacement(1)
        FPlacement(1) = New Point3D(FEmptyArea.Position.X, FEmptyArea.Position.Y, FEmptyArea.Position.Z)
    End Sub

    ''' <summary>
    ''' Get and set information kotak(i)
    ''' </summary>
    Public ReadOnly Property GetKotak(ByVal i As Integer) As Kotak
        Get
            Return FKotak(i)
        End Get
    End Property

    ''' <summary>
    ''' number area of kotak
    ''' </summary>
    Public ReadOnly Property CountKotak() As Integer
        Get
            Return FKotak.GetUpperBound(0)
        End Get
    End Property

    ''' <summary>
    ''' Get information placement(i)
    ''' </summary>
    Public ReadOnly Property GetPlacement(ByVal i As Integer) As Point3D
        Get
            Return FPlacement(i)
        End Get
    End Property

    ''' <summary>
    ''' Get number area of possible placement
    ''' </summary>
    Public ReadOnly Property CountPlacement() As Integer
        Get
            Return FPlacement.GetUpperBound(0)
        End Get
    End Property

    ''' <summary>
    ''' Get and set orientation kotak
    ''' </summary>
    Public Property OrientationKotak(ByVal i As Integer) As Boolean
        Get
            Return FKotak(i).Orientation
        End Get
        Set(ByVal Value As Boolean)
            FKotak(i).Orientation = Value
        End Set
    End Property

    Public Property KotakInEmptySpace(ByVal i As Integer) As Boolean
        Get
            Return FKotakInEmptyspace(i)
        End Get
        Set(ByVal Value As Boolean)
            FKotakInEmptyspace(i) = Value
        End Set
    End Property


    ''' <summary>
    ''' Get score for positioning rectangle
    ''' </summary>
    ''' <remarks>
    ''' - checking rectangle and position to others, it's ok or not.
    ''' - checking feasibility in that position
    '''
    ''' Scoring detail --> similiar with scoring function in cuboid
    ''' ----
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
    Public Function GetScorePositionRectangle(ByVal pointer As Integer, ByVal position As Point3D) As Single
        Dim i As Integer
        Dim cost1, cost2, benefit1, benefit2, ratio1, ratio2 As Single
        Dim areaFill, areaBounding, areaEmptyspace As Single

        'if fisibel --> get score, else: score = 0
        If CheckingFeasibilityPositionRectangle(pointer, position) = True Then
            'get bounding area
            GetBoundingArea(pointer, position)

            'get score
            'calculate totalRatio of benefit/cost
            cost1 = (FEmptyArea.Width - FBoundingArea.Width) * FBoundingArea.Depth
            benefit1 = (FEmptyArea.Depth - FBoundingArea.Depth) * FEmptyArea.Width

            cost2 = (FEmptyArea.Depth - FBoundingArea.Depth) * FBoundingArea.Width
            benefit2 = (FEmptyArea.Width - FBoundingArea.Width) * FEmptyArea.Depth

            If cost1 < 1 Then
                ratio1 = benefit1
            Else
                ratio1 = benefit1 / cost1
            End If
            If cost2 < 1 Then
                ratio2 = benefit2
            Else
                ratio2 = benefit2 / cost2
            End If

            'get area square
            areaFill = FKotak(pointer).Width * FKotak(pointer).Depth
            For i = 1 To FKotak.GetUpperBound(0)
                If FKotakInEmptyspace(i) = True Then areaFill += FKotak(i).Width * FKotak(i).Depth
            Next
            areaBounding = FBoundingArea.Width * FBoundingArea.Depth
            areaEmptyspace = FEmptyArea.Width * FEmptyArea.Depth

            'calculate fitness score
            Return (ratio1 + ratio2) * (areaFill / areaEmptyspace) * (areaFill / areaBounding)
        Else
            Return 0
        End If
    End Function

    ''' <summary>
    ''' Checking feasibility rectangle in position
    ''' </summary>
    Private Function CheckingFeasibilityPositionRectangle(ByVal pointer As Integer, ByVal position As Point3D) As Boolean
        Dim i As Integer
        Dim cek As Boolean

        '#cek fisibel1 --> cek apakah posisi, keluar dari empty space atau tidak...
        '--bisa diextend lo, fisibilitas untuk posisi asal ga keluar dari tolerate area
        '--tapi entar aja..
        cek = True                  'default cek = true --> it's okay to continue to next feasibility
        If (FKotak(pointer).Position.X < FEmptyArea.Position.X) Or _
            (FKotak(pointer).Position.Y < FEmptyArea.Position.Y) Or _
            (FKotak(pointer).Position.Z < FEmptyArea.Position.Z) Or _
            (FKotak(pointer).Position2.X > FEmptyArea.Position2.X) Or _
            (FKotak(pointer).Position2.Y > FEmptyArea.Position2.Y) Or _
            (FKotak(pointer).Position2.Z > FEmptyArea.Position2.Z) Then
            cek = False
        End If

        'cek fisibel2 --> cek apakah posisi, berhimpitan atau memotong area lain yang sebelumnya atau tidak.
        If cek = True Then
            For i = 1 To FKotak.GetUpperBound(0)
                If (FKotakInEmptyspace(i) = True) AndAlso (CheckingCollision2D(FKotak(pointer), FKotak(i)) = True) Then
                    cek = False             'andalso --> checking first statement before going to second statement
                    Exit For
                End If
            Next
        End If

        'final decision
        If cek = True Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Get bounding area
    ''' </summary>
    Private Sub GetBoundingArea(ByVal pointer As Integer, ByVal position As Point3D)
        Dim i As Integer
        Dim longWidth, longDepth, longHeight As Single

        'get maximum value
        'from kotak
        longWidth = 0 : longDepth = 0 : longHeight = 0
        For i = 1 To FKotak.GetUpperBound(0)
            If (FKotakInEmptyspace(i) = True) AndAlso (longDepth < FKotak(i).Position2.X) Then longDepth = FKotak(i).Position2.X
            If (FKotakInEmptyspace(i) = True) AndAlso (longWidth < FKotak(i).Position2.Y) Then longWidth = FKotak(i).Position2.Y
            If (FKotakInEmptyspace(i) = True) AndAlso (longHeight < FKotak(i).Position2.Z) Then longHeight = FKotak(i).Position2.Z
        Next
        If longDepth < FKotak(pointer).Position2.X Then longDepth = FKotak(pointer).Position2.X
        If longWidth < FKotak(pointer).Position2.Y Then longWidth = FKotak(pointer).Position2.Y
        If longHeight < FKotak(pointer).Position2.Z Then longHeight = FKotak(pointer).Position2.Z

        'get the bounding area
        FBoundingArea = New Kotak(longWidth, longDepth, longHeight)
    End Sub

    ''' <summary>
    ''' Placement kotak
    ''' </summary>
    Public Sub SetPlacement(ByVal pointer As Integer, ByVal position As Point3D, ByVal orientation As Boolean)
        FKotak(pointer).Orientation = orientation
        FKotak(pointer).Position = position
        UpdatePlacement()
    End Sub

    ''' <summary>
    ''' Update placement position after set
    ''' </summary>
    Private Sub UpdatePlacement()
        Dim i, j As Integer

        '#counting kotak dalam plot
        j = 0
        For i = 1 To FKotak.GetUpperBound(0)
            If FKotakInEmptyspace(i) = True Then j += 1
        Next

        Dim tempPlacementKiriAtas(j), tempPlacementKananBawah(j) As Point3D          'always create 2 point

        '#get koordinat: kiri-atas, kanan-bawah
        j = 0
        For i = 1 To FKotak.GetUpperBound(0)
            If FKotakInEmptyspace(i) = True Then
                j += 1
                tempPlacementKiriAtas(j) = New Point3D(FKotak(i).Position.X, FKotak(i).Position2.Y, FKotak(i).Position.Z)
                tempPlacementKananBawah(j) = New Point3D(FKotak(i).Position2.X, FKotak(i).Position.Y, FKotak(i).Position2.Z)
            End If
        Next

        '#filtering
        For i = 1 To FKotak.GetUpperBound(0)
            If FKotakInEmptyspace(i) = True Then
                '1. filtering X
                'posisi titik kiri-atas harus punya ruang kosong disebelah serong kanan (lihat gambar)\
                '
                '  |---------------------|
                '  |---------------------|
                '  |===========|
                '  |           |
                '  |===========|
                For j = 1 To tempPlacementKiriAtas.GetUpperBound(0)
                    If (FKotak(i).Position.Y = tempPlacementKiriAtas(j).Y) AndAlso (FKotak(i).Position.X <= tempPlacementKiriAtas(j).X) AndAlso (tempPlacementKiriAtas(j).X < FKotak(i).Position2.X) Then
                        tempPlacementKiriAtas(j).X = -1
                        tempPlacementKiriAtas(j).Y = -1
                    End If
                Next

                '2. filtering Y
                'posisi titik kiri-atas harus punya ruang kosong disebelah serong kanan (lihat gambar)\
                '
                '              |---------|
                '              |         |
                '  |===========|         |
                '  |           |         |
                '  |===========|---------|
                '
                For j = 1 To tempPlacementKananBawah.GetUpperBound(0)
                    If (FKotak(i).Position.X = tempPlacementKananBawah(j).X) AndAlso (FKotak(i).Position.Y <= tempPlacementKananBawah(j).Y) AndAlso (tempPlacementKananBawah(j).Y < FKotak(i).Position2.Y) Then
                        tempPlacementKananBawah(j).X = -1
                        tempPlacementKananBawah(j).Y = -1
                    End If
                Next
            End If
        Next

        '#offsetting
        Dim gapOffset As Single
        Dim pointerOffset As Integer

        '1. kiri-atas offsetting
        For i = 1 To tempPlacementKiriAtas.GetUpperBound(0)
            'reset data
            gapOffset = tempPlacementKiriAtas(i).X - FEmptyArea.Position.X
            pointerOffset = 0

            'offsetting to nearest box/area --> getting minimum gap
            For j = 1 To FKotak.GetUpperBound(0)
                'condition:
                '1. kotak in plot area
                '2. tempplacement fisibel (not fail in filtering)
                '3. position2 area x before placement.x
                '4. at same depth
                '5. have minimum gapoffset than before value
                If (FKotakInEmptyspace(j) = True) AndAlso _
                    ((tempPlacementKiriAtas(i).X > -1) And (tempPlacementKiriAtas(i).Y > -1)) AndAlso _
                    (FKotak(j).Position2.X <= tempPlacementKiriAtas(i).X) AndAlso _
                    ((FKotak(j).Position.Y <= tempPlacementKiriAtas(i).Y) And (tempPlacementKiriAtas(i).Y < FKotak(j).Position2.Y)) AndAlso _
                    ((tempPlacementKiriAtas(i).X - FKotak(j).Position2.X) < gapOffset) Then
                    gapOffset = tempPlacementKiriAtas(i).X - FKotak(j).Position2.X
                    pointerOffset = j
                End If
                If gapOffset = 0 Then Exit For
            Next

            'write gapoffset value
            If pointerOffset = 0 Then
                tempPlacementKiriAtas(i).X = 0
            Else
                tempPlacementKiriAtas(i).X = FKotak(pointerOffset).Position2.X
            End If
        Next

        '2. kanan-bawah offsetting
        For i = 1 To tempPlacementKananBawah.GetUpperBound(0)
            'reset data
            gapOffset = tempPlacementKananBawah(i).Y - -FEmptyArea.Position.Y
            pointerOffset = 0

            'offsetting to nearest box/area --> getting minimum gap
            For j = 1 To FKotak.GetUpperBound(0)
                'condition:
                '1. kotak in plot area
                '2. tempplacement fisibel (not fail in filtering)
                '3. position2 area Y before placement.Y
                '4. at same width
                '5. have minimum gapoffset than before value
                If (FKotakInEmptyspace(j) = True) AndAlso _
                    ((tempPlacementKananBawah(i).Y > -1) And (tempPlacementKananBawah(i).Y > -1)) AndAlso _
                    (FKotak(j).Position2.Y <= tempPlacementKananBawah(i).Y) AndAlso _
                    ((FKotak(j).Position.X <= tempPlacementKananBawah(i).X) And (tempPlacementKananBawah(i).X < FKotak(j).Position2.X)) AndAlso _
                    ((tempPlacementKananBawah(i).Y - FKotak(j).Position2.Y) < gapOffset) Then
                    gapOffset = tempPlacementKananBawah(i).Y - FKotak(j).Position2.Y
                    pointerOffset = j
                End If
                If gapOffset = 0 Then Exit For
            Next

            'write gapoffset value
            If pointerOffset = 0 Then
                tempPlacementKiriAtas(i).X = 0
            Else
                tempPlacementKiriAtas(i).X = FKotak(pointerOffset).Position2.X
            End If
        Next

        '#update value
        Dim temp(tempPlacementKiriAtas.GetUpperBound(0) + tempPlacementKananBawah.GetUpperBound(0)) As Point3D
        j = 0
        For i = 1 To tempPlacementKiriAtas.GetUpperBound(0)
            If (tempPlacementKiriAtas(i).X > -1) And (tempPlacementKiriAtas(i).Y > -1) Then
                j += 1
                temp(j) = tempPlacementKiriAtas(i)
            End If
        Next
        For i = 1 To tempPlacementKananBawah.GetUpperBound(0)
            If (tempPlacementKananBawah(i).X > -1) And (tempPlacementKananBawah(i).Y > -1) Then
                j += 1
                temp(j) = tempPlacementKananBawah(i)
            End If
        Next
        ReDim Preserve temp(j)

        If temp.GetUpperBound(0) > 1 Then
            '#sorting value
            For i = 1 To temp.GetUpperBound(0) - 1
                For j = 1 To temp.GetUpperBound(0)
                    If (temp(i).X > temp(j).X) Or _
                        ((temp(i).X = temp(j).X) And (temp(i).Y > temp(j).Y)) Then _
                        Swap(temp(i), temp(j))
                Next
            Next

            '#reupdate value --eliminate same coordinate
            j = 1
            For i = 2 To temp.GetUpperBound(0)
                If (temp(i).X <> temp(j).X) Or (temp(i).Y <> temp(j).Y) Then
                    j += 1
                    temp(j) = temp(i)
                End If
            Next
        End If

        'uploading data to variable
        ReDim FPlacement(j)
        For i = 1 To j
            FPlacement(i) = New Point3D(temp(i).X, temp(i).Y, temp(i).Z)
        Next
    End Sub


End Class
