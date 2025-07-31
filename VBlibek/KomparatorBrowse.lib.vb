Imports pkar

Public Class KomparatorBrowse

    Public Shared sLastError As String = ""

    Private Shared Function WytnijSamePunkty(sFileContent As String, sMsg As String) As String
        If sFileContent.Length < 100 Then
            sLastError = Localize.GetResManString("errDataShort") & sMsg
            Return ""
        End If

        sFileContent += vbLf
        Dim iInd = 1

        If Localize.IsCurrentLang("PL") Then
            iInd = sFileContent.IndexOf("Poszczegolne odpowiedzi:")
            If iInd < 5 Then iInd = sFileContent.IndexOf("Poszczególne odpowiedzi:")
            ' nie ma błędu - jak Polak dostanie z USA odpowiedzi, to nie bedzie tego tekstu
            ' ale samo przeskoczenie iInd jest.
            'if (iInd < 5)
            '{
            '    p.k.DialogBox(p.k.GetLangString("errDataNoPoszczeg") + sMsg);
            '    return "";
            '}
            If iInd < 5 Then iInd = 5 ' jakby po poprzednim bylo -1...
        End If

        Dim sOut = sFileContent.Substring(iInd)
        iInd = sOut.IndexOf("1: ")

        If iInd < 2 Then
            sLastError = Localize.GetResManString("errDataNoAnswers") & " (" & sMsg & ")"
            Return ""
        End If

        Return sOut.Substring(iInd)
    End Function

    Private Shared Function Nazwa2Numer(ByVal sTyp As String) As Integer
        Select Case sTyp.Trim().ToLower()
            Case "egzodynamik", "exodynamic"
                Return 1
            Case "egzostatyk", "exostatic"
                Return 2
            Case "statyk", "static"
                Return 3
            Case "endostatyk", "endostatic"
                Return 4
            Case "endodynamik", "endodynamic"
                Return 5
        End Select

        Return 3    ' nie wiadomo co, error - ale przyjmijmy ze to srodek
    End Function

    Public Shared Function Porownaj(sResult1 As String, sResult2 As String) As String
        Dim sMsgTwoje As String = Localize.GetResManString("msgBrowseTwoje")
        Dim sMsgDrugi As String = Localize.GetResManString("msgBrowseDrugiej")

        Dim sTxt1 = WytnijSamePunkty(sResult1, sMsgTwoje)
        Dim sTxt2 = WytnijSamePunkty(sResult2, sMsgDrugi)

        If String.IsNullOrEmpty(sTxt1) Then Return ""
        If String.IsNullOrEmpty(sTxt2) Then Return ""

        ' zerowanie tablicy roznic
        Dim aDiffsy(60) As Integer
        For i As Integer = 1 To 59
            aDiffsy(i) = 0
        Next

        ' wypelnienie tablicy roznic
        Dim iInd As Integer
        Dim sTmp As String
        Dim iTyp1, iTyp2 As Integer

        For i = 1 To 35
            sTmp = i.ToString() & ": "

            If sTxt1.IndexOf(sTmp) <> 0 Or sTxt2.IndexOf(sTmp) <> 0 Then
                sLastError = Localize.GetResManString("errDataNoPoint") & " " & i
                Return ""
            End If

            sTxt1 = sTxt1.Substring(sTmp.Length)
            sTxt2 = sTxt2.Substring(sTmp.Length)
            iInd = sTxt1.IndexOf(vbCr)
            If iInd = -1 Then iInd = sTxt1.IndexOf(vbLf)

            If iInd < 2 Then
                sLastError = Localize.GetResManString("errDataNoThisAnswer") & " " & i & " - " & sMsgTwoje
                Return ""
            End If

            iTyp1 = Nazwa2Numer(sTxt1.Substring(0, iInd))
            sTxt1 = sTxt1.Substring(iInd + 1)
            iInd = sTxt2.IndexOf(vbCr)
            If iInd = -1 Then iInd = sTxt2.IndexOf(vbLf)

            If iInd < 2 Then
                sLastError = Localize.GetResManString("errDataNoThisAnswer") & " " & i & " - " & sMsgDrugi
                Return ""
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

        sTmp = ""
        sTmp = sTmp & aDiffsy(11).ToString() & ";"
        sTmp = sTmp & aDiffsy(12).ToString() & ";"
        sTmp = sTmp & aDiffsy(13).ToString() & ";"
        sTmp = sTmp & aDiffsy(14).ToString() & ";"
        sTmp = sTmp & aDiffsy(15).ToString() & ";"
        sTmp = sTmp & aDiffsy(22).ToString() & ";"
        sTmp = sTmp & aDiffsy(23).ToString() & ";"
        sTmp = sTmp & aDiffsy(24).ToString() & ";"
        sTmp = sTmp & aDiffsy(25).ToString() & ";"
        sTmp = sTmp & aDiffsy(33).ToString() & ";"
        sTmp = sTmp & aDiffsy(34).ToString() & ";"
        sTmp = sTmp & aDiffsy(35).ToString() & ";"
        sTmp = sTmp & aDiffsy(44).ToString() & ";"
        sTmp = sTmp & aDiffsy(45).ToString() & ";"
        sTmp = sTmp & aDiffsy(55).ToString() & ";"

        Return sTmp
    End Function


End Class
