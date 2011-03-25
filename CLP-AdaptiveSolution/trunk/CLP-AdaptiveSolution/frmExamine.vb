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
    ''' #frmMainMenu code
    ''' --0. Parameter set
    ''' --1. Add reference for Automatic handling
    ''' --2. Add textConsole result
    ''' --3. Clear and add dataGrid
    ''' --4. Input data
    ''' </summary>
    Private Sub frmExamine_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '(1)
        'Add: References to Automatic Handling
        MyForm.formExamine = Me

        '(2)
        MyForm.formExamine.txtConsole.Text = MyForm.formMainMenu.txtConsole.Text

        '(3)
        algDrawDataGridExamine()

        '(4)
        algDrawDataGridInputExamine(packing.OutputBox, packing.OutputSpace)
        chkBox.Checked = True
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
            For i = 1 To dbDataBox.RowCount - 1
                dbDataBox.Item(0, i - 1).Value = True
            Next
        Else
            For i = 1 To dbDataBox.RowCount - 1
                dbDataBox.Item(0, i - 1).Value = False
            Next
        End If
    End Sub

    ''' <summary>
    ''' #CheckSpace variable
    ''' </summary>
    Private Sub chkSpace_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkSpace.CheckedChanged
        If chkSpace.Checked = True Then
            For i = 1 To dbDataSpace.RowCount - 1
                dbDataSpace.Item(0, i - 1).Value = True
            Next
        Else
            For i = 1 To dbDataSpace.RowCount - 1
                dbDataSpace.Item(0, i - 1).Value = False
            Next
        End If
    End Sub

    ''' <summary>
    ''' #Draw picture
    ''' --1. Variable set
    ''' --2. Count number Box and Space
    ''' --3. Copy data
    ''' --4. Drawing picture
    ''' </summary>
    Private Sub btnExamine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExamine.Click
        '(1)
        Dim countBox, countSpace As Integer
        Dim i, j As Integer

        '(2)
        countBox = 0
        For i = 1 To dbDataBox.RowCount - 1
            If dbDataBox.Item(0, i - 1).Value = True Then countBox += 1
        Next
        countSpace = 0
        For i = 1 To dbDataSpace.RowCount - 1
            If dbDataSpace.Item(0, i - 1).Value = True Then countSpace += 1
        Next

        '(3)
        Dim dataCont, dataBox(countBox), dataSpace(countSpace) As Box
        dataCont = packing.Box(0)

        j = 0
        For i = 1 To dbDataBox.RowCount - 1
            If dbDataBox.Item(0, i - 1).Value = True Then
                j += 1
                dataBox(j) = packing.Box(CInt(dbDataBox.Item(9, i - 1).Value))
            End If
        Next

        j = 0
        For i = 1 To dbDataSpace.RowCount - 1
            If dbDataSpace.Item(0, i - 1).Value = True Then
                j += 1
                dataSpace(j) = packing.Space(CInt(dbDataSpace.Item(6, i - 1).Value))
            End If
        Next

        '(4)
        algPrintBox2(dataCont, dataBox, dataSpace, True)
    End Sub
End Class