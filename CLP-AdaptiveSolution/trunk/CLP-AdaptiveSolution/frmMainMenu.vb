'#Importing classes
'--vb form controller
'--drawing 2d
'--drawing picture
Imports System.Windows.Forms


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
''' frmMainMenu.vb
''' ---
''' Coding of MainMenu control.
''' </summary>
Public Class MainMenu
    ' #Variable declaration
    ' --IMO, after re-read, I forget a lot.
    ' --CLS comment if remember.
    '
    ' -RawData variable
    ' --TextFiles that capture in memory
    ' --This variable is used due to computational purpose, esp. for testing large data
    Dim RawData(Nothing)() As strDataforBRTest
    '-RawNumber variable
    Dim RawNumberNow As Integer

    '-Graphic variable
    Dim locX, locY, locHeight, locDepth, locWidth As Integer
    Friend limIter As Integer
    Dim rotX As RotateHorizontal
    Dim rotY As RotateVertical

    Dim packingResult As Plot3D

    'runTime
    Private frunTime As TimeSpan

    ''' <summary>
    ''' #frmMainMenu code
    ''' </summary>
    Private Sub frmMainMenu_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Add: References to Automatic Handling
        MyForm.formMainMenu = Me
        'Add: Automatic opening file --> minimize effort in testing program
        btnOpenFile_Click(True, e)

        '//coding for automatic running
        'For i = 1 To 81
        '    btnNext_Click(True, e)
        'Next
        'btnExecute_Click(True, e)

        'algDrawDataGridExcel()
        'algOpenFileExcel(1)
        '//
    End Sub

    ''' <summary>
    ''' #btnOpenFile code
    ''' --(1) Disable button when reading data start
    ''' --(2) Get data from text, writing the status, set position RawNumber = 0
    ''' --(3) Enable button when reading data finish
    ''' </summary>
    Friend Sub btnOpenFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOpenFile.Click
        '(1)
        btnPrev.Enabled = False
        btnNext.Enabled = False

        '(2)
        algReadFileText(RawData)
        lblControl.Text = 0 & " / " & RawData.GetUpperBound(0)
        RawNumberNow = 0

        '(3)
        btnPrev.Enabled = True
        btnNext.Enabled = True
    End Sub

    ''' <summary>
    '''#btnExecute code
    '''--It's a toggle to execute the program
    '''--1. Deactivate: database-grid + checkbox + another button + frmExamine
    '''--2. Call Execution procedure
    '''--3. Reactivate: database-grid + checkbox + another button
    ''' </summary>
    Friend Sub btnExecute_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExecute.Click
        '(1)
        dbData.Visible = False
        btnExamine.Enabled = False
        chkCuboid.Enabled = False
        chkWall.Enabled = False
        chkStack.Enabled = False
        trckWall.Enabled = False
        trckStack.Enabled = False

        listConsole.Clear()
        MyForm.formExamine.Close()

        '(2)
        If (chkCuboid.Checked = False) And _
            (chkWall.Checked = False) And _
            (chkStack.Checked = False) Then
            txtConsole.Text = "Method is not checked"
        Else
            Execution.Execute(packingResult, frunTime)
            'txtConsole.Text = txtConsole.Text & "  |  RunTime = " & frunTime.TotalSeconds
            btnExamine.Enabled = True
        End If

        '(3)
        dbData.Visible = True
        chkCuboid.Enabled = True
        chkWall.Enabled = True
        chkStack.Enabled = True
        trckWall.Enabled = True
        trckStack.Enabled = True
    End Sub

    ''' <summary>
    ''' #btnPrev code
    ''' --It's a toogle for btnPrev code
    ''' --1. Not working if prev < 1
    ''' --2. Repositioning 'RawNumberNow' variable
    ''' --3. Copy data to 'Temp' --> using in algOpenFileText
    ''' --4. Get preview
    ''' --5. Execute data
    ''' </summary>
    Friend Sub btnPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrev.Click
        '(1)
        If ((RawNumberNow - 1) > RawData.GetLowerBound(0)) Then
            '(2)
            RawNumberNow -= 1
            lblControl.Text = RawNumberNow & " / " & RawData.GetUpperBound(0)

            '(3)
            Dim Temp(RawData(RawNumberNow).GetUpperBound(0)) As strDataforBRTest
            For i As Integer = 0 To RawData(RawNumberNow).GetUpperBound(0)
                Temp(i) = RawData(RawNumberNow)(i)
            Next

            '(4)
            picResult.Refresh()
            algDrawDataGridText()
            algOpenFileText(Temp)

            '(5)
            'btnExecute_Click(True, e)
        End If
    End Sub

    ''' <summary>
    ''' #btnPrev code
    ''' --It's a toogle for btnPrev code
    ''' --1. Not working if next > 1
    ''' --2. Repositioning 'RawNumberNow' variable
    ''' --3. Copy data to 'Temp' --> using in algOpenFileText
    ''' --4. Get preview
    ''' --5. Execute data
    ''' </summary>
    Friend Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        '(1)
        If ((RawNumberNow + 1) <= RawData.GetUpperBound(0)) Then
            '(2)
            RawNumberNow += 1
            lblControl.Text = RawNumberNow & " / " & RawData.GetUpperBound(0)

            '(3)
            Dim temp(RawData(RawNumberNow).GetUpperBound(0)) As strDataforBRTest
            For i As Integer = 0 To RawData(RawNumberNow).GetUpperBound(0)
                temp(i) = RawData(RawNumberNow)(i)
            Next

            '(4)
            picResult.Refresh()
            algDrawDataGridText()
            algOpenFileText(temp)

            '(5)
            'btnExecute_Click(True, e)
        End If
    End Sub

    ''' <summary>
    ''' #btnAutmated code
    ''' -Run automatic execution
    ''' </summary>
    Friend Sub btnAutomated_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAutomated.Click
        algAutomatedTestData()
    End Sub

    ''' <summary>
    ''' #txtGoTo code
    ''' -Go to the number of rawdata
    ''' --1. Read if ENTER PRESSED
    ''' --2. If RawData in Range
    ''' --3. Repositioning 'RawNumberNow' variable
    ''' --4. Copy data to 'Temp' --> using in algOpenFileText
    ''' --5. Get preview
    ''' --6. Execute data
    ''' </summary>
    Private Sub txtGoTo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtGoTo.KeyPress
        '(1)
        If e.KeyChar = Chr(13) Then
            '(2)
            If (RawData.GetLowerBound(0) < CInt(txtGoTo.Text)) And (CInt(txtGoTo.Text) <= RawData.GetUpperBound(0)) Then
                '(3)
                RawNumberNow = CInt(txtGoTo.Text)
                lblControl.Text = RawNumberNow & " / " & RawData.GetUpperBound(0)

                '(4)
                Dim temp(RawData(RawNumberNow).GetUpperBound(0)) As strDataforBRTest
                For i As Integer = 0 To RawData(RawNumberNow).GetUpperBound(0)
                    temp(i) = RawData(RawNumberNow)(i)
                Next

                '(5)
                picResult.Refresh()
                algDrawDataGridText()
                algOpenFileText(temp)

                '(6)
                'btnExecute_Click(True, e)
            End If
        End If
    End Sub

    ''' <summary>
    ''' #btnExamine code
    ''' -Show frmExamine
    ''' </summary>
    Private Sub btnExamine_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExamine.Click
        Dim frmExamine As New ExamineMenu
        frmExamine.packResult = packingResult
        frmExamine.Show()
    End Sub

    Private Sub s_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles s.Click
        Dim i As String
        i = InputBox("Type new stacking-iteration parameter!" & vbCrLf & "Current value = " & limIter, "Stack Method", 0, , )
        If Val(i) <> 0 Then limIter = Val(i)
    End Sub
End Class