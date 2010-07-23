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
''' - build the stacking depend on selection of based box.
''' - based box for each box can be determined by the largest box.
''' - it's possible to placement box by fitness value.
''' - fitness of stack it's depends on utilization.
''' </remarks>


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


Public Class Stack
    Inherits Placement

    Public Structure Ranking
        Dim SType As Integer
        Dim SWidth, SDepth, SHeight As Single
        Dim SFitness As Double
        Dim SSide As String
        Dim SFeasible As Boolean
    End Structure

    ''' <summary>
    ''' Constructor the class, input the data
    ''' </summary>
    Sub New(ByVal DataEmptySpace As Box, ByVal InputBox() As Box)
        'input data
        FInput = InputBox
        FEmptySpace = DataEmptySpace

        'recapitulation data
        recapitulation(FInput, FDataListInput)

        '--harusnya bis ini, data tersebut diolah.. entah bagaimana caranya
        'list must to do
        '1. rekap data dulu --anggap rekap ud selese dilakukan
        '2. cari stack mana yang paling besar --buat program ranking

    End Sub

    ''' <summary>
    ''' Get ranking of stacking --individual mode
    ''' </summary>
    Private Sub GetRankingIndividual(ByVal list() As ListBox, ByVal FRanking() As Ranking)
        Dim i, j, count As Integer
        Dim tBox As Box = Nothing                   'reset data first

        ReDim FRanking(list.GetUpperBound(0) * 3)
        count = 0
        For i = 1 To list.GetUpperBound(0)
            'find box, to simulated it
            For j = 1 To FInput.GetUpperBound(0)
                If FInput(j).Type = list(i).SType Then
                    tBox = DeepClone(FInput(j))
                    'clone object.... hope works well
                    Exit For
                End If
            Next

            For j = 1 To 3
                'set the list
                With FRanking(count)
                    Select Case j
                        Case 1
                            tBox.Alpha = True
                            .SSide = "Alpha"
                        Case 2
                            tBox.Beta = True
                            .SSide = "Beta"
                        Case 3
                            tBox.Gamma = True
                            .SSide = "Gamma"
                    End Select

                    .SDepth = tBox.Depth
                    .SWidth = tBox.Width
                    .SHeight = tBox.Height
                    .SType = tBox.Type
                    .SFeasible = False              'set feasible = false (default value)
                End With
            Next
        Next

    End Sub

    ''' <summary>
    ''' Get ranking of stacking --cuboid mode
    ''' </summary>
    Private Sub GetRankingCuboid()

    End Sub

    ''' <summary>
    ''' Get ranking of stacking --individual, cuboid
    ''' </summary>
    Private Sub GetRankingTotal()

    End Sub

    Public Sub GetOptimizeStack()

        'ambil data terbaru
        'looping sampe


    End Sub

    ''' <summary>
    ''' Stacking, this recursive procedure to make it!
    ''' </summary>
    Public Sub GetStack(ByVal posArea As Point3D)
        'input --> area zone, tolerate zone
        'output --> box and position
        '---build boolean to cek whether box has been located or not..

        'apa aj sih langkah buat rekursif ini.. pusink gw..
        'yang pasti harusnya ud jadi list buat dimasukkin..
        'kalo database ud lengkap, tinggal dipake
        'sekarang buat databse lebih dulu aj.




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