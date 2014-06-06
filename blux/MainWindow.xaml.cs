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


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MagInitialize();


            // Initialize color transformation matrices used to apply grayscale and to 
            // restore the original screen color.
            var g_MagEffectGrayscale = new float[,] {
                { 0.3f,  0.3f,  0.3f,  0.0f,  0.0f },
                { 0.6f,  0.6f,  0.6f,  0.0f,  0.0f },
                { 0.1f,  0.1f,  0.1f,  0.0f,  0.0f },
                { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };

            // http://msdn.microsoft.com/en-us/library/windows/desktop/ms533875%28v=vs.85%29.aspx
            var g_RedTest = new float[,] {
                { 1.0f,  0.0f,  0.0f,  0.0f,  0.0f },// 1.0 = Leave red on full
                { 0.0f,  0.5f,  0.0f,  0.0f,  0.0f },// 0.5 = Reduce green to half
                { 0.0f,  0.0f,  0.5f,  0.0f,  0.0f },// 0.5 = Reduce blue to half
                { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
                { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };

            //var g_MagEffectIdentity = new float[,] {
            //    { 1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
            //    { 0.0f,  1.0f,  0.0f,  0.0f,  0.0f },
            //    { 0.0f,  0.0f,  1.0f,  0.0f,  0.0f },
            //    { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
            //    { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            //};

            MagSetFullscreenColorEffect(g_RedTest);


            //System.Threading.Thread.Sleep(1000);
            


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MagUninitialize();
        }


       


       


     
    }
}
