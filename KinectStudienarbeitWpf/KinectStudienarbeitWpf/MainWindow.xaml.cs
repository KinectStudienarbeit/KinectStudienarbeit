using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Kinect;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BlenderResourceDictionary mainDictionary = null;
        BlenderModel mainModel = null;
        KinectSensor mainKinect = null;
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initializeKinect();
        }

        private void initializeKinect()
        {
            mainKinect = KinectSensor.KinectSensors.FirstOrDefault();
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            Label_KinectStatus.Content = mainKinect.Status;

        }

        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == KinectStatus.Connected)
            {
                mainKinect = e.Sensor;
            }
            Label_KinectStatus.Content = e.Status;
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (mainModel == null) return;

            if (Radio_Rotate.IsChecked.Value)
            {
                switch (e.Key)
                {
                    case Key.W:
                        mainModel.rotate(-20, 0, 0);
                        break;

                    case Key.S:
                        mainModel.rotate(20, 0, 0);
                        break;

                    case Key.D:
                        mainModel.rotate(0, 20, 0);
                        break;

                    case Key.A:
                        mainModel.rotate(0, -20, 0);
                        break;

                    case Key.E:
                        mainModel.rotate(0, 0, -20);
                        break;

                    case Key.Q:
                        mainModel.rotate(0, 0, 20);
                        break;
                }
            }
            else if (Radio_Translate.IsChecked.Value)
            {
                switch (e.Key)
                {
                    case Key.W:
                        mainModel.translate(0, 0.5, 0);
                        break;

                    case Key.S:
                        mainModel.translate(0, -0.5, 0);
                        break;

                    case Key.D:
                        mainModel.translate(0.5, 0, 0);
                        break;

                    case Key.A:
                        mainModel.translate(-0.5, 0, 0);
                        break;

                    case Key.E:
                        mainModel.translate(0, 0, -0.5);
                        break;

                    case Key.Q:
                        mainModel.translate(0, 0, 0.5);
                        break;
                }
            }
        }

        private void Button_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XAML files (*.xaml)|*.xaml";
            openFileDialog.ShowDialog();
            Textbox_File.Text = openFileDialog.FileName;
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Textbox_File.Text))
            {
                MessageBox.Show("Could not find file " + Textbox_File.Text + "!");
            }
            else
            {
                mainDictionary = new BlenderResourceDictionary(Textbox_File.Text);
                foreach (String s in mainDictionary.keyList)
                {
                    ComboBox_Models.Items.Add(s);
                }
                ComboBox_Models.SelectedItem = ComboBox_Models.Items[0];
                MessageBox.Show("File loaded successfully!");
            }

        }

        private void ComboBox_Models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainModel != null)
            {
                mainModel.removeFromViewport(this.mainViewPort);
            }

            mainModel = new BlenderModel(mainDictionary, ComboBox_Models.SelectedItem as String);
            mainModel.addToViewPort(this.mainViewPort);
        }

    }
}
