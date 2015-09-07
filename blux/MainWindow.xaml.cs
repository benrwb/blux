// Inspiration from http://arcanesanctum.net/negativescreen/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace blux
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double _multiplier, _offset; // for linking the sliders together
        Dictionary<int, int> _todLookup; // time of day lookup


        string _initialTimes = @"00:00	1900
05:00	1900
06:00	6500
18:00	6500
19:00	5500
20:00	4400
21:00	3400
22:00	2500
23:00	1900";

        public MainWindow()
        {
            InitializeComponent();


            _multiplier = (slider1.Maximum - slider1.Minimum) / (slider2.Maximum - slider2.Minimum);
            _offset = slider2.Minimum - (slider1.Minimum / _multiplier);

           
            m_timer.Tick += t_Elapsed;
            m_timer.Start();

         
            tb1.Text = _initialTimes;
            _todLookup = MainMain.BuildTimeOfDayLookup(tb1.Text);
        }

       
        DispatcherTimer m_timer = new DispatcherTimer() { Interval = System.TimeSpan.FromSeconds(1) };


        void t_Elapsed(object sender, EventArgs e)
        {
            slider1.Value = _todLookup[(int)DateTime.Now.TimeOfDay.TotalSeconds];
            ////slider1.Value = MainMain.TempFromNow();
            //double red, green, blue;
            //MainMain.FadeToRed_FromNow(out red, out green, out blue);
            //txtEditor.Text = string.Format("{0}\t{1}\t{2}", red, green, blue);
        }

              
        private void chkTimer_Click(object sender, RoutedEventArgs e)
        {
            m_timer.IsEnabled = chkTimer.IsChecked.Value;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }




      

       

       

        private void txtEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vals = txtEditor.Text.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length != 3) return;

            try
            {
                MainMain.SetGamma(
                    Convert.ToDouble(vals[0]),
                    Convert.ToDouble(vals[1]),
                    Convert.ToDouble(vals[2])
                    );
                lblError.Content = "";
            }
            catch (Exception ex)
            {
                lblError.Content = ex.Message;
            }

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider1 == null || slider2 == null || chkLink == null) return;
            if (chkLink.IsChecked == true)
            {
                if (sender == slider1)
                    slider2.Value = Math.Round((slider1.Value / _multiplier) + _offset, 2);
                if (sender == slider2)
                    slider1.Value = Math.Round((slider2.Value - _offset) * _multiplier, 0);
            }

            double red, green, blue;
            double intensity = (slider1.Value - slider1.Minimum) / (slider1.Maximum - slider1.Minimum);
            lblIntensity.Content = intensity.ToString("N2");
            if (Method1.IsChecked)
                MainMain.ColorTempToRGB1(slider1.Value, out red, out green, out blue);
            else if (Method2.IsChecked)
                MainMain.ColorTempToRGB2(slider1.Value, out red, out green, out blue);
            else if (Method3.IsChecked)
                MainMain.Method3(intensity, out red, out green, out blue);
            else if (Method4.IsChecked)
                MainMain.Method4(intensity, out red, out green, out blue);
            else // method 
                MainMain.Method5(intensity, out red, out green, out blue);

            var rrrr = (float)Math.Round(red / 255, 4);
            var gggg = (float)Math.Round(green / 255, 4);
            var bbbb = (float)Math.Round(blue / 255, 4);

            // BEGIN brightness
            rrrr = rrrr * (float)(slider2.Value / 100);
            gggg = gggg * (float)(slider2.Value / 100);
            bbbb = bbbb * (float)(slider2.Value / 100);
            // END brightness

            txtEditor.Text = string.Format("{0:N3}\t{1:N3}\t{2:N3}", rrrr, gggg, bbbb);
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            _todLookup = MainMain.BuildTimeOfDayLookup(tb1.Text);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tb1.Text = _initialTimes;
            _todLookup = MainMain.BuildTimeOfDayLookup(_initialTimes);
        }

        private void Curves_Click(object sender, RoutedEventArgs e)
        {
            (new Curves()).ShowDialog();
        }

        private void Mag_Click(object sender, RoutedEventArgs e)
        {
            (new Mag()).ShowDialog();
        }

        private void Mixer_Click(object sender, RoutedEventArgs e)
        {
            (new Mixer()).ShowDialog();
        }

        private void Check(int number)
        {
            // there must be an easier way than this...
            Method1.IsChecked = number == 1;
            Method2.IsChecked = number == 2;
            Method3.IsChecked = number == 3;
            Method4.IsChecked = number == 4;
            Method5.IsChecked = number == 5;
            Slider_ValueChanged(null, null); // apply changes
        }

        private void Method1_Click(object sender, RoutedEventArgs e)
        {
            Check(1);
        }

        private void Method2_Click(object sender, RoutedEventArgs e)
        {
            Check(2);
        }

        private void Method3_Click(object sender, RoutedEventArgs e)
        {
            Check(3);
        }

        private void Method4_Click(object sender, RoutedEventArgs e)
        {
            Check(4);
        }

        private void Method5_Click(object sender, RoutedEventArgs e)
        {
            Check(5);
        }
    }
}
