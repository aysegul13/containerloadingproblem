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
    Public FPoint1 As Point3D

    ''' <summary>
    ''' Point B
    ''' </summary>
    Public FPoint2 As Point3D

    ''' <summary>
    ''' Direction for vector
    ''' DepthLine --> true = right ; false = left
    ''' WidthLine --> true = up ;  false = down
    ''' HeightLine --> true = top ; false = bottom
    ''' </summary>
    Public FDirection As Boolean

    ''' <summary>
    ''' Standar constructor
    ''' </summary>
    Sub New(ByVal x1 As Single, ByVal y1 As Single, ByVal z1 As Single, ByVal x2 As Single, ByVal y2 As Single, ByVal z2 As Single)
        FPoint1 = New Point3D(x1, y1, z1)
        FPoint2 = New Point3D(x2, y2, z2)
        Sort()
    End Sub

    ''' <summary>
    ''' Default constructor
    ''' </summary>
    Sub New(ByVal p1 As Point3D, ByVal p2 As Point3D)
        FPoint1 = New Point3D(p1)
        FPoint2 = New Point3D(p2)
        Sort()
    End Sub

    ''' <summary>
    ''' Clone constructor
    ''' </summary>
    Sub New(ByVal line As Line3D)
        FPoint1 = New Point3D(line.FPoint1)
        FPoint2 = New Point3D(line.FPoint2)
    End Sub

    ''' <summary>
    ''' Identify line is equal with compare line
    ''' </summary>
    Public Function IsEqualTo(ByVal lineCompare As Line3D) As Boolean
        If (FPoint1.IsEqualTo(lineCompare.FPoint1) = True) And (FPoint2.IsEqualTo(lineCompare.FPoint2) = True) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Get length of line
    ''' </summary>
    Public Function Length() As Single
        Dim xval As Double = FPoint2.X - FPoint1.X
        Dim yval As Double = FPoint2.Y - FPoint1.Y
        Dim zval As Double = FPoint2.Z - FPoint1.Z
        Return System.Math.Sqrt(xval * xval + yval * yval + zval * zval)
    End Function

    ''' <summary>
    ''' Sorting line
    ''' </summary>
    Private Sub Sort()
        If (FPoint1.X > FPoint2.X) Or _
            ((FPoint1.X = FPoint2.X) And (FPoint1.Y > FPoint2.Y)) Or _
            ((FPoint1.X = FPoint2.X) And (FPoint1.Y = FPoint2.Y) And (FPoint1.Z > FPoint2.Z)) _
            Then procSwap(FPoint1, FPoint2)
    End Sub

    ''' <summary>
    ''' True --> width line (y1, y2)
    ''' </summary>
    Public Function IsWidthLine() As Boolean
        If (FPoint1.X = FPoint2.X) And (FPoint1.Y <> FPoint2.Y) And (FPoint1.Z = FPoint2.Z) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' True --&gt; height line (z1, z2)
    ''' </summary>
    Public Function IsHeightLine() As Boolean
        If (FPoint1.X = FPoint2.X) And (FPoint1.Y = FPoint2.Y) And (FPoint1.Z <> FPoint2.Z) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' True --&gt; depth line (x1, x2)
    ''' </summary>
    Public Function IsDepthLine() As Boolean
        If (FPoint1.X <> FPoint2.X) And (FPoint1.Y = FPoint2.Y) And (FPoint1.Z = FPoint2.Z) Then
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
            (((FPoint1.X < lineCompare.FPoint1.X) Or ((FPoint1.X = lineCompare.FPoint1.X) And (FPoint1.Y < lineCompare.FPoint1.Y))) Or _
             ((FPoint1.X = lineCompare.FPoint1.X) And (FPoint1.Y = lineCompare.FPoint1.Y) And (FPoint1.Z < lineCompare.FPoint1.Z))) Then
            Return True
        ElseIf ((Me.IsWidthLine = True) AndAlso (Me.IsWidthLine = lineCompare.IsWidthLine)) AndAlso _
            (((FPoint1.Z < lineCompare.FPoint1.Z) Or ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.X < lineCompare.FPoint1.X))) Or _
             ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.X = lineCompare.FPoint1.X) And (FPoint1.Y < lineCompare.FPoint1.Y))) Then
            Return True
        ElseIf ((Me.IsDepthLine = True) AndAlso (Me.IsDepthLine = lineCompare.IsDepthLine)) AndAlso _
            (((FPoint1.Z < lineCompare.FPoint1.Z) Or ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.Y < lineCompare.FPoint1.Y))) Or _
             ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.Y = lineCompare.FPoint1.Y) And (FPoint1.X < lineCompare.FPoint1.X))) Then
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
            (((FPoint1.X > lineCompare.FPoint1.X) Or ((FPoint1.X = lineCompare.FPoint1.X) And (FPoint1.Y > lineCompare.FPoint1.Y))) Or _
            ((FPoint1.X = lineCompare.FPoint1.X) And (FPoint1.Y = lineCompare.FPoint1.Y) And (FPoint1.Z > lineCompare.FPoint1.Z))) Then
            Return True
        ElseIf ((Me.IsWidthLine = True) AndAlso (Me.IsWidthLine = lineCompare.IsWidthLine)) AndAlso _
            (((FPoint1.Z > lineCompare.FPoint1.Z) Or ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.X > lineCompare.FPoint1.X))) Or _
             ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.X = lineCompare.FPoint1.X) And (FPoint1.Y > lineCompare.FPoint1.Y))) Then
            Return True
        ElseIf ((Me.IsDepthLine = True) AndAlso (Me.IsDepthLine = lineCompare.IsDepthLine)) AndAlso _
            (((FPoint1.Z > lineCompare.FPoint1.Z) Or ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.Y > lineCompare.FPoint1.Y))) Or _
             ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.Y = lineCompare.FPoint1.Y) And (FPoint1.X > lineCompare.FPoint1.X))) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Adding line
    ''' </summary>
    Public Function Add(ByVal adder As Line3D) As Line3D
        Dim addLine As Line3D
        Dim minVal, maxVal As Single

        'reset
        addLine = New Line3D(0, 0, 0, 0, 0, 0)

        'cek intersection first
        'get min, max value
        If Me.IsIntersectionWith(adder) = True Then
            If Me.IsHeightLine = True Then
                minVal = fMin(fMin(FPoint1.Z, FPoint2.Z), fMin(adder.FPoint1.Z, adder.FPoint2.Z))
                maxVal = fMax(fMax(FPoint1.Z, FPoint2.Z), fMax(adder.FPoint1.Z, adder.FPoint2.Z))
                addLine = New Line3D(FPoint1.X, FPoint1.Y, minVal, FPoint1.X, FPoint1.Y, maxVal)
            End If
            If Me.IsDepthLine = True Then
                minVal = fMin(fMin(FPoint1.X, FPoint2.X), fMin(adder.FPoint1.X, adder.FPoint2.X))
                maxVal = fMax(fMax(FPoint1.X, FPoint2.X), fMax(adder.FPoint1.X, adder.FPoint2.X))
                addLine = New Line3D(minVal, FPoint1.Y, FPoint1.Z, maxVal, FPoint1.Y, FPoint1.Z)
            End If
            If Me.IsWidthLine = True Then
                minVal = fMin(fMin(FPoint1.Y, FPoint2.Y), fMin(adder.FPoint1.Y, adder.FPoint2.Y))
                maxVal = fMax(fMax(FPoint1.Y, FPoint2.Y), fMax(adder.FPoint1.Y, adder.FPoint2.Y))
                addLine = New Line3D(FPoint1.X, minVal, FPoint1.Z, FPoint1.X, maxVal, FPoint1.Z)
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
            ((FPoint1.X = lineCompare.FPoint1.X) And (FPoint1.Y = lineCompare.FPoint1.Y)) AndAlso _
            ((((FPoint1.Z <= lineCompare.FPoint1.Z) And (lineCompare.FPoint1.Z <= FPoint2.Z)) Or _
             ((FPoint1.Z <= lineCompare.FPoint2.Z) And (lineCompare.FPoint2.Z <= FPoint2.Z))) Or _
            (((lineCompare.FPoint1.Z <= FPoint1.Z) And (FPoint1.Z <= lineCompare.FPoint2.Z)) Or _
             ((lineCompare.FPoint1.Z <= FPoint2.Z) And (FPoint2.Z <= lineCompare.FPoint2.Z)))) Then
            Return True
        ElseIf ((Me.IsWidthLine = True) AndAlso (Me.IsWidthLine = lineCompare.IsWidthLine)) AndAlso _
            ((FPoint1.X = lineCompare.FPoint1.X) And (FPoint1.Z = lineCompare.FPoint1.Z)) AndAlso _
            ((((FPoint1.Y <= lineCompare.FPoint1.Y) And (lineCompare.FPoint1.Y <= FPoint2.Y)) Or _
             ((FPoint1.Y <= lineCompare.FPoint2.Y) And (lineCompare.FPoint2.Y <= FPoint2.Y))) Or _
            (((lineCompare.FPoint1.Y <= FPoint1.Y) And (FPoint1.Y <= lineCompare.FPoint2.Y)) Or _
             ((lineCompare.FPoint1.Y <= FPoint2.Y) And (FPoint2.Y <= lineCompare.FPoint2.Y)))) Then
            Return True
        ElseIf ((Me.IsDepthLine = True) AndAlso (Me.IsDepthLine = lineCompare.IsDepthLine)) AndAlso _
            ((FPoint1.Z = lineCompare.FPoint1.Z) And (FPoint1.Y = lineCompare.FPoint1.Y)) AndAlso _
            ((((FPoint1.X <= lineCompare.FPoint1.X) And (lineCompare.FPoint1.X <= FPoint2.X)) Or _
             ((FPoint1.X <= lineCompare.FPoint2.X) And (lineCompare.FPoint2.X <= FPoint2.X))) Or _
            (((lineCompare.FPoint1.X <= FPoint1.X) And (FPoint1.X <= lineCompare.FPoint2.X)) Or _
             ((lineCompare.FPoint1.X <= FPoint2.X) And (FPoint2.X <= lineCompare.FPoint2.X)))) Then
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
             ((FPoint1.Z <= lineCompare.FPoint1.Z) Or (lineCompare.FPoint1.Z < FPoint2.Z) Or _
             (FPoint1.Z < lineCompare.FPoint2.Z) Or (lineCompare.FPoint2.Z <= FPoint2.Z)) Then
            Return True
        ElseIf ((Me.IsWidthLine = True) AndAlso (Me.IsWidthLine = lineCompare.IsWidthLine)) AndAlso _
            ((FPoint1.Y <= lineCompare.FPoint1.Y) Or (lineCompare.FPoint1.Y < FPoint2.Y) Or _
             (FPoint1.Y < lineCompare.FPoint2.Y) Or (lineCompare.FPoint2.Y <= FPoint2.Y)) Then
            Return True
        ElseIf ((Me.IsDepthLine = True) AndAlso (Me.IsDepthLine = lineCompare.IsDepthLine)) AndAlso _
            ((FPoint1.X <= lineCompare.FPoint1.X) Or (lineCompare.FPoint1.X < FPoint2.X) Or _
             (FPoint1.X < lineCompare.FPoint2.X) Or (lineCompare.FPoint2.X <= FPoint2.X)) Then
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
            intersectionLine = Me.GetIntersectionOnPlanarWith(Subtracter)

            'if intersection.distance below current line
            '-3 possibilities:
            '   1,2. resultline start from left point, or finish to right point
            '   3. result line start from left point, separated on middle, finish to right point
            If intersectionLine.Length < Me.Length Then
                If FPoint1.IsEqualTo(intersectionLine.FPoint1) = True Then
                    resultLine(1) = New Line3D(intersectionLine.FPoint2, FPoint2)
                ElseIf FPoint2.IsEqualTo(intersectionLine.FPoint2) = True Then
                    resultLine(1) = New Line3D(FPoint1, intersectionLine.FPoint1)
                Else
                    ReDim resultLine(2)
                    resultLine(1) = New Line3D(FPoint1, intersectionLine.FPoint1)
                    resultLine(2) = New Line3D(intersectionLine.FPoint2, FPoint2)
                End If
            End If
        End If

        'send result
        Return resultLine
    End Function

    ''' <summary>
    ''' Get intersection line on planar
    ''' </summary>
    Public Function GetIntersectionOnPlanarWith(ByVal lineCompare As Line3D) As Line3D
        Dim point(4) As Single

        'reset value
        GetIntersectionOnPlanarWith = Nothing

        If Me.IsIntersectionOnPlanarWith(lineCompare) = True Then
            If Me.IsHeightLine = True Then
                point(1) = FPoint1.Z : point(2) = FPoint2.Z
                point(3) = lineCompare.FPoint1.Z : point(4) = lineCompare.FPoint2.Z
            End If
            If Me.IsDepthLine = True Then
                point(1) = FPoint1.X : point(2) = FPoint2.X
                point(3) = lineCompare.FPoint1.X : point(4) = lineCompare.FPoint2.X
            End If
            If Me.IsWidthLine = True Then
                point(1) = FPoint1.Y : point(2) = FPoint2.Y
                point(3) = lineCompare.FPoint1.Y : point(4) = lineCompare.FPoint2.Y
            End If

            'sorting the value
            Array.Sort(point, 1, point.GetUpperBound(0))

            'return value
            If Me.IsHeightLine = True Then
                Return New Line3D(Me.FPoint1.X, Me.FPoint1.Y, point(2), _
                                  Me.FPoint2.X, Me.FPoint2.Y, point(3))
            End If
            If Me.IsDepthLine = True Then
                Return New Line3D(point(2), Me.FPoint1.Y, Me.FPoint1.Z, _
                                  point(3), Me.FPoint2.Y, Me.FPoint2.Z)
            End If
            If Me.IsWidthLine = True Then
                Return New Line3D(Me.FPoint1.X, point(2), Me.FPoint1.Z, _
                                  Me.FPoint2.X, point(3), Me.FPoint1.Z)
            End If
        End If
    End Function

    ''' <summary>
    ''' Substract special for contour --> very limited edition... yeah!
    ''' </summary>
    Public Function SubstractSpecial(ByVal Subtracter As Line3D) As Line3D()
        'reset
        Dim minPoint, maxPoint As Point3D
        If fMin(FPoint1.Distance(New Point3D(0, 0, 0)), _
               Subtracter.FPoint1.Distance(New Point3D(0, 0, 0))) = FPoint1.Distance(New Point3D(0, 0, 0)) Then
            minPoint = New Point3D(FPoint1)
        Else
            minPoint = New Point3D(Subtracter.FPoint1)
        End If
        If fMax(FPoint2.Distance(New Point3D(0, 0, 0)), _
               Subtracter.FPoint2.Distance(New Point3D(0, 0, 0))) = FPoint2.Distance(New Point3D(0, 0, 0)) Then
            maxPoint = New Point3D(FPoint2)
        Else
            maxPoint = New Point3D(Subtracter.FPoint2)
        End If

        Dim intersectionLine As Line3D
        Dim resultLine(1) As Line3D
        resultLine(1) = New Line3D(0, 0, 0, 0, 0, 0)

        'cek intersection first + do subtract
        intersectionLine = Me.GetIntersectionOnPlanarWith(Subtracter)
        If (Me.IsIntersectionWith(Subtracter) = True) And (intersectionLine.Length > 0) Then
            ReDim resultLine(2)
            resultLine(1) = New Line3D(New Point3D(minPoint), New Point3D(intersectionLine.FPoint1))
            resultLine(2) = New Line3D(New Point3D(intersectionLine.FPoint2), New Point3D(maxPoint))

            If (resultLine(1).Length = 0) And (resultLine(2).Length = 0) Then
                resultLine(1) = New Line3D(0, 0, 0, 0, 0, 0)
            ElseIf (resultLine(1).Length > 0) And (resultLine(2).Length = 0) Then
                ReDim Preserve resultLine(1)
            ElseIf (resultLine(1).Length = 0) And (resultLine(2).Length > 0) Then
                resultLine(1) = New Line3D(resultLine(2))
                ReDim Preserve resultLine(1)
            End If
        End If

        'send result
        Return resultLine
    End Function

End Class
