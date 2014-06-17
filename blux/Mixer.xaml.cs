using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace blux
{
    /// <summary>
    /// Interaction logic for Mixer.xaml
    /// </summary>
    public partial class Mixer : Window
    {
        TextBox m_textbox;

        public Mixer(TextBox tb)
        {
            m_textbox = tb;
            InitializeComponent();
            refresh();
        }

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

            double val0 = ((255 - rleft) * (255 - rtop)) / 65025; // amount of red going to the red channel
            double val1 = ((255 - gleft) * (255 - gtop)) / 65025; // amount of red going to the green channel
            double val2 = ((255 - bleft) * (255 - btop)) / 65025; // amount of red going to the blue channel

            double val3 = ((rleft) * (255 - rtop)) / 65025; // amount of green going to the red channel
            double val4 = ((gleft) * (255 - gtop)) / 65025; // amount of green going to the green channel
            double val5 = ((bleft) * (255 - btop)) / 65025; // amount of green going to the blue channel

            double val6 = ((255 - rleft) * (rtop)) / 65025; // amount of blue going to the red channel
            double val7 = ((255 - gleft) * (gtop)) / 65025; // amount of blue going to the green channel
            double val8 = ((255 - bleft) * (btop)) / 65025; // amount of blue going to the blue channel

            m_textbox.Text = string.Format(
                "\t{0}\t{1}\t{2}\t0\t0" + 
                "\t{3}\t{4}\t{5}\t0\t0" + 
                "\t{6}\t{7}\t{8}\t0\t0" +
                "\t0\t0\t0\t1\t0" + 
                "\t0\t0\t0\t0\t1",
                Math.Round(val0, 2),
                Math.Round(val1, 2),
                Math.Round(val2, 2),
                Math.Round(val3, 2),
                Math.Round(val4, 2),
                Math.Round(val5, 2),
                Math.Round(val6, 2),
                Math.Round(val7, 2),
                Math.Round(val8, 2)
            );


        }



    }
}
