Imports System.Windows.Forms                    'vb form controller
Imports System.Drawing.Drawing2D                'drawing in 2D
Imports System.Drawing                          'drawing picture

Public Class MainMenu
    'variable
    Dim testData(Nothing, Nothing) As setData
    Dim currentDataSet As Integer

    Dim locX, locY, locHeight, locDepth, locWidth As Integer
    Dim rotX As RotateHorizontal
    Dim rotY As RotateVertical


    Private Sub frmMainMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'references to handle automatic
        myForm.formMainMenu = Me

        btnOpenFile_Click(True, e)
        For i = 1 To 50
            btnNext_Click(True, e)
        Next
        btnExecute_Click(True, e)

        'algDrawDataGridExcel()
        'algOpenFileExcel(1)
    End Sub

    Private Sub btnExecute_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExecute.Click
        dbData.Visible = False
        Execution.Execute()
        dbData.Visible = True
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
        If (currentDataSet - 1 >= testData.GetLowerBound(0)) Then
            currentDataSet -= 1
            lblControl.Text = currentDataSet & " / " & testData.GetUpperBound(0)
            picResult.Refresh()

            Dim temp(testData.GetUpperBound(1)) As setData
            For i As Integer = 0 To testData.GetUpperBound(1)
                temp(i) = testData(currentDataSet, i)
            Next

            algDrawDataGridText()
            algOpenFileText(temp)
        End If
    End Sub

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        If (currentDataSet + 1 <= testData.GetUpperBound(0)) Then
            currentDataSet += 1
            lblControl.Text = currentDataSet & " / " & testData.GetUpperBound(0)
            picResult.Refresh()

            Dim temp(testData.GetUpperBound(1)) As setData
            For i As Integer = 0 To testData.GetUpperBound(1)
                temp(i) = testData(currentDataSet, i)
            Next

            algDrawDataGridText()
            algOpenFileText(temp)
        End If
    End Sub

    Private Sub btnOpenFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenFile.Click
        btnPrev.Enabled = False
        btnNext.Enabled = False

        algReadFileText(testData)
        lblControl.Text = 0 & " / " & testData.GetUpperBound(0)
        currentDataSet = 0

        btnPrev.Enabled = True
        btnNext.Enabled = True
    End Sub
End Class


