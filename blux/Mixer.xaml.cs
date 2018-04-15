using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace blux
{
    public partial class Mixer : Window
    {

        public Mixer()
        {
            InitializeComponent();

            MagInitialize();

            refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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





        private void myThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var t = (Thumb)sender;

            var newleft = Canvas.GetLeft(t) + e.HorizontalChange;
            if (newleft >= 0 && newleft <= 255)
                Canvas.SetLeft(t, newleft);

            var newtop = Canvas.GetTop(t) + e.VerticalChange;
            if (newtop >= 0 && newtop <= 255)
                Canvas.SetTop(t, newtop);

            refresh();
        }


        void refresh()
        {
            var rleft = Canvas.GetLeft(myThumbR);
            var rtop = Canvas.GetTop(myThumbR);
            var gleft = Canvas.GetLeft(myThumbG);
            var gtop = Canvas.GetTop(myThumbG);
            var bleft = Canvas.GetLeft(myThumbB);
            var btop = Canvas.GetTop(myThumbB);

            float val0 = (float)(((255 - rleft) * (255 - rtop)) / 65025); // amount of red going to the red channel
            float val1 = (float)(((255 - gleft) * (255 - gtop)) / 65025); // amount of red going to the green channel
            float val2 = (float)(((255 - bleft) * (255 - btop)) / 65025); // amount of red going to the blue channel

            float val3 = (float)(((rleft) * (255 - rtop)) / 65025); // amount of green going to the red channel
            float val4 = (float)(((gleft) * (255 - gtop)) / 65025); // amount of green going to the green channel
            float val5 = (float)(((bleft) * (255 - btop)) / 65025); // amount of green going to the blue channel
  
            float val6 = (float)(((255 - rleft) * (rtop)) / 65025); // amount of blue going to the red channel
            float val7 = (float)(((255 - gleft) * (gtop)) / 65025); // amount of blue going to the green channel
            float val8 = (float)(((255 - bleft) * (btop)) / 65025); // amount of blue going to the blue channel


            var matrix = new float[,] {
            /*               OUT    OUT    OUT    OUT        */
            /*               Red    Green  Blue   Alpha      */
            /* IN Red   */ { val0,  val1,  val2,  0.0f,  0.0f },
            /* IN Green */ { val3,  val4,  val5,  0.0f,  0.0f },
            /* IN Blue  */ { val6,  val7,  val8,  0.0f,  0.0f },
            /* IN Alpha */ { 0.0f,  0.0f,  0.0f,  1.0f,  0.0f },
            /*          */ { 0.0f,  0.0f,  0.0f,  0.0f,  1.0f }
            };
            
            MagSetFullscreenColorEffect(matrix);
        }

     



    }
}
