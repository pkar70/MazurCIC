using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using VBlib;
using vb14 = VBlib.pkarlibmodule14;

using pkar.UI.Extensions; // for Show() and Hide() extensions on UI elements


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MazurCiC
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private VBlib.MainPage inVb = new VBlib.MainPage();
        //private bool _LangPl = false;

        // tu zmiana wywo³ania by³a
        private void Strona_Loaded(object sender, RoutedEventArgs e)
        {
            inVb.ResetOdpowiedzi();

            // bo w Uno not implemented
            // musi byc wczesniej, bo inaczej pierwsza strona jest jeszcze po EN
            //#if NETFX_CORE
            //            if(p.k.IsThisMoje()) Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pl";
            //#endif 


            tbTeza.Text = pkar.Localize.GetResManString("msgTeza"); // "SprawdŸ dynamizm swojego charakteru";
            tbEgzoDyn.Text = "  " + pkar.Localize.GetResManString("msgEgzoDyn"); // "  Charakter, a œciœlej dynamizm charakteru, ka¿dej osoby zmienia siê wraz z wiekiem: zaczynaj¹c od egzodynamizmu, poprzez statyzm do endodynamizmu.";
            tbEgzoStat.Text = "  " + pkar.Localize.GetResManString("msgEgzoStat"); // "  Niniejsza aplikacja u³atwia samo-okreœlenie aktualnego dynamizmu - podczas udzielania odpowiedzi na kolejne pytania w górnej czêœci ekranu bêdzie konstruowany wykres odpowiadaj¹cy dynamizmowi z tych odpowiedzi wynikaj¹cemu.";
            tbStatyk.Text = "  " + pkar.Localize.GetResManString("msgStatyk"); // "  Znaj¹c dynamizmy dwojga osób mo¿na z du¿ym prawdopodobieñstwem oszacowaæ jak bêdzie wygl¹da³ ich zwi¹zek, czy ma³¿eñstwo bêdzie szczêœliwe czy te¿ siê rozpadnie.";
            tbEndoStat.Text = "  " + pkar.Localize.GetResManString("msgEndoStat"); // "  Opracowane na podstawie ksi¹¿ki polskiego cybernetyka, prof. Mariana Mazura pt. \"Cybernetyka i charakter\", tabela 15.2. Ksi¹¿ka dostêpna jest w postaci elektronicznej na stronach autonom.edu.pl .";
                
            tbRemark.FontSize = 0.85 * tbTeza.FontSize; // niby moglbym narzucic w XAML, ale tak jest elastyczniej :)

            //string sTmp = " ";
            //if (!p.k.GetPlatform("uwp")) sTmp = 
            tbEndoDyn.Text = "  " + pkar.Localize.GetResManString("msgEndoDyn");

            // powtorne uruchomienie to czasem powrot do strony glownej - wiêc przywróæ guziczki, ¿eby mo¿na by³o coœ zrobiæ
            bDalej.Show(); // .Visibility = Visibility.Visible;
            bWstecz.Show();
            bWstecz.IsEnabled = false;
            bDalej.Content = pkar.Localize.GetResManString("msgDalejDalej"); // "Dalej>";

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
                uiMenu.Show(false); // .Visibility = Visibility.Visible;
#endif
                PokazWynik();
            }
            else
            {
#if NETFX_CORE
                uiMenu.Show(false); //.Visibility = Visibility.Collapsed;
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
                bDalej.Content = pkar.Localize.GetResManString("msgDalejWynik"); //"Wynik";
            else
                bDalej.Content = pkar.Localize.GetResManString("msgDalejDalej"); // "Dalej>";

            if (iNowe == 1)
                bWstecz.IsEnabled = false;
            else
                bWstecz.IsEnabled = true;

            cbEgzoDyn.Show();
            cbEgzoStat.Show();
            cbStatyk.Show();
            cbEndoStat.Show();
            cbEndoDyn.Show();

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
            if (sTmp != sStringName + "r") tbRemark.Text = sTmp.Trim();
            tbEgzoDyn.Text = LocalGetLangString(sStringName + "a1").Trim();
            tbEgzoStat.Text = LocalGetLangString(sStringName + "a2").Trim();
            tbStatyk.Text = LocalGetLangString(sStringName + "a3").Trim();
            tbEndoStat.Text = LocalGetLangString(sStringName + "a4").Trim();
            tbEndoDyn.Text = LocalGetLangString(sStringName + "a5").Trim();

            tbRemark.Show(tbRemark.Text.Length > 1);

        }

        private string LocalGetLangString(string sMsg)
        {
            if (sMsg == "")
                return "";

            try
            {
                // force POLISH (ale tylko pytañ/odpowiedzi!) na moim komputerze
                if (vb14.GetSettingsString("COMPUTERNAME").Contains("PKAR"))
                {
                    string retVal;
                    // switch to PL
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pl";
                    retVal = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView().GetString(sMsg);
                    // switch to EN
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en";
                }

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

            // grEgzoDyn.Height = new GridLength(iSumy[1]);
            grEgzoDyn.Wysokosc = iSumy[1];
            grEgzoStat.Wysokosc = iSumy[2];
            grStatyk.Wysokosc = iSumy[3];
            grEndoStat.Wysokosc = iSumy[4];
            grEndoDyn.Wysokosc = iSumy[5];
        }

        private async System.Threading.Tasks.Task ZapiszWynikAsync()
        {

            string sTxt = inVb.ZapiszWynik(Windows.Storage.ApplicationData.Current.LocalFolder.Path, tbTeza.Text);

#if WINDOWS
            // if (p.k.GetPlatform("uwp"))
            { // na Android - wysy³amy bez pytania, bo nie ma pokazywania zapamiêtanych rezultatów
                if (!await vb14.DialogBoxResYNAsync("askWantSend")) // Czy chcesz wys³aæ rezultat?"))
                    return;
            }
#endif
            Windows.ApplicationModel.Email.EmailMessage oMsg = new Windows.ApplicationModel.Email.EmailMessage
            {
                Subject = pkar.Localize.GetResManString("emailSubject") // "Mój dynamizm charakteru";
            };

            // Win10.16xxx ma DateTime.Now.ToLongDateString()
            // sTxt = "Za³¹czam rezultat dzisiejszego testu dynamizmu charakteru\n\nData: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n\n" + sTxt;
            sTxt = pkar.Localize.GetResManString("emailBodyStart") + "\n\nData: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n\n" + sTxt;

            oMsg.Body = sTxt;

            // za³¹czniki dzia³aj¹ tylko w default windows mail app
            // Dim oStream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(oFile)
            // Dim oAttch = New Email.EmailAttachment("rezultat.txt", oStream)
            // oMsg.Attachments.Add(oAttch)

            await Windows.ApplicationModel.Email.EmailManager.ShowComposeNewEmailAsync(oMsg);

        }

        private void PokazWynik()
        {
            cbEgzoDyn.Show(false);// .Visibility = Visibility.Collapsed;
            cbEgzoStat.Show(false);
            cbStatyk.Show(false);
            cbEndoStat.Show(false);
            cbEndoDyn.Show(false);

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
                    sTxt = "    " + pkar.Localize.GetResManString("msgMaloOdpowiedzi"); //  Nie odpowiedzia³eœ na wszystkie pytania, a wiêc wyniki s¹ niewiarygodne.";
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
                sTxt = sTxt + "   " + pkar.Localize.GetResManString("msgDuzyRozrzut"); // Za du¿y rozrzut odpowiedzi.";

            tbEgzoDyn.Text = sTxt;

            if (iTyp == 0)
            {
                tbTeza.Text = pkar.Localize.GetResManString("msgNoResult"); // "Brak wyniku!";
            }
            else
            {
                tbTeza.Text = pkar.Localize.GetResManString("wynik00") + " " + inVb.Numer2Dynamizm(iTyp);
                tbEgzoStat.Text = "    " + pkar.Localize.GetResManString("wynik" + iTyp.ToString() + "l1");
                tbStatyk.Text = "    " + pkar.Localize.GetResManString("wynik" + iTyp.ToString() + "l2");
                tbEndoStat.Text = "    " + pkar.Localize.GetResManString("wynik" + iTyp.ToString() + "l3");
            }

            // bDalej.Content = " "
            // bDalej.IsEnabled = False
            bDalej.Show(false);
            uiProgress.Show(false);
            bWstecz.Show(false);

            ZapiszWynikAsync();
        }


        private void uiAnswer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            RadioButton oSender = sender as RadioButton;
            if (oSender.IsChecked.HasValue && oSender.IsChecked.Value)
                bDalej_Click(null, null);
            else
                oSender.IsChecked = true;
        }
        private void uiAnswerText_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement oSender = sender as FrameworkElement;
            switch (oSender.Name)
            {
                case "tbEgzoDyn":
                    cbEgzoDyn.IsChecked = true;
                    break;
                case "tbEgzoStat":
                    cbEgzoStat.IsChecked = true;
                    break;
                case "tbStatyk":
                    cbStatyk.IsChecked = true;
                    break;
                case "tbEndoStat":
                    cbEndoStat.IsChecked = true;
                    break;
                case "tbEndoDyn":
                    cbEndoDyn.IsChecked = true;
                    break;
                default:
                    return; // coœ nie tak, nie wiem co w³¹czyæ

            }

            bDalej_Click(null, null);
        }

        private void uiKomparator_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate(typeof(KomparatorBrowse));
        }
    }




}
