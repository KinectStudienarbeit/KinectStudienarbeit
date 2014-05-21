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
using System.Timers;
using System.Diagnostics;
using System.Windows.Threading;

namespace KinectStudienarbeitWpf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SolidColorBrush brushRed = new SolidColorBrush(Colors.Red);
        SolidColorBrush brushGreen = new SolidColorBrush(Colors.Green);
        SolidColorBrush brushBlue = new SolidColorBrush(Colors.Blue);
        Room mainRoom;

        BlenderResourceDictionary mainDictionary = null;
        BlenderModel mainModel = null;
        KinectSensor mainKinect = null;
        DispatcherTimer time = null;
        int secs = 0;

        public void displayMessage(String message)
        {
            Label_Start.Content = message;
            Label_Start.Visibility = System.Windows.Visibility.Visible;
        }

        public void displayColorFrame(BitmapSource bitmapSource)
        {
            ImageColorStream.Source = bitmapSource;
        }

        public void setHandMarkers(int rx, int ry, int lx, int ly, bool turn)
        {
            if (turn)
            {
                Ellipse_LeftHand.Fill = brushRed;
                Ellipse_RightHand.Fill = brushRed;
            }
            else
            {
                Ellipse_LeftHand.Fill = brushBlue;
                Ellipse_RightHand.Fill = brushGreen;
            }
            Canvas.SetLeft(Ellipse_RightHand, rx * ImageColorStream.Width / 640);
            Canvas.SetTop(Ellipse_RightHand, ry * ImageColorStream.Height / 480);
            Canvas.SetLeft(Ellipse_LeftHand, lx * ImageColorStream.Width / 640);
            Canvas.SetTop(Ellipse_LeftHand, ly * ImageColorStream.Height / 480);
        }

        public void displayKinectStatus(String status)
        {
            Label_KinectStatus.Content = status;
        }

        public MainWindow()
        {
            InitializeComponent();
            Label_Start.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            mainRoom = new Room(this);
            KinectHandler k = new KinectHandler(this, mainRoom);

            this.Left = 5;
            this.Top = 5;

            //initializeKinect();
            //Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            //mainDictionary = new BlenderResourceDictionary(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"\Spielraumblend4.xaml");

            //BlenderModel raum = new BlenderModel(mainDictionary, "Raum");
            //raum.addToViewport(this.mainViewPort);
            //raum.rotate(-90, 0, 0, false);
            //raum.rotate(0, -90, 0, false);
            //raum.translate(0, -0.5, -8, false);
            //raum.scale(0.05, 0.52, -0.5);
            //raum.translate(0, 0, 7, false);

            //BlenderModel wand = new BlenderModel(mainDictionary, "Wand");
            //wand.addToViewport(this.mainViewPort);
            //wand.rotate(0, -90, 0, false);
            //wand.rotate(0, 0, -90, false);
            //wand.scale(-0.8, -0.8, -0.8);
            //wand.translate(1.15, -0.6, 2.4, false);

            //mainModel = new BlenderModel(mainDictionary, "Quader");
            //mainModel.addToViewport(this.mainViewPort);
            //mainModel.scale(-0.8, -0.8, -0.8);
            //mainModel.translate(0, 0, 2.86, false);



        }

        /// <summary>
        /// The tick event for the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void time_Tick(object sender, EventArgs e)
        {
            secs++;
        }

        /// <summary>
        /// Deals with the keyboard input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (mainModel == null)
            {
                mainModel = mainRoom.currentElement.model;
                
            }
            mainRoom.currentElement.checkCoords();
            switch (e.Key)
            {
                case Key.Y:
                    mainModel.scale(0, 0, 0.5);
                    break;
                case Key.X:
                    mainModel.scale(0, 0, -0.5);
                    break;
            }

            if (mainModel == null) return;

            if (Radio_Rotate.IsChecked.Value)
            {
                switch (e.Key)
                {
                    case Key.W:
                        mainModel.rotate(-5, 0, 0);
                        break;

                    case Key.S:
                        mainModel.rotate(5, 0, 0);
                        break;

                    case Key.D:
                        mainModel.rotate(0, 5, 0);
                        break;

                    case Key.A:
                        mainModel.rotate(0, -5, 0);
                        break;

                    case Key.E:
                        mainModel.rotate(0, 0, -5);
                        break;

                    case Key.Q:
                        mainModel.rotate(0, 0, 5);
                        break;
                }
            }
            else if (Radio_Translate.IsChecked.Value)
            {
                switch (e.Key)
                {
                    case Key.W:
                        mainModel.translate(0, 0.01, 0, true);
                        break;

                    case Key.S:
                        mainModel.translate(0, -0.01, 0, true);
                        break;

                    case Key.D:
                        mainModel.translate(0.01, 0, 0, true);
                        break;

                    case Key.A:
                        mainModel.translate(-0.01, 0, 0, true);
                        break;

                    case Key.E:
                        mainModel.translate(0, 0, -0.01, true);
                        break;

                    case Key.Q:
                        mainModel.translate(0, 0, 0.01, true);
                        break;
                }
            }
        }

        /// <summary>
        /// Logic of the test GUI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XAML files (*.xaml)|*.xaml";
            openFileDialog.ShowDialog();
            Textbox_File.Text = openFileDialog.FileName;
        }

        /// <summary>
        /// Logic of the test GUI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(Textbox_File.Text))
            {
                MessageBox.Show("Could not find file " + Textbox_File.Text + "!");
            }
            else
            {
                mainDictionary = new BlenderResourceDictionary(Textbox_File.Text);
                ComboBox_Models.Items.Clear();
                foreach (String s in mainDictionary.keyList)
                {
                    ComboBox_Models.Items.Add(s);
                }
                ComboBox_Models.SelectedItem = ComboBox_Models.Items[0];
                MessageBox.Show("File loaded successfully!");
            }

        }

        /// <summary>
        /// Logic of the test GUI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_Models_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainModel != null)
            {
                mainModel.removeFromViewport(this.mainViewPort);
            }

            mainModel = new BlenderModel(mainDictionary, ComboBox_Models.SelectedItem as String);
            mainModel.addToViewport(this.mainViewPort);
        }

        /// <summary>
        /// Logic of the test GUI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Radio_Kinect_Checked(object sender, RoutedEventArgs e)
        {
            //probably to be removed
        }

    }
}
