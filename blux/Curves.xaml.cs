using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for Curves.xaml
    /// </summary>
    public partial class Curves : Window
    {
        WriteableBitmap wbmapR = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);
        WriteableBitmap wbmapG = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);
        WriteableBitmap wbmapB = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);
        int[] transformR = new int[256];
        int[] transformG = new int[256];
        int[] transformB = new int[256];

        public Curves()
        {
            InitializeComponent();


            foreach (var array in new[] { transformR, transformG, transformB })
                for (int i = 0; i < 256; i++)
                    array[i] = i;

            wbmapR.update(transformR, Colors.Red);
            wbmapG.update(transformR, Colors.Green);
            wbmapB.update(transformR, Colors.Blue);

            image1.Source = wbmapR;
            image2.Source = wbmapG;
            image3.Source = wbmapB;

            //for (int i = 0; i < 256; i++)
            //    wbmap.setPixel(i, 256 - transformArray[i], Colors.Red);
        }





        private void image1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int x = (int)e.GetPosition((IInputElement)sender).X;
                int y = (int)(255 - e.GetPosition((IInputElement)sender).Y);
                this.Title = string.Format("{0},{1}", x, y);

                var array =
                    sender == image1 ? transformR :
                    sender == image2 ? transformG :
                    transformB;

                var bmp =
                    sender == image1 ? wbmapR :
                    sender == image2 ? wbmapG :
                    wbmapB;

                var color =
                    sender == image1 ? Colors.Red :
                    sender == image2 ? Colors.Green :
                    Colors.Blue;

                array[x] = y;
                bmp.update(array, color);

                MainMain.CustomRamp(transformR, transformG, transformB);
            }
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            for (int i = 0; i < 256; i++)
                foreach (var transform in new[] { transformR, transformG, transformB })
                    transform[i] = Clamp(i +
                        (((i / 2) % 2 == 0) ? (int)slider1.Value : 0 - (int)slider1.Value));

            wbmapR.update(transformR, Colors.Red);
            wbmapG.update(transformG, Colors.Green);
            wbmapB.update(transformB, Colors.Blue);

            MainMain.CustomRamp(transformR, transformG, transformB);
        }

        private void silder3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int step = (int)slider3.Value;

            for (int i = 0; i < 256; i++)
                foreach (var transform in new[] { transformR, transformG, transformB })
                    transform[i] = Clamp(i +
                        (step - (i % step) - (i % step)));

            wbmapR.update(transformR, Colors.Red);
            wbmapG.update(transformG, Colors.Green);
            wbmapB.update(transformB, Colors.Blue);

            MainMain.CustomRamp(transformR, transformG, transformB);
        }

        private int Clamp(int val)
        {
            if (val < 0) return 0;
            if (val > 255) return 255;
            return val;
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Random r = new Random();
            int sli = (int)slider2.Value;

            for (int i = 0; i < 256; i++)
                foreach (var transform in new[] { transformR, transformG, transformB })
                    transform[i] = Clamp(i + (0 - (sli / 2)) + r.Next(sli));
            //int horiz = 5;
            //for (int i = 0; i < (256-horiz); i+=horiz)
            //    foreach (var transform in new[] { transformR, transformG, transformB })
            //    {
            //        var rnd = r.Next(sli);
            //        for (int h = 0; h < horiz;h++)
            //            transform[i+h] = Clamp(i + (0 - (sli / 2)) + rnd);
            //    }

            wbmapR.update(transformR, Colors.Red);
            wbmapG.update(transformG, Colors.Green);
            wbmapB.update(transformB, Colors.Blue);

            MainMain.CustomRamp(transformR, transformG, transformB);
        }

        private void sliderPosterise_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double posterise_multiplier = 255 / sliderPosterise.Value;

            for (int i = 0; i < 256; i++)
                foreach (var transform in new[] { transformR, transformG, transformB })
                    transform[i] = (int)(Convert.ToInt32(i / posterise_multiplier) * posterise_multiplier);

            wbmapR.update(transformR, Colors.Red);
            wbmapG.update(transformG, Colors.Green);
            wbmapB.update(transformB, Colors.Blue);

            MainMain.CustomRamp(transformR, transformG, transformB);
        }

        private void sliderPosterise2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int posterise_level = (int)sliderPosterise2.Value;
            if (posterise_level <= 1)
                throw new Exception("Posterise level 1 or below not allowed");
            double multiplier = 255.0 / (posterise_level - 1.0);

            foreach (var transform in new[] { transformR, transformG, transformB })
            {
                for (int i = 0; i < 256; i++)
                {
                    int index = (int)Math.Floor(i * (posterise_level / 256.0));
                    transform[i] = (int)Math.Round(index * multiplier);
                }
            }

            wbmapR.update(transformR, Colors.Red);
            wbmapG.update(transformG, Colors.Green);
            wbmapB.update(transformB, Colors.Blue);

            MainMain.CustomRamp(transformR, transformG, transformB);
        }
    }
}

public static class bitmapextensions
{


    public static void setPixel(
     this WriteableBitmap wbm,
        int x, int y, Color c)
    {
        if (y > wbm.PixelHeight - 1 ||
           x > wbm.PixelWidth - 1) return;
        if (y < 0 || x < 0) return;
        if (!wbm.Format.Equals(
               PixelFormats.Bgra32)) return;


        wbm.Lock();
        IntPtr buff = wbm.BackBuffer;
        int Stride = wbm.BackBufferStride;


        unsafe
        {
            byte* pbuff = (byte*)buff.ToPointer();
            int loc = y * Stride + x * 4;
            pbuff[loc] = c.B;
            pbuff[loc + 1] = c.G;
            pbuff[loc + 2] = c.R;
            pbuff[loc + 3] = c.A;
        }


        wbm.AddDirtyRect(
               new Int32Rect(x, y, 1, 1));
        wbm.Unlock();
    }

    public static void update(this WriteableBitmap wbm, int[] transformArray, Color backgroundColor)
    {
        if (!wbm.Format.Equals(PixelFormats.Bgra32)) return;


        wbm.Lock();
        IntPtr buff = wbm.BackBuffer;
        int Stride = wbm.BackBufferStride;


        unsafe
        {
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    Color c = transformArray[x] == (255 - y) ? Colors.Black : backgroundColor;
                    byte* pbuff = (byte*)buff.ToPointer();
                    int loc = y * Stride + x * 4;
                    pbuff[loc] = c.B;
                    pbuff[loc + 1] = c.G;
                    pbuff[loc + 2] = c.R;
                    pbuff[loc + 3] = c.A;
                }
            }
        }


        wbm.AddDirtyRect(new Int32Rect(0, 0, 255, 255));
        wbm.Unlock();
    }
}

