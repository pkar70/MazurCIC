using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MazurCiC
{
    class UCslupek : Grid
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
            set { _RowDef.Height = new GridLength(value, GridUnitType.Pixel); }
        }

        private RowDefinition _RowDef = new RowDefinition { Height = new GridLength(0, GridUnitType.Pixel) };
        private TextBlock _TxtBlk = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center , VerticalAlignment = VerticalAlignment.Bottom };  

        private Grid _GrdBlue = new Grid {Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.LightSkyBlue) };

    private void InitializeComponent()
        {
            // Initialization logic for the user control can be added here.
            // This is a placeholder for the actual implementation.

            this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            this.RowDefinitions.Add(_RowDef);

            _GrdBlue.SetValue(Grid.RowProperty, 1);
            this.Children.Add(_GrdBlue);

            _TxtBlk.SetValue(Grid.RowSpanProperty, 2);
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
