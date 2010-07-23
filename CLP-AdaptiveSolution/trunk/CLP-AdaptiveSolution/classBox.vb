''' <summary>
''' Box and Container
''' </summary>
''' <remarks>
''' for container
''' - the coordinate, side and orientation is fix.
''' - please be aware of dimension of container. it should be fix from begining.
''' 
''' for box
''' - the dynamic orientation and side of box still must be arrange by using container origin
''' 
''' for empty space in container
''' - it could be generate by an arrangement of empty space
''' - the coordinate in container maybe fix, include the side and orientation
''' </remarks>
Public Class Box

    ''' <summary>
    ''' Type of box
    ''' </summary>
    Private FBoxType As Integer
    ''' <summary>
    ''' One of input dimension
    ''' </summary>
    Private FDim1 As Single
    ''' <summary>
    ''' One of input dimension
    ''' </summary>
    Private FDim2 As Single
    ''' <summary>
    ''' One of input dimension
    ''' </summary>
    Private FDim3 As Single
    ''' <summary>
    ''' Width of box, depend on orientation-side
    ''' </summary>
    Private FWidth As Single
    ''' <summary>
    ''' Depth of box, depend on orientation-side
    ''' </summary>
    Private FDepth As Single
    ''' <summary>
    ''' Height of box, depend on orientation-side
    ''' </summary>
    Private FHeight As Single
    ''' <summary>
    ''' Coordinate 1 of temporary position
    ''' </summary>
    Private FPosTemp As Point3D
    ''' <summary>
    ''' Coordinate 2 of temporary position
    ''' </summary>
    Private FPosTemp2 As Point3D
    ''' <summary>
    ''' Coordinate 1 of container position
    ''' </summary>
    Private FPosContainer As Point3D
    ''' <summary>
    ''' Coordinate 2 of container position
    ''' </summary>
    Private FPosContainer2 As Point3D
    ''' <summary>
    ''' Orientation box
    ''' </summary>
    Private FOrientation As Boolean
    ''' <summary>
    ''' Side of d1-d2
    ''' </summary>
    Private FSideAlpha As Boolean
    ''' <summary>
    ''' Side of d1-d3
    ''' </summary>
    Private FSideBeta As Boolean
    ''' <summary>
    ''' Side of d2-d3
    ''' </summary>
    Private FSideGamma As Boolean
    ''' <summary>
    ''' Box in container
    ''' </summary>
    Private FBoxContainer As Boolean


    ''' <summary>
    ''' Standar construction
    ''' </summary>
    ''' <param name="boxType">Type</param>
    ''' <param name="d1">Dimension1</param>
    ''' <param name="d2">Dimension2</param>
    ''' <param name="d3">Dimension3</param>
    Sub New(ByVal boxType As Integer, _
                    ByVal d1 As Single, ByVal d2 As Single, ByVal d3 As Single)
        FBoxType = boxType

        FPosTemp = New Point3D(0, 0, 0)
        FPosContainer = New Point3D(0, 0, 0)

        FSideAlpha = True
        FSideBeta = False
        FSideGamma = False

        FBoxContainer = False
        FOrientation = True

        FDim1 = d1
        FDim2 = d2
        FDim3 = d3

        Update()
    End Sub

    ''' <summary>
    ''' Non-parameterized construction
    ''' </summary>
    Sub New()
        FBoxType = -1

        FPosTemp = New Point3D(0, 0, 0)
        FPosContainer = New Point3D(0, 0, 0)

        FSideAlpha = True
        FSideBeta = False
        FSideGamma = False

        FBoxContainer = False
        FOrientation = True

        FDim1 = 0
        FDim2 = 0
        FDim3 = 0

        Update()
    End Sub

    ''' <summary>
    ''' Full construction
    ''' </summary>
    ''' <param name="boxType">Type</param>
    ''' <param name="d1">Dimension1</param>
    ''' <param name="d2">Dimension2</param>
    ''' <param name="d3">Dimension3</param>
    ''' <param name="orientation">Orientation</param>
    ''' <param name="cekcontainer">Box in container</param>
    ''' <param name="alpha">Side alpha</param>
    ''' <param name="beta">Side beta</param>
    ''' <param name="gamma">Side gamma</param>
    Sub New(ByVal boxType As Integer, _
                    ByVal d1 As Single, ByVal d2 As Single, ByVal d3 As Single, _
                    ByVal orientation As Boolean, ByVal cekcontainer As Boolean, _
                    ByVal alpha As Boolean, ByVal beta As Boolean, ByVal gamma As Boolean)
        FBoxType = boxType

        FPosTemp = New Point3D(0, 0, 0)
        FPosContainer = New Point3D(0, 0, 0)

        If (alpha = False) And (beta = True) And (gamma = False) Then
            FSideAlpha = False
            FSideBeta = True
            FSideGamma = False
        ElseIf (alpha = False) And (beta = False) And (gamma = True) Then
            FSideAlpha = False
            FSideBeta = False
            FSideGamma = True
        Else
            FSideAlpha = True
            FSideBeta = False
            FSideGamma = False
        End If

        FBoxContainer = cekcontainer
        FOrientation = orientation

        FDim1 = d1
        FDim2 = d2
        FDim3 = d3

        Update()
    End Sub

    ''' <summary>
    ''' Construction for validate
    ''' </summary>
    Sub New(ByVal boxType As Integer, _
                    ByVal d1 As Single, ByVal d2 As Single, ByVal d3 As Single, _
                    ByVal orientation As Boolean)
        FBoxType = boxType

        FPosTemp = New Point3D(0, 0, 0)
        FPosContainer = New Point3D(0, 0, 0)

        FSideAlpha = True
        FSideBeta = False
        FSideGamma = False

        FBoxContainer = False
        FOrientation = orientation

        FDim1 = d1
        FDim2 = d2
        FDim3 = d3

        Update()
    End Sub

    ''' <summary>
    ''' Dimension construction
    ''' </summary>
    Sub New(ByVal boxType As Integer, _
                    ByVal width As Single, ByVal height As Single, ByVal depth As Single, _
                    ByVal side As Byte)
        FBoxType = boxType

        FPosTemp = New Point3D(0, 0, 0)
        FPosContainer = New Point3D(0, 0, 0)

        If side = 1 Then
            FSideAlpha = True
        ElseIf side = 2 Then
            FSideBeta = True
        Else
            FSideGamma = True
        End If

        FBoxContainer = False

        If width > depth Then
            FOrientation = True
        Else
            FOrientation = False
        End If

        FDim1 = depth
        FDim2 = width
        FDim3 = height
        'FWidth = width
        'FHeight = height
        'FDepth = depth

        Update()
        'FPosTemp2 = GetCoordinate2(FPosTemp)
        'FPosContainer2 = GetCoordinate2(FPosContainer)
    End Sub

    ''' <summary>
    ''' Construction from Clone
    ''' </summary>
    Sub New(ByVal cloneBox As Box)
        FBoxType = cloneBox.Type

        FPosTemp = cloneBox.FPosTemp
        FPosContainer = cloneBox.FPosContainer

        FSideAlpha = cloneBox.FSideAlpha
        FSideBeta = cloneBox.FSideBeta
        FSideGamma = cloneBox.FSideGamma

        FBoxContainer = cloneBox.FBoxContainer
        FOrientation = cloneBox.FOrientation

        FDim1 = cloneBox.FDim1
        FDim2 = cloneBox.FDim2
        FDim3 = cloneBox.FDim3

        Update()
    End Sub

    ''' <summary>
    ''' Depth
    ''' </summary>
    Public ReadOnly Property Depth() As Single
        Get
            Return FDepth
        End Get
    End Property

    ''' <summary>
    ''' Width
    ''' </summary>
    Public ReadOnly Property Width() As Single
        Get
            Return FWidth
        End Get
    End Property

    ''' <summary>
    ''' Height
    ''' </summary>
    Public ReadOnly Property Height() As Single
        Get
            Return FHeight
        End Get
    End Property

    ''' <summary>
    ''' Defining side alpha
    ''' </summary>
    Public Property Alpha() As Boolean
        Get
            Return FSideAlpha
        End Get
        Set(ByVal Value As Boolean)
            If Value = True Then
                FSideAlpha = Value
                FSideBeta = False
                FSideGamma = False
            Else
                FSideAlpha = False
                FSideBeta = False
                FSideGamma = False
            End If
            Update()
        End Set
    End Property

    ''' <summary>
    ''' Defining side beta
    ''' </summary>
    Public Property Beta() As Boolean
        Get
            Return FSideBeta
        End Get
        Set(ByVal Value As Boolean)
            If Value = True Then
                FSideAlpha = False
                FSideBeta = Value
                FSideGamma = False
            Else
                FSideAlpha = False
                FSideBeta = False
                FSideGamma = False
            End If
            Update()
        End Set
    End Property

    ''' <summary>
    ''' Defining side gamma
    ''' </summary>
    Public Property Gamma() As Boolean
        Get
            Return FSideGamma
        End Get
        Set(ByVal Value As Boolean)
            If Value = True Then
                FSideAlpha = False
                FSideBeta = False
                FSideGamma = Value
            Else
                FSideAlpha = False
                FSideBeta = False
                FSideGamma = False
            End If
            Update()
        End Set
    End Property

    ''' <summary>
    ''' Location box temporary
    ''' </summary>
    Public Property LocationContainer() As Point3D
        Get
            Return FPosContainer
        End Get
        Set(ByVal Value As Point3D)
            FPosContainer.X = Value.X
            FPosContainer.Y = Value.Y
            FPosContainer.Z = Value.Z
            Update()
        End Set
    End Property

    ''' <summary>
    ''' Location box in container
    ''' </summary>
    Public Property LocationTemp() As Point3D
        Get
            Return FPosTemp
        End Get
        Set(ByVal Value As Point3D)
            FPosTemp.X = Value.X
            FPosTemp.Y = Value.Y
            FPosTemp.Z = Value.Z
            Update()
        End Set
    End Property

    ''' <summary>
    ''' Box in container
    ''' </summary>
    Public Property InContainer() As Boolean
        Get
            Return FBoxContainer
        End Get
        Set(ByVal Value As Boolean)
            FBoxContainer = Value
        End Set
    End Property

    ''' <summary>
    ''' Location box 2 temporary
    ''' </summary>
    Public ReadOnly Property LocationContainer2() As Point3D
        Get
            Return FPosContainer2
        End Get
    End Property

    ''' <summary>
    ''' Location box 2 in container
    ''' </summary>
    Public ReadOnly Property LocationTemp2() As Point3D
        Get
            Return FPosTemp2
        End Get
    End Property

    ''' <summary>
    ''' Orientation of box
    ''' </summary>
    Public Property Orientation() As Boolean
        Get
            Return FOrientation
        End Get
        Set(ByVal Value As Boolean)
            FOrientation = Value
            Update()
        End Set
    End Property

    ''' <summary>
    ''' Type of box
    ''' </summary>
    Public Property Type() As Integer
        Get
            Return FBoxType
        End Get
        Set(ByVal Value As Integer)
            FBoxType = Value
        End Set
    End Property

    ''' <param name="xx">Dimension1</param>
    ''' <param name="yy">Dimension2</param>
    ''' <param name="zz">Dimension3</param>
    Public Sub SetDimension(ByVal xx As Single, ByVal yy As Single, ByVal zz As Single)
        FDim1 = xx
        FDim2 = yy
        FDim3 = zz
        Update()
    End Sub

    Public Sub SetD1(ByVal Value As Single)
        FDim1 = Value
        Update()
    End Sub

    Public Sub SetD2(ByVal Value As Single)
        FDim2 = Value
        Update()
    End Sub

    Public Sub SetD3(ByVal Value As Single)
        FDim3 = Value
        Update()
    End Sub

    ''' <summary>
    ''' Update every changed that correlated
    ''' </summary>
    Public Sub Update()
        UpdatedDimension()
        FPosTemp2 = GetCoordinate2(FPosTemp)
        FPosContainer2 = GetCoordinate2(FPosContainer)
    End Sub

    ''' <summary>
    ''' Update dimension
    ''' </summary>
    Private Sub UpdatedDimension()
        Dim t1, t2, t3 As Integer

        If FSideAlpha = True Then
            t1 = FDim1
            t2 = FDim2
            t3 = FDim3
        ElseIf FSideBeta = True Then
            t1 = FDim1
            t2 = FDim3
            t3 = FDim2
        Else
            t1 = FDim2
            t2 = FDim3
            t3 = FDim1
        End If

        'return dimension
        FWidth = t1
        FDepth = t2
        FHeight = t3

        'orientation fixation
        If ((FOrientation = True) And (FWidth < FDepth)) Or _
            ((FOrientation = False) And (FWidth > FDepth)) _
                Then Swap(FWidth, FDepth)
    End Sub

    ''' <summary>
    ''' Get coordinate for position 2
    ''' </summary>
    Private Function GetCoordinate2(ByVal pos As Point3D) As Point3D
        Return New Point3D(pos.X + FDepth, pos.Y + FWidth, pos.Z + FHeight)
    End Function
End Class
