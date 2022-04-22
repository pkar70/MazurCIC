using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using vb14 = VBlib.pkarlibmodule14;


namespace MazurCiC
{

    public sealed partial class LosyRelacji : Page 
    {
        private VBlib.LosyRelacji inVb = new VBlib.LosyRelacji();

        public LosyRelacji()
        {
            this.InitializeComponent();
        }

        //private int[] aDiffTyp = new int[21];
        //private int miAdd = 0;

        //private string WezOpis(int iTyp)
        //{
        //    switch(iTyp)
        //    {
        //        case 0:
        //            return vb14.GetLangString("/relacje/r0");
        //        case 1:
        //        case 2:
        //            return vb14.GetLangString("/relacje/r1");
        //        case 3:
        //        case 4:
        //            return vb14.GetLangString("/relacje/r3");
        //        case 5:
        //            return vb14.GetLangString("/relacje/r5");
        //        case 6:
        //        case 7:
        //            return vb14.GetLangString("/relacje/r6");
        //        case 8:
        //        case 9:
        //            return vb14.GetLangString("/relacje/r8");
        //        case 10:
        //        case 11:
        //            return vb14.GetLangString("/relacje/r10");
        //        case 12:
        //            return vb14.GetLangString("/relacje/r12");
        //        case 13:
        //        case 14:
        //            return vb14.GetLangString("/relacje/r13");

        //    }
        //    return "???";
        //}


        private void PokazRelacje()
        {
            //    int iStep = (int)(inVb.miAdd / (double)20);
            //    int iTyp = GetMaxTyp();  // maximum różnic, pointer = 0..14

            //    // tabelka przejsc pomiedzy typami
            //    // aTypTyp(iTyp, iStep) = iNewTyp
            //    // {11,12,13,14,15, 22,23,24,25, 33,34,35, 44,45, 55}
            //    // 0  1  2  3  4   5  6  7  8   9 10 11  12 13  14
            //    // DLA LAT  0,20,40,+60,+80
            //    int[,] aTypTyp = new int[,] { 
            //    {
            //        0,
            //        5,
            //        9,
            //        12,
            //        14
            //    },
            //    {
            //        1,
            //        6,
            //        10,
            //        13,
            //        14
            //    },
            //    {
            //        2,
            //        7,
            //        11,
            //        13,
            //        14
            //    },
            //    {
            //        3,
            //        8,
            //        11,
            //        13,
            //        14
            //    },
            //    {
            //        4,
            //        8,
            //        11,
            //        13,
            //        14
            //    },
            //    {
            //        5,
            //        9,
            //        12,
            //        14,
            //        14
            //    },
            //    {
            //        6,
            //        10,
            //        13,
            //        14,
            //        14
            //    },
            //    {
            //        7,
            //        11,
            //        13,
            //        14,
            //        14
            //    },
            //    {
            //        8,
            //        11,
            //        13,
            //        14,
            //        14
            //    },
            //    {
            //        9,
            //        12,
            //        14,
            //        14,
            //        14
            //    },
            //    {
            //        10,
            //        13,
            //        14,
            //        14,
            //        14
            //    },
            //    {
            //        11,
            //        13,
            //        14,
            //        14,
            //        14
            //    },
            //    {
            //        12,
            //        14,
            //        14,
            //        14,
            //        14
            //    },
            //    {
            //        13,
            //        14,
            //        14,
            //        14,
            //        14
            //    },
            //    {
            //        14,
            //        14,
            //        14,
            //        14,
            //        14
            //    } // poczatkowy 55
            //};

            //    iTyp = aTypTyp[iTyp, iStep];

            //    string sTxt;
            //    if (iTyp == 1 | iTyp == 3 | iTyp == 6 | iTyp == 8 | iTyp == 10 | iTyp == 13)
            //        sTxt = vb14.GetLangString("msgLosyBetween") + "\n\n" + WezOpis(iTyp - 1) + "\n\n" + vb14.GetLangString("msgLosyOraz") + "\n\n" + WezOpis(iTyp + 1);
            //    else
            //        sTxt = WezOpis(iTyp);

            // uiOpis.Text = sTxt;
            uiOpis.Text = inVb.PokazRelacjePart1();

            //// a teraz slupki
            //// 
            //// najpierw zerowanie wysokosci
            //var aSlupki = new int[16];
            //for (int i = 0; i <= 14; i++)
            //    aSlupki[i] = 0;

            //// teraz dodanie wartosci przesunietych o dekady
            //for (int i = 0; i <= 14; i++)
            //    aSlupki[aTypTyp[i, iStep]] = aSlupki[aTypTyp[i, iStep]] + aDiffTyp[i];

            var aSlupki = inVb.PokazRelacjePart2();

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
            uiBMinus.IsEnabled = (inVb.miAdd >= 20);
            //if (inVb.miAdd < 20)
            //    uiBMinus.IsEnabled = false;
            //else
            //    uiBMinus.IsEnabled = true;

            uiBPlus.IsEnabled = (inVb.miAdd <= 60); 
            //if (inVb.miAdd > 60)
            //    uiBPlus.IsEnabled = false;
            //else
            //    uiBPlus.IsEnabled = true;

            int iRok = DateTime.Now.Year + inVb.miAdd;
            uiBPlus.Content = (iRok + 20).ToString() + ">";
            uiBMinus.Content = "<" + (iRok - 20).ToString();

            if (inVb.miAdd == 0)
                uiNaRok.Text = vb14.GetLangString("msgLosyToday"); // "Stan na dzisiaj";
            else
                uiNaRok.Text = vb14.GetLangString("msgLosyPrognozaNa") + " " + iRok.ToString();
        }
        private void uiMinus_Click(object sender, RoutedEventArgs e)
        {
            if (inVb.miAdd > 19) inVb.miAdd -= 20;
            EnableDisablePlusMinus();
            PokazRelacje();
        }

        private void uiKoniec_Click(object sender, RoutedEventArgs e)
        {
            this.Navigate(typeof(MainPage));
        }

        private void uiPlus_Click(object sender, RoutedEventArgs e)
        {
            if (inVb.miAdd < 80)
                inVb.miAdd += 20;
            EnableDisablePlusMinus();
            PokazRelacje();
        }

        //private int GetMaxTyp()
        //{
        //    // sprawdzenie maksimum w tablicy roznic
        //    int iMaxCnt = 0;
        //    int iMaxPtr = 0;

        //    for (int i = 0; i <= 14; i++)
        //    {
        //        if (aDiffTyp[i] > iMaxCnt)
        //        {
        //            iMaxCnt = aDiffTyp[i];
        //            iMaxPtr = i;
        //        }
        //    }

        //    return iMaxPtr;
        //}

        private void uiPage_Loaded(object sender, RoutedEventArgs e)
        {
            EnableDisablePlusMinus();

            PokazRelacje();

            if (vb14.GetLangString("_lang") != "PL")
                vb14.DialogBox("Texts are autotranslated by Google, so it can contain some errors");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string sTxt = e.Parameter.ToString();
            // string sTxt = vb14.GetSettingsString("losyRelacjiParam");

            inVb.PartOnNavigatedTo(sTxt);

            //var aArr = sTxt.Split(';');
            //if (aArr.GetUpperBound(0) != 15)
            //    return;
            //for (int i = 0; i <= 14; i++)
            //    int.TryParse(aArr[i], out aDiffTyp[i]);
            //    // aDiffTyp[i] = Conversions.ToInteger(aArr[i]);
        }

    }
}