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
            if (txtPassword.Password == DateTime.Now.ToString("ddMMHHmm")) // password always changes so it's not in muscle memory
            { 
                this.DialogResult = true;
            }
            else
            {
                lblIncorrectPassword.Visibility = Visibility.Visible;

                lblIncorrectPassword.Visibility = Visibility.Visible;
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += HideMessage;
                timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
                timer.Start();
            }
        }

        private void HideMessage(object sender, EventArgs e)
        {
            lblIncorrectPassword.Visibility = Visibility.Hidden;
            ((DispatcherTimer)sender).Stop();
        }

      

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblHelpText.Visibility = txtPassword.SecurePassword.Length > 0 ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
