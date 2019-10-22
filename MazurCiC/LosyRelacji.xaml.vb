' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class LosyRelacji
    Inherits Page

    Dim aDiffTyp(20) As Integer
    Dim miAdd As Integer = 0

    Private Function WezOpis(iTyp As Integer) As String
        Dim oFile As StreamReader = File.OpenText("Assets\opisyRelacji.txt")

        Dim sSrch As String = "<h4>" & iTyp.ToString & "</h4>"
        Dim sLine As String = ""

        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf(sSrch) = 0 Then
                Exit Do
            End If
        Loop

        Dim sTmp As String
        sTmp = ""

        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf("<h4>") = 0 Then
                Exit Do
            End If
            sTmp = sTmp & vbCrLf & sLine
        Loop

        Return sTmp
    End Function

    Private Sub PokazRelacje()
        Dim iStep As Integer = miAdd / 20
        Dim iTyp As Integer = GetMaxTyp()  ' maximum różnic, pointer = 0..14

        ' tabelka przejsc pomiedzy typami
        ' aTypTyp(iTyp, iStep) = iNewTyp
        '                           {11,12,13,14,15, 22,23,24,25, 33,34,35, 44,45, 55}
        '                             0  1  2  3  4   5  6  7  8   9 10 11  12 13  14
        '                   DLA LAT  0,20,40,+60,+80
        Dim aTypTyp(,) As Integer = {
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

        iTyp = aTypTyp(iTyp, iStep)

        Dim sTxt As String
        If iTyp = 1 Or iTyp = 3 Or iTyp = 6 Or iTyp = 8 Or iTyp = 10 Or iTyp = 13 Then
            sTxt = "Relacja jest pośrednia między dwoma opisami:" & vbCrLf & vbCrLf & WezOpis(iTyp - 1) & vbCrLf & vbCrLf & "oraz" & vbCrLf & vbCrLf & WezOpis(iTyp + 1)
        Else
            sTxt = WezOpis(iTyp)
        End If

        uiOpis.Text = sTxt

        ' a teraz slupki
        '
        ' najpierw zerowanie wysokosci
        Dim aSlupki(15) As Integer
        For i As Integer = 0 To 14
            aSlupki(i) = 0
        Next

        ' teraz dodanie wartosci przesunietych o dekady
        For i As Integer = 0 To 14
            aSlupki(aTypTyp(i, iStep)) = aSlupki(aTypTyp(i, iStep)) + aDiffTyp(i)
        Next

        ' a teraz przepisanie tego do wielkosci - pewnie by mozna jakas petla...
        uiTyp00.Height = New GridLength(aSlupki(0))
        uiTyp01.Height = New GridLength(aSlupki(1))
        uiTyp02.Height = New GridLength(aSlupki(2))
        uiTyp03.Height = New GridLength(aSlupki(3))
        uiTyp04.Height = New GridLength(aSlupki(4))
        uiTyp05.Height = New GridLength(aSlupki(5))
        uiTyp06.Height = New GridLength(aSlupki(6))
        uiTyp07.Height = New GridLength(aSlupki(7))
        uiTyp08.Height = New GridLength(aSlupki(8))
        uiTyp09.Height = New GridLength(aSlupki(9))
        uiTyp10.Height = New GridLength(aSlupki(10))
        uiTyp11.Height = New GridLength(aSlupki(11))
        uiTyp12.Height = New GridLength(aSlupki(12))
        uiTyp13.Height = New GridLength(aSlupki(13))
        uiTyp14.Height = New GridLength(aSlupki(14))


    End Sub

    Private Sub EnableDisablePlusMinus()
        If miAdd < 20 Then
            uiBMinus.IsEnabled = False
        Else
            uiBMinus.IsEnabled = True
        End If

        If miAdd > 60 Then
            uiBPlus.IsEnabled = False
        Else
            uiBPlus.IsEnabled = True
        End If

        Dim iRok As Integer = Date.Now.Year() + miAdd
        uiBPlus.Content = (iRok + 20).ToString & ">"
        uiBMinus.Content = "<" & (iRok - 20).ToString

        If miAdd = 0 Then
            uiNaRok.Text = "Stan na dzisiaj"
        Else
            uiNaRok.Text = "Prognoza na " & iRok
        End If
    End Sub
    Private Sub uiMinus_Click(sender As Object, e As RoutedEventArgs) Handles uiBMinus.Click
        If miAdd > 19 Then miAdd -= 20
        EnableDisablePlusMinus()
        PokazRelacje()
    End Sub

    Private Sub uiKoniec_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(MainPage))
    End Sub

    Private Sub uiPlus_Click(sender As Object, e As RoutedEventArgs) Handles uiBPlus.Click
        If miAdd < 80 Then miAdd += 20
        EnableDisablePlusMinus()
        PokazRelacje()
    End Sub

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

    Private Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)

        EnableDisablePlusMinus()

        PokazRelacje()
    End Sub

    Protected Overrides Sub onNavigatedTo(e As NavigationEventArgs)
        ' Dim sTxt As String = e.Parameter.ToString
        Dim sTxt As String = App.GetSettingsString("losyRelacjiParam")

        Dim aArr As String() = sTxt.Split(";")
        If aArr.GetUpperBound(0) <> 15 Then Exit Sub
        For i As Integer = 0 To 14
            aDiffTyp(i) = CInt(aArr(i))
        Next

        'If sTxt.Length = 2 Then
        '    miTyp1 = CInt(sTxt.Substring(0, 1))
        '    miTyp2 = CInt(sTxt.Substring(1, 1))
        'Else
        '    miTyp1 = 0
        '    miTyp2 = 0
        'End If
    End Sub

End Class
