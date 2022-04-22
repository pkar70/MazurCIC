using System;
using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices.WindowsRuntime;
//using Windows.Foundation;
//using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Data;
//using Windows.UI.Xaml.Input;
//using Windows.UI.Xaml.Media;
//using Windows.UI.Xaml.Navigation;

using vb14 = VBlib.pkarlibmodule14;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MazurCiC
{
    public sealed partial class KomparatorBrowse : Page
    {
        public KomparatorBrowse()
        {
            this.InitializeComponent();
        }

        private void uiPorownaj_Click(object sender, RoutedEventArgs e)
        {

            string sPorownanie = VBlib.KomparatorBrowse.Porownaj(uiText1.Text, uiText2.Text);
            if (sPorownanie == "")
            {
                vb14.DialogBox(VBlib.KomparatorBrowse.sLastError);
                return;
            }
            // vb14.SetSettingsString("losyRelacjiParam", sPorownanie);
            this.Frame.Navigate(typeof(LosyRelacji), sPorownanie);
        }

        private void WczytajCombo(ComboBox oCombo, string sFileName)
        {
            oCombo.Items.Clear();
            sFileName = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sFileName);
            if (!System.IO.File.Exists(sFileName))
                return;

            IList<string> oLns = System.IO.File.ReadAllLines(sFileName);
            foreach (string oLine in oLns)
                oCombo.Items.Add(oLine);
        }

        private void uiPage_Loaded(object sender, RoutedEventArgs e)
        {
            // wypelnij combobox
            WczytajCombo(uiCombo1, "lista.txt");
            if (p.k.IsThisMoje())
            {
                WczytajCombo(uiCombo2, "listaInni.txt");
                uiCombo2.Visibility = Visibility.Visible;
            }
        }


        private void WczytajDane(TextBox oTBox, string sFileName)
        {
            sFileName = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sFileName);
            if (!System.IO.File.Exists(sFileName))
                return;

            string sTxt = System.IO.File.ReadAllText (sFileName);

            oTBox.Text = sTxt;
        }

        private void uiCombo1_Changed(object sender, SelectionChangedEventArgs e)
        {
            // zamien combobox na textbox

            WczytajDane(uiText1, uiCombo1.SelectedValue + ".txt");
        }

        private void uiCombo2_Changed(object sender, SelectionChangedEventArgs e)
        {
            string sTmp = uiCombo2.SelectedValue.ToString();
            if (!sTmp.ToLower().EndsWith(".txt")) sTmp += ".txt";
            WczytajDane(uiText2, sTmp);
        }
    }
}
