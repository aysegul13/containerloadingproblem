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
    Public Sub Execute(ByRef packing As Plot3D, ByRef runTime As TimeSpan)
        '(1)
        Dim i, count As Integer                                 '//i = counter,  count = iteration-counter
        Dim tempInput(Nothing), tempBoundingBox As Box
        Dim templistBox(Nothing) As strBoxList

        Dim bestFitness As Single = 0.0000001
        Dim bestPointer As Integer = Nothing
        Dim bestMethod As Byte

        Dim cuboidPacking(Nothing) As Cuboid
        Dim wallPacking(Nothing) As Wall
        Dim stackPacking(Nothing) As Stack

        Dim cuboidMethod As Boolean = MyForm.formMainMenu.chkCuboid.Checked
        Dim wallMethod As Boolean = MyForm.formMainMenu.chkWall.Checked
        Dim stackMethod As Boolean = MyForm.formMainMenu.chkStack.Checked

        Dim limitWall As Single = MyForm.formMainMenu.trckWall.Value / MyForm.formMainMenu.trckWall.Maximum
        Dim limitStack As Single = MyForm.formMainMenu.trckStack.Value / MyForm.formMainMenu.trckStack.Maximum
        Dim limitIter As Integer = MyForm.formMainMenu.limIter

        Dim contDimension As Point3D

        Dim execStartTime, observeStartTime As DateTime
        Dim deltaTime(3) As TimeSpan

        '//initialize execStartTime
        execStartTime = DateTime.Now

        '(2)
        algInputText(ExecDataBox, ExecListBox)

        '(3)
        '//Primary variable
        contDimension = New Point3D(ExecDataBox(0).Depth, _
                                    ExecDataBox(0).Width, _
                                    ExecDataBox(0).Height)
        packing = New Plot3D(contDimension.X, _
                                  contDimension.Y, _
                                  contDimension.Z)

        '(4)
        '//Rule stop
        Do Until (bestFitness = 0) Or (ExecDataBox.GetUpperBound(0) = 0)
            '(4a)
            '//Reset variable
            bestFitness = 0
            If (cuboidMethod = True) Then ReDim cuboidPacking(packing.CountSpace)
            If (wallMethod = True) Then ReDim wallPacking(packing.CountSpace)
            If (stackMethod = True) Then ReDim stackPacking(packing.CountSpace)

            '***
            MyForm.formMainMenu.listConsole.AppendText(vbCrLf)
            MyForm.formMainMenu.listConsole.AppendText(vbCrLf)
            MyForm.formMainMenu.listConsole.AppendText("---------------------" & vbCrLf)
            '***

            '(4b)
            MyForm.formMainMenu.listConsole.AppendText("packing" & vbCrLf)
            For i = 1 To packing.CountSpace
                '***
                MyForm.formMainMenu.listConsole.AppendText(i)

                '(4b.1)
                If (cuboidMethod = True) Then cuboidPacking(i) = New Cuboid(packing.Space(i), ExecDataBox)
                If (wallMethod = True) Then wallPacking(i) = New Wall(packing.Space(i), ExecDataBox, limitWall)
                If (stackMethod = True) Then stackPacking(i) = New Stack(packing.Space(i), ExecDataBox, limitStack, limitIter)
                '***
                MyForm.formMainMenu.listConsole.AppendText("-set")

                '(4b.2)
                observeStartTime = DateTime.Now
                If (cuboidMethod = True) Then cuboidPacking(i).GetOptimizeCuboid(False)

                deltaTime(1) = DateTime.Now - observeStartTime
                observeStartTime = DateTime.Now

                If (wallMethod = True) Then wallPacking(i).GetOptimizeWall()

                deltaTime(2) = DateTime.Now - observeStartTime
                observeStartTime = DateTime.Now

                If (stackMethod = True) Then stackPacking(i).GetOptimizeStack()

                deltaTime(3) = DateTime.Now - observeStartTime
                observeStartTime = DateTime.Now
                '***
                MyForm.formMainMenu.listConsole.AppendText("-optimize")

                '(4b.3)
                '//Fitness calculation: see formula to change
                If (cuboidMethod = True) Then
                    cuboidPacking(i).GetFitness(New Point3D(contDimension))
                    If bestFitness < cuboidPacking(i).Fitness Then
                        bestFitness = cuboidPacking(i).Fitness
                        bestPointer = i
                        bestMethod = 1
                    End If
                End If

                If (wallMethod = True) Then
                    wallPacking(i).GetFitness(New Point3D(contDimension))
                    If bestFitness < wallPacking(i).Fitness Then
                        bestFitness = wallPacking(i).Fitness
                        bestPointer = i
                        bestMethod = 2
                    End If
                End If

                If (stackMethod = True) Then
                    stackPacking(i).GetFitness(New Point3D(contDimension))
                    If bestFitness < stackPacking(i).Fitness Then
                        bestFitness = stackPacking(i).Fitness
                        bestPointer = i
                        bestMethod = 3
                    End If
                End If

                '***
                If (cuboidMethod = True) Then MyForm.formMainMenu.listConsole.AppendText("-fCuboid=" & cuboidPacking(i).Fitness & ":" & deltaTime(1).TotalSeconds & " ")
                If (wallMethod = True) Then MyForm.formMainMenu.listConsole.AppendText("-fWall=" & wallPacking(i).Fitness & ":" & deltaTime(2).TotalSeconds & " ")
                If (stackMethod = True) Then MyForm.formMainMenu.listConsole.AppendText("-fStack=" & stackPacking(i).Fitness & ":" & deltaTime(3).TotalSeconds)
                MyForm.formMainMenu.listConsole.AppendText(" (Best=" & bestFitness & ")" & vbCrLf)
            Next

            '(4c)
            '//Placement + get maximal empty space --> if the box can't fit it!
            If bestFitness > 0 Then
                '(4c.1)
                If (bestMethod = 1) And (cuboidMethod = True) Then
                    ExecDataBox = cuboidPacking(bestPointer).OutputBox
                    templistBox = cuboidPacking(bestPointer).OutputList
                    tempBoundingBox = cuboidPacking(bestPointer).OutputBoundingBox
                ElseIf (bestMethod = 2) And (wallMethod = True) Then
                    ExecDataBox = wallPacking(bestPointer).OutputBox
                    templistBox = wallPacking(bestPointer).OutputList
                    tempBoundingBox = wallPacking(bestPointer).OutputBoundingBox
                Else
                    ExecDataBox = stackPacking(bestPointer).OutputBox
                    templistBox = stackPacking(bestPointer).OutputList
                    tempBoundingBox = stackPacking(bestPointer).OutputBoundingBox
                End If

                '***
                MyForm.formMainMenu.listConsole.AppendText("outputdata" & vbCrLf)

                '(4c.2)
                '//Get tempBox and update output data
                '//tempInput --> input variable box; databox in "setnewBox" algorithm as output box.
                '//tempInput is need because 4c.3, use it for parameter
                procBoxClone(ExecDataBox, tempInput)
                '***
                MyForm.formMainMenu.listConsole.AppendText("establish tempbox" & vbCrLf)

                '(4c.3)
                packing.InsertNewBoxesA(packing.Space(bestPointer), _
                                       tempInput, _
                                       tempBoundingBox, _
                                       ExecDataBox)
                '***
                MyForm.formMainMenu.listConsole.AppendText("placement plot3D" & vbCrLf)

                '(4c.4)
                packing.GetSpace()

                '(4c.5)
                algRecapitulation(ExecDataBox, ExecListBox)

                '---
                '//Debug writing
                count += 1
                MyForm.formMainMenu.listConsole.AppendText(vbCrLf)
                MyForm.formMainMenu.listConsole.AppendText("=======================" & vbCrLf)
                MyForm.formMainMenu.listConsole.AppendText("--- " & count & " ---" & vbCrLf)
                MyForm.formMainMenu.listConsole.AppendText("=======================" & vbCrLf)
                MyForm.formMainMenu.listConsole.AppendText("BoundingBox : " & tempBoundingBox.Depth & " x " & tempBoundingBox.Width & " x " & tempBoundingBox.Height & " (" & tempBoundingBox.AbsPos1.X & "," & tempBoundingBox.AbsPos1.Y & "," & tempBoundingBox.AbsPos1.Z & ")" & vbCrLf)
                For i = 1 To packing.CountSpace
                    MyForm.formMainMenu.listConsole.AppendText(i & " : " & packing.Space(i).Depth & " x " & packing.Space(i).Width & " x " & packing.Space(i).Height & " (" & packing.Space(i).AbsPos1.X & "," & packing.Space(i).AbsPos1.Y & "," & packing.Space(i).AbsPos1.Z & ") + (" & packing.Space(i).AbsPos2.X & "," & packing.Space(i).AbsPos2.Y & "," & packing.Space(i).AbsPos2.Z & ")" & vbCrLf)
                Next
                MyForm.formMainMenu.listConsole.AppendText("=======================" & vbCrLf)
                '---
            End If
        Loop

        '(5)
        '//Validate box
        packing.GetValidation()
        If packing.Validation = True Then
            '//True = orientation of preview
            algPrintBox1(packing.OutputBox, True)
        End If

        '//initialize execStartTime
        runTime = DateTime.Now - execStartTime

        MyForm.formMainMenu.lblValidation.Text = "Validation = " & packing.Validation & vbCrLf & "Run Time = " & runTime.TotalSeconds

        '(6)
        algOutputInConsole(packing.OutputBox)
    End Sub
End Module