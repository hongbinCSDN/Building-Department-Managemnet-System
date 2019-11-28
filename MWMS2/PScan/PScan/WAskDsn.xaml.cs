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
    /// Interaction logic for WAskDsn.xaml
    /// </summary>
    public partial class WAskDsn : Window
    {
        public WAskDsn()
        {
            InitializeComponent();
        }
        public string DSN
        {
            get { return TextBoxDsn.Text; }
            set { TextBoxDsn.Text = value; }
        }
        public P_MW_DSN MwDsn
        {
            get;set;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonGen.IsEnabled == false)
            {
                P_MW_DSN mwDsn = MainService.Instance.LoadDocInfo(TextBoxDsn.Text);
            }
            DialogResult = true;
        }

        private void ButtonGen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                P_MW_DSN MwDsnFen = MainService.Instance.GenerateNewDsn();
                if (MwDsnFen != null && !string.IsNullOrWhiteSpace(MwDsnFen.DSN))
                {
                    TextBoxDsn.Text = MwDsnFen.DSN;
                    TextBoxDsn.IsEnabled = false;
                    ButtonGen.IsEnabled = false;
                    MwDsn = MwDsnFen;
                }
            }
            catch { }
        }
    }
}
