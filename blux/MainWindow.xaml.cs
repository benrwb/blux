using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
        public MainWindow()
        {
            // Use Task Scheduler to start this program at the desired time of day

            InitializeComponent();

            MagInitialize();

            var g_Default = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { 1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
                /* IN Green */ { 0.0f,  1.0f,  0.0f,  0.0f,  0.0f },
                /* IN Blue  */ { 0.0f,  0.0f,  1.0f,  0.0f,  0.0f },
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };
            StringBuilder sb = new StringBuilder();
            foreach (var row in g_Default)
            {
                sb.Append("\t" + row.ToString());
            }
            txtEditor.Text = sb.ToString();


        }


        private void Window_Closed(object sender, EventArgs e)
        {
            MagUninitialize();
        }


        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagInitialize();


        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagUninitialize();


        [DllImport("Magnification.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool MagSetFullscreenColorEffect( // Requires Windows 8 or above
            [In][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.R4)] float[,] pEffect
        );



        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // Default
            var Default = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { 1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
                /* IN Green */ { 0.0f,  1.0f,  0.0f,  0.0f,  0.0f },
                /* IN Blue  */ { 0.0f,  0.0f,  1.0f,  0.0f,  0.0f },
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };
            Set(Default);
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            // http://msdn.microsoft.com/en-us/library/windows/desktop/ms533875%28v=vs.85%29.aspx
            var Red1 = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { 1.0f,  0.0f,  0.0f,  0.0f,  0.0f }, // 1.0 = Leave red on full
                /* IN Green */ { 0.0f,  0.5f,  0.0f,  0.0f,  0.0f }, // 0.5 = Reduce green to half
                /* IN Blue  */ { 0.0f,  0.0f,  0.5f,  0.0f,  0.0f }, // 0.5 = Reduce blue to half
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };

            Set(Red1);
           
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // http://msdn.microsoft.com/en-us/library/windows/desktop/ms533875%28v=vs.85%29.aspx
            var Red2 = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { 0.8f,  0.1f,  0.1f,  0.0f,  0.0f }, 
                /* IN Green */ { 0.1f,  0.5f,  0.0f,  0.0f,  0.0f }, 
                /* IN Blue  */ { 0.1f,  0.0f,  0.5f,  0.0f,  0.0f }, 
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };

            Set(Red2);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var Grayscale = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { 0.3f,  0.3f,  0.3f,  0.0f,  0.0f },
                /* IN Green */ { 0.6f,  0.6f,  0.6f,  0.0f,  0.0f },
                /* IN Blue  */ { 0.1f,  0.1f,  0.1f,  0.0f,  0.0f },
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }

            };

            Set(Grayscale);
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
            try
            {
                if (vals.Length == 25)
                {
                    var floats = new float[5, 5];
                    int c = 0;
                    for (int a = 0; a < 5; a++)
                        for (int b = 0; b < 5; b++)
                            floats[a, b] = Convert.ToSingle(vals[c++]);

                    txtSummary.Content = string.Format("Red: {0}%, Green: {1}%, Blue: {2}%",
                        /* Red   */ (floats[0, 0] + floats[1, 0] + floats[2, 0]) * 100,
                        /* Green */ (floats[0, 1] + floats[1, 1] + floats[2, 1]) * 100,
                        /* Blue  */ (floats[0, 2] + floats[1, 2] + floats[2, 2]) * 100
                    );

                    MagSetFullscreenColorEffect(floats);
                }
            }
            catch { }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            txtEditor.Text = "	1	0	0	0	0	0	0.66	0	0	0	0	0	0.33	0	0	0	0	0	1	0	0	0	0	0	1";
        }

        

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            txtEditor.Text = "	0.299	0	0	0	0	0.587	0.0	0	0	0	0.114	0	0.0	0	0	0	0	0	1	0	0	0	0	0	1";
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            new Mixer(txtEditor).Show();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            txtEditor.Text = "	0.70	0.04	0.02	0	0	0.30	0.19	0.11	0	0	0	0.14	0.11	0	0	0	0	0	1	0	0	0	0	0	1";
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            txtEditor.Text = "	1	0	0	0	0	0	0.66	0	0	0	0	0	0.01	0	0	0	0	0	1	0	0	0	0	0	1";
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            txtEditor.Text = "	0.6	0	0	0	0	0.4	0.0	0	0	0	0.0	0	0.0	0	0	0	0	0	1	0	0	0	0	0	1";
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider1 == null || slider2 == null) return;

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

            var Temp = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { rrrr,  0.0f,  0.0f,  0.0f,  0.0f },
                /* IN Green */ { 0.0f,  gggg,  0.0f,  0.0f,  0.0f },
                /* IN Blue  */ { 0.0f,  0.0f,  bbbb,  0.0f,  0.0f },
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };
            Set(Temp);
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
