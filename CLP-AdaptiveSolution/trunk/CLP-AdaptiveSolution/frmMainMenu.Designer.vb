﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainMenu
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtDConHeight = New System.Windows.Forms.TextBox
        Me.txtDConWidth = New System.Windows.Forms.TextBox
        Me.txtDConDepth = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.dbData = New System.Windows.Forms.DataGridView
        Me.btnExecute = New System.Windows.Forms.Button
        Me.picResult = New System.Windows.Forms.PictureBox
        Me.btnY = New System.Windows.Forms.Button
        Me.btnOpenFile = New System.Windows.Forms.Button
        Me.btnPrev = New System.Windows.Forms.Button
        Me.btnNext = New System.Windows.Forms.Button
        Me.btnX = New System.Windows.Forms.Button
        Me.txtConsole = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblControl = New System.Windows.Forms.Label
        Me.btnAutomated = New System.Windows.Forms.Button
        Me.txtGoTo = New System.Windows.Forms.TextBox
        CType(Me.dbData, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picResult, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtDConHeight
        '
        Me.txtDConHeight.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.txtDConHeight.Location = New System.Drawing.Point(282, 8)
        Me.txtDConHeight.Name = "txtDConHeight"
        Me.txtDConHeight.Size = New System.Drawing.Size(74, 20)
        Me.txtDConHeight.TabIndex = 7
        Me.txtDConHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtDConWidth
        '
        Me.txtDConWidth.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.txtDConWidth.Location = New System.Drawing.Point(207, 8)
        Me.txtDConWidth.Name = "txtDConWidth"
        Me.txtDConWidth.Size = New System.Drawing.Size(74, 20)
        Me.txtDConWidth.TabIndex = 6
        Me.txtDConWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtDConDepth
        '
        Me.txtDConDepth.BackColor = System.Drawing.Color.FromArgb(CType(CType(128, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.txtDConDepth.Location = New System.Drawing.Point(132, 8)
        Me.txtDConDepth.Name = "txtDConDepth"
        Me.txtDConDepth.Size = New System.Drawing.Size(74, 20)
        Me.txtDConDepth.TabIndex = 5
        Me.txtDConDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 11)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(121, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Dimensi Kontainer (unit):"
        '
        'dbData
        '
        Me.dbData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dbData.Location = New System.Drawing.Point(15, 44)
        Me.dbData.Name = "dbData"
        Me.dbData.Size = New System.Drawing.Size(490, 407)
        Me.dbData.TabIndex = 8
        '
        'btnExecute
        '
        Me.btnExecute.Location = New System.Drawing.Point(525, 431)
        Me.btnExecute.Name = "btnExecute"
        Me.btnExecute.Size = New System.Drawing.Size(148, 39)
        Me.btnExecute.TabIndex = 9
        Me.btnExecute.Text = "Execute"
        Me.btnExecute.UseVisualStyleBackColor = True
        '
        'picResult
        '
        Me.picResult.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.picResult.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.picResult.Location = New System.Drawing.Point(525, 8)
        Me.picResult.Name = "picResult"
        Me.picResult.Size = New System.Drawing.Size(420, 407)
        Me.picResult.TabIndex = 10
        Me.picResult.TabStop = False
        '
        'btnY
        '
        Me.btnY.Location = New System.Drawing.Point(906, 431)
        Me.btnY.Name = "btnY"
        Me.btnY.Size = New System.Drawing.Size(49, 20)
        Me.btnY.TabIndex = 11
        Me.btnY.Text = "rotateY"
        Me.btnY.UseVisualStyleBackColor = True
        '
        'btnOpenFile
        '
        Me.btnOpenFile.Location = New System.Drawing.Point(525, 484)
        Me.btnOpenFile.Name = "btnOpenFile"
        Me.btnOpenFile.Size = New System.Drawing.Size(87, 30)
        Me.btnOpenFile.TabIndex = 13
        Me.btnOpenFile.Text = "Open File"
        Me.btnOpenFile.UseVisualStyleBackColor = True
        '
        'btnPrev
        '
        Me.btnPrev.Enabled = False
        Me.btnPrev.Location = New System.Drawing.Point(628, 484)
        Me.btnPrev.Name = "btnPrev"
        Me.btnPrev.Size = New System.Drawing.Size(63, 30)
        Me.btnPrev.TabIndex = 14
        Me.btnPrev.Text = "<<<"
        Me.btnPrev.UseVisualStyleBackColor = True
        '
        'btnNext
        '
        Me.btnNext.Enabled = False
        Me.btnNext.Location = New System.Drawing.Point(792, 484)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(65, 30)
        Me.btnNext.TabIndex = 15
        Me.btnNext.Text = ">>>"
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'btnX
        '
        Me.btnX.Location = New System.Drawing.Point(843, 431)
        Me.btnX.Name = "btnX"
        Me.btnX.Size = New System.Drawing.Size(52, 20)
        Me.btnX.TabIndex = 16
        Me.btnX.Text = "rotateX"
        Me.btnX.UseVisualStyleBackColor = True
        '
        'txtConsole
        '
        Me.txtConsole.Location = New System.Drawing.Point(15, 470)
        Me.txtConsole.Name = "txtConsole"
        Me.txtConsole.Size = New System.Drawing.Size(490, 20)
        Me.txtConsole.TabIndex = 18
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.Label2.Location = New System.Drawing.Point(362, 8)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(82, 30)
        Me.Label2.TabIndex = 19
        Me.Label2.Text = "Mulai bener!!!" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "yeahhh...."
        '
        'lblControl
        '
        Me.lblControl.AutoSize = True
        Me.lblControl.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblControl.Location = New System.Drawing.Point(711, 491)
        Me.lblControl.Name = "lblControl"
        Me.lblControl.Size = New System.Drawing.Size(27, 15)
        Me.lblControl.TabIndex = 20
        Me.lblControl.Text = "0/0"
        '
        'btnAutomated
        '
        Me.btnAutomated.Location = New System.Drawing.Point(679, 431)
        Me.btnAutomated.Name = "btnAutomated"
        Me.btnAutomated.Size = New System.Drawing.Size(148, 39)
        Me.btnAutomated.TabIndex = 21
        Me.btnAutomated.Text = "Automated It!"
        Me.btnAutomated.UseVisualStyleBackColor = True
        '
        'txtGoTo
        '
        Me.txtGoTo.Location = New System.Drawing.Point(876, 491)
        Me.txtGoTo.Name = "txtGoTo"
        Me.txtGoTo.Size = New System.Drawing.Size(50, 20)
        Me.txtGoTo.TabIndex = 22
        '
        'MainMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(967, 528)
        Me.Controls.Add(Me.txtGoTo)
        Me.Controls.Add(Me.btnAutomated)
        Me.Controls.Add(Me.lblControl)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtConsole)
        Me.Controls.Add(Me.btnX)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnPrev)
        Me.Controls.Add(Me.btnOpenFile)
        Me.Controls.Add(Me.btnY)
        Me.Controls.Add(Me.picResult)
        Me.Controls.Add(Me.btnExecute)
        Me.Controls.Add(Me.dbData)
        Me.Controls.Add(Me.txtDConHeight)
        Me.Controls.Add(Me.txtDConWidth)
        Me.Controls.Add(Me.txtDConDepth)
        Me.Controls.Add(Me.Label1)
        Me.Name = "MainMenu"
        Me.Text = "CLP-AdaptiveSolution: Main Menu"
        CType(Me.dbData, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picResult, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtDConHeight As System.Windows.Forms.TextBox
    Friend WithEvents txtDConWidth As System.Windows.Forms.TextBox
    Friend WithEvents txtDConDepth As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents dbData As System.Windows.Forms.DataGridView
    Friend WithEvents btnExecute As System.Windows.Forms.Button
    Friend WithEvents picResult As System.Windows.Forms.PictureBox
    Friend WithEvents btnY As System.Windows.Forms.Button
    Friend WithEvents btnOpenFile As System.Windows.Forms.Button
    Friend WithEvents btnPrev As System.Windows.Forms.Button
    Friend WithEvents btnNext As System.Windows.Forms.Button
    Friend WithEvents btnX As System.Windows.Forms.Button
    Friend WithEvents txtConsole As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblControl As System.Windows.Forms.Label
    Friend WithEvents btnAutomated As System.Windows.Forms.Button
    Friend WithEvents txtGoTo As System.Windows.Forms.TextBox
End Class
