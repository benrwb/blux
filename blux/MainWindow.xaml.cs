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

namespace blux
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double _multiplier, _offset; // for linking the sliders together

      

        public MainWindow()
        {
            InitializeComponent();
            ShowHideControls();


            _multiplier = (slider1.Maximum - slider1.Minimum) / (slider2.Maximum - slider2.Minimum);
            _offset = slider2.Minimum - (slider1.Minimum / _multiplier);

           
            m_timer.Elapsed += t_Elapsed;
            m_timer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }



        Timer m_timer = new Timer() { Interval = 1000 };
        
        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate
            {
                //slider1.Value = MainMain.TempFromNow();
                double red, green, blue;
                MainMain.FadeToRed_FromNow(out red, out green, out blue);
                txtEditor.Text = string.Format("{0}\t{1}\t{2}", red, green, blue);
            });
        }




        
      
        private void chkTimer_Click(object sender, RoutedEventArgs e)
        {
            m_timer.Enabled = chkTimer.IsChecked.Value;
        }
       
  






        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // Default
            txtEditor.Text = "1.00	1.00	1.00";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Pink
            txtEditor.Text = "1.00	0.50	0.50";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // Yellow
            txtEditor.Text = "1.00	0.66	0.33";
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            // Neon
            txtEditor.Text = "1.00	0.66	0.01";
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            // Red
            txtEditor.Text = "1.00	0.00	0.00";
        }

    
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            // Orange
            txtEditor.Text = "0.70	0.19	0.11";
        }

     
        

      

       

        private void sliderPosterise_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtEditor_TextChanged(null, null);
        }

        private void chkPosterise_Changed(object sender, RoutedEventArgs e)
        {
            txtEditor_TextChanged(null, null);
            ShowHideControls();
        }
        private void ShowHideControls()
        {
            System.Windows.Visibility vis = (chkPosterise.IsChecked == true) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            sliderPosterise.Visibility = vis;
            lblPosterise.Visibility = vis;
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
                    Convert.ToDouble(vals[2]),
                    chkPosterise.IsChecked == true,
                    (int)sliderPosterise.Value
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
            if (slider1 == null || slider2 == null || chkLink == null || chkPosterise == null || sliderPosterise == null) return;
            if (chkLink.IsChecked == true)
            {
                if (sender == slider1)
                    slider2.Value = Math.Round((slider1.Value / _multiplier) + _offset, 2);
                if (sender == slider2)
                    slider1.Value = Math.Round((slider2.Value - _offset) * _multiplier, 0);
            }

            double red, green, blue;
            MainMain.ColorTempToRGB(slider1.Value, out red, out green, out blue);
            var rrrr = (float)Math.Round(red / 255, 4);
            var gggg = (float)Math.Round(green / 255, 4);
            var bbbb = (float)Math.Round(blue / 255, 4);

            // BEGIN brightness
            rrrr = rrrr * (float)(slider2.Value / 100);
            gggg = gggg * (float)(slider2.Value / 100);
            bbbb = bbbb * (float)(slider2.Value / 100);
            // END brightness

            txtEditor.Text = string.Format("{0:N4}\t{1:N4}\t{2:N4}", rrrr, gggg, bbbb);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Curves().ShowDialog();
        }

        private void slider1_Copy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = slider1_Copy.Value;
            double add = Convert.ToDouble(txtAdd.Text);
            double multiply = Convert.ToDouble(txtMultiply.Text);
            txtEditor.Text = string.Format("{0:N4}\t{1:N4}\t{2:N4}", 1, Math.Min((value / 100) + add, 1.0), ((value * multiply) / 100));
        }

        private void btnInvertRed_Click(object sender, RoutedEventArgs e)
        {
            int[] greenblue = new int[256];
            int[] red = new int[256];
            for (int i = 0; i < 256; i++)
            {
                greenblue[i] = i;
                red[i] = 255 - i;
            }
            //for (int i = 0; i < 16; i++)
            //    red[i] = i;
            //for (int i = 240; i < 256; i++)
            //    red[i] = i;

                MainMain.CustomRamp(red, greenblue, greenblue);

        }

        private void btnMixer_Click(object sender, RoutedEventArgs e)
        {
            Mixer m = new Mixer();
            m.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Mag m = new Mag();
            m.ShowDialog();
        }



               


     
    }
}
