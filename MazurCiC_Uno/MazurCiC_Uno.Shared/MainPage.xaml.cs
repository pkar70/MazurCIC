

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

using vb14 = VBlib.pkarlibmodule14;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MazurCiC
{


    public sealed partial class MainPage : Page
    {
        private VBlib.MainPage inVb = new VBlib.MainPage();

        public MainPage()
        {
            this.InitializeComponent();
        }

        //private int iPytanie = 0;
        //private int[] aiOdpowiedzi = new int[41];

        private void Strona_Loaded(object sender, RoutedEventArgs e)
        {
            inVb.ResetOdpowiedzi();

            //iPytanie = 0;
            //for (int i = 1; i <= 35; i++)
            //    aiOdpowiedzi[i] = 0;

            // bo w Uno not implemented
            // musi byc wczesniej, bo inaczej pierwsza strona jest jeszcze po EN
            //#if NETFX_CORE
            //            if(p.k.IsThisMoje()) Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pl";
            //#endif 


            tbTeza.Text = vb14.GetLangString("msgTeza"); // "Sprawdź dynamizm swojego charakteru";
            tbEgzoDyn.Text = "  " + vb14.GetLangString("msgEgzoDyn"); // "  Charakter, a ściślej dynamizm charakteru, każdej osoby zmienia się wraz z wiekiem: zaczynając od egzodynamizmu, poprzez statyzm do endodynamizmu.";
            tbEgzoStat.Text = "  " + vb14.GetLangString("msgEgzoStat"); // "  Niniejsza aplikacja ułatwia samo-określenie aktualnego dynamizmu - podczas udzielania odpowiedzi na kolejne pytania w górnej części ekranu będzie konstruowany wykres odpowiadający dynamizmowi z tych odpowiedzi wynikającemu.";
            tbStatyk.Text = "  " + vb14.GetLangString("msgStatyk"); // "  Znając dynamizmy dwojga osób można z dużym prawdopodobieństwem oszacować jak będzie wyglądał ich związek, czy małżeństwo będzie szczęśliwe czy też się rozpadnie.";
            tbEndoStat.Text = "  " + vb14.GetLangString("msgEndoStat"); // "  Opracowane na podstawie książki polskiego cybernetyka, prof. Mariana Mazura pt. \"Cybernetyka i charakter\", tabela 15.2. Książka dostępna jest w postaci elektronicznej na stronach autonom.edu.pl .";

            tbRemark.FontSize = 0.85 * tbTeza.FontSize; // niby moglbym narzucic w XAML, ale tak jest elastyczniej :)

            string sTmp = " ";
            if(!p.k.GetPlatform("uwp")) sTmp = "  " + vb14.GetLangString("msgEndoDyn");
            tbEndoDyn.Text = sTmp;

            // powtorne uruchomienie to czasem powrot do strony glownej - więc przywróć guziczki, żeby można było coś zrobić
            bDalej.Visibility = Visibility.Visible;
            bWstecz.Visibility = Visibility.Visible;
            bWstecz.IsEnabled = false;
            bDalej.Content = vb14.GetLangString("msgDalejDalej"); // "Dalej>";

            // do sprawdzenia ktora szerokosc jest podana;
            // jesli <1000, to gdy width<height propozycja obrocenia ekranu
            if (uiGrid.ActualWidth < 800)
            {
                if (uiGrid.ActualWidth < uiGrid.ActualHeight)
                    vb14.DialogBoxRes("msgBetterLandscape");
            }

        }


        private void bDalej_Click(object sender, RoutedEventArgs e)
        {
            if (inVb.iPytanie > 35)
                return;

            ZabierzOdpowiedz();

            if (inVb.iPytanie > 34)
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
                ZmianaPytania(inVb.iPytanie + 1);
                uiProgress.Value = inVb.iPytanie;
            }
        }
        private void bWstecz_Click(object sender, RoutedEventArgs e)
        {
            if (inVb.iPytanie < 2)
                return;

            ZabierzOdpowiedz();
            ZmianaPytania(inVb.iPytanie - 1);
            uiProgress.Value = inVb.iPytanie;
        }
        private void ZmianaPytania(int iNowe)
        {
            if (iNowe == 35)
                bDalej.Content = vb14.GetLangString("msgDalejWynik"); //"Wynik";
            else
                bDalej.Content = vb14.GetLangString("msgDalejDalej"); // "Dalej>";

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


            inVb.iPytanie = iNowe;

            string sStringName = "/tezy/" + "t" + iNowe.ToString();

            string sTmp = LocalGetLangString(sStringName + "t");
            int iInd;

            iInd = sTmp.IndexOf("). ");
            if (iInd > 0) sTmp = sTmp.Substring(iInd + 3);
            tbTeza.Text = sTmp.Trim();
            sTmp = LocalGetLangString(sStringName + "r").Trim();
            if(sTmp != sStringName + "r") tbRemark.Text = sTmp.Trim();
            tbEgzoDyn.Text = LocalGetLangString(sStringName + "a1").Trim();
            tbEgzoStat.Text = LocalGetLangString(sStringName + "a2").Trim();
            tbStatyk.Text = LocalGetLangString(sStringName + "a3").Trim();
            tbEndoStat.Text = LocalGetLangString(sStringName + "a4").Trim();
            tbEndoDyn.Text = LocalGetLangString(sStringName + "a5").Trim();

            if (tbRemark.Text.Length < 2)
                tbRemark.Visibility = Visibility.Collapsed;
            else
                tbRemark.Visibility = Visibility.Visible;

        }

        private static string LocalGetLangString(string sMsg)
        {
            if (sMsg == "")
                return "";

            try
            {
                return Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString(sMsg);
            }
            catch
            {
            }
            return sMsg;
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

            inVb.aiOdpowiedzi[inVb.iPytanie] = iOdp;

            var iSumy = new int[7];
            for (int i = 1; i <= 35; i++)
                iSumy[inVb.aiOdpowiedzi[i]] = iSumy[inVb.aiOdpowiedzi[i]] + 1;

            grEgzoDyn.Height = new GridLength(iSumy[1]);
            grEgzoStat.Height = new GridLength(iSumy[2]);
            grStatyk.Height = new GridLength(iSumy[3]);
            grEndoStat.Height = new GridLength(iSumy[4]);
            grEndoDyn.Height = new GridLength(iSumy[5]);
        }

        //private string Numer2Dynamizm(int iNo)
        //{
            //switch (iNo)
            //{
            //    case 1:
            //        return vb14.GetLangString("typEgzodynamik"); // "egzodynamik";
            //    case 2:
            //            return vb14.GetLangString("typEgzostatyk"); //"egzostatyk";
            //    case 3:
            //            return vb14.GetLangString("typStatyk"); //"statyk";
            //    case 4:
            //            return vb14.GetLangString("typEndostatyk"); //"endostatyk";
            //    case 5:
            //            return vb14.GetLangString("typEndodynamik"); //"endodynamik";
            //    default:
            //            return "ERROR";
        //    }

        //    // wersje EN: exodynamic, exostatic, static, endostatic, endodynamic
        //}
        //private int PoliczIleDynamizmu(int iNo)
        //{
        //    int iRet = 0;
        //    for (int i = 1; i <= 35; i++)
        //    {
        //        if (aiOdpowiedzi[i] == iNo)
        //            iRet += 1;
        //    }
        //    return iRet;
        //}
        //private double SredniDynamizm()
        //{
        //    double iSuma = 0;
        //    int iCnt = 0;

        //    for (int i = 1; i <= 35; i++)
        //    {
        //        if (aiOdpowiedzi[i] > 0)
        //            iCnt += 1;
        //        iSuma += aiOdpowiedzi[i];
        //    }

        //    return iSuma / iCnt;
        //}
        //private double SredniDynamizmBezSmieci()
        //{
        //    var aDynamizm = new int[7];

        //    for (int i = 1; i <= 5; i++)
        //        aDynamizm[i] = 0;

        //    for (int i = 1; i <= 35; i++)
        //        aDynamizm[aiOdpowiedzi[i]] += 1;

        //    // uciecie smieci od dołu i od góry
        //    for (int i = 1; i <= 5; i++)
        //    {
        //        if (aDynamizm[i] > 4)
        //            break;
        //        aDynamizm[i] = 0;
        //    }

        //    for (int i = 5; i >= 1; i += -1)
        //    {
        //        if (aDynamizm[i] > 4)
        //            break;
        //        aDynamizm[i] = 0;
        //    }

        //    int iCnt = 0;
        //    double iSuma = 0;
        //    for (int i = 1; i <= 5; i++)
        //    {
        //        iCnt += aDynamizm[i];
        //        iSuma += i * aDynamizm[i];
        //    }

        //    return iSuma / iCnt;
        //}
        private async System.Threading.Tasks.Task ZapiszWynik()
        {

            // string sTxt = "";

            string sTxt = inVb.ZapiszWynik(Windows.Storage.ApplicationData.Current.LocalFolder.Path, tbTeza.Text);

            //sTxt = tbTeza.Text + " (" + SredniDynamizm().ToString("f2") + "/" + SredniDynamizmBezSmieci().ToString("f2") + ")\n\n";

            //sTxt = sTxt + vb14.GetLangString("msgRozkladOdpowiedzi") + "\n"; // "Rozklad odpowiedzi:\n";
            //for (int i = 1; i <= 5; i++)
            //    sTxt = sTxt + Numer2Dynamizm(i) + ":\t" + PoliczIleDynamizmu(i).ToString() + "\n";

            //string sPoszczOdp = vb14.GetLangString("msgPoszczOdpowiedzi"); // Poszczególne odpowiedzi:\n";
            //sTxt = sTxt + "\n" + sPoszczOdp + "\n";
            ////if (sPoszczOdp != "Poszczególne odpowiedzi:")
            ////    sTxt += "\nPoszczególne odpowiedzi:\n";   // wersja polska byc musi, bo porównywarka tego szuka

            //for (int i = 1; i <= 35; i++)
            //    sTxt = sTxt + i.ToString() + ": " + Numer2Dynamizm(aiOdpowiedzi[i]) + "\n";

            //// zapisuje plik, mimo że Android tego nie wykorzystuje w ogóle
            //Windows.Storage.StorageFolder oFold = Windows.Storage.ApplicationData.Current.LocalFolder;
            //string sFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss");

            //// plik indeksowy (omijanie niedziałania oFold.Files()
            //Windows.Storage.StorageFile oFile;
            //oFile = await oFold.CreateFileAsync("lista.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
            //await Windows.Storage.FileIO.AppendTextAsync(oFile, sFileName + "\n", Windows.Storage.Streams.UnicodeEncoding.Utf8);

            //// plik z danymi

            //oFile = await oFold.CreateFileAsync(sFileName + ".txt");

            //await Windows.Storage.FileIO.AppendTextAsync(oFile, sTxt, Windows.Storage.Streams.UnicodeEncoding.Utf8);

            if (p.k.GetPlatform("uwp"))
            { // na Android - wysyłamy bez pytania, bo nie ma pokazywania zapamiętanych rezultatów
                if (!await vb14.DialogBoxResYNAsync("askWantSend")) // Czy chcesz wysłać rezultat?"))
                    return;
            }

            Windows.ApplicationModel.Email.EmailMessage oMsg = new Windows.ApplicationModel.Email.EmailMessage
            {
                Subject = vb14.GetLangString("emailSubject") // "Mój dynamizm charakteru";
            };

            // Win10.16xxx ma DateTime.Now.ToLongDateString()
            // sTxt = "Załączam rezultat dzisiejszego testu dynamizmu charakteru\n\nData: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n\n" + sTxt;
            sTxt = vb14.GetLangString("emailBodyStart") + "\n\nData: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n\n" + sTxt;

            oMsg.Body = sTxt;

            // załączniki działają tylko w default windows mail app
            // Dim oStream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(oFile)
            // Dim oAttch = New Email.EmailAttachment("rezultat.txt", oStream)
            // oMsg.Attachments.Add(oAttch)

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(oMsg);

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
                iSumy[inVb.aiOdpowiedzi[i]] = iSumy[inVb.aiOdpowiedzi[i]] + 1;
                if (inVb.aiOdpowiedzi[i] == 0)
                    sTxt = "    " + vb14.GetLangString("msgMaloOdpowiedzi"); //  Nie odpowiedziałeś na wszystkie pytania, a więc wyniki są niewiarygodne.";
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
                sTxt = sTxt + "   " + vb14.GetLangString("msgDuzyRozrzut"); // Za duży rozrzut odpowiedzi.";

            tbEgzoDyn.Text = sTxt;

            if(iTyp == 0)
            {
                tbTeza.Text = vb14.GetLangString("msgNoResult"); // "Brak wyniku!";
            }
            else
            {
                tbTeza.Text = vb14.GetLangString("wynik00") + " " + inVb.Numer2Dynamizm(iTyp);
                tbEgzoStat.Text = "    " + vb14.GetLangString("wynik" + iTyp.ToString() + "l1");
                tbStatyk.Text = "    " + vb14.GetLangString("wynik" + iTyp.ToString() + "l2");
                tbEndoStat.Text = "    " + vb14.GetLangString("wynik" + iTyp.ToString() + "l3");
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
            this.Navigate(typeof(KomparatorBrowse));
        }
    }

}
