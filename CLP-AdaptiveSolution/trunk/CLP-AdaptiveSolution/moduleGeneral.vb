Imports System
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' Box
''' </summary>
Public Structure ListBox
    Dim SCount, SType As Integer
End Structure

Module General
    Public Function ShallowClone(ByVal instance As ICloneable) As Object
        Return instance.Clone
    End Function

    ' This is one way to do deep cloning. But works only if the 
    ' objects and its references are serializable
    Public Function DeepClone(ByVal obj As Object) As Object
        Dim m As New MemoryStream
        Dim b As New BinaryFormatter
        'serialize the object into the stream.
        b.Serialize(m, obj)
        'position streem pointer back to first byte.
        m.Position = 0
        'deserialize into another object.
        Return b.Deserialize(m)
        'release memory
        m.Close()
    End Function


    Public Sub Recapitulation(ByVal inputbox() As Box, ByRef datalistbox() As ListBox)
        Dim i, j As Integer

        Dim temp() As Integer
        Dim tempBox(,) As Integer

        ReDim temp(inputbox.GetUpperBound(0))
        For i = 1 To inputbox.GetUpperBound(0)
            temp(i) = inputbox(i).Type
        Next

        'input dataSORT
        Array.Sort(temp, 1, temp.GetUpperBound(0))      'sorting array from item 1 to getupperbound
        ReDim tempBox(temp.GetUpperBound(0), 2)
        'start from null
        j = 0
        tempBox(j, 1) = 0
        tempBox(j, 2) = 0
        For i = 1 To temp.GetUpperBound(0)
            If temp(i) <> tempBox(j, 1) Then
                j += 1
                tempBox(j, 1) = temp(i)
                tempBox(j, 2) = 1
            Else
                tempBox(j, 2) += 1
            End If
        Next

        ReDim datalistbox(j)
        For i = 1 To j
            datalistbox(i).SType = tempBox(i, 1)
            datalistbox(i).SCount = tempBox(i, 2)
        Next
    End Sub



    Public Sub Swap(ByRef object1 As Object, ByRef object2 As Object)
        Dim objecttemp As Object
        objecttemp = object1
        object1 = object2
        object2 = objecttemp
    End Sub

    Public Function Max(ByVal object1 As Object, ByVal object2 As Object) As Object
        If object1 > object2 Then
            Return object1
        Else
            Return object2
        End If
    End Function

    Public Function Max3(ByVal object1 As Object, ByVal object2 As Object, ByVal object3 As Object) As Object
        Return Max(object1, Min(object2, object3))
    End Function

    Public Function Min(ByVal object1 As Object, ByVal object2 As Object) As Object
        If object1 > object2 Then
            Return object2
        Else
            Return object1
        End If
    End Function

    Public Function Min3(ByVal object1 As Object, ByVal object2 As Object, ByVal object3 As Object) As Object
        Return Min(object1, Min(object2, object3))
    End Function

    Public Class MyForm
        Public Shared formMainMenu As New MainMenu
    End Class

    Public Sub printBox(ByVal solution() As Box, ByVal orientation As Boolean)
        'getting the bounding box
        Dim initiate As New Cube(0, 0, solution(0).Height, solution(0).Width, solution(0).Depth, 1, 1)

        'getting the bounding box.. will help to scaling picture box
        Dim picWidth, picHeight As Integer
        picWidth = initiate.BoundsRect.Width - initiate.Width
        picHeight = initiate.BoundsRect.Height - initiate.Height

        'get scaling
        'methos scaling is by using scale-ratio variable
        'ex: origin*scale-ratio = template --> scale ratio = template / origin
        '---if origin < template --> scale-ratio > 1
        '---if origin > template --> scale-ratio < 1
        '---if 2 origin, pic the minimum scale-ratio
        Dim sclRatio As Single
        If (MyForm.formMainMenu.picResult.Width / initiate.BoundsRect.Width) _
            < (MyForm.formMainMenu.picResult.Height / initiate.BoundsRect.Height) Then
            sclRatio = (MyForm.formMainMenu.picResult.Width - 1) / initiate.BoundsRect.Width
        Else
            sclRatio = (MyForm.formMainMenu.picResult.Height - 1) / initiate.BoundsRect.Height
        End If


        'scale and draw all cube
        Dim bm As New Bitmap(MyForm.formMainMenu.picResult.Width, MyForm.formMainMenu.picResult.Height)
        Dim gr As Graphics = Graphics.FromImage(bm)

        'position in cube class
        'x = 0 --> for width
        'y = 0 --> for height
        '
        '0,0,0 = {origin}
        ' posX = 0
        ' posY = 0
        ' posZ = initiate.height * sclratio
        '
        'how to get 
        Dim drawCube(solution.GetUpperBound(0)) As Cube

        If orientation = True Then
            '--container
            drawCube(0) = New Cube(0, 0, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, 1, 1)

            '--box
            For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
                drawCube(i) = New Cube(PositionInPicture(sclRatio, solution(i).LocationContainer, solution(i).Height, initiate.Height, 1, 1), _
                                       solution(i).Height * sclRatio, _
                                       solution(i).Width * sclRatio, _
                                       solution(i).Depth * sclRatio, _
                                       1, 1)
            Next
        Else
            'masi blon bener..males..hoohoo
            '--container
            drawCube(0) = New Cube(0, initiate.BoundsRect.Height, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, -1, -1)

            '--box
            For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
                drawCube(i) = New Cube(PositionInPicture(sclRatio, solution(i).LocationContainer, solution(i).Height, initiate.Height, -1, -1), _
                                       solution(i).Height * sclRatio, _
                                       solution(i).Width * sclRatio, _
                                       solution(i).Depth * sclRatio, _
                                       -1, -1)
            Next
        End If

        'drawing container + cube
        gr.DrawPath(Pens.Red, drawCube(0).GetContainer)
        For i As Integer = 1 To drawCube.GetUpperBound(0)
            gr.DrawPath(Pens.Blue, drawCube(i).GetCube)
        Next

        MyForm.formMainMenu.picResult.Image = bm
        gr.Dispose()
    End Sub

    Public Sub algDrawing(ByVal solution() As Box, ByVal orientation As Boolean)
        'getting the bounding box
        Dim initiate As New Cube(0, 0, solution(0).Height, solution(0).Width, solution(0).Depth, 1, 1)

        'getting the bounding box.. will help to scaling picture box
        Dim picWidth, picHeight As Integer
        picWidth = initiate.BoundsRect.Width - initiate.Width
        picHeight = initiate.BoundsRect.Height - initiate.Height

        'get scaling
        'methos scaling is by using scale-ratio variable
        'ex: origin*scale-ratio = template --> scale ratio = template / origin
        '---if origin < template --> scale-ratio > 1
        '---if origin > template --> scale-ratio < 1
        '---if 2 origin, pic the minimum scale-ratio
        Dim sclRatio As Single
        If (MyForm.formMainMenu.picResult.Width / initiate.BoundsRect.Width) _
            < (MyForm.formMainMenu.picResult.Height / initiate.BoundsRect.Height) Then
            sclRatio = (MyForm.formMainMenu.picResult.Width - 1) / initiate.BoundsRect.Width
        Else
            sclRatio = (MyForm.formMainMenu.picResult.Height - 1) / initiate.BoundsRect.Height
        End If


        'scale and draw all cube
        Dim bm As New Bitmap(MyForm.formMainMenu.picResult.Width, MyForm.formMainMenu.picResult.Height)
        Dim gr As Graphics = Graphics.FromImage(bm)

        'position in cube class
        'x = 0 --> for width
        'y = 0 --> for height
        '
        '0,0,0 = {origin}
        ' posX = 0
        ' posY = 0
        ' posZ = initiate.height * sclratio
        '
        'how to get 
        Dim drawCube(solution.GetUpperBound(0)) As Cube

        If orientation = True Then
            '--container
            drawCube(0) = New Cube(0, 0, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, 1, 1)

            '--box
            For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
                drawCube(i) = New Cube(PositionInPicture(sclRatio, solution(i).LocationContainer, solution(i).Height, initiate.Height, 1, 1), _
                                       solution(i).Height * sclRatio, _
                                       solution(i).Width * sclRatio, _
                                       solution(i).Depth * sclRatio, _
                                       1, 1)
            Next
        Else
            'masi blon bener..males..hoohoo
            '--container
            drawCube(0) = New Cube(0, initiate.BoundsRect.Height, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, -1, -1)

            '--box
            For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
                drawCube(i) = New Cube(PositionInPicture(sclRatio, solution(i).LocationContainer, solution(i).Height, initiate.Height, -1, -1), _
                                       solution(i).Height * sclRatio, _
                                       solution(i).Width * sclRatio, _
                                       solution(i).Depth * sclRatio, _
                                       -1, -1)
            Next
        End If

        'drawing container + cube
        gr.DrawPath(Pens.Red, drawCube(0).GetContainer)
        For i As Integer = 1 To drawCube.GetUpperBound(0)
            gr.DrawPath(Pens.Blue, drawCube(i).GetCube)
        Next

        '--bounding box draw TEMPORARY
        gr.DrawPath(Pens.Green, drawCube(drawCube.GetUpperBound(0)).GetCube)

        MyForm.formMainMenu.picResult.Image = bm
        gr.Dispose()
    End Sub

    Private Function PositionInPicture(ByVal scl As Single, ByVal positionContainer As Point3D, _
                             ByVal height As Integer, ByVal heightContainer As Integer, _
                             ByVal rotateX As RotateHorizontal, ByVal rotateY As RotateVertical) As Point
        Dim x, y, z As Single
        x = positionContainer.X * scl
        y = positionContainer.Y * scl
        z = heightContainer - positionContainer.Z

        'position diubah sehingga titik cube ke atas (tambahin Z aja)
        z = (z - height) * scl
        'position diubah sehingga ke titik origin


        'transfer position
        PositionInPicture.X = y + (x / 2 * rotateX) 'width position
        PositionInPicture.Y = z + (x / 2 * rotateY) 'height position
    End Function

    ''' <summary>
    ''' Checking collision between 2 area in 2Dplot
    ''' True --> collision
    ''' </summary>
    Public Function CheckingCollision2D(ByVal area1 As Kotak, ByVal area2 As Kotak) As Boolean
        'if overlap on all axes --> area overlap
        If (area1.Position2.Y <= area2.Position.Y) Or (area2.Position2.Y <= area1.Position.Y) Then  'cek overlap in vertical
            Return False
        ElseIf (area1.Position2.X <= area2.Position.X) Or (area2.Position2.X <= area1.Position.X) Then  'cek overlap in horizontal
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' Checking collision between 2 area in 2Dplot
    ''' True --> collision
    ''' </summary>
    Public Function CheckingAreaInBound2D(ByVal area1 As Kotak, ByVal area2 As Kotak) As Boolean
        'if overlap on all axes --> area overlap
        If CheckingCollision2D(area1, area2) = True Then
            Dim point(4) As Point3D

            'reset data --> true = in bound
            Dim cek As Boolean = True

            'area1 in area2?
            point(1) = New Point3D(area1.Position)
            point(2) = New Point3D(area1.Position.X + area1.Depth, area1.Position.Y, area1.Position.Z)
            point(3) = New Point3D(area1.Position2)
            point(4) = New Point3D(area1.Position.X, area1.Position.Y + area1.Width, area1.Position.Z)
            For i As Integer = 1 To 4
                If (area2.Position.X <= point(i).X) And (point(i).X <= area2.Position2.X) And _
                   (area2.Position.Y <= point(i).Y) And (point(i).Y <= area2.Position2.Y) Then
                    cek = True
                Else
                    cek = False
                End If
            Next
            'give value
            Return cek
        Else
            Return False
        End If
    End Function
End Module

