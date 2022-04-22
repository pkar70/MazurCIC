using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MazurCiC
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // ConfigureFilters(Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);
            InitializeLogging(); // nowsze Uno

            this.InitializeComponent();
            // this.Suspending += OnSuspending;
        }

        protected Windows.UI.Xaml.Controls.Frame OnLaunchFragment(Windows.UI.Xaml.Window win)
        {
            Windows.UI.Xaml.Controls.Frame mRootFrame = win.Content as Windows.UI.Xaml.Controls.Frame;

            //' Do not repeat app initialization when the Window already has content,
            //' just ensure that the window is active

            if (mRootFrame is null)
            {
                //' Create a Frame to act as the navigation context and navigate to the first page
                mRootFrame = new Windows.UI.Xaml.Controls.Frame();

                mRootFrame.NavigationFailed += OnNavigationFailed;

                ////' PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
                //mRootFrame.Navigated += OnNavigatedAddBackButton;
                //Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += OnBackButtonPressed;

                //' Place the frame in the current Window
                win.Content = mRootFrame;

                p.k.InitLib(null);
            }

            return mRootFrame;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if NETFX_CORE
            if (p.k.IsThisMoje()) Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "pl";
#endif

            Windows.UI.Xaml.Controls.Frame rootFrame = OnLaunchFragment(Windows.UI.Xaml.Window.Current);

#if NETFX_CORE
            if (e.PrelaunchActivated == false)
            {
#endif

            if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Windows.UI.Xaml.Window.Current.Activate();
#if NETFX_CORE
            }
#endif
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }


#if NETFX_CORE
        // previous UNO
        /// <summary>
        /// Configures global logging
        /// </summary>
        static void InitializeLogging()
        {
            // konieczne Microsoft.Extensions.Logging.Filter 1.1.2
            Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory.WithFilter(new FilterLoggerSettings
                    {
                        { "Uno", LogLevel.Warning },
                        { "Windows", LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },
					}
                )
#if DEBUG
                .AddConsole(LogLevel.Debug);
#else
                .AddConsole(LogLevel.Information);
#endif
        }
#else

        // nowsze Uno
        /// <summary>
        /// Configures global Uno Platform logging
        /// </summary>
        private static void InitializeLogging()
        {

            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                // builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

                // Layouter specific messages
                // builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

                // builder.AddFilter("Windows.Storage", LogLevel.Debug );

                // Binding related messages
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
                // builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

                // Binder memory references tracking
                // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

                // RemoteControl and HotReload related
                // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
            });

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
            global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
        }
#endif


        // PKAR added wedle https://stackoverflow.com/questions/39262926/uwp-hardware-back-press-work-correctly-in-mobile-but-error-with-pc
        private void OnNavigatedAddBackButton(object sender, NavigationEventArgs e)
        {
            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped EndIfDirectiveTrivia */
            var oFrame = sender as Frame;
            if (oFrame == null)
                return;

            Windows.UI.Core.SystemNavigationManager oNavig = Windows.UI.Core.SystemNavigationManager.GetForCurrentView();

            if (oFrame.CanGoBack)
                oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            else
                oNavig.AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackButtonPressed(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            try
            {
                (Window.Current.Content as Frame).GoBack();
                e.Handled = true;
            }
            catch 
            {
            }
        }

    }

}
