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
''' moduleExecution.vb
''' ---
''' Execution the program
''' </summary>
Module Execution
    ''' <summary>
    ''' Selected dataBox which one  to execute
    ''' </summary>
    Dim ExecDataBox(Nothing) As Box
    ''' <summary>
    ''' List of selected dataBox
    ''' </summary>
    Dim ExecListBox(Nothing) As strBoxList


    ''' <summary>
    ''' #Execute code
    ''' -Main program to execute data
    ''' -Input: dataBox
    ''' -Output: coordinate of dataBox
    ''' --1. Set variable
    ''' --2. Input data --> console to memory
    ''' --3. Create class plot3D ???
    ''' --4. Find best solution, start iteration
    ''' ---4a. Reset value
    ''' ---4b. Start iteration for each empty space
    ''' ----4b.1 Set method
    ''' ----4b.2 Optimize method ---> output: fitness and coordinate of dataBox (selected method)
    ''' ----4b.3 Fitness selection
    ''' ---4c. Get result if fitness = good
    ''' ----4c.1 Generate result
    ''' ----4c.2 Clone result to temp ??? I forget what for
    ''' ----4c.3 Submit result in plot3D
    ''' ----4c.4 Generate empty space
    ''' ----4c.5 Update data recapitulation
    ''' --5. If VALID = TRUE then Print_result
    ''' --6. Generate final result
    ''' </summary>
    Public Sub Execute()
        '(1)
        Dim i, count As Integer                         '//i = counter,  count = iteration-counter
        Dim tempInput(Nothing), tempBoundingBox As Box
        Dim templistBox(Nothing) As strBoxList

        Dim bestFitness As Single = 0.0000001
        Dim bestPointer As Integer = Nothing
        Dim bestMethod As Byte

        Dim cuboidPacking(Nothing) As Cuboid
        Dim wallPacking(Nothing) As Wall
        Dim stackPacking(Nothing) As Stack

        '(2)
        algInputText(ExecDataBox, ExecListBox)

        '(3)
        '//Primary variable
        Dim packing As New Plot3D(ExecDataBox(0).Depth, _
                                  ExecDataBox(0).Width, _
                                  ExecDataBox(0).Height)

        '(4)
        '//bestFitness = 0 ???
        Do Until bestFitness = 0
            '(4a)
            '//Reset variable
            bestFitness = 0
            '//Reset method (temporary)
            ReDim cuboidPacking(packing.CountSpace)
            ReDim wallPacking(packing.CountSpace)
            ReDim stackPacking(packing.CountSpace)

            '***
            Console.WriteLine()
            Console.WriteLine()
            Console.WriteLine("---------------------")
            '***

            '(4b)
            For i = 1 To packing.CountSpace
                '***
                Console.Write("packing : " & i)

                '(4b.1)
                cuboidPacking(i) = New Cuboid(packing.Space(i), ExecDataBox)
                wallPacking(i) = New Wall(packing.Space(i), ExecDataBox)
                stackPacking(i) = New Stack(packing.Space(i), ExecDataBox)
                '***
                Console.Write("-set")

                '(4b.2)
                cuboidPacking(i).GetOptimizeCuboid(False)
                wallPacking(i).GetOptimizeWall()
                stackPacking(i).GetOptimizeStack()
                '***
                Console.Write("-optimize")

                '---
                '//Fitness calculation
                'calculate fitness --> for temporary set fitness to score first
                If bestFitness < cuboidPacking(i).Compactness Then
                    bestFitness = cuboidPacking(i).Compactness
                    bestPointer = i
                    bestMethod = 1
                End If
                '---
                If bestFitness < wallPacking(i).Compactness Then
                    bestFitness = wallPacking(i).Compactness
                    bestPointer = i
                    bestMethod = 2
                End If

                '(4b.3)
                If bestFitness < stackPacking(i).Compactness Then
                    bestFitness = stackPacking(i).Compactness
                    bestPointer = i
                    bestMethod = 3
                End If
                '***
                'Console.WriteLine("-fitness = " & cuboidPacking(i).Score & " (best=" & bestFitness & ")")
                'Console.WriteLine("-fitness = " & wallPacking(i).Utilization & " (best=" & bestFitness & ")")
                'Console.WriteLine("-fitness = " & stackPacking(i).Utilization & " (best=" & bestFitness & ")")
            Next

            '(4c)
            '//Placement + get maximal empty space --> if the box can't fit it!
            If bestFitness > 0 Then
                '(4c.1)
                If bestMethod = 1 Then
                    ExecDataBox = cuboidPacking(bestPointer).OutputBox
                    templistBox = cuboidPacking(bestPointer).OutputList
                    tempBoundingBox = cuboidPacking(bestPointer).OutputBoundingBox
                ElseIf bestMethod = 2 Then
                    ExecDataBox = wallPacking(bestPointer).OutputBox
                    templistBox = wallPacking(bestPointer).OutputList
                    tempBoundingBox = wallPacking(bestPointer).OutputBoundingBox
                Else
                    ExecDataBox = stackPacking(bestPointer).OutputBox
                    templistBox = stackPacking(bestPointer).OutputList
                    tempBoundingBox = stackPacking(bestPointer).OutputBoundingBox
                End If
                '***
                Console.WriteLine("outputdata")

                '(4c.2)
                '//Get tempBox and update output data
                '//tempInput --> input variable box; databox in "setnewBox" algorithm as output box.
                '//tempInput is need because 4c.3, use it for parameter
                procBoxClone(ExecDataBox, tempInput)
                '***
                Console.WriteLine("establish tempbox")

                '(4c.3)
                packing.InsertNewBoxes1(packing.Space(bestPointer), _
                                       tempInput, _
                                       tempBoundingBox, _
                                       ExecDataBox)
                '***
                Console.WriteLine("placement plot3D")

                '(4c.4)
                packing.GetSpace()

                '(4c.5)
                algRecapitulation(ExecDataBox, ExecListBox)

                '---
                '//Debug writing
                count += 1
                Console.WriteLine("=======================")
                Console.WriteLine("--- " & count & " ---")
                Console.WriteLine("=======================")
                Console.WriteLine("BoundingBox : " & tempBoundingBox.Depth & " x " & tempBoundingBox.Width & " x " & tempBoundingBox.Height & " (" & tempBoundingBox.AbsPos1.X & "," & tempBoundingBox.AbsPos1.Y & "," & tempBoundingBox.AbsPos1.Z & ")")
                For i = 1 To packing.CountSpace
                    Console.WriteLine(i & " : " & packing.Space(i).Depth & " x " & packing.Space(i).Width & " x " & packing.Space(i).Height & " (" & packing.Space(i).AbsPos1.X & "," & packing.Space(i).AbsPos1.Y & "," & packing.Space(i).AbsPos1.Z & ") + (" & packing.Space(i).AbsPos2.X & "," & packing.Space(i).AbsPos2.Y & "," & packing.Space(i).AbsPos2.Z & ")")
                Next
                Console.WriteLine("=======================")
                '---
            End If
        Loop

        '(5)
        '//Validate box
        packing.GetValidation()
        If packing.Validation = True Then
            '//True = orientation of preview
            algPrintBox(packing.Output, True)
        End If
        MyForm.formMainMenu.Label2.Text = packing.Validation
        
        '(6)
        algOutputInConsole(packing.Output)
    End Sub
End Module