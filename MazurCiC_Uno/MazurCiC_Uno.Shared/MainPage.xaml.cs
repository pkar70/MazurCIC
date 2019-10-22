
/*


* migracja do C#/Uno
* migracja do pkarmodule (namespace/class p.k.)
* przeniesienie stringów z MainPage.XAML/.cs do Strings\PL (nie z innych stron, bo inne i tak tylko pod Windows)
* przeniesienie stringów z tekstyPytan.txt do tezy.resw (bo Android nie umie wstawić pliku do appx), a poza tym ułatwienie w translacji późniejszej
* dodanie komentarza do pytań (niektórych) - było w pliku, ale nie było pokazywane w ogóle.
* dla nie UWP pokazuje info na początku - że wersja Windows jest bogatsza
* UWP/IsThisMoje - wymuszenie PL, nawet jak start nie jest w PL [w UWP, bo w Uno to nie dziala]
* przetłumaczenie pytań na angielski (głównie googletranslator)
* teksty z KomparatorBrowse do strings.resw
* teksty z LosyRelacji do strings.resw [ale nie same relacje!]
* przy cofaniu - cofa ProgressBar

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

            //if (!File.Exists(@"Assets\tekstyPytan.txt"))
            //{
            //    p.k.DialogBoxResError(10, "err10BezTwierdzen"); // "Nie widzę pliku z treścią, a przecież był w Install!");
            //}

            tbTeza.Text = p.k.GetLangString("msgTeza"); // "Sprawdź dynamizm swojego charakteru";
            tbEgzoDyn.Text = "  " + p.k.GetLangString("msgEgzoDyn"); // "  Charakter, a ściślej dynamizm charakteru, każdej osoby zmienia się wraz z wiekiem: zaczynając od egzodynamizmu, poprzez statyzm do endodynamizmu.";
            tbEgzoStat.Text = "  " + p.k.GetLangString("msgEgzoStat"); // "  Niniejsza aplikacja ułatwia samo-określenie aktualnego dynamizmu - podczas udzielania odpowiedzi na kolejne pytania w górnej części ekranu będzie konstruowany wykres odpowiadający dynamizmowi z tych odpowiedzi wynikającemu.";
            tbStatyk.Text = "  " + p.k.GetLangString("msgStatyk"); // "  Znając dynamizmy dwojga osób można z dużym prawdopodobieństwem oszacować jak będzie wyglądał ich związek, czy małżeństwo będzie szczęśliwe czy też się rozpadnie.";
            tbEndoStat.Text = "  " + p.k.GetLangString("msgEndoStat"); // "  Opracowane na podstawie książki polskiego cybernetyka, prof. Mariana Mazura pt. \"Cybernetyka i charakter\", tabela 15.2. Książka dostępna jest w postaci elektronicznej na stronach autonom.edu.pl .";

            tbRemark.FontSize = 0.85 * tbTeza.FontSize; // niby moglbym narzucic w XAML, ale tak jest elastyczniej :)

            string sTmp = " ";
            if(!p.k.GetPlatform("uwp")) sTmp = "  " + p.k.GetLangString("msgEndoDyn");
            tbEndoDyn.Text = sTmp;

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

            // próba, czy na Android nie trzeba pomijać prefixu pliku
            //string sStringName = "";
            //if (p.k.GetPlatform("uwp")) sStringName = "/tezy/";
            //sStringName = sStringName + "t" + iNowe.ToString();
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

            //StreamReader oFile;
            //string sSrch, sLine;

            //oFile = File.OpenText(@"Assets\tekstyPytan.txt");
            //sSrch = "Twierdzenie 15. " + iNowe.ToString() + " ";
            //sLine = "";

            //while (!oFile.EndOfStream)
            //{
            //    sLine = oFile.ReadLine();
            //    if (sLine.IndexOf(sSrch) == 0)
            //        break;
            //}

            //// Twierdzenie 15. 24 (o stosunku do panowania). Im mniejszy jest współczynnik dynamizmu, tym silniejsze jest dążenie do panowania.
            //int iInd;

            //iInd = sLine.IndexOf("). ");
            //tbTeza.Text = sLine.Substring(iInd + 3);

            //// C) Egzodynamik nie chce ...
            //// BC) Egzostatyk przeciwstawia ...
            //// B) Statyk uznaje ...
            //// AB) Endostatyk sprzyja ...
            //// A) Endodynamik pragnie ...
            //string sTmp;
            //sTmp = "";

            //// pomin wszystko do C) 

            //while (!oFile.EndOfStream)
            //{
            //    sLine = oFile.ReadLine();
            //    if (sLine.IndexOf("C) ") == 0)
            //        break;
            //}

            //sTmp = sLine.Substring(3);
            //while (!oFile.EndOfStream)
            //{
            //    sLine = oFile.ReadLine();
            //    if (sLine.IndexOf("BC) ") == 0)
            //        break;
            //    sTmp = sTmp + "\n" + sLine;
            //}

            //tbEgzoDyn.Text = sTmp;

            //sTmp = sLine.Substring(4);
            //while (!oFile.EndOfStream)
            //{
            //    sLine = oFile.ReadLine();
            //    if (sLine.IndexOf("B) ") == 0)
            //        break;
            //    sTmp = sTmp + "\n" + sLine;
            //}

            //tbEgzoStat.Text = sTmp;
            //// cbEgzoStat.Content = sTmp

            //sTmp = sLine.Substring(3);
            //while (!oFile.EndOfStream)
            //{
            //    sLine = oFile.ReadLine();
            //    if (sLine.IndexOf("AB) ") == 0)
            //        break;
            //    sTmp = sTmp + "\n" + sLine;
            //}

            //tbStatyk.Text = sTmp;

            //sTmp = sLine.Substring(4);
            //while (!oFile.EndOfStream)
            //{
            //    sLine = oFile.ReadLine();
            //    if (sLine.IndexOf("A) ") == 0)
            //        break;
            //    sTmp = sTmp + "\n" + sLine;
            //}

            //tbEndoStat.Text = sTmp;

            //sTmp = sLine.Substring(3);
            //while (!oFile.EndOfStream)
            //{
            //    sLine = oFile.ReadLine();
            //    if (sLine.IndexOf("Twierdzenie 15") == 0)
            //        break;
            //    sTmp = sTmp + "\n" + sLine;
            //}

            //tbEndoDyn.Text = sTmp.Trim();
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
            Windows.Storage.StorageFolder oFold = Windows.Storage.ApplicationData.Current.LocalFolder;

            string sTxt = "";

            sTxt = tbTeza.Text + " (" + SredniDynamizm().ToString("f2") + "/" + SredniDynamizmBezSmieci().ToString("f2") + ")\n\n";

            sTxt = sTxt + p.k.GetLangString("msgRozkladOdpowiedzi") + "\n"; // "Rozklad odpowiedzi:\n";
            for (int i = 1; i <= 5; i++)
                sTxt = sTxt + Numer2Dynamizm(i) + ":\t" + PoliczIleDynamizmu(i).ToString() + "\n";

            string sPoszczOdp = p.k.GetLangString("msgPoszczOdpowiedzi"); // Poszczególne odpowiedzi:\n";
            sTxt = sTxt + "\n" + sPoszczOdp + "\n";
            if (sPoszczOdp != "Poszczególne odpowiedzi:")
                sTxt += "\nPoszczególne odpowiedzi:\n";   // wersja polska byc musi, bo porównywarka tego szuka

            for (int i = 1; i <= 35; i++)
                sTxt = sTxt + i.ToString() + ": " + Numer2Dynamizm(aiOdpowiedzi[i]) + "\n";

#if NETFX_CORE
            // jako że nie ma Windows.Storage.FileIO w Uno, ani prawie w ogóle StorageFile...
            string sFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            Windows.Storage.StorageFile oFile = await oFold.CreateFileAsync("lista.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
            // to była próba ominięcia niemania FileIO, ale nawet CreateFile nie ma w Uno
            //Stream oStream = await oFile.OpenStreamForWriteAsync();
            //oStream.Seek(0, SeekOrigin.End);
            //oStream.WriteAsync();
            await Windows.Storage.FileIO.AppendTextAsync(oFile, sFileName + "\n");

            oFile = await oFold.CreateFileAsync(sFileName + ".txt");
            await Windows.Storage.FileIO.AppendTextAsync(oFile, sTxt);

            if (!await p.k.DialogBoxResYN("askWantSend")) // Czy chcesz wysłać rezultat?"))
                return;
#endif



#if NETFX_CORE
            Windows.ApplicationModel.Email.EmailMessage oMsg = new Windows.ApplicationModel.Email.EmailMessage();
            oMsg.Subject = p.k.GetLangString("emailSubject"); // "Mój dynamizm charakteru";

            // Win10.16xxx ma DateTime.Now.ToLongDateString()
            // sTxt = "Załączam rezultat dzisiejszego testu dynamizmu charakteru\n\nData: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n\n" + sTxt;
            sTxt = p.k.GetLangString("emailBodyStart") + "\n\nData: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n\n" + sTxt;

            oMsg.Body = sTxt;

            // załączniki działają tylko w default windows mail app
            // Dim oStream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(oFile)
            // Dim oAttch = New Email.EmailAttachment("rezultat.txt", oStream)
            // oMsg.Attachments.Add(oAttch)

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(oMsg);

#elif __ANDROID__
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

            //switch (iTyp)
            //{
            //    case 0:
            //            break;
            //    case 1:  // C
            //            tbEgzoStat.Text = "    Prawdopodobnie masz poniżej 16 lat.";
            //            tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) jesteś typowym Dzieckiem.";
            //            tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie endodynamik.";
            //            break;
            //    case 2: // BC
            //            tbEgzoStat.Text = "    Prawdopodobnie masz od 16 do 35 lat.";
            //            tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) jesteś Dzieckiem z elementami Dorosłego.";
            //            tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie endostatyk.";
            //            break;
            //    case 3:  // B
            //            tbEgzoStat.Text = "    Prawdopodobnie masz od 36 do 60 lat.";
            //            tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) stanowisz wzorcowy przykład Dorosłego.";
            //            tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie inny statyk.";
            //            break;
            //    case 4:  // AB
            //            tbEgzoStat.Text = "    Prawdopodobnie masz powyżej 60 lat.";
            //            tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) jesteś Dorosłym z elementami Rodzica.";
            //            tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie egzostatyk.";
            //            break;
            //    case 5:  // A
            //            tbEgzoStat.Text = "    Najwyraźniej masz tzw. \"charakter przyspieszony\"...";
            //            tbStatyk.Text = "    W terminologii Erica Berne (analizie transakcyjnej) stanowisz wzorcowy przykład Rodzica.";
            //            tbEndoStat.Text = "    Najlepszym partnerem w miłości będzie dla Ciebie egzodynamik.";
            //            break;
            //}


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
