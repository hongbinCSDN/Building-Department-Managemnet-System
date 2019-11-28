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

namespace PScan
{
    /// <summary>
    /// WLogin.xaml 的互動邏輯
    /// </summary>
    public partial class WLogin : Window
    {
        public WLogin()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxUsername.Text == "")
            {
                MessageBox.Show("Please input Username.");
                TextBoxUsername.Focus();
                return;
            }
            if (PasswordBoxPassword.Password == "")
            {
                MessageBox.Show("Please input Password.");
                PasswordBoxPassword.Focus();
                return;
            }


           // MainService.Instance.Login(TextBoxUsername.Text, PasswordBoxPassword.Password);

            try
            {
                if (RadioButtonPortal.IsChecked != null && RadioButtonPortal.IsChecked.Value == true)
                {
                    MainService.Instance.LoginPortal(TextBoxUsername.Text, PasswordBoxPassword.Password);
                }
                else if (RadioButtonMWMS.IsChecked != null && RadioButtonMWMS.IsChecked.Value == true)
                {
                    MainService.Instance.Login(TextBoxUsername.Text, PasswordBoxPassword.Password);
                }
                if(MainService.Instance.Alive) { 
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Username or Password.");
            }


        }
    }
}
