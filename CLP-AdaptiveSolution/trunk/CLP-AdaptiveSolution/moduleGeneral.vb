'#Importing classes
'--system.drawing
'--system.drawing2D
'--system.io --> read file text (I/O basic text)
'--excel --> read excel file
'--system.runtime.serialization.formatters.binary
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

'#Structure data
''' <summary>
''' Box
''' </summary>
Public Structure strBoxList
    Dim SCount, SType As Integer
End Structure

''' <summary>
''' Data structure for BR Test
''' </summary>
Public Structure strDataforBRTest
    Dim TypeData As strBoxList
    Dim Dimension() As Single
    Dim Fisibility() As Boolean
End Structure


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
''' moduleGeneral.vb
''' ---
''' list:
''' 1. 
''' </summary>
Module General

    ''' <summary>
    ''' ##procShallowClone code
    ''' </summary>
    Public Function procShallowClone(ByVal instance As ICloneable) As Object
        Return instance.Clone
    End Function

    ''' <summary>
    ''' ##procDeepClone code
    ''' </summary>
    ''' This is one way to do deep cloning. But works only if the 
    ''' objects and its references are serializable
    Public Function procDeepClone(ByVal obj As Object) As Object
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

    ''' <summary>
    ''' ##algRecapitulation code
    ''' -Recapitulation MasterDataBox --> MasterListBox
    ''' --0. Parameter set --> dataBoxParam(), listBoxParam()
    ''' --1. Variable set
    ''' --2. Input data
    ''' --3. Sorting data
    ''' ---  Sorting array from item 1 to getupperbound
    ''' --4. Create list box
    ''' --5. Recapitulation core
    ''' </summary>
    Public Sub algRecapitulation(ByVal inputbox() As Box, _
                                 ByRef datalistbox() As strBoxList)
        '(1)
        Dim i, j As Integer
        Dim temp() As Integer
        Dim tempBox(,) As Integer

        '(2)
        ReDim temp(inputbox.GetUpperBound(0))
        For i = 1 To inputbox.GetUpperBound(0)
            temp(i) = inputbox(i).Type
        Next

        '(3)
        Array.Sort(temp, 1, temp.GetUpperBound(0))
        ReDim tempBox(temp.GetUpperBound(0), 2)

        '(4)
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

        '(5)
        ReDim datalistbox(j)
        For i = 1 To j
            datalistbox(i).SType = tempBox(i, 1)
            datalistbox(i).SCount = tempBox(i, 2)
        Next
    End Sub

    ''' <summary>
    ''' ##procSwap code
    ''' </summary>
    Public Sub procSwap(ByRef object1 As Object, ByRef object2 As Object)
        Dim objecttemp As Object
        objecttemp = object1
        object1 = object2
        object2 = objecttemp
    End Sub

    ''' <summary>
    ''' ##function GetBoxFromList
    ''' Get dataBox from type Box in list
    ''' </summary>
    Public Function fGetBoxFromList(ByVal typeBox As Integer, ByVal dataBox() As Box) As Box
        Dim i As Integer

        For i = 1 To dataBox.GetUpperBound(0)
            If dataBox(i).Type = typeBox Then Exit For
        Next
        Return New Box(dataBox(i))
    End Function

    ''' <summary>
    ''' ##functionMax code
    ''' </summary>
    Public Function fMax(ByVal object1 As Object, ByVal object2 As Object) As Object
        If object1 > object2 Then
            Return object1
        Else
            Return object2
        End If
    End Function

    ''' <summary>
    ''' ##functionSwap code
    ''' </summary>
    Public Function fMax3(ByVal object1 As Object, ByVal object2 As Object, ByVal object3 As Object) As Object
        Return fMax(object1, fMin(object2, object3))
    End Function

    ''' <summary>
    ''' ##functionSwap code
    ''' </summary>
    Public Function fMin(ByVal object1 As Object, ByVal object2 As Object) As Object
        If object1 > object2 Then
            Return object2
        Else
            Return object1
        End If
    End Function

    ''' <summary>
    ''' ##functionMin3 code
    ''' </summary>
    Public Function fMin3(ByVal object1 As Object, ByVal object2 As Object, ByVal object3 As Object) As Object
        Return fMin(object1, fMin(object2, object3))
    End Function

    ''' <summary>
    ''' ##boxClone code
    ''' </summary>
    Public Sub procBoxClone(ByVal fromBox() As Box, ByRef toBox() As Box)
        ReDim toBox(fromBox.GetUpperBound(0))
        For i = 1 To fromBox.GetUpperBound(0)
            toBox(i) = New Box(fromBox(i))
        Next
    End Sub

    ''' <summary>
    ''' ##function GroupBox
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get height variation
    ''' </summary>
    Public Function fGetGroupBox(ByVal inputBox() As Box, ByVal Above As Boolean) As Box()()
        '(1)
        Dim i, j, k As Integer
        Dim varHeight(inputBox.GetUpperBound(0)) As Single

        '(2)
        '//Get data
        If Above = True Then
            For i = 1 To inputBox.GetUpperBound(0)
                varHeight(i) = inputBox(i).AbsPos2.Z
            Next
        Else
            For i = 1 To inputBox.GetUpperBound(0)
                varHeight(i) = inputBox(i).AbsPos1.Z
            Next
        End If
        
        '//Sort: increasing
        Array.Sort(varHeight, 1, varHeight.GetUpperBound(0))
        '//Filtering data
        If varHeight.GetUpperBound(0) > 1 Then
            j = 1
            For i = 2 To varHeight.GetUpperBound(0)
                If varHeight(i) <> varHeight(j) Then
                    j = j + 1
                    varHeight(j) = varHeight(i)
                End If
            Next
            ReDim Preserve varHeight(j)
        End If

        '(3)
        k = 0
        Dim groupBox()() As Box = New Box(varHeight.GetUpperBound(0))() {}
        For i = 1 To varHeight.GetUpperBound(0)
            k = 0
            groupBox(i) = New Box(inputBox.GetUpperBound(0)) {}
            If Above = True Then
                For j = 1 To inputBox.GetUpperBound(0)
                    If varHeight(i) = inputBox(j).AbsPos2.Z Then
                        k += 1
                        groupBox(i)(k) = New Box(inputBox(j))
                    End If
                Next
            Else
                For j = 1 To inputBox.GetUpperBound(0)
                    If varHeight(i) = inputBox(j).AbsPos1.Z Then
                        k += 1
                        groupBox(i)(k) = New Box(inputBox(j))
                    End If
                Next
            End If
            ReDim Preserve groupBox(i)(k)
        Next

        Return groupBox
    End Function

    ''' <summary>
    ''' ##class GetVarHeightBox
    ''' -Get box variation of boxes
    ''' -if IsTop = true >> differing by absZ.2
    ''' -if IsTop = false >> differing by absZ.1
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Get height
    ''' --3. Sort height
    ''' --4. Refine value
    ''' --5. Return value
    ''' </summary>
    Public Function fGetVarHeightBox(ByVal inputBox() As Box, ByVal Above As Boolean) As Single()
        '(1)
        Dim i, j As Integer
        Dim varHeight(inputBox.GetUpperBound(0)) As Single

        '(2)
        If Above = True Then
            For i = 1 To inputBox.GetUpperBound(0)
                varHeight(i) = inputBox(i).AbsPos2.Z
            Next
        Else
            For i = 1 To inputBox.GetUpperBound(0)
                varHeight(i) = inputBox(i).AbsPos1.Z
            Next
        End If

        '(3)
        Array.Sort(varHeight, 1, varHeight.GetUpperBound(0))

        '(4)
        If varHeight.GetUpperBound(0) > 1 Then
            j = 1
            For i = 2 To varHeight.GetUpperBound(0)
                If varHeight(i) <> varHeight(j) Then
                    j += 1
                    varHeight(j) = varHeight(i)
                End If
            Next
            ReDim Preserve varHeight(j)
        End If

        '(5)
        Return varHeight
    End Function

    ''' <summary>
    ''' ##class MyForm code
    ''' </summary>
    Public Class MyForm
        Public Shared formMainMenu As New MainMenu
        Public Shared formExamine As New ExamineMenu
    End Class
End Module