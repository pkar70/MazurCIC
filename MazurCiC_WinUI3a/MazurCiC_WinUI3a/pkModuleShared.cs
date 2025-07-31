
using System;
using System.Collections.Generic;
//using System.Text;
 using System.IO;
//using System.Runtime.CompilerServices;
using System.Linq;
//using static VBlib.Extensions;
using vb14 = VBlib.pkarlibmodule14;

using System.Runtime.InteropServices.WindowsRuntime; // dla ToArray
//using Microsoft.Extensions.Logging;
//using VBlib;

using static pkar.DotNetExtensions;

// logowanie tylko dla Uno.Droid, dla zwykłego UWP tego nie robimy
//#if !NETFX_CORE
//using Microsoft.Extensions.Logging;
//#endif


#if NETFX_CORE
using Winek = Windows.UI.Xaml; // dla WinUI 3
#else
using Winek = Microsoft.UI.Xaml; // dla WinUI 3
#endif
// using Winek = Windows.UI.Xaml; // dla UWP

// wersja dla Uno 4.0.11
// zakładam istnienie VBlib!

// 2022.04.12, pełny SYNC pkarmodulewithlib.vb


namespace p
{
    public partial class PkApplication : Winek.Application
    {
        protected Winek.Controls.Frame OnLaunchFragment(Winek.Window win)
        {
            Winek.Controls.Frame mRootFrame = win.Content as Winek.Controls.Frame;

            //' Do not repeat app initialization when the Window already has content,
            //' just ensure that the window is active

            if (mRootFrame is null)
            {
                //' Create a Frame to act as the navigation context and navigate to the first page
                mRootFrame = new Winek.Controls.Frame();

                mRootFrame.NavigationFailed += OnNavigationFailed;

                //' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
                mRootFrame.Navigated += OnNavigatedAddBackButton;
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += OnBackButtonPressed;

                //' Place the frame in the current Window
                win.Content = mRootFrame;

                p.k.InitLib(null);
            }

            return mRootFrame;
        }

        void OnNavigationFailed(object sender, Winek.Navigation.NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        #region "Back button"

        private void OnNavigatedAddBackButton(object sender, Winek.Navigation.NavigationEventArgs e)
        {
            try
            {
                Winek.Controls.Frame oFrame = sender as Winek.Controls.Frame;
                if (oFrame is null) return;

                Windows.UI.Core.SystemNavigationManager oNavig = Windows.UI.Core.SystemNavigationManager.GetForCurrentView();


                if (oFrame.CanGoBack)
                    oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;
                else
                    oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;

                return;
            }
            catch (Exception ex)
            {
                p.k.CrashMessageExit("@OnNavigatedAddBackButton", ex.Message);
            }
        }

        private void OnBackButtonPressed(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            try
            {
                (Winek.Window.Current.Content as Winek.Controls.Frame)?.GoBack();
                e.Handled = true;
            }
            catch { }
        }

        #endregion

#if false

        // to działa w Uno 4, ale potem już nie, bo potem zlikwidowali UWP i jest tylko WinUI gdzie tego nie ma
        //  RemoteSystems, Timer
        protected override async void OnBackgroundActivated(Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs args)
        {
            moTaskDeferal = args.TaskInstance.GetDeferral(); // w pkarmodule.App


            bool bNoComplete = false;
            bool bObsluzone = false;

            //' lista komend danej aplikacji
            string sLocalCmds = "";

            //' zwroci false gdy to nie jest RemoteSystem; gdy true, to zainicjalizowało odbieranie
            if (!bObsluzone) bNoComplete = RemSysInit(args, sLocalCmds);

            if (!bNoComplete) moTaskDeferal.Complete();
        }
#endif

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        internal virtual async System.Threading.Tasks.Task<string> AppServiceLocalCommand(string sCommand)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return "NO";
        }

        //' CommandLine, Toasts
        protected async System.Threading.Tasks.Task<bool> OnActivatedFragment(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            //' to jest m.in. dla Toast i tak dalej?

            //' próba czy to commandline
            if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.CommandLineLaunch)
            {

                Windows.ApplicationModel.Activation.CommandLineActivatedEventArgs commandLine = args as Windows.ApplicationModel.Activation.CommandLineActivatedEventArgs;
                Windows.ApplicationModel.Activation.CommandLineActivationOperation operation = commandLine?.Operation;
                string strArgs = operation?.Arguments;


                p.k.InitLib(strArgs.Split(' ').ToList<string>()); // mamy command line, próbujemy zrobić z tego string() (.Net Standard 1.4)

                if (!string.IsNullOrEmpty(strArgs))
                {
                    await ObsluzCommandLineAsync(strArgs);
                    Winek.Window.Current.Close();
                }
                return true;
            }


            return false;   // to Toast, proszę przejść do odpowiedniej strony
        }

        #region "RemoteSystem/Background"

        private Windows.ApplicationModel.Background.BackgroundTaskDeferral moTaskDeferal = null;
        private Windows.ApplicationModel.AppService.AppServiceConnection moAppConn;
        private string msLocalCmdsHelp = "";

        private void RemSysOnServiceClosed(Windows.ApplicationModel.AppService.AppServiceConnection appCon, Windows.ApplicationModel.AppService.AppServiceClosedEventArgs args)
        {
            if (appCon != null) appCon.Dispose();
            if (moTaskDeferal != null)
            {
                moTaskDeferal.Complete();
                moTaskDeferal = null;
            }
        }

        private void RemSysOnTaskCanceled(Windows.ApplicationModel.Background.IBackgroundTaskInstance sender, Windows.ApplicationModel.Background.BackgroundTaskCancellationReason reason)
        {
            if (moTaskDeferal != null)
            {
                moTaskDeferal.Complete();
                moTaskDeferal = null;
            }
        }

        ///<summary>
        ///do sprawdzania w OnBackgroundActivated
        ///jak zwróci True, to znaczy że nie wolno zwalniać moTaskDeferal !
        ///sLocalCmdsHelp: tekst do odesłania na HELP
        ///</summary>
        public bool RemSysInit(Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs args, string sLocalCmdsHelp)
        {
            Windows.ApplicationModel.AppService.AppServiceTriggerDetails oDetails =
             args.TaskInstance.TriggerDetails as Windows.ApplicationModel.AppService.AppServiceTriggerDetails;
            if (oDetails is null) return false;

            msLocalCmdsHelp = sLocalCmdsHelp;

            args.TaskInstance.Canceled += RemSysOnTaskCanceled;
            moAppConn = oDetails.AppServiceConnection;
            moAppConn.RequestReceived += RemSysOnRequestReceived;
            moAppConn.ServiceClosed += RemSysOnServiceClosed;
            return true;
        }

        public async System.Threading.Tasks.Task<string> CmdLineOrRemSysAsync(string sCommand)
        {
            string sResult = p.k.AppServiceStdCmd(sCommand, msLocalCmdsHelp);
            if (string.IsNullOrEmpty(sResult))
                sResult = await AppServiceLocalCommand(sCommand);

            return sResult;
        }

        public async System.Threading.Tasks.Task ObsluzCommandLineAsync(string sCommand)

        {
            Windows.Storage.StorageFolder oFold = Windows.Storage.ApplicationData.Current.TemporaryFolder;
            if (oFold is null) return;

            string sLockFilepathname = System.IO.Path.Combine(oFold.Path, "cmdline.lock");
            string sResultFilepathname = System.IO.Path.Combine(oFold.Path, "stdout.txt");

            try
            {
                System.IO.File.WriteAllText(sLockFilepathname, "lock");
            }
            catch
            {
                return;
            }

            string sResult = await CmdLineOrRemSysAsync(sCommand);
            if (string.IsNullOrEmpty(sResult))
                sResult = "(empty - probably unrecognized command)";

            System.IO.File.WriteAllText(sResultFilepathname, sResult);

            System.IO.File.Delete(sLockFilepathname);
        }

        private async void RemSysOnRequestReceived(Windows.ApplicationModel.AppService.AppServiceConnection sender, Windows.ApplicationModel.AppService.AppServiceRequestReceivedEventArgs args)
        {
            // 'Get a deferral so we can use an awaitable API to respond to the message

            string sStatus;
            string sResult = "";
            Windows.ApplicationModel.AppService.AppServiceDeferral messageDeferral = args.GetDeferral();

            if (vb14.GetSettingsBool("remoteSystemDisabled"))
            {
                sStatus = "No permission";
            }
            else
            {
                Windows.Foundation.Collections.ValueSet oInputMsg = args.Request.Message;

                sStatus = "ERROR while processing command";

                if (oInputMsg.ContainsKey("command"))
                {

                    String sCommand = (string)oInputMsg["command"];
                    sResult = await CmdLineOrRemSysAsync(sCommand);
                }

                if (sResult != "") sStatus = "OK";
            }

            Windows.Foundation.Collections.ValueSet oResultMsg = new Windows.Foundation.Collections.ValueSet();
            oResultMsg.Add("status", sStatus);
            oResultMsg.Add("result", sResult);

            await args.Request.SendResponseAsync(oResultMsg);

            messageDeferral.Complete();
            moTaskDeferal.Complete();
        }


        #endregion

        public static void OpenRateIt()
        {
            var sUri = new Uri("ms-windows-store://review/?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName);
            sUri.OpenBrowser();
        }

    }


    public static partial class k
    {

        public static void InitLib(List<string> aCmdLineArgs, bool bUseOwnFolderIfNotSD = true)
        {
            InitSettings(aCmdLineArgs);

            vb14.LibInitToast(FromLibMakeToast);
            vb14.LibInitDialogBox(FromLibDialogBoxAsync, FromLibDialogBoxYNAsync, FromLibDialogBoxInputAllDirectAsync);

            vb14.LibInitClip(FromLibClipPut, FromLibClipPutHtml);

            vb14.LangEnsureInit();

#if PKAR_USEDATALOG
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            InitDatalogFolder(bUseOwnFolderIfNotSD);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#endif
        }


        #region "CrashMessage"
        // większość w VBlib

        /// <summary>DialogBox z dotychczasowym logiem i skasowanie logu</summary>
        public async static System.Threading.Tasks.Task CrashMessageShowAsync()
        {
            string sTxt = vb14.GetSettingsString("appFailData");
            if (string.IsNullOrEmpty(sTxt))
                return;
            await FromLibDialogBoxAsync("FAIL messages:\n" + sTxt).ConfigureAwait(true);
            vb14.SetSettingsString("appFailData", "");
        }


        /// <summary>
        ///      Dodaj do logu, ewentualnie toast, i zakończ App
        ///      </summary>
        public static void CrashMessageExit(string sTxt, string exMsg)
        {
            vb14.CrashMessageAdd(sTxt, exMsg);
            Winek.Application.Current.Exit();
        }

        #endregion

        #region "Clipboard"
        // -- CLIPBOARD ---------------------------------------------

        public static void FromLibClipPut(string sTxt)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage oClipCont = new Windows.ApplicationModel.DataTransfer.DataPackage
            {
                RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy
            };
            oClipCont.SetText(sTxt);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(oClipCont);
        }

        public static void FromLibClipPutHtml(string sHtml)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage oClipCont = new Windows.ApplicationModel.DataTransfer.DataPackage
            {
                RequestedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy
            };
            oClipCont.SetHtmlFormat(sHtml);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(oClipCont);
        }


        /// <summary>
        ///      w razie Catch() zwraca ""
        ///      </summary>
        public async static System.Threading.Tasks.Task<string> ClipGetAsync()
        {
            try
            {
                Windows.ApplicationModel.DataTransfer.DataPackageView oClipCont = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent(); //< --nie ma w UNO
                return await oClipCont.GetTextAsync();
            }
            catch
            {
            }
            return "";
        }
        #endregion

        #region "Get/Set Settings"
        // -- Get/Set Settings ---------------------------------------------

        /// <summary>
        /// inicjalizacja pełnych zmiennych, bez tego wywołania będą tylko defaulty z pliku INI (i nie będzie pamiętania)
        /// </summary>

        private static string TryGetInstallDir()
        {
#if NETFX_CORE
            // dla UWP to może być: Windows.ApplicationModel.Package.Current.InstalledLocation.Path
            // albo System.AppContext.BaseDirectory
            return Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
#else
            return "";
            //// dla Android robimy bardziej naokoło
            ////Windows.Storage.StorageFolder oInstFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            ////Windows.Storage.StorageFile oFile = await oInstFolder.GetFileAsync("AppxManifest.xml");
            //string sAppDir;// = oFile.Path;
            //sAppDir = Android.App.Application.Context.Assets  PackageCodePath;
            //if (System.IO.File.Exists(System.IO.Path.Combine(sAppDir, "defaults.ini")))
            //{ // "/data/app/pkar.AutyzmTest-1/base.apk"
            //    int i = 1;
            //}
            //else
            //{
            //    int i = 2;
            //}

            //Windows.Storage.StorageFile oFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///defaults.ini"));

            //return sAppDir;
#endif
        }


        private static void InitSettings(List<string> aCmdLineArgs)
        {
            string sAppName = Windows.ApplicationModel.Package.Current.DisplayName;
            string sAppDir = TryGetInstallDir();

            vb14.InitSettings(sAppName, Environment.GetEnvironmentVariables(), new UwpConfigurationSource(), Windows.Storage.ApplicationData.Current.LocalFolder.Path,
                            Windows.Storage.ApplicationData.Current.RoamingFolder.Path, aCmdLineArgs);

            //Microsoft.Extensions.Configuration.IConfigurationBuilder oBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            //oBuilder = oBuilder.AddIniRelDebugSettings(VBlib.IniLikeDefaults.sIniContent);  
            //oBuilder = oBuilder.AddEnvironmentVariablesROConfigurationSource(sAppName, Environment.GetEnvironmentVariables()); // Environment.GetEnvironmentVariables, Std 2.0
            //oBuilder = oBuilder.AddUwpSettings();
            //oBuilder = oBuilder.AddJsonRwSettings(Windows.Storage.ApplicationData.Current.LocalFolder.Path,
            //                Windows.Storage.ApplicationData.Current.RoamingFolder.Path);
            //if (aCmdLineArgs != null) oBuilder = oBuilder.AddCommandLineRO(aCmdLineArgs);  // Environment.GetCommandLineArgs, Std 1.5, ale nie w UWP?

            //Microsoft.Extensions.Configuration.IConfigurationRoot settings = oBuilder.Build();

            //vb14.LibInitSettings(settings);
        }

#if false

#region "string"

        // odwołanie się do zmiennych
        public static string FromLibGetSettingsString(string sName, string sDefault)
        {
            string sTmp;
            sTmp = sDefault;

            if (Windows.Storage.ApplicationData.Current.RoamingSettings.Values.ContainsKey(sName))
                sTmp = Windows.Storage.ApplicationData.Current.RoamingSettings.Values[sName].ToString();
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(sName))
                sTmp = Windows.Storage.ApplicationData.Current.LocalSettings.Values[sName].ToString();

            return sTmp;
        }

        public static void FromLibSetSettingsString(string sName, string sValue, bool bRoam)
        {
            try
            {
                if (bRoam) Windows.Storage.ApplicationData.Current.RoamingSettings.Values[sName] = sValue;
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[sName] = sValue;
                //return true;
            }
            catch
            {
                // jesli przepełniony bufor (za długa zmienna) - nie zapisuj dalszych błędów
                //return false;
            }

        }

#endregion
#region "int"

        public static int FromLibGetSettingsInt(string sName, int iDefault)
        {
            int sTmp;

            sTmp = iDefault;

            {
                var withBlock = Windows.Storage.ApplicationData.Current;
                if (withBlock.RoamingSettings.Values.ContainsKey(sName))
                    sTmp = System.Convert.ToInt32(withBlock.RoamingSettings.Values[sName].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                if (withBlock.LocalSettings.Values.ContainsKey(sName))
                    sTmp = System.Convert.ToInt32(withBlock.LocalSettings.Values[sName].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            }

            return sTmp;
        }

        public static void FromLibSetSettingsInt(string sName, int sValue, bool bRoam)
        {
            {
                var withBlock = Windows.Storage.ApplicationData.Current;
                if (bRoam)
                    withBlock.RoamingSettings.Values[sName] = sValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                withBlock.LocalSettings.Values[sName] = sValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }
        }



#endregion
#region "Long"
        public static long FromLibGetSettingsLong(String sName, long iDefault)
        {
            long sTmp = iDefault;

            {
                var withBlock = Windows.Storage.ApplicationData.Current;
                if (withBlock.RoamingSettings.Values.ContainsKey(sName))
                    sTmp = System.Convert.ToInt64(withBlock.RoamingSettings.Values[sName].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                if (withBlock.LocalSettings.Values.ContainsKey(sName))
                    sTmp = System.Convert.ToInt64(withBlock.LocalSettings.Values[sName].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            }

            return sTmp;
        }

        public static void FromLibSetSettingsLong(string sName, long sValue, bool bRoam)
        {
            {
                var withBlock = Windows.Storage.ApplicationData.Current;
                if (bRoam)
                    withBlock.RoamingSettings.Values[sName] = sValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                withBlock.LocalSettings.Values[sName] = sValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

        }
#endregion
#region "bool"
        public static bool FromLibGetSettingsBool(string sName, bool iDefault = false)
        {
            bool sTmp;

            sTmp = iDefault;
            {
                var withBlock = Windows.Storage.ApplicationData.Current;
                if (withBlock.RoamingSettings.Values.ContainsKey(sName))
                    sTmp = System.Convert.ToBoolean(withBlock.RoamingSettings.Values[sName].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                if (withBlock.LocalSettings.Values.ContainsKey(sName))
                    sTmp = System.Convert.ToBoolean(withBlock.LocalSettings.Values[sName].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            }

            return sTmp;
        }


        public static void FromLibSetSettingsBool(string sName, bool sValue, bool bRoam = false)
        {
            {
                var withBlock = Windows.Storage.ApplicationData.Current;
                if (bRoam)
                    withBlock.RoamingSettings.Values[sName] = sValue.ToString();
                withBlock.LocalSettings.Values[sName] = sValue.ToString();
            }
        }

#endregion
#region "Date"
        //public static void SetSettingsDate(string sName)
        //{
        //    string sValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    SetSettingsString(sName, sValue);
        //}


#endregion
#endif

#endregion

#region "testy sieciowe"
        // -- Testy sieciowe ---------------------------------------------


        public static bool IsFamilyMobile()
        { // Brewiarz: wymuszanie zmiany dark/jasne
          // GrajCyganie: zmiana wielkosci okna
          // pociagi: ile rzadkow ma pokazac (rozmiar ekranu)
          // kamerki: full screen wlacz/wylacz tylko dla niego
          // sympatia...
          // TODO: WASM w zależności od rozmiaru ekranu?
            return (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile");
            //return Windows.System.Profile.AnalyticsInfo.DeviceForm.ToLower().Contains("mobile");
        }

        public static bool IsFamilyDesktop()
        {
            return (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop");
        }

        // [Obsolete("Jest w .Net Standard 2.0 (lib)")]
        public static bool NetIsIPavailable(bool bMsg)
        {

            if (vb14.GetSettingsBool("offline"))
                return false;

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return true;
            if (bMsg)
                vb14.DialogBox("ERROR: no IP network available");
            return false;
        }

        // [Obsolete("Jest w .Net Standard 2.0 (lib), ale on jest nie do telefonu :)")]
        public static bool NetIsCellInet()
        {
            return Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile().IsWwanConnectionProfile;
        }

#if __ANDROID__
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        private static bool AndroidReminderObsolete()
        {
            var oContext = Android.App.Application.Context;
            Android.Net.ConnectivityManager cm =
                (Android.Net.ConnectivityManager)oContext.GetSystemService(Android.Content.Context.ConnectivityService);

            // OBSOLETE w Android 29 (=Q)
            // https://developer.android.com/reference/android/net/NetworkInfo
            // postawione tutaj, Å¼eby przy kazdej kompilacji sprawdzac czy jeszcze w Android to jest
            Android.Net.NetworkInfo info = cm.ActiveNetworkInfo;
            if (info == null) return false;
            if (!info.IsConnected) return false;

            // drugie reminder
            string sDirName = Android.OS.Environment.DirectoryMusic;
            string sPathName = Android.OS.Environment.GetExternalStoragePublicDirectory(sDirName).CanonicalPath;
            // ta ścieżka jest niedostępna dla app od Android Q, API29, Android 10

            return true;
        }
#pragma warning restore IDE0059 // Unnecessary assignment of a value
#endif
        //[Obsolete("Jest w .Net Standard 2.0 (lib)")]
        public static string GetHostName()
        {
            string sNazwa = System.Net.Dns.GetHostName();
            return sNazwa;
            //IReadOnlyList<Windows.Networking.HostName> hostNames = Windows.Networking.Connectivity.NetworkInformation.GetHostNames();
            //foreach (Windows.Networking.HostName oItem in hostNames)
            //{
            //    if (oItem.DisplayName.Contains(".local"))
            //        return oItem.DisplayName.Replace(".local", "");
            //}
            //return "";
        }

        //[Obsolete("Jest w .Net Standard 2.0 (lib)")]
        public static bool IsThisMoje()
        {
            string sTmp = GetHostName().ToLowerInvariant();
            if (sTmp.StartsWith("home-pkar"))
                return true;
            if (sTmp == "lumia_pkar")
                return true;
            if (sTmp == "kuchnia_pk")
                return true;
            if (sTmp == "ppok_pk")
                return true;
            // If sTmp.Contains("pkar") Then Return True
            // If sTmp.EndsWith("_pk") Then Return True
            return false;
        }

        // unimplemented in Uno yet
#if false

        public async static System.Threading.Tasks.Task<bool> NetWiFiOffOn()
        {

            // https://social.msdn.microsoft.com/Forums/ie/en-US/60c4a813-dc66-4af5-bf43-e632c5f85593/uwpbluetoothhow-to-turn-onoff-wifi-bluetooth-programmatically?forum=wpdevelop
            var result222 = await Windows.Devices.Radios.Radio.RequestAccessAsync();
            IReadOnlyList<Windows.Devices.Radios.Radio> radios = await Windows.Devices.Radios.Radio.GetRadiosAsync();

            foreach (var oRadio in radios)
            {
                if (oRadio.Kind == Windows.Devices.Radios.RadioKind.WiFi)
                {
                    Windows.Devices.Radios.RadioAccessStatus oStat = await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.Off);
                    if (oStat != Windows.Devices.Radios.RadioAccessStatus.Allowed)
                        return false;
                    await Task.Delay(3 * 1000);
                    oStat = await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.On);
                    if (oStat != Windows.Devices.Radios.RadioAccessStatus.Allowed)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///      Zwraca -1 (no radio), 0 (off), 1 (on), ale gdy bMsg to pokazuje dokładniej błąd (nie włączony, albo nie ma radia Bluetooth) - wedle stringów podanych, które mogą być jednak identyfikatorami w Resources
        ///      </summary>
        public async static Task<int> NetIsBTavailableAsync(bool bMsg, bool bRes = false, string sBtDisabled = "ERROR: Bluetooth is not enabled", string sNoRadio = "ERROR: Bluetooth radio not found")
        {


            // Dim result222 As Windows.Devices.Radios.RadioAccessStatus = Await Windows.Devices.Radios.Radio.RequestAccessAsync()
            // If result222 <> Windows.Devices.Radios.RadioAccessStatus.Allowed Then Return -1

            IReadOnlyList<Windows.Devices.Radios.Radio> oRadios = await Windows.Devices.Radios.Radio.GetRadiosAsync();

            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
            bool bHasBT = false;

            foreach (Windows.Devices.Radios.Radio oRadio in oRadios)
            {
                if (oRadio.Kind == Windows.Devices.Radios.RadioKind.Bluetooth)
                {
                    if (oRadio.State == Windows.Devices.Radios.RadioState.On)
                        return 1;
                    bHasBT = true;
                }
            }

            if (bHasBT)
            {
                if (bMsg)
                {
                    if (bRes)
                        await DialogBoxResAsync(sBtDisabled);
                    else
                        await DialogBoxAsync(sBtDisabled);
                }
                return 0;
            }
            else
            {
                if (bMsg)
                {
                    if (bRes)
                        await DialogBoxResAsync(sNoRadio);
                    else
                        await DialogBoxAsync(sNoRadio);
                }
                return -1;
            }
        }

        /// <summary>
        ///      Zwraca true/false czy State (po call) jest taki jak bOn; wymaga devCap=radios
        ///      </summary>
        public async static Task<bool> NetTrySwitchBTOnAsync(bool bOn)
        {
            int iCurrState = await NetIsBTavailableAsync(false);
            if (iCurrState == -1)
                return false;

            // jeśli nie trzeba przełączać... 
            if (bOn && iCurrState == 1)
                return true;
            if (!bOn && iCurrState == 0)
                return true;

            // czy mamy prawo przełączyć? (devCap=radios)
            Windows.Devices.Radios.RadioAccessStatus result222 = await Windows.Devices.Radios.Radio.RequestAccessAsync();
            if (result222 != Windows.Devices.Radios.RadioAccessStatus.Allowed)
                return false;


            IReadOnlyList<Windows.Devices.Radios.Radio> radios = await Windows.Devices.Radios.Radio.GetRadiosAsync();

            foreach (var oRadio in radios)
            {
                if (oRadio.Kind == Windows.Devices.Radios.RadioKind.Bluetooth)
                {
                    Windows.Devices.Radios.RadioAccessStatus oStat;
                    if (bOn)
                        oStat = await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.On);
                    else
                        oStat = await oRadio.SetStateAsync(Windows.Devices.Radios.RadioState.Off);
                    if (oStat != Windows.Devices.Radios.RadioAccessStatus.Allowed)
                        return false;
                }
            }

            return true;
        }


#endif

#endregion

#region "DialogBoxy"
        // -- DialogBoxy - tylko jako wskok z VBlib -----------------------------------------

        public async static System.Threading.Tasks.Task FromLibDialogBoxAsync(string sMsg)
        {
            Windows.UI.Popups.MessageDialog oMsg = new Windows.UI.Popups.MessageDialog(sMsg);
            await oMsg.ShowAsync();
        }

        public async static System.Threading.Tasks.Task<bool> FromLibDialogBoxYNAsync(string sMsg, string sYes = "Tak", string sNo = "Nie")
        {
            Windows.UI.Popups.MessageDialog oMsg = new Windows.UI.Popups.MessageDialog(sMsg);
            Windows.UI.Popups.UICommand oYes = new Windows.UI.Popups.UICommand(sYes);
            Windows.UI.Popups.UICommand oNo = new Windows.UI.Popups.UICommand(sNo);
            oMsg.Commands.Add(oYes);
            oMsg.Commands.Add(oNo);
            oMsg.DefaultCommandIndex = 1;    // default: No
            oMsg.CancelCommandIndex = 1;
            Windows.UI.Popups.IUICommand oCmd = await oMsg.ShowAsync();
            if (oCmd == null)
                return false;
            if (oCmd.Label == sYes)
                return true;

            return false;
        }


        public async static System.Threading.Tasks.Task<string> FromLibDialogBoxInputAllDirectAsync(string sMsg, string sDefault = "", string sYes = "Continue", string sNo = "Cancel")
        {
            Winek.Controls.TextBox oInputTextBox = new Winek.Controls.TextBox
            {
                AcceptsReturn = false,
                Text = sDefault
            };
            Winek.Controls.ContentDialog oDlg = new Winek.Controls.ContentDialog
            {
                Content = oInputTextBox,
                PrimaryButtonText = sYes,
                SecondaryButtonText = sNo,
                Title = sMsg
            };

            var oCmd = await oDlg.ShowAsync();
//#if !NETFX_CORE
//            oDlg.Dispose();
//#endif
            if (oCmd != Winek.Controls.ContentDialogResult.Primary)
                return "";

            return oInputTextBox.Text;
        }

#endregion

#region "CheckPlatform etc"

        public static string GetPlatform()
        {
#if NETFX_CORE
            return "uwp";
#elif __ANDROID__
            return "android";
#elif __IOS__
        return "ios";
#elif __WASM__
        return "wasm";
#else
        return "other";
#endif
        }

        public static bool GetPlatform(string sPlatform)
        {
            if (string.IsNullOrEmpty(sPlatform)) return false;
            if (GetPlatform().Equals(sPlatform, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }


        public static T GetPlatform<T>(T bUwp, T bAndro, T bIos, T bWasm, T bOther)
        {
#if NETFX_CORE
            return bUwp;
#elif __ANDROID__
            return bAndro;
#elif __IOS__
        return bIos;
#elif __WASM__
            return bWasm;
#else
        return bOther;
#endif
        }


#endregion


        // --- INNE FUNKCJE ------------------------

#region "toasty"
        //public static void SetBadgeNo(int iInt)
        //{
        //    // https://docs.microsoft.com/en-us/windows/uwp/controls-and-patterns/tiles-and-notifications-badges

        //    Windows.Data.Xml.Dom.XmlDocument oXmlBadge;
        //    oXmlBadge = Windows.UI.Notifications.BadgeUpdateManager.GetTemplateContent(Windows.UI.Notifications.BadgeTemplateType.BadgeNumber);

        //    Windows.Data.Xml.Dom.XmlElement oXmlNum;
        //    oXmlNum = (Windows.Data.Xml.Dom.XmlElement)oXmlBadge.SelectSingleNode("/badge");
        //    oXmlNum.SetAttribute("value", iInt.ToString());

        //    Windows.UI.Notifications.BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(new Windows.UI.Notifications.BadgeNotification(oXmlBadge));
        //}

        public static string ToastAction(string sAType, string sAct, string sGuid, string sContent)
        {
            string sTmp = sContent;
            if (!string.IsNullOrEmpty(sTmp))
                sTmp = vb14.GetSettingsString(sTmp, sTmp);

            string sTxt = "<action " + "activationType=\"" + sAType + "\" " + "arguments=\"" + sAct + sGuid + "\" " + "content=\"" + sTmp + "\"/> ";
            return sTxt;
        }

        /// <summary>
        ///      dwa kolejne teksty, sMsg oraz sMsg1
        ///      </summary>

        public static void FromLibMakeToast(string sMsg, string sMsg1 = "")
        {
            // Mój Uno: razem z Android, ich Uno - tylko UWP
            var sXml = "<visual><binding template='ToastGeneric'><text>" + vb14.XmlSafeString(sMsg);
            if (!string.IsNullOrEmpty(sMsg1))
                sXml = sXml + "</text><text>" + vb14.XmlSafeString(sMsg1);
            sXml += "</text></binding></visual>";
            var oXml = new Windows.Data.Xml.Dom.XmlDocument();
            oXml.LoadXml("<toast>" + sXml + "</toast>");
            var oToast = new Windows.UI.Notifications.ToastNotification(oXml);

            // WYMAGA MOJEJ KOMPILACJI
            Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().Show(oToast);

        }

        public static void MakeToast(string sMsg, string sMsg1 = "")
        {
            FromLibMakeToast(sMsg, sMsg1);
        }


#if NETFX_CORE
        // z datą!
        public static void MakeToast(DateTime oDate, string sMsg, string sMsg1 = "")
        {
            string sXml = "<visual><binding template='ToastGeneric'><text>" + vb14.XmlSafeString(sMsg);
            if (sMsg1 != "") sXml = sXml + "</text><text>" + vb14.XmlSafeString(sMsg1);
            sXml += "</text></binding></visual>";
            var oXml = new Windows.Data.Xml.Dom.XmlDocument();
            oXml.LoadXml("<toast>" + sXml + "</toast>");
            try
            {
                // ' Dim oToast = New Windows.UI.Notifications.ScheduledToastNotification(oXml, oDate, TimeSpan.FromHours(1), 10)
                var oToast = new Windows.UI.Notifications.ScheduledToastNotification(oXml, oDate);
                Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().AddToSchedule(oToast);
            }
            catch
            {
            }
        }


        public static void RemoveScheduledToasts()
        {
            var oToastMan = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
            try
            {
                while (oToastMan.GetScheduledToastNotifications().Count > 0)
                    oToastMan.RemoveFromSchedule(oToastMan.GetScheduledToastNotifications().ElementAt(0));
            }
            catch
            {
                // ' ponoc na desktopm nie dziala
            }
        }
#endif

#endregion

        // [Obsolete("Jest w .Net Standard 2.0 (lib)")]
#pragma warning disable CA1024 // Use properties where appropriate
        public static string GetAppVers()
#pragma warning restore CA1024 // Use properties where appropriate
        {
            return Windows.ApplicationModel.Package.Current.Id.Version.Major + "." +
                Windows.ApplicationModel.Package.Current.Id.Version.Minor + "." +
                Windows.ApplicationModel.Package.Current.Id.Version.Build;

        }


        public static int WinVer()
        {
            // Unknown = 0,
            // Threshold1 = 1507,   // 10240
            // Threshold2 = 1511,   // 10586
            // Anniversary = 1607,  // 14393 Redstone 1
            // Creators = 1703,     // 15063 Redstone 2
            // FallCreators = 1709 // 16299 Redstone 3
            // April = 1803		// 17134
            // October = 1809		// 17763
            // ? = 190?		// 18???
            // April  1803, 17134, RS5

#if NETFX_CORE
            ulong u = ulong.Parse(Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion, System.Globalization.CultureInfo.InvariantCulture);
            u = (u & 0xFFFF0000L) >> 16;
            return (int)u;
#elif __ANDROID__
            return (int)Android.OS.Build.VERSION.SdkInt;
#else
            return 0;
#endif
        }


        public static async System.Threading.Tasks.Task<string> GetBuildTimestampAsync()
        {
#if NETFX_CORE
            Windows.Storage.StorageFolder install_folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Windows.Storage.StorageFile manifest_file = await install_folder.GetFileAsync("AppxManifest.xml");
            DateTimeOffset modDate = (await manifest_file.GetBasicPropertiesAsync()).DateModified;
#pragma warning disable CA1305 // Specify IFormatProvider
            return modDate.ToString("yyyy.MM.dd HH:mm");
#pragma warning restore CA1305 // Specify IFormatProvider
#else
return "";
#endif
        }


#region "triggers"
        public static bool IsTriggersRegistered(string sNamePrefix)
        {
            if (sNamePrefix == null) sNamePrefix = "";
            sNamePrefix = sNamePrefix.Replace(" ", "").Replace("'", "");

            try
            {
                foreach (var oTask in Windows.ApplicationModel.Background.BackgroundTaskRegistration.AllTasks)
                {
                    if (oTask.Value.Name.ToUpperInvariant().Contains(sNamePrefix.ToUpperInvariant()))
                        return true;
                }
            }
            catch
            {
                // np. gdy nie ma permissions, to może być FAIL
            }

            return false;
        }

        /// <summary>
        /// jakikolwiek z prefixem Package.Current.DisplayName
        /// </summary>
        public static bool IsTriggersRegistered()
        {
            return IsTriggersRegistered(Windows.ApplicationModel.Package.Current.DisplayName);
        }

        /// <summary>
        /// wszystkie z prefixem Package.Current.DisplayName
        /// </summary>
        public static void UnregisterTriggers()
        {
            UnregisterTriggers(Windows.ApplicationModel.Package.Current.DisplayName);
        }

        public static void UnregisterTriggers(string sNamePrefix)
        {
            if (sNamePrefix == null) sNamePrefix = "";
            sNamePrefix = sNamePrefix.Replace(" ", "").Replace("'", "");

            try
            {
                foreach (var oTask in Windows.ApplicationModel.Background.BackgroundTaskRegistration.AllTasks)
                {
                    if (string.IsNullOrEmpty(sNamePrefix) ||
                        oTask.Value.Name.ToUpperInvariant().Contains(sNamePrefix.ToUpperInvariant()))
                    {
                        oTask.Value.Unregister(true);
                    }
                }
            }
            catch
            {
                // np. gdy nie ma permissions, to może być FAIL
            }
        }

        public static async System.Threading.Tasks.Task<bool> CanRegisterTriggersAsync()
        {
            Windows.ApplicationModel.Background.BackgroundAccessStatus oBAS;
            oBAS = await Windows.ApplicationModel.Background.BackgroundExecutionManager.RequestAccessAsync();

            if (oBAS == Windows.ApplicationModel.Background.BackgroundAccessStatus.AlwaysAllowed) return true;
            if (oBAS == Windows.ApplicationModel.Background.BackgroundAccessStatus.AllowedSubjectToSystemPolicy) return true;

            return false;
        }

        public static Windows.ApplicationModel.Background.BackgroundTaskRegistration RegisterTimerTrigger(string sName, uint iMinutes, bool bOneShot = false, Windows.ApplicationModel.Background.SystemCondition oCondition = null)
        {
            try
            {
                var builder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

                builder.SetTrigger(new Windows.ApplicationModel.Background.TimeTrigger(iMinutes, bOneShot));
                builder.Name = sName;
                if (oCondition is object) builder.AddCondition(oCondition);
                var oRet = builder.Register();
                return oRet;
            }
            catch
            {
                // np. gdy nie ma permissions, to może być FAIL
            }

            return null;
        }

        public static Windows.ApplicationModel.Background.BackgroundTaskRegistration RegisterUserPresentTrigger(string sName = "", bool bOneShot = false)
        {
            try
            {
                Windows.ApplicationModel.Background.BackgroundTaskBuilder builder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();
                Windows.ApplicationModel.Background.BackgroundTaskRegistration oRet;

                Windows.ApplicationModel.Background.SystemTrigger oTrigger;
                oTrigger = new Windows.ApplicationModel.Background.SystemTrigger(Windows.ApplicationModel.Background.SystemTriggerType.UserPresent, bOneShot);

                builder.SetTrigger(oTrigger);
                builder.Name = sName;

                if (string.IsNullOrEmpty(sName)) builder.Name = GetTriggerNamePrefix() + "_userpresent";


                oRet = builder.Register();

                return oRet;
            }
            catch
            {
                // brak możliwości rejestracji (na przykład)
                return null;
            }
        }

        private static string GetTriggerNamePrefix()
        {
            string sName = Windows.ApplicationModel.Package.Current.DisplayName;
            sName = sName.Replace(" ", "").Replace("'", "");
            return sName;
        }

        private static string GetTriggerPolnocnyName()
        {
            return GetTriggerNamePrefix() + "_polnocny";
        }


        /// <summary>
        /// Tak naprawdę powtarzalny - w OnBackgroundActivated wywołaj IsThisTriggerPolnocny
        /// </summary>
        public static async System.Threading.Tasks.Task DodajTriggerPolnocny()
        {
            if (!await p.k.CanRegisterTriggersAsync()) return;

            DateTime oDateNew = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 40, 0);
            if (DateTime.Now.Hour > 21)
                oDateNew = oDateNew.AddDays(1);

            uint iMin = (uint)(oDateNew - DateTime.Now).TotalMinutes;

            string sName = GetTriggerPolnocnyName();
            p.k.RegisterTimerTrigger(sName, iMin, false);
        }

        /// <summary>
        /// para z DodajTriggerPolnocny, do wywoływania w OnBackgroundActivated
        /// </summary>
        public static bool IsThisTriggerPolnocny(Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs args)
        {
            if (args?.TaskInstance?.Task is null) return false;
            string sName = GetTriggerPolnocnyName();
            if (args.TaskInstance.Task.Name != sName) return false;

            // no dobrze, jest to trigger północny, ale czy o północy...
#pragma warning disable CA1305 // Specify IFormatProvider
            string sCurrDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
#pragma warning restore CA1305 // Specify IFormatProvider
            vb14.SetSettingsString("lastPolnocnyTry", sCurrDate);

            bool bRet;
            DateTime oDateNew = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 40, 0);

            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute > 20)    // 40 minut, ale system dodaje ±15 minut!
            {
                // tak, to jest północny o północy
                bRet = true;
                oDateNew = oDateNew.AddDays(1);
                vb14.SetSettingsString("lastPolnocnyOk", sCurrDate);
            }
            else
            {
                // północny, ale nie o północy
                bRet = false;
            }
            int iMin = (int)(oDateNew - DateTime.Now).TotalMinutes;

            // Usuwamy istniejący, robimy nowy
            UnregisterTriggers(sName);
            RegisterTimerTrigger(sName, (uint)iMin, false);

            return bRet;
        }

#if NETFX_CORE || __ANDROID__
        public static Windows.ApplicationModel.Background.BackgroundTaskRegistration RegisterToastTrigger(string sName)
        {
            try
            {
                var builder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

                builder.SetTrigger(new Windows.ApplicationModel.Background.ToastNotificationActionTrigger());
                builder.Name = sName;
                var oRet = builder.Register();
                return oRet;
            }
            catch
            {
                // np. gdy nie ma permissions, to może być FAIL
            }

            return null;
        }
#endif

        // ServicingCompleted unimplemted w Uno
#if false
        public static Background.BackgroundTaskRegistration RegisterServicingCompletedTrigger(string sName)
        {
            try
            {
                Background.BackgroundTaskBuilder builder = new Background.BackgroundTaskBuilder();
                Windows.ApplicationModel.Background.BackgroundTaskRegistration oRet;

                builder.SetTrigger(new Background.SystemTrigger(Background.SystemTriggerType.ServicingComplete, true));
                builder.Name = sName;
                oRet = builder.Register();
                return oRet;
            }
            catch (Exception ex)
            {
            }

            return null/* TODO Change to default(_) if this is not a reference type */;
        }
#endif

#endregion

#region "RemoteSystem"
        public static bool IsTriggerAppService(Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs args)
        {
            if (args?.TaskInstance?.TriggerDetails is null) return false;

#if NETFX_CORE
#pragma warning disable IDE0019 // Use pattern matching
            Windows.ApplicationModel.AppService.AppServiceTriggerDetails oDetails =
                args.TaskInstance.TriggerDetails as Windows.ApplicationModel.AppService.AppServiceTriggerDetails;
#pragma warning restore IDE0019 // Use pattern matching
            if (oDetails is null) return false;
            return true;
#else
            return false;
#endif
        }

        public static string AppServiceStdCmd(string sCommand, string sLocalCmds)
        {
            if (string.IsNullOrEmpty(sCommand)) return "";

            string sTmp = vb14.LibAppServiceStdCmd(sCommand, sLocalCmds);
            if (sTmp != "") return sTmp;

#pragma warning disable CA1308 // Normalize strings to uppercase
            switch (sCommand.ToLowerInvariant())
#pragma warning restore CA1308 // Normalize strings to uppercase
            {
                // case "ping":
                case "ver":
                    return p.k.GetAppVers();
                case "localdir":
                    return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                // case "appdir":
                case "installeddate":
#pragma warning disable CA1305 // Specify IFormatProvider
                    return Windows.ApplicationModel.Package.Current.InstalledDate.ToString("yyyy.MM.dd HH:mm:ss");
#pragma warning restore CA1305 // Specify IFormatProvider

                // case "help":
                // case "debug vars":
                case "debug triggers":
                    return DumpTriggers();
                case "debug toasts":
                    return DumpToasts();

                case "debug memsize":
                    return GetAppMemData();
                case "debug rungc":
                    sTmp = "Memory usage before Global Collector call: " + GetAppMemData() + "\n";
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    sTmp = sTmp + "After: " + GetAppMemData();
                    return sTmp;

                //case "debug crashmsg":
                //case "debug crashmsg clear":
                case "lib unregistertriggers":
                    sTmp = DumpTriggers();
                    UnregisterTriggers(""); // całkiem wszystkie
                    return sTmp;
                case "lib isfamilymobile":
                    return IsFamilyMobile().ToString();
                case "lib isfamilydesktop":
                    return IsFamilyDesktop().ToString();
                case "lib netisipavailable":
                    return NetIsIPavailable(false).ToString();
                case "lib netiscellinet":
                    return NetIsCellInet().ToString();
                case "lib gethostname":
                    return GetHostName();
                case "lib isthismoje":
                    return IsThisMoje().ToString();
                case "lib istriggersregistered":
                    return IsTriggersRegistered().ToString();

                    //case "lib pkarmode 1":
                    //case "lib pkarmode 0":
                    //case "lib pkarmode":
            }

            return "";  // oznacza: to nie jest standardowa komenda
        }

        private static string GetAppMemData()
        {
#if NETFX_CORE || __ANDROID__
            return Windows.System.MemoryManager.AppMemoryUsage.ToString(System.Globalization.CultureInfo.InvariantCulture) + "/" + Windows.System.MemoryManager.AppMemoryUsageLimit.ToString(System.Globalization.CultureInfo.InvariantCulture);
#else
            return "GetAppMemData is not implemented on non-UWP";
#endif
        }

        //private static string DumpSettings()
        //{
        //    string sRoam = "";
        //    try
        //    {
        //        foreach (var oVal in Windows.Storage.ApplicationData.Current.RoamingSettings.Values)
        //        {
        //            sRoam += oVal.Key + "\t" + oVal.Value.ToString() + "\n";
        //        }
        //    }
        //    catch { };

        //    string sLocal = "";
        //    try
        //    {
        //        foreach (var oVal in Windows.Storage.ApplicationData.Current.LocalSettings.Values)
        //        {
        //            sLocal += oVal.Key + "\t" + oVal.Value.ToString() + "\n";
        //        }
        //    }
        //    catch { };

        //    string sRet = "Dumping Settings\n";
        //    if (sRoam != "")
        //        sRet += "\nRoaming:\n" + sRoam;
        //    else
        //        sRet += "(no roaming settings)\n";

        //    if (sLocal != "")
        //        sRet += "\nLocal:\n" + sLocal;
        //    else
        //        sRet += "(no local settings)\n";


        //    return sRet;
        //}

        private static string DumpTriggers()
        {
            string sRet = "";

            try
            {
                foreach (var oTask in Windows.ApplicationModel.Background.BackgroundTaskRegistration.AllTasks)
                {
                    sRet += oTask.Value.Name + "\n";    //GetType niestety nie daje rzeczywistego typu
                }
            }
            catch { };

            if (sRet == "") return "No registered triggers\n";

            return "Dumping Triggers\n\n" + sRet;

        }
        private static string DumpToasts()
        {
#if NETFX_CORE
            string sResult = "";

            foreach (Windows.UI.Notifications.ScheduledToastNotification oToast
                in Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications())
            {
#pragma warning disable CA1305 // Specify IFormatProvider
                sResult = sResult + oToast.DeliveryTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
#pragma warning restore CA1305 // Specify IFormatProvider
            }

            if (sResult == "")
                sResult = "(no toasts scheduled)";
            else
                sResult = "Toasts scheduled for dates: \n" + sResult;
            return sResult;
#else
            return "DumpToasts on non-UWP is not implemented";
#endif
        }

        //private static async System.Threading.Tasks.Task<string> DumpSDKvers()
        //{
        //    var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("AppxManifest.XML");
        //    using (Stream stream = await file.OpenStreamForReadAsync())
        //    {
        //        //var doc = XDocument.Load(stream);
        //        //<Package
        //        // <Dependencies>
        //        //  <TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.14393.0" MaxVersionTested="10.0.19041.0" />

        //    }
        //    return "bla";
        //}

#endregion

#region "Datalog"
#if NETFX_CORE
        private async static System.Threading.Tasks.Task<Windows.Storage.StorageFolder> GetSDcardFolderAsync()
        {
            // uwaga: musi być w Manifest RemoteStorage oraz fileext!

            Windows.Storage.StorageFolder oRootDir;

            try
            {
                oRootDir = Windows.Storage.KnownFolders.RemovableDevices;
            }
            catch
            {
                return null;
            }// brak uprawnień, może być także THROW

            try
            {
                IReadOnlyList<Windows.Storage.StorageFolder> oCards = await oRootDir.GetFoldersAsync();
                if (oCards.Count < 1) return null;
                return oCards[0];
            }
            catch
            {
            }

            return null;
        }

#endif


#if NETFX_CORE
        private async static System.Threading.Tasks.Task<Windows.Storage.StorageFolder> GetLogFolderRootDatalogsOnSDcardAsync()
        {
            Windows.Storage.StorageFolder oSdCard;
            Windows.Storage.StorageFolder oFold;

            oSdCard = await GetSDcardFolderAsync();

            if (oSdCard is null) return null;

            oFold = await oSdCard.CreateFolderAsync("DataLogs", Windows.Storage.CreationCollisionOption.OpenIfExists);
            if (oFold == null) return null;

            string sAppName = Windows.ApplicationModel.Package.Current.DisplayName;
            sAppName = sAppName.Replace(" ", "").Replace("'", "");
            oFold = await oFold.CreateFolderAsync(sAppName, Windows.Storage.CreationCollisionOption.OpenIfExists);

            return oFold;
        }
#else
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async static System.Threading.Tasks.Task<Windows.Storage.StorageFolder> GetLogFolderRootDatalogsOnSDcardAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return null;
        }
#endif

        private async static System.Threading.Tasks.Task<Windows.Storage.StorageFolder> GetLogFolderRootDatalogsInAppAsync()
        {
            Windows.Storage.StorageFolder oSdCard;
            Windows.Storage.StorageFolder oFold;

            oSdCard = Windows.Storage.ApplicationData.Current.LocalFolder;
            oFold = await oSdCard.CreateFolderAsync("DataLogs", Windows.Storage.CreationCollisionOption.OpenIfExists);
            return oFold;
        }

        public async static System.Threading.Tasks.Task<Windows.Storage.StorageFolder> GetLogFolderRootAsync(bool bUseOwnFolderIfNotSD = true)
        {
            Windows.Storage.StorageFolder oFold = null;

            if (IsFamilyMobile())
            { // poza UWP zwróci null
                oFold = await GetLogFolderRootDatalogsOnSDcardAsync();
            }
            if (oFold is object) return oFold;

            // albo w UWP nie ma karty, albo poza UWP
            if (!bUseOwnFolderIfNotSD) return null;
            oFold = await GetLogFolderRootDatalogsInAppAsync();

            return oFold;
        }
#if PKAR_USEDATALOG

        /// <summary>
        /// do wywolania raz, na poczatku - inicjalizacja zmiennych w VBlib (sciezki root)
        /// </summary>
        public async static System.Threading.Tasks.Task InitDatalogFolder(bool bUseOwnFolderIfNotSD = true)
        {
            Windows.Storage.StorageFolder oFold = await GetLogFolderRootAsync(bUseOwnFolderIfNotSD);
            if (oFold is null) return;
            vb14.LibInitDataLog(oFold.Path);
        }
#endif

#endregion

    //    public static Windows.Devices.Geolocation.BasicGeoposition GetDomekGeopos(UInt16 iDecimalDigits = 0)
    //    {
    //        switch (iDecimalDigits)
    //        {
    //            case 1:
    //                return NewBasicGeoposition(50.0, 19.9);
    //            case 2:
    //                return NewBasicGeoposition(50.01, 19.97);
    //            case 3:
    //                return NewBasicGeoposition(50.019, 19.978);
    //            case 4:
    //                return NewBasicGeoposition(50.0198, 19.9787);
    //            case 5:
    //                return NewBasicGeoposition(50.01985, 19.97872);
    //            default:
    //                return NewBasicGeoposition(50, 20);
    //        }
    //}

        //public static Windows.Devices.Geolocation.BasicGeoposition NewBasicGeoposition(double dLat, double dLon)
        //{
        //    var oPoint = new Windows.Devices.Geolocation.BasicGeoposition
        //    {
        //        Latitude = dLat,
        //        Longitude = dLon
        //    };
        //    return oPoint;
        //}

        // supress, bo dla Android i DEBUG nie ma await, ale kiedy indziej jest
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async System.Threading.Tasks.Task<bool> IsFullVersion()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
#if DEBUG
            return true;
#else
#if !NETFX_CORE
            return false;
#else
            // if(IsThisMoje()) return true;

            if (!Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
                return false;

            var oLicencja = await Windows.Services.Store.StoreContext.GetDefault().GetAppLicenseAsync();
            if (!oLicencja.IsActive) return false; // bez licencji? jakżeż to możliwe?
            if (oLicencja.IsTrial) return false;
            return true;
#endif
#endif
        }
    }


    static partial class Extensions
    {

        #region "Zapis/Odczyt tekstu z pliku"

#if NETFX_CORE
// tak ma być, dla nieUWP robię inaczej
    [Obsolete("Raczej się pozbądź, przejdź na .Net")]
    public static async System.Threading.Tasks.Task WriteAllTextAsync(this Windows.Storage.StorageFile oFile, string sTxt)
    {
        System.IO.Stream oStream = await oFile.OpenStreamForWriteAsync(); 
        Windows.Storage.Streams.DataWriter oWriter = new Windows.Storage.Streams.DataWriter(oStream.AsOutputStream()); // <- NO UNO
        oWriter.WriteString(sTxt);
        await oWriter.FlushAsync();
        await oWriter.StoreAsync(); // <- NO UNO
        oWriter.Dispose();
        // oStream.Flush()
        // oStream.Dispose()
    }

    [Obsolete("Raczej się pozbądź, przejdź na .Net")]
    public static async System.Threading.Tasks.Task<string> ReadAllTextAsync(this Windows.Storage.StorageFile oFile)
    {
        // ' zamiast File.ReadAllText(oFile.Path)
        Stream oStream = await oFile.OpenStreamForReadAsync();
        Windows.Storage.Streams.DataReader oReader = new Windows.Storage.Streams.DataReader(oStream.AsInputStream()); // <- NO UNO
        uint iSize = (uint)oStream.Length;
        await oReader.LoadAsync(iSize); // <- NO UNO
        string sTxt = oReader.ReadString(iSize);
        oReader.Dispose();
        oStream.Dispose();
        return sTxt;
    }

#elif __ANDROID__ || __IOS__
    [Obsolete("Raczej się pozbądź, przejdź na .Net")]
    public static async System.Threading.Tasks.Task<string> ReadAllTextAsync(this Windows.Storage.StorageFile oFile)
        { // niby wersja dużo prostsza, bez Readerów
            return await Windows.Storage.FileIO.ReadTextAsync(oFile);
        }

    [Obsolete("Raczej się pozbądź, przejdź na .Net")]
    public static async System.Threading.Tasks.Task WriteAllTextAsync(this Windows.Storage.StorageFile oFile, string sTxt)
        {
            await Windows.Storage.FileIO.WriteTextAsync(oFile, sTxt);
        }
#endif

#if NETFX_CORE

        [Obsolete("Raczej się pozbądź, przejdź na .Net")]
    public static async System.Threading.Tasks.Task WriteAllTextToFileAsync(this Windows.Storage.StorageFolder oFold, string sFileName, string sTxt, Windows.Storage.CreationCollisionOption oOption = Windows.Storage.CreationCollisionOption.FailIfExists)
        { // wymaga powyzszej funkcji
            Windows.Storage.StorageFile oFile = await oFold.CreateFileAsync(sFileName, oOption);
            if (oFile is null) return;
            await oFile.WriteAllTextAsync(sTxt);
        }

    [Obsolete("Raczej się pozbądź, przejdź na .Net")]
    public static async System.Threading.Tasks.Task<string> ReadAllTextFromFileAsync(this Windows.Storage.StorageFolder oFold, string sFileName)
        {
#pragma warning disable IDE0019 // Use pattern matching
            Windows.Storage.StorageFile oFile = await oFold.TryGetItemAsync(sFileName) as Windows.Storage.StorageFile;
#pragma warning restore IDE0019 // Use pattern matching
            if (oFile is null) return null;
            return await oFile.ReadAllTextAsync();
        }
#endif
    /// <summary>
    /// appenduje string, i dodaje vbCrLf
    /// </summary>
    [Obsolete("Raczej się pozbądź, przejdź na .Net")]
    public static async System.Threading.Tasks.Task AppendLineAsync(this Windows.Storage.StorageFile oFile, string sTxt)
        {
            await oFile.AppendStringAsync(sTxt + "\n");
        }

    /// <summary>
    /// appenduje string, nic nie dodając. Zwraca FALSE gdy nie udało się otworzyć pliku.
    /// </summary>
    [Obsolete("Raczej się pozbądź, przejdź na .Net")]
    public static async System.Threading.Tasks.Task<bool> AppendStringAsync(this Windows.Storage.StorageFile oFile, string sTxt)
        {
            if (string.IsNullOrEmpty(sTxt)) return true;

            System.IO.Stream oStream = null;
            try
            {
                oStream = await oFile.OpenStreamForWriteAsync();
            }
            catch
            {
                return false; // ' mamy błąd otwarcia pliku
            }

            oStream.Seek(0, System.IO.SeekOrigin.End);
            Windows.Storage.Streams.DataWriter oWriter = new Windows.Storage.Streams.DataWriter(oStream.AsOutputStream());
            oWriter.WriteString(sTxt);
            await oWriter.FlushAsync();
#if NETFX_CORE
            await oWriter.StoreAsync();
#endif
            oWriter.Dispose();

            return true;
            //'oStream.Flush() - already closed
            //'oStream.Dispose()
        }

#endregion

        public static void SetLangText(this Winek.Controls.TextBlock uiElement, string stringId)
        {
            uiElement.Text = pkar.Localize.GetResManString(stringId);
        }

        public static void SetLangText(this Winek.Controls.Button uiElement, string stringId)
        {
            uiElement.Content= pkar.Localize.GetResManString(stringId);
        }


#if NETFX_CORE
        public static void OpenExplorer(this Windows.Storage.StorageFolder oFold)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Windows.System.Launcher.LaunchFolderAsync(oFold);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }
#endif

        public static void OpenBrowser(this Uri oUri, bool bForceEdge = false)
    {
        if (bForceEdge)
        { // tylko w FilteredRss
            var options = new Windows.System.LauncherOptions()
            {
                TargetApplicationPackageFamilyName = "Microsoft.MicrosoftEdge_8wekyb3d8bbwe"
            };
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Windows.System.Launcher.LaunchUriAsync(oUri, options);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
        else
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Windows.System.Launcher.LaunchUriAsync(oUri);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }

#if NETFX_CORE
        public async static System.Threading.Tasks.Task<string> GetDocumentHtml(this Winek.Controls.WebView uiWebView)
    {
        try
        {
            return await uiWebView.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
        }
        catch
        {
            return "";
        }// jesli strona jest pusta, jest Exception
        }
#endif
#region "GPS related"

#if PK_GPS_USED
        public static pkar.BasicGeopos ToMyGeopos(this Windows.Devices.Geolocation.BasicGeoposition oPos)
        {
            return new pkar.BasicGeopos(oPos.Latitude, oPos.Longitude);
        }
#endif

#if PK_USE_GEO
        public static Windows.Devices.Geolocation.BasicGeoposition ToWinGeopos(this pkar.BasicGeopos oPos)
        {
            var oPoint = new Windows.Devices.Geolocation.BasicGeoposition()
            {
                Latitude = oPos.Latitude,
                Longitude = oPos.Longitude,
                Altitude = oPos.Altitude
            };
            // oPos.CopyTo(oPoint);
            //{
            //    Latitude = oPos.Latitude,
            //    Longitude = oPos.Longitude,
            //    Altitude = oPos.Altitude
            //};
            return oPoint;
        }

        public static Windows.Devices.Geolocation.Geopoint ToWinGeopoint(this pkar.BasicGeopos oPos)
        {
            return new Windows.Devices.Geolocation.Geopoint(oPos.ToWinGeopos());
        }


#endif
#endregion
        
        public static string ToDebugString(this Windows.Storage.Streams.IBuffer oBuf, int iMaxLen)
    {
        string sRet = oBuf.Length + ": ";

        // NIE MA W C# IBuffer.ToArray? - jest, ale wymaga USING
        Byte[] oArr = oBuf.ToArray();

        for (int i = 0; i < Math.Min(oBuf.Length - 1, iMaxLen); i++)
#pragma warning disable CA1305 // Specify IFormatProvider
            sRet = sRet + oArr.ElementAt(i).ToString("X2") + " ";
#pragma warning restore CA1305 // Specify IFormatProvider

        return sRet + "\n";
    }

#if !NETFX_CORE
        #region "Settingsy jako Extension"

        public static void GetSettingsString(this Winek.Controls.TextBlock oItem, string sName = "", string sDefault = "")
    {
        if (sName == "") sName = oItem.Name;
        string sTxt = vb14.GetSettingsString(sName, sDefault);
        oItem.Text = sTxt;
    }

    public static void SetSettingsString(this Winek.Controls.TextBlock oItem, string sName = "", bool bRoam = false)
    {
        if (sName == "") sName = oItem.Name;
        vb14.SetSettingsString(sName, oItem.Text, bRoam);
    }


public static void GetSettingsString(this Winek.Controls.TextBox oItem, string sName = "", string sDefault = "")
{
    if (sName == "") sName = oItem.Name;
        string sTxt = vb14.GetSettingsString(sName, sDefault);
        oItem.Text = sTxt;
    }

public static void SetSettingsString(this Winek.Controls.TextBox oItem, string sName = "", bool bRoam = false) 
{        if(sName == "") sName = oItem.Name;
        vb14.SetSettingsString(sName, oItem.Text, bRoam);
    }


public static void GetSettingsBool(this Winek.Controls.ToggleSwitch oItem, string sName = "", bool bDefault = false)
{
    if (sName == "") sName = oItem.Name;
        bool bBool = vb14.GetSettingsBool(sName, bDefault);
        oItem.IsOn = bBool;
    }

public static void SetSettingsBool(this Winek.Controls.ToggleSwitch oItem, string sName = "", bool bRoam = false)
{
    if (sName == "") sName = oItem.Name;
        vb14.SetSettingsBool(sName, oItem.IsOn, bRoam);
    }


public static void GetSettingsBool(this Winek.Controls.Primitives.ToggleButton oItem, string sName = "", bool bDefault = false)
{
    if (sName == "") sName = oItem.Name;
        bool bBool = vb14.GetSettingsBool(sName, bDefault);
        oItem.IsChecked = bBool;
    }

public static void SetSettingsBool(this Winek.Controls.Primitives.ToggleButton oItem, string sName = "", bool bRoam = false)
{
    if (sName == "") sName = oItem.Name;
        vb14.SetSettingsBool(sName, oItem.IsChecked ?? false, bRoam);
    }


public static void GetSettingsBool(this Winek.Controls.AppBarToggleButton oItem, string sName = "", bool bDefault = false)
{
    if (sName == "") sName = oItem.Name;
        bool bBool = vb14.GetSettingsBool(sName, bDefault);
        oItem.IsChecked = bBool;
    }

    public static void SetSettingsBool(this Winek.Controls.AppBarToggleButton oItem, string sName = "", bool bRoam = false)
    {
        if (sName == "") sName = oItem.Name;
        vb14.SetSettingsBool(sName, oItem.IsChecked ?? false, bRoam);
    }

    public static void SetSettingsInt(this Winek.Controls.Slider oItem, string sName = "", bool bRoam = false)
    {
        if (sName == "") sName = oItem.Name;
        vb14.SetSettingsInt(sName, (int)oItem.Value, bRoam);
    }

    public static void GetSettingsInt(this Winek.Controls.Slider oItem, string sName = "", double defVal = 5)
    {
        if (sName == "") sName = oItem.Name;
        oItem.Value = vb14.GetSettingsInt(sName, (int)defVal);
    }

#endregion

#endif
        public static async void ShowAppVers(this Winek.Controls.TextBlock oItem)
    {
        string sTxt = p.k.GetAppVers();
#if DEBUG
        sTxt += " (debug " + await p.k.GetBuildTimestampAsync() + ")";
#endif
        oItem.Text = sTxt;
    }

    public static void ShowAppVers(this Winek.Controls.Page oPage)
    {
        Winek.Controls.Grid oGrid = (Winek.Controls.Grid)oPage.Content;
        if(oGrid is null)
            {
            // skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
            System.Diagnostics.Debug.WriteLine("GetAppVers(null) wymaga Grid jako podstawy Page");
            throw new ArgumentException("GetAppVers(null) wymaga Grid jako podstawy Page");
        }

        int iCols = 0;
        if (oGrid.ColumnDefinitions != null) iCols = oGrid.ColumnDefinitions.Count; // może być 0
        int iRows = 0;
        if (oGrid.RowDefinitions != null) iRows = oGrid.RowDefinitions.Count; // może być 0

        Winek.Controls.TextBlock oTB = new Winek.Controls.TextBlock
        {
            Name = "uiPkAutoVersion",
            VerticalAlignment = Winek.VerticalAlignment.Center,
            HorizontalAlignment = Winek.HorizontalAlignment.Center,
            FontSize = 10
        };

        if (iRows > 2) Winek.Controls.Grid.SetRow(oTB, 1);
        if (iCols > 1)
        {
            Winek.Controls.Grid.SetColumn(oTB, 0);
            Winek.Controls.Grid.SetColumnSpan(oTB, iCols);
        }
        oGrid.Children.Add(oTB);
        oTB.ShowAppVers();
    }


        #region "MAUI_ulatwiacz"
#if false
        /// <summary>
        /// żeby było tak samo jak w MAUI, skoro nie da się w MAUI tego zrobić
        /// </summary>
        public static void GoBack(this Winek.Controls.Page oPage)
    {
        oPage.Frame.GoBack();
    }


    /// <summary>
    /// żeby było tak samo jak w MAUI, skoro nie da się w MAUI tego zrobić
    /// </summary>
    public static void Navigate(this Winek.Controls.Page oPage, Type sourcePageType)
    {
        oPage.Frame.Navigate(sourcePageType);
    }
#endif
#endregion

//#region "ProgressBar/Ring"
//    //  dodałem 25 X 2020

//    private static int _mProgRingShowCnt; // = 0;

//        public static void ProgRingInit(this Winek.Controls.Page oPage, bool bRing, bool bBar)
//        {
//            // 2020.11.24: dodaję force-off do ProgRing na Init
//            _mProgRingShowCnt = 0;   // skoro inicjalizuje, to znaczy że na pewno trzeba wyłączyć

//#pragma warning disable IDE0019 // Use pattern matching
//            Winek.Controls.Grid oGrid = oPage?.Content as Winek.Controls.Grid;
//#pragma warning restore IDE0019 // Use pattern matching
//            if (oGrid is null)
//            {
//                // skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
//                vb14.DebugOut("ProgRingInit wymaga Grid jako podstawy Page");
//                throw new ArgumentException("ProgRingInit wymaga Grid jako podstawy Page");
//            }

//            // *TODO* sprawdz czy istnieje juz taki Control?

//            int iCols = 0;
//            if (oGrid.ColumnDefinitions is object)
//                iCols = oGrid.ColumnDefinitions.Count; // moze byc 0
//            int iRows = 0;
//            if (oGrid.RowDefinitions is object)
//                iRows = oGrid.RowDefinitions.Count; // moze byc 0

//            if (bRing && (oPage.FindName("uiPkAutoProgRing") is null))
//            {
//                var _mProgRing = new Winek.Controls.ProgressRing
//                {
//                    Name = "uiPkAutoProgRing",
//                    VerticalAlignment = Winek.VerticalAlignment.Center,
//                    HorizontalAlignment = Winek.HorizontalAlignment.Center,
//                    Visibility = Winek.Visibility.Collapsed
//                };
//                Winek.Controls.Canvas.SetZIndex(_mProgRing, 10000);



//                if (iRows > 1)
//                {
//                    Winek.Controls.Grid.SetRow(_mProgRing, 0);
//                    Winek.Controls.Grid.SetRowSpan(_mProgRing, iRows);
//                }

//                if (iCols > 1)
//                {
//                    Winek.Controls.Grid.SetColumn(_mProgRing, 0);
//                    Winek.Controls.Grid.SetColumnSpan(_mProgRing, iCols);
//                }

//                oGrid.Children.Add(_mProgRing);
//            }

//            if (bBar && (oPage.FindName("uiPkAutoProgBar") is null))
//            {
//                Winek.Controls.ProgressBar _mProgBar = new Winek.Controls.ProgressBar
//                {
//                    Name = "uiPkAutoProgBar",
//                    VerticalAlignment = Winek.VerticalAlignment.Bottom,
//                    HorizontalAlignment = Winek.HorizontalAlignment.Stretch,
//                    Visibility = Winek.Visibility.Collapsed
//                };
//                Winek.Controls.Canvas.SetZIndex(_mProgBar, 10000);
//                if (iRows > 1)
//                    Winek.Controls.Grid.SetRow(_mProgBar, iRows - 1);
//                if (iCols > 1)
//                {
//                    Winek.Controls.Grid.SetColumn(_mProgBar, 0);
//                    Winek.Controls.Grid.SetColumnSpan(_mProgBar, iCols);
//                }

//                oGrid.Children.Add(_mProgBar);
//            }

//            if (oPage.FindName("uiPkAutoProgText") is null)
//            {
//                //var uiSett = new Windows.UI.ViewManagement.UISettings();
//                //var color = uiSett.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
//                var color = (Windows.UI.Color)oPage.Resources["SystemAccentColor"];

//                var _mProgText = new Winek.Controls.TextBlock
//                {
//                    Name = "uiPkAutoProgText",
//                    VerticalAlignment = Winek.VerticalAlignment.Center,
//                    HorizontalAlignment = Winek.HorizontalAlignment.Center,
//                    Visibility = Winek.Visibility.Collapsed,
//                    Foreground = new Winek.Media.SolidColorBrush(color)
//                };
//                Winek.Controls.Canvas.SetZIndex(_mProgText, 10000);

//                if (iRows > 1)
//                {
//                    Winek.Controls.Grid.SetRow(_mProgText, 0);
//                    Winek.Controls.Grid.SetRowSpan(_mProgText, iRows);
//                }

//                if (iCols > 1)
//                {
//                    Winek.Controls.Grid.SetColumn(_mProgText, 0);
//                    Winek.Controls.Grid.SetColumnSpan(_mProgText, iCols);
//                }

//                oGrid.Children.Add(_mProgText);
//            }
//        }
//            public static void ProgRingShow(this Winek.Controls.Page oPage, bool bVisible, bool bForce = false, double dMin = 0d, double dMax = 100d)
//    {
//        var _mProgBar = (Winek.Controls.ProgressBar)oPage.FindName("uiPkAutoProgBar");
//            var _mProgText = (Winek.Controls.TextBlock)oPage.FindName("uiPkAutoProgText");
//            var _mProgRing = (Winek.Controls.ProgressRing)oPage.FindName("uiPkAutoProgRing");

//        if (_mProgBar is object)
//        {
//            _mProgBar.Minimum = dMin;
//            _mProgBar.Value = dMin;
//            _mProgBar.Maximum = dMax;
//        }

//        if (bForce)
//        {
//            if (bVisible)
//            {
//                _mProgRingShowCnt = 1;
//            }
//            else
//            {
//                _mProgRingShowCnt = 0;
//            }
//        }
//        else if (bVisible)
//        {
//            _mProgRingShowCnt += 1;
//        }
//        else
//        {
//            _mProgRingShowCnt -= 1;
//        }

//        vb14.DebugOut("ProgRingShow(" + bVisible + ", " + bForce + "...), current ShowCnt=" + _mProgRingShowCnt);

//        try
//        {
//            if (_mProgRingShowCnt > 0)
//            {
//                vb14.DebugOut("ProgRingShow - mam pokazac");
//                if (_mProgRing is object)
//                {
//                    double dSize;
//                    var oGrid = _mProgRing.Parent as Winek.Controls.Grid;
//                    dSize = Math.Min(oGrid.ActualHeight, oGrid.ActualWidth) / 2;
//                    dSize = Math.Max(dSize, 200); // jakby jeszcze nie było ustawione (Android!)
//                    _mProgRing.Width = dSize;
//                    _mProgRing.Height = dSize;
//                    _mProgRing.Visibility = Winek.Visibility.Visible;
//                    _mProgRing.IsActive = true;
//                }

//                if (_mProgBar is object)
//                    _mProgBar.Visibility = Winek.Visibility.Visible;

//                    if (_mProgText is object)
//                    {
//                        _mProgText.Visibility = Winek.Visibility.Visible;
//                        _mProgText.Text = "";
//                    }

//                }
//                else
//            {
//                vb14.DebugOut("ProgRingShow - mam ukryc");
//                if (_mProgRing is object)
//                {
//                    _mProgRing.Visibility = Winek.Visibility.Collapsed;
//                    _mProgRing.IsActive = false;
//                }

//                    if (_mProgText is object)
//                        _mProgText.Visibility = Winek.Visibility.Collapsed;


//                    if (_mProgBar is object)
//                    _mProgBar.Visibility = Winek.Visibility.Collapsed;
//            }
//        }
//        catch
//        {

//        }

//    }

//        public static void ProgRingText(this Winek.Controls.Page oPage, string message)
//        {
//            var _mProgText = (Winek.Controls.TextBlock)oPage.FindName("uiPkAutoProgText");
//            if (_mProgText is object)
//                _mProgText.Text = message;
//            else
//                throw new ArgumentException("ProgRingText called, ale nie ma uiPkAutoProgText");

//        }

//        public static void ProgRingVal(this Winek.Controls.Page oPage, double dValue)
//    {
//        var _mProgBar = (Winek.Controls.ProgressBar)oPage.FindName("uiPkAutoProgBar");
//        if (_mProgBar is null)
//        {
//            // skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
//            vb14.DebugOut("ProgRing(double) wymaga wczesniej ProgRingInit");
//            throw new ArgumentException("ProgRing(double) wymaga wczesniej ProgRingInit");
//        }

//        _mProgBar.Value = dValue;
//    }

//    public static void ProgRingInc(this Winek.Controls.Page oPage)
//    {
//        var _mProgBar = (Winek.Controls.ProgressBar)oPage.FindName("uiPkAutoProgBar");
//        if (_mProgBar is null)
//        {
//            // skoro to nie Grid, to nie ma jak umiescic koniecznych elementow
//            vb14.DebugOut("ProgRing(double) wymaga wczesniej ProgRingInit");
//            throw new ArgumentException("ProgRing(double) wymaga wczesniej ProgRingInit");
//        }

//        double dVal = _mProgBar.Value + 1;
//        if (dVal > _mProgBar.Maximum)
//        {
//            vb14.DebugOut("ProgRingInc na wiecej niz Maximum?");
//            _mProgBar.Value = _mProgBar.Maximum;
//        }
//        else
//        {
//            _mProgBar.Value = dVal;
//        }
//    }


//#endregion


    //public static async System.Threading.Tasks.Task<bool> FileExistsAsync(this Windows.Storage.StorageFolder oFold, string sFileName)
    //{
    //    try
    //    {
    //        Windows.Storage.StorageFile oTemp = (Windows.Storage.StorageFile)await oFold.TryGetItemAsync(sFileName);
    //        if (oTemp is null) return false;
    //        return true;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}


    // Install-Package Microsoft.VisualBasic

#region Bluetooth debug strings

        public static string ToDebugString(this Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisement oAdv)
        {
            if (oAdv is null)
            {
                return "ERROR: Advertisement is Nothing, unmoglich!";
            }

            string sRet = "";
            if (oAdv.DataSections is object)
            {
                sRet = sRet + "Adverisement, number of data sections: " + oAdv.DataSections.Count + "\n";
                foreach (Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementDataSection oItem in oAdv.DataSections)
                    sRet = sRet + " DataSection: " + oItem.Data.ToDebugString(32);
            }

            if (oAdv.Flags is object)
                sRet = sRet + "Adv.Flags: " + oAdv.Flags + "\n";
            sRet = sRet + "Adv local name: " + oAdv.LocalName + "\n";
            if (oAdv.ManufacturerData is object)
            {
                foreach (Windows.Devices.Bluetooth.Advertisement.BluetoothLEManufacturerData oItem in oAdv.ManufacturerData)
                {
                    sRet = sRet + " ManufacturerData.Company: " + oItem.CompanyId + "\n";
                    sRet = sRet + " ManufacturerData.Data: " + oItem.Data.ToDebugString(32) + "\n";
                }
            }

            if (oAdv.ServiceUuids is object)
            {
                foreach (Guid oItem in oAdv.ServiceUuids)
                    sRet = sRet + " service " + oItem.ToString() + "\n";
            }

            return sRet;
        }

        public static async System.Threading.Tasks.Task<string> ToDebugStringAsync(this Windows.Devices.Bluetooth.GenericAttributeProfile.GattDescriptor oDescriptor)
        {
            string sRet;
            sRet = "      descriptor: " + oDescriptor.Uuid.ToString() + "\t" + oDescriptor.Uuid.AsGattReservedDescriptorName() + "\n";
            var oRdVal = await oDescriptor.ReadValueAsync();
            if (oRdVal.Status == Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                var oVal = oRdVal.Value;
                sRet = sRet + oVal.ToArray().ToDebugString(8) + "\n"; // wymaga USING
            }
            else
            {
                sRet = sRet + "      ReadValueAsync status = " + oRdVal.Status.ToString() + "\n";
            }

            return sRet;
        }

        public static string ToDebugString(this Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties oProp)
        {
            string sRet = "      CharacteristicProperties: ";
            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Read))
            {
                sRet += "[read] ";
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.AuthenticatedSignedWrites))
            {
                sRet += "[AuthenticatedSignedWrites] ";
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Broadcast))
            {
                sRet += "[broadcast] ";
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Indicate))
            {
                sRet += "[indicate] ";
                // bCanRead = False
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.None))
            {
                sRet += "[NONE] ";
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Notify))
            {
                sRet += "[notify] ";
                // bCanRead = False
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.ReliableWrites))
            {
                sRet += "[reliableWrite] ";
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.Write))
            {
                sRet += "[write] ";
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.WritableAuxiliaries))
            {
                sRet += "[WritableAuxiliaries] ";
            }

            if (oProp.HasFlag(Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicProperties.WriteWithoutResponse))
            {
                sRet += "[writeNoResponse] ";
            }

            return sRet;
        }

        public static async System.Threading.Tasks.Task<string> ToDebugStringAsync(this Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic oChar)
        {
            string sRet = "      CharacteristicProperties: " + oChar.CharacteristicProperties.ToDebugString() + "\n";
            bool bCanRead = false;
            if (sRet.Contains("[read]"))
                bCanRead = true;
            // ewentualnie wygaszenie gdy:
            // sProp &= "[indicate] "
            // bCanRead = False
            // sProp &= "[notify] "
            // bCanRead = False


            var oDescriptors = await oChar.GetDescriptorsAsync();
            if (oDescriptors is null)
                return sRet;
            if (oDescriptors.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                sRet = sRet + "      GetDescriptorsAsync.Status = " + oDescriptors.Status.ToString() + "\n";
                return sRet;
            }

            foreach (var oDescr in oDescriptors.Descriptors)
                sRet = sRet + await oDescr.ToDebugStringAsync() + "\n";
            if (bCanRead)
            {
                var oRd = await oChar.ReadValueAsync();
                if (oRd.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
                {
                    sRet = sRet + "ReadValueAsync.Status=" + oRd.Status + "\n";
                }
                else
                {
                    sRet = sRet + "      characteristic data (read):" + "\n";
                    //sRet = sRet + oRd.Value.ToArray().ToDebugString(8) + "\n"; NIE MA TOARRAY
                }
            }

            return sRet;
        }

        public static async System.Threading.Tasks.Task<string> ToDebusStringAsync(this Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceService oServ)
        {
            if (oServ is null)
                return "";
            var oChars = await oServ.GetCharacteristicsAsync();
            if (oChars is null)
                return "";
            if (oChars.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                return "    GetCharacteristicsAsync.Status = " + oChars.Status.ToString();
            }

            string sRet = "";
            foreach (var oChr in oChars.Characteristics)
            {
                sRet = sRet + "\n    characteristic: " + oChr.Uuid.ToString() + oChr.Uuid.AsGattReservedCharacteristicName() + "\n";
                sRet = sRet + await oChr.ToDebugStringAsync() + "\n";
            }

            return sRet;
        }

        public static async System.Threading.Tasks.Task<string> ToDebusStringAsync(this Windows.Devices.Bluetooth.BluetoothLEDevice oDevice)
        {

            // If oDevice.BluetoothAddress = mLastBTdeviceDumped Then
            // DebugOut("DebugBTdevice, but MAC same as previous - skipping")
            // Return
            // End If
            // mLastBTdeviceDumped = oDevice.BluetoothAddress

            string sRet = "";
            sRet = sRet + "DebugBTdevice, data dump:" + "\n";
            sRet = sRet + "Device name: " + oDevice.Name + "\n";
            sRet = sRet + "MAC address: " + oDevice.BluetoothAddress.ToHexBytesString() + "\n";
            sRet = sRet + "Connection status: " + oDevice.ConnectionStatus.ToString() + "\n";
            var oDAI = oDevice.DeviceAccessInformation;
            sRet = sRet + "\n" + "DeviceAccessInformation:" + "\n";
            sRet = sRet + "  CurrentStatus: " + oDAI.CurrentStatus.ToString() + "\n";
            var oDApperr = oDevice.Appearance;
            sRet = sRet + "\nAppearance:" + "\n";
            sRet = sRet + "  Category: " + oDApperr.Category + "\n";
            sRet = sRet + "  Subcategory: " + oDApperr.SubCategory + "\n";
            sRet = sRet + "Services: " + oDApperr.SubCategory + "\n";
            var oSrv = await oDevice.GetGattServicesAsync();
            if (oSrv.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                sRet = sRet + "  GetGattServicesAsync.Status = " + oSrv.Status.ToString() + "\n";
                return sRet;
            }

            foreach (var oSv in oSrv.Services)
            {
                sRet = sRet + "\n  service: " + oSv.Uuid.ToString() + "\t\t" + oSv.Uuid.AsGattReservedServiceName() + "\n";
                sRet = sRet + await oSv.ToDebusStringAsync() + "\n";
            }
            return sRet;
        }


#endregion


    }

}

#region ".Net configuration - UWP settings"

namespace p
{

    // to miałby być .Nuget

    // Nugetowanie: https://docs.microsoft.com/en-us/nuget/guides/create-uwp-packages
    // https://stackoverflow.com/questions/34612015/how-to-package-a-net-library-that-targets-the-universal-windows-platform-and-de
    // https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-visual-studio?tabs=netcore-cli

    // ale w RunTime nie może być dziedziczenia, więc tak się niestety nie da.



    // na wzór
    // https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.Extensions.Configuration.CommandLine/src/


    public partial class UwpConfigurationProvider : Microsoft.Extensions.Configuration.ConfigurationProvider
    {
        private void LoadData(Windows.Foundation.Collections.IPropertySet settSource)
        {
            foreach (var oItem in settSource)
                Data[oItem.Key] = (string)oItem.Value;
        }

        public override void Load()
        {
            LoadData(Windows.Storage.ApplicationData.Current.RoamingSettings.Values);
            LoadData(Windows.Storage.ApplicationData.Current.LocalSettings.Values);
        }

        public override void Set(string key, string value)
        {
            if (value is null) value = "";

            if (value.ToUpperInvariant().StartsWithOrdinal("[ROAM]"))
            {
#pragma warning disable IDE0057 // Use range operator
                value = value.Substring("[roam]".Length);
#pragma warning restore IDE0057 // Use range operator
                try
                {
                    Windows.Storage.ApplicationData.Current.RoamingSettings.Values[key] = value;
                }
                catch
                { // za długa wartość?}
                }
            }

            Data[key] = value;
            try
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = value;
            }
            catch
            { // za długa wartość?
            }
        }
    }

        public partial class UwpConfigurationSource : Microsoft.Extensions.Configuration.IConfigurationSource
        {
            public Microsoft.Extensions.Configuration.IConfigurationProvider Build(Microsoft.Extensions.Configuration.IConfigurationBuilder builder)
            {
                return new UwpConfigurationProvider();
            }
        }
    }

    internal static partial class Extensions
    {
        public static Microsoft.Extensions.Configuration.IConfigurationBuilder AddUwpSettings(this Microsoft.Extensions.Configuration.IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Add(new p.UwpConfigurationSource());
            return configurationBuilder;
        }
    }

#endregion

#region "Konwertery Bindings XAML"

namespace pkarConv
{
    // parameter = NEG robi negację
    public class KonwersjaVisibility : Winek.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool bTemp = (bool)value;
            if (parameter != null)
            {
                string sParam = (string)parameter;
                if (sParam.ToUpperInvariant() == "NEG") bTemp = !bTemp;
            }

            if (bTemp) return Winek.Visibility.Visible;
            return Winek.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // ConvertBack is not implemented for a OneWay binding.
            throw new NotImplementedException();
        }
    }

    // ULONG to String
    public class KonwersjaMAC : Winek.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ulong uMAC = (ulong)value;
            if (uMAC == 0) return "";

            return uMAC.ToHexBytesString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // ConvertBack is not implemented for a OneWay binding.
            throw new NotImplementedException();
        }
    }

}
#endregion
