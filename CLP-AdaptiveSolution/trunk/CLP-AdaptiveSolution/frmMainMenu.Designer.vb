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
        Me.lblInfo1 = New System.Windows.Forms.Label
        Me.dbData = New System.Windows.Forms.DataGridView
        Me.btnExecute = New System.Windows.Forms.Button
        Me.picResult = New System.Windows.Forms.PictureBox
        Me.btnOpenFile = New System.Windows.Forms.Button
        Me.btnPrev = New System.Windows.Forms.Button
        Me.btnNext = New System.Windows.Forms.Button
        Me.txtConsole = New System.Windows.Forms.TextBox
        Me.lblValidation = New System.Windows.Forms.Label
        Me.lblControl = New System.Windows.Forms.Label
        Me.btnAutomated = New System.Windows.Forms.Button
        Me.txtGoTo = New System.Windows.Forms.TextBox
        Me.btnExamine = New System.Windows.Forms.Button
        Me.chkCuboid = New System.Windows.Forms.CheckBox
        Me.chkWall = New System.Windows.Forms.CheckBox
        Me.chkStack = New System.Windows.Forms.CheckBox
        Me.trckWall = New System.Windows.Forms.TrackBar
        Me.trckStack = New System.Windows.Forms.TrackBar
        Me.listConsole = New System.Windows.Forms.TextBox
        Me.lblInfo2 = New System.Windows.Forms.Label
        Me.s = New System.Windows.Forms.Label
        CType(Me.dbData, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picResult, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trckWall, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trckStack, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'lblInfo1
        '
        Me.lblInfo1.AutoSize = True
        Me.lblInfo1.Location = New System.Drawing.Point(12, 11)
        Me.lblInfo1.Name = "lblInfo1"
        Me.lblInfo1.Size = New System.Drawing.Size(121, 13)
        Me.lblInfo1.TabIndex = 4
        Me.lblInfo1.Text = "Dimensi Kontainer (unit):"
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
        Me.btnExecute.Location = New System.Drawing.Point(529, 435)
        Me.btnExecute.Name = "btnExecute"
        Me.btnExecute.Size = New System.Drawing.Size(127, 39)
        Me.btnExecute.TabIndex = 9
        Me.btnExecute.Text = "Execute"
        Me.btnExecute.UseVisualStyleBackColor = True
        '
        'picResult
        '
        Me.picResult.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.picResult.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.picResult.Location = New System.Drawing.Point(519, 15)
        Me.picResult.Name = "picResult"
        Me.picResult.Size = New System.Drawing.Size(420, 407)
        Me.picResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.picResult.TabIndex = 10
        Me.picResult.TabStop = False
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
        'txtConsole
        '
        Me.txtConsole.Location = New System.Drawing.Point(15, 470)
        Me.txtConsole.Name = "txtConsole"
        Me.txtConsole.Size = New System.Drawing.Size(490, 20)
        Me.txtConsole.TabIndex = 18
        '
        'lblValidation
        '
        Me.lblValidation.AutoEllipsis = True
        Me.lblValidation.AutoSize = True
        Me.lblValidation.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lblValidation.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblValidation.ForeColor = System.Drawing.SystemColors.GradientActiveCaption
        Me.lblValidation.Location = New System.Drawing.Point(362, 8)
        Me.lblValidation.Name = "lblValidation"
        Me.lblValidation.Size = New System.Drawing.Size(82, 30)
        Me.lblValidation.TabIndex = 19
        Me.lblValidation.Text = "Mulai bener!!!" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "yeahhh...."
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
        Me.btnAutomated.Location = New System.Drawing.Point(667, 435)
        Me.btnAutomated.Name = "btnAutomated"
        Me.btnAutomated.Size = New System.Drawing.Size(127, 39)
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
        'btnExamine
        '
        Me.btnExamine.Location = New System.Drawing.Point(807, 435)
        Me.btnExamine.Name = "btnExamine"
        Me.btnExamine.Size = New System.Drawing.Size(127, 39)
        Me.btnExamine.TabIndex = 24
        Me.btnExamine.Text = "Examine It!"
        Me.btnExamine.UseVisualStyleBackColor = True
        '
        'chkCuboid
        '
        Me.chkCuboid.AutoSize = True
        Me.chkCuboid.Location = New System.Drawing.Point(960, 434)
        Me.chkCuboid.Name = "chkCuboid"
        Me.chkCuboid.Size = New System.Drawing.Size(97, 17)
        Me.chkCuboid.TabIndex = 25
        Me.chkCuboid.Text = "Cuboid method"
        Me.chkCuboid.UseVisualStyleBackColor = True
        '
        'chkWall
        '
        Me.chkWall.AutoSize = True
        Me.chkWall.Location = New System.Drawing.Point(960, 464)
        Me.chkWall.Name = "chkWall"
        Me.chkWall.Size = New System.Drawing.Size(85, 17)
        Me.chkWall.TabIndex = 26
        Me.chkWall.Text = "Wall method"
        Me.chkWall.UseVisualStyleBackColor = True
        '
        'chkStack
        '
        Me.chkStack.AutoSize = True
        Me.chkStack.Location = New System.Drawing.Point(959, 493)
        Me.chkStack.Name = "chkStack"
        Me.chkStack.Size = New System.Drawing.Size(92, 17)
        Me.chkStack.TabIndex = 27
        Me.chkStack.Text = "Stack method"
        Me.chkStack.UseVisualStyleBackColor = True
        '
        'trckWall
        '
        Me.trckWall.Location = New System.Drawing.Point(1099, 443)
        Me.trckWall.Maximum = 100
        Me.trckWall.Name = "trckWall"
        Me.trckWall.Size = New System.Drawing.Size(113, 40)
        Me.trckWall.TabIndex = 28
        Me.trckWall.Value = 5
        '
        'trckStack
        '
        Me.trckStack.Location = New System.Drawing.Point(1099, 486)
        Me.trckStack.Maximum = 100
        Me.trckStack.Name = "trckStack"
        Me.trckStack.Size = New System.Drawing.Size(113, 40)
        Me.trckStack.TabIndex = 29
        Me.trckStack.Value = 1
        '
        'listConsole
        '
        Me.listConsole.AcceptsReturn = True
        Me.listConsole.AcceptsTab = True
        Me.listConsole.AllowDrop = True
        Me.listConsole.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.listConsole.Location = New System.Drawing.Point(947, 15)
        Me.listConsole.MaxLength = 32767000
        Me.listConsole.Multiline = True
        Me.listConsole.Name = "listConsole"
        Me.listConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.listConsole.Size = New System.Drawing.Size(289, 407)
        Me.listConsole.TabIndex = 30
        Me.listConsole.WordWrap = False
        '
        'lblInfo2
        '
        Me.lblInfo2.AutoSize = True
        Me.lblInfo2.Location = New System.Drawing.Point(1133, 429)
        Me.lblInfo2.Name = "lblInfo2"
        Me.lblInfo2.Size = New System.Drawing.Size(79, 13)
        Me.lblInfo2.TabIndex = 31
        Me.lblInfo2.Text = "Wall Parameter"
        '
        's
        '
        Me.s.AutoSize = True
        Me.s.Location = New System.Drawing.Point(1133, 473)
        Me.s.Name = "s"
        Me.s.Size = New System.Drawing.Size(86, 13)
        Me.s.TabIndex = 32
        Me.s.Text = "Stack Parameter"
        '
        'MainMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.AutoSize = True
        Me.ClientSize = New System.Drawing.Size(1239, 528)
        Me.Controls.Add(Me.s)
        Me.Controls.Add(Me.lblInfo2)
        Me.Controls.Add(Me.listConsole)
        Me.Controls.Add(Me.trckStack)
        Me.Controls.Add(Me.trckWall)
        Me.Controls.Add(Me.chkStack)
        Me.Controls.Add(Me.chkWall)
        Me.Controls.Add(Me.chkCuboid)
        Me.Controls.Add(Me.btnExamine)
        Me.Controls.Add(Me.txtGoTo)
        Me.Controls.Add(Me.btnAutomated)
        Me.Controls.Add(Me.lblControl)
        Me.Controls.Add(Me.lblValidation)
        Me.Controls.Add(Me.txtConsole)
        Me.Controls.Add(Me.btnNext)
        Me.Controls.Add(Me.btnPrev)
        Me.Controls.Add(Me.btnOpenFile)
        Me.Controls.Add(Me.picResult)
        Me.Controls.Add(Me.btnExecute)
        Me.Controls.Add(Me.dbData)
        Me.Controls.Add(Me.txtDConHeight)
        Me.Controls.Add(Me.txtDConWidth)
        Me.Controls.Add(Me.txtDConDepth)
        Me.Controls.Add(Me.lblInfo1)
        Me.Name = "MainMenu"
        Me.Text = "CLP-AdaptiveSolution: Main Menu"
        CType(Me.dbData, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picResult, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trckWall, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trckStack, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtDConHeight As System.Windows.Forms.TextBox
    Friend WithEvents txtDConWidth As System.Windows.Forms.TextBox
    Friend WithEvents txtDConDepth As System.Windows.Forms.TextBox
    Friend WithEvents lblInfo1 As System.Windows.Forms.Label
    Friend WithEvents dbData As System.Windows.Forms.DataGridView
    Friend WithEvents btnExecute As System.Windows.Forms.Button
    Friend WithEvents picResult As System.Windows.Forms.PictureBox
    Friend WithEvents btnOpenFile As System.Windows.Forms.Button
    Friend WithEvents btnPrev As System.Windows.Forms.Button
    Friend WithEvents btnNext As System.Windows.Forms.Button
    Friend WithEvents txtConsole As System.Windows.Forms.TextBox
    Friend WithEvents lblValidation As System.Windows.Forms.Label
    Friend WithEvents lblControl As System.Windows.Forms.Label
    Friend WithEvents btnAutomated As System.Windows.Forms.Button
    Friend WithEvents txtGoTo As System.Windows.Forms.TextBox
    Friend WithEvents btnExamine As System.Windows.Forms.Button
    Friend WithEvents chkCuboid As System.Windows.Forms.CheckBox
    Friend WithEvents chkWall As System.Windows.Forms.CheckBox
    Friend WithEvents chkStack As System.Windows.Forms.CheckBox
    Friend WithEvents trckWall As System.Windows.Forms.TrackBar
    Friend WithEvents trckStack As System.Windows.Forms.TrackBar
    Friend WithEvents listConsole As System.Windows.Forms.TextBox
    Friend WithEvents lblInfo2 As System.Windows.Forms.Label
    Friend WithEvents s As System.Windows.Forms.Label
End Class
