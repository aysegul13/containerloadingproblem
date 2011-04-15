'#Importing classes
'--vb form controller
'--drawing 2d
'--drawing picture
Imports System.Windows.Forms

Public Class ExamineMenu
    ''' <summary>
    ''' #packing variable
    ''' </summary>
    Private packing As Plot3D

    ''' <summary>
    ''' #cube Container
    ''' </summary>
    Private fcubeCont As Cube

    ''' <summary>
    ''' #cube Pointer
    ''' </summary>
    Private fcubePoint As Cube

    ''' <summary>
    ''' #cube Space
    ''' </summary>
    Private fcubeSpace() As Cube

    ''' <summary>
    ''' #cube Box
    ''' </summary>
    Private fcubeBox() As Cube



    ''' <summary>
    ''' #frmMainMenu code
    ''' --0. Parameter set
    ''' --1. Add reference for Automatic handling
    ''' --2. Add textConsole result
    ''' --3. Clear and add dataGrid
    ''' --4. Input data
    ''' --5. GetCube 
    ''' --6. Draw Picture
    ''' </summary>
    Private Sub frmExamine_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '(1)
        '//Add references to automatic handling
        MyForm.formExamine = Me
        '(2)
        MyForm.formExamine.txtConsole.Text = MyForm.formMainMenu.txtConsole.Text
        '(3)
        algDrawDataGridExamine()
        '(4)
        algDrawDataGridInputExamine(packing.OutputBox, packing.OutputSpace)
        '(5)
        GetCubeRaw()
        '(6)
        algPrintBox2(fcubeCont, fcubePoint, fcubeBox, fcubeSpace, False)
    End Sub

    ''' <summary>
    ''' #Packing result variable
    ''' </summary>
    Public Property packResult() As Plot3D
        Get
            Return packing
        End Get
        Set(ByVal value As Plot3D)
            packing = value
        End Set
    End Property

    ''' <summary>
    ''' #CheckBox variable
    ''' </summary>
    Private Sub chkBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkBox.CheckedChanged
        If chkBox.Checked = True Then
            For i = 1 To dbDataBox.RowCount
                dbDataBox.Item(0, i - 1).Value = True
            Next
        Else
            For i = 1 To dbDataBox.RowCount
                dbDataBox.Item(0, i - 1).Value = False
            Next
        End If
    End Sub

    ''' <summary>
    ''' #CheckSpace variable
    ''' </summary>
    Private Sub chkSpace_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSpace.CheckedChanged
        If chkSpace.Checked = True Then
            For i = 1 To dbDataSpace.RowCount
                dbDataSpace.Item(0, i - 1).Value = True
            Next
        Else
            For i = 1 To dbDataSpace.RowCount
                dbDataSpace.Item(0, i - 1).Value = False
            Next
        End If
    End Sub

    ''' <summary>
    ''' #Draw picture
    ''' --1. Variable set
    ''' --1. GetCubeExamine
    ''' --2. Drawing picture
    ''' </summary>
    Private Sub btnExamine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExamine.Click
        '(1)
        Dim cubeBox(Nothing), cubeSpace(Nothing) As Cube
        '(2)
        GetCubeExamine(cubeBox, cubeSpace)
        '(3)
        algPrintBox2(fcubeCont, fcubePoint, cubeBox, cubeSpace, False)
    End Sub


    ''' <summary>
    ''' #Draw picture
    ''' --1. Variable set
    ''' --2. Copy data
    ''' --3. Drawing picture
    ''' </summary>
    Private Sub GetCubeExamine(ByRef cubeBox() As Cube, ByRef cubeSpace() As Cube)
        '(1)
        Dim i, j As Integer
        ReDim cubeBox(fcubeBox.GetUpperBound(0)), cubeSpace(fcubeSpace.GetUpperBound(0))

        '(2)
        '//Box
        j = 0
        For i = 1 To dbDataBox.RowCount
            If dbDataBox.Item(0, i - 1).Value = True Then
                j += 1
                cubeBox(j) = New Cube(fcubeBox(CInt(dbDataBox.Item(9, i - 1).Value)))
            End If
        Next
        ReDim Preserve cubeBox(j)
        '//Space
        j = 0
        For i = 1 To dbDataSpace.RowCount
            If dbDataSpace.Item(0, i - 1).Value = True Then
                j += 1
                cubeSpace(j) = New Cube(fcubeSpace(CInt(dbDataSpace.Item(7, i - 1).Value)))
            End If
        Next
        ReDim Preserve cubeSpace(j)
    End Sub


    ''' <summary>
    ''' #Get Cube
    ''' --1. Variable set
    ''' --2. Count number Box and Space
    ''' --3. Copy data
    ''' --4. GetCube
    ''' </summary>
    Private Sub GetCubeRaw()
        '(1)
        Dim i, countBox, countSpace As Integer

        chkBox.Enabled = True
        chkSpace.Enabled = True
        chkBox.Checked = True
        chkSpace.Checked = True

        '(2)
        countBox = 0
        For i = 1 To dbDataBox.RowCount
            If dbDataBox.Item(0, i - 1).Value = True Then countBox += 1
        Next
        countSpace = 0
        For i = 1 To dbDataSpace.RowCount
            If dbDataSpace.Item(0, i - 1).Value = True Then countSpace += 1
        Next

        '(3)
        Dim dataCont, dataBox(countBox), dataSpace(countSpace) As Box
        dataCont = packing.Box(0)

        For i = 1 To dbDataBox.RowCount
            If dbDataBox.Item(0, i - 1).Value = True Then _
                dataBox(CInt(dbDataBox.Item(9, i - 1).Value)) = packing.Box(CInt(dbDataBox.Item(9, i - 1).Value))
        Next

        For i = 1 To dbDataSpace.RowCount
            If dbDataSpace.Item(0, i - 1).Value = True Then _
                dataSpace(CInt(dbDataSpace.Item(7, i - 1).Value)) = packing.Space(CInt(dbDataSpace.Item(7, i - 1).Value))
        Next

        '(4)
        algBoxtoCube(dataCont, dataBox, dataSpace, True, _
                     fcubeCont, fcubeBox, fcubeSpace)
    End Sub

    ''' <summary>
    ''' #Cell mouse enter in dbBox
    ''' 1. Variable set
    ''' 2. Get cube pointer
    ''' 3. Get cube examine
    ''' 4. Draw cube
    ''' </summary>
    Private Sub dbDataBox_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dbDataBox.CellMouseEnter        'dataBox(1) = packing.Box(CInt(dbDataBox.Item(9, e.RowIndex).Value))
        If (-1 < e.RowIndex) And (e.RowIndex < dbDataBox.RowCount) Then
            '(1)
            Dim cubeBox(Nothing), cubeSpace(Nothing) As Cube
            '(2)
            fcubePoint = New Cube(fcubeBox(CInt(dbDataBox.Item(9, e.RowIndex).Value)))
            '(3)
            GetCubeExamine(cubeBox, cubeSpace)
            '(4)
            algPrintBox2(fcubeCont, fcubePoint, cubeBox, cubeSpace, True)
        End If
    End Sub

    ''' <summary>
    ''' #Cell mouse leave in dbBox
    ''' Call Examine click
    ''' </summary>
    Private Sub dbDataBox_CellMouseLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dbDataBox.CellMouseLeave
        btnExamine_Click(True, e)
    End Sub

    ''' <summary>
    ''' #Cell content click in dbBox
    ''' </summary>
    Private Sub dbDataBox_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dbDataBox.CellContentClick
        If (-1 < e.RowIndex) And (e.RowIndex < dbDataBox.RowCount) Then _
            dbDataBox.Item(0, e.RowIndex).Value = Not (dbDataBox.Item(0, e.RowIndex).Value)
    End Sub

    ''' <summary>
    ''' #Cell mouse enter in dbSpace
    ''' 1. Variable set
    ''' 2. Get cube pointer
    ''' 3. Get cube examine
    ''' 4. Draw cube
    ''' </summary>
    Private Sub dbDataSpace_CellMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dbDataSpace.CellMouseEnter
        If (-1 < e.RowIndex) And (e.RowIndex < dbDataSpace.RowCount) Then
            '(1)
            Dim cubeBox(Nothing), cubeSpace(Nothing) As Cube
            '(2)
            fcubePoint = New Cube(fcubeSpace(dbDataSpace.Item(7, e.RowIndex).Value))
            '(3)
            GetCubeExamine(cubeBox, cubeSpace)
            '(4)
            algPrintBox2(fcubeCont, fcubePoint, cubeBox, cubeSpace, True)
        End If
    End Sub

    ''' <summary>
    ''' #Cell mouse leave in dbSpace
    ''' Call Examine click
    ''' </summary>
    Private Sub dbDataSpace_CellMouseLeave(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dbDataSpace.CellMouseLeave
        btnExamine_Click(True, e)
    End Sub

    ''' <summary>
    ''' #Cell content click in dbSpace
    ''' </summary>
    Private Sub dbDataSpace_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dbDataSpace.CellContentClick
        If (-1 < e.RowIndex) And (e.RowIndex < dbDataSpace.RowCount) Then _
            dbDataSpace.Item(0, e.RowIndex).Value = Not (dbDataSpace.Item(0, e.RowIndex).Value)
    End Sub

    ''' <summary>
    ''' #Cell sort in dbBox
    ''' Call Examine click
    ''' </summary>
    Private Sub dbDataBox_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles dbDataBox.Sorted
        btnExamine_Click(True, e)
    End Sub

    ''' <summary>
    ''' #Cell sort in dbSpace
    ''' Call Examine click
    ''' </summary>
    Private Sub dbDataSpace_Sorted(ByVal sender As Object, ByVal e As System.EventArgs) Handles dbDataSpace.Sorted
        btnExamine_Click(True, e)
    End Sub
End Class