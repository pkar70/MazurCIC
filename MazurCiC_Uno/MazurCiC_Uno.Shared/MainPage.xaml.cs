
/*

2021.06.19
 * [Android] przejście na Uno last stable 3.8 (z którego robię własny build dla pozostałych app).
        Teoretycznie bez zmian, ale chodzi o sprawdzenie czy czegoś nie spieprzyli w Uno

2021.04.03
* [Android] przejście na Uno last stable 3.6.6 (z którego robię własny build dla pozostałych app).
        Teoretycznie bez zmian, ale chodzi o sprawdzenie czy czegoś nie spieprzyli w Uno

2021.02.26
* [Android] przejście na Uno last stable 3.5.1 (z którego robię własny build dla pozostałych app).
        Teoretycznie bez zmian, ale chodzi o sprawdzenie czy czegoś nie spieprzyli w Uno

2020.10.27
* [Android] przejście na Uno last stable 3.1.6 (z którego robię własny build dla pozostałych app).
        Teoretycznie bez zmian, ale chodzi o sprawdzenie czy czegoś nie spieprzyli w Uno

2020.08.27
* [Android] przejście z Uno.945 (własne) do 3.0.12 (laststable)
*   bo gogus wymaga aktualizacji do SDK 10, a to dopiero później zrobili w Uno
* nie działa, próba wycofania się: wyłączam splash, zmiany w main.cs, 

2020.02.12
 * [Android] przejście na Uno.945 (moja kompilacja) - zaktualizowanie pkModuleShared (np. Uno już ma ClipPut, etc.)
 * [Android] splashscreen
 * przywracanie guzików przy Page_Load - jakby był powrót do aplikacji po wyłączeniu guzików
 * z mainpage usuwam stary kod (plik z pytaniami - sprzed migracji do Resources\tezy.rese)

2019.12.22
 * rebase Uno (pkar: GetAppVersion, OpenBrowser)
 * MailManager w Uno - usuwam Xamarin.Essentials


* podniesienie trochę tekstów do góry (był błąd! wpisywało wszystko do Row=0, a nie do Row=1!)
* gdy komentarz jest pusty, to Visibility.Collapsed


STORE 10.1910

* migracja do C#/Uno
* migracja do pkarmodule (namespace/class p.k.)
* przeniesienie stringów z MainPage.XAML/.cs do Strings\PL (nie z innych stron, bo inne i tak tylko pod Windows)
* przeniesienie stringów z tekstyPytan.txt do tezy.resw (bo Android nie umie wstawić pliku do appx), a poza tym ułatwienie w translacji późniejszej
* dodanie komentarza do pytań (niektórych) - było w pliku, ale nie było pokazywane w ogóle.
* dla nie UWP pokazuje info na początku - że wersja Windows jest bogatsza
* UWP/IsThisMoje - wymuszenie PL, nawet jak start nie jest w PL [w UWP, bo w Uno to nie dziala]
* przetłumaczenie pytań na angielski (głównie googletranslator)
* teksty z KomparatorBrowse do strings.resw (xaml i .cs)
* teksty z LosyRelacji do strings.resw [ale nie same relacje!]
* przy cofaniu - cofa ProgressBar
* przeniesienie tekstów losów relacji z txt do resw

STRIPPEDDOWN nonUWP:
* nie ma pliku z odpowiedziami pamiętanego (Uno bez StorageFile, bez AppendStringAsync)
* nie ma porównywarki (to wynika z powyższego, ale także ze świadomej decyzji - "windows lepsze")

Wersja LAST VB: 1.7.1, 2018.09.03

 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MazurCiC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private int iPytanie = 0;
        private int[] aiOdpowiedzi = new int[41];

        private void Strona_Loaded(object sender, RoutedEventArgs e)
        {
            iPytanie = 0;
            for (int i = 1; i <= 35; i++)
                aiOdpowiedzi[i] = 0;

            // bo w Uno not implemented
            // musi byc wczesniej, bo inaczej pierwsza strona jest jeszcze po EN
            //#if NETFX_CORE
            //            if(p.k.IsThisMoje()) Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pl";
            //#endif 


            tbTeza.Text = p.k.GetLangString("msgTeza"); // "Sprawdź dynamizm swojego charakteru";
            tbEgzoDyn.Text = "  " + p.k.GetLangString("msgEgzoDyn"); // "  Charakter, a ściślej dynamizm charakteru, każdej osoby zmienia się wraz z wiekiem: zaczynając od egzodynamizmu, poprzez statyzm do endodynamizmu.";
            tbEgzoStat.Text = "  " + p.k.GetLangString("msgEgzoStat"); // "  Niniejsza aplikacja ułatwia samo-określenie aktualnego dynamizmu - podczas udzielania odpowiedzi na kolejne pytania w górnej części ekranu będzie konstruowany wykres odpowiadający dynamizmowi z tych odpowiedzi wynikającemu.";
            tbStatyk.Text = "  " + p.k.GetLangString("msgStatyk"); // "  Znając dynamizmy dwojga osób można z dużym prawdopodobieństwem oszacować jak będzie wyglądał ich związek, czy małżeństwo będzie szczęśliwe czy też się rozpadnie.";
            tbEndoStat.Text = "  " + p.k.GetLangString("msgEndoStat"); // "  Opracowane na podstawie książki polskiego cybernetyka, prof. Mariana Mazura pt. \"Cybernetyka i charakter\", tabela 15.2. Książka dostępna jest w postaci elektronicznej na stronach autonom.edu.pl .";

            tbRemark.FontSize = 0.85 * tbTeza.FontSize; // niby moglbym narzucic w XAML, ale tak jest elastyczniej :)

            string sTmp = " ";
            if(!p.k.GetPlatform("uwp")) sTmp = "  " + p.k.GetLangString("msgEndoDyn");
            tbEndoDyn.Text = sTmp;

            // powtorne uruchomienie to czasem powrot do strony glownej - więc przywróć guziczki, żeby można było coś zrobić
            bDalej.Visibility = Visibility.Visible;
            bWstecz.Visibility = Visibility.Visible;
            bWstecz.IsEnabled = false;
            bDalej.Content = p.k.GetLangString("msgDalejDalej"); // "Dalej>";

            // do sprawdzenia ktora szerokosc jest podana;
            // jesli <1000, to gdy width<height propozycja obrocenia ekranu
            if (uiGrid.ActualWidth < 800)
            {
                if (uiGrid.ActualWidth < uiGrid.ActualHeight)
                    p.k.DialogBoxRes("msgBetterLandscape");
            }

        }


        private void bDalej_Click(object sender, RoutedEventArgs e)
        {
            if (iPytanie > 35)
                return;

            uiProgress.Value = iPytanie;

            ZabierzOdpowiedz();

            if (iPytanie > 34)
            {

#if NETFX_CORE
                uiMenu.Visibility = Visibility.Visible;
#endif
                PokazWynik();
            }
            else
            {
#if NETFX_CORE
                uiMenu.Visibility = Visibility.Collapsed;
#endif
                ZmianaPytania(iPytanie + 1);
            }
        }
        private void bWstecz_Click(object sender, RoutedEventArgs e)
        {
            if (iPytanie < 2)
                return;

            uiProgress.Value = iPytanie;

            ZabierzOdpowiedz();
            ZmianaPytania(iPytanie - 1);
        }
        private void ZmianaPytania(int iNowe)
        {
            if (iNowe == 35)
                bDalej.Content = p.k.GetLangString("msgDalejWynik"); //"Wynik";
            else
                bDalej.Content = p.k.GetLangString("msgDalejDalej"); // "Dalej>";

            if (iNowe == 1)
                bWstecz.IsEnabled = false;
            else
                bWstecz.IsEnabled = true;

            cbEgzoDyn.Visibility = Visibility.Visible;
            cbEgzoStat.Visibility = Visibility.Visible;
            cbStatyk.Visibility = Visibility.Visible;
            cbEndoStat.Visibility = Visibility.Visible;
            cbEndoDyn.Visibility = Visibility.Visible;

            cbEgzoDyn.IsChecked = false;
            cbEgzoStat.IsChecked = false;
            cbStatyk.IsChecked = false;
            cbEndoStat.IsChecked = false;
            cbEndoDyn.IsChecked = false;


            iPytanie = iNowe;

            string sStringName = "/tezy/" + "t" + iNowe.ToString();

            string sTmp = p.k.GetLangString(sStringName + "t");
            int iInd;

            iInd = sTmp.IndexOf("). ");
            if (iInd > 0) sTmp = sTmp.Substring(iInd + 3);
            tbTeza.Text = sTmp.Trim();
            sTmp = p.k.GetLangString(sStringName + "r").Trim();
            if(sTmp != sStringName + "r") tbRemark.Text = sTmp.Trim();
            tbEgzoDyn.Text = p.k.GetLangString(sStringName + "a1").Trim();
            tbEgzoStat.Text = p.k.GetLangString(sStringName + "a2").Trim();
            tbStatyk.Text = p.k.GetLangString(sStringName + "a3").Trim();
            tbEndoStat.Text = p.k.GetLangString(sStringName + "a4").Trim();
            tbEndoDyn.Text = p.k.GetLangString(sStringName + "a5").Trim();

            if (tbRemark.Text.Length < 2)
                tbRemark.Visibility = Visibility.Collapsed;
            else
                tbRemark.Visibility = Visibility.Visible;

        }

        private void ZabierzOdpowiedz()
        {
            int iOdp = 0;
            if (cbEgzoDyn.IsChecked.HasValue && cbEgzoDyn.IsChecked.Value)
                iOdp = 1;
            if (cbEgzoStat.IsChecked.HasValue && cbEgzoStat.IsChecked.Value)
                iOdp = 2;
            if (cbStatyk.IsChecked.HasValue && cbStatyk.IsChecked.Value)
                iOdp = 3;
            if (cbEndoStat.IsChecked.HasValue && cbEndoStat.IsChecked.Value)
                iOdp = 4;
            if (cbEndoDyn.IsChecked.HasValue && cbEndoDyn.IsChecked.Value)
                iOdp = 5;

            if (iOdp == 0)
            {
            }

            aiOdpowiedzi[iPytanie] = iOdp;

            var iSumy = new int[7];
            for (int i = 1; i <= 35; i++)
                iSumy[aiOdpowiedzi[i]] = iSumy[aiOdpowiedzi[i]] + 1;

            grEgzoDyn.Height = new GridLength(iSumy[1]);
            grEgzoStat.Height = new GridLength(iSumy[2]);
            grStatyk.Height = new GridLength(iSumy[3]);
            grEndoStat.Height = new GridLength(iSumy[4]);
            grEndoDyn.Height = new GridLength(iSumy[5]);
        }

        private string Numer2Dynamizm(int iNo)
        {
            switch (iNo)
            {
                case 1:
                    return p.k.GetLangString("typEgzodynamik"); // "egzodynamik";
                case 2:
                        return p.k.GetLangString("typEgzostatyk"); //"egzostatyk";
                case 3:
                        return p.k.GetLangString("typStatyk"); //"statyk";
                case 4:
                        return p.k.GetLangString("typEndostatyk"); //"endostatyk";
                case 5:
                        return p.k.GetLangString("typEndodynamik"); //"endodynamik";
                default:
                        return "ERROR";
            }

            // wersje EN: exodynamic, exostatic, static, endostatic, endodynamic
        }
        private int PoliczIleDynamizmu(int iNo)
        {
            int iRet = 0;
            for (int i = 1; i <= 35; i++)
            {
                if (aiOdpowiedzi[i] == iNo)
                    iRet += 1;
            }
            return iRet;
        }
        private double SredniDynamizm()
        {
            double iSuma = 0;
            int iCnt = 0;

            for (int i = 1; i <= 35; i++)
            {
                if (aiOdpowiedzi[i] > 0)
                    iCnt += 1;
                iSuma += aiOdpowiedzi[i];
            }

            return iSuma / iCnt;
        }
        private double SredniDynamizmBezSmieci()
        {
            var aDynamizm = new int[7];

            for (int i = 1; i <= 5; i++)
                aDynamizm[i] = 0;

            for (int i = 1; i <= 35; i++)
                aDynamizm[aiOdpowiedzi[i]] += 1;

            // uciecie smieci od dołu i od góry
            for (int i = 1; i <= 5; i++)
            {
                if (aDynamizm[i] > 4)
                    break;
                aDynamizm[i] = 0;
            }

            for (int i = 5; i >= 1; i += -1)
            {
                if (aDynamizm[i] > 4)
                    break;
                aDynamizm[i] = 0;
            }

            int iCnt = 0;
            double iSuma = 0;
            for (int i = 1; i <= 5; i++)
            {
                iCnt += aDynamizm[i];
                iSuma += i * aDynamizm[i];
            }

            return iSuma / iCnt;
        }
        private async System.Threading.Tasks.Task ZapiszWynik()
        {

            string sTxt = "";

            sTxt = tbTeza.Text + " (" + SredniDynamizm().ToString("f2") + "/" + SredniDynamizmBezSmieci().ToString("f2") + ")\n\n";

            sTxt = sTxt + p.k.GetLangString("msgRozkladOdpowiedzi") + "\n"; // "Rozklad odpowiedzi:\n";
            for (int i = 1; i <= 5; i++)
                sTxt = sTxt + Numer2Dynamizm(i) + ":\t" + PoliczIleDynamizmu(i).ToString() + "\n";

            string sPoszczOdp = p.k.GetLangString("msgPoszczOdpowiedzi"); // Poszczególne odpowiedzi:\n";
            sTxt = sTxt + "\n" + sPoszczOdp + "\n";
            //if (sPoszczOdp != "Poszczególne odpowiedzi:")
            //    sTxt += "\nPoszczególne odpowiedzi:\n";   // wersja polska byc musi, bo porównywarka tego szuka

            for (int i = 1; i <= 35; i++)
                sTxt = sTxt + i.ToString() + ": " + Numer2Dynamizm(aiOdpowiedzi[i]) + "\n";

            // zapisuje plik, mimo że Android tego nie wykorzystuje w ogóle
            Windows.Storage.StorageFolder oFold = Windows.Storage.ApplicationData.Current.LocalFolder;
            string sFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss");

            // plik indeksowy (omijanie niedziałania oFold.Files()
            Windows.Storage.StorageFile oFile;
            oFile = await oFold.CreateFileAsync("lista.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
            await Windows.Storage.FileIO.AppendTextAsync(oFile, sFileName + "\n", Windows.Storage.Streams.UnicodeEncoding.Utf8);

            // plik z danymi

            oFile = await oFold.CreateFileAsync(sFileName + ".txt");

            await Windows.Storage.FileIO.AppendTextAsync(oFile, sTxt, Windows.Storage.Streams.UnicodeEncoding.Utf8);

            if (p.k.GetPlatform("uwp"))
            { // na Android - wysyłamy bez pytania, bo nie ma pokazywania zapamiętanych rezultatów
                if (!await p.k.DialogBoxResYNAsync("askWantSend")) // Czy chcesz wysłać rezultat?"))
                    return;
            }

            Windows.ApplicationModel.Email.EmailMessage oMsg = new Windows.ApplicationModel.Email.EmailMessage
            {
                Subject = p.k.GetLangString("emailSubject") // "Mój dynamizm charakteru";
            };

            // Win10.16xxx ma DateTime.Now.ToLongDateString()
            // sTxt = "Załączam rezultat dzisiejszego testu dynamizmu charakteru\n\nData: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n\n" + sTxt;
            sTxt = p.k.GetLangString("emailBodyStart") + "\n\nData: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n\n" + sTxt;

            oMsg.Body = sTxt;

            // załączniki działają tylko w default windows mail app
            // Dim oStream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(oFile)
            // Dim oAttch = New Email.EmailAttachment("rezultat.txt", oStream)
            // oMsg.Attachments.Add(oAttch)

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(oMsg);

#if false
            // https://docs.microsoft.com/en-us/xamarin/essentials/email?tabs=android
            try
            {
                Xamarin.Essentials.EmailMessage message = new Xamarin.Essentials.EmailMessage
                {
                    Subject = p.k.GetLangString("emailSubject"),
                    Body = sTxt
                };
                await Xamarin.Essentials.Email.ComposeAsync(message);
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
#endif
        }

        private void PokazWynik()
        {
            cbEgzoDyn.Visibility = Visibility.Collapsed;
            cbEgzoStat.Visibility = Visibility.Collapsed;
            cbStatyk.Visibility = Visibility.Collapsed;
            cbEndoStat.Visibility = Visibility.Collapsed;
            cbEndoDyn.Visibility = Visibility.Collapsed;

            tbEgzoStat.Text = "";
            tbStatyk.Text = "";
            tbEndoStat.Text = "";
            tbEndoDyn.Text = "";

            var iSumy = new int[7];
            int iNonZero = 0;
            string sTxt = "";

            for (int i = 1; i <= 35; i++)
            {
                iSumy[aiOdpowiedzi[i]] = iSumy[aiOdpowiedzi[i]] + 1;
                if (aiOdpowiedzi[i] == 0)
                    sTxt = "    " + p.k.GetLangString("msgMaloOdpowiedzi"); //  Nie odpowiedziałeś na wszystkie pytania, a więc wyniki są niewiarygodne.";
                else
                    iNonZero++;
            }

            int iMax = 0;
            int iTyp = 0;
            bool bOk = false;
            for (int i = 1; i <= 5; i++)
            {
                if (iSumy[i] > iNonZero * 0.4)
                    bOk = true;
                if (iSumy[i] > iMax)
                {
                    iMax = iSumy[i];
                    iTyp = i;
                }
            }

            if (!bOk)
                sTxt = sTxt + "   " + p.k.GetLangString("msgDuzyRozrzut"); // Za duży rozrzut odpowiedzi.";

            tbEgzoDyn.Text = sTxt;

            if(iTyp == 0)
            {
                tbTeza.Text = p.k.GetLangString("msgNoResult"); // "Brak wyniku!";
            }
            else
            {
                tbTeza.Text = p.k.GetLangString("wynik00") + " " + Numer2Dynamizm(iTyp);
                tbEgzoStat.Text = "    " + p.k.GetLangString("wynik" + iTyp.ToString() + "l1");
                tbStatyk.Text = "    " + p.k.GetLangString("wynik" + iTyp.ToString() + "l2");
                tbEndoStat.Text = "    " + p.k.GetLangString("wynik" + iTyp.ToString() + "l3");
            }

            // bDalej.Content = " "
            // bDalej.IsEnabled = False
            bDalej.Visibility = Visibility.Collapsed;
            uiProgress.Visibility = Visibility.Collapsed;
            bWstecz.Visibility = Visibility.Collapsed;

            ZapiszWynik();
        }


        private void uiAnswer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RadioButton oSender = sender as RadioButton;
            if (oSender.IsChecked.HasValue && oSender.IsChecked.Value)
                bDalej_Click(sender, e);
            else
                oSender.IsChecked = true;
        }

        private void uiKomparator_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(KomparatorBrowse));
        }
    }

}
