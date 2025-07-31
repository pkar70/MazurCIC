using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using vb14 = VBlib.pkarlibmodule14;
using pkar.UI.Extensions;

namespace MazurCiC
{

    public sealed partial class LosyRelacji : Page 
    {
        private VBlib.LosyRelacji inVb = new VBlib.LosyRelacji();

        public LosyRelacji()
        {
            this.InitializeComponent();
        }


        private void PokazRelacje()
        {
            uiOpis.Text = inVb.PokazRelacjePart1();

            var aSlupki = inVb.PokazRelacjePart2();

            // a teraz przepisanie tego do wielkosci - pewnie by mozna jakas petla...
            uiTyp00.Wysokosc = aSlupki[0];
            uiTyp01.Wysokosc = aSlupki[1];
            uiTyp02.Wysokosc = aSlupki[2];
            uiTyp03.Wysokosc = aSlupki[3];
            uiTyp04.Wysokosc = aSlupki[4];
            uiTyp05.Wysokosc = aSlupki[5];
            uiTyp06.Wysokosc = aSlupki[6];
            uiTyp07.Wysokosc = aSlupki[7];
            uiTyp08.Wysokosc = aSlupki[8];
            uiTyp09.Wysokosc = aSlupki[9];
            uiTyp10.Wysokosc = aSlupki[10];
            uiTyp11.Wysokosc = aSlupki[11];
            uiTyp12.Wysokosc = aSlupki[12];
            uiTyp13.Wysokosc = aSlupki[13];
            uiTyp14.Wysokosc = aSlupki[14];
        }

        private void EnableDisablePlusMinus()
        {
            uiBMinus.IsEnabled = (inVb.miAdd >= 20);
            uiBPlus.IsEnabled = (inVb.miAdd <= 60); 

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

        private void uiPage_Loaded(object sender, RoutedEventArgs e)
        {
            EnableDisablePlusMinus();

            PokazRelacje();

            if (!vb14.LangIsCurrent("pl"))
                this.MsgBox("Texts are autotranslated by Google, so it can contain some errors");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string sTxt = e.Parameter.ToString();
            // string sTxt = vb14.GetSettingsString("losyRelacjiParam");

            inVb.PartOnNavigatedTo(sTxt);

        }

    }
}