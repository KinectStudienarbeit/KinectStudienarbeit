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

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Interaktionslogik für ChooseModus.xaml
    /// </summary>
    public partial class ChooseModus : Window
    {
        private bool absolute;

        public ChooseModus(MainWindow mainWindow)
        {
            InitializeComponent();
            this.Left = mainWindow.Left + mainWindow.Width / 2 - this.Width / 2;
            this.Top = mainWindow.Top + mainWindow.Height / 2 - this.Height / 2;
        }

        public bool show()
        {
            ShowDialog();
            return absolute ;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            absolute = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            absolute = false;
            this.Close();
        }
    }
}
