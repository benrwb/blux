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
using System.Windows.Shapes;

namespace blux
{
    /// <summary>
    /// Interaction logic for Mag.xaml
    /// </summary>
    /// 
    /* Invert luma:
     * 	0	-0.5	-0.5	0	0	-0.5	0	-0.5	0	0	-0.5	-0.5	0	0	0	0	0	0	1	0	1	1	1	0	1
     */
    public partial class Mag : Window
    {
        public Mag()
        {
            InitializeComponent();

            MagInitialize();

            var default_matrix = new float[,] {
            /*               OUT    OUT    OUT    OUT        */
            /*               Red    Green  Blue   Alpha      */
            /* IN Red   */ { 1.0f,  0.0f,  0.0f,  0.0f,  0.0f },
            /* IN Green */ { 0.0f,  1.0f,  0.0f,  0.0f,  0.0f },
            /* IN Blue  */ { 0.0f,  0.0f,  1.0f,  0.0f,  0.0f },
            /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
            /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };

            textBox1.Text = Matrix2Text(default_matrix);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MagUninitialize();
        }

        private string Matrix2Text(float[,] matrix)
        {
            StringBuilder sb = new StringBuilder();
            foreach (float f in matrix)
            {
                sb.Append("\t" + f.ToString("0.##"));
            }
            return sb.ToString();
        }

        private float[,] Text2Matrix(string text)
        {
            var matrix = new float[5, 5];

            var vals = text.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                if (vals.Length == 25)
                {
                    int c = 0;
                    for (int a = 0; a < 5; a++)
                        for (int b = 0; b < 5; b++)
                            matrix[a, b] = Convert.ToSingle(vals[c++]);

                    //txtSummary.Content = string.Format("Red: {0}%, Green: {1}%, Blue: {2}%",
                    //    /* Red */ (floats[0, 0] + floats[1, 0] + floats[2, 0]) * 100,
                    //    /* Green */ (floats[0, 1] + floats[1, 1] + floats[2, 1]) * 100,
                    //    /* Blue */ (floats[0, 2] + floats[1, 2] + floats[2, 2]) * 100
                    //);

                }
            }
            catch {
                return null;
            }
            return matrix;
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

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            var matrix = Text2Matrix(textBox1.Text);
            if (matrix != null)
                MagSetFullscreenColorEffect(matrix);
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (chkMono == null) return; // UI not ready

            var valu = (float)slider1.Value / 10;

            //var matrix = new float[,] {
            ///*               OUT    OUT    OUT    OUT        */
            ///*               Red    Green  Blue   Alpha      */
            ///* IN Red   */ { 0.3f,  valu,  valu,  0.0f,  0.0f },
            ///* IN Green */ { 0.3f,  valu,  valu,  0.0f,  0.0f },
            ///* IN Blue  */ { 0.3f,  valu,  valu,  0.0f,  0.0f },
            ///* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
            ///*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            //};

            //var matrix = new float[,] {
            ///*               OUT    OUT    OUT    OUT        */
            ///*               Red    Green  Blue   Alpha      */
            ///* IN Red   */ { 0.299f,  0.299f,  0.299f,  0.0f,  0.0f },
            ///* IN Green */ { 0.587f,  0.587f,  0.587f,  0.0f,  0.0f },
            ///* IN Blue  */ { 0.114f,  0.114f,  0.114f,  0.0f,  0.0f },
            ///* IN Alpha */ { 0.0f,      0.0f,    0.0f,  1.0f,  0.0f },
            ///*          */ { 0.0f,      0.0f,    0.0f,  0.0f,  1.0f }
            //};

            //var matrix = new float[,] {
            ///*               OUT    OUT    OUT    OUT        */
            ///*               Red    Green  Blue   Alpha      */
            ///* IN Red   */ { 0.299f,  valu * 0.299f,  valu * 0.299f,  0.0f,  0.0f },
            ///* IN Green */ { 0.587f,  valu * 0.587f,  valu * 0.587f,  0.0f,  0.0f },
            ///* IN Blue  */ { 0.114f,  valu * 0.114f,  valu * 0.114f,  0.0f,  0.0f },
            ///* IN Alpha */ { 0.0f,             0.0f,           0.0f,  1.0f,  0.0f },
            ///*          */ { 0.0f,             0.0f,           0.0f,  0.0f,  1.0f }


            bool mono = chkMono.IsChecked == true;

            // calculate "faded down" values for red channel when NOT in mono mode.
            // this is so that it takes into account r/g/b not just red only
            float r2r = (valu * (1 - 0.299f)) + 0.299f; 
            float g2r = (1 - valu) * 0.587f;
            float b2r = (1 - valu) * 0.114f;

 

            var matrix = new float[,] {
            /*               OUT                     OUT                          OUT                          OUT        */
            /*               Red                     Green                        Blue                         Alpha      */
            /* IN Red   */ { (mono ? 0.299f : r2r),  valu * (mono ? 0.299f : 0),  valu * (mono ? 0.299f : 0),  0.0f,  0.0f },
            /* IN Green */ { (mono ? 0.587f : g2r),  valu * (mono ? 0.587f : 1),  valu * (mono ? 0.587f : 0),  0.0f,  0.0f },
            /* IN Blue  */ { (mono ? 0.114f : b2r),  valu * (mono ? 0.114f : 0),  valu * (mono ? 0.114f : 1),  0.0f,  0.0f },
            /* IN Alpha */ { 0.0f,                                         0.0f,                        0.0f,  1.0f,  0.0f },
            /*          */ { 0.0f,                                         0.0f,                        0.0f,  0.0f,  1.0f }
            };

            if (chkInvert.IsChecked == true)
            {
                for (int column = 0; column <= 2; column++)
                {
                    matrix[0, column] = 0 - matrix[0, column]; // invert the value, e.g. 0.27 becomes -0.27
                    matrix[1, column] = 0 - matrix[1, column];
                    matrix[2, column] = 0 - matrix[2, column];
                    matrix[4, column] = column == 0 ? 1 : valu; // add 1 to the result on red channel, valu on green/blue channels
                } 
            }

            textBox1.Text = Matrix2Text(matrix);
        }

        private void chkMono_Checked(object sender, RoutedEventArgs e)
        {
            slider1_ValueChanged(null, null);
        }

        private void chkInvert_Checked(object sender, RoutedEventArgs e)
        {
            slider1_ValueChanged(null, null);
        }

      

    }
}
