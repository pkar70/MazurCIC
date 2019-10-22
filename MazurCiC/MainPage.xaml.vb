' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

Imports Windows.Storage
Imports Windows.UI.Popups
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page

    Dim iPytanie As Integer = 0
    Dim aiOdpowiedzi(40) As Integer

    Private Async Sub Strona_Loaded(sender As Object, e As RoutedEventArgs)
        iPytanie = 0
        For i As Integer = 1 To 35
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

        uiProgress.Value = iPytanie

        ZabierzOdpowiedz()

        If iPytanie > 34 Then
            uiMenu.Visibility = Visibility.Visible
            PokazWynik()
        Else
            uiMenu.Visibility = Visibility.Collapsed
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
        Dim iOdp As Integer = 0
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
        For i As Integer = 1 To 35
            iSumy(aiOdpowiedzi(i)) = iSumy(aiOdpowiedzi(i)) + 1
        Next

        grEgzoDyn.Height = New GridLength(iSumy(1))
        grEgzoStat.Height = New GridLength(iSumy(2))
        grStatyk.Height = New GridLength(iSumy(3))
        grEndoStat.Height = New GridLength(iSumy(4))
        grEndoDyn.Height = New GridLength(iSumy(5))

    End Sub

    Private Function Numer2Dynamizm(iNo As Integer) As String
        Select Case iNo
            Case 1
                Return "egzodynamik"
            Case 2
                Return "egzostatyk"
            Case 3
                Return "statyk"
            Case 4
                Return "endostatyk"
            Case 5
                Return "endodynamik"
            Case Else
                Return "ERROR"
        End Select
    End Function
    Private Function PoliczIleDynamizmu(iNo As Integer) As Integer
        Dim iRet As Integer = 0
        For i As Integer = 1 To 35
            If aiOdpowiedzi(i) = iNo Then iRet += 1
        Next
        Return iRet
    End Function
    Private Function SredniDynamizm() As Double
        Dim iSuma As Double = 0
        Dim iCnt As Integer = 0

        For i As Integer = 1 To 35
            If aiOdpowiedzi(i) > 0 Then iCnt += 1
            iSuma += aiOdpowiedzi(i)
        Next

        Return iSuma / iCnt
    End Function
    Private Function SredniDynamizmBezSmieci() As Double
        Dim aDynamizm(6) As Integer

        For i As Integer = 1 To 5
            aDynamizm(i) = 0
        Next

        For i As Integer = 1 To 35
            aDynamizm(aiOdpowiedzi(i)) += 1
        Next

        ' uciecie smieci od dołu i od góry
        For i As Integer = 1 To 5
            If aDynamizm(i) > 4 Then Exit For
            aDynamizm(i) = 0
        Next

        For i As Integer = 5 To 1 Step -1
            If aDynamizm(i) > 4 Then Exit For
            aDynamizm(i) = 0
        Next

        Dim iCnt As Integer = 0
        Dim iSuma As Double = 0
        For i As Integer = 1 To 5
            iCnt += aDynamizm(i)
            iSuma += i * aDynamizm(i)
        Next

        Return iSuma / iCnt

    End Function
    Private Async Function ZapiszWynik() As Task
        Dim oFold As StorageFolder = ApplicationData.Current.LocalFolder

        Dim sTxt As String = ""

        sTxt = tbTeza.Text & " (" & SredniDynamizm().ToString("f2") &
        "/" & SredniDynamizmBezSmieci.ToString("f2") & ")" & vbCrLf & vbCrLf

        sTxt = sTxt & "Rozklad odpowiedzi:" & vbCrLf
        For i As Integer = 1 To 5
            sTxt = sTxt & Numer2Dynamizm(i) & ":" & vbTab & PoliczIleDynamizmu(i) & vbCrLf
        Next

        sTxt = sTxt & vbCrLf & "Poszczególne odpowiedzi:" & vbCrLf

        For i As Integer = 1 To 35
            sTxt = sTxt & i.ToString & ": " & Numer2Dynamizm(aiOdpowiedzi(i)) & vbCrLf
        Next

        Dim sFileName As String = Date.Now.ToString("yyyyMMdd-HHmmss")
        Dim oFile As StorageFile = Await oFold.CreateFileAsync("lista.txt", CreationCollisionOption.OpenIfExists)
        Await FileIO.AppendTextAsync(oFile, sFileName & vbCrLf)

        oFile = Await oFold.CreateFileAsync(sFileName & ".txt")
        Await FileIO.AppendTextAsync(oFile, sTxt)

        If Not Await App.AskYN("Czy chcesz wysłać rezultat?") Then Exit Function

        Dim oMsg As Email.EmailMessage = New Windows.ApplicationModel.Email.EmailMessage()
        oMsg.Subject = "Mój dynamizm charakteru"

        sTxt = "Załączam rezultat dzisiejszego testu dynamizmu charakteru" & vbCrLf & vbCrLf &
            "Data: " & Date.Now & vbCrLf & vbCrLf &
            sTxt

        oMsg.Body = sTxt

        ' załączniki działają tylko w default windows mail app
        'Dim oStream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(oFile)
        'Dim oAttch = New Email.EmailAttachment("rezultat.txt", oStream)
        'oMsg.Attachments.Add(oAttch)

        Await Email.EmailManager.ShowComposeNewEmailAsync(oMsg)
    End Function


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
        Dim iNonZero As Integer = 0
        Dim sTxt As String = ""

        For i As Integer = 1 To 35
            iSumy(aiOdpowiedzi(i)) = iSumy(aiOdpowiedzi(i)) + 1
            If aiOdpowiedzi(i) = 0 Then
                sTxt = "    Nie odpowiedziałeś na wszystkie pytania, a więc wyniki są niewiarygodne."
            Else
                iNonZero = iNonZero + 1
            End If
        Next

        Dim iMax As Integer = 0
        Dim iTyp As Integer = 0
        Dim bOk As Boolean = False
        For i As Integer = 1 To 5
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
                tbEgzoStat.Text = "    Prawdopodobnie masz od 36 do 60 lat."
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


        ' bDalej.Content = " "
        ' bDalej.IsEnabled = False
        bDalej.Visibility = Visibility.Collapsed
        uiProgress.Visibility = Visibility.Collapsed
        bWstecz.Visibility = Visibility.Collapsed

        ZapiszWynik()
    End Sub

    Private Sub uiTapStatyk_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles tbStatyk.Tapped
        If cbStatyk.IsChecked Then
            bDalej_Click(sender, e)
        Else
            cbStatyk.IsChecked = True
        End If
    End Sub

    Private Sub uiTapEgzoDyn_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles tbEgzoDyn.Tapped
        If cbEgzoDyn.IsChecked Then
            bDalej_Click(sender, e)
        Else
            cbEgzoDyn.IsChecked = True
        End If
    End Sub

    Private Sub uiTapEgzoStat_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles tbEgzoStat.Tapped
        If cbEgzoStat.IsChecked Then
            bDalej_Click(sender, e)
        Else
            cbEgzoStat.IsChecked = True
        End If
    End Sub

    Private Sub uiTapEndoStat_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles tbEndoStat.Tapped
        If cbEndoStat.IsChecked Then
            bDalej_Click(sender, e)
        Else
            cbEndoStat.IsChecked = True
        End If
    End Sub

    Private Sub uiTapEndoDyn_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles tbEndoDyn.Tapped
        If cbEndoDyn.IsChecked Then
            bDalej_Click(sender, e)
        Else
            cbEndoDyn.IsChecked = True
        End If
    End Sub

    Private Sub uiKomparator_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(KomparatorBrowse))
    End Sub
End Class
