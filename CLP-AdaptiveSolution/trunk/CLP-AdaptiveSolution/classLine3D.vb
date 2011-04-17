Imports System.Collections.Generic
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
''' classLine3D.vb
''' -Object 'line' in 3D-space
''' -Vector
''' </summary>
Public Class Line3D
    ''' <summary>
    ''' Point A
    ''' </summary>
    Public fPoint1 As Point3D

    ''' <summary>
    ''' Point B
    ''' </summary>
    Public fPoint2 As Point3D

    ''' <summary>
    ''' Direction for vector
    ''' DepthLine --> true = right ; false = left
    ''' WidthLine --> true = up ;  false = down
    ''' HeightLine --> true = top ; false = bottom
    ''' </summary>
    Public fDirection As Boolean

    ''' <summary>
    ''' Standar constructor
    ''' </summary>
    Sub New(ByVal x1 As Single, ByVal y1 As Single, ByVal z1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single)
        fPoint1 = New Point3D(x1, y1, z1)
        fPoint2 = New Point3D(x2, y2, z2)
        Sort()
    End Sub

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Sub New(ByVal p1 As Point3D, ByVal p2 As Point3D)
        fPoint1 = New Point3D(p1)
        fPoint2 = New Point3D(p2)
        Sort()
    End Sub

    ''' <summary>
    ''' Clone constructor
    ''' </summary>
    Sub New(ByVal line As Line3D)
        fPoint1 = New Point3D(line.fPoint1)
        fPoint2 = New Point3D(line.fPoint2)
    End Sub

    ''' <summary>
    ''' Identify line is equal with compare line
    ''' </summary>
    Public Function IsEqualTo(ByVal lineCompare As Line3D) As Boolean
        If (fPoint1.IsEqualTo(lineCompare.fPoint1) = True) And (fPoint2.IsEqualTo(lineCompare.fPoint2) = True) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Get length of line
    ''' </summary>
    Public Function Length() As Single
        Dim xval As Double = fPoint2.X - fPoint1.X
        Dim yval As Double = fPoint2.Y - fPoint1.Y
        Dim zval As Double = fPoint2.Z - fPoint1.Z
        Return System.Math.Sqrt(xval * xval + yval * yval + zval * zval)
    End Function

    ''' <summary>
    ''' Sorting line
    ''' </summary>
    Private Sub Sort()
        If (fPoint1.X > fPoint2.X) Or _
            ((fPoint1.X = fPoint2.X) And (fPoint1.Y > fPoint2.Y)) Or _
            ((fPoint1.X = fPoint2.X) And (fPoint1.Y = fPoint2.Y) And (fPoint1.Z > fPoint2.Z)) _
            Then procSwap(fPoint1, fPoint2)
    End Sub

    ''' <summary>
    ''' True --> width line (y1, y2)
    ''' </summary>
    Public Function IsWidthLine() As Boolean
        If (fPoint1.X = fPoint2.X) And (fPoint1.Y <> fPoint2.Y) And (fPoint1.Z = fPoint2.Z) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' True --&gt; height line (z1, z2)
    ''' </summary>
    Public Function IsHeightLine() As Boolean
        If (fPoint1.X = fPoint2.X) And (fPoint1.Y = fPoint2.Y) And (fPoint1.Z <> fPoint2.Z) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' True --&gt; depth line (x1, x2)
    ''' </summary>
    Public Function IsDepthLine() As Boolean
        If (fPoint1.X <> fPoint2.X) And (fPoint1.Y = fPoint2.Y) And (fPoint1.Z = fPoint2.Z) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Comparing lower value... --> true = yes, false = undefine
    ''' </summary>
    Public Function IsLowerInLineThan(ByVal lineCompare As Line3D) As Boolean
        If ((Me.IsHeightLine = True) AndAlso (Me.IsHeightLine = lineCompare.IsHeightLine)) AndAlso _
            (((fPoint1.X < lineCompare.fPoint1.X) Or ((fPoint1.X = lineCompare.fPoint1.X) And (fPoint1.Y < lineCompare.fPoint1.Y))) Or _
             ((fPoint1.X = lineCompare.fPoint1.X) And (fPoint1.Y = lineCompare.fPoint1.Y) And (fPoint1.Z < lineCompare.fPoint1.Z))) Then
            Return True
        ElseIf ((Me.IsWidthLine = True) AndAlso (Me.IsWidthLine = lineCompare.IsWidthLine)) AndAlso _
            (((fPoint1.Z < lineCompare.fPoint1.Z) Or ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.X < lineCompare.fPoint1.X))) Or _
             ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.X = lineCompare.fPoint1.X) And (fPoint1.Y < lineCompare.fPoint1.Y))) Then
            Return True
        ElseIf ((Me.IsDepthLine = True) AndAlso (Me.IsDepthLine = lineCompare.IsDepthLine)) AndAlso _
            (((fPoint1.Z < lineCompare.fPoint1.Z) Or ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.Y < lineCompare.fPoint1.Y))) Or _
             ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.Y = lineCompare.fPoint1.Y) And (fPoint1.X < lineCompare.fPoint1.X))) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Comparing higher value... --> true = yes, false = undefine
    ''' </summary>
    Public Function IsHigherInLineThan(ByVal lineCompare As Line3D) As Boolean
        If ((Me.IsHeightLine = True) AndAlso (Me.IsHeightLine = lineCompare.IsHeightLine)) AndAlso _
            (((fPoint1.X > lineCompare.fPoint1.X) Or ((fPoint1.X = lineCompare.fPoint1.X) And (fPoint1.Y > lineCompare.fPoint1.Y))) Or _
            ((fPoint1.X = lineCompare.fPoint1.X) And (fPoint1.Y = lineCompare.fPoint1.Y) And (fPoint1.Z > lineCompare.fPoint1.Z))) Then
            Return True
        ElseIf ((Me.IsWidthLine = True) AndAlso (Me.IsWidthLine = lineCompare.IsWidthLine)) AndAlso _
            (((fPoint1.Z > lineCompare.fPoint1.Z) Or ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.X > lineCompare.fPoint1.X))) Or _
             ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.X = lineCompare.fPoint1.X) And (fPoint1.Y > lineCompare.fPoint1.Y))) Then
            Return True
        ElseIf ((Me.IsDepthLine = True) AndAlso (Me.IsDepthLine = lineCompare.IsDepthLine)) AndAlso _
            (((fPoint1.Z > lineCompare.fPoint1.Z) Or ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.Y > lineCompare.fPoint1.Y))) Or _
             ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.Y = lineCompare.fPoint1.Y) And (fPoint1.X > lineCompare.fPoint1.X))) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Adding line
    ''' </summary>
    Public Function GetUnion(ByVal Adder As Line3D) As Line3D
        Dim addLine As Line3D
        Dim minVal, maxVal As Single

        'reset
        addLine = New Line3D(0, 0, 0, 0, 0, 0)

        'cek intersection first
        'get min, max value
        If Me.IsIntersectionWith(adder) = True Then
            If Me.IsHeightLine = True Then
                minVal = fMin(fMin(fPoint1.Z, fPoint2.Z), fMin(adder.fPoint1.Z, adder.fPoint2.Z))
                maxVal = fMax(fMax(fPoint1.Z, fPoint2.Z), fMax(adder.fPoint1.Z, adder.fPoint2.Z))
                addLine = New Line3D(fPoint1.X, fPoint1.Y, minVal, fPoint1.X, fPoint1.Y, maxVal)
            End If
            If Me.IsDepthLine = True Then
                minVal = fMin(fMin(fPoint1.X, fPoint2.X), fMin(adder.fPoint1.X, adder.fPoint2.X))
                maxVal = fMax(fMax(fPoint1.X, fPoint2.X), fMax(adder.fPoint1.X, adder.fPoint2.X))
                addLine = New Line3D(minVal, fPoint1.Y, fPoint1.Z, maxVal, fPoint1.Y, fPoint1.Z)
            End If
            If Me.IsWidthLine = True Then
                minVal = fMin(fMin(fPoint1.Y, fPoint2.Y), fMin(adder.fPoint1.Y, adder.fPoint2.Y))
                maxVal = fMax(fMax(fPoint1.Y, fPoint2.Y), fMax(adder.fPoint1.Y, adder.fPoint2.Y))
                addLine = New Line3D(fPoint1.X, minVal, fPoint1.Z, fPoint1.X, maxVal, fPoint1.Z)
            End If
        Else
            addLine = New Line3D(0, 0, 0, 0, 0, 0)
        End If

        'return value
        Return addLine
    End Function

    ''' <summary>
    ''' Checking that line join with adder line
    ''' </summary>
    Public Function IsIntersectionWith(ByVal lineCompare As Line3D) As Boolean
        'prioritize to get intersection
        '1. heightline, depthline, widthline
        '2. 2-axis same position, 1-axis can be different
        '3. position for 1-axis different is a1<=b1<=a2 or a1<=b2<=a2

        If ((Me.IsHeightLine = True) AndAlso (Me.IsHeightLine = lineCompare.IsHeightLine)) AndAlso _
            ((fPoint1.X = lineCompare.fPoint1.X) And (fPoint1.Y = lineCompare.fPoint1.Y)) AndAlso _
            ((((fPoint1.Z <= lineCompare.fPoint1.Z) And (lineCompare.fPoint1.Z <= fPoint2.Z)) Or _
             ((fPoint1.Z <= lineCompare.fPoint2.Z) And (lineCompare.fPoint2.Z <= fPoint2.Z))) Or _
            (((lineCompare.fPoint1.Z <= fPoint1.Z) And (fPoint1.Z <= lineCompare.fPoint2.Z)) Or _
             ((lineCompare.fPoint1.Z <= fPoint2.Z) And (fPoint2.Z <= lineCompare.fPoint2.Z)))) Then
            Return True
        ElseIf ((Me.IsWidthLine = True) AndAlso (Me.IsWidthLine = lineCompare.IsWidthLine)) AndAlso _
            ((fPoint1.X = lineCompare.fPoint1.X) And (fPoint1.Z = lineCompare.fPoint1.Z)) AndAlso _
            ((((fPoint1.Y <= lineCompare.fPoint1.Y) And (lineCompare.fPoint1.Y <= fPoint2.Y)) Or _
             ((fPoint1.Y <= lineCompare.fPoint2.Y) And (lineCompare.fPoint2.Y <= fPoint2.Y))) Or _
            (((lineCompare.fPoint1.Y <= fPoint1.Y) And (fPoint1.Y <= lineCompare.fPoint2.Y)) Or _
             ((lineCompare.fPoint1.Y <= fPoint2.Y) And (fPoint2.Y <= lineCompare.fPoint2.Y)))) Then
            Return True
        ElseIf ((Me.IsDepthLine = True) AndAlso (Me.IsDepthLine = lineCompare.IsDepthLine)) AndAlso _
            ((fPoint1.Z = lineCompare.fPoint1.Z) And (fPoint1.Y = lineCompare.fPoint1.Y)) AndAlso _
            ((((fPoint1.X <= lineCompare.fPoint1.X) And (lineCompare.fPoint1.X <= fPoint2.X)) Or _
             ((fPoint1.X <= lineCompare.fPoint2.X) And (lineCompare.fPoint2.X <= fPoint2.X))) Or _
            (((lineCompare.fPoint1.X <= fPoint1.X) And (fPoint1.X <= lineCompare.fPoint2.X)) Or _
             ((lineCompare.fPoint1.X <= fPoint2.X) And (fPoint2.X <= lineCompare.fPoint2.X)))) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Checking that line intersection with adder line --but only on planar
    ''' </summary>
    Public Function IsIntersectionOnPlanarWith(ByVal lineCompare As Line3D) As Boolean
        'prioritize to get intersection
        '1. heightline, depthline, widthline
        '2. 2-axis same position, 1-axis can be different
        '3. position for 1-axis different is a1<=b1<=a2 or a1<=b2<=a2

        If ((Me.IsHeightLine = True) AndAlso (Me.IsHeightLine = lineCompare.IsHeightLine)) AndAlso _
             ((fPoint1.Z <= lineCompare.fPoint1.Z) Or (lineCompare.fPoint1.Z < fPoint2.Z) Or _
             (fPoint1.Z < lineCompare.fPoint2.Z) Or (lineCompare.fPoint2.Z <= fPoint2.Z)) Then
            Return True
        ElseIf ((Me.IsWidthLine = True) AndAlso (Me.IsWidthLine = lineCompare.IsWidthLine)) AndAlso _
            ((fPoint1.Y <= lineCompare.fPoint1.Y) Or (lineCompare.fPoint1.Y < fPoint2.Y) Or _
             (fPoint1.Y < lineCompare.fPoint2.Y) Or (lineCompare.fPoint2.Y <= fPoint2.Y)) Then
            Return True
        ElseIf ((Me.IsDepthLine = True) AndAlso (Me.IsDepthLine = lineCompare.IsDepthLine)) AndAlso _
            ((fPoint1.X <= lineCompare.fPoint1.X) Or (lineCompare.fPoint1.X < fPoint2.X) Or _
             (fPoint1.X < lineCompare.fPoint2.X) Or (lineCompare.fPoint2.X <= fPoint2.X)) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Subtract line
    ''' </summary>
    Public Function Substract(ByVal Subtracter As Line3D) As Line3D()
        'reset
        Dim intersectionLine As Line3D
        Dim resultLine(1) As Line3D
        resultLine(1) = New Line3D(0, 0, 0, 0, 0, 0)

        'cek intersection first + do subtract
        If Me.IsIntersectionWith(Subtracter) = True Then
            intersectionLine = Me.GetIntersection(Subtracter)

            'if intersection.distance below current line
            '-3 possibilities:
            '   1,2. resultline start from left point, or finish to right point
            '   3. result line start from left point, separated on middle, finish to right point
            If intersectionLine.Length < Me.Length Then
                If fPoint1.IsEqualTo(intersectionLine.fPoint1) = True Then
                    resultLine(1) = New Line3D(intersectionLine.fPoint2, fPoint2)
                ElseIf fPoint2.IsEqualTo(intersectionLine.fPoint2) = True Then
                    resultLine(1) = New Line3D(fPoint1, intersectionLine.fPoint1)
                Else
                    ReDim resultLine(2)
                    resultLine(1) = New Line3D(fPoint1, intersectionLine.fPoint1)
                    resultLine(2) = New Line3D(intersectionLine.fPoint2, fPoint2)
                End If
            End If
        End If

        'send result
        Return resultLine
    End Function

    ''' <summary>
    ''' Get intersection line on planar
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Reset value
    ''' --3. Sorting value
    ''' --4. Return value
    ''' </summary>
    Public Function GetIntersection(ByVal lineCompare As Line3D) As Line3D
        '(1)
        Dim point(4) As Single

        '(2)
        GetIntersection = Nothing

        If Me.IsIntersectionOnPlanarWith(lineCompare) = True Then
            If Me.IsHeightLine = True Then
                point(1) = fPoint1.Z : point(2) = fPoint2.Z
                point(3) = lineCompare.fPoint1.Z : point(4) = lineCompare.fPoint2.Z
            End If
            If Me.IsDepthLine = True Then
                point(1) = fPoint1.X : point(2) = fPoint2.X
                point(3) = lineCompare.fPoint1.X : point(4) = lineCompare.fPoint2.X
            End If
            If Me.IsWidthLine = True Then
                point(1) = fPoint1.Y : point(2) = fPoint2.Y
                point(3) = lineCompare.fPoint1.Y : point(4) = lineCompare.fPoint2.Y
            End If

            '(3)
            Array.Sort(point, 1, point.GetUpperBound(0))

            '(4)
            If Me.IsHeightLine = True Then
                Return New Line3D(Me.fPoint1.X, Me.fPoint1.Y, point(2), _
                                  Me.fPoint2.X, Me.fPoint2.Y, point(3))
            End If
            If Me.IsDepthLine = True Then
                Return New Line3D(point(2), Me.fPoint1.Y, Me.fPoint1.Z, _
                                  point(3), Me.fPoint2.Y, Me.fPoint2.Z)
            End If
            If Me.IsWidthLine = True Then
                Return New Line3D(Me.fPoint1.X, point(2), Me.fPoint1.Z, _
                                  Me.fPoint2.X, point(3), Me.fPoint1.Z)
            End If
        End If
    End Function

    ''' <summary>
    ''' MergeSpecial
    ''' -Combining two lines is like replacing each other
    ''' -Futher theory, read Computational Geometric 
    ''' -How to combine
    ''' --a. For instance, there are two lines: line-A, line-B
    ''' --b. Get intersection line-A and line-B >> line-I-AB
    ''' --c. Get union line --if feasible >> line-U-AB
    ''' --d. Line intersection replace line union
    ''' --e. Return value
    '''
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get union line
    ''' --3. Get intersection line
    ''' --4. If intersection exist
    ''' --5. Get new line (intersectionline replacing some part of union line)
    ''' --6. Get value
    ''' </summary>
    Public Sub MergeSpecial(ByVal Adder As Line3D)
        '(1)
        Dim minPoint, maxPoint As Point3D
        Dim intersectionLine, unionLine As Line3D

        '(2)
        unionLine = Me.GetUnion(Adder)
        '(3)
        intersectionLine = Me.GetIntersection(Adder)

        '(4)
        If (Me.IsIntersectionWith(Adder) = True) Then
            If (intersectionLine.Length > 0) Then
                '(5)
                If fMin(intersectionLine.fPoint1.Distance(New Point3D(0, 0, 0)), _
                        unionLine.fPoint1.Distance(New Point3D(0, 0, 0))) = intersectionLine.fPoint1.Distance(New Point3D(0, 0, 0)) Then
                    minPoint = New Point3D(intersectionLine.fPoint1)
                Else
                    minPoint = New Point3D(unionLine.fPoint1)
                End If
                If fMax(intersectionLine.fPoint2.Distance(New Point3D(0, 0, 0)), _
                        unionLine.fPoint2.Distance(New Point3D(0, 0, 0))) = intersectionLine.fPoint2.Distance(New Point3D(0, 0, 0)) Then
                    maxPoint = New Point3D(intersectionLine.fPoint2)
                Else
                    maxPoint = New Point3D(unionLine.fPoint2)
                End If

                '(6)
                fPoint1 = New Point3D(minPoint) : fPoint2 = New Point3D(intersectionLine.fPoint1)
                Adder.fPoint1 = New Point3D(intersectionLine.fPoint2) : Adder.fPoint2 = New Point3D(maxPoint)
            Else
                '(6)
                fPoint1 = New Point3D(unionLine.fPoint1) : fPoint2 = New Point3D(unionLine.fPoint2)
                Adder.fPoint1 = New Point3D(0, 0, 0) : Adder.fPoint2 = New Point3D(0, 0, 0)
            End If
        End If
    End Sub


    ''' <summary>
    ''' Substract special for contour --> very limited edition... yeah!
    ''' -??? what's the hell is this??? i'm getting crazy
    ''' -(if i don't get wrong), this substract is substract and add line
    '''
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Reset
    ''' --3. Check intersection first + do subtract
    ''' --4. Return value
    ''' </summary>
    Public Function SubstractSpecial(ByVal Subtracter As Line3D) As Line3D()
        '(1)
        Dim minPoint, maxPoint As Point3D

        '(2)
        If fMin(fPoint1.Distance(New Point3D(0, 0, 0)), _
               Subtracter.fPoint1.Distance(New Point3D(0, 0, 0))) = fPoint1.Distance(New Point3D(0, 0, 0)) Then
            minPoint = New Point3D(fPoint1)
        Else
            minPoint = New Point3D(Subtracter.fPoint1)
        End If
        If fMax(fPoint2.Distance(New Point3D(0, 0, 0)), _
               Subtracter.fPoint2.Distance(New Point3D(0, 0, 0))) = fPoint2.Distance(New Point3D(0, 0, 0)) Then
            maxPoint = New Point3D(fPoint2)
        Else
            maxPoint = New Point3D(Subtracter.fPoint2)
        End If

        Dim intersectionLine As Line3D
        Dim resultLine(1) As Line3D
        resultLine(1) = New Line3D(0, 0, 0, 0, 0, 0)

        '(3)
        '//Do substract
        intersectionLine = Me.GetIntersection(Subtracter)
        If (Me.IsIntersectionWith(Subtracter) = True) And (intersectionLine.Length > 0) Then
            ReDim resultLine(2)
            resultLine(1) = New Line3D(New Point3D(minPoint), New Point3D(intersectionLine.fPoint1))
            resultLine(2) = New Line3D(New Point3D(intersectionLine.fPoint2), New Point3D(maxPoint))

            If (resultLine(1).Length = 0) And (resultLine(2).Length = 0) Then
                resultLine(1) = New Line3D(0, 0, 0, 0, 0, 0)
            ElseIf (resultLine(1).Length > 0) And (resultLine(2).Length = 0) Then
                ReDim Preserve resultLine(1)
            ElseIf (resultLine(1).Length = 0) And (resultLine(2).Length > 0) Then
                resultLine(1) = New Line3D(resultLine(2))
                ReDim Preserve resultLine(1)
            End If
        End If

        '(4)
        Return resultLine
    End Function
End Class
