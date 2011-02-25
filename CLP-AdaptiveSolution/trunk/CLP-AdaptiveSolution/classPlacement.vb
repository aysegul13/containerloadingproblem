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
''' Placement
''' - Basic class that control all placing things
''' </summary>
Public Class Placement

    ''' <summary>
    ''' Input box
    ''' </summary>
    Protected fInput() As Box

    ''' <summary>
    ''' Output box
    ''' </summary>
    Protected fOutput() As Box

    ''' <summary>
    ''' Recapitulation of input box
    ''' </summary>
    Protected fListInput() As strBoxList

    ''' <summary>
    ''' Recapitulation of output box
    ''' </summary>
    Protected fListOutput() As strBoxList

    ''' <summary>
    ''' Empty-space dimension
    ''' </summary>
    Protected fSpace As Box

    ''' <summary>
    ''' Bounding cuboid as result
    ''' </summary>
    Protected fBoundingBox As Box

    ''' <summary>
    ''' Status used of this packing method
    ''' </summary>
    Protected fMethod As New String("")

    ''' <summary>
    ''' Utilization box in space
    ''' </summary>
    Protected fUtilization As Single

    ''' <summary>
    ''' Compactness box in space
    ''' </summary>
    Protected fCompactness As Single

    ''' <summary>
    ''' Possible doing the placement
    ''' </summary>
    Protected fPossiblePlacement As Boolean

    ''' <summary>
    ''' Volume total of box in Container
    ''' </summary>
    Protected fVolume As Single

    ''' <summary>
    ''' Status of method
    ''' </summary> 
    Public ReadOnly Property MethodStatus() As String
        Get
            Return fMethod
        End Get
    End Property

    ''' <summary>
    ''' Bounding cuboid data
    ''' </summary>
    Public ReadOnly Property BoundingBox() As Box
        Get
            Return fBoundingBox
        End Get
    End Property

    ''' <summary>
    ''' Get utilization box in space
    ''' </summary>
    Public ReadOnly Property Utilization() As Single
        Get
            Return fUtilization
        End Get
    End Property


    ''' <summary>
    ''' Get utilization box in space
    ''' </summary>
    Public ReadOnly Property Compactness() As Integer
        Get
            Return fCompactness
        End Get
    End Property

    ''' <summary>
    ''' Output box
    ''' </summary>
    Public ReadOnly Property OutputBox() As Box()
        Get
            Return fOutput
        End Get
    End Property

    ''' <summary>
    ''' Output bounding box
    ''' </summary>
    Public ReadOnly Property OutputBoundingBox() As Box
        Get
            Return fBoundingBox
        End Get
    End Property

    ''' <summary>
    ''' Output list
    ''' </summary>
    Public ReadOnly Property OutputList() As strBoxList()
        Get
            Return fListOutput
        End Get
    End Property

End Class
