Public Class MainPage

    Public iPytanie As Integer = 0
    Public aiOdpowiedzi(40) As Integer

    Public Sub ResetOdpowiedzi()
        iPytanie = 0
        For i As Integer = 1 To 35
            aiOdpowiedzi(i) = 0
        Next
    End Sub

    Public Function Numer2Dynamizm(iNo As Integer) As String
        Select Case iNo
            Case 1
                Return GetLangString("typEgzodynamik")
            Case 2
                Return GetLangString("typEgzostatyk")
            Case 3
                Return GetLangString("typStatyk")
            Case 4
                Return GetLangString("typEndostatyk")
            Case 5
                Return GetLangString("typEndodynamik")
        End Select

        Return "ERROR"
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

    ''' <summary>
    ''' Zapisuje wynik do pliku 'datowanego' w sFolder, uzupełnia plik indeksowy
    ''' Zwraca body pliku, czyli co jest do wysłania emailem
    ''' </summary>
    ''' <param name="sFolder">Windows.Storage.ApplicationData.Current.LocalFolder.Path</param>
    ''' <param name="sTeza">tbTeza.Text</param>
    ''' <returns></returns>
    Public Function ZapiszWynik(sFolder As String, sTeza As String) As String

        Dim sTxt As String = ""

        sTxt = sTeza & " (" & SredniDynamizm().ToString("f2") &
        "/" & SredniDynamizmBezSmieci.ToString("f2") & ")" & vbCrLf & vbCrLf

        sTxt = sTxt & GetLangString("msgRozkladOdpowiedzi") & vbCrLf
        For i As Integer = 1 To 5
            sTxt = sTxt & Numer2Dynamizm(i) & ":" & vbTab & PoliczIleDynamizmu(i) & vbCrLf
        Next

        sTxt = sTxt & vbCrLf & GetLangString("msgPoszczOdpowiedzi") & vbCrLf

        For i As Integer = 1 To 35
            sTxt = sTxt & i.ToString & ": " & Numer2Dynamizm(aiOdpowiedzi(i)) & vbCrLf
        Next

        Dim sFileName As String = Date.Now.ToString("yyyyMMdd-HHmmss")
        IO.File.AppendAllText(IO.Path.Combine(sFolder, "lista.txt"), sFileName & vbCrLf)
        IO.File.WriteAllText(IO.Path.Combine(sFolder, sFileName & ".txt"), sTxt)

        Return sTxt
    End Function


End Class
