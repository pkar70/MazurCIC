﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System
Imports System.Reflection

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Class Resource_PL
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("VBlib.Resource_PL", GetType(Resource_PL).GetTypeInfo.Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to PL.
        '''</summary>
        Friend Shared ReadOnly Property _lang() As String
            Get
                Return ResourceManager.GetString("_lang", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Czy chcesz wysłać rezultat?.
        '''</summary>
        Friend Shared ReadOnly Property askWantSend() As String
            Get
                Return ResourceManager.GetString("askWantSend", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Załączam rezultat dzisiejszego testu dynamizmu charakteru.
        '''</summary>
        Friend Shared ReadOnly Property emailBodyStart() As String
            Get
                Return ResourceManager.GetString("emailBodyStart", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Mój dynamizm charakteru.
        '''</summary>
        Friend Shared ReadOnly Property emailSubject() As String
            Get
                Return ResourceManager.GetString("emailSubject", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Błąd.
        '''</summary>
        Friend Shared ReadOnly Property errAnyError() As String
            Get
                Return ResourceManager.GetString("errAnyError", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Błąd: brak odpowiedzi (dane.
        '''</summary>
        Friend Shared ReadOnly Property errDataNoAnswers() As String
            Get
                Return ResourceManager.GetString("errDataNoAnswers", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Błąd: brak punktu.
        '''</summary>
        Friend Shared ReadOnly Property errDataNoPoint() As String
            Get
                Return ResourceManager.GetString("errDataNoPoint", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Błąd: nieprawidłowe dane.
        '''</summary>
        Friend Shared ReadOnly Property errDataNoPoszczeg() As String
            Get
                Return ResourceManager.GetString("errDataNoPoszczeg", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Błąd: brak odpowiedzi.
        '''</summary>
        Friend Shared ReadOnly Property errDataNoThisAnswer() As String
            Get
                Return ResourceManager.GetString("errDataNoThisAnswer", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Błąd: za krótkie dane.
        '''</summary>
        Friend Shared ReadOnly Property errDataShort() As String
            Get
                Return ResourceManager.GetString("errDataShort", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Błąd odczytu pliku z danymi.
        '''</summary>
        Friend Shared ReadOnly Property errReadingPlikWynikow() As String
            Get
                Return ResourceManager.GetString("errReadingPlikWynikow", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Prawdopodobnie w trybie Landscape będzie wygodniej korzystać z aplikacji..
        '''</summary>
        Friend Shared ReadOnly Property msgBetterLandscape() As String
            Get
                Return ResourceManager.GetString("msgBetterLandscape", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to drugiej osoby.
        '''</summary>
        Friend Shared ReadOnly Property msgBrowseDrugiej() As String
            Get
                Return ResourceManager.GetString("msgBrowseDrugiej", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Twoje.
        '''</summary>
        Friend Shared ReadOnly Property msgBrowseTwoje() As String
            Get
                Return ResourceManager.GetString("msgBrowseTwoje", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Dalej&gt;.
        '''</summary>
        Friend Shared ReadOnly Property msgDalejDalej() As String
            Get
                Return ResourceManager.GetString("msgDalejDalej", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Wynik.
        '''</summary>
        Friend Shared ReadOnly Property msgDalejWynik() As String
            Get
                Return ResourceManager.GetString("msgDalejWynik", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Za duży rozrzut odpowiedzi..
        '''</summary>
        Friend Shared ReadOnly Property msgDuzyRozrzut() As String
            Get
                Return ResourceManager.GetString("msgDuzyRozrzut", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Charakter, a ściślej dynamizm charakteru, każdej osoby zmienia się wraz z wiekiem: zaczynając od egzodynamizmu, poprzez statyzm do endodynamizmu..
        '''</summary>
        Friend Shared ReadOnly Property msgEgzoDyn() As String
            Get
                Return ResourceManager.GetString("msgEgzoDyn", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Niniejsza aplikacja ułatwia samo-określenie aktualnego dynamizmu - podczas udzielania odpowiedzi na kolejne pytania w górnej części ekranu będzie konstruowany wykres odpowiadający dynamizmowi z tych odpowiedzi wynikającemu..
        '''</summary>
        Friend Shared ReadOnly Property msgEgzoStat() As String
            Get
                Return ResourceManager.GetString("msgEgzoStat", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to (porównywanie charakterów osób możliwe jest w aplikacji dla Windows).
        '''</summary>
        Friend Shared ReadOnly Property msgEndoDyn() As String
            Get
                Return ResourceManager.GetString("msgEndoDyn", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Opracowane na podstawie książki polskiego cybernetyka, prof. Mariana Mazura pt. &quot;Cybernetyka i charakter&quot;, tabela 15.2. Książka dostępna jest w postaci elektronicznej na stronach autonom.edu.pl ..
        '''</summary>
        Friend Shared ReadOnly Property msgEndoStat() As String
            Get
                Return ResourceManager.GetString("msgEndoStat", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Relacja jest pośrednia między dwoma opisami:.
        '''</summary>
        Friend Shared ReadOnly Property msgLosyBetween() As String
            Get
                Return ResourceManager.GetString("msgLosyBetween", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to oraz.
        '''</summary>
        Friend Shared ReadOnly Property msgLosyOraz() As String
            Get
                Return ResourceManager.GetString("msgLosyOraz", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Prognoza na.
        '''</summary>
        Friend Shared ReadOnly Property msgLosyPrognozaNa() As String
            Get
                Return ResourceManager.GetString("msgLosyPrognozaNa", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Stan na dzisiaj.
        '''</summary>
        Friend Shared ReadOnly Property msgLosyToday() As String
            Get
                Return ResourceManager.GetString("msgLosyToday", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Nie odpowiedziałeś na wszystkie pytania, a więc wyniki są niewiarygodne..
        '''</summary>
        Friend Shared ReadOnly Property msgMaloOdpowiedzi() As String
            Get
                Return ResourceManager.GetString("msgMaloOdpowiedzi", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Brak wyniku!.
        '''</summary>
        Friend Shared ReadOnly Property msgNoResult() As String
            Get
                Return ResourceManager.GetString("msgNoResult", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Poszczegolne odpowiedzi:.
        '''</summary>
        Friend Shared ReadOnly Property msgPoszczOdpowiedzi() As String
            Get
                Return ResourceManager.GetString("msgPoszczOdpowiedzi", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Rozklad odpowiedzi.
        '''</summary>
        Friend Shared ReadOnly Property msgRozkladOdpowiedzi() As String
            Get
                Return ResourceManager.GetString("msgRozkladOdpowiedzi", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Znając dynamizmy dwojga osób można z dużym prawdopodobieństwem oszacować jak będzie wyglądał ich związek, czy małżeństwo będzie szczęśliwe czy też się rozpadnie..
        '''</summary>
        Friend Shared ReadOnly Property msgStatyk() As String
            Get
                Return ResourceManager.GetString("msgStatyk", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Sprawdź dynamizm swojego charakteru.
        '''</summary>
        Friend Shared ReadOnly Property msgTeza() As String
            Get
                Return ResourceManager.GetString("msgTeza", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Tak.
        '''</summary>
        Friend Shared ReadOnly Property msgYes() As String
            Get
                Return ResourceManager.GetString("msgYes", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Solidarność egzodynamików jest słaba, ponieważ każdy z nich żyje w swoim świecie wyobraźni i tylko o nim chciałby mówić, nie słuchając o cudzych, toteż jedyną podstawą solidarności egzodynamików jest wspólna niechęć do narzucanych im zasad i przemocy. Dlatego dzieci wolą przebywać ze sobą bez udziału dorosłych - czyniąc wyjątek dla dorosłych egzodynamików, nie o wiek bowiem chodzi, lecz o klasę charakteru. Gdy w rodzinie jest jakiś wuj-figlarz, dzieci nie mogą się doczekać następnej jego wizyty. Solidarno [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje0() As String
            Get
                Return ResourceManager.GetString("relacje0", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Poddańczość egzodynamików wobec statyków występuje wtedy, gdy egzodynamicy przyswajają sobie zasady od statyków, na przykład dzieci od rodziców, uczniowie od nauczycieli. Towarzyszy temu opiekuńczość statyków wobec egzodynamików, na przykład rodziców wobec dzieci, nauczycieli wobec uczniów. Warunkiem występowania takich sympatii jest, żeby postępowanie jednego partnera stwarzało sytuację zgodną z charakterem drugiego, na przykład, żeby wpajanie zasad dzieciom zmierzało do zwiększenia atrakcyjności ich zai [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje1() As String
            Get
                Return ResourceManager.GetString("relacje1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Poddańczość u statyków i opiekuńczość u endodynamików są sympatiami występującymi w związku z przygotowaniem statyka do sukcesji po endodynamiku, gdy nadejdzie na to pora.
        '''Na podobnym tle mogą również występować afekty adoracji i protekcji.
        '''W małżeństwie statyka z endodynamiczką przedsiębiorcza żona wyżywa się w działalności poza domem, którym zajmuje się z upodobaniem statyczny mąż, zresztą nie zyskujący sobie przez to uznania żony, wyrażającej się o nim lekceważąco, że się do niczego innego nie nadaje [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje10() As String
            Get
                Return ResourceManager.GetString("relacje10", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Podstawą solidarności endostatyków jest statyczny odcień ich charakteru sprawiający, że podporządkowują się oni potrzebie tworzenia organizacji do osiągania wyznaczonych im celów. Natomiast do odchyleń od solidarności skłania endostatyków pragnienie zdobycia dla siebie wyższego stanowiska lub odgrywania większej roli. Typowym tego przykładem jest ograniczona solidarność członków grup kierowniczych.
        '''Przyjaźń endostatyków przybiera zwykle postać spółki. Odcień statyczny ich charteru zapewnia im rzetelność  [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje12() As String
            Get
                Return ResourceManager.GetString("relacje12", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Solidarność endodynamików jest słaba. Każdemu z nich zależy jedynie na własnej mocy socjologicznej i tylko lęk przed jej utratą skłania ich do solidarności między sobą, przeciw konkurentom.
        '''Przyjaźń między endodynamikami zawiązuje się niełatwo, i co najwyżej tylko w początkowym okresie zdobywania mocy socjologicznej. Po jej zdobyciu kończy się przyjaźń, a zaczyna rywalizacja. Tam gdzie moc socjologiczna jest niepodzielna, np. władza, o jej opanowaniu musi rozstrzygnąć walka między byłymi wspólnikami. Tak [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje13() As String
            Get
                Return ResourceManager.GetString("relacje13", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Okoliczność, że zwykle egzodynamizm występuje we wczesnym okresie życia, endodynamizm zaś w późnym, jest przyczyną, że sympatie oparte na przeciwieństwie klas charakteru powstają między młodymi kobietami i starszymi mężczyznami oraz młodymi mężczyznami i starszymi kobietami. O schadzkach ministrów z podniecającymi call girls ogół statyków dowiaduje się ze zgrozą jako o &quot;skandalu towarzyskim&quot;, a co najmniej ze zdziwieniem (&quot;tacy poważni politycy z takimi dziwkami, kto by to pomyślał&quot;), a tymczasem są to sp [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje3() As String
            Get
                Return ResourceManager.GetString("relacje3", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Podstawą solidarności egzostatyków jest statyczny odcień ich charakteru sprawiający, że podporządkowują się przynajmniej jakiejś ogólnej zasadzie, jak np. styl panujący w środowisku, wspólnota zawodowa itp. Natomiast do odchyleń od solidarności skłania egzostatyków pragnienie wyróżniania się ze środowiska, chęć popisywania się, podkreślanie własnej indywidualności. Ponieważ egzostatyzm występuje w okresie życia poprzedzającym wiek średni i u ludzi o upodobaniach artystycznych, więc też typowym przykładem  [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje5() As String
            Get
                Return ResourceManager.GetString("relacje5", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Statystyczna częstość sprawia, że egzostatyczka uchodzi za typ &quot;prawdziwej kobiety&quot;, wiernej, uczciwej, lojalnej (odcień statyczny), a zarazem pełnej wdzięku, nieco lekkomyślnej, nadającej życiu &quot;barwę&quot; (odcień egzodynamiczny), a endostatyk uchodzi za typ &quot;prawdziwego mężczyzny&quot;, wiernego, uczciwego, lojalnego (odcień statyczny), a zarazem dzielnego, przewidującego, mającego uzdolnienia organizacyjne (odcień endodynamiczny). Wspólność cech statycznych i przeciwieństwo pozostałych cech stwarzają podstawę d [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje6() As String
            Get
                Return ResourceManager.GetString("relacje6", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to 
        '''Solidarność statyków jest silna, statycy bowiem popierają innych statyków wyznających takie same zasady, oraz zrzeszają się w organizacjach opartych na takich zasadach, i służących do ich utwierdzania.
        '''Przyjaźnie statyków są szczególnie silne i trwałe, co wynika z takich cech statyków, jak wierność, szczerość, rzetelność i przywiązanie do długotrwałości. Sprawiają one, że nawet po wieloletniej przerwie w kontaktach zaprzyjaźnionych statyków przyjaźń ich utrzymuje się bez zmian.
        '''Małżeństwa statyków są sz [rest of string was truncated]&quot;;.
        '''</summary>
        Friend Shared ReadOnly Property relacje8() As String
            Get
                Return ResourceManager.GetString("relacje8", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Nie.
        '''</summary>
        Friend Shared ReadOnly Property resDlgNo() As String
            Get
                Return ResourceManager.GetString("resDlgNo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Tak.
        '''</summary>
        Friend Shared ReadOnly Property resDlgYes() As String
            Get
                Return ResourceManager.GetString("resDlgYes", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to egzodynamik.
        '''</summary>
        Friend Shared ReadOnly Property typEgzodynamik() As String
            Get
                Return ResourceManager.GetString("typEgzodynamik", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to egzostatyk.
        '''</summary>
        Friend Shared ReadOnly Property typEgzostatyk() As String
            Get
                Return ResourceManager.GetString("typEgzostatyk", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to endodynamik.
        '''</summary>
        Friend Shared ReadOnly Property typEndodynamik() As String
            Get
                Return ResourceManager.GetString("typEndodynamik", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to endostatyk.
        '''</summary>
        Friend Shared ReadOnly Property typEndostatyk() As String
            Get
                Return ResourceManager.GetString("typEndostatyk", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to statyk.
        '''</summary>
        Friend Shared ReadOnly Property typStatyk() As String
            Get
                Return ResourceManager.GetString("typStatyk", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Rezultat:.
        '''</summary>
        Friend Shared ReadOnly Property wynik00() As String
            Get
                Return ResourceManager.GetString("wynik00", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Prawdopodobnie masz poniżej 16 lat..
        '''</summary>
        Friend Shared ReadOnly Property wynik1l1() As String
            Get
                Return ResourceManager.GetString("wynik1l1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to W terminologii Erica Berne (analizie transakcyjnej) jesteś typowym Dzieckiem..
        '''</summary>
        Friend Shared ReadOnly Property wynik1l2() As String
            Get
                Return ResourceManager.GetString("wynik1l2", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Najlepszym partnerem w miłości będzie dla Ciebie endodynamik..
        '''</summary>
        Friend Shared ReadOnly Property wynik1l3() As String
            Get
                Return ResourceManager.GetString("wynik1l3", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Prawdopodobnie masz od 16 do 35 lat..
        '''</summary>
        Friend Shared ReadOnly Property wynik2l1() As String
            Get
                Return ResourceManager.GetString("wynik2l1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to W terminologii Erica Berne (analizie transakcyjnej) jesteś Dzieckiem z elementami Dorosłego..
        '''</summary>
        Friend Shared ReadOnly Property wynik2l2() As String
            Get
                Return ResourceManager.GetString("wynik2l2", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Najlepszym partnerem w miłości będzie dla Ciebie endostatyk..
        '''</summary>
        Friend Shared ReadOnly Property wynik2l3() As String
            Get
                Return ResourceManager.GetString("wynik2l3", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Prawdopodobnie masz od 36 do 60 lat..
        '''</summary>
        Friend Shared ReadOnly Property wynik3l1() As String
            Get
                Return ResourceManager.GetString("wynik3l1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to W terminologii Erica Berne (analizie transakcyjnej) stanowisz wzorcowy przykład Dorosłego..
        '''</summary>
        Friend Shared ReadOnly Property wynik3l2() As String
            Get
                Return ResourceManager.GetString("wynik3l2", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Najlepszym partnerem w miłości będzie dla Ciebie inny statyk..
        '''</summary>
        Friend Shared ReadOnly Property wynik3l3() As String
            Get
                Return ResourceManager.GetString("wynik3l3", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Prawdopodobnie masz powyżej 60 lat..
        '''</summary>
        Friend Shared ReadOnly Property wynik4l1() As String
            Get
                Return ResourceManager.GetString("wynik4l1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to W terminologii Erica Berne (analizie transakcyjnej) jesteś Dorosłym z elementami Rodzica..
        '''</summary>
        Friend Shared ReadOnly Property wynik4l2() As String
            Get
                Return ResourceManager.GetString("wynik4l2", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Najlepszym partnerem w miłości będzie dla Ciebie egzostatyk..
        '''</summary>
        Friend Shared ReadOnly Property wynik4l3() As String
            Get
                Return ResourceManager.GetString("wynik4l3", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Najwyraźniej masz tzw. &quot;charakter przyspieszony&quot;....
        '''</summary>
        Friend Shared ReadOnly Property wynik5l1() As String
            Get
                Return ResourceManager.GetString("wynik5l1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to W terminologii Erica Berne (analizie transakcyjnej) stanowisz wzorcowy przykład Rodzica..
        '''</summary>
        Friend Shared ReadOnly Property wynik5l2() As String
            Get
                Return ResourceManager.GetString("wynik5l2", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Najlepszym partnerem w miłości będzie dla Ciebie egzodynamik..
        '''</summary>
        Friend Shared ReadOnly Property wynik5l3() As String
            Get
                Return ResourceManager.GetString("wynik5l3", resourceCulture)
            End Get
        End Property
    End Class
End Namespace
