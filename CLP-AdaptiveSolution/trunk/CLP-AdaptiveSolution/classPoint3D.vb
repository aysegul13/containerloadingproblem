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
''' classPoint3D.vb
''' -Object 'point' in 3D-space
''' </summary>
Public Class Point3D
    ''' <summary>
    ''' Position in X-axes
    ''' </summary>
    Public X As Single
    ''' <summary>
    ''' Position in Y-axes
    ''' </summary>
    Public Y As Single
    ''' <summary>
    ''' Position in Z-axes
    ''' </summary>
    Public Z As Single

    ''' <summary>
    ''' Add new 3D coordinate
    ''' </summary>
    Sub New(ByVal xx As Single, ByVal yy As Single, ByVal zz As Single)
        X = xx
        Y = yy
        Z = zz
    End Sub

    ''' <summary>
    ''' Add new 3D coordinate - copy constructor
    ''' </summary>
    Sub New(ByVal Point As Point3D)
        X = Point.X
        Y = Point.Y
        Z = Point.Z
    End Sub

    ''' <summary>
    ''' Add new 3D coordinate without parameter
    ''' </summary>
    Sub New()
        X = 0.0
        Y = 0.0
        Z = 0.0
    End Sub

    ''' <summary>
    ''' Redefine the point variables
    ''' </summary>
    Public Sub SetPoint(ByVal xx As Single, ByVal yy As Single, ByVal zz As Single)
        X = xx
        Y = yy
        Z = zz
    End Sub

    ''' <summary>
    ''' Check equality between the point and the parameter point
    ''' </summary>
    Public Function IsEqualTo(ByVal Point As Point3D) As Boolean
        If X = Point.X And Y = Point.Y And Z = Point.Z Then
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' Change coordinate by adding distance for each axes
    ''' </summary>
    Public Function Add(ByVal Point As Point3D) As Point3D
        Dim NewPoint = New Point3D
        NewPoint.X = X + Point.X
        NewPoint.Y = Y + Point.Y
        NewPoint.Z = Z + Point.Z
        Return NewPoint
    End Function

    ''' <summary>
    ''' Change coordinate by substracting distance for each axes
    ''' </summary>
    Public Function Subtract(ByVal Point As Point3D) As Point3D
        Dim NewPoint = New Point3D
        NewPoint.X = X - Point.X
        NewPoint.Y = Y - Point.Y
        NewPoint.Y = Z - Point.Z
        Return NewPoint
    End Function

    ''' <summary>
    ''' Get distance the point and the other point
    ''' </summary>
    Public Function Distance(ByVal Point As Point3D) As Single
        Dim xval As Double = X - Point.X
        Dim yval As Double = Y - Point.Y
        Dim zval As Double = Z - Point.Z
        Return System.Math.Sqrt(xval * xval + yval * yval + zval * zval)
    End Function
End Class
