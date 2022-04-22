Public Class LosyRelacji
    Public aDiffTyp(20) As Integer
    Public miAdd As Integer = 0

    Private Function WezOpis(iTyp As Integer) As String
        Select Case iTyp

            Case 0
                Return GetLangString("relacje0")
            Case 1
            Case 2
                Return GetLangString("relacje1")
            Case 3
            Case 4
                Return GetLangString("relacje3")
            Case 5
                Return GetLangString("relacje5")
            Case 6
            Case 7
                Return GetLangString("relacje6")
            Case 8
            Case 9
                Return GetLangString("relacje8")
            Case 10
            Case 11
                Return GetLangString("relacje10")
            Case 12
                Return GetLangString("relacje12")
            Case 13
            Case 14
                Return GetLangString("relacje13")
        End Select

        Return "???"

    End Function


    Private Function GetMaxTyp() As Integer
        ' sprawdzenie maksimum w tablicy roznic
        Dim iMaxCnt As Integer = 0
        Dim iMaxPtr As Integer = 0

        For i As Integer = 0 To 14
            If aDiffTyp(i) > iMaxCnt Then
                iMaxCnt = aDiffTyp(i)
                iMaxPtr = i
            End If
        Next

        Return iMaxPtr
    End Function

    ' tabelka przejsc pomiedzy typami
    ' aTypTyp(iTyp, iStep) = iNewTyp
    '                           {11,12,13,14,15, 22,23,24,25, 33,34,35, 44,45, 55}
    '                             0  1  2  3  4   5  6  7  8   9 10 11  12 13  14
    '                   DLA LAT  0,20,40,+60,+80
    Private aTypTyp(,) As Integer = {
                                    {0, 5, 9, 12, 14}, ' poczatkowy 11
                                    {1, 6, 10, 13, 14}, ' poczatkowy 12
                                    {2, 7, 11, 13, 14}, ' poczatkowy 13
                                    {3, 8, 11, 13, 14}, ' poczatkowy 14
                                    {4, 8, 11, 13, 14}, ' poczatkowy 15
                                    {5, 9, 12, 14, 14}, ' poczatkowy 22
                                    {6, 10, 13, 14, 14}, ' poczatkowy 23
                                    {7, 11, 13, 14, 14}, ' poczatkowy 24
                                    {8, 11, 13, 14, 14}, ' poczatkowy 25
                                    {9, 12, 14, 14, 14}, ' poczatkowy 33
                                    {10, 13, 14, 14, 14}, ' poczatkowy 34
                                    {11, 13, 14, 14, 14}, ' poczatkowy 35
                                    {12, 14, 14, 14, 14}, ' poczatkowy 44
                                    {13, 14, 14, 14, 14}, ' poczatkowy 45
                                    {14, 14, 14, 14, 14} ' poczatkowy 55
                                    }

    Public Function PokazRelacjePart1() As String
        Dim iStep As Integer = miAdd / 20
        Dim iTyp As Integer = GetMaxTyp()  ' maximum różnic, pointer = 0..14

        iTyp = aTypTyp(iTyp, iStep)

        Dim sTxt As String
        If iTyp = 1 Or iTyp = 3 Or iTyp = 6 Or iTyp = 8 Or iTyp = 10 Or iTyp = 13 Then
            sTxt = GetLangString("msgLosyBetween") & vbCrLf & vbCrLf &
                WezOpis(iTyp - 1) & vbCrLf & vbCrLf & GetLangString("msgLosyOraz") &
                vbCrLf & vbCrLf & WezOpis(iTyp + 1)
        Else
            sTxt = WezOpis(iTyp)
        End If

        Return sTxt
    End Function

    Public Function PokazRelacjePart2() As Integer()
        Dim iStep As Integer = miAdd / 20

        ' najpierw zerowanie wysokosci
        Dim aSlupki(15) As Integer
        For i As Integer = 0 To 14
            aSlupki(i) = 0
        Next

        ' teraz dodanie wartosci przesunietych o dekady
        For i As Integer = 0 To 14
            aSlupki(aTypTyp(i, iStep)) = aSlupki(aTypTyp(i, iStep)) + aDiffTyp(i)
        Next

        Return aSlupki

    End Function


    Public Sub PartOnNavigatedTo(sLosyRelacji As String)

        Dim aArr As String() = sLosyRelacji.Split(";")
        If aArr.GetUpperBound(0) <> 15 Then Exit Sub
        For i As Integer = 0 To 14
            aDiffTyp(i) = CInt(aArr(i))
        Next

    End Sub

End Class
