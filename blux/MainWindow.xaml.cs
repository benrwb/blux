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
            InitializeComponent();

            MagInitialize();

            Button_Click_1(null, null);

            // Use Task Scheduler to start this program at the desired time of day
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

       
      

       


       


     
    }
}
