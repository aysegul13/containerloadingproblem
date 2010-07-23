
Public Class Plot3D
    ''' <summary>
    ''' Emptyspace in container (box type)
    ''' </summary>
    Private FEmptySpaceBox() As Box
    ''' <summary>
    ''' Box in container
    ''' </summary>
    Private FBox() As Box
    ''' <summary>
    ''' Initial emptyspace --> container
    ''' </summary>
    Private FInitialEmptySpace As Point3D
    ''' <summary>
    ''' Contour empty space
    ''' </summary>
    Private FEmptySpaceArea() As Contour
    ''' <summary>
    ''' BoundingBox in container --> similiar with box
    ''' </summary>
    Private FBoundingBox(Nothing) As Box

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Sub New(ByVal wCon As Single, ByVal dCon As Single, ByVal hCon As Single)
        FInitialEmptySpace = New Point3D(dCon, wCon, hCon)

        'initial data --> as container
        ReDim FBox(0)
        FBox(0) = New Box(-1, wCon, hCon, dCon, CByte(1))

        'make empty space
        ReDim FEmptySpaceBox(1)
        FEmptySpaceBox(1) = New Box(0, wCon, hCon, dCon, CByte(1))
        FEmptySpaceBox(1).LocationContainer = New Point3D(0, 0, 0)
        FEmptySpaceBox(1).LocationTemp = New Point3D(0, 0, 0)

        ReDim FEmptySpaceArea(1)
        FEmptySpaceArea(1) = New Contour(FEmptySpaceBox(1))
    End Sub

    ''' <summary>
    ''' Get utilization
    ''' </summary>
    Public ReadOnly Property Utilization() As Single
        Get
            Return (VolumeBox / VolumeContainer)
        End Get
    End Property

    ''' <summary>
    ''' Read box data
    ''' </summary>
    Public ReadOnly Property Box(ByVal i As Integer) As Box
        Get
            Return FBox(i)
        End Get
    End Property

    ''' <summary>
    ''' Get volume box
    ''' </summary>
    Public ReadOnly Property VolumeBox() As Single
        Get
            Return GetVolume()
        End Get
    End Property

    ''' <summary>
    ''' Get volume container
    ''' </summary>
    Public ReadOnly Property VolumeContainer() As Single
        Get
            Return (FInitialEmptySpace.X * FInitialEmptySpace.Y * FInitialEmptySpace.Z)
        End Get
    End Property

    ''' <summary>
    ''' Get box in collision; TRUE --&gt; collision, FALSE --&gt; free of colision
    ''' </summary>
    Public ReadOnly Property StatusCollision() As Boolean
        Get

        End Get
    End Property

    Public ReadOnly Property EmptySpace(ByVal i As Integer) As Box
        Get
            Return FEmptySpaceBox(i)
        End Get
    End Property

    Public ReadOnly Property CountEmptySpace() As Integer
        Get
            Return FEmptySpaceBox.GetUpperBound(0)
        End Get
    End Property

    ''' <summary>
    ''' Output data for printing
    ''' </summary>
    Public ReadOnly Property Output() As Box()
        Get
            Return FBox
        End Get
    End Property

    ''' <summary>
    ''' Set input new box in container
    ''' </summary>
    Public Sub SetNewBox(ByVal EmptySpace As Box, ByVal inputBox() As Box)
        'resize input box due added of box
        ReDim Preserve inputBox(FBox.GetUpperBound(0) + inputBox.GetUpperBound(0))

        'update value for box incontainer
        UpdatePositionInContainer(New Point3D(EmptySpace.LocationContainer.X, EmptySpace.LocationContainer.Y, EmptySpace.LocationContainer.Z), inputBox)

        'set new input
        For i As Integer = (FBox.GetUpperBound(0) + 1) To inputBox.GetUpperBound(0)
            FBox(i) = New Box(inputBox(i - FBox.GetUpperBound(0)))
        Next
    End Sub

    ''' <summary>
    ''' Set input new box + output mechanism
    ''' </summary>
    Public Sub SetNewBox(ByVal emptySpace As Box, ByVal inputBox() As Box, ByVal boundingBox As Box, ByRef outputBox() As Box)
        Dim i, j, l As Integer

        'separating box InContainer and not; inputbox --> incontainer, outbox --> not incontainer
        j = 0
        l = 0
        ReDim outputBox(inputBox.GetUpperBound(0))
        For i = 1 To inputBox.GetUpperBound(0)
            If inputBox(i).InContainer = True Then
                j += 1
                inputBox(j) = New Box(inputBox(i))
            Else
                l += 1
                outputBox(l) = New Box(inputBox(i))
            End If
        Next
        'resize output data
        ReDim Preserve inputBox(j)
        ReDim Preserve outputBox(l)

        'update value for box incontainer + boundingbox position in container
        UpdatePositionInContainer(New Point3D(emptySpace.LocationContainer.X, emptySpace.LocationContainer.Y, emptySpace.LocationContainer.Z), inputBox)
        boundingBox.LocationContainer = New Point3D(boundingBox.LocationContainer.X + emptySpace.LocationContainer.X, _
                                                    boundingBox.LocationContainer.Y + emptySpace.LocationContainer.Y, _
                                                    boundingBox.LocationContainer.Z + emptySpace.LocationContainer.Z)

        'resize FBox + bounding box
        j = FBox.GetUpperBound(0)
        ReDim Preserve FBox(FBox.GetUpperBound(0) + inputBox.GetUpperBound(0))
        ReDim Preserve FBoundingBox(FBoundingBox.GetUpperBound(0) + 1)

        'adding box FBox from FInput
        For i = 1 To inputBox.GetUpperBound(0)
            If inputBox(i).InContainer = True Then
                j += 1
                FBox(j) = New Box(inputBox(i))
            End If
        Next
        'adding new bounding box
        FBoundingBox(FBoundingBox.GetUpperBound(0)) = New Box(boundingBox)

        'update placement
        UpdateEmptySpace(inputBox, boundingBox)
    End Sub

    ''' <summary>
    ''' Get empty space --> taken from emptyarea
    ''' </summary>
    Public Sub GetEmptySpace()
        Dim i, j, k As Integer

        'counting much emptyspace
        j = 0
        For i = 1 To FEmptySpaceArea.GetUpperBound(0)
            j += FEmptySpaceArea(i).EmptySpace.GetUpperBound(0)
        Next

        'resize as count
        ReDim FEmptySpaceBox(j)

        'fill emptyspace
        k = 0
        For i = 1 To FEmptySpaceArea.GetUpperBound(0)
            With FEmptySpaceArea(i)
                For j = 1 To .EmptySpace.GetUpperBound(0)
                    k += 1
                    FEmptySpaceBox(k) = New Box(-1, .EmptySpace(j).Width, (FInitialEmptySpace.Z - .EmptySpace(j).Height), .EmptySpace(j).Depth, CByte(1))
                    FEmptySpaceBox(k).LocationContainer = New Point3D(.EmptySpace(j).Position)
                    FEmptySpaceBox(k).LocationTemp = New Point3D(0, 0, 0)
                Next
            End With
        Next

        'argghh.. akhirnya finish juga... ayo coba!!
    End Sub

    ''' <summary>
    ''' Generate new emptyspace
    ''' </summary>
    Private Sub UpdateEmptySpace(ByVal inputBox() As Box, ByVal boundingBox As Box)
        'abis box dimasukkin... untuk menentukan keadaan spacial, diperlukan algoritma sebagai berikut:
        '1. pemetaan ruang 2D berdasarkan ketinggian box.
        '   -diketahui box mana yang berada pada ketinggian nol
        '   -diketahui box dan spacial pada ketinggian tertentu; paling enak sih sebenernya kalo pas pertama aja identifikasinya
        '   ++logika sederhana
        '   ---saat pemetaan maka terbentuk 2 spacial: 1 spacial pada ketinggian Zmin, dan 1 spacial pada ketinggian Zmax
        '   ---spacial pada ketinggian tersebut dijadikan acuan
        '   ---yang masalah kalo spacial pada ketinggian yang sama cara nyatuinnya gimana?
        '   ---bisa diketahui dari empty space yang digunakan; jadi bisa ditrace itu masuk ke area2D yang mana
        '   ---ok.. tapi ngasi taunya itu cuboid sehingga perlu bisa digabung gimana ya?
        '
        '- kalo asal jadi bisa sih.. tinggal masukkin input cuboid, aja... biar ga bingung...
        '- jadi pas akhirnya dibuat cuboid box aja
        '
        '2. placement biasa seperti plot2D
        '   -ini perlu perubahan kalo misalnya box mengakomodasi adanya tolerasnsi
        '
        '3. generate empty space and update array
        '   -ya ini sbenernya biasa aj sih.. agak ribet pas update datanya aja

        '----
        '#khusus untuk cuboid... biar ga bingung namanya juga nyoba aja
        '1. build new contour (resize + construct)
        ReDim Preserve FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0) + 1)
        FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0)) = New Contour(inputBox, New Point3D(boundingBox.LocationContainer.X, boundingBox.LocationContainer.Y, boundingBox.LocationContainer2.Z))

        '2. resize contour (for empty space area)
        'harus dapetin dulu ini di contour yang mana... arghh..sebel.
        Dim restContour(Nothing) As Line3D
        For i As Integer = 1 To FEmptySpaceArea.GetUpperBound(0)
            If FEmptySpaceArea(i).CheckBoxInContour(inputBox(1)) = True Then
                'insert in new box
                FEmptySpaceArea(i).SetNewBox(inputBox, New Point3D(boundingBox.LocationContainer), restContour)

                '-iterate until no contour left
                Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
                    ReDim Preserve FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0) + 1)
                    FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0)) = New Contour(restContour, False)
                Loop
            End If
        Next
    End Sub

    ''' <summary>
    ''' Get utilization
    ''' </summary>
    Private Function GetVolume() As Single
        Dim i As Integer
        Dim vol As Single = 0
        For i = 1 To FBox.GetUpperBound(0)
            vol += FBox(i).Width * FBox(i).Depth * FBox(i).Height
        Next
        Return vol
    End Function

    ''' <summary>
    ''' Update position in container
    ''' </summary>
    Private Sub UpdatePositionInContainer(ByVal position As Point3D, ByRef dataBox() As Box)
        For i As Integer = 1 To dataBox.GetUpperBound(0)
            dataBox(i).LocationContainer = New Point3D(position.X + dataBox(i).LocationTemp.X, position.Y + dataBox(i).LocationTemp.Y, position.Z + dataBox(i).LocationTemp.Z)
        Next
    End Sub
End Class
