<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ExamineMenu
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
        Me.dbDataBox = New System.Windows.Forms.DataGridView
        Me.picResult = New System.Windows.Forms.PictureBox
        Me.txtConsole = New System.Windows.Forms.TextBox
        Me.dbDataSpace = New System.Windows.Forms.DataGridView
        Me.chkBox = New System.Windows.Forms.CheckBox
        Me.chkSpace = New System.Windows.Forms.CheckBox
        Me.btnExamine = New System.Windows.Forms.Button
        CType(Me.dbDataBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picResult, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dbDataSpace, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dbDataBox
        '
        Me.dbDataBox.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dbDataBox.Location = New System.Drawing.Point(8, 12)
        Me.dbDataBox.Name = "dbDataBox"
        Me.dbDataBox.Size = New System.Drawing.Size(490, 528)
        Me.dbDataBox.TabIndex = 9
        '
        'picResult
        '
        Me.picResult.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.picResult.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.picResult.Location = New System.Drawing.Point(508, 238)
        Me.picResult.Name = "picResult"
        Me.picResult.Size = New System.Drawing.Size(490, 430)
        Me.picResult.TabIndex = 11
        Me.picResult.TabStop = False
        '
        'txtConsole
        '
        Me.txtConsole.Location = New System.Drawing.Point(8, 546)
        Me.txtConsole.Name = "txtConsole"
        Me.txtConsole.Size = New System.Drawing.Size(490, 20)
        Me.txtConsole.TabIndex = 19
        '
        'dbDataSpace
        '
        Me.dbDataSpace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dbDataSpace.Location = New System.Drawing.Point(508, 13)
        Me.dbDataSpace.Name = "dbDataSpace"
        Me.dbDataSpace.Size = New System.Drawing.Size(490, 219)
        Me.dbDataSpace.TabIndex = 20
        '
        'chkBox
        '
        Me.chkBox.AutoSize = True
        Me.chkBox.Location = New System.Drawing.Point(22, 582)
        Me.chkBox.Name = "chkBox"
        Me.chkBox.Size = New System.Drawing.Size(102, 17)
        Me.chkBox.TabIndex = 21
        Me.chkBox.Text = "Check all Boxes"
        Me.chkBox.UseVisualStyleBackColor = True
        '
        'chkSpace
        '
        Me.chkSpace.AutoSize = True
        Me.chkSpace.Location = New System.Drawing.Point(22, 618)
        Me.chkSpace.Name = "chkSpace"
        Me.chkSpace.Size = New System.Drawing.Size(109, 17)
        Me.chkSpace.TabIndex = 22
        Me.chkSpace.Text = "Check all Spaces"
        Me.chkSpace.UseVisualStyleBackColor = True
        '
        'btnExamine
        '
        Me.btnExamine.Location = New System.Drawing.Point(277, 582)
        Me.btnExamine.Name = "btnExamine"
        Me.btnExamine.Size = New System.Drawing.Size(201, 53)
        Me.btnExamine.TabIndex = 23
        Me.btnExamine.Text = "Examine it!!"
        Me.btnExamine.UseVisualStyleBackColor = True
        '
        'ExamineMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1004, 673)
        Me.Controls.Add(Me.btnExamine)
        Me.Controls.Add(Me.chkSpace)
        Me.Controls.Add(Me.chkBox)
        Me.Controls.Add(Me.dbDataSpace)
        Me.Controls.Add(Me.txtConsole)
        Me.Controls.Add(Me.picResult)
        Me.Controls.Add(Me.dbDataBox)
        Me.Name = "ExamineMenu"
        Me.Text = "Examine it!!"
        CType(Me.dbDataBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picResult, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dbDataSpace, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents dbDataBox As System.Windows.Forms.DataGridView
    Friend WithEvents picResult As System.Windows.Forms.PictureBox
    Friend WithEvents txtConsole As System.Windows.Forms.TextBox
    Friend WithEvents dbDataSpace As System.Windows.Forms.DataGridView
    Friend WithEvents chkBox As System.Windows.Forms.CheckBox
    Friend WithEvents chkSpace As System.Windows.Forms.CheckBox
    Friend WithEvents btnExamine As System.Windows.Forms.Button
End Class
