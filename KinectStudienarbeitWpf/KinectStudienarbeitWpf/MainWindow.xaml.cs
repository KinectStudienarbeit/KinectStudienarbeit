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
        KinectHandler mainKinectHandler = null;
        DispatcherTimer time = null;
        int secs = 0;
        int round = 0;
        int[] timeCount = new int[4];
        String textBoxString = "Round 1\nReady?\n";
        String levelLabelString = "Level: easy";

        public MainWindow()
        {
            this.Left = 5;
            this.Top = 5;
            InitializeComponent();
            TextBlock_Message.Visibility = System.Windows.Visibility.Hidden;
            mainViewPort.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainRoom = new Room(this);
            mainKinectHandler = new KinectHandler(this, mainRoom);
            mainKinectHandler.absolute = new ChooseModus(this).show();
            MessageBoxResult mbr;
            mbr = MessageBox.Show("Press OK to start", "", MessageBoxButton.OK);
            if (mbr == MessageBoxResult.OK) mainKinectHandler.tutorial = true;
            mainViewPort.Visibility = System.Windows.Visibility.Visible;
            secs = 5;
            time = new DispatcherTimer();
            time.Interval = new TimeSpan(0, 0, 1);
            time.Tick += time_Tick_Countdown;
            TextBlock_Message.Text = textBoxString + secs.ToString();
            TextBlock_Message.Visibility = System.Windows.Visibility.Visible;
            time.Start();
        }

        public void displayMessage(String message)
        {
            TextBlock_Message.Text = message;
            TextBlock_Message.Visibility = System.Windows.Visibility.Visible;
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

        void startGame()
        {
            mainRoom.chooseNewElement();
            TextBlock_Message.Visibility = System.Windows.Visibility.Hidden;
            time = new DispatcherTimer();
            time.Interval = new TimeSpan(0, 0, 1);
            time.Tick += time_Tick;
            secs = 0;
            Label_Level.Content = levelLabelString;
            Label_Time.Content = "Time: " + secs.ToString() + "s";
            time.Start();
            mainKinectHandler.playing = true;
        }

        public void startNextRound()
        {
            time.Stop();
            mainKinectHandler.playing = false;
            timeCount[round] = secs;
            secs = 10;
            round++;
            switch (round)
            {
                case 1:
                    mainRoom.difficult = true;
                    mainKinectHandler.rotation = true;
                    textBoxString = "Time: " + timeCount[0].ToString() + "s\nRound 2\nReady?\n";
                    TextBlock_Message.Text = textBoxString + secs;
                    TextBlock_Message.Visibility = System.Windows.Visibility.Visible;
                    time = new DispatcherTimer();
                    time.Interval = new TimeSpan(0, 0, 1);
                    time.Tick += time_Tick_Countdown;
                    time.Start();
                    break;
                case 2:
                    mainKinectHandler.changeModus();
                    mainRoom.difficult = false;
                    mainKinectHandler.rotation = false;
                    textBoxString = "Time: " + timeCount[1].ToString() + "s\nModus Change! Round 1\nReady?\n";
                    TextBlock_Message.Text = textBoxString + secs;
                    TextBlock_Message.Visibility = System.Windows.Visibility.Visible;
                    time = new DispatcherTimer();
                    time.Interval = new TimeSpan(0, 0, 1);
                    time.Tick += time_Tick_Countdown;
                    time.Start();
                    break;
                case 3:
                    mainRoom.difficult = true;
                    mainKinectHandler.rotation = true;
                    textBoxString = "Time: " + timeCount[2].ToString() + "s\nRound 2\nReady?\n";
                    TextBlock_Message.Text = textBoxString + secs;
                    TextBlock_Message.Visibility = System.Windows.Visibility.Visible;
                    time = new DispatcherTimer();
                    time.Interval = new TimeSpan(0, 0, 1);
                    time.Tick += time_Tick_Countdown;
                    time.Start();
                    break;
                default:
                    TextBlock_Message.Text = "Thank you for playing!\nR1: " + timeCount[0].ToString() + "s   R2: " + timeCount[1].ToString() + "s\n\nR1: " + timeCount[2].ToString() + "s   R2: " + timeCount[3].ToString() + "s";
                    TextBlock_Message.Visibility = System.Windows.Visibility.Visible;
                    break;

            }

        }

        private void time_Tick_Countdown(object sender, EventArgs e)
        {
            if (secs > 0)
            {
                secs--;
                TextBlock_Message.Text = textBoxString + secs.ToString();
            }
            else
            {
                time.Stop();
                startGame();
            }
        }

        /// <summary>
        /// The tick event for the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void time_Tick(object sender, EventArgs e)
        {
            secs++;
            Label_Time.Content = "Time: " + secs.ToString() + "s";
        }

        public void displayElement(int number)
        {
            Label_Element.Content = "Element: " + number.ToString() + "/6";
        }
    }
}
