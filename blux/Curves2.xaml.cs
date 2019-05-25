using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace blux
{
    public partial class Curves2 : Window
    {
        WriteableBitmap wbmapR = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);
        WriteableBitmap wbmapG = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);
        WriteableBitmap wbmapB = new WriteableBitmap(255, 255, 96, 96, PixelFormats.Bgra32, null);

        int[] _transformR;
        int[] _transformG;
        int[] _transformB;

        Posterise _posterise;
       
        public Curves2()
        {
            InitializeComponent();

            _posterise = new Posterise();
            
            cboMethod.ItemsSource = _posterise.GetMethodNames();

            cboMethod_SelectionChanged(null, null);

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
                    sender == image1 ? _transformR :
                    sender == image2 ? _transformG :
                    _transformB;

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

                MainMain.CustomRamp(_transformR, _transformG, _transformB);
            }
        }

        

        
        private void cboMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string methodName = (string)cboMethod.SelectedItem;
            if (methodName == null) return; // nothing selected

            var details = _posterise.GetMethodDetails(methodName);

            showHideSlider(1, details.slider1Min, details.slider1Max, details.slider1Default);
            showHideSlider(2, details.slider2Min, details.slider2Max, details.slider2Default);
            showHideSlider(3, details.slider3Min, details.slider3Max, details.slider3Default);
            showHideSlider(4, details.slider4Min, details.slider4Max, details.slider4Default);

            apply();
        }

        private void showHideSlider(int num, int min, int max, int defaultValue)
        {
            var visibility = min == 0 && max == 0 ? Visibility.Hidden : Visibility.Visible;

            var slider = (Slider)this.FindName("slider" + num);
            slider.Visibility = visibility;
            slider.Minimum = min;
            slider.Maximum = max;
            if (defaultValue != 0)
                slider.Value = defaultValue;
            //if (slider.Value < min) slider.Value = min;
            //if (slider.Value > max) slider.Value = max;


            var label = (Label)this.FindName("label" + num);
            label.Visibility = visibility;

            var number = (TextBlock)this.FindName("number" + num);
            number.Visibility = visibility;
        }


        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            apply();
        }

        private void apply()
        {
            string methodName = (string)cboMethod.SelectedItem;
            if (methodName == null) return; // nothing selected

            int[] sliderValues = new[] { (int)slider1.Value, (int)slider2.Value, (int)slider3.Value, (int)slider4.Value };

            var values = _posterise.ApplyMethod(methodName, sliderValues);
            _transformR = values.Red;
            _transformG = values.Green;
            _transformB = values.Blue;

            wbmapR.update(_transformR, Colors.Red);
            wbmapG.update(_transformG, Colors.Green);
            wbmapB.update(_transformB, Colors.Blue);

            MainMain.CustomRamp(_transformR, _transformG, _transformB);
        }

       
       
    }
}

