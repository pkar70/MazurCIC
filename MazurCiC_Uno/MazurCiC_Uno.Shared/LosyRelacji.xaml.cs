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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MazurCiC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LosyRelacji : Page
    {
        public LosyRelacji()
        {
            this.InitializeComponent();
        }

        private int[] aDiffTyp = new int[21];
        private int miAdd = 0;

        private string WezOpis(int iTyp)
        {
            switch(iTyp)
            {
                case 0:
                    return p.k.GetLangString("/relacje/r0");
                case 1:
                case 2:
                    return p.k.GetLangString("/relacje/r1");
                case 3:
                case 4:
                    return p.k.GetLangString("/relacje/r3");
                case 5:
                    return p.k.GetLangString("/relacje/r5");
                case 6:
                case 7:
                    return p.k.GetLangString("/relacje/r6");
                case 8:
                case 9:
                    return p.k.GetLangString("/relacje/r8");
                case 10:
                case 11:
                    return p.k.GetLangString("/relacje/r10");
                case 12:
                    return p.k.GetLangString("/relacje/r12");
                case 13:
                case 14:
                    return p.k.GetLangString("/relacje/r13");

            }
            return "???";
        }


        //private string WezOpisPlikTXT(int iTyp)
        //{
        //    StreamReader oFile = File.OpenText(@"Assets\opisyRelacji.txt");

        //    string sSrch = "<h4>" + iTyp.ToString() + "</h4>";
        //    string sLine = "";

        //    while (!oFile.EndOfStream)
        //    {
        //        sLine = oFile.ReadLine();
        //        if (sLine.IndexOf(sSrch) == 0)
        //            break;
        //    }

        //    string sTmp;
        //    sTmp = "";

        //    while (!oFile.EndOfStream)
        //    {
        //        sLine = oFile.ReadLine();
        //        if (sLine.IndexOf("<h4>") == 0)
        //            break;
        //        sTmp = sTmp + "\n" + sLine;
        //    }

        //    return sTmp;
        //}

        private void PokazRelacje()
        {
            int iStep = (int)(miAdd / (double)20);
            int iTyp = GetMaxTyp();  // maximum różnic, pointer = 0..14

            // tabelka przejsc pomiedzy typami
            // aTypTyp(iTyp, iStep) = iNewTyp
            // {11,12,13,14,15, 22,23,24,25, 33,34,35, 44,45, 55}
            // 0  1  2  3  4   5  6  7  8   9 10 11  12 13  14
            // DLA LAT  0,20,40,+60,+80
            int[,] aTypTyp = new int[,] { 
            {
                0,
                5,
                9,
                12,
                14
            },
            {
                1,
                6,
                10,
                13,
                14
            },
            {
                2,
                7,
                11,
                13,
                14
            },
            {
                3,
                8,
                11,
                13,
                14
            },
            {
                4,
                8,
                11,
                13,
                14
            },
            {
                5,
                9,
                12,
                14,
                14
            },
            {
                6,
                10,
                13,
                14,
                14
            },
            {
                7,
                11,
                13,
                14,
                14
            },
            {
                8,
                11,
                13,
                14,
                14
            },
            {
                9,
                12,
                14,
                14,
                14
            },
            {
                10,
                13,
                14,
                14,
                14
            },
            {
                11,
                13,
                14,
                14,
                14
            },
            {
                12,
                14,
                14,
                14,
                14
            },
            {
                13,
                14,
                14,
                14,
                14
            },
            {
                14,
                14,
                14,
                14,
                14
            } // poczatkowy 55
        };

            iTyp = aTypTyp[iTyp, iStep];

            string sTxt;
            if (iTyp == 1 | iTyp == 3 | iTyp == 6 | iTyp == 8 | iTyp == 10 | iTyp == 13)
                sTxt = p.k.GetLangString("msgLosyBetween") + "\n\n" + WezOpis(iTyp - 1) + "\n\n" + p.k.GetLangString("msgLosyOraz") + "\n\n" + WezOpis(iTyp + 1);
            else
                sTxt = WezOpis(iTyp);

            uiOpis.Text = sTxt;

            // a teraz slupki
            // 
            // najpierw zerowanie wysokosci
            var aSlupki = new int[16];
            for (int i = 0; i <= 14; i++)
                aSlupki[i] = 0;

            // teraz dodanie wartosci przesunietych o dekady
            for (int i = 0; i <= 14; i++)
                aSlupki[aTypTyp[i, iStep]] = aSlupki[aTypTyp[i, iStep]] + aDiffTyp[i];

            // a teraz przepisanie tego do wielkosci - pewnie by mozna jakas petla...
            uiTyp00.Height = new GridLength(aSlupki[0]);
            uiTyp01.Height = new GridLength(aSlupki[1]);
            uiTyp02.Height = new GridLength(aSlupki[2]);
            uiTyp03.Height = new GridLength(aSlupki[3]);
            uiTyp04.Height = new GridLength(aSlupki[4]);
            uiTyp05.Height = new GridLength(aSlupki[5]);
            uiTyp06.Height = new GridLength(aSlupki[6]);
            uiTyp07.Height = new GridLength(aSlupki[7]);
            uiTyp08.Height = new GridLength(aSlupki[8]);
            uiTyp09.Height = new GridLength(aSlupki[9]);
            uiTyp10.Height = new GridLength(aSlupki[10]);
            uiTyp11.Height = new GridLength(aSlupki[11]);
            uiTyp12.Height = new GridLength(aSlupki[12]);
            uiTyp13.Height = new GridLength(aSlupki[13]);
            uiTyp14.Height = new GridLength(aSlupki[14]);
        }

        private void EnableDisablePlusMinus()
        {
            if (miAdd < 20)
                uiBMinus.IsEnabled = false;
            else
                uiBMinus.IsEnabled = true;

            if (miAdd > 60)
                uiBPlus.IsEnabled = false;
            else
                uiBPlus.IsEnabled = true;

            int iRok = DateTime.Now.Year + miAdd;
            uiBPlus.Content = (iRok + 20).ToString() + ">";
            uiBMinus.Content = "<" + (iRok - 20).ToString();

            if (miAdd == 0)
                uiNaRok.Text = p.k.GetLangString("msgLosyToday"); // "Stan na dzisiaj";
            else
                uiNaRok.Text = p.k.GetLangString("msgLosyPrognozaNa") + " " + iRok.ToString();
        }
        private void uiMinus_Click(object sender, RoutedEventArgs e)
        {
            if (miAdd > 19)
                miAdd -= 20;
            EnableDisablePlusMinus();
            PokazRelacje();
        }

        private void uiKoniec_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void uiPlus_Click(object sender, RoutedEventArgs e)
        {
            if (miAdd < 80)
                miAdd += 20;
            EnableDisablePlusMinus();
            PokazRelacje();
        }

        private int GetMaxTyp()
        {
            // sprawdzenie maksimum w tablicy roznic
            int iMaxCnt = 0;
            int iMaxPtr = 0;

            for (int i = 0; i <= 14; i++)
            {
                if (aDiffTyp[i] > iMaxCnt)
                {
                    iMaxCnt = aDiffTyp[i];
                    iMaxPtr = i;
                }
            }

            return iMaxPtr;
        }

        private void uiPage_Loaded(object sender, RoutedEventArgs e)
        {
            EnableDisablePlusMinus();

            PokazRelacje();

            if (p.k.GetLangString("_lang") != "PL")
                p.k.DialogBox("Texts are autotranslated by Google, so it can contain some errors");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Dim sTxt As String = e.Parameter.ToString
            string sTxt = p.k.GetSettingsString("losyRelacjiParam");

            var aArr = sTxt.Split(';');
            if (aArr.GetUpperBound(0) != 15)
                return;
            for (int i = 0; i <= 14; i++)
                int.TryParse(aArr[i], out aDiffTyp[i]);
                // aDiffTyp[i] = Conversions.ToInteger(aArr[i]);
        }

    }
}