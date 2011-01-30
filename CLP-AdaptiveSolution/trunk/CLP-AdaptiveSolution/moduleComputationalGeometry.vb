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
''' moduleComputationalGeometry.vb
''' ---
''' list:
''' 1. 
''' </summary>
Public Module ComputationalGeometry
    ''' <summary>
    ''' #fCheckCollision2D code
    ''' -Checking collision between 2 area in 2Dplot
    ''' -True --> collision
    ''' --0. Parameter set
    ''' --1. Checking collision in vertical
    ''' --2. Checking collision in horizontal
    ''' </summary>
    Public Function functCheckCollision2D(ByVal area1 As Kotak, _
                                         ByVal area2 As Kotak) As Boolean
        If (area1.Position2.Y <= area2.Position.Y) Or (area2.Position2.Y <= area1.Position.Y) Then '(1)
            Return False
        ElseIf (area1.Position2.X <= area2.Position.X) Or (area2.Position2.X <= area1.Position.X) Then  '(2)
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' #fCheckCollision3D code
    ''' -Checking collision between 2 box in 3Dplot
    ''' -True --> collision
    ''' --0. Parameter set
    ''' --1. Checking collision in sb.Z
    ''' --2. Checking collision in sb.Y
    ''' --3. Checking collision in sb.X
    ''' </summary>
    Public Function functCheckCollision3D(ByVal box1 As Box, _
                                         ByVal box2 As Box) As Boolean
        If (box1.AbsPos2.Z <= box2.AbsPos1.Z) Or (box2.AbsPos2.Z <= box1.AbsPos1.Z) Then '(1)
            Return False
        ElseIf (box1.AbsPos2.Y <= box2.AbsPos1.Y) Or (box2.AbsPos2.Y <= box1.AbsPos1.Y) Then '(2)
            Return False
        ElseIf (box1.AbsPos2.X <= box2.AbsPos1.X) Or (box2.AbsPos2.X <= box1.AbsPos1.X) Then '(3)
            Return False
        Else
            Return True
        End If
    End Function


    ''' <summary>
    ''' #fCheckAreaInBound3D code
    ''' -Checking collision between 2 area in 3Dplot
    ''' -True --> collision
    ''' --0. Parameter set
    ''' --1. Checking collision
    ''' --2. Variable set
    ''' --- Cek (True=InBound)
    ''' --3. Checking Area1 C Area2
    ''' --4. Get value
    ''' </summary>
    Public Function functCheckBoxInBound(ByVal box1 As Box, _
                                         ByVal box2 As Box) As Boolean
        '(1)
        If functCheckCollision3D(box1, box2) = True Then
            '(2)
            Dim point(4) As Point3D
            Dim cek As Boolean = True

            '(3)
            point(1) = New Point3D(box1.AbsPos1)
            point(2) = New Point3D(box1.AbsPos2)
            For i As Integer = 1 To 2
                If (box2.AbsPos1.X <= point(i).X) And (point(i).X <= box2.AbsPos2.X) And _
                    (box2.AbsPos1.Y <= point(i).Y) And (point(i).Y <= box2.AbsPos2.Y) And _
                    (box2.AbsPos1.Z <= point(i).Z) And (point(i).Z <= box2.AbsPos2.Z) Then
                    cek = True
                Else
                    cek = False
                    Exit For
                End If
            Next

            '(4)
            Return cek
        Else
            '(4)
            Return False
        End If
    End Function

    ''' <summary>
    ''' #fCheckAreaInBound2D code
    ''' -Checking collision between 2 area in 2Dplot
    ''' -True --> collision
    ''' --0. Parameter set
    ''' --1. Checking collision
    ''' --2. Variable set
    ''' --- Cek (True=InBound)
    ''' --3. Checking Area1 C Area2
    ''' --4. Get value
    ''' </summary>
    Public Function functCheckAreaInBound(ByVal area1 As Kotak, _
                                           ByVal area2 As Kotak) As Boolean
        '(1)
        If functCheckCollision2D(area1, area2) = True Then
            '(2)
            Dim point(4) As Point3D
            Dim cek As Boolean = True

            '(3)
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
                    Exit For
                End If
            Next

            '(4)
            Return cek
        Else
            '(4)
            Return False
        End If
    End Function
End Module
