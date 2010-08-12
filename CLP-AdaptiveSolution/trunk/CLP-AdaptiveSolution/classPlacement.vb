Public Class Placement
    ''' <summary>
    ''' Input box
    ''' </summary>
    Protected FInput() As Box
    ''' <summary>
    ''' Output box
    ''' </summary>
    Protected FOutput() As Box
    ''' <summary>
    ''' Recapitulation of input box
    ''' </summary>
    Protected FDataListInput() As ListBox
    ''' <summary>
    ''' Recapitulation of output box
    ''' </summary>
    Protected FDataListOutput() As ListBox
    ''' <summary>
    ''' Cuboid size
    ''' </summary>
    Protected FEmptySpace As Box
    ''' <summary>
    ''' Bounding cuboid as result
    ''' </summary>
    Protected FBoundingCuboid As Box
    ''' <summary>
    ''' Status used of this packing method
    ''' </summary>
    Protected FMethodStatus As New String("")
    ''' <summary>
    ''' Utilization of box
    ''' </summary>
    Protected FUtilization As Single
    ''' <summary>
    ''' Possible doing the placement
    ''' </summary>
    Protected FPossiblePlacement As Boolean

    ''' <summary>
    ''' Status of method
    ''' </summary> 
    Public ReadOnly Property MethodStatus() As String
        Get
            Return FMethodStatus
        End Get
    End Property

    ''' <summary>
    ''' Bounding cuboid data
    ''' </summary>
    Public ReadOnly Property BoundingCuboid() As Box
        Get
            If FBoundingCuboid Is Nothing Then
                Return New Box()
            Else
                Return FBoundingCuboid
            End If
        End Get
    End Property

    Public ReadOnly Property Utilization() As Single
        Get
            Return FUtilization
        End Get
    End Property

End Class
