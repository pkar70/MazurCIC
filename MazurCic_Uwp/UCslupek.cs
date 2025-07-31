using System;
using System.Collections.Generic;
using System.Text;

#if !NETFX_CORE
using RootUI = Microsoft.UI;
using RootXAML = Microsoft.UI.Xaml;
using RootCtrl = Microsoft.UI.Xaml.Controls;
#else
using RootUI = Windows.UI;
using RootXAML = Windows.UI.Xaml;
using RootCtrl = Windows.UI.Xaml.Controls;
#endif

namespace MazurCiC
{
    class UCslupek : RootCtrl.Grid
    {
        public UCslupek()
        {
            this.InitializeComponent();
        }


        public string Text
        {
            get { return _TxtBlk.Text; }
            set { _TxtBlk.Text = value; }
        }

        public double Wysokosc
        {
            get { return _RowDef.Height.Value; }
            set { _RowDef.Height = new RootXAML.GridLength(value, RootXAML.GridUnitType.Pixel); }
        }

        private RootCtrl.RowDefinition _RowDef = new RootCtrl.RowDefinition { Height = new RootXAML.GridLength(0, RootXAML.GridUnitType.Pixel) };
        private RootCtrl.TextBlock _TxtBlk = new RootCtrl.TextBlock { HorizontalAlignment = RootXAML.HorizontalAlignment.Center, VerticalAlignment = RootXAML.VerticalAlignment.Bottom };

        private RootCtrl.Grid _GrdBlue = new RootCtrl.Grid { Background = new RootXAML.Media.SolidColorBrush(RootUI.Colors.LightSkyBlue) };

        private void InitializeComponent()
        {
            // Initialization logic for the user control can be added here.
            // This is a placeholder for the actual implementation.

            this.RowDefinitions.Add(new RootCtrl.RowDefinition { Height = new RootXAML.GridLength(1, RootXAML.GridUnitType.Star) });
            this.RowDefinitions.Add(_RowDef);

            _GrdBlue.SetValue(RootCtrl.Grid.RowProperty, 1);
            this.Children.Add(_GrdBlue);

            _TxtBlk.SetValue(RootCtrl.Grid.RowSpanProperty, 2);
            this.Children.Add(_TxtBlk);

            //< Grid Grid.Column = "1" Grid.Row = "1" >
            //    < Grid.RowDefinitions >
            //        < RowDefinition Height = "*" />
            //        < RowDefinition x: Name = "grEgzoDyn" Height = "0" />
            //    </ Grid.RowDefinitions >
            //    < Grid Grid.Row = "1" Background = "LightSkyBlue" />
            //    < TextBlock HorizontalAlignment = "Center" Grid.RowSpan = "2" x: Uid = "uiEgzodynamik" Text = "egzodynamik" VerticalAlignment = "Bottom" />
            //</ Grid >

        }

    }
}
