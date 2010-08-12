Imports System

Module Execution
    Dim dataBox(Nothing) As Box
    Dim listBox(Nothing) As ListBox

    Public Sub Execute()
        'input data
        algInputText(dataBox, listBox)

        'start execution --> create class plot3D
        Dim packing As New Plot3D(dataBox(0).Width, dataBox(0).Depth, dataBox(0).Height)

        'variabel + rest data
        Dim bestFitness As Single = 0.0000001
        Dim bestPointer As Integer = Nothing
        Dim i As Integer
        Dim cuboidPacking(Nothing) As Cuboid
        Dim wallPacking(Nothing) As Wall

        Dim tempInput(Nothing), tempBoundingBox As Box
        Dim templistBox(Nothing) As ListBox


        'break variable
        Dim count As Integer = 0


        Do Until bestFitness = 0
            'reset data
            'ReDim cuboidPacking(packing.CountEmptySpace)
            ReDim wallPacking(packing.CountEmptySpace)
            bestFitness = 0

            'writeline
            Console.WriteLine()
            Console.WriteLine()
            Console.WriteLine("---------------------")

            'iteration
            For i = 1 To packing.CountEmptySpace

                Console.Write("packing : " & i)

                'set cuboid
                'cuboidPacking(i) = New Cuboid(packing.EmptySpace(i), dataBox)
                wallPacking(i) = New Wall(packing.EmptySpace(i), dataBox)

                Console.Write("-set")

                'set optimization
                'cuboidPacking(i).GetOptimizeCuboid(False)
                wallPacking(i).GetOptimizeWall()

                Console.Write("-optimize")

                'calculate fitness --> for temporary set fitness to score first
                'If bestFitness < cuboidPacking(i).Score Then
                '    bestFitness = cuboidPacking(i).Score
                '    bestPointer = i
                'End If

                If bestFitness < wallPacking(i).Utilization Then
                    bestFitness = wallPacking(i).Utilization
                    bestPointer = i
                End If

                'Console.WriteLine("-fitness = " & cuboidPacking(i).Score & " (best=" & bestFitness & ")")
                Console.WriteLine("-fitness = " & wallPacking(i).Utilization & " (best=" & bestFitness & ")")
            Next

            '---
            'placement + get maximal empty space --> if the box can't fit it!
            If bestFitness > 0 Then
                'get output data
                'dataBox = cuboidPacking(bestPointer).OutputBox
                'templistBox = cuboidPacking(bestPointer).OutputList
                'tempBoundingBox = cuboidPacking(bestPointer).OutputBoundingBox
                dataBox = wallPacking(bestPointer).OutputBox
                templistBox = wallPacking(bestPointer).OutputList
                tempBoundingBox = wallPacking(bestPointer).OutputBoundingBox

                Console.WriteLine("outputdata")

                'get tempBox and update output data
                'tempinput --> input variable  box; databox in "setnewBox" algorithm as output box.
                ReDim Preserve tempInput(dataBox.GetUpperBound(0))
                For i = 1 To dataBox.GetUpperBound(0)
                    tempInput(i) = New Box(dataBox(i))
                Next

                Console.WriteLine("establish tempbox")

                'placement in plot3D
                packing.SetNewBox(packing.EmptySpace(bestPointer), tempInput, tempBoundingBox, dataBox)

                Console.WriteLine("placement plot3D")

                'generate empty space
                packing.GetEmptySpace()

                'update recap data
                Recapitulation(dataBox, listBox)

                count += 1
                Console.WriteLine("=======================")
                Console.WriteLine("--- " & count & " ---")
                Console.WriteLine("=======================")
                Console.WriteLine("BoundingBox : " & tempBoundingBox.Depth & " x " & tempBoundingBox.Width & " x " & tempBoundingBox.Height & " (" & tempBoundingBox.LocationContainer.X & "," & tempBoundingBox.LocationContainer.Y & "," & tempBoundingBox.LocationContainer.Z & ")")
                For i = 1 To packing.CountEmptySpace
                    Console.WriteLine(i & " : " & packing.EmptySpace(i).Depth & " x " & packing.EmptySpace(i).Width & " x " & packing.EmptySpace(i).Height & " (" & packing.EmptySpace(i).LocationContainer.X & "," & packing.EmptySpace(i).LocationContainer.Y & "," & packing.EmptySpace(i).LocationContainer.Z & ") + (" & packing.EmptySpace(i).LocationContainer2.X & "," & packing.EmptySpace(i).LocationContainer2.Y & "," & packing.EmptySpace(i).LocationContainer2.Z & ")")
                Next
                Console.WriteLine("=======================")
            End If
        Loop

        'print in drawing
        PrintBox(packing.Output, True)

        'write box packing -yes/not
        algOutputText(packing.Output)
    End Sub
End Module