Imports System
Imports System.Object

''' <summary>
''' Stacking
''' </summary>
''' <remarks>
''' Input: Databox (array), Emptyspace
''' Output: Coordinate of each box in emptyspace, it can be multiple stack
''' 
''' Process:
''' - 1 stack = 1 layer
''' - 1 layer = filling by tallest or most dimension...
''' - placement each layer = build cuboid 1 depth...
''' - iteration until there's no layer that can be added...
''' </remarks>

Public Class Stack
    Inherits Placement

    ''' <summary>
    ''' Constructor the class, input the data
    ''' </summary>
    Sub New(ByVal DEmpty As Box, ByVal InputBox() As Box)
        Dim i As Integer

        'input data --empty space, input box
        FEmptySpace = New Box(DEmpty)

        ReDim FInput(InputBox.GetUpperBound(0))
        For i = 1 To InputBox.GetUpperBound(0)
            FInput(i) = New Box(InputBox(i))
        Next

        'recapitulation box
        Recapitulation(FInput, FDataListInput)
    End Sub

    ''' <summary>
    ''' Optimizing stacking
    ''' </summary>
    Public Sub GetOptimizeStack()
        'it's better working with tempBox
        '1. looping until (nobox = true) or (noemptyspace = true)
        '   --each looping
        '   2. find ranking dimension for box
        '   3. let layer class handling to filling box
        '   4. update filled box, update rest box
        '   5. generate empty-space + update empty-space

        '#variable
        Dim i, j, k As Integer

        '#preparing step
        'copy emptyspace to list + fdatalistoutput
        Dim emptySpaceList(1) As Box
        emptySpaceList(1) = New Box(-1, FEmptySpace.Width, FEmptySpace.Height, FEmptySpace.Depth, CByte(1))

        Recapitulation(FInput, FDataListOutput)
        For i = 1 To FDataListOutput.GetUpperBound(0)
            FDataListOutput(i).SCount = 0
        Next

        'prepare cek as boolean for FInput controller
        'default cek = false --> box(i) hasn't placed yet
        Dim cek(FInput.GetUpperBound(0)) As Boolean

        Do Until (emptySpaceList.GetUpperBound(0) = 0) Or (cek(0) = True)



            '#update cek(0)
            cek(0) = True
            For i = 1 To cek.GetUpperBound(0)
                If cek(i) = False Then
                    cek(0) = False
                    Exit For
                End If
            Next

            '#update emptySpaceList
            'cek(0) = false --> there's still others box
            'check 1-by-1 fisibility emptySpace to fill at empty-Space
            If cek(0) = False Then
                For i = 1 To emptySpaceList.GetUpperBound(0)
                    For j = 1 To FDataListInput.GetUpperBound(0)
                        'checking possibilities of emptyspace only if box.count > 0
                        If FDataListOutput(j).SCount > 0 Then
                            For k = 1 To FInput.GetUpperBound(0)
                                If (FDataListInput(j).SType = FInput(k).Type) Then

                                End If
                            Next
                        End If


                    Next
                Next

            End If
        Loop

    End Sub

    ''' <summary>
    ''' Stacking, this recursive procedure to make it!
    ''' </summary>
    Public Sub GetStack()


    End Sub



End Class


' jangan lupa, ntar bikin langsung prosedur rekursif..
' ga susah koq, pokoknya limit rekursif selese saat ga ada lagi yang bisa ditumpuk karn ketinggian dan luas tumpukan melebihi tolerasnsi
' selain itu, variabel bisa diakses langsung  biar mudah...
' pada intinya nanti ada back tracking untuk ngumpulin hasilnya...
' paling enak ud disiapin, ranking dan urutan area zone untuk ditempati.. apakah oke ao ga...
' trus pas masang perlu ditempatin juga, penempatan tiap balok di area xone yang paling efektif, dan efisien
' apalagi ya... gw sebenernya juga masi bingung, mo coding tapi gw ud ngantuk..
' jangan lupa ke kedutaan, buat dapet visa...




'1.cek limit height ada ato ga... kl ada, berarti bisa masuk, kl ga, berarti ga masuk.
'2.cek fisibilitas ranking, ada yang fisibel ato ga... kl ada yang fisibel berarti lakukan packing
'3.proses packing dilakukan dengan mengambil box dari ranking yang paling besar terdahulu, kemudian satu-per-satu dimasukkan
'
' - yang masi membingungkan adalah bagaimana caranya supaya proses packing tidak rekursif ke belakang, tampaknya harus dibuat mekanisme control untuk mengatur prioritas antar prosedur
'
' 
'pengennya prosesnya kira2 seperti ini
'
'#cek apakah posisi sekarang masi ada balok yang ketinggiannya mencukupi (dibawah) ketinggian batas
'#bila cek=true
'   #re-fitness balok dengan kondisi area yang ada, cek fisibilitas dari masing2 balok
'   #sorting balok menurut fitness
'   #iterasi sampai balok tidak ada yang bisa masuk lagi
'       #packing tiap balok berdasarkan aturan (gunakan nilai packing fitness terbesar)
'   #generate area zone, berdasarkan prinsip maximal space --> mungkin ini yang okeh
'   #list area zone sehingga bisa digunakan
'       #cek sisi mana saja yang boleh tertoleransi, sisi mana yang tidak boleh tertoleransi
'   #rekapitulasi lagi, balok yang telah terpakai dan blon terpakai
'   #rekapitulasi urutan prioritas area zone yang akan dipacking
'   #iterasi panggilan prosedur untuk area zone yang telah diprioritaskan, mulai dari prioritas paling besar
'       #prioritas paling besar: paling rendah ketinggiannya, paling besar ukuran
'       #lanjutkan memanggil urutan yang lain
