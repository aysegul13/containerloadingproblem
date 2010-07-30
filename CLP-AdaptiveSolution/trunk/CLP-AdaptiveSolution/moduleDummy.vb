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
    Dim dataBox(Nothing) As Box
    Dim listBox(Nothing) As ListBox

    Public Sub algDummyCuboid(ByVal dataBoxParam() As Box, ByVal listBoxParam() As ListBox, ByRef dummy() As Box)
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
        dummy(dummy.GetUpperBound(0)) = New Box(-1, cuboidcontoh.BoundingCuboid.Depth, cuboidcontoh.BoundingCuboid.Width, cuboidcontoh.BoundingCuboid.Height, cuboidcontoh.BoundingCuboid.Orientation)
        'set new
        'For i = 1 To cuboidcontoh.MaxBox
        For i = 1 To cuboidcontoh.UsedBox
            dummy(i) = New Box(1, cuboidcontoh.Box.Depth, cuboidcontoh.Box.Width, cuboidcontoh.Box.Height)
            dummy(i).Orientation = cuboidcontoh.Box.Orientation
            dummy(i).LocationContainer = New Point3D(cuboidcontoh.PositionBoxInCuboid(i).X, cuboidcontoh.PositionBoxInCuboid(i).Y, cuboidcontoh.PositionBoxInCuboid(i).Z)
        Next

        'MyForm.formMainMenu.txtConsole.Text = cuboidcontoh.MethodStatus & " , " & cuboidcontoh.Score

    End Sub


    Public Sub algDummyStack(ByVal dataBoxParam() As Box, ByVal listBoxParam() As ListBox, ByRef dummy() As Box)
        'databox param = data input box
        'sortbox param = data input box that has been sorted
        'dummy = solution box

        'build a stacking --it is ONE!! not more...
        Dim stackingcontoh As New Stack(dataBoxParam(0), dataBoxParam)


    End Sub

    Public Sub algDummyWall()
        algInputText(dataBox, listBox)

        Dim packingWall As New Wall(dataBox(0), dataBox)
        packingWall.GetOptimizeWall()
    End Sub


    Public Sub algDummyLayer(ByVal dataBoxParam() As Box, ByVal listBoxParam() As ListBox, ByRef dummy() As Box)
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
            .LocationContainer.X = 0
            .LocationContainer.Y = 0
            .LocationContainer.Z = 0
            .Beta = True
            .Update()
        End With

        With dummy(2)
            .LocationContainer.X = 0
            .LocationContainer.Y = 0
            .LocationContainer.Z = dummy(1).LocationContainer2.Z
            .Beta = True
            .Update()
        End With

        With dummy(3)
            .LocationContainer = New Point3D(dummy(1).LocationContainer2.X, 0, 0)
            .Beta = True
        End With

        With dummy(4)
            .LocationContainer = New Point3D(dummy(1).LocationContainer2.X, 0, dummy(1).LocationContainer2.Z)
            .Beta = True
        End With
    End Sub

End Module
