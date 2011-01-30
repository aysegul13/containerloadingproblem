'#Importing classes
'--system.io --> read file text (I/O basic text)
'--excel --> read excel file
'--drawing 2d
'--drawing picture
Imports System.IO                               'reading file
Imports System.Object
Imports System.Drawing
Imports Excel = Microsoft.Office.Interop.Excel  'excel controller

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
''' moduleCore.vb
''' ---
''' list:
''' 1. 
''' </summary>
Module Core
    ''' <summary>
    ''' ##algReadFileText code
    ''' -Read text file for input data
    ''' -Default filename: br-testdata.txt
    ''' --0. Parameter set --> BRData(...,...) --> strDataforBRTest
    ''' --1. Variable set
    ''' --2. Read BR-test file: Header
    ''' --3. Read BR-test file: TestCase
    ''' </summary>
    Public Sub algReadFileText(ByRef BRData(,) As strDataforBRTest)
        '(1)
        Dim maxSet, maxItem, i, j, k As Integer
        Dim inputString(0) As String

        Try
            '(2)
            'Create an instance of StreamReader to read from a file.
            Using sr As StreamReader = New StreamReader("br-testdata.txt")
                'Number of set data + set array
                maxSet = CInt(sr.ReadLine)
                sr.ReadLine()
                sr.ReadLine()
                maxItem = CInt(sr.ReadLine)
                ReDim BRData(maxSet, maxItem)

                'Close position
                sr.Close()
            End Using

            '(3)
            Using sr As StreamReader = New StreamReader("br-testdata.txt")
                sr.ReadLine()
                'Iteration
                For i = 1 To maxSet
                    'Retrieve header data
                    inputString(0) = sr.ReadLine

                    'Retrieve container size
                    inputString(0) = sr.ReadLine
                    inputString = inputString(0).Split(New Char() {" "c})
                    ReDim BRData(i, 0).Dimension(3)
                    ReDim BRData(i, 0).Fisibility(3)
                    For j = 1 To 3
                        BRData(i, 0).Dimension(j) = CSng(inputString(j))
                    Next

                    'Enter 1 time (we don't need maxitem anymore)
                    sr.ReadLine()

                    'Retrieve item
                    For j = 1 To maxItem
                        'Set item
                        ReDim BRData(i, j).Dimension(3)
                        ReDim BRData(i, j).Fisibility(3)
                        'Get type
                        inputString(0) = sr.ReadLine
                        inputString = inputString(0).Split(New Char() {" "c})
                        BRData(i, j).TypeData.SType = CInt(inputString(1))
                        'Get item dimension + fisibility
                        For k = 1 To 3
                            BRData(i, j).Dimension(k) = CSng(inputString(k * 2))
                            If CInt(inputString((k * 2) + 1)) = 0 Then
                                BRData(i, j).Fisibility(k) = False
                            Else
                                BRData(i, j).Fisibility(k) = True
                            End If
                        Next
                        'Number item
                        BRData(i, j).TypeData.SCount = CInt(inputString(8))
                    Next
                Next
                sr.Close()
            End Using
        Catch e As Exception
            'Let the user know what went wrong.
            Console.WriteLine("The file could not be read: " & e.Message)
        End Try
    End Sub


    ''' <summary>
    ''' ##algInputText code
    ''' -Input DataGrid to MasterData
    ''' --0. Parameter set --> dataBoxParam(), listBoxParam()
    ''' --1. Variable set
    ''' --2. Input container
    ''' --3. Input box
    ''' --3a. Number box
    ''' --3b. Box
    ''' --4. Recapitulation
    ''' </summary>
    Public Sub algInputText(ByRef dataBoxParam() As Box, _
                            ByRef listBoxParam() As strBoxList)
        '(1)
        Dim i, j, count As Integer

        '(2)
        With MyForm.formMainMenu
            '(3)
            dataBoxParam(0) = New Box(0, _
                                      MyForm.formMainMenu.txtDConDepth.Text, _
                                      MyForm.formMainMenu.txtDConWidth.Text, _
                                      MyForm.formMainMenu.txtDConHeight.Text, _
                                      False, False, True)

            'dataBoxParam(0).IsAlpha = True  --> if no prob, you can erase it
            dataBoxParam(0).Orientation = False

            '(3a)
            count = 0
            For i = 1 To CInt(.dbData.RowCount - 1)
                count += CInt(.dbData.Item(4, i - 1).Value)
            Next

            '(3b)
            ReDim Preserve dataBoxParam(count)
            count = 0
            For i = 1 To .dbData.RowCount - 1
                For j = 1 To .dbData.Item(4, i - 1).Value
                    count += 1
                    dataBoxParam(count) = New Box(.dbData.Item(0, i - 1).Value, _
                                                  .dbData.Item(1, i - 1).Value, _
                                                  .dbData.Item(2, i - 1).Value, _
                                                  .dbData.Item(3, i - 1).Value, _
                                                  .dbData.Item(6, i - 1).Value, _
                                                  .dbData.Item(7, i - 1).Value, _
                                                  .dbData.Item(8, i - 1).Value)
                Next
            Next
        End With

        '(4)
        algRecapitulation(dataBoxParam, listBoxParam)
    End Sub

    ''' <summary>
    ''' ##algDrawDataGridText code
    ''' -Clear/reset data grid view
    ''' </summary>
    Private Sub algDrawDataGridReset()
        With MyForm.formMainMenu
            For i As Integer = 1 To .dbData.RowCount
                .dbData.Rows.Clear()
            Next
            For i As Integer = 1 To .dbData.ColumnCount
                .dbData.Columns.Clear()
            Next
        End With
    End Sub

    ''' <summary>
    ''' ##algDrawDataGridExcel code
    ''' -Create data grid view
    ''' -Excel: grid for excel input
    ''' --1. Clear data grid
    ''' --2. Insert column + caption
    ''' --3. Resize column width
    ''' </summary>
    Public Sub algDrawDataGridExcel()
        With MyForm.formMainMenu
            '(1)
            algDrawDataGridReset()

            '(2)
            .dbData.Columns.Add("colName", "BoxType")
            .dbData.Columns.Add("dim1", "D1")
            .dbData.Columns.Add("dim2", "D2")
            .dbData.Columns.Add("dim3", "D3")
            .dbData.Columns.Add("isPacking", "Pack")

            '(3)
            .dbData.Columns(0).Width = 50
            .dbData.Columns(1).Width = (.dbData.Width - 170) / 3
            .dbData.Columns(2).Width = (.dbData.Width - 170) / 3
            .dbData.Columns(3).Width = (.dbData.Width - 170) / 3
            .dbData.Columns(4).Width = 50
        End With

    End Sub

    ''' <summary>
    ''' ##algDrawDataGridText code
    ''' -Create data grid view
    ''' -Text: grid for i/o input
    ''' --1. Clear data grid
    ''' --2. Insert column + caption
    ''' --3. Resize column width
    ''' </summary>
    Public Sub algDrawDataGridText()
        With MyForm.formMainMenu
            '(1)
            algDrawDataGridReset()

            '(2)
            .dbData.Columns.Add("type", "Type")
            .dbData.Columns.Add("dim1", "D1")
            .dbData.Columns.Add("dim2", "D2")
            .dbData.Columns.Add("dim3", "D3")
            .dbData.Columns.Add("count", "Count")
            .dbData.Columns.Add("isPacking", "Pack")
            .dbData.Columns.Add("fis1", "F1")
            .dbData.Columns.Add("fis2", "F2")
            .dbData.Columns.Add("fis3", "F3")

            '(3)
            .dbData.Columns(0).Width = 40
            .dbData.Columns(4).Width = 40
            .dbData.Columns(5).Width = 50
            .dbData.Columns(6).Width = 40
            .dbData.Columns(7).Width = 40
            .dbData.Columns(8).Width = 40
            .dbData.Columns(1).Width = (.dbData.Width - 330) / 3
            .dbData.Columns(2).Width = (.dbData.Width - 330) / 3
            .dbData.Columns(3).Width = (.dbData.Width - 330) / 3
        End With
    End Sub

    ''' <summary>
    ''' ##algDrawDataGridAutomated code
    ''' -Create data grid view
    ''' -Automated: Grid for Result Automation
    ''' --1. Clear data grid
    ''' --2. Insert column + caption
    ''' --3. Resize column width
    ''' </summary>
    Public Sub algDrawDataGridAutomated()
        With MyForm.formMainMenu
            '(1)
            algDrawDataGridReset()

            '(2)
            .dbData.Columns.Add("Test", "Type")
            .dbData.Columns.Add("volItem", "volI")
            .dbData.Columns.Add("volPacked", "volP")
            .dbData.Columns.Add("volContainer", "volC")
            .dbData.Columns.Add("count", "Count")
            .dbData.Columns.Add("isPacking", "Pack")
            .dbData.Columns.Add("%Pack", "%U.Pack")
            .dbData.Columns.Add("%Container", "%U.Cont.")

            '(3)
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

    ''' <summary>
    ''' ##algInputExcel code
    ''' -Read excel file and input data
    ''' -Formating of the excel file can be seen on the file example
    ''' --1. Variable set
    ''' --2. Inputing all data
    ''' --2a. Read number box
    ''' --2b. Input container dimension
    ''' --2c. Input box dimension
    ''' --3. Recapitulating list-of-Box
    ''' </summary>
    Public Sub algInputExcel(ByRef dataBoxParam() As Box, ByRef listBoxParam() As strBoxList)
        '(1)
        Dim i As Integer

        '(2)
        With MyForm.formMainMenu
            '(2a)
            ReDim dataBoxParam(.dbData.RowCount - 1)

            '(2b)
            dataBoxParam(0) = New Box(0, _
                                        MyForm.formMainMenu.txtDConDepth.Text, _
                                        MyForm.formMainMenu.txtDConWidth.Text, _
                                        MyForm.formMainMenu.txtDConHeight.Text, _
                                        False, False, True)
            'dataBoxParam(0).IsAlpha = True --> if okay, delete this statement
            dataBoxParam(0).Orientation = False

            '(2c)
            For i = 1 To .dbData.RowCount - 1
                dataBoxParam(i) = New Box(.dbData.Item(0, i - 1).Value, _
                                          .dbData.Item(1, i - 1).Value, _
                                          .dbData.Item(2, i - 1).Value, _
                                          .dbData.Item(3, i - 1).Value, _
                                          .dbData.Item(6, i - 1).Value, _
                                          .dbData.Item(7, i - 1).Value, _
                                          .dbData.Item(8, i - 1).Value)
            Next
        End With

        '(3)
        algRecapitulation(dataBoxParam, listBoxParam)
    End Sub

    ''' <summary>
    ''' ##algOutputInConsole code
    ''' -Generate result in MainMenu
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Reset data
    ''' --3. Recapitulation
    ''' --4. Output
    ''' ---4a. Generating result
    ''' ---4b. Calculating utilization
    ''' ---4c. Writing in console
    ''' </summary>
    Public Sub algOutputInConsole(ByVal databoxParam() As Box)
        '(1)
        Dim i, j As Integer
        Dim listBoxOutput(Nothing) As strBoxList
        Dim volItem, volContainer, volTotal As Single

        '(2)
        volItem = 0
        volTotal = 0

        '(3)
        algRecapitulation(databoxParam, listBoxOutput)

        '(4)
        With MyForm.formMainMenu
            '(4a)
            For i = 1 To .dbData.RowCount - 1
                For j = 1 To listBoxOutput.GetUpperBound(0)
                    If CInt(.dbData.Item(0, i - 1).Value) = listBoxOutput(j).SType _
                        Then .dbData.Item(5, i - 1).Value = listBoxOutput(j).SCount
                Next

                'Calculate vol.item
                volItem += CSng(.dbData.Item(1, i - 1).Value) * _
                            CSng(.dbData.Item(2, i - 1).Value) * _
                            CSng(.dbData.Item(3, i - 1).Value) * _
                            CInt(.dbData.Item(5, i - 1).Value)
                '---
                volTotal += CSng(.dbData.Item(1, i - 1).Value) * _
                            CSng(.dbData.Item(2, i - 1).Value) * _
                            CSng(.dbData.Item(3, i - 1).Value) * _
                            CInt(.dbData.Item(4, i - 1).Value)
            Next
            '(4b)
            volContainer = CSng(MyForm.formMainMenu.txtDConDepth.Text) * _
                            CSng(MyForm.formMainMenu.txtDConHeight.Text) * _
                            CSng(MyForm.formMainMenu.txtDConWidth.Text)
            '(4c)
            .txtConsole.Text = "Item = " & volItem.ToString("#.##") & _
                                "  |  TotalItem = " & volTotal.ToString("#.##") & _
                                "   |   U.Packing = " & (100 * volItem / volTotal).ToString("#.##") & " %" & _
                                "   |   U.Container = " & (100 * volItem / volContainer).ToString("#.##") & " %"
        End With
    End Sub

    ''' <summary>
    ''' ##algOutputInExcel code
    ''' -Generate result in Excel
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Reset data
    ''' --3. Recapitulation
    ''' --4. Output
    ''' ---4a. Generating result
    ''' ---4b. Calculating utilization
    ''' ---4c. Writing in console
    ''' </summary>
    Public Sub algOutputInExcel(ByVal databoxParam() As Box)
        '(1)
        Dim i, j As Integer
        Dim volItem, volContainer, volTotal As Single

        '(2)
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

    ''' <summary>
    ''' ##algOpenFileText code
    ''' -Transfer from RawData to Grid
    ''' --0. Parameter set
    ''' --1. Container data
    ''' --2. Box data
    ''' </summary>
    Public Sub algOpenFileText(ByVal choosenSetData() As strDataforBRTest)
        With MyForm.formMainMenu
            '(1)
            .txtDConDepth.Text = choosenSetData(0).Dimension(1)
            .txtDConWidth.Text = choosenSetData(0).Dimension(2)
            .txtDConHeight.Text = choosenSetData(0).Dimension(3)

            '(2)
            '//Clearbox and add rows
            .dbData.Rows.Clear()
            For i = 1 To choosenSetData.GetUpperBound(0)
                .dbData.Rows.Add()
                '//Type
                .dbData.Item(0, i - 1).Value = choosenSetData(i).TypeData.SType
                '//Dimension
                For j = 1 To 3
                    .dbData.Item(j, i - 1).Value = choosenSetData(i).Dimension(j)
                Next
                '//Count
                .dbData.Item(4, i - 1).Value = choosenSetData(i).TypeData.SCount
                '//Fisibility
                For j = 1 To 3
                    .dbData.Item(j + 5, i - 1).Value = choosenSetData(i).Fisibility(j)
                Next
            Next
        End With
    End Sub

    ''' <summary>
    ''' ##algOpenFileExcel code
    ''' -Transfer from Excel to Grid
    ''' --0. Parameter set
    ''' --1. Variable data
    ''' --2. Open excel configuration
    ''' --3. Container data
    ''' --4. Box data
    ''' --5. Close open configuration
    ''' </summary>
    Public Sub algOpenFileExcel(ByVal sheetNumber As Integer)
        '(1)
        Dim i, j As Integer
        Dim xlApp As Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet

        '(2)
        xlApp = New Excel.ApplicationClass
        xlWorkBook = xlApp.Workbooks.Open("dataCLP.xlsx")
        xlWorkSheet = xlWorkBook.Worksheets("Sheet" & sheetNumber)

        With MyForm.formMainMenu
            '(3)
            .txtDConDepth.Text = xlWorkSheet.Cells(1, 1).Value
            .txtDConWidth.Text = xlWorkSheet.Cells(1, 2).Value
            .txtDConHeight.Text = xlWorkSheet.Cells(1, 3).Value

            '(4)
            '//Clearbox and add rows
            .dbData.Rows.Clear()
            For i = 1 To Val(xlWorkSheet.Cells(2, 1).Value)
                .dbData.Rows.Add()
                .dbData.Item(0, i - 1).Value = xlWorkSheet.Cells(i + 2, 4).value
                For j = 1 To 3
                    .dbData.Item(j, i - 1).Value = xlWorkSheet.Cells(i + 2, j).Value
                Next
            Next
        End With

        '(5)
        xlWorkBook.Close()
        xlApp.Quit()
        releaseObject(xlApp)
        releaseObject(xlWorkBook)
        releaseObject(xlWorkSheet)
    End Sub


    ''' <summary>
    ''' #algAutomatedTestData code
    ''' -Automated calculation of bischoff/ratcliff test.
    ''' -It works automatically, you just wait for the result.
    ''' -Save results to excel
    ''' --1. Variable set
    ''' --2. Read caseTest data
    ''' --3. Prepare header result in datagrid
    ''' --4. Calculation in each iteration
    ''' ---4a. Calculate data
    ''' ---4b. Get and write result
    ''' ---4c. Record data
    ''' ---4d. Sum for average
    ''' --5. Get average
    ''' --6. Write to datagrid
    ''' </summary>
    Public Sub algAutomatedTestData()
        '(1)
        Dim i, j, k As Integer
        Dim e As System.EventArgs = Nothing
        Dim tempString(0) As String

        With MyForm.formMainMenu
            '(2)
            .btnOpenFile_Click(True, e)

            '(3)
            '//Get iteration
            tempString(0) = .lblControl.Text
            tempString = tempString(0).Split(New Char() {" "c})
            '//Reset
            j = CInt(tempString(2))
            Dim recordItem(j), recordTotalItem(j), _
                recordUPacking(j), recordUContainer(j), _
                recordNumberItem(j), recordNumberPacked(j) As Single

            recordNumberItem(0) = 0
            recordNumberPacked(0) = 0
            recordItem(0) = 0
            recordTotalItem(0) = 0
            recordUPacking(0) = 0
            recordUContainer(0) = 0

            '(4)
            For i = 1 To j
                '(4a)
                .lblControl.Text = i & " / " & j
                .btnNext_Click(True, e)

                '(4b)
                tempString(0) = .txtConsole.Text
                tempString = tempString(0).Split(New Char() {" "c})

                '(4c)
                recordNumberItem(i) = 0
                recordNumberPacked(i) = 0
                For k = 1 To .dbData.RowCount - 1
                    recordNumberItem(i) += CInt(.dbData.Item(4, k - 1).Value)
                    recordNumberPacked(i) += CInt(.dbData.Item(5, k - 1).Value)
                Next
                recordItem(i) = CSng(tempString(2))
                recordTotalItem(i) = CSng(tempString(8))
                recordUPacking(i) = CSng(tempString(16))
                recordUContainer(i) = CSng(tempString(25))

                '(4d)
                recordNumberItem(0) += recordNumberItem(i)
                recordNumberPacked(0) += recordNumberPacked(i)
                recordItem(0) += recordItem(i)
                recordTotalItem(0) += recordTotalItem(i)
                recordUPacking(0) += recordUPacking(i)
                recordUContainer(0) += recordUContainer(i)
            Next

            '(5)
            recordNumberItem(0) = recordNumberItem(0) / j
            recordNumberPacked(0) = recordNumberPacked(0) / j
            recordItem(0) = recordItem(0) / j
            recordTotalItem(0) = recordTotalItem(0) / j
            recordUPacking(0) = recordUPacking(0) / j
            recordUContainer(0) = recordUContainer(0) / j

            '(6)
            algDrawDataGridAutomated()
            '//List data + list average --> row 0
            For i = 0 To j
                .dbData.Rows.Add()
                '//Type
                If i = 0 Then
                    .dbData.Item(0, i).Value = "AVG"
                    .dbData.Item(3, i).Value = (CSng(.txtDConDepth.Text) * CSng(.txtDConHeight.Text) * CSng(.txtDConWidth.Text)).ToString("#.##")
                Else
                    .dbData.Item(0, i).Value = i
                End If
                '//volitem + volpacked + volcontainer
                .dbData.Item(1, i).Value = recordItem(i).ToString("#.##")
                .dbData.Item(2, i).Value = recordTotalItem(i).ToString("#.##")
                '.dbData.Item(3, i).Value = recordContainer(i).ToString("#.##")
                '//count + packing
                .dbData.Item(4, i).Value = recordNumberItem(i).ToString("#")
                .dbData.Item(5, i).Value = recordNumberPacked(i).ToString("#")
                '//% pack + %container
                .dbData.Item(6, i).Value = recordUPacking(i).ToString("#.##") & "%"
                .dbData.Item(7, i).Value = recordUContainer(i).ToString("#.##") & "%"
            Next
        End With
    End Sub

    ''' <summary>
    ''' #ReleaseObject code
    ''' -Standard procedure to reelease object after binding it.
    ''' </summary>
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
    ''' ##algPrintBox code
    ''' -Print result into preview area
    ''' --0. Parameter set
    ''' --- Solution --&gt; coordinat Box that want to draw in preview
    ''' --- Orientation --&gt; ???
    ''' --1. Variable set
    ''' --2. Get bounding box dimension
    ''' --3. Get actual scalling
    ''' --- Methods by using scale-ratio variable
    ''' --- ex: origin*scale-ratio = template --&gt; scale ratio = template / origin
    ''' --- if origin &lt; template --&gt; scale-ratio &gt; 1
    ''' --- if origin &gt; template --&gt; scale-ratio &lt; 1
    ''' --- if 2 origin, pic the minimum scale-ratio
    ''' --4. Scale and rendering all cube
    ''' --4a. Rendering if orientation = true --&gt; container, box
    ''' --4b. Rendering if orientation = false --&gt; container, box
    ''' --5. Drawing container and box
    ''' </summary>
    Public Sub algPrintBox(ByVal solution() As Box, _
                              ByVal orientation As Boolean)
        '(1)
        Dim initiate As New Cube(0, 0, solution(0).Height, solution(0).Width, solution(0).Depth, 1, 1)
        Dim picWidth, picHeight As Integer

        '(2)
        picWidth = initiate.BoundsRect.Width - initiate.Width
        picHeight = initiate.BoundsRect.Height - initiate.Height

        '(3)
        Dim sclRatio As Single
        If (MyForm.formMainMenu.picResult.Width / initiate.BoundsRect.Width) _
            < (MyForm.formMainMenu.picResult.Height / initiate.BoundsRect.Height) Then
            sclRatio = (MyForm.formMainMenu.picResult.Width - 1) / initiate.BoundsRect.Width
        Else
            sclRatio = (MyForm.formMainMenu.picResult.Height - 1) / initiate.BoundsRect.Height
        End If

        '(4)
        Dim bm As New Bitmap(MyForm.formMainMenu.picResult.Width, MyForm.formMainMenu.picResult.Height)
        Dim gr As Graphics = Graphics.FromImage(bm)

        '//
        '//position in cube class
        '//x = 0 --> for width
        '//y = 0 --> for height
        '//0,0,0 = {origin}
        '//posX = 0
        '//posY = 0
        '//posZ = initiate.height * sclratio
        '
        '//how to get ???
        Dim drawCube(solution.GetUpperBound(0)) As Cube

        '(4a, 4b)
        If orientation = True Then
            '--container
            drawCube(0) = New Cube(0, 0, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, 1, 1)

            '--box
            For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
                drawCube(i) = New Cube(fPositionZplusInPicture(sclRatio, _
                                                         solution(i).AbsPos1, _
                                                         solution(i).Height, _
                                                         initiate.Height, _
                                                         1, 1), _
                                       solution(i).Height * sclRatio, _
                                       solution(i).Width * sclRatio, _
                                       solution(i).Depth * sclRatio, _
                                       1, 1)
            Next
        Else
            '//!!!
            '//masi blon bener..males..hoohoo
            '//--container
            drawCube(0) = New Cube(0, _
                                   initiate.BoundsRect.Height, _
                                   solution(0).Height * sclRatio, _
                                   solution(0).Width * sclRatio, _
                                   solution(0).Depth * sclRatio, _
                                   -1, -1)
            '//--box
            For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
                drawCube(i) = New Cube(fPositionZplusInPicture(sclRatio, _
                                                         solution(i).AbsPos1, _
                                                         solution(i).Height, _
                                                         initiate.Height, _
                                                         -1, -1), _
                                       solution(i).Height * sclRatio, _
                                       solution(i).Width * sclRatio, _
                                       solution(i).Depth * sclRatio, _
                                       -1, -1)
            Next
        End If

        '(5)
        gr.DrawPath(Pens.Red, drawCube(0).GetContainer)
        For i As Integer = 1 To drawCube.GetUpperBound(0)
            gr.DrawPath(Pens.Blue, drawCube(i).GetCube)
        Next
        MyForm.formMainMenu.picResult.Image = bm
        gr.Dispose()
    End Sub

    ''' <summary>
    ''' #functionPositioninPicture code
    ''' -Position Zplus in picture
    ''' --0. Parameter set
    ''' --1. Variable set
    ''' --2. Position scaling
    ''' --3. Change position --&gt; cube to upper
    ''' --4. Transform position
    ''' </summary>
    Public Function fPositionZplusInPicture(ByVal scl As Single, _
                                               ByVal positionContainer As Point3D, _
                                               ByVal height As Integer, _
                                               ByVal heightContainer As Integer, _
                                               ByVal rotateX As RotateHorizontal, _
                                               ByVal rotateY As RotateVertical) As Point
        '(1)
        Dim x, y, z As Single

        '(2)
        x = positionContainer.X * scl
        y = positionContainer.Y * scl
        z = heightContainer - positionContainer.Z

        '(3)
        'Position diubah sehingga titik cube ke atas (tambahin Z aja)
        z = (z - height) * scl

        '(???)
        'Position diubah sehingga ke titik origin

        '(4)
        fPositionZplusInPicture.X = y + (x / 2 * rotateX) 'width position
        fPositionZplusInPicture.Y = z + (x / 2 * rotateY) 'height position
    End Function
End Module


'''' <summary>
'''' #algDrawing code
'''' </summary>
'Public Sub algDrawing(ByVal solution() As Box, ByVal orientation As Boolean)
'    'getting the bounding box
'    Dim initiate As New Cube(0, 0, solution(0).Height, solution(0).Width, solution(0).Depth, 1, 1)

'    'getting the bounding box.. will help to scaling picture box
'    Dim picWidth, picHeight As Integer
'    picWidth = initiate.BoundsRect.Width - initiate.Width
'    picHeight = initiate.BoundsRect.Height - initiate.Height

'    'get scaling
'    'methos scaling is by using scale-ratio variable
'    'ex: origin*scale-ratio = template --> scale ratio = template / origin
'    '---if origin < template --> scale-ratio > 1
'    '---if origin > template --> scale-ratio < 1
'    '---if 2 origin, pic the minimum scale-ratio
'    Dim sclRatio As Single
'    If (MyForm.formMainMenu.picResult.Width / initiate.BoundsRect.Width) _
'        < (MyForm.formMainMenu.picResult.Height / initiate.BoundsRect.Height) Then
'        sclRatio = (MyForm.formMainMenu.picResult.Width - 1) / initiate.BoundsRect.Width
'    Else
'        sclRatio = (MyForm.formMainMenu.picResult.Height - 1) / initiate.BoundsRect.Height
'    End If


'    'scale and draw all cube
'    Dim bm As New Bitmap(MyForm.formMainMenu.picResult.Width, MyForm.formMainMenu.picResult.Height)
'    Dim gr As Graphics = Graphics.FromImage(bm)

'    'position in cube class
'    'x = 0 --> for width
'    'y = 0 --> for height
'    '
'    '0,0,0 = {origin}
'    ' posX = 0
'    ' posY = 0
'    ' posZ = initiate.height * sclratio
'    '
'    'how to get 
'    Dim drawCube(solution.GetUpperBound(0)) As Cube

'    If orientation = True Then
'        '--container
'        drawCube(0) = New Cube(0, 0, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, 1, 1)

'        '--box
'        For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
'            drawCube(i) = New Cube(fPositionZplusInPicture(sclRatio, solution(i).LocationContainer, solution(i).Height, initiate.Height, 1, 1), _
'                                   solution(i).Height * sclRatio, _
'                                   solution(i).Width * sclRatio, _
'                                   solution(i).Depth * sclRatio, _
'                                   1, 1)
'        Next
'    Else
'        'masi blon bener..males..hoohoo
'        '--container
'        drawCube(0) = New Cube(0, initiate.BoundsRect.Height, solution(0).Height * sclRatio, solution(0).Width * sclRatio, solution(0).Depth * sclRatio, -1, -1)

'        '--box
'        For i = 1 To solution.GetUpperBound(0)      'write cube for all solution (getupper)
'            drawCube(i) = New Cube(fPositionZplusInPicture(sclRatio, solution(i).LocationContainer, solution(i).Height, initiate.Height, -1, -1), _
'                                   solution(i).Height * sclRatio, _
'                                   solution(i).Width * sclRatio, _
'                                   solution(i).Depth * sclRatio, _
'                                   -1, -1)
'        Next
'    End If

'    'drawing container + cube
'    gr.DrawPath(Pens.Red, drawCube(0).GetContainer)
'    For i As Integer = 1 To drawCube.GetUpperBound(0)
'        gr.DrawPath(Pens.Blue, drawCube(i).GetCube)
'    Next

'    '--bounding box draw TEMPORARY
'    gr.DrawPath(Pens.Green, drawCube(drawCube.GetUpperBound(0)).GetCube)

'    MyForm.formMainMenu.picResult.Image = bm
'    gr.Dispose()
'End Sub
