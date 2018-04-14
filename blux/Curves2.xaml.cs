﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Curves2 : Window
    {


        public class MethodDetails
        {
            public string Name { get; set; }
            public Action Function;
            public int slider1Min;
            public int slider1Max;
            public int slider2Min;
            public int slider2Max;
            public int slider3Min;
            public int slider3Max;
        }

        public BindingList<MethodDetails> _methods;

        WriteableBitmap wbmapR = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);
        WriteableBitmap wbmapG = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);
        WriteableBitmap wbmapB = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);
        int[] transformR = new int[256];
        int[] transformG = new int[256];
        int[] transformB = new int[256];

        public Curves2()
        {
            InitializeComponent();

            _methods = new BindingList<MethodDetails>()
        {
            new MethodDetails() { Name = "Linear", Function = linear },
            new MethodDetails() { Name = "Noise1", Function = noise1, slider1Min = 0, slider1Max = 25 },
            new MethodDetails() { Name = "Noise2", Function = noise2, slider1Min = 0, slider1Max = 50 },
            new MethodDetails() { Name = "Noise3", Function = noise3, slider1Min = 1, slider1Max = 255 },
            new MethodDetails() { Name = "Posterise1", Function = posterise1, slider1Min = 1, slider1Max = 10 },
            new MethodDetails() { Name = "Posterise2", Function = posterise2, slider1Min = 2, slider1Max = 10 },
            new MethodDetails() { Name = "Posterise_1BitCustom", Function = posterise_1bit_custom, slider1Min = 100, slider1Max = 255 },
            new MethodDetails() { Name = "Posterise_2BitCustom", Function = posterise_2bit_custom, slider1Min = 50, slider1Max = 200, slider2Min = 150, slider2Max = 250 },
            new MethodDetails() { Name = "Posterise_3BitCustom", Function = posterise_3bit_custom, slider1Min = 50, slider1Max = 200, slider2Min = 150, slider2Max = 250, slider3Min = 150, slider3Max = 250 }
        };
            cboMethod.DisplayMemberPath = "Name";
            cboMethod.ItemsSource = _methods;

            apply();

            image1.Source = wbmapR;
            image2.Source = wbmapG;
            image3.Source = wbmapB;
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

        private int Clamp(int val)
        {
            if (val < 0) return 0;
            if (val > 255) return 255;
            return val;
        }

        private void showHideSlider(int num, int min, int max)
        {
            var visibility = min == 0 && max == 0 ? Visibility.Hidden : Visibility.Visible; 

            var slider = (Slider)this.FindName("slider" + num);
            slider.Visibility = visibility;
            if (slider.Value < min) slider.Value = min;
            if (slider.Value > max) slider.Value = max;
            slider.Minimum = min;
            slider.Maximum = max;

            var label = (Label)this.FindName("label" + num);
            label.Visibility = visibility;

            var number = (TextBlock)this.FindName("number" + num);
            number.Visibility = visibility;
        }

        private void cboMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var details = (MethodDetails)cboMethod.SelectedItem;

            showHideSlider(1, details.slider1Min, details.slider1Max);
            showHideSlider(2, details.slider2Min, details.slider2Max);
            showHideSlider(3, details.slider3Min, details.slider3Max);

            apply();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            apply();
        }

        private void apply()
        {
            var details = (MethodDetails)cboMethod.SelectedItem;
            if (details == null) return; // nothing selected
            details.Function();
           
            wbmapR.update(transformR, Colors.Red);
            wbmapG.update(transformG, Colors.Green);
            wbmapB.update(transformB, Colors.Blue);

            MainMain.CustomRamp(transformR, transformG, transformB);
        }

        private void linear()
        {
            foreach (var array in new[] { transformR, transformG, transformB })
                for (int i = 0; i < 256; i++)
                    array[i] = i;
        }

        private void noise1()
        {
            for (int i = 0; i < 256; i++)
                foreach (var transform in new[] { transformR, transformG, transformB })
                    transform[i] = Clamp(i +
                        (((i / 2) % 2 == 0) ? (int)slider1.Value : 0 - (int)slider1.Value));
        }

        private void noise2()
        {
            Random r = new Random();

            for (int i = 0; i < 256; i++)
                foreach (var transform in new[] { transformR, transformG, transformB })
                    transform[i] = Clamp(i + (0 - ((int)slider1.Value / 2)) + r.Next((int)slider1.Value));
        }

        private void noise3()
        {
            int step =(int)slider1.Value;
            for (int i = 0; i < 256; i++)
                foreach (var transform in new[] { transformR, transformG, transformB })
                    transform[i] = Clamp(i +
                        (step - (i % step) - (i % step)));
        }

        private void posterise1()
        {
            double posterise_multiplier = 255.0 / (int)slider1.Value;

            for (int i = 0; i < 256; i++)
                foreach (var transform in new[] { transformR, transformG, transformB })
                    transform[i] = (int)(Convert.ToInt32(i / posterise_multiplier) * posterise_multiplier);
        }

        private void posterise2()
        {
            int posterise_level = (int)slider1.Value;

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
        }


        private void posterise_1bit_custom()
        {
            int threshold = (int)slider1.Value;

            foreach (var transform in new[] { transformR, transformG, transformB })
            {
                for (int i = 0; i < 256; i++)
                {
                    transform[i] = i >= threshold ? 255 : 0;
                }
            }
        }

        private void posterise_2bit_custom()
        {
            int threshold1 = (int)slider1.Value;
            int threshold2 = (int)slider2.Value;

            foreach (var transform in new[] { transformR, transformG, transformB })
            {
                for (int i = 0; i < 256; i++)
                {
                    transform[i] = i >= threshold2 ? 255
                        : i >= threshold1 ? 128
                        : 0;
                }
            }
        }

        private void posterise_3bit_custom()
        {
            int threshold1 = (int)slider1.Value;
            int threshold2 = (int)slider2.Value;
            int threshold3 = (int)slider3.Value;

            foreach (var transform in new[] { transformR, transformG, transformB })
            {
                for (int i = 0; i < 256; i++)
                {
                    transform[i] = i >= threshold3 ? 255
                        : i >= threshold2 ? 170
                        : i >= threshold1 ? 85
                        : 0;
                }
            }
        }
    }
}

