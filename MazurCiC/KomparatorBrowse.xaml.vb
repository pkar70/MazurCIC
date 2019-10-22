' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.Storage
Imports Windows.Storage.Streams
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class KomparatorBrowse
    Inherits Page

    Private Function WytnijSamePunkty(sIn As String, sMsg As String) As String
        If sIn.Length < 100 Then
            App.DialogBox("Błąd: za krótkie dane " & sMsg)
            Return ""
        End If

        sIn = sIn & vbCrLf

        Dim iInd As Integer = sIn.IndexOf("Poszczegolne odpowiedzi:")
        If iInd < 5 Then iInd = sIn.IndexOf("Poszczególne odpowiedzi:")
        If iInd < 5 Then
            App.DialogBox("Błąd: nieprawidłowe dane " & sMsg)
            Return ""
        End If

        Dim sOut As String = sIn.Substring(iInd)
        iInd = sOut.IndexOf("1: ")
        If iInd < 2 Then
            App.DialogBox("Błąd: brak odpowiedzi (dane " & sMsg & ")")
            Return ""
        End If
        Return sOut.Substring(iInd)

    End Function

    Private Function Nazwa2Numer(sTyp As String) As Integer
        Select Case sTyp.Trim
            Case "egzodynamik"
                Return 1
            Case "egzostatyk"
                Return 2
            Case "statyk"
                Return 3
            Case "endostatyk"
                Return 4
            Case "endodynamik"
                Return 5
        End Select

        Return 3    ' nie wiadomo co, error - ale przyjmijmy ze to srodek
    End Function

    Private Sub uiPorownaj_Click(sender As Object, e As RoutedEventArgs)

        Dim sTxt1 As String = WytnijSamePunkty(uiText1.Text, "Twoje")
        Dim sTxt2 As String = WytnijSamePunkty(uiText2.Text, "drugiej osoby")
        If sTxt1 = "" Then Exit Sub
        If sTxt2 = "" Then Exit Sub

        ' zerowanie tablicy roznic
        Dim aDiffsy(60) As Integer
        For i As Integer = 1 To 59
            aDiffsy(i) = 0
        Next

        ' wypelnienie tablicy roznic
        Dim iInd As Integer
        Dim sTmp As String
        Dim bError As Boolean = False
        Dim iTyp1, iTyp2 As Integer

        For i As Integer = 1 To 35
            sTmp = i.ToString & ": "
            If sTxt1.IndexOf(sTmp) <> 0 Or sTxt2.IndexOf(sTmp) <> 0 Then
                App.DialogBox("Błąd: brak punktu " & i)
                bError = True
                Exit For
            End If
            sTxt1 = sTxt1.Substring(sTmp.Length)
            sTxt2 = sTxt2.Substring(sTmp.Length)

            iInd = sTxt1.IndexOf(vbCr)
            If iInd = -1 Then iInd = sTxt1.IndexOf(vbLf)
            If iInd < 2 Then
                App.DialogBox("Błąd: brak odpowiedzi " & i & " w Twoich danych")
                bError = True
                Exit For
            End If
            iTyp1 = Nazwa2Numer(sTxt1.Substring(0, iInd))
            sTxt1 = sTxt1.Substring(iInd + 1)

            iInd = sTxt2.IndexOf(vbCr)
            If iInd = -1 Then iInd = sTxt2.IndexOf(vbLf)
            If iInd < 2 Then
                App.DialogBox("Błąd: brak odpowiedzi " & i & " w danych drugiej osoby")
                bError = True
                Exit For
            End If
            iTyp2 = Nazwa2Numer(sTxt2.Substring(0, iInd))
            sTxt2 = sTxt2.Substring(iInd + 1)

            If iTyp1 < iTyp2 Then
                iInd = iTyp1 * 10 + iTyp2
            Else
                iInd = iTyp2 * 10 + iTyp1
            End If

            aDiffsy(iInd) += 1
        Next
        If bError Then Exit Sub

        sTmp = ""
        sTmp = sTmp & aDiffsy(11).ToString & ";"
        sTmp = sTmp & aDiffsy(12).ToString & ";"
        sTmp = sTmp & aDiffsy(13).ToString & ";"
        sTmp = sTmp & aDiffsy(14).ToString & ";"
        sTmp = sTmp & aDiffsy(15).ToString & ";"

        sTmp = sTmp & aDiffsy(22).ToString & ";"
        sTmp = sTmp & aDiffsy(23).ToString & ";"
        sTmp = sTmp & aDiffsy(24).ToString & ";"
        sTmp = sTmp & aDiffsy(25).ToString & ";"

        sTmp = sTmp & aDiffsy(33).ToString & ";"
        sTmp = sTmp & aDiffsy(34).ToString & ";"
        sTmp = sTmp & aDiffsy(35).ToString & ";"

        sTmp = sTmp & aDiffsy(44).ToString & ";"
        sTmp = sTmp & aDiffsy(45).ToString & ";"

        sTmp = sTmp & aDiffsy(55).ToString & ";"

        App.SetSettingsString("losyRelacjiParam", sTmp)
        Me.Frame.Navigate(GetType(LosyRelacji), sTmp)
    End Sub

    Private Async Sub uiPage_Loaded(sender As Object, e As RoutedEventArgs)
        ' wypelnij combobox

        uiCombo1.Items.Clear()
        Dim oFold As StorageFolder = ApplicationData.Current.LocalFolder
        Dim oFile As StorageFile = Await oFold.TryGetItemAsync("lista.txt")
        If oFile Is Nothing Then Exit Sub

        Dim oLns As IList(Of String) = Await FileIO.ReadLinesAsync(oFile)
        For Each oLine As String In oLns
            uiCombo1.Items.Add(oLine)
        Next

    End Sub

    Private Async Sub uiCombo1_Changed(sender As Object, e As SelectionChangedEventArgs) Handles uiCombo1.SelectionChanged
        ' zamien combobox na textbox

        Dim sFile As String = uiCombo1.SelectedValue & ".txt"
        Dim oFold As StorageFolder = ApplicationData.Current.LocalFolder
        Dim oFile As StorageFile = Await oFold.TryGetItemAsync(sFile)
        If oFile Is Nothing Then Exit Sub

        Dim sTxt As String = Await FileIO.ReadTextAsync(oFile)
        uiText1.Text = sTxt

    End Sub
End Class
