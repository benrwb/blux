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
            // Use Task Scheduler to start this program at the desired time of day

            InitializeComponent();

            //MagInitialize();


            _multiplier = (slider1.Maximum - slider1.Minimum) / (slider2.Maximum - slider2.Minimum);
            _offset = slider2.Minimum - (slider1.Minimum / _multiplier);

           
            t.Elapsed += t_Elapsed;
            t.Start();
        }

        Timer t = new Timer() { Interval = 500 };

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke((Action)delegate
            {
                slider1.Value--;
                if (slider1.Value == 3400)
                    t.Enabled = false; // stop the timer at 3400K
            });
        }

      
        private void chkTimer_Click(object sender, RoutedEventArgs e)
        {
            t.Enabled = chkTimer.IsChecked.Value;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            //MagUninitialize();
        }

  
        //[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool MagInitialize();


        //[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool MagUninitialize();


        //[DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //public static extern bool MagSetFullscreenColorEffect( // Requires Windows 8 or above
        //    [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4)] float[,] pEffect
        //);

        // http://www.pinvoke.net/default.aspx/gdi32/setdevicegammaramp.html
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Blue;
        }

        public static void SetGamma(double red, double green, double blue)
        {
            if (red < 0.0 || red > 1.0 ||
                green < 0.0 || green > 1.0 ||
                blue < 0.0 || blue > 1.0)
                throw new Exception("Multiplier out of range");

            RAMP ramp = new RAMP();
            ramp.Red = new ushort[256];
            ramp.Green = new ushort[256];
            ramp.Blue = new ushort[256];

            for (int i = 0; i <= 255; i++)
            {
                ramp.Red[i] = (ushort)(Convert.ToByte(i * red) << 8);
                ramp.Green[i] = (ushort)(Convert.ToByte(i * green) << 8);
                ramp.Blue[i] = (ushort)(Convert.ToByte(i * blue) << 8);
            }
            SetDeviceGammaRamp(GetDC(IntPtr.Zero), ref ramp);
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

     
        

      

        private void Set(float[,] matrix)
        {
            StringBuilder sb = new StringBuilder();
            for (int a = 0; a < 5; a++)
                for (int b = 0; b < 5; b++)
                    sb.Append("\t" + matrix[a, b]);
            txtEditor.Text = sb.ToString();


            //MagSetFullscreenColorEffect(matrix);
        }

        private void txtEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            var vals = txtEditor.Text.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (vals.Length != 3) return;
            SetGamma(
                Convert.ToDouble(vals[0]),
                Convert.ToDouble(vals[1]),
                Convert.ToDouble(vals[2])
                );

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider1 == null || slider2 == null ||chkLink == null) return;
            if (chkLink.IsChecked == true)
            {
                if (sender == slider1)
                    slider2.Value = Math.Round((slider1.Value / _multiplier) + _offset, 2);
                if (sender == slider2)
                    slider1.Value = Math.Round((slider2.Value- _offset) * _multiplier, 0);
            }

            double red, green, blue;
            ColorTemp(slider1.Value, out red, out green, out blue);
            var rrrr = (float)Math.Round(red / 255, 2);
            var gggg = (float)Math.Round(green / 255, 2);
            var bbbb = (float)Math.Round(blue / 255, 2);
            
            // BEGIN brightness
            rrrr = rrrr * (float)(slider2.Value / 100);
            gggg = gggg * (float)(slider2.Value / 100);
            bbbb = bbbb * (float)(slider2.Value / 100);
            // END brightness

            txtEditor.Text = string.Format("{0:N2}\t{1:N2}\t{2:N2}", rrrr, gggg, bbbb);
        }








        private void ColorTemp(double temp, out double Red, out double Green, out double Blue)
        {
            // http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/ 
            // Start with a temperature, in Kelvin, somewhere between 1000 and 40000.  (Other values may work,
            // but I can't make any promises about the quality of the algorithm's estimates above 40000 K.)
            // Note also that the temperature and color variables need to be declared as floating-point.

            var Temperature = temp / 100;

            // Calculate Red:
            if (Temperature <= 66)
            {
                Red = 255;
            }
            else
            {
                Red = Temperature - 60;
                Red = 329.698727446 * Math.Pow(Red, -0.1332047592);
                if (Red < 0) Red = 0;
                if (Red > 255) Red = 255;
            }

            // Calculate Green:
            if (Temperature <= 66)
            {
                Green = Temperature;
                Green = 99.4708025861 * Math.Log(Green) - 161.1195681661;
                if (Green < 0) Green = 0;
                if (Green > 255) Green = 255;
            }
            else
            {
                Green = Temperature - 60;
                Green = 288.1221695283 * Math.Pow(Green, -0.0755148492);
                if (Green < 0) Green = 0;
                if (Green > 255) Green = 255;
            }

            // Calculate Blue:
            if (Temperature >= 66)
            {
                Blue = 255;
            }
            else
            {
                if (Temperature <= 19)
                {
                    Blue = 0;
                }
                else
                {
                    Blue = Temperature - 10;
                    Blue = 138.5177312231 * Math.Log(Blue) - 305.0447927307;
                    if (Blue < 0) Blue = 0;
                    if (Blue > 255) Blue = 255;
                }
            }
        }

     
     

        


     
    }
}
