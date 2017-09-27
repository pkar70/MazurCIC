' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    Dim iPytanie = 0
    Dim aiOdpowiedzi(40) As Integer

    Private Async Sub Strona_Loaded(sender As Object, e As RoutedEventArgs)
        iPytanie = 0
        For i = 1 To 35
            aiOdpowiedzi(i) = 0
        Next

        If Not File.Exists("Assets\tekstyPytan.txt") Then
            Dim msg As ContentDialog
            msg = New ContentDialog With {
                .Title = "Dziwaczność",
                .Content = "Nie widzę pliku z treścią, a przecież był w Install!",
                .CloseButtonText = "Pa"
            }
            Await msg.ShowAsync()
        End If

        tbTeza.Text = "Sprawdź dynamizm swojego charakteru"
        tbEgzoDyn.Text = "  Charakter, a ściślej dynamizm charakteru, każdej osoby zmienia się wraz z wiekiem: zaczynając od egzodynamizmu, poprzez statyzm do endodynamizmu."
        tbEgzoStat.Text = "  Niniejsza aplikacja ułatwia samo-określenie aktualnego dynamizmu - podczas udzielania odpowiedzi na kolejne pytania w górnej części ekranu będzie konstruowany wykres odpowiadający dynamizmowi z tych odpowiedzi wynikającemu."
        tbStatyk.Text = "  Znając dynamizmy dwojga osób można z dużym prawdopodobieństwem oszacować jak będzie wyglądał ich związek, czy małżeństwo będzie szczęśliwe czy też się rozpadnie."
        tbEndoStat.Text = "  Opracowane na podstawie książki polskiego cybernetyka, prof. Mariana Mazura pt. ""Cybernetyka i charakter"", tabela 15.2. Książka dostępna jest w postaci elektronicznej na stronach autonom.edu.pl ."
        tbEndoDyn.Text = "  "

    End Sub

    Private Sub bDalej_Click(sender As Object, e As RoutedEventArgs) Handles bDalej.Click

        If iPytanie > 35 Then Exit Sub

        ZabierzOdpowiedz()

        If iPytanie > 34 Then
            PokazWynik()
        Else
            ZmianaPytania(iPytanie + 1)
        End If

    End Sub
    Private Sub bWstecz_Click(sender As Object, e As RoutedEventArgs) Handles bWstecz.Click

        If iPytanie < 2 Then Exit Sub

        ZabierzOdpowiedz()
        ZmianaPytania(iPytanie - 1)

    End Sub
    Private Sub ZmianaPytania(iNowe As Integer)

        If iNowe = 35 Then
            bDalej.Content = "Wynik"
        Else
            bDalej.Content = "Dalej>"
        End If

        If iNowe = 1 Then
            bWstecz.IsEnabled = False
        Else
            bWstecz.IsEnabled = True
        End If

        cbEgzoDyn.Visibility = Visibility.Visible
        cbEgzoStat.Visibility = Visibility.Visible
        cbStatyk.Visibility = Visibility.Visible
        cbEndoStat.Visibility = Visibility.Visible
        cbEndoDyn.Visibility = Visibility.Visible

        cbEgzoDyn.IsChecked = False
        cbEgzoStat.IsChecked = False
        cbStatyk.IsChecked = False
        cbEndoStat.IsChecked = False
        cbEndoDyn.IsChecked = False


        iPytanie = iNowe

        Dim oFile As StreamReader
        Dim sSrch, sLine As String

        oFile = File.OpenText("Assets\tekstyPytan.txt")
        sSrch = "Twierdzenie 15. " & iNowe & " "
        sLine = ""

        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf(sSrch) = 0 Then
                Exit Do
            End If
        Loop

        ' Twierdzenie 15. 24 (o stosunku do panowania). Im mniejszy jest współczynnik dynamizmu, tym silniejsze jest dążenie do panowania.
        Dim iInd As Integer

        iInd = sLine.IndexOf("). ")
        tbTeza.Text = sLine.Substring(iInd + 3)

        'C) Egzodynamik nie chce ...
        'BC) Egzostatyk przeciwstawia ...
        'B) Statyk uznaje ...
        'AB) Endostatyk sprzyja ...
        'A) Endodynamik pragnie ...
        Dim sTmp As String
        sTmp = ""

        ' pomin wszystko do C) 

        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf("C) ") = 0 Then
                Exit Do
            End If
        Loop

        sTmp = sLine.Substring(3)
        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf("BC) ") = 0 Then
                Exit Do
            End If
            sTmp = sTmp & vbCrLf & sLine
        Loop

        tbEgzoDyn.Text = sTmp

        sTmp = sLine.Substring(4)
        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf("B) ") = 0 Then
                Exit Do
            End If
            sTmp = sTmp & vbCrLf & sLine
        Loop

        tbEgzoStat.Text = sTmp
        'cbEgzoStat.Content = sTmp

        sTmp = sLine.Substring(3)
        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf("AB) ") = 0 Then
                Exit Do
            End If
            sTmp = sTmp & vbCrLf & sLine
        Loop

        tbStatyk.Text = sTmp

        sTmp = sLine.Substring(4)
        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf("A) ") = 0 Then
                Exit Do
            End If
            sTmp = sTmp & vbCrLf & sLine
        Loop

        tbEndoStat.Text = sTmp

        sTmp = sLine.Substring(3)
        Do While Not oFile.EndOfStream
            sLine = oFile.ReadLine
            If sLine.IndexOf("Twierdzenie 15") = 0 Then
                Exit Do
            End If
            sTmp = sTmp & vbCrLf & sLine
        Loop

        tbEndoDyn.Text = sTmp.Trim

    End Sub

    Private Sub ZabierzOdpowiedz()
        Dim iOdp = 0
        If cbEgzoDyn.IsChecked Then iOdp = 1
        If cbEgzoStat.IsChecked Then iOdp = 2
        If cbStatyk.IsChecked Then iOdp = 3
        If cbEndoStat.IsChecked Then iOdp = 4
        If cbEndoDyn.IsChecked Then iOdp = 5

        If iOdp = 0 Then
            ' zapytaj czy na pewno mimo braku odpowiedzi - ale wtedy funkcja :)
        End If

        aiOdpowiedzi(iPytanie) = iOdp

        Dim iSumy(6) As Integer
        For i = 1 To 35
            iSumy(aiOdpowiedzi(i)) = iSumy(aiOdpowiedzi(i)) + 1
        Next

        grEgzoDyn.Height = New GridLength(iSumy(1))
        grEgzoStat.Height = New GridLength(iSumy(2))
        grStatyk.Height = New GridLength(iSumy(3))
        grEndoStat.Height = New GridLength(iSumy(4))
        grEndoDyn.Height = New GridLength(iSumy(5))

    End Sub


    Private Sub PokazWynik()

        cbEgzoDyn.Visibility = Visibility.Collapsed
        cbEgzoStat.Visibility = Visibility.Collapsed
        cbStatyk.Visibility = Visibility.Collapsed
        cbEndoStat.Visibility = Visibility.Collapsed
        cbEndoDyn.Visibility = Visibility.Collapsed

        tbEgzoStat.Text = ""
        tbStatyk.Text = ""
        tbEndoStat.Text = ""
        tbEndoDyn.Text = ""

        Dim iSumy(6) As Integer
        Dim iNonZero = 0
        Dim sTxt = ""

        For i = 1 To 35
            iSumy(aiOdpowiedzi(i)) = iSumy(aiOdpowiedzi(i)) + 1
            If aiOdpowiedzi(i) = 0 Then
                sTxt = "    Nie odpowiedziałeś na wszystkie pytania, a więc wyniki są niewiarygodne."
            Else
                iNonZero = iNonZero + 1
            End If
        Next

        Dim iMax = 0
        Dim iTyp = 0
        Dim bOk = False
        For i = 1 To 5
            If iSumy(i) > iNonZero * 0.4 Then bOk = True
            If iSumy(i) > iMax Then
                iMax = iSumy(i)
                iTyp = i
            End If
        Next

        If Not bOk Then
            sTxt = sTxt & "   Za duży rozrzut odpowiedzi."
        End If

        tbEgzoDyn.Text = sTxt


        Select Case iTyp
            Case 0
                tbTeza.Text = "Brak wyniku!"
            Case 1  ' C
                tbTeza.Text = "Rezultat: egzodynamik"
                tbEgzoStat.Text = "    Prawdopodobnie masz poniżej 16 lat."
                tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) jesteś typowym Dzieckiem."
                tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie endodynamik."
            Case 2 ' BC
                tbTeza.Text = "Rezultat: egzostatyk"
                tbEgzoStat.Text = "    Prawdopodobnie masz od 16 do 35 lat."
                tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) jesteś Dzieckiem z elementami Dorosłego."
                tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie endostatyk."
            Case 3  ' B
                tbTeza.Text = "Rezultat: statyk"
                tbEgzoStat.Text = "    Prawdopodobnie masz od 36 do 65 lat."
                tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) stanowisz wzorcowy przykład Dorosłego."
                tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie inny statyk."
            Case 4  ' AB
                tbTeza.Text = "Rezultat: endostatyk"
                tbEgzoStat.Text = "    Prawdopodobnie masz powyżej 60 lat."
                tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) jesteś Dorosłym z elementami Rodzica."
                tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie egzostatyk."
            Case 5  ' A
                tbTeza.Text = "Rezultat: endodynamik"
                tbEgzoStat.Text = "    Najwyraźniej masz tzw. ""charakter przyspieszony""..."
                tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) stanowisz wzorcowy przykład Rodzica."
                tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie egzodynamik."
        End Select


        bDalej.Content = " "
        bDalej.IsEnabled = False
    End Sub

End Class
