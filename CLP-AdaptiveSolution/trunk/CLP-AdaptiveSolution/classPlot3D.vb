
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
    Private fCollision As Boolean
    Private fOutContainer As Integer

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Sub New(ByVal dCon As Single, ByVal wCon As Single, ByVal hCon As Single)
        FInitialEmptySpace = New Point3D(dCon, wCon, hCon)

        'initial data --> as container
        ReDim FBox(0)
        FBox(0) = New Box(-1, dCon, wCon, hCon)

        'make empty space
        ReDim FEmptySpaceBox(1)
        FEmptySpaceBox(1) = New Box(0, dCon, wCon, hCon)
        FEmptySpaceBox(1).AbsPos1 = New Point3D(0, 0, 0)
        FEmptySpaceBox(1).RelPos1 = New Point3D(0, 0, 0)

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
    ''' Get box in collision; TRUE --> collision, FALSE --> free of colision
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
        UpdatePositionInContainer(New Point3D(EmptySpace.AbsPos1.X, EmptySpace.AbsPos1.Y, EmptySpace.AbsPos1.Z), inputBox)

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
        j = 0 : l = 0
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

        'update value for box incontainer
        UpdatePositionInContainer(New Point3D(emptySpace.AbsPos1.X, emptySpace.AbsPos1.Y, emptySpace.AbsPos1.Z), inputBox)

        'update boundingbox position in container
        boundingBox.AbsPos1 = New Point3D(boundingBox.AbsPos1.X + emptySpace.AbsPos1.X, _
                                                    boundingBox.AbsPos1.Y + emptySpace.AbsPos1.Y, _
                                                    boundingBox.AbsPos1.Z + emptySpace.AbsPos1.Z)

        'resize FBox + bounding box
        'FBox --> box that have been placed in container
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
                    FEmptySpaceBox(k) = New Box(-1, .EmptySpace(j).Depth, .EmptySpace(j).Width, (FInitialEmptySpace.Z - .EmptySpace(j).Height))
                    FEmptySpaceBox(k).AbsPos1 = New Point3D(.EmptySpace(j).Position)
                    FEmptySpaceBox(k).RelPos1 = New Point3D(0, 0, 0)
                Next
            End With
        Next

        'update empty space that volume =0
        j = 0
        For i = 1 To FEmptySpaceBox.GetUpperBound(0)
            With FEmptySpaceBox(i)
                If (.Depth * .Width * .Height) > 0 Then
                    j += 1
                    If i <> j Then
                        FEmptySpaceBox(j) = New Box(-1, .Depth, .Width, .Height)
                        FEmptySpaceBox(j).AbsPos1 = New Point3D(FEmptySpaceBox(i).AbsPos1)
                    End If
                End If
            End With
        Next
        ReDim Preserve FEmptySpaceBox(j)

        'argghh.. akhirnya finish juga... ayo coba!!
    End Sub

    ''' <summary>
    ''' Generate new emptyspace
    ''' </summary>
    ''' <remarks>Explanation:
    ''' abis box dimasukkin... untuk menentukan keadaan spacial, diperlukan algoritma sebagai berikut:
    ''' 1. pemetaan ruang 2D berdasarkan ketinggian box.
    '''   -diketahui box mana yang berada pada ketinggian nol
    '''   -diketahui box dan spacial pada ketinggian tertentu; paling enak sih sebenernya kalo pas pertama aja identifikasinya
    '''   ++logika sederhana
    '''   ---saat pemetaan maka terbentuk 2 spacial: 1 spacial pada ketinggian Zmin, dan 1 spacial pada ketinggian Zmax
    '''   ---spacial pada ketinggian tersebut dijadikan acuan
    '''   ---yang masalah kalo spacial pada ketinggian yang sama cara nyatuinnya gimana?
    '''   ---bisa diketahui dari empty space yang digunakan; jadi bisa ditrace itu masuk ke area2D yang mana
    '''   ---ok.. tapi ngasi taunya itu cuboid sehingga perlu bisa digabung gimana ya?
    '''
    ''' - kalo asal jadi bisa sih.. tinggal masukkin input cuboid, aja... biar ga bingung...
    ''' - jadi pas akhirnya dibuat cuboid box aja
    '''
    ''' 2. placement biasa seperti plot2D
    '''   -ini perlu perubahan kalo misalnya box mengakomodasi adanya tolerasnsi
    '''
    ''' 3. generate empty space and update array
    '''   -ya ini sbenernya biasa aj sih.. agak ribet pas update datanya aja
    '''</remarks>
    Private Sub UpdateEmptySpace(ByVal inputBox() As Box, ByVal boundingBox As Box)

        'kita manipulasi aja deh..
        ReDim inputBox(1)
        inputBox(1) = New Box(boundingBox)


        '===
        'Try
        '1. build new contour (resize + construct)
        Dim restContour(Nothing) As Line3D
        ReDim Preserve FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0) + 1)
        FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0)) = New Contour(inputBox, New Point3D(boundingBox.AbsPos1.X, boundingBox.AbsPos1.Y, boundingBox.AbsPos2.Z), restContour)
        Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
            ReDim Preserve FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0) + 1)
            FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0)) = New Contour(restContour, True)
        Loop
        'Catch ex As Exception
        '    MyForm.formMainMenu.txtConsole.Text = "error di buildnew contour --> " & MyForm.formMainMenu.txtConsole.Text
        '    Stop
        'End Try

        '===
        'Try
        '2. resize contour (for empty space area)
        'harus dapetin dulu ini di contour yang mana... arghh..sebel.
        Dim i, j As Integer
        For i = 1 To FEmptySpaceArea.GetUpperBound(0)
            For j = 1 To inputBox.GetUpperBound(0)
                If FEmptySpaceArea(i).CheckBoxInContour(inputBox(j)) = True Then
                    'insert in new box
                    FEmptySpaceArea(i).SetNewBox(inputBox, New Point3D(boundingBox.AbsPos1), restContour)
                    '-iterate until no contour left
                    Do Until (restContour.GetUpperBound(0) = 0) Or (restContour Is Nothing)
                        ReDim Preserve FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0) + 1)
                        FEmptySpaceArea(FEmptySpaceArea.GetUpperBound(0)) = New Contour(restContour, False)
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
        If FBox.GetUpperBound(0) < 2 Then
            fCollision = False
        Else
            Do Until (cek = True) Or (i = FBox.GetUpperBound(0))
                i += 1
                j = i
                Do Until (cek = True) Or (j = FBox.GetUpperBound(0))
                    j += 1
                    cek = functCheckCollision3D(FBox(i), FBox(j))
                Loop
            Loop
            If cek = True Then
                fCollision = True
            Else
                fCollision = False
            End If
        End If

        '(2)
        If FBox.GetUpperBound(0) = 0 Then
            fOutContainer = False
        Else
            For i = 1 To FBox.GetUpperBound(0)
                If functCheckBoxInBound(FBox(i), FBox(0)) = False Then
                    fOutContainer = True
                    Exit For
                End If
            Next
        End If
    End Sub
End Class
