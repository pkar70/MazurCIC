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
    public sealed partial class KomparatorBrowse : Page
    {
        public KomparatorBrowse()
        {
            this.InitializeComponent();
        }
        private string WytnijSamePunkty(string sIn, string sMsg)
        {
            if (sIn.Length < 100)
            {
                p.k.DialogBox(p.k.GetLangString("errDataShort") + sMsg);
                return "";
            }

            sIn += "\n";

            int iInd=1;
            if (p.k.GetLangString("_lang") == "PL")
            {
                iInd = sIn.IndexOf("Poszczegolne odpowiedzi:");
                if (iInd < 5)
                    iInd = sIn.IndexOf("Poszczególne odpowiedzi:");
                // nie ma błędu - jak Polak dostanie z USA odpowiedzi, to nie bedzie tego tekstu
                // ale samo przeskoczenie iInd jest.
                //if (iInd < 5)
                //{
                //    p.k.DialogBox(p.k.GetLangString("errDataNoPoszczeg") + sMsg);
                //    return "";
                //}
                if (iInd < 5) iInd = 5; // jakby po poprzednim bylo -1...
            }

            string sOut = sIn.Substring(iInd);
            iInd = sOut.IndexOf("1: ");
            if (iInd < 2)
            {
                p.k.DialogBox(p.k.GetLangString("errDataNoAnswers") + sMsg + ")");
                return "";
            }
            return sOut.Substring(iInd);
        }

        private int Nazwa2Numer(string sTyp)
        {
            switch (sTyp.Trim().ToLower())
            {
                case "egzodynamik":
                case "exodynamic":
                        return 1;
                case "egzostatyk":
                case "exostatic":
                        return 2;
                case "statyk":
                case "static":
                        return 3;
                case "endostatyk":
                case "endostatic":
                        return 4;
                case "endodynamik":
                case "endodynamic":
                        return 5;
            }

            return 3;    // nie wiadomo co, error - ale przyjmijmy ze to srodek
        }

        private void uiPorownaj_Click(object sender, RoutedEventArgs e)
        {
            string sMsgTwoje = p.k.GetLangString("msgBrowseTwoje");
            string sMsgDrugi = p.k.GetLangString("msgBrowseDrugiej");


            string sTxt1 = WytnijSamePunkty(uiText1.Text, sMsgTwoje);
            string sTxt2 = WytnijSamePunkty(uiText2.Text, sMsgDrugi);
            if (string.IsNullOrEmpty(sTxt1))
                return;
            if (string.IsNullOrEmpty(sTxt2))
                return;

            // zerowanie tablicy roznic
            var aDiffsy = new int[61];
            for (int i = 1; i <= 59; i++)
                aDiffsy[i] = 0;

            // wypelnienie tablicy roznic
            int iInd;
            string sTmp;
            bool bError = false;
            int iTyp1, iTyp2;

            for (int i = 1; i <= 35; i++)
            {
                sTmp = i.ToString() + ": ";
                if (sTxt1.IndexOf(sTmp) != 0 | sTxt2.IndexOf(sTmp) != 0)
                {
                    p.k.DialogBox(p.k.GetLangString("errDataNoPoint") + " " + i.ToString());
                    bError = true;
                    break;
                }
                sTxt1 = sTxt1.Substring(sTmp.Length);
                sTxt2 = sTxt2.Substring(sTmp.Length);

                iInd = sTxt1.IndexOf("\r");
                if (iInd == -1)
                    iInd = sTxt1.IndexOf("\n");
                if (iInd < 2)
                {
                    p.k.DialogBox(p.k.GetLangString("errDataNoThisAnswer") + " " + i.ToString() + " - " + sMsgTwoje);
                    bError = true;
                    break;
                }
                iTyp1 = Nazwa2Numer(sTxt1.Substring(0, iInd));
                sTxt1 = sTxt1.Substring(iInd + 1);

                iInd = sTxt2.IndexOf("\r");
                if (iInd == -1)
                    iInd = sTxt2.IndexOf("\n");
                if (iInd < 2)
                {
                    p.k.DialogBox("Błąd: brak odpowiedzi " + i.ToString() + " - " + sMsgDrugi);
                    bError = true;
                    break;
                }
                iTyp2 = Nazwa2Numer(sTxt2.Substring(0, iInd));
                sTxt2 = sTxt2.Substring(iInd + 1);

                if (iTyp1 < iTyp2)
                    iInd = iTyp1 * 10 + iTyp2;
                else
                    iInd = iTyp2 * 10 + iTyp1;

                aDiffsy[iInd] += 1;
            }
            if (bError)
                return;

            sTmp = "";
            sTmp = sTmp + aDiffsy[11].ToString() + ";";
            sTmp = sTmp + aDiffsy[12].ToString() + ";";
            sTmp = sTmp + aDiffsy[13].ToString() + ";";
            sTmp = sTmp + aDiffsy[14].ToString() + ";";
            sTmp = sTmp + aDiffsy[15].ToString() + ";";

            sTmp = sTmp + aDiffsy[22].ToString() + ";";
            sTmp = sTmp + aDiffsy[23].ToString() + ";";
            sTmp = sTmp + aDiffsy[24].ToString() + ";";
            sTmp = sTmp + aDiffsy[25].ToString() + ";";

            sTmp = sTmp + aDiffsy[33].ToString() + ";";
            sTmp = sTmp + aDiffsy[34].ToString() + ";";
            sTmp = sTmp + aDiffsy[35].ToString() + ";";

            sTmp = sTmp + aDiffsy[44].ToString() + ";";
            sTmp = sTmp + aDiffsy[45].ToString() + ";";

            sTmp = sTmp + aDiffsy[55].ToString() + ";";

            p.k.SetSettingsString("losyRelacjiParam", sTmp);
            this.Frame.Navigate(typeof(LosyRelacji), sTmp);
        }

        private async System.Threading.Tasks.Task WczytajCombo(ComboBox oCombo, string sFileName)
        {
            oCombo.Items.Clear();
            Windows.Storage.StorageFolder oFold = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile oFile = await oFold.TryGetItemAsync(sFileName) as Windows.Storage.StorageFile;
            if (oFile == null)
                return;

            IList<string> oLns = await Windows.Storage.FileIO.ReadLinesAsync(oFile);
            foreach (string oLine in oLns)
                oCombo.Items.Add(oLine);
        }

        private async void uiPage_Loaded(object sender, RoutedEventArgs e)
        {
            // wypelnij combobox
            await WczytajCombo(uiCombo1, "lista.txt");
            if (p.k.IsThisMoje())
            {
                await WczytajCombo(uiCombo2, "listaInni.txt");
                uiCombo2.Visibility = Visibility.Visible;
            }
        }


        private async System.Threading.Tasks.Task WczytajDane(TextBox oTBox, string sFileName)
        {
            Windows.Storage.StorageFolder oFold = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile oFile = await oFold.TryGetItemAsync(sFileName) as Windows.Storage.StorageFile;
            if (oFile == null)
                return;

            // var oStream = await oFile.OpenReadAsync();
            string sTxt;
            try
            {
                sTxt = await Windows.Storage.FileIO.ReadTextAsync(oFile, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch
            {
                sTxt = p.k.GetLangString("errReadingPlikWynikow");
            }
            oTBox.Text = sTxt;
        }

        private async void uiCombo1_Changed(object sender, SelectionChangedEventArgs e)
        {
            // zamien combobox na textbox

            await WczytajDane(uiText1, uiCombo1.SelectedValue + ".txt");
            //string sFile = uiCombo1.SelectedValue + ".txt";
            //Windows.Storage.StorageFolder oFold = Windows.Storage.ApplicationData.Current.LocalFolder;
            //Windows.Storage.StorageFile oFile = await oFold.TryGetItemAsync(sFile) as Windows.Storage.StorageFile;
            //if (oFile == null)
            //    return;

            //string sTxt = await Windows.Storage.FileIO.ReadTextAsync(oFile);
            //uiText1.Text = sTxt;
        }

        private async void uiCombo2_Changed(object sender, SelectionChangedEventArgs e)
        {
            string sTmp = uiCombo2.SelectedValue.ToString();
            if (!sTmp.ToLower().EndsWith(".txt")) sTmp += ".txt";
            await WczytajDane(uiText2, sTmp);
        }
    }
}
