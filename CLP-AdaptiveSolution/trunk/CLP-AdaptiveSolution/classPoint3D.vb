Imports System

Public Class Point3D
    ''' <summary>
    ''' Coordinate-X
    ''' </summary>
    Public X As Single
    ''' <summary>
    ''' Coordinate-Y
    ''' </summary>
    Public Y As Single
    ''' <summary>
    ''' Coordinate-Z
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
    ''' This point is equal to the parameter point
    ''' </summary>
    Public Function IsEqualTo(ByVal Point As Point3D) As Boolean
        If X = Point.X And Y = Point.Y And Z = Point.Z Then
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' Change 3D coordinate through add new 3D point
    ''' </summary>
    Public Function Add(ByVal Point As Point3D) As Point3D
        Dim NewPoint = New Point3D
        NewPoint.X = X + Point.X
        NewPoint.Y = Y + Point.Y
        NewPoint.Z = Z + Point.Z
        Return NewPoint
    End Function

    ''' <summary>
    ''' Change 3D coordinate through subtract new 3D point
    ''' </summary>
    Public Function Subtract(ByVal Point As Point3D) As Point3D
        Dim NewPoint = New Point3D
        NewPoint.X = X - Point.X
        NewPoint.Y = Y - Point.Y
        NewPoint.Y = Z - Point.Z
        Return NewPoint
    End Function

    ''' <summary>
    ''' Get distance with the other point
    ''' </summary>
    Public Function Distance(ByVal Point As Point3D) As Single
        Dim xval As Double = X - Point.X
        Dim yval As Double = Y - Point.Y
        Dim zval As Double = Z - Point.Z
        Return System.Math.Sqrt(xval * xval + yval * yval + zval * zval)
    End Function
End Class
