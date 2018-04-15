using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace blux
{
    public static class bitmapextensions
    {
        public static void setPixel(this WriteableBitmap wbm, int x, int y, Color c)
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
}