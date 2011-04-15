Imports System.Object
Imports System.IO                               'reading file
Imports Excel = Microsoft.Office.Interop.Excel  'excel controller
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Windows.Forms


''' <summary>
''' Module for testing purpose only
''' </summary>
Public Module Dummy
    Dim DumDataBox(Nothing) As Box
    Dim DumListBox(Nothing) As strBoxList

    Public Sub algDummyCuboid(ByVal dataBoxParam() As Box, ByVal listBoxParam() As strBoxList, ByRef dummy() As Box)
        'ISIAN
        Dim tempOrien As Boolean
        Dim tempSide As Byte
        tempSide = 2
        tempOrien = True

        'build cuboidcontoh
        Dim cuboidcontoh As New Cuboid(dataBoxParam(0), dataBoxParam(1), Val(MyForm.formMainMenu.txtConsole.Text))
        'cuboidcontoh.SetSideOrientation(tempSide, tempOrien)
        'cuboidcontoh.Construct1AxisCuboid(cuboidcontoh.FreeBox, "D", New Point3D(0, 0, 0), True)
        'cuboidcontoh.Construct2AxisCuboid(cuboidcontoh.FreeBox, "D", "W", New Point3D(0, 0, 0), True)
        'cuboidcontoh.ConstructCuboid(cuboidcontoh.FreeBox, 3, "D", "W", "H", New Point3D(0, 0, 0), False)

        cuboidcontoh.GetOptimizeCuboid(True)
        'cuboidcontoh.GetMaxScoreCuboid(False)

        ReDim dummy(cuboidcontoh.UsedBox + 1)
        'ReDim dummy(cuboidcontoh.MaxBox + 1)

        'set container & bounding box
        dummy(0) = dataBoxParam(0)
        dummy(dummy.GetUpperBound(0)) = New Box(-1, cuboidcontoh.BoundingBox.Depth, cuboidcontoh.BoundingBox.Width, cuboidcontoh.BoundingBox.Height)
        'set new
        'For i = 1 To cuboidcontoh.MaxBox
        For i = 1 To cuboidcontoh.UsedBox
            '//Insert Dummy Cuboid --> set all posibilities rotation
            dummy(i) = New Box(1, cuboidcontoh.Box.Depth, cuboidcontoh.Box.Width, cuboidcontoh.Box.Height, True, True, True)
            dummy(i).Orientation = cuboidcontoh.Box.Orientation
            dummy(i).AbsPos1 = New Point3D(cuboidcontoh.PositionBoxInCuboid(i).X, cuboidcontoh.PositionBoxInCuboid(i).Y, cuboidcontoh.PositionBoxInCuboid(i).Z)
        Next

        'MyForm.formMainMenu.txtConsole.Text = cuboidcontoh.MethodStatus & " , " & cuboidcontoh.Score

    End Sub


    Public Sub algDummyStack(ByVal dataBoxParam() As Box, ByVal listBoxParam() As strBoxList, ByRef dummy() As Box)
        'databox param = data input box
        'sortbox param = data input box that has been sorted
        'dummy = solution box

        'build a stacking --it is ONE!! not more...
        Dim stackingcontoh As New Stack(dataBoxParam(0), _
                                        dataBoxParam, _
                                        (MyForm.formMainMenu.trckStack.Value / MyForm.formMainMenu.trckStack.Maximum), _
                                        MyForm.formMainMenu.limiter)
    End Sub

    Public Sub algDummyWall()
        algInputText(DumDataBox, DumListBox)

        Dim packingWall As New Wall(DumDataBox(0), _
                                    DumDataBox, _
                                    (MyForm.formMainMenu.trckWall.Value / MyForm.formMainMenu.trckWall.Maximum))
        packingWall.GetOptimizeWall()
    End Sub


    Public Sub algDummyLayer(ByVal dataBoxParam() As Box, ByVal listBoxParam() As strBoxList, ByRef dummy() As Box)
        'databox param = data input box
        'sortbox param = data input box that has been sorted
        'dummy = solution box

        'build a stacking --it is ONE!! not more...
        Dim layercontoh As New Layer(dataBoxParam(0), dataBoxParam)
        layercontoh.GetOptimizeLayer()

    End Sub


    Public Sub algDummySolution(ByVal dataBoxParam() As Box, ByVal sortBoxParam(,) As Integer, ByRef dummy() As Box)
        ReDim dummy(4)

        dummy(0) = dataBoxParam(0)
        dummy(1) = dataBoxParam(1)
        dummy(2) = dataBoxParam(2)
        dummy(3) = dataBoxParam(3)
        dummy(4) = dataBoxParam(4)

        'set dummy solution in container
        With dummy(1)
            .AbsPos1.X = 0
            .AbsPos1.Y = 0
            .AbsPos1.Z = 0
            .IsBeta = True
            .UpdateAll()
        End With

        With dummy(2)
            .AbsPos1.X = 0
            .AbsPos1.Y = 0
            .AbsPos1.Z = dummy(1).AbsPos2.Z
            .IsBeta = True
            .UpdateAll()
        End With

        With dummy(3)
            .AbsPos1 = New Point3D(dummy(1).AbsPos2.X, 0, 0)
            .IsBeta = True
        End With

        With dummy(4)
            .AbsPos1 = New Point3D(dummy(1).AbsPos2.X, 0, dummy(1).AbsPos2.Z)
            .IsBeta = True
        End With
    End Sub

End Module
