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

            Button_Click_1(null, null);

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

        //var g_Default = new float[,] {
        //    /*               OUT     OUT   OUT     OUT        */
        //    /*               Red    Green  Blue   Alpha       */
        //    /* IN Red   */ { 1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
        //    /* IN Green */ { 0.0f,  1.0f,  0.0f,  0.0f,  0.0f },
        //    /* IN Blue  */ { 0.0f,  0.0f,  1.0f,  0.0f,  0.0f },
        //    /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
        //    /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
        //};

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            // http://msdn.microsoft.com/en-us/library/windows/desktop/ms533875%28v=vs.85%29.aspx
            var g_RedTest = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { 1.0f,  0.0f,  0.0f,  0.0f,  0.0f }, // 1.0 = Leave red on full
                /* IN Green */ { 0.0f,  0.5f,  0.0f,  0.0f,  0.0f }, // 0.5 = Reduce green to half
                /* IN Blue  */ { 0.0f,  0.0f,  0.5f,  0.0f,  0.0f }, // 0.5 = Reduce blue to half
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };

            MagSetFullscreenColorEffect(g_RedTest);
           
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // http://msdn.microsoft.com/en-us/library/windows/desktop/ms533875%28v=vs.85%29.aspx
            var g_AnotherTest = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { 0.8f,  0.1f,  0.1f,  0.0f,  0.0f }, 
                /* IN Green */ { 0.1f,  0.5f,  0.0f,  0.0f,  0.0f }, 
                /* IN Blue  */ { 0.1f,  0.0f,  0.5f,  0.0f,  0.0f }, 
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };

            MagSetFullscreenColorEffect(g_AnotherTest);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var g_MagEffectGrayscale = new float[,] {
                /*               OUT     OUT   OUT     OUT        */
                /*               Red    Green  Blue   Alpha       */
                /* IN Red   */ { 0.3f,  0.3f,  0.3f,  0.0f,  0.0f },
                /* IN Green */ { 0.6f,  0.6f,  0.6f,  0.0f,  0.0f },
                /* IN Blue  */ { 0.1f,  0.1f,  0.1f,  0.0f,  0.0f },
                /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }

            };

            MagSetFullscreenColorEffect(g_MagEffectGrayscale);
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
                    MagSetFullscreenColorEffect(floats);
                }
            }
            catch { }
        }

       
      

       


       


     
    }
}
