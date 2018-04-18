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
using System.Windows.Threading;

namespace blux
{
    public partial class CbaPassword : Window
    {
        string _Password = "compl1cated";

        public CbaPassword()
        {
            InitializeComponent();
            txtPassword.Focus();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password == _Password)
            {
                this.DialogResult = true;
            }
            else
            {
                lblIncorrectPassword.Visibility = Visibility.Visible;

                lblIncorrectPassword.Visibility = Visibility.Visible;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += Timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lblIncorrectPassword.Visibility = Visibility.Hidden;
            ((DispatcherTimer)sender).Stop();
        }
    }              
}
