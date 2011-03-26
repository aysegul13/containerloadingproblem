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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ExamineMenu))
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
        Me.dbDataBox.AllowUserToAddRows = False
        Me.dbDataBox.AllowUserToDeleteRows = False
        Me.dbDataBox.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dbDataBox, "dbDataBox")
        Me.dbDataBox.Name = "dbDataBox"
        Me.dbDataBox.ReadOnly = True
        '
        'picResult
        '
        resources.ApplyResources(Me.picResult, "picResult")
        Me.picResult.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.picResult.Name = "picResult"
        Me.picResult.TabStop = False
        '
        'txtConsole
        '
        resources.ApplyResources(Me.txtConsole, "txtConsole")
        Me.txtConsole.Name = "txtConsole"
        '
        'dbDataSpace
        '
        Me.dbDataSpace.AllowUserToAddRows = False
        Me.dbDataSpace.AllowUserToDeleteRows = False
        Me.dbDataSpace.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        resources.ApplyResources(Me.dbDataSpace, "dbDataSpace")
        Me.dbDataSpace.Name = "dbDataSpace"
        Me.dbDataSpace.ReadOnly = True
        '
        'chkBox
        '
        resources.ApplyResources(Me.chkBox, "chkBox")
        Me.chkBox.Name = "chkBox"
        Me.chkBox.UseVisualStyleBackColor = True
        '
        'chkSpace
        '
        resources.ApplyResources(Me.chkSpace, "chkSpace")
        Me.chkSpace.Name = "chkSpace"
        Me.chkSpace.UseVisualStyleBackColor = True
        '
        'btnExamine
        '
        Me.btnExamine.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.btnExamine, "btnExamine")
        Me.btnExamine.Name = "btnExamine"
        Me.btnExamine.UseVisualStyleBackColor = True
        '
        'ExamineMenu
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        resources.ApplyResources(Me, "$this")
        Me.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange
        Me.CancelButton = Me.btnExamine
        Me.Controls.Add(Me.btnExamine)
        Me.Controls.Add(Me.chkSpace)
        Me.Controls.Add(Me.chkBox)
        Me.Controls.Add(Me.dbDataSpace)
        Me.Controls.Add(Me.txtConsole)
        Me.Controls.Add(Me.picResult)
        Me.Controls.Add(Me.dbDataBox)
        Me.Name = "ExamineMenu"
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
