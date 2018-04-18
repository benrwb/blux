// Inspiration from http://arcanesanctum.net/negativescreen/
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace blux
{
    public partial class MainWindow : Window
    {
        double _multiplier, _offset; // for linking the sliders together
        Dictionary<int, int> _todLookup; // time of day lookup


       static string _defaultSettings = @"00:00	1900
05:00	1900
06:00	6500
18:30	6500
19:00	5000
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

         
            tb1.Text = LoadSettings();
            _todLookup = MainMain.BuildTimeOfDayLookup(tb1.Text);
            btnReload.IsEnabled = false;
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
                MainMain.Method1(slider1.Value, out red, out green, out blue);
            else if (Method2.IsChecked)
                MainMain.Method2(slider1.Value, out red, out green, out blue);
            else if (Method3.IsChecked)
                MainMain.Method3(intensity, out red, out green, out blue);
            else if (Method4.IsChecked)
                MainMain.Method4(intensity, out red, out green, out blue);
            else if (Method5.IsChecked)
                MainMain.Method5(intensity, out red, out green, out blue);
            else
                throw new Exception("Unknown method");

            var rrrr = (float)Math.Round(red / 255, 4);
            var gggg = (float)Math.Round(green / 255, 4);
            var bbbb = (float)Math.Round(blue / 255, 4);

            // BEGIN brightness
            rrrr = rrrr * (float)(slider2.Value / 100);
            gggg = gggg * (float)(slider2.Value / 100);
            bbbb = bbbb * (float)(slider2.Value / 100);
            // END brightness

            txtEditor.Text = string.Format("{0:N3}\t{1:N3}\t{2:N3}", rrrr, gggg, bbbb); // will trigger txtEditor_TextChanged(), which calls update()
        }

        private void txtEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
           update();
        }

        private void chkPosterise_Checked(object sender, RoutedEventArgs e)
        {
            update();
        }

        private void chkPosterise_Indeterminate(object sender, RoutedEventArgs e)
        {
            // Checkbox state is Indeterminate (IsChecked == null) while dialog is open

            CheckBox myCheckBox = e.Source as CheckBox;

            CbaPassword pwd = new CbaPassword();
            pwd.Owner = this;
            if (pwd.ShowDialog().Value)
            {
                myCheckBox.IsChecked = false;
                update();
            }
            else
            {
                myCheckBox.IsChecked = true;
            }
        }


        private void update()
        {
            var vals = txtEditor.Text.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length != 3) return;

            try
            {
                MainMain.SetGamma(
                    Convert.ToDouble(vals[0]),
                    Convert.ToDouble(vals[1]),
                    Convert.ToDouble(vals[2]),
                    chkPosterise.IsChecked == null || chkPosterise.IsChecked == true
                    );
                lblError.Content = "";
            }
            catch (Exception ex)
            {
                lblError.Content = ex.Message;
            }
        }



        private void tb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnReload != null)
            btnReload.IsEnabled = true;
        }
        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings(tb1.Text);
            _todLookup = MainMain.BuildTimeOfDayLookup(tb1.Text);
            btnReload.IsEnabled = false;
        }


        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tb1.Text = _defaultSettings;
            SaveSettings(tb1.Text);
            _todLookup = MainMain.BuildTimeOfDayLookup(tb1.Text);
            btnReload.IsEnabled = false;
        }

        private void Curves_Click(object sender, RoutedEventArgs e)
        {
            (new Curves2()).ShowDialog();
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





        private static string GetSettingsFileName()
        {
            string folderName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "blux");
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            return Path.Combine(folderName, "program.txt"); ;
        }

        private static string LoadSettings()
        {
            var fileName = GetSettingsFileName();
            if (File.Exists(fileName))
            {
                // Load previously-saved settings
                using (var sr = new StreamReader(fileName))
                {
                    return sr.ReadToEnd();
                }
            }
            else
            {
                // Start with default settings
                return _defaultSettings;
            }
        }

      

        private static void SaveSettings(string settings)
        {
            using (StreamWriter sw = new StreamWriter(GetSettingsFileName()))
            {
                sw.Write(settings);
            }
        }

      
    }
}
