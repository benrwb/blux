﻿// Inspiration from http://arcanesanctum.net/negativescreen/
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

           
            m_timer.Elapsed += t_Elapsed;
            m_timer.Start();

         
            tb1.Text = _initialTimes;
            _todLookup = MainMain.BuildTimeOfDayLookup(tb1.Text);
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
                slider1.Value = _todLookup[(int)DateTime.Now.TimeOfDay.TotalSeconds];
                ////slider1.Value = MainMain.TempFromNow();
                //double red, green, blue;
                //MainMain.FadeToRed_FromNow(out red, out green, out blue);
                //txtEditor.Text = string.Format("{0}\t{1}\t{2}", red, green, blue);
            });
        }




        
      
        private void chkTimer_Click(object sender, RoutedEventArgs e)
        {
            m_timer.Enabled = chkTimer.IsChecked.Value;
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

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            _todLookup = MainMain.BuildTimeOfDayLookup(tb1.Text);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tb1.Text = _initialTimes;
            _todLookup = MainMain.BuildTimeOfDayLookup(_initialTimes);
        }

        
               


     
    }
}
