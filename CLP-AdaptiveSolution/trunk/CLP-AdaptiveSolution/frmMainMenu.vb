Imports System.Windows.Forms
Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Drawing.Drawing2D
Imports System.Drawing


Public Class MainMenu
    Dim locX, locY, locHeight, locDepth, locWidth As Integer
    Dim rotX As RotateHorizontal
    Dim rotY As RotateVertical

    Private Sub frmMainMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'references to handle automatic
        myForm.formMainMenu = Me

        btnOpenFile_Click(True, e)
        openDataGridExcel(1)
    End Sub


    Private Sub btnExecute_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExecute.Click
        dbData.Visible = False
        'Execution.Execute()
        Try
            Execution.Execute()
        Catch ex As Exception
        End Try
        dbData.Visible = True
    End Sub

    Private Sub openDataGridExcel(ByVal sheetNumber As Integer)
        Dim i, j As Integer

        Dim xlApp As Excel.Application
        Dim xlWorkBook As Excel.Workbook
        Dim xlWorkSheet As Excel.Worksheet
        xlApp = New Excel.ApplicationClass
        xlWorkBook = xlApp.Workbooks.Open("E:\Documents\Research\dataCLP.xlsx")          '#nanti diganti alamat filenya
        xlWorkSheet = xlWorkBook.Worksheets("Sheet" & sheetNumber)

        '----transfer data to grid
        'container dimension
        txtDConDepth.Text = xlWorkSheet.Cells(1, 1).Value
        txtDConWidth.Text = xlWorkSheet.Cells(1, 2).Value
        txtDConHeight.Text = xlWorkSheet.Cells(1, 3).Value

        'clearbox and add rows
        dbData.Rows.Clear()

        'box dimension
        For i = 1 To Val(xlWorkSheet.Cells(2, 1).Value)
            dbData.Rows.Add()
            dbData.Item(0, i - 1).Value = xlWorkSheet.Cells(i + 2, 4).value
            For j = 1 To 3
                dbData.Item(j, i - 1).Value = xlWorkSheet.Cells(i + 2, j).Value
            Next
        Next
        '----finish transfer data to grid

        xlWorkBook.Close()
        xlApp.Quit()

        releaseObject(xlApp)
        releaseObject(xlWorkBook)
        releaseObject(xlWorkSheet)
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

    Private Sub btnRotX_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRotX.Click
        'draw picture
        rotX += 1
        'drawPicture(locX, locY, locHeight, locWidth, locDepth, rotX, rotY)
    End Sub

    Private Sub btnRotY_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRotY.Click
        'draw picture
        rotY += 1
        'drawPicture(locX, locY, locHeight, locWidth, locDepth, rotX, rotY)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        rotX += -1
        'drawPicture(locX, locY, locHeight, locWidth, locDepth, rotX, rotY)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'draw picture
        rotY += -1
        'drawPicture(locX, locY, locHeight, locWidth, locDepth, rotX, rotY)
    End Sub

    Private Sub btnPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrev.Click

    End Sub

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click

    End Sub

    Private Sub btnOpenFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenFile.Click
        For i As Integer = 1 To dbData.RowCount
            dbData.Rows.Clear()
        Next
        For i As Integer = 1 To dbData.ColumnCount
            dbData.Columns.Clear()
        Next

        dbData.Columns.Add("colName", "BoxType")
        dbData.Columns.Add("dim1", "D1")
        dbData.Columns.Add("dim2", "D2")
        dbData.Columns.Add("dim3", "D3")
        dbData.Columns.Add("isPacking", "Pack")
        dbData.Columns(0).Width = 50
        dbData.Columns(1).Width = (dbData.Width - 170) / 3
        dbData.Columns(2).Width = (dbData.Width - 170) / 3
        dbData.Columns(3).Width = (dbData.Width - 170) / 3
        dbData.Columns(4).Width = 50
        openDataGridExcel(1)
    End Sub
End Class


