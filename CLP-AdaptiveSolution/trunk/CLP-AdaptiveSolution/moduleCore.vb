Imports System.Object
Imports System.IO                               'reading file
Imports Excel = Microsoft.Office.Interop.Excel  'excel controller
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms

Module Core
    Public Sub algInputExcel(ByRef dataBoxParam() As Box, ByRef listBoxParam() As ListBox)
        Dim i As Integer

        'input semua data
        With MyForm.formMainMenu
            'input dataBOX
            ReDim dataBoxParam(.dbData.RowCount - 1)

            '--container
            dataBoxParam(0) = New Box(0, MyForm.formMainMenu.txtDConDepth.Text, MyForm.formMainMenu.txtDConWidth.Text, MyForm.formMainMenu.txtDConHeight.Text)
            dataBoxParam(0).Alpha = True
            dataBoxParam(0).Orientation = False

            '--box
            For i = 1 To .dbData.RowCount - 1
                dataBoxParam(i) = New Box(.dbData.Item(0, i - 1).Value, .dbData.Item(1, i - 1).Value, .dbData.Item(2, i - 1).Value, .dbData.Item(3, i - 1).Value)
            Next
        End With

        'recapitulation data... make list of box
        recapitulation(dataBoxParam, listBoxParam)

        '---
        'data ud diinput---selesai disini
    End Sub


    Public Sub algInputText(ByRef dataBoxParam() As Box, ByRef listBoxParam() As ListBox)
        Dim i, j, count As Integer

        'input semua data
        With MyForm.formMainMenu
            '--container
            dataBoxParam(0) = New Box(0, MyForm.formMainMenu.txtDConDepth.Text, MyForm.formMainMenu.txtDConWidth.Text, MyForm.formMainMenu.txtDConHeight.Text)
            dataBoxParam(0).Alpha = True
            dataBoxParam(0).Orientation = False

            '--box
            'count number box
            count = 0
            For i = 1 To CInt(.dbData.RowCount - 1)
                count += CInt(.dbData.Item(4, i - 1).Value)
            Next

            '--box
            ReDim Preserve dataBoxParam(count)
            count = 0
            For i = 1 To .dbData.RowCount - 1
                For j = 1 To .dbData.Item(4, i - 1).Value
                    count += 1
                    dataBoxParam(count) = New Box(.dbData.Item(0, i - 1).Value, .dbData.Item(1, i - 1).Value, .dbData.Item(2, i - 1).Value, .dbData.Item(3, i - 1).Value)
                Next
            Next
        End With

        'recapitulation data... make list of box
        Recapitulation(dataBoxParam, listBoxParam)

        '---
        'data ud diinput---selesai disini
    End Sub

    Public Sub algOutputText(ByVal databoxParam() As Box)
        Dim i, j As Integer
        Dim listBoxOutput(Nothing) As ListBox
        Dim volItem, volContainer, volTotal As Single

        'reset data
        volItem = 0
        volTotal = 0

        'recapitulation listboxOutput
        Recapitulation(databoxParam, listBoxOutput)

        With MyForm.formMainMenu
            '1 release recapitulation
            For i = 1 To .dbData.RowCount - 1
                For j = 1 To listBoxOutput.GetUpperBound(0)
                    If CInt(.dbData.Item(0, i - 1).Value) = listBoxOutput(j).SType Then .dbData.Item(5, i - 1).Value = listBoxOutput(j).SCount
                Next

                'calculate vol.item
                volItem += CSng(.dbData.Item(1, i - 1).Value) * CSng(.dbData.Item(2, i - 1).Value) * CSng(.dbData.Item(3, i - 1).Value) * CInt(.dbData.Item(5, i - 1).Value)
                volTotal += CSng(.dbData.Item(1, i - 1).Value) * CSng(.dbData.Item(2, i - 1).Value) * CSng(.dbData.Item(3, i - 1).Value) * CInt(.dbData.Item(4, i - 1).Value)
            Next

            '2. calculate(utilization)
            volContainer = CSng(MyForm.formMainMenu.txtDConDepth.Text) * _
                            CSng(MyForm.formMainMenu.txtDConHeight.Text) * _
                            CSng(MyForm.formMainMenu.txtDConWidth.Text)

            '3. output to console
            .txtConsole.Text = "Item = " & volItem.ToString("#.##") & "  |  TotalItem = " & volTotal.ToString("#.##") & "   |   U.Packing = " & (100 * volItem / volTotal).ToString("#.##") & " %" & "   |   U.Container = " & (100 * volItem / volContainer).ToString("#.##") & " %"
        End With
    End Sub

    Public Sub algOutputExcel(ByVal databoxParam() As Box)
        Dim i, j As Integer
        Dim volItem, volContainer, volTotal As Single

        'reset data
        volItem = 0
        volTotal = 0

        With MyForm.formMainMenu
            '1. output packing box to db.grid
            For i = 1 To databoxParam.GetUpperBound(0)
                For j = 1 To .dbData.RowCount - 1
                    If (CInt(.dbData.Item(0, j - 1).Value) = databoxParam(i).Type) AndAlso _
                        (CStr(.dbData.Item(4, j - 1).Value) = "") Then
                        .dbData.Item(4, j - 1).Value = "v"
                        volItem += databoxParam(i).Depth * databoxParam(i).Width * databoxParam(i).Height
                    End If
                Next
            Next
            For j = 1 To .dbData.RowCount - 1
                If (CStr(.dbData.Item(4, j - 1).Value) <> "v") Then
                    .dbData.Item(4, j - 1).Value = "NOT"
                End If
                volTotal += CSng(.dbData.Item(1, j - 1).Value) * CSng(.dbData.Item(2, j - 1).Value) * CSng(.dbData.Item(3, j - 1).Value)
            Next

            '2. calculate(utilization)
            volContainer = CSng(MyForm.formMainMenu.txtDConDepth.Text) * _
                            CSng(MyForm.formMainMenu.txtDConHeight.Text) * _
                            CSng(MyForm.formMainMenu.txtDConWidth.Text)

            '3. output to console
            .txtConsole.Text = "Item = " & volItem.ToString("#.##") & "   |   packing = " & (100 * volItem / volTotal).ToString("#.##") & "%" & "   |   utilization = " & (100 * volItem / volContainer).ToString("#.##") & "%"
        End With
    End Sub

    Public Sub algDummySolution(ByVal dataBoxParam() As Box, ByVal sortBoxParam(,) As Integer, ByRef dummy() As Box)
        ReDim dummy(4)

        dummy(0) = dataBoxParam(0)
        dummy(1) = dataBoxParam(1)
        dummy(2) = dataBoxParam(2)
        dummy(3) = dataBoxParam(3)
        dummy(4) = dataBoxParam(4)

        'set dummy solution in container
        With dummy(1)
            .LocationContainer.X = 0
            .LocationContainer.Y = 0
            .LocationContainer.Z = 0
            .Beta = True
            .Update()
        End With

        With dummy(2)
            .LocationContainer.X = 0
            .LocationContainer.Y = 0
            .LocationContainer.Z = dummy(1).LocationContainer2.Z
            .Beta = True
            .Update()
        End With

        With dummy(3)
            .LocationContainer = New Point3D(dummy(1).LocationContainer2.X, 0, 0)
            .Beta = True
        End With

        With dummy(4)
            .LocationContainer = New Point3D(dummy(1).LocationContainer2.X, 0, dummy(1).LocationContainer2.Z)
            .Beta = True
        End With
    End Sub

    Public Sub algDummyCuboid(ByVal dataBoxParam() As Box, ByVal listBoxParam() As ListBox, ByRef dummy() As Box)
        'ISIAN
        Dim tempOrien As Boolean
        Dim tempSide As Byte
        tempSide = 2
        tempOrien = True

        'build cuboidcontoh
        Dim cuboidcontoh As New Cuboid(dataBoxParam(0), dataBoxParam(1), Val(MyForm.formMainMenu.txtConsole.Text))
        'cuboidcontoh.SetSideOrientation(tempSide, tempOrien)
        'cuboidcontoh.Construct1AxisCuboid(cuboidcontoh.FreeBox, "D", New Point3D(0, 0, 0), True)
        'cuboidcontoh.Construct2AxisCuboid(cuboidcontoh.FreeBox, "D", "W", New Point3D(0, 0, 0), True)
        'cuboidcontoh.ConstructCuboid(cuboidcontoh.FreeBox, 3, "D", "W", "H", New Point3D(0, 0, 0), False)

        cuboidcontoh.GetOptimizeCuboid(True)
        'cuboidcontoh.GetMaxScoreCuboid(False)

        ReDim dummy(cuboidcontoh.UsedBox + 1)
        'ReDim dummy(cuboidcontoh.MaxBox + 1)

        'set container & bounding box
        dummy(0) = dataBoxParam(0)
        dummy(dummy.GetUpperBound(0)) = New Box(-1, cuboidcontoh.BoundingCuboid.Depth, cuboidcontoh.BoundingCuboid.Width, cuboidcontoh.BoundingCuboid.Height, cuboidcontoh.BoundingCuboid.Orientation)
        'set new
        'For i = 1 To cuboidcontoh.MaxBox
        For i = 1 To cuboidcontoh.UsedBox
            dummy(i) = New Box(1, cuboidcontoh.Box.Depth, cuboidcontoh.Box.Width, cuboidcontoh.Box.Height)
            dummy(i).Orientation = cuboidcontoh.Box.Orientation
            dummy(i).LocationContainer = New Point3D(cuboidcontoh.PositionBoxInCuboid(i).X, cuboidcontoh.PositionBoxInCuboid(i).Y, cuboidcontoh.PositionBoxInCuboid(i).Z)
        Next

        'MyForm.formMainMenu.txtConsole.Text = cuboidcontoh.MethodStatus & " , " & cuboidcontoh.Score

    End Sub

    Public Sub algDummyStack(ByVal dataBoxParam() As Box, ByVal listBoxParam() As ListBox, ByRef dummy() As Box)
        'databox param = data input box
        'sortbox param = data input box that has been sorted
        'dummy = solution box

        'build a stacking --it is ONE!! not more...
        Dim stackingcontoh As New Stack(dataBoxParam(0), dataBoxParam)


    End Sub

    Public Sub algDummyLayer(ByVal dataBoxParam() As Box, ByVal listBoxParam() As ListBox, ByRef dummy() As Box)
        'databox param = data input box
        'sortbox param = data input box that has been sorted
        'dummy = solution box

        'build a stacking --it is ONE!! not more...
        Dim layercontoh As New Layer(dataBoxParam(0), dataBoxParam)
        layercontoh.GetOptimizeLayer()

    End Sub

    Public Sub algRefineNewBox(ByRef dataBox() As Box)
        Dim i, j As Integer
        Dim temp(dataBox.GetUpperBound(0)) As Box

        'clone box to temp
        j = 0
        For i = 1 To dataBox.GetUpperBound(0)
            If dataBox(i).InContainer = False Then

            End If

            j += 1

            temp(i) = New Box(dataBox(i))
        Next
    End Sub

    Public Sub algReadFileText(ByRef BRData(,) As setData)
        'variable
        Dim maxSet, maxItem, i, j, k As Integer
        Dim inputString(0) As String

        'readfile text BR
        Try
            ' Create an instance of StreamReader to read from a file.
            Using sr As StreamReader = New StreamReader("br-testdata.txt")
                'number of set data + set array
                maxSet = CInt(sr.ReadLine)
                sr.ReadLine()
                sr.ReadLine()
                maxItem = CInt(sr.ReadLine)
                ReDim BRData(maxSet, maxItem)

                'close position
                sr.Close()
            End Using

            Using sr As StreamReader = New StreamReader("br-testdata.txt")
                sr.ReadLine()
                'iteration
                For i = 1 To maxSet
                    'retrieve header data
                    inputString(0) = sr.ReadLine

                    'retrieve container size
                    inputString(0) = sr.ReadLine
                    inputString = inputString(0).Split(New Char() {" "c})
                    ReDim BRData(i, 0).Dimension(3)
                    ReDim BRData(i, 0).Fisibility(3)
                    For j = 1 To 3
                        BRData(i, 0).Dimension(j) = CSng(inputString(j))
                    Next

                    'enter 1 time (we don't need maxitem anymore)
                    sr.ReadLine()

                    'retrieve item
                    For j = 1 To maxItem
                        'set item
                        ReDim BRData(i, j).Dimension(3)
                        ReDim BRData(i, j).Fisibility(3)

                        'get type
                        inputString(0) = sr.ReadLine
                        inputString = inputString(0).Split(New Char() {" "c})
                        BRData(i, j).TypeData.SType = CInt(inputString(1))

                        'get item dimension + fisibility
                        For k = 1 To 3
                            BRData(i, j).Dimension(k) = CSng(inputString(k * 2))
                            If CInt(inputString((k * 2) + 1)) = 0 Then
                                BRData(i, j).Fisibility(k) = False
                            Else
                                BRData(i, j).Fisibility(k) = True
                            End If
                        Next

                        'number item
                        BRData(i, j).TypeData.SCount = CInt(inputString(8))
                    Next
                Next
                sr.Close()
            End Using
        Catch e As Exception
            ' Let the user know what went wrong.
            Console.WriteLine("The file could not be read: " & e.Message)
        End Try
    End Sub


    Public Sub algOpenFileText(ByVal choosenSetData() As setData)
        With MyForm.formMainMenu
            '----transfer data to grid
            'container dimension
            .txtDConDepth.Text = choosenSetData(0).Dimension(1)
            .txtDConWidth.Text = choosenSetData(0).Dimension(2)
            .txtDConHeight.Text = choosenSetData(0).Dimension(3)

            'clearbox and add rows
            .dbData.Rows.Clear()

            'box dimension
            For i = 1 To choosenSetData.GetUpperBound(0)
                .dbData.Rows.Add()

                'type
                .dbData.Item(0, i - 1).Value = choosenSetData(i).TypeData.SType
                'dimension
                For j = 1 To 3
                    .dbData.Item(j, i - 1).Value = choosenSetData(i).Dimension(j)
                Next
                'count
                .dbData.Item(4, i - 1).Value = choosenSetData(i).TypeData.SCount
                'fisibility
                For j = 1 To 3
                    .dbData.Item(j + 5, i - 1).Value = choosenSetData(i).Fisibility(j)
                Next
            Next
            '----finish transfer data to grid
        End With

    End Sub

    Public Sub algOpenFileExcel(ByVal sheetNumber As Integer)
        Dim i, j As Integer

        Dim xlApp As Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet
        xlApp = New Excel.ApplicationClass
        xlWorkBook = xlApp.Workbooks.Open("dataCLP.xlsx")
        xlWorkSheet = xlWorkBook.Worksheets("Sheet" & sheetNumber)

        With MyForm.formMainMenu
            '----transfer data to grid
            'container dimension
            .txtDConDepth.Text = xlWorkSheet.Cells(1, 1).Value
            .txtDConWidth.Text = xlWorkSheet.Cells(1, 2).Value
            .txtDConHeight.Text = xlWorkSheet.Cells(1, 3).Value

            'clearbox and add rows
            .dbData.Rows.Clear()

            'box dimension
            For i = 1 To Val(xlWorkSheet.Cells(2, 1).Value)
                .dbData.Rows.Add()
                .dbData.Item(0, i - 1).Value = xlWorkSheet.Cells(i + 2, 4).value
                For j = 1 To 3
                    .dbData.Item(j, i - 1).Value = xlWorkSheet.Cells(i + 2, j).Value
                Next
            Next
            '----finish transfer data to grid
        End With

        xlWorkBook.Close()
        xlApp.Quit()

        releaseObject(xlApp)
        releaseObject(xlWorkBook)
        releaseObject(xlWorkSheet)
    End Sub

    Public Sub algDrawDataGridExcel()
        With MyForm.formMainMenu
            For i As Integer = 1 To .dbData.RowCount
                .dbData.Rows.Clear()
            Next
            For i As Integer = 1 To .dbData.ColumnCount
                .dbData.Columns.Clear()
            Next

            'insert column
            .dbData.Columns.Add("colName", "BoxType")
            .dbData.Columns.Add("dim1", "D1")
            .dbData.Columns.Add("dim2", "D2")
            .dbData.Columns.Add("dim3", "D3")
            .dbData.Columns.Add("isPacking", "Pack")

            'resize column width
            .dbData.Columns(0).Width = 50
            .dbData.Columns(1).Width = (.dbData.Width - 170) / 3
            .dbData.Columns(2).Width = (.dbData.Width - 170) / 3
            .dbData.Columns(3).Width = (.dbData.Width - 170) / 3
            .dbData.Columns(4).Width = 50
        End With

    End Sub

    Public Sub algDrawDataGridText()
        With MyForm.formMainMenu
            'clear data grid
            For i As Integer = 1 To .dbData.RowCount
                .dbData.Rows.Clear()
            Next
            For i As Integer = 1 To .dbData.ColumnCount
                .dbData.Columns.Clear()
            Next

            'insert column
            .dbData.Columns.Add("type", "Type")
            .dbData.Columns.Add("dim1", "D1")
            .dbData.Columns.Add("dim2", "D2")
            .dbData.Columns.Add("dim3", "D3")
            .dbData.Columns.Add("count", "Count")
            .dbData.Columns.Add("isPacking", "Pack")
            .dbData.Columns.Add("fis1", "F1")
            .dbData.Columns.Add("fis2", "F2")
            .dbData.Columns.Add("fis3", "F3")

            'resize column width
            .dbData.Columns(0).Width = 40
            .dbData.Columns(4).Width = 40
            .dbData.Columns(5).Width = 50
            .dbData.Columns(6).Width = 40
            .dbData.Columns(7).Width = 40
            .dbData.Columns(8).Width = 40
            .dbData.Columns(1).Width = (.dbData.Width - 290) / 3
            .dbData.Columns(2).Width = (.dbData.Width - 290) / 3
            .dbData.Columns(3).Width = (.dbData.Width - 290) / 3
        End With

    End Sub

    Public Sub algDrawing(ByVal solution() As Box, ByVal orientation As Boolean)
        'getting the bounding box
        Dim initiate As New Cube(0, 0, solution(0).Height, solution(0).Width, solution(0).Depth, 1, 1)

        'getting the bounding box.. will help to scaling picture box
        Dim picWidth, picHeight As Integer
        picWidth = initiate.BoundsRect.Width - initiate.Width
        picHeight = initiate.BoundsRect.Height - initiate.Height

        'get scaling
        'methos scaling is by using scale-ratio variable
        'ex: origin*scale-ratio = template --> scale ratio = template / origin
        '---if origin < template --> scale-ratio > 1
        '---if origin > template --> scale-ratio < 1
        '---if 2 origin, pic the minimum scale-ratio
        Dim sclRatio As Single
        If (MyForm.formMainMenu.picResult.Width / initiate.BoundsRect.Width) _
            < (MyForm.formMainMenu.picResult.Height / initiate.BoundsRect.Height) Then
            sclRatio = (MyForm.formMainMenu.picResult.Width - 1) / initiate.BoundsRect.Width
        Else
            sclRatio = (MyForm.formMainMenu.picResult.Height - 1) / initiate.BoundsRect.Height
        End If


        'scale and draw all cube
        Dim bm As New Bitmap(MyForm.formMainMenu.picResult.Width, MyForm.formMainMenu.picResult.Height)
        Dim gr As Graphics = Graphics.FromImage(bm)

        'position in cube class
        'x = 0 --> for width
        'y = 0 --> for height
        '
        '0,0,0 = {origin}
        ' posX = 0
        ' posY = 0
        ' posZ = initiate.height * sclratio
        '
        'how to get 
        Dim drawCube(solution.GetUpperBound(0)) As Cube

        If orientation = True Then
            '--container
            drawCube(0) = New Cube(0, 0, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, 1, 1)

            '--box
            For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
                drawCube(i) = New Cube(PositionInPicture(sclRatio, solution(i).LocationContainer, solution(i).Height, initiate.Height, 1, 1), _
                                       solution(i).Height * sclRatio, _
                                       solution(i).Width * sclRatio, _
                                       solution(i).Depth * sclRatio, _
                                       1, 1)
            Next
        Else
            'masi blon bener..males..hoohoo
            '--container
            drawCube(0) = New Cube(0, initiate.BoundsRect.Height, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, -1, -1)

            '--box
            For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
                drawCube(i) = New Cube(PositionInPicture(sclRatio, solution(i).LocationContainer, solution(i).Height, initiate.Height, -1, -1), _
                                       solution(i).Height * sclRatio, _
                                       solution(i).Width * sclRatio, _
                                       solution(i).Depth * sclRatio, _
                                       -1, -1)
            Next
        End If

        'drawing container + cube
        gr.DrawPath(Pens.Red, drawCube(0).GetContainer)
        For i As Integer = 1 To drawCube.GetUpperBound(0)
            gr.DrawPath(Pens.Blue, drawCube(i).GetCube)
        Next

        '--bounding box draw TEMPORARY
        gr.DrawPath(Pens.Green, drawCube(drawCube.GetUpperBound(0)).GetCube)

        MyForm.formMainMenu.picResult.Image = bm
        gr.Dispose()
    End Sub


    Private Sub releaseObject(ByVal obj As Object)
        Try
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
            obj = Nothing
        Catch ex As Exception
            obj = Nothing
        Finally
            GC.Collect()
        End Try
    End Sub

    ''' <summary>
    ''' Automated calculation of bischoff/ratcliff test.
    ''' Save results to excel
    ''' </summary>
    Public Sub algAutomatedTestData()
        Dim i, j, k As Integer
        Dim e As System.EventArgs = Nothing
        Dim tempString(0) As String

        With MyForm.formMainMenu
            'open file
            .btnOpenFile_Click(True, e)

            'get iteration
            tempString(0) = .lblControl.Text
            tempString = tempString(0).Split(New Char() {" "c})

            'reset data
            j = CInt(tempString(2))
            Dim recordItem(j), recordTotalItem(j), recordContainer(j), _
                recordUPacking(j), recordUContainer(j), _
                recordNumberItem(j), recordNumberPacked(j) As Single

            recordNumberItem(0) = 0
            recordNumberPacked(0) = 0
            recordItem(0) = 0
            recordTotalItem(0) = 0
            recordContainer(0) = 0
            recordUPacking(0) = 0
            recordUContainer(0) = 0

            'iteration
            For i = 1 To j
                'process set data i
                .lblControl.Text = i & " / " & j
                .btnNext_Click(True, e)

                'get result
                tempString(0) = .txtConsole.Text
                tempString = tempString(0).Split(New Char() {" "c})

                'record data
                recordNumberItem(i) = 0
                recordNumberPacked(i) = 0
                For k = 1 To .dbData.RowCount - 1
                    recordNumberItem(i) += CInt(.dbData.Item(4, k - 1).Value)
                    recordNumberPacked(i) += CInt(.dbData.Item(5, k - 1).Value)
                Next

                recordItem(i) = CSng(tempString(2))
                recordTotalItem(i) = CSng(tempString(8))
                recordContainer(i) = CSng(tempString(14))

                recordUPacking(i) = CSng(tempString(22))
                recordUContainer(i) = CSng(tempString(31))

                'calculate total for average
                recordNumberItem(0) += recordNumberItem(i)
                recordNumberPacked(0) += recordNumberPacked(i)
                recordItem(0) += recordItem(i)
                recordTotalItem(0) += recordTotalItem(i)
                recordContainer(0) += recordContainer(i)
                recordUPacking(0) += recordUPacking(i)
                recordUContainer(0) += recordUContainer(i)
            Next

            'calculate average
            recordNumberItem(0) = recordNumberItem(0) / j
            recordNumberPacked(0) = recordNumberPacked(0) / j
            recordItem(0) = recordItem(0) / j
            recordTotalItem(0) = recordTotalItem(0) / j
            recordContainer(0) = recordContainer(0) / j
            recordUPacking(0) = recordUPacking(0) / j
            recordUContainer(0) = recordUContainer(0) / j

            'write data to dbgrid
            algDrawDataGridAutomated()
            'list data + list average --> row 0
            For i = 0 To j
                .dbData.Rows.Add()
                'type
                If i = 0 Then
                    .dbData.Item(0, i).Value = "AVG"
                Else
                    .dbData.Item(0, i).Value = i
                End If

                'volitem + volpacked + volcontainer
                .dbData.Item(1, i).Value = recordItem(i).ToString("#.##")
                .dbData.Item(2, i).Value = recordTotalItem(i).ToString("#.##")
                .dbData.Item(3, i).Value = recordContainer(i).ToString("#.##")

                'count + packing
                .dbData.Item(4, i).Value = recordNumberItem(i).ToString("#")
                .dbData.Item(5, i).Value = recordNumberPacked(i).ToString("#")

                '% pack + %container
                .dbData.Item(6, i).Value = recordUPacking(i).ToString("#.##") & "%"
                .dbData.Item(7, i).Value = recordUContainer(i).ToString("#.##") & "%"
            Next
        End With
    End Sub

    Public Sub algDrawDataGridAutomated()
        With MyForm.formMainMenu
            'clear data grid
            For i As Integer = 1 To .dbData.RowCount
                .dbData.Rows.Clear()
            Next
            For i As Integer = 1 To .dbData.ColumnCount
                .dbData.Columns.Clear()
            Next

            'insert column
            .dbData.Columns.Add("Test", "Type")
            .dbData.Columns.Add("volItem", "volI")
            .dbData.Columns.Add("volPacked", "volP")
            .dbData.Columns.Add("volContainer", "volC")
            .dbData.Columns.Add("count", "Count")
            .dbData.Columns.Add("isPacking", "Pack")
            .dbData.Columns.Add("%Pack", "%U.Pack")
            .dbData.Columns.Add("%Container", "%U.Cont.")

            'resize column width
            .dbData.Columns(0).Width = 40
            .dbData.Columns(4).Width = 40
            .dbData.Columns(5).Width = 50
            .dbData.Columns(6).Width = 60
            .dbData.Columns(7).Width = 60
            .dbData.Columns(1).Width = (.dbData.Width - 290) / 3
            .dbData.Columns(2).Width = (.dbData.Width - 290) / 3
            .dbData.Columns(3).Width = (.dbData.Width - 290) / 3
        End With
    End Sub
End Module
